#pragma once

using namespace System::Text::RegularExpressions;
using namespace System::Collections;
using namespace System::IO;
using namespace System::Windows::Forms;
using namespace System;
using namespace System::Xml;

namespace TVRename
{
    enum RuleAction { kRemove, kSwap, kMerge, kInsert, kIgnoreEp, kRename, kSplit, kCollapse };

    public ref class ShowRule
    {
    public:

        RuleAction DoWhatNow;
        int First;
        int Second;
        String ^UserSuppliedText;

        ShowRule()
        {
            SetToDefaults();
        }

        void SetToDefaults()
        {
            DoWhatNow = kIgnoreEp;
            First = Second = -1;
            UserSuppliedText = "";
        }
        ShowRule(XmlReader ^reader)
        {
            SetToDefaults();
            reader->Read();
            while (reader->Name != "Rule")
                return;

            reader->Read();
            while (reader->Name != "Rule")
            {
                if (reader->Name == "DoWhatNow")
                    DoWhatNow = (RuleAction)reader->ReadElementContentAsInt();
                else if (reader->Name == "First")
                    First = reader->ReadElementContentAsInt();
                else if (reader->Name == "Second")
                    Second = reader->ReadElementContentAsInt();
                else if (reader->Name == "Text")
                    UserSuppliedText = reader->ReadElementContentAsString();
                else
                    reader->ReadOuterXml();
            }
        }


        ShowRule(ShowRule ^O)
        {
            DoWhatNow = O->DoWhatNow;
            First = O->First;
            Second = O->Second;
            UserSuppliedText = O->UserSuppliedText;
        }


        void WriteXML(XmlWriter ^writer)
        {
            writer->WriteStartElement("Rule");
            writer->WriteStartElement("DoWhatNow");
            writer->WriteValue((int)DoWhatNow);
            writer->WriteEndElement();
            writer->WriteStartElement("First");
            writer->WriteValue(First);
            writer->WriteEndElement();
            writer->WriteStartElement("Second");
            writer->WriteValue(Second);
            writer->WriteEndElement();
            writer->WriteStartElement("Text");
            writer->WriteValue(UserSuppliedText);
            writer->WriteEndElement();
            writer->WriteEndElement(); // ShowRule
        }

        String ^ActionInWords()
        {
            switch (DoWhatNow)
            {
            case kIgnoreEp: return "Ignore";
            case kRemove: return "Remove";
                case kCollapse: return "Collapse";
            case kSwap: return "Swap";
            case kMerge: return "Merge";
            case kSplit: return "Split";
            case kInsert: return "Insert";
            case kRename: return "Rename";
            default: return "<Unknown>";
            }
        }

    };

    typedef Generic::List<ShowRule ^> RuleSet;
    typedef Generic::Dictionary<int, RuleSet ^> RuleDict;

} // namepsace
