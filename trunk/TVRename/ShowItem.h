#pragma once

//#include "RenameItem.h"
#include "TheTVDB.h"
#include "ShowRule.h"
#include "Settings.h"

using namespace System::Text::RegularExpressions;
using namespace System::Collections;
using namespace System::IO;
using namespace System::Windows::Forms;
using namespace System;
using namespace System::Xml;

namespace TVRename
{
    ref class ShowItem;

    public ref class ProcessedEpisode :
        public Episode
        {
        public:
            int EpNum2; // if we are a concatenation of episodes, this is the last one in the series.  Otherwise, same as EpNum
            bool Ignore;
            bool NextToAir;
            int OverallNumber;
            ShowItem ^SI;

            ProcessedEpisode(SeriesInfo ^ser, Season ^seas, ShowItem ^si) :
            Episode(ser, seas)
            {
                NextToAir = false;
                OverallNumber = -1;
                Ignore = false;
                EpNum2 = EpNum;
                SI = si;
            }

            ProcessedEpisode(ProcessedEpisode ^O) :
            Episode(O)
            {
                NextToAir = O->NextToAir;
                EpNum2 = O->EpNum2;
                Ignore = O->Ignore;
                SI = O->SI;
                OverallNumber = O->OverallNumber;
            }
            ProcessedEpisode(Episode ^e, ShowItem ^si) :
            Episode(e)
            {
                OverallNumber = -1;
                NextToAir = false;
                EpNum2 = EpNum;
                Ignore = false;
                SI = si;
            }

            static int EPNumberSorter( ProcessedEpisode^ e1, ProcessedEpisode^ e2 )
            {
                int ep1 = e1->EpNum;
                int ep2 = e2->EpNum;

                return ep1-ep2;
            }

            static int DVDOrderSorter( ProcessedEpisode^ e1, ProcessedEpisode^ e2 )
            {
                int ep1 = e1->EpNum;
                int ep2 = e2->EpNum;

                String ^key = "DVD_episodenumber";
                if ( e1->Items->ContainsKey(key) &&
                    e2->Items->ContainsKey(key)  )
                {
                    String ^n1 = e1->Items[key];
                    String ^n2 = e2->Items[key];
                    if ((n1 != "") && (n2 != ""))
                    {
                        try 
                        {
                            int t1 = (int)(1000.0 * double::Parse(n1));
                            int t2 = (int)(1000.0 * double::Parse(n2));
                            ep1 = t1;
                            ep2 = t2;
                        } 
                        catch (FormatException ^)
                        {
                        }
                    }
                }

                return ep1-ep2;
            }
        };

        typedef Generic::List<ProcessedEpisode ^> ProcessedEpisodeList;
        typedef Generic::Dictionary<int, ProcessedEpisodeList ^> EpisodeDict; // dictionary by season #
        typedef Generic::List<int> IgnoreSeasonList;
        typedef Generic::Dictionary<int, StringList ^> FolderLocationDict; 

        //enum CheckType { checkNone = 0, checkAll = 1, checkRecent = 2}; // TODO: remove this, and make a list of seasons/eps to ignore

        public ref class ShowItem
        {
        public:
            TheTVDB ^TVDB;
            bool UseCustomShowName;
            String ^CustomShowName;
            RuleDict ^SeasonRules;
            EpisodeDict ^SeasonEpisodes; // built up by applying rules.
            bool ShowNextAirdate;
            int TVDBCode;

            bool AutoAddNewSeasons;
            String ^AutoAdd_FolderBase; // TODO: use magical renaming tokens here
            String ^AutoAdd_SeasonFolderName; // TODO: use magical renaming tokens here
            bool AutoAdd_FolderPerSeason;

            FolderLocationDict ^ManualFolderLocations;

            bool UseSequentialMatch;
            bool DoRename;
            bool CountSpecials;
            bool DoMissingCheck;
            bool PadSeasonToTwoDigits;
            IgnoreSeasonList ^IgnoreSeasons;
            bool DVDOrder; // sort by DVD order, not the default sort we get
            bool ForceCheckAll;

        public:
            SeriesInfo ^TheSeries()
            {
                return TVDB->GetSeries(TVDBCode);
            }
            String ^ShowName()
            {
                if (UseCustomShowName)
                    return CustomShowName;
                SeriesInfo ^ser = TheSeries();
                if (ser != nullptr)
                    return ser->Name;
                return "<Error: "+TVDBCode.ToString()+ " not in thetvdb>";
            }
            void SetDefaults(TheTVDB ^db)
            {
                TVDB = db;
                ManualFolderLocations = gcnew FolderLocationDict();
                IgnoreSeasons = gcnew IgnoreSeasonList();
                UseCustomShowName = false;
                CustomShowName = "";
                UseSequentialMatch = false;
                SeasonRules = gcnew RuleDict();
                SeasonEpisodes = gcnew EpisodeDict();
                ShowNextAirdate = true;
                TVDBCode = -1;
                //                WhichSeasons = gcnew Generic::List<int>;
                //                NamingStyle = (int)NStyle::DefaultStyle();
                AutoAddNewSeasons = true;
                PadSeasonToTwoDigits = false;
                AutoAdd_FolderBase = "";
                AutoAdd_FolderPerSeason = true;
                AutoAdd_SeasonFolderName = "Season ";
                DoRename = true;
                DoMissingCheck = true;
                CountSpecials = false;
                DVDOrder = false;
                ForceCheckAll = false;
            }
            //Generic::List<int> ^WhichSeasons()
            //{
            //    Generic::List<int> ^r = gcnew Generic::List<int>();
            //    for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in SeasonEpisodes)
            //        r->Add(kvp->Key);
            //    return r;
            //}

            ShowItem(TheTVDB ^db)
            {
                SetDefaults(db);
            }

            RuleSet ^RulesForSeason(int n)
            {
                if (SeasonRules->ContainsKey(n))
                    return SeasonRules[n];
                else
                    return nullptr;
            }

            String ^AutoFolderNameForSeason(int n, TVSettings ^settings)
            {
                bool leadingZero = settings->LeadingZeroOnSeason || PadSeasonToTwoDigits;
                String ^r = AutoAdd_FolderBase;
                if (r == "")
                    return "";

                if (!r->EndsWith("\\"))
                    r += "\\";
                if (AutoAdd_FolderPerSeason)
                {
                    if (n == 0)
                        r += settings->SpecialsFolderName;
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


            int MaxSeason()
            {
                int max = 0;
                for each (KeyValuePair<int, ProcessedEpisodeList ^> ^kvp in SeasonEpisodes)
                    if (kvp->Key > max)
                        max = kvp->Key;
                return max;
            }

            //String ^NiceName(int season)
            //{
            //    // something like "Simpsons (S3)"
            //    return String::Concat(ShowName," (S",season,")");
            //}

            void WriteXMLSettings(XmlWriter ^writer)
            {
                writer->WriteStartElement("ShowItem");

                writer->WriteStartElement("UseCustomShowName");
                writer->WriteValue(UseCustomShowName);
                writer->WriteEndElement();
                writer->WriteStartElement("CustomShowName");
                writer->WriteValue(CustomShowName);
                writer->WriteEndElement();
                writer->WriteStartElement("ShowNextAirdate");
                writer->WriteValue(ShowNextAirdate);
                writer->WriteEndElement();
                writer->WriteStartElement("TVDBID");
                writer->WriteValue(TVDBCode);
                writer->WriteEndElement();
                writer->WriteStartElement("AutoAddNewSeasons");
                writer->WriteValue(AutoAddNewSeasons);
                writer->WriteEndElement();
                writer->WriteStartElement("FolderBase");
                writer->WriteValue(AutoAdd_FolderBase);
                writer->WriteEndElement();
                writer->WriteStartElement("FolderPerSeason");
                writer->WriteValue(AutoAdd_FolderPerSeason);
                writer->WriteEndElement();
                writer->WriteStartElement("SeasonFolderName");
                writer->WriteValue(AutoAdd_SeasonFolderName);
                writer->WriteEndElement();
                writer->WriteStartElement("DoRename");
                writer->WriteValue(DoRename);
                writer->WriteEndElement();
                writer->WriteStartElement("DoMissingCheck");
                writer->WriteValue(DoMissingCheck);
                writer->WriteEndElement();
                writer->WriteStartElement("CountSpecials");
                writer->WriteValue(CountSpecials);
                writer->WriteEndElement();
                writer->WriteStartElement("DVDOrder");
                writer->WriteValue(DVDOrder);
                writer->WriteEndElement();
                writer->WriteStartElement("ForceCheckAll");
                writer->WriteValue(ForceCheckAll);
                writer->WriteEndElement();
                writer->WriteStartElement("UseSequentialMatch");
                writer->WriteValue(UseSequentialMatch);
                writer->WriteEndElement();
                writer->WriteStartElement("PadSeasonToTwoDigits");
                writer->WriteValue(PadSeasonToTwoDigits);
                writer->WriteEndElement();

                writer->WriteStartElement("IgnoreSeasons");
                for each (int i in IgnoreSeasons)
                {
                    writer->WriteStartElement("Ignore");
                    writer->WriteValue(i);
                    writer->WriteEndElement();
                }
                writer->WriteEndElement();

                for each (KeyValuePair<int, RuleSet ^> ^kvp in SeasonRules)
                {
                    if (kvp->Value->Count > 0)
                    {
                        writer->WriteStartElement("Rules");
                        writer->WriteStartAttribute("SeasonNumber");
                        writer->WriteValue(kvp->Key);
                        writer->WriteEndAttribute();

                        for each (ShowRule ^r in kvp->Value)
                            r->WriteXML(writer);

                        writer->WriteEndElement(); // Rules
                    }
                }
                for each (KeyValuePair<int, StringList ^> ^kvp in ManualFolderLocations)
                {
                    if (kvp->Value->Count > 0)
                    {
                        writer->WriteStartElement("SeasonFolders");
                        writer->WriteStartAttribute("SeasonNumber");
                        writer->WriteValue(kvp->Key);
                        writer->WriteEndAttribute();

                        for each (String ^s in kvp->Value)
                        {
                            writer->WriteStartElement("Folder");
                            writer->WriteStartAttribute("Location");
                            writer->WriteValue(s);
                            writer->WriteEndAttribute();
                            writer->WriteEndElement(); // Folder
                        }

                        

                        writer->WriteEndElement(); // Rules
                    }
                }

                writer->WriteEndElement(); // ShowItem
            }

            ShowItem(TheTVDB ^db, XmlReader ^reader, TVSettings ^settings)
            {
                SetDefaults(db);

                reader->Read();
                if (reader->Name != "ShowItem")
                    return; // bail out

                reader->Read();
                while (!reader->EOF)
                {
                    if ((reader->Name == "ShowItem") && !reader->IsStartElement())
                        break; // all done

                    if (reader->Name == "ShowName")
                    {
                        CustomShowName = reader->ReadElementContentAsString();
                        UseCustomShowName = true;
                    }
                    if (reader->Name == "UseCustomShowName")
                        UseCustomShowName = reader->ReadElementContentAsBoolean();
                    if (reader->Name == "CustomShowName")
                        CustomShowName = reader->ReadElementContentAsString();
                    else if (reader->Name == "TVDBID")
                        TVDBCode = reader->ReadElementContentAsInt();
                    else if (reader->Name == "CountSpecials")
                        CountSpecials = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "ShowNextAirdate")
                        ShowNextAirdate = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "AutoAddNewSeasons")
                        AutoAddNewSeasons = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "FolderBase")
                        AutoAdd_FolderBase = reader->ReadElementContentAsString();
                    else if (reader->Name == "FolderPerSeason")
                        AutoAdd_FolderPerSeason = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "SeasonFolderName")
                        AutoAdd_SeasonFolderName = reader->ReadElementContentAsString();
                    else if (reader->Name == "DoRename")
                        DoRename = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "DoMissingCheck")
                        DoMissingCheck = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "DVDOrder")
                        DVDOrder = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "ForceCheckAll")
                        ForceCheckAll = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "PadSeasonToTwoDigits")
                        PadSeasonToTwoDigits = reader->ReadElementContentAsBoolean();
                    else if (reader->Name == "IgnoreSeasons")
                    {
                        if (!reader->IsEmptyElement)
                        {
                            reader->Read();
                            while (reader->Name != "IgnoreSeasons")
                            {
                                if (reader->Name == "Ignore")
                                    IgnoreSeasons->Add(reader->ReadElementContentAsInt());
                                else
                                    reader->ReadOuterXml();
                            }
                        }
                        reader->Read();
                    }
                    else if (reader->Name == "Rules")
                    {
                        if (!reader->IsEmptyElement)
                        {
                            int snum = int::Parse(reader->GetAttribute("SeasonNumber"));
                            SeasonRules[snum] = gcnew RuleSet();
                            reader->Read();
                            while (reader->Name != "Rules")
                            {
                                if (reader->Name == "Rule")
                                {
                                    SeasonRules[snum]->Add(gcnew ShowRule(reader->ReadSubtree()));
                                    reader->Read();
                                }
                            }
                        }
                        reader->Read();
                    }
                    else if (reader->Name == "SeasonFolders")
                    {
                        if (!reader->IsEmptyElement)
                        {
                            int snum = int::Parse(reader->GetAttribute("SeasonNumber"));
                            ManualFolderLocations[snum] = gcnew StringList();
                            reader->Read();
                            while (reader->Name != "SeasonFolders")
                            {
                                if ((reader->Name == "Folder") && reader->IsStartElement())
                                {
                                    String ^ff = reader->GetAttribute("Location");
                                    if (AutoFolderNameForSeason(snum, settings) != ff)
                                        ManualFolderLocations[snum]->Add(ff);
                                }
                                reader->Read();
                            }
                        }
                        reader->Read();
                    }

                    else 
                        reader->ReadOuterXml();
                } // while
            }



            static ProcessedEpisodeList ^ProcessedListFromEpisodes(EpisodeList ^el, ShowItem ^si)
            {
                ProcessedEpisodeList ^pel = gcnew ProcessedEpisodeList();
                for each (Episode ^e in el)
                    pel->Add(gcnew ProcessedEpisode(e, si));
                return pel;
            }

            FolderLocationDict ^AllFolderLocations(TVSettings ^settings)
            {
                FolderLocationDict ^fld = gcnew FolderLocationDict();
                
                for each (KeyValuePair<int, StringList ^> ^kvp in ManualFolderLocations)
                {
                    fld[kvp->Key] = gcnew StringList();
                    for each (String ^s in kvp->Value)
                        fld[kvp->Key]->Add(s);
                }

                if (AutoAddNewSeasons && (AutoAdd_FolderBase != ""))
                {
                    int highestThereIs = -1;
                    for each (KeyValuePair<int , ProcessedEpisodeList ^> ^kvp in SeasonEpisodes)
                        if (kvp->Key > highestThereIs)
                            highestThereIs = kvp->Key;

                    for (int i=0;i<=highestThereIs;i++) // start at 0 for specials season
                    {
                        if (IgnoreSeasons->Contains(i))
                            continue;

                        String ^newName = AutoFolderNameForSeason(i, settings);
                        if ((newName != "") && (DirectoryInfo(newName).Exists))
                        {
                            if (!fld->ContainsKey(i))
                              fld[i] = gcnew StringList();
                            fld[i]->Add(newName);
                        }
                    }
                }
                    
                return fld;

            }


        }; // ShowItem

        int CompareShowItemNames(ShowItem ^one, ShowItem ^two);

        typedef Generic::List<ShowItem ^> ShowItemList;


} // namespace


