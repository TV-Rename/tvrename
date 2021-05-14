// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMDbLib.Client;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Trending;
using TMDbLib.Objects.TvShows;
using TVRename.Forms;
using Cast = TMDbLib.Objects.Movies.Cast;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename.TMDB
{
    // ReSharper disable once InconsistentNaming
    public class LocalCache : MediaCache, iMovieSource,iTVSource
    {
        private static readonly TMDbClient Client = new TMDbClient("2dcfd2d08f80439d7ef5210f217b80b4");

        protected internal static readonly List<(string Code, string Name)> COUNTRIES = new List<(string Code, string Name)> { ("AD","Andorra"),
("AE","United Arab Emirates"),
("AF","Afghanistan"),
("AG","Antigua and Barbuda"),
("AI","Anguilla"),
("AL","Albania"),
("AM","Armenia"),
("AN","Netherlands Antilles"),
("AO","Angola"),
("AQ","Antarctica"),
("AR","Argentina"),
("AS","American Samoa"),
("AT","Austria"),
("AU","Australia"),
("AW","Aruba"),
("AZ","Azerbaijan"),
("BA","Bosnia and Herzegovina"),
("BB","Barbados"),
("BD","Bangladesh"),
("BE","Belgium"),
("BF","Burkina Faso"),
("BG","Bulgaria"),
("BH","Bahrain"),
("BI","Burundi"),
("BJ","Benin"),
("BM","Bermuda"),
("BN","Brunei Darussalam"),
("BO","Bolivia"),
("BR","Brazil"),
("BS","Bahamas"),
("BT","Bhutan"),
("BV","Bouvet Island"),
("BW","Botswana"),
("BY","Belarus"),
("BZ","Belize"),
("CA","Canada"),
("CC","Cocos  Islands"),
("CD","Congo"),
("CF","Central African Republic"),
("CG","Congo"),
("CH","Switzerland"),
("CI","Cote D'Ivoire"),
("CK","Cook Islands"),
("CL","Chile"),
("CM","Cameroon"),
("CN","China"),
("CO","Colombia"),
("CR","Costa Rica"),
("CS","Serbia and Montenegro"),
("CU","Cuba"),
("CV","Cape Verde"),
("CX","Christmas Island"),
("CY","Cyprus"),
("CZ","Czech Republic"),
("DE","Germany"),
("DJ","Djibouti"),
("DK","Denmark"),
("DM","Dominica"),
("DO","Dominican Republic"),
("DZ","Algeria"),
("EC","Ecuador"),
("EE","Estonia"),
("EG","Egypt"),
("EH","Western Sahara"),
("ER","Eritrea"),
("ES","Spain"),
("ET","Ethiopia"),
("FI","Finland"),
("FJ","Fiji"),
("FK","Falkland Islands"),
("FM","Micronesia"),
("FO","Faeroe Islands"),
("FR","France"),
("GA","Gabon"),
("GB","United Kingdom"),
("GD","Grenada"),
("GE","Georgia"),
("GF","French Guiana"),
("GH","Ghana"),
("GI","Gibraltar"),
("GL","Greenland"),
("GM","Gambia"),
("GN","Guinea"),
("GP","Guadaloupe"),
("GQ","Equatorial Guinea"),
("GR","Greece"),
("GT","Guatemala"),
("GU","Guam"),
("GW","Guinea-Bissau"),
("GY","Guyana"),
("HK","Hong Kong"),
("HN","Honduras"),
("HR","Croatia"),
("HT","Haiti"),
("HU","Hungary"),
("ID","Indonesia"),
("IE","Ireland"),
("IL","Israel"),
("IN","India"),
("IQ","Iraq"),
("IR","Iran"),
("IS","Iceland"),
("IT","Italy"),
("JM","Jamaica"),
("JO","Jordan"),
("JP","Japan"),
("KE","Kenya"),
("KG","Kyrgyz Republic"),
("KH","Cambodia"),
("KI","Kiribati"),
("KM","Comoros"),
("KN","St. Kitts and Nevis"),
("KP","North Korea"),
("KR","South Korea"),
("KW","Kuwait"),
("KY","Cayman Islands"),
("KZ","Kazakhstan"),
("LA","Lao People's Democratic Republic"),
("LB","Lebanon"),
("LC","St. Lucia"),
("LI","Liechtenstein"),
("LK","Sri Lanka"),
("LR","Liberia"),
("LS","Lesotho"),
("LT","Lithuania"),
("LU","Luxembourg"),
("LV","Latvia"),
("LY","Libyan Arab Jamahiriya"),
("MA","Morocco"),
("MC","Monaco"),
("MD","Moldova"),
("ME","Montenegro"),
("MG","Madagascar"),
("MH","Marshall Islands"),
("MK","Macedonia"),
("ML","Mali"),
("MM","Myanmar"),
("MN","Mongolia"),
("MO","Macao"),
("MP","Northern Mariana Islands"),
("MQ","Martinique"),
("MR","Mauritania"),
("MS","Montserrat"),
("MT","Malta"),
("MU","Mauritius"),
("MV","Maldives"),
("MW","Malawi"),
("MX","Mexico"),
("MY","Malaysia"),
("MZ","Mozambique"),
("NA","Namibia"),
("NC","New Caledonia"),
("NE","Niger"),
("NF","Norfolk Island"),
("NG","Nigeria"),
("NI","Nicaragua"),
("NL","Netherlands"),
("NO","Norway"),
("NP","Nepal"),
("NR","Nauru"),
("NU","Niue"),
("NZ","New Zealand"),
("OM","Oman"),
("PA","Panama"),
("PE","Peru"),
("PF","French Polynesia"),
("PG","Papua New Guinea"),
("PH","Philippines"),
("PK","Pakistan"),
("PL","Poland"),
("PM","St. Pierre and Miquelon"),
("PN","Pitcairn Island"),
("PR","Puerto Rico"),
("PS","Palestinian Territory"),
("PT","Portugal"),
("PW","Palau"),
("PY","Paraguay"),
("QA","Qatar"),
("RE","Reunion"),
("RO","Romania"),
("RS","Serbia"),
("RU","Russia"),
("RW","Rwanda"),
("SA","Saudi Arabia"),
("SB","Solomon Islands"),
("SC","Seychelles"),
("SD","Sudan"),
("SE","Sweden"),
("SG","Singapore"),
("SH","St. Helena"),
("SI","Slovenia"),
("SJ","Svalbard & Jan Mayen Islands"),
("SK","Slovakia"),
("SL","Sierra Leone"),
("SM","San Marino"),
("SN","Senegal"),
("SO","Somalia"),
("SR","Suriname"),
("SS","South Sudan"),
("ST","Sao Tome and Principe"),
("SU","Soviet Union"),
("SV","El Salvador"),
("SY","Syrian Arab Republic"),
("SZ","Swaziland"),
("TC","Turks and Caicos Islands"),
("TD","Chad"),
("TF","French Southern Territories"),
("TG","Togo"),
("TH","Thailand"),
("TJ","Tajikistan"),
("TK","Tokelau"),
("TL","Timor-Leste"),
("TM","Turkmenistan"),
("TN","Tunisia"),
("TO","Tonga"),
("TR","Turkey"),
("TT","Trinidad and Tobago"),
("TV","Tuvalu"),
("TW","Taiwan"),
("TZ","Tanzania"),
("UA","Ukraine"),
("UG","Uganda"),
("US","United States of America"),
("UY","Uruguay"),
("UZ","Uzbekistan"),
("VA","Holy See"),
("VC","St. Vincent and the Grenadines"),
("VE","Venezuela"),
("VG","British Virgin Islands"),
("VI","US Virgin Islands"),
("VN","Vietnam"),
("VU","Vanuatu"),
("WF","Wallis and Futuna Islands"),
("WS","Samoa"),
("XC","Czechoslovakia"),
("XG","East Germany"),
("XK","Kosovo"),
("YE","Yemen"),
("YT","Mayotte"),
("YU","Yugoslavia"),
("ZA","South Africa"),
("ZM","Zambia"),
("ZW","Zimbabwe"),
 };
        protected internal static readonly List<(string Code, string Name)> LANGUAGES = new List<(string Code, string Name)> { ("ar-AE","Arabic"),
            ("be-BY","Belarusian"),
            ("bg-BG","Bulgarian"),
            ("bn-BD","Bangla (India)"),
            ("ca-ES","Catalan (Spain)"),
            ("cn-CN","Chinese"),
            ("cs-CZ","Czech (Czech Republic)"),
            ("da-DK","Danish"),
            ("de-AT","German (Austrian)"),
            ("de-CH","German (Swiss)"),
            ("de-DE","German"),
            ("el-GR","Greek"),
            ("en-AU","English (Australian)"),
            ("en-CA","English (Canadian)"),
            ("en-GB","English"),
            ("en-IE","English (Ireland)"),
            ("en-NZ","English (New Zealand)"),
            ("en-US","English (United States)"),
            ("eo-EO","Esperanto "),
            ("es-ES","Spanish"),
            ("es-MX","Spanish (Mexico)"),
            ("et-EE","Estonian "),
            ("eu-ES","Basque (Spain)"),
            ("fa-IR","Farsi"),
            ("fi-FI","Finnish"),
            ("fr-CA","French (Canada)"),
            ("fr-FR","French"),
            ("gl-ES","Galician (Spain)"),
            ("he-IL","Hebrew (Israel)"),
            ("hi-IN","Hindi (India)"),
            ("hu-HU","Hungarian"),
            ("id-ID","Indonesian"),
            ("it-IT","Italian"),
            ("ja-JP","Japanese"),
            ("ka-GE","Georgian"),
            ("kk-KZ","Kazakh"),
            ("kn-IN","Kannada (India)"),
            ("ko-KR","Korean"),
            ("lt-LT","Lithuanian"),
            ("lv-LV","Latvian"),
            ("ml-IN","Malayalam (India)"),
            ("ms-MY","Malay (Malaysia)"),
            ("ms-SG","Malay (Singapore)"),
            ("nl-NL","Dutch (Netherlands)"),
            ("no-NO","Norwegian"),
            ("pl-PL","Polish"),
            ("pt-BR","Portuguese (Brazil)"),
            ("pt-PT","Portuguese"),
            ("ro-RO","Romanian"),
            ("ru-RU","Russian"),
            ("si-LK","Sinhala (Sri Lanka)"),
            ("sk-SK","Slovak"),
            ("sl-SI","Slovenian"),
            ("sq-AL","Albanian"),
            ("sr-RS","Serbian"),
            ("sv-SE","Swedish"),
            ("ta-IN","Tamil (India)"),
            ("te-IN","Telugu (India)"),
            ("th-TH","Thai"),
            ("tl-PH","Tagalog (Philippines)"),
            ("tr-TR","Turkish"),
            ("uk-UA","Ukrainian"),
            ("vi-VN","Vietnamese"),
            ("zh-CN","Chinese"),
            ("zh-HK","Chinese (Hong Kong)"),
            ("zh-TW","Chinese (Taiwan)"),
            ("zu-ZA","Zulu (South Africa)"),
        };
        
        private UpdateTimeTracker latestMovieUpdateTime;
        private UpdateTimeTracker latestTvUpdateTime;

        //We are using the singleton design pattern
        //http://msdn.microsoft.com/en-au/library/ff650316.aspx

        private static volatile LocalCache? InternalInstance;
        private static readonly object SyncRoot = new object();

        [NotNull]
        public static LocalCache Instance
        {
            get
            {
                if (InternalInstance is null)
                {
                    lock (SyncRoot)
                    {
                        // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                        if (InternalInstance is null)
                        {
                            InternalInstance = new LocalCache();
                        }
                    }
                }

                return InternalInstance;
            }
        }
        
        public void Setup(FileInfo? loadFrom, FileInfo cache, CommandLineArgs cla)
        {
            System.Diagnostics.Debug.Assert(cache != null);
            CacheFile = cache;

            //assume that the data is up to date (this will be overridden by the value in the XML if we have a prior install)
            //If we have no prior install then the app has no shows and is by definition up-to-date
            latestMovieUpdateTime = new UpdateTimeTracker();
            latestTvUpdateTime = new UpdateTimeTracker();

            LastErrorMessage = string.Empty;

            LoadOk = loadFrom is null || (CachePersistor.LoadMovieCache(loadFrom, this) && CachePersistor.LoadTvCache(loadFrom, this));  
        }

        public bool Connect(bool showErrorMsgBox) => true;

        public void SaveCache()
        {
            lock (MOVIE_LOCK)
            {
                lock (SERIES_LOCK)
                {
                    CachePersistor.SaveCache(Series, Movies, CacheFile, latestMovieUpdateTime.LastSuccessfulServerUpdateTimecode());
                }
            }
        }

        public override bool EnsureUpdated(SeriesSpecifier s, bool bannersToo, bool showErrorMsgBox)
        {
            if (s.Provider != TVDoc.ProviderType.TMDB)
            {
                throw new SourceConsistencyException($"Asked to update {s.Name} from TMDB, but the Id is not for TMDB.", TVDoc.ProviderType.TMDB);
            }

            if (s.Type == MediaConfiguration.MediaType.movie)
            {
                return EnsureMovieUpdated(s.TmdbId, s.LanguageCode,s.Name, showErrorMsgBox);
            }

            return EnsureSeriesUpdated(s, showErrorMsgBox);
        }

        private bool EnsureSeriesUpdated(SeriesSpecifier s, bool showErrorMsgBox)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(s.TmdbId) && !Series[s.TmdbId].Dirty)
                {
                    return true;
                }
            }

            Say($"Series {s.Name} from TMDB");
            try
            {
                CachedSeriesInfo downloadedSi = DownloadSeriesNow(s, showErrorMsgBox);

                if (downloadedSi.TmdbCode != s.TmdbId && s.TmdbId == -1)
                {
                    lock (SERIES_LOCK)
                    {
                        Series.TryRemove(-1, out _);
                    }
                }

                if (downloadedSi.TmdbCode != s.TmdbId && s.TmdbId == 0)
                {
                    lock (SERIES_LOCK)
                    {
                        Series.TryRemove(0, out _);
                    }
                }

                lock (SERIES_LOCK)
                {
                    AddSeriesToCache(downloadedSi);
                }
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return true;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return true;
            }
            finally
            {
                SayNothing();
            }

            return true;
        }

        private bool EnsureMovieUpdated(int  id, string languageCode, string name, bool showErrorMsgBox)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id) && !Movies[id].Dirty)
                {
                    return true;
                }
            }

            Say($"{name} from TMDB");
            try
            {
                CachedMovieInfo downloadedSi = DownloadMovieNow(id,languageCode, showErrorMsgBox);

                if (downloadedSi.TmdbCode != id && id == -1)
                {
                    lock (MOVIE_LOCK)
                    {
                        Movies.TryRemove(-1, out _);
                    }
                }

                lock (MOVIE_LOCK)
                {
                    AddMovieToCache(downloadedSi);
                }
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return true;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return true;
            }
            finally
            {
                SayNothing();
            }

            return true;
        }

        private void AddMovieToCache([NotNull] CachedMovieInfo si)
        {
            int id = si.TmdbCode;
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id))
                {
                    Movies[id].Merge(si);
                }
                else
                {
                    Movies[id] = si;
                }
            }
        }

        private void AddSeriesToCache([NotNull] CachedSeriesInfo si)
        {
            int id = si.TmdbCode;
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(id))
                {
                    Series[id].Merge(si);
                }
                else
                {
                    Series[id] = si;
                }
            }
        }
        public bool GetUpdates(bool showErrorMsgBox, CancellationToken cts, IEnumerable<SeriesSpecifier> ss)
        {
            Say("Validating TMDB cache");
            MarkPlaceHoldersDirty(ss);

            try
            {
                Say($"Updates list from TMDB since {latestMovieUpdateTime.LastSuccessfulServerUpdateDateTime()}");

                long updateFromEpochTime = latestMovieUpdateTime.LastSuccessfulServerUpdateTimecode();
                if (updateFromEpochTime == 0)
                {
                    MarkAllDirty();
                    latestMovieUpdateTime.RegisterServerUpdate(DateTime.Now.ToUnixTime());
                    return true;
                }

                List<int> updates = Client.GetChangesMovies(cts, latestMovieUpdateTime).Select(item => item.Id).Distinct().ToList();

                Say($"Processing {updates.Count} updates from TMDB. From between {latestMovieUpdateTime.LastSuccessfulServerUpdateDateTime()} and {latestMovieUpdateTime.ProposedServerUpdateDateTime()}");
                foreach (int id in updates)
                {
                    if (!cts.IsCancellationRequested)
                    {
                        if (HasMovie(id))
                        {
                            CachedMovieInfo? x = GetMovie(id);
                            if (!(x is null))
                            {
                                LOGGER.Info(
                                    $"Identified that Movie with TMDB Id {id} {x.Name} should be updated.");

                                x.Dirty = true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                lock (MOVIE_LOCK)
                {
                    Say($"Identified {Movies.Values.Count(info => info.Dirty && !info.IsSearchResultOnly)} TMDB Movies need updating");
                    LOGGER.Info(Movies.Values.Where(info => info.Dirty && !info.IsSearchResultOnly).Select(info => info.Name).ToCsv);
                }
                return true;
            }
            catch (SourceConnectivityException conex)
            {
                LastErrorMessage = conex.Message;
                return false;
            }
            catch (SourceConsistencyException sce)
            {
                LOGGER.Error(sce.Message);
                LastErrorMessage = sce.Message;
                return false;
            }
            finally
            {
                SayNothing();
            }

        }

        private void MarkPlaceHoldersDirty(IEnumerable<SeriesSpecifier> ss)
        {
            foreach (SeriesSpecifier downloadShow in ss)
            {
                if (downloadShow.Type == MediaConfiguration.MediaType.tv)
                {
                    if (!HasShow(downloadShow.TmdbId))
                    {
                        AddPlaceholderSeries(downloadShow);
                    }
                    else
                    {
                        CachedSeriesInfo? Show = GetSeries(downloadShow.TmdbId);
                        Show?.UpgradeSearchResultToDirty();
                    }
                }
                else
                {
                    if (!HasMovie(downloadShow.TmdbId))
                    {
                        AddPlaceholderMovie(downloadShow);
                    }
                    else
                    {
                        CachedMovieInfo? movie = GetMovie(downloadShow.TmdbId);
                        movie?.UpgradeSearchResultToDirty();
                    }
                }
            }
        }

        private void MarkAllDirty()
        {
            lock (MOVIE_LOCK)
            {
                foreach (CachedMovieInfo m in Movies.Values)
                {
                    m.Dirty = true;
                }
            }
        }

        private void MarkPlaceholdersDirty()
        {
            lock (MOVIE_LOCK)
            {
                // anything with a srv_lastupdated of 0 should be marked as dirty
                // typically, this'll be placeholder cachedSeries
                foreach (CachedMovieInfo ser in Movies.Values.Where(ser => ser.SrvLastUpdated == 0))
                {
                    ser.Dirty = true;
                }
            }
        }

        private void AddPlaceholderSeries([NotNull] SeriesSpecifier ss)
            => AddPlaceholderSeries(ss.TvdbSeriesId, ss.TvMazeSeriesId,ss.TmdbId, ss.LanguageCode);

        private void AddPlaceholderMovie([NotNull] SeriesSpecifier ss)
            => AddPlaceholderMovie(ss.TvdbSeriesId, ss.TvMazeSeriesId, ss.TmdbId, ss.LanguageCode);


        public void UpdatesDoneOk()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            latestMovieUpdateTime.RecordSuccessfulUpdate();
        }

        public CachedSeriesInfo GetSeries(string showName, bool showErrorMsgBox, string languageCode)
        {
            throw new NotImplementedException(); //todo - (BulkAdd Manager needs to work for new providers)
        }

        public CachedMovieInfo GetMovie(PossibleNewMovie show, string languageCode, bool showErrorMsgBox) => GetMovie(show.RefinedHint, show.PossibleYear,languageCode, showErrorMsgBox, false);

        public CachedMovieInfo? GetMovie(string hint, int? possibleYear, string languageCode, bool showErrorMsgBox,bool useMostPopularMatch)
        {
            Search(hint, showErrorMsgBox,MediaConfiguration.MediaType.movie,languageCode);

            string showName = hint;

            if (string.IsNullOrEmpty(showName))
            {
                return null;
            }

            showName = showName.ToLower();

            List<CachedMovieInfo> matchingShows = GetSeriesDictMatching(showName).Values.ToList();

            if (matchingShows.Count == 0)
            {
                return null;
            }

            if (matchingShows.Count == 1)
            {
                return matchingShows.First();
            }

            List<CachedMovieInfo> exactMatchingShows = matchingShows
                .Where(info => info.Name.CompareName().Equals(showName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (exactMatchingShows.Count == 0 && !useMostPopularMatch)
            {
                return null;
            }

            if (exactMatchingShows.Count == 1)
            {
                return exactMatchingShows.First();
            }

            if (possibleYear is null && !useMostPopularMatch)
            {
                return null;
            }

            if (possibleYear != null)
            {
                List<CachedMovieInfo> exactMatchingShowsWithYear = exactMatchingShows
                    .Where(info => info.Year == possibleYear).ToList();

                if (exactMatchingShowsWithYear.Count == 0 && !useMostPopularMatch)
                {
                    return null;
                }

                if (exactMatchingShowsWithYear.Count == 1)
                {
                    return exactMatchingShowsWithYear.First();
                }
            }
            if (!useMostPopularMatch)
            {
                return null;
            }

            if (matchingShows.All(s => s.Popularity.HasValue))
            {
                return matchingShows.OrderByDescending(s => s.Popularity).First();
            }
            return null;
        }

        [NotNull]
        private Dictionary<int, CachedMovieInfo> GetSeriesDictMatching(string testShowName)
        {
            Dictionary<int, CachedMovieInfo> matchingSeries = new Dictionary<int, CachedMovieInfo>();

            testShowName = testShowName.CompareName();

            if (string.IsNullOrEmpty(testShowName))
            {
                return matchingSeries;
            }

            lock (MOVIE_LOCK)
            {
                foreach (KeyValuePair<int, CachedMovieInfo> kvp in Movies)
                {
                    string show = kvp.Value.Name.CompareName();

                    if (show.Contains(testShowName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //We have a match
                        matchingSeries.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            return matchingSeries;
        }

        public CachedMovieInfo? GetMovie(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }
            lock (MOVIE_LOCK)
            {
                return HasMovie(id.Value) ? Movies[id.Value] : null;
            }
        }
        public CachedSeriesInfo? GetSeries(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }
            lock (SERIES_LOCK)
            {
                return HasShow(id.Value) ? Series[id.Value] : null;
            }
        }
        public bool HasMovie(int id)
        {
            lock (MOVIE_LOCK)
            {
                return Movies.ContainsKey(id);
            }
        }

        public bool HasShow(int id)
        {
            lock (SERIES_LOCK)
            {
                return Series.ContainsKey(id);
            }
        }

        public void Tidy(IEnumerable<MovieConfiguration> libraryValues)
        {
            // remove any shows from cache that aren't in My Movies
            List<int> removeList = new List<int>();

            lock (MOVIE_LOCK)
            {
                foreach (KeyValuePair<int, CachedMovieInfo> kvp in Movies)
                {
                    bool found = libraryValues.Any(si => si.TmdbCode == kvp.Key);
                    if (!found)
                    {
                        removeList.Add(kvp.Key);
                    }
                }

                foreach (int i in removeList)
                {
                    ForgetMovie(i);
                }
            }
        }

        public void Tidy(IEnumerable<ShowConfiguration> libraryValues)
        {
            // remove any shows from TMDB that aren't in My Shows
            List<int> removeList = new List<int>();

            lock (SERIES_LOCK)
            {
                foreach (KeyValuePair<int, CachedSeriesInfo> kvp in Series)
                {
                    if (libraryValues.All(si => si.TmdbCode != kvp.Key))
                    {
                        removeList.Add(kvp.Key);
                    }
                }

                foreach (int i in removeList)
                {
                    ForgetShow(i);
                }
            }
        }

        public void ForgetEverything()
        {
            lock (MOVIE_LOCK)
            {
                Movies.Clear();
            }
            lock (SERIES_LOCK)
            {
                Series.Clear();
            }

            SaveCache();
            //All cachedSeries will be forgotten and will be fully refreshed, so we'll only need updates after this point
            latestMovieUpdateTime.Reset();
            LOGGER.Info($"Forget everything, so we assume we have TMDB updates until {latestMovieUpdateTime}");
        }

        public void ForgetShow(int id)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(id))
                {
                    Series.TryRemove(id, out _);
                }
            }
        }

        public void ForgetShow(int tvdb, int tvmaze, int tmdb, bool makePlaceholder, bool useCustomLanguage, string langCode)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(tmdb))
                {
                    Series.TryRemove(tmdb, out CachedSeriesInfo _);
                    if (makePlaceholder)
                    {
                        if (useCustomLanguage && langCode.HasValue())
                        {
                            AddPlaceholderSeries(tvdb, tvmaze,tmdb, langCode!);
                        }
                        else
                        {
                            AddPlaceholderSeries(tvdb, tvmaze,tmdb);
                        }
                    }
                }
                else
                {
                    if (tvmaze > 0 && makePlaceholder)
                    {
                        AddPlaceholderSeries(tvdb, tvmaze, tmdb);
                    }
                }
            }
        }

        public void UpdateSeries(CachedSeriesInfo si)
        {
            lock (SERIES_LOCK)
            {
                Series[si.TmdbCode] = si;
            }
        }

        public void AddOrUpdateEpisode(Episode e)
        {
            lock (SERIES_LOCK)
            {
                if (!Series.ContainsKey(e.SeriesId))
                {
                    throw new SourceConsistencyException(
                        $"Can't find the cachedSeries to add the episode to (TVMaze). EpId:{e.EpisodeId} SeriesId:{e.SeriesId} {e.Name}", TVDoc.ProviderType.TMDB);
                }

                CachedSeriesInfo ser = Series[e.SeriesId];

                ser.AddEpisode(e);
            }
        }

        public void AddBanners(int seriesId, IEnumerable<Banner> seriesBanners)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(seriesId))
                {
                    foreach (Banner b in seriesBanners)
                    {
                        if (!Series.ContainsKey(b.SeriesId))
                        {
                            throw new SourceConsistencyException(
                                $"Can't find the cachedSeries to add the banner {b.BannerId} to (TheTVDB). {seriesId},{b.SeriesId}", TVDoc.ProviderType.TMDB);
                        }

                        CachedSeriesInfo ser = Series[b.SeriesId];

                        ser.AddOrUpdateBanner(b);
                    }

                    Series[seriesId].BannersLoaded = true;
                }
                else
                {
                    LOGGER.Warn($"Banners were found for cachedSeries {seriesId} - Ignoring them.");
                }
            }
        }

        public void ForgetMovie(int id)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(id))
                {
                    Movies.TryRemove(id, out _);
                }
            }
        }

        public void ForgetMovie(int tvdb,int tvmaze,int tmdb, bool makePlaceholder, bool useCustomLanguage, string? langCode)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(tmdb))
                {
                    Movies.TryRemove(tmdb, out CachedMovieInfo _);
                    if (makePlaceholder)
                    {
                        if (useCustomLanguage && langCode.HasValue())
                        {
                            AddPlaceholderSeries(tvdb,tvmaze,tmdb, langCode!);
                        }
                        else
                        {
                            AddPlaceholderSeries(tvdb, tvmaze,tmdb);
                        }
                    }
                }
                else
                {
                    if (tmdb > 0 && makePlaceholder)
                    {
                        AddPlaceholderSeries(tvdb, tvmaze,tmdb);
                    }
                }
            }
        }

        public void AddPoster(int seriesId, IEnumerable<Banner> select)
        {
            throw new NotImplementedException(); //TODO
        }

        private void AddPlaceholderMovie(int tvdb, int tvmaze,int tmdb) 
        {
            lock (MOVIE_LOCK)
            {
                Movies[tmdb] = new CachedMovieInfo(tvdb,tvmaze,tmdb) { Dirty = true };
            }
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze, int tmdb)
        {
            lock (SERIES_LOCK)
            {
                Series[tmdb] = new CachedSeriesInfo(tvdb, tvmaze, tmdb) { Dirty = true };
            }
        }

        private void AddPlaceholderSeries(int tvdb, int tvmaze, int tmdb, string customLanguageCode)
        {
            lock (SERIES_LOCK)
            {
                Series[tmdb] = new CachedSeriesInfo(tvdb, tvmaze, tmdb, customLanguageCode) {Dirty = true};
            }
        }

        private void AddPlaceholderMovie(int tvdb, int tvmaze, int tmdb, string customLanguageCode)
        {
            lock (MOVIE_LOCK)
            {
                Movies[tmdb] = new CachedMovieInfo(tvdb, tvmaze, tmdb, customLanguageCode) { Dirty = true };
            }
        }

        public void Update(CachedMovieInfo si)
        {
            lock (MOVIE_LOCK)
            {
                Movies[si.TmdbCode] = si;
            }
        }

        public void LatestUpdateTimeIs(string time)
        {
            latestMovieUpdateTime.Load(time);
            LOGGER.Info($"Loaded file with updates until {latestMovieUpdateTime.LastSuccessfulServerUpdateDateTime()}");
        }

        public override TVDoc.ProviderType Provider() => TVDoc.ProviderType.TMDB;

        public Language PreferredLanguage => throw new NotImplementedException();

        public Language GetLanguageFromCode(string customLanguageCode) => throw new NotImplementedException();

        public CachedMovieInfo GetMovieAndDownload(int id, string languageCode, bool showErrorMsgBox) => HasMovie(id)
            ? CachedMovieData[id]
            : DownloadMovieNow(id,languageCode,showErrorMsgBox);

        internal CachedMovieInfo DownloadMovieNow(int id,string languageCode, bool showErrorMsgBox)
        {
            Movie downloadedMovie = Client.GetMovieAsync(id,languageCode,languageCode, MovieMethods.ExternalIds|MovieMethods.Images|MovieMethods.AlternativeTitles|MovieMethods.ReleaseDates |MovieMethods.Changes|MovieMethods.Videos|MovieMethods.Credits).Result;
            if (downloadedMovie is null)
            {
                throw new MediaNotFoundException(id,"TMDB no longer has this movie",TVDoc.ProviderType.TMDB,TVDoc.ProviderType.TMDB);
            }
            CachedMovieInfo m = new CachedMovieInfo
            {
                Imdb = downloadedMovie.ExternalIds.ImdbId,
                TmdbCode = downloadedMovie.Id,
                Name = downloadedMovie.Title,
                Runtime = downloadedMovie.Runtime.ToString(),
                FirstAired = GetReleaseDateDetail(downloadedMovie, TVSettings.Instance.TMDBRegion) ?? downloadedMovie.ReleaseDate,
                Genres = downloadedMovie.Genres.Select(genre => genre.Name).ToList(),
                Overview = downloadedMovie.Overview,
                Network = downloadedMovie.ProductionCompanies.FirstOrDefault()?.Name, //TODO UPdate Movie to include multiple production companies
                Status = downloadedMovie.Status,
                ShowLanguage= downloadedMovie.OriginalLanguage,
                SiteRating = (float)downloadedMovie.VoteAverage,
                SiteRatingVotes = downloadedMovie.VoteCount,
                PosterUrl = PosterImageUrl(downloadedMovie.PosterPath),
                SrvLastUpdated = DateTime.UtcNow.Date.ToUnixTime(),
                CollectionName = downloadedMovie.BelongsToCollection?.Name,
                CollectionId = downloadedMovie.BelongsToCollection?.Id,
                TagLine = downloadedMovie.Tagline,
                Popularity = downloadedMovie.Popularity,
                TwitterId=downloadedMovie.ExternalIds.TwitterId,
                InstagramId = downloadedMovie.ExternalIds.InstagramId,
                FacebookId=downloadedMovie.ExternalIds.InstagramId,
                FanartUrl = OriginalImageUrl(downloadedMovie.BackdropPath),
                ContentRating = GetCertification(downloadedMovie, TVSettings.Instance.TMDBRegion) ?? GetCertification(downloadedMovie, "US") ?? string.Empty,
                OfficialUrl =downloadedMovie.Homepage,
                TrailerUrl = GetYouTubeUrl(downloadedMovie),
                Dirty = false,
            };

            foreach (string? s in downloadedMovie.AlternativeTitles.Titles.Select(title => title.Title))
            {
                m.AddAlias(s);
            }
            foreach (Cast? s in downloadedMovie.Credits.Cast)
            {
                m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath),s.Name,s.Character,s.CastId,s.Order));
            }
            foreach (TMDbLib.Objects.General.Crew? s in downloadedMovie.Credits.Crew)
            {
                m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name,  s.Job, s.Department, s.CreditId));
            }

            File(m);

            return m;
        }

        private DateTime? GetReleaseDateDetail(Movie downloadedMovie, string? country)
        {
            IOrderedEnumerable<DateTime> dates = downloadedMovie.ReleaseDates?.Results
                .Where(rel => rel.Iso_3166_1 == country)
                .SelectMany(rel => rel.ReleaseDates)
                .Select(d=>d.ReleaseDate)
                .OrderBy(time => time);

            if (dates?.Any() ?? false)
            {
                return dates.First();
            }

            return null;
        }

        internal CachedSeriesInfo DownloadSeriesNow(SeriesSpecifier ss, bool showErrorMsgBox)
        {
            int id = ss.TmdbId > 0 ? ss.TmdbId : GetSeriesIdFromOtherCodes(ss) ?? 0;

            TvShow? downloadedSeries = Client.GetTvShowAsync(id,TvShowMethods.ExternalIds | TvShowMethods.Images | TvShowMethods.AlternativeTitles | TvShowMethods.ContentRatings | TvShowMethods.Changes | TvShowMethods.Videos | TvShowMethods.Credits,ss.LanguageCode).Result;
            if (downloadedSeries is null)
            {
                throw new MediaNotFoundException(id, "TMDB no longer has this show", TVDoc.ProviderType.TMDB, TVDoc.ProviderType.TMDB);
            }
            CachedSeriesInfo m = new CachedSeriesInfo
            {
                Imdb = downloadedSeries.ExternalIds.ImdbId,
                TmdbCode = downloadedSeries.Id,
                TvdbCode = downloadedSeries.ExternalIds.TvdbId.ToInt(ss.TvdbSeriesId),
                TvMazeCode = -1,
                Name = downloadedSeries.Name,
                Runtime = downloadedSeries.EpisodeRunTime.FirstOrDefault().ToString(System.Globalization.CultureInfo.CurrentCulture), //todo  use average?
                FirstAired = downloadedSeries.FirstAirDate,
                Genres = downloadedSeries.Genres.Select(genre => genre.Name).ToList(),
                Overview = downloadedSeries.Overview,
                Network = downloadedSeries.Networks.FirstOrDefault()?.Name, //TODO Utilise multiple networks
                Status = MapStatus(downloadedSeries.Status),
                ShowLanguage = downloadedSeries.OriginalLanguage,
                SiteRating = (float)downloadedSeries.VoteAverage,
                SiteRatingVotes = downloadedSeries.VoteCount,
                PosterUrl = PosterImageUrl(downloadedSeries.PosterPath),
                SrvLastUpdated = DateTime.UtcNow.Date.ToUnixTime(),
                TagLine = downloadedSeries.Tagline,
                TwitterId = downloadedSeries.ExternalIds.TwitterId,
                InstagramId = downloadedSeries.ExternalIds.InstagramId,
                FacebookId = downloadedSeries.ExternalIds.InstagramId,
                //FanartUrl = OriginalImageUrl(downloadedSeries.BackdropPath),  //TODO **** on Website
                ContentRating = GetCertification(downloadedSeries, TVSettings.Instance.TMDBRegion) ?? GetCertification(downloadedSeries, "US") ?? string.Empty,// todo allow user to choose
                OfficialUrl = downloadedSeries.Homepage,
                Type = downloadedSeries.Type,
                TrailerUrl = GetYouTubeUrl(downloadedSeries),
                Popularity = downloadedSeries.Popularity,
                Dirty = false,
            };

            foreach (string? s in downloadedSeries.AlternativeTitles.Results.Select(title => title.Title))
            {
                m.AddAlias(s);
            }
            foreach (TMDbLib.Objects.TvShows.Cast? s in downloadedSeries.Credits.Cast)
            {
                m.AddActor(new Actor(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Character, 0, s.Order));
            }
            foreach (TMDbLib.Objects.General.Crew? s in downloadedSeries.Credits.Crew)
            {
                m.AddCrew(new Crew(s.Id, OriginalImageUrl(s.ProfilePath), s.Name, s.Job, s.Department, s.CreditId));
            }
            if (downloadedSeries.Images.Backdrops.Any())
            {
                double bestBackdropRating = downloadedSeries.Images.Backdrops.Select(x => x.VoteAverage).Max();
                foreach (ImageData? image in downloadedSeries.Images.Backdrops.Where(x =>
                    Math.Abs(x.VoteAverage - bestBackdropRating) < .01))
                {
                    Banner newBanner = new Banner(downloadedSeries.Id)
                    {
                        BannerId = 1,
                        BannerPath = OriginalImageUrl(image.FilePath),
                        BannerType = "fanart",
                        Rating = image.VoteAverage,
                        RatingCount = image.VoteCount
                    };

                    m.AddOrUpdateBanner(newBanner);
                }
            }
            if (downloadedSeries.Images.Posters.Any()) { 
                double bestPosterRating = downloadedSeries.Images.Posters.Select(x => x.VoteAverage).Max();
                foreach (ImageData? image in downloadedSeries.Images.Posters.Where(x =>
                    Math.Abs(x.VoteAverage - bestPosterRating) < .01))
                {
                    Banner newBanner = new Banner(downloadedSeries.Id)
                    {
                        BannerId = 2,
                        BannerPath = PosterImageUrl(image.FilePath),
                        BannerType = "poster",
                        Rating = image.VoteAverage,
                        RatingCount = image.VoteCount
                    };

                    m.AddOrUpdateBanner(newBanner);
                }
            }

            foreach (var searchSeason in downloadedSeries.Seasons)
            {
                int snum = searchSeason.SeasonNumber;
                TvSeason? downloadedSeason = Client.GetTvSeasonAsync(downloadedSeries.Id, snum, TvSeasonMethods.Images, ss.LanguageCode).Result;
                
                Season newSeason = new Season(downloadedSeason.Id??0,snum,downloadedSeason.Name,downloadedSeason.Overview,string.Empty, downloadedSeason.PosterPath,downloadedSeries.Id);
                m.AddSeason(newSeason);

                foreach (TvSeasonEpisode? downloadedEpisode in downloadedSeason.Episodes)
                {
                    Episode newEpisode = new Episode(downloadedSeries.Id, m)
                    {
                        Name = downloadedEpisode.Name,
                        Overview = downloadedEpisode.Overview,
                        AirTime = downloadedEpisode.AirDate,
                        AirStamp = downloadedEpisode.AirDate,
                        FirstAired = downloadedEpisode.AirDate,
                        AiredEpNum = downloadedEpisode.EpisodeNumber,
                        AiredSeasonNumber = downloadedEpisode.SeasonNumber,
                        ProductionCode = downloadedEpisode.ProductionCode,
                        EpisodeId = downloadedEpisode.Id,
                        SiteRatingCount = downloadedEpisode.VoteCount,
                        EpisodeRating =
                            downloadedEpisode.VoteAverage.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        SeasonId = newSeason.SeasonId,
                        Filename = OriginalImageUrl(downloadedEpisode.StillPath),
                        EpisodeDirector = downloadedEpisode.Crew
                            .Where(x => x.Department == "Directing" && x.Job == "Director").Select(crew => crew.Name)
                            .ToPsv(),
                        EpisodeGuestStars = downloadedEpisode.GuestStars.Select(c => c.Name).ToPsv(),
                        Writer = downloadedEpisode.Crew
                            .Where(x => x.Department == "Writing").Select(crew => crew.Name)
                            .ToPsv()
                    };

                    m.AddEpisode(newEpisode);
                }

                if (downloadedSeason.Images != null && downloadedSeason.Images.Posters.Count>0)
                {
                    double bestRating = downloadedSeason.Images.Posters.Select(x => x.VoteAverage).Max();
                    foreach (ImageData? image in downloadedSeason.Images.Posters.Where(x=>Math.Abs(x.VoteAverage - bestRating) < .01))
                    {
                        Banner newBanner = new Banner(downloadedSeries.Id)
                        {
                            BannerId  = 10+snum,
                            BannerPath = OriginalImageUrl(image.FilePath),
                            BannerType = "season",
                            Rating = image.VoteAverage,
                            RatingCount = image.VoteCount,
                            SeasonId = downloadedSeason.Id??0
                        };

                        m.AddOrUpdateBanner(newBanner);
                    }
                }
            }

            m.BannersLoaded = true;

            File(m);

            return m;
        }
        private static string MapStatus(string s)
        {
            if (s == "Returning Series")
            {
                return "Continuing";
            }

            return s;
        }

        private int? GetSeriesIdFromOtherCodes(SeriesSpecifier ss)
        {
            if (ss.ImdbCode.HasValue())
            {
                FindContainer? x = Client.FindAsync(FindExternalSource.Imdb, ss.ImdbCode).Result;

                if (ss.Type == MediaConfiguration.MediaType.tv)
                {
                    foreach (SearchTv? show in x.TvResults)
                    {
                        return show.Id;
                    }
                }
                else if (ss.Type ==MediaConfiguration.MediaType.movie)
                {
                    foreach (SearchMovie? show in x.MovieResults)
                    {
                        return show.Id;
                    }
                }
            }

            if (ss.TvdbSeriesId>0)
            {
                FindContainer? x = Client.FindAsync(FindExternalSource.TvDb, ss.TvdbSeriesId.ToString()).Result;

                if (ss.Type == MediaConfiguration.MediaType.tv)
                {
                    foreach (SearchTv? show in x.TvResults)
                    {
                        return show.Id;
                    }
                }
                else if (ss.Type == MediaConfiguration.MediaType.movie)
                {
                    foreach (SearchMovie? show in x.MovieResults)
                    {
                        return show.Id;
                    }
                }
            }

            return null;
        }

        private string GetYouTubeUrl(Movie downloadedMovie)
        {
            string yid = downloadedMovie.Videos.Results.Where(video => video.Type == "Trailer" && video.Site == "YouTube").OrderByDescending(v => v.Size).Select(video => video.Key).FirstOrDefault() ?? string.Empty;
            return yid.HasValue() ? $"https://www.youtube.com/watch?v={yid}" : string.Empty;
        }

        private string GetYouTubeUrl(TvShow downloadedMovie)
        {
            string yid = downloadedMovie.Videos.Results.Where(video => video.Type == "Trailer" && video.Site == "YouTube").OrderByDescending(v => v.Size).Select(video => video.Key).FirstOrDefault() ?? string.Empty;
            return yid.HasValue() ? $"https://www.youtube.com/watch?v={yid}" : string.Empty;
        }
        private string? GetCertification(Movie downloadedMovie, string country)
        {
            return downloadedMovie.ReleaseDates?.Results
                .Where(rel => rel.Iso_3166_1 == country)
                .Select(rel => rel.ReleaseDates.First().Certification)
                .FirstOrDefault();
        }
        private string? GetCertification(TvShow downloadedShow, string country)
        {
            return downloadedShow.ContentRatings?.Results
                .Where(rel => rel.Iso_3166_1 == country)
                .Select(rel => rel.Rating)
                .FirstOrDefault();
        }

        public override void Search(string text, bool showErrorMsgBox, MediaConfiguration.MediaType type, string languageCode)
        {
            if (type ==MediaConfiguration.MediaType.movie)
            {
                SearchContainer<SearchMovie> results = Client.SearchMovieAsync(text,languageCode).Result;
                LOGGER.Info(
                    $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {text}");

                foreach (SearchMovie result in results.Results)
                {
                    LOGGER.Info($"   Movie: {result.Title}:{result.Id}   {result.Popularity}");
                    File(result);
                    try
                    {
                        DownloadMovieNow(result.Id,languageCode, showErrorMsgBox);
                    }
                    catch (MediaNotFoundException sex)
                    {
                        LOGGER.Warn($"Could not get full details of {result.Id} while searching for '{text}'");
                    }
                }
            }
            else
            {
                SearchContainer<SearchTv>? results = Client.SearchTvShowAsync(text).Result;
                LOGGER.Info(
                    $"Got {results.Results.Count:N0} of {results.TotalResults:N0} results searching for {text}");

                foreach (SearchTv result in results.Results)
                {
                    LOGGER.Info($"   TV Show: {result.Name}:{result.Id}   {result.Popularity}");
                    File(result);
                    try
                    {
                        SeriesSpecifier ss = new SeriesSpecifier(-1, -1, result.Id, true, languageCode, result.Name,
                            TVDoc.ProviderType.TMDB, null, MediaConfiguration.MediaType.tv);
                        DownloadSeriesNow(ss, showErrorMsgBox);
                    }
                    catch (MediaNotFoundException sex)
                    {
                        LOGGER.Warn($"Could not get full details of {result.Id} while searching for '{text}'");
                    }
                }
            }
        }

        private CachedSeriesInfo File(SearchTv result)
        {
            CachedSeriesInfo m = new CachedSeriesInfo
            {
                TmdbCode = result.Id,
                Name = result.Name,
                FirstAired = result.FirstAirDate,
                Overview = result.Overview,
                //Status = result.Status,
                ShowLanguage = result.OriginalLanguage,
                SiteRating = (float)result.VoteAverage,
                SiteRatingVotes = result.VoteCount,
                PosterUrl = PosterImageUrl(result.PosterPath),
                Popularity = result.Popularity,
                IsSearchResultOnly = true,
                Dirty = false,
                SrvLastUpdated = DateTime.UtcNow.Date.ToUnixTime(),
                //FanartUrl = OriginalImageUrl(result.BackdropPath),  //TODO **** on Website
            };

            File(m);
            return m;
        }

        private CachedMovieInfo File(SearchMovie result)
        {
            CachedMovieInfo m = new CachedMovieInfo
            {
                TmdbCode = result.Id,
                Name = result.Title,
                FirstAired = result.ReleaseDate,
                Overview = result.Overview,
                ShowLanguage = result.OriginalLanguage,
                SiteRating = (float)result.VoteAverage,
                SiteRatingVotes = result.VoteCount,
                PosterUrl = PosterImageUrl(result.PosterPath),
                Popularity = result.Popularity,
                FanartUrl = OriginalImageUrl(result.BackdropPath),
                IsSearchResultOnly = true,
                Dirty = false,
            };

            File(m);
            return m;
        }
        private static string? ImageUrl(string source) => ImageUrl(source, "w600_and_h900_bestv2");
        private static string? PosterImageUrl(string source) => ImageUrl(source, "w600_and_h900_bestv2");
        private static string? OriginalImageUrl(string source) => ImageUrl(source, "original");

        private static string? ImageUrl(string source,string type)
        {
            if (source.HasValue())
            {
                return "https://image.tmdb.org/t/p/"+type + source;
            }

            return null;
        }

        public CachedMovieInfo? LookupMovieByImdb(string imdbToTest, string languageCode, bool showErrorMsgBox)
        {
            FindContainer? results = Client.FindAsync(FindExternalSource.Imdb, imdbToTest).Result;
            LOGGER.Info($"Got {results.MovieResults.Count:N0} results searching for {imdbToTest}");
            foreach (SearchMovie result in results.MovieResults)
            {
                DownloadMovieNow(result.Id, languageCode, showErrorMsgBox); 
            }

            if (results.MovieResults.Count == 0)
            {
                return null;
            }

            if (results.MovieResults.Count == 1)
            {
                lock (MOVIE_LOCK)
                {
                    return Movies[results.MovieResults.First().Id];
                }
            }

            return null;
        }


        public int? LookupTvbdIdByImdb(string imdbToTest, bool showErrorMsgBox)
        {
            FindContainer? results = Client.FindAsync(FindExternalSource.Imdb, imdbToTest).Result;
            LOGGER.Info($"Got {results.TvResults.Count:N0} results searching for {imdbToTest}");


            if (results.TvResults.Count == 0)
            {
                return null;
            }

            if (results.TvResults.Count == 1)
            {
                return results.TvResults.First().Id;
            }

            return null;
        }

        private void File(CachedMovieInfo cachedMovie)
        {
            lock (MOVIE_LOCK)
            {
                if (Movies.ContainsKey(cachedMovie.TmdbCode))
                {
                    Movies[cachedMovie.TmdbCode].Merge(cachedMovie);
                }
                else
                {
                    Movies[cachedMovie.TmdbCode] = cachedMovie;
                }
            }
        }

        private void File(CachedSeriesInfo s)
        {
            lock (SERIES_LOCK)
            {
                if (Series.ContainsKey(s.TmdbCode))
                {
                    Series[s.TmdbCode].Merge(s);
                }
                else
                {
                    Series[s.TmdbCode] = s;
                }
            }
        }

        public Dictionary<int, CachedMovieInfo> GetMovieIdsFromCollection(int collectionId,string languageCode)
        {
            Dictionary<int, CachedMovieInfo> returnValue = new Dictionary<int, CachedMovieInfo>();
            TMDbLib.Objects.Collections.Collection collection = Client.GetCollectionAsync(collectionId, languageCode,languageCode).Result;
            if (collection == null)
            {
                return returnValue;
            }

            foreach (SearchMovie? m in collection.Parts)
            {
                int id = m.Id;
                CachedMovieInfo info = File(m);
                returnValue.Add(id, info);
            }

            return returnValue;
        }

        public CachedMovieInfo? LookupMovieByTvdb(int tvdbId, bool showErrorMsgBox)
        {
            throw new NotImplementedException(); //TODO
        }

        public IEnumerable<CachedMovieInfo> ServerAccuracyCheck()
        {
            Say("TMDB Accuracy Check running");
            TmdbAccuracyCheck check = new TmdbAccuracyCheck(this);
            lock (MOVIE_LOCK)
            {
                foreach (CachedMovieInfo si in Movies.Values.Where(info => !info.IsSearchResultOnly).OrderBy(s => s.Name).ToList())
                {
                    check.ServerAccuracyCheck(si);
                }
            }

            foreach (string issue in check.Issues)
            {
                LOGGER.Warn(issue);
            }

            SayNothing();
            return check.ShowsToUpdate;
        }

        public async Task<Recomendations> GetRecommendations(TVDoc mDoc, BackgroundWorker sender, List<ShowConfiguration> shows, string languageCode)
        {
            int total = shows.Count;
            int current = 0;
            Task<SearchContainer<SearchTv>> topRated = Client.GetTvShowTopRatedAsync(language:languageCode);
            Task<SearchContainer<SearchTv>> trending = Client.GetTrendingTvAsync(TimeWindow.Week);
            await topRated;
            await trending;

            Recomendations returnValue = new Recomendations();

            foreach (SearchTv? top in topRated.Result.Results)
            {
                File(top);
                returnValue.AddTopRated(top.Id);
            }
            foreach (SearchTv? top in trending.Result.Results)
            {
                File(top);
                returnValue.AddTrending(top.Id);
            }


            foreach (ShowConfiguration? arg in shows)
            {
                try
                {
                    AddRecommendationsFrom(arg, returnValue,languageCode);

                    sender.ReportProgress(100 * current++ / total, arg.CachedShow?.Name);
                }
                catch
                {
                    //todo record and resolve /retry errors
                }
            }

            return returnValue;
        }

        private void AddRecommendationsFrom(ShowConfiguration arg, Recomendations returnValue,string languageCode)
        {
            if (arg.TmdbCode == 0)
            {
                string? imdb = arg.CachedShow?.Imdb;
                if (!imdb.HasValue())
                {
                    return;
                }

                int? tmdbcode = LookupTvbdIdByImdb(imdb!, false);
                if (!tmdbcode.HasValue)
                {
                    return;
                }

                arg.TmdbCode = tmdbcode.Value;
            }

            Task<SearchContainer<SearchTv>>? related = Client.GetTvShowRecommendationsAsync(arg.TmdbCode, languageCode);
            Task<SearchContainer<SearchTv>>? similar = Client.GetTvShowSimilarAsync(arg.TmdbCode, languageCode);

            Task.WaitAll(related, similar);
            if (related.Result != null)
            {
                foreach (SearchTv? s in related.Result.Results)
                {
                    File(s);
                    returnValue.AddRelated(s.Id, arg);
                }
            }

            if (similar.Result != null)
            {
                foreach (SearchTv? s in similar.Result.Results)
                {
                    File(s);
                    returnValue.AddSimilar(s.Id, arg);
                }
            }
        }

        public async Task<Recomendations> GetRecommendations(TVDoc mDoc, BackgroundWorker sender, List<MovieConfiguration> movies, string languageCode)
        {
            int total = movies.Count;
            int current = 0;
            Task<SearchContainer<SearchMovie>> topRated = Client.GetMovieTopRatedListAsync(languageCode);
            Task<SearchContainer<SearchMovie>> trending = Client.GetTrendingMoviesAsync(TimeWindow.Week);
            await topRated;
            await trending;

            Recomendations returnValue = new Recomendations();

            foreach (SearchMovie? top in topRated.Result.Results)
            {
                File(top);
                returnValue.AddTopRated(top.Id);
            }
            foreach (SearchMovie? top in trending.Result.Results)
            {
                File(top);
                returnValue.AddTrending(top.Id);
            }


            foreach (MovieConfiguration? arg in movies)
            {
                try
                {
                    Task<SearchContainer<SearchMovie>>? related = Client.GetMovieRecommendationsAsync(arg.TmdbCode, languageCode);
                    Task<SearchContainer<SearchMovie>>? similar = Client.GetMovieSimilarAsync(arg.TmdbCode, languageCode);

                    Task.WaitAll(related, similar);
                    foreach (SearchMovie? movie in related.Result.Results)
                    {
                        File(movie);
                        returnValue.AddRelated(movie.Id, arg);
                    }


                    foreach (SearchMovie? movie in similar.Result.Results)
                    {
                        File(movie);
                        returnValue.AddSimilar(movie.Id, arg);

                    }

                    sender.ReportProgress(100 * current++ / total, arg.CachedMovie?.Name);
                }
                catch
                {
                    //todo - record error, retry etc
                }
            }

            //var related = movies.Select(arg => (arg.TmdbCode,Client.GetMovieRecommendationsAsync(arg.TmdbCode))).ToList();
            //var similar = movies.Select(arg => (arg.TmdbCode,Client.GetMovieSimilarAsync(arg.TmdbCode))).ToList();

            //Task.WaitAll(related.Select(tuple => tuple.Item2).ToArray());
            //Task.WaitAll(similar.Select(tuple => tuple.Item2).ToArray());
            return returnValue;
        }

        public string LanguageCode(string languageName)
        {
            return LANGUAGES.Where(x => x.Name == languageName).Select(x => x.Code).FirstOrDefault() ?? "en-US";
        }

        public string RegionCode(string regionName)
        {
            return COUNTRIES.Where(x => x.Name == regionName).Select(x => x.Code).FirstOrDefault() ?? "US";
        }

        public string? LanguageName(string languageCode)
        {
            return LANGUAGES.Where(x => x.Code == languageCode).Select(x => x.Name).FirstOrDefault();
        }

        public string? RegionName(string regionCode)
        {
            return COUNTRIES.Where(x => x.Code == regionCode).Select(x => x.Name).FirstOrDefault();
        }
    }
}
