//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Linq;

namespace TVRename
{
    using Alphaleonis.Win32.Filesystem;
    using System;

    public class ActionPyTivoMeta : ActionWriteMetadata
    {
        public ActionPyTivoMeta(FileInfo nfo, ProcessedEpisode pe) : base(nfo, pe.Show)
        {
            Episode = pe;
        }

        #region Action Members

        public override string Name => "Write pyTivo Meta";

        public override ActionOutcome Go(TVRenameStats stats)
        {
            if (Episode is null)
            {
                return new ActionOutcome("PyTivoMetaData Go called with no Episode Set");
            }
            try
            {
                // create folder if it does not exist. (Only really applies when .meta\ folder is being used.)
                if (!Where.Directory.Exists)
                {
                    Directory.CreateDirectory(Where.Directory.FullName);
                }

                using (
                    System.IO.StreamWriter writer = new(Where.FullName, false,
                        System.Text.Encoding.GetEncoding(1252)))
                {
                    // See: http://pytivo.sourceforge.net/wiki/index.php/Metadata
                    writer.WriteLine($"title : {Episode.Show.ShowName}");
                    writer.WriteLine($"seriesTitle : {Episode.Show.ShowName}");
                    writer.WriteLine($"episodeTitle : {Episode.Name}");
                    writer.WriteLine(
                        $"episodeNumber : {Episode.AppropriateSeasonNumber}{Episode.AppropriateEpNum:0#}");
                    writer.WriteLine("isEpisode : true");
                    writer.WriteLine($"description : {Episode.Overview}");
                    if (Episode.FirstAired != null)
                    {
                        writer.WriteLine($"originalAirDate : {Episode.FirstAired.Value:yyyy-MM-dd}T00:00:00Z");
                    }

                    writer.WriteLine($"callsign : {Episode.Show.CachedShow?.Networks.ToCsv()}");

                    WriteEntries(writer, "vDirector", Episode.EpisodeDirector);
                    WriteEntries(writer, "vWriter", Episode.Writer);
                    WriteEntries(writer, "vActor", string.Join("|", Episode.Show.GetActorNames()));
                    WriteEntries(writer, "vGuestStar",
                        Episode.EpisodeGuestStars); // not worrying about actors being repeated
                    WriteEntries(writer, "vProgramGenre", string.Join("|", Episode.Show.Genres));
                }

                return ActionOutcome.Success();
            }
            catch (Exception e)
            {
                return new ActionOutcome(e);
            }
        }

        private static void WriteEntries(System.IO.TextWriter writer, string heading, string? entries)
        {
            if (string.IsNullOrEmpty(entries))
            {
                return;
            }

            if (!entries.Contains("|"))
            {
                writer.WriteLine($"{heading} : {entries}");
            }
            else
            {
                foreach (string entry in entries.Split('|').Where(entry => !string.IsNullOrEmpty(entry)))
                {
                    writer.WriteLine($"{heading} : {entry}");
                }
            }
        }

        #endregion Action Members

        #region Item Members

        public override bool SameAs(Item o)
        {
            return o is ActionPyTivoMeta meta && meta.Where == Where;
        }

        public override int CompareTo(Item obj)
        {
            if (obj is not ActionPyTivoMeta nfo)
            {
                return -1;
            }

            if (Episode is null && nfo.Episode is null)
            {
                return string.Compare(Where.FullName, nfo.Where.FullName, StringComparison.Ordinal);
            }

            if (Episode is null)
            {
                return 1;
            }

            if (nfo.Episode is null)
            {
                return -1;
            }

            return string.Compare(Where.FullName + Episode.Name, nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }

        #endregion Item Members
    }
}
