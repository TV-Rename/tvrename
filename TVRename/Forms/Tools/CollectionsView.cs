using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename.Forms
{
    public partial class CollectionsView : Form
    {
        private readonly List<CollectionMember> collectionMovies;
        private  List<CollectionMember> incompleteCollectionMovies;
        private readonly TVDoc mDoc;
        private readonly UI mainUi;

        public CollectionsView([NotNull] TVDoc doc, UI main)
        {
            InitializeComponent();
            collectionMovies = new List<CollectionMember>();
            incompleteCollectionMovies = new List<CollectionMember>();
            mDoc = doc;
            mainUi = main;
            Scan();
        }

        // ReSharper disable once InconsistentNaming
        private void UpdateUI()
        {
            olvCollections.SetObjects(chkRemoveCompleted.Checked ? incompleteCollectionMovies : collectionMovies);
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
            var bw = (BackgroundWorker) sender;

            List<(int, string)> collectionIds = mDoc.FilmLibrary.Values
                .Select(c => (c.CachedMovie?.CollectionId, c.CachedMovie?.CollectionName))
                .Where((a) => a.CollectionId.HasValue && a.CollectionName.HasValue())
                .Select(a => (a.CollectionId.Value, a.CollectionName)).Distinct().ToList();

            int total = collectionIds.Count;
            int current = 0;

            collectionMovies.Clear();
            foreach ((int, string) collection in collectionIds)
            {
                var shows = TMDB.LocalCache.Instance.GetMovieIdsFromCollection(collection.Item1);
                foreach (KeyValuePair<int, CachedMovieInfo> neededShow in shows)
                {
                    var c = new CollectionMember {CollectionName = collection.Item2, Movie = neededShow.Value};

                    c.IsInLibrary = mDoc.FilmLibrary.ContainsKey(c.TmdbCode);
                    collectionMovies.Add(c);
                }

                bw.ReportProgress(100 * current++ / total, collection.Item2);
            }

            var incompleteCollections = collectionMovies.GroupBy(member => member.CollectionName)
                .Where(members => members.Any(x => !x.IsInLibrary)).Select(members => members.Key).ToList();

            incompleteCollectionMovies =
                collectionMovies.Where(member => incompleteCollections.Contains(member.CollectionName)).ToList();
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
            if (olvCollections.IsDisposed)
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

            CollectionMember mlastSelected = (CollectionMember) e.Model;

            possibleMergedEpisodeRightClickMenu.Items.Clear();

            
            if (mlastSelected.IsInLibrary)
            {
                MovieConfiguration? si = mDoc.FilmLibrary.GetMovie(mlastSelected.TmdbCode);
                if ( si!=null)
                {
                    AddRcMenuItem("Force Refresh", (o, args) => mainUi.ForceMovieRefresh(si, false));
                    AddRcMenuItem("Edit Movie", (o, args) => mainUi.EditMovie(si));
                }
            }
            else
            {
                AddRcMenuItem("Add to Library...", (o, args) => AddToLibrary(mlastSelected.Movie));
            }

            possibleMergedEpisodeRightClickMenu.Items.Add(new ToolStripSeparator());
        }

        private void AddToLibrary(CachedMovieInfo si)
        {

                // need to add a new showitem
                var found = new MovieConfiguration(si.TmdbCode,TVDoc.ProviderType.TMDB);
                ///TODO put UI to get folder
            mDoc.Add(found);

                

            mDoc.SetDirty();
        mDoc.ExportMovieInfo();
        
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }
    }

    internal class CollectionMember
    {
        public string CollectionName;
        public CachedMovieInfo Movie;

        public string MovieName =>Movie.Name;
        public int TmdbCode=>Movie.TmdbCode;

        public bool IsInLibrary;
        public int? MovieYear => Movie.Year;
    }
}

