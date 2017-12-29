// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
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

        public int PctLocalSearch;
        public int PctMediaLib;
        public int PctRss;
        public int PctuTorrent;

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
            pbMediaLib.Value = ((PctMediaLib < 0) ? 0 : ((PctMediaLib > 100) ? 100 : PctMediaLib));
            pbMediaLib.Update();
            pbLocalSearch.Value = ((PctLocalSearch < 0) ? 0 : ((PctLocalSearch > 100) ? 100 : PctLocalSearch));
            pbLocalSearch.Update();
            pbRSS.Value = ((PctRss < 0) ? 0 : ((PctRss > 100) ? 100 : PctRss));
            pbRSS.Update();
            pbDownloading.Value = ((PctuTorrent < 0) ? 0 : ((PctuTorrent > 100) ? 100 : PctuTorrent));
            pbDownloading.Update();
        }

        public void MediaLibProg(int p)
        {
            PctMediaLib = p;
        }

        public void LocalSearchProg(int p)
        {
            PctLocalSearch = p;
        }

        public void RssProg(int p)
        {
            PctRss = p;
        }

        public void DownloadingProg(int p)
        {
            PctuTorrent = p;
        }

        private void ScanProgress_Load(object sender, EventArgs e)
        {
            Ready = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
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
