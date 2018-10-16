using System.Windows.Forms;

namespace TVRename
{
    // ReSharper disable once InconsistentNaming
    public partial class TVRenameSplash : Form
    {
        public TVRenameSplash()
        {
            InitializeComponent();
            lblVersion.Text = Helpers.DisplayVersion;
        }
        public void UpdateStatus(string status) { lblStatus.Text = status; }
        public void UpdateProgress(int progress) { prgComplete.Value = progress; }
        public void UpdateInfo(string info) { lblInfo.Text = info; }
    }
}
