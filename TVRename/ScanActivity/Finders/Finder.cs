namespace TVRename;

public abstract class Finder : ScanActivity
{
    public ItemList ActionList { protected get; set; }

    protected Finder(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
    {
        ActionList = MDoc.TheActionList;
    }

    // ReSharper disable once InconsistentNaming
    public enum FinderDisplayType { local, downloading, search }

    public abstract FinderDisplayType DisplayType();
}
