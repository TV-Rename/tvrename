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
		public bool Ready;
		public bool Finished;

		public int pctMediaLib;
		public int pctLocalSearch;
		public int pctRSS;
		public int pctuTorrent;

	

		public ScanProgress(bool e1, bool e2, bool e3, bool e4, bool e5, bool e6)
		{
			Ready = false;
			Finished = false;
			InitializeComponent();

			lb1.Enabled = e1;
			lb3.Enabled = e3;
			lb4.Enabled = e4;
			lb5.Enabled = e5;
		}

        public void UpdateProg()
        {
            pbMediaLib.Value = ((pctMediaLib < 0) ? 0 : ((pctMediaLib > 100) ? 100 : pctMediaLib));
            pbMediaLib.Update();
            pbLocalSearch.Value = ((pctLocalSearch<0) ? 0 : ((pctLocalSearch>100) ? 100 : pctLocalSearch));
            pbLocalSearch.Update();
            pbRSS.Value = ((pctRSS<0) ? 0 : ((pctRSS>100) ? 100 : pctRSS));
            pbRSS.Update();
            pbuTorrent.Value = ((pctuTorrent<0) ? 0 : ((pctuTorrent>100) ? 100 : pctuTorrent));
            pbuTorrent.Update();
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

        public void uTorrentProg(int p)
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
                this.Close();
        }
        public void Done()
        {
            Finished = true;
        }
    }
}