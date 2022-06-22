using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename.Forms.Supporting
{
    public partial class ChooseDownload : Form
    {
        public ChooseDownload(ItemMissing im, IEnumerable<ActionTDownload> options)
        {
            InitializeComponent();
            lblEpisodeName.Text = $"{im.Show.ShowName.ToUiVersion()} - {im}";
            if (im is MovieItemMissing mim)
            {
                lblEpisodeName.Text = $"{mim.MovieConfig.ShowNameWithYear.ToUiVersion()}";
            }
            olvSize.AspectToStringConverter = delegate (object x)
            {
                long sizeBytes = (long)x;

                return $"{(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}";
            };
            olvChooseDownload.SetObjects(options);
            SetButtonVisibility();
            olvChooseDownload.Sort(olvSeeders, SortOrder.Descending);
        }

        public ActionTDownload? UserChosenAction
        {
            get;
            private set;
        }

        private void OlvChooseDownload_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SetButtonVisibility();
        }

        private void SetButtonVisibility()
        {
            btnOK.Enabled = olvChooseDownload.SelectedObject != null;
        }

        private void BtnOK_Click(object sender, System.EventArgs e)
        {
            Ok();
        }

        private void Ok()
        {
            UserChosenAction = (ActionTDownload)olvChooseDownload.SelectedObject;
            DialogResult = olvChooseDownload.SelectedObject != null ? DialogResult.OK : DialogResult.Cancel;
        }

        private void bnCancelAll_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void olvChooseDownload_DoubleClick(object sender, System.EventArgs e)
        {
            Ok();
        }
    }
}
