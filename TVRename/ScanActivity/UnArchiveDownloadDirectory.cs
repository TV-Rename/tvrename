using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    internal class UnArchiveDownloadDirectory : ScanActivity
    {
        public UnArchiveDownloadDirectory(TVDoc doc, TVDoc.ScanSettings settings) : base(doc, settings)
        { }

        public override bool Active() => TVSettings.Instance.UnArchiveFilesInDownloadDirectory;
        [NotNull]
        protected override string CheckName() => "Unarchived files in download directory";

        protected override void DoCheck(SetProgressDelegate progress)
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

                ReviewFilesInDownloadDirectory(dirPath);
            }
        }

        private void ReviewFilesInDownloadDirectory(string dirPath)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories).Where(File.Exists))
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
            catch (System.IO.DirectoryNotFoundException ex)
            {
                LOGGER.Warn(ex, $"Could not access files in {dirPath}");
            }
            catch (System.IO.IOException ex)
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

        private bool HasMissing([NotNull] MovieConfiguration x, [NotNull] FileInfo fi) => FinderHelper.FileNeeded(fi,x,new DirFilesCache());
    }
}
