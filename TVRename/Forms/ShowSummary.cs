// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using SourceGrid;
using SourceGrid.Cells.Controllers;
using SourceGrid.Cells.Views;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using ContentAlignment = DevAge.Drawing.ContentAlignment;
using Directory = System.IO.Directory;
using RowHeader = SourceGrid.Cells.RowHeader;

namespace TVRename
{
    public partial class ShowSummary : Form
    {
        private readonly TVDoc mDoc;

        private List<string> mFoldersToOpen;
        private List<FileInfo> mLastFileList;
        private Season mLastSeasonClicked;
        private ShowItem mLastShowClicked;
        private List<ShowSummaryData> showList;

        public ShowSummary(TVDoc doc)
        {
            InitializeComponent();

            mDoc = doc;
            showList = new List<ShowSummaryData>();
        }

        public void GenerateData()
        {
            foreach (ShowItem si in mDoc.Library.GetShowItems())
            {
                ShowSummaryData show = AddShowDetails(si);
                showList.Add(show);
            }
        }

        public void PopulateGrid()
        {
            Cell colTitleModel = new Cell
            {
                ElementText = new ActorsGrid.RotatedText(-90.0f),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = ContentAlignment.BottomCenter
            };

            Cell topleftTitleModel = new Cell
            {
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                TextAlignment = ContentAlignment.BottomLeft
            };

            grid1.Columns.Clear();
            grid1.Rows.Clear();

            int maxSeason = GetMaxSeason(showList);

            int cols = maxSeason + 2;
            int rows = showList.Count + 1;

            // Draw Header
            grid1.ColumnsCount = cols;
            grid1.RowsCount = rows;
            grid1.FixedColumns = 1;
            grid1.FixedRows = 1;
            grid1.Selection.EnableMultiSelection = false;

            grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
            grid1.Rows[0].Height = 65;

            ColumnHeader h = new ColumnHeader("Show")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };

            grid1[0, 0] = h;
            grid1[0, 0].View = topleftTitleModel;

            // Draw season
            for (int c = 0; c < maxSeason + 1; c++)
            {
                h = new ColumnHeader(c == 0 ? "Specials" : $"Season {c}")
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false
                };

                grid1[0, c + 1] = h;
                grid1[0, c + 1].View = colTitleModel;

                grid1.Columns[c + 1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            }

            grid1.Columns[0].Width = 150;

            // Draw Shows

            int r = 0; // TODO: remove reliance on index
            foreach (ShowSummaryData show in showList)
            {
                RowHeader rh = new RowHeader(show.ShowName) {ResizeEnabled = false};

                grid1[r + 1, 0] = rh;
                grid1[r + 1, 0].AddController(new ShowClickEvent(this, show.ShowItem));

                foreach (ShowSummaryData.ShowSummarySeasonData seasonData in show.SeasonDataList)
                {
                    ShowSummaryData.SummaryOutput output = seasonData.GetOuput();
                    grid1[r + 1, seasonData.SeasonNumber + 1] = new SourceGrid.Cells.Cell(output.Details, typeof(string));
                    grid1[r + 1, seasonData.SeasonNumber + 1].View = new Cell
                    {
                        BackColor = output.Color,
                        ForeColor = Color.White,
                        TextAlignment = ContentAlignment.BottomRight
                    };
                    grid1[r + 1, seasonData.SeasonNumber + 1].AddController(new ShowClickEvent(this, show.ShowItem, seasonData.Season));
                    grid1[r + 1, seasonData.SeasonNumber + 1].Editor.EditableMode = EditableMode.None;
                }
                r++;
            }
            grid1.AutoSizeCells();
        }

        private int GetMaxSeason(IEnumerable<ShowSummaryData> showList)
        {
            int maxSeason = 0;
            foreach (ShowSummaryData show in showList)
            {
                if (show.MaxSeason > maxSeason)
                    maxSeason = show.MaxSeason;
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
                ShowName = si.ShowName,
                ShowItem = si
            };

            if (ser != null)
            {
                foreach (int snum in si.DVDOrder? ser.DVDSeasons.Keys: ser.AiredSeasons.Keys)
                {
                    ShowSummaryData.ShowSummarySeasonData seasonData = getSeasonDetails(si, ser, snum);
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

            Dictionary<int, Season> seasons = si.DVDOrder ? ser.DVDSeasons : ser.AiredSeasons;

            if ((snum >= 0) && (seasons.ContainsKey(snum)))
            {
                season = seasons[snum];

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

                    List<FileInfo> fl = TVDoc.FindEpOnDisk(dfc,ei,false);
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
            showRightClickMenu.Close();
            RightClickCommands n = (RightClickCommands)e.ClickedItem.Tag;
            switch (n)
            {
                case RightClickCommands.kVisitTVDBSeason:
                    {
                        TVDBFor(mLastSeasonClicked);
                        break;
                    }

                case RightClickCommands.kVisitTVDBSeries:
                    {
                        TVDBFor(mLastShowClicked);
                        break;
                    }
                case RightClickCommands.kForceRefreshSeries:
                    ForceRefresh(mLastShowClicked);
                    break;
                default:
                    {
                        if ((n >= RightClickCommands.kWatchBase) && (n < RightClickCommands.kOpenFolderBase))
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if ((mLastFileList != null) && (wn >= 0) && (wn < mLastFileList.Count))
                                Helpers.SysOpen(mLastFileList[wn].FullName);
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < mFoldersToOpen.Count)
                            {
                                string folder = mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                    Helpers.SysOpen(folder);
                            }
                        }
                        else
                            Debug.Fail("Unknown right-click action " + n);
                        break;
                    }
            }
        }

        private void TVDBFor(Season seas)
        {
            if (seas == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteURL(seas.TheSeries.TVDBCode, seas.SeasonID, false));
        }

        private void TVDBFor(ShowItem si)
        {
            if (si == null)
                return;

            Helpers.SysOpen(TheTVDB.Instance.WebsiteURL(si.TVDBCode, -1, false));
        }

        private void ForceRefresh(ShowItem si)
        {
            if (si != null)
                TheTVDB.Instance.ForgetShow(si.TVDBCode, true);
            mDoc.DoDownloadsFG();
        }

        #region Nested type: ShowClickEvent

        private class ShowClickEvent : ControllerBase
        {
            private readonly ShowSummary gridSummary;
            private readonly Season season;
            private readonly ShowItem show;

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

            public override void OnMouseDown(CellContext sender, MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Right)
                    return;
                gridSummary.showRightClickMenu.Items.Clear();

                Season seas = season;

                gridSummary.mLastFileList = new List<FileInfo>();
                gridSummary.mFoldersToOpen = new List<String>();

                gridSummary.mLastShowClicked = show;
                gridSummary.mLastSeasonClicked = season;

                List<String> added = new List<String>();

                if (show != null && seas == null)
                {
                    GenerateMenu(gridSummary.showRightClickMenu, "Force Refresh", RightClickCommands.kForceRefreshSeries);
                    GenerateSeparator(gridSummary.showRightClickMenu);
                    GenerateMenu(gridSummary.showRightClickMenu, "Visit thetvdb.com", RightClickCommands.kVisitTVDBSeries);
                }

                if (show != null && seas != null)
                    GenerateMenu(gridSummary.showRightClickMenu, "Visit thetvdb.com", RightClickCommands.kVisitTVDBSeason);

                if ((seas != null) && (show != null) && (show.AllFolderLocations().ContainsKey(seas.SeasonNumber)))
                {
                    int n = gridSummary.mFoldersToOpen.Count;
                    bool first = true;
                    foreach (string folder in show.AllFolderLocations()[seas.SeasonNumber])
                    {
                        if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                GenerateSeparator(gridSummary.showRightClickMenu);
                                first = false;
                            }

                            GenerateMenu(gridSummary.showRightClickMenu, "Open: " + folder, (int)RightClickCommands.kOpenFolderBase + n);
                            gridSummary.mFoldersToOpen.Add(folder);
                            n++;
                        }
                    }
                }
                else if (show != null)
                {
                    int n = gridSummary.mFoldersToOpen.Count;
                    bool first = true;

                    foreach (KeyValuePair<int, List<String>> kvp in show.AllFolderLocations())
                    {
                        foreach (string folder in kvp.Value)
                        {
                            if ((!string.IsNullOrEmpty(folder)) && Directory.Exists(folder) && !added.Contains(folder))
                            {
                                added.Add(folder); // don't show the same folder more than once
                                if (first)
                                {
                                    GenerateSeparator(gridSummary.showRightClickMenu);
                                    first = false;
                                }

                                GenerateMenu(gridSummary.showRightClickMenu, "Open: " + folder, (int)RightClickCommands.kOpenFolderBase + n);
                                gridSummary.mFoldersToOpen.Add(folder);
                                n++;
                            }
                        }
                    }
                }

                if (seas != null && show != null)
                {
                    // for each episode in season, find it on disk
                    bool first = true;
                    foreach (ProcessedEpisode epds in show.SeasonEpisodes[seas.SeasonNumber])
                    {
                        List<FileInfo> fl = TVDoc.FindEpOnDisk(new DirFilesCache() , epds,false);
                        if ((fl != null) && (fl.Count > 0))
                        {
                            if (first)
                            {
                                GenerateSeparator(gridSummary.showRightClickMenu);
                                first = false;
                            }

                            int n = gridSummary.mLastFileList.Count;
                            foreach (FileInfo fi in fl)
                            {
                                GenerateMenu(gridSummary.showRightClickMenu, "Watch: " + fi.FullName, (int)RightClickCommands.kWatchBase + n);
                                gridSummary.mLastFileList.Add(fi);
                                n++;
                            }
                        }
                    }
                }

                Point pt = new Point(e.X, e.Y);
                gridSummary.showRightClickMenu.Show(sender.Grid.PointToScreen(pt));
            }

            private void GenerateMenu(ContextMenuStrip showRightClickMenu, string menuName, RightClickCommands rightClickCommand)
            {
                GenerateMenu(showRightClickMenu, menuName, (int)rightClickCommand);
            }

            private void GenerateMenu(ContextMenuStrip showRightClickMenu, string menuName, int rightClickCommand)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(menuName) {Tag = rightClickCommand};
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
            public int MaxSeason;
            public readonly List<ShowSummarySeasonData> SeasonDataList = new List<ShowSummarySeasonData>();
            public ShowItem ShowItem;
            public string ShowName;

            public void AddSeason(ShowSummarySeasonData seasonData)
            {
                SeasonDataList.Add(seasonData);

                // set the max season number
                if (seasonData.SeasonNumber >= MaxSeason)
                    MaxSeason = seasonData.SeasonNumber;
            }

            #region Nested type: ShowSummarySeasonData

            public class ShowSummarySeasonData
            {
                private readonly int episodeAiredCount;
                private readonly int episodeCount;
                private readonly int episodeGotCount;
                public readonly Season Season;
                public readonly int SeasonNumber;

                public ShowSummarySeasonData(int seasonNumber, int episodeCount, int episodeAiredCount, int episodeGotCount, Season season)
                {
                    SeasonNumber = seasonNumber;
                    this.episodeCount = episodeCount;
                    this.episodeAiredCount = episodeAiredCount;
                    this.episodeGotCount = episodeGotCount;
                    Season = season;
                }

                public SummaryOutput GetOuput()
                {
                    SummaryOutput output = new SummaryOutput();
                    if (SeasonNumber == 0)
                    {
                        output.Details = $"{episodeGotCount} / {episodeCount}";
                        if (episodeGotCount == episodeCount)
                            output.Color = Color.Green;
                        else if (episodeGotCount == 0)
                            output.Color = Color.Red;
                        else
                            output.Color = Color.Orange;
                    }
                    else
                    {
                        // show amount of aired eps
                        output.Details = $"{episodeGotCount} / {episodeAiredCount}";
                        // show amount of unaired eps
                        output.Details += episodeCount - episodeAiredCount == 0 ? string.Empty : $" ({episodeCount - episodeAiredCount})";

                        if (episodeGotCount == episodeAiredCount)
                            output.Color = (episodeCount - episodeAiredCount) == 0 ? Color.Green : Color.GreenYellow;
                        else
                            output.Color = episodeGotCount == 0 ? Color.Red : Color.Orange;
                    }
                    return output;
                }
            }

            #endregion

            #region Nested type: SummaryOutput

            public class SummaryOutput
            {
                public Color Color;
                public string Details;
            }
            #endregion
        }
        #endregion
    }
}
