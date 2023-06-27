using System;
using System.Security;

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

    /// <exception cref="ArgumentException">Locaiton is not valid.</exception>
    /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.IO.IOException"></exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    protected override void Do()
    {
        using System.IO.StreamWriter file = new(Location());
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
                $"{pe.TheCachedSeries.Name.InDoubleQuotes()},{pe.AppropriateSeasonNumber},{pe.EpisodeNumbersAsText},{pe.Name.InDoubleQuotes()},{dt:G},{im.TargetFolder.InDoubleQuotes()},{im.Filename.InDoubleQuotes()},{pe.SeriesId}");
        }
    }
}
