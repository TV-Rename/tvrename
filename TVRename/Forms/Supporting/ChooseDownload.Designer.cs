namespace TVRename.Forms.Supporting
{
    partial class ChooseDownload
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bnCancelAll = new System.Windows.Forms.Button();
            this.olvChooseDownload = new TVRename.ObjectListViewFlickerFree();
            this.olvName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSize = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSeeders = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvSource = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblEpisodeName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvChooseDownload)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.olvChooseDownload, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(511, 546);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.bnCancelAll);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 514);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(505, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(427, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(346, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // bnCancelAll
            // 
            this.bnCancelAll.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancelAll.Location = new System.Drawing.Point(265, 3);
            this.bnCancelAll.Name = "bnCancelAll";
            this.bnCancelAll.Size = new System.Drawing.Size(75, 23);
            this.bnCancelAll.TabIndex = 4;
            this.bnCancelAll.Text = "Cancel All";
            this.bnCancelAll.UseVisualStyleBackColor = true;
            this.bnCancelAll.Click += new System.EventHandler(this.bnCancelAll_Click);
            // 
            // olvChooseDownload
            // 
            this.olvChooseDownload.AllColumns.Add(this.olvName);
            this.olvChooseDownload.AllColumns.Add(this.olvSize);
            this.olvChooseDownload.AllColumns.Add(this.olvSeeders);
            this.olvChooseDownload.AllColumns.Add(this.olvSource);
            this.olvChooseDownload.AllowColumnReorder = true;
            this.olvChooseDownload.CellEditUseWholeCell = false;
            this.olvChooseDownload.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvName,
            this.olvSize,
            this.olvSeeders,
            this.olvSource});
            this.olvChooseDownload.Cursor = System.Windows.Forms.Cursors.Default;
            this.olvChooseDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvChooseDownload.FullRowSelect = true;
            this.olvChooseDownload.HasCollapsibleGroups = false;
            this.olvChooseDownload.HideSelection = false;
            this.olvChooseDownload.Location = new System.Drawing.Point(3, 38);
            this.olvChooseDownload.MultiSelect = false;
            this.olvChooseDownload.Name = "olvChooseDownload";
            this.olvChooseDownload.ShowCommandMenuOnRightClick = true;
            this.olvChooseDownload.ShowGroups = false;
            this.olvChooseDownload.ShowItemToolTips = true;
            this.olvChooseDownload.Size = new System.Drawing.Size(505, 470);
            this.olvChooseDownload.SortGroupItemsByPrimaryColumn = false;
            this.olvChooseDownload.TabIndex = 1;
            this.olvChooseDownload.UseCompatibleStateImageBehavior = false;
            this.olvChooseDownload.UseFilterIndicator = true;
            this.olvChooseDownload.UseFiltering = true;
            this.olvChooseDownload.View = System.Windows.Forms.View.Details;
            this.olvChooseDownload.SelectedIndexChanged += new System.EventHandler(this.OlvChooseDownload_SelectedIndexChanged);
            this.olvChooseDownload.DoubleClick += new System.EventHandler(this.olvChooseDownload_DoubleClick);
            // 
            // olvName
            // 
            this.olvName.AspectName = "SourceName";
            this.olvName.Text = "Name";
            this.olvName.Width = 300;
            // 
            // olvSize
            // 
            this.olvSize.AspectName = "sizeBytes";
            this.olvSize.Text = "Size";
            // 
            // olvSeeders
            // 
            this.olvSeeders.AspectName = "Seeders";
            this.olvSeeders.Text = "Seeders";
            // 
            // olvSource
            // 
            this.olvSource.AspectName = "UpstreamSource";
            this.olvSource.Text = "Source";
            this.olvSource.Width = 400;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblEpisodeName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(505, 29);
            this.panel1.TabIndex = 2;
            // 
            // lblEpisodeName
            // 
            this.lblEpisodeName.AutoSize = true;
            this.lblEpisodeName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEpisodeName.Location = new System.Drawing.Point(103, 10);
            this.lblEpisodeName.Name = "lblEpisodeName";
            this.lblEpisodeName.Size = new System.Drawing.Size(88, 13);
            this.lblEpisodeName.TabIndex = 1;
            this.lblEpisodeName.Text = "Episode Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected Episode:";
            // 
            // ChooseDownload
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(511, 546);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseDownload";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Download";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvChooseDownload)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private ObjectListViewFlickerFree olvChooseDownload;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblEpisodeName;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.OLVColumn olvName;
        private BrightIdeasSoftware.OLVColumn olvSize;
        private BrightIdeasSoftware.OLVColumn olvSeeders;
        private BrightIdeasSoftware.OLVColumn olvSource;
        private System.Windows.Forms.Button bnCancelAll;
    }
}
