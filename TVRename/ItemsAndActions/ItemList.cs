// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public class ItemList : List<Item>
    {
        public void Add(ItemList slil)
        {
            if (slil == null) return;
            foreach (Item sli in slil)
            {
                Add(sli);
            }
        }

        public IEnumerable<Action> Actions( ) => this.OfType<Action>();

        public IEnumerable<ItemMissing> MissingItems() => this.OfType<ItemMissing>();

        public IEnumerable<ActionCopyMoveRename> CopyMoveItems() => this.OfType<ActionCopyMoveRename>();

        public void Replace(ItemList toRemove, ItemList newList)
        {
            Remove(toRemove);
            Add(newList);
        }

        internal void Remove(List<Item> toRemove)
        {
            if (toRemove == null) return;
            foreach (Item sli in toRemove)
            {
                Remove(sli);
            }
        }
    }
}
