// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for DownloadProgress
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class DownloadProgress : Form
    {
        private readonly CacheUpdater mDoc;
        private readonly System.Timers.Timer newTimer;
        private readonly CancellationTokenSource token;

        public DownloadProgress(CacheUpdater doc, CancellationTokenSource cts)
        {
            InitializeComponent();
            mDoc = doc;
            token = cts;
            tmrUpdate.Start();
            newTimer = new System.Timers.Timer(100);
            newTimer.Elapsed += NewTimerOnElapsed;
            newTimer.SynchronizingObject = this;
            newTimer.AutoReset = true;
        }

        private void NewTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Tick();
        }

        private void Tick()
        {
            if (mDoc.DownloadDone)
            {
                Close();
            }
            else
            {
                UpdateStuff();
            }
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            tmrUpdate.Stop();
            newTimer.Stop();
            DialogResult = DialogResult.Abort;
            token.Cancel();
        }

        private void tmrUpdate_Tick(object sender, System.EventArgs e)
        {
            Tick();
        }

        private void UpdateStuff()
        {
            txtCurrent.Text =
                TheTVDB.LocalCache.Instance.CurrentDLTask +
                TVmaze.LocalCache.Instance.CurrentDLTask;
            pbProgressBar.Value = mDoc.DownloadPct;
        }
    }
}
