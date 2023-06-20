using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace TVRename;
// "PossibleNewMovie" represents a folder found by doing a Check in the 'Bulk Add Movie' dialog

public class PossibleNewMovie : ISeriesSpecifier
{
    private readonly string movieStub;
    private readonly FileInfo movieFile;
    public DirectoryInfo Directory => movieFile.Directory;

    // ReSharper disable once InconsistentNaming
    public string RefinedHint;

    public int? PossibleYear;
    internal int ProviderCode;
    internal TVDoc.ProviderType SourceProvider;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public CachedMovieInfo? CachedMovie => TVDoc.GetMovieCache(SourceProvider).GetMovie(ProviderCode);

    public bool CodeKnown => !CodeUnknown;
    public bool CodeUnknown => ProviderCode <= 0;

    public string CodeString => CodeKnown ? $"{ProviderCode} ({SourceProvider.PrettyPrint()})" : "<Unknown>";

    public PossibleNewMovie(FileInfo possibleMovieFile, bool andGuess, bool showErrorMsgBox)
    {
        movieStub = possibleMovieFile.MovieFileNameBase();
        movieFile = possibleMovieFile;

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
        int? tmdbId = FindShowCode("tmdbid", "tmdb").ToInt();
        Locale preferredLocale = new();

        int? tmdbCode = ValidateOnTMDB(tmdbId, preferredLocale);
        if (tmdbCode.HasValue)
        {
            SetId(tmdbCode.Value, TVDoc.ProviderType.TMDB);
            Logger.Info($"BULK ADD AUTO ID: identified {Name} ({movieFile.Name}) based on TMDB = {tmdbId} which validated to {tmdbCode}");
            return;
        }

        string? imdbToTest = null;
        string? imdbId = FindShowCode("imdbid", "imdb");

        if (imdbId.HasValue())
        {
            imdbToTest = imdbId;
        }
        else
        {
            string? id = FindShowCode("id", "id");
            if (id?.StartsWith("tt", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                imdbToTest = id;
            }
        }

        if (imdbToTest.HasValue())
        {
            CachedMovieInfo? s = TMDB.LocalCache.Instance.LookupMovieByImdb(imdbToTest, preferredLocale);
            if (s != null)
            {
                SetId(s.TmdbCode, TVDoc.ProviderType.TMDB);
                ImdbCode = imdbToTest;
                Logger.Info($"BULK ADD AUTO ID: identified {Name} ({movieFile.Name}) based on IMDB = {imdbToTest} which we looked up to get {s.TmdbCode}");
                return;
            }
        }

        //Do a Search on TMDB
        CachedMovieInfo? ser = TMDB.LocalCache.Instance.GetMovie(this, preferredLocale, showErrorMsgBox);
        if (ser != null)
        {
            SetId(ser.TmdbCode, TVDoc.ProviderType.TMDB);
            Logger.Info($"BULK ADD AUTO ID: identified {Name} ({movieFile.Name}) based on Name = {RefinedHint} which we looked up to get {ser.TmdbCode}");
            return;
        }

        //Tweak the hints and do another Search on TMDB
        ser = ParseHints(showErrorMsgBox);
        if (ser != null)
        {
            SetId(ser.TmdbCode, TVDoc.ProviderType.TMDB);
            Logger.Info($"BULK ADD AUTO ID: identified {Name} ({movieFile.Name}) based on Name = {RefinedHint}({PossibleYear}) which we looked up to get {ser.TmdbCode}");
            return;
        }

        //Tweak the hints and do another Search on TMDB
        int? tvdbId = FindShowCode("tvdbid", "tvdb").ToInt();
        if (tvdbId.HasValue)
        {
            CachedMovieInfo? s2 = TMDB.LocalCache.Instance.LookupMovieByTvdb(tvdbId.Value, preferredLocale);
            if (s2 != null)
            {
                Logger.Info($"BULK ADD AUTO ID: identified {Name} ({movieFile.Name}) based on TVDB = {tvdbId}({PossibleYear}) which we looked up to get {s2.TmdbCode}");
                SetId(s2.TmdbCode, TVDoc.ProviderType.TMDB);
            }
            else
            {
                //Find movie on TVDB based on Id
                CachedMovieInfo? s3 = TheTVDB.LocalCache.Instance.GetMovieAndDownload(this, preferredLocale, showErrorMsgBox);
                if (s3 != null)
                {
                    Logger.Info($"BULK ADD AUTO ID: identified {Name} ({movieFile.Name}) based on TVDB(s3) = {tvdbId}({PossibleYear}) which we looked up to get {s3.TmdbCode}");
                    SetId(s3.TvdbCode, TVDoc.ProviderType.TheTVDB);
                }
            }
        }
    }

    public void SetId(int code, TVDoc.ProviderType provider)
    {
        ProviderCode = code;
        SourceProvider = provider;
    }

    private CachedMovieInfo? ParseHints(bool showErrorMsgBox)
    {
        Match mat = Regex.Match(RefinedHint.Trim(), @"\s(\d{4})$");
        if (mat.Success)
        {
            int newPossibleYear = mat.Groups[1].Value.ToInt(0);

            //Try removing any year
            string showNameNoYear = RefinedHint.RemoveYearFromEnd();

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

            //todo -validate if this can work for multi-providers
            return TMDB.LocalCache.Instance.GetMovie(this, new Locale(), showErrorMsgBox);
        }

        return null;
    }

    private static int? ValidateOnTMDB(int? tmdbId, Locale locale)
    {
        if (tmdbId.HasValue)
        {
            try
            {
                ISeriesSpecifier ss = new SearchSpecifier(tmdbId.Value, locale, TVDoc.ProviderType.TMDB, MediaConfiguration.MediaType.movie);

                CachedMovieInfo series = TMDB.LocalCache.Instance.GetMovieAndDownload(ss);
                return series.TmdbCode;
            }
            catch (MediaNotFoundException)
            {
                //continue to try the next method
            }
        }

        return null;
    }

    private string? FindShowCode(string simpleIdCode, string uniqueIdCode)
    {
        List<string> possibleFilenames = new() { $"{movieStub}.nfo", $"{movieStub}.xml" };
        foreach (string fileName in possibleFilenames)
        {
            try
            {
                IEnumerable<FileInfo> files = Directory.EnumerateFiles(fileName).ToList();
                if (files.Any())
                {
                    foreach (string x in files.Select(info => FindShowCode(info, simpleIdCode, uniqueIdCode)).ValidStrings())
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

    private static string? FindShowCode(FileInfo file, string simpleIdCode, string? uniqueIdCode)
    {
        try
        {
            using System.IO.StreamReader? streamReader = file.OpenText();
            using XmlReader reader = XmlReader.Create(streamReader);
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
        catch (OperationCanceledException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.Message}");
        }
        catch (Exception e)
        {
            Logger.Error($"Could not parse {file.FullName} to try and see whether there is any Ids inside. Got a {e.Message}");
        }

        return null;
    }

    private static (string hint, int? year) GuessShowName(string refinedHint)
    {
        refinedHint = FinderHelper.RemoveSceneTerms(refinedHint.CompareName());

        (string newRefinedHint, int? possibleYear) = FinderHelper.SplitIntoTitleYear(refinedHint);

        return (newRefinedHint.CompareName(), possibleYear);
    }

    public TVDoc.ProviderType Provider => SourceProvider;

    public int TvdbId => Provider == TVDoc.ProviderType.TheTVDB && CodeKnown ? ProviderCode : -1;

    public string Name => RefinedHint;

    public MediaConfiguration.MediaType Media => MediaConfiguration.MediaType.tv;

    public int TvMazeId => Provider == TVDoc.ProviderType.TVmaze && CodeKnown ? ProviderCode : -1;

    public int TmdbId => Provider == TVDoc.ProviderType.TMDB && CodeKnown ? ProviderCode : -1;

    public string? ImdbCode { get; private set; }

    public Locale TargetLocale => new();

    public ProcessedSeason.SeasonType SeasonOrder => throw new InvalidOperationException();

    public bool HasStub => !string.IsNullOrWhiteSpace(movieStub);

    public void UpdateId(int id, TVDoc.ProviderType source)
    {
        SourceProvider = source;
        ProviderCode = id;
    }

    public bool Matches(PossibleNewMovie ai) => movieStub.Equals(ai.movieStub, StringComparison.CurrentCultureIgnoreCase);
}
