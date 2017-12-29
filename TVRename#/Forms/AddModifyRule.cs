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
        private readonly ShowRule _mRule;

        public AddModifyRule(ShowRule rule)
        {
            _mRule = rule;
            InitializeComponent();

            FillDialog();
        }

        private void rb_Click(object sender, EventArgs e)
        {
            EnableDisableAndLabels();
        }

        private void FillDialog()
        {
            switch (_mRule.DoWhatNow)
            {
                case RuleAction.KRename:
                    rbRename.Checked = true;
                    break;

                case RuleAction.KCollapse:
                    rbCollapse.Checked = true;
                    break;

                case RuleAction.KRemove:
                    rbRemove.Checked = true;
                    break;

                case RuleAction.KSwap:
                    rbSwap.Checked = true;
                    break;

                case RuleAction.KMerge:
                    rbMerge.Checked = true;
                    break;

                case RuleAction.KSplit:
                    rbSplit.Checked = true;
                    break;

                case RuleAction.KInsert:
                    rbInsert.Checked = true;
                    break;

                default:
                case RuleAction.KIgnoreEp:
                    rbIgnore.Checked = true;
                    break;
            }

            txtUserText.Text = _mRule.UserSuppliedText;
            if (_mRule.First != -1)
                txtValue1.Text = _mRule.First.ToString();
            if (_mRule.Second != -1)
                txtValue2.Text = _mRule.Second.ToString();

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
            RuleAction dwn = RuleAction.KIgnoreEp;

            if (rbIgnore.Checked)
                dwn = RuleAction.KIgnoreEp;
            else if (rbSwap.Checked)
                dwn = RuleAction.KSwap;
            else if (rbMerge.Checked)
                dwn = RuleAction.KMerge;
            else if (rbInsert.Checked)
                dwn = RuleAction.KInsert;
            else if (rbRemove.Checked)
                dwn = RuleAction.KRemove;
            else if (rbCollapse.Checked)
                dwn = RuleAction.KCollapse;
            else if (rbRename.Checked)
                dwn = RuleAction.KRename;
            else if (rbSplit.Checked)
                dwn = RuleAction.KSplit;

            _mRule.DoWhatNow = dwn;
            _mRule.UserSuppliedText = txtUserText.Enabled ? txtUserText.Text : "";

            try
            {
                _mRule.First = txtValue1.Enabled ? Convert.ToInt32(txtValue1.Text) : -1;
            }
            catch
            {
                _mRule.First = -1;
            }

            try
            {
                _mRule.Second = txtValue2.Enabled ? Convert.ToInt32(txtValue2.Text) : -1;
            }
            catch
            {
                _mRule.Second = -1;
            }
        }
    }
}
