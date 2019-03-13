using System;
using System.Linq;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class MissingCSV : ActionListExporter
    {
        public MissingCSV(ItemList theActionList) : base(theActionList) {}

        public override bool Active() => TVSettings.Instance.ExportMissingCSV;
        protected override string Location() => TVSettings.Instance.ExportMissingCSVTo;
        public override bool ApplicableFor(TVSettings.ScanType st) => (st==TVSettings.ScanType.Full );

        protected override void Do()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Location()))
            {
                file.WriteLine("Show Name,Season,Episode,Episode Name,Air Date,Folder,Nice Name,thetvdb.com Code");

                foreach (ItemMissing im in TheActionList.MissingItems().ToList())
                {
                    ProcessedEpisode pe = im.Episode;
                    DateTime? dt = pe.GetAirDateDt(true);
                    file.WriteLine($"\"{pe.TheSeries.Name}\",{pe.AppropriateSeasonNumber},{pe.NumsAsString()},\"{pe.Name}\",{dt:G},\"{im.TargetFolder}\",\"{im.Filename }\",{pe.SeriesId}");
                }
            }
        }
    }
}
