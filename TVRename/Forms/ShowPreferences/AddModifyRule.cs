// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 
using System;
using System.Windows.Forms;

namespace TVRename
{
    /// <inheritdoc />
    ///  <summary>
    ///  Summary for AddModifyRule
    ///  WARNING: If you change the name of this class, you will need to change the
    ///           'Resource File Name' property for the managed resource compiler tool
    ///           associated with all .resx files this class depends on.  Otherwise,
    ///           the designers will not be able to interact properly with localized
    ///           resources associated with this form.
    ///  </summary>
    public partial class AddModifyRule : Form
    {
        private readonly ShowRule mRule;
        private readonly Season mSeason;
        private readonly bool mdvdOrder;

        public AddModifyRule(ShowRule rule, Season season, bool dvdOrder)
        {
            mRule = rule;
            mSeason = season;
            mdvdOrder = dvdOrder;

            InitializeComponent();

            FillDialog();
        }

        private void rb_Click(object sender, EventArgs e) => EnableDisableAndLabels();

        private void FillDialog()
        {
            switch (mRule.DoWhatNow)
            {
                case RuleAction.kRename:
                    rbRename.Checked = true;
                    break;

                case RuleAction.kCollapse:
                    rbCollapse.Checked = true;
                    break;

                case RuleAction.kRemove:
                    rbRemove.Checked = true;
                    break;

                case RuleAction.kSwap:
                    rbSwap.Checked = true;
                    break;

                case RuleAction.kMerge:
                    rbMerge.Checked = true;
                    break;

                case RuleAction.kSplit:
                    rbSplit.Checked = true;
                    break;

                case RuleAction.kInsert:
                    rbInsert.Checked = true;
                    break;

                default:
                case RuleAction.kIgnoreEp:
                    rbIgnore.Checked = true;
                    break;
            }

            txtUserText.Text = mRule.UserSuppliedText;
            if (mRule.First != -1)
                txtValue1.Text = mRule.First.ToString();
            if (mRule.Second != -1)
                txtValue2.Text = mRule.Second.ToString();

            EnableDisableAndLabels();
        }

        private void EnableDisableAndLabels()
        {
            if (rbRemove.Checked)
            {
                txtLabel1.Text = "&From/Number:";
                txtLabel2.Text = "T&o:";
                txtLeaveBlank.Visible = false;
                txtLabel2.Enabled = true;
                txtValue1.Enabled = true;
                txtValue2.Enabled = true;
                txtUserText.Enabled = false;
                txtWithNameLabel.Enabled = false;
            }
            else if (rbSwap.Checked)
            {
                txtLabel1.Text = "&Number:";
                txtLabel2.Text = "N&umber:";
                txtLeaveBlank.Visible = false;
                txtLabel2.Enabled = true;
                txtValue1.Enabled = true;
                txtValue2.Enabled = true;
                txtUserText.Enabled = false;
                txtWithNameLabel.Enabled = false;
            }
            else if (rbMerge.Checked || rbCollapse.Checked)
            {
                txtLabel1.Text = "&From:";
                txtLabel2.Text = "T&o:";
                txtLeaveBlank.Visible = true;
                txtLabel2.Enabled = true;
                txtValue1.Enabled = true;
                txtValue2.Enabled = true;
                txtUserText.Enabled = true;
                txtWithNameLabel.Enabled = true;
            }
            else if (rbInsert.Checked)
            {
                txtLabel1.Text = "&At:";
                txtLabel2.Text = "N&umber:";
                txtLeaveBlank.Visible = false;
                txtLabel2.Enabled = false;
                txtValue1.Enabled = true;
                txtValue2.Enabled = false;
                txtUserText.Enabled = true;
                txtWithNameLabel.Enabled = true;
            }
            else if (rbIgnore.Checked)
            {
                txtLabel1.Text = "&From/Number:";
                txtLabel2.Text = "T&o:";
                txtLeaveBlank.Visible = false;
                txtLabel2.Enabled = true;
                txtValue1.Enabled = true;
                txtValue2.Enabled = true;
                txtUserText.Enabled = false;
                txtWithNameLabel.Enabled = false;
            }
            else if (rbRename.Checked)
            {
                txtLabel1.Text = "&Number:";
                txtLabel2.Text = "N&umber:";
                txtLeaveBlank.Visible = false;
                txtLabel2.Enabled = false;
                txtValue1.Enabled = true;
                txtValue2.Enabled = false;
                txtUserText.Enabled = true;
                txtWithNameLabel.Enabled = true;
            }
            else if (rbSplit.Checked)
            {
                txtLabel1.Text = "&Number:";
                txtLabel2.Text = "Int&o:";
                txtLeaveBlank.Visible = false;
                txtLabel2.Enabled = true;
                txtValue1.Enabled = true;
                txtValue2.Enabled = true;
                txtUserText.Enabled = false;
                txtWithNameLabel.Enabled = false;
            }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            RuleAction dwn = RuleAction.kIgnoreEp;

            if (rbIgnore.Checked)
                dwn = RuleAction.kIgnoreEp;
            else if (rbSwap.Checked)
                dwn = RuleAction.kSwap;
            else if (rbMerge.Checked)
                dwn = RuleAction.kMerge;
            else if (rbInsert.Checked)
                dwn = RuleAction.kInsert;
            else if (rbRemove.Checked)
                dwn = RuleAction.kRemove;
            else if (rbCollapse.Checked)
                dwn = RuleAction.kCollapse;
            else if (rbRename.Checked)
                dwn = RuleAction.kRename;
            else if (rbSplit.Checked)
                dwn = RuleAction.kSplit;

            mRule.DoWhatNow = dwn;
            mRule.UserSuppliedText = txtUserText.Enabled ? txtUserText.Text : "";

            try
            {
                mRule.First = txtValue1.Enabled ? Convert.ToInt32(txtValue1.Text) : -1;
            }
            catch
            {
                mRule.First = -1;
            }

            try
            {
                mRule.Second = txtValue2.Enabled ? Convert.ToInt32(txtValue2.Text) : -1;
            }
            catch
            {
                mRule.Second = -1;
            }

            //validation Rules
            if (!mSeason.ContainsEpisode(mRule.First, mdvdOrder))
            {
                MessageBox.Show("First episode number is not valid for the selected season", "Modify Rules",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValue1.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            //these 3 types only have one episode cited
            if (!(mRule.DoWhatNow == RuleAction.kRename || mRule.DoWhatNow == RuleAction.kInsert || mRule.DoWhatNow == RuleAction.kSplit) &&
                !mSeason.ContainsEpisode(mRule.Second, mdvdOrder))
            {
                MessageBox.Show("Second episode number is not valid for the selected season", "Modify Rules",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValue2.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            //these 3 types only have one episode cited - others must be in order
            if (!(mRule.DoWhatNow == RuleAction.kRename || mRule.DoWhatNow == RuleAction.kInsert || mRule.DoWhatNow == RuleAction.kSplit) &&
                mRule.First < mRule.Second)
            {
                MessageBox.Show("Second episode number must be before the first episode number", "Modify Rules",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValue2.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            //Swap, merge and collapse can't be done on the same episode numbers
            if ((mRule.DoWhatNow == RuleAction.kSwap || mRule.DoWhatNow == RuleAction.kMerge || mRule.DoWhatNow == RuleAction.kCollapse) &&
                (txtValue2.Text.Equals(txtValue1.Text)))
            {
                MessageBox.Show("Episode Numbers must be different", "Modify Rules",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValue2.Focus();
                DialogResult = DialogResult.None;
            }
        }
    }
}
