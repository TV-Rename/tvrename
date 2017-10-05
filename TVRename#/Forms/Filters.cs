using System;
using System.Linq;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class Filters : Form
    {
        private TVDoc doc;

        public Filters(TVDoc doc)
        {
            this.doc = doc;
            InitializeComponent();
            this.clbGenre.Items.AddRange(doc.getGenres().Cast<object>().ToArray());
            this.cmbNetwork.Items.AddRange(doc.getNetworks().Cast<object>().ToArray());
            this.cmbShowStatus.Items.AddRange(doc.getStatuses().Cast<object>().ToArray());
            this.cmbRating.Items.AddRange(doc.GetRatings().Cast<object>().ToArray());

            setButtonStates();
        }

        private void setButtonStates()
        {
            ShowFilter filter = TVSettings.Instance.Filter;
            if (filter != null)
            {
                //Filter By Show Names
                Boolean filterByShowNames = filter.ShowName != null;
                tbShowName.Text = (filterByShowNames ? filter.ShowName : "");

                //Filter By Show Status
                Boolean filterByShowStatus = filter.ShowStatus != null;
                cmbShowStatus.SelectedItem = (filterByShowStatus ? filter.ShowStatus : "");

                //Filter By Show Rating
                Boolean filterByShowRating = filter.ShowRating != null;
                cmbRating.SelectedItem = (filterByShowRating ? filter.ShowRating : "");

                //Filter By Show Network
                Boolean filterByShowNetwork = filter.ShowNetwork != null;
                cmbNetwork.SelectedItem = (filterByShowNetwork ? filter.ShowNetwork : "");


                //Filter By Genre
                foreach (String genre in filter.Genres)
                {
                    int genre_index = clbGenre.Items.IndexOf(genre);
                    clbGenre.SetItemChecked(genre_index, true);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ShowFilter filter = TVSettings.Instance.Filter;

            //Filter By Show Name

            filter.ShowName = String.IsNullOrEmpty(tbShowName.Text) ? null : tbShowName.Text;
            filter.ShowStatus = String.IsNullOrEmpty(cmbShowStatus.Text) ? null : cmbShowStatus.SelectedItem.ToString();
            filter.ShowNetwork  = String.IsNullOrEmpty(cmbNetwork.Text) ? null : cmbNetwork.SelectedItem.ToString();
            filter.ShowRating = String.IsNullOrEmpty(cmbRating.Text) ? null : cmbRating.SelectedItem.ToString();

            filter.Genres.Clear();
            foreach (String genre in clbGenre.CheckedItems)
            {
                filter.Genres.Add(genre);
            }

            this.doc.SetDirty();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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

