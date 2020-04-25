using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace TVRename
{
    /// <summary>
    /// Handles a thread-safe implementation of the 'library' this will hold all the ShowItem configuration as well
    /// many methods that provide summaries of the data in the library
    /// </summary>
    public class ShowLibrary : ConcurrentDictionary<int, ShowItem>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        [NotNull]
        public IEnumerable<ShowItem> Shows => Values;

        [NotNull]
        public ICollection<SeriesSpecifier> SeriesSpecifiers
        {
            get
            {
                List<SeriesSpecifier> value = new List<SeriesSpecifier>();
                foreach (KeyValuePair<int, ShowItem> series in this)
                {
                    value.Add(new SeriesSpecifier(series.Value.TvdbCode, series.Value.TVmazeCode, series.Value.UseCustomLanguage, series.Value.CustomLanguageCode, series.Value.ShowName,series.Value.Provider,series.Value.TheSeries()?.Imdb));
                }

                return value;
            }
        }

        [NotNull]
        public IEnumerable<string> ShowStatuses =>Shows.Select(item => item.ShowStatus).Distinct().OrderBy(s => s);

        [NotNull]
        public IEnumerable<string> SeasonWords()
        {
            //See https://github.com/TV-Rename/tvrename/issues/241 for background
            List<string> results = TVSettings.Instance.searchSeasonWordsArray.ToList();

            if (!TVSettings.Instance.ForceBulkAddToUseSettingsOnly)
            {
                IEnumerable<string> seasonWordsFromShows =
                    from si in Values select CustomSeasonName.GetTextFromPattern(si.AutoAddCustomFolderFormat);

                results.AddRange(seasonWordsFromShows.Distinct());

                results.Add(TVSettings.Instance.defaultSeasonWord);
            }

            return results.Where(t => !string.IsNullOrWhiteSpace(t)).Select(s =>s.Trim() ).Distinct();
        }

        [NotNull]
        public IEnumerable<string> GetSeasonPatterns()
        {
            List<string> results = new List<string> {TVSettings.Instance.SeasonFolderFormat};

            IEnumerable<string> seasonWordsFromShows = Values.Select(si => si.AutoAddCustomFolderFormat);

            results.AddRange(seasonWordsFromShows.Distinct().ToList());

            return results;
        }

        [NotNull]
        public IEnumerable<string> GetGenres()
        {
            List<string> allGenres = new List<string>();
            foreach (ShowItem si in Values)
            {
                allGenres.AddRange(si.Genres);
            }

            List<string> distinctGenres = allGenres.Distinct().ToList();
            distinctGenres.Sort();
            return distinctGenres;
        }

        [NotNull]
        public IEnumerable<string> GetStatuses()
        {
            return Values
                .Where(s => !string.IsNullOrWhiteSpace(s?.ShowStatus))
                .Select(s => s.ShowStatus)
                .Distinct()
                .OrderBy(s => s);
        }

        [NotNull]
        public IEnumerable<string> GetTypes()
        {
            return Values
                .Select(s => s.TheSeries()?.Type)
                .Distinct()
                .Where(s => s.HasValue())
                .OrderBy(s => s);
        }

        [NotNull]
        public IEnumerable<string> GetNetworks()
        {
            return Values
                .Select(si => si.TheSeries())
                .Where(seriesInfo => !string.IsNullOrWhiteSpace(seriesInfo?.Network))
                .Select(seriesInfo => seriesInfo.Network)
                .Distinct()
                .OrderBy(s=>s);
        }

        [NotNull]
        public IEnumerable<string> GetContentRatings()
        {
            return Values.Select(si => si.TheSeries())
                .Where(s => !string.IsNullOrWhiteSpace(s?.ContentRating))
                .Select(s => s.ContentRating)
                .Distinct()
                .OrderBy(s=>s);
        }

        [NotNull]
        public List<ShowItem> GetSortedShowItems()
        {
            List<ShowItem> returnList = Shows.ToList();
            returnList.Sort(ShowItem.CompareShowItemNames);
            return returnList;
        }

        [CanBeNull]
        public ShowItem GetShowItem(int id) => ContainsKey(id) ? this[id] : null;

        public void GenDict()
        {
            foreach (ShowItem show in Values)
            {
                GenerateEpisodeDict(show);
            }
        }

        public static bool GenerateEpisodeDict([NotNull] ShowItem si)
        {
            // copy data from tvdb
            // process as per rules
            // done!

            bool r = true;

            lock (si.Provider==ShowItem.ProviderType.TVmaze? TVmaze.LocalCache.SERIES_LOCK: TheTVDB.LocalCache.SERIES_LOCK)
            {
                si.ClearEpisodes();

                SeriesInfo ser = si.Provider == ShowItem.ProviderType.TVmaze
                    ? TVmaze.LocalCache.Instance.GetSeries(si.TVmazeCode)
                    : TheTVDB.LocalCache.Instance.GetSeries(si.TvdbCode);

                if (ser is null)
                {
                    string source = si.Provider == ShowItem.ProviderType.TVmaze ? "TVMaze" : "TVDB";
                    Logger.Warn($"Asked to generate episodes for {si.ShowName}, but this has not yet been downloaded from {source}");
                    return false;
                }

                foreach (Episode ep in ser.Episodes)
                {
                    si.AddEpisode(ep);
                }

                foreach (int snum in si.AppropriateSeasons().Keys.ToList())
                {
                    List<ProcessedEpisode> pel = GenerateEpisodes(si, snum, true);
                    si.SeasonEpisodes[snum] = pel;
                    if (pel is null)
                    {
                        r = false;
                    }
                }

                AddOverallCount(si);
            }

            return r;
        }
        
        private static void AddOverallCount([NotNull] ShowItem si)
        {
            // now, go through and number them all sequentially
            List<int> theKeys = si.AppropriateSeasons().Keys.ToList();
            theKeys.Sort();

            int overallCount = 1;
            foreach (int snum in theKeys)
            {
                if (snum == 0)
                {
                    continue;
                }

                foreach (ProcessedEpisode pe in si.SeasonEpisodes[snum])
                {
                    pe.OverallNumber = overallCount;
                    overallCount += 1 + pe.EpNum2 - pe.AppropriateEpNum;
                }
            }
        }

        [CanBeNull]
        public static List<ProcessedEpisode> GenerateEpisodes([NotNull] ShowItem si, int snum, bool applyRules)
        {
            if (!si.AppropriateSeasons().ContainsKey(snum))
            {
                Logger.Error($"Asked to update season {snum} of {si.ShowName}, but it does not exist");
                return null;
            }

            ProcessedSeason seas = si.AppropriateSeasons()[snum];

            if (seas is null)
            {
                Logger.Error($"Asked to update season {snum} of {si.ShowName}, whilst it exists, it has no contents");
                return null; 
            }

            List<ProcessedEpisode> eis = seas.Episodes.Values.Select(e => new ProcessedEpisode(e, si)).ToList();

            switch (si.Order)
            {
                case ProcessedSeason.SeasonType.dvd:
                    eis.Sort(ProcessedEpisode.DVDOrderSorter);
                    AutoMerge(eis, si);
                    Renumber(eis);
                    break;

                case ProcessedSeason.SeasonType.aired:
                    eis.Sort(ProcessedEpisode.EpNumberSorter);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (si.CountSpecials && si.AppropriateSeasons().ContainsKey(0) && !TVSettings.Instance.IgnoreAllSpecials)
            {
                // merge specials in
                MergeSpecialsIn(si, snum, eis);
            }

            if (applyRules)
            {
                List<ShowRule> rules = si.RulesForSeason(snum);
                if (rules != null)
                {
                    ApplyRules(eis, rules, si);
                }
            }

            return eis;
        }

        private static void AutoMerge([NotNull] List<ProcessedEpisode> eis,ShowItem si)
        {
            for (int i = 1; i < eis.Count; i++)
            {
                if (eis[i - 1].DvdEpNum == eis[i].DvdEpNum && eis[i].DvdEpNum > 0)
                {
                    //We have a candidate to merge
                    MergeEpisodes(eis,si,RuleAction.kMerge,i-1,i,null);
                }
            }
        }

        private static void MergeSpecialsIn([NotNull] ShowItem si, int snum,  [NotNull] List<ProcessedEpisode> eis)
        {
            foreach (Episode ep in si.AppropriateSeasons()[0].Episodes.Values)
            {
                MergeEpisode(si, snum, ep, eis);
            }

            // renumber to allow for specials
            int epnumr = 1;
            foreach (ProcessedEpisode t in eis)
            {
                t.SetEpisodeNumbers(epnumr, epnumr + t.EpNum2 - t.AppropriateEpNum);
                epnumr++;
            }
        }

        private static void MergeEpisode(ShowItem si, int snum, [NotNull] Episode ep, IList<ProcessedEpisode> eis)
        {
            if (!ep.AirsBeforeSeason.HasValue)
            {
                return;
            }

            if (!ep.AirsBeforeEpisode.HasValue)
            {
                return;
            }

            int sease = ep.AirsBeforeSeason.Value;
            if (sease != snum)
            {
                return;
            }

            int epnum = ep.AirsBeforeEpisode.Value;
            for (int i = 0; i < eis.Count; i++)
            {
                if (eis[i].AppropriateSeasonNumber == sease && eis[i].AppropriateEpNum == epnum)
                {
                    ProcessedEpisode pe = new ProcessedEpisode(ep, si)
                    {
                        TheAiredProcessedSeason = eis[i].TheAiredProcessedSeason,
                        TheDvdProcessedSeason = eis[i].TheDvdProcessedSeason,
                        SeasonId = eis[i].SeasonId
                    };

                    eis.Insert(i, pe);
                    break;
                }
            }
        }

        internal void Add([NotNull] ShowItem found)
        {
            if (found.TvdbCode == -1)
            {
                return;
            }

            if (TryAdd(found.TvdbCode, found))
            {
                return;
            }

            if (ContainsKey(found.TvdbCode))
            {
                Logger.Warn($"Failed to Add {found.ShowName} with TVDBId={found.TvdbCode} to library, but it's already present");
            }
            else
            {
                Logger.Error($"Failed to Add {found.ShowName} with TVDBId={found.TvdbCode} to library");
            }
        }

        public static void ApplyRules([NotNull] List<ProcessedEpisode> eis, [NotNull] IEnumerable<ShowRule> rules, ShowItem show)
        {
            foreach (ShowRule sr in rules)
            {
                try
                {
                    // turn nn1 and nn2 from ep number into position in array
                    int n1 = FindIndex(eis, sr.First);
                    int n2 = FindIndex(eis, sr.Second);

                    switch (sr.DoWhatNow)
                    {
                        case RuleAction.kRename:
                            RenameEpisode(eis, n1, sr.UserSuppliedText);
                            break;

                        case RuleAction.kRemove:
                            RemoveEpisode(eis, n1, n2);
                            break;

                        case RuleAction.kIgnoreEp:
                            IgnoreEpisodes(eis, n1, n2);
                            break;

                        case RuleAction.kSplit:
                            SplitEpisode(eis, show, sr.Second, n1);
                            break;

                        case RuleAction.kMerge:
                        case RuleAction.kCollapse:
                            MergeEpisodes(eis, show, sr.DoWhatNow, n1, n2, sr.UserSuppliedText);
                            break;

                        case RuleAction.kSwap:
                            SwapEpisode(eis, n1, n2);
                            break;

                        case RuleAction.kInsert:
                        {
                            // this only applies for inserting an episode, at the end of the list
                            if (sr.First == eis[eis.Count - 1].AppropriateEpNum + 1) // after the last episode
                            {
                                n1 = eis.Count;
                            }

                            InsertEpisode(eis, show, n1, sr.UserSuppliedText);
                            break;
                        }
                    }

                    Renumber(eis);
                }
                catch (Exception e)
                {
                    Logger.Warn(
                        $"Please review rules for {show.ShowName} season {eis.FirstOrDefault()?.AppropriateSeasonNumber}");
                    Logger.Warn(e,$"Could not process rule for {show.ShowName}, {sr.DoWhatNow}:{sr.First}:{sr.Second}:{sr.UserSuppliedText}");
                }
            } // for each rule

            RemoveIgnoredEpisodes(eis);
        }

        private static void RemoveIgnoredEpisodes([NotNull] IList<ProcessedEpisode> eis)
        {
            // now, go through and remove the ignored ones (but don't renumber!!)
            for (int i = eis.Count - 1; i >= 0; i--)
            {
                if (eis[i].Ignore)
                {
                    eis.RemoveAt(i);
                }
            }
        }

        private static int FindIndex([NotNull] IReadOnlyList<ProcessedEpisode> eis, int episodeNumber)
        {
            for (int i = 0; i < eis.Count; i++)
            {
                if (eis[i].AppropriateEpNum == episodeNumber)
                {
                    return i;
                }
            }
            return -1;
        }

        private static void IgnoreEpisodes([NotNull] IReadOnlyList<ProcessedEpisode> eis, int fromIndex, int toIndex)
        {
            int ec = eis.Count;

            if (toIndex == -1)
            {
                if (ValidIndex( toIndex , ec))
                {
                    eis[toIndex].Ignore = true;
                }
            }
            else
            {
                for (int i = fromIndex; i <= toIndex; i++)
                {
                    if (ValidIndex(i, ec))
                    {
                        eis[i].Ignore = true;
                    }
                }
            }
        }

        private static void RemoveEpisode([NotNull] List<ProcessedEpisode> eis, int fromIndex, int toIndex)
        {
            int ec = eis.Count;
            if (ValidIndex(fromIndex, ec) && ValidIndex(toIndex, ec))
            {
                eis.RemoveRange(fromIndex, 1 + toIndex - fromIndex);
            }
            else if (ValidIndex(fromIndex, ec) && toIndex == -1)
            {
                eis.RemoveAt(fromIndex);
            }
            else
            {
                //arguments are not consistent, so we'll do nothing
            }
            Renumber(eis);
        }

        private static bool ValidIndex(int index, int maxIndex) => index < maxIndex && index >= 0;

        private static void RenameEpisode([NotNull] IReadOnlyList<ProcessedEpisode> eis, int index,string txt)
        {
            int ec = eis.Count;
            if (ValidIndex(index, ec))
            {
                eis[index].Name = txt;
            }
        }

        private static void SplitEpisode([NotNull] IList<ProcessedEpisode> eis, ShowItem si, int numberOfNewParts, int index)
        {
            int ec = eis.Count;
            // split one episode into a multi-parter
            if (ValidIndex(index, ec))
            {
                ProcessedEpisode ei = eis[index];
                string nameBase = ei.Name;
                eis.RemoveAt(index); // remove old one

                foreach (int i in Enumerable.Range(1,numberOfNewParts))
                // make numberOfNewParts new parts
                {
                    ProcessedEpisode pe2 =
                        new ProcessedEpisode(ei, si, ProcessedEpisode.ProcessedEpisodeType.split)
                        {
                            Name = $"{nameBase} (Part {i})",
                            AiredEpNum = -2,
                            DvdEpNum = -2,
                            EpNum2 = -2
                        };

                    eis.Insert(index + i-1, pe2);
                }
            }
        }

        private static void MergeEpisodes([NotNull] List<ProcessedEpisode> eis, ShowItem si, RuleAction action, int fromIndex, int toIndex, [CanBeNull] string newName)
        {
            int ec = eis.Count;
            if (ValidIndex(fromIndex, ec) && ValidIndex(toIndex, ec) && fromIndex < toIndex)
            {
                ProcessedEpisode oldFirstEi = eis[fromIndex];
                List<string> episodeNames = new List<string> { eis[fromIndex].Name };
                string defaultCombinedName = eis[fromIndex].Name + " + ";
                string combinedSummary = eis[fromIndex].Overview + "<br/><br/>";
                List<Episode> alleps = new List<Episode> {eis[fromIndex]};
                for (int i = fromIndex + 1; i <= toIndex; i++)
                {
                    episodeNames.Add(eis[i].Name);
                    defaultCombinedName += eis[i].Name;
                    combinedSummary += eis[i].Overview;
                    alleps.Add(eis[i]);
                    if (i != toIndex)
                    {
                        defaultCombinedName += " + ";
                        combinedSummary += "<br/><br/>";
                    }
                }

                eis.RemoveRange(fromIndex, toIndex - fromIndex);

                eis.RemoveAt(fromIndex);

                string combinedName = GetBestNameFor(episodeNames, defaultCombinedName);

                ProcessedEpisode pe2 = new ProcessedEpisode(oldFirstEi, si, alleps)
                {
                    Name = string.IsNullOrEmpty(newName) ? combinedName : newName,
                    AiredEpNum = oldFirstEi.AiredEpNum,
                    DvdEpNum = oldFirstEi.DvdEpNum,
                    EpNum2 = action == RuleAction.kMerge ? alleps.Max(episode => episode.GetEpisodeNumber(si.Order)) : oldFirstEi.AppropriateEpNum,
                    Overview = combinedSummary
                };

                eis.Insert(fromIndex, pe2);
            }
        }

        private static void InsertEpisode([NotNull] IList<ProcessedEpisode> eis, ShowItem si, int index, string txt)
        {
            int ec = eis.Count;

            if (ValidIndex(index, ec))
            {
                ProcessedEpisode t = eis[index];
                ProcessedEpisode n = new ProcessedEpisode(t, si, txt, t.AiredEpNum + 1, t.DvdEpNum + 1, t.EpNum2 + 1);
                eis.Insert(index, n);
            }
            else if (index == ec)
            {
                ProcessedEpisode t = eis[index - 1];
                ProcessedEpisode n = new ProcessedEpisode(t, si,txt,t.AiredEpNum+1,t.DvdEpNum+1,t.EpNum2+1);
                eis.Add(n);
            }
            else
            {
                //Parameters are invalid, so we'll do nothing
            }
        }

        private static void SwapEpisode([NotNull] List<ProcessedEpisode> eis, int n1, int n2)
        {
            int ec = eis.Count;
            if (ValidIndex(n1, ec) && ValidIndex(n2, ec))
            {
                ProcessedEpisode t = eis[n1];
                eis[n1] = eis[n2];
                eis[n2] = t;
            }
        }

        public static string GetBestNameFor([NotNull] List<string> episodeNames, string defaultName)
        {
            string root = StringExtensions.GetCommonStartString(episodeNames);
            int shortestEpisodeName = episodeNames.Min(x => x.Length);
            int longestEpisodeName = episodeNames.Max(x => x.Length);
            bool namesSameLength = shortestEpisodeName == longestEpisodeName;
            bool rootIsIgnored = root.Trim().StartsWith("Episode", StringComparison.OrdinalIgnoreCase) ||
                                 root.Trim().StartsWith("Part", StringComparison.OrdinalIgnoreCase);

            if (!namesSameLength || rootIsIgnored || root.Length <= 3 || root.Length <= shortestEpisodeName / 2)
            {
                return defaultName;
            }

            char[] charsToTrim = {',', '.', ';', ':', '-', '('};
            string[] wordsToTrim = {"part", "episode","pt","chapter"};

            return root.Trim().TrimEnd(wordsToTrim).Trim().TrimEnd(charsToTrim).Trim();
        }

        private static void Renumber([NotNull] IReadOnlyList<ProcessedEpisode> eis)
        {
            if (eis.Count == 0)
            {
                return; // nothing to do
            }

            // renumber 
            // pay attention to specials etc.
            int n = eis[0].AppropriateEpNum == 0 ? 0 : 1;

            foreach (ProcessedEpisode t in eis)
            {
                if (t.AppropriateEpNum == -1)
                {
                    continue;
                }

                int num = t.EpNum2 - t.AppropriateEpNum;
                if((t.AppropriateEpNum!= n || t.EpNum2!=n+num) && !(t.Show.Order==ProcessedSeason.SeasonType.dvd && t.NotOnDvd()))
                {
                    t.SetEpisodeNumbers(n, n + num);
                }
                n += num + 1;
            }
        }

        [NotNull]
        internal IEnumerable<ShowItem> GetRecentShows() => GetSortedShowItems().Where(IsRecent);

        private static bool IsRecent([NotNull] ShowItem si)
        {
            // only scan "recent" shows
            int days = TVSettings.Instance.WTWRecentDays;
            return si.ActiveSeasons.Clone().Select(pair => pair.Value).SelectMany(eis => eis).Any(ei => ei.WithinDays(days));
        }

        [NotNull]
        public List<ProcessedEpisode> NextNShows(int nShows, int nDaysPast, int nDaysFuture)
        {
            DateTime notBefore = DateTime.Now.AddDays(-nDaysPast);
            List<ProcessedEpisode> found = new List<ProcessedEpisode>();
            
            for (int i = 0; i < nShows; i++)
            {
                ProcessedEpisode nextAfterThat = GetNextMostRecentProcessedEpisode(nDaysFuture, found, notBefore);

                if (nextAfterThat is null)
                {
                    return found;
                }

                DateTime? nextdt = nextAfterThat.GetAirDateDt(true);
                if (nextdt.HasValue)
                {
                    notBefore = nextdt.Value;
                    found.Add(nextAfterThat);
                }
            }

            return found;
        }

        [CanBeNull]
        private ProcessedEpisode GetNextMostRecentProcessedEpisode(int nDaysFuture, ICollection<ProcessedEpisode> found, DateTime notBefore)
        {
            ProcessedEpisode nextAfterThat = null;
            TimeSpan howClose = TimeSpan.MaxValue;
            foreach (ShowItem si in GetSortedShowItems())
            {
                lock (si.Provider==ShowItem.ProviderType.TVmaze ? TVmaze.LocalCache.SERIES_LOCK : TheTVDB.LocalCache.SERIES_LOCK)
                {
                    if (!si.ShowNextAirdate)
                    {
                        continue;
                    }

                    foreach (KeyValuePair<int, List<ProcessedEpisode>> v in si.ActiveSeasons.ToList())
                    {
                        if (si.IgnoreSeasons.Contains(v.Key))
                        {
                            continue; // ignore this season
                        }

                        if (v.Key == 0 && TVSettings.Instance.IgnoreAllSpecials)
                        {
                            continue;
                        }

                        if (v.Value is null)
                        {
                            continue;
                        }

                        foreach (ProcessedEpisode ei in v.Value)
                        {
                            if (found.Contains(ei))
                            {
                                continue;
                            }

                            DateTime? airdt = ei.GetAirDateDt(true);

                            if (airdt is null || airdt == DateTime.MaxValue)
                            {
                                continue;
                            }

                            DateTime dt = airdt.Value;

                            TimeSpan timeUntil = dt.Subtract(DateTime.Now);
                            if (timeUntil.TotalDays > nDaysFuture)
                            {
                                continue; //episode is too far in the future
                            }

                            TimeSpan ts = dt.Subtract(notBefore);
                            if (ts.TotalSeconds < 0)
                            {
                                continue; //episode is too far in the past
                            }

                            //if we have a closer match
                            if (TimeSpan.Compare(ts, howClose) < 0)
                            {
                                howClose = ts;
                                nextAfterThat = ei;
                            }
                        }
                    }
                }
            }

            return nextAfterThat;
        }

        public void AddRange([NotNull] IEnumerable<ShowItem> addedShows)
        {
            foreach (ShowItem show in addedShows)
            {
                Add(show);
            }
        }

        internal void Remove([NotNull] ShowItem si)
        {
            if (!TryRemove(si.TvdbCode, out _))
            {
                Logger.Error($"Failed to remove {si.ShowName} from the library with TVDBId={si.TvdbCode}");
            }
        }

        [NotNull]
        public IEnumerable<ProcessedEpisode> GetRecentAndFutureEps(int days)
        {
            List<ProcessedEpisode> returnList = new List<ProcessedEpisode>();

            foreach (ShowItem si in Values)
            {
                if (!si.ShowNextAirdate)
                {
                    continue;
                }

                foreach (List<ProcessedEpisode> eis in si.ActiveSeasons.Select(p => p.Value))
                {
                    bool nextToAirFound = false;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        DateTime? dt = ei.GetAirDateDt(true);
                        if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
                        {
                            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
                            if (ts.TotalHours >= -24 * days) // in the future (or fairly recent)
                            {
                                if (ts.TotalHours >= 0 && !nextToAirFound)
                                {
                                    nextToAirFound = true;
                                    ei.NextToAir = true;
                                }
                                else
                                {
                                    ei.NextToAir = false;
                                }

                                returnList.Add(ei);
                            }
                        }
                    }
                }
            }

            return returnList;
        }

        public void LoadFromXml([NotNull] XElement xmlSettings)
        {
            foreach (ShowItem si in xmlSettings.Descendants("ShowItem").Select(showSettings => new ShowItem(showSettings)))
            {
                if (si.UseCustomShowName) // see if custom show name is actually the real show name
                {
                    SeriesInfo ser = si.TheSeries();
                    if (ser != null && si.CustomShowName == ser.Name)
                    {
                        // then, turn it off
                        si.CustomShowName = string.Empty;
                        si.UseCustomShowName = false;
                    }
                }

                Add(si);
            }
        }

        [NotNull]
        public IEnumerable<ProcessedEpisode> RecentEpisodes(int days)
        {
            List<ProcessedEpisode> episodes = new List<ProcessedEpisode>();

            // for each show, see if any episodes were aired in "recent" days...
            foreach (ShowItem si in GetRecentShows())
            {
                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.ActiveSeasons)
                {
                    foreach (ProcessedEpisode ei in kvp.Value)
                    {
                        if (ei.WithinDays(days))
                        {
                            episodes.Add(ei);
                        }
                    }
                }
            }

            return episodes;
        }

        public void ReIndex()
        {
            List<int> toReIndex = this.Where(x => x.Key != x.Value.TvdbCode).Select(x=>x.Key).ToList();

            foreach (int x in toReIndex)
            {
                TryRemove(x,out ShowItem si);
                Add(si);
            }
        }
    }
}
