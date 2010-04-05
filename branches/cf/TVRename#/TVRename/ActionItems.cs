// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

// Derivatives of "ActionItem" are the actions to do, as a result of doing a scan on the "Scan" tab.
// An "IgnoreItem" represents a file/episode to never ask the user about again. (Right-click->Ignore Selected / Options->Ignore List)

namespace TVRename
{
    public enum ActionType
    {
        kMissing,
        kCopyMoveRename,
        kRSS,
        kDownload,
        kNFO,
        kuTorrenting
    }

    public class IgnoreItem
    {
        public string FileAndPath;

        public IgnoreItem(XmlReader r)
        {
            if (r.Name == "Ignore")
                this.FileAndPath = r.ReadElementContentAsString();
        }

        public IgnoreItem(string fileAndPath)
        {
            this.FileAndPath = fileAndPath;
        }

        public bool SameFileAs(IgnoreItem o)
        {
            if (string.IsNullOrEmpty(this.FileAndPath) || string.IsNullOrEmpty(o.FileAndPath))
                return false;
            return this.FileAndPath == o.FileAndPath;
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Ignore");
            writer.WriteValue(this.FileAndPath);
            writer.WriteEndElement(); // Ignore
        }

        public void Read(XmlReader r)
        {
        }
    }

    public abstract class ActionItem
    {
        public bool Done;
        public string ErrorText;
        public bool HasError;
        public ProcessedEpisode PE; // can be null if not applicable or known

        public ActionType Type;

        protected ActionItem(ActionType t, ProcessedEpisode pe)
        {
            this.PE = pe;
            this.Done = false;
            this.Type = t;
            this.HasError = false;
            this.ErrorText = "";
        }

        public abstract IgnoreItem GetIgnore();

        public abstract ListViewItem GetLVI(ListView lv);

        public abstract string FilenameForProgress();

        public abstract string TargetFolder();

        // nullptr if none, otherwise folder "of interest" for this item
        // e.g. where file is missing from, or downloader is downloading to

        public virtual bool Action(TVDoc doc)
        {
            // default is to do nothing
            // also set Done
            this.Done = true;
            return true; // all ok
        }

        public abstract bool SameAs(ActionItem o);

        public virtual int IconNumber()
        {
            return -1;
        }

        // Search predicate 
        public static bool DoneOK(ActionItem i)
        {
            return i.Done && !i.HasError;
        }
    }

    public class ActionDownload : ActionItem
    {
        public string BannerPath;
        public FileInfo Destination;
        public ShowItem SI;

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string bannerPath)
            : base(ActionType.kDownload, pe)
        {
            this.Destination = dest;
            this.SI = si;
            this.BannerPath = bannerPath;
        }

        public override IgnoreItem GetIgnore()
        {
            if (this.Destination == null)
                return null;
            else
                return new IgnoreItem(this.Destination.FullName);
        }

        public override string TargetFolder()
        {
            if (this.Destination == null)
                return null;
            else
                return this.Destination.DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.Destination.Name;
        }

        public bool SameAs2(ActionDownload o)
        {
            return (o.Destination == this.Destination);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionDownload) (o));
        }

        public override int IconNumber()
        {
            return 5;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = this.SI.ShowName();
            lvi.SubItems.Add(this.PE != null ? this.PE.SeasonNumber.ToString() : "");
            lvi.SubItems.Add(this.PE != null ? this.PE.NumsAsString() : "");

            if (this.PE != null)
            {
                DateTime? dt = this.PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(this.Destination.DirectoryName);
            lvi.SubItems.Add(this.BannerPath);

            if (string.IsNullOrEmpty(this.BannerPath))
                lvi.BackColor = Helpers.WarningColor();

            lvi.SubItems.Add(this.Destination.Name);

            lvi.Tag = this;
            lvi.Group = lv.Groups[5]; // download image
            // lv->Items->Add(lvi);

            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            byte[] theData = this.SI.TVDB.GetPage(this.BannerPath, false, typeMaskBits.tmBanner, false);
            if (theData == null)
            {
                this.ErrorText = "Unable to download " + this.BannerPath;
                this.HasError = true;
                return false;
            }

            FileStream fs = new FileStream(this.Destination.FullName, FileMode.Create);
            fs.Write(theData, 0, theData.Length);
            fs.Close();

            this.Done = true;
            return true;
        }
    }

    public class ActionCopyMoveRename : ActionItem
    {
        #region Op enum

        public enum Op
        {
            Copy,
            Move,
            Rename
        }

        #endregion

        public FileInfo From;

        public Op Operation;
        public FileInfo To;

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep)
            : base(ActionType.kCopyMoveRename, ep)
        {
            this.Operation = operation;
            this.From = from;
            this.To = to;
        }

        public bool IsMoveRename() // same thing to the OS
        {
            return ((this.Operation == Op.Move) || (this.Operation == Op.Rename));
        }

        public override IgnoreItem GetIgnore()
        {
            if (this.To == null)
                return null;
            else
                return new IgnoreItem(this.To.FullName);
        }

        public override int IconNumber()
        {
            return (this.IsMoveRename() ? 4 : 3);
        }

        public override string TargetFolder()
        {
            if (this.To == null)
                return null;
            else
                return this.To.DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.To.Name;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            if (this.PE == null)
            {
                lvi.Text = "";
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }
            else
            {
                lvi.Text = this.PE.TheSeries.Name;
                lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
                lvi.SubItems.Add(this.PE.NumsAsString());
                DateTime? dt = this.PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }

            lvi.SubItems.Add(this.From.DirectoryName);
            lvi.SubItems.Add(this.From.Name);
            lvi.SubItems.Add(this.To.DirectoryName);
            lvi.SubItems.Add(this.To.Name);

            if (this.Operation == Op.Rename)
                lvi.Group = lv.Groups[1];
            else if (this.Operation == Op.Copy)
                lvi.Group = lv.Groups[2];
            else if (this.Operation == Op.Move)
                lvi.Group = lv.Groups[3];

            //lv->Items->Add(lvi);
            return lvi;
        }

        public bool SameSource(ActionCopyMoveRename o)
        {
            return (Helpers.Same(this.From, o.From));
        }

        public bool SameAs2(ActionCopyMoveRename o)
        {
            return ((this.Operation == o.Operation) && Helpers.Same(this.From, o.From) && Helpers.Same(this.To, o.To));
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionCopyMoveRename) (o));
        }

        public long FileSize()
        {
            try
            {
                return this.From.Length;
            }
            catch
            {
                return 1;
            }
        }
    }

    public class ActionMissing : ActionItem
    {
        public string TheFileNoExt;

        public ActionMissing(ProcessedEpisode pe, string whereItShouldBeNoExt)
            : base(ActionType.kMissing, pe)
        {
            this.TheFileNoExt = whereItShouldBeNoExt;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(this.TheFileNoExt))
                return null;
            else
                return new IgnoreItem(this.TheFileNoExt);
        }

        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(this.TheFileNoExt))
                return null;
            else
                return new FileInfo(this.TheFileNoExt).DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.PE.Name;
        }

        public override bool Action(TVDoc doc)
        {
            return true; // return success, but don't set as Done
        }

        public override int IconNumber()
        {
            return 1;
        }

        public bool SameAs2(ActionMissing o)
        {
            return string.Compare(o.TheFileNoExt, this.TheFileNoExt) == 0;
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionMissing) (o));
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = this.PE.SI.ShowName();
            lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
            lvi.SubItems.Add(this.PE.NumsAsString());

            DateTime? dt = this.PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            FileInfo fi = new FileInfo(this.TheFileNoExt);
            lvi.SubItems.Add(fi.DirectoryName);
            lvi.SubItems.Add(fi.Name);

            lvi.Tag = this;
            lvi.Group = lv.Groups[0];

            //lv->Items->Add(lvi);
            return lvi;
        }
    }

    public class ActionRSS : ActionItem
    {
        public RSSItem RSS;
        public string TheFileNoExt;

        public ActionRSS(RSSItem rss, string toWhereNoExt, ProcessedEpisode pe)
            : base(ActionType.kRSS, pe)
        {
            this.PE = pe;
            this.RSS = rss;
            this.TheFileNoExt = toWhereNoExt;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(this.TheFileNoExt))
                return null;
            else
                return new IgnoreItem(this.TheFileNoExt);
        }

        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(this.TheFileNoExt))
                return null;
            else
                return new FileInfo(this.TheFileNoExt).DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.RSS.Title;
        }

        public bool SameAs2(ActionRSS o)
        {
            return (o.RSS == this.RSS);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionRSS) (o));
        }

        public override int IconNumber()
        {
            return 6;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = this.PE.SI.ShowName();
            lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
            lvi.SubItems.Add(this.PE.NumsAsString());
            DateTime? dt = this.PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(this.TheFileNoExt);
            lvi.SubItems.Add(this.RSS.Title);

            lvi.Group = lv.Groups[4];
            lvi.Tag = this;

            // lv->Items->Add(lvi);
            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(this.RSS.URL);
                if ((r == null) || (r.Length == 0))
                {
                    this.HasError = true;
                    this.ErrorText = "No data downloaded";
                }

                string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + doc.FilenameFriendly(this.RSS.Title);
                if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                    saveTemp += ".torrent";
                File.WriteAllBytes(saveTemp, r);

                System.Diagnostics.Process.Start(doc.Settings.uTorrentPath, "/directory \"" + (new FileInfo(this.TheFileNoExt).Directory.FullName) + "\" \"" + saveTemp + "\"");

                this.HasError = false;
            }
            catch (Exception e)
            {
                this.ErrorText = e.Message;
                this.HasError = true;
            }
            this.Done = true;

            return !this.HasError;
        }
    }

    public class ActionNFO : ActionItem
    {
        public ShowItem SI; // if for an entire show, rather than specific episode
        public FileInfo Where;

        public ActionNFO(FileInfo nfo, ProcessedEpisode pe)
            : base(ActionType.kNFO, pe)
        {
            this.SI = null;
            this.Where = nfo;
        }

        public ActionNFO(FileInfo nfo, ShowItem si)
            : base(ActionType.kNFO, null)
        {
            this.SI = si;
            this.Where = nfo;
        }

        public override IgnoreItem GetIgnore()
        {
            if (this.Where == null)
                return null;
            else
                return new IgnoreItem(this.Where.FullName);
        }

        public override string TargetFolder()
        {
            if (this.Where == null)
                return null;
            else
                return this.Where.DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return this.Where.Name;
        }

        public bool SameAs2(ActionNFO o)
        {
            return (o.Where == this.Where);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionNFO) (o));
        }

        public override int IconNumber()
        {
            return 7;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            if (this.PE != null)
            {
                lvi.Text = this.PE.SI.ShowName();
                lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
                lvi.SubItems.Add(this.PE.NumsAsString());
                DateTime? dt = this.PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }
            else
            {
                lvi.Text = this.SI.ShowName();
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }

            lvi.SubItems.Add(this.Where.DirectoryName);
            lvi.SubItems.Add(this.Where.Name);

            lvi.Group = lv.Groups[6];
            lvi.Tag = this;

            //lv->Items->Add(lvi);
            return lvi;
        }

        public static void WriteInfo(XmlWriter writer, ShowItem si, string whichItem, string @as)
        {
            string t = si.TheSeries().GetItem(whichItem);
            if (!string.IsNullOrEmpty(t))
            {
                writer.WriteStartElement(@as);
                writer.WriteValue(t);
                writer.WriteEndElement();
            }
        }

        public override bool Action(TVDoc doc)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(this.Where.FullName, settings);

            if (this.PE != null) // specific episode
            {
                // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                writer.WriteStartElement("episodedetails");
                writer.WriteStartElement("title");
                writer.WriteValue(this.PE.Name);
                writer.WriteEndElement();
                writer.WriteStartElement("season");
                writer.WriteValue(this.PE.SeasonNumber);
                writer.WriteEndElement();
                writer.WriteStartElement("episode");
                writer.WriteValue(this.PE.EpNum);
                writer.WriteEndElement();
                writer.WriteStartElement("plot");
                writer.WriteValue(this.PE.Overview);
                writer.WriteEndElement();
                writer.WriteStartElement("aired");
                if (this.PE.FirstAired != null)
                    writer.WriteValue(this.PE.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
                writer.WriteEndElement(); // episodedetails
            }
            else if (this.SI != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                writer.WriteStartElement("title");
                writer.WriteValue(this.SI.ShowName());
                writer.WriteEndElement();

                writer.WriteStartElement("episodeguideurl");
                writer.WriteValue(this.SI.TVDB.BuildURL(true, true, this.SI.TVDBCode, this.SI.TVDB.PreferredLanguage(this.SI.TVDBCode)));
                writer.WriteEndElement();

                WriteInfo(writer, this.SI, "Overview", "plot");

                string genre = this.SI.TheSeries().GetItem("Genre");
                if (!string.IsNullOrEmpty(genre))
                {
                    genre = genre.Trim('|');
                    genre = genre.Replace("|", " / ");
                    writer.WriteStartElement("genre");
                    writer.WriteValue(genre);
                    writer.WriteEndElement();
                }

                WriteInfo(writer, this.SI, "FirstAired", "premiered");
                WriteInfo(writer, this.SI, "Rating", "rating");

                // actors...
                string actors = this.SI.TheSeries().GetItem("Actors");
                if (!string.IsNullOrEmpty(actors))
                {
                    foreach (string aa in actors.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(aa))
                        {
                            writer.WriteStartElement("actor");
                            writer.WriteStartElement("name");
                            writer.WriteValue(aa);
                            writer.WriteEndElement(); // name
                            writer.WriteEndElement(); // actor
                        }
                    }
                }

                WriteInfo(writer, this.SI, "ContentRating", "mpaa");
                WriteInfo(writer, this.SI, "IMDB_ID", "id");

                writer.WriteStartElement("tvdbid");
                writer.WriteValue(this.SI.TheSeries().TVDBCode);
                writer.WriteEndElement();

                string rt = this.SI.TheSeries().GetItem("Runtime");
                if (!string.IsNullOrEmpty(rt))
                {
                    writer.WriteStartElement("runtime");
                    writer.WriteValue(rt + " minutes");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // tvshow
            }

            writer.Close();
            this.Done = true;
            return true;
        }
    }

    public class TorrentEntry
    {
        public string DownloadingTo;
        public int PercentDone;
        public string TorrentFile;

        public TorrentEntry(string torrentfile, string to, int percent)
        {
            this.TorrentFile = torrentfile;
            this.DownloadingTo = to;
            this.PercentDone = percent;
        }
    }

    public class ActionuTorrenting : ActionItem
    {
        public string DesiredLocationNoExt;
        public TorrentEntry Entry;

        public ActionuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt)
            : base(ActionType.kuTorrenting, pe)
        {
            this.DesiredLocationNoExt = desiredLocationNoExt;
            this.Entry = te;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(this.DesiredLocationNoExt))
                return null;
            else
                return new IgnoreItem(this.DesiredLocationNoExt);
        }

        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(this.Entry.DownloadingTo))
                return null;
            else
                return new FileInfo(this.Entry.DownloadingTo).DirectoryName;
        }

        public override string FilenameForProgress()
        {
            return "";
        }

        public bool SameAs2(ActionuTorrenting o)
        {
            return (o.Entry == this.Entry);
        }

        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && this.SameAs2((ActionuTorrenting) (o));
        }

        public override int IconNumber()
        {
            return 2;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = this.PE.SI.ShowName();
            lvi.SubItems.Add(this.PE.SeasonNumber.ToString());
            lvi.SubItems.Add(this.PE.NumsAsString());
            DateTime? dt = this.PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(this.Entry.TorrentFile);
            lvi.SubItems.Add(this.Entry.DownloadingTo);
            int p = this.Entry.PercentDone;
            lvi.SubItems.Add(p == -1 ? "" : this.Entry.PercentDone + "% Complete");

            lvi.Group = lv.Groups[7];
            lvi.Tag = this;

            //	lv->Items->Add(lvi);
            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            this.Done = true;
            return true;
        }
    }

    public class ActionSorter : System.Collections.Generic.IComparer<ActionItem>
    {
        #region IComparer<ActionItem> Members

        public virtual int Compare(ActionItem x, ActionItem y)
        {
            if (x.Type == y.Type)
            {
                if (x.Type == ActionType.kCopyMoveRename)
                {
                    ActionCopyMoveRename xx = (ActionCopyMoveRename) (x);
                    ActionCopyMoveRename yy = (ActionCopyMoveRename) (y);

                    string s1 = xx.From.FullName + (xx.From.Directory.Root.FullName != xx.To.Directory.Root.FullName ? "0" : "1");
                    string s2 = yy.From.FullName + (yy.From.Directory.Root.FullName != yy.To.Directory.Root.FullName ? "0" : "1");

                    return s1.CompareTo(s2);
                }
                if (x.Type == ActionType.kDownload)
                {
                    ActionDownload xx = (ActionDownload) (x);
                    ActionDownload yy = (ActionDownload) (y);

                    return xx.Destination.FullName.CompareTo(yy.Destination.FullName);
                }
                if (x.Type == ActionType.kRSS)
                {
                    ActionRSS xx = (ActionRSS) (x);
                    ActionRSS yy = (ActionRSS) (y);

                    return xx.RSS.URL.CompareTo(yy.RSS.URL);
                }
                if (x.Type == ActionType.kMissing)
                {
                    ActionMissing xx = (ActionMissing) (x);
                    ActionMissing yy = (ActionMissing) (y);

                    return (xx.TheFileNoExt + xx.PE.Name).CompareTo(yy.TheFileNoExt + yy.PE.Name);
                }
                if (x.Type == ActionType.kNFO)
                {
                    ActionNFO xx = (ActionNFO) (x);
                    ActionNFO yy = (ActionNFO) (y);

                    if (xx.PE == null)
                        return 1;
                    else if (yy.PE == null)
                        return -1;
                    else
                        return (xx.Where.FullName + xx.PE.Name).CompareTo(yy.Where.FullName + yy.PE.Name);
                }
                if (x.Type == ActionType.kuTorrenting)
                {
                    ActionuTorrenting xx = (ActionuTorrenting) (x);
                    ActionuTorrenting yy = (ActionuTorrenting) (y);

                    if (xx.PE == null)
                        return 1;
                    else if (yy.PE == null)
                        return -1;
                    else
                        return (xx.DesiredLocationNoExt).CompareTo(yy.DesiredLocationNoExt);
                }
                System.Diagnostics.Debug.Fail("Unknown type in ActionItem::Compare"); // uhoh
                return 1;
            }
            else
            {
                // different types
                return ((int) x.Type - (int) y.Type);
            }
        }

        #endregion
    }

    public class LVResults
    {
        #region WhichResults enum

        public enum WhichResults
        {
            Checked,
            Selected,
            All
        } ;

        #endregion

        public bool AllSameType;

        public System.Collections.Generic.List<ActionCopyMoveRename> CopyMove;
        public int Count;
        public System.Collections.Generic.List<ActionDownload> Download;
        public System.Collections.Generic.List<ActionItem> FlatList;
        public System.Collections.Generic.List<ActionMissing> Missing;
        public System.Collections.Generic.List<ActionNFO> NFO;
        public System.Collections.Generic.List<ActionRSS> RSS;
        public System.Collections.Generic.List<ActionCopyMoveRename> Rename;
        public System.Collections.Generic.List<ActionuTorrenting> uTorrenting;

        public LVResults(ListView lv, bool @checked) // if not checked, then selected items
        {
            this.Go(lv, @checked ? WhichResults.Checked : WhichResults.Selected);
        }

        public LVResults(ListView lv, WhichResults which)
        {
            this.Go(lv, which);
        }

        public void Go(ListView lv, WhichResults which)
        {
            this.uTorrenting = new System.Collections.Generic.List<ActionuTorrenting>();
            this.Missing = new System.Collections.Generic.List<ActionMissing>();
            this.RSS = new System.Collections.Generic.List<ActionRSS>();
            this.CopyMove = new System.Collections.Generic.List<ActionCopyMoveRename>();
            this.Rename = new System.Collections.Generic.List<ActionCopyMoveRename>();
            this.Download = new System.Collections.Generic.List<ActionDownload>();
            this.NFO = new System.Collections.Generic.List<ActionNFO>();
            this.FlatList = new System.Collections.Generic.List<ActionItem>();

            System.Collections.Generic.List<ListViewItem> sel = new System.Collections.Generic.List<ListViewItem>();
            if (which == WhichResults.Checked)
            {
                ListView.CheckedListViewItemCollection ss = lv.CheckedItems;
                foreach (ListViewItem lvi in ss)
                    sel.Add(lvi);
            }
            else if (which == WhichResults.Selected)
            {
                ListView.SelectedListViewItemCollection ss = lv.SelectedItems;
                foreach (ListViewItem lvi in ss)
                    sel.Add(lvi);
            }
            else // all
            {
                foreach (ListViewItem lvi in lv.Items)
                    sel.Add(lvi);
            }

            this.Count = sel.Count;

            if (sel.Count == 0)
                return;

            ActionType t = ((ActionItem) (sel[0].Tag)).Type;

            this.AllSameType = true;
            foreach (ListViewItem lvi in sel)
            {
                if (lvi == null)
                    continue;

                ActionItem Action = (ActionItem) (lvi.Tag);
                this.FlatList.Add(Action);
                ActionType t2 = Action.Type;
                if (t2 != t)
                    this.AllSameType = false;

                switch (t2)
                {
                    case ActionType.kCopyMoveRename:
                        {
                            ActionCopyMoveRename cmr = (ActionCopyMoveRename) (Action);
                            if (cmr.Operation == ActionCopyMoveRename.Op.Rename)
                                this.Rename.Add((ActionCopyMoveRename) (Action));
                            else // copy/move
                                this.CopyMove.Add((ActionCopyMoveRename) (Action));
                            break;
                        }
                    case ActionType.kDownload:
                        this.Download.Add((ActionDownload) (Action));
                        break;
                    case ActionType.kRSS:
                        this.RSS.Add((ActionRSS) (Action));
                        break;
                    case ActionType.kMissing:
                        this.Missing.Add((ActionMissing) (Action));
                        break;
                    case ActionType.kNFO:
                        this.NFO.Add((ActionNFO) (Action));
                        break;
                    case ActionType.kuTorrenting:
                        this.uTorrenting.Add((ActionuTorrenting) (Action));
                        break;
                }
            }
        }
    }
}
