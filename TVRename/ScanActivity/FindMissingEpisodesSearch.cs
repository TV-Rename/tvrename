namespace TVRename;

internal class FindMissingEpisodesSearch(TVDoc doc, TVDoc.ScanSettings settings) : FindMissingEpisodes(doc, settings)
{
    protected override string CheckName() => "Looked online for the missing files to see if they can be downloaded";

    protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.search;
}
