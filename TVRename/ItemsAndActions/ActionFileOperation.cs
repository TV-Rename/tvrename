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
    public abstract class ActionFileOperation : Action
    {
        protected TVSettings.TidySettings? Tidyup;
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        protected void DeleteOrRecycleFile(FileInfo? file)
        {
            if (file is null)
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

        protected void DoTidyUp(DirectoryInfo? di) => FileHelper.DoTidyUp(di, Tidyup);
        protected void DeleteOrRecycleFolder(DirectoryInfo? di) => FileHelper.DeleteOrRecycleFolder(di, Tidyup);
    }
}
