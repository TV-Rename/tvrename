//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal class CheckShows : ScanActivity
    {
        public CheckShows(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        {
        }

        protected override string CheckName() => "Looked in the library to find missing files";

        protected override void DoCheck(SetProgressDelegate prog)
        {
            if (TVSettings.Instance.RenameCheck)
            {
                MDoc.Stats().RenameChecksDone++;
            }

            if (TVSettings.Instance.MissingCheck)
            {
                MDoc.Stats().MissingChecksDone++;
            }

            DirFilesCache dfc = new DirFilesCache();

            List<ShowConfiguration> showList = Settings.Shows;

            if (Settings.Type == TVSettings.ScanType.Full && showList.Count > 0)
            {
                // only do episode count if we're doing all shows and seasons
                MDoc.CurrentStats.NsNumberOfEpisodes = 0;
            }

            int c = 0;
            UpdateStatus(c, showList.Count, "Checking shows");
            foreach (ShowConfiguration si in showList.OrderBy(item => item.ShowName))
            {
                UpdateStatus(c++, showList.Count, si.ShowName);
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                LOGGER.Info("Rename and missing check: " + si.ShowName);
                try
                {
                    new CheckAllFoldersExist(MDoc).CheckIfActive(si, dfc, Settings);
                    new MergeLibraryEpisodes(MDoc).CheckIfActive(si, dfc, Settings);
                    new RenameAndMissingCheck(MDoc).CheckIfActive(si, dfc, Settings);
                }
                catch (TVRenameOperationInterruptedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, $"Failed to scan {si.ShowName}. Please double check settings for this show: {si}: {si.AutoAddFolderBase}");
                }
            } // for each show

            c = 0;
            UpdateStatus(c, Settings.Movies.Count, "Checking movies");
            foreach (MovieConfiguration si in Settings.Movies.OrderBy(item => item.ShowName))
            {
                UpdateStatus(c++, Settings.Movies.Count, si.ShowName);
                if (Settings.Token.IsCancellationRequested)
                {
                    return;
                }

                LOGGER.Info("Rename and missing check: " + si.ShowName);
                try
                {
                    new CheckAllMovieFoldersExist(MDoc).CheckIfActive(si, dfc, Settings);
                    new RenameAndMissingMovieCheck(MDoc).CheckIfActive(si, dfc, Settings);
                }
                catch (TVRenameOperationInterruptedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, $"Failed to scan {si.ShowName}. Please double check settings for this movie: {si.Code}: {si}");
                }
            } // for each movie

            MDoc.RemoveIgnored();
        }

        public override bool Active() => TVSettings.Instance.RenameCheck || TVSettings.Instance.MissingCheck;
    }
}
