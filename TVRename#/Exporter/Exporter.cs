using System;
using System.Collections.Generic;
using System.IO;

namespace TVRename
{
    abstract class Exporter
    {
        public abstract bool Active();
        protected abstract string Location();
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
        protected readonly TVDoc mDoc;
        

        public UpcomingExporter(TVDoc doc)
        {
            this.mDoc = doc;
        }

        private string Produce() 
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
                    if (this.Generate(ms,lpe ))
                    {
                        return System.Text.Encoding.ASCII.GetString(ms.ToArray());
                    }
               
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to produce records to put into Export file at: {0}", Location());
            }
            return "";
        }

        public void Run()
        {
            if (Active())
            {
                try
                {

                    //Create the directory if needed
                    Directory.CreateDirectory(Path.GetDirectoryName(Location()) ??"");

                    //Write Contents to file
                    StreamWriter file = new StreamWriter(Location());
                    String contents = Produce();
                    file.Write(contents);
                    file.Close();

                    Logger.Info("Output File to :{0}", Location());
                    Logger.Trace("contents of File are :{0}", contents);
                }
                catch (Exception e)
                {
                    Logger.Error(e,"Failed to Output File to :{0}", Location());
                }
            }
            else
            {
                Logger.Trace("SKipped (Disabled) Output File to :{0}", Location());
            }
        }

        protected abstract bool Generate(Stream str, List<ProcessedEpisode> elist);
    }

}
