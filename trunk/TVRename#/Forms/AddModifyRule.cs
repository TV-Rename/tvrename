//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;


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
    public class AddModifyRule : System.Windows.Forms.Form
    {
        private ShowRule mRule;

        public AddModifyRule(ShowRule rule)
        {
            mRule = rule;
            InitializeComponent();

            FillDialog();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        private System.Windows.Forms.RadioButton rbRemove;

        private System.Windows.Forms.RadioButton rbSwap;


        private System.Windows.Forms.RadioButton rbMerge;
        private System.Windows.Forms.RadioButton rbInsert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txtLabel1;
        private System.Windows.Forms.Label txtLabel2;
        private System.Windows.Forms.TextBox txtValue1;
        private System.Windows.Forms.TextBox txtValue2;
        private System.Windows.Forms.Label txtWithNameLabel;







        private System.Windows.Forms.TextBox txtUserText;

        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.RadioButton rbIgnore;
        private System.Windows.Forms.RadioButton rbRename;
        private System.Windows.Forms.Label txtLeaveBlank;
        private System.Windows.Forms.RadioButton rbSplit;
        private System.Windows.Forms.RadioButton rbCollapse;


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(AddModifyRule)));
            this.rbRemove = (new System.Windows.Forms.RadioButton());
            this.rbSwap = (new System.Windows.Forms.RadioButton());
            this.rbMerge = (new System.Windows.Forms.RadioButton());
            this.rbInsert = (new System.Windows.Forms.RadioButton());
            this.label1 = (new System.Windows.Forms.Label());
            this.txtLabel1 = (new System.Windows.Forms.Label());
            this.txtLabel2 = (new System.Windows.Forms.Label());
            this.txtValue1 = (new System.Windows.Forms.TextBox());
            this.txtValue2 = (new System.Windows.Forms.TextBox());
            this.txtWithNameLabel = (new System.Windows.Forms.Label());
            this.txtUserText = (new System.Windows.Forms.TextBox());
            this.bnOK = (new System.Windows.Forms.Button());
            this.bnCancel = (new System.Windows.Forms.Button());
            this.rbIgnore = (new System.Windows.Forms.RadioButton());
            this.rbRename = (new System.Windows.Forms.RadioButton());
            this.txtLeaveBlank = (new System.Windows.Forms.Label());
            this.rbSplit = (new System.Windows.Forms.RadioButton());
            this.rbCollapse = (new System.Windows.Forms.RadioButton());
            this.SuspendLayout();
            // 
            // rbRemove
            // 
            this.rbRemove.AutoSize = true;
            this.rbRemove.Location = new System.Drawing.Point(61, 58);
            this.rbRemove.Name = "rbRemove";
            this.rbRemove.Size = new System.Drawing.Size(236, 17);
            this.rbRemove.TabIndex = 2;
            this.rbRemove.TabStop = true;
            this.rbRemove.Text = "R&emove : Remove episode(s) from the series";
            this.rbRemove.UseVisualStyleBackColor = true;
            this.rbRemove.Click += new System.EventHandler(rb_Click);
            // 
            // rbSwap
            // 
            this.rbSwap.AutoSize = true;
            this.rbSwap.Location = new System.Drawing.Point(61, 81);
            this.rbSwap.Name = "rbSwap";
            this.rbSwap.Size = new System.Drawing.Size(204, 17);
            this.rbSwap.TabIndex = 3;
            this.rbSwap.TabStop = true;
            this.rbSwap.Text = "&Swap : Swap position of two episodes";
            this.rbSwap.UseVisualStyleBackColor = true;
            this.rbSwap.Click += new System.EventHandler(rb_Click);
            // 
            // rbMerge
            // 
            this.rbMerge.AutoSize = true;
            this.rbMerge.Location = new System.Drawing.Point(61, 104);
            this.rbMerge.Name = "rbMerge";
            this.rbMerge.Size = new System.Drawing.Size(239, 17);
            this.rbMerge.TabIndex = 4;
            this.rbMerge.TabStop = true;
            this.rbMerge.Text = "&Merge : Merge episodes into multi-episode file";
            this.rbMerge.UseVisualStyleBackColor = true;
            this.rbMerge.Click += new System.EventHandler(rb_Click);
            // 
            // rbInsert
            // 
            this.rbInsert.AutoSize = true;
            this.rbInsert.Location = new System.Drawing.Point(61, 127);
            this.rbInsert.Name = "rbInsert";
            this.rbInsert.Size = new System.Drawing.Size(240, 17);
            this.rbInsert.TabIndex = 5;
            this.rbInsert.TabStop = true;
            this.rbInsert.Text = "Inser&t : Insert another episode into the season";
            this.rbInsert.UseVisualStyleBackColor = true;
            this.rbInsert.Click += new System.EventHandler(rb_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action:";
            // 
            // txtLabel1
            // 
            this.txtLabel1.AutoSize = true;
            this.txtLabel1.Location = new System.Drawing.Point(13, 217);
            this.txtLabel1.Name = "txtLabel1";
            this.txtLabel1.Size = new System.Drawing.Size(42, 13);
            this.txtLabel1.TabIndex = 6;
            this.txtLabel1.Text = "Label1:";
            // 
            // txtLabel2
            // 
            this.txtLabel2.AutoSize = true;
            this.txtLabel2.Location = new System.Drawing.Point(13, 242);
            this.txtLabel2.Name = "txtLabel2";
            this.txtLabel2.Size = new System.Drawing.Size(42, 13);
            this.txtLabel2.TabIndex = 8;
            this.txtLabel2.Text = "Label2:";
            // 
            // txtValue1
            // 
            this.txtValue1.Location = new System.Drawing.Point(80, 213);
            this.txtValue1.Name = "txtValue1";
            this.txtValue1.Size = new System.Drawing.Size(100, 20);
            this.txtValue1.TabIndex = 7;
            // 
            // txtValue2
            // 
            this.txtValue2.Location = new System.Drawing.Point(80, 239);
            this.txtValue2.Name = "txtValue2";
            this.txtValue2.Size = new System.Drawing.Size(100, 20);
            this.txtValue2.TabIndex = 9;
            // 
            // txtWithNameLabel
            // 
            this.txtWithNameLabel.AutoSize = true;
            this.txtWithNameLabel.Location = new System.Drawing.Point(13, 269);
            this.txtWithNameLabel.Name = "txtWithNameLabel";
            this.txtWithNameLabel.Size = new System.Drawing.Size(63, 13);
            this.txtWithNameLabel.TabIndex = 10;
            this.txtWithNameLabel.Text = "New Name:";
            // 
            // txtUserText
            // 
            this.txtUserText.Location = new System.Drawing.Point(80, 266);
            this.txtUserText.Name = "txtUserText";
            this.txtUserText.Size = new System.Drawing.Size(254, 20);
            this.txtUserText.TabIndex = 11;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(175, 308);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 12;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += bnOK_Click;
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(256, 308);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 13;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // rbIgnore
            // 
            this.rbIgnore.AutoSize = true;
            this.rbIgnore.Location = new System.Drawing.Point(61, 12);
            this.rbIgnore.Name = "rbIgnore";
            this.rbIgnore.Size = new System.Drawing.Size(246, 17);
            this.rbIgnore.TabIndex = 1;
            this.rbIgnore.TabStop = true;
            this.rbIgnore.Text = "&Ignore : Don\'t rename or check for episode(s)";
            this.rbIgnore.UseVisualStyleBackColor = true;
            this.rbIgnore.Click += new System.EventHandler(rb_Click);
            // 
            // rbRename
            // 
            this.rbRename.AutoSize = true;
            this.rbRename.Location = new System.Drawing.Point(61, 35);
            this.rbRename.Name = "rbRename";
            this.rbRename.Size = new System.Drawing.Size(203, 17);
            this.rbRename.TabIndex = 1;
            this.rbRename.TabStop = true;
            this.rbRename.Text = "&Rename : Set episode name manually";
            this.rbRename.UseVisualStyleBackColor = true;
            this.rbRename.Click += new System.EventHandler(rb_Click);
            // 
            // txtLeaveBlank
            // 
            this.txtLeaveBlank.AutoSize = true;
            this.txtLeaveBlank.Location = new System.Drawing.Point(77, 289);
            this.txtLeaveBlank.Name = "txtLeaveBlank";
            this.txtLeaveBlank.Size = new System.Drawing.Size(173, 13);
            this.txtLeaveBlank.TabIndex = 10;
            this.txtLeaveBlank.Text = "(Leave blank for automatic naming)";
            // 
            // rbSplit
            // 
            this.rbSplit.AutoSize = true;
            this.rbSplit.Location = new System.Drawing.Point(61, 150);
            this.rbSplit.Name = "rbSplit";
            this.rbSplit.Size = new System.Drawing.Size(221, 17);
            this.rbSplit.TabIndex = 5;
            this.rbSplit.TabStop = true;
            this.rbSplit.Text = "S&plit: Make one episode count as multiple";
            this.rbSplit.UseVisualStyleBackColor = true;
            this.rbSplit.Click += new System.EventHandler(rb_Click);
            // 
            // rbCollapse
            // 
            this.rbCollapse.AutoSize = true;
            this.rbCollapse.Location = new System.Drawing.Point(61, 173);
            this.rbCollapse.Name = "rbCollapse";
            this.rbCollapse.Size = new System.Drawing.Size(217, 17);
            this.rbCollapse.TabIndex = 5;
            this.rbCollapse.TabStop = true;
            this.rbCollapse.Text = "&Collapse: Merge episodes, and renumber";
            this.rbCollapse.UseVisualStyleBackColor = true;
            this.rbCollapse.Click += new System.EventHandler(rb_Click);
            // 
            // AddModifyRule
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(341, 341);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.txtUserText);
            this.Controls.Add(this.txtValue2);
            this.Controls.Add(this.txtValue1);
            this.Controls.Add(this.txtLeaveBlank);
            this.Controls.Add(this.txtWithNameLabel);
            this.Controls.Add(this.txtLabel2);
            this.Controls.Add(this.txtLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbRename);
            this.Controls.Add(this.rbIgnore);
            this.Controls.Add(this.rbCollapse);
            this.Controls.Add(this.rbSplit);
            this.Controls.Add(this.rbInsert);
            this.Controls.Add(this.rbMerge);
            this.Controls.Add(this.rbSwap);
            this.Controls.Add(this.rbRemove);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddModifyRule";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add/Modify Rule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
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


