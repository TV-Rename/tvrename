using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public partial class AutoAddShow : Form
    {
        private readonly CodeFinder codeFinder;
        private readonly string originalHint;
        private readonly MediaConfiguration.MediaType mediaType;

        public AutoAddShow(string hint,string filename,List<string> directories, MediaConfiguration.MediaType type)
        {
            InitializeComponent();
            ShowConfiguration = new ShowConfiguration();
            MovieConfiguration = new MovieConfiguration();
            mediaType = type;

            lblFileName.Text = "Filename: "+filename;

            codeFinder = (type==MediaConfiguration.MediaType.tv)
                ? (CodeFinder) new TheTvdbCodeFinder("") {Dock = DockStyle.Fill}
                : new TmdbCodeFinder("") { Dock = DockStyle.Fill };

            codeFinder.SetHint(hint);
            codeFinder.SelectionChanged += MTCCF_SelectionChanged;
            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(codeFinder);
            pnlCF.ResumeLayout();
            ActiveControl = codeFinder; // set initial focus to the code entry/show finder control

            cbDirectory.SuspendLayout();
            cbDirectory.Items.Clear();
            foreach (string folder in directories)
            {
                cbDirectory.Items.Add(folder.TrimEnd(Path.DirectorySeparatorChar.ToString()));
            }

            if (TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation)
            {
                cbDirectory.Text = TVSettings.Instance.DefShowLocation.TrimEnd(Path.DirectorySeparatorChar.ToString()); //todo use another DefMovieLocation
            }
            else
            {
                cbDirectory.SelectedIndex = 0;
            }
            
            cbDirectory.ResumeLayout();

            originalHint = hint;
        }

        private void MTCCF_SelectionChanged(object sender, EventArgs e)
        {
            string mediaName = mediaType == MediaConfiguration.MediaType.tv ?  codeFinder.SelectedShow()?.Name : codeFinder.SelectedMovie()?.Name;

            string filenameFriendly = TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(mediaName));
            lblDirectoryName.Text = System.IO.Path.DirectorySeparatorChar + filenameFriendly;
        }

        public ShowConfiguration ShowConfiguration { get; }
        public MovieConfiguration MovieConfiguration { get; }

        private void SetShowItem()
        {
            int code = codeFinder.SelectedCode();

            ShowConfiguration.TvdbCode = code;
            ShowConfiguration.AutoAddFolderBase = cbDirectory.Text+lblDirectoryName.Text;

            //Set Default Timezone and if not then set on Network
            ShowConfiguration.ShowTimeZone = TVSettings.Instance.DefaultShowTimezoneName ?? TimeZoneHelper.TimeZoneForNetwork(codeFinder.SelectedShow()?.Network, ShowConfiguration.ShowTimeZone);

            if (!originalHint.Contains(codeFinder.SelectedShow()?.Name??string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                ShowConfiguration.AliasNames.Add(originalHint);
            }
        }

        private void SetMovieItem()
        {
            int code = codeFinder.SelectedCode();

            MovieConfiguration.TvdbCode = code;
            // Sort MovieConfiguration.Locations akin to Bulk add using cbDirectory.Text+lblDirectoryName.Text;

            if (!originalHint.Contains(codeFinder.SelectedMovie()?.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase))
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

            switch (mediaType)
            {
                case MediaConfiguration.MediaType.tv:
                    SetShowItem();
                    break;
                case MediaConfiguration.MediaType.movie:
                    SetMovieItem();
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool OkToClose()
        {
            if (mediaType == MediaConfiguration.MediaType.tv &&  TheTVDB.LocalCache.Instance.HasSeries(codeFinder.SelectedCode())) //todo Get add show to work with TVMAZE
            {
                return true;
            }
            if (mediaType == MediaConfiguration.MediaType.movie && TMDB.LocalCache.Instance.HasMovie(codeFinder.SelectedCode())) //todo Get add show to work with TVMAZE
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
    }
}
