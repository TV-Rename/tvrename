//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Xml;
using System.Xml.Linq;

// Per-season sets of rules for manipulating episodes from thetvdb into multi-episode files,
// removing, adding, swapping them around, etc.

namespace TVRename
{
    public class ShowRule
    {
        public RuleAction DoWhatNow;
        public int First;
        public int Second;
        public string UserSuppliedText;
        public bool RenumberAfter;

        public ShowRule()
        {
            DoWhatNow = RuleAction.kIgnoreEp;
            First = -1;
            Second = -1;
            UserSuppliedText = string.Empty;
            RenumberAfter = true;
        }

        public ShowRule(XElement? xmlSettings) : this()
        {
            if (xmlSettings != null)
            {
                DoWhatNow = xmlSettings.ExtractEnum("DoWhatNow", RuleAction.kIgnoreEp);
                First = xmlSettings.ExtractInt("First", -1);
                Second = xmlSettings.ExtractInt("Second", -1);
                UserSuppliedText = xmlSettings.ExtractString("Text");
                RenumberAfter = xmlSettings.ExtractBool("RenumberAfter", true);
            }
        }

        public override string ToString()
        {
            return $"ShowRule: {ActionInWords()} with parameters {First}, {Second} and usertext: {UserSuppliedText} ({RenumberAfter})";
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Rule");
            writer.WriteElement("DoWhatNow", (int)DoWhatNow);
            writer.WriteElement("First", First);
            writer.WriteElement("Second", Second);
            writer.WriteElement("Text", UserSuppliedText);
            writer.WriteElement("RenumberAfter", RenumberAfter);
            writer.WriteEndElement(); // Rule
        }

        public string ActionInWords()
        {
            return DoWhatNow switch
            {
                RuleAction.kIgnoreEp => "Ignore",
                RuleAction.kRemove => "Remove",
                RuleAction.kCollapse => "Collapse",
                RuleAction.kSwap => "Swap",
                RuleAction.kMerge => "Merge",
                RuleAction.kSplit => "Split",
                RuleAction.kInsert => "Insert",
                RuleAction.kRename => "Rename",
                _ => "<Unknown>"
            };
        }
    }
}
