using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVRename.Forms;

public partial class DuplicateMovieFinder : Form
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly List<DuplicateMovie> dupMovies;
    private readonly TVDoc mDoc;
    private readonly UI mainUi;

    private Task? scan;

    public DuplicateMovieFinder(TVDoc doc, UI main)
    {
        InitializeComponent();
        dupMovies = [];
        mDoc = doc;
        mainUi = main;
        StartScan();
    }

    private void StartScan()
    {
        var progressHandler = new Progress<ProgressReport>(scanReport =>
        {
            // This body executes safely on the main thread
            pbProgress.SetProgress(scanReport.ProgressPercentage);
            lblStatus.Text = scanReport.UpdateText.ToUiVersion();
        });

        scan = Scan(progressHandler);
    }

    private async Task Scan(IProgress<ProgressReport> reporter)
    {
        btnRefresh.Visible = false;
        pbProgress.Visible = true;
        lblStatus.Visible = true;

        int total = mDoc.FilmLibrary.Movies.Count();
        ThreadSafeCounter currentRecord = new();

        dupMovies.Clear();

        foreach (MovieConfiguration? movie in mDoc.FilmLibrary.Movies)
        {
            await ProcessMovieAsync(movie);

            reporter.Report(new ProgressReport()
            {
                ProgressPercentage = 100 * currentRecord.Increment() / total,
                UpdateText = movie.ShowName
            });
        }

        btnRefresh.Visible = true;
        pbProgress.Visible = false;
        lblStatus.Visible = false;
        if (olvDuplicates.IsDisposed)
        {
            return;
        }

        UpdateUI();
    }


    // ReSharper disable once InconsistentNaming
    private void UpdateUI()
    {
        olvDuplicates.SetObjects(dupMovies, true);
    }

    private void rightClickMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        rightClickMenu.Close();
    }

    private async Task ProcessMovieAsync(MovieConfiguration movie)
    {
        List<FileInfo> files = [.. (await movie.MovieFilesAsync()).Where(fiTemp => movie.NameMatch(fiTemp, false))];

        if (files.Count > 1)
        {
            DuplicateMovie duplicateMovie = new(movie, files);
            dupMovies.Add(duplicateMovie);

            duplicateMovie.IsSample = files.Any(f => f.IsSampleFile());
            duplicateMovie.IsDeleted = files.Any(f => f.IsDeletedStubFile());

            if (files.Count == 2)
            {
                duplicateMovie.IsDoublePart = FileHelper.IsDoublePartMovie(files[0], files[1]);
            }
        }
    }

    private void BtnRefresh_Click_1(object sender, EventArgs e)
    {

    }

    private async void olvDuplicates_CellRightClick(object sender, BrightIdeasSoftware.CellRightClickEventArgs e)
    {
        if (e.Model is null)
        {
            return;
        }

        DuplicateMovie mlastSelected = (DuplicateMovie)e.Model;
        MovieConfiguration si = mlastSelected.Movie;

        rightClickMenu.Items.Clear();

        rightClickMenu.Add("Force Refresh", async (_, _) =>
        {
            await mainUi.ForceMovieRefreshAsync([si], false);
            await UpdateAsync(mlastSelected);
        });
        rightClickMenu.Add("Update", async (_, _) =>
        {
            await UpdateAsync(mlastSelected);
        });
        rightClickMenu.Add("Edit Movie", async (_, _) =>
        {
            await mainUi.EditMovieAsync(si);
            await UpdateAsync(mlastSelected);
        });
        rightClickMenu.Add("Choose Best", async (_, _) => await MergeItemsAsync(mlastSelected, mainUi));

        rightClickMenu.AddSeparator();

        foreach (FileInfo? f in mlastSelected.Files)
        {
            rightClickMenu.Add("Visit " + f.FullName, async (_, _) =>
            {
                f.FullName.OpenFolderSelectFile();
                await UpdateAsync(mlastSelected);
            });
        }
    }

    private async Task UpdateAsync(DuplicateMovie duplicate)
    {
        dupMovies.Remove(duplicate);
        await ProcessMovieAsync(duplicate.Movie);
        UpdateUI();
    }

    private async Task MergeItemsAsync(DuplicateMovie mlastSelected, UI ui)
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
        await UpdateAsync(mlastSelected);
    }

    private static void MergeConfigurationAndFiles(MovieConfiguration mlastSelectedMovie, FileInfo file1, FileInfo file2, UI ui)
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
                throw new NotSupportedException($"MergeConfigurationAndFiles: BetterQualityFile returned invalid VideoComparison {newResult}");
        }
    }

    private static void AskUserAboutFileReplacement(FileInfo file1, FileInfo file2, MovieConfiguration pep, UI owner)
    {
        try
        {
            using ChooseFile question = new(file1, file2);

            owner.ShowChildDialog(question);
            ChooseFile.ChooseFileDialogResult result = question.Answer;

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
                    throw new NotSupportedException($"result = {result} is not supported by {System.Reflection.MethodBase.GetCurrentMethod()}");
            }
        }
        catch (System.IO.FileNotFoundException)
        {
        }
    }

    private static void UpgradeFile(string message, FileInfo keepFile, MovieConfiguration movie, FileInfo removeFile)
    {
        Logger.Info($"{message} remove {removeFile.FullName} as it is not as good quality than {keepFile.FullName}");
        try
        {
            if (movie.ManualLocations.Contains(removeFile.DirectoryName))
            {
                movie.ManualLocations.Remove(removeFile.DirectoryName);
            }

            removeFile.Delete(); //TODO use FileHelper

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
