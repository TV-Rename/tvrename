using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using JetBrains.Annotations;

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
                ShowItem bestShow;
                if (cbShowList.SelectedItem == "<Auto>")
                {
                    bestShow = FinderHelper.FindBestMatchingShow(droppedFile, mDoc.Library.Shows);
                }
                else
                {
                    bestShow = mDoc.Library.Shows.FirstOrDefault(item => item.ShowName == cbShowList.SelectedItem);
                }

                if (bestShow is null)
                {
                    if (true) //TODO replace with Settings
                    {
                        List<ShowItem> addedShows = FinderHelper.FindShows(new List<string> {droppedFile.Name}, mDoc);
                        bestShow = addedShows.FirstOrDefault();
                    }

                    if (bestShow is null)
                    {
                        Logger.Info($"Cannot find show for {droppedFile.FullName}, ignoring this file.");
                        continue;
                    }
                }

                if (!FinderHelper.FindSeasEp(droppedFile, out int seasonNum, out int episodeNum, out int _,bestShow,out TVSettings.FilenameProcessorRE _))
                {
                    Logger.Info($"Cannot find episode for {bestShow.ShowName} for {droppedFile.FullName}, ignoring this file.");
                    continue;
                }

                SeriesInfo s = bestShow.TheSeries();
                if (s is null)
                {
                    //We have not downloaded the series, so have to assume that we need the episode/file
                    Logger.Info($"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it has not been downloaded yet, ignoring this file.");
                    continue;
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
                        Logger.Info($"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it already has the appropriate name.");
                        continue;
                    }

                    mDoc.TheActionList.Add(new ActionCopyMoveRename(droppedFile, targetFile, episode));
                }
                catch (SeriesInfo.EpisodeNotFoundException)
                {
                    Logger.Info($"Can't rename file for {bestShow.ShowName} for {droppedFile.FullName}, as it does not have Episode {episodeNum} for Season {seasonNum}.");
                }
            }

            parent.FillActionList();
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
