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
                    value.Add(new SeriesSpecifier(series.Key,series.Value.UseCustomLanguage,series.Value.CustomLanguageCode,series.Value.ShowName));
                }

                return value;
            }
        }

        [NotNull]
        private IEnumerable<string> GetSeasonWords()
        {
            //See https://github.com/TV-Rename/tvrename/issues/241 for background
            List<string> results = TVSettings.Instance.searchSeasonWordsArray.ToList();

            if (!TVSettings.Instance.ForceBulkAddToUseSettingsOnly)
            {
                IEnumerable<string> seasonWordsFromShows =
                    from si in Values select CustomSeasonName.GetTextFromPattern(si.AutoAddCustomFolderFormat);

                results = seasonWordsFromShows.Distinct().ToList();

                results.Add(TVSettings.Instance.defaultSeasonWord);
            }

            return results.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct();
        }

        [NotNull]
        public IEnumerable<string> GetSeasonPatterns()
        {
            List<string> results = new List<string> {TVSettings.Instance.SeasonFolderFormat};

            IEnumerable<string> seasonWordsFromShows = Values.Select(si => si.AutoAddCustomFolderFormat);

            results.AddRange(seasonWordsFromShows.Distinct().ToList());

            return results;
        }

        private IEnumerable<string> seasonWordsCache;

        [NotNull]
        internal IEnumerable<string> SeasonWords()
        {
            return seasonWordsCache ?? (seasonWordsCache = GetSeasonWords());
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
        public List<ShowItem> GetShowItems()
        {
            List<ShowItem> returnList = Values.ToList();
            returnList.Sort(TVRename.ShowItem.CompareShowItemNames);
            return returnList;
        }

        [CanBeNull]
        public ShowItem ShowItem(int id) => ContainsKey(id) ? this[id] : null;

        public bool GenDict() => Values.All(GenerateEpisodeDict);

        public static bool GenerateEpisodeDict([NotNull] ShowItem si)
        {
            // copy data from tvdb
            // process as per rules
            // done!

            bool r = true;

            lock (TheTVDB.SERIES_LOCK)
            {
                si.SeasonEpisodes.Clear();

                SeriesInfo ser = TheTVDB.Instance.GetSeries(si.TvdbCode);

                if (ser is null)
                {
                    Logger.Warn($"Asked to generate episodes for {si.ShowName}, but this has not yet been downloaded from TVDB");
                    return false;
                }

                Dictionary<int, Season> seasonsToUse = si.DvdOrder
                    ? ser.DvdSeasons
                    : ser.AiredSeasons;

                foreach (KeyValuePair<int, Season> kvp in seasonsToUse)
                {
                    List<ProcessedEpisode> pel = GenerateEpisodes(si, ser, kvp.Key, true);
                    si.SeasonEpisodes[kvp.Key] = pel;
                    if (pel is null)
                    {
                        r = false;
                    }
                }

                List<int> theKeys = new List<int>();
                // now, go through and number them all sequentially
                foreach (int snum in seasonsToUse.Keys)
                {
                    theKeys.Add(snum);
                }

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
                        if (si.DvdOrder)
                        {
                            overallCount += 1 + pe.EpNum2 - pe.DvdEpNum;
                        }
                        else
                        {
                            overallCount += 1 + pe.EpNum2 - pe.AiredEpNum;
                        }
                    }
                }
            }

            return r;
        }

        [CanBeNull]
        public static List<ProcessedEpisode> GenerateEpisodes([NotNull] ShowItem si, [NotNull] SeriesInfo ser, int snum, bool applyRules)
        {
            List<ProcessedEpisode> eis = new List<ProcessedEpisode>();

            Dictionary<int, Season> seasonsToUse = si.DvdOrder ? ser.DvdSeasons : ser.AiredSeasons;

            if (!seasonsToUse.ContainsKey(snum))
            {
                Logger.Error($"Asked to update season {snum} of {si.ShowName}, but it does not exist");
                return null;
            }

            Season seas = seasonsToUse[snum];

            if (seas is null)
            {
                Logger.Error($"Asked to update season {snum} of {si.ShowName}, whilst it exists, it has no contents");
                return null; 
            }

            foreach (Episode e in seas.Episodes.Values)
            {
                eis.Add(new ProcessedEpisode(e, si)); // add a copy
            }

            if (si.DvdOrder)
            {
                eis.Sort(ProcessedEpisode.DVDOrderSorter);
                Renumber(eis);
            }
            else
            {
                eis.Sort(ProcessedEpisode.EpNumberSorter);
            }

            if (si.CountSpecials && seasonsToUse.ContainsKey(0) && !TVSettings.Instance.IgnoreAllSpecials)
            {
                // merge specials in
                MergeSpecialsIn(si, snum, seasonsToUse, eis);
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

        private static void MergeSpecialsIn(ShowItem si, int snum, [NotNull] Dictionary<int, Season> seasonsToUse, [NotNull] List<ProcessedEpisode> eis)
        {
            foreach (Episode ep in seasonsToUse[0].Episodes.Values)
            {
                MergeEpisode(si, snum, ep, eis);
            }

            // renumber to allow for specials
            int epnumr = 1;
            foreach (ProcessedEpisode t in eis)
            {
                t.EpNum2 = epnumr + (t.EpNum2 - t.AppropriateEpNum);
                t.AppropriateEpNum = epnumr;
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
                if ((eis[i].AppropriateSeasonNumber == sease) && (eis[i].AppropriateEpNum == epnum))
                {
                    ProcessedEpisode pe = new ProcessedEpisode(ep, si)
                    {
                        TheAiredSeason = eis[i].TheAiredSeason,
                        TheDvdSeason = eis[i].TheDvdSeason,
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

        public static void ApplyRules([NotNull] List<ProcessedEpisode> eis, [NotNull] List<ShowRule> rules, ShowItem si)
        {
            foreach (ShowRule sr in rules)
            {
                int nn1 = sr.First;
                int nn2 = sr.Second;
                string txt = sr.UserSuppliedText;

                // turn nn1 and nn2 from ep number into position in array
                int n1 = FindIndex(eis, nn1);
                int n2 = FindIndex(eis,nn2);

                if (sr.DoWhatNow == RuleAction.kInsert)
                {
                    // this only applies for inserting an episode, at the end of the list
                    if (nn1 == eis[eis.Count - 1].AppropriateEpNum + 1) // after the last episode
                    {
                        n1 = eis.Count;
                    }
                }
                
                switch (sr.DoWhatNow)
                {
                    case RuleAction.kRename:
                        {
                            RenameEpisode(eis, n1, txt);
                            break;
                        }
                    case RuleAction.kRemove:
                        {
                            RemoveEpisode(eis, n1, n2);
                            break;
                        }
                    case RuleAction.kIgnoreEp:
                        {
                            IgnoreEpisodes(eis, n1, n2);
                            break;
                        }
                    case RuleAction.kSplit:
                        {
                            SplitEpisode(eis, si, nn2, n1);
                            break;
                        }
                    case RuleAction.kMerge:
                    case RuleAction.kCollapse:
                        {
                            RemoveEpisode(eis, si, sr, n1, n2, txt);
                            break;
                        }
                    case RuleAction.kSwap:
                        {
                            SwapEpisode(eis, n1, n2);
                            break;
                        }
                    case RuleAction.kInsert:
                        {
                            InsertEpisode(eis, si, n1, txt);
                            break;
                        }
                    default:
                        throw new ArgumentException("Inappropriate ruleType " + sr.DoWhatNow);
                } // switch DoWhatNow

                Renumber(eis);
            } // for each rule

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

        private static void IgnoreEpisodes([NotNull] List<ProcessedEpisode> eis, int n1, int n2)
        {
            int ec = eis.Count;
            int maxIndex = (n2 == -1)? n1:n2;

            for (int i = n1; i <= maxIndex; i++)
            {
                if ((i < ec) && (i >= 0))
                {
                    eis[i].Ignore = true;
                }
            }
        }

        private static void RemoveEpisode([NotNull] List<ProcessedEpisode> eis, int n1, int n2)
        {
            int ec = eis.Count;
            if (ValidIndex(n1, ec) && ValidIndex(n2, ec))
            {
                eis.RemoveRange(n1, 1 + n2 - n1);
            }
            else if (ValidIndex(n1, ec) && (n2 == -1))
            {
                eis.RemoveAt(n1);
            }
            else
            {
                //arguments are not consistent, so we'll do nothing
            }
        }

        private static bool ValidIndex(int index, int maxIndex) => (index < maxIndex) && (index >= 0);

        private static void RenameEpisode([NotNull] IReadOnlyList<ProcessedEpisode> eis, int index,string txt)
        {
            int ec = eis.Count;
            if (ValidIndex(index, ec))
            {
                eis[index].Name = txt;
            }
        }

        private static void SplitEpisode([NotNull] IList<ProcessedEpisode> eis, ShowItem si, int nn2, int n1)
        {
            int ec = eis.Count;
            // split one episode into a multi-parter
            if (ValidIndex(n1, ec))
            {
                ProcessedEpisode ei = eis[n1];
                string nameBase = ei.Name;
                eis.RemoveAt(n1); // remove old one
                for (int i = 0; i < nn2; i++) // make n2 new parts
                {
                    ProcessedEpisode pe2 =
                        new ProcessedEpisode(ei, si, ProcessedEpisode.ProcessedEpisodeType.split)
                        {
                            Name = nameBase + " (Part " + (i + 1) + ")",
                            AiredEpNum = -2,
                            DvdEpNum = -2,
                            EpNum2 = -2
                        };

                    eis.Insert(n1 + i, pe2);
                }
            }
        }

        private static void RemoveEpisode([NotNull] List<ProcessedEpisode> eis, ShowItem si, ShowRule sr, int n1, int n2, [CanBeNull] string txt)
        {
            int ec = eis.Count;
            if (ValidIndex(n1, ec) && ValidIndex(n2, ec) && (n1 < n2))
            {
                ProcessedEpisode oldFirstEi = eis[n1];
                List<string> episodeNames = new List<string> { eis[n1].Name };
                string defaultCombinedName = eis[n1].Name + " + ";
                string combinedSummary = eis[n1].Overview + "<br/><br/>";
                List<Episode> alleps = new List<Episode> {eis[n1]};
                for (int i = n1 + 1; i <= n2; i++)
                {
                    episodeNames.Add(eis[i].Name);
                    defaultCombinedName += eis[i].Name;
                    combinedSummary += eis[i].Overview;
                    alleps.Add(eis[i]);
                    if (i != n2)
                    {
                        defaultCombinedName += " + ";
                        combinedSummary += "<br/><br/>";
                    }
                }

                eis.RemoveRange(n1, n2 - n1);

                eis.RemoveAt(n1);

                string combinedName = GetBestNameFor(episodeNames, defaultCombinedName);

                ProcessedEpisode pe2 = new ProcessedEpisode(oldFirstEi, si, alleps)
                {
                    Name = ((string.IsNullOrEmpty(txt)) ? combinedName : txt),
                    AiredEpNum = -2,
                    DvdEpNum = -2
                };

                if (sr.DoWhatNow == RuleAction.kMerge)
                {
                    pe2.EpNum2 = -2 + n2 - n1;
                }
                else
                {
                    pe2.EpNum2 = -2;
                }

                pe2.Overview = combinedSummary;
                eis.Insert(n1, pe2);
            }
        }

        private static void InsertEpisode([NotNull] List<ProcessedEpisode> eis, ShowItem si, int n1, string txt)
        {
            int ec = eis.Count;
            if (ValidIndex(n1, ec))
            {
                ProcessedEpisode t = eis[n1];
                ProcessedEpisode n = new ProcessedEpisode(t.TheSeries, t.TheAiredSeason, t.TheDvdSeason, si)
                {
                    Name = txt,
                    AiredEpNum = -2,
                    DvdEpNum = -2,
                    EpNum2 = -2
                };

                eis.Insert(n1, n);
            }
            else if (n1 == eis.Count)
            {
                ProcessedEpisode t = eis[n1 - 1];
                ProcessedEpisode n = new ProcessedEpisode(t.TheSeries, t.TheAiredSeason, t.TheDvdSeason, si)
                {
                    Name = txt,
                    AiredEpNum = -2,
                    DvdEpNum = -2,
                    EpNum2 = -2
                };

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
            string root = Helpers.GetCommonStartString(episodeNames);
            int shortestEpisodeName = episodeNames.Min(x => x.Length);
            int longestEpisodeName = episodeNames.Max(x => x.Length);
            bool namesSameLength = (shortestEpisodeName == longestEpisodeName);
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

        private static void Renumber([NotNull] List<ProcessedEpisode> eis)
        {
            if (eis.Count == 0)
            {
                return; // nothing to do
            }

            // renumber 
            // pay attention to specials etc.
            int n = (eis[0].AppropriateEpNum == 0) ? 0 : 1;

            foreach (ProcessedEpisode t in eis)
            {
                if (t.AppropriateEpNum == -1)
                {
                    continue;
                }

                int num = t.EpNum2 - t.AppropriateEpNum;
                t.AppropriateEpNum = n;
                t.EpNum2 = n + num;
                n += num + 1;
            }
        }

        [NotNull]
        internal List<ShowItem> GetRecentShows()
        {
            // only scan "recent" shows
            List<ShowItem> shows = new List<ShowItem>();
            int dd = TVSettings.Instance.WTWRecentDays;

            // for each show, see if any episodes were aired in "recent" days...
            foreach (ShowItem si in GetShowItems())
            {
                bool added = false;

                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {
                    if (added)
                    {
                        break;
                    }

                    if (si.IgnoreSeasons.Contains(kvp.Key))
                    {
                        continue; // ignore this season
                    }

                    if (kvp.Key == 0 && TVSettings.Instance.IgnoreAllSpecials)
                    {
                        continue;
                    }

                    List<ProcessedEpisode> eis = kvp.Value;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        if (!ei.WithinDays(dd))
                        {
                            continue;
                        }

                        shows.Add(si);
                        added = true;
                        break;
                    }
                }
            }

            return shows;
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
            foreach (ShowItem si in GetShowItems().ToList())
            {
                if (!si.ShowNextAirdate)
                {
                    continue;
                }

                foreach (KeyValuePair<int, List<ProcessedEpisode>> v in si.SeasonEpisodes.ToList())
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

                        if ((airdt is null) || (airdt == DateTime.MaxValue))
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

                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {
                    if (si.IgnoreSeasons.Contains(kvp.Key))
                    {
                        continue; // ignore this season
                    }

                    if (kvp.Key == 0 && TVSettings.Instance.IgnoreAllSpecials)
                    {
                        continue;
                    }

                    List<ProcessedEpisode> eis = kvp.Value;

                    bool nextToAirFound = false;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        DateTime? dt = ei.GetAirDateDt(true);
                        if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        {
                            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
                            if (ts.TotalHours >= (-24 * days)) // in the future (or fairly recent)
                            {
                                if ((ts.TotalHours >= 0) && (!nextToAirFound))
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
                    if ((ser != null) && (si.CustomShowName == ser.Name))
                    {
                        // then, turn it off
                        si.CustomShowName = "";
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
                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in si.SeasonEpisodes)
                {
                    if (si.IgnoreSeasons.Contains(kvp.Key))
                    {
                        continue; // ignore this season
                    }

                    if (kvp.Key == 0 && TVSettings.Instance.IgnoreAllSpecials)
                    {
                        continue;
                    }

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
            List<int> toReIndex = this.Where(x => (x.Key != x.Value.TvdbCode)).Select(x=>x.Key).ToList();

            foreach (int x in toReIndex)
            {
                TryRemove(x,out ShowItem si);
                Add(si);
            }
        }
    }
}
