//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

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
