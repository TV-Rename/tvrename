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
        KRemove,
        KSwap,
        KMerge,
        KInsert,
        KIgnoreEp,
        KRename,
        KSplit,
        KCollapse
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
                    DoWhatNow = (RuleAction) reader.ReadElementContentAsInt();
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

        public ShowRule(ShowRule o)
        {
            DoWhatNow = o.DoWhatNow;
            First = o.First;
            Second = o.Second;
            UserSuppliedText = o.UserSuppliedText;
        }

        public void SetToDefaults()
        {
            DoWhatNow = RuleAction.KIgnoreEp;
            First = Second = -1;
            UserSuppliedText = "";
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("Rule");
            XMLHelper.WriteElementToXML(writer,"DoWhatNow",(int) DoWhatNow);
            XMLHelper.WriteElementToXML(writer,"First",First);
            XMLHelper.WriteElementToXML(writer,"Second",Second);           
            XMLHelper.WriteElementToXML(writer,"Text",UserSuppliedText);
            writer.WriteEndElement(); // Rule
        }

        public string ActionInWords()
        {
            switch (DoWhatNow)
            {
                case RuleAction.KIgnoreEp:
                    return "Ignore";
                case RuleAction.KRemove:
                    return "Remove";
                case RuleAction.KCollapse:
                    return "Collapse";
                case RuleAction.KSwap:
                    return "Swap";
                case RuleAction.KMerge:
                    return "Merge";
                case RuleAction.KSplit:
                    return "Split";
                case RuleAction.KInsert:
                    return "Insert";
                case RuleAction.KRename:
                    return "Rename";
                default:
                    return "<Unknown>";
            }
        }
    }
}
