// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public class ActionPyTivoMeta : Item, Action, ScanListItem, ActionWriteMetadata
    {
        public FileInfo Where;

        public ActionPyTivoMeta(FileInfo nfo, ProcessedEpisode pe)
        {
            this.Episode = pe;
            this.Where = nfo;
        }

        #region Action Members

        public string Name
        {
            get { return "Write pyTivo Meta"; }
        }

        public bool Done { get; private set; }
        public bool Error { get; private set; }
        public string ErrorText { get; set; }

        public string ProgressText
        {
            get { return this.Where.Name; }
        }

        public double PercentDone
        {
            get { return this.Done ? 100 : 0; }
        }

        public long SizeOfWork
        {
            get { return 10000; }
        }

        public bool Go(TVSettings tvsettings, ref bool pause, TVRenameStats stats)
        {
            // "try" and silently fail.  eg. when file is use by other...
            StreamWriter writer;
            try
            {
                // create folder if it does not exist. (Only really applies when .meta\ folder is being used.)
                if (!this.Where.Directory.Exists)
                    this.Where.Directory.Create();
                writer = new StreamWriter(this.Where.FullName);
                if (writer == null)
                    return false;
            }
            catch (Exception)
            {
                this.Done = true;
                return true;
            }

            // See: http://pytivo.sourceforge.net/wiki/index.php/Metadata
            writer.WriteLine(string.Format("title : {0}", this.Episode.SI.ShowName));
            writer.WriteLine(string.Format("seriesTitle : {0}", this.Episode.SI.ShowName));
            writer.WriteLine(string.Format("episodeTitle : {0}", this.Episode.Name));
            writer.WriteLine(string.Format("episodeNumber : {0}{1:0#}", this.Episode.SeasonNumber, this.Episode.EpNum));
            writer.WriteLine("isEpisode : true");
            writer.WriteLine(string.Format("description : {0}", this.Episode.Overview));
            if (this.Episode.FirstAired != null)
                writer.WriteLine(string.Format("originalAirDate : {0:yyyy-MM-dd}T00:00:00Z",this.Episode.FirstAired.Value));
            writer.WriteLine(string.Format("callsign : {0}", this.Episode.SI.TheSeries().GetItem("Network")));

            WriteEntries(writer, "vDirector", this.Episode.EpisodeDirector);
            WriteEntries(writer, "vWriter", this.Episode.Writer);
            WriteEntries(writer, "vActor", this.Episode.SI.TheSeries().GetItem("Actors"));
            WriteEntries(writer, "vGuestStar", this.Episode.EpisodeGuestStars); // not worring about actors being repeated
            WriteEntries(writer, "vProgramGenre", this.Episode.SI.TheSeries().GetItem("Genre"));

            writer.Close();
            this.Done = true;
            return true;
        }

        private void WriteEntries(StreamWriter writer, string Heading, string Entries)
        {
            if (string.IsNullOrEmpty(Entries))
                return;
            if (!Entries.Contains("|"))
                writer.WriteLine(string.Format("{0} : {1}", Heading, Entries));
            else
            {
                foreach (string entry in Entries.Split('|'))
                    if (!string.IsNullOrEmpty(entry))
                        writer.WriteLine(string.Format("{0} : {1}", Heading, entry));
            }
        }

        #endregion

        #region Item Members

        public bool SameAs(Item o)
        {
            return (o is ActionPyTivoMeta) && ((o as ActionPyTivoMeta).Where == this.Where);
        }

        public int Compare(Item o)
        {
            ActionPyTivoMeta nfo = o as ActionPyTivoMeta;

            if (this.Episode == null)
                return 1;
            if (nfo ==null || nfo.Episode == null)
                return -1;
            return (this.Where.FullName + this.Episode.Name).CompareTo(nfo.Where.FullName + nfo.Episode.Name);
        }

        #endregion

        #region ScanListItem Members

        public IgnoreItem Ignore
        {
            get
            {
                if (this.Where == null)
                    return null;
                return new IgnoreItem(this.Where.FullName);
            }
        }

        public ListViewItem ScanListViewItem
        {
            get
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = this.Episode.SI.ShowName;
                lvi.SubItems.Add(this.Episode.SeasonNumber.ToString());
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

        string ScanListItem.TargetFolder
        {
            get
            {
                if (this.Where == null)
                    return null;
                return this.Where.DirectoryName;
            }
        }

        public string ScanListViewGroup
        {
            get { return "lvgActionMeta"; }
        }

        public int IconNumber
        {
            get { return 7; }
        }

        public ProcessedEpisode Episode { get; private set; }

        #endregion

    }
}