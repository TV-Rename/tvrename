// 
// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    class RecentXSPFExporter : RecentExporter
    {
        public RecentXSPFExporter(TVDoc doc) : base(doc)
        {
        }

        public override bool Active() => TVSettings.Instance.ExportRecentXSPF;
        protected override string Location() => TVSettings.Instance.ExportRecentXSPFTo;

        protected override string GenerateHeader()
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<playlist xmlns=\"http://xspf.org/ns/0/\" version=\"1\">\r\n\t<title>Playlist</title>\r\n\t<trackList>";
        }

        protected override string GenerateRecord(ProcessedEpisode ep, FileInfo fileLocation, string name, string image, int length)
        {
            return $"\t\t<track>\r\n\t\t\t<location>{fileLocation.URLPathFullName()}</location>\r\n\t\t\t<title>{name}</title>\r\n\t\t\t<image>{image}</image>\r\n\t\t</track>";
        }

        protected override string GenerateFooter()
        {
            return "\t</trackList>\r\n</playlist>";
        }
    }
}
