//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

internal sealed record ThemeChoice(string Key, string Name)
{
    public override string ToString() => Name;
}

// Themes\*.theme.xml are offered in Preferences; Defaults\ holds the built in originals, Backups\ the last good copies
internal static class ThemeManager
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    internal const string LIGHT = "Light";
    internal const string WINDOWS = "Windows";
    private const string DARK = "Dark";

    private static readonly List<string> StartupProblems = [];

    // null when the standard look is used
    internal static Theme? Active { get; private set; }

    internal static bool IsThemed => Active is not null;

    // Most of the fixes are only needed when the theme is built on Windows' dark look
    internal static bool IsDark => Active is { IsLightBase: false } && Application.IsDarkModeEnabled;

    internal static string ThemesFolder => PathManager.ThemesFolder;

    // Called once the settings are loaded, before the main window is created
    internal static void Load()
    {
        try
        {
            PrepareFolder();
            Active = ResolveTheme(TVSettings.Instance.Theme);
        }
        catch (Exception ex)
        {
            Active = null;
            Logger.Error(ex, "Could not load the theme, so the standard look is used");
        }

        TVSettings.Instance.ColorMode = Active is { IsLightBase: false } ? SystemColorMode.Dark : SystemColorMode.Classic;

        Logger.Info(Active is null
            ? "Using the standard look"
            : $"Using the \"{Active.DisplayName}\" theme ({(Active.IsLightBase ? "light" : "dark")} base)");
    }

    private static string ThemeFile(string key) => Path.Combine(ThemesFolder, key + Theme.FILE_SUFFIX);
    private static string BackupFile(string key) => Path.Combine(ThemesFolder, "Backups", key + Theme.FILE_SUFFIX);
    private static string DefaultsFile(string key) => Path.Combine(ThemesFolder, "Defaults", key + Theme.FILE_SUFFIX);

    private static void PrepareFolder()
    {
        Directory.CreateDirectory(Path.Combine(ThemesFolder, "Defaults"));
        Directory.CreateDirectory(Path.Combine(ThemesFolder, "Backups"));

        foreach (string key in Theme.BuiltInKeys)
        {
            string text = Theme.BuiltInFileText(key);
            string defaults = DefaultsFile(key);
            if (!File.Exists(defaults) || File.ReadAllText(defaults) != text)
            {
                File.WriteAllText(defaults, text, Encoding.UTF8);
            }

            if (!File.Exists(ThemeFile(key)))
            {
                File.WriteAllText(ThemeFile(key), text, Encoding.UTF8);
            }
        }
    }

    // Light (the standard look), every theme file found, and Same as Windows
    internal static List<ThemeChoice> Choices()
    {
        List<ThemeChoice> themes = [];
        try
        {
            foreach (string path in Directory.GetFiles(ThemesFolder, "*" + Theme.FILE_SUFFIX))
            {
                string key = Theme.KeyFromPath(path);
                themes.Add(new ThemeChoice(key, Theme.ReadDisplayName(path) ?? $"{key} (has a mistake)"));
            }
        }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        {
            Logger.Warn(ex, $"Could not list the themes in {ThemesFolder}");
        }

        if (!themes.Any(t => t.Key.Equals(DARK, StringComparison.OrdinalIgnoreCase)))
        {
            themes.Add(new ThemeChoice(DARK, DARK));
        }

        List<ThemeChoice> choices = [new ThemeChoice(LIGHT, "Light (default)")];
        choices.AddRange(themes
            .OrderBy(t => t.Key.Equals(DARK, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase));
        choices.Add(new ThemeChoice(WINDOWS, "Same as Windows"));
        return choices;
    }

    private static Theme? ResolveTheme(string key)
    {
        if (key.Equals(LIGHT, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (key.Equals(WINDOWS, StringComparison.OrdinalIgnoreCase))
        {
            return Helpers.IsSystemDarkMode() ? LoadTheme(DARK) : null;
        }

        return LoadTheme(key);
    }

    private static Theme? LoadTheme(string key)
    {
        string path = ThemeFile(key);
        string backup = BackupFile(key);

        if (File.Exists(path))
        {
            List<string> colourProblems = [];
            try
            {
                Theme theme = Theme.Load(path, colourProblems);
                if (colourProblems.Count == 0)
                {
                    // A clean load becomes the copy to fall back on
                    if (!File.Exists(backup) || File.ReadAllText(backup) != File.ReadAllText(path))
                    {
                        File.Copy(path, backup, true);
                    }
                }
                else
                {
                    StartupProblems.Add($"The theme file {Path.GetFileName(path)} has some mistakes, so a few colours use their default:");
                    StartupProblems.AddRange(colourProblems.Select(p => "    " + p));
                    LogProblems();
                }

                return theme;
            }
            catch (ThemeFileException ex)
            {
                StartupProblems.Add($"The theme file {Path.GetFileName(path)} could not be used because {ex.Message}.");
            }
        }
        else
        {
            StartupProblems.Add($"The theme file {Path.GetFileName(path)} was not found.");
        }

        if (File.Exists(backup))
        {
            try
            {
                Theme theme = Theme.Load(backup, []);
                StartupProblems.Add("The last copy that loaded correctly (in the Backups folder) is being used instead.");
                LogProblems();
                return theme;
            }
            catch (ThemeFileException ex)
            {
                Logger.Warn($"The backup theme {backup} could not be used either: {ex.Message}");
            }
        }

        if (Theme.BuiltIn(key) is { } original)
        {
            StartupProblems.Add($"The original {original.DisplayName} theme is being used instead.");
            LogProblems();
            return original;
        }

        StartupProblems.Add("The standard look is being used instead.");
        LogProblems();
        return null;
    }

    private static void LogProblems() => Logger.Warn("Theme problem: " + string.Join(Environment.NewLine, StartupProblems));

    // Called once the main window is showing
    internal static void ShowStartupProblems(IWin32Window owner)
    {
        if (StartupProblems.Count == 0)
        {
            return;
        }

        string message = string.Join(Environment.NewLine, StartupProblems) + Environment.NewLine + Environment.NewLine
            + $"Theme files are in {ThemesFolder}. The last copy of each theme that loaded without mistakes is in the "
            + "Backups folder, and the original built in themes are in the Defaults folder; copy one of them back over the "
            + "theme file to undo a change.";
        StartupProblems.Clear();
        MessageBox.Show(owner, message, "TV Rename theme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
