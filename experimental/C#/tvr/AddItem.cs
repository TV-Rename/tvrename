//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

using System.Collections;
using System;

namespace TVRename
{
	public enum FolderModeEnum: int
	{
		kfmFlat,
		kfmFolderPerSeason,
		kfmSpecificSeason
	}

	public class AddItem
	{
		// represents a folder in the Folder Monitor, where the user is entering what show+season it is
		public string Folder;
		public string ShowName;
		public SeriesInfo TheSeries;
		public FolderModeEnum FolderMode;
		public int SpecificSeason;

		public AddItem(string folder, FolderModeEnum folderMode, int season)
		{
			Folder = folder;
			SpecificSeason = season;
			FolderMode = folderMode;
			TheSeries = null;
		}
	}


} // namespace