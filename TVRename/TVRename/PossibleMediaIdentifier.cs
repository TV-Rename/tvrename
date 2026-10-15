//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Text.RegularExpressions;
using TMDbLib.Objects.Exceptions;
using TMDbLib.Objects.People;
using static TVRename.TVDoc;

namespace TVRename;

class PossibleMediaIdentifier
{
    protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    protected static async Task<int?> ValidateOnTVDBAsync(int? tvdbId, Locale locale, bool showMessageBox, MediaConfiguration.MediaType mediaType)
    {
        if (tvdbId.HasValue)
        {
            try
            {
                ISeriesSpecifier ss = new SearchSpecifier(tvdbId.Value, locale, TVDoc.ProviderType.TheTVDB, mediaType);

                if (mediaType == MediaConfiguration.MediaType.movie)
                {
                    CachedMovieInfo? series = await TheTVDB.LocalCache.Instance.GetMovieAndDownloadAsync(ss, locale, showMessageBox, new ThreadSafeCounter(), 1);
                    return series?.TmdbCode;
                }
                else
                {
                    CachedSeriesInfo? s = await TheTVDB.LocalCache.Instance.GetSeriesOrDownloadAsync(ss,false);
                    if (s != null)
                    {
                        return s.TvdbCode;
                    }
                }
            }
            catch (MediaNotFoundException)
            {
                //continue to try the next method
            }
        }
        return null;
    }


    protected static async Task<int?> ValidateOnTMDBAsync(int? tmdbId, Locale locale, MediaConfiguration.MediaType mediaType)
    {
        if (tmdbId.HasValue)
        {
            try
            {
                ISeriesSpecifier ss = new SearchSpecifier(tmdbId.Value, locale, TVDoc.ProviderType.TMDB, mediaType);

                if (mediaType == MediaConfiguration.MediaType.movie)
                {
                    CachedMovieInfo series = await TMDB.LocalCache.Instance.GetMovieAndDownloadAsync(ss);
                    return series.TmdbCode;
                }
                else
                {
                    CachedSeriesInfo? series = await TMDB.LocalCache.Instance.GetSeriesOrDownloadAsync(ss);
                    return series?.TmdbCode;
                }
            }
            catch (MediaNotFoundException)
            {
                //continue to try the next method
            }
        }

        return null;
    }

    protected static async Task<int?> ValidateOnTVMazeAsync(int? tvmazeId, Locale locale)
    {
        if (tvmazeId.HasValue)
        {
            try
            {
                ISeriesSpecifier ss = new SearchSpecifier(tvmazeId.Value, locale, TVDoc.ProviderType.TVmaze, MediaConfiguration.MediaType.tv);

                CachedSeriesInfo? series = await TVmaze.LocalCache.Instance.GetSeriesOrDownloadAsync(ss);
                return series?.TvMazeCode;
            }
            catch (MediaNotFoundException)
            {
                //continue to try the next method
            }
        }

        return null;
    }

    protected static string GuessShowName(PossibleNewTvShow ai, ShowLibrary library)
    {
        // see if we can guess a season number and show name, too
        // Assume is blah\blah\blah\show\season X
        string showName = ai.FolderName;

        foreach (string seasonWord in library.SeasonWords())
        {
            string seasonFinder = ".*" + seasonWord + "[ _\\.]+([0-9]+).*";
            if (Regex.Count(showName, seasonFinder, RegexOptions.IgnoreCase) == 0)
            {
                continue;
            }

            try
            {
                // remove season folder from end of the path
                showName = Regex.Replace(showName, "(.*)\\\\" + seasonFinder, "$1", RegexOptions.IgnoreCase);
                break;
            }
            catch (ArgumentException)
            {
                //Ignore this Exception
            }
        }

        // assume last folder element is the show name
        showName = showName[(showName.LastIndexOf(Path.DirectorySeparatorChar.ToString(),
            StringComparison.Ordinal) + 1)..];

        return showName;
    }
}
interface iPossibleTVShowIdentifier
{
    Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewTvShow show,bool showErrorMsgBox);
}
interface iPossibleMovieIdentifier
{
    Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewMovie movie, bool showErrorMsgBox);
}

class IdentifyFinalBoss : IdentifyFromNFOFiles, iPossibleMovieIdentifier
{
    public new async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewMovie movie,bool showErrorMsgBox)
    {
        Locale localeToUse = new(TVSettings.Instance.PreferredTVDBLanguage);
        //Tweak the hints and do another Search on TMDB
        int? tvdbId = FindShowCode(movie, "tvdbid", "tvdb").ToInt();
        if (tvdbId.HasValue)
        {
            CachedMovieInfo? s2 = await TMDB.LocalCache.Instance.LookupMovieByTvdbAsync(tvdbId.Value, localeToUse);
            if (s2 != null)
            {
                return (TVDoc.ProviderType.TMDB, s2.TmdbCode);
            }
            else
            {
                //Find movie on TVDB based on Id
                CachedMovieInfo? s3 = await
                    TheTVDB.LocalCache.Instance.GetMovieAndDownloadAsync(movie, localeToUse, showErrorMsgBox, new ThreadSafeCounter(), 1);

                if (s3 != null)
                {
                    return (TVDoc.ProviderType.TheTVDB, s3.TvdbCode);
                }
            }
        }
        throw new MediaNotIdentifiedException();
    }
}

class IdentifyByTweakedSearchName(ShowLibrary library) : PossibleMediaIdentifier, iPossibleTVShowIdentifier
{
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewTvShow show, bool showErrorMsgBox)
    {
        Language languageToUse = TVSettings.Instance.DefaultProvider == TVDoc.ProviderType.TMDB
            ? TVSettings.Instance.TMDBLanguage
            : TVSettings.Instance.PreferredTVDBLanguage;

        Locale localeToUse = new(languageToUse);

        string showName = GuessShowName(show, library);

        //Try removing any year
        string showNameNoYear = showName.RemoveBracketedYear();

        //Remove anything we can from hint to make it cleaner and hence more likely to match
        string refinedHint = FinderHelper.RemoveSeriesEpisodeIndicators(showNameNoYear, library.SeasonWords());

        if (string.IsNullOrWhiteSpace(refinedHint))
        {
            Logger.Info($"Ignoring {showName} as it refines to nothing.");
        }

        var ser = await TheTVDB.LocalCache.Instance.SearchSeriesAsync(refinedHint, showErrorMsgBox, localeToUse);

        show.RefinedHint = refinedHint;
        if (ser != null)
        {
            return (TVDoc.ProviderType.TheTVDB, ser.TvdbId);
        }
        throw new MediaNotIdentifiedException();
    }
}

class IdentifyByTweakedName() : PossibleMediaIdentifier, iPossibleMovieIdentifier
{
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewMovie movie, bool showErrorMsgBox)
    {
        //Tweak the hints and do another Search on TMDB
        var ser = await ParseHintsAsync(movie, showErrorMsgBox);
        if (ser != null)
        {
            return (TVDoc.ProviderType.TMDB, ser.TmdbCode);
        }
        throw new MediaNotIdentifiedException();
    }

    public async Task<CachedMovieInfo?> ParseHintsAsync(PossibleNewMovie movie, bool showErrorMsgBox)
    {
        try
        {
            Match mat = Regex.Match(movie.RefinedHint.Trim(), @"\s(\d{4})$");
            if (mat.Success)
            {
                int newPossibleYear = mat.Groups[1].Value.ToInt(0);

                //Try removing any year
                string showNameNoYear = movie.RefinedHint.RemoveYearFromEnd();

                //Remove anything we can from hint to make it cleaner and hence more likely to match
                string refinedHint = showNameNoYear;

                if (string.IsNullOrWhiteSpace(refinedHint))
                {
                    Logger.Info($"Ignoring {movie.RefinedHint} as it refines to nothing.");
                    return null;
                }

                if (movie.RefinedHint.Equals(refinedHint, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                movie.RefinedHint = refinedHint;
                movie.PossibleYear ??= newPossibleYear;

                //todo -validate if this can work for multi-providers
                return await GetMovieAsync(movie, new Locale(), showErrorMsgBox);
            }
        }
        catch (RegexMatchTimeoutException ex)
        {
            Logger.Warn(ex, $"Could not parse hints for new movie");
        }
        return null;
    }

    public async Task<CachedMovieInfo?> GetMovieAsync(PossibleNewMovie show, Locale preferredLocale, bool showErrorMsgBox) => await TMDB.LocalCache.Instance.SearchMovieAsync(show.RefinedHint, show.PossibleYear, preferredLocale, showErrorMsgBox, false);
}

[Serializable]
internal class MediaNotIdentifiedException : Exception
{
    public MediaNotIdentifiedException()
    {
    }

    public MediaNotIdentifiedException(string? message) : base(message)
    {
    }

    public MediaNotIdentifiedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

class IdentifyByMovieRefinedHint() : PossibleMediaIdentifier, iPossibleMovieIdentifier
{
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewMovie movie, bool showErrorMsgBox)
    {
        Locale localeToUse = new(TVSettings.Instance.TMDBLanguage);
        //Do a Search on TMDB
        CachedMovieInfo? ser = await TMDB.LocalCache.Instance.SearchMovieAsync(movie.RefinedHint, movie.PossibleYear, localeToUse, showErrorMsgBox,false);
        if (ser != null)
        {
            return (TVDoc.ProviderType.TMDB, ser.TmdbCode);
        }
        throw new MediaNotIdentifiedException();
    }
}

class IdentifyBySearchName(ShowLibrary library) : PossibleMediaIdentifier, iPossibleTVShowIdentifier
{
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewTvShow show, bool showErrorMsgBox)
    {
        Locale localeToUse = new(TVSettings.Instance.PreferredTVDBLanguage);

        string showName = GuessShowName(show, library);

        CachedSeriesInfo? ser = await TheTVDB.LocalCache.Instance.SearchSeriesAsync(showName, showErrorMsgBox, localeToUse);
        if (ser != null)
        {
            return (TVDoc.ProviderType.TheTVDB, ser.TvdbId);
        }

        throw new MediaNotIdentifiedException();
    }
}

class IdentifyFromNFOFiles : PossibleMediaIdentifier, iPossibleTVShowIdentifier, iPossibleMovieIdentifier
{
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewTvShow show, bool showErrorMsgBox)
    {
        List<string> possibleFilenames = ["series.xml", "tvshow.nfo"];
        foreach (string fileName in possibleFilenames)
        {
            try
            {
                IEnumerable<FileInfo> files = show.FindFiles(fileName);
                if (files.IsAny())
                {
                    foreach (int x in files.Select(FindTvdbShowCode).Where(x => x != -1))
                    {
                        return (TVDoc.ProviderType.TheTVDB, x);
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (NotSupportedException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (System.IO.IOException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
        }
        throw new MediaNotIdentifiedException();
    }
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewMovie movie, bool showErrorMsgBox)
    {
        Locale preferredLocale = new();

        try{
            int? tvdbId = FindShowCode(movie ,"tvdbid", "tvdb").ToInt();
            if (tvdbId.HasValue)
            {
                CachedMovieInfo? s3 = await TheTVDB.LocalCache.Instance.GetMovieAndDownloadAsync(movie, preferredLocale, showErrorMsgBox, new ThreadSafeCounter(), 1);

                if (s3 != null)
                {
                    return (TVDoc.ProviderType.TheTVDB, s3.TvdbCode);
                }
            }
        }
        catch (MediaNotFoundException ex)
        {
            //not really an issue so we can continue
            Logger.Warn($"Could not match the possible movie (MediaNotFoundException: {ex.Message}): {movie}");
        }
        catch (SourceConnectivityException ex)
        {
            Logger.Warn($"Could not match the possible movie (SourceConnectivityException: {ex.Message}): {movie}");
        }
        catch (SourceConsistencyException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }
        catch (GeneralHttpException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }
        catch (AggregateException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }

        try{
        //Lookup based on TMDB ID Being Present
        int? tmdbId = FindShowCode(movie, "tmdbid", "tmdb").ToInt();
        int? tmdbCode = await ValidateOnTMDBAsync(tmdbId, preferredLocale, MediaConfiguration.MediaType.movie);
            if (tmdbCode.HasValue)
            {
                return (TVDoc.ProviderType.TMDB, tmdbCode.Value);
            }
        }
        catch (MediaNotFoundException ex)
        {
            //not really an issue so we can continue
            Logger.Warn($"Could not match the possible movie (MediaNotFoundException: {ex.Message}): {movie}");
        }
        catch (SourceConnectivityException ex)
        {
            Logger.Warn($"Could not match the possible movie (SourceConnectivityException: {ex.Message}): {movie}");
        }
        catch (SourceConsistencyException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }
        catch (GeneralHttpException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }
        catch (AggregateException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }

        try
        {
            string? imdbToTest = null;
            string? imdbId = FindShowCode(movie, "imdbid", "imdb");
            if (imdbId.HasValue())
            {
                imdbToTest = imdbId;
            }
            else
            {
                string? id = FindShowCode(movie, "id", "id");
                if (id?.StartsWith("tt", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    imdbToTest = id;
                }
            }

            if (imdbToTest.HasValue())
            {
                CachedMovieInfo? s = await TMDB.LocalCache.Instance.LookupMovieByImdbAsync(imdbToTest, preferredLocale);
                if (s != null)
                {
                    return (TVDoc.ProviderType.TMDB, s.TmdbCode);
                }
            }
        }
        catch (MediaNotFoundException ex)
        {
            //not really an issue so we can continue
            Logger.Warn($"Could not match the possible movie (MediaNotFoundException: {ex.Message}): {movie}");
        }
        catch (SourceConnectivityException ex)
        {
            Logger.Warn($"Could not match the possible movie (SourceConnectivityException: {ex.Message}): {movie}");
        }
        catch (SourceConsistencyException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }
        catch (GeneralHttpException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }
        catch (AggregateException ex)
        {
            Logger.Error(ex, $"Could not match the possible movie: {movie}");
        }

        throw new MediaNotIdentifiedException();
    }

    protected string? FindShowCode(PossibleNewMovie movie, string simpleIdCode, string uniqueIdCode)
    {
        List<string> possibleFilenames = [$"{movie.movieStub}.nfo", $"{movie.movieStub}.xml"];
        foreach (string fileName in possibleFilenames)
        {
            try
            {
                IEnumerable<FileInfo> files = [.. movie.Directory.EnumerateFiles(fileName)];
                if (files.IsAny())
                {
                    foreach (string x in files.Select(info => FindShowCode(info, simpleIdCode, uniqueIdCode)).ValidStrings())
                    {
                        return x;
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (NotSupportedException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
            }
            catch (System.IO.IOException e)
            {
                Logger.Warn($"Could not look in {fileName} for any ShowCodes {e.ErrorText()}");
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
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.ErrorText()}");
        }
        catch (System.IO.IOException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.ErrorText()}");
        }
        catch (UnauthorizedAccessException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.ErrorText()}");
        }
        catch (OperationCanceledException xe)
        {
            Logger.Warn($"Could not parse {file.FullName} to try and see whether there is any Ids inside, got {xe.ErrorText()}");
        }
        catch (Exception e)
        {
            Logger.Error($"Could not parse {file.FullName} to try and see whether there is any Ids inside. Got a {e.ErrorText()}");
        }

        return null;
    }

    private static int FindTvdbShowCode(FileInfo file)
    {
        var s = FindShowCode(file, "tvdbid", "tvdb");
        bool success = int.TryParse(s, out int x);
        if (success && x != -1)
        {
            return x;
        }
        return -1;
    }
}


class IdentifyFromFilenameCodes : PossibleMediaIdentifier, iPossibleTVShowIdentifier, iPossibleMovieIdentifier
{
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewTvShow show, bool showErrorMsgBox)
    {
        Locale preferredLocale = new();
        (string codeType, int code)? x = FileHelper.ExtractIdsFrom(show.FolderName);
        if (x.HasValue) //todo - add check for one folder per show
        {
            (string? codeType, int code) = x.Value;
            switch (codeType.ToUpper())
            {
                case "TVDB":
                    int? tvDBCode = await ValidateOnTVDBAsync(code, preferredLocale, showErrorMsgBox, MediaConfiguration.MediaType.tv);

                    if (tvDBCode != null)
                    {
                        return (TVDoc.ProviderType.TheTVDB, tvDBCode.Value);
                    }
                    break;
                case "TMDB":
                    int? tmdbCodeA = await ValidateOnTMDBAsync(code, preferredLocale, MediaConfiguration.MediaType.tv  );
                    if (tmdbCodeA.HasValue)
                    {
                        return (TVDoc.ProviderType.TMDB, tmdbCodeA.Value);
                    }
                    break;
                case "IMDB":
                    CachedSeriesInfo? s = await TMDB.LocalCache.Instance.LookupTVShowByImdbAsync("tt" + code, preferredLocale);
                    if (s != null)
                    {
                        return (TVDoc.ProviderType.TMDB, s.TmdbCode);
                    }
                    break;
                case "TVMAZE":
                    int? tvMazeId = await ValidateOnTVMazeAsync(code, preferredLocale);
                    if (tvMazeId != null)
                    {
                        return (TVDoc.ProviderType.TVmaze, tvMazeId.Value);
                    }
                    break;
                default:
                    //nothing to do, as this is not valid for a tv show
                    Logger.Error($"BULK ADD AUTO ID: could not identify {show.Name} ({show.FolderName}) based on {codeType} = {code} as this is not a valid code type for a tv show");
                    break;
            }
            ;
        }
        throw new MediaNotIdentifiedException();
    }

    private (string codeType, int code)? GetCodeFromNames(PossibleNewMovie movie)
    {
        string filename = GetFilename(movie);
        string foldername = GetFolderName(movie);
        var x = FileHelper.ExtractIdsFrom(filename);
        if (x.HasValue)
        {
            return x;
        }
        var y = FileHelper.ExtractIdsFrom(foldername);
        if (y.HasValue)
        {
            return y;
        }
        return null;
    }
    private string GetFilename(PossibleNewMovie movie) => movie.movieFile.Name;
    private string GetFolderName(PossibleNewMovie movie) => movie.movieFile.Directory.Name;
    public async Task<(ProviderType type, int code)> IdentifyAsync(PossibleNewMovie movie, bool showErrorMsgBox)
    {
        Locale preferredLocale = new();
        (string codeType, int code)? x = GetCodeFromNames(movie);
        if (x.HasValue)
        {
            (string? codeType, int code) = x.Value;
            switch (codeType.ToUpper())
            {
                case "TVDB":
                    int? tvDBCode = await ValidateOnTVDBAsync(code, preferredLocale, showErrorMsgBox, MediaConfiguration.MediaType.movie);
                    if (tvDBCode != null)
                    {
                        return (TVDoc.ProviderType.TheTVDB, tvDBCode.Value);
                    }
                    break;
                case "TMDB":
                    int? tmdbCodeA = await ValidateOnTMDBAsync(code, preferredLocale, MediaConfiguration.MediaType.movie);
                    if (tmdbCodeA.HasValue)
                    {
                        return (TVDoc.ProviderType.TMDB, tmdbCodeA.Value);
                    }
                    break;
                case "IMDB":
                    CachedMovieInfo? s = await TMDB.LocalCache.Instance.LookupMovieByImdbAsync("tt" + code, preferredLocale);
                    if (s != null)
                    {
                        return (TVDoc.ProviderType.TMDB, s.TmdbCode);
                    }
                    break;
                case "TVMAZE":
                    //nothing to do, as this is not valid for a movie
                    break;
                default:
                    //nothing to do, as this is not valid for a movie
                    Logger.Error($"BULK ADD AUTO ID: could not identify {movie.Name} ({movie.movieFile.FullName}) based on {codeType} = {code} as this is not a valid code type for a movie");
                    break;
            }
            ;
        }

        throw new MediaNotIdentifiedException();
    }
}
