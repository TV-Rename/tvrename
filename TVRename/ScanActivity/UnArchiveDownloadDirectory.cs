using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    internal class UnArchiveDownloadDirectory : ScanActivity
    {
        public UnArchiveDownloadDirectory(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        { }

        public override bool Active() => TVSettings.Instance.UnArchiveFilesInDownloadDirectory;
        protected override string CheckName() => "Unarchived files in download directory";

        protected override void DoCheck(SetProgressDelegate prog)
        {
            int totalDownloadFolders = TVSettings.Instance.DownloadFolders.Count;
            int c = 0;

            foreach (string dirPath in TVSettings.Instance.DownloadFolders.ToList())
            {
                UpdateStatus(c++, totalDownloadFolders, dirPath);

                if (!Directory.Exists(dirPath) || Settings.Token.IsCancellationRequested)
                {
                    continue;
                }

                ReviewFilesInDownloadDirectory(dirPath, Settings.Owner);
            }
        }

        private void ReviewFilesInDownloadDirectory(string dirPath, IDialogParent owner)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories).Where(File.Exists))
                {
                    if (Settings.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    FileInfo fi = new(filePath);

                    if (!fi.IsArchiveFile())
                    {
                        continue;
                    }

                    ReviewArchive(fi);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LOGGER.Warn(ex, $"Could not access files in {dirPath}");
            }
            catch (DirectoryNotFoundException ex)
            {
                LOGGER.Warn(ex, $"Could not access files in {dirPath}");
            }
            catch (IOException ex)
            {
                LOGGER.Warn(ex, $"Could not access files in {dirPath}");
            }
        }

        private void ReviewArchive(FileInfo fi)
        {
            List<ShowConfiguration> matchingShowsAll = MDoc.TvLibrary.GetSortedShowItems().Where(si => si.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();
            List<ShowConfiguration> matchingShows = FinderHelper.RemoveShortShows(matchingShowsAll);
            List<MovieConfiguration> matchingMoviesAll = MDoc.FilmLibrary.GetSortedMovies().Where(mi => mi.NameMatch(fi, TVSettings.Instance.UseFullPathNameToMatchSearchFolders)).ToList();
            List<MovieConfiguration> matchingMovies = FinderHelper.RemoveShortShows(matchingMoviesAll);

            List<MovieConfiguration> matchingMoviesNoShows =
                FinderHelper.RemoveShortMedia(matchingMovies, matchingShows);
            List<ShowConfiguration> matchingShowsNoMovies =
                FinderHelper.RemoveShortMedia(matchingShows, matchingMovies);

            if (matchingShowsNoMovies.Any())
            {
                MDoc.TheActionList.Add(new ActionUnArchive(fi,matchingShowsNoMovies.First()));
                return;
            }

            if (matchingMoviesNoShows.Any(x => HasMissing(x, fi)))
            {
                MDoc.TheActionList.Add(new ActionUnArchive(fi, matchingMoviesNoShows.First(x => HasMissing(x, fi))));
            }
        }

        private void PlanUnarchive(FileInfo fi)
        {
            throw new NotImplementedException();
        }

        private bool HasMissing(MovieConfiguration x, FileInfo fi)
        {
            return FinderHelper.FileNeeded(fi,x,new DirFilesCache());
        }
    }
}
