using System.Windows.Forms;

namespace TVRename
{
    public class BTError : BTItem
    {
        public string Message;

        public BTError()
            : base(BTChunk.kError)
        {
            Message = string.Empty;
        }

        public override string AsText()
        {
            return $"Error:{Message}";
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTError:" + Message);
            tn.Add(n);
        }

        public override void Write(System.IO.Stream sw)
        {
        }
    }
}