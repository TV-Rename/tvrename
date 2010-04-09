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

    public class TorrentEntry
    {
        public string DownloadingTo;
        public int PercentDone;
        public string TorrentFile;

        public TorrentEntry(string torrentfile, string to, int percent)
        {
            this.TorrentFile = torrentfile;
            this.DownloadingTo = to;
            this.PercentDone = percent;
        }
    }

    public class ActionuTorrenting : ActionItem
    {
        public string DesiredLocationNoExt;
        public TorrentEntry Entry;

        public ActionuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt)
            : base(ActionType.kuTorrenting, pe)
        {
            this.DesiredLocationNoExt = desiredLocationNoExt;
            this.Entry = te;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(this.DesiredLocationNoExt))
                return null;
            return new IgnoreItem(this.DesiredLocationNoExt);
        }

        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(this.Entry.DownloadingTo))
                return null;
            return new FileInfo(this.Entry.DownloadingTo).DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return "";
        }

        public bool SameAs2(ActionuTorrenting o)
        {
            return (o.Entry == this.Entry);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionuTorrenting) (o));
        }

        public override int IconNumber()
        {
            return 2;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = this.PE.SI.ShowName();
            lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
            lvi.SubItems.Add(this.PE.NumsAsString());
            DateTime? dt = this.PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(this.Entry.TorrentFile);
            lvi.SubItems.Add(this.Entry.DownloadingTo);
            int p = this.Entry.PercentDone;
            lvi.SubItems.Add(p == -1 ? "" : this.Entry.PercentDone + "% Complete");

            lvi.Group = lv.Groups[7];
            lvi.Tag = this;

            //	lv->Items->Add(lvi);
            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            this.Done = true;
            return true;
        }
    }
}