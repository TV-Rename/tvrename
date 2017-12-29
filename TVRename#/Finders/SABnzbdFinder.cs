using System;
using System.Diagnostics;
using System.Net;
using Alphaleonis.Win32.Filesystem;
using NLog;
using TVRename.SAB;

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

        protected static Logger Logger = LogManager.GetCurrentClassLogger();


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
                Result res = Result.Deserialize(r);
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

            Queue sq;
            try
            {
                sq = Queue.Deserialize(r);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error processing data from SABnzbd (Queue Check)");
                prog.Invoke(startpct + totPct);
                return;
            }

            Debug.Assert(sq != null); // shouldn't happen
            if ( sq.Slots == null || sq.Slots.Length == 0) // empty queue
                return;

            ItemList newList = new ItemList();
            ItemList toRemove = new ItemList();
            int c = TheActionList.Count + 2;
            int n = 1;

            foreach (Item action1 in TheActionList)
            {
                if (ActionCancel)
                    return;

                n++;
                prog.Invoke(startpct + totPct * n / c);

                if (!(action1 is ItemMissing))
                    continue;

                ItemMissing action = (ItemMissing)(action1);

                string showname = Helpers.SimplifyName(action.Episode.Si.ShowName);

                foreach (QueueSlotsSlot te in sq.Slots)
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

            foreach (Item i in toRemove)
                TheActionList.Remove(i);

            foreach (Item action in newList)
                TheActionList.Add(action);

            prog.Invoke(startpct + totPct);
        }

    }
}
