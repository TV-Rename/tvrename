using JetBrains.Annotations;
using System.Linq;

namespace TVRename
{
    internal class FolderBaseTvCheck : TvShowCheck
    {
        public FolderBaseTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        public override bool Check() => !Show.AutoAddFolderBase.HasValue() && Show.AutoAddNewSeasons();

        [NotNull]
        public override string Explain() => "This TV show does not have an automatic folder base specified.";

        protected override void FixInternal()
        {
            if (TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation && TVSettings.Instance.DefShowLocation.HasValue())
            {
                Show.AutoAddFolderBase = TVSettings.Instance.DefShowLocation.EnsureEndsWithSeparator() + TVSettings.Instance.DefaultTVShowFolder(Show);

                return;
            }

            if (TVSettings.Instance.LibraryFolders.Count > 1)
            {
                throw new FixCheckException("Can't fix movie as multiple TV Show Library Folders are specified");
            }

            if (TVSettings.Instance.LibraryFolders.Count == 0)
            {
                throw new FixCheckException("Can't fix movie as no TV Show Library Folders are specified");
            }

            Show.AutoAddFolderBase = TVSettings.Instance.LibraryFolders.First().EnsureEndsWithSeparator()
                                     + TVSettings.Instance.DefaultTVShowFolder(Show);
        }

        [NotNull]
        public override string CheckName => "[TV] Has an automatic base folder supplied";
    }
}
