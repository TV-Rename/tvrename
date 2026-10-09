//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal abstract class MissingActionListExporter(ItemList theActionList) : ActionListExporter(theActionList)
{
    public override bool ApplicableFor(TVSettings.ScanType st) => st == TVSettings.ScanType.Full && TVSettings.Instance.RestrictMissingExportsToFullScans;
}
