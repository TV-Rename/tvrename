using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.Forms.Tools
{
    public partial class QuickRename : Form, IDialogParent
    {
        private readonly TVDoc mDoc;
        private readonly UI parent;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public QuickRename([NotNull] TVDoc tvDoc, UI ui)
        {
            mDoc = tvDoc;
            parent = ui;
            InitializeComponent();

            mDoc.TheActionList.Clear();
            parent.FillActionList();

            txtFileFormatPreview.Text = TVSettings.Instance.NamingStyle.StyleString;
            txtFileFormatPreview.Enabled = false;

            cbShowList.Items.Add("<Auto>");

            foreach (ShowConfiguration si in tvDoc.TvLibrary.Shows.ToArray().OrderBy(item => item.ShowName))
            {
                cbShowList.Items.Add(si.ShowName);
            }

            cbShowList.SelectedItem = "<Auto>";
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

        private void Panel1_DragDrop(object sender, [NotNull] DragEventArgs e)
        {
            Logger.Info("Starting quick rename.");
            // Get a list of filenames being dragged
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (FileInfo droppedFile in files.Select(droppedFileName => new FileInfo(droppedFileName)))
            {
                ProcessUnknown(droppedFile, this);
            }

            parent.FillActionList();
            parent.FocusOnScanResults();

            Logger.Info("Finished quick rename.");
        }

        private void ProcessDirectory([NotNull] DirectoryInfo droppedDir)
        {
            if ((droppedDir.Attributes & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory)
            {
                Logger.Error($"{droppedDir.FullName} is not a directory, CONTACT DEV TEAM.");
                return;
            }

            foreach (FileInfo subFile in droppedDir.GetFiles())
            {
                ProcessUnknown(subFile, this);
            }

            foreach (DirectoryInfo subFile in droppedDir.GetDirectories())
            {
                ProcessDirectory(subFile);
            }
        }

        private void ProcessUnknown([NotNull] FileInfo droppedFile, IDialogParent owner)
        {
            if ((droppedFile.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
            {
                ProcessDirectory(new DirectoryInfo(droppedFile.FullName));
            }
            else
            {
                ProcessFile(droppedFile, owner);
            }
        }

        private void ProcessFile([NotNull] FileInfo droppedFile, IDialogParent owner)
        {
            if ((droppedFile.Attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
            {
                Logger.Error($"{droppedFile.FullName} is a directory, ignoring.");
                return;
            }

            if (!droppedFile.IsMovieFile())
            {
                Logger.Info($"{droppedFile.FullName} is not a movie file, ignoring.");
                return;
            }

            // Note that the extension of the file may not be fi.extension as users can put ".mkv.t" for example as an extension
            string otherExtension = TVSettings.Instance.FileHasUsefulExtensionDetails(droppedFile, true);

            ShowConfiguration? bestShow = (string)cbShowList.SelectedItem == "<Auto>"
                ? FinderHelper.FindBestMatchingShow(droppedFile.FullName, mDoc.TvLibrary.Shows)
                : mDoc.TvLibrary.Shows.FirstOrDefault(item => item.ShowName == (string)cbShowList.SelectedItem);

            if (bestShow is null)
            {
                if (TVSettings.Instance.AutoAddAsPartOfQuickRename)
                {
                    IEnumerable<MediaConfiguration> addedShows = FinderHelper.FindMedia(new List<FileInfo> { droppedFile }, mDoc, owner);
                    bestShow = addedShows.OfType<ShowConfiguration>().FirstOrDefault();

                    if (bestShow != null && !mDoc.AlreadyContains(bestShow))
                    {
                        mDoc.Add(bestShow.AsList(),true);
                        mDoc.TvAddedOrEdited(true, false, false, parent, bestShow);

                        Logger.Info($"Added new show called: {bestShow.ShowName}");
                    }
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

            try
            {
                ProcessedEpisode episode = bestShow.GetEpisode(seasonNum, episodeNum);

                string filename = TVSettings.Instance.FilenameFriendly(
                    TVSettings.Instance.NamingStyle.NameFor(episode, otherExtension,
                        droppedFile.DirectoryName.Length));

                FileInfo targetFile =
                    new(droppedFile.DirectoryName.EnsureEndsWithSeparator() + filename);

                if (droppedFile.FullName == targetFile.FullName)
                {
                    Logger.Info(
                        $"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it already has the appropriate name.");

                    return;
                }

                mDoc.TheActionList.Add(new ActionCopyMoveRename(droppedFile, targetFile, episode, mDoc));

                // if we're copying/moving a file across, we might also want to make a thumbnail or NFO for it
                mDoc.TheActionList.AddNullableRange(new DownloadIdentifiersController().ProcessEpisode(episode, targetFile));

                //If keep together is active then we may want to copy over related files too
                if (TVSettings.Instance.KeepTogether)
                {
                    FileFinder.KeepTogether(mDoc.TheActionList, false, true, mDoc);
                }
                if (TVSettings.Instance.CopySubsFolders)
                {
                    FileFinder.CopySubsFolders(mDoc.TheActionList, true, mDoc);
                }
            }
            catch (ShowConfiguration.EpisodeNotFoundException)
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
