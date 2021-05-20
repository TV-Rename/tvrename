using JetBrains.Annotations;
using System;

namespace TVRename
{
    internal class UseManualFoldersTvShowCheck : CustomTvShowCheck
    {
        public UseManualFoldersTvShowCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        protected override string FieldName => "Use Manual Folders for TV Show";
        protected override bool Field => Show.UsesManualFolders();

        protected override void FixInternal()
        {
            Show.ManualFolderLocations.Clear();
            Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefault;
        }
    }
}
