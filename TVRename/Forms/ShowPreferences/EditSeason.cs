// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
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
        private readonly List<ProcessedEpisode> mOriginalEps;
        private readonly ShowItem show;
        private readonly int mSeasonNumber;
        private readonly List<ProcessedEpisode> episodesToAddToSeen;
        private readonly List<ProcessedEpisode> episodesToRemoveFromSeen;

        public EditSeason([NotNull] ShowItem si, List<ProcessedEpisode> originalEpList, int seasonNumber, CustomEpisodeName style)
        {
            nameStyle = style;
            InitializeComponent();

            episodesToAddToSeen = new List<ProcessedEpisode>();
            episodesToRemoveFromSeen = new List<ProcessedEpisode>();

            show = si;
            mOriginalEps = originalEpList;
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
        }

        private void FillSeenEpisodes(bool keepSel)
        {
            List<int> sel = new List<int>();
            if (keepSel)
            {
                foreach (int i in lvSeenEpisodes.SelectedIndices)
                {
                    sel.Add(i);
                }
            }

            lvSeenEpisodes.Items.Clear();
            foreach (ProcessedEpisode ep in mOriginalEps.Where(ep => ep.PreviouslySeen))
            {
                ListViewItem lvi = new ListViewItem { Text = ep.AppropriateEpNum.ToString() };
                lvi.SubItems.Add(ep.Name);
                lvi.Tag = ep;
                lvSeenEpisodes.Items.Add(lvi);
            }

            if (keepSel)
            {
                foreach (int i in sel)
                {
                    if (i < lvSeenEpisodes.Items.Count)
                    {
                        int n = i;
                        if ((n >= 0) && (n < lvSeenEpisodes.Items.Count))
                        {
                            lvSeenEpisodes.Items[n].Selected = true;
                        }
                    }
                }
            }
        }

        private void bnAddRule_Click(object sender, System.EventArgs e)
        {
            ShowRule sr = new ShowRule();
            AddModifyRule ar = new AddModifyRule(sr, show.GetSeason(mSeasonNumber), show.DvdOrder);

            bool res = ar.ShowDialog() == DialogResult.OK;
            if (res)
            {
                workingRuleSet.Add(sr);
            }

            FillRuleList(false, 0);
        }

        private void FillRuleList(bool keepSel, int adj)
        {
            List<int> sel = new List<int>();
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
                ListViewItem lvi = new ListViewItem {Text = sr.ActionInWords()};

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
            if (lvRuleList.SelectedItems.Count == 0)
            {
                return;
            }

            ShowRule sr = (ShowRule) (lvRuleList.SelectedItems[0].Tag);
            AddModifyRule ar = new AddModifyRule(sr,show.GetSeason(mSeasonNumber),show.DvdOrder);
            ar.ShowDialog(); // modifies rule in-place if OK'd
            FillRuleList(false, 0);
        }

        private void bnDelRule_Click(object sender, System.EventArgs e)
        {
            if (lvRuleList.SelectedItems.Count == 0)
            {
                return;
            }

            ShowRule sr = (ShowRule) (lvRuleList.SelectedItems[0].Tag);

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
            if (p >= (lvRuleList.Items.Count - 1))
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
            bnEdit_Click(null, null);
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
            List<ProcessedEpisode> pel = new List<ProcessedEpisode>();

            if (mOriginalEps != null)
            {
                foreach (ProcessedEpisode pe in mOriginalEps)
                {
                    pel.Add(new ProcessedEpisode(pe));
                }

                ShowLibrary.ApplyRules(pel, workingRuleSet, show);
            }

            lbEpsPreview.BeginUpdate();
            lbEpsPreview.Items.Clear();
            foreach (ProcessedEpisode pe in pel)
            {
                lbEpsPreview.Items.Add(nameStyle.NameFor(pe));
            }

            lbEpsPreview.EndUpdate();
        }

        private void Button2_Click(object sender, System.EventArgs e)
        {
            List<ProcessedEpisode> possibleEpisodes = new List<ProcessedEpisode>();
            foreach (ProcessedEpisode testEp in mOriginalEps)
            {
                if (testEp.PreviouslySeen)
                {
                    continue;
                }

                if (episodesToAddToSeen.Contains(testEp))
                {
                    continue;
                }

                possibleEpisodes.Add(testEp);
            }
            possibleEpisodes.AddRange(episodesToRemoveFromSeen);

            NewSeenEpisode nse = new NewSeenEpisode(possibleEpisodes);
            DialogResult dialogResult = nse.ShowDialog();

            if (dialogResult != DialogResult.OK )
            {
                return;
            }
            if (nse.ChosenEpisode is null)
            {
                return;
            }

            episodesToAddToSeen.Add(nse.ChosenEpisode);

            ListViewItem lvi = new ListViewItem { Text = nse.ChosenEpisode.AppropriateEpNum.ToString() };
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

            for (int index = 0; index < lvSeenEpisodes.SelectedItems.Count; index++)
            {
                ProcessedEpisode pe = (ProcessedEpisode)lvSeenEpisodes.SelectedItems[index].Tag;
                episodesToRemoveFromSeen.Add(pe);
                lvSeenEpisodes.Items.Remove(lvSeenEpisodes.SelectedItems[index]);
            }
        }

        private void LvSeenEpisodes_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            bnRemoveSeen.Enabled = (lvSeenEpisodes.SelectedItems.Count > 0);
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
    }
}
