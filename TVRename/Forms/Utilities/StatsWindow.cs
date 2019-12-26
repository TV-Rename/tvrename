// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private readonly TVRenameStats stats;

        public StatsWindow(TVRenameStats s)
        {
            stats = s;
            InitializeComponent();
        }

        private void StatsWindow_Load(object sender, System.EventArgs e)
        {
            txtFM.Text = stats.FilesMoved.ToString();
            txtFR.Text = stats.FilesRenamed.ToString();
            txtFC.Text = stats.FilesCopied.ToString();
            txtRCD.Text = stats.RenameChecksDone.ToString();
            txtMCD.Text = stats.MissingChecksDone.ToString();
            txtFAOD.Text = stats.FindAndOrganisesDone.ToString();
            txtAAS.Text = stats.AutoAddedShows.ToString();
            txtTM.Text = stats.TorrentsMatched.ToString();
            txtNOS.Text = stats.NsNumberOfShows.ToString();
            txtNOSeas.Text = stats.NsNumberOfSeasons.ToString();
            int noe = stats.NsNumberOfEpisodes;
            txtEOD.Text = noe == -1 ? "?" : noe.ToString();
            txtTE.Text = stats.NsNumberOfEpisodesExpected.ToString();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
