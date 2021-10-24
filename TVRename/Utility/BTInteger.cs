using System.Windows.Forms;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public class BTInteger : BTItem
    {
        public long Value;

        public BTInteger()
            : base(BTChunk.kInteger)
        {
            Value = 0;
        }

        public BTInteger(long n)
            : base(BTChunk.kInteger)
        {
            Value = n;
        }

        public override string AsText()
        {
            return "Integer=" + Value;
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new("Integer:" + Value);
            tn.Add(n);
        }

        public override void Write(System.IO.Stream sw)
        {
            sw.WriteByte((byte)'i');
            byte[] b = System.Text.Encoding.ASCII.GetBytes(Value.ToString());
            sw.Write(b, 0, b.Length);
            sw.WriteByte((byte)'e');
        }
    }
}
