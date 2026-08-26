using BrightIdeasSoftware;

namespace TVRename;

public class TextActionComparer(int column) : ObjectListViewComparer<string>(column)
{
    protected override string GetValue(OLVListItem x, int columnId) => x.SubItems[columnId].Text;
}
