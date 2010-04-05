// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Xml;

// Per-season sets of rules for manipulating episodes from thetvdb into multi-episode files,
// removing, adding, swapping them around, etc.

namespace TVRename
{
    public enum RuleAction
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
            this.SetToDefaults();
        }

        public ShowRule(XmlReader reader)
        {
            this.SetToDefaults();
            reader.Read();
            while (reader.Name != "Rule")
                return;

            reader.Read();
            while (reader.Name != "Rule")
            {
                if (reader.Name == "DoWhatNow")
                    this.DoWhatNow = (RuleAction) reader.ReadElementContentAsInt();
                else if (reader.Name == "First")
                    this.First = reader.ReadElementContentAsInt();
                else if (reader.Name == "Second")
                    this.Second = reader.ReadElementContentAsInt();
                else if (reader.Name == "Text")
                    this.UserSuppliedText = reader.ReadElementContentAsString();
                else
                    reader.ReadOuterXml();
            }
        }

        public ShowRule(ShowRule O)
        {
            this.DoWhatNow = O.DoWhatNow;
            this.First = O.First;
            this.Second = O.Second;
            this.UserSuppliedText = O.UserSuppliedText;
        }

        public void SetToDefaults()
        {
            this.DoWhatNow = RuleAction.kIgnoreEp;
            this.First = this.Second = -1;
            this.UserSuppliedText = "";
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Rule");
            writer.WriteStartElement("DoWhatNow");
            writer.WriteValue((int) this.DoWhatNow);
            writer.WriteEndElement();
            writer.WriteStartElement("First");
            writer.WriteValue(this.First);
            writer.WriteEndElement();
            writer.WriteStartElement("Second");
            writer.WriteValue(this.Second);
            writer.WriteEndElement();
            writer.WriteStartElement("Text");
            writer.WriteValue(this.UserSuppliedText);
            writer.WriteEndElement();
            writer.WriteEndElement(); // ShowRule
        }

        public string ActionInWords()
        {
            switch (this.DoWhatNow)
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
}
