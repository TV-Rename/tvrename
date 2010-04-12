// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public class ActionMissing : ActionItem
    {
        public string TheFileNoExt;

        public ActionMissing(ProcessedEpisode pe, string whereItShouldBeNoExt)
            : base(ActionType.kMissing, pe)
        {
            this.TheFileNoExt = whereItShouldBeNoExt;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(this.TheFileNoExt))
                return null;
            return new IgnoreItem(this.TheFileNoExt);
        }

        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(this.TheFileNoExt))
                return null;
            return new FileInfo(this.TheFileNoExt).DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.PE.Name;
        }

        public override bool Action(TVDoc doc)
        {
            return true; // return success, but don't set as Done
        }

        public override int IconNumber()
        {
            return 1;
        }

        public bool SameAs2(ActionMissing o)
        {
            return string.Compare(o.TheFileNoExt, this.TheFileNoExt) == 0;
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionMissing) (o));
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = this.PE.SI.ShowName();
            lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
            lvi.SubItems.Add(this.PE.NumsAsString());

            DateTime? dt = this.PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            FileInfo fi = new FileInfo(this.TheFileNoExt);
            lvi.SubItems.Add(fi.DirectoryName);
            lvi.SubItems.Add(fi.Name);

            lvi.Tag = this;
            lvi.Group = lv.Groups[0];

            //lv->Items->Add(lvi);
            return lvi;
        }
    }
}