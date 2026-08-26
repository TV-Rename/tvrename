using System.Text;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTString(byte[] s) : BTItem(BTChunk.kString)
{
    public byte[] Data = s;

    public string AsString() => Encoding.UTF8.GetString(Data);
}
