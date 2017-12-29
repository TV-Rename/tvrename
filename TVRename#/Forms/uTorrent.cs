// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using File = Alphaleonis.Win32.Filesystem.File;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

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
    public partial class UTorrent : Form
    {
        private readonly SetProgressDelegate _setProg;

        private readonly TVDoc _mDoc;
        private FileSystemWatcher _watcher;

        public UTorrent(TVDoc doc, SetProgressDelegate progdel)
        {
            _mDoc = doc;
            _setProg = progdel;

            InitializeComponent();

            _watcher.Error += WatcherError;

            bool en = false;
            // are there any missing items in the to-do list?
            foreach (Item i in _mDoc.TheActionList)
            {
                if (i is ItemMissing)
                {
                    en = true;
                    break;
                }
            }
            cbUTMatchMissing.Enabled = en;
            EnableDisable();

            bnUTRefresh_Click(null, null);
        }

        private void bnUTRefresh_Click(object sender, EventArgs e)
        {
            RefreshResumeDat();
        }

        private void UtSelectNone()
        {
            for (int i = 0; i < lbUTTorrents.Items.Count; i++)
                lbUTTorrents.SetItemChecked(i, false);
        }

        private void UtSelectAll()
        {
            for (int i = 0; i < lbUTTorrents.Items.Count; i++)
                lbUTTorrents.SetItemChecked(i, true);
        }

        private void bnUTAll_Click(object sender, EventArgs e)
        {
            UtSelectAll();
        }

        private void bnUTNone_Click(object sender, EventArgs e)
        {
            UtSelectNone();
        }

        private bool CheckResumeDatPath()
        {
            if (string.IsNullOrEmpty(TVSettings.Instance.ResumeDatPath) || !File.Exists(TVSettings.Instance.ResumeDatPath))
            {
                MessageBox.Show("Please set the resume.dat path in Preferences before using this feature", "µTorrent", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private void RefreshResumeDat()
        {
            if (!CheckResumeDatPath())
                return;

            List<string> checkedItems = new List<String>();
            foreach (string torrent in lbUTTorrents.CheckedItems)
                checkedItems.Add(torrent);

            lbUTTorrents.Items.Clear();
            // open resume.dat file, fill checked list box with torrents available to choose from

            string file = TVSettings.Instance.ResumeDatPath;
            if (!File.Exists(file))
                return;
            BEncodeLoader bel = new BEncodeLoader();
            BtFile resumeDat = bel.Load(file);
            if (resumeDat == null)
                return;
            BtDictionary dict = resumeDat.GetDict();
            for (int i = 0; i < dict.Items.Count; i++)
            {
                BtItem it = dict.Items[i];
                if (it.Type == BtChunk.KDictionaryItem)
                {
                    BtDictionaryItem d2 = (BtDictionaryItem) (it);
                    if ((d2.Key != ".fileguard") && (d2.Data.Type == BtChunk.KDictionary))
                        lbUTTorrents.Items.Add(d2.Key);
                }
            }

            foreach (string torrent in checkedItems)
            {
                for (int i = 0; i < lbUTTorrents.Items.Count; i++)
                {
                    if (lbUTTorrents.Items[i].ToString() == torrent)
                        lbUTTorrents.SetItemChecked(i, true);
                }
            }
        }

        private void bnUTGo_Click(object sender, EventArgs e)
        {
            if (!CheckResumeDatPath())
                return;

            string searchFolder = txtUTSearchFolder.Text;
            string resumeDatFile = TVSettings.Instance.ResumeDatPath;
            bool testMode = chkUTTest.Checked;

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

            lvUTResults.Items.Clear();

            BtResume btp = new BtResume(_setProg, resumeDatFile);

            List<string> sl = new List<String>();

            foreach (string torrent in lbUTTorrents.CheckedItems)
                sl.Add(torrent);

            btp.DoWork(sl, searchFolder, lvUTResults, cbUTUseHashing.Checked, cbUTMatchMissing.Checked, cbUTSetPrio.Checked, 
                       testMode, chkUTSearchSubfolders.Checked, _mDoc.TheActionList, TVSettings.Instance.FnpRegexs,
                       _mDoc.Args);

            if (!testMode)
                RestartUTorrent();
        }

        private void cbUTUseHashing_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void EnableDisable()
        {
            bool en = cbUTUseHashing.Checked;
            txtUTSearchFolder.Enabled = en;
            bnUTBrowseSearchFolder.Enabled = en;
            chkUTSearchSubfolders.Enabled = en;

            lbDPMatch.Enabled = cbUTSetPrio.Checked && cbUTUseHashing.Checked;
            lbDPMissing.Enabled = cbUTSetPrio.Checked && cbUTMatchMissing.Checked;
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

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbUTMatchMissing_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void cbUTSetPrio_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            RefreshResumeDat();
        }

        private void WatcherError(object unnamedParameter1, ErrorEventArgs unnamedParameter2)
        {
            while (!_watcher.EnableRaisingEvents)
            {
                try
                {
                    StartWatching();
                    RefreshResumeDat();
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }

        private void StartWatching()
        {
            FileInfo f = new FileInfo(TVSettings.Instance.ResumeDatPath);
            if (f.Exists && f.Directory != null)
            {
                _watcher.Path = f.Directory.Name;
                _watcher.Filter = "resume.dat";
                _watcher.EnableRaisingEvents = true;
            }
            else
                _watcher.EnableRaisingEvents = false;
        }

        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            RefreshResumeDat();
        }

        private void bnUTBrowseSearchFolder_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = txtUTSearchFolder.Text;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtUTSearchFolder.Text = folderBrowser.SelectedPath;
        }
    }
}
