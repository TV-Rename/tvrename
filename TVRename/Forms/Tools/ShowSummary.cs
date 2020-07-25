// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private UI MainWindow { get; }
        private readonly TVDoc mDoc;

        private readonly List<ShowSummaryData> showList;

        public ShowSummary(TVDoc doc, UI parent)
        {
            MainWindow = parent;
            mDoc = doc;

            InitializeComponent();
            
            showList = new List<ShowSummaryData>();
            Scan();
        }

        private void GenerateData(BackgroundWorker bw)
        {
            int total = mDoc.Library.Count;
            int current = 0;
            showList.Clear();

            foreach (ShowItem si in mDoc.Library.GetSortedShowItems())
            {
                bw.ReportProgress(100 * current++ / total, si.ShowName);
                showList.Add(AddShowDetails(si));
            }
        }

        private void PopulateGrid()
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

            int cols = maxSeason + 3;
            int rows = showList.Count + 1;

            // Draw Header
            grid1.ColumnsCount = cols;
            grid1.RowsCount = rows;
            grid1.FixedColumns = 2;
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

            ColumnHeader h2 = new ColumnHeader("Status")
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false
            };

            grid1[0, 1] = h2;
            grid1[0, 1].View = topleftTitleModel;

            // Draw season
            for (int c = chkHideSpecials.Checked?1:0; c < maxSeason + 1; c++)
            {
                h = new ColumnHeader(ProcessedSeason.UIFullSeasonWord(c))
                {
                    AutomaticSortEnabled = false,
                    ResizeEnabled = false
                };

                grid1[0, c + 2] = h;
                grid1[0, c + 2].View = colTitleModel;

                grid1.Columns[c + 2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
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

                if (chkOnlyShowEnded.Checked &&
                    !show.ShowItem.ShowStatus.Equals("Ended", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (chkHideDiskEps.Checked && show.HasEpisodesOnDisk())
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

                RowHeader rh2 = new RowHeader(show.ShowItem.ShowStatus)
                {
                    ResizeEnabled = false,
                    View = new Cell { ForeColor = show.ShowItem.DoMissingCheck ? Color.Black : Color.Gray }
                };

                //Gray if the show is not checked for missing episodes in the scan

                grid1[r + 1, 1] = rh2;

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

                    grid1[r + 1, seasonData.SeasonNumber + 2] =
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

                    grid1[r + 1, seasonData.SeasonNumber + 2].AddController(new ShowClickEvent(this, show.ShowItem, seasonData.ProcessedSeason));
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
                    if (seasonData != null)
                    {
                        showSummary.AddSeason(seasonData);
                    }
                }
            }
            return showSummary;
        }

        private static ShowSummaryData.ShowSummarySeasonData? GetSeasonDetails([NotNull] ShowItem si, int snum)
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

            if (processedSeason != null)
            {
                return new ShowSummaryData.ShowSummarySeasonData(snum, epCount, epAiredCount, epGotCount,
                    processedSeason, si.IgnoreSeasons.Contains(snum));
            }

            return null;
        }

        private void showRightClickMenu_ItemClicked(object sender, [NotNull] ToolStripItemClickedEventArgs e)
        {
            showRightClickMenu.Close();
        }

        private static void TvdbFor(ProcessedSeason? seas)
        {
            if (seas is null)
            {
                return;
            }

            Helpers.OpenUrl(seas.TVDBWebsiteUrl);
        }

        private static void TvdbFor(ShowItem? si)
        {
            if (si?.WebsiteUrl is null)
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

        #region Nested type: ShowClickEvent

        private class ShowClickEvent : ControllerBase
        {
            private readonly ShowSummary gridSummary;
            private readonly ProcessedSeason? processedSeason;
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

                if (seas is null)
                {
                    AddRcMenuItem(gridSummary.showRightClickMenu, "Force Refresh",(o, args) =>
                    {
                        gridSummary.MainWindow.ForceRefresh(new List<ShowItem> {show}, false);
                    }
                        );

                    GenerateSeparator(gridSummary.showRightClickMenu);
                }

                AddRcMenuItem(gridSummary.showRightClickMenu, "Visit thetvdb.com",
                    (o, args) =>
                    {
                        if (seas is null)
                        {
                            TvdbFor(show);
                        }
                        else
                        {
                            TvdbFor(processedSeason);
                        }
                    }
                );

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

                bool first = true;
                foreach (string folder in afl[seas.SeasonNumber].OrderBy(s => s))
                {
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            GenerateSeparator(gridSummary.showRightClickMenu);
                            first = false;
                        }

                        AddRcMenuItem(gridSummary.showRightClickMenu, "Open: " + folder, (sender, args) =>
                        {
                            Helpers.OpenFolder(folder);
                        }); 
                    }
                }
            }

            private void GenerateRightClickOpenMenu(ICollection<string> added)
            {
                bool first = true;

                foreach (KeyValuePair<int, List<string>> kvp in show.AllExistngFolderLocations().OrderBy(pair => pair.Key))
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

                            AddRcMenuItem(gridSummary.showRightClickMenu, "Open: " + folder, (sender, args) =>
                            {
                                Helpers.OpenFolder(folder);
                            });
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

                        foreach (FileInfo fi in fl)
                        {
                            ToolStripMenuItem tsi = new ToolStripMenuItem("Watch: " + fi.FullName);
                            tsi.Click += (sender, args) => { Helpers.OpenFile(fi.FullName); };
                            gridSummary.showRightClickMenu.Items.Add(tsi);
                        }
                    }
                }
            }
            private void AddRcMenuItem([NotNull] ContextMenuStrip showRightClickMenu, string name, EventHandler command)
            {
                ToolStripMenuItem tsi = new ToolStripMenuItem(name);
                tsi.Click += command;
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

                public bool HasEpisodesOnDisk() => episodeGotCount > 0;
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

            public bool HasEpisodesOnDisk()
            {
                return SeasonDataList.Any(ssn => ssn.HasEpisodesOnDisk());
            }
        }
        #endregion

        private void CheckedChanged(object sender, EventArgs e) => PopulateGrid();

        private void btnClear_Click(object sender, EventArgs e)
        {
            chkHideIgnored.Checked = false;
            chkHideComplete.Checked = false;
            chkHideSpecials.Checked = false;
            chkHideUnaired.Checked = false;
            chkHideNotScanned.Checked = false;
            chkOnlyShowEnded.Checked = false;
            chkHideDiskEps.Checked = false;

            PopulateGrid();
        }

        private void Scan()
        {
            btnRefresh.Visible = false;
            pbProgress.Visible = true;
            lblStatus.Visible = true;
            bwRescan.RunWorkerAsync();
        }

        private void BwRescan_DoWork(object sender, DoWorkEventArgs e)
        {
            GenerateData((BackgroundWorker)sender);
        }

        private void BwRescan_ProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState.ToString();
        }

        private void BwRescan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRefresh.Visible = true;
            pbProgress.Visible = false;
            lblStatus.Visible = false;
            PopulateGrid();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Scan();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
