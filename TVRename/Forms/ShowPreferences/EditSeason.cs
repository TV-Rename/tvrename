//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TVRename.Forms.ShowPreferences;

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
    public partial class EditSeason : Form
    {
        private readonly CustomEpisodeName nameStyle;
        private readonly List<ShowRule> workingRuleSet;
        private readonly List<ProcessedEpisode>? mOriginalEps;
        private readonly ShowConfiguration show;
        private readonly int mSeasonNumber;
        private readonly List<ProcessedEpisode> episodesToAddToSeen;
        private readonly List<ProcessedEpisode> episodesToRemoveFromSeen;

        public EditSeason([NotNull] ShowConfiguration si, int seasonNumber, CustomEpisodeName style)
        {
            mOriginalEps = ShowLibrary.GenerateEpisodes(si, seasonNumber, false);

            nameStyle = style;
            InitializeComponent();

            episodesToAddToSeen = new List<ProcessedEpisode>();
            episodesToRemoveFromSeen = new List<ProcessedEpisode>();

            show = si;
            mSeasonNumber = seasonNumber;

            workingRuleSet = si.SeasonRules.ContainsKey(seasonNumber)
                ? new List<ShowRule>(si.SeasonRules[seasonNumber])
                : new List<ShowRule>();

            txtShowName.Text = si.ShowName;
            txtSeasonNumber.Text = seasonNumber.ToString();

            FillRuleList(false, 0);
            FillSeenEpisodes(false);
            lvSeenEpisodes.ListViewItemSorter = new NumberAsTextSorter(0);
            lvSeenEpisodes.Sort();

            UpdatePreviouslySeenButtons();
            UpdateRuleButtons();
        }
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            lvRuleList.ScaleListViewColumns(factor);
            lvSeenEpisodes.ScaleListViewColumns(factor);
        }
        private void FillSeenEpisodes(bool keepSel)
        {
            List<int> sel = new();
            if (keepSel)
            {
                foreach (int i in lvSeenEpisodes.SelectedIndices)
                {
                    sel.Add(i);
                }
            }

            lvSeenEpisodes.Items.Clear();
            if (mOriginalEps != null)
            {
                foreach (ProcessedEpisode ep in mOriginalEps.Where(ep => ep.PreviouslySeen))
                {
                    ListViewItem lvi = new() { Text = ep.EpNumsAsString() };
                    lvi.SubItems.Add(ep.Name);
                    lvi.Tag = ep;
                    lvSeenEpisodes.Items.Add(lvi);
                }
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < lvSeenEpisodes.Items.Count)
                    {
                        if (i >= 0 && i < lvSeenEpisodes.Items.Count)
                        {
                            lvSeenEpisodes.Items[i].Selected = true;
                        }
                    }
                }
            }
        }

        private void bnAddRule_Click(object sender, System.EventArgs e)
        {
            ShowRule sr = new();
            AddModifyRule ar = new(sr, show, ProcessedEpisodes());

            bool res = ar.ShowDialog(this) == DialogResult.OK;
            if (res)
            {
                workingRuleSet.Add(sr);
            }

            FillRuleList(false, 0);
        }

        private void FillRuleList(bool keepSel, int adj)
        {
            List<int> sel = new();
            if (keepSel)
            {
                foreach (int i in lvRuleList.SelectedIndices)
                {
                    sel.Add(i);
                }
            }

            lvRuleList.Items.Clear();
            foreach (ShowRule sr in workingRuleSet)
            {
                ListViewItem lvi = new() { Text = sr.ActionInWords() };

                lvi.SubItems.Add(sr.First == -1 ? string.Empty : sr.First.ToString());
                lvi.SubItems.Add(sr.Second == -1 ? string.Empty : sr.Second.ToString());
                lvi.SubItems.Add(sr.UserSuppliedText);
                lvi.SubItems.Add(sr.RenumberAfter ? "Yes" : "No");
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
                        if (n >= 0 && n < lvRuleList.Items.Count)
                        {
                            lvRuleList.Items[n].Selected = true;
                        }
                    }
                }
            }

            FillPreview();
        }

        private void bnEdit_Click(object sender, System.EventArgs e)
        {
            EditSelectedRule();
        }

        private void EditSelectedRule()
        {
            if (lvRuleList.SelectedItems.Count == 0)
            {
                return;
            }

            ShowRule sr = (ShowRule)lvRuleList.SelectedItems[0].Tag;
            AddModifyRule ar = new(sr, show, ProcessedEpisodes());
            ar.ShowDialog(this); // modifies rule in-place if OK'd
            FillRuleList(false, 0);
        }

        private void bnDelRule_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedItems.Count == 0)
            {
                return;
            }

            ShowRule sr = (ShowRule)lvRuleList.SelectedItems[0].Tag;

            workingRuleSet.Remove(sr);
            FillRuleList(false, 0);
        }

        private void bnRuleUp_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedIndices.Count != 1)
            {
                return;
            }

            int p = lvRuleList.SelectedIndices[0];
            if (p <= 0)
            {
                return;
            }

            ShowRule sr = workingRuleSet[p];
            workingRuleSet.RemoveAt(p);
            workingRuleSet.Insert(p - 1, sr);

            FillRuleList(true, -1);
        }

        private void bnRuleDown_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedIndices.Count != 1)
            {
                return;
            }

            int p = lvRuleList.SelectedIndices[0];
            if (p >= lvRuleList.Items.Count - 1)
            {
                return;
            }

            ShowRule sr = workingRuleSet[p];
            workingRuleSet.RemoveAt(p);
            workingRuleSet.Insert(p + 1, sr);
            FillRuleList(true, +1);
        }

        private void lvRuleList_DoubleClick(object sender, System.EventArgs e)
        {
            EditSelectedRule();
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            show.SeasonRules[mSeasonNumber] = workingRuleSet;
            ApplyChangesToSeenEpisodes();
            Close();
        }

        private void ApplyChangesToSeenEpisodes()
        {
            foreach (ProcessedEpisode epIdToRemove in episodesToRemoveFromSeen)
            {
                TVSettings.Instance.PreviouslySeenEpisodes.Remove(epIdToRemove.EpisodeId);
            }

            foreach (ProcessedEpisode epIdToAdd in episodesToAddToSeen)
            {
                TVSettings.Instance.PreviouslySeenEpisodes.EnsureAdded(epIdToAdd);
            }
        }

        private void bnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void FillPreview()
        {
            IEnumerable<ProcessedEpisode> pel = ProcessedEpisodes();

            lbEpsPreview.BeginUpdate();
            lbEpsPreview.Items.Clear();
            foreach (ProcessedEpisode pe in pel)
            {
                lbEpsPreview.Items.Add(nameStyle.NameFor(pe));
            }

            lbEpsPreview.EndUpdate();
        }

        [NotNull]
        private IEnumerable<ProcessedEpisode> ProcessedEpisodes()
        {
            List<ProcessedEpisode> pel = new();

            if (mOriginalEps != null)
            {
                foreach (ProcessedEpisode pe in mOriginalEps)
                {
                    pel.Add(new ProcessedEpisode(pe));
                }

                ShowLibrary.ApplyRules(pel, workingRuleSet, show);
            }

            return pel;
        }

        private void Button2_Click(object sender, System.EventArgs e)
        {
            List<ProcessedEpisode> possibleEpisodes = new();
            if (mOriginalEps != null)
            {
                possibleEpisodes.AddRange(mOriginalEps.Where(testEp => !testEp.PreviouslySeen).Where(testEp => !episodesToAddToSeen.Contains(testEp)));
            }

            possibleEpisodes.AddRange(episodesToRemoveFromSeen);

            NewSeenEpisode nse = new(possibleEpisodes);
            DialogResult dialogResult = nse.ShowDialog(this);

            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            if (nse.ChosenEpisode is null)
            {
                return;
            }

            episodesToAddToSeen.Add(nse.ChosenEpisode);

            ListViewItem lvi = new() { Text = nse.ChosenEpisode.AppropriateEpNum.ToString() };
            lvi.SubItems.Add(nse.ChosenEpisode.Name);
            lvi.Tag = nse.ChosenEpisode;
            lvSeenEpisodes.Items.Add(lvi);
        }

        private void Button1_Click(object sender, System.EventArgs e)
        {
            if (lvSeenEpisodes.SelectedItems.Count == 0)
            {
                return;
            }

            for (int index = lvSeenEpisodes.SelectedItems.Count-1; index >=0; index--)
            {
                ProcessedEpisode pe = (ProcessedEpisode)lvSeenEpisodes.SelectedItems[index].Tag;
                episodesToRemoveFromSeen.Add(pe);
                lvSeenEpisodes.Items.Remove(lvSeenEpisodes.SelectedItems[index]);
            }
        }

        private void UpdatePreviouslySeenButtons()
        {
            bnRemoveSeen.Enabled = lvSeenEpisodes.SelectedItems.Count > 0;
        }

        private void UpdateRuleButtons()
        {
            bool anythingSelected = lvRuleList.SelectedItems.Count > 0;

            bnDelRule.Enabled = anythingSelected;
            bnEdit.Enabled = anythingSelected;
            bnRuleUp.Enabled = anythingSelected;
            bnRuleDown.Enabled = anythingSelected;
        }

        private void LvSeenEpisodes_ColumnClick(object sender, [NotNull] ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                lvSeenEpisodes.ListViewItemSorter = new NumberAsTextSorter(e.Column);
            }
            else
            {
                lvSeenEpisodes.ListViewItemSorter = new TextSorter(e.Column);
            }

            lvSeenEpisodes.Sort();
        }

        private void LvRuleList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdateRuleButtons();
        }

        private void LvSeenEpisodes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UpdatePreviouslySeenButtons();
        }
    }
}
