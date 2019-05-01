// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using Path = System.IO.Path;

namespace TVRename
{
    internal static class FinderHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool FindSeasEp(FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si,
    [CanBeNull] out TVSettings.FilenameProcessorRE re)
        {
            return FindSeasEp(fi, out seas, out ep, out maxEp, si, TVSettings.Instance.FNPRegexs,
                TVSettings.Instance.LookForDateInFilename, out re);
        }

        public static bool FindSeasEp([CanBeNull] FileInfo fi, out int seas, out int ep, out int maxEp, ShowItem si,
            List<TVSettings.FilenameProcessorRE> rexps, bool doDateCheck, [CanBeNull] out TVSettings.FilenameProcessorRE re)
        {
            re = null;
            if (fi is null)
            {
                seas = -1;
                ep = -1;
                maxEp = -1;
                return false;
            }

            if (doDateCheck && FindSeasEpDateCheck(fi, out seas, out ep, out maxEp, si))
            {
                return true;
            }

            string filename = fi.Name;
            int l = filename.Length;
            int le = fi.Extension.Length;
            filename = filename.Substring(0, l - le);
            return FindSeasEp(fi.Directory.FullName, filename, out seas, out ep, out maxEp, si, rexps, out re);
        }

        public static bool FindSeasEpDateCheck([CanBeNull] FileInfo fi, out int seas, out int ep, out int maxEp, [CanBeNull] ShowItem si)
        {
            ep = -1;
            seas = -1;
            maxEp = -1;

            if (fi is null || si is null)
            {
                return false;
            }

            // look for a valid airdate in the filename
            // check for YMD, DMY, and MDY
            // only check against airdates we expect for the given show
            SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TvdbCode);

            if (ser is null)
            {
                return false;
            }

            string[] dateFormats = { "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy", "MM-dd-yy" };
            string filename = fi.Name;
            if (filename is null)
            {
                return false;
            }
            // force possible date separators to a dash
            filename = filename.Replace("/", "-");
            filename = filename.Replace(".", "-");
            filename = filename.Replace(",", "-");
            filename = filename.Replace(" ", "-");

            Dictionary<int, Season> seasonsToUse = si.DvdOrder ? ser.DvdSeasons : ser.AiredSeasons;
            if (seasonsToUse is null)
            {
                return false;
            }

            foreach (KeyValuePair<int, Season> kvp in seasonsToUse)
            {
                if (kvp.Value?.Episodes?.Values is null)
                {
                    continue;
                }

                if (!(si.IgnoreSeasons is null) && (si.IgnoreSeasons.Contains(kvp.Value.SeasonNumber)))
                {
                    continue;
                }

                if ((kvp.Value.SeasonNumber == 0) && TVSettings.Instance.IgnoreAllSpecials)
                {
                    continue;
                }

                foreach (Episode epi in kvp.Value.Episodes.Values)
                {
                    DateTime? dt = epi.GetAirDateDt(); // file will have local timezone date, not ours
                    if (dt is null)
                    {
                        continue;
                    }

                    TimeSpan closestDate = TimeSpan.MaxValue;

                    foreach (string dateFormat in dateFormats)
                    {
                        string datestr = dt.Value.ToString(dateFormat);

                        if (filename.Contains(datestr) && DateTime.TryParseExact(datestr, dateFormat,
                                new CultureInfo("en-GB"), DateTimeStyles.None, out DateTime dtInFilename))
                        {
                            TimeSpan timeAgo = DateTime.Now.Subtract(dtInFilename);

                            if (timeAgo < closestDate)
                            {
                                seas = (si.DvdOrder ? epi.DvdSeasonNumber : epi.AiredSeasonNumber);
                                ep = si.DvdOrder ? epi.DvdEpNum : epi.AiredEpNum;
                                closestDate = timeAgo;
                            }
                        }
                    }
                }
            }

            return ((ep != -1) && (seas != -1));
        }

        public static bool FindSeasEp([CanBeNull] DirectoryInfo di, out int seas, out int ep, ShowItem si,
            [CanBeNull] out TVSettings.FilenameProcessorRE re)
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

        public static bool FindSeasEp(string directory, string filename, out int seas, out int ep, out int maxEp,
            ShowItem si, [NotNull] List<TVSettings.FilenameProcessorRE> rexps)
        {
            return FindSeasEp(directory, filename, out seas, out ep, out maxEp, si, rexps, out TVSettings.FilenameProcessorRE _);
        }

        public static bool FindSeasEp(string itemName, out int seas, out int ep, out int maxEp, ShowItem show)
        {
            return FindSeasEp(string.Empty, itemName, out seas, out ep, out maxEp, show, TVSettings.Instance.FNPRegexs, out TVSettings.FilenameProcessorRE _);
        }

        public static bool FindSeasEp(FileInfo theFile, out int seasF, out int epF, out int maxEp, ShowItem sI)
        {
            return FindSeasEp(theFile, out seasF, out epF, out maxEp, sI, out TVSettings.FilenameProcessorRE _);
        }

        public static bool FileNeeded(FileInfo fi, ShowItem si, DirFilesCache dfc)
        {
            if (FindSeasEp(fi, out int seasF, out int epF, out _, si, out _))
            {
                return EpisodeNeeded(si, dfc, seasF, epF, fi);
            }

            //We may need the file
            return true;
        }

        public static bool FileNeeded([NotNull] DirectoryInfo di, [NotNull] ShowItem si, DirFilesCache dfc)
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

        [NotNull]
        public static List<FileInfo> FindEpOnDisk(this DirFilesCache dfc, [NotNull] ProcessedEpisode pe,
            bool checkDirectoryExist = true)
        {
            return FindEpOnDisk(dfc, pe.Show, pe, checkDirectoryExist);
        }

        [NotNull]
        public static List<FileInfo> FindEpOnDisk([CanBeNull] DirFilesCache dfc, [NotNull] ShowItem si, [NotNull] Episode epi,
            bool checkDirectoryExist = true)
        {
            DirFilesCache cache = dfc ?? new DirFilesCache();

            List<FileInfo> ret = new List<FileInfo>();

            int seasWanted = si.DvdOrder ? epi.TheDvdSeason.SeasonNumber : epi.TheAiredSeason.SeasonNumber;
            int epWanted = si.DvdOrder ? epi.DvdEpNum : epi.AiredEpNum;

            int snum = seasWanted;

            Dictionary<int, List<string>> dirs = si.AllFolderLocationsEpCheck(checkDirectoryExist);

            if (!dirs.ContainsKey(snum))
            {
                return ret;
            }

            foreach (string folder in dirs[snum])
            {
                FileInfo[] files = cache.GetFiles(folder);
                if (files is null)
                {
                    continue;
                }

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

                    if ((seasFound == seasWanted) && (epFound == epWanted))
                    {
                        ret.Add(fiTemp);
                    }
                }
            }

            return ret;
        }

        public static bool EpisodeNeeded([NotNull] ShowItem si, DirFilesCache dfc, int seasF, int epF,
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
                SeriesInfo s = si.TheSeries();
                if (s is null)
                {
                    //We have not downloaded the series, so have to assume that we need the episode/file
                    return true;
                }

                Episode ep = s.GetEpisode(seasF, epF, si.DvdOrder);
                ProcessedEpisode pep = new ProcessedEpisode(ep, si);

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
            catch (SeriesInfo.EpisodeNotFoundException)
            {
                //Ignore exception, we may need the file
                return true;
            }
            return true;
        }

        [NotNull]
        public static string SimplifyFilename([NotNull] string filename, [CanBeNull] string showNameHint)
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

                bool nameIsNumber = (Regex.Match(showNameHint, "^[0-9]+$").Success);

                if (nameIsNumber && returnFilename.Contains(showNameHint)) // e.g. "24", or easy exact match of show name at start of filename
                {
                    return Regex.Replace(returnFilename, "(^|\\W)" + showNameHint + "\\b", string.Empty);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Error($"Error in SimplifyFilename for {filename} and {showNameHint}, got {returnFilename} with error {ex.Message}");
            }

            foreach (Match m in Regex.Matches(showNameHint, "(?:^|[^a-z]|\\b)([0-9]{3,})")
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
            [CanBeNull] ShowItem si, [NotNull] IEnumerable<TVSettings.FilenameProcessorRE> rexps, [CanBeNull] out TVSettings.FilenameProcessorRE rex)
        {
            string showNameHint = (si != null) ? si.ShowName : string.Empty;
            maxEp = -1;
            seas = -1;
            ep = -1;
            rex = null;

            filename = SimplifyFilename(filename, showNameHint);

            string fullPath =
                directory + Path.DirectorySeparatorChar +
                filename; // construct full path with sanitised filename

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
                        if ((seas != -1) && (ep != -1))
                        {
                            return true;
                        }
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return ((seas != -1) && (ep != -1));
        }

        private static (int seas, int ep, int maxEp) IdentifyEpisode(ShowItem si, [NotNull] Match m, TVSettings.FilenameProcessorRE re)
        {
            if (!int.TryParse(m.Groups["s"].ToString(), out int seas))
            {
                if (!re.RegExpression.Contains("<s>") && si.AppropriateSeasons()?.Count == 1)
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
                        int p2 = Math.Min(p, hint.IndexOf(m.Groups.SyncRoot.ToString(), StringComparison.Ordinal));

                        if (seas != -1 && ep != -1)
                        {
                            return hint.Remove(p2 != -1 ? p2 : p);
                        }
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            return hint;
        }
    }
}
