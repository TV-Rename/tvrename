//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.AddIns.Themes;

/// <summary>
/// Adds the Theme setting to the Display page of Preferences when Preferences opens, lists the theme files
/// found at that moment and saves the choice when Preferences is closed with OK.
/// </summary>
internal static class ThemeChooser
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // Where the setting goes on the Display page, in the page's design units (7 x 15 font units, 96 dpi):
    // to the right of "Startup tab", above "Show in taskbar".
    private const int LabelX = 257, LabelY = 142, ComboX = 310, ComboY = 138, ComboWidth = 124, LinkX = 440;
    private const float DesignFontWidth = 7F;

    private sealed record Choice(string Key, string Text)
    {
        public override string ToString() => Text;
    }

    public static void AddTo(Form preferences)
    {
        try
        {
            if (preferences.Controls.Find("tpDisplay", true).FirstOrDefault() is not TabPage page)
            {
                Logger.Warn("Theme add-in: the Display page was not found in Preferences, so the Theme setting was not added");
                return;
            }

            float scale = preferences.CurrentAutoScaleDimensions.Width > 0
                ? preferences.CurrentAutoScaleDimensions.Width / DesignFontWidth
                : 1F;
            Point At(int x, int y) => new((int)Math.Round(x * scale), (int)Math.Round(y * scale));

            List<Choice> choices = [new(ThemeManager.LightKey, "Light (default)")];
            choices.AddRange(ThemeManager.AvailableThemes().Select(t => new Choice(t.Key, t.Name)));
            choices.Add(new Choice(ThemeManager.WindowsKey, "Same as Windows"));

            Label label = new()
            {
                Name = "lblThemeAddIn",
                AutoSize = true,
                Text = "The&me:",
                Location = At(LabelX, LabelY)
            };

            ComboBox combo = new()
            {
                Name = "cbThemeAddIn",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = At(ComboX, ComboY),
                Width = (int)Math.Round(ComboWidth * scale),
                DropDownWidth = (int)Math.Round(220 * scale)
            };
            combo.Items.AddRange(choices.Cast<object>().ToArray());
            combo.SelectedItem = choices.FirstOrDefault(c => c.Key.Equals(ThemeManager.Selected, StringComparison.OrdinalIgnoreCase))
                ?? choices[0];

            LinkLabel folderLink = new()
            {
                Name = "llThemeFolderAddIn",
                AutoSize = true,
                Text = "Folder",
                Location = At(LinkX, LabelY)
            };
            folderLink.LinkClicked += (_, _) => OpenThemesFolder();

            ToolTip tips = new();
            const string HELP = "Light is the standard look. Other themes come from the .theme.xml files in the Themes folder; "
                + "copy one, change its colours and it appears here. A new theme is used the next time TV Rename starts.";
            tips.SetToolTip(label, HELP);
            tips.SetToolTip(combo, HELP);
            tips.SetToolTip(folderLink, "Open the folder that holds the theme files");

            page.Controls.Add(label);
            page.Controls.Add(combo);
            page.Controls.Add(folderLink);

            preferences.FormClosed += (_, _) =>
            {
                tips.Dispose();
                if (preferences.DialogResult == DialogResult.OK && combo.SelectedItem is Choice chosen)
                {
                    SaveChoice(preferences, chosen);
                }
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Theme add-in: could not add the Theme setting to Preferences");
        }
    }

    private static void SaveChoice(Form preferences, Choice chosen)
    {
        if (chosen.Key.Equals(ThemeManager.Selected, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            ThemeManager.SaveSelection(chosen.Key);
            IWin32Window? owner = preferences.Owner ?? Form.ActiveForm;
            MessageBox.Show(owner, $"TV Rename will use the \"{chosen.Text}\" theme the next time it starts.", "Theme",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Theme add-in: could not save the chosen theme");
            MessageBox.Show($"The theme choice could not be saved: {ex.Message}", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static void OpenThemesFolder()
    {
        if (ThemeManager.ThemesFolder is not { } folder)
        {
            return;
        }

        try
        {
            // Full path, so the Windows folder window is used whatever is on PATH.
            string explorer = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(explorer, $"\"{folder}\"") { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, $"Could not open {folder}");
        }
    }
}
