using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class BTList : BTItem
    {
        public readonly List<BTItem> Items;

        public BTList()
            : base(BTChunk.kList)
        {
            Items = new List<BTItem>();
        }

        public override string AsText()
        {
            return "List={" + Items.Select(x => x.AsText()).ToCsv() + "}";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new("List");
            tn.Add(n);
            foreach (BTItem t in Items)
            {
                t.Tree(n.Nodes);
            }
        }

        public override void Write([NotNull] System.IO.Stream sw)
        {
            sw.WriteByte((byte)'l');
            foreach (BTItem i in Items)
            {
                i.Write(sw);
            }

            sw.WriteByte((byte)'e');
        }
    }
}
