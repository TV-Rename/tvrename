//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    public sealed class ItemList : SafeList<Item>
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
        public List<Item> Checked => this.Where(i=>i.CheckedItem).ToList();

        [NotNull]
        public List<ItemMissing> Missing => this.OfType<ItemMissing>().ToList();

        [NotNull]
        public List<ShowItemMissing> MissingEpisodes => this.OfType<ShowItemMissing>().ToList();
        [NotNull]
        public List<MovieItemMissing> MissingMovies => this.OfType<MovieItemMissing>().ToList();

        [NotNull]
        public List<ActionMoveRenameDirectory> MoveRenameDirectories => this.OfType<ActionMoveRenameDirectory>().ToList();

        [NotNull]
        public List<ActionCopyMoveRename> CopyMove => this.OfType<ActionCopyMoveRename>().Where(a => a.Operation != ActionCopyMoveRename.Op.rename).ToList();

        [NotNull]
        public List<ActionTDownload> DownloadTorrents => this.OfType<ActionTDownload>().ToList();

        [NotNull]
        public List<ActionDownloadImage> SaveImages => this.OfType<ActionDownloadImage>().ToList();

        [NotNull]
        public List<ActionCopyMoveRename> CopyMoveRename => this.OfType<ActionCopyMoveRename>().ToList();

        [NotNull]
        public List<ItemDownloading> Downloading => this.OfType<ItemDownloading>().ToList();

        public void Replace(IEnumerable<Item>? toRemove, IEnumerable<Item>? newList)
        {
            Remove(toRemove);
            Add(newList);
        }

        [NotNull]
        public List<Action> TorrentActions => this.Where(a => a is ActionTRemove || a is ActionTDownload).OfType<Action>().ToList();

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

        public void Replace(IEnumerable<Item>? toRemove, Item? newItem)
        {
            Replace(toRemove, new List<Item> { newItem });
        }
    }
}
