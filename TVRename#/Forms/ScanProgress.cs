// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for ScanProgress
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class ScanProgress : Form
    {
        public bool Finished;
        public bool Ready;

        public int pctLocalSearch;
        public int pctMediaLib;
        public int pctRSS;
        public int pctuTorrent;

        public ScanProgress(bool mediaLib, bool searchLocal, bool downloading, bool rss)
        {
            this.Ready = false;
            this.Finished = false;
            this.InitializeComponent();

            this.lbMediaLibrary.Enabled = mediaLib;
            this.lbSearchLocally.Enabled = searchLocal;
            this.lbCheckDownloading.Enabled = downloading;
            this.lbSearchRSS.Enabled = rss;
        }

        public void UpdateProg()
        {
            this.pbMediaLib.Value = ((this.pctMediaLib < 0) ? 0 : ((this.pctMediaLib > 100) ? 100 : this.pctMediaLib));
            this.pbMediaLib.Update();
            this.pbLocalSearch.Value = ((this.pctLocalSearch < 0) ? 0 : ((this.pctLocalSearch > 100) ? 100 : this.pctLocalSearch));
            this.pbLocalSearch.Update();
            this.pbRSS.Value = ((this.pctRSS < 0) ? 0 : ((this.pctRSS > 100) ? 100 : this.pctRSS));
            this.pbRSS.Update();
            this.pbDownloading.Value = ((this.pctuTorrent < 0) ? 0 : ((this.pctuTorrent > 100) ? 100 : this.pctuTorrent));
            this.pbDownloading.Update();
        }

        public void MediaLibProg(int p)
        {
            this.pctMediaLib = p;
        }

        public void LocalSearchProg(int p)
        {
            this.pctLocalSearch = p;
        }

        public void RSSProg(int p)
        {
            this.pctRSS = p;
        }

        public void DownloadingProg(int p)
        {
            this.pctuTorrent = p;
        }

        private void ScanProgress_Load(object sender, System.EventArgs e)
        {
            this.Ready = true;
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            this.UpdateProg();
            this.timer1.Start();
            if (this.Finished)
                this.Close();
        }

        public void Done()
        {
            this.Finished = true;
        }
    }
}
