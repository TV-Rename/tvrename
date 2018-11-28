namespace TVRename
{
    internal class FindMissingEpisodesDownloading : FindMissingEpisodes
    {
        public FindMissingEpisodesDownloading(TVDoc doc) : base(doc) { }

        protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.downloading;
    }
}
