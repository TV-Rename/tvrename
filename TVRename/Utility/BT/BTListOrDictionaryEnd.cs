using System.IO;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTListOrDictionaryEnd : BTItem
{
    public BTListOrDictionaryEnd()
        : base(BTChunk.kListOrDictionaryEnd)
    {
    }

    public override void Write(Stream sw)
    {
        sw.WriteByte((byte)'e');
    }
}
