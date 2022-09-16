using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace TVRename.Forms.Tools;

public partial class DoScanPartNotifier : Form
{
    private readonly PostScanActivity activity;
    private readonly CancellationTokenSource source = new();
    public DoScanPartNotifier(PostScanActivity activity)
    {
        this.activity = activity;
        InitializeComponent();
        bwDo.RunWorkerAsync();
    }

    private void Update(int percent, string? message,string? lastUpdate)
    {
        pbProgress.Value = percent.Between(0, 100);
        pbProgress.Update();
        lblMessage.Text = message?.ToUiVersion() ?? string.Empty;
        lblLastUpdate.Text = lastUpdate?.ToUiVersion() ?? string.Empty;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        bwDo.CancelAsync();
    }

    private void bwDo_DoWork(object sender, DoWorkEventArgs e)
    {
        Thread.CurrentThread.Name ??= $"{activity.ActivityName()} Thread"; // Can only set it once

        activity.Check(source.Token,(percent, message, lastUpdate) =>
        {
            BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
            backgroundWorker.ReportProgress(percent.Between(0,100), new Tuple<string, string>(message, lastUpdate));
            if (backgroundWorker.CancellationPending)
            {
                source.Cancel();
            }
        });
    }

    private void bwDo_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Tuple<string, string>? updateMessages = e.UserState as Tuple<string, string>;
        Update(e.ProgressPercentage, updateMessages?.Item1,updateMessages?.Item2);
    }

    private void bwDo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        Close();
    }

    private void DoScanPartNotifier_Shown(object sender, EventArgs e)
    {
        Text = $"{activity.ActivityName()} Progress";
    }
}
