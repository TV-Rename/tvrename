//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NodaTime;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TVRename.Utility
{
    internal class FileMatcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private List<TVSettings.FilenameProcessorRE> regexesToUse = TVSettings.Instance.FNPRegexs;
        private ShowConfiguration? show;
        private ProcessedEpisode? episode;
        public FileMatcher()
        {
        }
        public FileMatcher SetRegEx(List<TVSettings.FilenameProcessorRE> regexes)
        {
            regexesToUse = regexes;
            return this;
        }
        public FileMatcher ResetRegex()
        {
            regexesToUse = TVSettings.Instance.FNPRegexs;
            return this;
        }

        public FileMatchResult VerifyMatchesEpisode(string name, ProcessedEpisode episode)
        {
            this.episode = episode;
            this.show = episode.Show;

            int targetSeason = episode.AppropriateSeasonNumber;
            int targetEpisode = episode.AppropriateEpNum;

            if (episode.Show.UseAirDateMatch)
            {
                var x = MatchUsingEpisodeDate(name);
                if (x.IsMatch && x.FoundEpisode == targetEpisode && x.FoundSeason == targetSeason)
                {
                    return x;
                }
            }

            var Y = MatchUsingRegEx(name,string.Empty);
            if (Y.IsMatch)
            {
                if (Y.FoundSeason == targetSeason && Y.FoundEpisode == targetEpisode)
                {
                    return Y;
                }
            }

            if (episode.Show.UseSequentialMatch)
            {
                var x = MatchUsingSequentialNumber(name);
                if (x.IsMatch)
                {
                    return x;
                }
            }

            if (episode.Show.UseEpNameMatch)
            {
                var x = MatchUsingEpisodeName(name);
                if (x.IsMatch)
                {
                    if (x.FoundEpisode == targetEpisode && x.FoundSeason == targetSeason)
                    {
                        return x;
                    }
                }
            }

            return FileMatchResult.None;
        }

        public FileMatchResult VerifyMatchesEpisode(FileInfo dce, ProcessedEpisode episode)
        {
            this.episode = episode;
            this.show = episode.Show;

            int targetSeason = episode.AppropriateSeasonNumber;
            int targetEpisode = episode.AppropriateEpNum;

            if (episode.Show.UseAirDateMatch)
            {
                var x = MatchUsingEpisodeDate(dce.Name);
                if (x.IsMatch && x.FoundEpisode == targetEpisode && x.FoundSeason == targetSeason)
                {
                    return x;
                }
            }

            var Y = MatchUsingRegEx(dce);
            if (Y.IsMatch)
            {
                if (Y.FoundSeason == targetSeason && Y.FoundEpisode == targetEpisode)
                {
                    return Y;
                }
            }

            if (episode.Show.UseSequentialMatch)
            {
                var x = MatchUsingSequentialNumber(dce.RemoveExtension(false));
                if (x.IsMatch)
                {
                    return x;
                }
            }

            if (episode.Show.UseEpNameMatch)
            {
                var x = MatchUsingEpisodeName(dce);
                if (x.IsMatch)
                {
                    if (x.FoundEpisode == targetEpisode && x.FoundSeason == targetSeason)
                    {
                        return x;
                    }
                }
            }

            return FileMatchResult.None;
        }

        public FileMatchResult Match(string testString, ShowConfiguration? show)
        {
            this.show = show;
            return MatchInternal(testString, string.Empty);
        }

        public FileMatchResult Match(FileInfo? fileToTest, ShowConfiguration? show)
        {
            this.show = show;
            if (fileToTest is null)
            {
                return FileMatchResult.None;
            }
            return MatchInternal(fileToTest.RemoveExtension(), fileToTest.Directory.FullName);
        }
        public FileMatchResult Match(DirectoryInfo? directoryToTest, ShowConfiguration? show)
        {
            this.show = show;
            if (directoryToTest is null)
            {
                return FileMatchResult.None;
            }
            return MatchInternal(directoryToTest.Name, directoryToTest.Parent.FullName);
        }

        public FileMatchResult Match(string testString)
        {
            return MatchUsingRegEx(testString,string.Empty);
        }
        public FileMatchResult Match(FileInfo fileToTest)
        {
            return MatchUsingRegEx(fileToTest.RemoveExtension(), fileToTest.Directory.FullName);
        }

        private FileMatchResult MatchInternal(string fileString, string folderString)
        {
            if (show is null)
            {
                throw new InvalidOperationException("Show must be set before calling Match.");
            }

            if (show.UseAirDateMatch)
            {
                var x = MatchUsingEpisodeDate(fileString);
                if (x.IsMatch)
                {
                    return x;
                }
            }

            var y = MatchUsingRegEx(fileString, folderString);
            if (y.IsMatch)
            {
                return y;
            }

            if (show.UseEpNameMatch)
            {
                var x = MatchUsingEpisodeName(fileString);
                if (x.IsMatch)
                {
                    return x;
                }
            }

            return FileMatchResult.None;
        }

        private FileMatchResult MatchUsingRegEx(FileInfo? fi)
        {
            if (fi is null) return FileMatchResult.None;

            return MatchUsingRegEx(fi.RemoveExtension(), fi.Directory.FullName);
        }

        private FileMatchResult MatchUsingRegEx(string fileString, string folderString) 
        {
            string filename = FinderHelper.SimplifyFilename(fileString, show?.ShowName);

            string fullPath = folderString.HasValue()
                ? folderString + Path.DirectorySeparatorChar + filename
                : filename; // construct full path with sanitised filename

            fullPath = fullPath.ToLower() + " ";
            filename = filename.ToLower() + " ";

            foreach (TVSettings.FilenameProcessorRE re in regexesToUse.Where(re => re.Enabled))
            {
                try
                {
                    string pathToTest = re.UseFullPath ? fullPath : filename;

                    Match m = Regex.Match(pathToTest, re.RegExpression, RegexOptions.IgnoreCase);

                    if (m.Success)
                    {

                        (int? seas, int? ep, int? maxEp) = IdentifyEpisode(show, m, re);

                        if (seas.HasValue && ep.HasValue)
                        {
                            return new FileMatchResult
                            {
                                IsMatch = true,
                                FoundSeason = seas,
                                FoundEpisode = ep,
                                FoundMaxEpisode = maxEp,
                                MatchReason = $"Matched regex {re.RegExpression} with {pathToTest} - {re.Notes}"
                            };
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

            return FileMatchResult.None;
        }

        private static (int? seas, int? ep, int? maxEp) IdentifyEpisode(ShowConfiguration? si, Match m, TVSettings.FilenameProcessorRE re)
        {
            int? maxEp = null;
            int? ep = null;
            int? seas = null;

            if (int.TryParse(m.Groups["s"].ToString(), out int seasparsed))
            {
                seas = seasparsed;
            }
            else
            {
                if (!re.RegExpression.Contains("<s>") && (si?.AppropriateSeasons().Count ?? 0) == 1)
                {
                    seas = 1;
                }
            }

            if (int.TryParse(m.Groups["e"].ToString(), out int eparsed))
            {
                ep = eparsed;
            }

            if (int.TryParse(m.Groups["f"].ToString(), out int maxEparsed))
            {
                maxEp = maxEparsed;
            }

            return (seas, ep, maxEp);
        }

        private FileMatchResult MatchUsingEpisodeName(FileInfo? fi)
        {
            if (fi is null) return FileMatchResult.None;

            return MatchUsingEpisodeName(fi.Name);
        }
        private FileMatchResult MatchUsingEpisodeName(string name)
        {
            if (name is null || show is null)
            {
                return FileMatchResult.None;
            }

            CachedSeriesInfo? ser = show.CachedShow;
            
            if (ser is null)
            {
                return FileMatchResult.None;
            }

            string simplifiedFilename = name.CompareName();

            foreach (Episode epi in show.EpisodesToUse())
            {
                string simplifiedEpName = epi.Name.CompareName();

                if (simplifiedFilename.Contains(simplifiedEpName))
                {
                    return new FileMatchResult
                    {
                        IsMatch = true,
                        FoundSeason = epi.GetSeasonNumber(show.Order),
                        FoundEpisode = epi.GetEpisodeNumber(show.Order),
                        MatchReason = $"Matched episode name {epi.Name} with {name}"
                    };
                }
            }

            return FileMatchResult.None;
        }

        private FileMatchResult MatchUsingEpisodeDate(string filename)
        {
            if (filename is null || show is null)
            {
                return FileMatchResult.None;
            }

            // look for a valid airdate in the filename
            // check for YMD, DMY, and MDY
            // only check against airdates we expect for the given show
            CachedSeriesInfo? ser = show.CachedShow;

            if (ser is null)
            {
                return FileMatchResult.None;
            }

            string[] dateFormats = ["yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy", "yy-MM-dd", "dd-MM-yy", "MM-dd-yy"];

            // force possible date separators to a dash
            filename = filename.Replace("/", "-");
            filename = filename.Replace(".", "-");
            filename = filename.Replace(",", "-");
            filename = filename.Replace(" ", "-");

            int? ep = null;
            int? seas = null;

            foreach (Episode epi in show.EpisodesToUse())
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
                            seas = epi.GetSeasonNumber(show.Order);
                            ep = epi.GetEpisodeNumber(show.Order);
                            closestDate = timeAgo;
                        }
                    }
                }
            }

            if (ep.HasValue && seas.HasValue)
            {
                return new FileMatchResult
                {
                    IsMatch = true,
                    FoundSeason = seas,
                    FoundEpisode = ep,
                    MatchReason = $"Matched episode airdate with {filename}"
                };
            }

            return FileMatchResult.None;
        }

        private FileMatchResult MatchUsingSequentialNumber(string filename)
        {
            if (episode is null)
            {
                throw new InvalidOperationException("Episode must be set before calling IdentifyFile.");
            }

            if (!episode.OverallNumber.HasValue)
            {
                return FileMatchResult.None;
            }

            string matchText = "X " + filename + " X"; // need to pad to let it match non-numbers at start and end
            string[] regexes =
            [
                @"\D\s(E|e|Ep|ep|episode|Episode)\s?0*(?<sequencenumber>\d+)\s\D",
                @"\D\s0*(?<sequencenumber>\d+)\s\D",
                @"\D\s0*(?<sequencenumber>\d+)v\d+\s\D",
            ];

            bool matches = regexes
                .Select(regex => Regex.Match(matchText, regex))
                .Where(match => match.Success)
                .Select(match => int.Parse(match.Groups["sequencenumber"].Value) == episode.OverallNumber.Value)
                .FirstOrDefault();

            if (matches)
            {
                return new FileMatchResult
                {
                    IsMatch = true,
                    FoundEpisode = episode.AppropriateEpNum,
                    FoundSeason = episode.AppropriateSeasonNumber,
                    MatchReason = $"Matched sequential episode number {episode.OverallNumber.Value} with {filename}"
                };
            }
            return FileMatchResult.None;
        }
    }

    class FileMatchResult
    {
        internal static readonly FileMatchResult None = new() { IsMatch = false };

        public bool IsMatch { get; set; }
        public int? FoundSeason { get; set; }
        public int? FoundEpisode { get; set; }
        public int? FoundMaxEpisode { get; set; }
        public string? MatchReason { get; set; }
    }
}
