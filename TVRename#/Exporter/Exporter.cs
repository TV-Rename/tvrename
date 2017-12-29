using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TVRename
{
    abstract class Exporter
    {
        public abstract bool Active();
        public abstract string Location();
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    }

    abstract class ShowsExporter : Exporter
    {
        public abstract void Run(List<ShowItem> shows);
    }


    abstract class MissingExporter : Exporter
    {
        public abstract void Run(ItemList TheActionList);
    }

    abstract class UpcomingExporter : Exporter
    {
        protected TVDoc mDoc;
        

        public UpcomingExporter(TVDoc doc)
        {
            mDoc = doc;
        }

        public string produce() 
        {
            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P
                
                MemoryStream ms = new MemoryStream(); //duplicated the IF statement one for RSS and one for XML so that both can be generated.
                List<ProcessedEpisode> lpe = mDoc.NextNShows(TVSettings.Instance.ExportRSSMaxShows, TVSettings.Instance.ExportRSSDaysPast, TVSettings.Instance.ExportRSSMaxDays);
                if (lpe != null)
                    if (generate(ms,lpe ))
                    {
                        return Encoding.ASCII.GetString(ms.ToArray());
                    }
               
            }
            catch
            {
            }
            return "";
        }

        public void Run()
        {
            if (Active())
            {
                StreamWriter file = new StreamWriter(Location());
                String contents = produce();
                file.Write(contents);
                file.Close();
                logger.Info("Output File to :{0}", Location());
                logger.Trace("contents of File are :{0}", contents);
            }
        }

        protected abstract bool generate(Stream str, List<ProcessedEpisode> elist);
    }

}
