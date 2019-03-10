// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Diagnostics;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    public abstract class ActionFileOperation : Action
    {
        protected TVSettings.TidySettings Tidyup;
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        protected void DeleteOrRecycleFile([CanBeNull] FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            if (Tidyup is null ||  Tidyup.DeleteEmptyIsRecycle)
            {
                LOGGER.Info($"Recycling {file.FullName}");
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file.FullName,
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                LOGGER.Info($"Deleting {file.FullName}");
                file.Delete(true);
            }
        }

        protected void DeleteOrRecycleFolder([CanBeNull] DirectoryInfo di)
        {
            if (di == null)
            {
                return;
            }

            if (Tidyup ==null ||Tidyup.DeleteEmptyIsRecycle)
            {
                LOGGER.Info($"Recycling {di.FullName}");
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(di.FullName,
                    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
            }
            else
            {
                LOGGER.Info($"Deleting {di.FullName}");
                di.Delete(true, true);
            }
        }

        protected void DoTidyup([CanBeNull] DirectoryInfo di)
        {
#if DEBUG
            Debug.Assert(Tidyup != null);
            Debug.Assert(Tidyup.DeleteEmpty);
#else
            if (this.Tidyup == null || !this.Tidyup.DeleteEmpty)
                return;
#endif
            // See if we should now delete the folder we just moved that file from.
            if (di == null)
            {
                return;
            }

            //if there are sub-directories then we shouldn't remove this one
            DirectoryInfo[] directories = di.GetDirectories();
            foreach (DirectoryInfo subdi in directories)
            {
                bool okToDelete = Tidyup.EmptyIgnoreWordsArray.Any(word =>
                    subdi.Name.Contains(word, StringComparison.OrdinalIgnoreCase));

                if (!okToDelete)
                {
                    return;
                }
            }
            //we know that each subfolder is OK to delete

            //if the directory is the root download folder do not delete
            if (TVSettings.Instance.DownloadFolders.Contains(di.FullName))
            {
                return;
            }

            // Do not delete any monitor folders either
            if (TVSettings.Instance.LibraryFolders.Contains(di.FullName))
            {
                return;
            }

            FileInfo[] files = di.GetFiles();
            if (files.Length == 0)
            {
                // its empty, so just delete it
                DeleteOrRecycleFolder(di);
                return;
            }

            if (Tidyup.EmptyIgnoreExtensions && !Tidyup.EmptyIgnoreWords)
            {
                return; // nope
            }

            foreach (FileInfo fi in files)
            {
                bool okToDelete = Tidyup.EmptyIgnoreExtensions &&
                                  Tidyup.EmptyIgnoreExtensionsArray.Contains(fi.Extension);

                if (okToDelete)
                {
                    continue; // onto the next file
                }

                // look in the filename
                if (Tidyup.EmptyIgnoreWordsArray.Any(word =>
                    fi.Name.Contains(word, StringComparison.OrdinalIgnoreCase)))
                {
                    okToDelete = true;
                }

                if (!okToDelete)
                {
                    return;
                }
            }

            if (Tidyup.EmptyMaxSizeCheck)
            {
                // how many MB are we deleting?
                long totalBytes = files.Sum(fi => fi.Length);

                if (totalBytes / (1024 * 1024) > Tidyup.EmptyMaxSizeMB)
                {
                    return; // too much
                }
            }

            DeleteOrRecycleFolder(di);
        }
    }
}
