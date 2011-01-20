// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;
using System.IO;

namespace TVRename
{
    /// <summary>
    /// Summary for uTorrent
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class uTorrent : Form
    {
        private SetProgressDelegate SetProg;

        private TVDoc mDoc;
        private System.IO.FileSystemWatcher watcher;

        public uTorrent(TVDoc doc, SetProgressDelegate progdel)
        {
            this.mDoc = doc;
            this.SetProg = progdel;

            this.InitializeComponent();

            this.watcher.Error += this.WatcherError;

            bool en = false;
            // are there any missing items in the to-do list?
            foreach (Item i in this.mDoc.TheActionList)
            {
                if (i is ItemMissing)
                {
                    en = true;
                    break;
                }
            }
            this.cbUTMatchMissing.Enabled = en;
            this.EnableDisable();

            this.bnUTRefresh_Click(null, null);
        }

        private void bnUTRefresh_Click(object sender, System.EventArgs e)
        {
            this.RefreshResumeDat();
        }

        private void UTSelectNone()
        {
            for (int i = 0; i < this.lbUTTorrents.Items.Count; i++)
                this.lbUTTorrents.SetItemChecked(i, false);
        }

        private void UTSelectAll()
        {
            for (int i = 0; i < this.lbUTTorrents.Items.Count; i++)
                this.lbUTTorrents.SetItemChecked(i, true);
        }

        private void bnUTAll_Click(object sender, System.EventArgs e)
        {
            this.UTSelectAll();
        }

        private void bnUTNone_Click(object sender, System.EventArgs e)
        {
            this.UTSelectNone();
        }

        private bool CheckResumeDatPath()
        {
            if (string.IsNullOrEmpty(this.mDoc.Settings.ResumeDatPath) || !File.Exists(this.mDoc.Settings.ResumeDatPath))
            {
                MessageBox.Show("Please set the resume.dat path in Preferences before using this feature", "µTorrent", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private void RefreshResumeDat()
        {
            if (!this.CheckResumeDatPath())
                return;

            StringList checkedItems = new StringList();
            foreach (string torrent in this.lbUTTorrents.CheckedItems)
                checkedItems.Add(torrent);

            this.lbUTTorrents.Items.Clear();
            // open resume.dat file, fill checked list box with torrents available to choose from

            string file = this.mDoc.Settings.ResumeDatPath;
            if (!File.Exists(file))
                return;
            BEncodeLoader bel = new BEncodeLoader(mDoc.Args);
            BTFile resumeDat = bel.Load(file);
            if (resumeDat == null)
                return;
            BTDictionary dict = resumeDat.GetDict();
            for (int i = 0; i < dict.Items.Count; i++)
            {
                BTItem it = dict.Items[i];
                if (it.Type == BTChunk.kDictionaryItem)
                {
                    BTDictionaryItem d2 = (BTDictionaryItem) (it);
                    if ((d2.Key != ".fileguard") && (d2.Data.Type == BTChunk.kDictionary))
                        this.lbUTTorrents.Items.Add(d2.Key);
                }
            }

            foreach (string torrent in checkedItems)
            {
                for (int i = 0; i < this.lbUTTorrents.Items.Count; i++)
                {
                    if (this.lbUTTorrents.Items[i].ToString() == torrent)
                        this.lbUTTorrents.SetItemChecked(i, true);
                }
            }
        }

        private void bnUTGo_Click(object sender, System.EventArgs e)
        {
            if (!this.CheckResumeDatPath())
                return;

            string searchFolder = this.txtUTSearchFolder.Text;
            string resumeDatFile = this.mDoc.Settings.ResumeDatPath;
            bool testMode = this.chkUTTest.Checked;

            if (!File.Exists(resumeDatFile))
                return;

            if (!testMode && !CheckUTorrentClosed())
                return;
            //
            //				 int action = actNone;
            //				 if (chkUTSearchSubfolders->Checked)
            //					 action |=  actSearchSubfolders;
            //
            //
            //
            //				 if ( (action & (actRename | actCopy | actMatchMissing | actHashSearch)) == 0 )
            //					 return;
            //

            this.lvUTResults.Items.Clear();

            BTResume btp = new BTResume(this.SetProg, resumeDatFile);

            StringList sl = new StringList();

            foreach (string torrent in this.lbUTTorrents.CheckedItems)
                sl.Add(torrent);

            btp.DoWork(sl, searchFolder, this.lvUTResults, this.cbUTUseHashing.Checked, this.cbUTMatchMissing.Checked, this.cbUTSetPrio.Checked, 
                       testMode, this.chkUTSearchSubfolders.Checked, this.mDoc.TheActionList, this.mDoc.Settings.FNPRegexs,
                       mDoc.Args);

            if (!testMode)
                RestartUTorrent();
        }

        private void cbUTUseHashing_CheckedChanged(object sender, System.EventArgs e)
        {
            this.EnableDisable();
        }

        private void EnableDisable()
        {
            bool en = this.cbUTUseHashing.Checked;
            this.txtUTSearchFolder.Enabled = en;
            this.bnUTBrowseSearchFolder.Enabled = en;
            this.chkUTSearchSubfolders.Enabled = en;

            this.lbDPMatch.Enabled = this.cbUTSetPrio.Checked && this.cbUTUseHashing.Checked;
            this.lbDPMissing.Enabled = this.cbUTSetPrio.Checked && this.cbUTMatchMissing.Checked;
        }

        private static bool CheckUTorrentClosed()
        {
            DialogResult dr = MessageBox.Show("Make sure µTorrent is not running, then click OK.", "TVRename", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            return (dr == DialogResult.OK);
        }

        private static bool RestartUTorrent()
        {
            MessageBox.Show("You may now restart µTorrent.", "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        private void bnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void cbUTMatchMissing_CheckedChanged(object sender, System.EventArgs e)
        {
            this.EnableDisable();
        }

        private void cbUTSetPrio_CheckedChanged(object sender, System.EventArgs e)
        {
            this.EnableDisable();
        }

        private void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            this.RefreshResumeDat();
        }

        private void WatcherError(object UnnamedParameter1, System.IO.ErrorEventArgs UnnamedParameter2)
        {
            while (!this.watcher.EnableRaisingEvents)
            {
                try
                {
                    this.StartWatching();
                    this.RefreshResumeDat();
                }
                catch
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private void txtResumeDatFolder_TextChanged(object sender, System.EventArgs e)
        {
            this.StartWatching();
        }

        private void StartWatching()
        {
            FileInfo f = new FileInfo(this.mDoc.Settings.ResumeDatPath);
            if (f.Exists)
            {
                this.watcher.Path = f.Directory.Name;
                this.watcher.Filter = "resume.dat";
                this.watcher.EnableRaisingEvents = true;
            }
            else
                this.watcher.EnableRaisingEvents = false;
        }

        private void watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            this.RefreshResumeDat();
        }

        private void bnUTBrowseSearchFolder_Click(object sender, System.EventArgs e)
        {
            this.folderBrowser.SelectedPath = this.txtUTSearchFolder.Text;

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtUTSearchFolder.Text = this.folderBrowser.SelectedPath;
        }
    }
}