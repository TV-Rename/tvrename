using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TVRename.Settings;
using System.IO;
using TVRename.db_access;
using TVRename.db_access.documents;
using TVRename.db_access.repository;

namespace TVRename.Shows
{
    public class ShowItem : IEntity<ShowItemDocument>
    {
        public ShowItemDocument innerDocument;
        private ShowItemRepository repo;

        public ShowItem(ShowItemDocument inner)
        {
            this.innerDocument = inner;
        }

        public ShowItem()
        {
            this.innerDocument = DefaultObjectFactory.getShowItemDocument(null);
        }

        public ShowItem(TheTVDB db)
        {
            innerDocument = DefaultObjectFactory.getShowItemDocument(db);
            repo = new ShowItemRepository(RavenSession.SessionInstance);
        }

        public ShowItem(TheTVDB db, int tvDBCode)
        {
            innerDocument = DefaultObjectFactory.getShowItemDocument(db);
            this.innerDocument.TVDBCode = tvDBCode;
            repo = new ShowItemRepository(RavenSession.SessionInstance);
        }

        public SeriesInfo TheSeries()
        {
            return this.innerDocument.TVDB.GetSeries(this.innerDocument.TVDBCode);
        }

        public string ShowName
        {
            get
            {
                if (this.innerDocument.UseCustomShowName)
                    return this.innerDocument.CustomShowName;
                SeriesInfo ser = this.TheSeries();
                if (ser != null)
                    return ser.Name;
                return "<" + this.innerDocument.TVDBCode + " not downloaded>";
            }
        }

        public string ShowStatus
        {
            get
            {
                SeriesInfo ser = this.TheSeries();
                if (ser != null && ser.Items != null && ser.Items.ContainsKey("Status"))
                    return ser.Items["Status"];

                return "Unknown";
            }
        }

        public enum ShowAirStatus
        {
            NoEpisodesOrSeasons,
            Aired,
            PartiallyAired,
            NoneAired
        }

        public ShowAirStatus SeasonsAirStatus
        {
            get
            {
                if (HasSeasonsAndEpisodes)
                {
                    if (HasAiredEpisodes && !HasUnairedEpisodes)
                    {
                        return ShowAirStatus.Aired;
                    }
                    else if (HasUnairedEpisodes && !HasAiredEpisodes)
                    {
                        return ShowAirStatus.NoneAired;
                    }
                    else if (HasAiredEpisodes && HasUnairedEpisodes)
                    {
                        return ShowAirStatus.PartiallyAired;
                    }
                    else
                    {
                        //System.Diagnostics.Debug.Assert(false, "That is weird ... we have 'seasons and episodes' but none are aired, nor unaired. That case shouldn't actually occur !");
                        return ShowAirStatus.NoEpisodesOrSeasons;
                    }
                }
                else
                {
                    return ShowAirStatus.NoEpisodesOrSeasons;
                }
            }
        }

        bool HasSeasonsAndEpisodes
        {
            get
            {
                if (this.TheSeries() != null && this.TheSeries().Seasons != null && this.TheSeries().Seasons.Count > 0)
                {
                    foreach (System.Collections.Generic.KeyValuePair<int, Season> s in this.TheSeries().Seasons)
                    {
                        if (this.innerDocument.IgnoreSeasons.Contains(s.Key))
                            continue;
                        if (s.Value.Episodes != null && s.Value.Episodes.Count > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        bool HasUnairedEpisodes
        {
            get
            {
                if (HasSeasonsAndEpisodes)
                {
                    foreach (System.Collections.Generic.KeyValuePair<int, Season> s in this.TheSeries().Seasons)
                    {
                        if (this.innerDocument.IgnoreSeasons.Contains(s.Key))
                            continue;
                        if (s.Value.Status == Season.SeasonStatus.NoneAired || s.Value.Status == Season.SeasonStatus.PartiallyAired)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        bool HasAiredEpisodes
        {
            get
            {
                if (HasSeasonsAndEpisodes)
                {
                    foreach (System.Collections.Generic.KeyValuePair<int, Season> s in this.TheSeries().Seasons)
                    {
                        if (this.innerDocument.IgnoreSeasons.Contains(s.Key))
                            continue;
                        if (s.Value.Status == Season.SeasonStatus.PartiallyAired || s.Value.Status == Season.SeasonStatus.Aired)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }


        public string[] Genres
        {
            get
            {
                SeriesInfo ser = this.TheSeries();
                if (ser != null && ser.Items != null && ser.Items.ContainsKey("Genre"))
                {
                    string[] genres = null;
                    string[] genreItems = ser.Items["Genre"].Split('|');
                    if (genreItems != null && genreItems.Length > 0)
                    {
                        System.Collections.Generic.List<string> genreItemsList = new System.Collections.Generic.List<string>();
                        foreach (string genreItem in genreItems)
                        {
                            if (!string.IsNullOrEmpty(genreItem.Trim()))
                            {
                                genreItemsList.Add(genreItem);
                            }
                        }
                        if (genreItemsList.Count > 0)
                        {
                            genres = genreItemsList.ToArray();
                        }
                    }
                    return genres;
                }
                else
                {
                    return null;
                }
            }
        }


        

        //Generic.List<int>WhichSeasons()
        //{
        //    System.Collections.Generic.List<int>r = gcnew System.Collections.Generic.List<int>();
        //    for each (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList>kvp in SeasonEpisodes)
        //        r->Add(kvp->Key);
        //    return r;
        //}

        public System.Collections.Generic.List<ShowRule> RulesForSeason(int n)
        {
            if (this.innerDocument.SeasonRules.ContainsKey(n))
                return this.innerDocument.SeasonRules[n];
            else
                return null;
        }

        public string AutoFolderNameForSeason(int n, Config settings)
        {
            bool leadingZero = settings.innerDocument.LeadingZeroOnSeason || this.innerDocument.PadSeasonToTwoDigits;
            string r = this.innerDocument.AutoAdd_FolderBase;
            if (string.IsNullOrEmpty(r))
                return "";

            if (!r.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                r += System.IO.Path.DirectorySeparatorChar.ToString();
            if (this.innerDocument.AutoAdd_FolderPerSeason)
            {
                if (n == 0)
                    r += settings.innerDocument.SpecialsFolderName;
                else
                {
                    r += this.innerDocument.AutoAdd_SeasonFolderName;
                    if ((n < 10) && leadingZero)
                        r += "0";
                    r += n.ToString();
                }
            }
            return r;
        }

        public int MaxSeason()
        {
            int max = 0;
            foreach (System.Collections.Generic.KeyValuePair<int, List<ProcessedEpisode>> kvp in this.innerDocument.SeasonEpisodes)
            {
                if (kvp.Key > max)
                    max = kvp.Key;
            }
            return max;
        }

        public void saveConfig()
        {
            repo.Add(this);
        }

        public static List<ProcessedEpisode> ProcessedListFromEpisodes(System.Collections.Generic.List<Episode> el, ShowItem si)
        {
            List<ProcessedEpisode> pel = new List<ProcessedEpisode>();
            foreach (Episode e in el)
                pel.Add(new ProcessedEpisode(e, si));
            return pel;
        }

        public System.Collections.Generic.Dictionary<int, StringList> AllFolderLocations(Config settings)
        {
            return this.AllFolderLocations(settings, true);
        }

        public static string TTS(string s) // trim trailing slash
        {
            return s.TrimEnd(System.IO.Path.DirectorySeparatorChar);
        }

        public System.Collections.Generic.Dictionary<int, StringList> AllFolderLocations(Config settings, bool manualToo)
        {
            System.Collections.Generic.Dictionary<int, StringList> fld = new System.Collections.Generic.Dictionary<int, StringList>();

            if (manualToo)
            {
                foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in this.innerDocument.ManualFolderLocations)
                {
                    if (!fld.ContainsKey(kvp.Key))
                        fld[kvp.Key] = new StringList();
                    foreach (string s in kvp.Value)
                        fld[kvp.Key].Add(TTS(s));
                }
            }

            if (this.innerDocument.AutoAddNewSeasons && (!string.IsNullOrEmpty(this.innerDocument.AutoAdd_FolderBase)))
            {
                int highestThereIs = -1;
                foreach (System.Collections.Generic.KeyValuePair<int, List<ProcessedEpisode>> kvp in this.innerDocument.SeasonEpisodes)
                {
                    if (kvp.Key > highestThereIs)
                        highestThereIs = kvp.Key;
                }
                foreach (int i in this.innerDocument.SeasonEpisodes.Keys)
                {
                    if (this.innerDocument.IgnoreSeasons.Contains(i))
                        continue;

                    string newName = this.AutoFolderNameForSeason(i, settings);
                    if ((!string.IsNullOrEmpty(newName)) && (Directory.Exists(newName)))
                    {
                        if (!fld.ContainsKey(i))
                            fld[i] = new StringList();
                        if (!fld[i].Contains(newName))
                            fld[i].Add(TTS(newName));
                    }
                }
            }

            return fld;
        }

        public static int CompareShowItemNames(ShowItem one, ShowItem two)
        {
            string ones = one.ShowName; // + " " +one->SeasonNumber.ToString("D3");
            string twos = two.ShowName; // + " " +two->SeasonNumber.ToString("D3");
            return ones.CompareTo(twos);
        }

        ShowItemDocument IEntity<ShowItemDocument>.GetInnerDocument()
        {
            return this.innerDocument;
        }
    }
}
