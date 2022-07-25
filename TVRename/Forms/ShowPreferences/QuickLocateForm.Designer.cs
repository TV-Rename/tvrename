namespace TVRename.Forms.ShowPreferences
{
    partial class QuickLocateForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbDirectory = new System.Windows.Forms.ComboBox();
            this.bnCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.txtShowFolder = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Location:";
            // 
            // cbDirectory
            // 
            this.cbDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirectory.FormattingEnabled = true;
            this.cbDirectory.Location = new System.Drawing.Point(71, 16);
            this.cbDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbDirectory.Name = "cbDirectory";
            this.cbDirectory.Size = new System.Drawing.Size(316, 23);
            this.cbDirectory.Sorted = true;
            this.cbDirectory.TabIndex = 5;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(540, 58);
            this.bnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(88, 27);
            this.bnCancel.TabIndex = 8;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(446, 58);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(88, 27);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // txtShowFolder
            // 
            this.txtShowFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShowFolder.Location = new System.Drawing.Point(393, 16);
            this.txtShowFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtShowFolder.Name = "txtShowFolder";
            this.txtShowFolder.Size = new System.Drawing.Size(233, 23);
            this.txtShowFolder.TabIndex = 9;
            // 
            // QuickLocateForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(642, 98);
            this.ControlBox = false;
            this.Controls.Add(this.txtShowFolder);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbDirectory);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickLocateForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quick Locate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDirectory;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox txtShowFolder;
    }
}
