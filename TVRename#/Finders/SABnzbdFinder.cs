using System;
using System.Net;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;


namespace TVRename
{
    class SaBnzbdFinder :Finder
    {
        public SaBnzbdFinder(TVDoc i) : base(i) { }

        public override bool Active()
        {
            return TVSettings.Instance.CheckSaBnzbd;
        }

        public override FinderDisplayType DisplayType()
        {
            return FinderDisplayType.Downloading;
        }

        protected static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public override void Check(SetProgressDelegate prog, int startpct, int totPct)
        {
            if (String.IsNullOrEmpty(TVSettings.Instance.SabapiKey) || String.IsNullOrEmpty(TVSettings.Instance.SabHostPort))
            {
                prog.Invoke(startpct + totPct);
                return;
            }

            // get list of files being downloaded by SABnzbd

            // Something like:
            // http://localhost:8080/sabnzbd/api?mode=queue&apikey=xxx&start=0&limit=8888&output=xml
            String theUrl = "http://" + TVSettings.Instance.SabHostPort +
                            "/sabnzbd/api?mode=queue&start=0&limit=8888&output=xml&apikey=" + TVSettings.Instance.SabapiKey;

            WebClient wc = new WebClient();
            byte[] r = null;
            try
            {
                r = wc.DownloadData(theUrl);
            }
            catch (WebException)
            {
            }

            if (r == null)
            {
                prog.Invoke(startpct + totPct);
                return;
            }

            try
            {
                SAB.Result res = SAB.Result.Deserialize(r);
                if (res != null && res.Status == "False")
                {
                    Logger.Error("Error processing data from SABnzbd (Queue Check): {0}",res.Error );
                    prog.Invoke(startpct + totPct);
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
                Logger.Error(e, "Error processing data from SABnzbd (Queue Check)");
                prog.Invoke(startpct + totPct);
                return;
            }

            System.Diagnostics.Debug.Assert(sq != null); // shouldn't happen
            if (sq == null || sq.Slots == null || sq.Slots.Length == 0) // empty queue
                return;

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = TheActionList.Count + 2;
            int n = 1;

            foreach (ITem action1 in TheActionList)
            {
                if (ActionCancel)
                    return;

                n++;
                prog.Invoke(startpct + totPct * n / c);

                if (!(action1 is ItemMissing))
                    continue;

                ItemMissing action = (ItemMissing)(action1);

                string showname = Helpers.SimplifyName(action.Episode.Si.ShowName);

                foreach (SAB.QueueSlotsSlot te in sq.Slots)
                {
                    //foreach (queueSlotsSlot te in qs)
                    {
                        FileInfo file = new FileInfo(te.Filename);
                        //if (!TVSettings.Instance.UsefulExtension(file.Extension, false)) // not a usefile file extension
                        //    continue;

                        if (FileHelper.SimplifyAndCheckFilename(file.FullName, showname, true, false))
                        {
                            int seasF;
                            int epF;
                            if (TVDoc.FindSeasEp(file, out seasF, out epF, action.Episode.Si) &&
                                (seasF == action.Episode.SeasonNumber) && (epF == action.Episode.EpNum))
                            {
                                toRemove.Add(action1);
                                newList.Add(new ItemSaBnzbd(te, action.Episode, action.TheFileNoExt));
                                break;
                            }
                        }
                    }
                }
            }

            foreach (ITem i in toRemove)
                TheActionList.Remove(i);

            foreach (ITem action in newList)
                TheActionList.Add(action);

            prog.Invoke(startpct + totPct);
        }

    }
}
