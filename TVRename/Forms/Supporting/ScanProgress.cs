//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System.Windows.Forms;

namespace TVRename;

/// <summary>
/// Summary for ScanProgress
///
/// WARNING: If you change the name of this class, you will need to change the
///          'Resource File Name' property for the managed resource compiler tool
///          associated with all .resx files this class depends on.  Otherwise,
///          the designers will not be able to interact properly with localized
///          resources associated with this form.
/// </summary>
public partial class ScanProgress : Form
{
    private bool finished;
    public bool Ready;

    private int pctAutoBulkAdd;
    private int pctLocalSearch;
    private int pctDownloadFolder;
    private int pctMediaLib;
    private int pctDownloading;
    private int pctuTorrent;
    private string? msg;
    private string? lastUpdate;
    private UI ui;

    public ScanProgress(UI ui, bool autoBulkAdd, bool mediaLib, bool downloadFolder, bool searchLocal,
        bool downloading, bool rss)
    {
        Ready = false;
        finished = false;
        this.ui = ui;
        InitializeComponent();

        lbBulkAutoAdd.Enabled = autoBulkAdd;
        lbMediaLibrary.Enabled = mediaLib;
        lbDownloadFolder.Enabled = downloadFolder;
        lbSearchLocally.Enabled = searchLocal;
        lbCheckDownloading.Enabled = downloading;
        lbSearchRSS.Enabled = rss;
    }

    private void UpdateProg()
    {
        pbBulkAutoAdd.Value = pctAutoBulkAdd.Between(0,100);
        pbBulkAutoAdd.Update();
        pbMediaLib.Value = pctMediaLib.Between(0, 100);
        pbMediaLib.Update();
        pbDownloadFolder.Value = pctDownloadFolder.Between(0, 100);
        pbDownloadFolder.Update();
        pbLocalSearch.Value = pctLocalSearch.Between(0, 100);
        pbLocalSearch.Update();
        pbRSS.Value = pctDownloading.Between(0, 100);
        pbRSS.Update();
        pbDownloading.Value = pctuTorrent.Between(0, 100);
        pbDownloading.Update();

        if (!finished){
            Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue(
                pbBulkAutoAdd.Value + pbMediaLib.Value + pbDownloadFolder.Value + pbLocalSearch.Value + pbRSS.Value + pbDownloading.Value
                , 600
                , ui.Handle);
        }
        lblMessage.Text = msg?.ToUiVersion();
        lblDetail.Text = lastUpdate?.ToUiVersion();
    }

    public void MediaLibProg(int p, string message, string lastUpdated)
    {
        pctMediaLib = p;
        msg = message;
        lastUpdate = lastUpdated;
    }

    public void DownloadFolderProg(int p, string message, string lastUpdated)
    {
        pctDownloadFolder = p;
        msg = message;
        lastUpdate = lastUpdated;
    }

    public void LocalSearchProg(int p, string message, string lastUpdated)
    {
        pctLocalSearch = p;
        msg = message;
        lastUpdate = lastUpdated;
    }

    public void ToBeDownloadedProg(int p, string message, string lastUpdated)
    {
        pctDownloading = p;
        msg = message;
        lastUpdate = lastUpdated;
    }

    public void DownloadingProg(int p, string message, string lastUpdated)
    {
        pctuTorrent = p;
        msg = message;
        lastUpdate = lastUpdated;
    }

    private void ScanProgress_Load(object sender, System.EventArgs e)
    {
        Ready = true;
        timer1.Start();
    }

    private void timer1_Tick(object sender, System.EventArgs e)
    {
        UpdateProg();
        timer1.Start();
        if (finished)
        {
            Close();
        }
    }

    public void Done()
    {
        finished = true;
    }

    public void AddNewProg(int p, string message, string lastUpdated)
    {
        pctAutoBulkAdd = p;
        msg = message;
        lastUpdate = lastUpdated;
    }
}
