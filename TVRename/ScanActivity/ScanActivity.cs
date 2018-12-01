// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;

namespace TVRename
{
    public abstract class ScanActivity
    {
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        protected readonly TVDoc MDoc;

        protected ScanActivity(TVDoc doc) => MDoc = doc;

        public abstract void Check(SetProgressDelegate prog, int startpct, int totPct, ICollection<ShowItem> showList,
            TVDoc.ScanSettings settings);

        public abstract bool Active();

        private void Check(SetProgressDelegate prog, List<ShowItem> showList, TVDoc.ScanSettings settings) =>
            Check(prog, 0, 100, showList, settings);

        public void CheckIfActive(SetProgressDelegate prog, List<ShowItem> showList,TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                 Check(prog, showList, settings);
            }
        }

        internal void CheckIfActive(SetProgressDelegate prog, int startpct, int totPct, List<ShowItem> showList, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(prog,startpct,totPct, showList, settings);
            }
        }
    }
}
