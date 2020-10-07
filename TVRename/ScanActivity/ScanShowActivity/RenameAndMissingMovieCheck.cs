using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

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
            var allFolders = si.Locations.ToList();
            if (allFolders.Count == 0) // no folders defined for this show
            {
                return; // so, nothing to do.
            }

            //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
            //it has all the required files for that show
            //Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si));

            //TODO Put the banner refresh period into the settings file, we'll default to 3 months
            //TODO - work out if this banner thing is needed for Movies
            /*DateTime cutOff = DateTime.Now.AddMonths(-3);
            DateTime lastUpdate = si.BannersLastUpdatedOnDisk ?? DateTime.Now.AddMonths(-4);
            bool timeForBannerUpdate = cutOff.CompareTo(lastUpdate) == 1;

            if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
            {
                Doc.TheActionList.Add(
                    downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));

                si.BannersLastUpdatedOnDisk = DateTime.Now;
                Doc.SetDirty();
            }*/

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

            FileInfo file = null;

            foreach (FileInfo fi in files)
            {
                if (settings.Token.IsCancellationRequested)
                {
                    return;
                }

                FileInfo actualFile = fi;

                // == RENAMING CHECK ==
                if (renCheck && TVSettings.Instance.FileHasUsefulExtension(fi, true))
                {
                    // Note that the extension of the file may not be fi.extension as users can put ".mkv.t" for example as an extension
                    string otherExtension = TVSettings.Instance.FileHasUsefulExtensionDetails(fi, true);

                    string newName = TVSettings.Instance.FilenameFriendly(CustomMovieName.NameFor(si, TVSettings.Instance.MovieFilenameFormat, otherExtension));

                    FileInfo fileWorthAdding = CheckFile(folder, fi, actualFile, newName, files,si);

                    if (fileWorthAdding != null)
                    {
                        
                        file = fileWorthAdding;
                    }
                }
            } // foreach file in folder

            // == MISSING CHECK part 2/2 (includes NFO and Thumbnails) ==
            // look at the official list of episodes, and look to see if we have any gaps

            if (file is null) // not here locally
            {
                // second part of missing check is to see what is missing!
                if (missCheck)
                {
                    // then add it as officially missing
                    Doc.TheActionList.Add(new MovieItemMissing(si, folder));

                }// if doing missing check
            }
            else
            {
                Doc.TheActionList.Add(downloadIdentifiers.ProcessMovie(si, file));
            } // up to date check, for each episode in thetvdb
        }

        private FileInfo? CheckFile([NotNull] string folder, FileInfo fi, [NotNull] FileInfo actualFile, string newName, IEnumerable<FileInfo> files, MovieConfiguration c)
        {
            if (TVSettings.Instance.RetainLanguageSpecificSubtitles)
            {
                (bool isSubtitleFile, string subtitleExtension) = fi.IsLanguageSpecificSubtitle();

                if (isSubtitleFile && actualFile.Name != newName)
                {
                    newName = TVSettings.Instance.FilenameFriendly(CustomMovieName.NameFor(c,TVSettings.Instance.MovieFilenameFormat));
                }
            }

            FileInfo newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename

            if (newFile.FullName != actualFile.FullName)
            {
                //Check that the file does not already exist
                //if (FileHelper.FileExistsCaseSensitive(newFile.FullName))
                if (FileHelper.FileExistsCaseSensitive(files, newFile))
                {
                    LOGGER.Warn(
                        $"Identified that {actualFile.FullName} should be renamed to {newName}, but it already exists.");
                }
                else
                {
                    LOGGER.Info($"Identified that {actualFile.FullName} should be renamed to {newName}.");
                    Doc.TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                        newFile, c, false, null, Doc));

                    //The following section informs the DownloadIdentifers that we already plan to
                    //copy a file in the appropriate place and they do not need to worry about downloading 
                    //one for that purpose
                    downloadIdentifiers.NotifyComplete(newFile);

                    if (newFile.IsMovieFile())
                    {
                        return newFile;
                    }
                }
            }
            else
            {
                if (actualFile.IsMovieFile())
                {
                    //File is correct name
                    LOGGER.Debug($"Identified that {actualFile.FullName} is in the right place. Marking it as 'seen'.");
                    //Record this movie as seen

                    TVSettings.Instance.PreviouslySeenMovies.EnsureAdded(c);
                    if (TVSettings.Instance.IgnorePreviouslySeenMovies)
                    {
                        Doc.SetDirty();
                    }

                    return actualFile;
                }
            }

            return null;
        }

        protected override bool Active() => true;
    }
}
