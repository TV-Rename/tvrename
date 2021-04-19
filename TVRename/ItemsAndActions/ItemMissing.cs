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

    public abstract class ItemMissing : Item
    {
        public string TheFileNoExt;
        public string Filename;
        protected string Folder;

        public override string DestinationFile => Filename;
        public override string ScanListViewGroup => "lvgActionMissing";
        public override string DestinationFolder => Folder;
        public override string TargetFolder => new FileInfo(TheFileNoExt).DirectoryName;
        public override int IconNumber => 1;
        public abstract bool DoRename { get; }
        public abstract MediaConfiguration Show { get; }
        public override IgnoreItem? Ignore => GenerateIgnore(TheFileNoExt);
        public void AddComment(string p0)
        {
            ErrorText += p0;
        }
    }

    public class ShowItemMissing : ItemMissing
    {
        public ShowItemMissing([NotNull] ProcessedEpisode pe, [NotNull] string whereItShouldBeFolder)
        {
            Episode = pe;
            Filename = TVSettings.Instance.FilenameFriendly(TVSettings.Instance.NamingStyle.NameFor(pe, null, whereItShouldBeFolder.Length));
            TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + Filename;
            Folder = whereItShouldBeFolder;
        }
        #region Item Members

        public ProcessedEpisode MissingEpisode =>Episode ?? throw new InvalidOperationException();

        public override bool SameAs(Item o)
        {
            return o is ShowItemMissing missing && string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0;
        }
       
        public override string Name => "Missing Episode";

        public override int CompareTo(object? o)
        {
            if (!(o is ShowItemMissing miss))
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

        public override bool DoRename => Episode?.Show.DoRename ?? true;

        public override MediaConfiguration Show => MissingEpisode.Show;
        public override string ToString() => $"{Show.ShowName} {MissingEpisode}";
    }
}
