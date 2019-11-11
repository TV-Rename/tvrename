using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;
using DirectoryInfo = Alphaleonis.Win32.Filesystem.DirectoryInfo;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace TVRename.Forms.Tools
{
    public partial class QuickRename : Form
    {
        private readonly TVDoc mDoc;
        private readonly UI parent;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public QuickRename([NotNull] TVDoc tvDoc, UI ui)
        {
            mDoc=tvDoc;
            parent = ui;
            InitializeComponent();

            mDoc.TheActionList.Clear();
            parent.FillActionList(false);

            txtFileFormatPreview.Text = TVSettings.Instance.NamingStyle.StyleString;
            txtFileFormatPreview.Enabled = false;

            cbShowList.Items.Add("<Auto>");

            foreach (ShowItem si in tvDoc.Library.Shows.ToArray().OrderBy(item => item.ShowName))
            {
                cbShowList.Items.Add(si.ShowName);
            }

            cbShowList.SelectedItem = "<Auto>";
        }

        private void Panel1_DragDrop(object sender, [NotNull] DragEventArgs e)
        {
            Logger.Info("Starting quick rename.");
            // Get a list of filenames being dragged
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (FileInfo droppedFile in files.Select(droppedFileName => new FileInfo(droppedFileName)))
            {
                ProcessUnknown(droppedFile);
            }

            parent.FillActionList(true);
            parent.FocusOnScanResults();
            
            Logger.Info("Finished quick rename.");
        }

        private void ProcessDirectory([NotNull] DirectoryInfo droppedDir)
        {
            if ((droppedDir.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                Logger.Error($"{droppedDir.FullName} is not a directory, CONTACT DEV TEAM.");
                return;
            }

            foreach (FileInfo subFile in droppedDir.GetFiles())
            {
                ProcessUnknown(subFile);
            }

            foreach (DirectoryInfo subFile in droppedDir.GetDirectories())
            {
                ProcessDirectory(subFile);
            }
        }

        private void ProcessUnknown([NotNull] FileInfo droppedFile)
        {
            if ((droppedFile.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                ProcessDirectory(new DirectoryInfo(droppedFile.FullName));
            }
            else
            {
                ProcessFile(droppedFile);
            }
        }

        private void ProcessFile([NotNull] FileInfo droppedFile)
        {
            if ((droppedFile.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Logger.Error($"{droppedFile.FullName} is a directory, ignoring.");
                return;
            }

            if (!droppedFile.IsMovieFile())
            {
                Logger.Info($"{droppedFile.FullName} is not a movie file, ignoring.");
                return;
            }

            ShowItem bestShow = (string)cbShowList.SelectedItem == "<Auto>"
                ? FinderHelper.FindBestMatchingShow(droppedFile, mDoc.Library.Shows)
                : mDoc.Library.Shows.FirstOrDefault(item => item.ShowName == (string)cbShowList.SelectedItem);

            if (bestShow is null)
            {
                if (TVSettings.Instance.AutoAddAsPartOfQuickRename)
                {
                    List<ShowItem> addedShows = FinderHelper.FindShows(new List<string> {droppedFile.Name}, mDoc);
                    bestShow = addedShows.FirstOrDefault();
                }

                if (bestShow is null)
                {
                    Logger.Info($"Cannot find show for {droppedFile.FullName}, ignoring this file.");
                    return;
                }
            }

            if (!FinderHelper.FindSeasEp(droppedFile, out int seasonNum, out int episodeNum, out int _, bestShow,
                out TVSettings.FilenameProcessorRE _))
            {
                Logger.Info($"Cannot find episode for {bestShow.ShowName} for {droppedFile.FullName}, ignoring this file.");
                return;
            }

            SeriesInfo s = bestShow.TheSeries();
            if (s is null)
            {
                //We have not downloaded the series, so have to assume that we need the episode/file
                Logger.Info(
                    $"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it has not been downloaded yet, ignoring this file.");

                return;
            }

            try
            {
                Episode ep = s.GetEpisode(seasonNum, episodeNum, bestShow.DvdOrder);
                ProcessedEpisode episode = new ProcessedEpisode(ep, bestShow);

                string filename = TVSettings.Instance.FilenameFriendly(
                    TVSettings.Instance.NamingStyle.NameFor(episode, droppedFile.Extension,
                        droppedFile.DirectoryName.Length));

                FileInfo targetFile =
                    new FileInfo(droppedFile.DirectoryName + Path.DirectorySeparatorChar + filename);

                if (droppedFile.FullName == targetFile.FullName)
                {
                    Logger.Info(
                        $"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it already has the appropriate name.");

                    return;
                }

                mDoc.TheActionList.Add(new ActionCopyMoveRename(droppedFile, targetFile, episode,mDoc));

                // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                mDoc.TheActionList.AddRange(new DownloadIdentifiersController().ProcessEpisode(episode, targetFile));

                //If keep together is active then we may want to copy over related files too
                if (TVSettings.Instance.KeepTogether)
                {
                    FileFinder.KeepTogether(mDoc.TheActionList, false, true,mDoc);
                }
            }
            catch (SeriesInfo.EpisodeNotFoundException)
            {
                Logger.Info(
                    $"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it does not have Episode {episodeNum} for Season {seasonNum}.");
            }
        }

        private void Panel1_DragEnter(object sender, [NotNull] DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void BnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
