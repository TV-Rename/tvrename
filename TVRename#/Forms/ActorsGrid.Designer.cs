//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


namespace TVRename
{
    partial class ActorsGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(ActorsGrid)));
            this.grid1 = (new SourceGrid.Grid());
            this.bnClose = (new System.Windows.Forms.Button());
            this.cbGuestStars = (new System.Windows.Forms.CheckBox());
            this.bnSave = (new System.Windows.Forms.Button());
            this.saveFile = (new System.Windows.Forms.SaveFileDialog());
            this.rbName = (new System.Windows.Forms.RadioButton());
            this.rbTotals = (new System.Windows.Forms.RadioButton());
            this.rbCustom = (new System.Windows.Forms.RadioButton());
            this.label1 = (new System.Windows.Forms.Label());
            this.SuspendLayout();
            // 
            // grid1
            // 
            this.grid1.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.grid1.BackColor = System.Drawing.SystemColors.Window;
            this.grid1.Location = new System.Drawing.Point(12, 12);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(907, 522);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // bnClose
            // 
            this.bnClose.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnClose.Location = new System.Drawing.Point(844, 548);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 1;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(bnClose_Click);
            // 
            // cbGuestStars
            // 
            this.cbGuestStars.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.cbGuestStars.AutoSize = true;
            this.cbGuestStars.Location = new System.Drawing.Point(12, 552);
            this.cbGuestStars.Name = "cbGuestStars";
            this.cbGuestStars.Size = new System.Drawing.Size(119, 17);
            this.cbGuestStars.TabIndex = 4;
            this.cbGuestStars.Text = "Include &Guest Stars";
            this.cbGuestStars.UseVisualStyleBackColor = true;
            this.cbGuestStars.CheckedChanged += new System.EventHandler(cbGuestStars_CheckedChanged);
            // 
            // bnSave
            // 
            this.bnSave.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnSave.Location = new System.Drawing.Point(763, 548);
            this.bnSave.Name = "bnSave";
            this.bnSave.Size = new System.Drawing.Size(75, 23);
            this.bnSave.TabIndex = 5;
            this.bnSave.Text = "&Save";
            this.bnSave.UseVisualStyleBackColor = true;
            this.bnSave.Click += new System.EventHandler(bnSave_Click);
            // 
            // rbName
            // 
            this.rbName.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.rbName.AutoSize = true;
            this.rbName.Checked = true;
            this.rbName.Location = new System.Drawing.Point(199, 552);
            this.rbName.Name = "rbName";
            this.rbName.Size = new System.Drawing.Size(53, 17);
            this.rbName.TabIndex = 6;
            this.rbName.TabStop = true;
            this.rbName.Text = "Name";
            this.rbName.UseVisualStyleBackColor = true;
            this.rbName.CheckedChanged += new System.EventHandler(rbName_CheckedChanged);
            // 
            // rbTotals
            // 
            this.rbTotals.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.rbTotals.AutoSize = true;
            this.rbTotals.Location = new System.Drawing.Point(258, 552);
            this.rbTotals.Name = "rbTotals";
            this.rbTotals.Size = new System.Drawing.Size(54, 17);
            this.rbTotals.TabIndex = 6;
            this.rbTotals.Text = "Totals";
            this.rbTotals.UseVisualStyleBackColor = true;
            this.rbTotals.CheckedChanged += new System.EventHandler(rbTotals_CheckedChanged);
            // 
            // rbCustom
            // 
            this.rbCustom.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.rbCustom.AutoSize = true;
            this.rbCustom.Location = new System.Drawing.Point(318, 552);
            this.rbCustom.Name = "rbCustom";
            this.rbCustom.Size = new System.Drawing.Size(60, 17);
            this.rbCustom.TabIndex = 6;
            this.rbCustom.Text = "Custom";
            this.rbCustom.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 554);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Sort:";
            // 
            // ActorsGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnClose;
            this.ClientSize = new System.Drawing.Size(931, 582);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbCustom);
            this.Controls.Add(this.rbTotals);
            this.Controls.Add(this.rbName);
            this.Controls.Add(this.bnSave);
            this.Controls.Add(this.cbGuestStars);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.grid1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ActorsGrid";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Actors Grid";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbName;
        private System.Windows.Forms.RadioButton rbTotals;
        private System.Windows.Forms.RadioButton rbCustom;
        private System.Windows.Forms.Label label1;
        private SourceGrid.Grid grid1;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.CheckBox cbGuestStars;
        private System.Windows.Forms.Button bnSave;
        private System.Windows.Forms.SaveFileDialog saveFile;
    }
}