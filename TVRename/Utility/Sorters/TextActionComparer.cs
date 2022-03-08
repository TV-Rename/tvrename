using BrightIdeasSoftware;
using JetBrains.Annotations;

namespace TVRename
{
    public class TextActionComparer : ObjectListViewComparer<string>
    {
        public TextActionComparer(int column) : base(column) { }

        protected override string GetValue([NotNull] OLVListItem x, int columnId) => x.SubItems[columnId].Text;
    }
}
