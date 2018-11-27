namespace TVRename
{
    internal class FindMissingEpisodesSearch : FindMissingEpisodes
    {
        public FindMissingEpisodesSearch(TVDoc doc) : base(doc) { }

        protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.search;
    }
}
