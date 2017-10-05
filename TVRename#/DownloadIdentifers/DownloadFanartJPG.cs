using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadFanartJPG : DownloadIdentifier
    {
        private static List<string> doneFanartJPG;
        private const string defaultFileName = "fanart.jpg";

        public DownloadFanartJPG() 
        {
            reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.downloadImage;
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            //We only want to do something if the fanart option is enabled. If the XBMC option is enabled then let it do the work.
            if ((TVSettings.Instance.FanArtJpg) && !TVSettings.Instance.XBMCImages)
            {
                ItemList TheActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(si.AutoAdd_FolderBase, defaultFileName);

                bool doesntExist =  !fi.Exists;
                if ((forceRefresh ||doesntExist) &&(!doneFanartJPG.Contains(fi.FullName)))
                {
                    string bannerPath = si.TheSeries().GetSeriesFanartPath();

                    if (!string.IsNullOrEmpty(bannerPath))
                        TheActionList.Add(new ActionDownload(si, null, fi, bannerPath, false));
                    doneFanartJPG.Add(fi.FullName);
                }
                return TheActionList;

            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override void reset()
        {
            doneFanartJPG = new List<string>(); 
            base.reset();
        }

    }

}
