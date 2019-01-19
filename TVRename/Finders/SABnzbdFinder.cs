// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    class SABnzbdFinder : DownloadingFinder
    {
        public SABnzbdFinder(TVDoc i) : base(i) { }

        public override bool Active() => TVSettings.Instance.CheckSABnzbd;
        protected override string Checkname() => "Looked in the listed SABnz queue to see if the episode is already being downloaded";

        protected override void Check(SetProgressDelegate prog, ICollection<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.SABAPIKey) || string.IsNullOrEmpty(TVSettings.Instance.SABHostPort))
            {
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
                return;
            }

            try
            {
                SAB.Result res = SAB.Result.Deserialize(r);
                if (res != null && res.status == "False")
                {
                    LOGGER.Error("Error processing data from SABnzbd (Queue Check): {0}", res.error);
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
                return;
            }

            try
            {
                System.Diagnostics.Debug.Assert(sq != null); // shouldn't happen
                if (sq.slots == null || sq.slots.Length == 0) // empty queue
                    return;

                ItemList newList = new ItemList();
                ItemList toRemove = new ItemList();
                int c = ActionList.MissingItems().Count() + 2;
                int n = 1;

                foreach (ItemMissing action in ActionList.MissingItems())
                {
                    if (settings.Token.IsCancellationRequested)
                        return;

                    UpdateStatus(n++, c, action.Filename);

                    if (action.Episode?.Show is null) continue;

                    string showname = Helpers.SimplifyName(action.Episode.Show.ShowName);

                    foreach (SAB.QueueSlotsSlot te in sq.slots)
                    {
                        if (te.filename is null) continue;

                        FileInfo file = new FileInfo(te.filename);

                        if (!FileHelper.SimplifyAndCheckFilename(file.FullName, showname, true, false)) continue;

                        if (!FinderHelper.FindSeasEp(file, out int seasF, out int epF, out int _,
                                action.Episode.Show) ||
                            (seasF != action.Episode.AppropriateSeasonNumber) ||
                            (epF != action.Episode.AppropriateEpNum)) continue;

                        toRemove.Add(action);
                        newList.Add(new ItemDownloading(te, action.Episode, action.TheFileNoExt, DownloadApp.SABnzbd));
                        break;
                    }
                }

                ActionList.Replace(toRemove, newList);
            }
            catch (NullReferenceException nre)
            {
                LOGGER.Error(nre,$"Null Reference in SAB - {r}");
            }
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
