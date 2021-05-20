//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private bool finished;
        public bool Ready;

        private int pctAutoBulkAdd;
        private int pctLocalSearch;
        private int pctDownloadFolder;
        private int pctMediaLib;
        private int pctDownloading;
        private int pctuTorrent;
        private string? msg;

        public ScanProgress(bool autoBulkAdd, bool mediaLib, bool downloadFolder, bool searchLocal, bool downloading, bool rss)
        {
            Ready = false;
            finished = false;
            InitializeComponent();

            lbBulkAutoAdd.Enabled = autoBulkAdd;
            lbMediaLibrary.Enabled = mediaLib;
            lbDownloadFolder.Enabled = downloadFolder;
            lbSearchLocally.Enabled = searchLocal;
            lbCheckDownloading.Enabled = downloading;
            lbSearchRSS.Enabled = rss;
        }

        private void UpdateProg()
        {
            pbBulkAutoAdd.Value = pctAutoBulkAdd < 0 ? 0 : pctAutoBulkAdd > 100 ? 100 : pctAutoBulkAdd;
            pbBulkAutoAdd.Update();
            pbMediaLib.Value = pctMediaLib < 0 ? 0 : pctMediaLib > 100 ? 100 : pctMediaLib;
            pbMediaLib.Update();
            pbDownloadFolder.Value = pctDownloadFolder < 0 ? 0 : pctDownloadFolder > 100 ? 100 : pctDownloadFolder;
            pbDownloadFolder.Update();
            pbLocalSearch.Value = pctLocalSearch < 0 ? 0 : pctLocalSearch > 100 ? 100 : pctLocalSearch;
            pbLocalSearch.Update();
            pbRSS.Value = pctDownloading < 0 ? 0 : pctDownloading > 100 ? 100 : pctDownloading;
            pbRSS.Update();
            pbDownloading.Value = pctuTorrent < 0 ? 0 : pctuTorrent > 100 ? 100 : pctuTorrent;
            pbDownloading.Update();
            lblMessage.Text = msg;
        }

        public void MediaLibProg(int p, string message)
        {
            pctMediaLib = p;
            msg = message;
        }

        public void DownloadFolderProg(int p, string message)
        {
            pctDownloadFolder = p;
            msg = message;
        }

        public void LocalSearchProg(int p, string message)
        {
            pctLocalSearch = p;
            msg = message;
        }

        public void ToBeDownloadedProg(int p, string message)
        {
            pctDownloading = p;
            msg = message;
        }

        public void DownloadingProg(int p, string message)
        {
            pctuTorrent = p;
            msg = message;
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
            if (finished)
            {
                Close();
            }
        }

        public void Done()
        {
            finished = true;
        }

        public void AddNewProg(int p, string message)
        {
            pctAutoBulkAdd = p;
            msg = message;
        }
    }
}
