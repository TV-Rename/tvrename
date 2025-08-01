//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal abstract class RecentExporter : Exporter
{
    private readonly TVDoc doc;

    protected RecentExporter(TVDoc doc)
    {
        this.doc = doc;
    }

    /// <exception cref="ArgumentException">Locaiton is not valid.</exception>
    /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="System.IO.IOException"></exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    protected override void Do()
    {
        IEnumerable<ProcessedEpisode> lpe = [.. doc.TvLibrary.RecentEpisodes(TVSettings.Instance.WTWRecentDays)];
        DirFilesCache dfc = new();

        //Write Contents to file
        using System.IO.StreamWriter file = new(Location());
        file.WriteLine(GenerateHeader());
        foreach (ProcessedEpisode episode in lpe)
        {
            try
            {
                List<FileInfo> files = dfc.FindEpOnDisk(episode, false);

                if (!files.Any())
                {
                    continue;
                }

                string name = TVSettings.Instance.NamingStyle.NameFor(episode);
                int length = files.First().GetFilmLength();

                file.WriteLine(GenerateRecord(episode, files.First(), name, length));
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex,
                    $"Had to skip saving {episode.Show.ShowName} S{episode.AppropriateSeasonNumber}E{episode.AppropriateEpNum} saving to {Location()}");
            }
        }

        file.WriteLine(GenerateFooter());
    }

    protected abstract string GenerateHeader();

    protected abstract string GenerateRecord(ProcessedEpisode ep, FileInfo file, string name, int length);

    protected abstract string GenerateFooter();
}
