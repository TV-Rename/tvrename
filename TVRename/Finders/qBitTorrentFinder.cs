// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class qBitTorrentFinder : DownloadingFinder
    {
        public qBitTorrentFinder(TVDoc i) : base(i) { }
        public override bool Active() => TVSettings.Instance.CheckqBitTorrent;
        [NotNull]
        protected override string Checkname() => "Looked in the qBitTorrent for the missing files to see if they are being downloaded";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            List<TorrentEntry> downloading = GetqBitTorrentDownloads();
            SearchForAppropriateDownloads(downloading, DownloadApp.qBitTorrent,settings);
        }

        [NotNull]
        private static List<TorrentEntry> GetqBitTorrentDownloads()
        {
            List < TorrentEntry >  ret = new List<TorrentEntry>();

            // get list of files being downloaded by qBitTorrentFinder
            string host = TVSettings.Instance.qBitTorrentHost;
            string port = TVSettings.Instance.qBitTorrentPort;
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                return ret;
            }

            string url = $"http://{host}:{port}/query/";
            string settingsString = null;
            string downloadsString = null;
            string torrentDetailsString = null;

            try
            {
                settingsString= JsonHelper.Obtain(url + "preferences");
                downloadsString = JsonHelper.Obtain(url + "torrents?filter=all");

                JToken settings = JToken.Parse(settingsString);
                JArray currentDownloads = JArray.Parse(downloadsString);

                foreach (JToken torrent in currentDownloads.Children())
                {
                    torrentDetailsString = JsonHelper.Obtain(url + "propertiesFiles/" + torrent["hash"]);
                    JArray stuff2 = JArray.Parse(torrentDetailsString);

                    foreach (JToken file in stuff2.Children())
                    {
                        string downloadedFilename = file["name"].ToString();
                        bool isOnHold = (file["priority"].Value<int>() == 0);
                        if (!downloadedFilename.Contains(".!qB\\.unwanted\\") && !isOnHold)
                        {
                            ret.Add(new TorrentEntry(torrent["name"].ToString(),
                                settings["save_path"] + downloadedFilename,
                                (int)(100 * file["progress"].ToObject<float>())));
                        }
                    }

                    if (!stuff2.Children().Any())
                    {
                        ret.Add(
                            new TorrentEntry(
                                torrent["name"].ToString()
                                , TVSettings.Instance.FilenameFriendly(
                                      settings["save_path"] + torrent["name"].ToString()) +
                                  TVSettings.Instance.VideoExtensionsArray[0]
                                , 0
                            )
                        );
                    }
                }
            }
            catch (WebException)
            {
                LOGGER.Warn(
                    $"Could not connect to {url}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections");
            }
            catch (JsonReaderException ex)
            {
                LOGGER.Warn(ex,
                    $"Could not parse data recieved from {url}, {settingsString} {downloadsString} {torrentDetailsString}");
            }

            return ret;
        }

        internal static void StartTorrentDownload(string torrentUrl, string torrentFileName, bool downloadFileFirst)
        {
            string host = TVSettings.Instance.qBitTorrentHost;
            string port = TVSettings.Instance.qBitTorrentPort;
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                LOGGER.Warn($"Could not download {torrentUrl} via qBitTorrent as settings are not entered for host and port");
                return;
            }


            if (downloadFileFirst)
            {
                AddFile(torrentFileName, $"http://{host}:{port}/api/v2/torrents/add");
            }
            else
            {
                DownloadUrl(torrentUrl, $"http://{host}:{port}/command/download");
            }
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
                        LOGGER.Warn(
                            $"Tried to download {torrentName} from file to qBitTorrent via {url}. Got following response {response.StatusCode}");
                    }
                    else
                    {
                        LOGGER.Info(
                            $"Started download of {torrentName} via file to qBitTorrent using {url}. Got following response {response.StatusCode}");
                    }
                }
            }
            catch (WebException)
            {
                LOGGER.Warn(
                    $"Could not connect to {url} to download {torrentName}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections");
            }
        }

        private static void DownloadUrl(string torrentUrl, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Dictionary<string, string> values = new Dictionary<string, string> {{"urls", torrentUrl}};
                    FormUrlEncodedContent content = new FormUrlEncodedContent(values);
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        LOGGER.Warn(
                            $"Tried to download {torrentUrl} from qBitTorrent via {url}. Got following response {response.StatusCode}");
                    }
                    else
                    {
                        LOGGER.Info(
                            $"Started download of {torrentUrl} via qBitTorrent using {url}. Got following response {response.StatusCode}");
                    }
                }
            }
            catch (WebException)
            {
                LOGGER.Warn(
                    $"Could not connect to {url} to download {torrentUrl}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections");
            }
        }
    }
}
