//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

// Helpful functions and classes

namespace TVRename;

public static class Helpers
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Gets a value indicating whether application is running under Mono.
    /// </summary>
    /// <value>
    ///   <c>true</c> if application is running under Mono; otherwise, <c>false</c>.
    /// </value>
    public static bool OnMono => Type.GetType("Mono.Runtime") != null;

    public static bool InDebug() => Debugger.IsAttached;

    public static int Between(this int value, int min, int max) =>
        value < min ? min :
        value > max ? max :
        value;

    public static bool In<T>(this T? item, params T[] items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        return items.Contains(item);
    }

    public static string PrettyPrint(this TVSettings.ScanType st)
    {
        return st switch
        {
            TVSettings.ScanType.Quick => "Quick",
            TVSettings.ScanType.Full => "Full",
            TVSettings.ScanType.Recent => "Recent",
            TVSettings.ScanType.SingleShow => "Single",
            TVSettings.ScanType.FastSingleShow => "Single (Fast)",
            _ => throw new ArgumentOutOfRangeException(nameof(st), st, null)
        };
    }

    public static string PrettyPrint(this MovieConfiguration.MovieFolderFormat format)
    {
        return format switch
        {
            MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile => "Single Movie per Folder",
            MovieConfiguration.MovieFolderFormat.multiPerDirectory => "Many Movies per Folder",
            MovieConfiguration.MovieFolderFormat.bluray => "Bluray format",
            MovieConfiguration.MovieFolderFormat.dvd => "DVD format",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public static string PrettyPrint(this MediaConfiguration.MediaType st)
    {
        return st switch
        {
            MediaConfiguration.MediaType.tv => "TV Shows",
            MediaConfiguration.MediaType.movie => "Movies",
            MediaConfiguration.MediaType.both => "TV Shows and Movies",
            _ => throw new ArgumentOutOfRangeException(nameof(st), st, null)
        };
    }

    public static string PrettyPrint(this ProcessedSeason.SeasonType st)
    {
        return st switch
        {
            ProcessedSeason.SeasonType.dvd => "dvd",
            ProcessedSeason.SeasonType.aired => "official",
            ProcessedSeason.SeasonType.absolute => "absolute",
            ProcessedSeason.SeasonType.alternate => "alternate",
            _ => throw new ArgumentOutOfRangeException(nameof(st), st, null)
        };
    }

    public static string PrettyPrint(this TVDoc.ProviderType type)
    {
        return type switch
        {
            TVDoc.ProviderType.libraryDefault => "Library Default",
            TVDoc.ProviderType.TVmaze => "TV Maze",
            TVDoc.ProviderType.TheTVDB => "The TVDB",
            TVDoc.ProviderType.TMDB => "TMDB",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static void Swap<T>(
        this IList<T> list,
        int firstIndex,
        int secondIndex
    )
    {
        if (firstIndex == secondIndex)
        {
            return;
        }
        (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
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

    /// <summary>
    /// Gets the application display version from the current assemblies <see cref="AssemblyInformationalVersionAttribute"/>.
    /// </summary>
    /// <value>
    /// The application display version.
    /// </value>
    public static string DisplayVersion
    {
        get
        {
            string v = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().First().InformationalVersion;
#if DEBUG
            v += DebugText;
#endif
            return v;
        }
    }

    public static string DebugText => " ** Debug Build **";

    public static string Pad(this int i)
    {
        if (i.ToString().Length > 1)
        {
            return i.ToString();
        }

        return "0" + i;
    }

    public static string Pad(this int i, int size)
    {
        return i.ToString().Length >= size ? i.ToString() : i.ToString().PadLeft(size,'0');
    }

    public static string PrettyPrint(this DateTime? dt)
    {
        try
        {
            if (dt != null && dt.Value.CompareTo(DateTime.MaxValue) != 0)
            {
                return dt.Value.ToShortDateString();
            }
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        return string.Empty;
    }

    public static long ToUnixTime(this DateTime date) => Convert.ToInt64((date.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);

    public static DateTime FromUnixTime(this double unixTime) => DateTime.UnixEpoch.AddSeconds(unixTime);
    public static DateTime FromUnixTime(this long unixTime) => DateTime.UnixEpoch.AddSeconds(unixTime);

    private static readonly DateTime WindowsStartDateTime = new(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static bool OpenFolder(string? folder)
    {
        if (folder is null)
        {
            return false;
        }
        if (Directory.Exists(folder))
        {
            return SysOpen("explorer.exe",folder.EnsureEndsWithSeparator());
        }
        return false;
    }

    public static void OpenFolderSelectFile(string filename)
    {
        string args = $"/e, /select, \"{filename}\"";

        ProcessStartInfo info = new() { FileName = "explorer", Arguments = args };
        Process.Start(info);
    }

    public static bool OpenUrl(string url) => OpenUrlInternal(url);

    private static bool OpenUrlInternal(string url)
    {
        try
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception e)
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    Logger.Error(e, $"Could not open URL: {url}");
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not open URL: {url}");
            return false;
        }

        return true;
    }

    public static void OpenFile(string filename) => OpenUrlInternal(filename);

    static bool SysOpen(string? what, string? arguments)
    {
        if (string.IsNullOrWhiteSpace(what))
        {
            return false;
        }

        try
        {
            if (arguments.HasValue())
            {
                Process.Start(what, arguments);
            }
            else
            {
                Process.Start(what);
            }
            return true;
        }
        catch (Win32Exception e)
        {
            Logger.Warn(e, $"Could not open {what}");
            return false;
        }
        catch (System.IO.FileNotFoundException e)
        {
            Logger.Warn(e, $"Could not open {what}");
            return false;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Could not open {what}");
            return false;
        }
    }

    public static Color WarningColor() => Color.FromArgb(255, 210, 210);

    public static T LongestShowName<T>(this IEnumerable<T> media) where T : MediaConfiguration
    {
        IEnumerable<T> mediaConfigurations = media as T[] ?? media.ToArray();
        int longestName = mediaConfigurations.Select(configuration => configuration.ShowName.Length).Max();
        return mediaConfigurations.First(config => config.ShowName.Length == longestName);
    }

    public static string TranslateColorToHtml(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    public static DateTime GetMinWindowsTime(DateTime dateTime)
    {
        //Any cachedSeries before 1980 will get 1980 as the timestamp
        return dateTime.CompareTo(WindowsStartDateTime) < 0 ? WindowsStartDateTime : dateTime;
    }

    public static int ToInt(this string? text, int def)
    {
        if (text is null)
        {
            return def;
        }
        try
        {
            return int.Parse(text);
        }
        catch
        {
            return def;
        }
    }

    public static float ToPercent(this string text, float def)
    {
        float value;
        try
        {
            value = float.Parse(text);
        }
        catch
        {
            return def;
        }

        if (value < 1)
        {
            return 1;
        }

        if (value > 100)
        {
            return 100;
        }

        return value;
    }

    public static int ToInt(this string text, int min, int def, int max)
    {
        int value;
        try
        {
            value = int.Parse(text);
        }
        catch
        {
            return def;
        }

        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
