//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.Forms
{
    partial class DuplicateMovieFinder
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DuplicateMovieFinder));
            btnClose = new Button();
            label1 = new Label();
            rightClickMenu = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            lblStatus = new Label();
            pbProgress = new ProgressBar();
            btnRefresh = new Button();
            olvDuplicates = new BrightIdeasSoftware.ObjectListView();
            olvName = new BrightIdeasSoftware.OLVColumn();
            olvFiles = new BrightIdeasSoftware.OLVColumn();
            olvNumber = new BrightIdeasSoftware.OLVColumn();
            olvDoublePart = new BrightIdeasSoftware.OLVColumn();
            olvSample = new BrightIdeasSoftware.OLVColumn();
            olvDeleted = new BrightIdeasSoftware.OLVColumn();
            rightClickMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)olvDuplicates).BeginInit();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.Cancel;
            btnClose.Location = new Point(1079, 766);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 27);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 15);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(107, 15);
            label1.TabIndex = 7;
            label1.Text = "Checks Performed:";
            // 
            // rightClickMenu
            // 
            rightClickMenu.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
            rightClickMenu.Name = "menuSearchSites";
            rightClickMenu.ShowImageMargin = false;
            rightClickMenu.Size = new Size(156, 26);
            rightClickMenu.ItemClicked += rightClickMenu_ItemClicked;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(155, 22);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(131, 772);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 11;
            lblStatus.Visible = false;
            // 
            // pbProgress
            // 
            pbProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbProgress.Location = new Point(7, 766);
            pbProgress.Margin = new Padding(4, 3, 4, 3);
            pbProgress.Name = "pbProgress";
            pbProgress.Size = new Size(117, 27);
            pbProgress.TabIndex = 10;
            pbProgress.Visible = false;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRefresh.Location = new Point(7, 766);
            btnRefresh.Margin = new Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(88, 27);
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += BtnRefresh_Click_1;
            // 
            // olvDuplicates
            // 
            olvDuplicates.AllColumns.Add(olvName);
            olvDuplicates.AllColumns.Add(olvFiles);
            olvDuplicates.AllColumns.Add(olvNumber);
            olvDuplicates.AllColumns.Add(olvDoublePart);
            olvDuplicates.AllColumns.Add(olvSample);
            olvDuplicates.AllColumns.Add(olvDeleted);
            olvDuplicates.AllowColumnReorder = true;
            olvDuplicates.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            olvDuplicates.CellEditUseWholeCell = false;
            olvDuplicates.Columns.AddRange(new ColumnHeader[] { olvName, olvFiles, olvNumber, olvDoublePart, olvSample, olvDeleted });
            olvDuplicates.ContextMenuStrip = rightClickMenu;
            olvDuplicates.FullRowSelect = true;
            olvDuplicates.IncludeColumnHeadersInCopy = true;
            olvDuplicates.Location = new Point(7, 33);
            olvDuplicates.Margin = new Padding(4, 3, 4, 3);
            olvDuplicates.MultiSelect = false;
            olvDuplicates.Name = "olvDuplicates";
            olvDuplicates.ShowCommandMenuOnRightClick = true;
            olvDuplicates.ShowItemCountOnGroups = true;
            olvDuplicates.Size = new Size(1159, 725);
            olvDuplicates.TabIndex = 12;
            olvDuplicates.UseCompatibleStateImageBehavior = false;
            olvDuplicates.UseFilterIndicator = true;
            olvDuplicates.UseFiltering = true;
            olvDuplicates.View = View.Details;
            olvDuplicates.CellRightClick += olvDuplicates_CellRightClick;
            // 
            // olvName
            // 
            olvName.AspectName = "Name";
            olvName.Text = "Movie";
            olvName.UseInitialLetterForGroup = true;
            // 
            // olvFiles
            // 
            olvFiles.AspectName = "Filenames";
            olvFiles.Groupable = false;
            olvFiles.Text = "Filenames";
            olvFiles.Width = 594;
            // 
            // olvNumber
            // 
            olvNumber.AspectName = "NumberOfFiles";
            olvNumber.Text = "# files";
            // 
            // olvDoublePart
            // 
            olvDoublePart.AspectName = "IsDoublePart";
            olvDoublePart.Text = "MultiPart Movie";
            olvDoublePart.Width = 88;
            // 
            // olvSample
            // 
            olvSample.AspectName = "IsSample";
            olvSample.Text = "Sample?";
            // 
            // olvDeleted
            // 
            olvDeleted.AspectName = "IsDeleted";
            olvDeleted.Text = "Deleted?";
            // 
            // DuplicateMovieFinder
            // 
            AcceptButton = btnClose;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnClose;
            ClientSize = new Size(1181, 796);
            Controls.Add(olvDuplicates);
            Controls.Add(lblStatus);
            Controls.Add(pbProgress);
            Controls.Add(btnRefresh);
            Controls.Add(label1);
            Controls.Add(btnClose);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimizeBox = false;
            MinimumSize = new Size(697, 456);
            Name = "DuplicateMovieFinder";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Duplicate Movies";
            rightClickMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)olvDuplicates).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnRefresh;
        private BrightIdeasSoftware.ObjectListView olvDuplicates;
        private BrightIdeasSoftware.OLVColumn olvName;
        private BrightIdeasSoftware.OLVColumn olvFiles;
        private BrightIdeasSoftware.OLVColumn olvDoublePart;
        private BrightIdeasSoftware.OLVColumn olvSample;
        private BrightIdeasSoftware.OLVColumn olvDeleted;
        private BrightIdeasSoftware.OLVColumn olvNumber;
    }
}
