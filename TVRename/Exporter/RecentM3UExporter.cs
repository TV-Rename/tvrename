//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    internal class RecentM3UExporter : RecentExporter
    {
        public RecentM3UExporter(TVDoc doc) : base(doc)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportRecentM3U;

        protected override string Location() => TVSettings.Instance.ExportRecentM3UTo;
        [NotNull]
        protected override string Name() => "Recent M3U Exporter";

        [NotNull]
        protected override string GenerateHeader() => "#EXTM3U";

        [NotNull]
        protected override string GenerateRecord(ProcessedEpisode ep, [NotNull] FileInfo file, string name, int length)
        {
            return $"#EXTINF:{length},{file.Name}\r\n{file.UrlPathFullName()}";
        }

        [NotNull]
        protected override string GenerateFooter() => string.Empty;
    }
}
