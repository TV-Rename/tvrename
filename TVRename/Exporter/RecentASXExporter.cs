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
    internal class RecentASXExporter : RecentExporter
    {
        public RecentASXExporter(TVDoc doc) : base(doc)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportRecentASX;
        protected override string Location() => TVSettings.Instance.ExportRecentASXTo;

        [NotNull]
        protected override string GenerateHeader() => "<ASX version=\"3\">";

        [NotNull]
        protected override string GenerateRecord(ProcessedEpisode ep, [NotNull] FileInfo file, string name, int length)
        {
            string filen = System.Security.SecurityElement.Escape(file.UrlPathFullName());
            return $"<Entry>\r\n<ref href=\"{filen}\" />\r\n</Entry><title>{System.Security.SecurityElement.Escape(name)}</title>";
        }

        [NotNull]
        protected override string GenerateFooter() => "</ASX>";
    }
}
