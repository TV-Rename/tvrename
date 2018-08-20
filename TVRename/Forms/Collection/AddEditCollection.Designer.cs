namespace TVRename
{
    partial class AddEditCollection
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditCollection));
            this.TvColl = new System.Windows.Forms.TreeView();
            this.TxtCollPath = new System.Windows.Forms.TextBox();
            this.TxtCollName = new System.Windows.Forms.TextBox();
            this.TxtCollDesc = new System.Windows.Forms.TextBox();
            this.LblFolder = new System.Windows.Forms.Label();
            this.LblCollName = new System.Windows.Forms.Label();
            this.LblCollDesc = new System.Windows.Forms.Label();
            this.PnlCollAddUpdate = new System.Windows.Forms.Panel();
            this.BtCancel = new System.Windows.Forms.Button();
            this.BtSave = new System.Windows.Forms.Button();
            this.BtOk = new System.Windows.Forms.Button();
            this.BtUp = new System.Windows.Forms.Button();
            this.ImgList = new System.Windows.Forms.ImageList(this.components);
            this.BtEdit = new System.Windows.Forms.Button();
            this.BtDel = new System.Windows.Forms.Button();
            this.BtDown = new System.Windows.Forms.Button();
            this.BtAdd = new System.Windows.Forms.Button();
            this.PnlCollAddUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // TvColl
            // 
            this.TvColl.Location = new System.Drawing.Point(12, 12);
            this.TvColl.Name = "TvColl";
            this.TvColl.ShowNodeToolTips = true;
            this.TvColl.Size = new System.Drawing.Size(152, 142);
            this.TvColl.TabIndex = 0;
            this.TvColl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvColl_AfterSelect);
            // 
            // TxtCollPath
            // 
            this.TxtCollPath.Location = new System.Drawing.Point(76, 6);
            this.TxtCollPath.Name = "TxtCollPath";
            this.TxtCollPath.Size = new System.Drawing.Size(100, 20);
            this.TxtCollPath.TabIndex = 7;
            // 
            // TxtCollName
            // 
            this.TxtCollName.Location = new System.Drawing.Point(76, 32);
            this.TxtCollName.Name = "TxtCollName";
            this.TxtCollName.Size = new System.Drawing.Size(100, 20);
            this.TxtCollName.TabIndex = 9;
            // 
            // TxtCollDesc
            // 
            this.TxtCollDesc.Location = new System.Drawing.Point(76, 58);
            this.TxtCollDesc.Multiline = true;
            this.TxtCollDesc.Name = "TxtCollDesc";
            this.TxtCollDesc.Size = new System.Drawing.Size(158, 55);
            this.TxtCollDesc.TabIndex = 11;
            // 
            // LblFolder
            // 
            this.LblFolder.Location = new System.Drawing.Point(5, 6);
            this.LblFolder.Name = "LblFolder";
            this.LblFolder.Size = new System.Drawing.Size(65, 20);
            this.LblFolder.TabIndex = 6;
            this.LblFolder.Text = "Folder";
            this.LblFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblCollName
            // 
            this.LblCollName.Location = new System.Drawing.Point(5, 32);
            this.LblCollName.Name = "LblCollName";
            this.LblCollName.Size = new System.Drawing.Size(65, 20);
            this.LblCollName.TabIndex = 8;
            this.LblCollName.Text = "Name";
            this.LblCollName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblCollDesc
            // 
            this.LblCollDesc.Location = new System.Drawing.Point(5, 58);
            this.LblCollDesc.Name = "LblCollDesc";
            this.LblCollDesc.Size = new System.Drawing.Size(65, 20);
            this.LblCollDesc.TabIndex = 10;
            this.LblCollDesc.Text = "Description";
            this.LblCollDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PnlCollAddUpdate
            // 
            this.PnlCollAddUpdate.Controls.Add(this.BtCancel);
            this.PnlCollAddUpdate.Controls.Add(this.BtSave);
            this.PnlCollAddUpdate.Controls.Add(this.TxtCollDesc);
            this.PnlCollAddUpdate.Controls.Add(this.LblCollDesc);
            this.PnlCollAddUpdate.Controls.Add(this.TxtCollPath);
            this.PnlCollAddUpdate.Controls.Add(this.LblCollName);
            this.PnlCollAddUpdate.Controls.Add(this.TxtCollName);
            this.PnlCollAddUpdate.Controls.Add(this.LblFolder);
            this.PnlCollAddUpdate.Location = new System.Drawing.Point(200, 12);
            this.PnlCollAddUpdate.Name = "PnlCollAddUpdate";
            this.PnlCollAddUpdate.Size = new System.Drawing.Size(241, 145);
            this.PnlCollAddUpdate.TabIndex = 5;
            // 
            // BtCancel
            // 
            this.BtCancel.Location = new System.Drawing.Point(76, 119);
            this.BtCancel.Name = "BtCancel";
            this.BtCancel.Size = new System.Drawing.Size(75, 23);
            this.BtCancel.TabIndex = 12;
            this.BtCancel.Text = "Cancel";
            this.BtCancel.UseVisualStyleBackColor = true;
            this.BtCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // BtSave
            // 
            this.BtSave.Location = new System.Drawing.Point(159, 119);
            this.BtSave.Name = "BtSave";
            this.BtSave.Size = new System.Drawing.Size(75, 23);
            this.BtSave.TabIndex = 13;
            this.BtSave.Text = "Save";
            this.BtSave.UseVisualStyleBackColor = true;
            this.BtSave.Click += new System.EventHandler(this.BtSave_Click);
            // 
            // BtOk
            // 
            this.BtOk.Location = new System.Drawing.Point(366, 163);
            this.BtOk.Name = "BtOk";
            this.BtOk.Size = new System.Drawing.Size(75, 23);
            this.BtOk.TabIndex = 14;
            this.BtOk.Text = "Ok";
            this.BtOk.UseVisualStyleBackColor = true;
            this.BtOk.Click += new System.EventHandler(this.BtOk_Click);
            // 
            // BtUp
            // 
            this.BtUp.ImageIndex = 0;
            this.BtUp.ImageList = this.ImgList;
            this.BtUp.Location = new System.Drawing.Point(170, 11);
            this.BtUp.Name = "BtUp";
            this.BtUp.Size = new System.Drawing.Size(24, 24);
            this.BtUp.TabIndex = 1;
            this.BtUp.UseVisualStyleBackColor = true;
            this.BtUp.Click += new System.EventHandler(this.BtUp_Click);
            // 
            // ImgList
            // 
            this.ImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImgList.ImageStream")));
            this.ImgList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImgList.Images.SetKeyName(0, "UpArrow.png");
            this.ImgList.Images.SetKeyName(1, "DownArrow.png");
            this.ImgList.Images.SetKeyName(2, "AddIcon.png");
            this.ImgList.Images.SetKeyName(3, "RemoveIcon.png");
            this.ImgList.Images.SetKeyName(4, "EditIcon.png");
            // 
            // BtEdit
            // 
            this.BtEdit.ImageIndex = 4;
            this.BtEdit.ImageList = this.ImgList;
            this.BtEdit.Location = new System.Drawing.Point(170, 101);
            this.BtEdit.Name = "BtEdit";
            this.BtEdit.Size = new System.Drawing.Size(24, 24);
            this.BtEdit.TabIndex = 4;
            this.BtEdit.UseVisualStyleBackColor = true;
            this.BtEdit.Click += new System.EventHandler(this.BtEdit_Click);
            // 
            // BtDel
            // 
            this.BtDel.ImageIndex = 3;
            this.BtDel.ImageList = this.ImgList;
            this.BtDel.Location = new System.Drawing.Point(170, 131);
            this.BtDel.Name = "BtDel";
            this.BtDel.Size = new System.Drawing.Size(24, 24);
            this.BtDel.TabIndex = 5;
            this.BtDel.UseVisualStyleBackColor = true;
            this.BtDel.Click += new System.EventHandler(this.BtDel_Click);
            // 
            // BtDown
            // 
            this.BtDown.ImageIndex = 1;
            this.BtDown.ImageList = this.ImgList;
            this.BtDown.Location = new System.Drawing.Point(170, 41);
            this.BtDown.Name = "BtDown";
            this.BtDown.Size = new System.Drawing.Size(24, 24);
            this.BtDown.TabIndex = 2;
            this.BtDown.UseVisualStyleBackColor = true;
            this.BtDown.Click += new System.EventHandler(this.BtDown_Click);
            // 
            // BtAdd
            // 
            this.BtAdd.ImageIndex = 2;
            this.BtAdd.ImageList = this.ImgList;
            this.BtAdd.Location = new System.Drawing.Point(170, 71);
            this.BtAdd.Name = "BtAdd";
            this.BtAdd.Size = new System.Drawing.Size(24, 24);
            this.BtAdd.TabIndex = 3;
            this.BtAdd.UseVisualStyleBackColor = true;
            this.BtAdd.Click += new System.EventHandler(this.BtAdd_Click);
            // 
            // AddEditCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 198);
            this.Controls.Add(this.BtAdd);
            this.Controls.Add(this.BtDown);
            this.Controls.Add(this.BtDel);
            this.Controls.Add(this.BtEdit);
            this.Controls.Add(this.BtUp);
            this.Controls.Add(this.BtOk);
            this.Controls.Add(this.PnlCollAddUpdate);
            this.Controls.Add(this.TvColl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditCollection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add / Edit collections";
            this.PnlCollAddUpdate.ResumeLayout(false);
            this.PnlCollAddUpdate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TvColl;
        private System.Windows.Forms.TextBox TxtCollPath;
        private System.Windows.Forms.TextBox TxtCollName;
        private System.Windows.Forms.TextBox TxtCollDesc;
        private System.Windows.Forms.Label LblFolder;
        private System.Windows.Forms.Label LblCollName;
        private System.Windows.Forms.Label LblCollDesc;
        private System.Windows.Forms.Panel PnlCollAddUpdate;
        private System.Windows.Forms.Button BtSave;
        private System.Windows.Forms.Button BtOk;
        private System.Windows.Forms.Button BtCancel;
        private System.Windows.Forms.Button BtUp;
        private System.Windows.Forms.ImageList ImgList;
        private System.Windows.Forms.Button BtEdit;
        private System.Windows.Forms.Button BtDel;
        private System.Windows.Forms.Button BtDown;
        private System.Windows.Forms.Button BtAdd;
    }
}
