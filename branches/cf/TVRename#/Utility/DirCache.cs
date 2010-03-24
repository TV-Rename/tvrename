//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

// Recursively reads and caches files and folders, and info about them, as this is way faster
// than repeatedly hitting the filesystem.

using System.IO;
using System;
using System.Windows.Forms;

namespace TVRename
{
    public class DirCache : System.Collections.Generic.List<DirCacheEntry>
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
                    foreach (DirectoryInfo di2 in dirs)
                        n += CountFiles(di2.FullName, subFolders);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            return n;
        }

        public DirCache()
        {
        }
        public DirCache(SetProgressDelegate prog, string folder, bool subFolders, TVSettings theSettings)
        {
            BuildDirCache(prog, 0, 0, folder, subFolders, theSettings);
        }
        public int AddFolder(SetProgressDelegate prog, int initialCount, int totalFiles, string folder, bool subFolders, TVSettings theSettings)
        {
            return BuildDirCache(prog, initialCount, totalFiles, folder, subFolders, theSettings);
        }

        private int BuildDirCache(SetProgressDelegate prog, int initialCount, int totalFiles, string folder, bool subFolders, TVSettings theSettings)
        {
            int filesDone = initialCount;

            if (!Directory.Exists(folder))
            {
                System.Windows.Forms.DialogResult res = MessageBox.Show("The search folder \"" + folder + " does not exist.\n", "Folder does not exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        this.Add(new DirCacheEntry(ff, theSettings));
                    if ((prog != null) && (totalFiles != 0))
                        prog.Invoke(100 * (filesDone) / totalFiles);
                }

                if (subFolders)
                {
                    DirectoryInfo[] dirs = di.GetDirectories();
                    foreach (DirectoryInfo di2 in dirs)
                        filesDone = BuildDirCache(prog, filesDone, totalFiles, di2.FullName, subFolders, theSettings);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch
            {
            }
            return filesDone;
        }
    }

} // namespace
