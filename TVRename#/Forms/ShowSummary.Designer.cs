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
            this.grid1 = new SourceGrid.Grid();
            this.showRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // grid1
            // 
            this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid1.BackColor = System.Drawing.SystemColors.Window;
            this.grid1.Location = new System.Drawing.Point(0, 0);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(721, 571);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // showRightClickMenu
            // 
            this.showRightClickMenu.Name = "showRightClickMenu";
            this.showRightClickMenu.ShowImageMargin = false;
            this.showRightClickMenu.Size = new System.Drawing.Size(128, 26);
            this.showRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.showRightClickMenu_ItemClicked);
            // 
            // ShowSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 573);
            this.Controls.Add(this.grid1);
            this.Name = "ShowSummary";
            this.Text = "Show Summary";
            this.ResumeLayout(false);

        }

        #endregion

        private SourceGrid.Grid grid1;
        private System.Windows.Forms.ContextMenuStrip showRightClickMenu;
    }
}
