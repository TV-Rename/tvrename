using System.Windows.Forms;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class BTDictionaryItem : BTItem
    {
        public BTItem Data;
        public string Key;

        public BTDictionaryItem(string k, BTItem d)
            : base(BTChunk.kDictionaryItem)
        {
            Key = k;
            Data = d;
        }

        public override string AsText()
        {
            if (Key == "pieces" && Data.Type == BTChunk.kString)
            {
                return "<File hash data>";
            }

            return $"{Key}=>{Data.AsText()}";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            if (Key == "pieces" && Data.Type == BTChunk.kString)
            {
                // 20 byte chunks of SHA1 hash values
                TreeNode n = new("Key=" + Key);
                tn.Add(n);
                n.Nodes.Add(new TreeNode("<File hash data>" + ((BTString)Data).PieceAsNiceString(0)));
            }
            else
            {
                TreeNode n = new("Key=" + Key);
                tn.Add(n);
                Data.Tree(n.Nodes);
            }
        }

        public override void Write(System.IO.Stream sw)
        {
            new BTString(Key).Write(sw);
            Data.Write(sw);
        }
    }
}
