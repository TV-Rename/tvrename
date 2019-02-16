// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using System.IO;
    using Directory = Alphaleonis.Win32.Filesystem.Directory;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ActionPyTivoMeta : ActionWriteMetadata
    {
        public ActionPyTivoMeta(FileInfo nfo, ProcessedEpisode pe) :base(nfo,null)
        {
            Episode = pe;
        }

        #region Action Members

        public override string Name => "Write pyTivo Meta";

        public override bool Go(ref bool pause, TVRenameStats stats)
        {
            try
            {
                // create folder if it does not exist. (Only really applies when .meta\ folder is being used.)
                if (!Where.Directory.Exists)
                    Directory.CreateDirectory(Where.Directory.FullName);

                using (
                    StreamWriter writer = new StreamWriter(Where.FullName, false,
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
                        writer.WriteLine($"originalAirDate : {Episode.FirstAired.Value:yyyy-MM-dd}T00:00:00Z");
                    writer.WriteLine($"callsign : {Episode.Show.TheSeries().Network}");

                    WriteEntries(writer, "vDirector", Episode.EpisodeDirector);
                    WriteEntries(writer, "vWriter", Episode.Writer);
                    WriteEntries(writer, "vActor", string.Join("|", Episode.Show.TheSeries().GetActorNames()));
                    WriteEntries(writer, "vGuestStar",
                        Episode.EpisodeGuestStars); // not worring about actors being repeated
                    WriteEntries(writer, "vProgramGenre", string.Join("|", Episode.Show.TheSeries().Genres()));
                }

                Done = true;
                return true;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                Error = true;
                Done = true;
                return false;
            }
        }
    
        private static void WriteEntries(TextWriter writer, string heading, string entries)
        {
            if (string.IsNullOrEmpty(entries))
                return;
            if (!entries.Contains("|"))
                writer.WriteLine($"{heading} : {entries}");
            else
            {
                foreach (string entry in entries.Split('|'))
                    if (!string.IsNullOrEmpty(entry))
                        writer.WriteLine($"{heading} : {entry}");
            }
        }

        #endregion

        #region Item Members

        public override bool SameAs(Item o)
        {
            return (o is ActionPyTivoMeta meta) && (meta.Where == Where);
        }

        public override int Compare(Item o)
        {
            ActionPyTivoMeta nfo = o as ActionPyTivoMeta;

            if (Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return string.Compare((Where.FullName + Episode.Name), nfo.Where.FullName + nfo.Episode.Name, StringComparison.Ordinal);
        }

        #endregion
    }
}
