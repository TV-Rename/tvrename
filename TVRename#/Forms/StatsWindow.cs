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
        private TVRenameStats _stats;

        public StatsWindow(TVRenameStats s)
        {
            _stats = s;
            InitializeComponent();
        }

        private void StatsWindow_Load(object sender, System.EventArgs e)
        {
            txtFM.Text = _stats.FilesMoved.ToString();
            txtFR.Text = _stats.FilesRenamed.ToString();
            txtFC.Text = _stats.FilesCopied.ToString();
            txtRCD.Text = _stats.RenameChecksDone.ToString();
            txtMCD.Text = _stats.MissingChecksDone.ToString();
            txtFAOD.Text = _stats.FindAndOrganisesDone.ToString();
            txtAAS.Text = _stats.AutoAddedShows.ToString();
            txtTM.Text = _stats.TorrentsMatched.ToString();
            txtNOS.Text = _stats.NsNumberOfShows.ToString();
            txtNOSeas.Text = _stats.NsNumberOfSeasons.ToString();
            int noe = _stats.NsNumberOfEpisodes;
            txtEOD.Text = ((noe == -1) ? "?" : noe.ToString());
            txtTE.Text = _stats.NsNumberOfEpisodesExpected.ToString();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
