namespace TVRename;

internal class TvShowSeasonFormatCheck : TvShowCheck
{
    public TvShowSeasonFormatCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    public override bool Check() => Show.AutoAddType == ShowConfiguration.AutomaticFolderType.customFolderFormat && TVSettings.Instance.DefShowUseSubFolders;

    public override string Explain() => $"TV Show does not use the library default for AutomaticFolder creation ({TVSettings.Instance.SeasonFolderFormat}), it uses {Show.AutoAddType}{(Show.AutoAddType == ShowConfiguration.AutomaticFolderType.customFolderFormat ? $" {Show.AutoAddCustomFolderFormat}" : "")}";

    protected override void FixInternal()
    {
        Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
        //TODO Should move files from the old location to the new one!!
    }

    public override string CheckName => "[TV] Use Custom season Folder Name Format";
}
