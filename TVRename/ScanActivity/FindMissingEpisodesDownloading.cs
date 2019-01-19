namespace TVRename
{
    internal class FindMissingEpisodesDownloading : FindMissingEpisodes
    {
        public FindMissingEpisodesDownloading(TVDoc doc) : base(doc) { }
        protected override string Checkname() => "Looked in download applications for the missing files";
        protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.downloading;
    }
}
