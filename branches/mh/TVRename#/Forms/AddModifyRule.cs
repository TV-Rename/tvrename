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
            this.mRule = rule;
            this.InitializeComponent();

            this.FillDialog();
        }

        private void rb_Click(object sender, System.EventArgs e)
        {
            this.EnableDisableAndLabels();
        }

        private void FillDialog()
        {
            switch (this.mRule.DoWhatNow)
            {
                case RuleAction.kRename:
                    this.rbRename.Checked = true;
                    break;

                case RuleAction.kCollapse:
                    this.rbCollapse.Checked = true;
                    break;

                case RuleAction.kRemove:
                    this.rbRemove.Checked = true;
                    break;

                case RuleAction.kSwap:
                    this.rbSwap.Checked = true;
                    break;

                case RuleAction.kMerge:
                    this.rbMerge.Checked = true;
                    break;

                case RuleAction.kSplit:
                    this.rbSplit.Checked = true;
                    break;

                case RuleAction.kInsert:
                    this.rbInsert.Checked = true;
                    break;

                default:
                case RuleAction.kIgnoreEp:
                    this.rbIgnore.Checked = true;
                    break;
            }

            this.txtUserText.Text = this.mRule.UserSuppliedText;
            if (this.mRule.First != -1)
                this.txtValue1.Text = this.mRule.First.ToString();
            if (this.mRule.Second != -1)
                this.txtValue2.Text = this.mRule.Second.ToString();

            this.EnableDisableAndLabels();
        }

        private void EnableDisableAndLabels()
        {
            if (this.rbRemove.Checked)
            {
                this.txtLabel1.Text = "&From/Number:";
                this.txtLabel2.Text = "T&o (Optional):";
                this.txtLeaveBlank.Visible = false;
                this.txtLabel2.Enabled = true;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = true;
                this.txtUserText.Enabled = false;
                this.txtWithNameLabel.Enabled = false;
                this.txtWithNameLabel.Text = "New Name:";
            }
            else if (this.rbSwap.Checked)
            {
                this.txtLabel1.Text = "&Number:";
                this.txtLabel2.Text = "N&umber:";
                this.txtLeaveBlank.Visible = false;
                this.txtLabel2.Enabled = true;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = true;
                this.txtUserText.Enabled = false;
                this.txtWithNameLabel.Enabled = false;
                this.txtWithNameLabel.Text = "New Name:";
            }
            else if (this.rbMerge.Checked || this.rbCollapse.Checked)
            {
                this.txtLabel1.Text = "&From:";
                this.txtLabel2.Text = "T&o:";
                this.txtLeaveBlank.Visible = true;
                this.txtLabel2.Enabled = true;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = true;
                this.txtUserText.Enabled = true;
                this.txtWithNameLabel.Enabled = true;
                this.txtWithNameLabel.Text = "New Name:";
            }
            else if (this.rbInsert.Checked)
            {
                this.txtLabel1.Text = "&At:";
                this.txtLabel2.Text = "N&umber:";
                this.txtLeaveBlank.Visible = false;
                this.txtLabel2.Enabled = false;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = false;
                this.txtUserText.Enabled = true;
                this.txtWithNameLabel.Enabled = true;
                this.txtWithNameLabel.Text = "New Name:";
            }
            else if (this.rbIgnore.Checked)
            {
                this.txtLabel1.Text = "&From/Number:";
                this.txtLabel2.Text = "T&o (Optional):";
                this.txtLeaveBlank.Visible = false;
                this.txtLabel2.Enabled = true;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = true;
                this.txtUserText.Enabled = false;
                this.txtWithNameLabel.Enabled = false;
                this.txtWithNameLabel.Text = "New Name:";
            }
            else if (this.rbRename.Checked)
            {
                this.txtLabel1.Text = "&Number:";
                this.txtLabel2.Text = "N&umber:";
                this.txtLeaveBlank.Visible = false;
                this.txtLabel2.Enabled = false;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = false;
                this.txtUserText.Enabled = true;
                this.txtWithNameLabel.Enabled = true;
                this.txtWithNameLabel.Text = "New Name:";
            }
            else if (this.rbSplit.Checked)
            {
                this.txtLabel1.Text = "&Number:";
                this.txtLabel2.Text = "Int&o:";
                this.txtLeaveBlank.Visible = true;
                this.txtLabel2.Enabled = true;
                this.txtValue1.Enabled = true;
                this.txtValue2.Enabled = true;
                this.txtUserText.Enabled = true;
                this.txtWithNameLabel.Enabled = true;
                this.txtWithNameLabel.Text = "Delimiters:";
            }
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            RuleAction dwn = RuleAction.kIgnoreEp;

            if (this.rbIgnore.Checked)
                dwn = RuleAction.kIgnoreEp;
            else if (this.rbSwap.Checked)
                dwn = RuleAction.kSwap;
            else if (this.rbMerge.Checked)
                dwn = RuleAction.kMerge;
            else if (this.rbInsert.Checked)
                dwn = RuleAction.kInsert;
            else if (this.rbRemove.Checked)
                dwn = RuleAction.kRemove;
            else if (this.rbCollapse.Checked)
                dwn = RuleAction.kCollapse;
            else if (this.rbRename.Checked)
                dwn = RuleAction.kRename;
            else if (this.rbSplit.Checked)
                dwn = RuleAction.kSplit;

            this.mRule.DoWhatNow = dwn;
            this.mRule.UserSuppliedText = this.txtUserText.Enabled ? this.txtUserText.Text : "";

            try
            {
                this.mRule.First = this.txtValue1.Enabled ? Convert.ToInt32(this.txtValue1.Text) : -1;
            }
            catch
            {
                this.mRule.First = -1;
            }

            try
            {
                this.mRule.Second = this.txtValue2.Enabled ? Convert.ToInt32(this.txtValue2.Text) : -1;
            }
            catch
            {
                this.mRule.Second = -1;
            }
        }
    }
}