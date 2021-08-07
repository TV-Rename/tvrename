using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename
{
    internal class RenameAndMissingMovieCheck : ScanMovieActivity
    {
        private readonly DownloadIdentifiersController downloadIdentifiers;

        protected override string ActivityName() => "Rename & Missing Movie Check";

        public RenameAndMissingMovieCheck(TVDoc doc) : base(doc)
        {
            downloadIdentifiers = new DownloadIdentifiersController();
        }

        protected override void Check(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            List<string> allFolders = si.Locations.ToList();
            if (allFolders.Count == 0) // no folders defined for this show
            {
                return; // so, nothing to do.
            }

            // process each folder for show...
            foreach (string folder in allFolders)
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                CheckMovieFolder(si, dfc, settings, folder);
            }
        }

        private void CheckMovieFolder(MovieConfiguration si, DirFilesCache dfc, TVDoc.ScanSettings settings, string folder)
        {
            if (settings.Token.IsCancellationRequested)
            {
                return;
            }

            FileInfo[] files = dfc.GetFiles(folder);

            bool renCheck = TVSettings.Instance.RenameCheck && si.DoRename && Directory.Exists(folder); // renaming check needs the folder to exist

            bool missCheck = TVSettings.Instance.MissingCheck && si.DoMissingCheck;

            if (!renCheck && !missCheck)
            {
                return;
            }

            FileInfo[] movieFiles = files.Where(f => f.IsMovieFile()).ToArray();

            if (movieFiles.Length == 0)
            {
                FileIsMissing(si, folder);
                return;
            }

            if (settings.Token.IsCancellationRequested)
            {
                return;
            }

            List<string> bases = movieFiles.Select(GetBase).Distinct().ToList();
            string newBase = TVSettings.Instance.FilenameFriendly(si.ProposedFilename);

            if (bases.Count == 1 && bases[0].Equals(newBase))
            {
                //All Seems OK

                //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
                //it has all the required files for that show
                Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, movieFiles.First(m => m.Name.StartsWith(newBase, StringComparison.Ordinal))));

                return;
            }

            if (si.Format==MovieConfiguration.MovieFolderFormat.multiPerDirectory)
            {
                //we have 3 options - matching file / close file that needs rename / else it's missing
                if (bases.Any(x => x.Equals(newBase)))
                {
                    Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, movieFiles.First(m => m.Name.StartsWith(newBase, StringComparison.Ordinal))));

                    return;
                }

                if (renCheck)
                {
                    List<string> matchingBases = bases.Where(x => IsClose(x, si)).ToList();
                    if (matchingBases.Any())
                    {
                        foreach (string baseString in matchingBases)
                        {
                            //rename all files with this base
                            PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
                        }

                        return;
                    }
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (missCheck)
                {
                    FileIsMissing(si, folder);
                }
                return;
            }

            if (renCheck && bases.Count == 1 && !bases[0].Equals(newBase, StringComparison.CurrentCultureIgnoreCase))
            {
                string baseString = bases[0];

                PlanToRenameFilesInFolder(si, settings, folder, files, baseString, newBase);
            }
            else
            {
                if (movieFiles.First().IsMovieFile())
                {
                    //File is correct name
                    LOGGER.Debug($"Identified that {movieFiles.First().FullName} is in the right place. Marking it as 'seen'.");
                    //Record this movie as seen

                    TVSettings.Instance.PreviouslySeenMovies.EnsureAdded(si);
                    if (TVSettings.Instance.IgnorePreviouslySeenMovies)
                    {
                        Doc.SetDirty();
                    }
                }
            }
        }

        private void PlanToRenameFilesInFolder(MovieConfiguration si, TVDoc.ScanSettings settings, string folder, FileInfo[] files,
            string baseString, string newBase)
        {
            foreach (FileInfo fi in files)
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                if (fi.Name.StartsWith(baseString, StringComparison.CurrentCultureIgnoreCase))
                {
                    string newName = baseString.HasValue() ? fi.Name.Replace(baseString, newBase) : newBase + fi.Extension;
                    FileInfo newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename

                    if (newFile.IsMovieFile())
                    {
                        //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
                        //it has all the required files for that show
                        Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, newFile));
                    }

                    if (newFile.FullName != fi.FullName)
                    {
                        //Check that the file does not already exist
                        //if (FileHelper.FileExistsCaseSensitive(newFile.FullName))
                        if (FileHelper.FileExistsCaseSensitive(files, newFile))
                        {
                            LOGGER.Warn(
                                $"Identified that {fi.FullName} should be renamed to {newName}, but it already exists.");
                        }
                        else
                        {
                            LOGGER.Info($"Identified that {fi.FullName} should be renamed to {newName}.");
                            Doc.TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                                newFile, si, false, null, Doc));

                            //The following section informs the DownloadIdentifers that we already plan to
                            //copy a file in the appropriate place and they do not need to worry about downloading
                            //one for that purpose
                            downloadIdentifiers.NotifyComplete(newFile);
                        }
                    }
                    else
                    {
                        if (fi.IsMovieFile())
                        {
                            //File is correct name
                            LOGGER.Debug($"Identified that {fi.FullName} is in the right place. Marking it as 'seen'.");
                            //Record this movie as seen

                            TVSettings.Instance.PreviouslySeenMovies.EnsureAdded(si);
                            if (TVSettings.Instance.IgnorePreviouslySeenMovies)
                            {
                                Doc.SetDirty();
                            }
                        }
                    }
                }
            } // foreach file in folder
        }

        private bool IsClose(string baseFileName, MovieConfiguration config)
        {
            (string targetFolder, string targetFolderEarlier, string targetFolderLater) = config.NeighbouringFolderNames();
            return baseFileName.Equals(targetFolderEarlier) || baseFileName.Equals(targetFolderLater) || baseFileName.Equals(targetFolder);
        }

        public static string GetBase(FileInfo fileInfo)
        {
            //The base is the filename with no multipart extensions
            return fileInfo.MovieFileNameBase();
        }

        private void FileIsMissing(MovieConfiguration si, string folder)
        {
            // second part of missing check is to see what is missing!
            if (TVSettings.Instance.MissingCheck && si.DoMissingCheck)
            {
                DateTime? dt = si.CachedMovie?.FirstAired;

                bool inPast = dt.HasValue && dt.Value.CompareTo(DateTime.Now) < 0;
                bool shouldCheckFutureDated = (TVSettings.Instance.CheckFutureDatedMovies || si.ForceCheckFuture) && dt.HasValue;
                bool shouldCheckNoDatedMovies = (TVSettings.Instance.CheckNoDatedMovies || si.ForceCheckNoAirdate) && !dt.HasValue;

                if (inPast || shouldCheckFutureDated || shouldCheckNoDatedMovies)
                {
                    // then add it as officially missing
                    Doc.TheActionList.Add(new MovieItemMissing(si, folder));
                }
                else
                {
                    LOGGER.Info($"{si.Name} not considered missing as it {(dt.HasValue? $"is in the future ({dt.Value.ToString("d", System.Globalization.DateTimeFormatInfo.InvariantInfo)})": "has no airdate")} (and the settings)");
                }
            } // if doing missing check
        }

        protected override bool Active() => true;
    }
}
