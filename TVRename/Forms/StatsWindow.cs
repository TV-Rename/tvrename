// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
            Stats = s;
            InitializeComponent();
        }

        private void StatsWindow_Load(object sender, System.EventArgs e)
        {
            txtFM.Text = Stats.FilesMoved.ToString();
            txtFR.Text = Stats.FilesRenamed.ToString();
            txtFC.Text = Stats.FilesCopied.ToString();
            txtRCD.Text = Stats.RenameChecksDone.ToString();
            txtMCD.Text = Stats.MissingChecksDone.ToString();
            txtFAOD.Text = Stats.FindAndOrganisesDone.ToString();
            txtAAS.Text = Stats.AutoAddedShows.ToString();
            txtTM.Text = Stats.TorrentsMatched.ToString();
            txtNOS.Text = Stats.NS_NumberOfShows.ToString();
            txtNOSeas.Text = Stats.NS_NumberOfSeasons.ToString();
            int noe = Stats.NS_NumberOfEpisodes;
            txtEOD.Text = ((noe == -1) ? "?" : noe.ToString());
            txtTE.Text = Stats.NS_NumberOfEpisodesExpected.ToString();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
