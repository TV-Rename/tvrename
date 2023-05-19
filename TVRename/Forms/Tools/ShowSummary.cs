//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using SourceGrid;
using SourceGrid.Cells.Controllers;
using SourceGrid.Cells.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TVRename.Forms;
using ColumnHeader = SourceGrid.Cells.ColumnHeader;
using ContentAlignment = DevAge.Drawing.ContentAlignment;
using Directory = System.IO.Directory;
using RowHeader = SourceGrid.Cells.RowHeader;

namespace TVRename;

public partial class ShowSummary : Form, IDialogParent
{
    private UI MainWindow { get; }
    private readonly TVDoc mDoc;

    private readonly SafeList<ShowSummaryData> showList;

    public ShowSummary(TVDoc doc, UI parent)
    {
        MainWindow = parent;
        mDoc = doc;
        showList = new SafeList<ShowSummaryData>();

        InitializeComponent();
        InitializeCmbShowStatus();

        Scan();
    }

    private void InitializeCmbShowStatus()
    {
        if (mDoc.TvLibrary.ShowStatuses.HasAny())
        {
            cmbShowStatus.Items.Clear();
            foreach (string s in mDoc.TvLibrary.ShowStatuses)
            {
                cmbShowStatus.Items.Add(s);
            }
            cmbShowStatus.SelectedIndex = 0;
        }
    }

    private void GenerateData(BackgroundWorker bw)
    {
        int total = mDoc.TvLibrary.Shows.Count();
        int current = 0;
        showList.Clear();

        foreach (ShowConfiguration si in mDoc.TvLibrary.GetSortedShowItems())
        {
            bw.ReportProgress(100 * current++ / total, si.ShowName);
            showList.Add(AddShowDetails(si));
        }
    }

    private void PopulateGrid()
    {
        cmbShowStatus.Enabled = chkOnlyShow.Checked;

        if (grid1.IsDisposed || !showList.Any())
        {
            return;
        }

        Cell colTitleModel = new()
        {
            BackColor = Color.SteelBlue,
            ForeColor = Color.White,
            TextAlignment = ContentAlignment.BottomCenter
        };

        Cell topleftTitleModel = new()
        {
            BackColor = Color.SteelBlue,
            ForeColor = Color.White,
            TextAlignment = ContentAlignment.BottomLeft
        };

        grid1.Columns.Clear();
        grid1.Rows.Clear();

        int maxSeason = GetMaxSeason(showList);

        // Draw Header
        grid1.ColumnsCount = maxSeason + 3;
        grid1.RowsCount = showList.Count + 1;
        grid1.FixedColumns = 2;
        grid1.FixedRows = 1;
        grid1.Selection.EnableMultiSelection = false;

        grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
        grid1.Rows[0].Height = 65;

        grid1[0, 0] = new ColumnHeader("Show")
        {
            AutomaticSortEnabled = false,
            ResizeEnabled = false,
            View = topleftTitleModel
        };

        grid1[0, 1] = new ColumnHeader("Status")
        {
            AutomaticSortEnabled = false,
            ResizeEnabled = false,
            View = topleftTitleModel
        };

        // Draw season
        for (int c = chkHideSpecials.Checked ? 1 : 0; c < maxSeason + 1; c++)
        {
            grid1[0, c + 2] = new ColumnHeader(c == 0 ? ProcessedSeason.UIFullSeasonWord(c) : "S" + c.Pad(2))
            {
                AutomaticSortEnabled = false,
                ResizeEnabled = false,
                View = colTitleModel
            };

            grid1.Columns[c + 2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
        }

        grid1.Columns[0].Width = 150;

        // Draw Shows

        int row = 1;
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
            if (chkHideNotScanned.Checked && !show.ShowConfiguration.DoMissingCheck)
            {
                continue;
            }

            if (chkOnlyShow.Checked &&
                !show.ShowConfiguration.ShowStatus.Equals(cmbShowStatus.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (chkHideDiskEps.Checked && show.HasEpisodesOnDisk())
            {
                continue;
            }

            //Gray if the show is not checked for missing episodes in the scan
            Color cellColour = show.ShowConfiguration.DoMissingCheck ? Color.Black : Color.Gray;
            if (show.ShowConfiguration.DoMissingCheck && TVSettings.Instance.ShowStatusColors.AppliesTo(show.ShowConfiguration))
            {
                cellColour = TVSettings.Instance.ShowStatusColors.GetColour(show.ShowConfiguration);
            }

            grid1[row, 0] = new RowHeader(show.ShowName)
            {
                ResizeEnabled = false,
                View = new Cell { ForeColor = cellColour }
            };
            grid1[row, 0].AddController(new ShowClickEvent(this, show.ShowConfiguration, mDoc));

            grid1[row, 1] = new RowHeader(show.ShowConfiguration.ShowStatus)
            {
                ResizeEnabled = false,
                View = new Cell { ForeColor = cellColour }
            };

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

                grid1[row, seasonData.SeasonNumber + 2] =
                    new SourceGrid.Cells.Cell(output.Details, typeof(string))
                    {
                        View = new Cell
                        {
                            BackColor = output.Color,
                            ForeColor = Color.White,
                            TextAlignment = ContentAlignment.BottomRight
                        },
                        Editor = { EditableMode = EditableMode.None }
                    };

                grid1[row, seasonData.SeasonNumber + 2].AddController(new ShowClickEvent(this, show.ShowConfiguration, seasonData.ProcessedSeason, mDoc));
            }
            row++;
        }
        grid1.AutoSizeCells();
    }

    private static int GetMaxSeason(IEnumerable<ShowSummaryData> shows)
    {
        return shows.Select(x => x.MaxSeason).DefaultIfEmpty(0).Max();
    }

    private static ShowSummaryData AddShowDetails(ShowConfiguration si)
    {
        ShowSummaryData showSummary = new(showName: si.ShowName, showConfiguration: si);

        if (si.CachedShow != null)
        {
            foreach (int snum in si.AppropriateSeasons().Keys)
            {
                ShowSummaryData.ShowSummarySeasonData? seasonData = GetSeasonDetails(si, snum);
                if (seasonData != null)
                {
                    showSummary.AddSeason(seasonData);
                }
            }
        }
        return showSummary;
    }

    private static ShowSummaryData.ShowSummarySeasonData? GetSeasonDetails(ShowConfiguration si, int snum)
    {
        int epCount = 0;
        int epGotCount = 0;
        int epAiredCount = 0;
        DirFilesCache dfc = new();
        ProcessedSeason? processedSeason = null;

        if (snum >= 0 && si.AppropriateSeasons().TryGetValue(snum, out processedSeason))
        {
            List<ProcessedEpisode> eis = si.SeasonEpisodes[snum];

            foreach (ProcessedEpisode ei in eis)
            {
                epCount++;

                // if has air date and has been aired in the past
                if (ei.FirstAired != null && ei.FirstAired < TimeHelpers.LocalNow())
                {
                    epAiredCount++;
                }

                List<FileInfo> fl = dfc.FindEpOnDisk(ei, false);
                if (fl.Count != 0)
                {
                    epGotCount++;
                }
            }
        }

        if (processedSeason != null)
        {
            return new ShowSummaryData.ShowSummarySeasonData(snum, epCount, epAiredCount, epGotCount, processedSeason);
        }

        return null;
    }

    private void showRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
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
        private readonly ShowConfiguration show;
        private readonly TVDoc mDoc;

        public ShowClickEvent(ShowSummary gridSummary, ShowConfiguration show, TVDoc doc)
        {
            this.show = show;
            this.gridSummary = gridSummary;
            this.mDoc = doc;
        }

        public ShowClickEvent(ShowSummary gridSummary, ShowConfiguration show, ProcessedSeason processedSeason, TVDoc doc)
        {
            this.show = show;
            this.processedSeason = processedSeason;
            this.gridSummary = gridSummary;
            this.mDoc = doc;
        }

        public override void OnMouseDown(CellContext sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            gridSummary.rightClickMenu.Items.Clear();

            if (processedSeason != null)
            {
                if (processedSeason.Show.IgnoreSeasons.Contains(processedSeason.SeasonNumber))
                {
                    gridSummary.rightClickMenu.Add("Stop Ignoring Season", (_, _) =>
                    {
                        processedSeason.Show.IgnoreSeasons.Remove(processedSeason.SeasonNumber);
                        mDoc.TvAddedOrEdited(false, false, false, null, processedSeason.Show);
                        gridSummary.PopulateGrid();
                        gridSummary.MainWindow.FillMyShows();
                    });
                }
                else
                {
                    gridSummary.rightClickMenu.Add("Ignore Season", (_, _) =>
                    {
                        processedSeason.Show.IgnoreSeasons.Add(processedSeason.SeasonNumber);
                        mDoc.TvAddedOrEdited(false, false, false, null, processedSeason.Show);
                        gridSummary.PopulateGrid();
                        gridSummary.MainWindow.FillMyShows();
                    });
                }
            }

            if (show.DoMissingCheck)
            {
                gridSummary.rightClickMenu.Add("Stop Checking TV Show", (_, _) =>
                {
                    show.DoMissingCheck = false;
                    mDoc.TvAddedOrEdited(false, false, false, null, show);
                    gridSummary.PopulateGrid();
                    gridSummary.MainWindow.FillMyShows();
                });
            }
            else
            {
                gridSummary.rightClickMenu.Add("Start Checking TV Show", (_, _) =>
                {
                    show.DoMissingCheck = true;
                    mDoc.TvAddedOrEdited(false, false, false, null, show);
                    gridSummary.PopulateGrid();
                    gridSummary.MainWindow.FillMyShows();
                });
            }

            gridSummary.rightClickMenu.AddSeparator();

            if (processedSeason is null)
            {
                gridSummary.rightClickMenu.Add("Force Refresh", (_, _) =>
                {
                    gridSummary.MainWindow.ForceRefresh(show, false);
                });

                gridSummary.rightClickMenu.AddSeparator();
            }

            gridSummary.rightClickMenu.Add("Visit Source",
                (_, _) =>
                {
                    if (processedSeason is null)
                    {
                        UI.TvSourceFor(show);
                    }
                    else
                    {
                        UI.TvSourceFor(processedSeason);
                    }
                }
            );

            List<string> added = new();

            if (processedSeason != null)
            {
                GenerateOpenMenu(processedSeason, added);
            }

            GenerateRightClickOpenMenu(added);

            if (processedSeason != null)
            {
                GenerateRightClickWatchMenu(processedSeason);
            }

            Point pt = new(e.X, e.Y);
            gridSummary.rightClickMenu.Show(sender.Grid.PointToScreen(pt));
        }

        private void GenerateOpenMenu(ProcessedSeason seas, ICollection<string> added)
        {
            Dictionary<int, SafeList<string>> afl = show.AllExistngFolderLocations();

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
                        gridSummary.rightClickMenu.AddSeparator();
                        first = false;
                    }

                    gridSummary.rightClickMenu.Add("Open: " + folder, (_, _) =>
                    {
                        folder.OpenFolder();
                    });
                }
            }
        }

        private void GenerateRightClickOpenMenu(ICollection<string> added)
        {
            bool first = true;

            foreach (KeyValuePair<int, SafeList<string>> kvp in show.AllExistngFolderLocations().OrderBy(pair => pair.Key))
            {
                foreach (string folder in kvp.Value)
                {
                    if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder) && !added.Contains(folder))
                    {
                        added.Add(folder); // don't show the same folder more than once
                        if (first)
                        {
                            gridSummary.rightClickMenu.AddSeparator();
                            first = false;
                        }

                        gridSummary.rightClickMenu.Add("Open: " + folder, (_, _) =>
                        {
                            folder.OpenFolder();
                        });
                    }
                }
            }
        }

        private void GenerateRightClickWatchMenu(ProcessedSeason seas)
        {
            // for each episode in season, find it on disk
            bool first = true;
            DirFilesCache dfc = new();
            foreach (ProcessedEpisode epds in show.SeasonEpisodes[seas.SeasonNumber])
            {
                List<FileInfo> fl = dfc.FindEpOnDisk(epds, false);
                if (fl.Any())
                {
                    if (first)
                    {
                        gridSummary.rightClickMenu.AddSeparator();
                        first = false;
                    }

                    foreach (FileInfo fi in fl)
                    {
                        gridSummary.rightClickMenu.Items.Add("Watch: " + fi.FullName, (_, _) => fi.OpenFile());
                    }
                }
            }
        }
    }

    #endregion Nested type: ShowClickEvent

    #region Nested type: ShowSummaryData

    public class ShowSummaryData
    {
        public int MaxSeason;
        public readonly List<ShowSummarySeasonData> SeasonDataList = new();
        public readonly ShowConfiguration ShowConfiguration;
        public readonly string ShowName;

        public ShowSummaryData(string showName, ShowConfiguration showConfiguration)
        {
            ShowName = showName;
            ShowConfiguration = showConfiguration;
        }

        public void AddSeason(ShowSummarySeasonData seasonData)
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

            public ShowSummarySeasonData(int seasonNumber, int episodeCount, int episodeAiredCount, int episodeGotCount, ProcessedSeason processedSeason)
            {
                SeasonNumber = seasonNumber;
                this.episodeCount = episodeCount;
                this.episodeAiredCount = episodeAiredCount;
                this.episodeGotCount = episodeGotCount;
                ProcessedSeason = processedSeason;
            }

            public bool IsSpecial => SeasonNumber == 0;
            public bool Ignored => ProcessedSeason.Show.IgnoreSeasons.Contains(SeasonNumber);

            public SummaryOutput GetOuput()
            {
                SummaryOutput output = new(ignored: Ignored, special: IsSpecial);

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

            public bool HasMissingEpisodes() => episodeGotCount != episodeCount;

            public bool HasAiredMssingEpisodes() => episodeGotCount != episodeAiredCount;

            public bool HasEpisodesOnDisk() => episodeGotCount > 0;
        }

        #endregion Nested type: ShowSummarySeasonData

        #region Nested type: SummaryOutput

        public class SummaryOutput
        {
            public Color Color;
            public string? Details;
            public readonly bool Ignored;
            public readonly bool Special;

            public SummaryOutput(bool ignored, bool special)
            {
                Ignored = ignored;
                Special = special;
            }
        }

        #endregion Nested type: SummaryOutput

        public bool HasMssingEpisodes(bool ignoreSpecials, bool ignoreIgnoredSeasons)
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

    #endregion Nested type: ShowSummaryData

    private void CheckedChanged(object sender, EventArgs e) => PopulateGrid();

    private void btnClear_Click(object sender, EventArgs e)
    {
        chkHideIgnored.Checked = false;
        chkHideComplete.Checked = false;
        chkHideSpecials.Checked = false;
        chkHideUnaired.Checked = false;
        chkHideNotScanned.Checked = false;
        chkOnlyShow.Checked = false;
        chkHideDiskEps.Checked = false;

        PopulateGrid();
    }

    private void Scan()
    {
        btnRefresh.Visible = false;
        EnableCheckboxes(false);

        pbProgress.Visible = true;
        lblStatus.Visible = true;
        bwRescan.RunWorkerAsync();
    }

    private void EnableCheckboxes(bool enabled)
    {
        chkHideIgnored.Enabled = enabled;
        chkHideComplete.Enabled = enabled;
        chkHideSpecials.Enabled = enabled;
        chkHideUnaired.Enabled = enabled;
        chkHideNotScanned.Enabled = enabled;
        chkOnlyShow.Enabled = enabled;
        chkHideDiskEps.Enabled = enabled;

        btnClear.Enabled = enabled;
    }

    private void BwRescan_DoWork(object sender, DoWorkEventArgs e)
    {
        System.Threading.Thread.CurrentThread.Name ??= "ShowSummary Scan Thread"; // Can only set it once
        GenerateData((BackgroundWorker)sender);
    }

    private void BwRescan_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        pbProgress.Value = e.ProgressPercentage.Between(0, 100);
        if (e.UserState is not null)
        {
            lblStatus.Text = e.UserState.ToString()?.ToUiVersion();
        }
    }

    private void BwRescan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        btnRefresh.Visible = true;
        EnableCheckboxes(true);

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

    private void cmbShowStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateGrid();
    }
}
