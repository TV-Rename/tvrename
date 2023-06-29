using System.Text;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTString : BTItem
{
    public byte[] Data;

    public BTString(byte[] s) : base(BTChunk.kString)
    {
        Data = s;
    }

    public string AsString() => Encoding.UTF8.GetString(Data);
}
