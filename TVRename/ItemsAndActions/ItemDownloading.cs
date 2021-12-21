//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using JetBrains.Annotations;

namespace TVRename
{
    public class ItemDownloading : Item
    {
        private readonly IDownloadInformation entry;
        private readonly string desiredLocationNoExt;

        public override IgnoreItem? Ignore => GenerateIgnore(desiredLocationNoExt);
        [NotNull]
        public override string ScanListViewGroup => "lvgDownloading";

        public override string? DestinationFolder => TargetFolder;
        public override string DestinationFile => entry.FileIdentifier;
        public override string SourceDetails => entry.RemainingText;

        public override int IconNumber { get; }
        public override string? TargetFolder => string.IsNullOrEmpty(entry.Destination) ? null : new FileInfo(entry.Destination).DirectoryName;

        private ItemDownloading(IDownloadInformation dl, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing undoItem)
        {
            this.desiredLocationNoExt = desiredLocationNoExt;
            entry = dl;
            UndoItemMissing = undoItem;
            IconNumber = tApp switch
            {
                DownloadingFinder.DownloadApp.uTorrent => 2,
                DownloadingFinder.DownloadApp.SABnzbd => 8,
                DownloadingFinder.DownloadApp.qBitTorrent => 10,
                _ => 0
            };
        }

        public ItemDownloading(IDownloadInformation dl, ProcessedEpisode pe, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing me) : this(dl, desiredLocationNoExt, tApp, me)
        {
            Episode = pe;
        }

        public ItemDownloading(IDownloadInformation dl, MovieConfiguration mc, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp, ItemMissing me) : this(dl, desiredLocationNoExt, tApp, me)
        {
            Episode = null;
            Movie = mc;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ItemDownloading torrenting && entry == torrenting.entry;
        }

        [NotNull]
        public override string Name => "Already Downloading";

        public override bool CheckedItem { get => false; set { }  }

        public override int CompareTo(Item o)
        {
            if (o is not ItemDownloading ut)
            {
                return -1;
            }

            if (Episode is null)
            {
                return 1;
            }

            if (ut.Episode is null)
            {
                return -1;
            }

            return string.Compare(desiredLocationNoExt, ut.desiredLocationNoExt, StringComparison.Ordinal);
        }

        #endregion Item Members
    }
}
