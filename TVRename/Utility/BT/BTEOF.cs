using System.IO;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTEOF : BTItem
{
    public BTEOF()
        : base(BTChunk.kBTEOF)
    {
    }

    public override void Write(Stream sw)
    {
    }
}
