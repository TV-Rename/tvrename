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
        /// Processes all file tasks.
        /// </summary>
        void ProcessAll();

        /// <summary>
        /// Quits the application.
        /// </summary>
        void Quit();
    }
}
