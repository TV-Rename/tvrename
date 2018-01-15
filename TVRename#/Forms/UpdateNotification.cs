using System;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class UpdateNotification : Form
    {
        private readonly UpdateVersion newVersion;

        public UpdateNotification(UpdateVersion update)
        {
            this.newVersion = update;
            InitializeComponent();
            this.tbReleaseNotes.Text = this.newVersion.ReleaseNotesText;
            this.lblStatus.Text = $@"There is new version {update.VersionNumber} available since {update.ReleaseDate}.";
        }

        private void bnReleaseNotes_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(this.newVersion.ReleaseNotesUrl);
        }

        private void btnDownloadNow_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen(this.newVersion.DownloadUrl);
        }

    }
}
