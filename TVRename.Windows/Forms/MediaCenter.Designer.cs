namespace TVRename.Windows.Forms
{
    partial class MediaCenter
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Per Show", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Per Season", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Per Episode", System.Windows.Forms.HorizontalAlignment.Left);
            this.buttonAdd = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonPresets = new System.Windows.Forms.Button();
            this.contextMenuStripPresets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemPresetKodi = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPresetMede8er = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPresetPyTivo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPresetPlex = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripAdd = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAddText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddImage = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripPresets.SuspendLayout();
            this.contextMenuStripAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAdd.Location = new System.Drawing.Point(93, 251);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader2});
            this.listView.FullRowSelect = true;
            listViewGroup1.Header = "Per Show";
            listViewGroup1.Name = "show";
            listViewGroup2.Header = "Per Season";
            listViewGroup2.Name = "season";
            listViewGroup3.Header = "Per Episode";
            listViewGroup3.Name = "episode";
            this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.listView.Location = new System.Drawing.Point(12, 12);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(560, 233);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 125;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Format";
            this.columnHeader3.Width = 85;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Location";
            this.columnHeader4.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "File";
            this.columnHeader2.Width = 170;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEdit,
            this.toolStripMenuItemRemove});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(118, 48);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            this.toolStripMenuItemEdit.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItemEdit.Text = "&Edit...";
            this.toolStripMenuItemEdit.Click += new System.EventHandler(this.toolStripMenuItemEdit_Click);
            // 
            // toolStripMenuItemRemove
            // 
            this.toolStripMenuItemRemove.Name = "toolStripMenuItemRemove";
            this.toolStripMenuItemRemove.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItemRemove.Text = "&Remove";
            this.toolStripMenuItemRemove.Click += new System.EventHandler(this.toolStripMenuItemRemove_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Location = new System.Drawing.Point(255, 251);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonPresets
            // 
            this.buttonPresets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPresets.Location = new System.Drawing.Point(12, 251);
            this.buttonPresets.Name = "buttonPresets";
            this.buttonPresets.Size = new System.Drawing.Size(75, 23);
            this.buttonPresets.TabIndex = 1;
            this.buttonPresets.Text = "&Presets...";
            this.buttonPresets.UseVisualStyleBackColor = true;
            this.buttonPresets.Click += new System.EventHandler(this.buttonPresets_Click);
            // 
            // contextMenuStripPresets
            // 
            this.contextMenuStripPresets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPresetKodi,
            this.toolStripMenuItemPresetPlex,
            this.toolStripMenuItemPresetMede8er,
            this.toolStripMenuItemPresetPyTivo});
            this.contextMenuStripPresets.Name = "contextMenuStripPresets";
            this.contextMenuStripPresets.ShowImageMargin = false;
            this.contextMenuStripPresets.Size = new System.Drawing.Size(128, 114);
            // 
            // toolStripMenuItemPresetKodi
            // 
            this.toolStripMenuItemPresetKodi.Name = "toolStripMenuItemPresetKodi";
            this.toolStripMenuItemPresetKodi.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItemPresetKodi.Text = "&Kodi";
            this.toolStripMenuItemPresetKodi.Click += new System.EventHandler(this.toolStripMenuItemPresetKodi_Click);
            // 
            // toolStripMenuItemPresetMede8er
            // 
            this.toolStripMenuItemPresetMede8er.Name = "toolStripMenuItemPresetMede8er";
            this.toolStripMenuItemPresetMede8er.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItemPresetMede8er.Text = "&Mede8er";
            this.toolStripMenuItemPresetMede8er.Click += new System.EventHandler(this.toolStripMenuItemPresetMede8er_Click);
            // 
            // toolStripMenuItemPresetPyTivo
            // 
            this.toolStripMenuItemPresetPyTivo.Name = "toolStripMenuItemPresetPyTivo";
            this.toolStripMenuItemPresetPyTivo.Size = new System.Drawing.Size(95, 22);
            this.toolStripMenuItemPresetPyTivo.Text = "py&Tivo";
            this.toolStripMenuItemPresetPyTivo.Click += new System.EventHandler(this.toolStripMenuItemPresetPyTivo_Click);
            // 
            // toolStripMenuItemPresetPlex
            // 
            this.toolStripMenuItemPresetPlex.Name = "toolStripMenuItemPresetPlex";
            this.toolStripMenuItemPresetPlex.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItemPresetPlex.Text = "&Plex";
            this.toolStripMenuItemPresetPlex.Click += new System.EventHandler(this.toolStripMenuItemPresetPlex_Click);
            // 
            // contextMenuStripAdd
            // 
            this.contextMenuStripAdd.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddText,
            this.toolStripMenuItemAddImage});
            this.contextMenuStripAdd.Name = "contextMenuStripPresets";
            this.contextMenuStripAdd.ShowImageMargin = false;
            this.contextMenuStripAdd.Size = new System.Drawing.Size(111, 48);
            // 
            // toolStripMenuItemAddText
            // 
            this.toolStripMenuItemAddText.Name = "toolStripMenuItemAddText";
            this.toolStripMenuItemAddText.Size = new System.Drawing.Size(110, 22);
            this.toolStripMenuItemAddText.Text = "&Text file..";
            this.toolStripMenuItemAddText.Click += new System.EventHandler(this.toolStripMenuItemAddText_Click);
            // 
            // toolStripMenuItemAddImage
            // 
            this.toolStripMenuItemAddImage.Name = "toolStripMenuItemAddImage";
            this.toolStripMenuItemAddImage.Size = new System.Drawing.Size(110, 22);
            this.toolStripMenuItemAddImage.Text = "&Image file...";
            this.toolStripMenuItemAddImage.Click += new System.EventHandler(this.toolStripMenuItemAddImage_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(416, 251);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "&OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(497, 251);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Enabled = false;
            this.buttonEdit.Location = new System.Drawing.Point(174, 251);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonEdit.TabIndex = 6;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // MediaCenter
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(584, 286);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonPresets);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.buttonAdd);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MediaCenter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Media Center Files";
            this.Load += new System.EventHandler(this.MediaCenter_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripPresets.ResumeLayout(false);
            this.contextMenuStripAdd.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button buttonPresets;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPresets;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPresetKodi;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPresetMede8er;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPresetPyTivo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAdd;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddImage;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRemove;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPresetPlex;
    }
}