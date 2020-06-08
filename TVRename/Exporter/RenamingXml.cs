// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

namespace TVRename
{
    internal class RenamingXml : ActionListXml
    {
        public RenamingXml(ItemList theActionList) : base(theActionList)
        {
        }

        protected override bool IsOutput(Item a) => a is ActionCopyMoveRename cmr && cmr.Operation == ActionCopyMoveRename.Op.rename;
        public override bool ApplicableFor(TVSettings.ScanType st) => true;

        public override bool Active() => TVSettings.Instance.ExportRenamingXML;
        protected override string Location() => TVSettings.Instance.ExportRenamingXMLTo;
        protected override string MainXmlElementName() => "Renaming";
    }
}
