namespace TVRename
{
    internal class FindMissingEpisodesSearch : FindMissingEpisodes
    {
        public FindMissingEpisodesSearch(TVDoc doc) : base(doc) { }
        protected override string Checkname() => "Looked online for the missing files to see if they can be downloaded";
        protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.search;
    }
}
