using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public partial class AutoAddShow : Form
    {
        private readonly CombinedCodeFinder tvCodeFinder;
        private readonly CombinedCodeFinder movieCodeFinder;
        private readonly string originalHint;
        public readonly bool SingleTvShowFound;
        public readonly bool SingleMovieFound;
        private readonly bool assumeMovie;

        public AutoAddShow(string hint,FileInfo file)
        {
            InitializeComponent();
            ShowConfiguration = new ShowConfiguration();
            MovieConfiguration = new MovieConfiguration();
            assumeMovie = FinderHelper.IgnoreHint(hint) || !file.FileNameNoExt().ContainsAnyCharactersFrom("0123456789");

            lblFileName.Text = "Filename: "+file.FullName;

            tvCodeFinder = new CombinedCodeFinder("",MediaConfiguration.MediaType.tv,TVDoc.ProviderType.TheTVDB) {Dock = DockStyle.Fill};
            movieCodeFinder = new CombinedCodeFinder("",MediaConfiguration.MediaType.movie,TVDoc.ProviderType.TMDB) { Dock = DockStyle.Fill };

            tvCodeFinder.SelectionChanged += MTCCF_SelectionChanged;
            movieCodeFinder.SelectionChanged += MTCCF_SelectionChanged;

            SingleTvShowFound = tvCodeFinder.SetHint(hint) && TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation;
            SingleMovieFound = movieCodeFinder.SetHint(hint) && TVSettings.Instance.DefMovieDefaultLocation.HasValue() && TVSettings.Instance.DefMovieUseDefaultLocation && assumeMovie;

            originalHint = hint;

            if (SingleTvShowFound )
            {
                string filenameFriendly = TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(tvCodeFinder.tvShowInitialFound.Name));
                SetShowItem(tvCodeFinder.tvShowInitialFound.TvdbCode, TVSettings.Instance.DefShowLocation+ System.IO.Path.DirectorySeparatorChar + filenameFriendly);
                if (ShowConfiguration.Code == -1)
                {
                    SetShowItem();
                }
            }
            if (SingleMovieFound)
            {
                SetMovieItem(movieCodeFinder.movieInitialFound.TmdbCode, TVSettings.Instance.DefMovieDefaultLocation);
                if (MovieConfiguration.Code == -1)
                {
                    SetMovieItem();
                }
            }

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(tvCodeFinder);
            pnlCF.ResumeLayout();

            panel1.SuspendLayout();
            panel1.Controls.Add(movieCodeFinder);
            panel1.ResumeLayout();

            UpdateDirectoryDropDown(cbDirectory, TVSettings.Instance.LibraryFolders, TVSettings.Instance.DefShowLocation, TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation,tpTV);
            UpdateDirectoryDropDown(cbMovieDirectory, TVSettings.Instance.MovieLibraryFolders, TVSettings.Instance.DefMovieDefaultLocation, true,tpMovie);
        }

        private static void UpdateDirectoryDropDown(ComboBox comboBox, List<string> folders, string? defaultValue, bool useDefaultValue, TabPage tabToDisable)
        {
            comboBox.SuspendLayout();
            comboBox.Items.Clear();
            if (folders.Count == 0)
            {
                tabToDisable.Enabled = false;
            }
            else
            {
                foreach (string folder in folders)
                {
                    comboBox.Items.Add(folder.TrimEnd(Path.DirectorySeparatorChar.ToString()));
                }

                if (useDefaultValue && defaultValue.HasValue())
                {
                    comboBox.Text = defaultValue!.TrimEnd(Path.DirectorySeparatorChar.ToString());
                }
                else
                {
                    comboBox.SelectedIndex = 0;
                }
            }

            comboBox.ResumeLayout();
        }

        private void MTCCF_SelectionChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tpTV)
            {
                string filenameFriendly = TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(tvCodeFinder.SelectedShow()?.Name));
                lblDirectoryName.Text = System.IO.Path.DirectorySeparatorChar + filenameFriendly;
            }
        }

        public ShowConfiguration ShowConfiguration { get; }
        public MovieConfiguration MovieConfiguration { get; }

        private void SetShowItem()
        {
            int code = tvCodeFinder.SelectedCode();

            SetShowItem(code, cbDirectory.Text + lblDirectoryName.Text);
        }

        private void SetShowItem(int code,string folderbase)
        {
            ShowConfiguration.TvdbCode = code;
            ShowConfiguration.AutoAddFolderBase = folderbase;

            //Set Default Timezone and if not then set on Network
            ShowConfiguration.ShowTimeZone = TVSettings.Instance.DefaultShowTimezoneName ?? TimeZoneHelper.TimeZoneForNetwork(tvCodeFinder.SelectedShow()?.Network, ShowConfiguration.ShowTimeZone);

            if (!originalHint.Contains(tvCodeFinder.SelectedShow()?.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                ShowConfiguration.AliasNames.Add(originalHint);
            }
        }

        private void SetMovieItem()
        {
            int code = movieCodeFinder.SelectedCode();

            SetMovieItem(code, cbMovieDirectory.Text);
        }

        private void SetMovieItem(int code, string folderbase)
        {
            MovieConfiguration.TmdbCode = code;
            MovieConfiguration.UseAutomaticFolders = true;
            MovieConfiguration.AutomaticFolderRoot = folderbase;
            MovieConfiguration.Format = MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile;
            MovieConfiguration.UseCustomFolderNameFormat = false;
            MovieConfiguration.ConfigurationProvider = TVDoc.ProviderType.TMDB;

            if (!originalHint.Contains(movieCodeFinder.SelectedMovie()?.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                MovieConfiguration.AliasNames.Add(originalHint);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!OkToClose())
            {
                DialogResult = DialogResult.None;
                return;
            }

            if (tabControl1.SelectedTab == tpTV)
            {
                SetShowItem();
            }
            if (tabControl1.SelectedTab == tpMovie)
            {
                SetMovieItem();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool OkToClose()
        {
            if (tabControl1.SelectedTab == tpTV  &&  TheTVDB.LocalCache.Instance.HasSeries(tvCodeFinder.SelectedCode())) //todo Get add show to work with TVMAZE
            {
                return true;
            }
            if (tabControl1.SelectedTab==tpMovie && TMDB.LocalCache.Instance.HasMovie(movieCodeFinder.SelectedCode())) //todo Get add show to work with TVMAZE
            {
                return true;
            }

            DialogResult dr = MessageBox.Show("Code unknown, close anyway?", "TVRename Auto Add Media",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return dr != DialogResult.No;
        }

        private void btnSkipAutoAdd_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void btnIgnoreFile_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            Close();
        }

        private void AutoAddShow_Load(object sender, EventArgs e)
        {
            (assumeMovie ? tpMovie : tpTV).Visible = true;
            ActiveControl = assumeMovie ? movieCodeFinder : tvCodeFinder; // set initial focus to the code entry/show finder control
            tabControl1.SelectedTab = (assumeMovie ? tpMovie : tpTV);
        }
    }
}

