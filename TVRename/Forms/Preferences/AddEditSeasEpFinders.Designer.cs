//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


using System;
using SourceGrid;

namespace TVRename
{
    partial class AddEditSeasEpFinders
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditSeasEpFinders));
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnDelete = new System.Windows.Forms.Button();
            this.bnAdd = new System.Windows.Forms.Button();
            this.lvPreview = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bnBrowse = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.cbShowList = new System.Windows.Forms.ComboBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.tmrFillPreview = new System.Windows.Forms.Timer(this.components);
            this.chkTestAll = new System.Windows.Forms.CheckBox();
            this.bnDefaults = new System.Windows.Forms.Button();
            this.Grid1 = new SourceGrid.Grid();
            this.bnDown = new System.Windows.Forms.Button();
            this.bnUp = new System.Windows.Forms.Button();
            this.rdoFileSystem = new System.Windows.Forms.RadioButton();
            this.rdoTorrentQueue = new System.Windows.Forms.RadioButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(895, 693);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 6;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(976, 693);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 7;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnDelete
            // 
            this.bnDelete.Location = new System.Drawing.Point(101, 0);
            this.bnDelete.Name = "bnDelete";
            this.bnDelete.Size = new System.Drawing.Size(75, 23);
            this.bnDelete.TabIndex = 5;
            this.bnDelete.Text = "&Delete";
            this.bnDelete.UseVisualStyleBackColor = true;
            this.bnDelete.Click += new System.EventHandler(this.bnDelete_Click);
            // 
            // bnAdd
            // 
            this.bnAdd.Location = new System.Drawing.Point(20, 0);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(75, 23);
            this.bnAdd.TabIndex = 4;
            this.bnAdd.Text = "&Add";
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
            // 
            // lvPreview
            // 
            this.lvPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvPreview.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvPreview.HideSelection = false;
            this.lvPreview.Location = new System.Drawing.Point(0, 61);
            this.lvPreview.Name = "lvPreview";
            this.lvPreview.Size = new System.Drawing.Size(1039, 338);
            this.lvPreview.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvPreview.TabIndex = 8;
            this.lvPreview.UseCompatibleStateImageBehavior = false;
            this.lvPreview.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Filename";
            this.columnHeader1.Width = 335;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Show(s)";
            this.columnHeader5.Width = 175;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Season";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Episode";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Rule";
            this.columnHeader4.Width = 250;
            // 
            // bnBrowse
            // 
            this.bnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowse.Location = new System.Drawing.Point(729, 32);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(75, 23);
            this.bnBrowse.TabIndex = 9;
            this.bnBrowse.Text = "&Browse...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(810, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Show:";
            // 
            // cbShowList
            // 
            this.cbShowList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbShowList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShowList.FormattingEnabled = true;
            this.cbShowList.Location = new System.Drawing.Point(853, 33);
            this.cbShowList.Name = "cbShowList";
            this.cbShowList.Size = new System.Drawing.Size(182, 21);
            this.cbShowList.TabIndex = 11;
            this.cbShowList.SelectedIndexChanged += new System.EventHandler(this.cbShowList_SelectedIndexChanged);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(399, 33);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(324, 20);
            this.txtFolder.TabIndex = 12;
            this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
            // 
            // tmrFillPreview
            // 
            this.tmrFillPreview.Interval = 500;
            this.tmrFillPreview.Tick += new System.EventHandler(this.tmrFillPreview_Tick);
            // 
            // chkTestAll
            // 
            this.chkTestAll.AutoSize = true;
            this.chkTestAll.Checked = true;
            this.chkTestAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTestAll.Location = new System.Drawing.Point(20, 38);
            this.chkTestAll.Name = "chkTestAll";
            this.chkTestAll.Size = new System.Drawing.Size(61, 17);
            this.chkTestAll.TabIndex = 13;
            this.chkTestAll.Text = "Test All";
            this.chkTestAll.UseVisualStyleBackColor = true;
            this.chkTestAll.CheckedChanged += new System.EventHandler(this.chkTestAll_CheckedChanged);
            // 
            // bnDefaults
            // 
            this.bnDefaults.Location = new System.Drawing.Point(182, 0);
            this.bnDefaults.Name = "bnDefaults";
            this.bnDefaults.Size = new System.Drawing.Size(75, 23);
            this.bnDefaults.TabIndex = 14;
            this.bnDefaults.Text = "D&efaults";
            this.bnDefaults.UseVisualStyleBackColor = true;
            this.bnDefaults.Click += new System.EventHandler(this.bnDefaults_Click);
            // 
            // Grid1
            // 
            this.Grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Grid1.BackColor = System.Drawing.SystemColors.Window;
            this.Grid1.EnableSort = true;
            this.Grid1.Location = new System.Drawing.Point(3, 10);
            this.Grid1.Name = "Grid1";
            this.Grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.Grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.Grid1.Size = new System.Drawing.Size(1036, 266);
            this.Grid1.TabIndex = 15;
            this.Grid1.TabStop = true;
            this.Grid1.ToolTipText = "";
            // 
            // bnDown
            // 
            this.bnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnDown.Location = new System.Drawing.Point(960, 0);
            this.bnDown.Name = "bnDown";
            this.bnDown.Size = new System.Drawing.Size(75, 23);
            this.bnDown.TabIndex = 17;
            this.bnDown.Text = "Move &Down";
            this.bnDown.UseVisualStyleBackColor = true;
            this.bnDown.Click += new System.EventHandler(this.bnDown_Click);
            // 
            // bnUp
            // 
            this.bnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnUp.Location = new System.Drawing.Point(879, 0);
            this.bnUp.Name = "bnUp";
            this.bnUp.Size = new System.Drawing.Size(75, 23);
            this.bnUp.TabIndex = 16;
            this.bnUp.Text = "Move &Up";
            this.bnUp.UseVisualStyleBackColor = true;
            this.bnUp.Click += new System.EventHandler(this.bnUp_Click);
            // 
            // rdoFileSystem
            // 
            this.rdoFileSystem.AutoSize = true;
            this.rdoFileSystem.Checked = true;
            this.rdoFileSystem.Location = new System.Drawing.Point(316, 35);
            this.rdoFileSystem.Name = "rdoFileSystem";
            this.rdoFileSystem.Size = new System.Drawing.Size(81, 17);
            this.rdoFileSystem.TabIndex = 18;
            this.rdoFileSystem.TabStop = true;
            this.rdoFileSystem.Text = "Test Folder:";
            this.rdoFileSystem.UseVisualStyleBackColor = true;
            // 
            // rdoTorrentQueue
            // 
            this.rdoTorrentQueue.AutoSize = true;
            this.rdoTorrentQueue.Location = new System.Drawing.Point(219, 37);
            this.rdoTorrentQueue.Name = "rdoTorrentQueue";
            this.rdoTorrentQueue.Size = new System.Drawing.Size(94, 17);
            this.rdoTorrentQueue.TabIndex = 19;
            this.rdoTorrentQueue.Text = "Torrent Queue";
            this.rdoTorrentQueue.UseVisualStyleBackColor = true;
            this.rdoTorrentQueue.CheckedChanged += new System.EventHandler(this.RadioButton2_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.Grid1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvPreview);
            this.splitContainer1.Panel2.Controls.Add(this.rdoTorrentQueue);
            this.splitContainer1.Panel2.Controls.Add(this.bnAdd);
            this.splitContainer1.Panel2.Controls.Add(this.rdoFileSystem);
            this.splitContainer1.Panel2.Controls.Add(this.bnDelete);
            this.splitContainer1.Panel2.Controls.Add(this.bnDown);
            this.splitContainer1.Panel2.Controls.Add(this.bnBrowse);
            this.splitContainer1.Panel2.Controls.Add(this.bnUp);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.bnDefaults);
            this.splitContainer1.Panel2.Controls.Add(this.cbShowList);
            this.splitContainer1.Panel2.Controls.Add(this.chkTestAll);
            this.splitContainer1.Panel2.Controls.Add(this.txtFolder);
            this.splitContainer1.Size = new System.Drawing.Size(1047, 685);
            this.splitContainer1.SplitterDistance = 279;
            this.splitContainer1.TabIndex = 20;
            // 
            // AddEditSeasEpFinders
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(1063, 728);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(917, 430);
            this.Name = "AddEditSeasEpFinders";
            this.ShowInTaskbar = false;
            this.Text = "Filename Processors";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Timer tmrFillPreview;

        private System.Windows.Forms.CheckBox chkTestAll;
        private System.Windows.Forms.Button bnDefaults;
        private SourceGrid.Grid Grid1;

        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnDelete;
        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.ListView lvPreview;
        private System.Windows.Forms.Button bnBrowse;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbShowList;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button bnDown;
        private System.Windows.Forms.Button bnUp;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.RadioButton rdoFileSystem;
        private System.Windows.Forms.RadioButton rdoTorrentQueue;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
