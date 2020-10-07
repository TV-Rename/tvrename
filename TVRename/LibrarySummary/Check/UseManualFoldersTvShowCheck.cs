using System;
using JetBrains.Annotations;

namespace TVRename
{
    class UseManualFoldersTvShowCheck : CustomTvShowCheck
    {
        public UseManualFoldersTvShowCheck([NotNull] ShowConfiguration show) : base(show) { }

        protected override string FieldName => "Use Manual Folders for TV Show";
        protected override bool Field => Show.UsesManualFolders();

        protected override void FixInternal()
        {
            throw new NotImplementedException();
        }
    }
}