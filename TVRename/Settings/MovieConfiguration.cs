using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename
{
    public class MovieConfiguration : MediaConfiguration
    {
        public bool UseManualLocations;
        public readonly List<string> ManualLocations;

        public bool UseAutomaticFolders;
        public string AutomaticFolderRoot;

        public bool UseCustomFolderNameFormat;
        internal string CustomFolderNameFormat;

        public string CustomNamingFormat;
        public bool UseCustomNamingFormat;

        public MovieFolderFormat Format;

        public enum MovieFolderFormat
        {
            singleDirectorySingleFile,
            singleDirectoryMultiFile,
            multiPerDirectory,
            cd,
            dvd
        }

        public MovieConfiguration()
        {
            UseCustomShowName = false;
            CustomShowName = string.Empty;
            UseCustomLanguage = false;
            UseManualLocations = false;
            UseCustomNamingFormat = false;
            UseCustomFolderNameFormat = false;
            UseCustomRegion = false;
            
            ManualLocations = new List<string>();
            CustomNamingFormat = string.Empty;
            CustomFolderNameFormat = string.Empty;
            CustomRegionCode = string.Empty;
            ConfigurationProvider = TVDoc.ProviderType.libraryDefault;

            TvdbCode = -1;
            TVmazeCode = -1;
            TmdbCode = -1;

            Format = TVSettings.Instance.DefMovieFolderFormat;
            DoRename = TVSettings.Instance.DefMovieDoRenaming;
            DoMissingCheck = TVSettings.Instance.DefMovieDoMissingCheck;
            UseAutomaticFolders = TVSettings.Instance.DefMovieUseutomaticFolders;
            AutomaticFolderRoot = TVSettings.Instance.DefMovieUseDefaultLocation ? TVSettings.Instance.DefMovieDefaultLocation ?? string.Empty : string.Empty;
            
        }

        public String ShowNameWithYear => $"{ShowName} ({CachedMovie?.Year})";
        public MovieConfiguration(int code, TVDoc.ProviderType type) : this()
        {
            ConfigurationProvider = type;
            switch (type)
            {
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

        public MovieConfiguration([NotNull] XElement xmlSettings) : this()
        {
            UseCustomShowName = xmlSettings.ExtractBool("UseCustomShowName", false);
            UseCustomLanguage = xmlSettings.ExtractBool("UseCustomLanguage", false);
            CustomLanguageCode = xmlSettings.ExtractString("CustomLanguageCode");
            CustomShowName = xmlSettings.ExtractString("CustomShowName");
            TvdbCode = xmlSettings.ExtractInt("TVDBID", -1);
            TVmazeCode = xmlSettings.ExtractInt("TVMAZEID", -1);
            TmdbCode = xmlSettings.ExtractInt("TMDBID", -1);
            DoRename = xmlSettings.ExtractBool("DoRename", true);
            DoMissingCheck = xmlSettings.ExtractBool("DoMissingCheck", true);
            ConfigurationProvider = GetConfigurationProviderType(xmlSettings.ExtractInt("ConfigurationProvider"));

            UseManualLocations = xmlSettings.ExtractBool("UseManualLocations", false);
            UseAutomaticFolders = xmlSettings.ExtractBool("useAutomaticFolders", true);
            AutomaticFolderRoot = xmlSettings.ExtractString("automaticFolderRoot");
            UseCustomFolderNameFormat = xmlSettings.ExtractBool("UseCustomFolderNameFormat", false);
            CustomFolderNameFormat = xmlSettings.ExtractString("CustomFolderNameFormat");
            CustomNamingFormat = xmlSettings.ExtractString("CustomNamingFormat");
            UseCustomNamingFormat = xmlSettings.ExtractBool("UseCustomNamingFormat", false);

            SetupAliases(xmlSettings);
            SetupLocations(xmlSettings);
        }

        public MovieConfiguration(PossibleNewMovie movie): this()
        {
            TmdbCode = movie.TMDBCode ??-1;
            TvdbCode = movie.TvdbCode ??-1;

            if (!movie.TmdbCodeUnknown)
            {
                ConfigurationProvider = TVSettings.Instance.DefaultMovieProvider == TVDoc.ProviderType.TMDB
                    ? TVDoc.ProviderType.libraryDefault
                    : TVDoc.ProviderType.TMDB;
            }
            else if (!movie.TvdbCodeUnknown)
            {
                ConfigurationProvider = TVSettings.Instance.DefaultMovieProvider == TVDoc.ProviderType.TheTVDB
                    ? TVDoc.ProviderType.libraryDefault
                    : TVDoc.ProviderType.TheTVDB;
            }
        }

        protected override MediaType GetMediaType() => MediaType.movie;
        protected override Dictionary<int, SafeList<string>> AllFolderLocations(bool manualToo, bool checkExist)
        {
            Dictionary<int, SafeList<string>> fld = new Dictionary<int, SafeList<string>>
            {
                [0] = new SafeList<string>()
            };

            if (manualToo && UseManualLocations)
            {
                foreach (string kvp in ManualLocations.ToList())
                {
                    fld[0].Add(kvp.TrimSlash());
                    
                }
            }

            if (UseAutomaticFolders && !string.IsNullOrEmpty(AutomaticFolderRoot))
            {
                string newName = AutoFolderNameForMovie();

                if (!checkExist || Directory.Exists(newName))
                {
                    if (!fld[0].Contains(newName))
                    {
                        fld[0].Add(newName.TrimSlash());
                    }
                }
            }
            return fld;
        }

        private string AutoFolderNameForMovie()
        {
            if (string.IsNullOrEmpty(AutomaticFolderRoot))
            {
                return string.Empty;
            }

            if (UseCustomNamingFormat)
            {
                return AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.NameFor(this,CustomNamingFormat);
            }

            return AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.NameFor(this, TVSettings.Instance.MovieFolderFormat);
        }

        private void SetupLocations([NotNull] XElement xmlSettings)
        {
            foreach (string alias in xmlSettings.Descendants("Locations").Descendants("Location").Select(alias => alias.Value).Where(s => s.HasValue()).Distinct())
            {
                ManualLocations.Add(alias);
            }
        }

        protected override MediaCache LocalCache()
        {
            return Provider switch
            {
                TVDoc.ProviderType.libraryDefault => LocalCache(TVSettings.Instance.DefaultMovieProvider),
                _ => LocalCache(Provider)
            };
        }

        protected override TVDoc.ProviderType DefaultProvider() => TVSettings.Instance.DefaultMovieProvider;

        private static MediaCache LocalCache(TVDoc.ProviderType t)
        {
            return t switch
            {
                TVDoc.ProviderType.TVmaze => TVmaze.LocalCache.Instance,
                TVDoc.ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
                TVDoc.ProviderType.TMDB => TMDB.LocalCache.Instance,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public CachedMovieInfo? CachedMovie => (CachedMovieInfo) CachedData;

        public IEnumerable<string> Locations => AllFolderLocations(true, false).Values.SelectMany(x => x);
        public void WriteXmlSettings([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("MovieItem");

            writer.WriteElement("UseManualLocations", UseManualLocations);

            writer.WriteElement("useAutomaticFolders", UseAutomaticFolders);
            writer.WriteElement("automaticFolderRoot", AutomaticFolderRoot);

            writer.WriteElement("UseCustomFolderNameFormat", UseCustomFolderNameFormat);
            writer.WriteElement("CustomFolderNameFormat", CustomFolderNameFormat);

            writer.WriteElement("CustomNamingFormat", CustomNamingFormat);
            writer.WriteElement("UseCustomNamingFormat", UseCustomNamingFormat);

            writer.WriteElement("UseCustomShowName", UseCustomShowName);
            writer.WriteElement("CustomShowName", CustomShowName);
            writer.WriteElement("UseCustomLanguage", UseCustomLanguage);
            writer.WriteElement("CustomLanguageCode", CustomLanguageCode);
            writer.WriteElement("TVDBID", TvdbCode);
            writer.WriteElement("TVMAZEID", TVmazeCode);
            writer.WriteElement("TMDBID", TmdbCode);
            writer.WriteElement("DoRename", DoRename);
            writer.WriteElement("DoMissingCheck", DoMissingCheck);
            writer.WriteElement("ConfigurationProvider", (int)ConfigurationProvider);

            writer.WriteStartElement("Locations");
            foreach (string str in ManualLocations)
            {
                writer.WriteElement("Location", str);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("AliasNames");
            foreach (string str in AliasNames)
            {
                writer.WriteElement("Alias", str);
            }
            writer.WriteEndElement();

            writer.WriteEndElement(); // ShowItem
        }

        public IEnumerable<string> AutomaticLocations() => AllFolderLocations(false, false).Values.SelectMany(x => x);
        
    }
}
