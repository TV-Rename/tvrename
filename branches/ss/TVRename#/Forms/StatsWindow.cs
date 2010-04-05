// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for StatsWindow
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class StatsWindow : Form
    {
        private TVRenameStats Stats;

        public StatsWindow(TVRenameStats s)
        {
            this.Stats = s;
            this.InitializeComponent();
        }

        private void StatsWindow_Load(object sender, System.EventArgs e)
        {
            this.txtFM.Text = this.Stats.FilesMoved.ToString();
            this.txtFR.Text = this.Stats.FilesRenamed.ToString();
            this.txtFC.Text = this.Stats.FilesCopied.ToString();
            this.txtRCD.Text = this.Stats.RenameChecksDone.ToString();
            this.txtMCD.Text = this.Stats.MissingChecksDone.ToString();
            this.txtFAOD.Text = this.Stats.FindAndOrganisesDone.ToString();
            this.txtAAS.Text = this.Stats.AutoAddedShows.ToString();
            this.txtTM.Text = this.Stats.TorrentsMatched.ToString();
            this.txtNOS.Text = this.Stats.NS_NumberOfShows.ToString();
            this.txtNOSeas.Text = this.Stats.NS_NumberOfSeasons.ToString();
            int noe = this.Stats.NS_NumberOfEpisodes;
            this.txtEOD.Text = ((noe == -1) ? "?" : noe.ToString());
            this.txtTE.Text = this.Stats.NS_NumberOfEpisodesExpected.ToString();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}