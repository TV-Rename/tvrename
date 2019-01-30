namespace TVRename
{
    partial class ChooseFile
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
            this.btnLeft = new System.Windows.Forms.Button();
            this.Ignore = new System.Windows.Forms.Button();
            this.btnKeepRight = new System.Windows.Forms.Button();
            this.txtLengthLeft = new System.Windows.Forms.Label();
            this.lnkOpenLeftFolder = new System.Windows.Forms.LinkLabel();
            this.txtDimensionsLeft = new System.Windows.Forms.Label();
            this.txtPathLeft = new System.Windows.Forms.Label();
            this.txtPathRight = new System.Windows.Forms.Label();
            this.lblDimensionsRight = new System.Windows.Forms.Label();
            this.lnkOpenRightFolder = new System.Windows.Forms.LinkLabel();
            this.lblLengthRight = new System.Windows.Forms.Label();
            this.lblSizeRight = new System.Windows.Forms.Label();
            this.lblNameRight = new System.Windows.Forms.Label();
            this.txtSizeLeft = new System.Windows.Forms.Label();
            this.txtNameLeft = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(311, 50);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 46);
            this.btnLeft.TabIndex = 0;
            this.btnLeft.Text = "Keep";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // Ignore
            // 
            this.Ignore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Ignore.Location = new System.Drawing.Point(276, 275);
            this.Ignore.Name = "Ignore";
            this.Ignore.Size = new System.Drawing.Size(110, 23);
            this.Ignore.TabIndex = 2;
            this.Ignore.Text = "Ignore (keep both)";
            this.Ignore.UseVisualStyleBackColor = true;
            this.Ignore.Click += new System.EventHandler(this.Ignore_Click);
            // 
            // btnKeepRight
            // 
            this.btnKeepRight.Location = new System.Drawing.Point(311, 179);
            this.btnKeepRight.Name = "btnKeepRight";
            this.btnKeepRight.Size = new System.Drawing.Size(75, 46);
            this.btnKeepRight.TabIndex = 3;
            this.btnKeepRight.Text = "Keep";
            this.btnKeepRight.UseVisualStyleBackColor = true;
            this.btnKeepRight.Click += new System.EventHandler(this.btnKeepRight_Click);
            // 
            // txtLengthLeft
            // 
            this.txtLengthLeft.AutoSize = true;
            this.txtLengthLeft.Location = new System.Drawing.Point(10, 66);
            this.txtLengthLeft.Name = "txtLengthLeft";
            this.txtLengthLeft.Size = new System.Drawing.Size(35, 13);
            this.txtLengthLeft.TabIndex = 4;
            this.txtLengthLeft.Text = "label1";
            // 
            // lnkOpenLeftFolder
            // 
            this.lnkOpenLeftFolder.AutoSize = true;
            this.lnkOpenLeftFolder.Location = new System.Drawing.Point(10, 113);
            this.lnkOpenLeftFolder.Name = "lnkOpenLeftFolder";
            this.lnkOpenLeftFolder.Size = new System.Drawing.Size(118, 13);
            this.lnkOpenLeftFolder.TabIndex = 5;
            this.lnkOpenLeftFolder.TabStop = true;
            this.lnkOpenLeftFolder.Text = "Open Containing Folder";
            this.lnkOpenLeftFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenLeftFolder_LinkClicked);
            // 
            // txtDimensionsLeft
            // 
            this.txtDimensionsLeft.AutoSize = true;
            this.txtDimensionsLeft.Location = new System.Drawing.Point(10, 83);
            this.txtDimensionsLeft.Name = "txtDimensionsLeft";
            this.txtDimensionsLeft.Size = new System.Drawing.Size(35, 13);
            this.txtDimensionsLeft.TabIndex = 6;
            this.txtDimensionsLeft.Text = "label2";
            // 
            // txtPathLeft
            // 
            this.txtPathLeft.AutoSize = true;
            this.txtPathLeft.Location = new System.Drawing.Point(10, 100);
            this.txtPathLeft.Name = "txtPathLeft";
            this.txtPathLeft.Size = new System.Drawing.Size(35, 13);
            this.txtPathLeft.TabIndex = 7;
            this.txtPathLeft.Text = "label3";
            // 
            // txtPathRight
            // 
            this.txtPathRight.AutoSize = true;
            this.txtPathRight.Location = new System.Drawing.Point(10, 229);
            this.txtPathRight.Name = "txtPathRight";
            this.txtPathRight.Size = new System.Drawing.Size(35, 13);
            this.txtPathRight.TabIndex = 11;
            this.txtPathRight.Text = "label4";
            // 
            // lblDimensionsRight
            // 
            this.lblDimensionsRight.AutoSize = true;
            this.lblDimensionsRight.Location = new System.Drawing.Point(10, 212);
            this.lblDimensionsRight.Name = "lblDimensionsRight";
            this.lblDimensionsRight.Size = new System.Drawing.Size(35, 13);
            this.lblDimensionsRight.TabIndex = 10;
            this.lblDimensionsRight.Text = "label5";
            // 
            // lnkOpenRightFolder
            // 
            this.lnkOpenRightFolder.AutoSize = true;
            this.lnkOpenRightFolder.Location = new System.Drawing.Point(10, 242);
            this.lnkOpenRightFolder.Name = "lnkOpenRightFolder";
            this.lnkOpenRightFolder.Size = new System.Drawing.Size(118, 13);
            this.lnkOpenRightFolder.TabIndex = 9;
            this.lnkOpenRightFolder.TabStop = true;
            this.lnkOpenRightFolder.Text = "Open Containing Folder";
            this.lnkOpenRightFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenRightFolder_LinkClicked);
            // 
            // lblLengthRight
            // 
            this.lblLengthRight.AutoSize = true;
            this.lblLengthRight.Location = new System.Drawing.Point(10, 195);
            this.lblLengthRight.Name = "lblLengthRight";
            this.lblLengthRight.Size = new System.Drawing.Size(35, 13);
            this.lblLengthRight.TabIndex = 8;
            this.lblLengthRight.Text = "label6";
            // 
            // lblSizeRight
            // 
            this.lblSizeRight.AutoSize = true;
            this.lblSizeRight.Location = new System.Drawing.Point(11, 179);
            this.lblSizeRight.Name = "lblSizeRight";
            this.lblSizeRight.Size = new System.Drawing.Size(35, 13);
            this.lblSizeRight.TabIndex = 15;
            this.lblSizeRight.Text = "label7";
            // 
            // lblNameRight
            // 
            this.lblNameRight.AutoSize = true;
            this.lblNameRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNameRight.Location = new System.Drawing.Point(11, 162);
            this.lblNameRight.Name = "lblNameRight";
            this.lblNameRight.Size = new System.Drawing.Size(0, 13);
            this.lblNameRight.TabIndex = 14;
            // 
            // txtSizeLeft
            // 
            this.txtSizeLeft.AutoSize = true;
            this.txtSizeLeft.Location = new System.Drawing.Point(11, 50);
            this.txtSizeLeft.Name = "txtSizeLeft";
            this.txtSizeLeft.Size = new System.Drawing.Size(35, 13);
            this.txtSizeLeft.TabIndex = 13;
            this.txtSizeLeft.Text = "label9";
            // 
            // txtNameLeft
            // 
            this.txtNameLeft.AutoSize = true;
            this.txtNameLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNameLeft.Location = new System.Drawing.Point(11, 33);
            this.txtNameLeft.Name = "txtNameLeft";
            this.txtNameLeft.Size = new System.Drawing.Size(0, 13);
            this.txtNameLeft.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(11, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(203, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Which file would you like to keep?";
            // 
            // ChooseFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Ignore;
            this.ClientSize = new System.Drawing.Size(390, 306);
            this.ControlBox = false;
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblSizeRight);
            this.Controls.Add(this.lblNameRight);
            this.Controls.Add(this.txtSizeLeft);
            this.Controls.Add(this.txtNameLeft);
            this.Controls.Add(this.txtPathRight);
            this.Controls.Add(this.lblDimensionsRight);
            this.Controls.Add(this.lnkOpenRightFolder);
            this.Controls.Add(this.lblLengthRight);
            this.Controls.Add(this.txtPathLeft);
            this.Controls.Add(this.txtDimensionsLeft);
            this.Controls.Add(this.lnkOpenLeftFolder);
            this.Controls.Add(this.txtLengthLeft);
            this.Controls.Add(this.btnKeepRight);
            this.Controls.Add(this.Ignore);
            this.Controls.Add(this.btnLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseFile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose File";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button Ignore;
        private System.Windows.Forms.Button btnKeepRight;
        private System.Windows.Forms.Label txtLengthLeft;
        private System.Windows.Forms.LinkLabel lnkOpenLeftFolder;
        private System.Windows.Forms.Label txtDimensionsLeft;
        private System.Windows.Forms.Label txtPathLeft;
        private System.Windows.Forms.Label txtPathRight;
        private System.Windows.Forms.Label lblDimensionsRight;
        private System.Windows.Forms.LinkLabel lnkOpenRightFolder;
        private System.Windows.Forms.Label lblLengthRight;
        private System.Windows.Forms.Label lblSizeRight;
        private System.Windows.Forms.Label lblNameRight;
        private System.Windows.Forms.Label txtSizeLeft;
        private System.Windows.Forms.Label txtNameLeft;
        private System.Windows.Forms.Label label10;
    }
}