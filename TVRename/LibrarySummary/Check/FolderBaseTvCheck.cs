using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    internal class FolderBaseTvCheck : TvShowCheck
    {
        public FolderBaseTvCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc) { }

        public override bool Check() => !Show.AutoAddFolderBase.HasValue() && Show.AutoAddNewSeasons();

        public override string Explain() => "This TV show does not have an automatic folder base specified.";

        protected override void FixInternal()
        {
            if (TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation && TVSettings.Instance.DefShowLocation.HasValue())
            {
                Show.AutoAddFolderBase =
                    TVSettings.Instance.DefShowLocation.EnsureEndsWithSeparator()
                    + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(Show.ShowName));

                return;
            }

            if (TVSettings.Instance.LibraryFolders.Count > 1)
            {
                throw new FixCheckException("Can't fix movie as multiple TV Library Folders are specified");
            }

            if (TVSettings.Instance.LibraryFolders.Count == 0)
            {
                throw new FixCheckException("Can't fix movie as no TV Library Folders are specified");
            }

            Show.AutoAddFolderBase = TVSettings.Instance.MovieLibraryFolders.First().EnsureEndsWithSeparator()
                                     + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(Show.ShowName));
        }

        public override string CheckName => "[TV] Has an automatic base folder supplied";
    }
}