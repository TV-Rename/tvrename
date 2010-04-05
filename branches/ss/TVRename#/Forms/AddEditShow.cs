//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System.Text.RegularExpressions;
using System.Windows.Forms;

//            this->txtCustomShowName->TextChanged += gcnew System::EventHandler(this, &AddEditShow::txtCustomShowName_TextChanged);

namespace TVRename
{

    /// <summary>
    /// Summary for AddEditShow
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class AddEditShow : Form
    {
        private ShowItem mSI;
        private TheTVDB mTVDB;
        private TheTVDBCodeFinder mTCCF;
        public string ShowTimeZone;

        public AddEditShow(ShowItem si, TheTVDB db, string timezone)
        {
            mSI = si;
            mTVDB = db;
            InitializeComponent();

            cbTimeZone.BeginUpdate();
            cbTimeZone.Items.Clear();

            foreach (string s in TimeZone.ZoneNames())
                cbTimeZone.Items.Add(s);

            cbTimeZone.EndUpdate();


            mTCCF = new TheTVDBCodeFinder(si.TVDBCode != -1 ? si.TVDBCode.ToString() : "", mTVDB);
            mTCCF.Dock = DockStyle.Fill;
            //mTCCF->SelectionChanged += gcnew System::EventHandler(this, &AddEditShow::lvMatches_ItemSelectionChanged);

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(mTCCF);
            pnlCF.ResumeLayout();

            chkCustomShowName.Checked = si.UseCustomShowName;
            if (chkCustomShowName.Checked)
                txtCustomShowName.Text = si.CustomShowName;
            chkCustomShowName_CheckedChanged(null, null);

            cbSequentialMatching.Checked = si.UseSequentialMatch;
            chkShowNextAirdate.Checked = si.ShowNextAirdate;
            chkSpecialsCount.Checked = si.CountSpecials;
            chkFolderPerSeason.Checked = si.AutoAdd_FolderPerSeason;
            txtSeasonFolderName.Text = si.AutoAdd_SeasonFolderName;
            txtBaseFolder.Text = si.AutoAdd_FolderBase;
            chkAutoFolders.Checked = si.AutoAddNewSeasons;
            chkAutoFolders_CheckedChanged(null, null);
            chkFolderPerSeason_CheckedChanged(null, null);

            chkThumbnailsAndStuff.Checked = false; // TODO
            cbDoRenaming.Checked = si.DoRename;
            cbDoMissingCheck.Checked = si.DoMissingCheck;
            cbDoMissingCheck_CheckedChanged(null, null);

            chkPadTwoDigits.Checked = si.PadSeasonToTwoDigits;
            SetRightNumberOfHashes();

            ShowTimeZone = (!string.IsNullOrEmpty(timezone)) ? timezone : TimeZone.DefaultTimeZone();
            cbTimeZone.Text = ShowTimeZone;
            chkDVDOrder.Checked = si.DVDOrder;
            chkForceCheckAll.Checked = si.ForceCheckAll;

            bool first = true;
            si.IgnoreSeasons.Sort();
            foreach (int i in si.IgnoreSeasons)
            {
                if (!first)
                    txtIgnoreSeasons.Text += " ";
                txtIgnoreSeasons.Text += i.ToString();
                first = false;
            }

            foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in si.ManualFolderLocations)
            {
                foreach (string s in kvp.Value)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = kvp.Key.ToString();
                    lvi.SubItems.Add(s);

                    lvSeasonFolders.Items.Add(lvi);
                }
            }
            lvSeasonFolders.Sort();

            txtSeasonNumber_TextChanged(null, null);
            txtFolder_TextChanged(null, null);

        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (!OKToClose())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            SetmSI();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }


        private bool OKToClose()
        {
            if (!mTVDB.HasSeries(mTCCF.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Add/Edit Show", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return false;
            }

            return true;
        }

        private void SetmSI()
        {
            int code = mTCCF.SelectedCode();

            string tz = (cbTimeZone.SelectedItem != null) ? cbTimeZone.SelectedItem.ToString() : "";

            mSI.CustomShowName = txtCustomShowName.Text;
            mSI.UseCustomShowName = chkCustomShowName.Checked;
            ShowTimeZone = tz; //TODO: move to somewhere else. make timezone manager for tvdb?
            mSI.ShowNextAirdate = chkShowNextAirdate.Checked;
            mSI.PadSeasonToTwoDigits = chkPadTwoDigits.Checked;
            mSI.TVDBCode = code;
            //todo mSI->SeasonNumber = seasnum;
            mSI.CountSpecials = chkSpecialsCount.Checked;
            //                                 mSI->Rules = mWorkingRuleSet;  // TODO
            mSI.DoRename = cbDoRenaming.Checked;
            mSI.DoMissingCheck = cbDoMissingCheck.Checked;

            mSI.AutoAddNewSeasons = chkAutoFolders.Checked;
            mSI.AutoAdd_FolderPerSeason = chkFolderPerSeason.Checked;
            mSI.AutoAdd_SeasonFolderName = txtSeasonFolderName.Text;
            mSI.AutoAdd_FolderBase = txtBaseFolder.Text;

            mSI.DVDOrder = chkDVDOrder.Checked;
            mSI.ForceCheckAll = chkForceCheckAll.Checked;
            mSI.UseSequentialMatch = cbSequentialMatching.Checked;

            string slist = txtIgnoreSeasons.Text;
            mSI.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
                mSI.IgnoreSeasons.Add(int.Parse(match.Value));

            mSI.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    if (!mSI.ManualFolderLocations.ContainsKey(seas))
                        mSI.ManualFolderLocations.Add(seas, new StringList());

                    mSI.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                }
            }
        }


        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        private void chkFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
        {
            txtSeasonFolderName.Enabled = chkFolderPerSeason.Checked;
        }
        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBaseFolder.Text))
                folderBrowser.SelectedPath = txtBaseFolder.Text;

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtBaseFolder.Text = folderBrowser.SelectedPath;
        }
        private void chkAutoFolders_CheckedChanged(object sender, System.EventArgs e)
        {
            gbAutoFolders.Enabled = chkAutoFolders.Checked;
        }
        private void cbDoMissingCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            chkForceCheckAll.Enabled = cbDoMissingCheck.Checked;
        }
        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            if (lvSeasonFolders.SelectedItems.Count > 0)
                foreach (ListViewItem lvi in lvSeasonFolders.SelectedItems)
                    lvSeasonFolders.Items.Remove(lvi);
        }
        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = txtSeasonNumber.Text;
            lvi.SubItems.Add(txtFolder.Text);

            lvSeasonFolders.Items.Add(lvi);

            txtSeasonNumber.Text = "";
            txtFolder.Text = "";

            lvSeasonFolders.Sort();
        }
        private void bnBrowseFolder_Click(object sender, System.EventArgs e)
        {
            folderBrowser.SelectedPath = txtFolder.Text;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtFolder.Text = folderBrowser.SelectedPath;
        }
        private void txtSeasonNumber_TextChanged(object sender, System.EventArgs e)
        {
            bool isNumber = Regex.Match(txtSeasonNumber.Text, "^[0-9]+$").Success;
            bnAdd.Enabled = isNumber && (!string.IsNullOrEmpty(txtSeasonNumber.Text));
        }
        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            bool ok = true;
            if (!string.IsNullOrEmpty(txtFolder.Text))
            {
                try
                {
                    ok = System.IO.Directory.Exists(txtFolder.Text);
                }
                catch
                {
                }
            }
            if (ok)
                txtFolder.BackColor = System.Drawing.SystemColors.Window;
            else
                txtFolder.BackColor = Helpers.WarningColor();

        }
        private void chkCustomShowName_CheckedChanged(object sender, System.EventArgs e)
        {
            txtCustomShowName.Enabled = chkCustomShowName.Checked;
        }
        private void chkPadTwoDigits_CheckedChanged(object sender, System.EventArgs e)
        {
            SetRightNumberOfHashes();
        }

        private void SetRightNumberOfHashes()
        {
            txtHash.Text = chkPadTwoDigits.Checked ? "##" : "#";
        }
    }
}
