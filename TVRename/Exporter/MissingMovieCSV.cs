namespace TVRename;

internal class MissingMovieCsv : MissingActionListExporter
{
    public MissingMovieCsv(ItemList theActionList) : base(theActionList)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportMissingMoviesCSV;
    protected override string Name() => "Missing Movie CSV Exporter";

    protected override string Location() => TVSettings.Instance.ExportMissingMoviesCSVTo;

    protected override void Do()
    {
        using (System.IO.StreamWriter file = new(Location()))
        {
            file.WriteLine("Movie Name,Year,Folder,Nice Name");

            foreach (MovieItemMissing im in TheActionList.MissingMovies)
            {
                MovieConfiguration pe = im.MovieConfig;
                file.WriteLine(
                    $"{pe.ShowName.InDoubleQuotes()},{pe.CachedMovie?.Year},{im.TargetFolder.InDoubleQuotes()},{im.Filename.InDoubleQuotes()}");
            }
        }
    }
}
