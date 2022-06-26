using System.Windows.Forms;

namespace TVRename;

// ReSharper disable once InconsistentNaming
public partial class TVRenameSplash : Form
{
    public TVRenameSplash()
    {
        InitializeComponent();
        lblVersion.Text = Helpers.DisplayVersion.ToUiVersion();
    }

    public void update(string status, int progress, string details)
    {
        UpdateStatus(status);
        UpdateInfo(details);
        UpdateProgress(progress);
    }
    public void UpdateStatus(string status)
    {
        if (IsHandleCreated)
        {
            Invoke((MethodInvoker)delegate { lblStatus.Text = status.ToUiVersion(); });
        }
    }

    public void UpdateProgress(int progress)
    {
        if (IsHandleCreated)
        {
            Invoke((MethodInvoker)delegate { prgComplete.Value = progress; });
        }
    }

    public void UpdateInfo(string info)
    {
        if (IsHandleCreated)
        {
            Invoke((MethodInvoker)delegate { lblInfo.Text = info.ToUiVersion(); });
        }
    }
}
