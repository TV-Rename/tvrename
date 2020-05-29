// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using SourceGrid;
using SourceGrid.Cells.Controllers;
using SourceGrid.Cells.Views;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using ContentAlignment = DevAge.Drawing.ContentAlignment;
using Directory = System.IO.Directory;
using RowHeader = SourceGrid.Cells.RowHeader;

namespace TVRename
{
    public partial class ShowSummary : Form,IDialogParent
    {
        private readonly TVDoc mDoc;

        private List<string> mFoldersToOpen;
        private List<FileInfo> mLastFileList;
        private ProcessedSeason mLastProcessedSeasonClicked;
        private ShowItem mLastShowClicked;
        private readonly List<ShowSummaryData> showList;

        public ShowSummary(TVDoc doc)
        {
            InitializeComponent();

            mDoc = doc;
            showList = new List<ShowSummaryData>();
        }

        public void GenerateData()
        {
            foreach (ShowItem si in mDoc.Library.GetSortedShowItems())
            {
                showList.Add(AddShowDetails(si));
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
            for (int c = chkHideSpecials.Checked?1:0; c < maxSeason + 1; c++)
            {
                h = new ColumnHeader(ProcessedSeason.UIFullSeasonWord(c))
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

            int r = 0; 
            foreach (ShowSummaryData show in showList)
            {
                //Ignore shows with no missing episodes
                if (chkHideComplete.Checked && !show.HasMssingEpisodes(chkHideSpecials.Checked, chkHideIgnored.Checked))
                {
                    continue;
                }

                //Ignore shows with no missing aired episodes
                if (chkHideUnaired.Checked && !show.HasAiredMssingEpisodes(chkHideSpecials.Checked, chkHideIgnored.Checked))
                {
                    continue;
                }

                //Ignore shows which do not have the missing check
                if (chkHideNotScanned.Checked && !show.ShowItem.DoMissingCheck)
                {
                    continue;
                }

                RowHeader rh = new RowHeader(show.ShowName)
                {
                    ResizeEnabled = false,
                    View = new Cell{ForeColor = show.ShowItem.DoMissingCheck ? Color.Black : Color.Gray}
                };

                //Gray if the show is not checked for missing episodes in the scan

                grid1[r + 1, 0] = rh;
                grid1[r + 1, 0].AddController(new ShowClickEvent(this, show.ShowItem));

                foreach (ShowSummaryData.ShowSummarySeasonData seasonData in show.SeasonDataList)
                {
                    ShowSummaryData.SummaryOutput output = seasonData.GetOuput();

                    //Ignore Season if checkbox is checked
                    if (chkHideSpecials.Checked && output.Special)
                    {
                        continue;
                    }

                    //Ignore Season if checkbox is checked
                    if (chkHideIgnored.Checked && output.Ignored)
                    {
                        continue;
                    }

                    grid1[r + 1, seasonData.SeasonNumber + 1] =
                        new SourceGrid.Cells.Cell(output.Details, typeof(string))
                        {
                            View = new Cell
                            {
                                BackColor = output.Color,
                                ForeColor = Color.White,
                                TextAlignment = ContentAlignment.BottomRight
                            },
                            Editor = {EditableMode = EditableMode.None}
                        };

                    grid1[r + 1, seasonData.SeasonNumber + 1].AddController(new ShowClickEvent(this, show.ShowItem, seasonData.ProcessedSeason));
                }
                r++;
            }
            grid1.AutoSizeCells();
        }

        private static int GetMaxSeason([NotNull] IEnumerable<ShowSummaryData> shows)
        {
            return shows.Select(x => x.MaxSeason).DefaultIfEmpty(0).Max();
        }

        [NotNull]
        private static ShowSummaryData AddShowDetails([NotNull] ShowItem si)
        {
            ShowSummaryData showSummary = new ShowSummaryData
                {
                    ShowName = si.ShowName,
                    ShowItem = si
                };

            if (si.TheSeries() != null)
            {
                foreach (int snum in si.AppropriateSeasons().Keys)
                {
                    ShowSummaryData.ShowSummarySeasonData seasonData = GetSeasonDetails(si, snum);
                    showSummary.AddSeason(seasonData);
                }
            }
            return showSummary;
        }

        [NotNull]
        private static ShowSummaryData.ShowSummarySeasonData GetSeasonDetails([NotNull] ShowItem si, int snum)
        {
            int epCount = 0;
            int epGotCount = 0;
            int epAiredCount = 0;
            DirFilesCache dfc = new DirFilesCache();
            ProcessedSeason processedSeason = null;

            Dictionary<int, ProcessedSeason> seasons = si.AppropriateSeasons();

            if (snum >= 0 && seasons.ContainsKey(snum))
            {
                processedSeason = seasons[snum];

                List<ProcessedEpisode> eis = si.SeasonEpisodes[snum];

                foreach (ProcessedEpisode ei in eis)
                {
                    epCount++;

                    // if has air date and has been aired in the past
                    if (ei.FirstAired != null && ei.FirstAired < DateTime.Now)
                    {
                        epAiredCount++;
                    }

                    List<FileInfo> fl = dfc.FindEpOnDisk(ei,false);
                    if (fl.Count != 0)
                    {
                        epGotCount++;
                    }
                }
            }
            return new ShowSummaryData.ShowSummarySeasonData(snum, epCount, epAiredCount, epGotCount, processedSeason,si.IgnoreSeasons.Contains(snum));
        }

        private void showRightClickMenu_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();
            RightClickCommands n = (RightClickCommands)e.ClickedItem.Tag;
            switch (n)
            {
                case RightClickCommands.kVisitTvSourceSeason:
                    {
                        TvdbFor(mLastProcessedSeasonClicked);
                        break;
                    }

                case RightClickCommands.kVisitTvSourceSeries:
                    {
                        TvdbFor(mLastShowClicked);
                        break;
                    }
                case RightClickCommands.kForceRefreshSeries:
                    ForceRefresh(mLastShowClicked);
                    break;
                default:
                    {
                        if (n >= RightClickCommands.kWatchBase && n < RightClickCommands.kOpenFolderBase)
                        {
                            int wn = n - RightClickCommands.kWatchBase;
                            if (mLastFileList != null && wn >= 0 && wn < mLastFileList.Count)
                            {
                                Helpers.OpenFile(mLastFileList[wn].FullName);
                            }
                        }
                        else if (n >= RightClickCommands.kOpenFolderBase)
                        {
                            int fnum = n - RightClickCommands.kOpenFolderBase;

                            if (fnum < mFoldersToOpen.Count)
                            {
                                string folder = mFoldersToOpen[fnum];

                                if (Directory.Exists(folder))
                                {
                                    Helpers.OpenFolder(folder);
                                }
                            }
                        }
                        else
                        {
                            Debug.Fail("Unknown right-click action " + n);
                        }

                        break;
                    }
            }
        }

        private static void TvdbFor([CanBeNull] ProcessedSeason seas)
        {
            if (seas is null)
            {
                return;
            }

            Helpers.OpenUrl(seas.TVDBWebsiteUrl);
        }

        private static void TvdbFor([CanBeNull] ShowItem si)
        {
            if (si is null)
            {
                return;
            }

            Helpers.OpenUrl(si.WebsiteUrl);
        }

        private delegate void ShowChildConsumer(Form childForm);
        public void ShowChildDialog(Form childForm)
        {
            if (InvokeRequired)
            {
                ShowChildConsumer d = ShowChildDialog;
                Invoke(d, childForm);
            }
            else
            {
                childForm.ShowDialog(this);
            }
        }

        private void ForceRefresh([CanBeNull] ShowItem si)
        {
            mDoc.ForceRefresh(new List<ShowItem> {si},false,false ,this);
        }

        #region Nested type: ShowClickEvent

        private class ShowClickEvent : ControllerBase
        {
            private readonly ShowSummary gridSummary;
            private readonly ProcessedSeason processedSeason;
            private readonly ShowItem show;

            public ShowClickEvent(ShowSummary gridSummary, ShowItem show)
            {
                this.show = show;
                this.gridSummary = gridSummary;
            }

            public ShowClickEvent(ShowSummary gridSummary, ShowItem show, ProcessedSeason processedSeason)
            {
                this.show = show;
                this.processedSeason = processedSeason;
                this.gridSummary = gridSummary;
            }

            public override void OnMouseDown(CellContext sender, [NotNull] MouseEventArgs e)
            {
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }

                gridSummary.showRightClickMenu.Items.Clear();

                ProcessedSeason seas = processedSeason;

                gridSummary.mLastFileList = new List<FileInfo>();
                gridSummary.mFoldersToOpen = new List<string>();

                gridSummary.mLastShowClicked = show;
                gridSummary.mLastProcessedSeasonClicked = processedSeason;

                if (show is null)
                {
                    return;
                }

                if (seas is null)
                {
                    GenerateMenu(gridSummary.showRightClickMenu, "Force Refresh",
                        RightClickCommands.kForceRefreshSeries);

                    GenerateSeparator(gridSummary.showRightClickMenu);
                }

                GenerateMenu(gridSummary.showRightClickMenu, "Visit thetvdb.com",
                    seas is null ? RightClickCommands.kVisitTvSourceSeries : RightClickCommands.kVisitTvSourceSeason);

                List<string> added = new List<string>();

                if (seas != null)
                {
                    GenerateOpenMenu(seas, added);
                }

                GenerateRightClickOpenMenu(added);

                if (seas != null)
                {
                    GenerateRightClickWatchMenu(seas);
                }

                Point pt = new Point(e.X, e.Y);
                gridSummary.showRightClickMenu.Show(sender.Grid.PointToScreen(pt));
            }

            private void GenerateOpenMenu([NotNull] ProcessedSeason seas, ICollection<string> added)
            {
                Dictionary<int, List<string>> afl = show.AllExistngFolderLocations();

                if (!afl.ContainsKey(seas.SeasonNumber))
                {
                    return;
                }

                int n = gridSummary.mFoldersToOpen.Count;
                bool first = true;
                foreach (string folder in afl[seas.SeasonNumber])
                {
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            GenerateSeparator(gridSummary.showRightClickMenu);
                            first = false;
                        }

                        GenerateMenu(gridSummary.showRightClickMenu, "Open: " + folder,
                            (int) RightClickCommands.kOpenFolderBase + n);

                        gridSummary.mFoldersToOpen.Add(folder);
                        n++;
                    }
                }
            }

            private void GenerateRightClickOpenMenu(ICollection<string> added)
            {
                int n = gridSummary.mFoldersToOpen.Count;
                bool first = true;

                foreach (KeyValuePair<int, List<string>> kvp in show.AllExistngFolderLocations())
                {
                    foreach (string folder in kvp.Value)
                    {
                        if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                        {
                            added.Add(folder); // don't show the same folder more than once
                            if (first)
                            {
                                GenerateSeparator(gridSummary.showRightClickMenu);
                                first = false;
                            }

                            GenerateMenu(gridSummary.showRightClickMenu, "Open: " + folder,
                                (int) RightClickCommands.kOpenFolderBase + n);

                            gridSummary.mFoldersToOpen.Add(folder);
                            n++;
                        }
                    }
                }
            }

            private void GenerateRightClickWatchMenu([NotNull] ProcessedSeason seas)
            {
                // for each episode in season, find it on disk
                bool first = true;
                DirFilesCache dfc = new DirFilesCache();
                foreach (ProcessedEpisode epds in show.SeasonEpisodes[seas.SeasonNumber])
                {
                    List<FileInfo> fl = dfc.FindEpOnDisk(epds, false);
                    if (fl.Count > 0)
                    {
                        if (first)
                        {
                            GenerateSeparator(gridSummary.showRightClickMenu);
                            first = false;
                        }

                        int n = gridSummary.mLastFileList.Count;
                        foreach (FileInfo fi in fl)
                        {
                            GenerateMenu(gridSummary.showRightClickMenu, "Watch: " + fi.FullName,
                                (int) RightClickCommands.kWatchBase + n);

                            gridSummary.mLastFileList.Add(fi);
                            n++;
                        }
                    }
                }
            }

            private static void GenerateMenu([NotNull] ContextMenuStrip showRightClickMenu, string menuName, RightClickCommands rightClickCommand)
            {
                GenerateMenu(showRightClickMenu, menuName, (int)rightClickCommand);
            }

            private static void GenerateMenu([NotNull] ContextMenuStrip showRightClickMenu, string menuName, int rightClickCommand)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(menuName) {Tag = rightClickCommand};
                showRightClickMenu.Items.Add(tsi);
            }

            private static void GenerateSeparator([NotNull] ContextMenuStrip showRightClickMenu)
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

            public void AddSeason([NotNull] ShowSummarySeasonData seasonData)
            {
                SeasonDataList.Add(seasonData);

                // set the max season number
                if (seasonData.SeasonNumber >= MaxSeason)
                {
                    MaxSeason = seasonData.SeasonNumber;
                }
            }

            #region Nested type: ShowSummarySeasonData

            public class ShowSummarySeasonData
            {
                private readonly int episodeAiredCount;
                private readonly int episodeCount;
                private readonly int episodeGotCount;
                public readonly ProcessedSeason ProcessedSeason;
                public readonly int SeasonNumber;
                public readonly bool Ignored;

                public ShowSummarySeasonData(int seasonNumber, int episodeCount, int episodeAiredCount, int episodeGotCount, ProcessedSeason processedSeason,bool ignored)
                {
                    SeasonNumber = seasonNumber;
                    this.episodeCount = episodeCount;
                    this.episodeAiredCount = episodeAiredCount;
                    this.episodeGotCount = episodeGotCount;
                    ProcessedSeason = processedSeason;
                    Ignored = ignored;
                }

                public bool IsSpecial => SeasonNumber == 0;

                [NotNull]
                public SummaryOutput GetOuput()
                {
                    SummaryOutput output = new SummaryOutput
                    {
                        Ignored = Ignored,
                        Special = IsSpecial
                    };

                    if (IsSpecial)
                    {
                        output.Details = $"{episodeGotCount} / {episodeCount}";
                        if (Ignored)
                        {
                            output.Color = Color.LightSlateGray;
                        }
                        else if (episodeGotCount == episodeCount)
                        {
                            output.Color = Color.Green;
                        }
                        else if (episodeGotCount == 0)
                        {
                            output.Color = Color.Red;
                        }
                        else
                        {
                            output.Color = Color.Orange;
                        }
                    }
                    else
                    {
                        // show amount of aired eps
                        output.Details = $"{episodeGotCount} / {episodeAiredCount}";
                        // show amount of unaired eps
                        if (episodeCount > episodeAiredCount)
                        {
                            output.Details += $" ({episodeCount - episodeAiredCount})";
                        }

                        if (Ignored)
                        {
                            output.Color = Color.LightSlateGray;
                        }
                        else if (episodeGotCount == episodeAiredCount)
                        {
                            output.Color = episodeCount - episodeAiredCount == 0 ? Color.Green : Color.GreenYellow;
                        }
                        else
                        {
                            output.Color = episodeGotCount == 0 ? Color.Red : Color.Orange;
                        }
                    }
                    return output;
                }

                public bool HasMissingEpisodes() => episodeGotCount!=episodeCount;

                public bool HasAiredMssingEpisodes() => episodeGotCount != episodeAiredCount;
            }

            #endregion

            #region Nested type: SummaryOutput

            public class SummaryOutput
            {
                public Color Color;
                public string Details;
                public bool Ignored;
                public bool Special;
            }
            #endregion

            public bool HasMssingEpisodes(bool ignoreSpecials,bool ignoreIgnoredSeasons)
            {
                foreach (ShowSummarySeasonData ssn in SeasonDataList)
                {
                    if (ignoreIgnoredSeasons && ssn.Ignored)
                    {
                        continue;
                    }

                    if (ignoreSpecials && ssn.IsSpecial)
                    {
                        continue;
                    }

                    if (ssn.HasMissingEpisodes())
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool HasAiredMssingEpisodes(bool ignoreSpecials, bool ignoreIgnoredSeasons)
            {
                foreach (ShowSummarySeasonData ssn in SeasonDataList)
                {
                    if (ignoreIgnoredSeasons && ssn.Ignored)
                    {
                        continue;
                    }

                    if (ignoreSpecials && ssn.IsSpecial)
                    {
                        continue;
                    }

                    if (ssn.HasAiredMssingEpisodes())
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        private void chkHideIgnored_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void chkHideSpecials_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            chkHideIgnored.Checked = false;
            chkHideComplete.Checked = false;
            chkHideSpecials.Checked = false;
            chkHideUnaired.Checked = false;
            chkHideNotScanned.Checked = false;
            PopulateGrid();
        }

        private void chkHideComplete_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void chkHideNotScanned_CheckedChanged(object sender, EventArgs e)
        {
            PopulateGrid();
        }
    }

    public interface IDialogParent: IWin32Window
    {
        void ShowChildDialog(Form childForm);
    }
}
