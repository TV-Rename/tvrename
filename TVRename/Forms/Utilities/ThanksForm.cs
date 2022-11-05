using System;
using System.Windows.Forms;

namespace TVRename.Forms.Utilities;

public partial class ThanksForm : Form
{
    public ThanksForm()
    {
        InitializeComponent();
    }

    private void btnLicence_Click(object sender, EventArgs e)
    {
        "https://thetvdb.com/".OpenUrlInBrowser();
    }

    private void BtnVisitTVMaze_Click(object sender, EventArgs e)
    {
        "https://www.tvmaze.com/".OpenUrlInBrowser();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        "https://www.themoviedb.org/".OpenUrlInBrowser();
    }
}
