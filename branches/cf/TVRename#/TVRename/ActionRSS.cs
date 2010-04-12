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

    public class ActionRSS : ActionItem
    {
        public RSSItem RSS;
        public string TheFileNoExt;

        public ActionRSS(RSSItem rss, string toWhereNoExt, ProcessedEpisode pe)
            : base(ActionType.kRSS, pe)
        {
            this.PE = pe;
            this.RSS = rss;
            this.TheFileNoExt = toWhereNoExt;
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
            return this.RSS.Title;
        }

        public bool SameAs2(ActionRSS o)
        {
            return (o.RSS == this.RSS);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionRSS) (o));
        }

        public override int IconNumber()
        {
            return 6;
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

            lvi.SubItems.Add(this.TheFileNoExt);
            lvi.SubItems.Add(this.RSS.Title);

            lvi.Group = lv.Groups[4];
            lvi.Tag = this;

            // lv->Items->Add(lvi);
            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(this.RSS.URL);
                if ((r == null) || (r.Length == 0))
                {
                    this.HasError = true;
                    this.ErrorText = "No data downloaded";
                }

                string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + doc.FilenameFriendly(this.RSS.Title);
                if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                    saveTemp += ".torrent";
                File.WriteAllBytes(saveTemp, r);

                System.Diagnostics.Process.Start(doc.Settings.uTorrentPath, "/directory \"" + (new FileInfo(this.TheFileNoExt).Directory.FullName) + "\" \"" + saveTemp + "\"");

                this.HasError = false;
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.HasError = true;
            }
            this.Done = true;

            return !this.HasError;
        }
    }
}