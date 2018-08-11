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
            Ready = false;
            Finished = false;
            InitializeComponent();

            lbMediaLibrary.Enabled = mediaLib;
            lbSearchLocally.Enabled = searchLocal;
            lbCheckDownloading.Enabled = downloading;
            lbSearchRSS.Enabled = rss;
        }

        public void UpdateProg()
        {
            pbMediaLib.Value = ((pctMediaLib < 0) ? 0 : ((pctMediaLib > 100) ? 100 : pctMediaLib));
            pbMediaLib.Update();
            pbLocalSearch.Value = ((pctLocalSearch < 0) ? 0 : ((pctLocalSearch > 100) ? 100 : pctLocalSearch));
            pbLocalSearch.Update();
            pbRSS.Value = ((pctRSS < 0) ? 0 : ((pctRSS > 100) ? 100 : pctRSS));
            pbRSS.Update();
            pbDownloading.Value = ((pctuTorrent < 0) ? 0 : ((pctuTorrent > 100) ? 100 : pctuTorrent));
            pbDownloading.Update();
        }

        public void MediaLibProg(int p)
        {
            pctMediaLib = p;
        }

        public void LocalSearchProg(int p)
        {
            pctLocalSearch = p;
        }

        public void RSSProg(int p)
        {
            pctRSS = p;
        }

        public void DownloadingProg(int p)
        {
            pctuTorrent = p;
        }

        private void ScanProgress_Load(object sender, System.EventArgs e)
        {
            Ready = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            UpdateProg();
            timer1.Start();
            if (Finished)
                Close();
        }

        public void Done()
        {
            Finished = true;
        }
    }
}
