// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace TVRename
{
    public class ItemList : List<Item>, INotifyPropertyChanged
    {
        public void Add([CanBeNull] IEnumerable<Item> slil)
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
        public List<Action> Actions => this.OfType<Action>().ToList();

        [NotNull]
        public List<ItemMissing> Missing => this.OfType<ItemMissing>().ToList();

        [NotNull]
        public List<ActionCopyMoveRename> CopyMove => this.OfType<ActionCopyMoveRename>().Where(a=>a.Operation!=ActionCopyMoveRename.Op.rename).ToList();

        [NotNull]
        public List<ActionTDownload> DownloadTorrents => this.OfType<ActionTDownload>().ToList();

        [NotNull]
        public List<ActionDownloadImage> SaveImages => this.OfType<ActionDownloadImage>().ToList();

        [NotNull]
        public List<ActionCopyMoveRename> CopyMoveRename => this.OfType<ActionCopyMoveRename>().ToList();

        public void Replace([CanBeNull] IEnumerable<Item> toRemove, [CanBeNull] IEnumerable<Item> newList)
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

        public void NotifyUpdated()
        {
            OnPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] [CanBeNull] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Replace([CanBeNull] IEnumerable<Item> toRemove, [CanBeNull] Item newItem)
        {
            Replace(toRemove,new List<Item>{newItem});
        }
    }
}
