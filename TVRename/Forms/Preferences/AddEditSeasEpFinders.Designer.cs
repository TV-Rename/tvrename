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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditSeasEpFinders));
            bnOK = new System.Windows.Forms.Button();
            bnCancel = new System.Windows.Forms.Button();
            bnDelete = new System.Windows.Forms.Button();
            bnAdd = new System.Windows.Forms.Button();
            lvPreview = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader5 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            bnBrowse = new System.Windows.Forms.Button();
            folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            label1 = new System.Windows.Forms.Label();
            cbShowList = new System.Windows.Forms.ComboBox();
            txtFolder = new System.Windows.Forms.TextBox();
            tmrFillPreview = new System.Windows.Forms.Timer(components);
            chkTestAll = new System.Windows.Forms.CheckBox();
            bnDefaults = new System.Windows.Forms.Button();
            Grid1 = new Grid();
            bnDown = new System.Windows.Forms.Button();
            bnUp = new System.Windows.Forms.Button();
            rdoFileSystem = new System.Windows.Forms.RadioButton();
            rdoTorrentQueue = new System.Windows.Forms.RadioButton();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            txtTestString = new System.Windows.Forms.TextBox();
            rdoTextString = new System.Windows.Forms.RadioButton();
            label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // bnOK
            // 
            bnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            bnOK.Location = new System.Drawing.Point(1044, 800);
            bnOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnOK.Name = "bnOK";
            bnOK.Size = new System.Drawing.Size(88, 27);
            bnOK.TabIndex = 6;
            bnOK.Text = "OK";
            bnOK.UseVisualStyleBackColor = true;
            bnOK.Click += bnOK_Click;
            // 
            // bnCancel
            // 
            bnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            bnCancel.Location = new System.Drawing.Point(1139, 800);
            bnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnCancel.Name = "bnCancel";
            bnCancel.Size = new System.Drawing.Size(88, 27);
            bnCancel.TabIndex = 7;
            bnCancel.Text = "Cancel";
            bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnDelete
            // 
            bnDelete.Location = new System.Drawing.Point(118, 0);
            bnDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnDelete.Name = "bnDelete";
            bnDelete.Size = new System.Drawing.Size(88, 27);
            bnDelete.TabIndex = 5;
            bnDelete.Text = "&Delete";
            bnDelete.UseVisualStyleBackColor = true;
            bnDelete.Click += bnDelete_Click;
            // 
            // bnAdd
            // 
            bnAdd.Location = new System.Drawing.Point(23, 0);
            bnAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnAdd.Name = "bnAdd";
            bnAdd.Size = new System.Drawing.Size(88, 27);
            bnAdd.TabIndex = 4;
            bnAdd.Text = "&Add";
            bnAdd.UseVisualStyleBackColor = true;
            bnAdd.Click += bnAdd_Click;
            // 
            // lvPreview
            // 
            lvPreview.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvPreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader5, columnHeader2, columnHeader3, columnHeader4 });
            lvPreview.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            lvPreview.Location = new System.Drawing.Point(0, 98);
            lvPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lvPreview.Name = "lvPreview";
            lvPreview.Size = new System.Drawing.Size(1212, 362);
            lvPreview.Sorting = System.Windows.Forms.SortOrder.Ascending;
            lvPreview.TabIndex = 8;
            lvPreview.UseCompatibleStateImageBehavior = false;
            lvPreview.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Filename";
            columnHeader1.Width = 335;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Show(s)";
            columnHeader5.Width = 175;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Season";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Episode";
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Rule";
            columnHeader4.Width = 250;
            // 
            // bnBrowse
            // 
            bnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            bnBrowse.Location = new System.Drawing.Point(1125, 66);
            bnBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnBrowse.Name = "bnBrowse";
            bnBrowse.Size = new System.Drawing.Size(88, 27);
            bnBrowse.TabIndex = 9;
            bnBrowse.Text = "&Browse...";
            bnBrowse.UseVisualStyleBackColor = true;
            bnBrowse.Click += bnBrowse_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(115, 38);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 10;
            label1.Text = "Show:";
            // 
            // cbShowList
            // 
            cbShowList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cbShowList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbShowList.FormattingEnabled = true;
            cbShowList.Location = new System.Drawing.Point(166, 35);
            cbShowList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbShowList.Name = "cbShowList";
            cbShowList.Size = new System.Drawing.Size(410, 23);
            cbShowList.TabIndex = 11;
            cbShowList.SelectedIndexChanged += cbShowList_SelectedIndexChanged;
            // 
            // txtFolder
            // 
            txtFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtFolder.Location = new System.Drawing.Point(684, 68);
            txtFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new System.Drawing.Size(433, 23);
            txtFolder.TabIndex = 12;
            txtFolder.TextChanged += txtFolder_TextChanged;
            // 
            // tmrFillPreview
            // 
            tmrFillPreview.Interval = 500;
            tmrFillPreview.Tick += tmrFillPreview_Tick;
            // 
            // chkTestAll
            // 
            chkTestAll.AutoSize = true;
            chkTestAll.Checked = true;
            chkTestAll.CheckState = System.Windows.Forms.CheckState.Checked;
            chkTestAll.Location = new System.Drawing.Point(23, 37);
            chkTestAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            chkTestAll.Name = "chkTestAll";
            chkTestAll.Size = new System.Drawing.Size(64, 19);
            chkTestAll.TabIndex = 13;
            chkTestAll.Text = "Test All";
            chkTestAll.UseVisualStyleBackColor = true;
            chkTestAll.CheckedChanged += chkTestAll_CheckedChanged;
            // 
            // bnDefaults
            // 
            bnDefaults.Location = new System.Drawing.Point(212, 0);
            bnDefaults.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnDefaults.Name = "bnDefaults";
            bnDefaults.Size = new System.Drawing.Size(88, 27);
            bnDefaults.TabIndex = 14;
            bnDefaults.Text = "D&efaults";
            bnDefaults.UseVisualStyleBackColor = true;
            bnDefaults.Click += bnDefaults_Click;
            // 
            // Grid1
            // 
            Grid1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            Grid1.BackColor = System.Drawing.SystemColors.Window;
            Grid1.EnableSort = true;
            Grid1.Location = new System.Drawing.Point(4, 12);
            Grid1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Grid1.Name = "Grid1";
            Grid1.OptimizeMode = CellOptimizeMode.ForRows;
            Grid1.SelectionMode = GridSelectionMode.Cell;
            Grid1.Size = new System.Drawing.Size(1209, 306);
            Grid1.TabIndex = 15;
            Grid1.TabStop = true;
            Grid1.ToolTipText = "";
            // 
            // bnDown
            // 
            bnDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            bnDown.Location = new System.Drawing.Point(1120, 0);
            bnDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnDown.Name = "bnDown";
            bnDown.Size = new System.Drawing.Size(88, 27);
            bnDown.TabIndex = 17;
            bnDown.Text = "Move &Down";
            bnDown.UseVisualStyleBackColor = true;
            bnDown.Click += bnDown_Click;
            // 
            // bnUp
            // 
            bnUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            bnUp.Location = new System.Drawing.Point(1026, 0);
            bnUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bnUp.Name = "bnUp";
            bnUp.Size = new System.Drawing.Size(88, 27);
            bnUp.TabIndex = 16;
            bnUp.Text = "Move &Up";
            bnUp.UseVisualStyleBackColor = true;
            bnUp.Click += bnUp_Click;
            // 
            // rdoFileSystem
            // 
            rdoFileSystem.AutoSize = true;
            rdoFileSystem.Checked = true;
            rdoFileSystem.Location = new System.Drawing.Point(587, 70);
            rdoFileSystem.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rdoFileSystem.Name = "rdoFileSystem";
            rdoFileSystem.Size = new System.Drawing.Size(85, 19);
            rdoFileSystem.TabIndex = 18;
            rdoFileSystem.TabStop = true;
            rdoFileSystem.Text = "Test Folder:";
            rdoFileSystem.UseVisualStyleBackColor = true;
            rdoFileSystem.CheckedChanged += rdoFileSystem_CheckedChanged;
            // 
            // rdoTorrentQueue
            // 
            rdoTorrentQueue.AutoSize = true;
            rdoTorrentQueue.Location = new System.Drawing.Point(470, 70);
            rdoTorrentQueue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rdoTorrentQueue.Name = "rdoTorrentQueue";
            rdoTorrentQueue.Size = new System.Drawing.Size(101, 19);
            rdoTorrentQueue.TabIndex = 19;
            rdoTorrentQueue.Text = "Torrent Queue";
            rdoTorrentQueue.UseVisualStyleBackColor = true;
            rdoTorrentQueue.CheckedChanged += RadioButton2_CheckedChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            splitContainer1.Location = new System.Drawing.Point(14, 2);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(Grid1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(txtTestString);
            splitContainer1.Panel2.Controls.Add(rdoTextString);
            splitContainer1.Panel2.Controls.Add(lvPreview);
            splitContainer1.Panel2.Controls.Add(rdoTorrentQueue);
            splitContainer1.Panel2.Controls.Add(bnAdd);
            splitContainer1.Panel2.Controls.Add(rdoFileSystem);
            splitContainer1.Panel2.Controls.Add(bnDelete);
            splitContainer1.Panel2.Controls.Add(bnDown);
            splitContainer1.Panel2.Controls.Add(bnBrowse);
            splitContainer1.Panel2.Controls.Add(bnUp);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(bnDefaults);
            splitContainer1.Panel2.Controls.Add(cbShowList);
            splitContainer1.Panel2.Controls.Add(chkTestAll);
            splitContainer1.Panel2.Controls.Add(txtFolder);
            splitContainer1.Size = new System.Drawing.Size(1222, 790);
            splitContainer1.SplitterDistance = 321;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 20;
            // 
            // txtTestString
            // 
            txtTestString.Location = new System.Drawing.Point(84, 68);
            txtTestString.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtTestString.Name = "txtTestString";
            txtTestString.Size = new System.Drawing.Size(378, 23);
            txtTestString.TabIndex = 21;
            txtTestString.TextChanged += textBox1_TextChanged;
            // 
            // rdoTextString
            // 
            rdoTextString.AutoSize = true;
            rdoTextString.Location = new System.Drawing.Point(23, 68);
            rdoTextString.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rdoTextString.Name = "rdoTextString";
            rdoTextString.Size = new System.Drawing.Size(49, 19);
            rdoTextString.TabIndex = 20;
            rdoTextString.Text = "Text:";
            rdoTextString.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(308, 6);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(512, 15);
            label2.TabIndex = 22;
            label2.Text = "Valid Tags: <s> Season Number, <e> Episode Number, <f> Max Episode Number (for multipart)";
            // 
            // AddEditSeasEpFinders
            // 
            AcceptButton = bnOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = bnCancel;
            ClientSize = new System.Drawing.Size(1240, 840);
            Controls.Add(splitContainer1);
            Controls.Add(bnOK);
            Controls.Add(bnCancel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(1067, 490);
            Name = "AddEditSeasEpFinders";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Filename Processors";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);

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
        private System.Windows.Forms.TextBox txtTestString;
        private System.Windows.Forms.RadioButton rdoTextString;
        private System.Windows.Forms.Label label2;
    }
}
