using System;
using System.Net;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


namespace TVRename
{
    class SABnzbdFinder :Finder
    {
        public SABnzbdFinder(TVDoc i) : base(i) { }

        public override bool Active()
        {
            return TVSettings.Instance.CheckSABnzbd;
        }

        public override Finder.FinderDisplayType DisplayType()
        {
            return FinderDisplayType.Downloading;
        }

        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.SABAPIKey) || String.IsNullOrEmpty(TVSettings.Instance.SABHostPort))
            {
                prog.Invoke(startpct + totPct);
                return;
            }

            // get list of files being downloaded by SABnzbd

            // Something like:
            // http://localhost:8080/sabnzbd/api?mode=queue&apikey=xxx&start=0&limit=8888&output=xml
            string theURL = "http://" + TVSettings.Instance.SABHostPort +
                            "/sabnzbd/api?mode=queue&start=0&limit=8888&output=xml&apikey=" + TVSettings.Instance.SABAPIKey;

            WebClient wc = new WebClient();
            byte[] r = null;
            try
            {
                r = wc.DownloadData(theURL);
            }
            catch (WebException)
            {
                logger.Warn("Failed to obtain SABnzbd, please recheck settings: " + theURL);
            }

            if (r == null)
            {
                prog.Invoke(startpct + totPct);
                return;
            }

            try
            {
                SAB.result res = SAB.result.Deserialize(r);
                if (res != null && res.status == "False")
                {
                    logger.Error("Error processing data from SABnzbd (Queue Check): {0}",res.error );
                    prog.Invoke(startpct + totPct);
                    return;
                }
            }
            catch
            {
                // wasn't a result/error combo.  this is good!
            }

            SAB.queue sq = null;
            try
            {
                sq = SAB.queue.Deserialize(r);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error processing data from SABnzbd (Queue Check)");
                prog.Invoke(startpct + totPct);
                return;
            }

            System.Diagnostics.Debug.Assert(sq != null); // shouldn't happen
            if (sq?.slots == null || sq.slots.Length == 0) // empty queue
                return;

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = this.TheActionList.Count + 2;
            int n = 1;

            foreach (Item Action1 in this.TheActionList)
            {
                if (this.ActionCancel)
                    return;

                n++;
                prog.Invoke(startpct + totPct * n / c);

                if (!(Action1 is ItemMissing))
                    continue;

                ItemMissing Action = (ItemMissing)(Action1);

                string showname = Helpers.SimplifyName(Action.Episode.SI.ShowName);

                foreach (SAB.queueSlotsSlot te in sq.slots)
                {
                    //foreach (queueSlotsSlot te in qs)
                    {
                        FileInfo file = new FileInfo(te.filename);
                        //if (!TVSettings.Instance.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        //    continue;

                        if (!FileHelper.SimplifyAndCheckFilename(file.FullName, showname, true, false)) continue;
                        if (!TVDoc.FindSeasEp(file, out int seasF, out int epF, Action.Episode.SI) ||
                            (seasF != Action.Episode.SeasonNumber) || (epF != Action.Episode.EpNum)) continue;
                        toRemove.Add(Action1);
                        newList.Add(new ItemSABnzbd(te, Action.Episode, Action.TheFileNoExt));
                        break;
                    }
                }
            }

            foreach (Item i in toRemove)
                this.TheActionList.Remove(i);

            foreach (Item Action in newList)
                this.TheActionList.Add(Action);

            prog.Invoke(startpct + totPct);
        }

    }
}
