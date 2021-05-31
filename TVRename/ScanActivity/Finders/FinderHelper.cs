//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using NLog;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace TVRename
{
    internal static class FinderHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowConfiguration? si, out TVSettings.FilenameProcessorRE? re)
        {
            return FindSeasEp(fi, out seas, out ep, out maxEp, si, TVSettings.Instance.FNPRegexs, out re);
        }

        public static bool FindSeasEp(string itemName, out int seas, out int ep, out int maxEp, ShowConfiguration? si, IEnumerable<TVSettings.FilenameProcessorRE> rexps, out TVSettings.FilenameProcessorRE? re)
        {
            return FindSeasEp(string.Empty, itemName, out seas, out ep, out maxEp, si, rexps, out re);
        }

        public static bool FindSeasEp(FileInfo? fi, out int seas, out int ep, out int maxEp, ShowConfiguration? si,
            IEnumerable<TVSettings.FilenameProcessorRE> rexps, out TVSettings.FilenameProcessorRE? re)
        {
            if (fi is null)
            {
                re = null;
                seas = -1;
                ep = -1;
                maxEp = -1;
                return false;
            }

            return FindSeasEp(fi.Directory.FullName, fi.RemoveExtension(), out seas, out ep, out maxEp, si, rexps, out re);
        }

        public static bool FindSeasEpNameCheck(FileInfo? fi, ShowConfiguration? si, out int seas, out int ep)
        {
            ep = -1;
            seas = -1;

            if (fi is null || si is null)
            {
                return false;
            }

            CachedSeriesInfo? ser = si.CachedShow;

            if (ser is null)
            {
                return false;
            }

            string simplifiedFilename = fi.Name.CompareName();

            foreach (Episode epi in si.EpisodesToUse())
            {
                string simplifiedEpName = epi.Name.CompareName();

                if (simplifiedFilename.Contains(simplifiedEpName))
                {
                    seas = epi.GetSeasonNumber(si.Order);
                    ep = epi.GetEpisodeNumber(si.Order);
                    return true;
                }
            }

            return false;
        }

        public static bool MatchesSequentialNumber(string filename, [NotNull] ProcessedEpisode pe)
        {
            if (pe.OverallNumber == -1)
            {
                return false;
            }

            string num = pe.OverallNumber.ToString();
            string matchText = "X" + filename + "X"; // need to pad to let it match non-numbers at start and end

            Match betterMatch = Regex.Match(matchText, @"(E|e|Ep|ep|episode|Episode) ?0*(?<sequencenumber>\d+)\D");

            if (betterMatch.Success)
            {
                int sequenceNUm = int.Parse(betterMatch.Groups["sequencenumber"]?.Value ?? "-2");
                return sequenceNUm == pe.OverallNumber;
            }

            return Regex.Match(matchText, @"\D0*" + num + @"\D").Success;
        }

        public static bool FindSeasEpDateCheck(string? filename, out int seas, out int ep, ShowConfiguration? si)
        {
            ep = -1;
            seas = -1;

            if (filename is null || si is null)
            {
                return false;
            }

            // look for a valid airdate in the filename
            // check for YMD, DMY, and MDY
            // only check against airdates we expect for the given show
            CachedSeriesInfo ser = si.CachedShow;

            if (ser is null)
            {
                return false;
            }

            string[] dateFormats = { "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy", "MM-dd-yy" };

            // force possible date separators to a dash
            filename = filename.Replace("/", "-");
            filename = filename.Replace(".", "-");
            filename = filename.Replace(",", "-");
            filename = filename.Replace(" ", "-");

            foreach (Episode epi in si.EpisodesToUse())
            {
                LocalDateTime? dt = epi.GetAirDateDt(); // file will have local timezone date, not ours
                if (dt is null)
                {
                    continue;
                }

                TimeSpan closestDate = TimeSpan.MaxValue;

                foreach (string dateFormat in dateFormats)
                {
                    string datestr = dt.Value.ToString(dateFormat, CultureInfo.CurrentCulture);

                    if (filename.Contains(datestr) && DateTime.TryParseExact(datestr, dateFormat,
                            new CultureInfo("en-GB"), DateTimeStyles.None, out DateTime dtInFilename))
                    {
                        TimeSpan timeAgo = DateTime.Now.Subtract(dtInFilename);

                        if (timeAgo < closestDate)
                        {
                            seas = epi.GetSeasonNumber(si.Order);
                            ep = epi.GetEpisodeNumber(si.Order);
                            closestDate = timeAgo;
                        }
                    }
                }
            }

            return ep != -1 && seas != -1;
        }

        public static bool FindSeasEp(DirectoryInfo? di, out int seas, out int ep, ShowConfiguration si,
            out TVSettings.FilenameProcessorRE? re)
        {
            List<TVSettings.FilenameProcessorRE> rexps = TVSettings.Instance.FNPRegexs;
            re = null;

            if (di is null)
            {
                seas = -1;
                ep = -1;
                return false;
            }

            return FindSeasEp(di.Parent.FullName, di.Name, out seas, out ep, out int _, si, rexps, out re);
        }

        public static bool FindSeasEp(string itemName, out int seas, out int ep, out int maxEp, ShowConfiguration? show)
        {
            return FindSeasEp(string.Empty, itemName, out seas, out ep, out maxEp, show, TVSettings.Instance.FNPRegexs, out TVSettings.FilenameProcessorRE _);
        }

        public static bool FindSeasEp(FileInfo theFile, out int seasF, out int epF, out int maxEp, ShowConfiguration? sI)
        {
            return FindSeasEp(theFile, out seasF, out epF, out maxEp, sI, out TVSettings.FilenameProcessorRE _);
        }

        public static bool FileNeeded(FileInfo fi, ShowConfiguration si, DirFilesCache dfc)
        {
            if (FindSeasEp(fi, out int seasF, out int epF, out _, si, out _))
            {
                return EpisodeNeeded(si, dfc, seasF, epF, fi);
            }

            //We may need the file
            return true;
        }

        public static bool FileNeeded(FileInfo fi, MovieConfiguration si, DirFilesCache dfc)
        {
            return MovieNeeded(si, dfc, fi);
        }

        private static bool MovieNeeded(MovieConfiguration si, DirFilesCache dfc, FileInfo fi)
        {
            if (fi is null)
            {
                throw new ArgumentNullException(nameof(fi));
            }

            if (si is null)
            {
                throw new ArgumentNullException(nameof(si));
            }

            foreach (FileInfo testFileInfo in FindMovieOnDisk(dfc, si))
            {
                //We will check that the file that is found is not the one we are testing
                if (fi.FullName == testFileInfo.FullName)
                {
                    continue;
                }

                //We have found another file that matches
                return false;
            }
            return true;
        }

        public static bool FileNeeded(DirectoryInfo? di, ShowConfiguration? si, DirFilesCache dfc)
        {
            if (di is null)
            {
                throw new ArgumentNullException(nameof(di));
            }

            if (si is null)
            {
                throw new ArgumentNullException(nameof(si));
            }

            if (FindSeasEp(di, out int seasF, out int epF, si, out _))
            {
                return EpisodeNeeded(si, dfc, seasF, epF, di);
            }

            //We may need the file
            return true;
        }

        public static bool FileNeeded(DirectoryInfo? di, MovieConfiguration? si, DirFilesCache dfc)
        {
            if (di is null)
            {
                throw new ArgumentNullException(nameof(di));
            }

            if (si is null)
            {
                throw new ArgumentNullException(nameof(si));
            }

            foreach (FileInfo testFileInfo in FindMovieOnDisk(dfc, si))
            {
                //We will check that the file that is found is not the one we are testing
                if (di.FullName == testFileInfo.FullName)
                {
                    continue;
                }

                //We have found another file that matches
                return false;
            }
            return true;
        }

        public static IEnumerable<FileInfo> FindMovieOnDisk(this DirFilesCache cache, MovieConfiguration si)
        {
            List<FileInfo> ret = new List<FileInfo>();

            foreach (FileInfo fiTemp in si.Locations
                .Select(cache.GetFiles)
                .SelectMany(files => files.Where(fiTemp => fiTemp.IsMovieFile())))
            {
                {
                    ret.Add(fiTemp);
                }
            }

            return ret;
        }

        [NotNull]
        public static List<FileInfo> FindEpOnDisk(this DirFilesCache? dfc, ProcessedEpisode pe,
            bool checkDirectoryExist = true)
        {
            return FindEpOnDisk(dfc, pe.Show, pe, checkDirectoryExist);
        }

        [NotNull]
        private static List<FileInfo> FindEpOnDisk(DirFilesCache? dfc, ShowConfiguration si, ProcessedEpisode epi,
            bool checkDirectoryExist = true)
        {
            DirFilesCache cache = dfc ?? new DirFilesCache();

            List<FileInfo> ret = new List<FileInfo>();

            int seasWanted = epi.AppropriateSeasonNumber;
            int epWanted = epi.AppropriateEpNum;

            int snum = seasWanted;

            Dictionary<int, SafeList<string>> dirs = si.AllFolderLocationsEpCheck(checkDirectoryExist);

            if (!dirs.ContainsKey(snum))
            {
                return ret;
            }

            foreach (string folder in dirs[snum])
            {
                FileInfo[] files = cache.GetFiles(folder);

                foreach (FileInfo fiTemp in files.Where(fiTemp => fiTemp.IsMovieFile()))
                {
                    if (!FindSeasEp(fiTemp, out int seasFound, out int epFound, out int _, si))
                    {
                        continue;
                    }

                    if (seasFound == -1)
                    {
                        seasFound = seasWanted;
                    }

                    if (seasFound == seasWanted && epFound == epWanted)
                    {
                        ret.Add(fiTemp);
                    }
                }
            }

            return ret;
        }

        private static bool EpisodeNeeded([NotNull] ShowConfiguration si, DirFilesCache dfc, int seasF, int epF,
            [NotNull] FileSystemInfo fi)
        {
            if (si is null)
            {
                throw new ArgumentNullException(nameof(si));
            }

            if (fi is null)
            {
                throw new ArgumentNullException(nameof(fi));
            }

            try
            {
                CachedSeriesInfo s = si.CachedShow;
                if (s is null)
                {
                    //We have not downloaded the cachedSeries, so have to assume that we need the episode/file
                    return true;
                }

                ProcessedEpisode pep = si.GetEpisode(seasF, epF);

                foreach (FileInfo testFileInfo in FindEpOnDisk(dfc, si, pep))
                {
                    //We will check that the file that is found is not the one we are testing
                    if (fi.FullName == testFileInfo.FullName)
                    {
                        continue;
                    }

                    //We have found another file that matches
                    return false;
                }
            }
            catch (ShowConfiguration.EpisodeNotFoundException)
            {
                //Ignore exception, we may need the file
                return true;
            }
            return true;
        }

        [NotNull]
        private static string SimplifyFilename([NotNull] string filename, string? showNameHint)
        {
            // Look at showNameHint and try to remove the first occurrence of it from filename
            // This is very helpful if the show's name has a >= 4 digit number in it, as that
            // would trigger the 1302 -> 13,02 matcher
            // Also, shows like "24" can cause confusion

            //TODO: More replacement of non useful characters - MarkSummerville
            string returnFilename = filename.Replace(".", " "); // turn dots into spaces

            if (string.IsNullOrEmpty(showNameHint))
            {
                return returnFilename;
            }

            try
            {
                if (returnFilename.StartsWith(showNameHint!, StringComparison.Ordinal))
                {
                    return returnFilename.Remove(0, showNameHint.Length);
                }

                bool nameIsNumber = Regex.Match(showNameHint!, "^[0-9]+$").Success;

                if (nameIsNumber && returnFilename.Contains(showNameHint!)) // e.g. "24", or easy exact match of show name at start of filename
                {
                    return Regex.Replace(returnFilename, "(^|\\W)" + showNameHint + "\\b", string.Empty);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Error($"Error in SimplifyFilename for {filename} and {showNameHint}, got {returnFilename} with error {ex.Message}");
            }

            foreach (Match m in Regex.Matches(showNameHint!, "(?:^|[^a-z]|\\b)([0-9]{3,})")
            ) // find >= 3 digit numbers in show name
            {
                if (m.Groups.Count > 1) // just in case
                {
                    string number = m.Groups[1].Value;
                    returnFilename = Regex.Replace(returnFilename, "(^|\\W)" + number + "\\b",
                        ""); // remove any occurrences of that number in the filename
                }
            }

            return returnFilename;
        }

        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp,
            ShowConfiguration? si, IEnumerable<TVSettings.FilenameProcessorRE> rexps, out TVSettings.FilenameProcessorRE? rex)
        {
            string showNameHint = si != null ? si.ShowName : string.Empty;
            maxEp = -1;
            seas = -1;
            ep = -1;
            rex = null;

            filename = SimplifyFilename(filename, showNameHint);

            string fullPath = directory.HasValue()
                ? directory + Path.DirectorySeparatorChar + filename
                : filename; // construct full path with sanitised filename

            fullPath = fullPath.ToLower() + " ";

            foreach (TVSettings.FilenameProcessorRE re in rexps.Where(re => re.Enabled))
            {
                try
                {
                    string pathToTest = re.UseFullPath ? fullPath : filename.ToLower() + " ";

                    Match m = Regex.Match(pathToTest, re.RegExpression, RegexOptions.IgnoreCase);

                    if (m.Success)
                    {
                        (seas, ep, maxEp) = IdentifyEpisode(si, m, re);

                        rex = re;
                        if (seas != -1 && ep != -1)
                        {
                            return true;
                        }
                    }
                }
                catch (FormatException fe)
                {
                    Logger.Warn($"Please check for the regex {re.RegExpression} as it's causing an FormatException error {fe.Message}");
                }
                catch (ArgumentException ae)
                {
                    Logger.Warn($"Please check for the regex {re.RegExpression} as it's causing an ArgumentException error {ae.Message}");
                }
            }

            return seas != -1 && ep != -1;
        }

        private static (int seas, int ep, int maxEp) IdentifyEpisode(ShowConfiguration? si, [NotNull] Match m, TVSettings.FilenameProcessorRE re)
        {
            if (!int.TryParse(m.Groups["s"].ToString(), out int seas))
            {
                if (!re.RegExpression.Contains("<s>") && (si?.AppropriateSeasons().Count ?? 0) == 1)
                {
                    seas = 1;
                }
                else
                {
                    seas = -1;
                }
            }

            if (!int.TryParse(m.Groups["e"].ToString(), out int ep))
            {
                ep = -1;
            }

            if (!int.TryParse(m.Groups["f"].ToString(), out int maxEp))
            {
                maxEp = -1;
            }

            return (seas, ep, maxEp);
        }

        [NotNull]
        public static string RemoveSeriesEpisodeIndicators([NotNull] string hint, [NotNull] IEnumerable<string> seasonWords)
        {
            string hint2 = hint.RemoveDiacritics();
            hint2 = RemoveSe(hint2);
            hint2 = hint2.ToLower();
            hint2 = hint2.Replace("'", "");
            hint2 = hint2.Replace("&", "and");
            hint2 = Regex.Replace(hint2, "[_\\W]+", " ");
            foreach (string term in TVSettings.Instance.AutoAddIgnoreSuffixesArray)
            {
                hint2 = hint2.RemoveAfter(term);
            }

            foreach (string seasonWord in seasonWords)
            {
                hint2 = hint2.RemoveAfter(seasonWord);
            }

            hint2 = hint2.Trim();
            return hint2;
        }

        [NotNull]
        public static List<T> RemoveShortShows<T>([NotNull] IReadOnlyCollection<T> matchingShows)
            where T : MediaConfiguration
        {
            //Remove any shows from the list that are subsets of all the ohters
            //so that a file does not match CSI and CSI: New York
            return matchingShows.Where(testShow => !IsInferiorTo(testShow, matchingShows)).ToList();
        }

        private static bool IsInferiorTo(MediaConfiguration testShow, [NotNull] IEnumerable<MediaConfiguration> matchingShows)
        {
            return matchingShows.Any(compareShow => IsInferiorTo(testShow, compareShow));
        }

        private static bool IsInferiorTo([NotNull] MediaConfiguration testShow, [NotNull] MediaConfiguration compareShow)
        {
            return compareShow.ShowName.StartsWith(testShow.ShowName, StringComparison.Ordinal) && testShow.ShowName.Length < compareShow.ShowName.Length;
        }

        public static bool BetterShowsMatch(FileInfo matchedFile, MediaConfiguration currentlyMatchedShow,
            bool useFullPath, [NotNull] TVDoc doc)
        {
            if (currentlyMatchedShow is ShowConfiguration currentlyMatchedTvShow)
            {
                IEnumerable<ShowConfiguration> showConfigurations = doc.TvLibrary.Shows
                    .Where(item => item.NameMatch(matchedFile, useFullPath))
                    .Where(item => !HaveACommonId(item, currentlyMatchedTvShow));

                return showConfigurations
                    .Any(testShow => testShow.ShowName.Contains(currentlyMatchedTvShow.ShowName));
            }

            return doc.FilmLibrary.Movies
                .Where(item => item.NameMatch(matchedFile, useFullPath))
                .Where(item => !HaveACommonId(item, currentlyMatchedShow))
                .Any(testShow => testShow.ShowName.Contains(currentlyMatchedShow.ShowName));
        }

        private static bool HaveACommonId(MediaConfiguration item, MediaConfiguration currentlyMatchedTvShow)
        {
            return HaveSameNonZeroId(item, currentlyMatchedTvShow, TVDoc.ProviderType.TheTVDB)
                || HaveSameNonZeroId(item, currentlyMatchedTvShow, TVDoc.ProviderType.TMDB)
                || HaveSameNonZeroId(item, currentlyMatchedTvShow, TVDoc.ProviderType.TVmaze);
        }

        private static bool HaveSameNonZeroId(MediaConfiguration item, MediaConfiguration currentlyMatchedTvShow, TVDoc.ProviderType p)
        {
            return (item.IdFor(p) == currentlyMatchedTvShow.IdFor(p))
                   && (item.IdFor(p) > 0)
                   && (currentlyMatchedTvShow.IdFor(p) > 0);
        }

        private static string RemoveSe(string hint)
        {
            foreach (TVSettings.FilenameProcessorRE re in TVSettings.Instance.FNPRegexs)
            {
                if (!re.Enabled)
                {
                    continue;
                }

                try
                {
                    Match m = Regex.Match(hint, re.RegExpression, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        if (!int.TryParse(m.Groups["s"].ToString(), out int seas))
                        {
                            seas = -1;
                        }

                        if (!int.TryParse(m.Groups["e"].ToString(), out int ep))
                        {
                            ep = -1;
                        }

                        int p = Math.Min(m.Groups["s"].Index, m.Groups["e"].Index);
                        int p2 = Math.Min(p, hint.IndexOf(m.Groups.SyncRoot!.ToString(), StringComparison.Ordinal));

                        if (seas != -1 && ep != -1)
                        {
                            return hint.Remove(p2 != -1 ? p2 : p);
                        }
                    }
                }
                catch (FormatException)
                {
                    //didn't work - just try the next one
                }
                catch (ArgumentException)
                {
                    //didn't work - just try the next one
                }
            }

            return hint;
        }

        private static bool LookForSeries(string test, [NotNull] IEnumerable<MediaConfiguration> shows)
        {
            return shows.Any(si => si.NameMatch(test));
        }

        private static bool LookForMovies(string test, [NotNull] IEnumerable<MediaConfiguration> shows)
        {
            return shows.Any(si => si.NameMatch(test));
        }

        private static bool LookForMovie(FileSystemInfo test, [NotNull] IEnumerable<MovieConfiguration> shows)
        {
            return shows.Any(si => si.NameMatch(test, TVSettings.Instance.UseFullPathNameToMatchSearchFolders));
        }

        private static bool LookForSeries(FileSystemInfo test, [NotNull] IEnumerable<ShowConfiguration> shows)
        {
            return shows.Any(si => si.NameMatch(test, TVSettings.Instance.UseFullPathNameToMatchSearchFolders));
        }

        public static bool IgnoreHint(string hint)
        {
            return TVSettings.Instance.AutoAddMovieTermsArray.Any(term =>
                hint.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        [NotNull]
        public static List<MediaConfiguration> FindMedia([NotNull] IEnumerable<FileInfo> possibleShows, TVDoc doc, IDialogParent owner)
        {
            List<MediaConfiguration> addedShows = new List<MediaConfiguration>();

            foreach (FileInfo file in possibleShows)
            {
                string hint = file.RemoveExtension(TVSettings.Instance.UseFullPathNameToMatchSearchFolders) + ".";

                //remove any search folders  from the hint. They are probbably useless at helping specify the showname
                foreach (var path in TVSettings.Instance.DownloadFolders)
                {
                    if (hint.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                    {
                        hint = hint.RemoveFirst(path.Length);
                    }
                }

                //If the hint contains certain terms then we'll ignore it
                if (TVSettings.Instance.IgnoredAutoAddHints.Contains(hint))
                {
                    Logger.Info(
                        $"Ignoring {hint} as it is in the list of ignored terms the user has selected to ignore from prior Auto Adds.");

                    continue;
                }

                //Remove any (nnnn) in the hint - probably a year
                string refinedHint = Regex.Replace(hint, @"\(\d{4}\)", "");

                //Remove anything we can from hint to make it cleaner and hence more likely to match
                refinedHint = RemoveSeriesEpisodeIndicators(refinedHint, doc.TvLibrary.SeasonWords());

                if (string.IsNullOrWhiteSpace(refinedHint))
                {
                    Logger.Info($"Ignoring {hint} as it refines to nothing.");
                    continue;
                }

                //if hint doesn't match existing added shows
                if (LookForSeries(refinedHint, addedShows))
                {
                    Logger.Info($"Ignoring {hint} as it matches shows already being added.");
                    continue;
                }
                if (LookForMovies(refinedHint, addedShows))
                {
                    Logger.Info($"Ignoring {hint} as it matches existing movies already being added: {addedShows.Where(si => si.NameMatch(refinedHint)).Select(s => s.ShowName).ToCsv()}");
                    continue;
                }

                //if hint doesn't match existing added shows
                if (LookForSeries(refinedHint, doc.TvLibrary.Shows))
                {
                    Logger.Info($"Ignoring {hint} as it matches shows already in the library.");
                    continue;
                }
                if (LookForMovies(refinedHint, doc.FilmLibrary.Movies))
                {
                    Logger.Info($"Ignoring {hint} as it matches existing movies already in the library: {doc.FilmLibrary.Movies.Where(si => si.NameMatch(refinedHint)).Select(s => s.ShowName).ToCsv()}");
                    continue;
                }

                //If there are no LibraryFolders then we cant use the simplified UI
                if (TVSettings.Instance.LibraryFolders.Count + TVSettings.Instance.MovieLibraryFolders.Count == 0)
                {
                    MessageBox.Show(
                        "Please add some monitor (library) folders under 'Bulk Add Shows' to use the 'Auto Add' functionality (Alternatively you can add them or turn it off in settings).",
                        "Can't Auto Add Show", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    continue;
                }

                bool assumeMovie = IgnoreHint(hint) || !file.FileNameNoExt().ContainsAnyCharactersFrom("0123456789");

                if (assumeMovie && TVSettings.Instance.DefMovieDefaultLocation.HasValue() && TVSettings.Instance.DefMovieUseDefaultLocation && TVSettings.Instance.AutomateAutoAddWhenOneMovieFound)
                {
                    //TODO - Make generic, currently uses TMDB only
                    CachedMovieInfo? foundMovie = TMDB.LocalCache.Instance.GetMovie(refinedHint, null, new Locale(), true, true);
                    if (foundMovie != null)
                    {
                        // no need to popup dialog
                        Logger.Info($"Auto Adding New Movie for '{refinedHint}' (directly) : {foundMovie.Name}");

                        MovieConfiguration newMovie = new MovieConfiguration
                        {
                            TmdbCode = foundMovie.TmdbCode,
                            UseAutomaticFolders = true,
                            AutomaticFolderRoot = TVSettings.Instance.DefMovieDefaultLocation ?? string.Empty,
                            Format = MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile,
                            UseCustomFolderNameFormat = false,
                            ConfigurationProvider = TVDoc.ProviderType.TMDB
                        };

                        if (!hint.Contains(foundMovie.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            newMovie.AliasNames.Add(hint);
                        }

                        addedShows.Add(newMovie);
                        doc.Stats().AutoAddedMovies++;
                        continue;
                    }
                }
                //popup dialog
                AutoAddMedia askForMatch = new AutoAddMedia(refinedHint, file, assumeMovie);

                if (askForMatch.SingleTvShowFound && !askForMatch.SingleMovieFound && TVSettings.Instance.AutomateAutoAddWhenOneShowFound)
                {
                    // no need to popup dialog
                    Logger.Info($"Auto Adding New Show for '{refinedHint}' : {askForMatch.ShowConfiguration.CachedShow?.Name}");
                    addedShows.Add(askForMatch.ShowConfiguration);
                    doc.Stats().AutoAddedShows++;
                }
                else if (askForMatch.SingleMovieFound && !askForMatch.SingleTvShowFound && TVSettings.Instance.AutomateAutoAddWhenOneMovieFound)
                {
                    // no need to popup dialog
                    Logger.Info($"Auto Adding New Movie for '{refinedHint}' : {askForMatch.MovieConfiguration.CachedMovie?.Name}");
                    addedShows.Add(askForMatch.MovieConfiguration);
                    doc.Stats().AutoAddedMovies++;
                }
                else
                {
                    Logger.Info($"Auto Adding New Show/Movie by asking about for '{refinedHint}'");
                    owner.ShowChildDialog(askForMatch);
                    DialogResult dr = askForMatch.DialogResult;

                    if (dr == DialogResult.OK)
                    {
                        //If added add show ot collection
                        if (askForMatch.ShowConfiguration.Code > 0)
                        {
                            addedShows.Add(askForMatch.ShowConfiguration);
                            doc.Stats().AutoAddedShows++;
                        }
                        else if (askForMatch.MovieConfiguration.Code > 0)
                        {
                            addedShows.Add(askForMatch.MovieConfiguration);
                            doc.Stats().AutoAddedMovies++;
                        }
                    }
                    else if (dr == DialogResult.Abort)
                    {
                        Logger.Info("Skippng Auto Add Process");
                        break;
                    }
                    else if (dr == DialogResult.Ignore)
                    {
                        Logger.Info($"Permenantly Ignoring 'Auto Add' for: {hint}");
                        TVSettings.Instance.IgnoredAutoAddHints.Add(hint);
                    }
                    else
                    {
                        Logger.Info($"Cancelled Auto adding new show/movie {hint}");
                    }
                }

                askForMatch.Dispose();
            }

            return addedShows;
        }

        public static ShowConfiguration? FindBestMatchingShow([NotNull] FileInfo fi, [NotNull] IEnumerable<ShowConfiguration> shows) => FindBestMatchingShow(fi.Name, shows);

        public static MovieConfiguration? FindBestMatchingMovie([NotNull] FileInfo fi, [NotNull] IEnumerable<MovieConfiguration> shows) => FindBestMatchingShow(fi.Name, shows);

        public static ShowConfiguration? FindBestMatchingShow(string filename, [NotNull] IEnumerable<ShowConfiguration> shows)
        {
            IEnumerable<ShowConfiguration> showItems = shows as ShowConfiguration[] ?? shows.ToArray();

            IEnumerable<ShowConfiguration> showsMatchAtStart = showItems
                .Where(item => FileHelper.SimplifyAndCheckFilenameAtStart(filename, item.ShowName));

            IEnumerable<ShowConfiguration> matchAtStart = showsMatchAtStart as ShowConfiguration[] ?? showsMatchAtStart.ToArray();

            if (matchAtStart.Any())
            {
                return matchAtStart.OrderByDescending(s => s.ShowName.Length).First();
            }

            IEnumerable<ShowConfiguration> otherMatchingShows = FindMatchingShows(filename, showItems);
            return otherMatchingShows.OrderByDescending(s => s.ShowName.Length).FirstOrDefault();
        }

        public static MovieConfiguration? FindBestMatchingShow(string filename, [NotNull] IEnumerable<MovieConfiguration> shows)
        {
            IEnumerable<MovieConfiguration> showItems = shows as MovieConfiguration[] ?? shows.ToArray();

            IEnumerable<MovieConfiguration> showsMatchAtStart = showItems
                .Where(item => FileHelper.SimplifyAndCheckFilenameAtStart(filename, item.ShowName));

            IEnumerable<MovieConfiguration> matchAtStart = showsMatchAtStart as MovieConfiguration[] ?? showsMatchAtStart.ToArray();

            if (matchAtStart.Any())
            {
                return matchAtStart.OrderByDescending(s => s.ShowName.Length).First();
            }

            IEnumerable<MovieConfiguration> otherMatchingShows = FindMatchingShows(filename, showItems);
            return otherMatchingShows.OrderByDescending(s => s.ShowName.Length).FirstOrDefault();
        }

        [NotNull]
        public static IEnumerable<ShowConfiguration> FindMatchingShows([NotNull] FileInfo fi, [NotNull] IEnumerable<ShowConfiguration> sil)
        {
            return FindMatchingShows(fi.Name, sil);
        }

        [NotNull]
        public static IEnumerable<ShowConfiguration> FindMatchingShows(string filename, [NotNull] IEnumerable<ShowConfiguration> sil)
        {
            return sil.Where(item => item.NameMatch(filename));
        }

        public static IEnumerable<MovieConfiguration> FindMatchingShows(string filename, [NotNull] IEnumerable<MovieConfiguration> sil)
        {
            return sil.Where(item => item.NameMatch(filename));
        }

        public static FileInfo GenerateTargetName(ItemMissing mi, FileInfo from)
        {
            if (mi.DoRename && TVSettings.Instance.RenameCheck)
            {
                return new FileInfo(mi.TheFileNoExt + @from.Extension);
            }

            return new FileInfo(mi.DestinationFolder.EnsureEndsWithSeparator() + @from.Name);
        }

        public static FileInfo GenerateTargetName(string folder, ProcessedEpisode pep, FileInfo fi)
        {
            string filename = fi.Name;

            if (pep.Show.DoRename && TVSettings.Instance.RenameCheck)
            {
                filename = TVSettings.Instance.FilenameFriendly(
                    TVSettings.Instance.NamingStyle.NameFor(pep, fi.Extension, folder.Length));
            }

            return new FileInfo(folder.EnsureEndsWithSeparator() + filename);
        }
    }
}
