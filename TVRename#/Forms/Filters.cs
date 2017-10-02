using System;
using System.Windows.Forms;

namespace TVRename.Forms
{
    public partial class Filters : Form
    {
        private TVDoc doc;

        public Filters(TVDoc doc)
        {
            InitializeComponent( doc);
            this.doc = doc;
            setButtonStates();
        }

        private void setButtonStates()
        {
            ShowFilter filter = TVSettings.Instance.Filter;
            if (filter != null)
            {
                //Filter By Show Names
                Boolean filterByShowNames = filter.ShowName != null;
                cbShowName.Checked = filterByShowNames;
                tbShowName.Enabled = filterByShowNames;
                tbShowName.Text = (filterByShowNames ? filter.ShowName : "");

                //Filter By Show Status
                Boolean filterByShowStatus = filter.ShowStatus != null;
                cbShowStatus.Checked = filterByShowStatus;
                cmbShowStatus.Enabled = filterByShowStatus;
                cmbShowStatus.SelectedItem = (filterByShowStatus ? filter.ShowStatus : "Continuing");

                //Filter By Genre
                Boolean filterByGenre = filter.Genres.Count != 0;
                cbGenre.Checked = filterByGenre;
                clbGenre.Enabled = filterByGenre;
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
            filter.ShowName = null;
            if (cbShowName.Checked)
            {
                filter.ShowName = tbShowName.Text;
            }

            //Filter By Show Status
            filter.ShowStatus = null;
            if (cbShowStatus.Checked)
            {
                filter.ShowStatus = cmbShowStatus.SelectedItem.ToString();
            }

            //Filter By Genre
            filter.Genres.Clear();
            if (cbGenre.Checked)
            {
                foreach (String genre in clbGenre.CheckedItems)
                {
                    filter.Genres.Add(genre);
                }
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

        private void cbShowStatus_CheckedChanged(object sender, EventArgs e)
        {
            this.cmbShowStatus.Enabled = this.cbShowStatus.Checked;
        }

        private void cbShowName_CheckedChanged(object sender, EventArgs e)
        {
            this.tbShowName.Enabled = this.cbShowName.Checked;
        }

        private void cbGenre_CheckedChanged(object sender, EventArgs e)
        {
            this.clbGenre.Enabled = this.cbGenre.Checked;
        }

        public bool filtersApplied()
        {
            return cbShowName.Enabled || cbShowStatus.Enabled || cbGenre.Enabled;
        }
    }
}
