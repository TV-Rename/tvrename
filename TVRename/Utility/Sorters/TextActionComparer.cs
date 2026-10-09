//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;

namespace TVRename;

public class TextActionComparer(int column) : ObjectListViewComparer<string>(column)
{
    protected override string GetValue(OLVListItem x, int columnId) => x.SubItems[columnId].Text;
}
