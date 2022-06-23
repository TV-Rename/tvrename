using System;
using System.Windows.Forms;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTString : BTItem
{
    public byte[] Data;

    public BTString(string s) : base(BTChunk.kString)
    {
        Data = System.Text.Encoding.UTF8.GetBytes(s);
    }

    public BTString() : base(BTChunk.kString)
    {
        Data = Array.Empty<byte>();
    }

    public override string AsText() => "String=" + AsString();

    public string AsString() => System.Text.Encoding.UTF8.GetString(Data);

    public static string CharsToHex(byte[] data, int start, int n)
    {
        string r = string.Empty;
        for (int i = 0; i < n; i++)
        {
            r += (data[start + i] < 16 ? "0" : "") + data[start + i].ToString("x").ToUpper();
        }

        return r;
    }

    public string PieceAsNiceString(int pieceNum) => CharsToHex(Data, pieceNum * 20, 20);

    public override void Tree(TreeNodeCollection tn)
    {
        TreeNode n = new($"String:{AsString()}");
        tn.Add(n);
    }

    public override void Write(System.IO.Stream sw)
    {
        // Byte strings are encoded as follows: <string length encoded in base ten ASCII>:<string data>

        byte[] len = System.Text.Encoding.ASCII.GetBytes(Data.Length.ToString());
        sw.Write(len, 0, len.Length);
        sw.WriteByte((byte)':');
        sw.Write(Data, 0, Data.Length);
    }
}
