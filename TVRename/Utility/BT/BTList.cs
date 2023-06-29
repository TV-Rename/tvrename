using System.Collections.Generic;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public class BTList : BTItem
{
    public readonly List<BTItem> Items;

    public BTList()
        : base(BTChunk.kList)
    {
        Items = new List<BTItem>();
    }
}
