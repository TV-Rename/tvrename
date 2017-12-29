// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for EditRules
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class EditRules : Form
    {
        private CustomName NameStyle;
        private List<ShowRule> WorkingRuleSet;
        private List<ProcessedEpisode> mOriginalEps;
        private ShowItem mSI;
        private int mSeasonNumber;

        public EditRules(ShowItem si, List<ProcessedEpisode> originalEpList, int seasonNumber, CustomName style)
        {
            NameStyle = style;
            InitializeComponent();

            mSI = si;
            mOriginalEps = originalEpList;
            mSeasonNumber = seasonNumber;

            if (si.SeasonRules.ContainsKey(seasonNumber))
                WorkingRuleSet = new List<ShowRule>(si.SeasonRules[seasonNumber]);
            else
                WorkingRuleSet = new List<ShowRule>();

            txtShowName.Text = si.ShowName;
            txtSeasonNumber.Text = seasonNumber.ToString();

            FillRuleList(false, 0);
        }

        private void bnAddRule_Click(object sender, System.EventArgs e)
        {
            ShowRule sr = new ShowRule();
            AddModifyRule ar = new AddModifyRule(sr);

            bool res = ar.ShowDialog() == DialogResult.OK;
            if (res)
                WorkingRuleSet.Add(sr);

            FillRuleList(false, 0);
        }

        private void FillRuleList(bool keepSel, int adj)
        {
            List<int> sel = new List<int>();
            if (keepSel)
            {
                foreach (int i in lvRuleList.SelectedIndices)
                    sel.Add(i);
            }

            lvRuleList.Items.Clear();
            foreach (ShowRule sr in WorkingRuleSet)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = sr.ActionInWords();
                lvi.SubItems.Add(sr.First == -1 ? "" : sr.First.ToString());
                lvi.SubItems.Add(sr.Second == -1 ? "" : sr.Second.ToString());
                lvi.SubItems.Add(sr.UserSuppliedText);
                lvi.Tag = sr;
                lvRuleList.Items.Add(lvi);
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < lvRuleList.Items.Count)
                    {
                        int n = i + adj;
                        if ((n >= 0) && (n < lvRuleList.Items.Count))
                            lvRuleList.Items[n].Selected = true;
                    }
                }
            }

            FillPreview();
        }

        private void bnEdit_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedItems.Count == 0)
                return;
            ShowRule sr = (ShowRule) (lvRuleList.SelectedItems[0].Tag);
            AddModifyRule ar = new AddModifyRule(sr);
            ar.ShowDialog(); // modifies rule in-place if OK'd
            FillRuleList(false, 0);
        }

        private void bnDelRule_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedItems.Count == 0)
                return;
            ShowRule sr = (ShowRule) (lvRuleList.SelectedItems[0].Tag);

            WorkingRuleSet.Remove(sr);
            FillRuleList(false, 0);
        }

        private void bnRuleUp_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedIndices.Count != 1)
                return;
            int p = lvRuleList.SelectedIndices[0];
            if (p <= 0)
                return;

            ShowRule sr = WorkingRuleSet[p];
            WorkingRuleSet.RemoveAt(p);
            WorkingRuleSet.Insert(p - 1, sr);

            FillRuleList(true, -1);
        }

        private void bnRuleDown_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedIndices.Count != 1)
                return;
            int p = lvRuleList.SelectedIndices[0];
            if (p >= (lvRuleList.Items.Count - 1))
                return;

            ShowRule sr = WorkingRuleSet[p];
            WorkingRuleSet.RemoveAt(p);
            WorkingRuleSet.Insert(p + 1, sr);
            FillRuleList(true, +1);
        }

        private void lvRuleList_DoubleClick(object sender, System.EventArgs e)
        {
            bnEdit_Click(null, null);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            mSI.SeasonRules[mSeasonNumber] = WorkingRuleSet;
            Close();
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void FillPreview()
        {
            List<ProcessedEpisode> pel = new List<ProcessedEpisode>();

            if (mOriginalEps != null)
            {
                foreach (ProcessedEpisode pe in mOriginalEps)
                    pel.Add(new ProcessedEpisode(pe));

                TVDoc.ApplyRules(pel, WorkingRuleSet, mSI);
            }

            lbEpsPreview.BeginUpdate();
            lbEpsPreview.Items.Clear();
            foreach (ProcessedEpisode pe in pel)
                lbEpsPreview.Items.Add(NameStyle.NameForExt(pe, null, 0));
            lbEpsPreview.EndUpdate();
        }
    }
}
