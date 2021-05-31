using System;

namespace TVRename
{
    public class BEncodeLoader
    {
        public BTItem ReadString(System.IO.Stream sr, long length)
        {
            System.IO.BinaryReader br = new System.IO.BinaryReader(sr);

            byte[] c = br.ReadBytes((int)length);

            BTString bts = new BTString();
            bts.Data = c;
            return bts;
        }

        public static BTItem ReadInt(System.IO.FileStream sr)
        {
            long r = 0;
            int c;
            bool neg = false;
            while ((c = sr.ReadByte()) != 'e')
            {
                if (c == '-')
                {
                    neg = true;
                }
                else if ((c >= '0') && (c <= '9'))
                {
                    r = (r * 10) + c - '0';
                }
            }

            if (neg)
            {
                r = -r;
            }

            BTInteger bti = new BTInteger { Value = r };
            return bti;
        }

        public BTItem ReadDictionary(System.IO.FileStream sr)
        {
            BTDictionary d = new BTDictionary();
            for (; ; )
            {
                BTItem next = ReadNext(sr);
                if ((next.Type == BTChunk.kListOrDictionaryEnd) || (next.Type == BTChunk.kBTEOF))
                {
                    return d;
                }

                if (next.Type != BTChunk.kString)
                {
                    BTError e = new BTError();
                    e.Message = "Didn't get string as first of pair in dictionary";
                    return e;
                }

                BTDictionaryItem di = new BTDictionaryItem(((BTString)next).AsString(), ReadNext(sr));
                d.Items.Add(di);
            }
        }

        public BTItem ReadList(System.IO.FileStream sr)
        {
            BTList ll = new BTList();
            for (; ; )
            {
                BTItem next = ReadNext(sr);
                if (next.Type == BTChunk.kListOrDictionaryEnd)
                {
                    return ll;
                }

                ll.Items.Add(next);
            }
        }

        public BTItem ReadNext(System.IO.FileStream sr)
        {
            if (sr.Length == sr.Position)
            {
                return new BTEOF();
            }

            // Read the next character from the stream to see what is next

            int c = sr.ReadByte();
            if (c == 'd')
            {
                return ReadDictionary(sr); // dictionary
            }

            if (c == 'l')
            {
                return ReadList(sr); // list
            }

            if (c == 'i')
            {
                return ReadInt(sr); // integer
            }

            if (c == 'e')
            {
                return new BTListOrDictionaryEnd(); // end of list/dictionary/etc.
            }

            if ((c >= '0') && (c <= '9')) // digits mean it is a string of the specified length
            {
                string r = Convert.ToString(c - '0');
                while ((c = sr.ReadByte()) != ':')
                {
                    r += Convert.ToString(c - '0');
                }

                return ReadString(sr, Convert.ToInt32(r));
            }

            BTError e = new BTError
            {
                Message = $"Error: unknown BEncode item type: {c}"
            };

            return e;
        }

        public BTFile? Load(string filename)
        {
            BTFile f = new BTFile();

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
