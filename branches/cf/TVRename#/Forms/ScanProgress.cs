// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
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

        public ScanProgress(bool e1, bool e2, bool e3, bool e4, bool e5, bool e6)
        {
            this.Ready = false;
            this.Finished = false;
            this.InitializeComponent();

            this.lb1.Enabled = e1;
            this.lb3.Enabled = e3;
            this.lb4.Enabled = e4;
            this.lb5.Enabled = e5;
        }

        public void UpdateProg()
        {
            this.pbMediaLib.Value = ((this.pctMediaLib < 0) ? 0 : ((this.pctMediaLib > 100) ? 100 : this.pctMediaLib));
            this.pbMediaLib.Update();
            this.pbLocalSearch.Value = ((this.pctLocalSearch < 0) ? 0 : ((this.pctLocalSearch > 100) ? 100 : this.pctLocalSearch));
            this.pbLocalSearch.Update();
            this.pbRSS.Value = ((this.pctRSS < 0) ? 0 : ((this.pctRSS > 100) ? 100 : this.pctRSS));
            this.pbRSS.Update();
            this.pbuTorrent.Value = ((this.pctuTorrent < 0) ? 0 : ((this.pctuTorrent > 100) ? 100 : this.pctuTorrent));
            this.pbuTorrent.Update();
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

        public void uTorrentProg(int p)
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