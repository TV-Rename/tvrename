using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadpyTivoMetaData : DownloadIdentifier
    {

        public DownloadpyTivoMetaData() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadMetaData;
        }

        public override ItemList ProcessEpisode(ProcessedEpisode dbep, FileInfo filo, bool forceRefresh)
        {
            if (TVSettings.Instance.pyTivoMeta)
            {
                ItemList TheActionList = new ItemList(); 
                string fn = filo.Name;
                fn += ".txt";
                string folder = filo.DirectoryName;
                if (TVSettings.Instance.pyTivoMetaSubFolder)
                    folder += "\\.meta";
                FileInfo meta = FileHelper.FileInFolder(folder, fn);

                if (!meta.Exists || (dbep.Srv_LastUpdated > TimeZone.Epoch(meta.LastWriteTime)))
                    TheActionList.Add(new ActionPyTivoMeta(meta, dbep));

                return TheActionList;
            }
            return base.ProcessEpisode(dbep, filo, forceRefresh);
        }

        public override void reset()
        {
            base.reset();
        }

    }
}
