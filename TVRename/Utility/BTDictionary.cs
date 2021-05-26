using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class BTDictionary : BTItem
    {
        public readonly List<BTDictionaryItem> Items;

        public BTDictionary()
            : base(BTChunk.kDictionary)
        {
            Items = new List<BTDictionaryItem>();
        }

        public override string AsText()
        {
            return "Dictionary=[" + Items.Select(x => x.AsText()).ToCsv() + "]";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("Dictionary");
            tn.Add(n);
            foreach (BTDictionaryItem t in Items)
            {
                t.Tree(n.Nodes);
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public bool RemoveItem(string key)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == key)
                {
                    Items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public BTItem? GetItem(string key) => GetItem(key, false);

        public BTItem? GetItem(string key, bool ignoreCase)
        {
            foreach (BTDictionaryItem t in Items)
            {
                if ((t.Key == key) || (ignoreCase && ((t.Key.ToLower() == key.ToLower()))))
                    return t.Data;
            }

            return null;
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte)'d');
            foreach (BTDictionaryItem i in Items)
            {
                i.Write(sw);
            }

            sw.WriteByte((byte)'e');
        }
    }
}
