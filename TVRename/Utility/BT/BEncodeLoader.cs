using System;
using JetBrains.Annotations;

namespace TVRename
{
    public class BEncodeLoader
    {
        [NotNull]
        private static BTItem ReadString([NotNull] System.IO.Stream sr, long length)
        {
            System.IO.BinaryReader br = new(sr);
            return new BTString {Data = br.ReadBytes((int)length) };
        }

        [NotNull]
        private static BTItem ReadInt([NotNull] System.IO.FileStream sr)
        {
            long r = 0;
            int c;
            bool neg = false;
            while ((c = sr.ReadByte()) != 'e')
            {
                switch (c)
                {
                    case '-':
                        neg = true;
                        break;
                    case >= '0' and <= '9':
                        r *= 10;
                        r += c - '0';
                        break;
                }
            }

            if (neg)
            {
                r = -r;
            }

            BTInteger bti = new() { Value = r };
            return bti;
        }

        [NotNull]
        private BTItem ReadDictionary([NotNull] System.IO.FileStream sr)
        {
            BTDictionary d = new();
            for (;;)
            {
                BTItem next = ReadNext(sr);
                if (next.Type == BTChunk.kListOrDictionaryEnd || next.Type == BTChunk.kBTEOF)
                {
                    return d;
                }

                if (next.Type != BTChunk.kString)
                {
                    return new BTError {Message = "Didn't get string as first of pair in dictionary"};
                }

                BTDictionaryItem di = new(((BTString)next).AsString(), ReadNext(sr));
                d.Items.Add(di);
            }
        }

        [NotNull]
        private BTItem ReadList([NotNull] System.IO.FileStream sr)
        {
            BTList ll = new();
            for (;;)
            {
                BTItem next = ReadNext(sr);
                if (next.Type == BTChunk.kListOrDictionaryEnd)
                {
                    return ll;
                }

                ll.Items.Add(next);
            }
        }

        [NotNull]
        private BTItem ReadNext([NotNull] System.IO.FileStream sr)
        {
            if (sr.Length == sr.Position)
            {
                return new BTEOF();
            }

            // Read the next character from the stream to see what is next

            int c = sr.ReadByte();
            switch (c)
            {
                case 'd':
                    return ReadDictionary(sr); // dictionary
                case 'l':
                    return ReadList(sr); // list
                case 'i':
                    return ReadInt(sr); // integer
                case 'e':
                    return new BTListOrDictionaryEnd(); // end of list/dictionary/etc.
                // digits mean it is a string of the specified length
                case >= '0' and <= '9':
                {
                    string r = Convert.ToString(c - '0');
                    while ((c = sr.ReadByte()) != ':')
                    {
                        r += Convert.ToString(c - '0');
                    }

                    return ReadString(sr, Convert.ToInt32(r));
                }
                default:
                {
                    BTError e = new()
                    {
                        Message = $"Error: unknown BEncode item type: {c}"
                    };

                    return e;
                }
            }
        }

        public BTFile? Load(string filename)
        {
            BTFile f = new();

            System.IO.FileStream sr;
            try
            {
                sr = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }

            while (sr.Position < sr.Length)
            {
                f.Items.Add(ReadNext(sr));
            }

            sr.Close();

            return f;
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    }
}