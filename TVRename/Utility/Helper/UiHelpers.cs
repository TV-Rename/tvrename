using DaveChambers.FolderBrowserDialogEx;
using BrightIdeasSoftware;
using NLog;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace TVRename.Forms;

public static class UiHelpers
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static Color WarningColor() => Color.FromArgb(255, 210, 210);

    public static string TranslateColorToHtml(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static void Add(this ToolStripItemCollection items, string name, EventHandler command)
    {
        ToolStripMenuItem tsi = new(name.ToUiVersion());
        tsi.Click += command;
        items.Add(tsi);
    }
    public static bool ShowDialogAndOk(FolderBrowserDialogEx d, IWin32Window owner)
    {
        try
        {
            return ShowDialogAndOk(() => d.ShowDialog(owner));
        }
        catch (Win32Exception ex)
        {
            Logger.Error(ex, $"Failed to load dialog for {d.SelectedPath}");
            return false;
        }
    }
    public static bool ShowDialogAndOk(FolderBrowserDialog d, IWin32Window owner)
    {
        try
        {
            return ShowDialogAndOk(() => d.ShowDialog(owner));
        }
        catch (Win32Exception ex)
        {
            Logger.Error(ex, $"Failed to load dialog for {d.SelectedPath}");
            return false;
        }
    }
    public static bool ShowDialogAndOk(CommonDialog d, IWin32Window owner)
    {
        return ShowDialogAndOk(() => d.ShowDialog(owner));
    }
    public static bool ShowDialogAndOk(Form d, IWin32Window owner)
    {
        return ShowDialogAndOk(() => d.ShowDialog(owner));
    }
    private static bool ShowDialogAndOk(Func<DialogResult> function)
    {
        try
        {
            return function() == DialogResult.OK;
        }
        catch (SEHException ex)
        {
            Logger.Error(ex, "Could not load Dialog:");
        }
        catch (InvalidOperationException ex)
        {
            Logger.Error(ex, "Could not load Dialog:");
        }
        return false;
    }

    public static void Add(this ContextMenuStrip items, string name, EventHandler command)
    {
        items.Items.Add(name, command);
    }

    public static void AddSeparator(this ContextMenuStrip showRightClickMenu)
    {
        ToolStripSeparator tss = new();
        showRightClickMenu.Items.Add(tss);
    }

    /// <exception cref="ArgumentNullException"><paramref name="uiElement"/> is <see langword="null"/></exception>
    /// <exception cref="ObjectDisposedException">Control is already disposed.</exception>
    public static void SafeInvoke(this Control uiElement, System.Action updater, bool forceSynchronous)
    {
        if (uiElement is null)
        {
            throw new ArgumentNullException(nameof(uiElement));
        }

        if (uiElement.InvokeRequired)
        {
            if (forceSynchronous)
            {
                uiElement.Invoke(delegate { SafeInvoke(uiElement, updater, true); });
            }
            else
            {
                uiElement.BeginInvoke(delegate { SafeInvoke(uiElement, updater, false); });
            }
        }
        else
        {
            if (uiElement.IsDisposed)
            {
                throw new ObjectDisposedException("Control is already disposed.");
            }

            try
            {
                updater();
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Failed to Invoke");
            }
        }
    }

    public static void SetProgressStateNone(IntPtr uiHandle)
    {
        try
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, uiHandle);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static void SetProgress(int pct, IntPtr uiHandle)
    {
        try
        {
            TaskbarManager.Instance.SetProgressValue(pct, 100, uiHandle);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static void SetProgressStateNormal(IntPtr uiHandle)
    {
        try
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal, uiHandle);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public static void UpdateColorControls(this Control myControl, Color fore, Color back, Color highlight)
    {
        myControl.BackColor = back;
        myControl.ForeColor = fore;

        if (myControl is DataGridView myDgv)
        {
            myDgv.ColumnHeadersDefaultCellStyle.BackColor = back;
            myDgv.ColumnHeadersDefaultCellStyle.ForeColor = fore;
        }
        else if (myControl is ObjectListView olv)
        {
            olv.SelectedBackColor = highlight;
            HeaderStateStyle hss = new()
            {
                ForeColor = fore,
                BackColor = back
            };

            HeaderFormatStyle hfs = new()
            {
                Hot = hss,
                Normal = hss,
                Pressed = hss
            };

            olv.HeaderFormatStyle = hfs;
        }
        else if (myControl is ProgressBar pb)
        {
            pb.ForeColor = highlight;
        }
        else if (myControl is TabControl tc)
        {
            tc.BackColor = back;
        }
        // Any other non-standard controls should be implemented here aswell...

        foreach (Control subC in myControl.Controls)
        {
            UpdateColorControls(subC, fore, back, highlight);
        }
    }

    public static void StopTextDisappearing(TextBox tb, Control filterButton)
    {
        NativeMethods.StopTextDisappearing(tb, filterButton);
    }
}
