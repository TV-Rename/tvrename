// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Net;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    class SABnzbdFinder : DownloadingFinder
    {
        public SABnzbdFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.CheckSABnzbd;

        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.SABAPIKey) || string.IsNullOrEmpty(TVSettings.Instance.SABHostPort))
            {
                prog.Invoke(totPct);
                return;
            }

            // get list of files being downloaded by SABnzbd

            // Something like:
            // http://localhost:8080/sabnzbd/api?mode=queue&apikey=xxx&start=0&limit=8888&output=xml
            string theUrl = "http://" + TVSettings.Instance.SABHostPort +
                            "/sabnzbd/api?mode=queue&start=0&limit=8888&output=xml&apikey=" + TVSettings.Instance.SABAPIKey;

            byte[] r = DownloadPage(theUrl);

            if (r == null)
            {
                prog.Invoke(totPct);
                return;
            }

            try
            {
                SAB.Result res = SAB.Result.Deserialize(r);
                if (res != null && res.status == "False")
                {
                    LOGGER.Error("Error processing data from SABnzbd (Queue Check): {0}", res.error);
                    prog.Invoke(totPct);
                    return;
                }
            }
            catch
            {
                // wasn't a result/error combo.  this is good!
            }

            SAB.Queue sq;
            try
            {
                sq = SAB.Queue.Deserialize(r);
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Error processing data from SABnzbd (Queue Check)");
                prog.Invoke(totPct);
                return;
            }

            System.Diagnostics.Debug.Assert(sq != null); // shouldn't happen
            if (sq.slots == null || sq.slots.Length == 0) // empty queue
                return;

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = ActionList.Count + 2;
            int n = 1;

            foreach (ItemMissing action in ActionList.MissingItems())
            {
                if (ActionCancel)
                    return;

                prog.Invoke(startpct + ((totPct - startpct) * (++n) / (c)));

                string showname = Helpers.SimplifyName(action.Episode.Show.ShowName);

                foreach (SAB.QueueSlotsSlot te in sq.slots)
                {
                    FileInfo file = new FileInfo(te.filename);

                    if (!FileHelper.SimplifyAndCheckFilename(file.FullName, showname, true, false)) continue;
                    if (!TVDoc.FindSeasEp(file, out int seasF, out int epF, out int _, action.Episode.Show) ||
                        (seasF != action.Episode.AppropriateSeasonNumber) || (epF != action.Episode.AppropriateEpNum)) continue;
                    toRemove.Add(action);
                    newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, DownloadApp.SABnzbd));
                    break;
                }
            }

            foreach (Item i in toRemove)
                ActionList.Remove(i);

            foreach (Item action in newList)
                ActionList.Add(action);

            prog.Invoke(totPct);
        }

        private static byte[] DownloadPage(string theUrl)
        {
            WebClient wc = new WebClient();
            byte[] r = null;
            try
            {
                r = wc.DownloadData(theUrl);
            }
            catch (WebException)
            {
                LOGGER.Warn("Failed to obtain SABnzbd, please recheck settings: " + theUrl);
            }

            return r;
        }
    }
}
