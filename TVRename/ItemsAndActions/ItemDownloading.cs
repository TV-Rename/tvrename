// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;
using System;

namespace TVRename
{
    public class ItemDownloading : Item
    {
        private readonly IDownloadInformation entry;
        public readonly string DesiredLocationNoExt;

        public override IgnoreItem Ignore => GenerateIgnore(DesiredLocationNoExt);
        public override string ScanListViewGroup => "lvgDownloading";

        public override string DestinationFolder => TargetFolder;
        public override string DestinationFile => entry.FileIdentifier;
        public override string SourceDetails => entry.RemainingText;

        public override int IconNumber { get; }
        public override string TargetFolder => string.IsNullOrEmpty(entry.Destination) ? null : new FileInfo(entry.Destination).DirectoryName;

        public ItemDownloading(IDownloadInformation dl, ProcessedEpisode pe, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            entry = dl;
            IconNumber = tApp switch
            {
                DownloadingFinder.DownloadApp.uTorrent => 2,
                DownloadingFinder.DownloadApp.SABnzbd => 8,
                DownloadingFinder.DownloadApp.qBitTorrent => 10,
                _ => 0
            };
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ItemDownloading torrenting && entry == torrenting.entry;
        }

        public override string Name => "Already Downloading";

        public override int CompareTo(object o)
        {
            if (!(o is ItemDownloading ut))
            {
                return 0;
            }

            if (Episode is null)
            {
                return 1;
            }

            if (ut.Episode is null)
            {
                return -1;
            }

            return string.Compare(DesiredLocationNoExt, ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }
        #endregion
    }
}
