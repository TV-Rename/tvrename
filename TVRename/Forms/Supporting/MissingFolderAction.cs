//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using Alphaleonis.Win32.Filesystem;
using System.Windows.Forms;
using TVRename.Forms;

namespace TVRename;

public partial class MissingFolderAction : Form
{
    public string FolderName { get; private set; }
    public FaResult Result { get; private set; }

    public MissingFolderAction(string? showName, string? season, string folderName)
    {
        InitializeComponent();

        Result = FaResult.kfaCancel;
        FolderName = folderName;
        txtShow.Text = showName?.ToUiVersion();
        txtSeason.Text = season?.ToUiVersion();
        txtFolder.Text = FolderName.ToUiVersion();

        if (string.IsNullOrEmpty(FolderName))
        {
            txtFolder.Text = "Click Browse..., or Drag+Drop a folder onto this window.";
            bnCreate.Enabled = false;
            bnRetry.Enabled = false;
        }
    }

    private void bnIgnoreOnce_Click(object sender, System.EventArgs e)
    {
        Result = FaResult.kfaIgnoreOnce;
        Close();
    }

    private void bnIgnoreAlways_Click(object sender, System.EventArgs e)
    {
        Result = FaResult.kfaIgnoreAlways;
        Close();
    }

    private void bnCreate_Click(object sender, System.EventArgs e)
    {
        Result = FaResult.kfaCreate;
        Close();
    }

    private void bnRetry_Click(object sender, System.EventArgs e)
    {
        Result = FaResult.kfaRetry;
        Close();
    }

    private void bnCancel_Click(object sender, System.EventArgs e)
    {
        Result = FaResult.kfaCancel;
        Close();
    }

    private void bnBrowse_Click(object sender, System.EventArgs e)
    {
        folderBrowser.SelectedPath = FolderName;
        if (UiHelpers.ShowDialogAndOK(folderBrowser,this))
        {
            Result = FaResult.kfaDifferentFolder;
            FolderName = folderBrowser.SelectedPath;
            Close();
        }
    }

    private void MissingFolderAction_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void MissingFolderAction_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data is not null)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in files)
            {
                try
                {
                    DirectoryInfo di = new(path);
                    if (di.Exists)
                    {
                        FolderName = path;
                        Result = FaResult.kfaDifferentFolder;
                        Close();
                        return;
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
