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

        public override string Explain() => $"{MediaName} does not use the library default fpr AutomaticFolder creation, it uses {Show.AutoAddType}";

        protected override void FixInternal()
        {
            //todo
            throw new NotImplementedException();
        }
        public override string CheckName => "[TV] Use Custom season Folder Name Format";
    }
}