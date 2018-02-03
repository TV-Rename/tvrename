using System;
using System.Windows.Forms;
using TVRename.Core.Models.Cache;
using Show = TVRename.Core.Models.Show;

namespace TVRename.Windows.Forms
{
    public partial class MissingFolderAction : Form
    {
        public enum ActionResult
        {
            Retry,
            Cancel,
            Create,
            IgnoreOnce,
            IgnoreAlways,
            Location
        }

        public ActionResult Result { get; private set; } = ActionResult.Cancel;

        public MissingFolderAction(Show show, Season season, string location)
        {
            InitializeComponent();

            this.labelShow.Text = show.Name ?? show.Metadata.Name;
            this.labelSeason.Text = $"{season.Number} of {show.Metadata.Seasons.Count}";
            this.labelFolder.Text = location;
        }
        
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            this.Result = ActionResult.Location;

            Close();
        }

        private void buttonIgnoreOnce_Click(object sender, EventArgs e)
        {
            this.Result = ActionResult.IgnoreOnce;

            Close();
        }

        private void buttonIgnoreAlways_Click(object sender, EventArgs e)
        {
            this.Result = ActionResult.IgnoreAlways;

            Close();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            this.Result = ActionResult.Create;

            Close();
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            this.Result = ActionResult.Retry;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Result = ActionResult.Cancel;

            Close();
        }
    }
}
