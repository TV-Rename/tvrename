namespace TVRename;

internal abstract class MissingActionListExporter : ActionListExporter
{
    public MissingActionListExporter(ItemList theActionList) : base(theActionList)
    {
    }
    public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full && TVSettings.Instance.RestrictMissingExportsToFullScans;
}
