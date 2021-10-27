using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class DuplicateMovieFinder : Form
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
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

        private void AddRcMenuItem([NotNull] string label, EventHandler command)
        {
            ToolStripMenuItem tsi = new(label.Replace("&", "&&"));
            tsi.Click += command;
            possibleMergedEpisodeRightClickMenu.Items.Add(tsi);
        }

        private void PossibleMergedEpisodeRightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            possibleMergedEpisodeRightClickMenu.Close();
        }

        private void BwScan_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name ??= "DuplicateMovie Scan Thread"; // Can only set it once
            BackgroundWorker bw = (BackgroundWorker)sender;
            int total = mDoc.FilmLibrary.Movies.Count();
            int current = 0;

            dupMovies.Clear();
            foreach (MovieConfiguration? movie in mDoc.FilmLibrary.Movies)
            {
                ProcessMovie(movie);

                bw.ReportProgress(100 * current++ / total, movie.ShowName);
            }
        }

        private void ProcessMovie([NotNull] MovieConfiguration movie)
        {
            List<FileInfo> files = movie.Locations
                                .Select(s => new DirectoryInfo(s))
                                .Where(info => info.Exists)
                                .SelectMany(d => d.GetFiles())
                                .Where(f => f.IsMovieFile())
                                .Distinct()
                                .ToList();

            if (files.Count > 1)
            {
                DuplicateMovie duplicateMovie = new() { Movie = movie, Files = files };
                dupMovies.Add(duplicateMovie);

                duplicateMovie.IsSample = files.Any(f => f.IsSampleFile());
                duplicateMovie.IsDeleted = files.Any(f => f.IsDeletedStubFile());

                if (files.Count == 2)
                {
                    duplicateMovie.IsDoublePart = FileHelper.IsDoublePartMovie(files[0], files[1]);
                }
            }
        }

        private void BwScan_ProgressChanged(object sender, [NotNull] ProgressChangedEventArgs e)
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

        private void olvDuplicates_CellRightClick(object sender, [NotNull] BrightIdeasSoftware.CellRightClickEventArgs e)
        {
            if (e.Model is null)
            {
                return;
            }

            DuplicateMovie mlastSelected = (DuplicateMovie)e.Model;
            MovieConfiguration si = mlastSelected.Movie;

            possibleMergedEpisodeRightClickMenu.Items.Clear();

            AddRcMenuItem("Force Refresh", (_, _) =>
            {
                mainUi.ForceMovieRefresh(new List<MovieConfiguration> { si }, false);
                Update(mlastSelected);
            });
            AddRcMenuItem("Update", (_, _) =>
            {
                Update(mlastSelected);
            });
            AddRcMenuItem("Edit Movie", (_, _) =>
            {
                mainUi.EditMovie(si);
                Update(mlastSelected);
            });
            AddRcMenuItem("Choose Best", (_, _) => MergeItems(mlastSelected, mainUi));

            possibleMergedEpisodeRightClickMenu.Items.Add(new ToolStripSeparator());

            foreach (FileInfo? f in mlastSelected.Files)
            {
                AddRcMenuItem("Visit " + f.FullName, (_, _) =>
                {
                    Helpers.OpenFolderSelectFile(f.FullName);
                    Update(mlastSelected);
                });
            }
        }

        private void Update([NotNull] DuplicateMovie duplicate)
        {
            if (dupMovies.Contains(duplicate))
            {
                dupMovies.Remove(duplicate);
            }
            ProcessMovie(duplicate.Movie);
            UpdateUI();
        }

        private void MergeItems([NotNull] DuplicateMovie mlastSelected, UI ui)
        {
            foreach (FileInfo file1 in mlastSelected.Files)
            {
                foreach (FileInfo file2 in mlastSelected.Files)
                {
                    if (string.CompareOrdinal(file1.FullName, file2.FullName) > 0)
                    {
                        MergeConfigurationAndFiles(mlastSelected.Movie, file1, file2, ui);
                    }
                }
            }
            Update(mlastSelected);
        }

        private void MergeConfigurationAndFiles(MovieConfiguration mlastSelectedMovie, FileInfo file1, [NotNull] FileInfo file2, UI ui)
        {
            FileHelper.VideoComparison result = FileHelper.BetterQualityFile(file1, file2);

            FileHelper.VideoComparison newResult = result;

            switch (newResult)
            {
                case FileHelper.VideoComparison.secondFileBetter:
                    //remove first file and combine locations
                    UpgradeFile("System had identified to", file2, mlastSelectedMovie, file1);
                    break;

                case FileHelper.VideoComparison.cantTell:
                case FileHelper.VideoComparison.similar:
                    {
                        AskUserAboutFileReplacement(file1, file2, mlastSelectedMovie, ui);
                        return;
                    }
                //the other cases of the files being the same or the existing file being better are not enough to save the file
                case FileHelper.VideoComparison.firstFileBetter:
                case FileHelper.VideoComparison.same:
                    //remove second file and combine locations
                    UpgradeFile("System had identified to", file1, mlastSelectedMovie, file2);
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void AskUserAboutFileReplacement(FileInfo file1, FileInfo file2, [NotNull] MovieConfiguration pep, IDialogParent owner)
        {
            try
            {
                ChooseFile question = new(file1, file2);

                owner.ShowChildDialog(question);
                ChooseFile.ChooseFileDialogResult result = question.Answer;
                question.Dispose();

                switch (result)
                {
                    case ChooseFile.ChooseFileDialogResult.ignore:
                        Logger.Info($" User has selected keeping {file1.FullName} and {file2.FullName} and they will not be merged");
                        return;

                    case ChooseFile.ChooseFileDialogResult.left:
                        UpgradeFile("User selected to", file1, pep, file2);
                        return;

                    case ChooseFile.ChooseFileDialogResult.right:
                        UpgradeFile("User selected to", file2, pep, file1);
                        return;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }

        private static void UpgradeFile(string message, [NotNull] FileInfo keepFile, MovieConfiguration movie, [NotNull] FileInfo removeFile)
        {
            Logger.Info($"{message} remove {removeFile.FullName} as it is not as good quality than {keepFile.FullName}");
            try
            {
                if (movie.ManualLocations.Contains(removeFile.DirectoryName))
                {
                    movie.ManualLocations.Remove(removeFile.DirectoryName);
                }

                removeFile.Delete();

                if (removeFile.Directory.GetDirectories().Length > 0)
                {
                    return;
                }

                if (removeFile.Directory.GetFiles().Any(f => f.IsMovieFile()))
                {
                    return;
                }

                FileHelper.DoTidyUp(removeFile.Directory, TVSettings.Instance.Tidyup);
            }
            catch (System.IO.FileNotFoundException)
            { //ignored}
            }
            catch (System.IO.DirectoryNotFoundException)
            { //ignored}
            }
            catch (UnauthorizedAccessException)
            { //ignored}
            }
            catch (System.IO.IOException)
            { //ignored}
            }
        }
    }
}
