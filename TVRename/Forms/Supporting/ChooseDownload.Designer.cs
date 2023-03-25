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
            this.olvName = new BrightIdeasSoftware.OLVColumn();
            this.olvSize = new BrightIdeasSoftware.OLVColumn();
            this.olvSeeders = new BrightIdeasSoftware.OLVColumn();
            this.olvSource = new BrightIdeasSoftware.OLVColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblEpisodeName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSeeAll = new System.Windows.Forms.Button();
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
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 630);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.bnCancelAll);
            this.flowLayoutPanel1.Controls.Add(this.btnSeeAll);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 593);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(588, 34);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(496, 3);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 27);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(400, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // bnCancelAll
            // 
            this.bnCancelAll.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancelAll.Location = new System.Drawing.Point(304, 3);
            this.bnCancelAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.bnCancelAll.Name = "bnCancelAll";
            this.bnCancelAll.Size = new System.Drawing.Size(88, 27);
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
            this.olvChooseDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.olvChooseDownload.FullRowSelect = true;
            this.olvChooseDownload.HasCollapsibleGroups = false;
            this.olvChooseDownload.Location = new System.Drawing.Point(4, 43);
            this.olvChooseDownload.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.olvChooseDownload.MultiSelect = false;
            this.olvChooseDownload.Name = "olvChooseDownload";
            this.olvChooseDownload.ShowCommandMenuOnRightClick = true;
            this.olvChooseDownload.ShowGroups = false;
            this.olvChooseDownload.ShowItemToolTips = true;
            this.olvChooseDownload.Size = new System.Drawing.Size(588, 544);
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
            this.olvName.Width = 350;
            // 
            // olvSize
            // 
            this.olvSize.AspectName = "sizeBytes";
            this.olvSize.Text = "Size";
            this.olvSize.Width = 70;
            // 
            // olvSeeders
            // 
            this.olvSeeders.AspectName = "Seeders";
            this.olvSeeders.Text = "Seeders";
            this.olvSeeders.Width = 70;
            // 
            // olvSource
            // 
            this.olvSource.AspectName = "UpstreamSource";
            this.olvSource.Text = "Source";
            this.olvSource.Width = 467;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblEpisodeName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 3);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(588, 34);
            this.panel1.TabIndex = 2;
            // 
            // lblEpisodeName
            // 
            this.lblEpisodeName.AutoSize = true;
            this.lblEpisodeName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEpisodeName.Location = new System.Drawing.Point(120, 12);
            this.lblEpisodeName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEpisodeName.Name = "lblEpisodeName";
            this.lblEpisodeName.Size = new System.Drawing.Size(88, 13);
            this.lblEpisodeName.TabIndex = 1;
            this.lblEpisodeName.Text = "Episode Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected Episode:";
            // 
            // btnSeeAll
            // 
            this.btnSeeAll.Location = new System.Drawing.Point(208, 3);
            this.btnSeeAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSeeAll.Name = "btnSeeAll";
            this.btnSeeAll.Size = new System.Drawing.Size(88, 27);
            this.btnSeeAll.TabIndex = 5;
            this.btnSeeAll.Text = "See All";
            this.btnSeeAll.UseVisualStyleBackColor = true;
            this.btnSeeAll.Click += new System.EventHandler(this.btnSeeAll_Click);
            // 
            // ChooseDownload
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(596, 630);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
        private System.Windows.Forms.Button btnSeeAll;
    }
}
