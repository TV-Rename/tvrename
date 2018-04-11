using System;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class MissingCSV : ActionListExporter
    {
        public MissingCSV(ItemList theActionList) : base(theActionList)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportMissingCSV;
        protected override string Location() => TVSettings.Instance.ExportMissingCSVTo;

        public override bool ApplicableFor(TVSettings.ScanType st)
        {
            return (st==TVSettings.ScanType.Full );
        }

        public override void Run()
        {
            if (!Active()) return;

            try
            {

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
                {
                    file.WriteLine("Show Name,Season,Episode,Episode Name,Air Date,Folder,Nice Name,thetvdb.com Code");

                    foreach (Item action in this.TheActionList)
                    {
                        if (action is ItemMissing im)
                        {
                            ProcessedEpisode pe = im.Episode;
                            DateTime? dt = pe.GetAirDateDT(true);
                            file.WriteLine($"\"{pe.TheSeries.Name}\",{pe.AppropriateSeasonNumber},{pe.NumsAsString()},\"{pe.Name}\",{dt:G},\"{action.TargetFolder}\",\"{im.Filename }\",{pe.SeriesID}");
                        }

                    }

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
