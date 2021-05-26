using System;
using System.Windows.Forms;

namespace TVRename
{
    public class BTString : BTItem
    {
        public byte[] Data;

        public BTString(string s)
            : base(BTChunk.kString)
        {
            SetString(s);
        }

        public BTString()
            : base(BTChunk.kString)
        {
            Data = new byte[0];
        }

        public void SetString(string s)
        {
            Data = System.Text.Encoding.UTF8.GetBytes(s);
        }

        public override string AsText()
        {
            return "String=" + AsString();
        }

        public string AsString()
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            return enc.GetString(Data);
        }

        public byte[]? StringTwentyBytePiece(int pieceNum)
        {
            byte[] res = new byte[20];
            if (((pieceNum * 20) + 20) > Data.Length)
                return null;

            Array.Copy(Data, pieceNum * 20, res, 0, 20);
            return res;
        }

        public static string CharsToHex(byte[] data, int start, int n)
        {
            string r = string.Empty;
            for (int i = 0; i < n; i++)
                r += (data[start + i] < 16 ? "0" : "") + data[start + i].ToString("x").ToUpper();

            return r;
        }

        public string PieceAsNiceString(int pieceNum)
        {
            return CharsToHex(Data, pieceNum * 20, 20);
        }

        public override void Tree(TreeNodeCollection tn)
        {
            TreeNode n = new TreeNode($"String:{AsString()}");
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
}
