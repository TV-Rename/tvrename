using System;

namespace TVRename
{
    internal class TvShowSubdiretoryFormatCheck : TvShowCheck
    {
        public TvShowSubdiretoryFormatCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
        {
        }

        public override bool Check() => Show.AutoAddType != DefaultShowFormat();

        private ShowConfiguration.AutomaticFolderType DefaultShowFormat()
        {
            if (TVSettings.Instance.DefShowUseBase)
            {
                return ShowConfiguration.AutomaticFolderType.baseOnly;
            }

            if (!TVSettings.Instance.DefShowAutoFolders)
            {
                return ShowConfiguration.AutomaticFolderType.baseOnly;
            }

            return ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
        }

        public override string Explain() => $"TV Show does not use the library default ({DefaultShowFormat()}) for folders creation, it uses {GetShowFormatText()} ({Show.AutoAddType})";

        private string GetShowFormatText()
        {
            return Show.AutoAddType switch
            {
                ShowConfiguration.AutomaticFolderType.none => "No automatic season folders",
                ShowConfiguration.AutomaticFolderType.baseOnly => "base folder for all seasons",
                ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat =>
                    $"Subfolders per season {TVSettings.Instance.SeasonFolderFormat}",
                ShowConfiguration.AutomaticFolderType.customFolderFormat =>
                    $"Subfolders per season {Show.AutoAddCustomFolderFormat}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void FixInternal()
        {
            Show.AutoAddType = DefaultShowFormat();
            //TODO Should move files from the old location to the new one!!
        }

        public override string CheckName => "[TV] Use Custom season subfolder Format";
    }
}
