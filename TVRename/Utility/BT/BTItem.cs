namespace TVRename;

// ReSharper disable once InconsistentNaming
public abstract class BTItem
{
    public readonly BTChunk Type; // from enum

    protected BTItem(BTChunk type)
    {
        Type = type;
    }
}
