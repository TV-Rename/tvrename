//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TVRename.AddIns.Themes;

/// <summary>
/// Entry point of the Themes add-in. The add-in is self contained: TV Rename only calls
/// <see cref="Startup"/> (first thing in Program.Main) and <see cref="KeepStandardLook"/> (splash screen).
/// Everything else (theming each window as it opens, the Theme setting in Preferences) is done by watching
/// windows being shown, so no other form needs to know about themes.
///
/// Files, in the settings folder (the /userfilepath folder when that is used):
///   Themes\*.theme.xml        the themes offered in Preferences (the built in Dark and Cream are created if missing)
///   Themes\Defaults\          the original built in themes, rewritten every start so they are always pristine
///   Themes\Backups\           the last copy of each theme that loaded without a mistake
///   Themes\ThemeSettings.xml  which theme was chosen
/// </summary>
public static class ThemeManager
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    internal const string LightKey = "Light";
    internal const string WindowsKey = "Windows";
    internal const string DarkKey = "Dark";
    private const string SettingsFileName = "ThemeSettings.xml";

    private static string? themesFolder;
    private static bool quiet;
    private static readonly List<string> StartupProblems = [];

    /// <summary>The theme in use, or null for the standard (light) look.</summary>
    internal static Theme? Active { get; private set; }

    /// <summary>True when a theme is in use. Every theming step checks this, so the standard look is never touched.</summary>
    internal static bool IsThemed => Active is not null;

    /// <summary>True when the theme in use is built on Windows' dark look (most fixes are only needed then).</summary>
    internal static bool IsDark => Active is { IsLightBase: false } && Application.IsDarkModeEnabled;

    /// <summary>The choice saved in ThemeSettings.xml: Light, Windows or a theme file name.</summary>
    internal static string Selected { get; private set; } = LightKey;

    internal static string? ThemesFolder => themesFolder;

    /// <summary>
    /// Reads the chosen theme and, if it is not the standard look, switches the application to dark mode.
    /// Must be called before any window is created. Never throws: any problem means the standard look.
    /// </summary>
    public static void Startup(string[] args)
    {
        try
        {
            CommandLineArgs commandLine = new(args);
            quiet = commandLine.Hide || commandLine.Unattended || !Environment.UserInteractive;
            themesFolder = System.IO.Path.Combine(SettingsFolder(commandLine.UserFilePath), "Themes");

            PrepareFolder();
            Selected = ReadSelection();
            Active = ResolveTheme(Selected, StartupProblems);

            if (Active is { IsLightBase: false })
            {
                // TV Rename already has a colour mode setting; setting it keeps the rest of the program consistent.
                TVSettings.Instance.ColorMode = SystemColorMode.Dark;
                Application.SetColorMode(SystemColorMode.Dark);
                Logger.Info($"Theme add-in: using the \"{Active.DisplayName}\" theme (dark base)");
            }
            else if (Active is not null)
            {
                // Built on the standard light look: Windows stays in its normal (light) colour mode.
                Logger.Info($"Theme add-in: using the \"{Active.DisplayName}\" theme (light base)");
            }
            else
            {
                Logger.Info("Theme add-in: using the standard look");
            }
        }
        catch (Exception ex)
        {
            Active = null;
            Logger.Error(ex, "Theme add-in could not start, so the standard look is used");
        }

        try
        {
            WindowWatcher.Start();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Theme add-in could not watch for new windows");
        }
    }

    /// <summary>
    /// Keeps a window looking exactly as it does with the standard look, even when a theme is in use
    /// (used for the splash screen). Call it straight after InitializeComponent().
    /// </summary>
    public static void KeepStandardLook(Form form)
    {
        try
        {
            ThemeStyler.KeepStandardLook(form);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Theme add-in could not keep the standard look of {form.Name}");
        }
    }

    #region Files

    private static string SettingsFolder(string? userFilePath)
    {
        // Same rules as PathManager: the /userfilepath folder if given, otherwise TV Rename's application data folder.
        return string.IsNullOrEmpty(userFilePath)
            ? System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TVRename", "TVRename", "2.1")
            : System.IO.Path.GetFullPath(userFilePath);
    }

    private static string ThemeFile(string key) => System.IO.Path.Combine(themesFolder!, key + Theme.FileSuffix);
    private static string BackupFile(string key) => System.IO.Path.Combine(themesFolder!, "Backups", key + Theme.FileSuffix);
    private static string DefaultsFile(string key) => System.IO.Path.Combine(themesFolder!, "Defaults", key + Theme.FileSuffix);

    private static void PrepareFolder()
    {
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(themesFolder!, "Defaults"));
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(themesFolder!, "Backups"));

        // Defaults always holds the original built in themes, so there is always a clean copy to go back to;
        // a built in theme that is missing from the Themes folder is put (back) there.
        foreach (string key in Theme.BuiltInKeys)
        {
            string text = Theme.BuiltInFileText(key);
            string defaults = DefaultsFile(key);
            if (!System.IO.File.Exists(defaults) || System.IO.File.ReadAllText(defaults) != text)
            {
                System.IO.File.WriteAllText(defaults, text, Encoding.UTF8);
            }

            if (!System.IO.File.Exists(ThemeFile(key)))
            {
                System.IO.File.WriteAllText(ThemeFile(key), text, Encoding.UTF8);
            }
        }
    }

    private static string ReadSelection()
    {
        string path = System.IO.Path.Combine(themesFolder!, SettingsFileName);
        try
        {
            if (System.IO.File.Exists(path)
                && ((string?)XDocument.Load(path).Root?.Element("Theme"))?.Trim() is { Length: > 0 } key)
            {
                return key;
            }
        }
        catch (Exception ex) when (ex is XmlException or System.IO.IOException or UnauthorizedAccessException)
        {
            Logger.Warn(ex, $"Could not read {path}, so the standard look is used");
        }

        return LightKey;
    }

    internal static void SaveSelection(string key)
    {
        if (themesFolder is null)
        {
            return;
        }

        string path = System.IO.Path.Combine(themesFolder, SettingsFileName);
        new XDocument(new XElement("ThemeSettings", new XElement("Theme", key))).Save(path);
        Selected = key;
        Logger.Info($"Theme add-in: \"{key}\" chosen, it is used from the next start");
    }

    /// <summary>The themes to offer in Preferences: (file name, name to show). Dark first, then by name.</summary>
    internal static IReadOnlyList<(string Key, string Name)> AvailableThemes()
    {
        List<(string Key, string Name)> themes = [];
        if (themesFolder is null || !System.IO.Directory.Exists(themesFolder))
        {
            return [(DarkKey, DarkKey)];
        }

        foreach (string path in System.IO.Directory.GetFiles(themesFolder, "*" + Theme.FileSuffix))
        {
            string key = Theme.KeyFromPath(path);
            string name = Theme.ReadDisplayName(path) ?? $"{key} (has a mistake)";
            themes.Add((key, name));
        }

        if (!themes.Any(t => t.Key.Equals(DarkKey, StringComparison.OrdinalIgnoreCase)))
        {
            themes.Add((DarkKey, DarkKey));
        }

        return themes
            .OrderBy(t => t.Key.Equals(DarkKey, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <summary>The theme for a saved choice, or null for the standard look. Problems are described for the user.</summary>
    private static Theme? ResolveTheme(string key, List<string> problems)
    {
        if (key.Equals(LightKey, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (key.Equals(WindowsKey, StringComparison.OrdinalIgnoreCase))
        {
            return Helpers.IsSystemDarkMode() ? LoadTheme(DarkKey, problems) : null;
        }

        return LoadTheme(key, problems);
    }

    private static Theme? LoadTheme(string key, List<string> problems)
    {
        string path = ThemeFile(key);
        string backup = BackupFile(key);
        string problem;

        if (System.IO.File.Exists(path))
        {
            List<string> colourProblems = [];
            try
            {
                Theme theme = Theme.Load(path, colourProblems);
                if (colourProblems.Count == 0)
                {
                    // A clean load becomes the copy to fall back on.
                    if (!System.IO.File.Exists(backup) || System.IO.File.ReadAllText(backup) != System.IO.File.ReadAllText(path))
                    {
                        System.IO.File.Copy(path, backup, true);
                    }
                }
                else
                {
                    problems.Add($"The theme file {System.IO.Path.GetFileName(path)} has some mistakes, so a few colours use their default:");
                    problems.AddRange(colourProblems.Select(p => "    " + p));
                    LogProblems();
                }

                return theme;
            }
            catch (ThemeFileException ex)
            {
                problem = $"The theme file {System.IO.Path.GetFileName(path)} could not be used because {ex.Message}.";
            }
        }
        else
        {
            problem = $"The theme file {System.IO.Path.GetFileName(path)} was not found.";
        }

        problems.Add(problem);

        if (System.IO.File.Exists(backup))
        {
            try
            {
                Theme theme = Theme.Load(backup, []);
                problems.Add("The last copy that loaded correctly (in the Backups folder) is being used instead.");
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
            problems.Add($"The original {original.DisplayName} theme is being used instead.");
            LogProblems();
            return original;
        }

        problems.Add("The standard look is being used instead.");
        LogProblems();
        return null;

        void LogProblems() => Logger.Warn("Theme add-in: " + string.Join(Environment.NewLine, problems));
    }

    /// <summary>Tells the user about theme file mistakes once the main window is up (unless TV Rename runs hidden).</summary>
    private static void ShowStartupProblems()
    {
        if (StartupProblems.Count == 0 || quiet)
        {
            return;
        }

        string message = string.Join(Environment.NewLine, StartupProblems) + Environment.NewLine + Environment.NewLine
            + $"Theme files are in {themesFolder}. The last copy of each theme that loaded without mistakes is in the "
            + "Backups folder, and the original built in themes are in the Defaults folder; copy one of them back over the "
            + "theme file to undo a change.";
        StartupProblems.Clear();
        MessageBox.Show(message, "TV Rename theme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    #endregion

    /// <summary>
    /// Sees every window just before it first appears (WM_SHOWWINDOW, through a thread message hook), which is
    /// after the window has loaded and before it is painted. That is where the theme and the Preferences setting
    /// are added. An idle sweep catches anything the hook cannot see, such as windows on other threads.
    /// </summary>
    private static class WindowWatcher
    {
        private const int WH_CALLWNDPROC = 4;
        private const int WM_SHOWWINDOW = 0x0018;

        private static HookProc? hookProc;   // kept in a field so the delegate is never garbage collected
        private static IntPtr hook;
        private static bool busy;
        private static bool startupProblemsShown;
        private static readonly ConditionalWeakTable<Form, object> Seen = new();

        public static void Start()
        {
            hookProc = HookCallback;
            hook = SetWindowsHookEx(WH_CALLWNDPROC, hookProc, IntPtr.Zero, GetCurrentThreadId());
            if (hook == IntPtr.Zero)
            {
                Logger.Warn($"Theme add-in: window hook not available (error {Marshal.GetLastWin32Error()}), using the idle sweep only");
            }

            Application.Idle += OnIdle;
            Application.ApplicationExit += (_, _) =>
            {
                if (hook != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(hook);
                    hook = IntPtr.Zero;
                }
            };
        }

        private static IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && !busy && lParam != IntPtr.Zero)
            {
                try
                {
                    // CWPSTRUCT: lParam, wParam, message, hwnd. Read the message first; almost everything stops here.
                    int message = Marshal.ReadInt32(lParam, 2 * IntPtr.Size);
                    if (message == WM_SHOWWINDOW)
                    {
                        CWPSTRUCT msg = Marshal.PtrToStructure<CWPSTRUCT>(lParam);
                        if (msg.wParam != IntPtr.Zero && Control.FromHandle(msg.hwnd) is Form form)
                        {
                            busy = true;
                            try
                            {
                                OnFormShowing(form);
                            }
                            finally
                            {
                                busy = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Theme add-in: error while preparing a window");
                }
            }

            return CallNextHookEx(hook, code, wParam, lParam);
        }

        private static void OnFormShowing(Form form)
        {
            if (Seen.TryGetValue(form, out _) || ThemeStyler.IsStandardLookWindow(form))
            {
                return;
            }

            Seen.Add(form, true);

            if (form is TVRename.Forms.Preferences)
            {
                ThemeChooser.AddTo(form);
            }

            ThemeStyler.ApplyTheme(form);
        }

        private static void OnIdle(object? sender, EventArgs e)
        {
            try
            {
                // Wait for a window of our own (the main window), so the message is not hidden behind the splash screen.
                if (!startupProblemsShown && Application.OpenForms.Cast<Form>().Any(f => !f.InvokeRequired && f.Visible))
                {
                    startupProblemsShown = true;
                    ShowStartupProblems();
                }

                foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (!form.Visible || Seen.TryGetValue(form, out _) || ThemeStyler.IsStandardLookWindow(form))
                    {
                        continue;
                    }

                    if (form.InvokeRequired)
                    {
                        // A window on another thread: theme it on its own thread.
                        Seen.Add(form, true);
                        form.BeginInvoke((System.Windows.Forms.MethodInvoker)(() => ThemeStyler.ApplyTheme(form)));
                    }
                    else
                    {
                        OnFormShowing(form);
                    }
                }

                ThemeStyler.OnIdle();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Theme add-in: error in the idle sweep");
            }
        }

        private delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPSTRUCT
        {
            public IntPtr lParam;
            public IntPtr wParam;
            public int message;
            public IntPtr hwnd;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
    }
}
