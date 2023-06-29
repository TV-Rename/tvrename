using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTInteger : BTItem
{
    internal readonly long Value;

    public BTInteger(long value)
        : base(BTChunk.kInteger)
    {
        Value = value;
    }

    public override string AsText() => "Integer=" + Value;

    public override void Tree(TreeNodeCollection tn)
    {
        TreeNode n = new("Integer:" + Value);
        tn.Add(n);
    }

    public override void Write(Stream sw)
    {
        sw.WriteByte((byte)'i');
        byte[] b = Encoding.ASCII.GetBytes(Value.ToString());
        sw.Write(b, 0, b.Length);
        sw.WriteByte((byte)'e');
    }
}
