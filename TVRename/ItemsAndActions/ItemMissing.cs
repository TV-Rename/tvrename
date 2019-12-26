// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;

    public class ItemMissing : Item
    {
        public readonly string TheFileNoExt;
        public readonly string Filename;

        public ItemMissing([NotNull] ProcessedEpisode pe, [NotNull] string whereItShouldBeFolder)
        {
            Episode = pe;
            Filename = TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pe, null, whereItShouldBeFolder.Length));
            TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + Filename;
            DestinationFolder = whereItShouldBeFolder;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ItemMissing missing && string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0;
        }

        public override int Compare([CanBeNull] Item o)
        {
            //return (o is null || miss is null) ? 0 : (this.TheFileNoExt + this.Episode.Name).CompareTo(miss.TheFileNoExt + miss.Episode.Name);
            if (o is null || !(o is ItemMissing miss))
            {
                return 0;
            }

            if (!Episode.Show.ShowName.Equals(miss.Episode.Show.ShowName))
            {
                return string.Compare(Episode.Show.ShowName, miss.Episode.Show.ShowName, StringComparison.Ordinal);
            }

            if (!Episode.AppropriateSeasonNumber.Equals(miss.Episode.AppropriateSeasonNumber))
            {
                return Episode.AppropriateSeasonNumber.CompareTo(miss.Episode.AppropriateSeasonNumber);
            }

            return Episode.AppropriateEpNum.CompareTo(miss.Episode.AppropriateEpNum);
        }

        #endregion

        #region Item Members

        [CanBeNull]
        public override IgnoreItem Ignore => GenerateIgnore(TheFileNoExt);
        
        protected override string DestinationFolder { get; }

        protected override string DestinationFile => Filename;
        [NotNull]
        public override string ScanListViewGroup => "lvgActionMissing";

        [NotNull]
        public override string TargetFolder => new FileInfo(TheFileNoExt).DirectoryName;

        public override int IconNumber => 1;

        #endregion

        public void AddComment(string p0)
        {
            ErrorText += p0;
        }
    }
}
