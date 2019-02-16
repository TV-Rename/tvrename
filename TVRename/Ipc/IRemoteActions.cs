// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename.Ipc
{
    /// <summary>
    /// Remote actions available over IPC.
    /// </summary>
    /// <see cref="RemoteClient"/>
    internal interface IRemoteActions
    {
        /// <summary>
        /// Focuses the window and bring to foreground.
        /// </summary>
        void FocusWindow();

        /// <summary>
        /// Scans all files.
        /// </summary>
        void Scan();

        /// <summary>
        /// Scans all files.
        /// </summary>
        void QuickScan();

        /// <summary>
        /// Scans all files.
        /// </summary>
        void RecentScan();

        /// <summary>
        /// Processes all file tasks.
        /// </summary>
        void ProcessAll();

        /// <summary>
        /// Quits the application.
        /// </summary>
        void Quit();
    }
}
