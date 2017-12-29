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
        protected static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    }

    abstract class ShowsExporter : Exporter
    {
        public abstract void Run(List<ShowItem> shows);
    }


    abstract class MissingExporter : Exporter
    {
        public abstract void Run(ItemList theActionList);
    }

    abstract class UpcomingExporter : Exporter
    {
        protected TVDoc MDoc;
        

        public UpcomingExporter(TVDoc doc)
        {
            MDoc = doc;
        }

        public string Produce() 
        {
            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P
                
                MemoryStream ms = new MemoryStream(); //duplicated the IF statement one for RSS and one for XML so that both can be generated.
                List<ProcessedEpisode> lpe = MDoc.NextNShows(TVSettings.Instance.ExportRssMaxShows, TVSettings.Instance.ExportRssDaysPast, TVSettings.Instance.ExportRssMaxDays);
                if (lpe != null)
                    if (Generate(ms,lpe ))
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
                String contents = Produce();
                file.Write(contents);
                file.Close();
                Logger.Info("Output File to :{0}", Location());
                Logger.Trace("contents of File are :{0}", contents);
            }
        }

        protected abstract bool Generate(Stream str, List<ProcessedEpisode> elist);
    }

}
