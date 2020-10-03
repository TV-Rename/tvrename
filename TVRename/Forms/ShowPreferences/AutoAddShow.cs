using System;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;

namespace TVRename
{
    public partial class AutoAddShow : Form
    {
        private readonly TheTvdbCodeFinder codeFinder;
        private readonly string originalHint;

        public AutoAddShow(string hint,string filename)
        {
            InitializeComponent();
            ShowConfiguration = new ShowConfiguration();
            lblFileName.Text = "Filename: "+filename;
            codeFinder = new TheTvdbCodeFinder("") {Dock = DockStyle.Fill};
            codeFinder.SetHint(hint);
            codeFinder.SelectionChanged += MTCCF_SelectionChanged;
            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(codeFinder);
            pnlCF.ResumeLayout();
            ActiveControl = codeFinder; // set initial focus to the code entry/show finder control

            cbDirectory.SuspendLayout();
            cbDirectory.Items.Clear();
            foreach (string folder in TVSettings.Instance.LibraryFolders)
            {
                cbDirectory.Items.Add(folder.TrimEnd(Path.DirectorySeparatorChar.ToString()));
            }

            if (TVSettings.Instance.DefShowAutoFolders && TVSettings.Instance.DefShowUseDefLocation)
            {
                cbDirectory.Text = TVSettings.Instance.DefShowLocation.TrimEnd(Path.DirectorySeparatorChar.ToString());
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
            lblDirectoryName.Text = System.IO.Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(codeFinder.SelectedShow()?.Name ));
        }

        public ShowConfiguration ShowConfiguration { get; }

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

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!OkToClose())
            {
                DialogResult = DialogResult.None;
                return;
            }

            SetShowItem();
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool OkToClose()
        {
            if (TheTVDB.LocalCache.Instance.HasSeries(codeFinder.SelectedCode())) //todo Get add show to work with TVMAZE
            {
                return true;
            }

            DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Auto Add Show",
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
