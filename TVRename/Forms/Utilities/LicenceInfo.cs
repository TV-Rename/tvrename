using System;
using System.Windows.Forms;

namespace TVRename.Forms.Utilities
{
    public partial class LicenceInfoForm : Form
    {
        public LicenceInfoForm()
        {
            InitializeComponent();
            lblCopyright.Text = $"Copyright (C) {DateTime.Now.Year} TV Rename";
        }

        private void btnLicence_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen("https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}
