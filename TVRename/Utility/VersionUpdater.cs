//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TVRename;

public static class VersionUpdater
{
    private const string GITHUB_RELEASES_API_URL = "https://api.github.com/repos/TV-Rename/tvrename/releases";
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static async Task<ServerRelease?> CheckForUpdatesAsync()
    {
        Release? currentVersion = null;

        try
        {
            currentVersion = ObtainCurrentVersion();
        }
        catch (ArgumentException e)
        {
            Logger.Error(e, "Failed to establish if there are any new releases as could not parse internal version: " + Helpers.DisplayVersion);
        }

        (ServerRelease? latestVersion, ServerRelease? latestBetaVersion) = await GetLatestReleasesAsync().ConfigureAwait(false);

        bool production = TVSettings.Instance.mode == TVSettings.BetaMode.ProductionOnly;

        if (production)
        {
            if (currentVersion is null)
            {
                Logger.Warn("Internal version is missing, assuming we need to upgrade.");
                return latestVersion ?? latestBetaVersion;
            }
            if (currentVersion.SameVersionAs(latestVersion))
            {
                return null;
            }
            if (latestVersion is null)
            {
                Logger.Warn("Cannot obtain the latest version from GitHub");
                return null;
            }

            if (latestVersion.NewerThan(currentVersion))
            {
                return latestVersion;
            }
        }

        if (!production)
        {
            if (currentVersion is null)
            {
                Logger.Warn("Internal version is missing, assuming we need to upgrade to latest beta.");
                return latestBetaVersion ?? latestVersion;
            }
            if (currentVersion.SameVersionAs(latestBetaVersion))
            {
                return null;
            }
            if (latestBetaVersion is null)
            {
                Logger.Warn("Cannot obtain the latest beta version from GitHub");
                return null;
            }
            if (latestBetaVersion.NewerThan(currentVersion))
            {
                return latestBetaVersion;
            }
        }
        return null;
    }

    private static async Task<(ServerRelease? latestVersion, ServerRelease? latestBetaVersion)> GetLatestReleasesAsync()
    {
        ServerRelease? latestVersion = null;
        ServerRelease? latestBetaVersion = null;

        try
        {
            JArray? gitHubInfo = null;

            await HttpHelper.RetryOnExceptionAsync<Exception>
            (3, 2.Seconds(), GITHUB_RELEASES_API_URL, async () =>
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("user-agent", TVSettings.USER_AGENT);
                Task<string> response = client.GetStringAsync(GITHUB_RELEASES_API_URL);
                gitHubInfo = JArray.Parse(await response.ConfigureAwait(false));
            }).ConfigureAwait(false);

            if (gitHubInfo is null)
            {
                Logger.Error("Failed to contact GitHub to identify new releases - no exception raised");
                return (null, null);
            }

            foreach (JObject gitHubReleaseJson in gitHubInfo.Children<JObject>())
            {
                try
                {
                    JToken? jToken = gitHubReleaseJson["assets"];
                    if (jToken is null)
                    {
                        continue; //we have no files for this release, so ignore
                    }

                    ServerRelease? testVersion = ParseFromJson(gitHubReleaseJson);
                    if (testVersion is null)
                    {
                        Logger.Error($"Could not parse {gitHubReleaseJson}");
                        continue;
                    }

                    (latestBetaVersion, latestVersion) = UpdateLatest(testVersion, latestBetaVersion, latestVersion);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Logger.Debug("Generally happens because the release did not have an exe attached");
                    Logger.Debug(ex, gitHubReleaseJson.ToString());
                }
            }

            if (latestVersion is null)
            {
                Logger.Error("Could not find latest version information from GitHub: " + gitHubInfo);
                return (null, latestBetaVersion);
            }

            if (latestBetaVersion is null)
            {
                Logger.Error("Could not find latest beta version information from GitHub: " + gitHubInfo);
                return (latestVersion, null);
            }
        }
        catch (WebException wex)
        {
            Logger.LogWebException("Failed to contact GitHub to identify new releases", wex);
        }
        catch (HttpRequestException wex)
        {
            Logger.LogHttpRequestException("Failed to contact GitHub to identify new releases", wex);
        }
        catch (AggregateException ex) when (ex.InnerException is HttpRequestException wex)
        {
            Logger.LogHttpRequestException("Failed to contact GitHub to identify new releases", wex);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to contact GitHub to identify new releases");
        }

        return (latestVersion, latestBetaVersion);
    }

    private static (ServerRelease? latestBetaVersion, ServerRelease? latestVersion) UpdateLatest(ServerRelease testVersion,
        ServerRelease? latestBetaVersion, ServerRelease? latestVersion)
    {
        //all versions want to be considered if you are in the beta stream
        if (testVersion.NewerThan(latestBetaVersion))
        {
            latestBetaVersion = testVersion;
        }

        //If the latest version is a production one then update the latest production version
        if (!testVersion.IsBeta)
        {
            if (testVersion.NewerThan(latestVersion))
            {
                latestVersion = testVersion;
            }
        }

        return (latestBetaVersion, latestVersion);
    }

    private static ServerRelease? ParseFromJson(JObject gitHubReleaseJson)
    {
        DateTime releaseDate = DateTime.TryParse(gitHubReleaseJson["published_at"]?.ToString(), out DateTime rd)
            ? rd
            : DateTime.MinValue;

        string? url = (string?)gitHubReleaseJson["assets"]?[0]?["browser_download_url"];
        string? releaseNotesText = gitHubReleaseJson["body"]?.ToString();
        string? releaseNotesUrl = gitHubReleaseJson["html_url"]?.ToString();
        string? version = gitHubReleaseJson["tag_name"]?.ToString();

        if (url is null || releaseNotesText is null || releaseNotesUrl is null || version is null)
        {
            return null;
        }

        return new ServerRelease(
            version,
            Release.VersionType.semantic,
            url,
            releaseNotesText,
            releaseNotesUrl,
            gitHubReleaseJson["prerelease"]?.ToString() == "True",
            releaseDate);
    }

    private static Release ObtainCurrentVersion() => new(Helpers.Version, Release.VersionType.friendly);
}
