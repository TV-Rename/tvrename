using System;
using System.Windows.Forms;

namespace TVRename.Forms.Utilities
{
    public partial class ThanksForm : Form
    {
        public ThanksForm()
        {
            InitializeComponent();
        }

        private void btnLicence_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen("https://thetvdb.com/");
        }

        private void BtnVisitTVMaze_Click(object sender, EventArgs e)
        {
            Helpers.SysOpen("https://www.tvmaze.com/");
        }
    }
}
