// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Windows.Forms;

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
    public partial class UpcomingPopup : Form
    {
        private TVDoc mDoc;

        public UpcomingPopup(TVDoc doc)
        {
            this.mDoc = doc;
            this.InitializeComponent();
        }

        private void UpcomingPopup_Load(object sender, System.EventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = screenWidth - this.Width;
            this.Top = screenHeight - this.Height;

            this.FillSelf();
        }

        private void TimerOfDeath_Tick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void FillSelf()
        {
            this.lvUpcoming.BeginUpdate();
            this.lvUpcoming.Items.Clear();

            const int kN = 5;

            List<ProcessedEpisode> next5 = this.mDoc.Library.NextNShows(kN, 0, 9999);

            if (next5 != null)
            {
                foreach (ProcessedEpisode ei in next5)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = ei.HowLong();
                    lvi.SubItems.Add(ei.DayOfWeek());
                    lvi.SubItems.Add(ei.TimeOfDay());
                    lvi.SubItems.Add(TVSettings.Instance.NamingStyle.NameFor(ei));
                    this.lvUpcoming.Items.Add(lvi);
                }
                if (this.lvUpcoming.Items.Count > 0)
                {
                    int h1 = this.lvUpcoming.Items[0].GetBounds(ItemBoundsPortion.Entire).Height + 6;
                    this.Height = (h1 * this.lvUpcoming.Items.Count);
                }
            }

            int w = 0;
            for (int i = 0; i < this.lvUpcoming.Columns.Count; i++)
            {
                this.lvUpcoming.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                w += this.lvUpcoming.Columns[i].Width;
            }

            this.lvUpcoming.Width = w;
            this.lvUpcoming.SelectedIndices.Clear();
            this.hiddenButton.Select();

            this.lvUpcoming.EndUpdate();
        }

        private void lvUpcoming_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.lvUpcoming.SelectedIndices.Clear();
            this.hiddenButton.Select();
        }

        private void lvUpcoming_Enter(object sender, System.EventArgs e)
        {
            this.hiddenButton.Select();
        }
    }
}
