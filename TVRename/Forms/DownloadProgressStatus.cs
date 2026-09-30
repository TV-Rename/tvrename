//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using static TVRename.TVDoc;

namespace TVRename.Forms;

public class DownloadProgressStatus : Progress<DownloadProgressReport>
{
    readonly ProgressBar bar;
    readonly Label label;

    internal DownloadProgressStatus(ProgressBar bar, Label label)
    {
        this.bar = bar;
        this.label = label;
        reset();
    }

    readonly ConcurrentDictionary<ProviderType, (int done, int total)> status = new();

    void reset()
    {
        status.Clear();
        status[ProviderType.TheTVDB] = (0, 1);
        status[ProviderType.TMDB] = (0, 1);
        status[ProviderType.TVmaze] = (0, 1);
    }

    public void UpdateFromSource(ProviderType provider, int total)
    {
        status[provider] = (0, total);
        if (bar.InvokeRequired)
        {
            bar.Invoke(new MethodInvoker(delegate { bar.Maximum = status.Values.Sum(a => a.total); }));
        }
        else
        {
            bar.Maximum = status.Values.Sum(a => a.total);
        }
    }
    protected override void OnReport(DownloadProgressReport update)
    {
        if (update.UpdateType == DownloadProgressReport.Type.ProviderUpdates)
        {
            Update($"Downloading: {update.Provider.PrettyPrint()} updates", 0);
        }
        else if (update.UpdateType == DownloadProgressReport.Type.Final)
        {
            Update($"Downloading: {update.Message}", 0);
        }
        else
        {
            status[update.Provider] = (status[update.Provider].done + 1, status[update.Provider].total);
            Update($"Downloading: {update.Message}", status.Values.Sum(a => a.done));
        }

        base.OnReport(update);
    }

    private void Update(string message, int position)
    {
        if (bar.InvokeRequired)
        {
            bar.Invoke(new MethodInvoker(delegate
            {
                bar.SetProgress(position);
                bar.Enabled = true;
                bar.Visible = true;
            }));
        }
        else
        {
            bar.SetProgress(position);
            bar.Enabled = true;
            bar.Visible = true;
        }
        if (label.InvokeRequired)
        {
            label.Invoke(new MethodInvoker(delegate
            {
                label.Text = message.ToUiVersion();
                label.Visible = true;
                label.Enabled = true;
            }));
        }
        else
        {
            label.Text = message.ToUiVersion();
            label.Visible = true;
            label.Enabled = true;
        }
    }

    internal void MarkFinished()
    {
        bar.Enabled = false;
        bar.Visible = false;
        label.Visible = false;
        label.Enabled = false;
        label.Text = string.Empty;
    }
}
