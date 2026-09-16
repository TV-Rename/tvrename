using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVRename.Forms.Tools;

public partial class TaskNotifier : Form
{
    private Task? task;
    private readonly string name;
    private readonly CancellationTokenSource cancellationToken;

    public TaskNotifier( string name, CancellationTokenSource cancellationToken)
    {
        this.name = name;
        this.cancellationToken = cancellationToken;
        InitializeComponent();
    }

    public void Start(Task task)
    {
        this.task = task;
        Show();
    }

    public void UpdateProgress(TaskProgress progress)
    {
        pbProgress.SetProgress(progress.percent);
        pbProgress.Update();
        lblMessage.Text = progress.message.ToUiVersion() ?? string.Empty;
        if (progress.lastUpdate is not null)
            lblLastUpdate.Text = progress.lastUpdate.ToUiVersion() ?? string.Empty;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        cancellationToken.Cancel();
        Close();
    }

    private void DoScanPartNotifier_Shown(object sender, EventArgs e)
    {
        Text = $"{name} Progress";
    }

    
}

public class TaskProgress
{
    public TaskProgress(int percent, string message)
    {
        this.percent = percent;
        this.message = message;
    }
    public TaskProgress(int percent, string message, string? lastUpdate)
    {
        this.percent = percent;
        this.message = message;
        this.lastUpdate = lastUpdate;
    }

    public int percent;
    public string message = string.Empty;
    public string? lastUpdate;
}

public class TaskCompletionProgress(Action<TaskProgress> handler) : Progress<TaskProgress>(handler)
{
    int maxProgress;

    public void SetMaxProgress(int max)
    {
        maxProgress = max;
    }
    protected override void OnReport(TaskProgress progress)
    {
        int percent = (int)((double)progress.percent / maxProgress * 100);
        base.OnReport(new TaskProgress (percent, progress.message, progress.lastUpdate));
    }

}
