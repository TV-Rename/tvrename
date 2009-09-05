//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
#pragma once

#include "Settings.h"

using namespace System::Xml;
using namespace System;
using namespace System::IO;

namespace TVRename
{
    public ref class RSSItem
    {
    public:
        String ^URL;
        int Season;
        int Episode;
        String ^Title;
        String ^ShowName;

        RSSItem(String ^url, String ^title, int season, int episode, String ^showName)
        {
            URL = url;
            Season = season;
            Episode = episode;
            Title = title;
            ShowName = showName;
        }
    };

    public ref class RSSItemList : public System::Collections::Generic::List<RSSItem ^>
    {
    private:
        FNPRegexList ^Rexps; // only trustable while in DownloadRSS or its called functions

    public: bool DownloadRSS(String ^URL, FNPRegexList ^rexps)
        {
            Rexps = rexps;

            System::Net::WebClient ^wc = gcnew System::Net::WebClient();
            try {
                array<unsigned char> ^r = wc->DownloadData(URL);

                MemoryStream ^ms = gcnew MemoryStream(r);

                XmlReaderSettings ^settings = gcnew XmlReaderSettings();
                settings->IgnoreComments = true;
                settings->IgnoreWhitespace = true;
                XmlReader ^reader = XmlReader::Create(ms, settings);

                reader->Read();
                if (reader->Name != "xml")
                    return false;

                reader->Read();

                if (reader->Name != "rss") 
                    return false;

                reader->Read();

                while (!reader->EOF)
                {
                    if ((reader->Name == "rss") && (!reader->IsStartElement()))
                        break;

                    if (reader->Name == "channel")
                    {
                        if (!ReadChannel(reader->ReadSubtree()))
                            return false;
                        reader->Read();
                    }
                    else 
                        reader->ReadOuterXml();

                }

                ms->Close();

            }
            catch (...)
            {
                return false;
            }
            finally
            {
                Rexps = nullptr;
            }

            return true;
        }

    private: bool ReadChannel(XmlReader ^r)
        {
            r->Read();
            r->Read();
            while (!r->EOF)
            {
                if ((r->Name == "channel") && (!r->IsStartElement()))
                    break;
                if (r->Name == "item")
                {
                    if (!ReadItem(r->ReadSubtree()))
                        return false;
                    r->Read();
                }
                else 
                    r->ReadOuterXml();
            }
            return true;
        }

    private: bool ReadItem(XmlReader ^r);


    };


	
    // ref class MissingEpisode;
/*
    public ref class RSSMissingItem      ------------> becomes AIORSS
    {
    public:
        RSSItem ^RSS;
        MissingEpisode ^Episode;

        RSSMissingItem(RSSItem ^rss, MissingEpisode ^ep)
        {
            RSS = rss;
            Episode = ep;
        }
    };

    typedef System::Collections::Generic::List<RSSMissingItem ^> RSSMissingItemList;
*/



} // namespace




