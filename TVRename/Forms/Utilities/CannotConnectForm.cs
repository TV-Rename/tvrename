using System;
using System.Windows.Forms;

namespace TVRename.Forms.Utilities
{
    public partial class CannotConnectForm : Form
    {
        public CannotConnectForm(string header, string message)
        {
            InitializeComponent();
            label1.Text = message;
            Text = header;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void bnTVDB_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen("https://www.thetvdb.com/");
        }

        private void bnAPICheck_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen("https://www.isitdownrightnow.com/api.thetvdb.com.html");
        }

        private void bnOffline_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void bnContinue_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            Close();
        }
    }
}
