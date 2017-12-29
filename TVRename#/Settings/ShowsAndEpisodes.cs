// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using Alphaleonis.Win32.Filesystem;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

// These are what is used when processing folders for missing episodes, renaming, etc. of files.

// A "ProcessedEpisode" is generated by processing an Episode from thetvdb, and merging/renaming/etc.
//
// A "ShowItem" is a show the user has added on the "My Shows" tab

// TODO: C++ to C# conversion stopped it using some of the typedefs, such as "IgnoreSeasonList".  (a) probably should
// rename that to something more generic like IntegerList, and (b) then put it back into the classes & functions
// that use it (e.g. ShowItem.IgnoreSeasons)

namespace TVRename
{
    public class ProcessedEpisode : Episode
    {
        public int EpNum2; // if we are a concatenation of episodes, this is the last one in the series. Otherwise, same as EpNum
        public bool Ignore;
        public bool NextToAir;
        public int OverallNumber;
        public ShowItem SI;
        public ProcessedEpisodeType type;
        public List<Episode> sourceEpisodes;

        public enum ProcessedEpisodeType { single, split, merged};


        public ProcessedEpisode(SeriesInfo ser, Season seas, ShowItem si)
            : base(ser, seas)
        {
            NextToAir = false;
            OverallNumber = -1;
            Ignore = false;
            EpNum2 = EpNum;
            SI = si;
            type = ProcessedEpisodeType.single;
        }

        public ProcessedEpisode(ProcessedEpisode O)
            : base(O)
        {
            NextToAir = O.NextToAir;
            EpNum2 = O.EpNum2;
            Ignore = O.Ignore;
            SI = O.SI;
            OverallNumber = O.OverallNumber;
            type = O.type;
        }

        public ProcessedEpisode(Episode e, ShowItem si)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            EpNum2 = EpNum;
            Ignore = false;
            SI = si;
            type = ProcessedEpisodeType.single;
        }
        public ProcessedEpisode(Episode e, ShowItem si, ProcessedEpisodeType t)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            EpNum2 = EpNum;
            Ignore = false;
            SI = si;
            type = t;
        }

        public ProcessedEpisode(Episode e, ShowItem si, List<Episode> episodes)
            : base(e)
        {
            OverallNumber = -1;
            NextToAir = false;
            EpNum2 = EpNum;
            Ignore = false;
            SI = si;
            sourceEpisodes = episodes;
            type = ProcessedEpisodeType.merged ;
        }




        public string NumsAsString()
        {
            if (EpNum == EpNum2)
                return EpNum.ToString();
            else
                return EpNum + "-" + EpNum2;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static int EPNumberSorter(ProcessedEpisode e1, ProcessedEpisode e2)
        {
            int ep1 = e1.EpNum;
            int ep2 = e2.EpNum;

            return ep1 - ep2;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static int DVDOrderSorter(ProcessedEpisode e1, ProcessedEpisode e2)
        {
            int ep1 = e1.EpNum;
            int ep2 = e2.EpNum;

            string key = "DVD_episodenumber";
            if (e1.Items.ContainsKey(key) && e2.Items.ContainsKey(key))
            {
                string n1 = e1.Items[key];
                string n2 = e2.Items[key];
                if ((!string.IsNullOrEmpty(n1)) && (!string.IsNullOrEmpty(n2)))
                {
                    try
                    {
                        int t1 = (int) (1000.0 * double.Parse(n1));
                        int t2 = (int) (1000.0 * double.Parse(n2));
                        ep1 = t1;
                        ep2 = t2;
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            return ep1 - ep2;
        }
    }

    public class ShowItem
    {
        public bool AutoAddNewSeasons;
        public string AutoAdd_FolderBase; // TODO: use magical renaming tokens here
        public bool AutoAdd_FolderPerSeason;
        public string AutoAdd_SeasonFolderName; // TODO: use magical renaming tokens here

        public bool CountSpecials;
        public string CustomShowName;
        public bool DVDOrder; // sort by DVD order, not the default sort we get
        public bool DoMissingCheck;
        public bool DoRename;
        public bool ForceCheckFuture;
        public bool ForceCheckNoAirdate;
        public List<int> IgnoreSeasons;
        public Dictionary<int, List<String>> ManualFolderLocations;
        public bool PadSeasonToTwoDigits;
        public Dictionary<int, List<ProcessedEpisode>> SeasonEpisodes; // built up by applying rules.
        public Dictionary<int, List<ShowRule>> SeasonRules;
        public bool ShowNextAirdate;
        public int TVDBCode;
        public bool UseCustomShowName;
        public bool UseSequentialMatch;
        public List<string> AliasNames = new List<string>();
        public String CustomSearchURL;

        private DateTime? bannersLastUpdatedOnDisk;
        public DateTime? BannersLastUpdatedOnDisk
        {
            get
            {
                return bannersLastUpdatedOnDisk;
            }
            set
            {
                bannersLastUpdatedOnDisk = value;
            }
        }

        public ShowItem()
        {
            SetDefaults();
        }

        public ShowItem(int tvDBCode)
        {
            SetDefaults();
            TVDBCode = tvDBCode;
        }

        public ShowItem(XmlReader reader)
        {
            SetDefaults();

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
                    CustomShowName = reader.ReadElementContentAsString();
                    UseCustomShowName = true;
                }
                if (reader.Name == "UseCustomShowName")
                    UseCustomShowName = reader.ReadElementContentAsBoolean();
                if (reader.Name == "CustomShowName")
                    CustomShowName = reader.ReadElementContentAsString();
                else if (reader.Name == "TVDBID")
                    TVDBCode = reader.ReadElementContentAsInt();
                else if (reader.Name == "CountSpecials")
                    CountSpecials = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ShowNextAirdate")
                    ShowNextAirdate = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "AutoAddNewSeasons")
                    AutoAddNewSeasons = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "FolderBase")
                    AutoAdd_FolderBase = reader.ReadElementContentAsString();
                else if (reader.Name == "FolderPerSeason")
                    AutoAdd_FolderPerSeason = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "SeasonFolderName")
                    AutoAdd_SeasonFolderName = reader.ReadElementContentAsString();
                else if (reader.Name == "DoRename")
                    DoRename = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DoMissingCheck")
                    DoMissingCheck = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "DVDOrder")
                    DVDOrder = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "CustomSearchURL")
                    CustomSearchURL = reader.ReadElementContentAsString();
                else if (reader.Name == "ForceCheckAll") // removed 2.2.0b2
                    ForceCheckNoAirdate = ForceCheckFuture = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ForceCheckFuture")
                    ForceCheckFuture = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "ForceCheckNoAirdate")
                    ForceCheckNoAirdate = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "PadSeasonToTwoDigits")
                    PadSeasonToTwoDigits = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "BannersLastUpdatedOnDisk")
                {
                    if (!reader.IsEmptyElement)
                    {

                        BannersLastUpdatedOnDisk = reader.ReadElementContentAsDateTime();
                    }else
                    reader.Read();
                }

                else if (reader.Name == "UseSequentialMatch")
                    UseSequentialMatch = reader.ReadElementContentAsBoolean();
                else if (reader.Name == "IgnoreSeasons")
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        while (reader.Name != "IgnoreSeasons")
                        {
                            if (reader.Name == "Ignore")
                                IgnoreSeasons.Add(reader.ReadElementContentAsInt());
                            else
                                reader.ReadOuterXml();
                        }
                    }
                    reader.Read();
                }
                else if (reader.Name == "AliasNames")
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        while (reader.Name != "AliasNames")
                        {
                            if (reader.Name == "Alias")
                                AliasNames.Add(reader.ReadElementContentAsString());
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
                        SeasonRules[snum] = new List<ShowRule>();
                        reader.Read();
                        while (reader.Name != "Rules")
                        {
                            if (reader.Name == "Rule")
                            {
                                SeasonRules[snum].Add(new ShowRule(reader.ReadSubtree()));
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
                        ManualFolderLocations[snum] = new List<String>();
                        reader.Read();
                        while (reader.Name != "SeasonFolders")
                        {
                            if ((reader.Name == "Folder") && reader.IsStartElement())
                            {
                                string ff = reader.GetAttribute("Location");
                                if (AutoFolderNameForSeason(snum) != ff)
                                    ManualFolderLocations[snum].Add(ff);
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

        public SeriesInfo TheSeries()
        {
            return TheTVDB.Instance.GetSeries(TVDBCode);
        }

        public string ShowName
        {
            get
            {
                if (UseCustomShowName)
                    return CustomShowName;
                SeriesInfo ser = TheSeries();
                if (ser != null)
                    return ser.Name;
                return "<" + TVDBCode + " not downloaded>";
            }
        }

        public List<String> getSimplifiedPossibleShowNames()
        {
            List<String> possibles = new List<String>();

            String simplifiedShowName = Helpers.SimplifyName(ShowName);
            if (!(simplifiedShowName == "")) { possibles.Add( simplifiedShowName); }

            //Check the custom show name too
            if (UseCustomShowName)
            {
                String simplifiedCustomShowName = Helpers.SimplifyName(CustomShowName);
                if (!(simplifiedCustomShowName == "")) { possibles.Add(simplifiedCustomShowName); }
            }

            //Also add the aliases provided
            possibles.AddRange(from alias in AliasNames select Helpers.SimplifyName(alias));

            return possibles;

        }

        public string ShowStatus
        {
            get{
                SeriesInfo ser = TheSeries();
                if (ser != null ) return ser.getStatus();
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
                if (TheSeries() != null && TheSeries().Seasons != null && TheSeries().Seasons.Count > 0)
                {
                    foreach (KeyValuePair<int, Season> s in TheSeries().Seasons)
                    {
                        if(IgnoreSeasons.Contains(s.Key))
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
                    foreach (KeyValuePair<int, Season> s in TheSeries().Seasons)
                    {
                        if(IgnoreSeasons.Contains(s.Key))
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
                    foreach (KeyValuePair<int, Season> s in TheSeries().Seasons)
                    {
                        if(IgnoreSeasons.Contains(s.Key))
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
                return TheSeries()?.GetGenres();
            }
        }

        public void SetDefaults()
        {
            ManualFolderLocations = new Dictionary<int, List<string>>();
            IgnoreSeasons = new List<int>();
            UseCustomShowName = false;
            CustomShowName = "";
            UseSequentialMatch = false;
            SeasonRules = new Dictionary<int, List<ShowRule>>();
            SeasonEpisodes = new Dictionary<int, List<ProcessedEpisode>>();
            ShowNextAirdate = true;
            TVDBCode = -1;
            //                WhichSeasons = gcnew List<int>;
            //                NamingStyle = (int)NStyle.DefaultStyle();
            AutoAddNewSeasons = true;
            PadSeasonToTwoDigits = false;
            AutoAdd_FolderBase = "";
            AutoAdd_FolderPerSeason = true;
            AutoAdd_SeasonFolderName = "Season ";
            DoRename = true;
            DoMissingCheck = true;
            CountSpecials = false;
            DVDOrder = false;
            CustomSearchURL = "";
            ForceCheckNoAirdate = false;
            ForceCheckFuture = false;
            BannersLastUpdatedOnDisk = null; //assume that the baners are old and have expired

        }

        public List<ShowRule> RulesForSeason(int n)
        {
            if (SeasonRules.ContainsKey(n))
                return SeasonRules[n];
            else
                return null;
        }

        public string AutoFolderNameForSeason(int n)
        {
            bool leadingZero = TVSettings.Instance.LeadingZeroOnSeason || PadSeasonToTwoDigits;
            string r = AutoAdd_FolderBase;
            if (string.IsNullOrEmpty(r))
                return "";

            if (!r.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                r += System.IO.Path.DirectorySeparatorChar.ToString();
            if (AutoAdd_FolderPerSeason)
            {
                if (n == 0)
                    r += TVSettings.Instance.SpecialsFolderName;
                else
                {
                    r += AutoAdd_SeasonFolderName;
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
            foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in SeasonEpisodes)
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

            XMLHelper.WriteElementToXML(writer,"UseCustomShowName",UseCustomShowName);
            XMLHelper.WriteElementToXML(writer,"CustomShowName",CustomShowName);
            XMLHelper.WriteElementToXML(writer,"ShowNextAirdate",ShowNextAirdate);
            XMLHelper.WriteElementToXML(writer,"TVDBID",TVDBCode);
            XMLHelper.WriteElementToXML(writer,"AutoAddNewSeasons",AutoAddNewSeasons);
            XMLHelper.WriteElementToXML(writer,"FolderBase",AutoAdd_FolderBase);
            XMLHelper.WriteElementToXML(writer,"FolderPerSeason",AutoAdd_FolderPerSeason);
            XMLHelper.WriteElementToXML(writer,"SeasonFolderName",AutoAdd_SeasonFolderName);
            XMLHelper.WriteElementToXML(writer,"DoRename",DoRename);
            XMLHelper.WriteElementToXML(writer,"DoMissingCheck",DoMissingCheck);
            XMLHelper.WriteElementToXML(writer,"CountSpecials",CountSpecials);
            XMLHelper.WriteElementToXML(writer,"DVDOrder",DVDOrder);
            XMLHelper.WriteElementToXML(writer,"ForceCheckNoAirdate",ForceCheckNoAirdate);
            XMLHelper.WriteElementToXML(writer,"ForceCheckFuture",ForceCheckFuture);
            XMLHelper.WriteElementToXML(writer,"UseSequentialMatch",UseSequentialMatch);
            XMLHelper.WriteElementToXML(writer,"PadSeasonToTwoDigits",PadSeasonToTwoDigits);
            XMLHelper.WriteElementToXML(writer, "BannersLastUpdatedOnDisk", BannersLastUpdatedOnDisk);


            writer.WriteStartElement("IgnoreSeasons");
            foreach (int i in IgnoreSeasons)
            {
                XMLHelper.WriteElementToXML(writer,"Ignore",i);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("AliasNames");
            foreach (string str in AliasNames)
            {
                XMLHelper.WriteElementToXML(writer,"Alias",str);
            }
            writer.WriteEndElement();

            XMLHelper.WriteElementToXML(writer, "CustomSearchURL",CustomSearchURL);

            foreach (KeyValuePair<int, List<ShowRule>> kvp in SeasonRules)
            {
                if (kvp.Value.Count > 0)
                {
                    writer.WriteStartElement("Rules");
                    XMLHelper.WriteAttributeToXML(writer ,"SeasonNumber",kvp.Key);

                    foreach (ShowRule r in kvp.Value)
                        r.WriteXML(writer);

                    writer.WriteEndElement(); // Rules
                }
            }
            foreach (KeyValuePair<int, List<String>> kvp in ManualFolderLocations)
            {
                if (kvp.Value.Count > 0)
                {
                    writer.WriteStartElement("SeasonFolders");

                    XMLHelper.WriteAttributeToXML(writer,"SeasonNumber",kvp.Key);

                    foreach (string s in kvp.Value)
                    {
                        writer.WriteStartElement("Folder");
                        XMLHelper.WriteAttributeToXML(writer,"Location",s);
                        writer.WriteEndElement(); // Folder
                    }

                    writer.WriteEndElement(); // Rules
                }
            }

            writer.WriteEndElement(); // ShowItem
        }

        public static List<ProcessedEpisode> ProcessedListFromEpisodes(List<Episode> el, ShowItem si)
        {
            List<ProcessedEpisode> pel = new List<ProcessedEpisode>();
            foreach (Episode e in el)
                pel.Add(new ProcessedEpisode(e, si));
            return pel;
        }

        public Dictionary<int, List<string>> AllFolderLocations()
        {
            return AllFolderLocations( true);
        }

        public static string TTS(string s) // trim trailing slash
        {
            return s.TrimEnd(System.IO.Path.DirectorySeparatorChar);
        }

        public Dictionary<int, List<string>> AllFolderLocations(bool manualToo)
        {
            Dictionary<int, List<string>> fld = new Dictionary<int, List<string>>();

            if (manualToo)
            {
                foreach (KeyValuePair<int, List<string>> kvp in ManualFolderLocations)
                {
                    if (!fld.ContainsKey(kvp.Key))
                        fld[kvp.Key] = new List<String>();
                    foreach (string s in kvp.Value)
                        fld[kvp.Key].Add(TTS(s));
                }
            }

            if (AutoAddNewSeasons && (!string.IsNullOrEmpty(AutoAdd_FolderBase)))
            {
                int highestThereIs = -1;
                foreach (KeyValuePair<int, List<ProcessedEpisode>> kvp in SeasonEpisodes)
                {
                    if (kvp.Key > highestThereIs)
                        highestThereIs = kvp.Key;
                }
                foreach (int i in SeasonEpisodes.Keys)
                {
                    if (IgnoreSeasons.Contains(i))
                        continue;

                    string newName = AutoFolderNameForSeason(i);
                    if ((!string.IsNullOrEmpty(newName)) && (Directory.Exists(newName)))
                    {
                        if (!fld.ContainsKey(i))
                            fld[i] = new List<String>();
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
