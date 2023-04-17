//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename;

internal static class Beta
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    internal static void LogShowEpisodeSizes(TVDoc doc)
    {
        doc.PreventAutoScan("Show File Sizes");
        StringBuilder output = new();

        output.AppendLine("");
        output.AppendLine("##################################################");
        output.AppendLine("File Quality FINDER - Start");
        output.AppendLine("##################################################");
        output.AppendLine("Width,   Height,   Length,    ShowName,   FileName");
        output.AppendLine("##################################################");
        Logger.Info(output.ToString());

        DirFilesCache dfc = new();
        foreach (ShowConfiguration si in doc.TvLibrary.Shows)
        {
            foreach (List<ProcessedEpisode> episodes in si.SeasonEpisodes.Values.ToList())
            {
                foreach (ProcessedEpisode pep in episodes)
                {
                    List<FileInfo> files = dfc.FindEpOnDisk(pep);
                    foreach (FileInfo file in files)
                    {
                        int width = file.GetFrameWidth();
                        int height = file.GetFrameHeight();
                        int length = file.GetFilmLength();
                        Logger.Info($"{width,-10}   {height,-10}   {length,-10}    {pep.Show.ShowName,-50}  {file.Name}");
                    }
                }
            }
        }

        output.Clear();
        output.AppendLine("##################################################");
        output.AppendLine("File Quailty FINDER - End");
        output.AppendLine("##################################################");

        Logger.Info(output.ToString());
        doc.AllowAutoScan();
    }
}
