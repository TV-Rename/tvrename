// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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

        protected override string DestinationFolder => FileIdentifier;
        protected override string DestinationFile => Destination;
        protected override string SourceDetails => Remaining;
        private string FileIdentifier => entry.FileIdentifier;
        private string Destination => entry.Destination;
        private string Remaining => entry.RemainingText;
        public override int IconNumber { get; }
        public override string TargetFolder => string.IsNullOrEmpty(Destination) ? null : new FileInfo(Destination).DirectoryName;

        public ItemDownloading(IDownloadInformation dl, ProcessedEpisode pe, string desiredLocationNoExt, DownloadingFinder.DownloadApp tApp)
        {
            Episode = pe;
            DesiredLocationNoExt = desiredLocationNoExt;
            entry = dl;
            IconNumber = (tApp == DownloadingFinder.DownloadApp.uTorrent) ? 2 :
                         (tApp == DownloadingFinder.DownloadApp.SABnzbd)  ? 8 :
                         (tApp == DownloadingFinder.DownloadApp.qBitTorrent) ? 10 : 0;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemDownloading torrenting) && entry == torrenting.entry;
        }

        public override int Compare(Item o)
        {
            if (!(o is ItemDownloading ut))
            {
                return 0;
            }

            if (Episode == null)
            {
                return 1;
            }

            if (ut.Episode == null)
            {
                return -1;
            }

            return string.Compare((DesiredLocationNoExt), ut.DesiredLocationNoExt, StringComparison.Ordinal);
        }
        #endregion
    }
}
