//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

// Derivatives of "ActionItem" are the actions to do, as a result of doing a scan on the "Scan" tab.
// An "IgnoreItem" represents a file/episode to never ask the user about again. (Right-click->Ignore Selected / Options->Ignore List)

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace TVRename
{
    public enum ActionType : int
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
                FileAndPath = r.ReadElementContentAsString();
        }
        public IgnoreItem(string fileAndPath)
        {
            FileAndPath = fileAndPath;
        }

        public bool SameFileAs(IgnoreItem o)
        {
            if (string.IsNullOrEmpty(FileAndPath) || string.IsNullOrEmpty(o.FileAndPath))
                return false;
            return FileAndPath == o.FileAndPath;
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Ignore");
            writer.WriteValue(FileAndPath);
            writer.WriteEndElement(); // Ignore
        }
        public void Read(XmlReader r)
        {

        }

    }


    public abstract class ActionItem
    {
        public ProcessedEpisode PE; // can be null if not applicable or known

        public bool Done;
        public string ErrorText;
        public bool HasError;

        public ActionType Type;

        public abstract IgnoreItem GetIgnore();

        public abstract ListViewItem GetLVI(ListView lv);

        public abstract string FilenameForProgress();

        public abstract string TargetFolder(); // nullptr if none, otherwise folder "of interest" for this item
        // e.g. where file is missing from, or downloader is downloading to

        public virtual bool Action(TVDoc doc)
        {
            // default is to do nothing
            // also set Done
            Done = true;
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

        protected ActionItem(ActionType t, ProcessedEpisode pe)
        {
            PE = pe;
            Done = false;
            Type = t;
            HasError = false;
            ErrorText = "";
        }

    }

    public class ActionDownload : ActionItem
    {
        public ShowItem SI;
        public FileInfo Destination;
        public string BannerPath;

        public ActionDownload(ShowItem si, ProcessedEpisode pe, FileInfo dest, string bannerPath)
            : base(ActionType.kDownload, pe)
        {
            Destination = dest;
            SI = si;
            BannerPath = bannerPath;
        }
        public override IgnoreItem GetIgnore()
        {
            if (Destination == null)
                return null;
            else
                return new IgnoreItem(Destination.FullName);
        }
        public override string TargetFolder()
        {
            if (Destination == null)
                return null;
            else
                return Destination.DirectoryName;
        }
        public override string FilenameForProgress()
        {
            return Destination.Name;
        }
        public bool SameAs2(ActionDownload o)
        {
            return (o.Destination == Destination);
        }
        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && SameAs2((ActionDownload)(o));
        }
        public override int IconNumber()
        {
            return 5;
        }
        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = SI.ShowName();
            lvi.SubItems.Add(PE != null ? PE.SeasonNumber.ToString() : "");
            lvi.SubItems.Add(PE != null ? PE.NumsAsString() : "");

            if (PE != null)
            {
                DateTime? dt = PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(Destination.DirectoryName);
            lvi.SubItems.Add(BannerPath);

            if (string.IsNullOrEmpty(BannerPath))
                lvi.BackColor = Helpers.WarningColor();

            lvi.SubItems.Add(Destination.Name);

            lvi.Tag = this;
            lvi.Group = lv.Groups[5]; // download image
            // lv->Items->Add(lvi);

            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            byte[] theData = SI.TVDB.GetPage(BannerPath, false, typeMaskBits.tmBanner, false);
            if (theData == null)
            {
                ErrorText = "Unable to download " + BannerPath;
                HasError = true;
                return false;
            }

            FileStream fs = new FileStream(Destination.FullName, FileMode.Create);
            fs.Write(theData, 0, theData.Length);
            fs.Close();

            Done = true;
            return true;
        }
    }

    public class ActionCopyMoveRename : ActionItem
    {
        public FileInfo From;
        public FileInfo To;

        public enum Op : int
        {
            Copy,
            Move,
            Rename
        }

        public Op Operation;

        public bool IsMoveRename() // same thing to the OS
        {
            return ((Operation == Op.Move) || (Operation == Op.Rename));
        }

        public override IgnoreItem GetIgnore()
        {
            if (To == null)
                return null;
            else
                return new IgnoreItem(To.FullName);
        }

        public ActionCopyMoveRename(Op operation, FileInfo from, FileInfo to, ProcessedEpisode ep)
            : base(ActionType.kCopyMoveRename, ep)
        {
            Operation = operation;
            From = from;
            To = to;
        }
        public override int IconNumber()
        {
            return (IsMoveRename() ? 4 : 3);
        }
        public override string TargetFolder()
        {
            if (To == null)
                return null;
            else
                return To.DirectoryName;
        }
        public override string FilenameForProgress()
        {
            return To.Name;
        }
        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            if (PE == null)
            {
                lvi.Text = "";
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }
            else
            {
                lvi.Text = PE.TheSeries.Name;
                lvi.SubItems.Add(PE.SeasonNumber.ToString());
                lvi.SubItems.Add(PE.NumsAsString());
                DateTime? dt = PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }

            lvi.SubItems.Add(From.DirectoryName);
            lvi.SubItems.Add(From.Name);
            lvi.SubItems.Add(To.DirectoryName);
            lvi.SubItems.Add(To.Name);

            if (Operation == Op.Rename)
                lvi.Group = lv.Groups[1];
            else if (Operation == Op.Copy)
                lvi.Group = lv.Groups[2];
            else if (Operation == Op.Move)
                lvi.Group = lv.Groups[3];

            //lv->Items->Add(lvi);
            return lvi;
        }

        public bool SameSource(ActionCopyMoveRename o)
        {
            return (Helpers.Same(From, o.From));
        }
        public bool SameAs2(ActionCopyMoveRename o)
        {
            return ((Operation == o.Operation) && Helpers.Same(From, o.From) && Helpers.Same(To, o.To));
        }
        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && SameAs2((ActionCopyMoveRename)(o));
        }

        public long FileSize()
        {
            try
            {
                return From.Length;
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
            TheFileNoExt = whereItShouldBeNoExt;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(TheFileNoExt))
                return null;
            else
                return new IgnoreItem(TheFileNoExt);
        }

        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(TheFileNoExt))
                return null;
            else
                return new FileInfo(TheFileNoExt).DirectoryName;
        }
        public override string FilenameForProgress()
        {
            return PE.Name;
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
            return string.Compare(o.TheFileNoExt, TheFileNoExt) == 0;
        }
        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && SameAs2((ActionMissing)(o));
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = PE.SI.ShowName();
            lvi.SubItems.Add(PE.SeasonNumber.ToString());
            lvi.SubItems.Add(PE.NumsAsString());

            DateTime? dt = PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            FileInfo fi = new FileInfo(TheFileNoExt);
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
            PE = pe;
            RSS = rss;
            TheFileNoExt = toWhereNoExt;
        }
        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(TheFileNoExt))
                return null;
            else
                return new IgnoreItem(TheFileNoExt);
        }
        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(TheFileNoExt))
                return null;
            else
                return new FileInfo(TheFileNoExt).DirectoryName;
        }
        public override string FilenameForProgress()
        {
            return RSS.Title;
        }
        public bool SameAs2(ActionRSS o)
        {
            return (o.RSS == RSS);
        }
        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && SameAs2((ActionRSS)(o));
        }
        public override int IconNumber()
        {
            return 6;
        }
        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = PE.SI.ShowName();
            lvi.SubItems.Add(PE.SeasonNumber.ToString());
            lvi.SubItems.Add(PE.NumsAsString());
            DateTime? dt = PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(TheFileNoExt);
            lvi.SubItems.Add(RSS.Title);

            lvi.Group = lv.Groups[4];
            lvi.Tag = this;

            // lv->Items->Add(lvi);
            return lvi;
        }

        override public bool Action(TVDoc doc)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            try
            {
                byte[] r = wc.DownloadData(RSS.URL);
                if ((r == null) || (r.Length == 0))
                {
                    HasError = true;
                    ErrorText = "No data downloaded";
                }

                string saveTemp = Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar.ToString() + doc.FilenameFriendly(RSS.Title);
                if (new FileInfo(saveTemp).Extension.ToLower() != "torrent")
                    saveTemp += ".torrent";
                File.WriteAllBytes(saveTemp, r);

                System.Diagnostics.Process.Start(doc.Settings.uTorrentPath, "/directory \"" + (new FileInfo(TheFileNoExt).Directory.FullName) + "\" \"" + saveTemp + "\"");

                HasError = false;
            }
            catch (Exception e)
            {
                ErrorText = e.Message;
                HasError = true;
            }
            Done = true;

            return !HasError;
        }

    }


    public class ActionNFO : ActionItem
    {
        public FileInfo Where;
        public ShowItem SI; // if for an entire show, rather than specific episode

        public ActionNFO(FileInfo nfo, ProcessedEpisode pe)
            : base(ActionType.kNFO, pe)
        {
            SI = null;
            Where = nfo;
        }
        public override IgnoreItem GetIgnore()
        {

            if (Where == null)
                return null;
            else
                return new IgnoreItem(Where.FullName);
        }
        public ActionNFO(FileInfo nfo, ShowItem si)
            : base(ActionType.kNFO, null)
        {
            SI = si;
            Where = nfo;
        }
        public override string TargetFolder()
        {
            if (Where == null)
                return null;
            else
                return Where.DirectoryName;
        }
        public override string FilenameForProgress()
        {
            return Where.Name;
        }
        public bool SameAs2(ActionNFO o)
        {
            return (o.Where == Where);
        }
        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && SameAs2((ActionNFO)(o));
        }
        public override int IconNumber()
        {
            return 7;
        }

        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            if (PE != null)
            {
                lvi.Text = PE.SI.ShowName();
                lvi.SubItems.Add(PE.SeasonNumber.ToString());
                lvi.SubItems.Add(PE.NumsAsString());
                DateTime? dt = PE.GetAirDateDT(true);
                if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue)) != 0)
                    lvi.SubItems.Add(dt.Value.ToShortDateString());
                else
                    lvi.SubItems.Add("");
            }
            else
            {
                lvi.Text = SI.ShowName();
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
            }

            lvi.SubItems.Add(Where.DirectoryName);
            lvi.SubItems.Add(Where.Name);

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
            XmlWriter writer = XmlWriter.Create(Where.FullName, settings);

            if (PE != null) // specific episode
            {
                // See: http://xbmc.org/wiki/?title=Import_-_Export_Library#TV_Episodes
                writer.WriteStartElement("episodedetails");
                writer.WriteStartElement("title");
                writer.WriteValue(PE.Name);
                writer.WriteEndElement();
                writer.WriteStartElement("season");
                writer.WriteValue(PE.SeasonNumber);
                writer.WriteEndElement();
                writer.WriteStartElement("episode");
                writer.WriteValue(PE.EpNum);
                writer.WriteEndElement();
                writer.WriteStartElement("plot");
                writer.WriteValue(PE.Overview);
                writer.WriteEndElement();
                writer.WriteStartElement("aired");
                if (PE.FirstAired != null)
                    writer.WriteValue(PE.FirstAired.Value.ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
                writer.WriteEndElement(); // episodedetails
            }
            else if (SI != null) // show overview (tvshow.nfo)
            {
                // http://www.xbmc.org/wiki/?title=Import_-_Export_Library#TV_Shows

                writer.WriteStartElement("tvshow");

                writer.WriteStartElement("title");
                writer.WriteValue(SI.ShowName());
                writer.WriteEndElement();

                writer.WriteStartElement("episodeguideurl");
                writer.WriteValue(SI.TVDB.BuildURL(true, true, SI.TVDBCode, SI.TVDB.PreferredLanguage(SI.TVDBCode)));
                writer.WriteEndElement();

                WriteInfo(writer, SI, "Overview", "plot");

                string genre = SI.TheSeries().GetItem("Genre");
                if (!string.IsNullOrEmpty(genre))
                {
                    genre = genre.Trim('|');
                    genre = genre.Replace("|", " / ");
                    writer.WriteStartElement("genre");
                    writer.WriteValue(genre);
                    writer.WriteEndElement();
                }

                WriteInfo(writer, SI, "FirstAired", "premiered");
                WriteInfo(writer, SI, "Rating", "rating");

                // actors...
                string actors = SI.TheSeries().GetItem("Actors");
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

                WriteInfo(writer, SI, "ContentRating", "mpaa");
                WriteInfo(writer, SI, "IMDB_ID", "id");

                writer.WriteStartElement("tvdbid");
                writer.WriteValue(SI.TheSeries().TVDBCode);
                writer.WriteEndElement();

                string rt = SI.TheSeries().GetItem("Runtime");
                if (!string.IsNullOrEmpty(rt))
                {
                    writer.WriteStartElement("runtime");
                    writer.WriteValue(rt + " minutes");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // tvshow
            }

            writer.Close();
            Done = true;
            return true;
        }
    }

    public class TorrentEntry
    {
        public string TorrentFile;
        public string DownloadingTo;
        public int PercentDone;

        public TorrentEntry(string torrentfile, string to, int percent)
        {
            TorrentFile = torrentfile;
            DownloadingTo = to;
            PercentDone = percent;
        }
    }


    public class ActionuTorrenting : ActionItem
    {
        public TorrentEntry Entry;
        public string DesiredLocationNoExt;

        public ActionuTorrenting(TorrentEntry te, ProcessedEpisode pe, string desiredLocationNoExt)
            : base(ActionType.kuTorrenting, pe)
        {
            DesiredLocationNoExt = desiredLocationNoExt;
            Entry = te;
        }

        public override IgnoreItem GetIgnore()
        {
            if (string.IsNullOrEmpty(DesiredLocationNoExt))
                return null;
            else
                return new IgnoreItem(DesiredLocationNoExt);
        }
        public override string TargetFolder()
        {
            if (string.IsNullOrEmpty(Entry.DownloadingTo))
                return null;
            else
                return new FileInfo(Entry.DownloadingTo).DirectoryName;
        }
        public override string FilenameForProgress()
        {
            return "";
        }
        public bool SameAs2(ActionuTorrenting o)
        {
            return (o.Entry == Entry);
        }
        public override bool SameAs(ActionItem o)
        {
            return (this.Type == o.Type) && SameAs2((ActionuTorrenting)(o));
        }
        public override int IconNumber()
        {
            return 2;
        }


        public override ListViewItem GetLVI(ListView lv)
        {
            ListViewItem lvi = new ListViewItem();

            lvi.Text = PE.SI.ShowName();
            lvi.SubItems.Add(PE.SeasonNumber.ToString());
            lvi.SubItems.Add(PE.NumsAsString());
            DateTime? dt = PE.GetAirDateDT(true);
            if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                lvi.SubItems.Add(dt.Value.ToShortDateString());
            else
                lvi.SubItems.Add("");

            lvi.SubItems.Add(Entry.TorrentFile);
            lvi.SubItems.Add(Entry.DownloadingTo);
            int p = Entry.PercentDone;
            lvi.SubItems.Add(p == -1 ? "" : Entry.PercentDone.ToString() + "% Complete");

            lvi.Group = lv.Groups[7];
            lvi.Tag = this;

            //	lv->Items->Add(lvi);
            return lvi;
        }

        public override bool Action(TVDoc doc)
        {
            Done = true;
            return true;
        }
    }



    public class ActionSorter : System.Collections.Generic.IComparer<ActionItem>
    {
        public virtual int Compare(ActionItem x, ActionItem y)
        {
            if (x.Type == y.Type)
            {
                if (x.Type == ActionType.kCopyMoveRename)
                {
                    ActionCopyMoveRename xx = (ActionCopyMoveRename)(x);
                    ActionCopyMoveRename yy = (ActionCopyMoveRename)(y);

                    string s1 = xx.From.FullName + (xx.From.Directory.Root.FullName != xx.To.Directory.Root.FullName ? "0" : "1");
                    string s2 = yy.From.FullName + (yy.From.Directory.Root.FullName != yy.To.Directory.Root.FullName ? "0" : "1");

                    return s1.CompareTo(s2);
                }
                if (x.Type == ActionType.kDownload)
                {
                    ActionDownload xx = (ActionDownload)(x);
                    ActionDownload yy = (ActionDownload)(y);

                    return xx.Destination.FullName.CompareTo(yy.Destination.FullName);
                }
                if (x.Type == ActionType.kRSS)
                {
                    ActionRSS xx = (ActionRSS)(x);
                    ActionRSS yy = (ActionRSS)(y);

                    return xx.RSS.URL.CompareTo(yy.RSS.URL);
                }
                if (x.Type == ActionType.kMissing)
                {
                    ActionMissing xx = (ActionMissing)(x);
                    ActionMissing yy = (ActionMissing)(y);

                    return (xx.TheFileNoExt + xx.PE.Name).CompareTo(yy.TheFileNoExt + yy.PE.Name);
                }
                if (x.Type == ActionType.kNFO)
                {
                    ActionNFO xx = (ActionNFO)(x);
                    ActionNFO yy = (ActionNFO)(y);

                    if (xx.PE == null)
                        return 1;
                    else if (yy.PE == null)
                        return -1;
                    else
                        return (xx.Where.FullName + xx.PE.Name).CompareTo(yy.Where.FullName + yy.PE.Name);
                }
                if (x.Type == ActionType.kuTorrenting)
                {
                    ActionuTorrenting xx = (ActionuTorrenting)(x);
                    ActionuTorrenting yy = (ActionuTorrenting)(y);

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
                return ((int)x.Type - (int)y.Type);
            }
        }

    }


    public class LVResults
    {
        public System.Collections.Generic.List<ActionuTorrenting> uTorrenting;
        public System.Collections.Generic.List<ActionMissing> Missing;
        public System.Collections.Generic.List<ActionRSS> RSS;
        public System.Collections.Generic.List<ActionCopyMoveRename> Rename;
        public System.Collections.Generic.List<ActionCopyMoveRename> CopyMove;
        public System.Collections.Generic.List<ActionDownload> Download;
        public System.Collections.Generic.List<ActionNFO> NFO;
        public System.Collections.Generic.List<ActionItem> FlatList;
        public bool AllSameType;
        public int Count;

        public enum WhichResults { Checked, Selected, All };
        public LVResults(ListView lv, bool @checked) // if not checked, then selected items
        {
            Go(lv, @checked ? WhichResults.Checked : WhichResults.Selected);
        }
        public LVResults(ListView lv, WhichResults which)
        {
            Go(lv, which);
        }
        public void Go(ListView lv, WhichResults which)
        {
            uTorrenting = new System.Collections.Generic.List<ActionuTorrenting>();
            Missing = new System.Collections.Generic.List<ActionMissing>();
            RSS = new System.Collections.Generic.List<ActionRSS>();
            CopyMove = new System.Collections.Generic.List<ActionCopyMoveRename>();
            Rename = new System.Collections.Generic.List<ActionCopyMoveRename>();
            Download = new System.Collections.Generic.List<ActionDownload>();
            NFO = new System.Collections.Generic.List<ActionNFO>();
            FlatList = new System.Collections.Generic.List<ActionItem>();

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
            
            Count = sel.Count;

            if (sel.Count == 0)
                return;

            ActionType t = ((ActionItem)(sel[0].Tag)).Type;

            AllSameType = true;
            foreach (ListViewItem lvi in sel)
            {
                if (lvi == null)
                    continue;

                ActionItem Action = (ActionItem)(lvi.Tag);
                FlatList.Add(Action);
                ActionType t2 = Action.Type;
                if (t2 != t)
                    AllSameType = false;

                switch (t2)
                {
                    case ActionType.kCopyMoveRename:
                        {
                            ActionCopyMoveRename cmr = (ActionCopyMoveRename)(Action);
                            if (cmr.Operation == ActionCopyMoveRename.Op.Rename)
                                Rename.Add((ActionCopyMoveRename)(Action));
                            else // copy/move
                                CopyMove.Add((ActionCopyMoveRename)(Action));
                            break;
                        }
                    case ActionType.kDownload:
                        Download.Add((ActionDownload)(Action));
                        break;
                    case ActionType.kRSS:
                        RSS.Add((ActionRSS)(Action));
                        break;
                    case ActionType.kMissing:
                        Missing.Add((ActionMissing)(Action));
                        break;
                    case ActionType.kNFO:
                        NFO.Add((ActionNFO)(Action));
                        break;
                    case ActionType.kuTorrenting:
                        uTorrenting.Add((ActionuTorrenting)(Action));
                        break;
                }
            }
        }
    }
} // namespace