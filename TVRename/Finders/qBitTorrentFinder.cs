using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class qBitTorrentFinder : TorrentFinder
    {
        public qBitTorrentFinder(TVDoc i) : base(i) { }
        public override bool Active() => true;
        
        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            List<TorrentEntry> downloading = GetqBitTorrentDownloads();

            SearchForAppropriateDownloads(prog, startpct, totPct, downloading);
        }

        private static List<TorrentEntry> GetqBitTorrentDownloads()
        {
            List < TorrentEntry >  ret = new List<TorrentEntry>();

            // get list of files being downloaded by qBitTorrentFinder
            string host = "localhost";
            string port = "8080";
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(host))
                return ret;

            string url = $"http://{host}:{port}/query/";

            JToken settings = JsonHelper.ObtainToken(url + "preferences");
            JArray currentDownloads = JsonHelper.ObtainArray(url + "torrents?filter=all");

            foreach (JToken torrent in currentDownloads.Children())
            {
                JArray stuff2 = JsonHelper.ObtainArray(url + "propertiesFiles/" + torrent["hash"]);

                foreach (JToken file in stuff2.Children())
                {
                    ret.Add(new TorrentEntry(torrent["name"].ToString(), settings["save_path"] + Path.DirectorySeparator + file["name"], (int)(100 * file["progress"].ToObject<float>())));
                }
            }

            return ret;
        }
    }
}
