using System;

namespace TVRename;

internal class MissingMovieCsv : MissingActionListExporter
{
    public MissingMovieCsv(ItemList theActionList) : base(theActionList)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportMissingMoviesCSV;
    protected override string Name() => "Missing Movie CSV Exporter";

    protected override string Location() => TVSettings.Instance.ExportMissingMoviesCSVTo;

    /// <exception cref="ArgumentException">Locaiton is not valid.</exception>
    /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.IO.IOException"></exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    protected override void Do()
    {
        using System.IO.StreamWriter file = new(Location());
        file.WriteLine("Movie Name,Year,Folder,Nice Name");

        foreach (MovieItemMissing im in TheActionList.MissingMovies)
        {
            MovieConfiguration pe = im.MovieConfig;
            file.WriteLine(
                $"{pe.ShowName.InDoubleQuotes()},{pe.CachedMovie?.Year},{im.TargetFolder.InDoubleQuotes()},{im.Filename.InDoubleQuotes()}");
        }
    }
}
