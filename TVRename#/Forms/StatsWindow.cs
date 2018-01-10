using System.Windows.Forms;
using TVRename.Settings;

namespace TVRename
{
    public partial class StatsWindow : Form
    {
        public StatsWindow()
        {
            this.InitializeComponent();
        }

        private void StatsWindow_Load(object sender, System.EventArgs e)
        {
            this.txtFM.Text = Statistics.Instance.FilesMoved.ToString();
            this.txtFR.Text = Statistics.Instance.FilesRenamed.ToString();
            this.txtFC.Text = Statistics.Instance.FilesCopied.ToString();
            this.txtRCD.Text = Statistics.Instance.RenameChecksDone.ToString();
            this.txtMCD.Text = Statistics.Instance.MissingChecksDone.ToString();
            this.txtFAOD.Text = Statistics.Instance.FindAndOrganisesDone.ToString();
            this.txtAAS.Text = Statistics.Instance.AutoAddedShows.ToString();
            this.txtTM.Text = Statistics.Instance.TorrentsMatched.ToString();
            this.txtNOS.Text = "TODO";
            this.txtNOSeas.Text = "TODO";
            this.txtEOD.Text = "TODO";
            this.txtTE.Text = "TODO";
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
