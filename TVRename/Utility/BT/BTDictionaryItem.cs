namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTDictionaryItem(string k, BTItem d) : BTItem(BTChunk.kDictionaryItem)
{
    public readonly BTItem Data = d;
    public readonly string Key = k;
}
