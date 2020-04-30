// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    public class ItemList : List<Item>
    {
        public void Add([CanBeNull] ItemList slil)
        {
            if (slil is null)
            {
                return;
            }

            foreach (Item sli in slil)
            {
                Add(sli);
            }
        }

        [NotNull]
        public IEnumerable<Action> Actions( ) => this.OfType<Action>().ToList();

        [NotNull]
        public IEnumerable<ItemMissing> MissingItems() => this.OfType<ItemMissing>().ToList();

        [NotNull]
        public IEnumerable<ActionCopyMoveRename> CopyMoveItems() => this.OfType<ActionCopyMoveRename>().ToList();

        public void Replace(ItemList toRemove, ItemList newList)
        {
            Remove(toRemove);
            Add(newList);
        }

        internal void Remove([CanBeNull] IEnumerable<Item> toRemove)
        {
            if (toRemove is null)
            {
                return;
            }

            foreach (Item sli in toRemove)
            {
                Remove(sli);
            }
        }
    }
}
