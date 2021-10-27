using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public abstract class BTItem
    {
        public readonly BTChunk Type; // from enum

        protected BTItem(BTChunk type)
        {
            Type = type;
        }

        [NotNull]
        public virtual string AsText()
        {
            return $"Type ={Type}";
        }

        public virtual void Tree([NotNull] TreeNodeCollection tn)
        {
            TreeNode n = new("BTItem:" + Type);
            tn.Add(n);
        }

        public abstract void Write(System.IO.Stream sw);
    }
}
