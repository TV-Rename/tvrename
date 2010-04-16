//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


namespace TVRename
{
    partial class IgnoreEdit
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
            this.components = (new System.ComponentModel.Container());
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(IgnoreEdit)));
            this.bnOK = (new System.Windows.Forms.Button());
            this.bnCancel = (new System.Windows.Forms.Button());
            this.lbItems = (new System.Windows.Forms.ListBox());
            this.bnRemove = (new System.Windows.Forms.Button());
            this.label1 = (new System.Windows.Forms.Label());
            this.txtFilter = (new System.Windows.Forms.TextBox());
            this.timer1 = (new System.Windows.Forms.Timer(this.components));
            this.SuspendLayout();
            // 
            // bnOK
            // 
            this.bnOK.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnOK.Location = new System.Drawing.Point(216, 425);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 0;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(297, 425);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 1;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // lbItems
            // 
            this.lbItems.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.lbItems.FormattingEnabled = true;
            this.lbItems.IntegralHeight = false;
            this.lbItems.Location = new System.Drawing.Point(9, 38);
            this.lbItems.Name = "lbItems";
            this.lbItems.ScrollAlwaysVisible = true;
            this.lbItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbItems.Size = new System.Drawing.Size(363, 381);
            this.lbItems.Sorted = true;
            this.lbItems.TabIndex = 2;
            // 
            // bnRemove
            // 
            this.bnRemove.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
            this.bnRemove.Location = new System.Drawing.Point(9, 425);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(75, 23);
            this.bnRemove.TabIndex = 0;
            this.bnRemove.Text = "&Remove";
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(bnRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filter:";
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.txtFilter.Location = new System.Drawing.Point(44, 12);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(328, 20);
            this.txtFilter.TabIndex = 4;
            this.txtFilter.TextChanged += new System.EventHandler(txtFilter_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(timer1_Tick);
            // 
            // IgnoreEdit
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(384, 460);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbItems);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnRemove);
            this.Controls.Add(this.bnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IgnoreEdit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Ignore List";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.ListBox lbItems;

        private System.Windows.Forms.Button bnRemove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Timer timer1;
    }
}