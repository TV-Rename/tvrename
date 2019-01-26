using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class qBitTorrentFinder : DownloadingFinder
    {
        public qBitTorrentFinder(TVDoc i) : base(i) { }
        public override bool Active() => TVSettings.Instance.CheckqBitTorrent;
        protected override string Checkname() => "Looked in the qBitTorrent for the missing files to see if they are being downloaded";

        protected override void DoCheck(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            List<TorrentEntry> downloading = GetqBitTorrentDownloads();
            SearchForAppropriateDownloads(downloading, DownloadApp.qBitTorrent,settings);
        }

        private static List<TorrentEntry> GetqBitTorrentDownloads()
        {
            List < TorrentEntry >  ret = new List<TorrentEntry>();

            // get list of files being downloaded by qBitTorrentFinder
            string host = TVSettings.Instance.qBitTorrentHost;
            string port = TVSettings.Instance.qBitTorrentPort;
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
                return ret;

            string url = $"http://{host}:{port}/query/";

            JToken settings = null;
            JArray currentDownloads = null;

            try
            {
                settings = JsonHelper.ObtainToken(url + "preferences");
                currentDownloads = JsonHelper.ObtainArray(url + "torrents?filter=all");

                foreach (JToken torrent in currentDownloads.Children())
                {
                    JArray stuff2 = JsonHelper.ObtainArray(url + "propertiesFiles/" + torrent["hash"]);

                    foreach (JToken file in stuff2.Children())
                    {
                        ret.Add(new TorrentEntry(torrent["name"].ToString(),
                            settings["save_path"] + file["name"].ToString(),
                            (int) (100 * file["progress"].ToObject<float>())));
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
                LOGGER.Error(ex,
                    $"Could not parse data recieved from to {url}, {settings} {currentDownloads}");
            }

            return ret;
        }

        internal static void StartTorrentDownload(string torrentUrl)
        {
            string host = TVSettings.Instance.qBitTorrentHost;
            string port = TVSettings.Instance.qBitTorrentPort;
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                LOGGER.Warn($"Could not download {torrentUrl} via qBitTorrent as settings are not entered for host and port");
                return;
            }

            string url = $"http://{host}:{port}/command/download";

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
                LOGGER.Warn($"Could not connect to {url} to downlaod {torrentUrl}, Please check qBitTorrent Settings and ensure qBitTorrent is running with no password required for local connections");
            }
        }
    }
}
