//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using SourceGrid;

namespace TVRename
{

    /// <summary>
    /// Summary for AddEditSearchEngine
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public class AddEditSearchEngine : System.Windows.Forms.Form
    {
        private Searchers mSearchers;
        private CustomNameTagsFloatingWindow Cntfw;
        private SourceGrid.Grid Grid1;
        //array<SourceGrid::Cells::Editors::EditorBase ^> ^MyEditors;

        private ProcessedEpisode SampleEpisode;

        public AddEditSearchEngine(Searchers s, ProcessedEpisode pe)
        {
            SampleEpisode = pe;
            InitializeComponent();
            Cntfw = null;

            SetupGrid();
            mSearchers = s;

            for (int i = 0; i < mSearchers.Count(); i++)
            {
                AddNewRow();
                Grid1[i + 1, 0].Value = mSearchers.Name(i);
                Grid1[i + 1, 1].Value = mSearchers.URL(i);
            }
        }

        public void SetupGrid()
        {
            SourceGrid.Cells.Views.Cell titleModel = new SourceGrid.Cells.Views.Cell();
            titleModel.BackColor = Color.SteelBlue;
            titleModel.ForeColor = Color.White;
            titleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

            Grid1.Columns.Clear();
            Grid1.Rows.Clear();

            Grid1.RowsCount = 1;
            Grid1.ColumnsCount = 2;
            Grid1.FixedRows = 1;
            Grid1.FixedColumns = 0;
            Grid1.Selection.EnableMultiSelection = false;

            Grid1.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid1.Columns[0].Width = 80;

            Grid1.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;

            Grid1.AutoStretchColumnsToFitWidth = true;
            //Grid1->AutoSizeCells();
            Grid1.Columns.StretchToFit();

            //////////////////////////////////////////////////////////////////////
            // header row

            SourceGrid.Cells.ColumnHeader h;
            h = new SourceGrid.Cells.ColumnHeader("Name");
            h.AutomaticSortEnabled = false;
            Grid1[0, 0] = h;
            Grid1[0, 0].View = titleModel;

            h = new SourceGrid.Cells.ColumnHeader("URL");
            h.AutomaticSortEnabled = false;
            Grid1[0, 1] = h;
            Grid1[0, 1].View = titleModel;
        }

        public void AddNewRow()
        {
            int r = Grid1.RowsCount;
            Grid1.RowsCount = r + 1;

            Grid1[r, 0] = new SourceGrid.Cells.Cell("", typeof(string));
            Grid1[r, 1] = new SourceGrid.Cells.Cell("", typeof(string));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        ~AddEditSearchEngine()
        {
            if (Cntfw != null)
                Cntfw.Close();
        }

        private System.Windows.Forms.Button bnAdd;
        private System.Windows.Forms.Button bnDelete;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnTags;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(AddEditSearchEngine)));
            this.bnAdd = (new System.Windows.Forms.Button());
            this.bnDelete = (new System.Windows.Forms.Button());
            this.bnCancel = (new System.Windows.Forms.Button());
            this.bnOK = (new System.Windows.Forms.Button());
            this.bnTags = (new System.Windows.Forms.Button());
            this.Grid1 = (new SourceGrid.Grid());
            this.SuspendLayout();
            // 
            // bnAdd
            // 
            this.bnAdd.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnAdd.Location = new System.Drawing.Point(12, 343);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(75, 23);
            this.bnAdd.TabIndex = 1;
            this.bnAdd.Text = "&Add";
            this.bnAdd.UseVisualStyleBackColor = true;
            this.bnAdd.Click += new System.EventHandler(bnAdd_Click);
            // 
            // bnDelete
            // 
            this.bnDelete.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnDelete.Location = new System.Drawing.Point(93, 343);
            this.bnDelete.Name = "bnDelete";
            this.bnDelete.Size = new System.Drawing.Size(75, 23);
            this.bnDelete.TabIndex = 1;
            this.bnDelete.Text = "&Delete";
            this.bnDelete.UseVisualStyleBackColor = true;
            this.bnDelete.Click += new System.EventHandler(bnDelete_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(601, 343);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 2;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(520, 343);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 2;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(bnOK_Click);
            // 
            // bnTags
            // 
            this.bnTags.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnTags.Location = new System.Drawing.Point(189, 343);
            this.bnTags.Name = "bnTags";
            this.bnTags.Size = new System.Drawing.Size(75, 23);
            this.bnTags.TabIndex = 3;
            this.bnTags.Text = "Tags...";
            this.bnTags.UseVisualStyleBackColor = true;
            this.bnTags.Click += new System.EventHandler(bnTags_Click);
            // 
            // Grid1
            // 
            this.Grid1.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.Grid1.BackColor = System.Drawing.SystemColors.Window;
            this.Grid1.Location = new System.Drawing.Point(12, 12);
            this.Grid1.Name = "Grid1";
            this.Grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.Grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.Grid1.Size = new System.Drawing.Size(664, 321);
            this.Grid1.TabIndex = 4;
            this.Grid1.TabStop = true;
            this.Grid1.ToolTipText = "";
            // 
            // AddEditSearchEngine
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(688, 378);
            this.Controls.Add(this.Grid1);
            this.Controls.Add(this.bnTags);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnDelete);
            this.Controls.Add(this.bnAdd);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddEditSearchEngine";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modify Search Engines";
            this.ResumeLayout(false);

        }
        #endregion
        private void bnAdd_Click(object sender, System.EventArgs e)
        {
            AddNewRow();
            Grid1.Selection.Focus(new SourceGrid.Position(Grid1.RowsCount - 1, 1), true);
        }
        private void bnDelete_Click(object sender, System.EventArgs e)
        {
            // multiselection is off, so we can cheat...
            int[] rowsIndex = Grid1.Selection.GetSelectionRegion().GetRowsIndex();
            if (rowsIndex.Length > 0)
                Grid1.Rows.Remove(rowsIndex[0]);
        }
        private void bnOK_Click(object sender, System.EventArgs e)
        {
            mSearchers.Clear();
            for (int i = 1; i < Grid1.RowsCount; i++) // skip header row
            {
                string name = (string)(Grid1[i, 0].Value);
                string url = (string)(Grid1[i, 1].Value);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    mSearchers.Add(name, url);
            }
        }
        private void bnTags_Click(object sender, System.EventArgs e)
        {
            Cntfw = new CustomNameTagsFloatingWindow(SampleEpisode);
            Cntfw.Show(this);
            this.Focus();
        }
    }
}


