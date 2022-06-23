using System.Linq;

namespace TVRename;

internal class UseManualFoldersTvShowCheck : CustomTvShowCheck
{
    public UseManualFoldersTvShowCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    protected override string FieldName => "[TV] Use Manual season Folders for TV Show";
    protected override bool Field => Show.UsesManualFolders();

    protected override void FixInternal()
    {
        Show.ManualFolderLocations.Clear();
        Show.AutoAddType = ShowConfiguration.AutomaticFolderType.libraryDefaultFolderFormat;
    }
    protected override string CustomFieldValue => Show.ManualFolderLocations.Values.SelectMany(x=>x).ToCsv();

    protected override string DefaultFieldValue => Show.AutoAddFolderBase;
}
