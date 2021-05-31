using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
    public partial class AutoAddMedia : Form
    {
        private readonly CombinedCodeFinder tvCodeFinder;
        private readonly CombinedCodeFinder movieCodeFinder;
        private readonly string originalHint;
        public readonly bool SingleTvShowFound;
        public readonly bool SingleMovieFound;
        private readonly bool assumeMovie;

        public AutoAddMedia(string hint, FileInfo file, bool assumeMovie)
        {
            InitializeComponent();
            ShowConfiguration = new ShowConfiguration();
            MovieConfiguration = new MovieConfiguration();

            this.assumeMovie = assumeMovie;

            lblFileName.Text = "Filename: " + file.FullName;

            tvCodeFinder = new CombinedCodeFinder("", MediaConfiguration.MediaType.tv, TVDoc.ProviderType.libraryDefault) { Dock = DockStyle.Fill };
            movieCodeFinder = new CombinedCodeFinder("", MediaConfiguration.MediaType.movie, TVDoc.ProviderType.libraryDefault) { Dock = DockStyle.Fill };

            tvCodeFinder.SelectionChanged += MTCCF_SelectionChanged;
            movieCodeFinder.SelectionChanged += MTCCF_SelectionChanged;

            SingleTvShowFound = tvCodeFinder.SetHint(hint, TVSettings.Instance.DefaultProvider) && TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation;
            SingleMovieFound = movieCodeFinder.SetHint(hint, TVSettings.Instance.DefaultMovieProvider) && TVSettings.Instance.DefMovieDefaultLocation.HasValue() && TVSettings.Instance.DefMovieUseDefaultLocation && assumeMovie;

            originalHint = hint;

            if (SingleTvShowFound && tvCodeFinder.TvShowInitialFoundCode.HasValue && tvCodeFinder.TvShowInitialFound != null)
            {
                string filenameFriendly = TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(tvCodeFinder.TvShowInitialFound.Name));
                SetShowItem(tvCodeFinder.TvShowInitialFoundCode.Value, tvCodeFinder.Source, TVSettings.Instance.DefShowLocation + System.IO.Path.DirectorySeparatorChar + filenameFriendly);
                if (ShowConfiguration.Code == -1)
                {
                    SetShowItem();
                }
            }
            if (SingleMovieFound && movieCodeFinder.MovieInitialFoundCode.HasValue)
            {
                SetMovieItem(movieCodeFinder.MovieInitialFoundCode.Value, movieCodeFinder.Source, TVSettings.Instance.DefMovieDefaultLocation ?? string.Empty);
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

            UpdateDirectoryDropDown(cbDirectory, TVSettings.Instance.LibraryFolders, TVSettings.Instance.DefShowLocation, TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation, tpTV);
            UpdateDirectoryDropDown(cbMovieDirectory, TVSettings.Instance.MovieLibraryFolders, TVSettings.Instance.DefMovieDefaultLocation, true, tpMovie);
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

            SetShowItem(code, tvCodeFinder.Source, cbDirectory.Text + lblDirectoryName.Text);
        }

        private void SetShowItem(int code, TVDoc.ProviderType type, string folderbase)
        {
            ShowConfiguration.SetId(type, code);
            ShowConfiguration.AutoAddFolderBase = folderbase;
            ShowConfiguration.ConfigurationProvider = type == TVSettings.Instance.DefaultProvider ? TVDoc.ProviderType.libraryDefault : type;

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

            SetMovieItem(code, movieCodeFinder.Source, cbMovieDirectory.Text);
        }

        private void SetMovieItem(int code, TVDoc.ProviderType type, string folderbase)
        {
            MovieConfiguration.SetId(type, code);
            MovieConfiguration.UseAutomaticFolders = true;
            MovieConfiguration.AutomaticFolderRoot = folderbase;
            MovieConfiguration.Format = MovieConfiguration.MovieFolderFormat.singleDirectorySingleFile;
            MovieConfiguration.UseCustomFolderNameFormat = false;
            MovieConfiguration.ConfigurationProvider = type == TVSettings.Instance.DefaultMovieProvider ? TVDoc.ProviderType.libraryDefault : type;

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
            if (tabControl1.SelectedTab == tpTV && TVDoc.GetMediaCache(tvCodeFinder.Source).HasSeries(tvCodeFinder.SelectedCode()))
            {
                return true;
            }
            if (tabControl1.SelectedTab == tpMovie && TVDoc.GetMediaCache(movieCodeFinder.Source).HasMovie(movieCodeFinder.SelectedCode()))
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

        private void rdoMovieProvider_CheckedChanged(object sender, EventArgs e)
        {
            movieCodeFinder.SetSource(GetMovieProviderType());
        }

        private void rdoTVProvider_CheckedChanged(object sender, EventArgs e)
        {
            tvCodeFinder.SetSource(GetTvProviderType());
        }

        private TVDoc.ProviderType GetMovieProviderType()
        {
            if (rdoMovieLibraryDefault.Checked)
            {
                return TVDoc.ProviderType.libraryDefault;
            }
            if (rdoMovieTVDB.Checked)
            {
                return TVDoc.ProviderType.TheTVDB;
            }
            if (rdoMovieTMDB.Checked)
            {
                return TVDoc.ProviderType.TMDB;
            }
            return TVDoc.ProviderType.libraryDefault;
        }

        private TVDoc.ProviderType GetTvProviderType()
        {
            if (rdoTVTVMaze.Checked)
            {
                return TVDoc.ProviderType.TVmaze;
            }
            if (rdoTVDefault.Checked)
            {
                return TVDoc.ProviderType.libraryDefault;
            }
            if (rdoTVTVDB.Checked)
            {
                return TVDoc.ProviderType.TheTVDB;
            }
            if (rdoTVTMDB.Checked)
            {
                return TVDoc.ProviderType.TMDB;
            }
            return TVDoc.ProviderType.libraryDefault;
        }
    }
}
