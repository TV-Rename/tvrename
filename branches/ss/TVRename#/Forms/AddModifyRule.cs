//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.Windows.Forms;

namespace TVRename
{

    /// <summary>
    /// Summary for AddModifyRule
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class AddModifyRule : Form
    {
        private ShowRule mRule;

        public AddModifyRule(ShowRule rule)
        {
            mRule = rule;
            InitializeComponent();

            FillDialog();
        }

        private void rb_Click(object sender, System.EventArgs e)
        {
            EnableDisableAndLabels();
        }
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
        private void bnOK_Click(object sender, System.EventArgs e)
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
        }
    }
}


