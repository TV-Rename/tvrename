// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    class RecentM3UExporter : RecentExporter
    {
        public RecentM3UExporter(TVDoc doc) : base(doc)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportRecentM3U;
        protected override string Location() => TVSettings.Instance.ExportRecentM3UTo;

        protected override string GenerateHeader()
        {
            return "#EXTM3U";
        }

        protected override string GenerateRecord(ProcessedEpisode ep, FileInfo file, string name, int length)
        {
            return $"#EXTINF:{length},{file.Name}\r\n{file.UrlPathFullName()}";
        }

        protected override string GenerateFooter()
        {
            return string.Empty;
        }
    }
}
