// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class VersionUpdater
    {
        private const string GITHUB_RELEASES_API_URL = "https://api.github.com/repos/TV-Rename/tvrename/releases";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [ItemCanBeNull]
        public static async Task<Release> CheckForUpdatesAsync()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Release currentVersion;

            try
            {
                currentVersion = ObtainCurrentVersion();
            }
            catch (ArgumentException e)
            {
                Logger.Error("Failed to establish if there are any new releases as could not parse internal version: " + Helpers.DisplayVersion, e);
                return null;
            }

            (Release latestVersion, Release latestBetaVersion) = await GetLatestReleases().ConfigureAwait(false);

            switch (TVSettings.Instance.mode)
            {
                case TVSettings.BetaMode.ProductionOnly when latestVersion?.NewerThan(currentVersion)?? false:
                    return latestVersion;

                case TVSettings.BetaMode.BetaToo when latestBetaVersion?.NewerThan(currentVersion)??false:
                    return latestBetaVersion;

                default:
                    return null;
            }
        }

        private static async Task<(Release latestVersion, Release latestBetaVersion)> GetLatestReleases()
        {
            Release latestVersion = null;
            Release latestBetaVersion = null;

            try
            {
                JArray gitHubInfo = null;

                await HttpHelper.RetryOnExceptionAsync<Exception>
                (3, TimeSpan.FromSeconds(2), GITHUB_RELEASES_API_URL, async () =>
                {
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent", TVSettings.Instance.USER_AGENT);
                    Task<string> response = client.DownloadStringTaskAsync(GITHUB_RELEASES_API_URL);
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
                        if (!gitHubReleaseJson["assets"].HasValues)
                        {
                            continue; //we have no files for this release, so ignore
                        }

                        Release testVersion = ParseFromJson(gitHubReleaseJson);

                        (latestBetaVersion, latestVersion) = UpdateLatest(testVersion, latestBetaVersion, latestVersion);
                    }
                    catch (NullReferenceException ex)
                    {
                        Logger.Error("Looks like the JSON payload from GitHub has changed");
                        Logger.Debug(ex, gitHubReleaseJson.ToString());
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Logger.Debug("Generally happens because the release did not have an exe attached");
                        Logger.Debug(ex, gitHubReleaseJson.ToString());
                    }
                }

                if (latestVersion is null)
                {
                    Logger.Error("Could not find latest version information from GitHub: {0}", gitHubInfo.ToString());
                    return (null, latestBetaVersion);
                }

                if (latestBetaVersion is null)
                {
                    Logger.Error("Could not find latest beta version information from GitHub: {0}", gitHubInfo.ToString());
                    return (latestVersion, null);
                }
            }
            catch (WebException wex)
            {
                if (wex.IsUnimportant())
                {
                    Logger.Error($"Failed to contact GitHub to identify new releases {wex.LoggableDetails()}");
                }
                else
                {
                    Logger.Warn($"Failed to contact GitHub to identify new releases {wex.LoggableDetails()}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to contact GitHub to identify new releases");
            }

            return (latestVersion, latestBetaVersion);
        }

        private static (Release latestBetaVersion, Release latestVersion) UpdateLatest([NotNull] Release testVersion,
            Release latestBetaVersion, Release latestVersion)
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

        [NotNull]
        private static Release ParseFromJson([NotNull] JObject gitHubReleaseJson)
        {
            DateTime.TryParse(gitHubReleaseJson["published_at"].ToString(), out DateTime releaseDate);
            Release testVersion = new Release(gitHubReleaseJson["tag_name"].ToString(),
                Release.VersionType.semantic)
            {
                DownloadUrl = gitHubReleaseJson["assets"][0]["browser_download_url"].ToString(),
                ReleaseNotesText = gitHubReleaseJson["body"].ToString(),
                ReleaseNotesUrl = gitHubReleaseJson["html_url"].ToString(),
                ReleaseDate = releaseDate,
                IsBeta = gitHubReleaseJson["prerelease"].ToString() == "True"
            };
            return testVersion;
        }

        [NotNull]
        private static Release ObtainCurrentVersion()
        {
            string currentVersionString = Helpers.DisplayVersion;

            bool inDebug = currentVersionString.EndsWith(Helpers.DEBUG_TEXT, StringComparison.Ordinal);
            //remove debug stuff
            if (inDebug)
            {
                currentVersionString = currentVersionString.Substring(0,
                    currentVersionString.LastIndexOf(Helpers.DEBUG_TEXT, StringComparison.Ordinal));
            }

            return new Release(currentVersionString, Release.VersionType.friendly);
        }
    }
}
