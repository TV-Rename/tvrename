using System;
using System.Collections.Generic;
using System.IO;

namespace TVRename
{
    internal abstract class Exporter
    {
        public abstract bool Active();
        public abstract void Run();
        protected abstract string Location();
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    }

    internal abstract class ShowsExporter : Exporter
    {
        protected readonly ICollection<ShowItem> Shows;


        protected ShowsExporter(ICollection<ShowItem> shows)
        {
            this.Shows = shows;
        }
    }


    internal abstract class ActionListExporter : Exporter
    {
        protected readonly ItemList TheActionList;


        protected ActionListExporter(ItemList theActionList)
        {
            this.TheActionList = theActionList;
        }

        public abstract bool ApplicableFor(TVSettings.ScanType st);
    }

    internal abstract class UpcomingExporter : Exporter
    {
        protected readonly TVDoc Doc;


        protected UpcomingExporter(TVDoc doc)
        {
            this.Doc = doc;
        }

        private string Produce() 
        {
            try
            {
                // dirty try/catch to "fix" the problem that a share can disappear during a sleep/resume, and
                // when windows restarts, the share isn't "back" before this timer times out and fires
                // windows explorer tends to lose explorer windows on shares when slept/resumed, too, so its not
                // just me :P

                using (MemoryStream ms = new MemoryStream())
                {
                    List<ProcessedEpisode> lpe = this.Doc.Library.NextNShows(TVSettings.Instance.ExportRSSMaxShows,
                        TVSettings.Instance.ExportRSSDaysPast, TVSettings.Instance.ExportRSSMaxDays);
                    if (lpe != null)
                        if (Generate(ms, lpe))
                        {
                            return System.Text.Encoding.ASCII.GetString(ms.ToArray());
                        }
                }
            }

            catch (Exception e)
            {
                Logger.Error(e, "Failed to produce records to put into Export file at: {0}", Location());
            }
            return "";
        }

        public override void Run()
        {
            if (Active())
            {
                try
                {

                    //Create the directory if needed
                    Directory.CreateDirectory(Path.GetDirectoryName(Location()) ??"");
                    string contents = Produce();

                    //Write Contents to file
                    using (StreamWriter file = new StreamWriter(Location()))
                    {
                        file.Write(contents);
                    }

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
