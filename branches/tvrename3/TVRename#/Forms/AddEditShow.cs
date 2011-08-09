// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TVRename.Shows;

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
        public string ShowTimeZone;
        private ShowItem mSI;
        private TheTVDBCodeFinder mTCCF;
        private TheTVDB mTVDB;

        public AddEditShow(ShowItem si, TheTVDB db)
        {
            this.mSI = si;
            this.mTVDB = db;
            this.InitializeComponent();

            this.cbTimeZone.BeginUpdate();
            this.cbTimeZone.Items.Clear();

            foreach (string s in TimeZone.ZoneNames())
                this.cbTimeZone.Items.Add(s);

            this.cbTimeZone.EndUpdate();

            this.mTCCF = new TheTVDBCodeFinder(si.innerDocument.TVDBCode != -1 ? si.innerDocument.TVDBCode.ToString() : "", this.mTVDB);
            this.mTCCF.Dock = DockStyle.Fill;
            //mTCCF->SelectionChanged += gcnew System::EventHandler(this, &AddEditShow::lvMatches_ItemSelectionChanged);

            this.pnlCF.SuspendLayout();
            this.pnlCF.Controls.Add(this.mTCCF);
            this.pnlCF.ResumeLayout();

            this.chkCustomShowName.Checked = si.innerDocument.UseCustomShowName;
            if (this.chkCustomShowName.Checked)
                this.txtCustomShowName.Text = si.innerDocument.CustomShowName;
            this.chkCustomShowName_CheckedChanged(null, null);

            this.cbSequentialMatching.Checked = si.innerDocument.UseSequentialMatch;
            this.chkShowNextAirdate.Checked = si.innerDocument.ShowNextAirdate;
            this.chkSpecialsCount.Checked = si.innerDocument.CountSpecials;
            this.chkFolderPerSeason.Checked = si.innerDocument.AutoAdd_FolderPerSeason;
            this.txtSeasonFolderName.Text = si.innerDocument.AutoAdd_SeasonFolderName;
            this.txtBaseFolder.Text = si.innerDocument.AutoAdd_FolderBase;
            this.chkAutoFolders.Checked = si.innerDocument.AutoAddNewSeasons;
            this.chkFolderPerSeason_CheckedChanged(null, null);

            this.cbDoRenaming.Checked = si.innerDocument.DoRename;
            this.cbDoMissingCheck.Checked = si.innerDocument.DoMissingCheck;
            this.cbDoMissingCheck_CheckedChanged(null, null);

            this.chkPadTwoDigits.Checked = si.innerDocument.PadSeasonToTwoDigits;

            this.ShowTimeZone = ((si == null)||(si.TheSeries() == null)) ? TimeZone.DefaultTimeZone() : si.TheSeries().ShowTimeZone;

            this.cbTimeZone.Text = this.ShowTimeZone;
            this.chkDVDOrder.Checked = si.innerDocument.DVDOrder;
            this.cbIncludeFuture.Checked = si.innerDocument.ForceCheckFuture;
            this.cbIncludeNoAirdate.Checked = si.innerDocument.ForceCheckNoAirdate;

            bool first = true;
            si.innerDocument.IgnoreSeasons.Sort();
            foreach (int i in si.innerDocument.IgnoreSeasons)
            {
                if (!first)
                    this.txtIgnoreSeasons.Text += " ";
                this.txtIgnoreSeasons.Text += i.ToString();
                first = false;
            }

            foreach (System.Collections.Generic.KeyValuePair<int, StringList> kvp in si.innerDocument.ManualFolderLocations)
            {
                foreach (string s in kvp.Value)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = kvp.Key.ToString();
                    lvi.SubItems.Add(s);

                    this.lvSeasonFolders.Items.Add(lvi);
                }
            }
            this.lvSeasonFolders.Sort();

            this.txtSeasonNumber_TextChanged(null, null);
            this.txtFolder_TextChanged(null, null);

            this.ActiveControl = mTCCF; // set initial focus to the code entry/show finder control

        }

        private void buttonOK_Click(object sender, System.EventArgs e)
        {
            if (!this.OKToClose())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            this.SetmSI();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool OKToClose()
        {
            if (!this.mTVDB.HasSeries(this.mTCCF.SelectedCode()))
            {
                DialogResult dr = MessageBox.Show("tvdb code unknown, close anyway?", "TVRename Add/Edit Show", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return false;
            }

            return true;
        }

        private void SetmSI()
        {
            int code = this.mTCCF.SelectedCode();

            string tz = (this.cbTimeZone.SelectedItem != null) ? this.cbTimeZone.SelectedItem.ToString() : "";

            this.mSI.innerDocument.CustomShowName = this.txtCustomShowName.Text;
            this.mSI.innerDocument.UseCustomShowName = this.chkCustomShowName.Checked;
            this.ShowTimeZone = tz; //TODO: move to somewhere else. make timezone manager for tvdb?
            this.mSI.innerDocument.ShowNextAirdate = this.chkShowNextAirdate.Checked;
            this.mSI.innerDocument.PadSeasonToTwoDigits = this.chkPadTwoDigits.Checked;
            this.mSI.innerDocument.TVDBCode = code;
            //todo mSI->SeasonNumber = seasnum;
            this.mSI.innerDocument.CountSpecials = this.chkSpecialsCount.Checked;
            //                                 mSI->Rules = mWorkingRuleSet;  // TODO
            this.mSI.innerDocument.DoRename = this.cbDoRenaming.Checked;
            this.mSI.innerDocument.DoMissingCheck = this.cbDoMissingCheck.Checked;

            this.mSI.innerDocument.AutoAddNewSeasons = this.chkAutoFolders.Checked;
            this.mSI.innerDocument.AutoAdd_FolderPerSeason = this.chkFolderPerSeason.Checked;
            this.mSI.innerDocument.AutoAdd_SeasonFolderName = this.txtSeasonFolderName.Text;
            this.mSI.innerDocument.AutoAdd_FolderBase = this.txtBaseFolder.Text;

            this.mSI.innerDocument.DVDOrder = this.chkDVDOrder.Checked;
            this.mSI.innerDocument.ForceCheckFuture = this.cbIncludeFuture.Checked;
            this.mSI.innerDocument.ForceCheckNoAirdate = this.cbIncludeNoAirdate.Checked;

            this.mSI.innerDocument.UseSequentialMatch = this.cbSequentialMatching.Checked;

            string slist = this.txtIgnoreSeasons.Text;
            this.mSI.innerDocument.IgnoreSeasons.Clear();
            foreach (Match match in Regex.Matches(slist, "\\b[0-9]+\\b"))
                this.mSI.innerDocument.IgnoreSeasons.Add(int.Parse(match.Value));

            this.mSI.innerDocument.ManualFolderLocations.Clear();
            foreach (ListViewItem lvi in this.lvSeasonFolders.Items)
            {
                try
                {
                    int seas = int.Parse(lvi.Text);
                    if (!this.mSI.innerDocument.ManualFolderLocations.ContainsKey(seas))
                        this.mSI.innerDocument.ManualFolderLocations.Add(seas, new StringList());

                    this.mSI.innerDocument.ManualFolderLocations[seas].Add(lvi.SubItems[1].Text);
                }
                catch
                {
                }
            }
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void chkFolderPerSeason_CheckedChanged(object sender, System.EventArgs e)
        {
            this.txtSeasonFolderName.Enabled = this.chkFolderPerSeason.Checked;
        }

        private void bnBrowse_Click(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtBaseFolder.Text))
                this.folderBrowser.SelectedPath = this.txtBaseFolder.Text;

            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtBaseFolder.Text = this.folderBrowser.SelectedPath;
        }

        private void cbDoMissingCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            this.cbIncludeNoAirdate.Enabled = this.cbDoMissingCheck.Checked;
            this.cbIncludeFuture.Enabled = this.cbDoMissingCheck.Checked;
        }

        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            if (this.lvSeasonFolders.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in this.lvSeasonFolders.SelectedItems)
                    this.lvSeasonFolders.Items.Remove(lvi);
            }
        }

        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = this.txtSeasonNumber.Text;
            lvi.SubItems.Add(this.txtFolder.Text);

            this.lvSeasonFolders.Items.Add(lvi);

            this.txtSeasonNumber.Text = "";
            this.txtFolder.Text = "";

            this.lvSeasonFolders.Sort();
        }

        private void bnBrowseFolder_Click(object sender, System.EventArgs e)
        {
            this.folderBrowser.SelectedPath = this.txtFolder.Text;
            if (this.folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txtFolder.Text = this.folderBrowser.SelectedPath;
        }

        private void txtSeasonNumber_TextChanged(object sender, System.EventArgs e)
        {
            bool isNumber = Regex.Match(this.txtSeasonNumber.Text, "^[0-9]+$").Success;
            this.bnAdd.Enabled = isNumber && (!string.IsNullOrEmpty(this.txtSeasonNumber.Text));
        }

        private void txtFolder_TextChanged(object sender, System.EventArgs e)
        {
            bool ok = true;
            if (!string.IsNullOrEmpty(this.txtFolder.Text))
            {
                try
                {
                    ok = System.IO.Directory.Exists(this.txtFolder.Text);
                }
                catch
                {
                }
            }
            if (ok)
                this.txtFolder.BackColor = System.Drawing.SystemColors.Window;
            else
                this.txtFolder.BackColor = Helpers.WarningColor();
        }

        private void chkCustomShowName_CheckedChanged(object sender, System.EventArgs e)
        {
            this.txtCustomShowName.Enabled = this.chkCustomShowName.Checked;
        }

        private void chkAutoFolders_CheckedChanged(object sender, System.EventArgs e)
        {
            gbAutoFolders.Enabled = chkAutoFolders.Checked;
        }
    }
}