using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TVRename
{
    public abstract class MediaConfiguration : ISeriesSpecifier
    {
        protected static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        public bool DoMissingCheck;
        public bool DoRename;

        public int TvdbCode;

        // ReSharper disable once InconsistentNaming
        public int TVmazeCode;

        public int TmdbCode;
        public string? ImdbCode { get; set; } //todo - make sure this is set
        public Locale TargetLocale { get; }

        public bool UseCustomShowName;
        public string CustomShowName;
        public string LastName;

        public bool UseCustomRegion;
        public string? CustomRegionCode;

        public readonly List<string> AliasNames = new List<string>();
        protected internal TVDoc.ProviderType ConfigurationProvider;

        protected abstract MediaType GetMediaType();

        protected abstract Dictionary<int, SafeList<string>> AllFolderLocations(bool manualToo, bool checkExist);

        public override string ToString() => $"{GetMediaType()}: ({ConfigurationProvider.PrettyPrint()}) TVDB:{TvdbCode} TMDB:{TmdbCode} TVMaze:{TVmazeCode} ({CustomShowName},{CustomLanguageCode},{CustomRegionCode})";

        [NotNull]
        public Dictionary<int, SafeList<string>> AllExistngFolderLocations() => AllFolderLocations(true, true);

        [NotNull]
        public Dictionary<int, SafeList<string>> AllProposedFolderLocations() => AllFolderLocations(true, false);

        [NotNull]
        public Dictionary<int, SafeList<string>> AllFolderLocationsEpCheck(bool checkExist) => AllFolderLocations(true, checkExist);

        [NotNull]
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

        public Language? PreferredLanguage => UseCustomLanguage ? LocalCache().GetLanguageFromCode(CustomLanguageCode!) : LocalCache().PreferredLanguage();

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
            switch (instanceDefaultProvider)
            {
                case TVDoc.ProviderType.TVmaze:
                    return TVmazeCode > 0;

                case TVDoc.ProviderType.TheTVDB:
                    return TvdbCode > 0;

                case TVDoc.ProviderType.TMDB:
                    return TmdbCode > 0;

                case TVDoc.ProviderType.libraryDefault:
                default:
                    throw new ArgumentOutOfRangeException(nameof(instanceDefaultProvider), instanceDefaultProvider, null);
            }
        }

        public static int CompareNames(MediaConfiguration x, MediaConfiguration y)
        {
            string ones = x.ShowName;
            string twos = y.ShowName;
            return string.Compare(ones, twos, StringComparison.Ordinal);
        }

        [NotNull]
        public IEnumerable<string> GetActorNames()
        {
            return Actors.Select(x => x.ActorName);
        }

        public TVDoc.ProviderType Provider
        {
            get
            {
                switch (ConfigurationProvider)
                {
                    case TVDoc.ProviderType.libraryDefault:
                        return DefaultProvider();

                    case TVDoc.ProviderType.TVmaze:
                    case TVDoc.ProviderType.TheTVDB:
                    case TVDoc.ProviderType.TMDB:
                        return ConfigurationProvider;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int TvdbId => TvdbCode;

        public string Name => LastName;

        public MediaType Type => GetMediaType();

        public int TvMazeId => TVmazeCode;

        public int TmdbId => TmdbCode;

        public Language LanguageToUse => UseCustomLanguage
                ? Languages.Instance.GetLanguageFromCode(CustomLanguageCode)
                : TVSettings.Instance.PreferredTVDBLanguage;

        public bool UseCustomLanguage { get; set; }

        public string CustomLanguageCode { get; set; }

        protected abstract TVDoc.ProviderType DefaultProvider();

        [NotNull]
        public IEnumerable<string> Genres => CachedData?.Genres.Distinct() ?? new List<string>();

        [NotNull]
        public IEnumerable<Actor> Actors => CachedData?.GetActors() ?? new List<Actor>();

        [NotNull]
        protected IEnumerable<string> GetSimplifiedPossibleShowNames()
        {
            List<string> possibles = new List<string>();

            string simplifiedShowName = ShowName.CompareName();
            if (simplifiedShowName != "") { possibles.Add(simplifiedShowName); }

            //Check the custom show name too
            if (UseCustomShowName)
            {
                string simplifiedCustomShowName = CustomShowName.CompareName();
                if (simplifiedCustomShowName != "") { possibles.Add(simplifiedCustomShowName); }
            }

            //Also add the aliases provided
            possibles.AddNullableRange(AliasNames.Select(s => s.CompareName()).Where(s => s.HasValue()).Where(s => s.Length > 2));

            //Also use the aliases from theTVDB
            possibles.AddNullableRange(CachedData?.GetAliases().Select(s => s.CompareName()).Where(s => s.HasValue()).Where(s => s.Length > 6));

            return possibles;
        }

        public string ShowName
        {
            get
            {
                if (UseCustomShowName)
                {
                    return CustomShowName;
                }

                CachedMediaInfo ser = CachedData;
                if (ser?.Name.HasValue() ?? false)
                {
                    return ser.Name!;
                }
                if (LastName.HasValue())
                {
                    return LastName;
                }

                return "<" + Code + " not downloaded>";
            }
        }

        protected void SetupAliases([NotNull] XElement xmlSettings)
        {
            foreach (string alias in xmlSettings.Descendants("AliasNames").Descendants("Alias").Select(alias => alias.Value).Distinct())
            {
                AliasNames.Add(alias);
            }
        }

        public bool NameMatch([NotNull] FileSystemInfo file, bool useFullPath) => NameMatch(useFullPath ? file.FullName : file.Name);

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

        public void SetId(TVDoc.ProviderType type, int code)
        {
            switch (type)
            {
                case TVDoc.ProviderType.libraryDefault:
                    SetId(GetMediaType() == MediaType.tv ? TVSettings.Instance.DefaultProvider : TVSettings.Instance.DefaultMovieProvider, code);
                    break;

                case TVDoc.ProviderType.TVmaze:
                    TVmazeCode = code;
                    break;

                case TVDoc.ProviderType.TheTVDB:
                    TvdbCode = code;
                    break;

                case TVDoc.ProviderType.TMDB:
                    TmdbCode = code;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
