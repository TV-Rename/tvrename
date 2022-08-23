//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;
using System.Linq;

namespace TVRename;

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

    public List<Action> Actions => this.OfType<Action>().ToList();

    public List<Item> Checked => this.Where(i=>i.CheckedItem).ToList();

    public List<ItemMissing> Missing => this.OfType<ItemMissing>().ToList();

    public List<ShowItemMissing> MissingEpisodes => this.OfType<ShowItemMissing>().ToList();
    public List<ShowSeasonMissing> MissingSeasons => this.OfType<ShowSeasonMissing>().ToList();
    public List<MovieItemMissing> MissingMovies => this.OfType<MovieItemMissing>().ToList();

    public List<ActionMoveRenameDirectory> MoveRenameDirectories => this.OfType<ActionMoveRenameDirectory>().ToList();

    public List<ActionCopyMoveRename> CopyMove => this.OfType<ActionCopyMoveRename>().Where(a => a.Operation != ActionCopyMoveRename.Op.rename).ToList();

    public List<ActionTDownload> DownloadTorrents => this.OfType<ActionTDownload>().ToList();

    public List<ActionDownloadImage> SaveImages => this.OfType<ActionDownloadImage>().ToList();

    public List<ActionCopyMoveRename> CopyMoveRename => this.OfType<ActionCopyMoveRename>().ToList();

    public List<ItemDownloading> Downloading => this.OfType<ItemDownloading>().ToList();

    public void Replace(IEnumerable<Item>? toRemove, IEnumerable<Item>? newList)
    {
        Remove(toRemove);
        Add(newList);
    }

    public List<Action> TorrentActions => this.Where(a => a is ActionTRemove or ActionTDownload).OfType<Action>().ToList();

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

    public void Replace(IEnumerable<Item> toRemove, Item? newItem)
    {
        if (newItem is not null)
        {
            Replace(toRemove, new List<Item> { newItem });
        }
        else
        {
            Remove(toRemove);
        }
    }
}
