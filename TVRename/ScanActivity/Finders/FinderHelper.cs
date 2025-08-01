//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Alphaleonis.Win32.Filesystem;
using NLog;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Path = System.IO.Path;

namespace TVRename;

internal static class FinderHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowConfiguration? si, out TVSettings.FilenameProcessorRE? re)
        => FindSeasEp(fi, out seas, out ep, out maxEp, si, TVSettings.Instance.FNPRegexs, out re);

    public static bool FindSeasEp(string itemName, out int seas, out int ep, out int maxEp, ShowConfiguration? si, IEnumerable<TVSettings.FilenameProcessorRE> rexps, out TVSettings.FilenameProcessorRE? re)
        => FindSeasEp(string.Empty, itemName, out seas, out ep, out maxEp, si, rexps, out re);

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

    public static bool MatchesSequentialNumber(string filename, ProcessedEpisode pe)
    {
        if (pe.OverallNumber == -1)
        {
            return false;
        }

        string matchText = "X " + filename + " X"; // need to pad to let it match non-numbers at start and end
        string[] regexes =
        {
            @"\D\s(E|e|Ep|ep|episode|Episode)\s?0*(?<sequencenumber>\d+)\s\D",
            @"\D\s0*(?<sequencenumber>\d+)\s\D",
            @"\D\s0*(?<sequencenumber>\d+)v\d+\s\D",
        };

        return regexes
            .Select(regex => Regex.Match(matchText, regex))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups["sequencenumber"].Value) == pe.OverallNumber)
            .FirstOrDefault();
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
        CachedSeriesInfo? ser = si.CachedShow;

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
            LocalDateTime? dt = epi.LocalAirTime(); // file will have local timezone date, not ours
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
                    TimeSpan timeAgo = TimeHelpers.LocalNow().Subtract(dtInFilename);

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
        return FindSeasEp(string.Empty, itemName, out seas, out ep, out maxEp, show, TVSettings.Instance.FNPRegexs, out TVSettings.FilenameProcessorRE? _);
    }

    public static bool FindSeasEp(FileInfo theFile, out int seasF, out int epF, out int maxEp, ShowConfiguration? sI)
    {
        return FindSeasEp(theFile, out seasF, out epF, out maxEp, sI, out TVSettings.FilenameProcessorRE? _);
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

    /// <exception cref="ArgumentNullException"><paramref name="di"/> is <see langword="null"/></exception>
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

    /// <exception cref="ArgumentNullException"><paramref name="di"/> is <see langword="null"/></exception>
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
        return si.Locations
            .SelectMany(cache.GetFiles)
            .Where(fiTemp => fiTemp.IsMovieFile())
            .Where(fiTemp => si.NameMatch(fiTemp, false));
    }

    public static List<FileInfo> FindEpOnDisk(this DirFilesCache? dfc, ProcessedEpisode pe,
        bool checkDirectoryExist = true)
    {
        return FindEpOnDisk(dfc, pe.Show, pe, checkDirectoryExist);
    }

    private static List<FileInfo> FindEpOnDisk(DirFilesCache? dfc, ShowConfiguration si, ProcessedEpisode epi,
        bool checkDirectoryExist = true)
    {
        DirFilesCache cache = dfc ?? new DirFilesCache();

        List<FileInfo> ret = [];

        int seasWanted = epi.AppropriateSeasonNumber;
        int epWanted = epi.AppropriateEpNum;

        int snum = seasWanted;

        Dictionary<int, SafeList<string>> dirs = si.AllFolderLocationsEpCheck(checkDirectoryExist);

        if (!dirs.TryGetValue(snum, out SafeList<string>? folders))
        {
            return ret;
        }

        foreach (FileInfo fiTemp in folders
                     .Select(folder => cache.GetFiles(folder))
                     .SelectMany(files => files.Where(IsMovieFile)))
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

        return ret;
    }

    private static bool IsMovieFile(FileInfo f) => f.IsMovieFile();

    private static bool EpisodeNeeded(ShowConfiguration si, DirFilesCache dfc, int seasF, int epF,
        FileSystemInfo fi)
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
            CachedSeriesInfo? s = si.CachedShow;
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
        catch (EpisodeNotFoundException)
        {
            //Ignore exception, we may need the file
            return true;
        }
        return true;
    }

    private static string SimplifyFilename(string filename, string? showNameHint)
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
            if (returnFilename.StartsWith(showNameHint, StringComparison.Ordinal))
            {
                return returnFilename.Remove(0, showNameHint.Length);
            }

            if (showNameHint.IsNumeric() && returnFilename.Contains(showNameHint)) // e.g. "24", or easy exact match of show name at start of filename
            {
                return Regex.Replace(returnFilename, "(^|\\W)" + showNameHint + "\\b", string.Empty);
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Logger.Error($"Error in SimplifyFilename for {filename} and {showNameHint}, got {returnFilename} with error {ex.ErrorText()}");
        }

        foreach (Match m in Regex.Matches(showNameHint, "(?:^|[^a-z]|\\b)([0-9]{3,})").Cast<Match>()) // find >= 3 digit numbers in show name
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

    private static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp,
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
                Logger.Warn($"Please check for the regex {re.RegExpression} as it's causing an FormatException error {fe.ErrorText()}");
            }
            catch (ArgumentException ae)
            {
                Logger.Warn($"Please check for the regex {re.RegExpression} as it's causing an ArgumentException error {ae.ErrorText()}");
            }
        }

        return seas != -1 && ep != -1;
    }

    private static (int seas, int ep, int maxEp) IdentifyEpisode(ShowConfiguration? si, Match m, TVSettings.FilenameProcessorRE re)
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

    public static string RemoveSeriesEpisodeIndicators(string hint, IEnumerable<string> seasonWords)
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

    public static List<T> RemoveShortShows<T>(IReadOnlyCollection<T> matchingShows)
        where T : MediaConfiguration
    {
        //Remove any shows from the list that are subsets of all the ohters
        //so that a file does not match CSI and CSI: New York
        return [.. matchingShows.Where(testShow => !IsInferiorTo(testShow, matchingShows))];
    }

    public static List<T> RemoveShortMedia<T>(IEnumerable<T> matchingMovies, IEnumerable<MediaConfiguration> matchingShows) where T : MediaConfiguration
    {
        return [.. matchingMovies.Where(testShow => !IsContenedTo(testShow, matchingShows))];
    }

    private static bool IsInferiorTo(MediaConfiguration testShow, IEnumerable<MediaConfiguration> matchingShows)
    {
        return matchingShows.Any(compareShow => IsInferiorTo(testShow, compareShow));
    }

    private static bool IsInferiorTo(MediaConfiguration testShow, MediaConfiguration compareShow)
    {
        return compareShow.ShowName.StartsWith(testShow.ShowName, StringComparison.Ordinal) && testShow.ShowName.Length < compareShow.ShowName.Length;
    }

    private static bool IsContenedTo(MediaConfiguration testShow, IEnumerable<MediaConfiguration> matchingShows)
    {
        return matchingShows.Any(compareShow => IsContenedTo(testShow, compareShow));
    }

    private static bool IsContenedTo(MediaConfiguration testShow, MediaConfiguration compareShow)
    {
        return compareShow.ShowName.CompareName().Contains(testShow.ShowName.CompareName(), StringComparison.Ordinal) && testShow.ShowName.Length < compareShow.ShowName.Length;
    }

    public static bool BetterShowsMatch(FileInfo matchedFile, MediaConfiguration currentlyMatchedShow,
        bool useFullPath, TVDoc doc)
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
        return item.IdFor(p) == currentlyMatchedTvShow.IdFor(p)
               && item.IdFor(p) > 0
               && currentlyMatchedTvShow.IdFor(p) > 0;
    }

    private static string RemoveSe(string hint)
    {
        foreach (TVSettings.FilenameProcessorRE re in TVSettings.Instance.FNPRegexs.Where(re => re.Enabled))
        {
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
                    int p2 = Math.Min(p, hint.IndexOf(m.Groups["0"].ToString(), StringComparison.Ordinal));

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

    private static bool LookForSeries(string test, IEnumerable<MediaConfiguration> shows)
        => GetMatchingSeries(test,shows).Any();

    private static IEnumerable<MediaConfiguration> GetMatchingSeries(string test, IEnumerable<MediaConfiguration> shows)
        => shows.Where(si => si.NameMatch(test));

    private static bool LookForMovies(string test, IEnumerable<MediaConfiguration> shows)
        => GetMatchingMovies(test,shows).Any();

    private static IEnumerable<MediaConfiguration> GetMatchingMovies(string test, IEnumerable<MediaConfiguration> shows)
    {
        return shows.Where(si => si.NameMatch(test));
    }
    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private static bool LookForMovie(FileSystemInfo test, IEnumerable<MovieConfiguration> shows)
#pragma warning restore IDE0051 // Remove unused private members
    {
        return shows.Any(si => si.NameMatch(test, TVSettings.Instance.UseFullPathNameToMatchSearchFolders));
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private static bool LookForSeries(FileSystemInfo test, IEnumerable<ShowConfiguration> shows)
#pragma warning restore IDE0051 // Remove unused private members
    {
        return shows.Any(si => si.NameMatch(test, TVSettings.Instance.UseFullPathNameToMatchSearchFolders));
    }

    public static (string title, int? year) SplitIntoTitleYear(string hint)
    {
        const string PATTERN = @"\s(\d{4})$";
        Match m = Regex.Match(hint.Trim(), PATTERN);
        int? possibleYear = null;
        if (m.Success)
        {
            //Seems like we have a year in the date

            //Work out the year
            possibleYear = m.Groups[1].Value.ToInt();

            //remove year from string
            hint = Regex.Replace(hint.Trim(), PATTERN, " ");
        }

        return (hint, possibleYear);
    }

    public static string RemoveSceneTerms(string refinedHint)
    {
        List<string> removeCrapAfterTerms =
            [
                "2160p",
                "1080p",
                "720p",
                "480p",
                "dvdrip",
                "webrip",
                "brrip",
                "r5",
                "BDrip",
                "limited",
                "dvdscr",
                "unrated",
                "tv",
                "bluray",
                "hdrip",
                "3d",
                "xvid",
                "r6rip"
            ];

        foreach (string removeCrapAfterTerm in removeCrapAfterTerms)
        {
            if (refinedHint.Contains(removeCrapAfterTerm))
            {
                string pattern2 = @"(?:^|\s|$)" + Regex.Escape(removeCrapAfterTerm) + @"(?:^|\s|$)";
                Match match = Regex.Match(refinedHint, pattern2);
                if (match.Success)
                {
                    refinedHint = refinedHint.RemoveAfter(removeCrapAfterTerm);
                }
            }
        }

        return refinedHint;
    }

    public static IEnumerable<PossibleMedia> FindMedia(IEnumerable<FileInfo> possibleShows,
        TVDoc doc, IDialogParent owner)
    {
        List<PossibleMedia> addedShows = [];
        try
        {
            foreach (FileInfo file in possibleShows)
            {
                FindMedia(file, addedShows, doc, owner);
            }

            return addedShows;
        }
        catch (AutoAddCancelledException)
        {
            return addedShows;
        }
    }

    private static void FindMedia(FileInfo file, List<PossibleMedia> addedShows, TVDoc doc, IDialogParent owner)
    {
        //If the hint contains certain terms then we'll ignore it
        if (TVSettings.Instance.IgnoredAutoAddHints.Contains(file.RemoveExtension()))
        {
            Logger.Info(
                $"Ignoring {file.RemoveExtension()} as it is in the list of ignored terms the user has selected to ignore from prior Auto Adds.");

            return;
        }

        string filehint = file.RemoveExtension() + ".";
        string dirhint = RemoveDownloadFolders(file.DirectoryName);

        filehint = RemoveSceneTerms(filehint.CompareName()).RemoveBracketedYear().RemoveYearFromEnd();
        dirhint = RemoveSceneTerms(dirhint.CompareName()).RemoveBracketedYear().RemoveYearFromEnd();

        string hint = filehint == dirhint ? filehint
            : TVSettings.Instance.UseFullPathNameToMatchSearchFolders ? dirhint + " " + filehint
            : filehint;

        string refinedHint = hint.RemoveBracketedYear();

        //Remove anything we can from hint to make it cleaner and hence more likely to match
        refinedHint = RemoveSeriesEpisodeIndicators(refinedHint, doc.TvLibrary.SeasonWords());

        if (string.IsNullOrWhiteSpace(refinedHint))
        {
            Logger.Info($"Ignoring {hint} as it refines to nothing.");
            return;
        }

        //if hint doesn't match existing added shows
        List<MediaConfiguration> showConfigurations = [.. addedShows.Select(x => x.Configuration)];

        if (LookForSeries(refinedHint, showConfigurations))
        {
            Logger.Info($"Ignoring {hint}({refinedHint}) as it matches shows already being added. ({GetMatchingSeries(refinedHint, showConfigurations).Select(s => s.Name).ToCsv()}) already being added.");
            return;
        }

        if (LookForMovies(refinedHint, showConfigurations))
        {
            Logger.Info(
                $"Ignoring {hint}({refinedHint}) as it matches existing movies ({GetMatchingMovies(refinedHint, showConfigurations).Select(s => s.Name).ToCsv()}) already being added.");

            return;
        }

        //if hint doesn't match existing added shows
        if (LookForSeries(refinedHint, doc.TvLibrary.Shows))
        {
            Logger.Warn($"Ignoring {hint}({refinedHint}) as it matches shows ({GetMatchingSeries(refinedHint,doc.TvLibrary.Shows).Select(s=>s.Name).ToCsv()}) already in the library.");
            return;
        }

        if (LookForMovies(refinedHint, doc.FilmLibrary.Movies))
        {
            Logger.Warn(
                $"Ignoring {hint}({refinedHint}) as it matches existing movies ({GetMatchingMovies(refinedHint, doc.FilmLibrary.Movies).Select(s => s.Name).ToCsv()}) already in the library.");

            return;
        }

        //If there are no LibraryFolders then we can't use the simplified UI
        if (TVSettings.Instance.LibraryFolders.Count + TVSettings.Instance.MovieLibraryFolders.Count == 0)
        {
            MessageBox.Show(
                "Please add some monitor (library) folders under 'Bulk Add TV Shows' to use the 'Auto Add' functionality (Alternatively you can add them or turn it off in settings).",
                "Can't Auto Add Media", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return;
        }

        bool assumeMovie = GuessType(refinedHint, hint, file, doc.TvLibrary.SeasonWords()) !=
                           MediaConfiguration.MediaType.tv;

        Logger.Info($"Assuming {file.Name} ({refinedHint}) is a " + (assumeMovie
            ? "movie."
            : "TV Series."));

        if (assumeMovie && TVSettings.Instance.DefMovieDefaultLocation.HasValue() &&
            TVSettings.Instance.DefMovieUseDefaultLocation && TVSettings.Instance.AutomateAutoAddWhenOneMovieFound)
        {
            //TODO - Make generic, currently uses TMDB only
            CachedMovieInfo? foundMovie = TMDB.LocalCache.Instance.GetMovie(refinedHint, null, new Locale(), true, true);
            if (foundMovie != null)
            {
                // no need to popup dialog
                Logger.Info($"Auto Adding New Movie for '{refinedHint}' (directly) : {foundMovie.Name}");

                MovieConfiguration newMovie = new()
                {
                    TmdbCode = foundMovie.TmdbCode,
                    UseAutomaticFolders = true,
                    AutomaticFolderRoot = TVSettings.Instance.DefMovieDefaultLocation ?? string.Empty,
                    Format = MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile,
                    UseCustomFolderNameFormat = false,
                    ConfigurationProvider = foundMovie.Source
                };

                if (!hint.Contains(foundMovie.Name, StringComparison.OrdinalIgnoreCase))
                {
                    newMovie.AliasNames.Add(hint);
                }

                addedShows.Add(new PossibleMedia(newMovie,refinedHint));
                doc.Stats().AutoAddedMovies++;
                return;
            }
        }

        //popup dialog
        using AutoAddMedia askForMatch = new(refinedHint, file, assumeMovie);

        if (askForMatch.SingleTvShowFound && !askForMatch.SingleMovieFound &&
            TVSettings.Instance.AutomateAutoAddWhenOneShowFound)
        {
            // no need to popup dialog
            Logger.Info($"Auto Adding New Show for '{refinedHint}' : {askForMatch.ShowConfiguration.CachedShow?.Name}");
            addedShows.Add(new PossibleMedia(askForMatch.ShowConfiguration,refinedHint));
            doc.Stats().AutoAddedShows++;
        }
        else if (askForMatch.SingleMovieFound && !askForMatch.SingleTvShowFound &&
                 TVSettings.Instance.AutomateAutoAddWhenOneMovieFound)
        {
            // no need to popup dialog
            Logger.Info($"Auto Adding New Movie for '{refinedHint}' : {askForMatch.MovieConfiguration.CachedMovie?.Name}");
            addedShows.Add(new PossibleMedia(askForMatch.MovieConfiguration, refinedHint));
            doc.Stats().AutoAddedMovies++;
        }
        else
        {
            Logger.Info($"Auto Adding New Show/Movie by asking about for '{refinedHint}'");
            owner.ShowChildDialog(askForMatch);
            DialogResult dr = askForMatch.DialogResult;

            if (dr == DialogResult.OK)
            {
                //If added add show to the collection
                if (askForMatch.ShowConfiguration.Code > 0)
                {
                    addedShows.Add(new PossibleMedia(askForMatch.ShowConfiguration,refinedHint));
                    doc.Stats().AutoAddedShows++;
                }
                else if (askForMatch.MovieConfiguration.Code > 0)
                {
                    addedShows.Add(new PossibleMedia(askForMatch.MovieConfiguration,refinedHint));
                    doc.Stats().AutoAddedMovies++;
                }
            }
            else if (dr == DialogResult.Abort)
            {
                Logger.Info("Skippng Auto Add Process");
                throw new AutoAddCancelledException();
            }
            else if (dr == DialogResult.Ignore)
            {
                Logger.Info($"Permenantly Ignoring 'Auto Add' for: {hint}");
                TVSettings.Instance.IgnoredAutoAddHints.Add(file.RemoveExtension());
            }
            else
            {
                Logger.Info($"Cancelled Auto adding new show/movie {hint}");
            }
        }
    }

    public static string RemoveDownloadFolders(string hint)
    {
        //remove any search folders  from the hint. They are probably useless at helping specify the show's name
        foreach (string path in TVSettings.Instance.DownloadFolders)
        {
            if (hint.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            {
                hint = hint.RemoveFirst(path.Length);
            }
        }

        return hint;
    }

    private static MediaConfiguration.MediaType GuessType(string refinedHint, string hint, FileInfo file,
        IEnumerable<string> seasonWords)
    {
        //assuming we don't have a match so far, so we'll make a guess
        if (hint != RemoveSe(hint))
        {
            //we have removed some text from the hint from the TV finders, so assume its a TV show
            return MediaConfiguration.MediaType.tv;
        }

        if (seasonWords.Any(word => file.FullName.Contains(word)))
        {
            return MediaConfiguration.MediaType.tv;
        }

        if (TVSettings.Instance.AutoAddMovieTermsArray.Any(term =>
                file.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
        {
            return MediaConfiguration.MediaType.movie;
        }

        Logger.Warn($"Could not identify type of media for {file.Name}, based on '{hint}' and '{refinedHint}'");
        return MediaConfiguration.MediaType.both;
    }

    public static ShowConfiguration? FindBestMatchingShow(FileInfo fi, IEnumerable<ShowConfiguration> shows) => FindBestMatchingShow(fi.Name, shows);

    public static MovieConfiguration? FindBestMatchingMovie(FileInfo fi, IEnumerable<MovieConfiguration> shows) => FindBestMatchingShow(fi.Name, shows);

    public static ShowConfiguration? FindBestMatchingShow(string filename, IEnumerable<ShowConfiguration> shows)
    {
        IEnumerable<ShowConfiguration> showItems = shows as ShowConfiguration[] ?? [.. shows];

        IEnumerable<ShowConfiguration> showsMatchAtStart = showItems
            .Where(item => FileHelper.SimplifyAndCheckFilenameAtStart(filename, item.ShowName));

        IEnumerable<ShowConfiguration> matchAtStart = showsMatchAtStart as ShowConfiguration[] ?? [.. showsMatchAtStart];

        if (matchAtStart.Any())
        {
            return matchAtStart.MaxBy(s => s.ShowName.Length);
        }

        IEnumerable<ShowConfiguration> otherMatchingShows = FindMatchingShows(filename, showItems);
        return otherMatchingShows.MaxBy(s => s.ShowName.Length);
    }

    private static MovieConfiguration? FindBestMatchingShow(string filename, IEnumerable<MovieConfiguration> shows)
    {
        IEnumerable<MovieConfiguration> showItems = shows as MovieConfiguration[] ?? [.. shows];

        IEnumerable<MovieConfiguration> showsMatchAtStart = showItems
            .Where(item => FileHelper.SimplifyAndCheckFilenameAtStart(filename, item.ShowName));

        IEnumerable<MovieConfiguration> matchAtStart = showsMatchAtStart as MovieConfiguration[] ?? [.. showsMatchAtStart];

        if (matchAtStart.Any())
        {
            return matchAtStart.MaxBy(s => s.ShowName.Length);
        }

        IEnumerable<MovieConfiguration> otherMatchingShows = FindMatchingMovies(filename, showItems);
        return otherMatchingShows.MaxBy(s => s.ShowName.Length);
    }

    public static IEnumerable<ShowConfiguration> FindMatchingShows(FileInfo fi, IEnumerable<ShowConfiguration> sil)
        => FindMatchingShows(fi.Name, sil);

    public static IEnumerable<ShowConfiguration> FindMatchingShows(string filename, IEnumerable<ShowConfiguration> sil)
        => sil.Where(item => item.NameMatch(filename));

    private static IEnumerable<MovieConfiguration> FindMatchingMovies(string filename, IEnumerable<MovieConfiguration> sil)
        => sil.Where(item => item.NameMatch(filename));

    public static FileInfo GenerateTargetName(ItemMissing mi, FileInfo from)
    {
        if (mi.DoRename && TVSettings.Instance.RenameCheck)
        {
            return new FileInfo(mi.TheFileNoExt + from.Extension);
        }

        return new FileInfo(mi.DestinationFolder.EnsureEndsWithSeparator() + from.Name);
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

internal class AutoAddCancelledException : Exception
{
}
