// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace TVRename
{
    using System.IO;

    public partial class ShowSummary : Form
    {
        private readonly TVDoc mDoc;

        public List<String> mFoldersToOpen;
        public List<Alphaleonis.Win32.Filesystem.FileInfo> mLastFL;
        protected Season mLastSeasonClicked;
        protected ShowItem mLastShowClicked;

        public ShowSummary(TVDoc doc)
        {
            this.InitializeComponent();

            this.mDoc = doc;

            this.GenerateData();
        }

        private void GenerateData()
        {
            List<ShowSummaryData> showList = new List<ShowSummaryData>();

            foreach (ShowItem si in this.mDoc.GetShowItems(false))
            {
                ShowSummaryData show = this.AddShowDetails(si);
                showList.Add(show);
            }
            this.PopulateGrid(showList);
        }

        private void PopulateGrid(ICollection<ShowSummaryData> showList)
        {
            SourceGrid.Cells.Views.Cell colTitleModel = new SourceGrid.Cells.Views.Cell
            {
                ElementText = new ActorsGrid.RotatedText(-90.0f),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = DevAge.Drawing.ContentAlignment.BottomCenter
            };

            SourceGrid.Cells.Views.Cell topleftTitleModel = new SourceGrid.Cells.Views.Cell
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = DevAge.Drawing.ContentAlignment.BottomLeft
            };

            this.grid1.Columns.Clear();
            this.grid1.Rows.Clear();

            int maxSeason = this.GetMaxSeason(showList);

            int cols = maxSeason + 2;
            int rows = showList.Count + 1;

            // Draw Header
            this.grid1.ColumnsCount = cols;
            this.grid1.RowsCount = rows;
            this.grid1.FixedColumns = 1;
            this.grid1.FixedRows = 1;
            this.grid1.Selection.EnableMultiSelection = false;

            this.grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
            this.grid1.Rows[0].Height = 65;

            SourceGrid.Cells.ColumnHeader h = new SourceGrid.Cells.ColumnHeader("Show")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };

            this.grid1[0, 0] = h;
            this.grid1[0, 0].View = topleftTitleModel;

            // Draw season
            for (int c = 0; c < maxSeason + 1; c++)
            {
                h = new SourceGrid.Cells.ColumnHeader(c == 0 ? "Specials" : string.Format("Season {0}", c))
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false
                };

                this.grid1[0, c + 1] = h;
                this.grid1[0, c + 1].View = colTitleModel;

                this.grid1.Columns[c + 1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            }

            this.grid1.Columns[0].Width = 150;

            // Draw Shows
            SourceGrid.Cells.RowHeader rh;

            int r = 0; // TODO: remove reliance on index
            foreach (ShowSummaryData show in showList)
            {
                rh = new SourceGrid.Cells.RowHeader(show.showName);
                rh.ResizeEnabled = false;

                this.grid1[r + 1, 0] = rh;
                this.grid1[r + 1, 0].AddController(new ShowClickEvent(this, show.showItem));

                foreach (ShowSummaryData.ShowSummarySeasonData seasonData in show.seasonDataList)
                {
                    ShowSummaryData.SummaryOutput output = seasonData.getOuput();
                    this.grid1[r + 1, seasonData.seasonNumber + 1] = new SourceGrid.Cells.Cell(output.details, typeof(string));
                    this.grid1[r + 1, seasonData.seasonNumber + 1].View = new SourceGrid.Cells.Views.Cell
                    {
                        BackColor = output.color,
                        ForeColor = Color.White,
                        TextAlignment = DevAge.Drawing.ContentAlignment.BottomRight
                    };
                    this.grid1[r + 1, seasonData.seasonNumber + 1].AddController(new ShowClickEvent(this, show.showItem, seasonData.season));
                    this.grid1[r + 1, seasonData.seasonNumber + 1].Editor.EditableMode = SourceGrid.EditableMode.None;
                }
                r++;
            }
            this.grid1.AutoSizeCells();
        }

        private int GetMaxSeason(IEnumerable<ShowSummaryData> showList)
        {
            int maxSeason = 0;
            foreach (ShowSummaryData show in showList)
            {
                if (show.maxSeason > maxSeason)
                    maxSeason = show.maxSeason;
            }
            return maxSeason;
        }

        private ShowSummaryData AddShowDetails(ShowItem si)
        {
            TheTVDB db = TheTVDB.Instance;
            db.GetLock("ShowSummary");
            SeriesInfo ser = db.GetSeries(si.TVDBCode);

            ShowSummaryData showSummary = new ShowSummaryData
            {
                showName = si.ShowName,
                showItem = si
            };

            if (ser != null)
            {
                foreach (int snum in ser.Seasons.Keys)
                {
                    ShowSummaryData.ShowSummarySeasonData seasonData = this.getSeasonDetails(si, ser, snum);
                    showSummary.AddSeason(seasonData);
                }
            }
            db.Unlock("ShowSummary");
            return showSummary;
        }

        private ShowSummaryData.ShowSummarySeasonData getSeasonDetails(ShowItem si, SeriesInfo ser, int snum)
        {
            int epCount = 0;
            int epGotCount = 0;
            int epAiredCount = 0;
            DirFilesCache dfc = new DirFilesCache();

            Season season = null;

            if ((snum >= 0) && (ser.Seasons.ContainsKey(snum)))
            {
                season = ser.Seasons[snum];

                List<ProcessedEpisode> eis;

                if (si.SeasonEpisodes.ContainsKey(snum))
                    eis = si.SeasonEpisodes[snum]; // use processed episodes if they are available
                else
                    eis = ShowItem.ProcessedListFromEpisodes(season.Episodes, si);

                foreach (ProcessedEpisode ei in eis)
                {
                    epCount++;

                    // if has air date and has been aired in the past
                    if (ei.FirstAired != null && ei.FirstAired < DateTime.Now)
                        epAiredCount++;

                    List<Alphaleonis.Win32.Filesystem.FileInfo> fl = this.mDoc.FindEpOnDisk(dfc,ei,false);
                    if (fl != null)
                    {
                        if (fl.Count != 0)
                            epGotCount++;
                    }
                }
            }
            return new ShowSummaryData.ShowSummarySeasonData(snum, epCount, epAiredCount, epGotCount, season);
        }

        private void showRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.showRightClickMenu.Close();
            RightClickCommands n = (RightClickCommands)e.ClickedItem.Tag;
            switch (n)
            {
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
                case RightClickCommands.kForceRefreshSeries:
                    this.ForceRefresh(this.mLastShowClicked);
                    break;
                default:
                    {
                        if ((n >= RightClickCommands.kWatchBase) && (n < RightClickCommands.kOpenFolderBase))
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if ((this.mLastFL != null) && (wn >= 0) && (wn < this.mLastFL.Count))
                                Helpers.SysOpen(this.mLastFL[wn].FullName);
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < this.mFoldersToOpen.Count)
                            {
                                string folder = this.mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                    Helpers.SysOpen(folder);
                            }
                            return;
                        }
                        else
                            System.Diagnostics.Debug.Fail("Unknown right-click action " + n);
                        break;
                    }
            }
        }

        public void TVDBFor(Season seas)
        {
            if (seas == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteURL(seas.TheSeries.TVDBCode, seas.SeasonID, false));
        }

        public void TVDBFor(ShowItem si)
        {
            if (si == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteURL(si.TVDBCode, -1, false));
        }

        private void ForceRefresh(ShowItem si)
        {
            if (si != null)
                TheTVDB.Instance.ForgetShow(si.TVDBCode, true);
            this.mDoc.DoDownloadsFG();
        }

        #region Nested type: ShowClickEvent

        private class ShowClickEvent : SourceGrid.Cells.Controllers.ControllerBase
        {
            private ShowSummary gridSummary;
            private Season season;
            private ShowItem show;

            public ShowClickEvent(ShowSummary gridSummary, ShowItem show)
            {
                this.show = show;
                this.gridSummary = gridSummary;
            }

            public ShowClickEvent(ShowSummary gridSummary, ShowItem show, Season season)
            {
                this.show = show;
                this.season = season;
                this.gridSummary = gridSummary;
            }

            public override void OnMouseDown(SourceGrid.CellContext sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Right)
                    return;
                this.gridSummary.showRightClickMenu.Items.Clear();

                Season seas = this.season;

                this.gridSummary.mLastFL = new List<Alphaleonis.Win32.Filesystem.FileInfo>();
                this.gridSummary.mFoldersToOpen = new List<String>();

                this.gridSummary.mLastShowClicked = this.show;
                this.gridSummary.mLastSeasonClicked = this.season;

                List<String> added = new List<String>();

                if (this.show != null && seas == null)
                {
                    this.GenerateMenu(this.gridSummary.showRightClickMenu, "Force Refresh", RightClickCommands.kForceRefreshSeries);
                    this.GenerateSeparator(this.gridSummary.showRightClickMenu);
                    this.GenerateMenu(this.gridSummary.showRightClickMenu, "Visit thetvdb.com", RightClickCommands.kVisitTVDBSeries);
                }

                if (this.show != null && seas != null)
                    this.GenerateMenu(this.gridSummary.showRightClickMenu, "Visit thetvdb.com", RightClickCommands.kVisitTVDBSeason);

                if ((seas != null) && (this.show != null) && (this.show.AllFolderLocations().ContainsKey(seas.SeasonNumber)))
                {
                    int n = this.gridSummary.mFoldersToOpen.Count;
                    bool first = true;
                    foreach (string folder in this.show.AllFolderLocations()[seas.SeasonNumber])
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                this.GenerateSeparator(this.gridSummary.showRightClickMenu);
                                first = false;
                            }

                            this.GenerateMenu(this.gridSummary.showRightClickMenu, "Open: " + folder, (int)RightClickCommands.kOpenFolderBase + n);
                            this.gridSummary.mFoldersToOpen.Add(folder);
                            n++;
                        }
                    }
                }
                else if (this.show != null)
                {
                    int n = this.gridSummary.mFoldersToOpen.Count;
                    bool first = true;

                    foreach (KeyValuePair<int, List<String>> kvp in this.show.AllFolderLocations())
                    {
                        foreach (string folder in kvp.Value)
                        {
                            if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                            {
                                added.Add(folder); // don't show the same folder more than once
                                if (first)
                                {
                                    this.GenerateSeparator(this.gridSummary.showRightClickMenu);
                                    first = false;
                                }

                                this.GenerateMenu(this.gridSummary.showRightClickMenu, "Open: " + folder, (int)RightClickCommands.kOpenFolderBase + n);
                                this.gridSummary.mFoldersToOpen.Add(folder);
                                n++;
                            }
                        }
                    }
                }

                if (seas != null && this.show != null)
                {
                    // for each episode in season, find it on disk
                    bool first = true;
                    foreach (ProcessedEpisode epds in this.show.SeasonEpisodes[seas.SeasonNumber])
                    {
                        List<Alphaleonis.Win32.Filesystem.FileInfo> fl = this.gridSummary.mDoc.FindEpOnDisk(new DirFilesCache() , epds,false);
                        if ((fl != null) && (fl.Count > 0))
                        {
                            if (first)
                            {
                                this.GenerateSeparator(this.gridSummary.showRightClickMenu);
                                first = false;
                            }

                            int n = this.gridSummary.mLastFL.Count;
                            foreach (Alphaleonis.Win32.Filesystem.FileInfo fi in fl)
                            {
                                this.GenerateMenu(this.gridSummary.showRightClickMenu, "Watch: " + fi.FullName, (int)RightClickCommands.kWatchBase + n);
                                this.gridSummary.mLastFL.Add(fi);
                                n++;
                            }
                        }
                    }
                }

                Point pt = new Point(e.X, e.Y);
                this.gridSummary.showRightClickMenu.Show(sender.Grid.PointToScreen(pt));
            }

            private void GenerateMenu(ContextMenuStrip showRightClickMenu, string menuName, RightClickCommands rightClickCommand)
            {
                this.GenerateMenu(showRightClickMenu, menuName, (int)rightClickCommand);
            }

            private void GenerateMenu(ContextMenuStrip showRightClickMenu, string menuName, int rightClickCommand)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(menuName);
                tsi.Tag = rightClickCommand;
                showRightClickMenu.Items.Add(tsi);
            }

            private void GenerateSeparator(ContextMenuStrip showRightClickMenu)
            {
                ToolStripSeparator tss = new ToolStripSeparator();
                showRightClickMenu.Items.Add(tss);
            }
        }

        #endregion

        #region Nested type: ShowSummaryData

        public class ShowSummaryData
        {
            public int maxSeason;
            public List<ShowSummarySeasonData> seasonDataList = new List<ShowSummarySeasonData>();
            public ShowItem showItem;
            public string showName;

            public void AddSeason(ShowSummarySeasonData seasonData)
            {
                this.seasonDataList.Add(seasonData);

                // set the max season number
                if (seasonData.seasonNumber >= this.maxSeason)
                    this.maxSeason = seasonData.seasonNumber;
            }

            #region Nested type: ShowSummarySeasonData

            public class ShowSummarySeasonData
            {
                public int episodeAiredCount;
                public int episodeCount;

                public int episodeGotCount;

                public Season season;
                public int seasonNumber;

                public ShowSummarySeasonData(int seasonNumber, int episodeCount, int episodeAiredCount, int episodeGotCount, Season season)
                {
                    this.seasonNumber = seasonNumber;
                    this.episodeCount = episodeCount;
                    this.episodeAiredCount = episodeAiredCount;
                    this.episodeGotCount = episodeGotCount;
                    this.season = season;
                }

                public SummaryOutput getOuput()
                {
                    SummaryOutput output = new SummaryOutput();
                    if (this.seasonNumber == 0)
                    {
                        output.details = string.Format("{0} / {1}", this.episodeGotCount, this.episodeCount);
                        if (this.episodeGotCount == this.episodeCount)
                            output.color = Color.Green;
                        else if (this.episodeGotCount == 0)
                            output.color = Color.Red;
                        else
                            output.color = Color.Orange;
                    }
                    else
                    {
                        // show amount of aired eps
                        output.details = string.Format("{0} / {1}", this.episodeGotCount, this.episodeAiredCount);
                        // show amount of unaired eps
                        output.details += this.episodeCount - this.episodeAiredCount == 0 ? string.Empty : string.Format(" ({0})", this.episodeCount - this.episodeAiredCount);

                        if (this.episodeGotCount == this.episodeAiredCount)
                            output.color = (this.episodeCount - this.episodeAiredCount) == 0 ? Color.Green : Color.GreenYellow;
                        else
                            output.color = this.episodeGotCount == 0 ? Color.Red : Color.Orange;
                    }
                    return output;
                }
            }

            #endregion

            #region Nested type: SummaryOutput

            public class SummaryOutput
            {
                public Color color;
                public string details;
            }

            #endregion
        }

        #endregion
    }
}
