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
            this.showRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chkHideIgnored = new System.Windows.Forms.CheckBox();
            this.chkHideSpecials = new System.Windows.Forms.CheckBox();
            this.chkHideComplete = new System.Windows.Forms.CheckBox();
            this.chkHideUnaired = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // grid1
            // 
            this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid1.BackColor = System.Drawing.SystemColors.Window;
            this.grid1.EnableSort = true;
            this.grid1.Location = new System.Drawing.Point(0, 37);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(721, 534);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // showRightClickMenu
            // 
            this.showRightClickMenu.Name = "showRightClickMenu";
            this.showRightClickMenu.ShowImageMargin = false;
            this.showRightClickMenu.Size = new System.Drawing.Size(36, 4);
            this.showRightClickMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.showRightClickMenu_ItemClicked);
            // 
            // chkHideIgnored
            // 
            this.chkHideIgnored.AutoSize = true;
            this.chkHideIgnored.Location = new System.Drawing.Point(13, 13);
            this.chkHideIgnored.Name = "chkHideIgnored";
            this.chkHideIgnored.Size = new System.Drawing.Size(131, 17);
            this.chkHideIgnored.TabIndex = 1;
            this.chkHideIgnored.Text = "Hide Ignored Seasons";
            this.chkHideIgnored.UseVisualStyleBackColor = true;
            this.chkHideIgnored.CheckedChanged += new System.EventHandler(this.chkHideIgnored_CheckedChanged);
            // 
            // chkHideSpecials
            // 
            this.chkHideSpecials.AutoSize = true;
            this.chkHideSpecials.Location = new System.Drawing.Point(150, 12);
            this.chkHideSpecials.Name = "chkHideSpecials";
            this.chkHideSpecials.Size = new System.Drawing.Size(91, 17);
            this.chkHideSpecials.TabIndex = 2;
            this.chkHideSpecials.Text = "Hide Specials";
            this.chkHideSpecials.UseVisualStyleBackColor = true;
            this.chkHideSpecials.CheckedChanged += new System.EventHandler(this.chkHideSpecials_CheckedChanged);
            // 
            // chkHideComplete
            // 
            this.chkHideComplete.AutoSize = true;
            this.chkHideComplete.Location = new System.Drawing.Point(247, 12);
            this.chkHideComplete.Name = "chkHideComplete";
            this.chkHideComplete.Size = new System.Drawing.Size(130, 17);
            this.chkHideComplete.TabIndex = 3;
            this.chkHideComplete.Text = "Hide Complete Shows";
            this.chkHideComplete.UseVisualStyleBackColor = true;
            this.chkHideComplete.CheckedChanged += new System.EventHandler(this.chkHideComplete_CheckedChanged);
            // 
            // chkHideUnaired
            // 
            this.chkHideUnaired.AutoSize = true;
            this.chkHideUnaired.Location = new System.Drawing.Point(383, 12);
            this.chkHideUnaired.Name = "chkHideUnaired";
            this.chkHideUnaired.Size = new System.Drawing.Size(241, 17);
            this.chkHideUnaired.TabIndex = 4;
            this.chkHideUnaired.Text = "Hide Complete Shows With Unaired Episodes";
            this.chkHideUnaired.UseVisualStyleBackColor = true;
            this.chkHideUnaired.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(646, 9);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear Filters";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // ShowSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 573);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.chkHideUnaired);
            this.Controls.Add(this.chkHideComplete);
            this.Controls.Add(this.chkHideSpecials);
            this.Controls.Add(this.chkHideIgnored);
            this.Controls.Add(this.grid1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "ShowSummary";
            this.Text = "Show Summary";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SourceGrid.Grid grid1;
        private System.Windows.Forms.ContextMenuStrip showRightClickMenu;
        private System.Windows.Forms.CheckBox chkHideIgnored;
        private System.Windows.Forms.CheckBox chkHideSpecials;
        private System.Windows.Forms.CheckBox chkHideComplete;
        private System.Windows.Forms.CheckBox chkHideUnaired;
        private System.Windows.Forms.Button btnClear;
    }
}
