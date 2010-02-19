using System.IO;
using System;


namespace TVRename
{
    public class DirCacheEntry
    {
        public FileInfo TheFile;
        public string SimplifiedFullName;
        public string LowerName;
        public bool HasUsefulExtension_NotOthersToo;
        public bool HasUsefulExtension_OthersToo;
        public Int64 Length;

        public DirCacheEntry(FileInfo f, TVSettings theSettings)
        {
            TheFile = f;
            SimplifiedFullName = Helpers.SimplifyName(f.FullName);
            LowerName = f.Name.ToLower();
            Length = f.Length;
            if (theSettings != null)
            {
                HasUsefulExtension_NotOthersToo = theSettings.UsefulExtension(f.Extension, false);
                HasUsefulExtension_OthersToo = HasUsefulExtension_NotOthersToo | theSettings.UsefulExtension(f.Extension, true);
            }
        }
    }
    public class DirCacheList : System.Collections.Generic.List<DirCacheEntry>
    {
    }
public class DirCache
{
   

		public static int CountFiles(string folder, bool subFolders)
		{
			int n = 0;
			if (!Directory.Exists(folder))
				return n;
			if (folder.Length >= 248)
				return n;
			try
			{
				DirectoryInfo di = new DirectoryInfo(folder);
				n = di.GetFiles().Length;

				if (subFolders)
				{
					DirectoryInfo[] dirs = di.GetDirectories();
					foreach (DirectoryInfo di in dirs)
						n += CountFiles(di.FullName, subFolders);
				}
			}
			catch (UnauthorizedAccessException )
			{
			}
			return n;
		}

		public static int BuildDirCache(SetProgressDelegate prog, int initialCount, int totalFiles, DirCacheList fileCache, string folder, bool subFolders, TVSettings theSettings)
		{
			int filesDone = initialCount;

			if (!Directory.Exists(folder))
			{
				System.Windows.Forms.DialogResult res = MessageBox.Show("The search folder \"" + folder + " does not exist.\n", "Folder does not exist",MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			try
			{
				if (folder.Length >= 248)
				{
					MessageBox.Show("Skipping folder that has a name longer than the Windows permitted 247 characters: " + folder, "Path name too long", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return filesDone;
				}

				DirectoryInfo di = new DirectoryInfo(folder);
				if (!di.Exists)
					return filesDone;

				//                DirectorySecurity ^ds = di->GetAccessControl();


				FileInfo[] f2 = di.GetFiles();
				foreach (FileInfo ff in f2)
				{
					filesDone++;
					if ((ff.Name.Length + folder.Length) >= 260)
					{
						MessageBox.Show("Skipping file that has a path+name longer than the Windows permitted 259 characters: " + ff.Name + " in " + folder, "File+Path name too long", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
						fileCache.Add(new DirCacheEntry(ff, theSettings));
						if (prog != null)
				prog.Invoke(100*(filesDone) / totalFiles);
				}

				if (subFolders)
				{
					DirectoryInfo[] dirs = di.GetDirectories();
					foreach (DirectoryInfo di2 in dirs)
						filesDone = BuildDirCache(prog,filesDone,totalFiles,fileCache, di2.FullName, subFolders, theSettings);
				}
			}
			catch (UnauthorizedAccessException )
			{
			}
			catch
			{
			}
			return filesDone;
		}
}

} // namespace