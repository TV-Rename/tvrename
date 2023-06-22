//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// Helpful functions and classes

namespace TVRename;

public static class Helpers
{
    #region DebugingInfo

    /// <summary>
    /// Gets a value indicating whether application is running under Mono.
    /// </summary>
    /// <value>
    ///   <c>true</c> if application is running under Mono; otherwise, <c>false</c>.
    /// </value>
    public static bool OnMono => Type.GetType("Mono.Runtime") != null;

    public static bool InDebug() => Debugger.IsAttached;

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
    #endregion

    #region PrettyPrint
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

    public static string PrettyPrint(this TheTVDB.API.UpdateRecord.UpdateType st)
    {
        return st switch
        {
            TheTVDB.API.UpdateRecord.UpdateType.series => "TV Show",
            TheTVDB.API.UpdateRecord.UpdateType.movie => "Movie",
            TheTVDB.API.UpdateRecord.UpdateType.season => "TV Show Season",
            TheTVDB.API.UpdateRecord.UpdateType.episode => "TV Show Episode",
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

    #endregion

    #region Converters

    public static int Between(this int value, int min, int max) =>
        value < min ? min :
        value > max ? max :
        value;

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

    #endregion
}
