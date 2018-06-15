using System;
using System.Linq;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class Filters : Form
    {
        private readonly TVDoc doc;

        public Filters(TVDoc doc)
        {
            this.doc = doc;
            InitializeComponent();
            clbGenre.Items.AddRange(doc.Library.getGenres().Cast<object>().ToArray());
            cmbNetwork.Items.AddRange(doc.Library.getNetworks().Cast<object>().ToArray());
            cmbShowStatus.Items.AddRange(doc.Library.getStatuses().Cast<object>().ToArray());
            cmbRating.Items.AddRange(doc.Library.GetContentRatings().Cast<object>().ToArray());

            SetButtonStates();
        }

        private void SetButtonStates()
        {
            ShowFilter filter = TVSettings.Instance.Filter;
            if (filter != null)
            {
                //Filter By Show Names
                bool filterByShowNames = filter.ShowName != null;
                tbShowName.Text = (filterByShowNames ? filter.ShowName : "");

                //Filter By Show Status
                bool filterByShowStatus = filter.ShowStatus != null;
                cmbShowStatus.SelectedItem = (filterByShowStatus ? filter.ShowStatus : "");

                //Filter By Show Rating
                bool filterByShowRating = filter.ShowRating != null;
                cmbRating.SelectedItem = (filterByShowRating ? filter.ShowRating : "");

                //Filter By Show Network
                bool filterByShowNetwork = filter.ShowNetwork != null;
                cmbNetwork.SelectedItem = (filterByShowNetwork ? filter.ShowNetwork : "");


                //Filter By Genre
                foreach (string genre in filter.Genres)
                {
                    int genreIndex = clbGenre.Items.IndexOf(genre);
                    clbGenre.SetItemChecked(genreIndex, true);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ShowFilter filter = TVSettings.Instance.Filter;

            //Filter By Show Name

            filter.ShowName = string.IsNullOrEmpty(tbShowName.Text) ? null : tbShowName.Text;
            filter.ShowStatus = string.IsNullOrEmpty(cmbShowStatus.Text) ? null : cmbShowStatus.SelectedItem.ToString();
            filter.ShowNetwork  = string.IsNullOrEmpty(cmbNetwork.Text) ? null : cmbNetwork.SelectedItem.ToString();
            filter.ShowRating = string.IsNullOrEmpty(cmbRating.Text) ? null : cmbRating.SelectedItem.ToString();

            filter.Genres.Clear();
            foreach (string genre in clbGenre.CheckedItems)
            {
                filter.Genres.Add(genre);
            }

            doc.SetDirty();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void bnReset_Click(object sender, EventArgs e)
        {
            tbShowName.Text = "";
            cmbShowStatus.SelectedItem = "";
            cmbNetwork.SelectedItem = "";
            cmbRating.SelectedItem = "";
            clbGenre.ClearSelected();

            for (int i = 0; i < clbGenre.Items.Count; i++)
            {
                clbGenre.SetItemChecked(i, false);
            }
        }

    }
}

