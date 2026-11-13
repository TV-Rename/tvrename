//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using BrightIdeasSoftware;
using CefSharp.DevTools.CSS;
using DaveChambers.FolderBrowserDialogEx;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.ComponentModel;
using System.Runtime.InteropServices;

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

    public static bool IsSomethingSelected(this ListBox subject) => subject.SelectedIndex != -1;
    public static bool IsSomethingSelected(this ComboBox subject) => subject.SelectedIndex != -1;
    public static void ClearSelection(this ComboBox subject) => subject.SelectedIndex = -1;

    public static void SetProgress(this ProgressBar pb, int value)
    {
        if (value >= pb.Maximum)
        {
            pb.Maximum = value;
        }
        if (value < pb.Minimum)
        {
            pb.Minimum = value;
        }
        pb.Value = value;
    }

    public static bool ShowDialogAndOk(FolderBrowserDialogEx d, IWin32Window owner)
    {
        try
        {
            if (d.SelectedPath.StartsWith(@"\", StringComparison.OrdinalIgnoreCase))
            {
                d.SelectedPath = d.SelectedPath.RemoveFirstCharacter();
            }

            return ShowDialogAndOk(() => d.ShowDialog(owner));
        }
        catch (Win32Exception ex)
        {
            Logger.Error(ex, $"Failed to load dialog for '{d.SelectedPath}'");
            return false;
        }
    }
    public static bool ShowDialogAndOk(FolderBrowserDialog d, IWin32Window owner)
    {
        try
        {
            if (d.SelectedPath.StartsWith(@"\", StringComparison.OrdinalIgnoreCase))
            {
                d.SelectedPath = d.SelectedPath.RemoveFirstCharacter();
            }

            return ShowDialogAndOk(() => d.ShowDialog(owner));
        }
        catch (Win32Exception ex)
        {
            Logger.Error(ex, $"Failed to load dialog for '{d.SelectedPath}'");
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
        ArgumentNullException.ThrowIfNull(uiElement);

        if (uiElement.IsDisposed) 
        {
            Logger.Warn($"Asked to do action on {uiElement.Name}, but it's disposed.");
            return;
        }

        try
        {
            if (uiElement.InvokeRequired)
            {
                if (forceSynchronous)
                {
                    uiElement.Invoke(new MethodInvoker(updater));
                }
                else
                {
                    uiElement.BeginInvoke(delegate { SafeInvoke(uiElement, updater, false); });
                }
            }
            else
            {
                updater();
            }
        }
        catch (InvalidAsynchronousStateException ex)
        {
            // The control's thread no longer exists (form was closed)
            // Silently ignore as the operation is being cancelled anyway
            Logger.Warn(ex, "Failed to Invoke on {uiElement.Name}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to Invoke on {uiElement.Name}");
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

    // Called by each form after InitializeComponent; does nothing unless a theme is in use
    public static void UpdateColorControls(this Control myControl)
    {
        if (!ThemeManager.IsThemed)
        {
            return;
        }

        try
        {
            ThemeControl(myControl);
        }
        catch (Exception ex)
        {
            // Theming must never stop a window from opening
            Logger.Error(ex, $"Could not apply the theme to {myControl.Name}");
        }
    }

    private static void ThemeControl(Control c)
    {
        if (!ThemeHelper.FirstTime(c))
        {
            return;
        }

        bool dark = ThemeManager.IsDark;

        ThemeHelper.ApplyColourRules(c);
        if (dark)
        {
            ThemeHelper.FlattenSunkenBorder(c);
        }

        switch (c)
        {
            case TabControl tabControl:
                // TabControlThemed paints the tabs
                ThemeHelper.AdaptImageList(tabControl.ImageList);
                break;

            case TabPage tabPage:
                // A tab page that uses the visual style back colour is painted white whatever the colour mode
                tabPage.UseVisualStyleBackColor = false;
                tabPage.BackColor = ThemeHelper.T.Background;
                break;

            case ListView listView:
                ThemeHelper.ThemeListView(listView);
                break;

            case TreeView treeView when dark:
                ThemeHelper.WhenHandleCreated(treeView, () => ThemeHelper.SetWindowTheme(treeView.Handle, "DarkMode_Explorer", null));
                break;

            case MonthCalendar calendar:
                ThemeHelper.ThemeMonthCalendar(calendar);
                break;

            case ToolStrip toolStrip:
                ThemeHelper.UseThemeRenderer(toolStrip);
                ThemeHelper.AdaptToolStripItems(toolStrip.Items);
                break;

            case ComboBox { DropDownStyle: ComboBoxStyle.DropDownList } when dark:
                // The dark theme Windows' own file dialogs use for combo boxes
                ThemeHelper.WhenHandleCreated(c, () => ThemeHelper.SetWindowTheme(c.Handle, "DarkMode_CFD", null));
                break;

            case RichTextBox { Enabled: false, Parent.Enabled: true } infoText when dark:
                // A disabled rich text box is grey on grey; read only looks the same and can be read
                infoText.ReadOnly = true;
                infoText.TabStop = false;
                infoText.Enabled = true;
                infoText.ForeColor = ThemeHelper.T.ListText;
                ThemeHelper.WhenHandleCreated(c, () => ThemeHelper.SetWindowTheme(c.Handle, "DarkMode_Explorer", null));
                break;

            case TextBoxBase { Multiline: true } when dark:
            case ListBox when dark:
            case ScrollableControl { AutoScroll: true } when dark:
                // Dark scroll bars
                ThemeHelper.WhenHandleCreated(c, () => ThemeHelper.SetWindowTheme(c.Handle, "DarkMode_Explorer", null));
                break;

            case Button button when dark:
                // Buttons need the visual style back colour for the Windows 11 dark look
                button.UseVisualStyleBackColor = true;
                if (button.Image is { } buttonImage)
                {
                    button.Image = ThemeHelper.AdaptImageForDark(buttonImage);
                }
                break;

            case LinkLabel link:
                ThemeHelper.ThemeLinkLabel(link);
                break;

            case Label label when dark:
                label.Paint += ThemeHelper.RepaintDisabledLabel;
                break;

            default:
                break;
        }

        if (c.ContextMenuStrip is { } menu)
        {
            ThemeHelper.UseThemeRenderer(menu);
            ThemeHelper.AdaptToolStripItems(menu.Items);
        }

        c.BackColorChanged += ThemeHelper.OnColourChanged;
        c.ForeColorChanged += ThemeHelper.OnColourChanged;

        foreach (Control subC in c.Controls)
        {
            ThemeControl(subC);
        }

        c.ControlAdded += (_, e) =>
        {
            if (e.Control is not null)
            {
                UpdateColorControls(e.Control);
            }
        };
    }

    public static void StopTextDisappearing(TextBox tb, Control filterButton)
    {
        NativeMethods.StopTextDisappearing(tb, filterButton);
    }
}
