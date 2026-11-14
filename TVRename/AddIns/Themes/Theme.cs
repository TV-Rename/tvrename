//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Globalization;

namespace TVRename.AddIns.Themes;

/// <summary>
/// One colour a theme file can set: its name in the file, its default in a theme built on the dark look and in one
/// built on the light look, and what it is used for.
/// </summary>
internal sealed record ThemeColour(string Name, Color Default, Color LightDefault, string Description);

/// <summary>A problem with a whole theme file (missing, not XML, wrong root element). The file cannot be used at all.</summary>
internal sealed class ThemeFileException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>
/// A colour theme read from a *.theme.xml file. A theme is built on Windows' dark look (base="Dark") or on the
/// standard light look (base="Light"). Every colour has a default for its base, so a file only has to list the
/// colours it changes, and a colour with a mistake in it falls back to its default.
/// </summary>
internal sealed class Theme
{
    public const string FileSuffix = ".theme.xml";
    public const int FormatVersion = 1;

    private static Color Hex(int rgb) => Color.FromArgb(255, Color.FromArgb(rgb));

    /// <summary>All colours a theme can set, in the order they are written to a theme file.</summary>
    public static readonly IReadOnlyList<ThemeColour> Colours =
    [
        new("Background", Hex(0x202020), Hex(0xF0F0F0), "Background of windows, dialogs and panels."),
        new("Text", Hex(0xFFFFFF), Hex(0x000000), "Normal text on the Background colour."),
        new("ListBackground", Hex(0x323232), Hex(0xFFFFFF), "Background of lists, trees, grids and text boxes."),
        new("ListText", Hex(0xF0F0F0), Hex(0x000000), "Text in lists, trees, grids and text boxes."),
        new("SecondaryText", Hex(0xADADAD), Hex(0x6D6D6D), "Less important text, for example the messages under the scan progress bars."),
        new("DisabledText", Hex(0x9A9A9A), Hex(0x6D6D6D), "Text of items that are switched off, for example scan steps that will not run."),
        new("Border", Hex(0x555555), Hex(0xA0A0A0), "Borders the theme draws itself: tab strips, column headers, progress bars."),
        new("TabStrip", Hex(0x2D2D2D), Hex(0xF0F0F0), "Tabs that are not selected."),
        new("SelectedSideTab", Hex(0x444444), Hex(0xFFFFFF), "The selected tab in the side tab strips (main window, Preferences)."),
        new("HeaderBackground", Hex(0x2B2B2B), Hex(0xFFFFFF), "Column and row headers of lists and grids."),
        new("HeaderHot", Hex(0x3C3C3C), Hex(0xE5F3FF), "A column header under the mouse."),
        new("HeaderPressed", Hex(0x4B4B4B), Hex(0xCCE8FF), "A column header while it is clicked."),
        new("GroupTitle", Hex(0x6EAAFF), Hex(0x1E3287), "Group titles in lists (for example the groups in the Scan results)."),
        new("CalendarTitle", Hex(0x787878), Hex(0x6D6D6D), "Title bar and weekday names of the calendar on the Schedule tab (the weekday names sit on the list background, so keep it well apart from that)."),
        new("ActiveFilterButton", Hex(0x5A5A5A), Hex(0xD3D3D3), "A filter button while its filter is switched on."),
        new("Warning", Hex(0x6E2828), Hex(0xFFD2D2), "Background that flags a problem (an action with an error, a folder that does not exist)."),
        new("ScheduleToday", Hex(0x5C541E), Hex(0xFFFFE0), "Schedule rows for episodes airing today."),
        new("ScheduleFuture", Hex(0x3E3E3E), Hex(0xD3D3D3), "Schedule rows for episodes airing in the future."),
        new("SchedulePast", Hex(0x1C405C), Hex(0xADD8E6), "Schedule rows for episodes that have aired."),
        new("ScheduleText", Hex(0xFFFFFF), Hex(0x000000), "Text on the three Schedule row colours."),
        new("ProgressBar", Hex(0x3AA655), Hex(0x06B025), "The filled part of progress bars."),
        new("ProgressTrack", Hex(0x383838), Hex(0xE6E6E6), "The empty part of progress bars."),
        new("WebText", Hex(0xE6E6E6), Hex(0x212529), "Text on the Information, Images and Summary pages."),
        new("WebMutedText", Hex(0xA8A8A8), Hex(0x6C757D), "Small, less important text on those pages (episode counts, dates)."),
        new("WebLink", Hex(0x7DB8FF), Hex(0x0D6EFD), "Links on those pages."),
        new("WebBorder", Hex(0x4A4A4A), Hex(0xDEE2E6), "Table lines and borders on those pages."),
        new("WebSeasonRow", Hex(0x383838), Hex(0xE2E3E5), "The season heading rows in the Summary page tables."),
    ];

    private static readonly Dictionary<string, ThemeColour> ColoursByName =
        Colours.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Themes that come with the add-in. Each is written to the Themes folder if missing, and kept pristine in
    /// Themes\Defaults. Only the colours that differ from the base defaults are listed here.
    /// </summary>
    private static readonly IReadOnlyList<(string Key, string Name, bool Light, string Note, Dictionary<string, Color> Colours)> BuiltIns =
    [
        ("Dark", "Dark", false, "Windows' dark look, with the parts it leaves light or unreadable fixed.", new()),
        ("Cream", "Cream", true,
            "Warm cream colours taken from the Magnetar browser extension's glass toolbar (github.com/ArrCee76/magnetar). "
            + "Only its colours: Windows Forms cannot draw its see-through glass, blur or gradients.",
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Background"] = Hex(0xF5F1E8),
                ["Text"] = Hex(0x1E1A14),
                ["ListBackground"] = Hex(0xFBF8F0),
                ["ListText"] = Hex(0x1E1A14),
                ["SecondaryText"] = Hex(0x6B6459),
                ["DisabledText"] = Hex(0x8A8275),
                ["Border"] = Hex(0xC3BDB2),
                ["TabStrip"] = Hex(0xEFE9DC),
                ["SelectedSideTab"] = Hex(0xE2E7DB),
                ["HeaderBackground"] = Hex(0xEFE9DC),
                ["HeaderHot"] = Hex(0xE4DDC9),
                ["HeaderPressed"] = Hex(0xD9D1BA),
                ["GroupTitle"] = Hex(0x4A7C5C),
                ["CalendarTitle"] = Hex(0x6B6459),
                ["ActiveFilterButton"] = Hex(0xE2E7DB),
                ["Warning"] = Hex(0xF3D6CF),
                ["ScheduleToday"] = Hex(0xF4E9C6),
                ["ScheduleFuture"] = Hex(0xEFE9DC),
                ["SchedulePast"] = Hex(0xE1E7EC),
                ["ScheduleText"] = Hex(0x1E1A14),
                ["ProgressBar"] = Hex(0x4A7C5C),
                ["ProgressTrack"] = Hex(0xE4DDC9),
                ["WebText"] = Hex(0x1E1A14),
                ["WebMutedText"] = Hex(0x6B6459),
                ["WebLink"] = Hex(0x3D6B4E),
                ["WebBorder"] = Hex(0xE4DDC9),
                ["WebSeasonRow"] = Hex(0xEFE9DC),
            }),
    ];

    private readonly Dictionary<string, Color> values;

    private Theme(string key, string displayName, bool isLightBase, Dictionary<string, Color> values)
    {
        Key = key;
        DisplayName = displayName;
        IsLightBase = isLightBase;
        this.values = values;
    }

    /// <summary>The file name without ".theme.xml"; this is what the chosen theme is saved as.</summary>
    public string Key { get; }

    /// <summary>The name shown in Preferences.</summary>
    public string DisplayName { get; }

    /// <summary>True for a theme built on the standard light look (base="Light"), false for one built on the dark look.</summary>
    public bool IsLightBase { get; }

    /// <summary>The keys (file names) of the themes that come with the add-in.</summary>
    public static IEnumerable<string> BuiltInKeys => BuiltIns.Select(b => b.Key);

    /// <summary>A theme that comes with the add-in, exactly as shipped (used when its file cannot be read at all).</summary>
    public static Theme? BuiltIn(string key)
    {
        var builtIn = BuiltIns.FirstOrDefault(b => b.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        return builtIn.Key is null
            ? null
            : new Theme(builtIn.Key, builtIn.Name, builtIn.Light, new Dictionary<string, Color>(builtIn.Colours, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>The built in Dark theme.</summary>
    public static Theme BuiltInDark => BuiltIn("Dark")!;

    public Color Background => Get();
    public Color Text => Get();
    public Color ListBackground => Get();
    public Color ListText => Get();
    public Color SecondaryText => Get();
    public Color DisabledText => Get();
    public Color Border => Get();
    public Color TabStrip => Get();
    public Color SelectedSideTab => Get();
    public Color HeaderBackground => Get();
    public Color HeaderHot => Get();
    public Color HeaderPressed => Get();
    public Color GroupTitle => Get();
    public Color CalendarTitle => Get();
    public Color ActiveFilterButton => Get();
    public Color Warning => Get();
    public Color ScheduleToday => Get();
    public Color ScheduleFuture => Get();
    public Color SchedulePast => Get();
    public Color ScheduleText => Get();
    public Color ProgressBar => Get();
    public Color ProgressTrack => Get();
    public Color WebText => Get();
    public Color WebMutedText => Get();
    public Color WebLink => Get();
    public Color WebBorder => Get();
    public Color WebSeasonRow => Get();

    private Color Get([System.Runtime.CompilerServices.CallerMemberName] string name = "")
    {
        if (values.TryGetValue(name, out Color value))
        {
            return value;
        }

        ThemeColour definition = ColoursByName[name];
        return IsLightBase ? definition.LightDefault : definition.Default;
    }

    /// <summary>
    /// Reads a theme file. A problem with the file as a whole throws <see cref="ThemeFileException"/>; a problem with
    /// one colour is added to <paramref name="problems"/> and that colour keeps its default.
    /// </summary>
    public static Theme Load(string path, ICollection<string> problems)
    {
        string key = KeyFromPath(path);
        XDocument document;
        try
        {
            document = XDocument.Load(path, LoadOptions.SetLineInfo);
        }
        catch (XmlException ex)
        {
            throw new ThemeFileException($"it is not valid XML ({ex.Message.TrimEnd('.')})", ex);
        }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        {
            throw new ThemeFileException($"it could not be read ({ex.Message})", ex);
        }

        XElement? root = document.Root;
        if (root is null || root.Name.LocalName != "Theme")
        {
            throw new ThemeFileException("its first element must be <Theme>");
        }

        string baseLook = ((string?)root.Attribute("base"))?.Trim() ?? "Dark";
        bool lightBase;
        if (baseLook.Equals("Dark", StringComparison.OrdinalIgnoreCase))
        {
            lightBase = false;
        }
        else if (baseLook.Equals("Light", StringComparison.OrdinalIgnoreCase))
        {
            lightBase = true;
        }
        else
        {
            throw new ThemeFileException($"base=\"{baseLook}\" is not supported (it must be \"Dark\" or \"Light\")");
        }

        string displayName = ((string?)root.Attribute("name"))?.Trim() is { Length: > 0 } named ? named : key;

        Dictionary<string, Color> values = new(StringComparer.OrdinalIgnoreCase);
        foreach (XElement element in root.Elements())
        {
            int line = ((IXmlLineInfo)element).LineNumber;
            if (element.Name.LocalName != "Colour")
            {
                problems.Add($"line {line}: <{element.Name.LocalName}> is not a theme setting and was ignored");
                continue;
            }

            string? name = ((string?)element.Attribute("name"))?.Trim();
            string value = ((string?)element.Attribute("value"))?.Trim() ?? string.Empty;
            if (name is null || !ColoursByName.TryGetValue(name, out ThemeColour? definition))
            {
                problems.Add($"line {line}: \"{name}\" is not a theme colour and was ignored");
                continue;
            }

            if (value.Length == 0)
            {
                // Empty means "use the default".
                continue;
            }

            if (TryParseColour(value, out Color colour))
            {
                values[definition.Name] = colour;
            }
            else
            {
                Color fallback = lightBase ? definition.LightDefault : definition.Default;
                problems.Add($"line {line}: {definition.Name} has the value \"{value}\", which is not a colour, so the default ({ToText(fallback)}) was used");
            }
        }

        return new Theme(key, displayName, lightBase, values);
    }

    /// <summary>Just the name to show for a theme file, without checking its colours. Null if the file cannot be read.</summary>
    public static string? ReadDisplayName(string path)
    {
        try
        {
            XElement? root = XDocument.Load(path).Root;
            if (root?.Name.LocalName != "Theme")
            {
                return null;
            }

            return ((string?)root.Attribute("name"))?.Trim() is { Length: > 0 } named ? named : KeyFromPath(path);
        }
        catch (Exception ex) when (ex is XmlException or System.IO.IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    public static string KeyFromPath(string path)
    {
        string fileName = System.IO.Path.GetFileName(path);
        return fileName.EndsWith(FileSuffix, StringComparison.OrdinalIgnoreCase)
            ? fileName[..^FileSuffix.Length]
            : System.IO.Path.GetFileNameWithoutExtension(fileName);
    }

    /// <summary>Accepts #RRGGBB, #RGB, "R, G, B" and colour names (for example SteelBlue).</summary>
    public static bool TryParseColour(string text, out Color colour)
    {
        colour = Color.Empty;
        string value = text.Trim();

        string[] parts = value.Split(',');
        if (parts.Length == 3)
        {
            int[] rgb = new int[3];
            for (int i = 0; i < 3; i++)
            {
                if (!int.TryParse(parts[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out rgb[i]) || rgb[i] is < 0 or > 255)
                {
                    return false;
                }
            }

            colour = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            return true;
        }

        if (value.StartsWith('#'))
        {
            string hex = value[1..];
            if (hex.Length == 3)
            {
                hex = string.Concat(hex.Select(ch => new string(ch, 2)));
            }

            if (hex.Length == 6 && int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int rgbValue))
            {
                colour = Color.FromArgb(255, Color.FromArgb(rgbValue));
                return true;
            }

            return false;
        }

        Color named = Color.FromName(value);
        if (named.IsKnownColor && !named.IsSystemColor)
        {
            colour = Color.FromArgb(named.ToArgb());
            return true;
        }

        return false;
    }

    public static string ToText(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    /// <summary>The text of a built in theme's file: every colour with its value and what it is for.</summary>
    public static string BuiltInFileText(string key)
    {
        var builtIn = BuiltIns.First(b => b.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        StringBuilder sb = new();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<!--");
        sb.AppendLine($"  TV Rename theme file: {builtIn.Name}. {builtIn.Note}");
        sb.AppendLine();
        sb.AppendLine("  To make your own theme: copy this file in the same folder, give the copy a new name ending in");
        sb.AppendLine("  .theme.xml (for example Midnight.theme.xml), change name=\"...\" below and edit the colours.");
        sb.AppendLine("  Then choose it in Options > Preferences > Display > Theme and restart TV Rename.");
        sb.AppendLine("  Changes to the theme you are using also show the next time TV Rename starts.");
        sb.AppendLine();
        sb.AppendLine("  base=\"Dark\" builds the theme on Windows' dark look, base=\"Light\" on the standard light look.");
        sb.AppendLine("  Colours can be written as #RRGGBB (#1E1E2E), as R, G, B (30, 30, 46) or as a colour name (SteelBlue).");
        sb.AppendLine("  Leave a value empty, or delete the line, to use the default for the base.");
        sb.AppendLine();
        sb.AppendLine("  If a theme has a mistake, TV Rename says so when it starts and uses the last copy that loaded");
        sb.AppendLine("  correctly (kept in the Backups folder). The original built in themes are always in the Defaults folder.");
        sb.AppendLine("-->");
        sb.AppendLine($"<Theme name=\"{builtIn.Name}\" base=\"{(builtIn.Light ? "Light" : "Dark")}\" version=\"{FormatVersion}\">");
        foreach (ThemeColour colour in Colours)
        {
            Color value = builtIn.Colours.TryGetValue(colour.Name, out Color set)
                ? set
                : builtIn.Light ? colour.LightDefault : colour.Default;
            sb.AppendLine($"  <!-- {colour.Description} -->");
            sb.AppendLine($"  <Colour name=\"{colour.Name}\" value=\"{ToText(value)}\" />");
        }
        sb.AppendLine("</Theme>");
        return sb.ToString();
    }
}
