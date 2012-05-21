// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
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
        private ProcessedEpisodeList mOriginalEps;
        private ShowItem mSI;
        private int mSeasonNumber;

        public EditRules(ShowItem si, ProcessedEpisodeList originalEpList, int seasonNumber, CustomName style)
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
            this.bnPreview.Tag = true;

            this.FillRuleList(false, 0);

            foreach (ProcessedEpisode pe in originalEpList)
                this.lbEpsOriginal.Items.Add(this.NameStyle.NameForExt(pe, null, 0));
        }

        private void bnAddRule_Click(object sender, System.EventArgs e)
        {
            ShowRule sr = new ShowRule();
            AddModifyRule ar = new AddModifyRule(sr);

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
                lvi.Text = (this.lvRuleList.Items.Count + 1).ToString();
                lvi.SubItems.Add(sr.ActionInWords());
                lvi.SubItems.Add(sr.First == -1 ? "" : sr.First.ToString());
                lvi.SubItems.Add(sr.Second == -1 ? "" : sr.Second.ToString());
                lvi.SubItems.Add(sr.PlainEnglishDescription());
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
            AddModifyRule ar = new AddModifyRule(sr);
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

        private void FillPreview(int StopAfter = 0)
        {
            this.txtPreviewLabel.Text = StopAfter == 0
                                       ? "Processed List Preview :"
                                       : string.Format("Processed List Preview (Rules 1 to {0}):", StopAfter);

            ProcessedEpisodeList pel = new ProcessedEpisodeList();
            if (this.mOriginalEps != null)
            {
                foreach (ProcessedEpisode pe in this.mOriginalEps)
                    pel.Add(new ProcessedEpisode(pe));

                if (StopAfter == 0)
                    StopAfter = this.WorkingRuleSet.Count;
                TVDoc.ApplyRules(pel, this.WorkingRuleSet.GetRange(0, StopAfter), this.mSI);
            }

            this.lbEpsPreview.BeginUpdate();
            this.lbEpsPreview.Items.Clear();
            foreach (ProcessedEpisode pe in pel)
                this.lbEpsPreview.Items.Add(new PreviewListItem(pe, NameStyle));                
            this.lbEpsPreview.EndUpdate();
        }

        private void lbEpsPreview_DoubleClick(object sender, System.EventArgs e)
        {
            ShowRule sr = new ShowRule();
            if (lbEpsPreview.SelectedItem != null) 
                sr.First = ((PreviewListItem) lbEpsPreview.SelectedItem).EpNumber();
            AddModifyRule ar = new AddModifyRule(sr);

            if (ar.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.WorkingRuleSet.Add(sr);

            this.FillRuleList(false, 0);
        }

        private void EditRules_SizeChanged(object sender, System.EventArgs e)
        {
            lvRuleList.Columns[4].Width = lvRuleList.Width - lvRuleList.Columns[0].Width -
                                          lvRuleList.Columns[1].Width - lvRuleList.Columns[2].Width -
                                          lvRuleList.Columns[3].Width - SystemInformation.VerticalScrollBarWidth -
                                          lvRuleList.Columns.Count;
        }

        private void bnPreview_Click(object sender, System.EventArgs e)
        {
            if (!(bool)bnPreview.Tag)
            {
                bnPreview.Text = "Preview to...";
                bnPreview.Tag = true;
                this.FillPreview();
            }
            else if (lvRuleList.SelectedIndices.Count > 0)
            {
                bnPreview.Text = "Preview All";
                bnPreview.Tag = false;
                this.FillPreview(lvRuleList.SelectedIndices[0] + 1);
            }
        }

        private class PreviewListItem
        {
            private ProcessedEpisode pe;
            private CustomName nameStyle;

            public PreviewListItem(ProcessedEpisode PE, CustomName NameStyle)
            {
                pe = PE;
                nameStyle = NameStyle;
            }

            public override string ToString()
            {
                return this.nameStyle.NameForExt(pe, null, 0);
            }

            public int EpNumber()
            {
                return pe.EpNum2;
            }

        }
    }
}