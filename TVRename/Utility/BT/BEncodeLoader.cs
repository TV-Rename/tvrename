using System;
using System.IO;
using NLog;

namespace TVRename;

public static class BEncodeLoader
{
    private static BTItem ReadString(this FileStream sr, long length)
    {
        return new BTString(sr.ReadBytes(length) );
    }

    private static byte[] ReadBytes(this FileStream sr, long length)
    {
        byte[] arrfile = new byte[length];

        for (int i = 0; i < arrfile.Length; i++)
        {
            arrfile[i] = Convert.ToByte(sr.ReadByte());
        }

        return arrfile;
    }

    private static BTItem ReadInt(this FileStream sr)
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

        return new BTInteger( neg ?  -r : r);
    }

    private static BTItem ReadDictionary(this FileStream sr)
    {
        BTDictionary d = new();
        for (; ; )
        {
            BTItem next = ReadNext(sr);
            if (next.Type == BTChunk.kListOrDictionaryEnd || next.Type == BTChunk.kBTEOF)
            {
                return d;
            }

            if (next.Type != BTChunk.kString)
            {
                return new BTError( "Didn't get string as first of pair in dictionary" );
            }

            BTDictionaryItem di = new(((BTString)next).AsString(), ReadNext(sr));
            d.Items.Add(di);
        }
    }

    private static BTItem ReadList(this FileStream sr)
    {
        BTList ll = new();
        for (; ; )
        {
            BTItem next = sr.ReadNext();
            if (next.Type == BTChunk.kListOrDictionaryEnd)
            {
                return ll;
            }

            ll.Items.Add(next);
        }
    }

    private static BTItem ReadNext(this FileStream sr)
    {
        if (sr.Length == sr.Position)
        {
            return new BTEOF();
        }

        // Read the next character from the stream to see what is next

        int c = sr.ReadByte();
        return c switch
        {
            'd' => sr.ReadDictionary(), // dictionary
            'l' => sr.ReadList(), // list
            'i' => sr.ReadInt(), // integer
            'e' => new BTListOrDictionaryEnd(), // end of list/dictionary/etc.
            >= '0' and <= '9' => sr.ReadString(GetStringLength(sr, c)),// digits mean it is a string of the specified length
            _ => new BTError($"Error: unknown BEncode item type: {c}")
        };
    }

    private static int GetStringLength(this FileStream sr, int c)
    {
        //We have already read the first digit, so seed the return value with it
        string r = Convert.ToString(c - '0');
        while ((c = sr.ReadByte()) != ':')
        {
            r += Convert.ToString(c - '0');
        }

        return Convert.ToInt32(r);
    }

    public static BTFile? Load(string filename)
    {
        try
        {
            BTFile f = new();
            using FileStream sr = new(filename, FileMode.Open, FileAccess.Read);
            while (sr.Position < sr.Length)
            {
                f.Items.Add(sr.ReadNext());
            }
            sr.Close();
            return f;
        }
        catch (IOException e)
        {
            Logger.Warn(e.Message);
            return null;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return null;
        }
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
}
