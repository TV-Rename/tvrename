//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Linq;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class Filters : Form
    {
        private readonly TVDoc doc;
        private const string IS_NOT = "is not";
        private const string IS = "is";

        public Filters(TVDoc doc)
        {
            this.doc = doc;
            InitializeComponent();

            clbGenre.Items.AddRange(doc.TvLibrary.GetGenres().Cast<object>().ToArray());

            cmbNetwork.Items.Add(string.Empty);
            cmbNetwork.Items.AddRange(doc.TvLibrary.GetNetworks().Cast<object>().ToArray());

            cmbShowStatus.Items.Add(string.Empty);
            cmbShowStatus.Items.AddRange(doc.TvLibrary.GetStatuses().Cast<object>().ToArray());

            cmbRating.Items.Add(string.Empty);
            cmbRating.Items.AddRange(doc.TvLibrary.GetContentRatings().Cast<object>().ToArray());

            SetButtonStates();
        }

        private void SetButtonStates()
        {
            ShowFilter filter = TVSettings.Instance.Filter;
            {
                //Filter By Show Names
                bool filterByShowNames = filter.ShowName != null;
                tbShowName.Text = filterByShowNames ? filter.ShowName : string.Empty;

                //Filter By Show Status
                bool filterByShowStatus = filter.ShowStatus != null;
                cmbShowStatus.SelectedItem = filterByShowStatus ? filter.ShowStatus : string.Empty;

                //Filter By Show Rating
                bool filterByShowRating = filter.ShowRating != null;
                cmbRating.SelectedItem = filterByShowRating ? filter.ShowRating : string.Empty;

                //Filter By Show Network
                bool filterByShowNetwork = filter.ShowNetwork != null;
                cmbNetwork.SelectedItem = filterByShowNetwork ? filter.ShowNetwork : string.Empty;

                //Filter By Show Status
                cmbShowStatusType.SelectedItem = filter.ShowStatusInclude ? IS : IS_NOT;

                //Filter By Show Rating
                cmbRatingType.SelectedItem = filter.ShowRatingInclude ? IS : IS_NOT;

                //Filter By Show Network
                cmbNetworkType.SelectedItem = filter.ShowNetworkInclude ? IS : IS_NOT;

                //Filter By Genre
                foreach (string genre in filter.Genres)
                {
                    int genreIndex = clbGenre.Items.IndexOf(genre);
                    if (genreIndex > 0)
                    {
                        clbGenre.SetItemChecked(genreIndex, true);
                    }
                }

                chkIncludeBlanks.Checked = filter.IncludeBlankFields;
            }

            SeasonFilter sFilter = TVSettings.Instance.SeasonFilter;
            chkHideIgnoredSeasons.Checked = sFilter.HideIgnoredSeasons;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ShowFilter filter = TVSettings.Instance.Filter;

            filter.ShowName = string.IsNullOrEmpty(tbShowName.Text) ? null : tbShowName.Text;
            filter.ShowStatus = string.IsNullOrEmpty(cmbShowStatus.Text) ? null : cmbShowStatus.SelectedItem.ToString();
            filter.ShowNetwork = string.IsNullOrEmpty(cmbNetwork.Text) ? null : cmbNetwork.SelectedItem.ToString();
            filter.ShowRating = string.IsNullOrEmpty(cmbRating.Text) ? null : cmbRating.SelectedItem.ToString();

            filter.ShowStatusInclude = GetIncludeStatus(cmbShowStatusType);
            filter.ShowNetworkInclude = GetIncludeStatus(cmbNetworkType);
            filter.ShowRatingInclude = GetIncludeStatus(cmbRatingType);

            filter.IncludeBlankFields = chkIncludeBlanks.Checked;

            filter.Genres.Clear();
            foreach (string genre in clbGenre.CheckedItems)
            {
                filter.Genres.Add(genre);
            }

            SeasonFilter sFilter = TVSettings.Instance.SeasonFilter;
            sFilter.HideIgnoredSeasons = chkHideIgnoredSeasons.Checked;

            doc.SetDirty();
            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool GetIncludeStatus(ComboBox comboBox)
        {
            if (!comboBox.Text.HasValue())
            {
                return true;
            }

            if (comboBox.SelectedItem.ToString().Equals(IS_NOT))
            {
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void bnReset_Click(object sender, EventArgs e)
        {
            tbShowName.Text = string.Empty;
            cmbShowStatus.SelectedItem = string.Empty;
            cmbNetwork.SelectedItem = string.Empty;
            cmbRating.SelectedItem = string.Empty;

            cmbShowStatusType.SelectedItem = IS;
            cmbNetworkType.SelectedItem = IS;
            cmbRatingType.SelectedItem = IS;

            clbGenre.ClearSelected();

            for (int i = 0; i < clbGenre.Items.Count; i++)
            {
                clbGenre.SetItemChecked(i, false);
            }

            chkHideIgnoredSeasons.Checked = false;
        }
    }
}
