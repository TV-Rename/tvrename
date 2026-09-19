namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTInteger(long value) : BTItem(BTChunk.kInteger)
{
    internal readonly long Value = value;
}
