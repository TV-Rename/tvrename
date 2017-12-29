using System.Collections.Generic;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    class DownloadFanartJPG : DownloadIdentifier
    {
        private static List<string> _doneFanartJPG;
        private const string DefaultFileName = "fanart.jpg";

        public DownloadFanartJPG() 
        {
            Reset();
        }

        public override DownloadType GetDownloadType()
        {
            return DownloadType.DownloadImage;
        }

        public override ItemList ProcessShow(ShowItem si, bool forceRefresh)
        {
            //We only want to do something if the fanart option is enabled. If the KODI option is enabled then let it do the work.
            if ((TVSettings.Instance.FanArtJpg) && !TVSettings.Instance.KodiImages)
            {
                ItemList theActionList = new ItemList();
                FileInfo fi = FileHelper.FileInFolder(si.AutoAddFolderBase, DefaultFileName);

                bool doesntExist =  !fi.Exists;
                if ((forceRefresh ||doesntExist) &&(!_doneFanartJPG.Contains(fi.FullName)))
                {
                    string bannerPath = si.TheSeries().GetSeriesFanartPath();

                    if (!string.IsNullOrEmpty(bannerPath))
                        theActionList.Add(new ActionDownload(si, null, fi, bannerPath, false));
                    _doneFanartJPG.Add(fi.FullName);
                }
                return theActionList;

            }
            return base.ProcessShow(si, forceRefresh);
        }

        public override void Reset()
        {
            _doneFanartJPG = new List<string>(); 
            base.Reset();
        }

    }

}
