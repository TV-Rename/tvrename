using Alphaleonis.Win32.Filesystem;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TVRename;

public abstract class MediaConfiguration : ISeriesSpecifier
{
    protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    public bool DoMissingCheck;
    public bool DoRename;
    public bool ForceCheckFuture;
    public bool ForceCheckNoAirdate;

    public int TvdbCode;

    // ReSharper disable once InconsistentNaming
    public int TVmazeCode;

    public int TmdbCode;
    public string? ImdbCode { get; set; }

    public Locale TargetLocale
    {
        get
        {
            bool useValidRegion = UseCustomRegion && CustomRegionCode.HasValue() &&
                                  Regions.Instance.RegionFromName(CustomRegionCode!) != null;

            bool useValidLanguage = UseCustomLanguage && CustomLanguageCode.HasValue() &&
                                    Languages.Instance.GetLanguageFromCode(CustomLanguageCode) != null;

            return
                useValidLanguage && useValidRegion ? new Locale(Regions.Instance.RegionFromCode(CustomRegionCode)!, Languages.Instance.GetLanguageFromCode(CustomLanguageCode)!)
                : useValidLanguage ? new Locale(Languages.Instance.GetLanguageFromCode(CustomLanguageCode)!)
                : useValidRegion ? new Locale(Regions.Instance.RegionFromName(CustomRegionCode)!)
                : new Locale();
        }
    }

    public abstract ProcessedSeason.SeasonType SeasonOrder { get; }

    /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
    public void UpdateId(int id, TVDoc.ProviderType source)
    {
        switch (source)
        {
            case TVDoc.ProviderType.TVmaze:
                TVmazeCode = id;
                break;

            case TVDoc.ProviderType.TheTVDB:
                TvdbCode = id;
                break;

            case TVDoc.ProviderType.TMDB:
                TmdbCode = id;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }
    }

    public bool UseCustomShowName;
    public string? CustomShowName;
    public string? LastName;

    public bool UseCustomRegion;
    public string? CustomRegionCode;

    public readonly List<string> AliasNames = [];
    protected internal TVDoc.ProviderType ConfigurationProvider;

    protected abstract MediaType GetMediaType();

    protected abstract Dictionary<int, SafeList<string>> AllFolderLocations(bool manualToo, bool checkExist);

    public override string ToString() => $"{GetMediaType()}: ({ConfigurationProvider.PrettyPrint()}) TVDB:{TvdbCode} TMDB:{TmdbCode} TVMaze:{TVmazeCode} ({CustomShowName},{CustomLanguageCode},{CustomRegionCode}) [{LastName}]";

    public Dictionary<int, SafeList<string>> AllExistngFolderLocations() => AllFolderLocations(true, true);

    public Dictionary<int, SafeList<string>> AllProposedFolderLocations() => AllFolderLocations(true, false);

    public Dictionary<int, SafeList<string>> AllFolderLocationsEpCheck(bool checkExist) => AllFolderLocations(true, checkExist);

    public Dictionary<int, SafeList<string>> AllFolderLocations(bool manualToo) => AllFolderLocations(manualToo, true);

    public enum MediaType
    {
        tv,
        movie,
        both
    }

    protected static TVDoc.ProviderType GetConfigurationProviderType(int? value)
    {
        return value is null ? TVDoc.ProviderType.libraryDefault : (TVDoc.ProviderType)value;
    }

    public CachedMediaInfo? CachedData => Code > 0 ? LocalCache().GetMedia(Code, GetMediaType()) : null;

    protected abstract MediaCache LocalCache();

    public int Code
    {
        get
        {
            return Provider switch
            {
                TVDoc.ProviderType.libraryDefault => throw new ArgumentOutOfRangeException(),
                TVDoc.ProviderType.TVmaze => TVmazeCode,
                TVDoc.ProviderType.TheTVDB => TvdbCode,
                TVDoc.ProviderType.TMDB => TmdbCode,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public bool HasIdOfType(TVDoc.ProviderType instanceDefaultProvider)
    {
        return instanceDefaultProvider switch
        {
            TVDoc.ProviderType.TVmaze => TVmazeCode > 0,
            TVDoc.ProviderType.TheTVDB => TvdbCode > 0,
            TVDoc.ProviderType.TMDB => TmdbCode > 0,
            TVDoc.ProviderType.libraryDefault => throw new ArgumentOutOfRangeException(nameof(instanceDefaultProvider),
                instanceDefaultProvider, null),
            _ => throw new ArgumentOutOfRangeException(nameof(instanceDefaultProvider), instanceDefaultProvider, null)
        };
    }

    public bool AnyIdsMatch(MediaConfiguration newShow) =>
        IdsMatch(TvdbCode, newShow.TvdbCode) ||
        IdsMatch(TVmazeCode, newShow.TVmazeCode) ||
        IdsMatch(TmdbCode, newShow.TmdbCode);

    private static bool IdsMatch(int code1, int code2) => code1 == code2 && code1 > 0;

    public static int CompareNames(MediaConfiguration? x, MediaConfiguration? y)
    {
        string? ones = x?.ShowName;
        string? twos = y?.ShowName;
        return string.Compare(ones, twos, StringComparison.Ordinal);
    }

    public IEnumerable<string> GetActorNames()
    {
        return Actors.Select(x => x.ActorName);
    }

    public TVDoc.ProviderType Provider
    {
        get
        {
            return ConfigurationProvider switch
            {
                TVDoc.ProviderType.libraryDefault => DefaultProvider(),
                TVDoc.ProviderType.TVmaze => ConfigurationProvider,
                TVDoc.ProviderType.TheTVDB => ConfigurationProvider,
                TVDoc.ProviderType.TMDB => ConfigurationProvider,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    public int TvdbId => TvdbCode;

    public string? Name => LastName;

    public MediaType Media => GetMediaType();

    public int TvMazeId => TVmazeCode;

    public int TmdbId => TmdbCode;

    public bool UseCustomLanguage { get; set; }

    public string? CustomLanguageCode { get; set; }

    protected abstract TVDoc.ProviderType DefaultProvider();

    public IEnumerable<string> Genres => CachedData?.Genres.Distinct() ?? [];

    public IEnumerable<Actor> Actors => CachedData?.GetActors() ?? [];

    protected IEnumerable<string> GetSimplifiedPossibleShowNames()
    {
        List<string> possibles = [];

        string simplifiedShowName = ShowName.CompareName();
        if (simplifiedShowName != "") { possibles.Add(simplifiedShowName); }

        //Check the custom show name too
        if (UseCustomShowName && CustomShowName.HasValue())
        {
            string simplifiedCustomShowName = CustomShowName.CompareName();
            if (simplifiedCustomShowName != "") { possibles.Add(simplifiedCustomShowName); }
        }

        //Also add the aliases provided
        possibles.AddNullableRange(AliasNames.Select(s => s.CompareName()).ValidStrings().Where(s => s.Length > 3));

        //Also use the aliases from source provider
        possibles.AddNullableRange(CachedData?.GetAliases().Select(s => s.CompareName()).ValidStrings().Where(s => s.Length > 4));

        return possibles;
    }

    public string ShowName
    {
        get
        {
            if (UseCustomShowName && CustomShowName.HasValue())
            {
                return CustomShowName;
            }

            CachedMediaInfo? ser = CachedData;
            if (ser?.Name.HasValue() ?? false)
            {
                return ser.Name;
            }
            if (LastName.HasValue())
            {
                return LastName;
            }

            return $"({Code} not downloaded)";
        }
    }

    protected void SetupAliases(XElement xmlSettings)
    {
        foreach (string alias in xmlSettings.Descendants("AliasNames").Descendants("Alias").Select(alias => alias.Value).Distinct())
        {
            AliasNames.Add(alias);
        }
    }
    internal void CheckHintExists(string hint)
    {
        if (AliasNames.Contains(hint))
        {
            return;
        }
        AliasNames.Add(hint);
    }

    public bool NameMatch(FileSystemInfo file, bool useFullPath) => NameMatch(useFullPath ? file.FullName : file.Name);

    public bool NameMatch(string text)
    {
        return GetSimplifiedPossibleShowNames().Any(name => FileHelper.SimplifyAndCheckFilename(text.CompareName(), name, false, false));
    }

    public bool NameMatchFilters(string text)
    {
        return GetSimplifiedPossibleShowNames().Any(name => name.Contains(text.CompareName(), StringComparison.OrdinalIgnoreCase));
    }

    public int LengthNameMatch(FileInfo file, bool useFullPath)
    {
        string filename = useFullPath ? file.FullName : file.Name;
        return GetSimplifiedPossibleShowNames().Select(name => FileHelper.SimplifyAndCheckFilenameLength(filename.CompareName(), name, false, false)).Max();
    }

    /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
    public void SetId(TVDoc.ProviderType type, int code)
    {
        switch (type)
        {
            case TVDoc.ProviderType.libraryDefault:
                SetId(GetMediaType() == MediaType.tv ? TVSettings.Instance.DefaultProvider : TVSettings.Instance.DefaultMovieProvider, code);
                break;

            case TVDoc.ProviderType.TVmaze:
                TVmazeCode = code;
                TvdbCode = -1;
                TmdbCode = -1;
                break;

            case TVDoc.ProviderType.TheTVDB:
                TvdbCode = code;
                TmdbCode = -1;
                TVmazeCode = -1;
                break;

            case TVDoc.ProviderType.TMDB:
                TmdbCode = code;
                TvdbCode = -1;
                TVmazeCode = -1;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
