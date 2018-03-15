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
    /// Summary for EditRules
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class EditRules : System.Windows.Forms.Form
    {
        private CustomName NameStyle;
        private System.Collections.Generic.List<ShowRule> WorkingRuleSet;
        private List<ProcessedEpisode> mOriginalEps;
        private ShowItem mSI;
        private int mSeasonNumber;

        public EditRules(ShowItem si, List<ProcessedEpisode> originalEpList, int seasonNumber, CustomName style)
        {
            this.NameStyle = style;
            this.InitializeComponent();

            this.mSI = si;
            this.mOriginalEps = originalEpList;
            this.mSeasonNumber = seasonNumber;

            if (si.SeasonRules.ContainsKey(seasonNumber))
                this.WorkingRuleSet = new System.Collections.Generic.List<ShowRule>(si.SeasonRules[seasonNumber]);
            else
                this.WorkingRuleSet = new System.Collections.Generic.List<ShowRule>();

            this.txtShowName.Text = si.ShowName;
            this.txtSeasonNumber.Text = seasonNumber.ToString();

            this.FillRuleList(false, 0);
        }

        private void bnAddRule_Click(object sender, System.EventArgs e)
        {
            ShowRule sr = new ShowRule();
            AddModifyRule ar = new AddModifyRule(sr, this.mSI.GetSeason(this.mSeasonNumber), this.mSI.DVDOrder);

            bool res = ar.ShowDialog() == System.Windows.Forms.DialogResult.OK;
            if (res)
                this.WorkingRuleSet.Add(sr);

            this.FillRuleList(false, 0);
        }

        private void FillRuleList(bool keepSel, int adj)
        {
            System.Collections.Generic.List<int> sel = new System.Collections.Generic.List<int>();
            if (keepSel)
            {
                foreach (int i in this.lvRuleList.SelectedIndices)
                    sel.Add(i);
            }

            this.lvRuleList.Items.Clear();
            foreach (ShowRule sr in this.WorkingRuleSet)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = sr.ActionInWords();
                lvi.SubItems.Add(sr.First == -1 ? "" : sr.First.ToString());
                lvi.SubItems.Add(sr.Second == -1 ? "" : sr.Second.ToString());
                lvi.SubItems.Add(sr.UserSuppliedText);
                lvi.Tag = sr;
                this.lvRuleList.Items.Add(lvi);
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < this.lvRuleList.Items.Count)
                    {
                        int n = i + adj;
                        if ((n >= 0) && (n < this.lvRuleList.Items.Count))
                            this.lvRuleList.Items[n].Selected = true;
                    }
                }
            }

            this.FillPreview();
        }

        private void bnEdit_Click(object sender, System.EventArgs e)
        {
            if (this.lvRuleList.SelectedItems.Count == 0)
                return;
            ShowRule sr = (ShowRule) (this.lvRuleList.SelectedItems[0].Tag);
            AddModifyRule ar = new AddModifyRule(sr,this.mSI.GetSeason(this.mSeasonNumber),this.mSI.DVDOrder);
            ar.ShowDialog(); // modifies rule in-place if OK'd
            this.FillRuleList(false, 0);
        }

        private void bnDelRule_Click(object sender, System.EventArgs e)
        {
            if (this.lvRuleList.SelectedItems.Count == 0)
                return;
            ShowRule sr = (ShowRule) (this.lvRuleList.SelectedItems[0].Tag);

            this.WorkingRuleSet.Remove(sr);
            this.FillRuleList(false, 0);
        }

        private void bnRuleUp_Click(object sender, System.EventArgs e)
        {
            if (this.lvRuleList.SelectedIndices.Count != 1)
                return;
            int p = this.lvRuleList.SelectedIndices[0];
            if (p <= 0)
                return;

            ShowRule sr = this.WorkingRuleSet[p];
            this.WorkingRuleSet.RemoveAt(p);
            this.WorkingRuleSet.Insert(p - 1, sr);

            this.FillRuleList(true, -1);
        }

        private void bnRuleDown_Click(object sender, System.EventArgs e)
        {
            if (this.lvRuleList.SelectedIndices.Count != 1)
                return;
            int p = this.lvRuleList.SelectedIndices[0];
            if (p >= (this.lvRuleList.Items.Count - 1))
                return;

            ShowRule sr = this.WorkingRuleSet[p];
            this.WorkingRuleSet.RemoveAt(p);
            this.WorkingRuleSet.Insert(p + 1, sr);
            this.FillRuleList(true, +1);
        }

        private void lvRuleList_DoubleClick(object sender, System.EventArgs e)
        {
            this.bnEdit_Click(null, null);
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            this.mSI.SeasonRules[this.mSeasonNumber] = this.WorkingRuleSet;
            this.Close();
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void FillPreview()
        {
            List<ProcessedEpisode> pel = new List<ProcessedEpisode>();

            if (this.mOriginalEps != null)
            {
                foreach (ProcessedEpisode pe in this.mOriginalEps)
                    pel.Add(new ProcessedEpisode(pe));

                TVDoc.ApplyRules(pel, this.WorkingRuleSet, this.mSI);
            }

            this.lbEpsPreview.BeginUpdate();
            this.lbEpsPreview.Items.Clear();
            foreach (ProcessedEpisode pe in pel)
                this.lbEpsPreview.Items.Add(this.NameStyle.NameForExt(pe));
            this.lbEpsPreview.EndUpdate();
        }
    }
}
