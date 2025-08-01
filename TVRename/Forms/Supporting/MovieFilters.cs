//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using NLog;
using System;
using System.Linq;
using System.Windows.Forms;

namespace TVRename.Forms;

public partial class MovieFilters : Form
{
    private readonly TVDoc doc;
    private const string IS_NOT = "is not";
    private const string IS = "is";
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public MovieFilters(TVDoc doc)
    {
        this.doc = doc;
        InitializeComponent();

        try
        {
            clbGenre.Items.AddRange([.. doc.FilmLibrary.GetGenres().Cast<object>()]);

            cmbNetwork.Items.Add(string.Empty);
            cmbNetwork.Items.AddRange([.. doc.FilmLibrary.GetNetworks().Cast<object>()]);

            cmbShowStatus.Items.Add(string.Empty);
            cmbShowStatus.Items.AddRange([.. doc.FilmLibrary.GetStatuses().Cast<object>()]);

            cmbRating.Items.Add(string.Empty);
            cmbRating.Items.AddRange([.. doc.FilmLibrary.GetContentRatings().Cast<object>()]);

            cmbYear.Items.Add(string.Empty);
            cmbYear.Items.AddRange([.. doc.FilmLibrary.GetYears().Cast<object>()]);
        }
        catch (InvalidCastException ex)
        {
            Logger.Error(ex);
        }
        SetButtonStates();
    }

    private void SetButtonStates()
    {
        MovieFilter filter = TVSettings.Instance.MovieFilter;
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

            //Filter By Show Network
            bool filterByShowYear = filter.ShowYear != null;
            cmbYear.SelectedItem = filterByShowYear ? filter.ShowYear : string.Empty;

            //Filter By Show Status
            cmbShowStatusType.SelectedItem = filter.ShowStatusInclude ? IS : IS_NOT;

            //Filter By Show Rating
            cmbRatingType.SelectedItem = filter.ShowRatingInclude ? IS : IS_NOT;

            //Filter By Show Network
            cmbNetworkType.SelectedItem = filter.ShowNetworkInclude ? IS : IS_NOT;

            //Filter By Show Network
            cmbYearType.SelectedItem = filter.ShowYearInclude ? IS : IS_NOT;

            chkIncludeBlanks.Checked = filter.IncludeBlankFields;

            //Filter By Genre
            foreach (string genre in filter.Genres)
            {
                int genreIndex = clbGenre.Items.IndexOf(genre);
                if (genreIndex > 0)
                {
                    clbGenre.SetItemChecked(genreIndex, true);
                }
            }
        }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
        MovieFilter filter = TVSettings.Instance.MovieFilter;

        filter.ShowName = string.IsNullOrEmpty(tbShowName.Text) ? null : tbShowName.Text;
        filter.ShowStatus = string.IsNullOrEmpty(cmbShowStatus.Text) ? null : cmbShowStatus.SelectedItem?.ToString();
        filter.ShowNetwork = string.IsNullOrEmpty(cmbNetwork.Text) ? null : cmbNetwork.SelectedItem?.ToString();
        filter.ShowRating = string.IsNullOrEmpty(cmbRating.Text) ? null : cmbRating.SelectedItem?.ToString();
        filter.ShowYear = string.IsNullOrEmpty(cmbYear.Text) ? null : cmbYear.SelectedItem?.ToString();

        filter.ShowStatusInclude = GetIncludeStatus(cmbShowStatusType);
        filter.ShowNetworkInclude = GetIncludeStatus(cmbNetworkType);
        filter.ShowRatingInclude = GetIncludeStatus(cmbRatingType);
        filter.ShowYearInclude = GetIncludeStatus(cmbYearType);

        filter.IncludeBlankFields = chkIncludeBlanks.Checked;

        filter.Genres.Clear();
        foreach (string genre in clbGenre.CheckedItems)
        {
            filter.Genres.Add(genre);
        }

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

        if (IS_NOT.Equals(comboBox.SelectedItem.ToString()))
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
        cmbYear.SelectedItem = string.Empty;

        cmbShowStatusType.SelectedItem = IS;
        cmbNetworkType.SelectedItem = IS;
        cmbRatingType.SelectedItem = IS;
        cmbYearType.SelectedItem = IS;

        clbGenre.ClearSelected();

        for (int i = 0; i < clbGenre.Items.Count; i++)
        {
            clbGenre.SetItemChecked(i, false);
        }
    }
}
