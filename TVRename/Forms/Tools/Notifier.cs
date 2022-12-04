using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace TVRename.Forms.Tools;

public partial class Notifier : Form
{
    protected Notifier()
    {
        InitializeComponent();
    }

    protected void Start()
    {
        bwDo.RunWorkerAsync();
    }

    private void Update(int percent, string? message, string? lastUpdate)
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
        CancellationTokenSource source = new();
        Thread.CurrentThread.Name ??= $"{ActionName()} Thread"; // Can only set it once
        BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
        Do(backgroundWorker, source);
    }

    internal virtual void Do(BackgroundWorker backgroundWorker, CancellationTokenSource cancellationTokenSource)
        => throw new NotImplementedException();

    internal virtual string ActionName() => throw new NotImplementedException();

    protected void ReportProgress(CancellationTokenSource source, int percent, string message, string lastUpdate)
    {
        bwDo.ReportProgress(percent.Between(0, 100), new Tuple<string, string>(message, lastUpdate));
        if (bwDo.CancellationPending)
        {
            source.Cancel();
        }
    }

    private void bwDo_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Tuple<string, string>? updateMessages = e.UserState as Tuple<string, string>;
        Update(e.ProgressPercentage, updateMessages?.Item1, updateMessages?.Item2);
    }

    private void bwDo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        Close();
    }

    private void DoScanPartNotifier_Shown(object sender, EventArgs e)
    {
        Text = $"{ActionName()} Progress";
    }
}
