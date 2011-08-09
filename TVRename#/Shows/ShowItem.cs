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

        
        

        public ShowItem(TheTVDB db)
        {
            innerDocument = DefaultObjectFactory.getShowItemDocument(db);
        }

        public ShowItem(TheTVDB db, int tvDBCode)
        {
            innerDocument = DefaultObjectFactory.getShowItemDocument(db);
            this.innerDocument.TVDBCode = tvDBCode;
        }

        public ShowItem(TheTVDB db, XmlReader reader, Config settings)
        {
            innerDocument = DefaultObjectFactory.getShowItemDocument(db);

            reader.Read();
            if (reader.Name != "ShowItem")
                return; // bail out

            reader.Read();
            while (!reader.EOF)
            {
                if ((reader.Name == "ShowItem") && !reader.IsStartElement())
                    break; // all done

                if (reader.Name == "ShowName")
                {
                    this.innerDocument.CustomShowName = reader.ReadElementContentAsString();
                    this.innerDocument.UseCustomShowName = true;
                }
                if (reader.Name == "UseCustomShowName")
                    this.innerDocument.UseCustomShowName = reader.ReadElementContentAsBoolean();
                if (reader.Name == "CustomShowName")
                    this.innerDocument.CustomShowName = reader.ReadElementContentAsString();
                else if (reader.Name == "TVDBID")
                    this.innerDocument.TVDBCode = reader.ReadElementContentAsInt();
                else if (reader.Name == "CountSpecials")
                    this.innerDocument.CountSpecials = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowNextAirdate")
                    this.innerDocument.ShowNextAirdate = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoAddNewSeasons")
                    this.innerDocument.AutoAddNewSeasons = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderBase")
                    this.innerDocument.AutoAdd_FolderBase = reader.ReadElementContentAsString();
                else if (reader.Name == "FolderPerSeason")
                    this.innerDocument.AutoAdd_FolderPerSeason = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SeasonFolderName")
                    this.innerDocument.AutoAdd_SeasonFolderName = reader.ReadElementContentAsString();
                else if (reader.Name == "DoRename")
                    this.innerDocument.DoRename = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DoMissingCheck")
                    this.innerDocument.DoMissingCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DVDOrder")
                    this.innerDocument.DVDOrder = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ForceCheckAll") // removed 2.2.0b2
                    this.innerDocument.ForceCheckNoAirdate = this.innerDocument.ForceCheckFuture = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ForceCheckFuture")
                    this.innerDocument.ForceCheckFuture = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ForceCheckNoAirdate")
                    this.innerDocument.ForceCheckNoAirdate = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "PadSeasonToTwoDigits")
                    this.innerDocument.PadSeasonToTwoDigits = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "UseSequentialMatch")
                    this.innerDocument.UseSequentialMatch = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IgnoreSeasons")
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        while (reader.Name != "IgnoreSeasons")
                        {
                            if (reader.Name == "Ignore")
                                this.innerDocument.IgnoreSeasons.Add(reader.ReadElementContentAsInt());
                            else
                                reader.ReadOuterXml();
                        }
                    }
                    reader.Read();
                }
                else if (reader.Name == "Rules")
                {
                    if (!reader.IsEmptyElement)
                    {
                        int snum = int.Parse(reader.GetAttribute("SeasonNumber"));
                        this.innerDocument.SeasonRules[snum] = new System.Collections.Generic.List<ShowRule>();
                        reader.Read();
                        while (reader.Name != "Rules")
                        {
                            if (reader.Name == "Rule")
                            {
                                this.innerDocument.SeasonRules[snum].Add(new ShowRule(reader.ReadSubtree()));
                                reader.Read();
                            }
                        }
                    }
                    reader.Read();
                }
                else if (reader.Name == "SeasonFolders")
                {
                    if (!reader.IsEmptyElement)
                    {
                        int snum = int.Parse(reader.GetAttribute("SeasonNumber"));
                        this.innerDocument.ManualFolderLocations[snum] = new StringList();
                        reader.Read();
                        while (reader.Name != "SeasonFolders")
                        {
                            if ((reader.Name == "Folder") && reader.IsStartElement())
                            {
                                string ff = reader.GetAttribute("Location");
                                if (this.AutoFolderNameForSeason(snum, settings) != ff)
                                    this.innerDocument.ManualFolderLocations[snum].Add(ff);
                            }
                            reader.Read();
                        }
                    }
                    reader.Read();
                }

                else
                    reader.ReadOuterXml();
            } // while
        }

        public ShowItemDocument GetInnerDocument()
        {
            return this.innerDocument;
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
            foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in this.innerDocument.SeasonEpisodes)
            {
                if (kvp.Key > max)
                    max = kvp.Key;
            }
            return max;
        }

        //StringNiceName(int season)
        //{
        //    // something like "Simpsons (S3)"
        //    return String.Concat(ShowName," (S",season,")");
        //}

        public void WriteXMLSettings(XmlWriter writer)
        {
            writer.WriteStartElement("ShowItem");

            writer.WriteStartElement("UseCustomShowName");
            writer.WriteValue(this.innerDocument.UseCustomShowName);
            writer.WriteEndElement();
            writer.WriteStartElement("CustomShowName");
            writer.WriteValue(this.innerDocument.CustomShowName);
            writer.WriteEndElement();
            writer.WriteStartElement("ShowNextAirdate");
            writer.WriteValue(this.innerDocument.ShowNextAirdate);
            writer.WriteEndElement();
            writer.WriteStartElement("TVDBID");
            writer.WriteValue(this.innerDocument.TVDBCode);
            writer.WriteEndElement();
            writer.WriteStartElement("AutoAddNewSeasons");
            writer.WriteValue(this.innerDocument.AutoAddNewSeasons);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderBase");
            writer.WriteValue(this.innerDocument.AutoAdd_FolderBase);
            writer.WriteEndElement();
            writer.WriteStartElement("FolderPerSeason");
            writer.WriteValue(this.innerDocument.AutoAdd_FolderPerSeason);
            writer.WriteEndElement();
            writer.WriteStartElement("SeasonFolderName");
            writer.WriteValue(this.innerDocument.AutoAdd_SeasonFolderName);
            writer.WriteEndElement();
            writer.WriteStartElement("DoRename");
            writer.WriteValue(this.innerDocument.DoRename);
            writer.WriteEndElement();
            writer.WriteStartElement("DoMissingCheck");
            writer.WriteValue(this.innerDocument.DoMissingCheck);
            writer.WriteEndElement();
            writer.WriteStartElement("CountSpecials");
            writer.WriteValue(this.innerDocument.CountSpecials);
            writer.WriteEndElement();
            writer.WriteStartElement("DVDOrder");
            writer.WriteValue(this.innerDocument.DVDOrder);
            writer.WriteEndElement();
            writer.WriteStartElement("ForceCheckNoAirdate");
            writer.WriteValue(this.innerDocument.ForceCheckNoAirdate);
            writer.WriteEndElement();
            writer.WriteStartElement("ForceCheckFuture");
            writer.WriteValue(this.innerDocument.ForceCheckFuture);
            writer.WriteEndElement();
            writer.WriteStartElement("UseSequentialMatch");
            writer.WriteValue(this.innerDocument.UseSequentialMatch);
            writer.WriteEndElement();
            writer.WriteStartElement("PadSeasonToTwoDigits");
            writer.WriteValue(this.innerDocument.PadSeasonToTwoDigits);
            writer.WriteEndElement();

            writer.WriteStartElement("IgnoreSeasons");
            foreach (int i in this.innerDocument.IgnoreSeasons)
            {
                writer.WriteStartElement("Ignore");
                writer.WriteValue(i);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            foreach (System.Collections.Generic.KeyValuePair<int, System.Collections.Generic.List<ShowRule>> kvp in this.innerDocument.SeasonRules)
            {
                if (kvp.Value.Count > 0)
                {
                    writer.WriteStartElement("Rules");
                    writer.WriteStartAttribute("SeasonNumber");
                    writer.WriteValue(kvp.Key);
                    writer.WriteEndAttribute();

                    foreach (ShowRule r in kvp.Value)
                        r.WriteXML(writer);

                    writer.WriteEndElement(); // Rules
                }
            }
            foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in this.innerDocument.ManualFolderLocations)
            {
                if (kvp.Value.Count > 0)
                {
                    writer.WriteStartElement("SeasonFolders");
                    writer.WriteStartAttribute("SeasonNumber");
                    writer.WriteValue(kvp.Key);
                    writer.WriteEndAttribute();

                    foreach (string s in kvp.Value)
                    {
                        writer.WriteStartElement("Folder");
                        writer.WriteStartAttribute("Location");
                        writer.WriteValue(s);
                        writer.WriteEndAttribute();
                        writer.WriteEndElement(); // Folder
                    }

                    writer.WriteEndElement(); // Rules
                }
            }

            writer.WriteEndElement(); // ShowItem
        }

        public static ProcessedEpisodeList ProcessedListFromEpisodes(System.Collections.Generic.List<Episode> el, ShowItem si)
        {
            ProcessedEpisodeList pel = new ProcessedEpisodeList();
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
                foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in this.innerDocument.SeasonEpisodes)
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
    }
}
