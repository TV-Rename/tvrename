namespace TVRename
{
    internal class FindMissingEpisodesLocally : FindMissingEpisodes
    {
        public FindMissingEpisodesLocally(TVDoc doc) : base(doc) {}

        protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.local;
    }
}
