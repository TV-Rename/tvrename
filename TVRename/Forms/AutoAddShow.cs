using System;
using System.Windows.Forms;

namespace TVRename
{
    public partial class AutoAddShow : Form
    {
        private readonly ShowItem mSI;
        private readonly TheTVDBCodeFinder mTCCF;
        private readonly string originalHint;

        public AutoAddShow(string hint)
        {
            InitializeComponent();
            mSI = new ShowItem();
            mTCCF = new TheTVDBCodeFinder("") {Dock = DockStyle.Fill};
            mTCCF.SetHint(hint);
            mTCCF.SelectionChanged += MTCCF_SelectionChanged;
            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(mTCCF);
            pnlCF.ResumeLayout();
            ActiveControl = mTCCF; // set initial focus to the code entry/show finder control

            cbDirectory.SuspendLayout();
            cbDirectory.Items.Clear();
            cbDirectory.Items.AddRange(TVSettings.Instance.LibraryFolders.ToArray());
            cbDirectory.SelectedIndex = 0;
            cbDirectory.ResumeLayout();

            originalHint = hint;
        }

        private void MTCCF_SelectionChanged(object sender, EventArgs e)
        {
            lblDirectoryName.Text = System.IO.Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(mTCCF.SelectedShow()?.Name ));
        }

        public ShowItem ShowItem => mSI;

        private void SetShowItem()
        {
            int code = mTCCF.SelectedCode();


            mSI.TVDBCode = code;
            mSI.AutoAdd_FolderBase = cbDirectory.Text+lblDirectoryName.Text;
            mSI.PadSeasonToTwoDigits = true;
            //Set Default Timezone based on Network
            mSI.ShowTimeZone = TimeZone.TimeZoneForNetwork(mTCCF.SelectedShow()?.getNetwork());
            if (!originalHint.Contains(mTCCF.SelectedShow().Name, StringComparison.OrdinalIgnoreCase)) mSI.AliasNames.Add(originalHint);

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
            if (TheTVDB.Instance.HasSeries(mTCCF.SelectedCode())) return true;

            DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Auto Add Show",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return dr != DialogResult.No;
        }

        private void AutoAddShow_Load(object sender, EventArgs e)
        {

        }
    }
}
