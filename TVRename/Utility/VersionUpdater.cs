// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TVRename
{
    public static class VersionUpdater
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task<UpdateVersion> CheckForUpdatesAsync()
        {
            // ReSharper disable once InconsistentNaming
            const string GITHUB_RELEASES_API_URL = "https://api.github.com/repos/TV-Rename/tvrename/releases";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            UpdateVersion currentVersion;

            try
            {
                currentVersion = ObtainCurrentVersion();
            }
            catch (ArgumentException e)
            {
                Logger.Error("Failed to establish if there are any new releases as could not parse internal version: " + Helpers.DisplayVersion, e);
                return null;
            }

            UpdateVersion latestVersion = null;
            UpdateVersion latestBetaVersion = null;

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                Task<string> response = client.DownloadStringTaskAsync(GITHUB_RELEASES_API_URL);
                JArray gitHubInfo = JArray.Parse(await response.ConfigureAwait(false));

                foreach (JObject gitHubReleaseJson in gitHubInfo.Children<JObject>())
                {
                    try
                    {
                        if (!gitHubReleaseJson["assets"].HasValues) continue; //we have no files for this release, so ignore

                        DateTime.TryParse(gitHubReleaseJson["published_at"].ToString(), out DateTime releaseDate);
                        UpdateVersion testVersion = new UpdateVersion(gitHubReleaseJson["tag_name"].ToString(),
                            UpdateVersion.VersionType.semantic)
                        {
                            DownloadUrl = gitHubReleaseJson["assets"][0]["browser_download_url"].ToString(),
                            ReleaseNotesText = gitHubReleaseJson["body"].ToString(),
                            ReleaseNotesUrl = gitHubReleaseJson["html_url"].ToString(),
                            ReleaseDate = releaseDate,
                            IsBeta = (gitHubReleaseJson["prerelease"].ToString() == "True")
                        };

                        //all versions want to be considered if you are in the beta stream
                        if (testVersion.NewerThan(latestBetaVersion)) latestBetaVersion = testVersion;

                        //If the latest version is a production one then update the latest production version
                        if (!testVersion.IsBeta)
                        {
                            if (testVersion.NewerThan(latestVersion)) latestVersion = testVersion;
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        Logger.Warn("Looks like the JSON payload from GitHub has changed");
                        Logger.Debug(ex, gitHubReleaseJson.ToString());
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Logger.Debug("Generally happens because the release did not have an exe attached");
                        Logger.Debug(ex, gitHubReleaseJson.ToString());
                    }
                }
                if (latestVersion == null)
                {
                    Logger.Error("Could not find latest version information from GitHub: {0}", response);
                    return null;
                }
                if (latestBetaVersion == null)
                {
                    Logger.Error("Could not find latest beta version information from GitHub: {0}", response);
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to contact GitHub to identify new releases");
                return null;
            }

            if ((TVSettings.Instance.mode == TVSettings.BetaMode.ProductionOnly) &&
                (latestVersion.NewerThan(currentVersion))) return latestVersion;

            if ((TVSettings.Instance.mode == TVSettings.BetaMode.BetaToo) &&
                (latestBetaVersion.NewerThan(currentVersion)))
                return latestBetaVersion;

            return null;
        }

        private static UpdateVersion ObtainCurrentVersion()
        {
            string currentVersionString = Helpers.DisplayVersion;

            bool inDebug = currentVersionString.EndsWith(" ** Debug Build **");
            //remove debug stuff
            if (inDebug)
                currentVersionString = currentVersionString.Substring(0,
                    currentVersionString.LastIndexOf(" ** Debug Build **", StringComparison.Ordinal));

            return new UpdateVersion(currentVersionString, UpdateVersion.VersionType.friendly);
        }
    }
}
