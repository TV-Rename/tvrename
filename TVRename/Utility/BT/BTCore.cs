namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public abstract class BTCore
    {
        // ReSharper disable once InconsistentNaming
        private DirCache? FileCache;

        // ReSharper disable once InconsistentNaming
        private string? FileCacheIsFor;

        // ReSharper disable once InconsistentNaming
        private bool FileCacheWithSubFolders;

        protected BTCore()
        {
            FileCache = null;
            FileCacheIsFor = null;
            FileCacheWithSubFolders = false;
        }
    }
}
