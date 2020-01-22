// 
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
    internal class RecentXSPFExporter : RecentExporter
    {
        public RecentXSPFExporter(TVDoc doc) : base(doc)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportRecentXSPF;
        protected override string Location() => TVSettings.Instance.ExportRecentXSPFTo;

        [NotNull]
        protected override string GenerateHeader()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<playlist xmlns=\"http://xspf.org/ns/0/\" version=\"1\">\r\n\t<title>Playlist</title>\r\n\t<trackList>";
        }

        [NotNull]
        protected override string GenerateRecord(ProcessedEpisode ep, [NotNull] FileInfo fileLocation, string name, int length)
        {
            string file = System.Security.SecurityElement.Escape(fileLocation.UrlPathFullName());
            return $"\t\t<track>\r\n\t\t\t<location>{file}</location>\r\n\t\t\t<title>{System.Security.SecurityElement.Escape(name)}</title>\r\n\t\t</track>";
        }

        [NotNull]
        protected override string GenerateFooter() => "\t</trackList>\r\n</playlist>";
    }
}
