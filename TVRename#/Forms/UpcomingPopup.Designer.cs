//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System.ComponentModel;
using System.Windows.Forms;

namespace TVRename
{
    partial class UpcomingPopup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(UpcomingPopup)));
            this.lvUpcoming = (new System.Windows.Forms.ListView());
            this.columnHeader1 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader2 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader3 = (new System.Windows.Forms.ColumnHeader());
            this.columnHeader4 = (new System.Windows.Forms.ColumnHeader());
            this.TimerOfDeath = (new System.Windows.Forms.Timer(this.components));
            this.hiddenButton = (new System.Windows.Forms.Button());
            this.SuspendLayout();
            // 
            // lvUpcoming
            // 
            this.lvUpcoming.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.columnHeader1, this.columnHeader2, this.columnHeader3, this.columnHeader4 });
            this.lvUpcoming.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvUpcoming.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvUpcoming.Location = new System.Drawing.Point(0, 0);
            this.lvUpcoming.MultiSelect = false;
            this.lvUpcoming.Name = "lvUpcoming";
            this.lvUpcoming.Size = new System.Drawing.Size(458, 90);
            this.lvUpcoming.TabIndex = 1;
            this.lvUpcoming.UseCompatibleStateImageBehavior = false;
            this.lvUpcoming.View = System.Windows.Forms.View.Details;
            this.lvUpcoming.Enter += new System.EventHandler(lvUpcoming_Enter);
            this.lvUpcoming.SelectedIndexChanged += new System.EventHandler(lvUpcoming_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 228;
            // 
            // TimerOfDeath
            // 
            this.TimerOfDeath.Enabled = true;
            this.TimerOfDeath.Interval = 10000;
            this.TimerOfDeath.Tick += new System.EventHandler(TimerOfDeath_Tick);
            // 
            // hiddenButton
            // 
            this.hiddenButton.Location = new System.Drawing.Point(276, 31);
            this.hiddenButton.Name = "hiddenButton";
            this.hiddenButton.Size = new System.Drawing.Size(97, 23);
            this.hiddenButton.TabIndex = 0;
            this.hiddenButton.Text = "hiddenButton";
            this.hiddenButton.UseVisualStyleBackColor = true;
            this.hiddenButton.Visible = false;
            // 
            // UpcomingPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 90);
            this.Controls.Add(this.hiddenButton);
            this.Controls.Add(this.lvUpcoming);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpcomingPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Upcoming Shows";
            this.TopMost = true;
            this.Load += new System.EventHandler(UpcomingPopup_Load);
            this.ResumeLayout(false);

        }


        #endregion

        private ListView lvUpcoming;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Timer TimerOfDeath;
        private Button hiddenButton;
    }
}
