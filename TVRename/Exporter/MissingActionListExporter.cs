namespace TVRename;

internal abstract class MissingActionListExporter(ItemList theActionList) : ActionListExporter(theActionList)
{
    public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full && TVSettings.Instance.RestrictMissingExportsToFullScans;
}
