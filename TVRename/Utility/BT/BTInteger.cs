namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTInteger : BTItem
{
    internal readonly long Value;

    public BTInteger(long value)
        : base(BTChunk.kInteger)
    {
        Value = value;
    }
}
