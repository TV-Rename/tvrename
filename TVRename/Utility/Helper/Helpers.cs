//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
            TVSettings.ScanType.Incremental => "Incremental",
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
            string v = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                .Cast<AssemblyInformationalVersionAttribute>().First().InformationalVersion;
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
        return i.ToString().Length >= size ? i.ToString() : i.ToString().PadLeft(size, '0');
    }

    public static bool OpenFolder(string? folder)
    {
        if (folder is null || !Directory.Exists(folder))
        {
            return false;
        }

        return SysOpen("explorer.exe", folder.EnsureEndsWithSeparator().InDoubleQuotes());
    }

    public static void OpenFolderSelectFile(string filename)
    {
        string args = $"/e, /select, {filename.InDoubleQuotes()}";

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

    public static void OpenFile(this FileInfo file) => OpenUrlInternal(file.FullName);

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
