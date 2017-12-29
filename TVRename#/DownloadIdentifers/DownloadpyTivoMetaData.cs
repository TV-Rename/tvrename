using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadpyTivoMetaData : DownloadIdentifier
    {

        public DownloadpyTivoMetaData() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadMetaData;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.PyTivoMeta)
            {
                ItemList theActionList = new ItemList(); 
                string fn = filo.Name;
                fn += ".txt";
                string folder = filo.DirectoryName;
                if (TVSettings.Instance.PyTivoMetaSubFolder)
                    folder += "\\.meta";
                FileInfo meta = FileHelper.FileInFolder(folder, fn);

                if (!meta.Exists || (dbep.SrvLastUpdated > TimeZone.Epoch(meta.LastWriteTime)))
                    theActionList.Add(new ActionPyTivoMeta(meta, dbep));

                return theActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public override void Reset()
        {
            base.Reset();
        }

    }
}
