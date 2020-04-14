// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

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
            SetToDefaults();
        }

        public ShowRule([CanBeNull] XElement xmlSettings)
        {
            SetToDefaults();
            if (xmlSettings != null)
            {
                DoWhatNow = xmlSettings.ExtractEnum("DoWhatNow",RuleAction.kIgnoreEp);
                First = xmlSettings.ExtractInt("First",-1);
                Second = xmlSettings.ExtractInt("Second",-1);
                UserSuppliedText = xmlSettings.ExtractString("Text");
            }
        }

        public override string ToString()
        {
            return $"ShowRule: {ActionInWords()} with parameters {First}, {Second} and usertext: {UserSuppliedText}";
        }

        private void SetToDefaults()
        {
            DoWhatNow = RuleAction.kIgnoreEp;
            First = -1;
            Second = -1;
            UserSuppliedText = string.Empty;
        }

        public void WriteXml([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("Rule");
            writer.WriteElement("DoWhatNow",(int) DoWhatNow);
            writer.WriteElement("First",First);
            writer.WriteElement("Second",Second);           
            writer.WriteElement("Text",UserSuppliedText);
            writer.WriteEndElement(); // Rule
        }

        [NotNull]
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
}
