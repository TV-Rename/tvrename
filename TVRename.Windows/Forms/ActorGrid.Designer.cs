namespace TVRename.Windows.Forms
{
    partial class ActorGrid
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
            this.grid = new SourceGrid.Grid();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.BackColor = System.Drawing.Color.White;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grid.EnableSort = true;
            this.grid.FixedColumns = 1;
            this.grid.FixedRows = 1;
            this.grid.Location = new System.Drawing.Point(12, 12);
            this.grid.Name = "grid";
            this.grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid.Size = new System.Drawing.Size(655, 373);
            this.grid.TabIndex = 1;
            this.grid.TabStop = true;
            this.grid.ToolTipText = "";
            // 
            // ActorGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 397);
            this.Controls.Add(this.grid);
            this.MinimizeBox = false;
            this.Name = "ActorGrid";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Actor Grid";
            this.Load += new System.EventHandler(this.ActorGrid_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SourceGrid.Grid grid;
    }
}
