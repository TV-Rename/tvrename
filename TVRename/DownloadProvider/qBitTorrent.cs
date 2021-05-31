using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class qBitTorrent : IDownloadProvider
    {
        //See https://github.com/qbittorrent/qBittorrent/wiki/Web-API-Documentation for details
        // ReSharper disable once InconsistentNaming
        public enum qBitTorrentAPIVersion
        {
            v0, //Applies to qBittorrent up to v3.1.x
            v1, //Applies to qBittorrent v3.2.0-v4.0.4
            v2 //Applies to qBittorrent v4.1+
        }

        // ReSharper disable once InconsistentNaming
        private enum qBitTorrentAPIPath
        {
            settings,
            torrents,
            torrentDetails,
            addFile,
            addUrl,
            delete
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<TorrentEntry>? GetTorrentDownloads()
        {
            // get list of files being downloaded by qBitTorrentFinder

            if (string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentHost) || string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentPort))
            {
                return null;
            }

            string settingsString = null;
            string downloadsString = null;

            try
            {
                settingsString = HttpHelper.Obtain(GetApiUrl(qBitTorrentAPIPath.settings));
                downloadsString = HttpHelper.Obtain(GetApiUrl(qBitTorrentAPIPath.torrents));

                JToken settings = JToken.Parse(settingsString);
                JArray currentDownloads = JArray.Parse(downloadsString);

                if (!currentDownloads.HasValues && settings.HasValues)
                {
                    Logger.Info($"No Downloads available from qBitTorrent: {currentDownloads}");
                    return new List<TorrentEntry>();
                }

                if (!currentDownloads.HasValues || !settings.HasValues)
                {
                    Logger.Warn($"Could not get currentDownloads or settings from qBitTorrent: {settingsString} {currentDownloads}");
                    return null;
                }

                string savePath = (string)settings["save_path"] ?? string.Empty;

                List<TorrentEntry> ret = new List<TorrentEntry>();
                foreach (JToken torrent in currentDownloads.Children())
                {
                    AddFilesFromTorrent(ret, torrent, savePath);
                }
                return ret;
            }
            catch (WebException wex)
            {
                Logger.Warn(
                    $"Could not connect to local instance {TVSettings.Instance.qBitTorrentHost}:{TVSettings.Instance.qBitTorrentPort}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections: {wex.LoggableDetails()}");
            }
            catch (JsonReaderException ex)
            {
                Logger.Warn(ex,
                    $"Could not parse data recieved from {settingsString} {downloadsString}");
            }

            return null;
        }

        private static void AddFilesFromTorrent(ICollection<TorrentEntry> ret, [NotNull] JToken torrent, string savePath)
        {
            string torrentDetailsString = string.Empty;
            try
            {
                (string hashCode, string torrentName, bool completed) = ExtractTorrentDetails(torrent);

                string url = GetApiUrl(qBitTorrentAPIPath.torrentDetails) + hashCode;
                torrentDetailsString = HttpHelper.Obtain(url);
                JArray torrentDetails = JArray.Parse(torrentDetailsString);

                if (!torrentDetails.Children().Any())
                {
                    string proposedFilename = TVSettings.Instance.FilenameFriendly(savePath + torrentName) +
                                              TVSettings.Instance.VideoExtensionsArray[0];

                    ret.Add(new TorrentEntry(torrentName, proposedFilename, 0, false, hashCode));
                    return;
                }

                foreach (JToken file in torrentDetails.Children())
                {
                    (string downloadedFilename, bool isOnHold, int percentComplete) = ExtractTorrentFileDetails(file);

                    if (!downloadedFilename.Contains(".!qB\\.unwanted\\") && !isOnHold)
                    {
                        ret.Add(new TorrentEntry(torrentName, savePath + downloadedFilename, percentComplete, completed, hashCode));
                    }
                }
            }
            catch (JsonReaderException ex)
            {
                Logger.Warn(ex,
                    $"Could not parse data recieved from {torrentDetailsString}");
            }
        }

        private static (string downloadedFilename, bool isOnHold, int percentComplete) ExtractTorrentFileDetails([NotNull] JToken file)
        {
            string downloadedFilename = file["name"]?.ToString();
            string prioritystring = (string)file["priority"];
            bool b = int.TryParse(prioritystring, out int priority);
            bool isOnHold = !b || priority == 0;
            float? progress = (float?)file["progress"];
            int percentComplete = (int)(100 * (progress ?? 0));

            return (downloadedFilename, isOnHold, percentComplete);
        }

        private static (string hashCode, string torrentName, bool completed) ExtractTorrentDetails([NotNull] JToken torrent)
        {
            string hashCode = (string)torrent["hash"];
            string torrentName = (string)torrent["name"];
            string state = (string)torrent["state"];

            bool completed = state.In("uploading", "pausedUP", "queuedUP", "stalledUP", "checkingUP", "forcedUP");

            return (hashCode, torrentName, completed);
        }

        [NotNull]
        private static string GetApiUrl(qBitTorrentAPIPath path)
        {
            string url = $"http://{TVSettings.Instance.qBitTorrentHost}:{TVSettings.Instance.qBitTorrentPort}/";

            switch (TVSettings.Instance.qBitTorrentAPIVersion)
            {
                case qBitTorrentAPIVersion.v1:
                    {
                        return path switch
                        {
                            qBitTorrentAPIPath.settings => url + "query/preferences",
                            qBitTorrentAPIPath.torrents => url + "query/torrents?filter=all",
                            qBitTorrentAPIPath.torrentDetails => url + "query/propertiesFiles/",
                            qBitTorrentAPIPath.addFile => url + "command/upload",
                            qBitTorrentAPIPath.addUrl => url + "command/download",
                            qBitTorrentAPIPath.delete => url + "command/delete",
                            _ => throw new ArgumentOutOfRangeException(nameof(path), path, null)
                        };
                    }

                case qBitTorrentAPIVersion.v2:
                    {
                        return path switch
                        {
                            qBitTorrentAPIPath.settings => url + "api/v2/app/preferences",
                            qBitTorrentAPIPath.torrents => url + "api/v2/torrents/info?filter=all",
                            qBitTorrentAPIPath.torrentDetails => url + "api/v2/torrents/files?hash=",
                            qBitTorrentAPIPath.addFile => url + "api/v2/torrents/add",
                            qBitTorrentAPIPath.addUrl => url + "api/v2/torrents/add",
                            qBitTorrentAPIPath.delete => url + "api/v2/torrents/delete",
                            _ => throw new ArgumentOutOfRangeException(nameof(path), path, null)
                        };
                    }

                case qBitTorrentAPIVersion.v0:
                    throw new NotSupportedException("Only qBitTorrent API v1 and v2 are supported");
                default:
                    throw new ArgumentOutOfRangeException(nameof(TVSettings.Instance.qBitTorrentAPIVersion), TVSettings.Instance.qBitTorrentAPIVersion, null);
            }
        }

        public void StartUrlDownload(string torrentUrl)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentHost) || string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentPort))
            {
                Logger.Warn($"Could not download {torrentUrl} via qBitTorrent as settings are not entered for host and port");
                return;
            }

            DownloadUrl(torrentUrl, GetApiUrl(qBitTorrentAPIPath.addUrl));
        }

        public void StartTorrentDownload(FileInfo torrentFile)
        {
            StartTorrent(torrentFile.FullName);
        }

        public static void StartTorrent(string torrentFileName)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentHost) || string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentPort))
            {
                Logger.Warn($"Could not download {torrentFileName} via qBitTorrent as settings are not entered for host and port");
                return;
            }

            AddFile(torrentFileName, GetApiUrl(qBitTorrentAPIPath.addFile));
        }

        private static void AddFile(string torrentName, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    MultipartFormDataContent m = new MultipartFormDataContent();
                    m.AddFile("torrents", torrentName, "application/x-bittorrent");
                    HttpResponseMessage response = client.PostAsync(url, m).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Warn(
                            $"Tried to download {torrentName} from file to qBitTorrent via {url}. Got following response {response.StatusCode}");
                    }
                    else
                    {
                        Logger.Info(
                            $"Started download of {torrentName} via file to qBitTorrent using {url}. Got following response {response.StatusCode}");
                    }
                }
            }
            catch (WebException wex)
            {
                Logger.Warn(
                    $"Could not connect to {wex.Response.ResponseUri} to download {torrentName}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections : {wex.Message}");
            }
        }

        private static void DownloadUrl(string torrentUrl, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Dictionary<string, string> values = new Dictionary<string, string> { { "urls", torrentUrl } };
                    FormUrlEncodedContent content = new FormUrlEncodedContent(values);
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        string message =
                            $"Tried to download {torrentUrl} from qBitTorrent via {url}. Got following response {response.StatusCode}";
                        Logger.Warn(message);
                        throw new WebException(message);
                    }
                    else
                    {
                        Logger.Info(
                            $"Started download of {torrentUrl} via qBitTorrent using {url}. Got following response {response.StatusCode}");
                    }
                }
            }
            catch (WebException wex)
            {
                Logger.Warn(
                    $"Could not connect to {wex.Response.ResponseUri} to download {torrentUrl}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections : {wex.LoggableDetails()}");
                throw;
            }
        }

        public void RemoveCompletedDownload(TorrentEntry name)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentHost) || string.IsNullOrEmpty(TVSettings.Instance.qBitTorrentPort))
            {
                Logger.Warn($"Could not remove {name.DownloadingTo} via qBitTorrent as settings are not entered for host and port");
                return;
            }

            string url = GetApiUrl(qBitTorrentAPIPath.delete);

            //Annoyingly V1 uses POST, but V2 is a GET...
            if (TVSettings.Instance.qBitTorrentAPIVersion == qBitTorrentAPIVersion.v1)
            {
                string parametersString = HttpHelper.GetHttpParameters(new Dictionary<string, string> { { "hashes", name.Key } });
                HttpHelper.HttpRequest("POST", url, parametersString.RemoveCharactersFrom("?"), "application/x-www-form-urlencoded", null, null);
            }
            else
            {
                try
                {
                    string parametersString = HttpHelper.GetHttpParameters(new Dictionary<string, string> { { "hashes", name.Key }, { "deleteFiles", "false" } });
                    HttpHelper.HttpRequest("GET", url + parametersString, null, null, null, string.Empty);
                }
                catch (WebException wex)
                {
                    Logger.Warn($"Could not connect to {wex.Response.ResponseUri} to remove {name}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections : {wex.LoggableDetails()}");
                    throw;
                }
            }
        }

        public string Name() => "qBitTorrent";
    }
}
