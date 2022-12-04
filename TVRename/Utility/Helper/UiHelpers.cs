using System;
using System.Drawing;
using System.Windows.Forms;

namespace TVRename.Forms;

public static class UiHelpers
{
    public static Color WarningColor() => Color.FromArgb(255, 210, 210);

    public static string TranslateColorToHtml(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static void Add(this ToolStripItemCollection items, string name, EventHandler command)
    {
        ToolStripMenuItem tsi = new(name.ToUiVersion());
        tsi.Click += command;
        items.Add(tsi);
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
                uiElement.Invoke((System.Action)delegate { SafeInvoke(uiElement, updater, true); });
            }
            else
            {
                uiElement.BeginInvoke((System.Action)delegate { SafeInvoke(uiElement, updater, false); });
            }
        }
        else
        {
            if (uiElement.IsDisposed)
            {
                throw new ObjectDisposedException("Control is already disposed.");
            }

            updater();
        }
    }
}
