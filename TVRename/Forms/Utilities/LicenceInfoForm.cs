//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.Forms.Utilities;

public partial class LicenceInfoForm : Form
{
    public LicenceInfoForm()
    {
        InitializeComponent();
        lblCopyright.Text = $"Copyright (C) {TimeHelpers.LocalNow().Year} TV Rename";
    }

    private void btnLicence_Click(object sender, EventArgs e)
    {
        "https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md".OpenUrlInBrowser();
    }
}
