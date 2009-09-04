#pragma once

#include "TimeZone.h"

// TheTVDB -> Series (class SeriesInfo) -> Seasons (class Season) -> Episodes (class Episode)

using namespace System;
using namespace System::Xml;
using namespace System::IO;
using namespace System::Collections;
using namespace System::Data;
using namespace System::Text::RegularExpressions;
using namespace Ionic::Utils::Zip;
using namespace System::Net;
using namespace System::Threading;

using Collections::Generic::KeyValuePair;

namespace TVRename {
    ref class Episode;
    ref class SeriesInfo;
    ref class Season;
    ref class ExtraEp;

    typedef Generic::List<Episode ^> EpisodeList;
    typedef Generic::List<ExtraEp ^> ExtraEpList;
    typedef Generic::List<String ^> LanguageListType;
    typedef Generic::Dictionary<int, Season ^> SeasonDict;
    typedef Generic::Dictionary<String ^, String^> StringDict;
    typedef Generic::Dictionary <int, SeriesInfo ^> SeriesDict;

    enum typeMaskBits { tmMainSite = 0, tmXML = 1, tmBanner = 2, tmZIP = 4 }; // defined by thetvdb

    public ref class Season
    {
    public:
        SeriesInfo ^TheSeries;
        EpisodeList ^Episodes;
        int SeasonNumber;
        int SeasonID;

        Season(SeriesInfo ^theSeries, int number, int seasonid);
    };

    public ref class SeasonComparer :  System::Collections::Generic::IComparer<Season ^>
    {
    public: virtual int Compare(Season ^one, Season ^two)
            {
                return one->SeasonNumber - two->SeasonNumber;
            }
    };

    static String ^ReadStringFixQuotesAndSpaces(XmlReader ^r)
    {
        String ^res = r->ReadElementContentAsString();
        res = res->Replace("\\'","'");
        res = res->Replace("\\\"","\"");
        res = res->Trim();
        return res;
    }

    
    public ref class Episode
    {
    public:
        int EpisodeID;
        int EpNum;
        int SeriesID;
        int SeasonID;
        DateTime ^FirstAired;
        int Srv_LastUpdated;
        String ^Overview;
        String ^Name;
        StringDict ^Items; // other fields we don't specifically grab

        int ReadSeasonNum; // only use after loading to attach to the correct season!

        Season ^TheSeason;
        SeriesInfo ^TheSeries;

        bool Dirty;

        String ^GetItem(String ^which)
        {
            if (Items->ContainsKey(which))
                return Items[which];
            else
                return "";
        }

        bool OK()
        {
            return ((SeriesID != -1) &&
                (EpisodeID != -1) &&
                (EpNum != -1) &&
                ( (SeasonID != -1) || (ReadSeasonNum != -1))
                );
        }

        void SetDefaults(SeriesInfo ^ser, Season ^seas)
        {
            Items = gcnew StringDict();
            TheSeason = seas;
            TheSeries = ser;

            Overview = "";
            Name = "";
            EpisodeID = -1;
            SeriesID = -1;
            EpNum = -1;
            FirstAired = nullptr;
            Srv_LastUpdated = -1;
            Dirty = false;
        }

        Episode(Episode ^O)
        {
            EpisodeID = O->EpisodeID;
            SeriesID = O->SeriesID;
            EpNum = O->EpNum;
            FirstAired = O->FirstAired;
            Srv_LastUpdated = O->Srv_LastUpdated;
            Overview = O->Overview;
            Name = O->Name;
            TheSeason = O->TheSeason;
            TheSeries = O->TheSeries;
            SeasonID = O->SeasonID;
            Dirty = O->Dirty;

            Items = gcnew StringDict();
            for each (KeyValuePair<String ^, String^> ^i in O->Items)
                Items->Add(i->Key, i->Value);
        }

        property int SeasonNumber
        {
            int get() 
            { 
                if (TheSeason != nullptr) 
                    return TheSeason->SeasonNumber; 
                else
                    return -1;
            }
        }

        virtual String ^ToString() override
        {
            return "S" + SeasonNumber + "E" + EpNum + " (" + SeriesID+ "," + EpisodeID + "), Aired " + (FirstAired != nullptr ? FirstAired->ToString("yyyy-MM-dd") : "") + ", " + Name;
        }

        System::DateTime ^GetAirDateDT(bool correct);

        String ^HowLong()
        {
            DateTime ^dt = GetAirDateDT(true);
            if (dt == nullptr)
                return "";

            TimeSpan ^ts = dt->Subtract(DateTime::Now);  // how long...
            if (ts->TotalHours < 0)
                return "Aired";
            else
            {
                int h = ts->Hours;
                if (ts->TotalHours >= 1)
                {
                    if (ts->Minutes >= 30)
                        h += 1;
                    return ts->Days+"d "+h+"h"; // +ts->Minutes+"m "+ts->Seconds+"s";
                }
                else
                    return System::Math::Round(ts->TotalMinutes).ToString()+"min";
            }
            return "";

        }

        String ^DayOfWeek()
        {
            DateTime ^dt = GetAirDateDT(true);
            return dt->ToString("ddd");
        }

        String ^TimeOfDay()
        {
            DateTime ^dt = GetAirDateDT(true);
            return dt->ToString("t");
        }

        Episode(SeriesInfo ^ser, Season ^seas)
        {
            SetDefaults(ser,seas);
        }

        void SetSeriesSeason(SeriesInfo ^ser, Season ^seas)
        {
            TheSeason = seas;
            TheSeries = ser;
        }

        void WriteXml(XmlWriter ^writer)
        {
            writer->WriteStartElement("Episode");

            writer->WriteStartElement("id");
            writer->WriteValue(EpisodeID);
            writer->WriteEndElement();
            writer->WriteStartElement("seriesid");
            writer->WriteValue(SeriesID);
            writer->WriteEndElement();
            writer->WriteStartElement("seasonid");
            writer->WriteValue(SeasonID);
            writer->WriteEndElement();
            writer->WriteStartElement("EpisodeNumber");
            writer->WriteValue(EpNum);
            writer->WriteEndElement();
            writer->WriteStartElement("SeasonNumber");
            writer->WriteValue(SeasonNumber);
            writer->WriteEndElement();
            writer->WriteStartElement("lastupdated");
            writer->WriteValue(Srv_LastUpdated);
            writer->WriteEndElement();
            writer->WriteStartElement("Overview");
            writer->WriteValue(Overview);
            writer->WriteEndElement();
            writer->WriteStartElement("EpisodeName");
            writer->WriteValue(Name);
            writer->WriteEndElement();
            if (FirstAired != nullptr)
            {
                writer->WriteStartElement("FirstAired");
                writer->WriteValue(FirstAired->ToString("yyyy-MM-dd"));
                writer->WriteEndElement();
            }

            for each (KeyValuePair<String ^, String ^> ^kvp in Items)
            {
                writer->WriteStartElement(kvp->Key);
                writer->WriteValue(kvp->Value);
                writer->WriteEndElement();
            }


            writer->WriteEndElement();
        }

        Episode(SeriesInfo ^ser, Season ^seas, XmlReader ^r)
        {
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>

            SetDefaults(ser, seas);

            r->Read();
            if (r->Name != "Episode")
                return;

            r->Read();
            while (!r->EOF)
            {
                if ((r->Name == "Episode") && (!r->IsStartElement()))
                    break;
                if (r->Name == "id")
                    EpisodeID = r->ReadElementContentAsInt();
                if (r->Name == "seriesid")
                    SeriesID = r->ReadElementContentAsInt();  // thetvdb series id
                if (r->Name == "seasonid")
                    SeasonID = r->ReadElementContentAsInt();
                else if (r->Name == "EpisodeNumber")
                    EpNum = r->ReadElementContentAsInt();
                else if (r->Name == "SeasonNumber")
                    ReadSeasonNum = r->ReadElementContentAsInt();
                else if (r->Name == "lastupdated")
                    Srv_LastUpdated = r->ReadElementContentAsInt();
                else if (r->Name == "Overview")
                    Overview = ReadStringFixQuotesAndSpaces(r);
                else if (r->Name == "EpisodeName")
                    Name = ReadStringFixQuotesAndSpaces(r);
                else if (r->Name == "FirstAired")
                {
                    try 
                    {
                        FirstAired = DateTime::ParseExact(r->ReadElementContentAsString(),"yyyy-MM-dd",
                            gcnew System::Globalization::CultureInfo(""));
                    }
                    catch (...)
                    {
                        FirstAired = nullptr;
                    }
                }					
                else
                {
                    if ((r->IsEmptyElement) || !r->IsStartElement())
                        r->ReadOuterXml();
                    else
                    {
                        XmlReader ^r2 = r->ReadSubtree();
                        r2->Read();
                        String ^name = r2->Name;
                        Items[name] = r2->ReadElementContentAsString();
                        r->Read();
                    }
                }
            }
        }
    };

    public ref class SeriesInfo
    {
    private:
        String ^LastFiguredTZ;
        TZIBytes TZIBytes;

        void FigureOutTZI()
        {
            String ^tzstr = TimeZone;

            if (tzstr == "")
                tzstr = TZMagic::DefaultTZ();

            TZIBytes = TZMagic::GetTZ(tzstr);

            LastFiguredTZ = tzstr;
        }

    public:
        TZI *GetTZI()
        {
            static TZI sTZ;
            if (LastFiguredTZ != TimeZone)
                FigureOutTZI();

            if (TZIBytes == nullptr)
                return false;

            if (TZMagic::BytesToTZI(TZIBytes, &sTZ))
                return &sTZ;
            else
                return 0; // TODO: warn user
        }


    public:
        int TVDBCode;
        int Srv_LastUpdated;
        String ^Name;

        StringDict ^Items; // e.g. Overview, Banner, Poster, etc.
        DateTime ^AirsTime;
        SeasonDict ^Seasons;
        String ^Language;

        bool Dirty; // set to true if local info is known to be older than whats on the server

        String ^TimeZone; // set for us by ShowItem

        // note: "SeriesID" in a <Series> is the tv.com code,
        // "seriesid" in an <Episode> is the tvdb code!

        String ^GetItem(String ^which)
        {
            if (Items->ContainsKey(which))
                return Items[which];
            else
                return "";
        }

        SeriesInfo(String ^name, int id)
        {
            SetToDefauts();
            Name = name;
            TVDBCode = id;
        }

        SeriesInfo(XmlReader ^r)
        {
            SetToDefauts();
            LoadXml(r);
        }

        void SetToDefauts()
        {
            TimeZone = TZMagic::DefaultTZ(); // default, is correct for most shows
            LastFiguredTZ = "";

            Items = gcnew StringDict();
            Seasons = gcnew SeasonDict();
            Dirty = false;
            Name = "";
            AirsTime = nullptr;
            TVDBCode = -1;
            Language = "";
        }

        int LanguagePriority(LanguageListType ^languages)
        {
            if (Language == "")
                return 999999;
            int r = languages->IndexOf(Language); // -1 for not found
            return (r == -1) ? 999999 : r;
        }

        void Merge(SeriesInfo ^o, LanguageListType ^languages)
        {
            if (o->TVDBCode != TVDBCode)
                return; // that's not us!
            if (o->Srv_LastUpdated < Srv_LastUpdated)
                return; // older!?
            
            bool betterLanguage = o->LanguagePriority(languages) < LanguagePriority(languages); // lower is better

            Srv_LastUpdated = o->Srv_LastUpdated;

            // take the best bits of "o"
            // "o" is always newer/better than us, if there is a choice
            if ((o->Name != "") && betterLanguage)
                Name = o->Name;
            Items->Clear();
            for each (KeyValuePair<String ^, String ^> ^kvp in o->Items)
            {
                if ((kvp->Value != "") || betterLanguage)
                    Items[kvp->Key] = kvp->Value;
            }
            if (o->AirsTime != nullptr)
                AirsTime = o->AirsTime;
            if ((o->Seasons != nullptr) && (o->Seasons->Count != 0))
                Seasons = o->Seasons;

            if (betterLanguage)
                Language = o->Language;


            Dirty = o->Dirty;
        }

        void LoadXml(XmlReader ^r)
        {
            //<Data>
            // <Series>
            //  <id>...</id>
            //  etc.
            // </Series>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // ...
            //</Data>

            r->Read();
            if (r->Name != "Series")
                return;

            r->Read();
            while (!r->EOF)
            {

                if ((r->Name == "Series") && (!r->IsStartElement()))
                    break;
                if (r->Name == "id")
                    TVDBCode = r->ReadElementContentAsInt();
                else if (r->Name == "SeriesName")
                    Name = ReadStringFixQuotesAndSpaces(r);
                else if (r->Name == "lastupdated")
                    Srv_LastUpdated = r->ReadElementContentAsInt();
                else if ((r->Name == "Language") || (r->Name == "language"))
                    Language = r->ReadElementContentAsString();
                else if (r->Name == "TimeZone")
                    TimeZone = r->ReadElementContentAsString();
                else if (r->Name == "Airs_Time")
                {
                    AirsTime = DateTime::Parse("20:00");

                    try 
                    {
                        String ^theTime = r->ReadElementContentAsString();
                        if (theTime != "")
                        {
                            Items["Airs_Time"] = theTime;                        
                            AirsTime = DateTime::Parse(theTime);
                        }
                    }
                    catch (FormatException ^)
                    {
                    }
                }
                else 
                {
                    String ^name = r->Name;
                    Items[name] = r->ReadElementContentAsString();
                }
                //   r->ReadOuterXml(); // skip
            }
        } // LoadXml
        void WriteXml(XmlWriter ^writer)
        {
            writer->WriteStartElement("Series");

            writer->WriteStartElement("id");
            writer->WriteValue(TVDBCode);
            writer->WriteEndElement();

            writer->WriteStartElement("SeriesName");
            writer->WriteValue(Name);
            writer->WriteEndElement();

            writer->WriteStartElement("lastupdated");
            writer->WriteValue(Srv_LastUpdated);
            writer->WriteEndElement();

            writer->WriteStartElement("Language");
            writer->WriteValue(Language);
            writer->WriteEndElement();

            for each (KeyValuePair<String ^, String ^> ^kvp in Items)
            {
                writer->WriteStartElement(kvp->Key);
                writer->WriteValue(kvp->Value);
                writer->WriteEndElement();
            }
            writer->WriteStartElement("TimeZone");
            writer->WriteValue(TimeZone);
            writer->WriteEndElement();

            writer->WriteEndElement(); // series
        }
        Season ^GetOrAddSeason(int num, int seasonID)
        {
            if (Seasons->ContainsKey(num))
                return Seasons[num];

            Season ^s = gcnew Season(this, num, seasonID);
            Seasons[num] = s;

            return s;
        }
    };

    
    public ref class ExtraEp
    {
    public:
        int SeriesID;
        int EpisodeID;

        ExtraEp(int series, int episode) :
        SeriesID(series),
            EpisodeID(episode)
        {
        }
    };

    public ref class TheTVDB
    {
    private:
        int Srv_Time; // only update this after a 100% successful download
        int New_Srv_Time; 
        //System::Threading::Mutex ^Lock;

        Collections::Generic::List<String ^> ^WhoHasLock;

    private:
        SeriesDict ^Series; // TODO: make this private or a property.  have online/offline state that controls auto downloading of needed info.
        ExtraEpList ^ExtraEpisodes; // IDs of extra episodes to grab and merge in on next update

    public:
        String ^LastError;
        bool Connected;
        LanguageListType ^LanguagePriorityList;
        StringDict ^LanguageList;
        String ^CurrentDLTask;
        String ^XMLMirror;	
        String ^BannerMirror;
        String ^ZIPMirror;

        bool HasSeries(int id)
        {
            return Series->ContainsKey(id);
        }
        SeriesInfo ^GetSeries(int id)
        {
            if (!HasSeries(id))
                return nullptr;
            else
                return Series[id];
        }
        SeriesDict ^GetSeriesDict()
        {
            return Series;
        }

        void GetLock(String ^whoFor)
        {
            return;
            Diagnostics::Debug::Print("Lock Series for " + whoFor);
            Monitor::Enter(Series);
            WhoHasLock->Add(whoFor);
        }
        void Unlock(String ^whoFor)
        {
            return;
            int n = WhoHasLock->Count - 1;
            String ^whoHad = WhoHasLock[n];
#if defined(DEBUG)
            Diagnostics::Debug::Assert(whoFor == whoHad);
#endif
            Diagnostics::Debug::Print("Unlock series ("+whoFor+")");
            WhoHasLock->RemoveAt(n);

            Monitor::Exit(Series);
        }
    private:

        void Say(String ^s)
        {
            CurrentDLTask = s;
        }
    public:

        TheTVDB()
        {
            LastError = "";
            WhoHasLock = gcnew Collections::Generic::List<String ^>;
            LanguagePriorityList = gcnew LanguageListType();
            LanguagePriorityList->Add("en");
            Connected = false;
            ExtraEpisodes = gcnew ExtraEpList();

            LanguageList = gcnew StringDict();
            LanguageList["en"] = "English";

            XMLMirror = "http://thetvdb.com";
            BannerMirror = "http://thetvdb.com";
            ZIPMirror = "http://thetvdb.com";

            Series = gcnew SeriesDict();
            New_Srv_Time = Srv_Time = 0;

            LoadCache();
        }

        void LoadCache()
        {
            String ^fn = System::Windows::Forms::Application::UserAppDataPath+"\\TheTVDB.xml";

            if (!FileInfo(fn).Exists)
                return;

            FileStream ^fs = gcnew FileStream(fn, FileMode::Open);
            ProcessTVDBResponse(fs);
            fs->Close();
            UpdatesDoneOK();
        }

        void UpdatesDoneOK()
        {
            // call when all downloading and updating is done.  updates local Srv_Time with the tentative
            // new_srv_time value.
            Srv_Time = New_Srv_Time;

        }
        void SaveCache()
        {
            GetLock("SaveCache");
            String ^fname = System::Windows::Forms::Application::UserAppDataPath+"\\TheTVDB.xml";

            if (File::Exists(fname))
            {
                double hours = 999.9;
                if (File::Exists(fname+".0"))
                {
                    // see when the last rotate was, and only rotate if its been at least an hour since the last save
                    DateTime ^dt = FileInfo(fname+".0").LastWriteTime;
                    hours = DateTime::Now.Subtract(*dt).TotalHours;
                }
                if (hours >= 24.0) // rotate the save file daily
                {
                    for (int i=8;i>=0;i--)
                    {
                        String ^fn = fname + "." + i.ToString();
                        if (File::Exists(fn))
                        {
                            String ^fn2 = fname + "." + (i+1).ToString();
                            if (File::Exists(fn2))
                                File::Delete(fn2);
                            File::Move(fn, fn2);
                        }
                    }

                    File::Copy(fname, fname+".0");
                }
            }

            // write ourselves to disc for next time.  use same structure as thetvdb.com (limited fields, though)
            // to make loading easy
            XmlWriterSettings ^settings = gcnew XmlWriterSettings();
            settings->Indent = true;
            settings->NewLineOnAttributes = true;
            XmlWriter ^writer = XmlWriter::Create(fname, settings);
            writer->WriteStartDocument();
            writer->WriteStartElement("Data");
            writer->WriteStartAttribute("time");
            writer->WriteValue(Srv_Time);
            writer->WriteEndAttribute();

            String ^lp = "";
            for each (String ^s in LanguagePriorityList)
                lp += s+" ";
            writer->WriteStartAttribute("TVRename_LanguagePriority");
            writer->WriteValue(lp);
            writer->WriteEndAttribute();


            for each (KeyValuePair<int, SeriesInfo^> ^kvp in Series)
            {
                if (kvp->Value->Srv_LastUpdated != 0)
                {
                    kvp->Value->WriteXml(writer);
                    for each (KeyValuePair<int, Season ^> ^kvp2 in kvp->Value->Seasons)
                    {
                        Season ^seas = kvp2->Value;
                        for each (Episode ^e in seas->Episodes)
                            e->WriteXml(writer);
                    }
                }
            }

            writer->WriteEndElement(); // data

            writer->WriteEndDocument();
            writer->Close();
            Unlock("SaveCache");
        }

        public: Episode ^FindEpisodeByID(int id)
        {
            GetLock("FindEpisodeByID");
            for each (KeyValuePair<int, SeriesInfo^> ^kvp in Series)
            {
                for each (KeyValuePair<int, Season ^> ^kvp2 in kvp->Value->Seasons)
                {
                    Season ^seas = kvp2->Value;
                    for each (Episode ^e in seas->Episodes)
                        if (e->EpisodeID == id)
                        {
                            Unlock("FindEpisodeByID");
                            return e;
                        }
                }
            }
            Unlock("FindEpisodeByID");
            return nullptr;
        }

        public: bool Connect()
             {
                 Connected = GetMirrors() && GetLanguages();
                 return Connected;
             }

             private: array<unsigned char> ^GetPageZIP(String ^url, String ^extractFile, bool useKey)
             {
                 array<unsigned char> ^zipped = GetPage(url, useKey, tmZIP);
                 
                 if (zipped == nullptr)
                     return nullptr;

                 MemoryStream ^ms = gcnew MemoryStream(zipped);
                 MemoryStream ^theFile = gcnew MemoryStream();
                 try 
                 {
                     ZipFile ^zf = ZipFile::Read(ms);
                     zf->Extract(extractFile, theFile);
                     Diagnostics::Debug::Print("Downloaded " + url + ", " + ms->Length + " bytes became " + theFile->Length);
                 }
                 catch (Exception ^e)
                 {
                     LastError = CurrentDLTask + " : " + e->Message;
                     return nullptr;
                 }

                 // ZipFile allocates more buffer than is needed, so we need to resize the array before returning it
                 array<unsigned char> ^r = theFile->GetBuffer();
                 Array::Resize(r, (int)theFile->Length);

                 return r;
             }

             array<unsigned char> ^GetPage(String ^url, bool useKey, typeMaskBits mirrorType)
             {
                 String ^mirr = "";
                 switch (mirrorType)
                 {
                 case tmXML: 
                     mirr = XMLMirror; 
                     break;
                 case tmBanner: 
                     mirr = BannerMirror; 
                     break;
                 case tmZIP: 
                     mirr = ZIPMirror; 
                     break;
                 default: 
                 case tmMainSite:
                     mirr = "http://www.thetvdb.com"; break;
                 }
                 if (url->StartsWith("/"))
                     url = url->Substring(1);

                 String ^theURL = mirr+"/api/"+(useKey ? "5FEC454623154441" : "")+"/"+url;

                 //HttpWebRequest ^wr = dynamic_cast<HttpWebRequest ^>(HttpWebRequest::Create(theURL));
                 //wr->Timeout = 10000; // 10 seconds
                 //wr->Method = "GET";
                 //wr->KeepAlive = false;

                 System::Net::WebClient ^wc = gcnew System::Net::WebClient();

                 try {
                     array<unsigned char> ^r = wc->DownloadData(theURL);
                     //HttpWebResponse ^wres = dynamic_cast<HttpWebResponse ^>(wr->GetResponse());
                     //Stream ^str = wres->GetResponseStream();
                     //array<unsigned char> ^r = gcnew array<unsigned char>((int)str->Length);
                     //str->Read(r, 0, (int)str->Length);

                     if (!url->EndsWith(".zip"))
                         Diagnostics::Debug::Print("Downloaded " + url + ", " + r->Length + " bytes");

                     return r;
                 }
                 catch (Exception ^e)
                 {
                     LastError = CurrentDLTask + " : " + e->Message;
                     return nullptr;
                 }
             }
    public:
        void ForgetEverything()
        {
            GetLock("ForgetEverything");
            Series->Clear();
            Connected = false;
            SaveCache();
            Unlock("ForgetEverything");
        }
        void ForgetShow(int id, bool makePlaceholder)
        {
            GetLock("ForgetShow");
            if (Series->ContainsKey(id))
            {
                String ^name = Series[id]->Name;
                Series->Remove(id);
                if (makePlaceholder)
                    MakePlaceholderSeries(id, name);
            }
            Unlock("ForgetShow");
        }

        bool GetLanguages()
        {
            Say("TheTVDB Languages");

            array<unsigned char> ^p = GetPage("languages.xml", true, tmMainSite);
            if (p == nullptr)
                return false;;
            MemoryStream ^ms = gcnew MemoryStream(p);

            XmlReaderSettings ^settings = gcnew XmlReaderSettings();
            settings->IgnoreComments = true;
            settings->IgnoreWhitespace = true;
            XmlReader ^reader = XmlReader::Create(ms, settings);
            reader->Read();

            if (reader->Name != "xml")
                return false;

            reader->Read();

            if (reader->Name != "Languages")
                return false;

            reader->Read(); // move forward one

            LanguageList->Clear();

            while (!reader->EOF)
            {
                if (reader->Name == "Languages" && !reader->IsStartElement())
                    break; // end of mirror whatsit

                if (reader->Name != "Language")
                    return false;

                XmlReader ^r = reader->ReadSubtree();
                r->Read(); // puts us on "Language"
                int ID = -1;
                String ^name = "";
                String ^abbrev = "";

                r->Read(); // get onto the first thingy

                while (!r->EOF)
                {
                    if (r->Name == "Language" && !r->IsStartElement())
                    {
                        if ((ID != -1) && (name != "") && (abbrev != ""))
                            LanguageList[abbrev] = name;
                        break; // end of language whatsit
                    }

                    if (r->Name == "id")
                        ID = r->ReadElementContentAsInt();
                    else if (r->Name == "name")
                        name = r->ReadElementContentAsString();
                    else if (r->Name == "abbreviation")
                        abbrev = r->ReadElementContentAsString();
                    else
                        r->ReadOuterXml(); // skip unknown element
                }
                reader->Read(); // move forward one
            }
            return true;

        }

        bool GetMirrors()
        {
            // get mirror list
            Say("TheTVDB Mirrors");

            Generic::List<String ^> ^XMLMirrorList = gcnew Generic::List<String ^>;
            Generic::List<String ^> ^BannerMirrorList = gcnew Generic::List<String ^>;
            Generic::List<String ^> ^ZIPMirrorList = gcnew Generic::List<String ^>;

            array<unsigned char> ^p = GetPage("mirrors.xml", true, tmMainSite);
            if (p == nullptr)
                return false;;
            MemoryStream ^ms = gcnew MemoryStream(p);

            XmlReaderSettings ^settings = gcnew XmlReaderSettings();
            settings->IgnoreComments = true;
            settings->IgnoreWhitespace = true;
            XmlReader ^reader = XmlReader::Create(ms, settings);
            reader->Read();

            if (reader->Name != "xml")
                return false;

            reader->Read();

            if (reader->Name != "Mirrors")
                return false;

            reader->Read(); // move forward one

            while (!reader->EOF)
            {
                if (reader->Name == "Mirrors" && !reader->IsStartElement())
                    break; // end of mirror whatsit

                if (reader->Name != "Mirror")
                    return false;

                XmlReader ^r = reader->ReadSubtree();
                r->Read(); // puts us on "Mirror"
                int ID = -1;
                String ^mirrorPath = "";
                int typeMask = -1;

                r->Read(); // get onto the first thingy

                while (!r->EOF)
                {
                    if (r->Name == "Mirror" && !r->IsStartElement())
                    {
                        if ((ID != -1) && (mirrorPath != "") && (typeMask != -1))
                        {
                            if (typeMask & tmXML)
                                XMLMirrorList->Add(mirrorPath);
                            if (typeMask & tmBanner)
                                BannerMirrorList->Add(mirrorPath);
                            if (typeMask & tmZIP)
                                ZIPMirrorList->Add(mirrorPath);
                        }
                        break; // end of mirror whatsit
                    }

                    if (r->Name == "id")
                        ID = r->ReadElementContentAsInt();
                    else if (r->Name == "mirrorpath")
                        mirrorPath = r->ReadElementContentAsString();
                    else if (r->Name == "typemask")
                        typeMask = r->ReadElementContentAsInt();
                    else
                        r->ReadOuterXml(); // skip unknown element
                }
                reader->Read(); // move forward one
            }

            // choose a random mirror to use
            int c = 0;
            Random ^r = gcnew Random((Int32)DateTime::Now.Ticks & 0xFFFFFFFF);
            c = ZIPMirrorList->Count;
            if (c)
                ZIPMirror = ZIPMirrorList[r->Next(0,c-1)];
            c = XMLMirrorList->Count;
            if (c)
                XMLMirror = XMLMirrorList[r->Next(0,c-1)];
            c = BannerMirrorList->Count;
            if (c)
                BannerMirror = BannerMirrorList[r->Next(0,c-1)];

            return true;
        }

        bool GetUpdates()
        {
            Say("Updates list");
            int theTime = Srv_Time;

            if (theTime == 0)
            {
                // we can use the oldest thing we have locally.  It isn't safe to use the newest thing.
                // This will only happen the first time we do an update, so a false _all update isn't too bad.
                for each (KeyValuePair<int, SeriesInfo^> ^kvp in Series)
                {
                    SeriesInfo ^ser = kvp->Value;
                    if ((theTime == 0) || ((ser->Srv_LastUpdated != 0) && (ser->Srv_LastUpdated < theTime)) )
                        theTime = ser->Srv_LastUpdated;
                    for each (KeyValuePair<int, Season ^> ^kvp2 in kvp->Value->Seasons)
                    {
                        Season ^seas = kvp2->Value;

                        for each (Episode ^e in seas->Episodes)
                            if ((theTime == 0) || ((e->Srv_LastUpdated != 0) && (e->Srv_LastUpdated < theTime)) )
                                theTime = e->Srv_LastUpdated;
                    }
                }

            }

            // anything with a srv_lastupdated of 0 should be marked as dirty
            // typically, this'll be placeholder series
            for each (KeyValuePair<int, SeriesInfo^> ^kvp in Series)
            {
                SeriesInfo ^ser = kvp->Value;
                if ((ser->Srv_LastUpdated == 0) || (ser->Seasons->Count == 0))
                    ser->Dirty = true;
                for each (KeyValuePair<int, Season ^> ^kvp2 in kvp->Value->Seasons)
                {
                    for each (Episode ^ep in kvp2->Value->Episodes)
                        if (ep->Srv_LastUpdated == 0)
                            ep->Dirty = true;
                }
            }

            if (theTime == 0)
                return true; // that's it for now

            unsigned long epoch = (unsigned long)(DateTime::UtcNow.Subtract(DateTime(1970,1,1,0,0,0,0)).TotalSeconds);
            int seconds = epoch - theTime;
            if (seconds < 3540) // 59 minutes
                return true;

            String ^timePeriod = "";

            int howLongDays = (int)(seconds / (60*60*24));

            if ((howLongDays < 1) || (Series->Count == 0))
                timePeriod = "day";
            else if ((howLongDays >= 1) && (howLongDays < 7))
                timePeriod = "week";
            else if ((howLongDays >= 7) && (howLongDays < 28))
                timePeriod = "month";
            else
                timePeriod = "all";


            if (!Connected && !Connect())
                return false;


            if (timePeriod != "all")
                Say("Updates list for the " + timePeriod);
            else
                Say("Updates list for everything");

            // http://thetvdb.com/api/5FEC454623154441/updates/updates_day.xml
            // day, week, month, all

            String ^udf = "updates_"+timePeriod;
            array<unsigned char> ^p = GetPageZIP("updates/"+udf+".zip",udf+".xml", true);
            if (p == nullptr)
                return false;

            //BinaryWriter ^fs = gcnew BinaryWriter(gcnew FileStream("c:\\temp\\ud.xml", FileMode::Create));
            //fs->Write(p, 0, p->Length);
            //fs->Close();

            MemoryStream ^ms = gcnew MemoryStream(p);

            return ProcessUpdateList(ms);
        }

        bool ProcessUpdateList(MemoryStream ^ms)
        {
            // if updatetime > localtime for item, then remove it, so it will be downloaded later

            XmlReaderSettings ^settings = gcnew XmlReaderSettings();
            settings->IgnoreComments = true;
            settings->IgnoreWhitespace = true;
            XmlReader ^reader = XmlReader::Create(ms, settings);
            reader->Read();

            if (reader->Name != "xml")
                return false;

            reader->Read();

            if ((reader->Name != "Data") || (reader->AttributeCount != 1))
                return false;

            New_Srv_Time = int::Parse(reader->GetAttribute("time"));

            // what follows is the last update time for a bunch of zero or more series, episodes, and banners

            while (!reader->EOF)
            {
                reader->Read();
                if (reader->Name == "Series")
                {
                    //<Series>
                    // <id>70761</id>
                    // <time>1221383086</time>
                    //</Series>
                    int ID = -1;
                    int time = -1;
                    XmlReader ^r = reader->ReadSubtree();
                    r->Read(); // puts us on "Series"
                    r->Read(); // get onto first thingy
                    while (!r->EOF)
                    {
                        if (r->Name == "Series" && !r->IsStartElement())
                        {
                            if ((ID != -1) && (time != -1))
                            {
                                if (Series->ContainsKey(ID)) // this is a series we have
                                {
                                    if (time > Series[ID]->Srv_LastUpdated) // newer version on the server
                                        Series[ID]->Dirty = true; // mark as dirty, so it'll be fetched again later
                                }
                            }
                            break;
                        }

                        if (r->Name == "id")
                            ID = r->ReadElementContentAsInt();
                        else if (r->Name == "time")
                            time = r->ReadElementContentAsInt();
                        else 
                            r->ReadOuterXml(); // skip
                    }
                } // series
                else if (reader->Name == "Episode")
                {
                    //<Episode>
                    //<id>73175</id>
                    //<Series>72102</Series>
                    //<time>1221387596</time>
                    //</Episode>
                    int serID = -1;
                    int time = -1;
                    int epID = -1;
                    XmlReader ^r = reader->ReadSubtree();
                    r->Read(); // puts us on "Series"
                    r->Read(); // get onto first thingy
                    while (!r->EOF)
                    {
                        if (r->Name == "Episode" && !r->IsStartElement())
                        {
                            if ((serID != -1) && (time != -1) && (epID != -1))
                            {
                                if (Series->ContainsKey(serID))
                                {
                                    bool found = false;
                                    for each (KeyValuePair<int, Season ^> ^kvp2 in Series[serID]->Seasons)
                                    {
                                        Season ^seas = kvp2->Value;

                                        for each (Episode ^ep in seas->Episodes)
                                        {
                                            if (ep->EpisodeID == epID)
                                            {
                                                if ( ep->Srv_LastUpdated < time )
                                                   ep->Dirty = true; // mark episode as dirty.
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!found)
                                    {
                                        // must be a new episode
                                        ExtraEpisodes->Add(gcnew ExtraEp(serID, epID));

                                    }
                                }
                            }
                            break;
                        }

                        if (r->Name == "id")
                            epID = r->ReadElementContentAsInt();
                        else if (r->Name == "time")
                            time = r->ReadElementContentAsInt();
                        else if (r->Name == "Series")
                            serID = r->ReadElementContentAsInt();
                        else 
                            r->ReadOuterXml(); // skip
                    }
                }
                else
                    reader->ReadOuterXml(); // skip
            } // reader EOF

            // if more than 30% of a show's episodes are marked as dirty, just download the entire show again
            for each (KeyValuePair<int, SeriesInfo^> ^kvp in Series)
            {
               int totaleps = 0;
               int totaldirty = 0;
               for each (KeyValuePair<int, Season ^> ^kvp2 in kvp->Value->Seasons)
                    for each (Episode ^ep in kvp2->Value->Episodes)
                    {
                        if (ep->Dirty)
                            totaldirty++;
                        totaleps++;
                    }
                if (totaldirty >= totaleps/3)
                {
                    kvp->Value->Dirty = true;
                    kvp->Value->Seasons->Clear();
                }
            }



            return true;
        }

        void ProcessTVDBResponse(Stream ^ms)
        {
            // Will have one or more series, and episodes
            // all wrapped in <Data> </Data>


            // e.g.: 
            //<Data>
            // <Series>
            //  <id>...</id>
            //  etc.
            // </Series>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // <Episode>
            //  <id>...</id>
            //  blah blah
            // </Episode>
            // ...
            //</Data>

            GetLock("ProcessTVDBResponse");

            XmlReaderSettings ^settings = gcnew XmlReaderSettings();
            settings->IgnoreComments = true;
            settings->IgnoreWhitespace = true;
            XmlReader ^r = XmlReader::Create(ms, settings);

            r->Read();

            while (!r->EOF)
            {
                if ((r->Name == "Data") && !r->IsStartElement())
                    break; // that's it.
                if (r->Name == "Series")
                {
                    // The <series> returned by GetSeries have
                    // less info than other results from
                    // thetvdb.com, so we need to smartly merge
                    // in a <Series> if we already have some/all
                    // info on it (depending on which one came
                    // first).

                    SeriesInfo ^si = gcnew SeriesInfo(r->ReadSubtree());
                    if (Series->ContainsKey(si->TVDBCode))
                        Series[si->TVDBCode]->Merge(si, LanguagePriorityList);
                    else
                        Series[si->TVDBCode] = si;
                    r->Read();
                }
                else if (r->Name == "Episode")
                {
                    Episode ^e = gcnew Episode(nullptr, nullptr, r->ReadSubtree());
                    if (e->OK())
                    {
                        if (!Series->ContainsKey(e->SeriesID))
                        {
                            throw gcnew Exception("Can't find the series to add the episode to (TheTVDB).");
                        }
                        SeriesInfo ^ser = Series[e->SeriesID];
                        Season ^seas = ser->GetOrAddSeason(e->ReadSeasonNum,e->SeasonID);

                        bool added = false;
                        for (int i=0;i<seas->Episodes->Count;i++)
                        {
                            Episode ^ep = seas->Episodes[i];
                            if (ep->EpisodeID == e->EpisodeID)
                            {
                                seas->Episodes[i] = e;
                                added = true;
                                break;
                            }
                        }
                        if (!added)
                            seas->Episodes->Add(e);
                        e->SetSeriesSeason(ser, seas);
                    }
                    r->Read();
                }
                else if (r->Name == "xml")
                    r->Read();
                else if (r->Name == "Data")
                {
                    String ^time = r->GetAttribute("time");
                    if (time != nullptr)
                        New_Srv_Time = int::Parse(time);
                    
                    String ^lp = r->GetAttribute("TVRename_LanguagePriority");
                    if (lp != nullptr)
                    {
                        LanguagePriorityList->Clear();

                        for each (String ^s in lp->Split(' '))
                        {
                            if (s != "")
                              LanguagePriorityList->Add(s);
                        }
                    }

                    r->Read();
                }
                else
                    r->ReadOuterXml();
            }
            Unlock("ProcessTVDBResponse");
        }

        String ^PreferredLanguage(int seriesID)
        {
            if (!Series->ContainsKey(seriesID) || (Series[seriesID]->Language == ""))
            {
                // new series we don't know about, or don't have any language info
                SeriesInfo ^ser = DownloadSeriesNow(seriesID, false, true); // pretend we want "en", download overview
                if (ser == nullptr)
                    return "en";
                String ^name = ser->Name;
                ser = nullptr;
                ForgetShow(seriesID, true);

                // using the name found, search (which gives all languages)
                Search(name); // will find all languages available, and pick the "best"
            }

            if (!Series->ContainsKey(seriesID))
            {
                Diagnostics::Debug::Assert(Series->ContainsKey(seriesID));
                return "en"; // really shouldn't happen!
            }
            // and we have a language recorded for it
            SeriesInfo ^ser = Series[seriesID];
            if (ser->Language != "")
                return ser->Language; // return that language

            // otherwise, try for the user's top rated language
            if (LanguagePriorityList->Count > 0)
                return (LanguagePriorityList[0]);
            else
                return "en";
        }

        SeriesInfo ^DownloadSeriesNow(int code, bool episodesToo, bool forceEnglish)
        {
            String ^txt = "";
            if (Series->ContainsKey(code))
                txt = Series[code]->Name;
            else
                txt = "Code " + code.ToString();
            if (episodesToo)
                txt += " (Everything)";
            else
                txt += " Overview";
            Say(txt);

            String ^lang = forceEnglish ? "en" : PreferredLanguage(code);

            String ^url = episodesToo ? "series/"+code.ToString()+"/all/"+lang+".zip" :
                "series/"+code.ToString()+"/"+lang+".xml";

            array<unsigned char> ^p = episodesToo ? GetPageZIP(url, lang+".xml", true) : GetPage(url, true, tmXML);
            if (p == nullptr)
                return nullptr;

            MemoryStream ^ms = gcnew MemoryStream(p);

            ProcessTVDBResponse(ms);

            return (Series->ContainsKey(code)) ? Series[code] : nullptr;
        }
        bool DownloadEpisodeNow(int seriesID, int episodeID)
        {
            String ^txt = "";
            if (Series->ContainsKey(seriesID))
            {
                Episode ^ep = FindEpisodeByID(episodeID);
                String ^eptxt = "New Episode";
                if ((ep != nullptr) && (ep->TheSeason != nullptr))
                    eptxt = String::Format("S{0:00}E{1:00}",ep->TheSeason->SeasonNumber, ep->EpNum);

                txt = Series[seriesID]->Name + " ("+eptxt+")";
            }
            else
                return false; // shouldn't happen
            Say(txt);

            String ^url = "episodes/"+episodeID.ToString()+"/"+PreferredLanguage(seriesID)+".xml";

            array<unsigned char> ^p = GetPage(url, true, tmXML);
            
            if (p == nullptr)
                return false;

            MemoryStream ^ms = gcnew MemoryStream(p);

            ProcessTVDBResponse(ms);

            return true;
        }
        SeriesInfo ^MakePlaceholderSeries(int code, String ^name)
        {
            if (name == "")
                name = "";
            Series[code] = gcnew SeriesInfo(name, code);
            Series[code]->Dirty = true;
            return Series[code];
        }

        bool EnsureUpdated(int code)
        {
            if (!Series->ContainsKey(code) || (Series[code]->Seasons->Count == 0))
                return DownloadSeriesNow(code, true, false) != nullptr; // the whole lot!
            
            bool ok = true;
            
            if (Series[code]->Dirty)
                ok = (DownloadSeriesNow(code, false, false) != nullptr) && ok;

            for each (KeyValuePair<int, Season ^> ^kvp in Series[code]->Seasons)
            {
                Season ^seas = kvp->Value;
                for each (Episode ^e in seas->Episodes)
                    if (e->Dirty)
                        ExtraEpisodes->Add(gcnew ExtraEp(e->SeriesID, e->EpisodeID));
            }

            for each (ExtraEp ^ee in ExtraEpisodes)
                if (ee->SeriesID == code)
                    ok = DownloadEpisodeNow(ee->SeriesID, ee->EpisodeID) && ok;

            for (int i=ExtraEpisodes->Count - 1;i>=0;i--)
                if (ExtraEpisodes[i]->SeriesID == code)
                    ExtraEpisodes->RemoveAt(i);

            return ok;
        }

        void Search(String ^text)
        {
            // http://www.thetvdb.com/api/GetSeries.php?seriesname=prison
            // by default, english only.  add &language=all

            bool isNumber = Regex::Match(text,"^[0-9]+$")->Success;
            if (isNumber)
                DownloadSeriesNow(int::Parse(text),false, false);

            // but, the number could also be a name, so continue searching as usual

            array<unsigned char> ^p = GetPage("GetSeries.php?seriesname="+text+"&language=all",false, tmXML);

            if (p == nullptr)
                return;

            MemoryStream ^ms = gcnew MemoryStream(p);

            ProcessTVDBResponse(ms);
        }


        String ^WebsiteURL(int code, int seasid, bool summaryPage)
        {
            // Summary: http://www.thetvdb.com/?tab=series&id=75340&lid=7
            // Season 3: http://www.thetvdb.com/?tab=season&seriesid=75340&seasonid=28289&lid=7

            if (summaryPage || (seasid <= 0) || !Series->ContainsKey(code))
                return "http://www.thetvdb.com/?tab=series&id=" + code.ToString();
            else
                return "http://www.thetvdb.com/?tab=season&seriesid="+code.ToString()+"&seasonid="+seasid.ToString();
        }

        // Next episode to air of a given show		
        Episode ^NextAiring(int code)
        {
            if (!Series->ContainsKey(code) || (Series[code]->Seasons->Count == 0))
                return nullptr; // DownloadSeries(code, true);

            Episode ^next = nullptr;
            DateTime ^today = DateTime::Now;
            DateTime ^mostSoonAfterToday = DateTime(0);

            SeriesInfo ^ser = Series[code];
            for each (KeyValuePair<int, Season ^> ^kvp2 in ser->Seasons)
            {
                Season ^s = kvp2->Value;

                for each (Episode ^e in s->Episodes)
                {
                    DateTime ^dt = e->GetAirDateDT(true);
                    if (dt != nullptr)
                    {
                        if ((dt->CompareTo(today) > 0) && 
                            ( (mostSoonAfterToday->CompareTo(DateTime(0)) == 0) || (dt->CompareTo(mostSoonAfterToday ) < 0)))
                        {
                            mostSoonAfterToday = dt;
                            next = e;
                        }
                    }
                }
            }

            return next;
        }


    };
}

