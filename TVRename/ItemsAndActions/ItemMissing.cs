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

        public ProcessedEpisode MissingEpisode =>Episode ?? throw new InvalidOperationException();

        public override bool SameAs(Item o)
        {
            return o is ItemMissing missing && string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0;
        }

        public override string Name => "Missing Episode";

        public override int CompareTo(object? o)
        {
            if (o is null || !(o is ItemMissing miss))
            {
                return -1;
            }

            if (!MissingEpisode.Show.ShowName.Equals(miss.MissingEpisode.Show.ShowName))
            {
                return string.Compare(MissingEpisode.Show.ShowName, miss.MissingEpisode.Show.ShowName, StringComparison.Ordinal);
            }

            if (!MissingEpisode.AppropriateSeasonNumber.Equals(miss.MissingEpisode.AppropriateSeasonNumber))
            {
                return MissingEpisode.AppropriateSeasonNumber.CompareTo(miss.MissingEpisode.AppropriateSeasonNumber);
            }

            return MissingEpisode.AppropriateEpNum.CompareTo(miss.MissingEpisode.AppropriateEpNum);
        }

        #endregion

        #region Item Members

        public override IgnoreItem? Ignore => GenerateIgnore(TheFileNoExt);

        public override string DestinationFolder { get; }

        public override string DestinationFile => Filename;
        public override string ScanListViewGroup => "lvgActionMissing";

        public override string TargetFolder => new FileInfo(TheFileNoExt).DirectoryName;

        public override int IconNumber => 1;

        #endregion

        public void AddComment(string p0)
        {
            ErrorText += p0;
        }
    }

    public class MovieItemMissing : Item
    {
        public readonly string TheFileNoExt;
        public readonly string Filename;
        private readonly MovieConfiguration config;

        public MovieItemMissing([NotNull] MovieConfiguration movie, [NotNull] string whereItShouldBeFolder)
        {
            Episode = null;
            Filename = TVSettings.Instance.FilenameFriendly(CustomMovieName.NameFor(movie,TVSettings.Instance.MovieFilenameFormat));
            TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + Filename;
            DestinationFolder = whereItShouldBeFolder;
            config = movie;
        }

        #region Item Members


        public override bool SameAs(Item o)
        {
            return o is MovieItemMissing missing && string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0;
        }

        public override string Name => "Missing Movie";

        public override int CompareTo(object? o)
        {
            if (o is null || !(o is MovieItemMissing miss))
            {
                return -1;
            }

            return String.Compare(TheFileNoExt, miss.TheFileNoExt, StringComparison.Ordinal);
        }

        #endregion

        #region Item Members

        public override IgnoreItem? Ignore => GenerateIgnore(TheFileNoExt);

        public override string DestinationFolder { get; }

        public override string DestinationFile => Filename;
        public override string ScanListViewGroup => "lvgActionMissing";

        public override string TargetFolder => new FileInfo(TheFileNoExt).DirectoryName;

        public override int IconNumber => 1;

        public MovieConfiguration MovieConfig => config;
        #endregion
    }

}
