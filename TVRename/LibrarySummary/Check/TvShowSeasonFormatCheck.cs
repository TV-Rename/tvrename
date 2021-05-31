using JetBrains.Annotations;

namespace TVRename
{
    internal class TvShowSeasonFormatCheck : TvShowCheck
    {
        public TvShowSeasonFormatCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        public override bool Check() => Show.AutoAddType != ShowConfiguration.AutomaticFolderType.libraryDefault;

        public override string Explain() => $"TV Show does not use the library default for AutomaticFolder creation, it uses {Show.AutoAddType}{(Show.AutoAddType == ShowConfiguration.AutomaticFolderType.custom ? $" {Show.AutoAddCustomFolderFormat}" : "")}";

        protected override void FixInternal()
        {
            Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefault;
        }

        public override string CheckName => "[TV] Use Custom season Folder Name Format";
    }
}
