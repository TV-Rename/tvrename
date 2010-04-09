// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Threading;
using System.IO;

namespace TVRename
{
    // right click commands
    public enum RightClickCommands
    {
        None = 0,
        kEpisodeGuideForShow = 1,
        kVisitTVDBEpisode,
        kVisitTVDBSeason,
        kVisitTVDBSeries,
        kScanSpecificSeries,
        kWhenToWatchSeries,
        kForceRefreshSeries,
        kBTSearchFor,
        kActionIgnore,
        kActionBrowseForFile,
        kActionAction,
        kActionDelete,
        kActionIgnoreSeason,
        kWatchBase = 1000,
        kOpenFolderBase = 2000
    }

    /// <summary>
    /// Summary for UI
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class UI : Form
    {
        #region Delegates

        public delegate void IPCDelegate();

        #endregion

        protected int Busy;
        
        public IPCDelegate IPCBringToForeground;
        public IPCDelegate IPCDoAll;
        public IPCDelegate IPCQuit;
        public IPCDelegate IPCScan;
        protected bool InternalCheckChange;
        private int LastDLRemaining;

        public SetProgressDelegate SetProgress;
        private MyListView lvAction;
        protected TVDoc mDoc;
        protected StringList mFoldersToOpen;
        protected int mInternalChange;
        protected System.Collections.Generic.List<ActionItem> mLastActionsClicked;
        protected ProcessedEpisode mLastEpClicked;
        protected System.Collections.Generic.List<FileInfo> mLastFL;
        protected string mLastFolderClicked;
        protected Point mLastNonMaximizedLocation;
        protected Size mLastNonMaximizedSize;
        protected Season mLastSeasonClicked;
        protected ShowItem mLastShowClicked;

        public UI(TVDoc doc)
        {
            this.mDoc = doc;

            this.Busy = 0;
            this.mLastEpClicked = null;
            this.mLastFolderClicked = null;
            this.mLastSeasonClicked = null;
            this.mLastShowClicked = null;
            this.mLastActionsClicked = null;

            this.mInternalChange = 0;
            this.mFoldersToOpen = new StringList();

            this.InternalCheckChange = false;

            this.InitializeComponent();

            this.SetupIPC();

            try
            {
                this.LoadLayoutXML();
            }
            catch
            {
                // silently fail, doesn't matter too much
            }

            this.SetProgress += this.SetProgressActual;

            this.lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();

            if (this.mDoc.Args.Hide)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.Hide();
            }

            this.Text = this.Text + " " + Version.DisplayVersionString();

            this.FillShowLists();
            this.UpdateSearchButton();
            this.SetGuideHTMLbody("");
            this.mDoc.DoWhenToWatch(true);
            this.FillWhenToWatchList();
            this.mDoc.WriteUpcomingRSS();
            this.ShowHideNotificationIcon();

            int t = this.mDoc.Settings.StartupTab;
            if (t < this.tabControl1.TabCount)
                this.tabControl1.SelectedIndex = this.mDoc.Settings.StartupTab;
            this.tabControl1_SelectedIndexChanged(null, null);
        }

        public static int BGDLLongInterval()
        {
            return 1000 * 60 * 60; // one hour
        }

        protected void MoreBusy()
        {
            Interlocked.Increment(ref this.Busy);
        }

        protected void LessBusy()
        {
            Interlocked.Decrement(ref this.Busy);
        }

        private void SetupIPC()
        {
            this.IPCBringToForeground += this.ShowYourself;
            this.IPCScan += this.ScanAll;
            this.IPCDoAll += this.ActionAll;
            this.IPCQuit += this.Close;

            //Instantiate our server channel.
            var channel = new IpcServerChannel("TVRenameChannel");

            //Register the server channel.
            ChannelServices.RegisterChannel(channel, true);

            //Register this service type.
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IPCMethods), "IPCMethods", WellKnownObjectMode.Singleton);

            IPCMethods.Setup(this, this.mDoc);
        }

        public void SetProgressActual(int p)
        {
            if (p < 0)
                p = 0;
            else if (p > 100)
                p = 100;

            this.pbProgressBarx.Value = p;
            this.pbProgressBarx.Update();
        }

        public void ProcessArgs()
        {
            // TODO: Unify command line handling between here and in Program.cs

            if (this.mDoc.Args.Scan || this.mDoc.Args.DoAll) // doall implies scan
                this.ScanAll();
            if (this.mDoc.Args.DoAll)
                this.ActionAll();
            if (this.mDoc.Args.Quit || this.mDoc.Args.Hide)
                this.Close();
        }


        ~UI()
        {
            //		mDoc->StopBGDownloadThread();  TODO
            this.mDoc = null;
        }

        public void UpdateSearchButton()
        {
            string name = this.mDoc.GetSearchers().Name(this.mDoc.Settings.TheSearchers.CurrentSearchNum());
            this.bnWTWBTSearch.Text = name;
            this.bnActionBTSearch.Text = name;
            this.FillEpGuideHTML();
        }

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            TVDoc.SysOpen("http://tvrename.com");
        }

        private void UI_Load(object sender, System.EventArgs e)
        {
            this.ShowInTaskbar = this.mDoc.Settings.ShowInTaskbar && !this.mDoc.Args.Hide;

            foreach (TabPage tp in this.tabControl1.TabPages) // grr! TODO: why does it go white?
                tp.BackColor = System.Drawing.SystemColors.Control;

            this.Show();
            this.UI_LocationChanged(null, null);
            this.UI_SizeChanged(null, null);

            this.backgroundDownloadToolStripMenuItem.Checked = this.mDoc.Settings.BGDownload;
            this.offlineOperationToolStripMenuItem.Checked = this.mDoc.Settings.OfflineMode;
            this.BGDownloadTimer.Interval = 10000; // first time
            if (this.mDoc.Settings.BGDownload)
                this.BGDownloadTimer.Start();

            this.quickTimer.Start();
        }

        private ListView ListViewByName(string name)
        {
            if (name == "WhenToWatch")
                return this.lvWhenToWatch;
            if (name == "AllInOne")
                return this.lvAction;
            return null;
        }

        private void flushCacheToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            System.Windows.Forms.DialogResult res = MessageBox.Show("Are you sure you want to remove all " + "locally stored TheTVDB information?  This information will have to be downloaded again.  You " + "can force the refresh of a single show by holding down the \"Control\" key while clicking on " + "the \"Refresh\" button in the \"My Shows\" tab.", "Flush Web Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == System.Windows.Forms.DialogResult.Yes)
            {
                this.mDoc.GetTVDB(false, "").ForgetEverything();
                this.FillShowLists();
                this.FillEpGuideHTML();
                this.FillWhenToWatchList();
            }
        }

        private bool LoadWidths(XmlReader xml)
        {
            string forwho = xml.GetAttribute("For");

            ListView lv = this.ListViewByName(forwho);
            if (lv == null)
            {
                xml.ReadOuterXml();
                return true;
            }

            xml.Read();
            int c = 0;
            while (xml.Name == "Width")
            {
                if (c >= lv.Columns.Count)
                    return false;
                lv.Columns[c++].Width = xml.ReadElementContentAsInt();
            }
            xml.Read();
            return true;
        }

        private bool LoadLayoutXML()
        {
            if (this.mDoc.Args.Hide)
                return true;

            bool ok = true;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            string fn = System.Windows.Forms.Application.UserAppDataPath + System.IO.Path.DirectorySeparatorChar + "Layout.xml";
            if (!File.Exists(fn))
                return true;

            XmlReader reader = XmlReader.Create(fn, settings);

            reader.Read();
            if (reader.Name != "xml")
                return false;

            reader.Read();
            if (reader.Name != "TVRename")
                return false;

            if (reader.GetAttribute("Version") != "2.1")
                return false;

            reader.Read();
            if (reader.Name != "Layout")
                return false;

            reader.Read();
            while (reader.Name != "Layout")
            {
                if (reader.Name == "Window")
                {
                    reader.Read();
                    while (reader.Name != "Window")
                    {
                        if (reader.Name == "Size")
                        {
                            int x = int.Parse(reader.GetAttribute("Width"));
                            int y = int.Parse(reader.GetAttribute("Height"));
                            this.Size = new System.Drawing.Size(x, y);
                            reader.Read();
                        }
                        else if (reader.Name == "Location")
                        {
                            int x = int.Parse(reader.GetAttribute("X"));
                            int y = int.Parse(reader.GetAttribute("Y"));
                            this.Location = new Point(x, y);
                            reader.Read();
                        }
                        else if (reader.Name == "Maximized")
                            this.WindowState = (reader.ReadElementContentAsBoolean() ? FormWindowState.Maximized : FormWindowState.Normal);
                        else
                            reader.ReadOuterXml();
                    }
                    reader.Read();
                } // window
                else if (reader.Name == "ColumnWidths")
                    ok = this.LoadWidths(reader) && ok;
                else
                    reader.ReadOuterXml();
            } // while

            reader.Close();
            return ok;
        }

        private bool SaveLayoutXML()
        {
            if (this.mDoc.Args.Hide)
                return true;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(System.Windows.Forms.Application.UserAppDataPath + System.IO.Path.DirectorySeparatorChar + "Layout.xml", settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("TVRename");
            writer.WriteStartAttribute("Version");
            writer.WriteValue("2.1");
            writer.WriteEndAttribute(); // version
            writer.WriteStartElement("Layout");
            writer.WriteStartElement("Window");

            writer.WriteStartElement("Size");
            writer.WriteStartAttribute("Width");
            writer.WriteValue(this.mLastNonMaximizedSize.Width);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Height");
            writer.WriteValue(this.mLastNonMaximizedSize.Height);
            writer.WriteEndAttribute();
            writer.WriteEndElement(); // size

            writer.WriteStartElement("Location");
            writer.WriteStartAttribute("X");
            writer.WriteValue(this.mLastNonMaximizedLocation.X);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("Y");
            writer.WriteValue(this.mLastNonMaximizedLocation.Y);
            writer.WriteEndAttribute();
            writer.WriteEndElement(); // Location

            writer.WriteStartElement("Maximized");
            writer.WriteValue(this.WindowState == FormWindowState.Maximized);
            writer.WriteEndElement(); // maximized

            writer.WriteEndElement(); // window

            this.WriteColWidthsXML("WhenToWatch", writer);
            this.WriteColWidthsXML("AllInOne", writer);

            writer.WriteEndElement(); // Layout
            writer.WriteEndElement(); // tvrename
            writer.WriteEndDocument();

            writer.Close();
            writer = null;

            return true;
        }

        private void WriteColWidthsXML(string thingName, XmlWriter writer)
        {
            ListView lv = this.ListViewByName(thingName);
            if (lv == null)
                return;
            writer.WriteStartElement("ColumnWidths");
            writer.WriteStartAttribute("For");
            writer.WriteValue(thingName);
            writer.WriteEndAttribute();
            foreach (ColumnHeader lvc in lv.Columns)
            {
                writer.WriteStartElement("Width");
                writer.WriteValue(lvc.Width);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // columnwidths
        }

        private void UI_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (this.mDoc.Dirty())
            {
                System.Windows.Forms.DialogResult res = MessageBox.Show("Your changes have not been saved.  Do you wish to save before quitting?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res == System.Windows.Forms.DialogResult.Yes)
                    this.mDoc.WriteXMLSettings();
                else if (res == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;
                else if (res == System.Windows.Forms.DialogResult.No)
                {
                }
            }
            if (!e.Cancel)
            {
                this.SaveLayoutXML();
                this.mDoc.TidyTVDB();
                this.mDoc.Closing();
            }
        }

        private ContextMenuStrip BuildSearchMenu()
        {
            this.menuSearchSites.Items.Clear();
            for (int i = 0; i < this.mDoc.GetSearchers().Count(); i++)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(this.mDoc.GetSearchers().Name(i));
                tsi.Tag = i;
                this.menuSearchSites.Items.Add(tsi);
            }
            return this.menuSearchSites;
        }

        private void ChooseSiteMenu(int n)
        {
            ContextMenuStrip sm = this.BuildSearchMenu();
            if (n == 1)
                sm.Show(this.bnWTWChooseSite, new Point(0, 0));
            else if (n == 0)
                sm.Show(this.bnActionWhichSearch, new Point(0, 0));
        }

        private void bnWTWChooseSite_Click(object sender, System.EventArgs e)
        {
            this.ChooseSiteMenu(1);
        }

        private void bnEpGuideChooseSearch_Click(object sender, System.EventArgs e)
        {
            this.ChooseSiteMenu(2);
        }

        private void FillShowLists()
        {
            Season currentSeas = TreeNodeToSeason(this.MyShowTree.SelectedNode);
            ShowItem currentSI = this.TreeNodeToShowItem(this.MyShowTree.SelectedNode);

            ShowItemList expanded = new ShowItemList();
            foreach (TreeNode n in this.MyShowTree.Nodes)
            {
                if (n.IsExpanded)
                    expanded.Add(this.TreeNodeToShowItem(n));
            }

            this.MyShowTree.BeginUpdate();

            this.MyShowTree.Nodes.Clear();

            System.Collections.Generic.List<ShowItem> sil = this.mDoc.GetShowItems(true);
            foreach (ShowItem si in sil)
            {
                TreeNode tvn = this.AddShowItemToTree(si);
                if (expanded.Contains(si))
                    tvn.Expand();
            }
            this.mDoc.UnlockShowItems();

            foreach (ShowItem si in expanded)
            {
                foreach (TreeNode n in this.MyShowTree.Nodes)
                {
                    if (this.TreeNodeToShowItem(n) == si)
                        n.Expand();
                }
            }

            if (currentSeas != null)
                this.SelectSeason(currentSeas);
            else if (currentSI != null)
                this.SelectShow(currentSI);

            this.MyShowTree.EndUpdate();
        }

        private static string QuickStartGuide()
        {
            return "http://tvrename.com/quickstart-2.2.html";
        }

        private void ShowQuickStartGuide()
        {
            this.tabControl1.SelectTab(this.tbMyShows);
            this.epGuideHTML.Navigate(QuickStartGuide());
        }

        private void FillEpGuideHTML()
        {
            if (this.MyShowTree.Nodes.Count == 0)
                this.ShowQuickStartGuide();
            else
            {
                TreeNode n = this.MyShowTree.SelectedNode;
                this.FillEpGuideHTML(n);
            }
        }

        private ShowItem TreeNodeToShowItem(TreeNode n)
        {
            if (n == null)
                return null;

            ShowItem si = n.Tag as ShowItem;
            if (si != null)
                return si;

            ProcessedEpisode pe = n.Tag as ProcessedEpisode;
            if (pe != null)
                return pe.SI;

            Season seas = n.Tag as Season;
            if (seas != null)
            {
                if (seas.Episodes.Count > 0)
                {
                    int tvdbcode = seas.TheSeries.TVDBCode;
                    foreach (ShowItem si2 in this.mDoc.GetShowItems(true))
                    {
                        if (si2.TVDBCode == tvdbcode)
                        {
                            this.mDoc.UnlockShowItems();
                            return si2;
                        }
                    }
                    this.mDoc.UnlockShowItems();
                }
            }

            return null;
        }

        private static Season TreeNodeToSeason(TreeNode n)
        {
            if (n == null)
                return null;

            Season seas = n.Tag as Season;
            return seas;
        }

        private void FillEpGuideHTML(TreeNode n)
        {
            if (n == null)
            {
                this.FillEpGuideHTML(null, -1);
                return;
            }

            ProcessedEpisode pe = n.Tag as ProcessedEpisode;
            if (pe != null)
            {
                this.FillEpGuideHTML(pe.SI, pe.SeasonNumber);
                return;
            }

            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                // we have a TVDB season, but need to find the equiavlent one in our local processed episode collection
                if (seas.Episodes.Count > 0)
                {
                    int tvdbcode = seas.TheSeries.TVDBCode;
                    foreach (ShowItem si in this.mDoc.GetShowItems(true))
                    {
                        if (si.TVDBCode == tvdbcode)
                        {
                            this.mDoc.UnlockShowItems();
                            this.FillEpGuideHTML(si, seas.SeasonNumber);
                            return;
                        }
                    }
                    this.mDoc.UnlockShowItems();

                    if (pe != null)
                    {
                        this.FillEpGuideHTML(pe.SI, -1);
                        return;
                    }
                }
                this.FillEpGuideHTML(null, -1);
                return;
            }

            this.FillEpGuideHTML(this.TreeNodeToShowItem(n), -1);
        }

        private void FillEpGuideHTML(ShowItem si, int snum)
        {
            if (this.tabControl1.SelectedTab != this.tbMyShows)
                return;

            if (si == null)
            {
                this.SetGuideHTMLbody("");
                return;
            }
            TheTVDB db = this.mDoc.GetTVDB(true, "FillEpGuideHTML");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            if (ser == null)
            {
                this.SetGuideHTMLbody("Not downloaded, or not available");
                return;
            }

            string body = "";

            StringList skip = new StringList();
            skip.Add("Actors");
            skip.Add("banner");
            skip.Add("Overview");
            skip.Add("Airs_Time");
            skip.Add("Airs_DayOfWeek");
            skip.Add("fanart");
            skip.Add("poster");
            skip.Add("zap2it_id");

            if ((snum >= 0) && (ser.Seasons.ContainsKey(snum)))
            {
                if (!string.IsNullOrEmpty(ser.GetItem("banner")) && !string.IsNullOrEmpty(db.BannerMirror))
                    body += "<img width=758 height=140 src=\"" + db.BannerMirror + "/banners/" + ser.GetItem("banner") + "\"><br/>";

                Season s = ser.Seasons[snum];

                ProcessedEpisodeList eis = null;
                // int snum = s.SeasonNumber;
                if (si.SeasonEpisodes.ContainsKey(snum))
                    eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
                else
                    eis = ShowItem.ProcessedListFromEpisodes(s.Episodes, si);

                string seasText = snum == 0 ? "Specials" : ("Season " + snum);
                if ((eis.Count > 0) && (eis[0].SeasonID > 0))
                    seasText = " - <A HREF=\"" + db.WebsiteURL(si.TVDBCode, eis[0].SeasonID, false) + "\">" + seasText + "</a>";
                else
                    seasText = " - " + seasText;

                body += "<h1><A HREF=\"" + db.WebsiteURL(si.TVDBCode, -1, true) + "\">" + si.ShowName() + "</A>" + seasText + "</h1>";

                foreach (ProcessedEpisode ei in eis)
                {
                    string epl = ei.NumsAsString();

                    // http://www.thetvdb.com/?tab=episode&seriesid=73141&seasonid=5356&id=108303&lid=7
                    string episodeURL = "http://www.thetvdb.com/?tab=episode&seriesid=" + ei.SeriesID + "&seasonid=" + ei.SeasonID + "&id=" + ei.EpisodeID;

                    body += "<A href=\"" + episodeURL + "\" name=\"ep" + epl + "\">"; // anchor
                    body += "<b>" + CustomName.NameForNoExt(ei, CustomName.OldNStyle(6)) + "</b>";
                    body += "</A>"; // anchor
                    if (si.UseSequentialMatch && (ei.OverallNumber != -1))
                        body += " (#" + ei.OverallNumber + ")";

                    body += " <A HREF=\"" + this.mDoc.Settings.BTSearchURL(ei) + "\" class=\"search\">Search</A>";

                    System.Collections.Generic.List<System.IO.FileInfo> fl = this.mDoc.FindEpOnDisk(ei);
                    if (fl != null)
                    {
                        foreach (FileInfo fi in fl)
                            body += " <A HREF=\"file://" + fi.FullName + "\" class=\"search\">Watch</A>";
                    }

                    DateTime? dt = ei.GetAirDateDT(true);
                    if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        body += "<p>" + dt.Value.ToShortDateString() + " (" + ei.HowLong() + ")";

                    body += "<p><p>";

                    if (this.mDoc.Settings.ShowEpisodePictures)
                    {
                        body += "<table><tr>";
                        body += "<td width=100% valign=top>" + ei.Overview + "</td><td width=300 height=225>";
                        // 300x168 / 300x225
                        if (!string.IsNullOrEmpty(ei.GetItem("filename")))
                            body += "<img src=" + db.BannerMirror + "/banners/_cache/" + ei.GetItem("filename") + ">";
                        body += "</td></tr></table>";
                    }
                    else
                        body += ei.Overview;

                    body += "<p><hr><p>";
                } // for each episode in this season
            }
            else
            {
                // no epnum specified, just show an overview
                if ((!string.IsNullOrEmpty(ser.GetItem("banner"))) && (!string.IsNullOrEmpty(db.BannerMirror)))
                    body += "<img width=758 height=140 src=\"" + db.BannerMirror + "/banners/" + ser.GetItem("banner") + "\"><br/>";

                body += "<h1><A HREF=\"" + db.WebsiteURL(si.TVDBCode, -1, true) + "\">" + si.ShowName() + "</A> " + "</h1>";

                body += "<h2>Overview</h2>" + ser.GetItem("Overview");

                string actors = ser.GetItem("Actors");
                if (!string.IsNullOrEmpty(actors))
                {
                    bool first = true;
                    foreach (string aa in actors.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(aa))
                        {
                            if (!first)
                                body += ", ";
                            else
                                body += "<h2>Actors</h2>";
                            body += "<A HREF=\"http://www.imdb.com/find?s=nm&q=" + aa + "\">" + aa + "</a>";
                            first = false;
                        }
                    }
                }

                string airsTime = ser.GetItem("Airs_Time");
                string airsDay = ser.GetItem("Airs_DayOfWeek");
                if ((!string.IsNullOrEmpty(airsTime)) && (!string.IsNullOrEmpty(airsDay)))
                {
                    body += "<h2>Airs</h2> " + airsTime + " " + airsDay;
                    string net = ser.GetItem("Network");
                    if (!string.IsNullOrEmpty(net))
                    {
                        skip.Add("Network");
                        body += ", " + net;
                    }
                }

                bool firstInfo = true;
                foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in ser.Items)
                {
                    if (firstInfo)
                    {
                        body += "<h2>Information<table border=0>";
                        firstInfo = false;
                    }
                    if (!skip.Contains(kvp.Key))
                    {
                        if (kvp.Key == "SeriesID")
                            body += "<tr><td width=120px>tv.com</td><td><A HREF=\"http://www.tv.com/show/" + kvp.Value + "/summary.html\">Visit</a></td></tr>";
                        else if (kvp.Key == "IMDB_ID")
                            body += "<tr><td width=120px>imdb.com</td><td><A HREF=\"http://www.imdb.com/title/" + kvp.Value + "\">Visit</a></td></tr>";
                        else
                            body += "<tr><td width=120px>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>";
                    }
                }
                if (!firstInfo)
                    body += "</table>";
            }
            db.Unlock("FillEpGuideHTML");
            this.SetGuideHTMLbody(body);
        }

        // FillEpGuideHTML

        public static string EpGuidePath()
        {
            string tp = Path.GetTempPath();
            return tp + "tvrenameepguide.html";
        }

        public static string EpGuideURLBase()
        {
            return "file://" + EpGuidePath();
        }

        public void SetGuideHTMLbody(string body)
        {
            System.Drawing.Color col = System.Drawing.Color.FromName("ButtonFace");

            string css = "* { font-family: Tahoma, Arial; font-size 10pt; } " + "a:link { color: black } " + "a:visited { color:black } " + "a:hover { color:#000080 } " + "a:active { color:black } " + "a.search:link { color: #800000 } " + "a.search:visited { color:#800000 } " + "a.search:hover { color:#000080 } " + "a.search:active { color:#800000 } " + "* {background-color: #" + col.R.ToString("X2") + col.G.ToString("X2") + col.B.ToString("X2") + "}" + "* { color: black }";

            string html = "<html><head><STYLE type=\"text/css\">" + css + "</style>";

            html += "</head><body>";
            html += body;
            html += "</body></html>";

            this.epGuideHTML.Navigate("about:blank"); // make it close any file it might have open

            string path = EpGuidePath();

            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create));
            bw.Write(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(html));
            bw.Close();

            this.epGuideHTML.Navigate(EpGuideURLBase());
        }

        public void TVDBFor(ProcessedEpisode e)
        {
            if (e == null)
                return;

            TVDoc.SysOpen(this.mDoc.GetTVDB(false, "").WebsiteURL(e.SI.TVDBCode, e.SeasonID, false));
        }

        public void TVDBFor(Season seas)
        {
            if (seas == null)
                return;

            TVDoc.SysOpen(this.mDoc.GetTVDB(false, "").WebsiteURL(seas.TheSeries.TVDBCode, -1, false));
        }

        public void TVDBFor(ShowItem si)
        {
            if (si == null)
                return;

            TVDoc.SysOpen(this.mDoc.GetTVDB(false, "").WebsiteURL(si.TVDBCode, -1, false));
        }

        public void menuSearchSites_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            this.mDoc.SetSearcher((int) (e.ClickedItem.Tag));
            this.UpdateSearchButton();
        }

        public void bnWhenToWatchCheck_Click(object sender, System.EventArgs e)
        {
            this.RefreshWTW(true);
        }

        public void FillWhenToWatchList()
        {
            this.mInternalChange++;
            this.lvWhenToWatch.BeginUpdate();

            int dd = this.mDoc.Settings.WTWRecentDays;

            this.lvWhenToWatch.Groups[0].Header = "Aired in the last " + dd + " day" + ((dd == 1) ? "" : "s");

            // try to maintain selections if we can
            ProcessedEpisodeList selections = new ProcessedEpisodeList();
            foreach (ListViewItem lvi in this.lvWhenToWatch.SelectedItems)
                selections.Add((ProcessedEpisode) (lvi.Tag));

            Season currentSeas = TreeNodeToSeason(this.MyShowTree.SelectedNode);
            ShowItem currentSI = this.TreeNodeToShowItem(this.MyShowTree.SelectedNode);

            this.lvWhenToWatch.Items.Clear();

            System.Collections.Generic.List<DateTime> bolded = new System.Collections.Generic.List<DateTime>();

            foreach (ShowItem si in this.mDoc.GetShowItems(true))
            {
                if (!si.ShowNextAirdate)
                    continue;

                foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in si.SeasonEpisodes)
                {
                    if (si.IgnoreSeasons.Contains(kvp.Key))
                        continue; // ignore this season

                    ProcessedEpisodeList eis = kvp.Value;

                    bool nextToAirFound = false;

                    foreach (ProcessedEpisode ei in eis)
                    {
                        DateTime? dt = ei.GetAirDateDT(true);
                        if ((dt != null) && (dt.Value.CompareTo(DateTime.MaxValue) != 0))
                        {
                            TimeSpan ts = dt.Value.Subtract(DateTime.Now);
                            if (ts.TotalHours >= (-24 * dd)) // in the future (or fairly recent)
                            {
                                bolded.Add(dt.Value);
                                if ((ts.TotalHours >= 0) && (!nextToAirFound))
                                {
                                    nextToAirFound = true;
                                    ei.NextToAir = true;
                                }
                                else
                                    ei.NextToAir = false;

                                ListViewItem lvi = new System.Windows.Forms.ListViewItem();
                                lvi.Text = "";
                                for (int i = 0; i < 7; i++)
                                    lvi.SubItems.Add("");

                                this.UpdateWTW(ei, lvi);

                                this.lvWhenToWatch.Items.Add(lvi);

                                foreach (ProcessedEpisode pe in selections)
                                {
                                    if (pe.SameAs(ei))
                                    {
                                        lvi.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.mDoc.UnlockShowItems();
            this.lvWhenToWatch.Sort();

            this.lvWhenToWatch.EndUpdate();
            this.calCalendar.BoldedDates = bolded.ToArray();

            if (currentSeas != null)
                this.SelectSeason(currentSeas);
            else if (currentSI != null)
                this.SelectShow(currentSI);

            this.UpdateToolstripWTW();
            this.mInternalChange--;
        }

        public void lvWhenToWatch_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            int col = e.Column;
            // 3 4, or 6 = do date sort on 3
            // 1 or 2 = number sort
            // 5 = day sort
            // all others, text sort

            if (col == 6) // straight sort by date
            {
                this.lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                this.lvWhenToWatch.ShowGroups = false;
            }
            else if ((col == 3) || (col == 4))
            {
                this.lvWhenToWatch.ListViewItemSorter = new DateSorterWTW();
                this.lvWhenToWatch.ShowGroups = true;
            }
            else
            {
                this.lvWhenToWatch.ShowGroups = false;
                if ((col == 1) || (col == 2))
                    this.lvWhenToWatch.ListViewItemSorter = new NumberAsTextSorter(col);
                else if (col == 5)
                    this.lvWhenToWatch.ListViewItemSorter = new DaySorter(col);
                else
                    this.lvWhenToWatch.ListViewItemSorter = new TextSorter(col);
            }
            this.lvWhenToWatch.Sort();
        }

        public void lvWhenToWatch_Click(object sender, System.EventArgs e)
        {
            if (this.lvWhenToWatch.SelectedIndices.Count == 0)
            {
                this.txtWhenToWatchSynopsis.Text = "";
                return;
            }
            int n = this.lvWhenToWatch.SelectedIndices[0];

            ProcessedEpisode ei = (ProcessedEpisode) (this.lvWhenToWatch.Items[n].Tag);
            this.txtWhenToWatchSynopsis.Text = ei.Overview;

            this.mInternalChange++;
            DateTime? dt = ei.GetAirDateDT(true);
            if (dt != null)
            {
                this.calCalendar.SelectionStart = (DateTime) dt;
                this.calCalendar.SelectionEnd = (DateTime) dt;
            }
            this.mInternalChange--;

            if (this.mDoc.Settings.AutoSelectShowInMyShows)
                GotoEpguideFor(ei, false);
        }

        public void lvWhenToWatch_DoubleClick(object sender, System.EventArgs e)
        {
            if (this.lvWhenToWatch.SelectedItems.Count > 0)
            {
                ProcessedEpisode ei = (ProcessedEpisode) (this.lvWhenToWatch.SelectedItems[0].Tag);
                System.Collections.Generic.List<System.IO.FileInfo> fl = this.mDoc.FindEpOnDisk(ei);
                if ((fl != null) && (fl.Count > 0))
                {
                    TVDoc.SysOpen(fl[0].FullName);
                    return;
                }
            }

            this.bnWTWBTSearch_Click(null, null);
        }

        public void calCalendar_DateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
        {
            if (this.mInternalChange != 0)
                return;

            DateTime dt = this.calCalendar.SelectionStart;
            for (int i = 0; i < this.lvWhenToWatch.Items.Count; i++)
                this.lvWhenToWatch.Items[i].Selected = false;

            bool first = true;

            for (int i = 0; i < this.lvWhenToWatch.Items.Count; i++)
            {
                ListViewItem lvi = this.lvWhenToWatch.Items[i];
                ProcessedEpisode ei = (ProcessedEpisode) (lvi.Tag);
                DateTime? dt2 = ei.GetAirDateDT(true);
                if (dt2 != null)
                {
                    double h = dt2.Value.Subtract(dt).TotalHours;
                    if ((h >= 0) && (h < 24.0))
                    {
                        lvi.Selected = true;
                        if (first)
                        {
                            lvi.EnsureVisible();
                            first = false;
                        }
                    }
                }
            }
            this.lvWhenToWatch.Focus();
        }

        public void bnEpGuideRefresh_Click(object sender, System.EventArgs e)
        {
            this.bnWhenToWatchCheck_Click(null, null); // close enough!
            this.FillShowLists();
        }

        public void RefreshWTW(bool doDownloads)
        {
            if (doDownloads)
            {
                if (!this.mDoc.DoDownloadsFG())
                    return;
            }

            this.mInternalChange++;
            this.mDoc.DoWhenToWatch(true);
            this.FillShowLists();
            this.FillWhenToWatchList();
            this.mInternalChange--;

            this.mDoc.WriteUpcomingRSS();
        }

        public void refreshWTWTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.Busy == 0)
                this.RefreshWTW(false);
        }

        public void UpdateToolstripWTW()
        {
            // update toolstrip text too
            ProcessedEpisodeList next1 = this.mDoc.NextNShows(1, 36500);

            this.tsNextShowTxt.Text = "Next airing: ";
            if ((next1 != null) && (next1.Count >= 1))
            {
                ProcessedEpisode ei = next1[0];
                this.tsNextShowTxt.Text += CustomName.NameForNoExt(ei, CustomName.OldNStyle(1)) + ", " + ei.HowLong() + " (" + ei.DayOfWeek() + ", " + ei.TimeOfDay() + ")";
            }
            else
                this.tsNextShowTxt.Text += "---";
        }

        public void bnWTWBTSearch_Click(object sender, System.EventArgs e)
        {
            foreach (ListViewItem lvi in this.lvWhenToWatch.SelectedItems)
                this.mDoc.DoBTSearch((ProcessedEpisode) (lvi.Tag));
        }

        public void epGuideHTML_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;
            if (url.Contains("tvrenameepguide.html#ep"))
                return; // don't intercept
            if (url.EndsWith("tvrenameepguide.html"))
                return; // don't intercept
            if (url.CompareTo("about:blank") == 0)
                return; // don't intercept about:blank
            if (url == QuickStartGuide())
                return; // let the quickstartguide be shown

            if ((url.Substring(0, 7).CompareTo("http://") == 0) || (url.Substring(0, 7).CompareTo("file://") == 0))
            {
                e.Cancel = true;
                TVDoc.SysOpen(e.Url.AbsoluteUri);
            }
        }

        public void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            // double-click of notification icon causes a click then doubleclick event, 
            // so we need to do a timeout before showing the single click's popup
            this.tmrShowUpcomingPopup.Start();
        }

        public void tmrShowUpcomingPopup_Tick(object sender, System.EventArgs e)
        {
            this.tmrShowUpcomingPopup.Stop();
            UpcomingPopup UP = new UpcomingPopup(this.mDoc);
            UP.Show();
        }

        public void ShowYourself()
        {
            if (!this.mDoc.Settings.ShowInTaskbar)
                this.Show();
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        public void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            this.tmrShowUpcomingPopup.Stop();
            this.ShowYourself();
        }

        public void buyMeADrinkToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            BuyMeADrink bmad = new BuyMeADrink();
            bmad.ShowDialog();
        }

        public void GotoEpguideFor(ShowItem si, bool changeTab)
        {
            if (changeTab)
                this.tabControl1.SelectTab(this.tbMyShows);
            this.FillEpGuideHTML(si, -1);
        }

        public void GotoEpguideFor(Episode ep, bool changeTab)
        {
            if (changeTab)
                this.tabControl1.SelectTab(this.tbMyShows);

            this.SelectSeason(ep.TheSeason);
        }

        public void RightClickOnMyShows(ShowItem si, Point pt)
        {
            this.mLastShowClicked = si;
            this.mLastEpClicked = null;
            this.mLastSeasonClicked = null;
            this.mLastActionsClicked = null;
            this.BuildRightClickMenu(pt);
        }

        public void RightClickOnMyShows(Season seas, Point pt)
        {
            this.mLastShowClicked = this.mDoc.GetShowItem(seas.TheSeries.TVDBCode);
            this.mLastEpClicked = null;
            this.mLastSeasonClicked = seas;
            this.mLastActionsClicked = null;
            this.BuildRightClickMenu(pt);
        }

        public void RightClickOnShow(ProcessedEpisode ep, Point pt)
        {
            this.mLastEpClicked = ep;
            this.mLastShowClicked = ep != null ? ep.SI : null;
            this.mLastSeasonClicked = ep != null ? ep.TheSeason : null;
            this.mLastActionsClicked = null;
            this.BuildRightClickMenu(pt);
        }

        public void MenuGuideAndTVDB(bool addSep)
        {
            ShowItem si = this.mLastShowClicked;
            Season seas = this.mLastSeasonClicked;
            ProcessedEpisode ep = this.mLastEpClicked;
            ToolStripMenuItem tsi;

            if (si != null)
            {
                if (addSep)
                {
                    this.showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Episode Guide");
                tsi.Tag = (int) RightClickCommands.kEpisodeGuideForShow;
                this.showRightClickMenu.Items.Add(tsi);
            }

            if (ep != null)
            {
                if (addSep)
                {
                    this.showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int) RightClickCommands.kVisitTVDBEpisode;
                this.showRightClickMenu.Items.Add(tsi);
            }
            else if (seas != null)
            {
                if (addSep)
                {
                    this.showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int) RightClickCommands.kVisitTVDBSeason;
                this.showRightClickMenu.Items.Add(tsi);
            }
            else if (si != null)
            {
                if (addSep)
                {
                    this.showRightClickMenu.Items.Add(new ToolStripSeparator());
                    addSep = false;
                }
                tsi = new ToolStripMenuItem("Visit thetvdb.com");
                tsi.Tag = (int) RightClickCommands.kVisitTVDBSeries;
                this.showRightClickMenu.Items.Add(tsi);
            }
        }

        public void MenuShowAndEpisodes()
        {
            ShowItem si = this.mLastShowClicked;
            Season seas = this.mLastSeasonClicked;
            ProcessedEpisode ep = this.mLastEpClicked;
            ToolStripMenuItem tsi;

            if (si != null)
            {
                tsi = new ToolStripMenuItem("Force Refresh");
                tsi.Tag = (int) RightClickCommands.kForceRefreshSeries;
                this.showRightClickMenu.Items.Add(tsi);
                ToolStripSeparator tss = new ToolStripSeparator();
                this.showRightClickMenu.Items.Add(tss);
                tsi = new ToolStripMenuItem("Scan");
                tsi.Tag = (int) RightClickCommands.kScanSpecificSeries;
                this.showRightClickMenu.Items.Add(tsi);
                //tsi = gcnew ToolStripMenuItem("Renaming Check");     tsi->Tag = (int)kRenamingCheckSeries; showRightClickMenu->Items->Add(tsi);
                tsi = new ToolStripMenuItem("When to Watch");
                tsi.Tag = (int) RightClickCommands.kWhenToWatchSeries;
                this.showRightClickMenu.Items.Add(tsi);
            }

            if (ep != null)
            {
                System.Collections.Generic.List<System.IO.FileInfo> fl = this.mDoc.FindEpOnDisk(ep);
                if (fl != null)
                {
                    if (fl.Count > 0)
                    {
                        ToolStripSeparator tss = new ToolStripSeparator();
                        this.showRightClickMenu.Items.Add(tss);

                        int n = this.mLastFL.Count;
                        foreach (FileInfo fi in fl)
                        {
                            this.mLastFL.Add(fi);
                            tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsi.Tag = (int) RightClickCommands.kWatchBase + n;
                            this.showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
            else if ((seas != null) && (si != null))
            {
                // for each episode in season, find it on disk
                bool first = true;
                foreach (ProcessedEpisode epds in si.SeasonEpisodes[seas.SeasonNumber])
                {
                    System.Collections.Generic.List<System.IO.FileInfo> fl = this.mDoc.FindEpOnDisk(epds);
                    if ((fl != null) && (fl.Count > 0))
                    {
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            this.showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        int n = this.mLastFL.Count;
                        foreach (FileInfo fi in fl)
                        {
                            this.mLastFL.Add(fi);
                            tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsi.Tag = (int) RightClickCommands.kWatchBase + n;
                            this.showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
        }

        public void MenuFolders(LVResults lvr)
        {
            ShowItem si = this.mLastShowClicked;
            Season seas = this.mLastSeasonClicked;
            ProcessedEpisode ep = this.mLastEpClicked;
            ToolStripMenuItem tsi;
            StringList added = new StringList();

            if (ep != null)
            {
                if (ep.SI.AllFolderLocations(this.mDoc.Settings).ContainsKey(ep.SeasonNumber))
                {
                    int n = this.mFoldersToOpen.Count;
                    bool first = true;
                    foreach (string folder in ep.SI.AllFolderLocations(this.mDoc.Settings)[ep.SeasonNumber])
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder))
                        {
                            if (first)
                            {
                                ToolStripSeparator tss = new ToolStripSeparator();
                                this.showRightClickMenu.Items.Add(tss);
                                first = false;
                            }

                            tsi = new ToolStripMenuItem("Open: " + folder);
                            added.Add(folder);
                            this.mFoldersToOpen.Add(folder);
                            tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                            n++;
                            this.showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
            else if ((seas != null) && (si != null) && (si.AllFolderLocations(this.mDoc.Settings).ContainsKey(seas.SeasonNumber)))
            {
                int n = this.mFoldersToOpen.Count;
                bool first = true;
                foreach (string folder in si.AllFolderLocations(this.mDoc.Settings)[seas.SeasonNumber])
                {
                    if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            this.showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        tsi = new ToolStripMenuItem("Open: " + folder);
                        this.mFoldersToOpen.Add(folder);
                        tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                        n++;
                        this.showRightClickMenu.Items.Add(tsi);
                    }
                }
            }
            else if (si != null)
            {
                int n = this.mFoldersToOpen.Count;
                bool first = true;

                foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in si.AllFolderLocations(this.mDoc.Settings))
                {
                    foreach (string folder in kvp.Value)
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                ToolStripSeparator tss = new ToolStripSeparator();
                                this.showRightClickMenu.Items.Add(tss);
                                first = false;
                            }

                            tsi = new ToolStripMenuItem("Open: " + folder);
                            this.mFoldersToOpen.Add(folder);
                            tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                            n++;
                            this.showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }

            if (lvr != null) // add folders for selected Scan items
            {
                int n = this.mFoldersToOpen.Count;
                bool first = true;

                foreach (ActionItem Action in lvr.FlatList)
                {
                    string folder = Action.TargetFolder();
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            ToolStripSeparator tss = new ToolStripSeparator();
                            this.showRightClickMenu.Items.Add(tss);
                            first = false;
                        }

                        tsi = new ToolStripMenuItem("Open: " + folder);
                        this.mFoldersToOpen.Add(folder);
                        tsi.Tag = (int) RightClickCommands.kOpenFolderBase + n;
                        n++;
                        this.showRightClickMenu.Items.Add(tsi);
                    }
                }
            }
        }

        public void BuildRightClickMenu(Point pt)
        {
            this.showRightClickMenu.Items.Clear();
            this.mFoldersToOpen = new StringList();
            this.mLastFL = new System.Collections.Generic.List<System.IO.FileInfo>();

            this.MenuGuideAndTVDB(false);
            this.MenuShowAndEpisodes();
            this.MenuFolders(null);

            this.showRightClickMenu.Show(pt);
        }

        public void showRightClickMenu_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            this.showRightClickMenu.Close();
            RightClickCommands n = (RightClickCommands) e.ClickedItem.Tag;
            switch (n)
            {
                case RightClickCommands.kEpisodeGuideForShow: // epguide
                    if (this.mLastEpClicked != null)
                        this.GotoEpguideFor(this.mLastEpClicked, true);
                    else
                        this.GotoEpguideFor(this.mLastShowClicked, true);
                    break;

                case RightClickCommands.kVisitTVDBEpisode: // thetvdb.com
                    {
                        this.TVDBFor(this.mLastEpClicked);
                        break;
                    }

                case RightClickCommands.kVisitTVDBSeason:
                    {
                        this.TVDBFor(this.mLastSeasonClicked);
                        break;
                    }

                case RightClickCommands.kVisitTVDBSeries:
                    {
                        this.TVDBFor(this.mLastShowClicked);
                        break;
                    }
                case RightClickCommands.kScanSpecificSeries:
                    {
                        if (this.mLastShowClicked != null)
                        {
                            this.Scan(this.mLastShowClicked);
                            this.tabControl1.SelectTab(this.tbAllInOne);
                        }
                        break;
                    }

                case RightClickCommands.kWhenToWatchSeries: // when to watch
                    {
                        int code = -1;
                        if (this.mLastEpClicked != null)
                            code = this.mLastEpClicked.TheSeries.TVDBCode;
                        if (this.mLastShowClicked != null)
                            code = this.mLastShowClicked.TVDBCode;

                        if (code != -1)
                        {
                            this.tabControl1.SelectTab(this.tbWTW);

                            for (int i = 0; i < this.lvWhenToWatch.Items.Count; i++)
                                this.lvWhenToWatch.Items[i].Selected = false;

                            for (int i = 0; i < this.lvWhenToWatch.Items.Count; i++)
                            {
                                ListViewItem lvi = this.lvWhenToWatch.Items[i];
                                ProcessedEpisode ei = (ProcessedEpisode) (lvi.Tag);
                                if ((ei != null) && (ei.TheSeries.TVDBCode == code))
                                    lvi.Selected = true;
                            }
                            this.lvWhenToWatch.Focus();
                        }
                        break;
                    }
                case RightClickCommands.kForceRefreshSeries:
                    this.ForceRefresh(this.mLastShowClicked);
                    break;
                case RightClickCommands.kBTSearchFor:
                    {
                        foreach (ListViewItem lvi in this.lvAction.SelectedItems)
                        {
                            ActionMissing m = (ActionMissing) (lvi.Tag);
                            if (m != null)
                                this.mDoc.DoBTSearch(m.PE);
                        }
                    }
                    break;
                case RightClickCommands.kActionAction:
                    this.ActionAction(false);
                    break;
                case RightClickCommands.kActionBrowseForFile:
                    {
                        if ((this.mLastActionsClicked != null) && (this.mLastActionsClicked.Count > 0))
                        {
                            ActionMissing mi = (ActionMissing) this.mLastActionsClicked[0];
                            if (mi != null)
                            {
                                // browse for mLastActionClicked
                                this.openFile.Filter = "Video Files|" + this.mDoc.Settings.GetVideoExtensionsString().Replace(".", "*.") + "|All Files (*.*)|*.*";

                                if (this.openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    // make new ActionItem for copying/moving to specified location
                                    FileInfo from = new FileInfo(this.openFile.FileName);
                                    this.mDoc.TheActionList.Add(new ActionCopyMoveRename(this.mDoc.Settings.LeaveOriginals ? ActionCopyMoveRename.Op.Copy : ActionCopyMoveRename.Op.Move, from, new FileInfo(mi.TheFileNoExt + from.Extension), mi.PE));
                                    // and remove old Missing item
                                    this.mDoc.TheActionList.Remove(mi);
                                }
                            }
                            this.mLastActionsClicked = null;
                            this.FillActionList();
                        }

                        break;
                    }
                case RightClickCommands.kActionIgnore:
                    this.IgnoreSelected();
                    break;
                case RightClickCommands.kActionIgnoreSeason:
                    {
                        // add season to ignore list for each show selected
                        if ((this.mLastActionsClicked != null) && (this.mLastActionsClicked.Count > 0))
                        {
                            foreach (ActionItem ai in this.mLastActionsClicked)
                            {
                                int snum = ai.PE.SeasonNumber;

                                if (!ai.PE.SI.IgnoreSeasons.Contains(snum))
                                    ai.PE.SI.IgnoreSeasons.Add(snum);

                                // remove all other episodes of this season from the Action list
                                System.Collections.Generic.List<ActionItem> remove = new System.Collections.Generic.List<ActionItem>();
                                foreach (ActionItem Action in this.mDoc.TheActionList)
                                {
                                    if ((Action.PE != null) && (Action.PE.SeasonNumber == snum))
                                        remove.Add(Action);
                                }
                                foreach (ActionItem Action in remove)
                                    this.mDoc.TheActionList.Remove(Action);
                            }
                        }
                        this.mLastActionsClicked = null;
                        this.FillActionList();
                        break;
                    }
                case RightClickCommands.kActionDelete:
                    this.ActionDeleteSelected();
                    break;
                default:
                    {
                        if ((n >= RightClickCommands.kWatchBase) && (n < RightClickCommands.kOpenFolderBase))
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if ((this.mLastFL != null) && (wn >= 0) && (wn < this.mLastFL.Count))
                                TVDoc.SysOpen(this.mLastFL[wn].FullName);
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < this.mFoldersToOpen.Count)
                            {
                                string folder = this.mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                    TVDoc.SysOpen(folder);
                            }
                            return;
                        }
                        else
                            System.Diagnostics.Debug.Fail("Unknown right-click action " + n);
                        break;
                    }
            }

            this.mLastEpClicked = null;
        }

        public void tabControl1_DoubleClick(object sender, System.EventArgs e)
        {
            if (this.tabControl1.SelectedTab == this.tbMyShows)
                this.bnMyShowsRefresh_Click(null, null);
            else if (this.tabControl1.SelectedTab == this.tbWTW)
                this.bnWhenToWatchCheck_Click(null, null);
            else if (this.tabControl1.SelectedTab == this.tbAllInOne)
                this.bnActionCheck_Click(null, null);
        }

        public void folderRightClickMenu_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            switch ((int) (e.ClickedItem.Tag))
            {
                case 0: // open folder
                    TVDoc.SysOpen(this.mLastFolderClicked);
                    break;
                default:
                    break;
            }
        }

        public void RightClickOnFolder(string folderPath, Point pt)
        {
            this.mLastFolderClicked = folderPath;
            this.folderRightClickMenu.Items.Clear();

            ToolStripMenuItem tsi;
            int n = 0;

            tsi = new ToolStripMenuItem("Open: " + folderPath);
            tsi.Tag = n++;
            this.folderRightClickMenu.Items.Add(tsi);

            this.folderRightClickMenu.Show(pt);
        }

        public void lvWhenToWatch_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;
            if (this.lvWhenToWatch.SelectedItems.Count == 0)
                return;

            Point pt = this.lvWhenToWatch.PointToScreen(new Point(e.X, e.Y));
            ProcessedEpisode ei = (ProcessedEpisode) (this.lvWhenToWatch.SelectedItems[0].Tag);
            this.RightClickOnShow(ei, pt);
        }

        public void preferencesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.DoPrefs(false);
        }

        public void DoPrefs(bool scanOptions)
        {
            Preferences pref = new Preferences(this.mDoc, scanOptions);
            if (pref.ShowDialog() == DialogResult.OK)
            {
                this.mDoc.SetDirty();
                this.ShowHideNotificationIcon();
                this.FillWhenToWatchList();
                this.ShowInTaskbar = this.mDoc.Settings.ShowInTaskbar;
                this.FillEpGuideHTML();
            }
        }

        public void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.mDoc.WriteXMLSettings();
            this.mDoc.GetTVDB(false, "").SaveCache();
            this.SaveLayoutXML();
        }

        public void UI_SizeChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.mLastNonMaximizedSize = this.Size;
            if ((this.WindowState == FormWindowState.Minimized) && (!this.mDoc.Settings.ShowInTaskbar))
                this.Hide();
        }

        public void UI_LocationChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.mLastNonMaximizedLocation = this.Location;
        }

        public void statusTimer_Tick(object sender, System.EventArgs e)
        {
            int n = this.mDoc.DownloadDone ? 0 : this.mDoc.DownloadsRemaining;

            this.txtDLStatusLabel.Visible = (n != 0 || this.mDoc.Settings.BGDownload);
            if (n != 0)
            {
                this.txtDLStatusLabel.Text = "Background download: " + this.mDoc.GetTVDB(false, "").CurrentDLTask;
                this.backgroundDownloadNowToolStripMenuItem.Enabled = false;
            }
            else
                this.txtDLStatusLabel.Text = "Background download: Idle";

            if (this.Busy == 0)
            {
                if ((n == 0) && (this.LastDLRemaining > 0))
                {
                    // we've just finished a bunch of background downloads
                    this.mDoc.GetTVDB(false, "").SaveCache();
                    this.RefreshWTW(false);

                    this.backgroundDownloadNowToolStripMenuItem.Enabled = true;
                }
                this.LastDLRemaining = n;
            }
        }

        public void backgroundDownloadToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.mDoc.Settings.BGDownload = !this.mDoc.Settings.BGDownload;
            this.backgroundDownloadToolStripMenuItem.Checked = this.mDoc.Settings.BGDownload;
            this.statusTimer_Tick(null, null);
            this.mDoc.SetDirty();

            if (this.mDoc.Settings.BGDownload)
                this.BGDownloadTimer.Start();
            else
                this.BGDownloadTimer.Stop();
        }

        public void BGDownloadTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.Busy != 0)
            {
                this.BGDownloadTimer.Interval = 10000; // come back in 10 seconds
                this.BGDownloadTimer.Start();
                return;
            }
            this.BGDownloadTimer.Interval = BGDLLongInterval(); // after first time (10 seconds), put up to 60 minutes
            this.BGDownloadTimer.Start();

            if (this.mDoc.Settings.BGDownload && this.mDoc.DownloadDone) // only do auto-download if don't have stuff to do already
            {
                this.mDoc.StartBGDownloadThread(false);

                this.statusTimer_Tick(null, null);
            }
        }

        public void backgroundDownloadNowToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (this.mDoc.Settings.OfflineMode)
            {
                System.Windows.Forms.DialogResult res = MessageBox.Show("Ignore offline mode and download anyway?", "Background Download", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res != System.Windows.Forms.DialogResult.Yes)
                    return;
            }
            this.BGDownloadTimer.Stop();
            this.BGDownloadTimer.Start();

            this.mDoc.StartBGDownloadThread(false);

            this.statusTimer_Tick(null, null);
        }

        public void offlineOperationToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (!this.mDoc.Settings.OfflineMode)
            {
                if (MessageBox.Show("Are you sure you wish to go offline?", "TVRename", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }

            this.mDoc.Settings.OfflineMode = !this.mDoc.Settings.OfflineMode;
            this.offlineOperationToolStripMenuItem.Checked = this.mDoc.Settings.OfflineMode;
            this.mDoc.SetDirty();
        }

        public void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.tabControl1.SelectedTab == this.tbMyShows)
                this.FillEpGuideHTML();

            this.exportToolStripMenuItem.Enabled = false; //( (tabControl1->SelectedTab == tbMissing) ||
            //														  (tabControl1->SelectedTab == tbFnO) ||
            //														  (tabControl1->SelectedTab == tbRenaming) );
        }

        public void bugReportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            BugReport br = new BugReport(this.mDoc);
            br.ShowDialog();
        }

        public void exportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            // TODO: Export to CSV/XML

            //                
            //				if (tabControl1->SelectedTab == tbMissing)
            //				{
            //				if (!MissingListHasStuff())
            //				return;
            //
            //				saveFile->Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml";
            //				if (saveFile->ShowDialog() != ::DialogResult::OK)
            //				return;
            //
            //				if (saveFile->FilterIndex == 1) // counts from 1
            //				mDoc->ExportMissingCSV(saveFile->FileName);
            //				else if (saveFile->FilterIndex == 2)
            //				mDoc->ExportMissingXML(saveFile->FileName);
            //				}
            //				else if (tabControl1->SelectedTab == tbFnO)
            //				{
            //				saveFile->Filter = "XML Files|*.xml";
            //				if (saveFile->ShowDialog() != ::DialogResult::OK)
            //				return;
            //				mDoc->ExportFOXML(saveFile->FileName);
            //				}
            //				else if (tabControl1->SelectedTab == tbRenaming)
            //				{
            //				saveFile->Filter = "XML Files|*.xml";
            //				if (saveFile->ShowDialog() != ::DialogResult::OK)
            //				return;
            //				mDoc->ExportRenamingXML(saveFile->FileName);
            //				}
            //				
        }

        public void ShowHideNotificationIcon()
        {
            this.notifyIcon1.Visible = this.mDoc.Settings.NotificationAreaIcon && !this.mDoc.Args.Hide;
        }

        public void statisticsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            StatsWindow sw = new StatsWindow(this.mDoc.Stats());
            sw.ShowDialog();
        }

        ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// ////// //////// 

        public TreeNode AddShowItemToTree(ShowItem si)
        {
            TheTVDB db = this.mDoc.GetTVDB(true, "AddShowItemToTree");
            string name = si.ShowName();

            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            if (string.IsNullOrEmpty(name))
            {
                if (ser != null)
                    name = ser.Name;
                else
                    name = "-- Unknown : " + si.TVDBCode + " --";
            }

            TreeNode n = new TreeNode(name);
            n.Tag = si;

            if (ser != null)
            {
                System.Collections.Generic.List<int> theKeys = new System.Collections.Generic.List<int>();
                // now, go through and number them all sequentially
                foreach (int snum in ser.Seasons.Keys)
                    theKeys.Add(snum);

                theKeys.Sort();

                foreach (int snum in theKeys)
                {
                    string nodeTitle = snum == 0 ? "Specials" : "Season " + snum;
                    TreeNode n2 = new TreeNode(nodeTitle);
                    if (si.IgnoreSeasons.Contains(snum))
                        n2.ForeColor = Color.Gray;
                    n2.Tag = ser.Seasons[snum];
                    n.Nodes.Add(n2);
                }
            }

            this.MyShowTree.Nodes.Add(n);

            db.Unlock("AddShowItemToTree");

            return n;
        }

        public void UpdateWTW(ProcessedEpisode pe, ListViewItem lvi)
        {
            lvi.Tag = pe;

            // group 0 = just missed
            //       1 = this week
            //       2 = future / unknown

            DateTime? airdt = pe.GetAirDateDT(true);
            if (airdt == null)
            {
                // TODO: something!
                return;
            }
            DateTime dt = (DateTime) airdt;

            double ttn = (dt.Subtract(DateTime.Now)).TotalHours;

            if (ttn < 0)
                lvi.Group = this.lvWhenToWatch.Groups[0];
            else if (ttn < (7 * 24))
                lvi.Group = this.lvWhenToWatch.Groups[1];
            else if (!pe.NextToAir)
                lvi.Group = this.lvWhenToWatch.Groups[3];
            else
                lvi.Group = this.lvWhenToWatch.Groups[2];

            int n = 1;
            lvi.Text = pe.SI.ShowName();
            lvi.SubItems[n++].Text = (pe.SeasonNumber != 0) ? pe.SeasonNumber.ToString() : "Special";
            string estr = (pe.EpNum > 0) ? pe.EpNum.ToString() : "";
            if ((pe.EpNum > 0) && (pe.EpNum2 != pe.EpNum) && (pe.EpNum2 > 0))
                estr += "-" + pe.EpNum2;
            lvi.SubItems[n++].Text = estr;
            lvi.SubItems[n++].Text = dt.ToShortDateString();
            lvi.SubItems[n++].Text = dt.ToString("t");
            lvi.SubItems[n++].Text = dt.ToString("ddd");
            lvi.SubItems[n++].Text = pe.HowLong();
            lvi.SubItems[n++].Text = pe.Name;

            // icon..

            if (airdt.Value.CompareTo(DateTime.Now) < 0) // has aired
            {
                System.Collections.Generic.List<System.IO.FileInfo> fl = this.mDoc.FindEpOnDisk(pe);
                if ((fl != null) && (fl.Count > 0))
                    lvi.ImageIndex = 0;
                else if (pe.SI.DoMissingCheck)
                    lvi.ImageIndex = 1;
            }
        }

        public void SelectSeason(Season seas)
        {
            foreach (TreeNode n in this.MyShowTree.Nodes)
            {
                foreach (TreeNode n2 in n.Nodes)
                {
                    if (TreeNodeToSeason(n2) == seas)
                    {
                        n2.EnsureVisible();
                        this.MyShowTree.SelectedNode = n2;
                        return;
                    }
                }
            }
            this.FillEpGuideHTML(null);
        }

        public void SelectShow(ShowItem si)
        {
            foreach (TreeNode n in this.MyShowTree.Nodes)
            {
                if (this.TreeNodeToShowItem(n) == si)
                {
                    n.EnsureVisible();
                    this.MyShowTree.SelectedNode = n;
                    //FillEpGuideHTML();
                    return;
                }
            }
            this.FillEpGuideHTML(null);
        }

        private void bnMyShowsAdd_Click(object sender, System.EventArgs e)
        {
            this.MoreBusy();
            ShowItem si = new ShowItem(this.mDoc.GetTVDB(false, ""));
            TheTVDB db = this.mDoc.GetTVDB(true, "AddShow");
            AddEditShow aes = new AddEditShow(si, db, TimeZone.DefaultTimeZone());
            System.Windows.Forms.DialogResult dr = aes.ShowDialog();
            db.Unlock("AddShow");
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                this.mDoc.GetShowItems(true).Add(si);
                this.mDoc.UnlockShowItems();
                SeriesInfo ser = db.GetSeries(si.TVDBCode);
                if (ser != null)
                    ser.ShowTimeZone = aes.ShowTimeZone;
                this.ShowAddedOrEdited(true);
                this.SelectShow(si);
            }
            this.LessBusy();
        }

        private void ShowAddedOrEdited(bool download)
        {
            this.mDoc.SetDirty();
            this.RefreshWTW(download);
            this.FillShowLists();
        }

        private void bnMyShowsDelete_Click(object sender, System.EventArgs e)
        {
            TreeNode n = this.MyShowTree.SelectedNode;
            ShowItem si = this.TreeNodeToShowItem(n);
            if (si == null)
                return;

            System.Windows.Forms.DialogResult res = MessageBox.Show("Remove show \"" + si.ShowName() + "\".  Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != System.Windows.Forms.DialogResult.Yes)
                return;

            this.mDoc.GetShowItems(true).Remove(si);
            this.mDoc.UnlockShowItems();
            this.ShowAddedOrEdited(false);
        }

        private void bnMyShowsEdit_Click(object sender, System.EventArgs e)
        {
            TreeNode n = this.MyShowTree.SelectedNode;
            if (n == null)
                return;
            Season seas = TreeNodeToSeason(n);
            if (seas != null)
            {
                ShowItem si = this.TreeNodeToShowItem(n);
                if (si != null)
                    this.EditSeason(si, seas.SeasonNumber);
                return;
            }

            ShowItem si2 = this.TreeNodeToShowItem(n);
            if (si2 != null)
            {
                this.EditShow(si2);
                return;
            }
        }

        private void EditSeason(ShowItem si, int seasnum)
        {
            this.MoreBusy();

            TheTVDB db = this.mDoc.GetTVDB(true, "EditSeason");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);
            ProcessedEpisodeList pel = TVDoc.GenerateEpisodes(si, ser, seasnum, false);

            EditRules er = new EditRules(si, pel, seasnum, this.mDoc.Settings.NamingStyle);
            System.Windows.Forms.DialogResult dr = er.ShowDialog();
            db.Unlock("EditSeason");
            if (dr == DialogResult.OK)
            {
                this.ShowAddedOrEdited(false);
                if (ser != null)
                    this.SelectSeason(ser.Seasons[seasnum]);
            }

            this.LessBusy();
        }

        private void EditShow(ShowItem si)
        {
            this.MoreBusy();
            TheTVDB db = this.mDoc.GetTVDB(true, "EditShow");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            int oldCode = si.TVDBCode;

            AddEditShow aes = new AddEditShow(si, db, ser != null ? ser.ShowTimeZone : TimeZone.DefaultTimeZone());

            System.Windows.Forms.DialogResult dr = aes.ShowDialog();

            db.Unlock("EditShow");

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                if (ser != null)
                    ser.ShowTimeZone = aes.ShowTimeZone; // TODO: move into AddEditShow

                this.ShowAddedOrEdited(si.TVDBCode != oldCode);
                this.SelectShow(si);
            }
            this.LessBusy();
        }

        private void ForceRefresh(ShowItem si)
        {
            if (si != null)
                this.mDoc.GetTVDB(false, "").ForgetShow(si.TVDBCode, true);
            this.mDoc.DoDownloadsFG();
            this.FillShowLists();
            this.FillEpGuideHTML();
            this.RefreshWTW(false);
        }

        private void bnMyShowsRefresh_Click(object sender, System.EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                // nuke currently selected show to force getting it fresh
                TreeNode n = this.MyShowTree.SelectedNode;
                ShowItem si = this.TreeNodeToShowItem(n);
                this.ForceRefresh(si);
            }
            else
                this.ForceRefresh(null);
        }

        private void MyShowTree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            this.FillEpGuideHTML(e.Node);
        }

        private void bnMyShowsVisitTVDB_Click(object sender, System.EventArgs e)
        {
            TreeNode n = this.MyShowTree.SelectedNode;
            ShowItem si = this.TreeNodeToShowItem(n);
            if (si == null)
                return;
            Season seas = TreeNodeToSeason(n);

            int sid = -1;
            if (seas != null)
                sid = seas.SeasonID;
            TVDoc.SysOpen(this.mDoc.GetTVDB(false, "").WebsiteURL(si.TVDBCode, sid, false));
        }

        private void bnMyShowsOpenFolder_Click(object sender, System.EventArgs e)
        {
            TreeNode n = this.MyShowTree.SelectedNode;
            ShowItem si = this.TreeNodeToShowItem(n);
            if (si == null)
                return;

            Season seas = TreeNodeToSeason(n);
            System.Collections.Generic.Dictionary<int, StringList> afl = si.AllFolderLocations(this.mDoc.Settings);
            int[] keys = new int[afl.Count];
            afl.Keys.CopyTo(keys, 0);
            if ((seas == null) && (keys.Length > 0))
            {
                string f = si.AutoAdd_FolderBase;
                if (string.IsNullOrEmpty(f))
                {
                    int n2 = keys[0];
                    if (afl[n2].Count > 0)
                        f = afl[n2][0];
                }
                if (!string.IsNullOrEmpty(f))
                {
                    try
                    {
                        TVDoc.SysOpen(f);
                        return;
                    }
                    catch
                    {
                    }
                }
            }

            if ((seas != null) && (afl.ContainsKey(seas.SeasonNumber)))
            {
                foreach (string folder in afl[seas.SeasonNumber])
                {
                    if (Directory.Exists(folder))
                    {
                        TVDoc.SysOpen(folder);
                        return;
                    }
                }
            }
            try
            {
                if (!string.IsNullOrEmpty(si.AutoAdd_FolderBase) && (Directory.Exists(si.AutoAdd_FolderBase)))
                    TVDoc.SysOpen(si.AutoAdd_FolderBase);
            }
            catch
            {
            }
        }

        private void MyShowTree_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;

            this.MyShowTree.SelectedNode = this.MyShowTree.GetNodeAt(e.X, e.Y);

            Point pt = this.MyShowTree.PointToScreen(new Point(e.X, e.Y));
            TreeNode n = this.MyShowTree.SelectedNode;

            if (n == null)
                return;

            ShowItem si = this.TreeNodeToShowItem(n);
            Season seas = TreeNodeToSeason(n);

            if (seas != null)
                RightClickOnMyShows(seas, pt);
            else if (si != null)
                RightClickOnMyShows(si, pt);
        }

        private void quickstartGuideToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            this.ShowQuickStartGuide();
        }

        private ProcessedEpisodeList CurrentlySelectedPEL()
        {
            Season currentSeas = TreeNodeToSeason(this.MyShowTree.SelectedNode);
            ShowItem currentSI = this.TreeNodeToShowItem(this.MyShowTree.SelectedNode);

            int snum = (currentSeas != null) ? currentSeas.SeasonNumber : 1;
            ProcessedEpisodeList pel = null;
            if ((currentSI != null) && (currentSI.SeasonEpisodes.ContainsKey(snum)))
                pel = currentSI.SeasonEpisodes[snum];
            else
            {
                foreach (ShowItem si in this.mDoc.GetShowItems(true))
                {
                    foreach (System.Collections.Generic.KeyValuePair<int, ProcessedEpisodeList> kvp in si.SeasonEpisodes)
                    {
                        pel = kvp.Value;
                        break;
                    }
                    if (pel != null)
                        break;
                }
                this.mDoc.UnlockShowItems();
            }
            return pel;
        }

        private void filenameTemplateEditorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            CustomName cn = new CustomName(this.mDoc.Settings.NamingStyle.StyleString);
            CustomNameDesigner cne = new CustomNameDesigner(this.CurrentlySelectedPEL(), cn, this.mDoc);
            DialogResult dr = cne.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.mDoc.Settings.NamingStyle = cn;
                this.mDoc.SetDirty();
            }
        }

        private void searchEnginesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            ProcessedEpisodeList pel = this.CurrentlySelectedPEL();

            AddEditSearchEngine aese = new AddEditSearchEngine(this.mDoc.GetSearchers(), ((pel != null) && (pel.Count > 0)) ? pel[0] : null);
            DialogResult dr = aese.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.mDoc.SetDirty();
                this.UpdateSearchButton();
            }
        }

        private void filenameProcessorsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            //Season ^currentSeas = TreeNodeToSeason(MyShowTree->SelectedNode);
            ShowItem currentSI = this.TreeNodeToShowItem(this.MyShowTree.SelectedNode);
            string theFolder = "";

            if (currentSI != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in currentSI.AllFolderLocations(this.mDoc.Settings))
                {
                    foreach (string folder in kvp.Value)
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder))
                        {
                            theFolder = folder;
                            break;
                        }
                    }
                    if (!string.IsNullOrEmpty(theFolder))
                        break;
                }
            }

            AddEditSeasEpFinders d = new AddEditSeasEpFinders(this.mDoc.Settings.FNPRegexs, this.mDoc.GetShowItems(true), currentSI, theFolder, this.mDoc.Settings);
            this.mDoc.UnlockShowItems();

            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
                this.mDoc.SetDirty();
        }

        private void actorsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            new ActorsGrid(this.mDoc).ShowDialog();
        }

        private void quickTimer_Tick(object sender, System.EventArgs e)
        {
            this.quickTimer.Stop();
            this.ProcessArgs();
        }

        private void uTorrentToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            uTorrent ut = new uTorrent(this.mDoc, this.SetProgress);
            ut.ShowDialog();
            this.tabControl1.SelectedIndex = 1; // go to all-in-one tab
        }

        private void bnMyShowsCollapse_Click(object sender, System.EventArgs e)
        {
            this.MyShowTree.CollapseAll();
        }

        private void UI_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int t = -1;
            if (e.Control && (e.KeyCode == Keys.D1))
                t = 0;
            else if (e.Control && (e.KeyCode == Keys.D2))
                t = 1;
            else if (e.Control && (e.KeyCode == Keys.D3))
                t = 2;
            else if (e.Control && (e.KeyCode == Keys.D4))
                t = 3;
            else if (e.Control && (e.KeyCode == Keys.D5))
                t = 4;
            else if (e.Control && (e.KeyCode == Keys.D6))
                t = 5;
            else if (e.Control && (e.KeyCode == Keys.D7))
                t = 6;
            else if (e.Control && (e.KeyCode == Keys.D8))
                t = 7;
            else if (e.Control && (e.KeyCode == Keys.D9))
                t = 8;
            else if (e.Control && (e.KeyCode == Keys.D0))
                t = 9;
            if ((t >= 0) && (t < this.tabControl1.TabCount))
            {
                this.tabControl1.SelectedIndex = t;
                e.Handled = true;
            }
        }

        private void bnActionCheck_Click(object sender, System.EventArgs e)
        {
            this.ScanAll();
        }

        private void ScanAll()
        {
            this.tabControl1.SelectedTab = this.tbAllInOne;
            this.Scan(null);
        }

        private void Scan(ShowItem s)
        {
            this.MoreBusy();
            this.mDoc.ActionGo(s);
            this.LessBusy();
            this.FillActionList();
        }

        private static string GBMB(long size)
        {
            long gb1 = (1024 * 1024 * 1024);
            long gb = ((gb1 / 2) + size) / gb1;
            if (gb > 1)
                return gb + " GB";
            else
            {
                long mb1 = 1024 * 1024;
                long mb = ((mb1 / 2) + size) / mb1;
                return mb + " MB";
            }
        }

        private static string itemitems(int n)
        {
            if (n == 1)
                return "Item";
            else
                return "Items";
        }

        private void lvAction_RetrieveVirtualItem(object sender, System.Windows.Forms.RetrieveVirtualItemEventArgs e)
        {
            ActionItem Action = this.mDoc.TheActionList[e.ItemIndex];
            ListViewItem lvi = Action.GetLVI(this.lvAction);
            int n = Action.IconNumber();
            if (n != -1)
                lvi.ImageIndex = n;
            lvi.Checked = true;
            lvi.Tag = Action;

            const int kErrCol = 8;
            System.Diagnostics.Debug.Assert(lvi.SubItems.Count <= kErrCol);

            while (lvi.SubItems.Count < kErrCol)
                lvi.SubItems.Add(""); // pad our way to the error column

            if (Action.HasError)
            {
                lvi.SubItems.Add(Action.ErrorText); // error text
                lvi.BackColor = Helpers.WarningColor();
                if ((Action.Type == ActionType.kMissing) || (Action.Type == ActionType.kuTorrenting))
                    lvi.Checked = false;
            }
            else
                lvi.SubItems.Add("");

            System.Diagnostics.Debug.Assert(lvi.SubItems.Count == this.lvAction.Columns.Count);

            e.Item = lvi;
        }

        private void FillActionList()
        {
            this.InternalCheckChange = true;

            if (this.lvAction.VirtualMode)
                this.lvAction.VirtualListSize = this.mDoc.TheActionList.Count;
            else
            {
                this.lvAction.BeginUpdate();
                this.lvAction.Items.Clear();

                foreach (ActionItem Action in this.mDoc.TheActionList)
                {
                    ListViewItem lvi = Action.GetLVI(this.lvAction);
                    lvi.Checked = true;
                    lvi.Tag = Action;
                    this.lvAction.Items.Add(lvi);

                    int n = Action.IconNumber();
                    if (n != -1)
                        lvi.ImageIndex = n;

                    const int kErrCol = 8;
                    System.Diagnostics.Debug.Assert(lvi.SubItems.Count <= kErrCol);
                    if (Action.HasError)
                    {
                        while (lvi.SubItems.Count < kErrCol)
                            lvi.SubItems.Add(""); // pad our way to the error column
                        lvi.SubItems.Add(Action.ErrorText); // error text
                        lvi.BackColor = Helpers.WarningColor();
                        if ((Action.Type == ActionType.kMissing) || (Action.Type == ActionType.kuTorrenting))
                            lvi.Checked = false;
                    }
                }
                this.lvAction.EndUpdate();
            }

            // do nice totals for each group
            int missingCount = 0;
            int renameCount = 0;
            int copyCount = 0;
            long copySize = 0;
            int moveCount = 0;
            long moveSize = 0;
            int rssCount = 0;
            int downloadCount = 0;
            int nfoCount = 0;
            int utCount = 0;

            foreach (ActionItem Action in this.mDoc.TheActionList)
            {
                if (Action.Type == ActionType.kMissing)
                    missingCount++;
                else if (Action.Type == ActionType.kCopyMoveRename)
                {
                    ActionCopyMoveRename cmr = (ActionCopyMoveRename) (Action);
                    ActionCopyMoveRename.Op op = cmr.Operation;
                    if (op == ActionCopyMoveRename.Op.Copy)
                    {
                        copyCount++;
                        if (cmr.From.Exists)
                            copySize += cmr.From.Length;
                    }
                    else if (op == ActionCopyMoveRename.Op.Move)
                    {
                        moveCount++;
                        if (cmr.From.Exists)
                            moveSize += cmr.From.Length;
                    }
                    else if (op == ActionCopyMoveRename.Op.Rename)
                        renameCount++;
                }
                else if (Action.Type == ActionType.kDownload)
                    downloadCount++;
                else if (Action.Type == ActionType.kRSS)
                    rssCount++;
                else if (Action.Type == ActionType.kNFO)
                    nfoCount++;
                else if (Action.Type == ActionType.kuTorrenting)
                    utCount++;
            }

            this.lvAction.Groups[0].Header = "Missing (" + missingCount + " " + itemitems(missingCount) + ")";
            this.lvAction.Groups[1].Header = "Rename (" + renameCount + " " + itemitems(renameCount) + ")";
            this.lvAction.Groups[2].Header = "Copy (" + copyCount + " " + itemitems(copyCount) + ", " + GBMB(copySize) + ")";
            this.lvAction.Groups[3].Header = "Move (" + moveCount + " " + itemitems(moveCount) + ", " + GBMB(moveSize) + ")";
            this.lvAction.Groups[4].Header = "Download RSS (" + rssCount + " " + itemitems(rssCount) + ")";
            this.lvAction.Groups[5].Header = "Download (" + downloadCount + " " + itemitems(downloadCount) + ")";
            this.lvAction.Groups[6].Header = "NFO File (" + nfoCount + " " + itemitems(nfoCount) + ")";
            this.lvAction.Groups[7].Header = "Downloading In µTorrent (" + utCount + " " + itemitems(utCount) + ")";

            this.InternalCheckChange = false;

            this.UpdateActionCheckboxes();
        }

        private void bnActionAction_Click(object sender, System.EventArgs e)
        {
            this.ActionAction(true);
        }

        private void ActionAll()
        {
            this.ActionAction(true);
        }

        private void ActionAction(bool @checked)
        {
            LVResults lvr = new LVResults(this.lvAction, @checked);
            this.mDoc.ActionAction(this.SetProgress, lvr.FlatList);
            // remove items from master list, unless it had an error
            foreach (ActionItem i2 in (new LVResults(this.lvAction, @checked)).FlatList)
            {
                if (!lvr.FlatList.Contains(i2))
                    this.mDoc.TheActionList.Remove(i2);
            }

            this.FillActionList();
            this.RefreshWTW(false);
        }

        private void folderMonitorToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            FolderMonitor fm = new FolderMonitor(this.mDoc);
            fm.ShowDialog();
            this.FillShowLists();
        }

        private void torrentMatchToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            TorrentMatch tm = new TorrentMatch(this.mDoc, this.SetProgress);
            tm.ShowDialog();
            this.FillActionList();
        }

        private void bnActionWhichSearch_Click(object sender, System.EventArgs e)
        {
            this.ChooseSiteMenu(0);
        }

        private void lvAction_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                return;

            // build the right click menu for the _selected_ items, and types of items
            LVResults lvr = new LVResults(this.lvAction, false);

            if (lvr.Count == 0)
                return; // nothing selected

            Point pt = this.lvAction.PointToScreen(new Point(e.X, e.Y));

            this.showRightClickMenu.Items.Clear();

            // Action related items
            ToolStripMenuItem tsi;
            if (lvr.Count > lvr.Missing.Count) // not just missing selected
            {
                tsi = new ToolStripMenuItem("Action Selected");
                tsi.Tag = (int) RightClickCommands.kActionAction;
                this.showRightClickMenu.Items.Add(tsi);
            }

            tsi = new ToolStripMenuItem("Ignore Selected");
            tsi.Tag = (int) RightClickCommands.kActionIgnore;
            this.showRightClickMenu.Items.Add(tsi);

            tsi = new ToolStripMenuItem("Ignore Entire Season");
            tsi.Tag = (int) RightClickCommands.kActionIgnoreSeason;
            this.showRightClickMenu.Items.Add(tsi);

            tsi = new ToolStripMenuItem("Remove Selected");
            tsi.Tag = (int) RightClickCommands.kActionDelete;
            this.showRightClickMenu.Items.Add(tsi);

            if (lvr.Count == lvr.Missing.Count) // only missing items selected?
            {
                this.showRightClickMenu.Items.Add(new ToolStripSeparator());

                tsi = new ToolStripMenuItem("BT Search");
                tsi.Tag = (int) RightClickCommands.kBTSearchFor;
                this.showRightClickMenu.Items.Add(tsi);

                if (lvr.Count == 1) // only one selected
                {
                    tsi = new ToolStripMenuItem("Browse For...");
                    tsi.Tag = (int) RightClickCommands.kActionBrowseForFile;
                    this.showRightClickMenu.Items.Add(tsi);
                }
            }

            this.MenuGuideAndTVDB(true);
            this.MenuFolders(lvr);

            this.showRightClickMenu.Show(pt);
        }

        private void lvAction_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(this.lvAction, false);

            if (lvr.Count == 0)
            {
                // disable everything
                this.bnActionBTSearch.Enabled = false;
                return;
            }

            if (lvr.Download.Count > 0)
                this.bnActionBTSearch.Enabled = false;
            else
                this.bnActionBTSearch.Enabled = true;

            this.mLastShowClicked = null;
            this.mLastEpClicked = null;
            this.mLastSeasonClicked = null;
            this.mLastActionsClicked = null;

            this.showRightClickMenu.Items.Clear();
            this.mFoldersToOpen = new StringList();
            this.mLastFL = new System.Collections.Generic.List<System.IO.FileInfo>();

            this.mLastActionsClicked = new System.Collections.Generic.List<ActionItem>();

            foreach (ActionItem ai in lvr.FlatList)
                this.mLastActionsClicked.Add(ai);

            if ((lvr.Count == 1) && (this.lvAction.FocusedItem != null) && (this.lvAction.FocusedItem.Tag != null))
            {
                ActionItem Action = (ActionItem) (this.lvAction.FocusedItem.Tag);

                this.mLastEpClicked = Action.PE;
                if (Action.PE != null)
                {
                    this.mLastSeasonClicked = Action.PE.TheSeason;
                    this.mLastShowClicked = Action.PE.SI;
                }
                else
                {
                    this.mLastSeasonClicked = null;
                    this.mLastShowClicked = null;
                }

                if ((this.mLastEpClicked != null) && (this.mDoc.Settings.AutoSelectShowInMyShows))
                    this.GotoEpguideFor(this.mLastEpClicked, false);
            }
        }

        private void ActionDeleteSelected()
        {
            ListView.SelectedListViewItemCollection sel = this.lvAction.SelectedItems;
            foreach (ListViewItem lvi in sel)
                this.mDoc.TheActionList.Remove((ActionItem) (lvi.Tag));
            this.FillActionList();
        }

        private void lvAction_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.ActionDeleteSelected();
        }

        private void cbActionIgnore_Click(object sender, System.EventArgs e)
        {
            this.IgnoreSelected();
        }

        private void UpdateActionCheckboxes()
        {
            if (this.InternalCheckChange)
                return;

            LVResults all = new LVResults(this.lvAction, LVResults.WhichResults.All);
            LVResults chk = new LVResults(this.lvAction, LVResults.WhichResults.Checked);

            if (chk.Rename.Count == 0)
                this.cbRename.CheckState = CheckState.Unchecked;
            else
                this.cbRename.CheckState = (chk.Rename.Count == all.Rename.Count) ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.CopyMove.Count == 0)
                this.cbCopyMove.CheckState = CheckState.Unchecked;
            else
                this.cbCopyMove.CheckState = (chk.CopyMove.Count == all.CopyMove.Count) ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.RSS.Count == 0)
                this.cbRSS.CheckState = CheckState.Unchecked;
            else
                this.cbRSS.CheckState = (chk.RSS.Count == all.RSS.Count) ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.Download.Count == 0)
                this.cbDownload.CheckState = CheckState.Unchecked;
            else
                this.cbDownload.CheckState = (chk.Download.Count == all.Download.Count) ? CheckState.Checked : CheckState.Indeterminate;

            if (chk.NFO.Count == 0)
                this.cbNFO.CheckState = CheckState.Unchecked;
            else
                this.cbNFO.CheckState = (chk.NFO.Count == all.NFO.Count) ? CheckState.Checked : CheckState.Indeterminate;

            int total1 = all.Rename.Count + all.CopyMove.Count + all.RSS.Count + all.Download.Count + all.NFO.Count;
            int total2 = chk.Rename.Count + chk.CopyMove.Count + chk.RSS.Count + chk.Download.Count + chk.NFO.Count;

            if (total2 == 0)
                this.cbAll.CheckState = CheckState.Unchecked;
            else
                this.cbAll.CheckState = (total2 == total1) ? CheckState.Checked : CheckState.Indeterminate;
        }

        private void cbActionAllNone_Click(object sender, System.EventArgs e)
        {
            CheckState cs = this.cbAll.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                this.cbAll.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            this.InternalCheckChange = true;
            foreach (ListViewItem lvi in this.lvAction.Items)
                lvi.Checked = cs == CheckState.Checked;
            this.InternalCheckChange = false;
            this.UpdateActionCheckboxes();
        }

        private void cbActionRename_Click(object sender, System.EventArgs e)
        {
            CheckState cs = this.cbRename.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                this.cbRename.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            this.InternalCheckChange = true;
            foreach (ListViewItem lvi in this.lvAction.Items)
            {
                ActionItem i = (ActionItem) (lvi.Tag);
                if ((i != null) && (i.Type == ActionType.kCopyMoveRename) && (((ActionCopyMoveRename) i).Operation == ActionCopyMoveRename.Op.Rename))
                    lvi.Checked = cs == CheckState.Checked;
            }
            this.InternalCheckChange = false;
            this.UpdateActionCheckboxes();
        }

        private void cbActionCopyMove_Click(object sender, System.EventArgs e)
        {
            CheckState cs = this.cbCopyMove.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                this.cbCopyMove.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            this.InternalCheckChange = true;
            foreach (ListViewItem lvi in this.lvAction.Items)
            {
                ActionItem i = (ActionItem) (lvi.Tag);
                if ((i != null) && (i.Type == ActionType.kCopyMoveRename) && (((ActionCopyMoveRename) i).Operation != ActionCopyMoveRename.Op.Rename))
                    lvi.Checked = cs == CheckState.Checked;
            }
            this.InternalCheckChange = false;
            this.UpdateActionCheckboxes();
        }

        private void cbActionNFO_Click(object sender, System.EventArgs e)
        {
            CheckState cs = this.cbNFO.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                this.cbNFO.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            this.InternalCheckChange = true;
            foreach (ListViewItem lvi in this.lvAction.Items)
            {
                ActionItem i = (ActionItem) (lvi.Tag);
                if ((i != null) && (i.Type == ActionType.kNFO))
                    lvi.Checked = cs == CheckState.Checked;
            }
            this.InternalCheckChange = false;
            this.UpdateActionCheckboxes();
        }

        private void cbActionRSS_Click(object sender, System.EventArgs e)
        {
            CheckState cs = this.cbRSS.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                this.cbRSS.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            this.InternalCheckChange = true;
            foreach (ListViewItem lvi in this.lvAction.Items)
            {
                ActionItem i = (ActionItem) (lvi.Tag);
                if ((i != null) && (i.Type == ActionType.kRSS))
                    lvi.Checked = cs == CheckState.Checked;
            }
            this.InternalCheckChange = false;
            this.UpdateActionCheckboxes();
        }

        private void cbActionDownloads_Click(object sender, System.EventArgs e)
        {
            CheckState cs = this.cbDownload.CheckState;
            if (cs == CheckState.Indeterminate)
            {
                this.cbDownload.CheckState = CheckState.Unchecked;
                cs = CheckState.Unchecked;
            }

            this.InternalCheckChange = true;
            foreach (ListViewItem lvi in this.lvAction.Items)
            {
                ActionItem i = (ActionItem) (lvi.Tag);
                if ((i != null) && (i.Type == ActionType.kDownload))
                    lvi.Checked = cs == CheckState.Checked;
            }
            this.InternalCheckChange = false;
            this.UpdateActionCheckboxes();
        }

        private void lvAction_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if ((e.Index < 0) || (e.Index > this.lvAction.Items.Count))
                return;
            ActionItem Action = (ActionItem) (this.lvAction.Items[e.Index].Tag);
            if ((Action != null) && ((Action.Type == ActionType.kMissing) || (Action.Type == ActionType.kuTorrenting)))
                e.NewValue = CheckState.Unchecked;
        }

        private void bnActionOptions_Click(object sender, System.EventArgs e)
        {
            this.DoPrefs(true);
        }

        private void lvAction_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // double-click on an item will search for missing, do nothing (for now) for anything else
            foreach (ActionMissing miss in new LVResults(this.lvAction, false).Missing)
            {
                if (miss.PE != null)
                    this.mDoc.DoBTSearch(miss.PE);
            }
        }

        private void bnActionBTSearch_Click(object sender, System.EventArgs e)
        {
            LVResults lvr = new LVResults(this.lvAction, false);

            if (lvr.Count == 0)
                return;

            foreach (ActionItem i in lvr.FlatList)
            {
                if (i.PE != null)
                    this.mDoc.DoBTSearch(i.PE);
            }
        }

        private void bnRemoveSel_Click(object sender, System.EventArgs e)
        {
            this.ActionDeleteSelected();
        }

        private void IgnoreSelected()
        {
            LVResults lvr = new LVResults(this.lvAction, false);
            bool added = false;
            foreach (ActionItem Action in lvr.FlatList)
            {
                IgnoreItem ii = Action.GetIgnore();
                if (ii != null)
                {
                    this.mDoc.Ignore.Add(ii);
                    added = true;
                }
            }
            if (added)
            {
                this.mDoc.SetDirty();
                this.mDoc.RemoveIgnored();
                this.FillActionList();
            }
        }

        private void ignoreListToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            IgnoreEdit ie = new IgnoreEdit(this.mDoc);
            ie.ShowDialog();
        }

        private void lvAction_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            this.UpdateActionCheckboxes();
        }
    }
}
