//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal abstract class ActionListExporter(ItemList theActionList) : Exporter
{
    protected readonly ItemList TheActionList = theActionList;

    public abstract bool ApplicableFor(TVSettings.ScanType st);
}
