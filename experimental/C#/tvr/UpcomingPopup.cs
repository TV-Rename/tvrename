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


namespace TVRename
{

    /// <summary>
    /// Summary for UpcomingPopup
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public class UpcomingPopup : System.Windows.Forms.Form
    {
        private TVDoc mDoc;

        public UpcomingPopup(TVDoc doc)
        {
            mDoc = doc;
            InitializeComponent();
            //
            //TODO: Add the constructor code here
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        private System.Windows.Forms.ListView lvUpcoming;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Timer TimerOfDeath;
        private System.Windows.Forms.Button hiddenButton;
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Required designer variable.
        /// </summary>


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
        private void UpcomingPopup_Load(object sender, System.EventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = screenWidth - this.Width;
            this.Top = screenHeight - this.Height;

            FillSelf();
        }
        private void TimerOfDeath_Tick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void FillSelf()
        {
            lvUpcoming.BeginUpdate();
            lvUpcoming.Items.Clear();

            const int kN = 5;

            ProcessedEpisodeList next5 = mDoc.NextNShows(kN, 9999);

            if (next5 != null)
            {
                foreach (ProcessedEpisode ei in next5)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = ei.HowLong();
                    lvi.SubItems.Add(ei.DayOfWeek());
                    lvi.SubItems.Add(ei.TimeOfDay());
                    lvi.SubItems.Add(mDoc.Settings.NamingStyle.NameForExt(ei, null));
                    lvUpcoming.Items.Add(lvi);
                }
                if (lvUpcoming.Items.Count > 0)
                {
                    int h1 = lvUpcoming.Items[0].GetBounds(ItemBoundsPortion.Entire).Height + 6;
                    this.Height = (h1 * lvUpcoming.Items.Count);
                }
            }

            int w = 0;
            for (int i = 0; i < lvUpcoming.Columns.Count; i++)
            {
                lvUpcoming.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                w += lvUpcoming.Columns[i].Width;
            }

            lvUpcoming.Width = w;
            lvUpcoming.SelectedIndices.Clear();
            hiddenButton.Select();

            lvUpcoming.EndUpdate();
        }

        private void lvUpcoming_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            lvUpcoming.SelectedIndices.Clear();
            hiddenButton.Select();
        }
        private void lvUpcoming_Enter(object sender, System.EventArgs e)
        {
            hiddenButton.Select();
        }
    }
}