namespace TVRename
{
    using System.Drawing;

    partial class ShowSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowSummary));
            this.grid1 = new SourceGrid.Grid();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chkHideIgnored = new System.Windows.Forms.CheckBox();
            this.chkHideSpecials = new System.Windows.Forms.CheckBox();
            this.chkHideComplete = new System.Windows.Forms.CheckBox();
            this.chkHideUnaired = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.chkHideNotScanned = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkHideDiskEps = new System.Windows.Forms.CheckBox();
            this.chkOnlyShow = new System.Windows.Forms.CheckBox();
            this.bwRescan = new System.ComponentModel.BackgroundWorker();
            this.cmbShowStatus = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid1
            // 
            this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid1.BackColor = System.Drawing.SystemColors.Window;
            this.grid1.EnableSort = true;
            this.grid1.Location = new System.Drawing.Point(4, 61);
            this.grid1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(1469, 655);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // showRightClickMenu
            // 
            this.rightClickMenu.Name = "rightClickMenu";
            this.rightClickMenu.ShowImageMargin = false;
            this.rightClickMenu.Size = new System.Drawing.Size(36, 4);
            this.rightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.showRightClickMenu_ItemClicked);
            // 
            // chkHideIgnored
            // 
            this.chkHideIgnored.AutoSize = true;
            this.chkHideIgnored.Location = new System.Drawing.Point(4, 3);
            this.chkHideIgnored.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkHideIgnored.Name = "chkHideIgnored";
            this.chkHideIgnored.Size = new System.Drawing.Size(140, 19);
            this.chkHideIgnored.TabIndex = 1;
            this.chkHideIgnored.Text = "Hide Ignored Seasons";
            this.chkHideIgnored.UseVisualStyleBackColor = true;
            this.chkHideIgnored.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkHideSpecials
            // 
            this.chkHideSpecials.AutoSize = true;
            this.chkHideSpecials.Location = new System.Drawing.Point(163, 2);
            this.chkHideSpecials.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkHideSpecials.Name = "chkHideSpecials";
            this.chkHideSpecials.Size = new System.Drawing.Size(96, 19);
            this.chkHideSpecials.TabIndex = 2;
            this.chkHideSpecials.Text = "Hide Specials";
            this.chkHideSpecials.UseVisualStyleBackColor = true;
            this.chkHideSpecials.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkHideComplete
            // 
            this.chkHideComplete.AutoSize = true;
            this.chkHideComplete.Location = new System.Drawing.Point(276, 2);
            this.chkHideComplete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkHideComplete.Name = "chkHideComplete";
            this.chkHideComplete.Size = new System.Drawing.Size(143, 19);
            this.chkHideComplete.TabIndex = 3;
            this.chkHideComplete.Text = "Hide Complete Shows";
            this.chkHideComplete.UseVisualStyleBackColor = true;
            this.chkHideComplete.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkHideUnaired
            // 
            this.chkHideUnaired.AutoSize = true;
            this.chkHideUnaired.Location = new System.Drawing.Point(435, 2);
            this.chkHideUnaired.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkHideUnaired.Name = "chkHideUnaired";
            this.chkHideUnaired.Size = new System.Drawing.Size(264, 19);
            this.chkHideUnaired.TabIndex = 4;
            this.chkHideUnaired.Text = "Hide Complete Shows With Unaired Episodes";
            this.chkHideUnaired.UseVisualStyleBackColor = true;
            this.chkHideUnaired.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(1371, 3);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(88, 27);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear Filters";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkHideNotScanned
            // 
            this.chkHideNotScanned.AutoSize = true;
            this.chkHideNotScanned.Location = new System.Drawing.Point(723, 2);
            this.chkHideNotScanned.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkHideNotScanned.Name = "chkHideNotScanned";
            this.chkHideNotScanned.Size = new System.Drawing.Size(159, 19);
            this.chkHideNotScanned.TabIndex = 6;
            this.chkHideNotScanned.Text = "Hide Not Scanned Shows";
            this.chkHideNotScanned.UseVisualStyleBackColor = true;
            this.chkHideNotScanned.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.grid1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1477, 765);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.pbProgress);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 722);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1469, 40);
            this.panel1.TabIndex = 6;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(134, 9);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(10, 3);
            this.pbProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(117, 27);
            this.pbProgress.TabIndex = 6;
            this.pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(10, 3);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(88, 27);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(1378, 4);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 27);
            this.button1.TabIndex = 4;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmbShowStatus);
            this.panel2.Controls.Add(this.chkHideDiskEps);
            this.panel2.Controls.Add(this.chkOnlyShow);
            this.panel2.Controls.Add(this.chkHideIgnored);
            this.panel2.Controls.Add(this.chkHideSpecials);
            this.panel2.Controls.Add(this.chkHideNotScanned);
            this.panel2.Controls.Add(this.chkHideComplete);
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.chkHideUnaired);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 3);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1469, 52);
            this.panel2.TabIndex = 7;
            // 
            // chkHideDiskEps
            // 
            this.chkHideDiskEps.AutoSize = true;
            this.chkHideDiskEps.Location = new System.Drawing.Point(221, 27);
            this.chkHideDiskEps.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkHideDiskEps.Name = "chkHideDiskEps";
            this.chkHideDiskEps.Size = new System.Drawing.Size(188, 19);
            this.chkHideDiskEps.TabIndex = 8;
            this.chkHideDiskEps.Text = "Hide Shows with Disk Episodes";
            this.chkHideDiskEps.UseVisualStyleBackColor = true;
            this.chkHideDiskEps.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // chkOnlyShow
            // 
            this.chkOnlyShow.AutoSize = true;
            this.chkOnlyShow.Location = new System.Drawing.Point(4, 28);
            this.chkOnlyShow.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkOnlyShow.Name = "chkOnlyShow";
            this.chkOnlyShow.Size = new System.Drawing.Size(83, 19);
            this.chkOnlyShow.TabIndex = 7;
            this.chkOnlyShow.Text = "Only Show";
            this.chkOnlyShow.UseVisualStyleBackColor = true;
            this.chkOnlyShow.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // bwRescan
            // 
            this.bwRescan.WorkerReportsProgress = true;
            this.bwRescan.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwRescan_DoWork);
            this.bwRescan.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BwRescan_ProgressChanged);
            this.bwRescan.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BwRescan_RunWorkerCompleted);
            // 
            // cmbShowStatus
            // 
            this.cmbShowStatus.FormattingEnabled = true;
            this.cmbShowStatus.Location = new System.Drawing.Point(83, 24);
            this.cmbShowStatus.Name = "cmbShowStatus";
            this.cmbShowStatus.Size = new System.Drawing.Size(121, 23);
            this.cmbShowStatus.TabIndex = 9;
            this.cmbShowStatus.SelectedIndexChanged += new System.EventHandler(this.cmbShowStatus_SelectedIndexChanged);
            // 
            // ShowSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1477, 765);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimizeBox = false;
            this.Name = "ShowSummary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TV Show Summary";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SourceGrid.Grid grid1;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.CheckBox chkHideIgnored;
        private System.Windows.Forms.CheckBox chkHideSpecials;
        private System.Windows.Forms.CheckBox chkHideComplete;
        private System.Windows.Forms.CheckBox chkHideUnaired;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox chkHideNotScanned;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.ComponentModel.BackgroundWorker bwRescan;
        private System.Windows.Forms.CheckBox chkHideDiskEps;
        private System.Windows.Forms.CheckBox chkOnlyShow;
        private System.Windows.Forms.ComboBox cmbShowStatus;
    }
}
