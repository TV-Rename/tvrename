using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

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

        public string ShowNameWithYear => $"{ShowName} ({CachedMovie?.Year})";

        public MovieConfiguration(int code, TVDoc.ProviderType type) : this()
        {
            ConfigurationProvider = type;
            SetId(type, code);
        }

        public MovieConfiguration([NotNull] XElement xmlSettings) : this()
        {
            UseCustomShowName = xmlSettings.ExtractBool("UseCustomShowName", false);
            UseCustomLanguage = xmlSettings.ExtractBool("UseCustomLanguage", false);
            CustomLanguageCode = xmlSettings.ExtractString("CustomLanguageCode");
            UseCustomRegion = xmlSettings.ExtractBool("UseCustomRegion", false);
            CustomRegionCode = xmlSettings.ExtractString("CustomRegionCode");
            CustomShowName = xmlSettings.ExtractString("CustomShowName");
            TvdbCode = xmlSettings.ExtractInt("TVDBID", -1);
            TVmazeCode = xmlSettings.ExtractInt("TVMAZEID", -1);
            TmdbCode = xmlSettings.ExtractInt("TMDBID", -1);
            DoRename = xmlSettings.ExtractBool("DoRename", true);
            DoMissingCheck = xmlSettings.ExtractBool("DoMissingCheck", true);
            ConfigurationProvider = GetConfigurationProviderType(xmlSettings.ExtractInt("ConfigurationProvider"));
            LastName = xmlSettings.ExtractStringOrNull("LastName");
            ImdbCode = xmlSettings.ExtractStringOrNull("ImdbCode");
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

        public MovieConfiguration(PossibleNewMovie movie) : this()
        {
            if (movie.CodeUnknown)
            {
                return;
            }
            switch (movie.SourceProvider)
            {
                case TVDoc.ProviderType.TheTVDB:
                    TvdbCode = movie.ProviderCode;
                    break;

                case TVDoc.ProviderType.TMDB:
                    TmdbCode = movie.ProviderCode;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            ConfigurationProvider = TVSettings.Instance.DefaultMovieProvider == movie.SourceProvider
                ? TVDoc.ProviderType.libraryDefault
                : movie.SourceProvider;
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

            if (UseCustomFolderNameFormat)
            {
                return AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.NameFor(this, CustomFolderNameFormat);
            }

            return AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.NameFor(this, TVSettings.Instance.MovieFolderFormat);
        }

        public (string, string, string) NeighbouringFolderNames()
        {
            int? year = CachedMovie?.Year;

            if (!year.HasValue)
            {
                return (AutoFolderNameForMovie(), AutoFolderNameForMovie(), AutoFolderNameForMovie());
            }
            return (AutoFolderNameForMovie(), AutoFolderNameForMovie(year.Value - 1), AutoFolderNameForMovie(year.Value + 1));
        }

        private string AutoFolderNameForMovie(int year)
        {
            if (string.IsNullOrEmpty(AutomaticFolderRoot))
            {
                return string.Empty;
            }

            if (UseCustomFolderNameFormat)
            {
                return AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.NameFor(this, CustomFolderNameFormat, year);
            }

            return AutomaticFolderRoot.EnsureEndsWithSeparator() + CustomMovieName.NameFor(this, TVSettings.Instance.MovieFolderFormat, year);
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

        private static MediaCache LocalCache(TVDoc.ProviderType provider) => TVDoc.GetMediaCache(provider);

        public CachedMovieInfo? CachedMovie => (CachedMovieInfo)CachedData;

        public IEnumerable<string> Locations => AllFolderLocations(true, false).Values.SelectMany(x => x);

        public string ProposedFilename =>
            UseCustomNamingFormat
                ? CustomMovieName.NameFor(this, CustomNamingFormat)
                : CustomMovieName.NameFor(this, TVSettings.Instance.MovieFilenameFormat);

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
            writer.WriteElement("UseCustomRegion", UseCustomRegion);
            writer.WriteElement("CustomRegionCode", CustomRegionCode);
            writer.WriteElement("TVDBID", TvdbCode);
            writer.WriteElement("TVMAZEID", TVmazeCode);
            writer.WriteElement("TMDBID", TmdbCode);
            writer.WriteElement("DoRename", DoRename);
            writer.WriteElement("ImdbCode", ImdbCode);
            writer.WriteElement("LastName", LastName);
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
