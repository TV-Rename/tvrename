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
    public partial class FolderMonitorProgress : Form
    {
        public bool Ready;
        private readonly FolderMonitor mainForm;

        public FolderMonitorProgress(FolderMonitor thefm)
        {
            mainForm = thefm;
            InitializeComponent();
            timer1_Tick(null, null); // force immediate initial update
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            mainForm.TokenSource.Cancel();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (mainForm == null)
            {
                return;
            }

            timer1.Stop();

            BringToFront();

            pbProgress.Value = mainForm.FmpPercent;
            lbMessage.Text = mainForm.FmpUpto;
            
            if (mainForm.TokenSource.IsCancellationRequested)
            {
                Close();
            }

            timer1.Start();
        }

        private void FolderMonitorProgress_Load(object sender, System.EventArgs e)
        {
            Ready = true;
        }
    }
}
