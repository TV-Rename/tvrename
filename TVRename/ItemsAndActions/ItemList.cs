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
        AddNullableRange(slil);
    }

    public List<Action> Actions => [.. this.OfType<Action>()];

    public List<Item> Checked => [.. this.Where(i => i.CheckedItem)];

    public List<ItemMissing> Missing => [.. this.OfType<ItemMissing>()];

    public List<ShowItemMissing> MissingEpisodes => [.. this.OfType<ShowItemMissing>()];
    public List<ShowSeasonMissing> MissingSeasons => [.. this.OfType<ShowSeasonMissing>()];
    public List<MovieItemMissing> MissingMovies => [.. this.OfType<MovieItemMissing>()];

    public List<ActionMoveRenameDirectory> MoveRenameDirectories => [.. this.OfType<ActionMoveRenameDirectory>()];

    public List<ActionCopyMoveRename> CopyMove => [.. this.OfType<ActionCopyMoveRename>().Where(a => a.Operation != ActionCopyMoveRename.Op.rename)];

    public List<ActionTDownload> DownloadTorrents => [.. this.OfType<ActionTDownload>()];

    public List<ActionDownloadImage> SaveImages => [.. this.OfType<ActionDownloadImage>()];

    public List<ActionCopyMoveRename> CopyMoveRename => [.. this.OfType<ActionCopyMoveRename>()];

    public List<ItemDownloading> Downloading => [.. this.OfType<ItemDownloading>()];

    public void Replace(IEnumerable<Item>? toRemove, IEnumerable<Item>? newList)
    {
        Remove(toRemove);
        AddNullableRange(newList);
    }

    public List<Action> TorrentActions => [.. this.Where(a => a is ActionTRemove or ActionTDownload).OfType<Action>()];

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
            Replace(toRemove, [newItem]);
        }
        else
        {
            Remove(toRemove);
        }
    }

    public IEnumerable<Action> GetActionsForQueue(Action.QueueName queueType)
    {
        return Actions.Where(a=>a.Queue()==queueType);
    }
}
