namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTDictionaryItem : BTItem
{
    public readonly BTItem Data;
    public readonly string Key;

    public BTDictionaryItem(string k, BTItem d)
        : base(BTChunk.kDictionaryItem)
    {
        Key = k;
        Data = d;
    }
}
