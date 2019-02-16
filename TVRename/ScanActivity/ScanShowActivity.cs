// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Linq;

namespace TVRename
{
    public abstract class ScanShowActivity
    {
        protected static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        protected readonly TVDoc Doc;

        protected ScanShowActivity(TVDoc doc) => Doc = doc;

        protected abstract void Check(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings);

        protected abstract bool Active();

        public void CheckIfActive(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            if (Active())
            {
                Check(si,dfc,settings);
                LogActionListSummary();
            }
        }

        private void LogActionListSummary()
        {
            LOGGER.Debug($"   Summary of known actions after check: {Checkname()}");
            LOGGER.Debug($"      Missing Items: {Doc.TheActionList.MissingItems().Count()}");
            LOGGER.Debug($"      Copy/Move Items: {Doc.TheActionList.CopyMoveItems().Count()}");
            LOGGER.Debug($"      Total Actions: {Doc.TheActionList.Actions().Count()}");
        }

        protected abstract string Checkname();
    }
}
