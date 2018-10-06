// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using Alphaleonis.Win32.Filesystem;
    using System.Windows.Forms;

    public class ItemMissing : Item
    {
        public readonly string TheFileNoExt;
        private readonly string folder;
        public readonly string Filename;

        public ItemMissing(ProcessedEpisode pe, string whereItShouldBeFolder, string expectedFilenameNoExt)
        {
            Episode = pe;
            TheFileNoExt = whereItShouldBeFolder + System.IO.Path.DirectorySeparatorChar + expectedFilenameNoExt;
            folder = whereItShouldBeFolder;
            Filename = expectedFilenameNoExt;
        }

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ItemMissing missing) && (string.CompareOrdinal(missing.TheFileNoExt, TheFileNoExt) == 0);
        }

        public override int Compare(Item o)
        {
            //return (o == null || miss == null) ? 0 : (this.TheFileNoExt + this.Episode.Name).CompareTo(miss.TheFileNoExt + miss.Episode.Name);
            if (o == null || !(o is ItemMissing miss))
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

        public override IgnoreItem Ignore => GenerateIgnore(TheFileNoExt);
        
        protected override string SeriesName => Episode.Show.ShowName;
        protected override string SeasonNumber => Episode.AppropriateSeasonNumber.ToString();
        protected override string EpisodeNumber => Episode.NumsAsString();
        protected override string AirDate => Episode.GetAirDateDT(true).PrettyPrint();
        protected override string DestinationFolder => folder;
        protected override string DestinationFile => Filename;
        protected override string SourceDetails => string.Empty;
        public override string ScanListViewGroup => "lvgActionMissing";

        public override string TargetFolder => string.IsNullOrEmpty(TheFileNoExt) ? null : new FileInfo(TheFileNoExt).DirectoryName;

        public override int IconNumber => 1;

        #endregion
    }
}
