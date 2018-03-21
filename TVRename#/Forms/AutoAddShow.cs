using System;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class AutoAddShow : Form
    {
        private readonly ShowItem mSI;
        private readonly TheTVDBCodeFinder mTCCF;

        public AutoAddShow(string hint)
        {
            InitializeComponent();
            this.mSI = new ShowItem();
            this.mTCCF = new TheTVDBCodeFinder("") {Dock = DockStyle.Fill};
            this.mTCCF.SetHint(hint);
            this.mTCCF.SelectionChanged += MTCCF_SelectionChanged;
            this.pnlCF.SuspendLayout();
            this.pnlCF.Controls.Add(this.mTCCF);
            this.pnlCF.ResumeLayout();
            this.ActiveControl = this.mTCCF; // set initial focus to the code entry/show finder control

            this.cbDirectory.SuspendLayout();
            this.cbDirectory.Items.Clear();
            this.cbDirectory.Items.AddRange(TVSettings.Instance.MonitorFoldersNames.ToArray());
            this.cbDirectory.SelectedIndex = 0;
            this.cbDirectory.ResumeLayout();
        }

        private void MTCCF_SelectionChanged(object sender, EventArgs e)
        {
            this.lblDirectoryName.Text = System.IO.Path.DirectorySeparatorChar + TVSettings.Instance.FilenameFriendly(FileHelper.MakeValidPath(this.mTCCF.SelectedShowName( )));
        }

        public ShowItem ShowItem => this.mSI;

        private void SetShowItem()
        {
            int code = this.mTCCF.SelectedCode();


            this.mSI.TVDBCode = code;
            this.mSI.AutoAdd_FolderBase = this.cbDirectory.Text+this.lblDirectoryName.Text;
            this.mSI.PadSeasonToTwoDigits = true;
            //Set Default Timezone based on Network??
            //this.mSI.ShowTimeZone
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!OkToClose())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            SetShowItem();
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private bool OkToClose()
        {
            if (TheTVDB.Instance.HasSeries(this.mTCCF.SelectedCode())) return true;

            DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Add/Edit Show",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            return dr != DialogResult.No;
        }

        private void AutoAddShow_Load(object sender, EventArgs e)
        {

        }
    }
}
