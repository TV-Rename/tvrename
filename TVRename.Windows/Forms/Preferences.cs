using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using Microsoft.WindowsAPICodePack.Dialogs;
using TVRename.Windows.Configuration;

namespace TVRename.Windows.Forms
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            this.numericUpDownRecentDays.Value = Settings.Instance.RecentDays;
            this.numericUpDownDownloadThreads.Value = Settings.Instance.DownloadThreads;
            this.textBoxDefaultLocation.Text = Settings.Instance.DefaultLocation;
            this.textBoxSeasonTemplate.Text = Settings.Instance.SeasonTemplate;
            this.textBoxSpecialsTemplate.Text = Settings.Instance.SpecialsTemplate;
            this.textBoxEpisodeTemplate.Text = Settings.Instance.EpisodeTemplate;
        }

        private void buttonDefaultLocation_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog ofd = new CommonOpenFileDialog
            {
                DefaultDirectory = this.textBoxDefaultLocation.Text,
                IsFolderPicker = true,
                Multiselect = false,
                RestoreDirectory = true,
                EnsureValidNames = true,
                EnsurePathExists = true,
                EnsureFileExists = true
            };

            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.textBoxDefaultLocation.Text = Path.GetFullPath(ofd.FileName);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Settings.Instance.RecentDays = (int)this.numericUpDownRecentDays.Value;
            Settings.Instance.DownloadThreads = (int)this.numericUpDownDownloadThreads.Value;
            Settings.Instance.DefaultLocation = this.textBoxDefaultLocation.Text;
            Settings.Instance.SeasonTemplate = this.textBoxSeasonTemplate.Text;
            Settings.Instance.SpecialsTemplate = this.textBoxSpecialsTemplate.Text;
            Settings.Instance.EpisodeTemplate = this.textBoxEpisodeTemplate.Text;
            Settings.Instance.Dirty = true;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
