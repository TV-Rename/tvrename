using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{

    // "PossibleNewMovie" represents a folder found by doing a Check in the 'Bulk Add Movie' dialog

    public class PossibleNewMovie
    {
        public readonly string MovieStub;
        public readonly DirectoryInfo Directory;

        // ReSharper disable once InconsistentNaming
        public string RefinedHint;
        public int? PossibleYear;
        public string? ImdbCode;
        internal int ProviderCode;
        internal TVDoc.ProviderType Provider;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public CachedMovieInfo? CachedMovie => Provider ==TVDoc.ProviderType.TMDB ? TMDB.LocalCache.Instance.GetMovie(ProviderCode) : TheTVDB.LocalCache.Instance.GetMovie(ProviderCode);
        public bool CodeKnown => !CodeUnknown;
        public bool CodeUnknown => ProviderCode <= 0;

        public string CodeString => (CodeUnknown) ? "<Unknown>" : $"{ProviderCode} ({Provider.PrettyPrint()})";

        public PossibleNewMovie(FileInfo possibleMovieFile, bool andGuess,bool showErrorMsgBox)
        {
            MovieStub = possibleMovieFile.MovieFileNameBase();
            Directory = possibleMovieFile.Directory;

            (string? directoryRefinedHint, int? directoryPossibleYear) = GuessShowName(possibleMovieFile.Directory.Name);
            (string? fileRefinedHint, int? filePossibleYear) = GuessShowName(possibleMovieFile.MovieFileNameBase());

            RefinedHint = fileRefinedHint.HasValue() ? fileRefinedHint : directoryRefinedHint;
            PossibleYear = filePossibleYear ?? directoryPossibleYear;

            if (andGuess)
            {
                GuessMovie(showErrorMsgBox);
            }
        }

        public void GuessMovie(bool showErrorMsgBox)
        {
            //TODO  make generic, as this assumes TMDB

            //Lookup based on TMDB ID Being Present
            int? tmdbId = ConvertToInt(FindShowCode("tmdbid", "tmdb"));
            int? TMDBCode = ValidateOnTMDB(tmdbId, new Locale(), showErrorMsgBox);
            if (TMDBCode.HasValue)
            {
                SetId(TMDBCode.Value, TVDoc.ProviderType.TMDB);
                return;
            }

            string? imdbToTest = null;
            string? imdbId = FindShowCode("imdbid", "tmdb");

            if (imdbId.HasValue())
            {
                imdbToTest = imdbId;
            }
            else
            { 
                string? id = FindShowCode("id", "id");
                if (id?.StartsWith("tt",StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    imdbToTest = id;
                }
            }

            if (imdbToTest.HasValue())
            {
                CachedMovieInfo? s = TMDB.LocalCache.Instance.LookupMovieByImdb(imdbToTest!, new Locale(), showErrorMsgBox);
                if (s != null)
                {
                    SetId(s.TmdbCode, TVDoc.ProviderType.TMDB);
                    ImdbCode = imdbToTest;
                    return;
                }
            }

            //Do a Search on TMDB
            CachedMovieInfo? ser = TMDB.LocalCache.Instance.GetMovie(this, new Locale(), showErrorMsgBox);
            if (ser != null)
            {
                SetId(ser.TmdbCode, TVDoc.ProviderType.TMDB);
                return;
            }

            //Tweak the hints and do another Search on TMDB
            ser = ParseHints(showErrorMsgBox);
            if (ser != null)
            {
                SetId(ser.TmdbCode, TVDoc.ProviderType.TMDB);
                return;
            }

            //Tweak the hints and do another Search on TMDB
             int? tvdbId = ConvertToInt(FindShowCode("tvdbid", "tvdb"));
             if (tvdbId.HasValue)
             {
                 CachedMovieInfo? s2 = TMDB.LocalCache.Instance.LookupMovieByTvdb(tvdbId.Value, showErrorMsgBox);
                 if (s2 != null)
                 {
                     SetId(s2.TmdbCode, TVDoc.ProviderType.TMDB);
                 }
                else
                {
                    //Find movie on TVDB based on Id
                    CachedMovieInfo? s3 = TheTVDB.LocalCache.Instance.GetMovieAndDownload(tvdbId.Value, new Locale(), showErrorMsgBox);
                    if (s3 != null)
                    {
                        SetId(s3.TvdbCode, TVDoc.ProviderType.TheTVDB);
                    }

                }
            }
        }
        public void SetId(int code, TVDoc.ProviderType provider)
        {
            ProviderCode = code;
            Provider = provider;
        }

        private CachedMovieInfo? ParseHints(bool showErrorMsgBox)
        {
            Match mat = Regex.Match(RefinedHint.Trim(), @"\s(\d{4})$");
            if (mat.Success)
            {
                int newPossibleYear = mat.Groups[1].Value.ToInt(0);

                //Try removing any year
                string showNameNoYear =
                    Regex.Replace(RefinedHint.Trim(), @"\s\d{4}$", "").Trim();

                //Remove anything we can from hint to make it cleaner and hence more likely to match
                string refinedHint = showNameNoYear;

                if (string.IsNullOrWhiteSpace(refinedHint))
                {
                    Logger.Info($"Ignoring {RefinedHint} as it refines to nothing.");
                    return null;
                }

                if (RefinedHint.Equals(refinedHint, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                RefinedHint = refinedHint;
                PossibleYear ??= newPossibleYear;

                return TMDB.LocalCache.Instance.GetMovie(this, new Locale(), showErrorMsgBox);
            }

            return null;
        }

        private int? ValidateOnTMDB(int? tmdbId,Locale locale, bool showErrorMsgBox)
        {
            if (tmdbId.HasValue)
            {
                try
                {
                    CachedMovieInfo series = TMDB.LocalCache.Instance.GetMovieAndDownload(tmdbId.Value, locale, showErrorMsgBox);
                    return series.TmdbCode;
                }
                catch (MediaNotFoundException)
                {
                    //continue to try the next method
                }
            }

            return null;
        }

        private static int? ConvertToInt(string? s)
        {
            if (s.HasValue())
            {
                if (int.TryParse(s, out int x))
                {
                    return x;
                }
            }

            return null;
        }

        private string? FindShowCode(string simpleIdCode, string uniqueIdCode)
        {
            List<string> possibleFilenames = new List<string> { $"{MovieStub}.nfo", $"{MovieStub}.xml" };
            foreach (string fileName in possibleFilenames)
            {
                try
                {
                    IEnumerable<FileInfo> files = Directory.EnumerateFiles(fileName).ToList();
                    if (files.Any())
                    {
                        foreach (string x in files.Select(info => FindShowCode(info, simpleIdCode, uniqueIdCode))
                            .Where(x => x.HasValue()))
                        {
                            return x;
                        }
                    }
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
                catch (NotSupportedException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
                catch (System.IO.IOException e)
                {
                    Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.Message}");
                }
            }
            
            //Can't find it
            return null;
        }

        private static string? FindShowCode([NotNull] FileInfo file, string simpleIdCode, string uniqueIdCode)
        {
            try
            {
                using (System.IO.StreamReader? streamReader = file.OpenText())
                {
                    using (XmlReader reader = XmlReader.Create(streamReader))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name == simpleIdCode && reader.IsStartElement())
                            {
                                string s = reader.ReadElementContentAsString();
                                if (s.HasValue())
                                {
                                    return s;
                                }
                            }

                            if (reader.Name == "uniqueid" && reader.IsStartElement() &&
                                reader.GetAttribute("type") == uniqueIdCode)
                            {
                                string s = reader.ReadElementContentAsString();
                                if (s.HasValue())
                                {
                                    return s;
                                }
                            }
                        }
                    }
                }
            }
            catch (XmlException xe)
            {
                Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.Message}");
            }
            catch (System.IO.IOException xe)
            {
                Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.Message}");
            }
            catch (UnauthorizedAccessException xe)
            {
                Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.Message}");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Could not parse {file.FullName} to try and see whether there is any Ids inside.");
            }

            return null;
        }

        private static (string hint, int? year) GuessShowName(string refinedHint)
        {
            refinedHint = refinedHint.CompareName();
            int? possibleYear = null;

            List<string> removeCrapAfterTerms =
                new List<string> { "1080p", "720p","dvdrip","webrip","brrip","r5","BDrip","limited","dvdscr","unrated","tv","bluray","hdrip","3d","xvid","r6rip" };

            foreach (string? removeCrapAfterTerm in removeCrapAfterTerms)
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

            const string PATTERN = @"\s(\d{4})$";
            Match m = Regex.Match(refinedHint.Trim(), PATTERN);
            if (m.Success)
            {
                //Seems like we have a year in the date

                //Work out the year
                int.TryParse(m.Groups[1].Value, out int year);
                possibleYear = year;

                //remove year from string
                refinedHint = Regex.Replace(refinedHint.Trim(), PATTERN, " ");
            }

            refinedHint = refinedHint.CompareName();

            return (refinedHint,possibleYear);
        }
    }
}
