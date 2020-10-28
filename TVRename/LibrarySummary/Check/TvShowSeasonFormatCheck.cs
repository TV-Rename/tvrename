using System;
using JetBrains.Annotations;

namespace TVRename
{
    class TvShowSeasonFormatCheck : TvShowCheck
    {
        public TvShowSeasonFormatCheck([NotNull] ShowConfiguration show) : base(show)
        {
        }

        public override bool Check() => Show.AutoAddType !=ShowConfiguration.AutomaticFolderType.libraryDefault;

        public override string Explain() => $"TV Show does not use the library default for AutomaticFolder creation, it uses {Show.AutoAddType}{(Show.AutoAddType==ShowConfiguration.AutomaticFolderType.custom ? $" {Show.AutoAddCustomFolderFormat}" : "")}";

        protected override void FixInternal()
        {
            //todo
            throw new NotImplementedException();
        }
        public override string CheckName => "[TV] Use Custom season Folder Name Format";
    }
}
