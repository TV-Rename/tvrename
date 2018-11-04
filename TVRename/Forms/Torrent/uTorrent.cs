// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

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
        private readonly SetProgressDelegate SetProg;

        private readonly TVDoc mDoc;
        private System.IO.FileSystemWatcher watcher;

        public uTorrent(TVDoc doc, SetProgressDelegate progdel)
        {
            mDoc = doc;
            SetProg = progdel;

            InitializeComponent();

            watcher.Error += WatcherError;

            bool en = false;
            // are there any missing items in the to-do list?
            foreach (Item i in mDoc.TheActionList)
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

        private void UTSelectNone()
        {
            for (int i = 0; i < lbUTTorrents.Items.Count; i++)
                lbUTTorrents.SetItemChecked(i, false);
        }

        private void UTSelectAll()
        {
            for (int i = 0; i < lbUTTorrents.Items.Count; i++)
                lbUTTorrents.SetItemChecked(i, true);
        }

        private void bnUTAll_Click(object sender, EventArgs e)
        {
            UTSelectAll();
        }

        private void bnUTNone_Click(object sender, EventArgs e)
        {
            UTSelectNone();
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

            List<string> checkedItems = new List<string>();
            foreach (string torrent in lbUTTorrents.CheckedItems)
                checkedItems.Add(torrent);

            lbUTTorrents.Items.Clear();
            // open resume.dat file, fill checked list box with torrents available to choose from

            string file = TVSettings.Instance.ResumeDatPath;
            if (!File.Exists(file))
                return;
            BEncodeLoader bel = new BEncodeLoader();
            BTFile resumeDat = bel.Load(file);
            if (resumeDat == null)
                return;
            BTDictionary dict = resumeDat.GetDict();
            foreach (BTItem it in dict.Items)
            {
                if (it.Type == BTChunk.kDictionaryItem)
                {
                    BTDictionaryItem d2 = (BTDictionaryItem) (it);
                    if ((d2.Key != ".fileguard") && (d2.Data.Type == BTChunk.kDictionary))
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

            BTResume btp = new BTResume(SetProg, resumeDatFile);

            List<string> sl = new List<string>();

            foreach (string torrent in lbUTTorrents.CheckedItems)
                sl.Add(torrent);

            btp.DoWork(sl, searchFolder, lvUTResults, cbUTUseHashing.Checked, cbUTMatchMissing.Checked, cbUTSetPrio.Checked, 
                       testMode, chkUTSearchSubfolders.Checked, mDoc.TheActionList, TVSettings.Instance.FNPRegexs,
                       mDoc.Args);

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

        private void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            RefreshResumeDat();
        }

        private void WatcherError(object sender, System.IO.ErrorEventArgs e)
        {
            while (!watcher.EnableRaisingEvents)
            {
                try
                {
                    StartWatching();
                    RefreshResumeDat();
                }
                catch
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        private void StartWatching()
        {
            FileInfo f = new FileInfo(TVSettings.Instance.ResumeDatPath);
            if (f.Exists && f.Directory != null)
            {
                watcher.Path = f.Directory.Name;
                watcher.Filter = "resume.dat";
                watcher.EnableRaisingEvents = true;
            }
            else
                watcher.EnableRaisingEvents = false;
        }

        private void watcher_Created(object sender, System.IO.FileSystemEventArgs e)
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
