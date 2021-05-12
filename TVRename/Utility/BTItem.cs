using System.Windows.Forms;

namespace TVRename
{
    public abstract class BTItem
    {
        public readonly BTChunk Type; // from enum

        protected BTItem(BTChunk type)
        {
            Type = type;
        }

        public virtual string AsText()
        {
            return $"Type ={Type}";
        }

        public virtual void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode("BTItem:" + Type);
            tn.Add(n);
        }

        public abstract void Write(System.IO.Stream sw);
    }
}