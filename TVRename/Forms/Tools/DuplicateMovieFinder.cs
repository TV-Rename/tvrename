using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

namespace TVRename.Forms
{
    public partial class DuplicateMovieFinder : Form
    {
        private readonly List<DuplicateMovie> dupMovies;
        private readonly TVDoc mDoc;
        private readonly UI mainUi;

        public DuplicateMovieFinder([NotNull] TVDoc doc, UI main)
        {
            InitializeComponent();
            dupMovies = new List<DuplicateMovie>();
            mDoc = doc;
            mainUi = main;
            Scan();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            olvDuplicates.SetObjects(dupMovies);
        }

        private void AddRcMenuItem(string label, EventHandler command)
        {
            ToolStripMenuItem tsi = new ToolStripMenuItem(label);
            tsi.Click += command;
            possibleMergedEpisodeRightClickMenu.Items.Add(tsi);
        }

        private void PossibleMergedEpisodeRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            possibleMergedEpisodeRightClickMenu.Close();
        }

        private void BwScan_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker) sender;
            int total = mDoc.FilmLibrary.Movies.Count();
            int current = 0;

            dupMovies.Clear();
            foreach (MovieConfiguration? movie in mDoc.FilmLibrary.Movies)
            {
                List<FileInfo> files = movie.Locations
                    .Select(s => new DirectoryInfo(s))
                    .Where(info => info.Exists)
                    .SelectMany(d=>d.GetFiles())
                    .Where(f => f.IsMovieFile())
                    .Distinct()
                    .ToList();

                if (files.Count > 1)
                {
                    DuplicateMovie duplicateMovie = new DuplicateMovie{Movie=movie,Files=files};
                    dupMovies.Add(duplicateMovie);

                    duplicateMovie.IsSample = files.Any(f => f.IsSampleFile());
                    duplicateMovie.IsDeleted = files.Any(f => f.IsDeletedStubFile());

                    if (files.Count == 2)
                    {
                       duplicateMovie.IsDoublePart = FileHelper.IsDoublePartMovie(files[0],files[1]);
                    }
                }

                bw.ReportProgress(100 * current++ / total, movie.ShowName);
            }
        }

        private void BwScan_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState.ToString();
        }

        private void BwScan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRefresh.Visible = true;
            pbProgress.Visible = false;
            lblStatus.Visible = false;
            if (olvDuplicates.IsDisposed)
            {
                return;
            }

            UpdateUI();
        }
        private void BtnRefresh_Click_1(object sender, EventArgs e)
        {
            Scan();
        }

        private void Scan()
        {
            btnRefresh.Visible = false;
            pbProgress.Visible = true;
            lblStatus.Visible = true;
            bwScan.RunWorkerAsync();
        }

        private void olvDuplicates_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
        {
            if (e.Model is null)
            {
                return;
            }

            DuplicateMovie mlastSelected = (DuplicateMovie) e.Model;
            MovieConfiguration si = mlastSelected.Movie;

            possibleMergedEpisodeRightClickMenu.Items.Clear();

            AddRcMenuItem("Force Refresh", (o, args) => mainUi.ForceMovieRefresh(new List<MovieConfiguration> { si }, false));
            AddRcMenuItem("Edit Movie", (o, args) => mainUi.EditMovie(si));
            possibleMergedEpisodeRightClickMenu.Items.Add(new ToolStripSeparator());
            foreach (FileInfo? f in mlastSelected.Files)
            {
                AddRcMenuItem("Visit " + f.FullName, (o, args) => Helpers.OpenFolderSelectFile(f.FullName));
            }
        }
    }

    public class DuplicateMovie
    {
        internal MovieConfiguration Movie;
        internal List<FileInfo> Files;
        public bool IsDoublePart;
        public bool IsSample;
        public bool IsDeleted;
        public string Name => Movie.ShowName;
        public string Filenames => Files.Select(info => info.FullName).ToCsv();
    }
}

