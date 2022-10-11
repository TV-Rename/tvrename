using System;

namespace TVRename;

// ReSharper disable once InconsistentNaming
internal class MissingCSV : MissingActionListExporter
{
    public MissingCSV(ItemList theActionList) : base(theActionList)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportMissingCSV;

    protected override string Location() => TVSettings.Instance.ExportMissingCSVTo;
    protected override string Name() => "Missing CSV Exporter";

    protected override void Do()
    {
        using (System.IO.StreamWriter file = new(Location()))
        {
            file.WriteLine("Show Name,Season,Episode,Episode Name,Air Date,Folder,Nice Name,thetvdb.com Code");

            foreach (ShowSeasonMissing im in TheActionList.MissingSeasons)
            {
                file.WriteLine(
                    $"{im.Series.Name.InDoubleQuotes()},{im.SeasonNumberAsInt},,,,{im.TargetFolder.InDoubleQuotes()},{im.Filename.InDoubleQuotes()},{im.Series.Id()}");
            }
            foreach (ShowItemMissing? im in TheActionList.MissingEpisodes)
            {
                ProcessedEpisode pe = im.MissingEpisode;
                DateTime? dt = pe.GetAirDateDt(true);
                file.WriteLine(
                    $"{pe.TheCachedSeries.Name.InDoubleQuotes()},{pe.AppropriateSeasonNumber},{pe.EpNumsAsString()},{pe.Name.InDoubleQuotes()},{dt:G},{im.TargetFolder.InDoubleQuotes()},{im.Filename.InDoubleQuotes()},{pe.SeriesId}");
            }
        }
    }
}
