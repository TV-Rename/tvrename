//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

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
