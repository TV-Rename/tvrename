//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;

namespace TVRename;

internal class MoviesTxt : MoviesExporter
{
    public MoviesTxt(List<MovieConfiguration> shows) : base(shows)
    {
    }

    public override bool Active() => TVSettings.Instance.ExportMoviesTXT;
    protected override string Name() => "Movies TXT Exporter";

    protected override string Location() => TVSettings.Instance.ExportMoviesTXTTo;

    /// <exception cref="ArgumentException">Locaiton is not valid.</exception>
    /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.IO.IOException"></exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    protected override void Do()
    {
        using System.IO.StreamWriter file = new(Location());
        foreach (MovieConfiguration si in Shows)
        {
            file.WriteLine(si.ShowName);
        }
    }
}
