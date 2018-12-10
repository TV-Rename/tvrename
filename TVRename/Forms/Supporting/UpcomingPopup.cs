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
        private readonly TVDoc mDoc;

        public UpcomingPopup(TVDoc doc)
        {
            mDoc = doc;
            InitializeComponent();
        }

        private void UpcomingPopup_Load(object sender, System.EventArgs e)
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            Left = screenWidth - Width;
            Top = screenHeight - Height;

            FillSelf();
        }

        private void TimerOfDeath_Tick(object sender, System.EventArgs e)
        {
            Close();
        }

        private void FillSelf()
        {
            lvUpcoming.BeginUpdate();
            lvUpcoming.Items.Clear();

            const int N = 5;

            List<ProcessedEpisode> next5 = mDoc.Library.NextNShows(N, 0, 9999);

            if (next5 != null)
            {
                foreach (ProcessedEpisode ei in next5)
                {
                    ListViewItem lvi = new ListViewItem {Text = ei.HowLong()};
                    lvi.SubItems.Add(ei.DayOfWeek());
                    lvi.SubItems.Add(ei.TimeOfDay());
                    lvi.SubItems.Add(TVSettings.Instance.NamingStyle.NameFor(ei));
                    lvUpcoming.Items.Add(lvi);
                }
                if (lvUpcoming.Items.Count > 0)
                {
                    int h1 = lvUpcoming.Items[0].GetBounds(ItemBoundsPortion.Entire).Height + 6;
                    Height = (h1 * lvUpcoming.Items.Count);
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
