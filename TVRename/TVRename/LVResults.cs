// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename
{
    public class LvResults
    {
        #region WhichResults enum

        public enum WhichResults
        {
            Checked,
            selected,
            all
        }

        #endregion

        public int Count;
        public ItemList FlatList;
        public List<ActionCopyMoveRename> CopyMove;
        public List<ActionCopyMoveRename> Rename;
        public List<ActionDownloadImage> SaveImages;
        public List<ActionWriteMetadata> WriteMetadatas;
        public List<ActionFileMetaData> ModifyMetadatas;
        public List<ActionTDownload> DownloadTorrents;
        public List<ActionDelete> Deletes;
        public List<ItemMissing> Missing;
        public List<ItemDownloading> Downloading;

        public LvResults([NotNull] ListView lv, bool isChecked) // if not checked, then selected items
        {
            Go(lv, isChecked ? WhichResults.Checked : WhichResults.selected);
        }

        public LvResults([NotNull] ListView lv, WhichResults which)
        {
            Go(lv, which);
        }

        private void Go([NotNull] ListView lv, WhichResults which)
        {
            Missing = new List<ItemMissing>();
            WriteMetadatas = new List<ActionWriteMetadata>();
            CopyMove = new List<ActionCopyMoveRename>();
            Rename = new List<ActionCopyMoveRename>();
            SaveImages = new List<ActionDownloadImage>();
            DownloadTorrents = new List<ActionTDownload>();
            Downloading = new List<ItemDownloading>();
            ModifyMetadatas = new List<ActionFileMetaData>();
            Deletes = new List<ActionDelete>();
            FlatList = new ItemList();

            List<ListViewItem> sel = new List<ListViewItem>();
            sel.AddRange(GetSelectionCollection(lv, which));

            Count = sel.Count;

            if (sel.Count == 0)
            {
                return;
            }

            foreach (ListViewItem lvi in sel)
            {
                if (lvi is null)
                {
                    continue;
                }

                Item action = (Item)lvi.Tag;
                if (action != null)
                {
                    FlatList.Add(action);
                }

                switch (action)
                {
                    case ActionCopyMoveRename cmr when cmr.Operation == ActionCopyMoveRename.Op.rename:
                        Rename.Add(cmr);
                        break;
                    // copy/move
                    case ActionCopyMoveRename cmr:
                        CopyMove.Add(cmr);
                        break;
                    case ActionDownloadImage item:
                        SaveImages.Add(item);
                        break;
                    case ActionTDownload rss:
                        DownloadTorrents.Add(rss);
                        break;
                    case ItemMissing missing:
                        Missing.Add(missing);
                        break;
                    case ActionWriteMetadata nfo:
                        WriteMetadatas.Add(nfo);
                        break;
                    case ActionFileMetaData meta:
                        ModifyMetadatas.Add(meta);
                        break;
                    case ActionDelete delete:
                        Deletes.Add(delete);
                        break;
                    case ItemDownloading down:
                        Downloading.Add(down);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected value of action " + action?.GetType());
                }
            }
        }

        [NotNull]
        private static IEnumerable<ListViewItem> GetSelectionCollection([NotNull] ListView lv, WhichResults which)
        {
            switch (which)
            {
                case WhichResults.Checked:
                    return lv.CheckedItems.Cast<ListViewItem>();
                case WhichResults.selected:
                    return lv.SelectedItems.Cast<ListViewItem>();
                case WhichResults.all:
                    return lv.Items.Cast<ListViewItem>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(which), which, null);
            }
        }
    }
}
