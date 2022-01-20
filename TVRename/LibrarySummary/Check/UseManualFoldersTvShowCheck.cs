using JetBrains.Annotations;

namespace TVRename
{
    internal class UseManualFoldersTvShowCheck : CustomTvShowCheck
    {
        public UseManualFoldersTvShowCheck([NotNull] ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        [NotNull]
        protected override string FieldName => "[TV] Use Manual season Folders for TV Show";
        protected override bool Field => Show.UsesManualFolders();

        protected override void FixInternal()
        {
            Show.ManualFolderLocations.Clear();
            Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
        }
    }
}
