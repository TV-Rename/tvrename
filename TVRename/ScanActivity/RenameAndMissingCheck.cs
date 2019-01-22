using System;
using System.Collections.Generic;
using System.Linq;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    class RenameAndMissingCheck : ScanShowActivity
    {
        private readonly DownloadIdentifiersController downloadIdentifiers;

        protected override string Checkname() => "Rename & Missing Check";
        public RenameAndMissingCheck(TVDoc doc) : base(doc)
        {
            downloadIdentifiers = new DownloadIdentifiersController();
        }

        protected override void Check(ShowItem si, DirFilesCache dfc, TVDoc.ScanSettings settings)
        {
            Dictionary<int, List<string>> allFolders = si.AllExistngFolderLocations();
            if (allFolders.Count == 0) // no folders defined for this show
                return; // so, nothing to do.

            //This is the code that will iterate over the DownloadIdentifiers and ask each to ensure that
            //it has all the required files for that show
            if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (allFolders.Any()))
            {
                Doc.TheActionList.Add(downloadIdentifiers.ProcessShow(si));
            }

            //MS_TODO Put the bannerrefresh period into the settings file, we'll default to 3 months
            DateTime cutOff = DateTime.Now.AddMonths(-3);
            DateTime lastUpdate = si.BannersLastUpdatedOnDisk ?? DateTime.Now.AddMonths(-4);
            bool timeForBannerUpdate = (cutOff.CompareTo(lastUpdate) == 1);

            if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
            {
                Doc.TheActionList.Add(
                    downloadIdentifiers.ForceUpdateShow(DownloadIdentifier.DownloadType.downloadImage, si));

                si.BannersLastUpdatedOnDisk = DateTime.Now;
                Doc.SetDirty();
            }

            // process each folder for each season...

            int[] numbers = new int[si.SeasonEpisodes.Keys.Count];
            si.SeasonEpisodes.Keys.CopyTo(numbers, 0);

            int lastSeason = numbers.DefaultIfEmpty(0).Max();

            foreach (int snum in numbers)
            {
                if (settings.Token.IsCancellationRequested)
                    return;

                if ((si.IgnoreSeasons.Contains(snum)) || (!allFolders.ContainsKey(snum)))
                    continue; // ignore/skip this season

                if ((snum == 0) && (si.CountSpecials))
                    continue; // don't process the specials season, as they're merged into the seasons themselves

                // all the folders for this particular season
                List<string> folders = allFolders[snum];

                bool folderNotDefined = (folders.Count == 0);
                if (folderNotDefined && (TVSettings.Instance.MissingCheck && !si.AutoAddNewSeasons()))
                    continue; // folder for the season is not defined, and we're not auto-adding it

                List<ProcessedEpisode> eps = si.SeasonEpisodes[snum];
                int maxEpisodeNumber = GetMaxEpisodeNumber(eps);

                // base folder:
                if (!string.IsNullOrEmpty(si.AutoAddFolderBase) && (si.AutoAddType != ShowItem.AutomaticFolderType.none))
                {
                    // main image for the folder itself
                    Doc.TheActionList.Add(downloadIdentifiers.ProcessShow(si));
                }

                foreach (string folder in folders)
                {
                    if (settings.Token.IsCancellationRequested)
                        return;

                    FileInfo[] files = dfc.GetFiles(folder);
                    if (files == null)
                        continue;

                    if (TVSettings.Instance.NeedToDownloadBannerFile() && timeForBannerUpdate)
                    {
                        //Image series checks here
                        Doc.TheActionList.Add(
                            downloadIdentifiers.ForceUpdateSeason(DownloadIdentifier.DownloadType.downloadImage, si,
                                folder, snum));
                    }

                    bool renCheck =
                        TVSettings.Instance.RenameCheck && si.DoRename &&
                        Directory.Exists(folder); // renaming check needs the folder to exist

                    bool missCheck = TVSettings.Instance.MissingCheck && si.DoMissingCheck;

                    //Image series checks here
                    Doc.TheActionList.Add(downloadIdentifiers.ProcessSeason(si, folder, snum));

                    FileInfo[] localEps = new FileInfo[maxEpisodeNumber + 1];

                    int maxEpNumFound = 0;
                    if (!renCheck && !missCheck)
                        continue;

                    foreach (FileInfo fi in files)
                    {
                        if (settings.Token.IsCancellationRequested)
                            return;

                        if (!FinderHelper.FindSeasEp(fi, out int seasNum, out int epNum, out int _, si,
                            out TVSettings.FilenameProcessorRE _))
                            continue; // can't find season & episode, so this file is of no interest to us

                        if (seasNum == -1)
                            seasNum = snum;

                        int epIdx = eps.FindIndex(x =>
                            ((x.AppropriateEpNum == epNum) && (x.AppropriateSeasonNumber == seasNum)));

                        if (epIdx == -1)
                            continue; // season+episode number don't correspond to any episode we know of from thetvdb

                        ProcessedEpisode ep = eps[epIdx];
                        FileInfo actualFile = fi;

                        if (renCheck && TVSettings.Instance.FileHasUsefulExtension(fi, true, out string otherExtension)) // == RENAMING CHECK ==
                        {
                            string newName = TVSettings.Instance.FilenameFriendly(
                                TVSettings.Instance.NamingStyle.NameFor(ep, otherExtension, folder.Length));

                            if (TVSettings.Instance.RetainLanguageSpecificSubtitles &&
                                fi.IsLanguageSpecificSubtitle(out string subtitleExtension) &&
                                actualFile.Name != newName)
                            {
                                newName = TVSettings.Instance.FilenameFriendly(
                                    TVSettings.Instance.NamingStyle.NameFor(ep, subtitleExtension,
                                        folder.Length));
                            }

                            FileInfo newFile = FileHelper.FileInFolder(folder, newName); // rename updates the filename

                            if (newName != actualFile.Name)
                            {
                                //Check that the file does not already exist
                                //if (FileHelper.FileExistsCaseSensitive(newFile.FullName))
                                if (FileHelper.FileExistsCaseSensitive(files, newFile))
                                {
                                    LOGGER.Warn($"Identified that {actualFile.FullName} should be renamed to {newName}, but it already exists.");
                                }
                                else
                                {
                                    Doc.TheActionList.Add(new ActionCopyMoveRename(ActionCopyMoveRename.Op.rename, fi,
                                        newFile, ep, false, null));

                                    //The following section informs the DownloadIdentifers that we already plan to
                                    //copy a file inthe appropriate place and they do not need to worry about downloading 
                                    //one for that purpose
                                    downloadIdentifiers.NotifyComplete(newFile);

                                    localEps[epNum] = newFile;
                                }
                            }
                        }

                        if (missCheck && fi.IsMovieFile())
                        // == MISSING CHECK part 1/2 ==
                        {
                            // first pass of missing check is to tally up the episodes we do have
                            if (localEps[epNum] is null) localEps[epNum] = actualFile;

                            if (epNum > maxEpNumFound)
                                maxEpNumFound = epNum;
                        }
                    } // foreach file in folder

                    if (missCheck) // == MISSING CHECK part 2/2 (includes NFO and Thumbnails) ==
                    {
                        // second part of missing check is to see what is missing!

                        // look at the official list of episodes, and look to see if we have any gaps

                        DateTime today = DateTime.Now;
                        foreach (ProcessedEpisode dbep in eps)
                        {
                            if ((dbep.AppropriateEpNum > maxEpNumFound) || (localEps[dbep.AppropriateEpNum] == null)
                            ) // not here locally
                            {
                                DateTime? dt = dbep.GetAirDateDt(true);
                                bool dtOk = dt != null;

                                bool notFuture =
                                    (dtOk && (dt.Value.CompareTo(today) < 0)); // isn't an episode yet to be aired

                                bool noAirdatesUntilNow = true;
                                // for specials "season", see if any season has any airdates
                                // otherwise, check only up to the season we are considering
                                for (int i = 1; i <= ((snum == 0) ? lastSeason : snum); i++)
                                {
                                    if (ShowLibrary.HasAnyAirdates(si, i))
                                    {
                                        noAirdatesUntilNow = false;
                                        break;
                                    }
                                    else
                                    {
                                        //If the show is in its first season and no episodes have air dates
                                        if (lastSeason == 1)
                                        {
                                            noAirdatesUntilNow = false;
                                        }
                                    }
                                }

                                // only add to the missing list if, either:
                                // - force check is on
                                // - there are no airdates at all, for up to and including this season
                                // - there is an airdate, and it isn't in the future
                                if (noAirdatesUntilNow ||
                                    ((si.ForceCheckFuture || notFuture) && dtOk) ||
                                    (si.ForceCheckNoAirdate && !dtOk))
                                {
                                    // then add it as officially missing
                                    Doc.TheActionList.Add(new ItemMissing(dbep, folder,
                                        TVSettings.Instance.FilenameFriendly(
                                            TVSettings.Instance.NamingStyle.NameFor(dbep, null, folder.Length))));
                                }
                            }
                            else
                            {
                                if (settings.Type == TVSettings.ScanType.Full) Doc.CurrentStats.NsNumberOfEpisodes++;

                                // do NFO and thumbnail checks if required
                                FileInfo
                                    filo = localEps[dbep.AppropriateEpNum]; // filename (or future filename) of the file

                                Doc.TheActionList.Add(downloadIdentifiers.ProcessEpisode(dbep, filo));

                                //Record this episode as seen
                                TVSettings.Instance.PreviouslySeenEpisodes.EnsureAdded(dbep);
                            }
                        } // up to date check, for each episode in thetvdb
                    } // if doing missing check
                } // for each folder for this season of this show
            } // for each season of this show
        }

        private static int GetMaxEpisodeNumber(List<ProcessedEpisode> eps)
        {
            int maxEpisodeNumber = 0;
            foreach (ProcessedEpisode episode in eps)
            {
                if (episode.AppropriateEpNum > maxEpisodeNumber)
                    maxEpisodeNumber = episode.AppropriateEpNum;
            }

            return maxEpisodeNumber;
        }

        protected override bool Active() => true;
    }
}
