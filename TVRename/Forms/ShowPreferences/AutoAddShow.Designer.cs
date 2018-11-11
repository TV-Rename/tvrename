namespace TVRename
{
    partial class AutoAddShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoAddShow));
            this.cbDirectory = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlCF = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDirectoryName = new System.Windows.Forms.Label();
            this.btnIgnoreFile = new System.Windows.Forms.Button();
            this.btnSkipAutoAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbDirectory
            // 
            this.cbDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirectory.FormattingEnabled = true;
            this.cbDirectory.Location = new System.Drawing.Point(89, 317);
            this.cbDirectory.Name = "cbDirectory";
            this.cbDirectory.Size = new System.Drawing.Size(221, 21);
            this.cbDirectory.Sorted = true;
            this.cbDirectory.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(392, 350);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Quick Add";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(296, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Leave for later";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlCF
            // 
            this.pnlCF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCF.Location = new System.Drawing.Point(10, 10);
            this.pnlCF.Name = "pnlCF";
            this.pnlCF.Size = new System.Drawing.Size(462, 301);
            this.pnlCF.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 324);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Location:";
            // 
            // lblDirectoryName
            // 
            this.lblDirectoryName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDirectoryName.AutoSize = true;
            this.lblDirectoryName.Location = new System.Drawing.Point(318, 322);
            this.lblDirectoryName.Name = "lblDirectoryName";
            this.lblDirectoryName.Size = new System.Drawing.Size(0, 13);
            this.lblDirectoryName.TabIndex = 5;
            // 
            // btnIgnoreFile
            // 
            this.btnIgnoreFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIgnoreFile.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIgnoreFile.Location = new System.Drawing.Point(185, 350);
            this.btnIgnoreFile.Name = "btnIgnoreFile";
            this.btnIgnoreFile.Size = new System.Drawing.Size(105, 23);
            this.btnIgnoreFile.TabIndex = 6;
            this.btnIgnoreFile.Text = "Ignore File Forever";
            this.btnIgnoreFile.UseVisualStyleBackColor = true;
            this.btnIgnoreFile.Click += new System.EventHandler(this.btnIgnoreFile_Click);
            // 
            // btnSkipAutoAdd
            // 
            this.btnSkipAutoAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSkipAutoAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSkipAutoAdd.Location = new System.Drawing.Point(89, 350);
            this.btnSkipAutoAdd.Name = "btnSkipAutoAdd";
            this.btnSkipAutoAdd.Size = new System.Drawing.Size(90, 23);
            this.btnSkipAutoAdd.TabIndex = 7;
            this.btnSkipAutoAdd.Text = "Skip Auto Add";
            this.btnSkipAutoAdd.UseVisualStyleBackColor = true;
            this.btnSkipAutoAdd.Click += new System.EventHandler(this.btnSkipAutoAdd_Click);
            // 
            // AutoAddShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(479, 385);
            this.Controls.Add(this.btnSkipAutoAdd);
            this.Controls.Add(this.btnIgnoreFile);
            this.Controls.Add(this.lblDirectoryName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlCF);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbDirectory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoAddShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Show Detected...";
            this.Load += new System.EventHandler(this.AutoAddShow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDirectory;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlCF;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDirectoryName;
        private System.Windows.Forms.Button btnIgnoreFile;
        private System.Windows.Forms.Button btnSkipAutoAdd;
    }
}
