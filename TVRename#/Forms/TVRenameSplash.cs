using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TVRename
{
    public partial class TVRenameSplash : Form
    {
        public TVRenameSplash()
        {
            InitializeComponent();
            lblVersion.Text = Helpers.DisplayVersion;
        }
        public void UpdateStatus(string status) { lblStatus.Text = status; }
        public void UpdateProgress(int progress) { prgComplete.Value = progress; }
        public void UpdateInfo(string info) { lblInfo.Text = info; }

    }
}

