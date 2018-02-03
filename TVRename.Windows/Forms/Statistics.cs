using System;
using System.Linq;
using System.Windows.Forms;
using TVRename.Windows.Configuration;

namespace TVRename.Windows.Forms
{
    public partial class Statistics : Form
    {
        public Statistics()
        {
            InitializeComponent();

            // Files
            this.labelFilesMoved.Text = Stats.Instance.FilesMoved.ToString();
            this.labelFilesRenamed.Text = Stats.Instance.FilesRenamed.ToString();
            this.labelFilesCopied.Text = Stats.Instance.FilesCopied.ToString();

            // Checks
            this.labelChecksRename.Text = Stats.Instance.RenameChecksDone.ToString();
            this.labelChecksMissing.Text = Stats.Instance.MissingChecksDone.ToString();

            // Totals
            this.labelTotalShows.Text = Settings.Instance.Shows.Count.ToString();
            this.labelTotalSeasons.Text = Settings.Instance.Shows.Sum(s => s.Metadata.Seasons.Count).ToString();
            this.labelTotalEpisodes.Text = Settings.Instance.Shows.SelectMany(s => s.Metadata.Seasons.Values).Sum(s => s.Episodes.Count).ToString();
            this.labelTotalLocalEpisodes.Text = "?"; // TODO
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
