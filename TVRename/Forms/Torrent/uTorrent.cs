// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Linq;
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
    // ReSharper disable once InconsistentNaming
    public partial class uTorrent : Form
    {
        private readonly SetProgressDelegate setProg;
        private readonly TVDoc mDoc;
        private System.IO.FileSystemWatcher watcher;
        private readonly Dictionary<string, string> resumeDats = new Dictionary<string, string>();

        public uTorrent(TVDoc doc, SetProgressDelegate progdel)
        {
            mDoc = doc;
            setProg = progdel;

            InitializeComponent();

            watcher.Error += WatcherError;

            // are there any missing items in the to-do list?
            cbUTMatchMissing.Enabled = mDoc.TheActionList.MissingItems().Any();
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

        private void bnUTAll_Click(object sender, EventArgs e) => UtSelectAll();

        private void bnUTNone_Click(object sender, EventArgs e) => UtSelectNone();

        private static bool CheckResumeDatPath()
        {
            if (!string.IsNullOrEmpty(TVSettings.Instance.ResumeDatPath) &&
                File.Exists(TVSettings.Instance.ResumeDatPath)) return true;

            MessageBox.Show("Please set the resume.dat path in Preferences before using this feature", "µTorrent",
                MessageBoxButtons.OK);

            return false;
        }

        private void RefreshResumeDat()
        {
            if (!CheckResumeDatPath())
                return;

            if (Directory.Exists(Path.Combine(Path.GetDirectoryName(TVSettings.Instance.ResumeDatPath), "resume_dir")))
            {
                SetupResumeDats();
                return;
            }

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
            foreach (BTDictionaryItem d2 in dict.Items)
            {

                    if ((d2.Key != ".fileguard") && (d2.Data.Type == BTChunk.kDictionary))
                        lbUTTorrents.Items.Add(d2.Key);
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

        private void SetupResumeDats()
        {
            List<string> checkedItems = new List<string>();
            foreach (string torrent in lbUTTorrents.CheckedItems)
                checkedItems.Add(torrent);

            lbUTTorrents.Items.Clear();

            string resumeDir = Path.Combine(Path.GetDirectoryName(TVSettings.Instance.ResumeDatPath), "resume_dir");

            resumeDats.Clear();

            string[] files = Directory.GetFiles(resumeDir, "*.dat");
            foreach (string file in files)
            {
                BEncodeLoader bel = new BEncodeLoader();
                BTFile resumeDat = bel.Load(file);
                if (resumeDat == null)
                    break;

                BTDictionary dict = resumeDat.GetDict();
                foreach (BTDictionaryItem d2 in dict.Items)
                {
                    if ((d2.Key == ".fileguard") || (d2.Data.Type != BTChunk.kDictionary)) continue;
                    resumeDats.Add(Path.GetFileNameWithoutExtension(file), d2.Key);
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

            if (Directory.Exists(Path.Combine(Path.GetDirectoryName(TVSettings.Instance.ResumeDatPath), "resume_dir")))
            {
                UpdateResumeDats();
                return;
            }

            string searchFolder = txtUTSearchFolder.Text;
            string resumeDatFile = TVSettings.Instance.ResumeDatPath;
            bool testMode = chkUTTest.Checked;

            if (!File.Exists(resumeDatFile))
                return;

            if (!testMode && !CheckUTorrentClosed())
                return;

            lvUTResults.Items.Clear();

            BTResume btp = new BTResume(setProg, resumeDatFile);

            List<string> sl = new List<string>();

            foreach (string torrent in lbUTTorrents.CheckedItems)
                sl.Add(Path.Combine(Path.GetDirectoryName(resumeDatFile), torrent));

            btp.DoWork(sl, searchFolder, lvUTResults, cbUTUseHashing.Checked, cbUTMatchMissing.Checked,
                cbUTSetPrio.Checked,
                testMode, chkUTSearchSubfolders.Checked, mDoc.TheActionList, TVSettings.Instance.FNPRegexs,
                mDoc.Args);

            if (!testMode)
                RestartUTorrent();
        }

        private void UpdateResumeDats()
        {
            string searchFolder = txtUTSearchFolder.Text;
            string resumeDir = Path.Combine(Path.GetDirectoryName(TVSettings.Instance.ResumeDatPath), "resume_dir");
            bool testMode = chkUTTest.Checked;

            if (!Directory.Exists(resumeDir))
                return;

            if (!testMode && !CheckUTorrentClosed())
                return;

            lvUTResults.Items.Clear();

            List<string> sl = new List<string>();
            List<string> files = new List<string>();

            foreach (string torrent in lbUTTorrents.CheckedItems)
            {
                sl.Add(Path.Combine(Path.GetDirectoryName(TVSettings.Instance.ResumeDatPath), torrent));
                foreach (KeyValuePair<string, string> pair in resumeDats)
                {
                    if (pair.Value == torrent)
                    {
                        files.Add(Path.Combine(resumeDir, pair.Key) + ".dat");
                    }
                }
            }

            foreach (string resumeDatFile in files)
            {
                BTResume btp = new BTResume(setProg, resumeDatFile);

                btp.DoWork(sl, searchFolder, lvUTResults, cbUTUseHashing.Checked, cbUTMatchMissing.Checked,
                    cbUTSetPrio.Checked,
                    testMode, chkUTSearchSubfolders.Checked, mDoc.TheActionList, TVSettings.Instance.FNPRegexs,
                    mDoc.Args);
            }

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
            DialogResult dr = MessageBox.Show("Make sure µTorrent is not running, then click OK.", "TVRename",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            return (dr == DialogResult.OK);
        }

        private static void RestartUTorrent()
        {
            MessageBox.Show("You may now restart µTorrent.", "TVRename", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
