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
    public sealed class ItemList : List<Item>, INotifyPropertyChanged
    {
        public void Add(IEnumerable<Item>? slil)
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

        public void Replace(IEnumerable<Item>? toRemove, IEnumerable<Item>? newList)
        {
            Remove(toRemove);
            Add(newList);
        }

        public List<Item> TorrentActions => this.Where(a => a is ActionTRemove || a is ActionTDownload).ToList();

        internal void Remove(IEnumerable<Item>? toRemove)
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
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // ReSharper disable once ConstantConditionalAccessQualifier
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Replace(IEnumerable<Item>? toRemove, Item? newItem)
        {
            Replace(toRemove,new List<Item>{newItem});
        }
    }
}
