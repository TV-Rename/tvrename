//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


// Per-season sets of rules for manipulating episodes from thetvdb into multi-episode files,
// removing, adding, swapping them around, etc.

using System.Xml;

namespace TVRename
{
    public enum RuleAction : int
    {
        kRemove,
        kSwap,
        kMerge,
        kInsert,
        kIgnoreEp,
        kRename,
        kSplit,
        kCollapse
    }

    public class ShowRule
    {
        public RuleAction DoWhatNow;
        public int First;
        public int Second;
        public string UserSuppliedText;

        public ShowRule()
        {
            SetToDefaults();
        }

        public void SetToDefaults()
        {
            DoWhatNow = RuleAction.kIgnoreEp;
            First = Second = -1;
            UserSuppliedText = "";
        }
        public ShowRule(XmlReader reader)
        {
            SetToDefaults();
            reader.Read();
            while (reader.Name != "Rule")
                return;

            reader.Read();
            while (reader.Name != "Rule")
            {
                if (reader.Name == "DoWhatNow")
                    DoWhatNow = (RuleAction)reader.ReadElementContentAsInt();
                else if (reader.Name == "First")
                    First = reader.ReadElementContentAsInt();
                else if (reader.Name == "Second")
                    Second = reader.ReadElementContentAsInt();
                else if (reader.Name == "Text")
                    UserSuppliedText = reader.ReadElementContentAsString();
                else
                    reader.ReadOuterXml();
            }
        }


        public ShowRule(ShowRule O)
        {
            DoWhatNow = O.DoWhatNow;
            First = O.First;
            Second = O.Second;
            UserSuppliedText = O.UserSuppliedText;
        }


        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Rule");
            writer.WriteStartElement("DoWhatNow");
            writer.WriteValue((int)DoWhatNow);
            writer.WriteEndElement();
            writer.WriteStartElement("First");
            writer.WriteValue(First);
            writer.WriteEndElement();
            writer.WriteStartElement("Second");
            writer.WriteValue(Second);
            writer.WriteEndElement();
            writer.WriteStartElement("Text");
            writer.WriteValue(UserSuppliedText);
            writer.WriteEndElement();
            writer.WriteEndElement(); // ShowRule
        }

        public string ActionInWords()
        {
            switch (DoWhatNow)
            {
                case RuleAction.kIgnoreEp:
                    return "Ignore";
                case RuleAction.kRemove:
                    return "Remove";
                case RuleAction.kCollapse:
                    return "Collapse";
                case RuleAction.kSwap:
                    return "Swap";
                case RuleAction.kMerge:
                    return "Merge";
                case RuleAction.kSplit:
                    return "Split";
                case RuleAction.kInsert:
                    return "Insert";
                case RuleAction.kRename:
                    return "Rename";
                default:
                    return "<Unknown>";
            }
        }

    }


} // namepsace