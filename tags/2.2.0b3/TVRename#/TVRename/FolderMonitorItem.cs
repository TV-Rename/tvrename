// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    public enum FolderModeEnum
    {
        kfmFlat,
        kfmFolderPerSeason,
        kfmSpecificSeason
    }

    // "FolderMonitorItem" represents a folder in the Folder Monitor, where the user is entering what show+season it is
    public class FolderMonitorItem
    {
        public string Folder;
        public FolderModeEnum FolderMode;
        public string ShowName;
        public int SpecificSeason;
        public SeriesInfo TheSeries;

        public FolderMonitorItem(string folder, FolderModeEnum folderMode, int season)
        {
            this.Folder = folder;
            this.SpecificSeason = season;
            this.FolderMode = folderMode;
            this.TheSeries = null;
        }
    }
}