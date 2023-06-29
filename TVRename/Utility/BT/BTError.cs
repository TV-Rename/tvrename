using System.IO;
using System.Windows.Forms;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTError : BTItem
{
    public readonly string Message;

    public BTError(string message) : base(BTChunk.kError)
    {
        Message = message;
    }

    public override string AsText() => $"Error:{Message}";

    public override void Tree(TreeNodeCollection tn)
    {
        TreeNode n = new("BTError:" + Message);
        tn.Add(n);
    }

    public override void Write(Stream sw)
    {
    }
}
