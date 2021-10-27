using JetBrains.Annotations;

namespace TVRename
{
    internal class FindMissingEpisodesDownloading : FindMissingEpisodes
    {
        public FindMissingEpisodesDownloading(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        {
        }

        [NotNull]
        protected override string CheckName() => "Looked in download applications for the missing files";

        protected override Finder.FinderDisplayType CurrentType() => Finder.FinderDisplayType.downloading;
    }
}
