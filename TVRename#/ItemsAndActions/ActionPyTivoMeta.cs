// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
namespace TVRename
{
    using System;
    using System.Windows.Forms;
    using System.IO;
    using Directory = Alphaleonis.Win32.Filesystem.Directory;
    using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

    public class ActionPyTivoMeta : ActionWriteMetadata
    {

        public ActionPyTivoMeta(FileInfo nfo, ProcessedEpisode pe)
        {
            this.Episode = pe;
            this.Where = nfo;
        }


        #region Action Members

        public override string Name => "Write pyTivo Meta";

        public override bool Go( ref bool pause, TVRenameStats stats)
        {
            // "try" and silently fail.  eg. when file is use by other...
            StreamWriter writer;
            try
            {
                // create folder if it does not exist. (Only really applies when .meta\ folder is being used.)
                if (!this.Where.Directory.Exists)
                    Directory.CreateDirectory(this.Where.Directory.FullName);
                writer = new StreamWriter(this.Where.FullName, false, System.Text.Encoding.GetEncoding(1252));
            }
            catch (Exception)
            {
                this.Done = true;
                return true;
            }

            // See: http://pytivo.sourceforge.net/wiki/index.php/Metadata
            writer.WriteLine($"title : {this.Episode.SI.ShowName}");
            writer.WriteLine($"seriesTitle : {this.Episode.SI.ShowName}");
            writer.WriteLine($"episodeTitle : {this.Episode.Name}");
            writer.WriteLine(
                $"episodeNumber : {this.Episode.AppropriateSeasonNumber}{this.Episode.AppropriateEpNum:0#}");
            writer.WriteLine("isEpisode : true");
            writer.WriteLine($"description : {this.Episode.Overview}");
            if (this.Episode.FirstAired != null)
                writer.WriteLine($"originalAirDate : {this.Episode.FirstAired.Value:yyyy-MM-dd}T00:00:00Z");
            writer.WriteLine($"callsign : {this.Episode.SI.TheSeries().getNetwork()}");

            WriteEntries(writer, "vDirector", this.Episode.EpisodeDirector);
            WriteEntries(writer, "vWriter", this.Episode.Writer);
            WriteEntries(writer, "vActor", String.Join("|", this.Episode.SI.TheSeries().GetActors()));
            WriteEntries(writer, "vGuestStar", this.Episode.EpisodeGuestStars); // not worring about actors being repeated
            WriteEntries(writer, "vProgramGenre", String.Join("|", this.Episode.SI.TheSeries().GetGenres()));

            writer.Close();
            this.Done = true;
            return true;
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
            return (o is ActionPyTivoMeta meta) && (meta.Where == this.Where);
        }

        public override int Compare(Item o)
        {
            ActionPyTivoMeta nfo = o as ActionPyTivoMeta;

            if (this.Episode == null)
                return 1;
            if (nfo?.Episode == null)
                return -1;
            return (this.Where.FullName + this.Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        #region Item Members



        public override ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem {Text = this.Episode.SI.ShowName};

                lvi.SubItems.Add(this.Episode.AppropriateSeasonNumber.ToString());
                lvi.SubItems.Add(this.Episode.NumsAsString());
                DateTime? dt = this.Episode.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");

                lvi.SubItems.Add(this.Where.DirectoryName);
                lvi.SubItems.Add(this.Where.Name);

                lvi.Tag = this;

                //lv->Items->Add(lvi);
                return lvi;
            }
        }



        #endregion

    }
}
