//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using TVRename.Forms;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public partial class TVRenameSplash : Form
{
    public TVRenameSplash()
    {
        InitializeComponent();
        lblVersion.Text = Helpers.DisplayVersion.ToUiVersion();
    }

    public void Update(string status, int progress, string details)
    {
        UpdateStatus(status);
        UpdateInfo(details);
        UpdateProgress(progress);
    }

    public void UpdateStatus(string status) => lblStatus.SafeInvoke(() => lblStatus.Text = status.ToUiVersion(), true);
    public void UpdateProgress(int progress) => prgComplete.SafeInvoke(() => prgComplete.SetProgress(progress), true);
    public void UpdateInfo(string info) => lblInfo.SafeInvoke(() => lblInfo.Text = info.ToUiVersion(), true);
}
