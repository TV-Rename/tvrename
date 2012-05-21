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

        public string PlainEnglishDescription()
        {
            switch (this.DoWhatNow)
            {
                case RuleAction.kIgnoreEp:
                case RuleAction.kRemove:
                    return this.Second == -1
                               ? string.Format("{0} episode {1}.", this.ActionInWords(), this.First)
                               : string.Format("{0} episodes {1} through {2}.", this.ActionInWords(), this.First, this.Second);
                case RuleAction.kSwap:
                    return string.Format("{0} episode {1} with {2}.", this.ActionInWords(), this.First, this.Second);
                case RuleAction.kCollapse:
                case RuleAction.kMerge:
                    return string.IsNullOrEmpty(this.UserSuppliedText)
                        ? string.Format("{0} episodes {1} through {2}.", this.ActionInWords(), this.First, this.Second)
                        : string.Format("{0} episodes {1} through {2} and rename to '{3}'.", this.ActionInWords(), this.First, this.Second, this.UserSuppliedText);
                case RuleAction.kSplit:
                    return string.Format("{0} episode {1} into {2} parts.", this.ActionInWords(), this.First, this.Second);
                case RuleAction.kInsert:
                    return string.Format("{0} new episode '{1}' in position {2}.", this.ActionInWords(), this.UserSuppliedText, this.First);
                case RuleAction.kRename:
                    return string.Format("{0} Episode {1} to '{2}'.", this.ActionInWords(), this.First, this.UserSuppliedText);
                default:
                    return "<Unknown>";
            }
        }
    }
}
