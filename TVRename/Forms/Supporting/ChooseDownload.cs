using System.Collections.Generic;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename.Forms.Supporting
{
    public partial class ChooseDownload : Form
    {
        public ChooseDownload([NotNull] ItemMissing im, IEnumerable<ActionTDownload> options)
        {
            InitializeComponent();
            lblEpisodeName.Text = $"{im.Show.ShowName} - {im}";
            if (im is MovieItemMissing mim)
            {
                lblEpisodeName.Text = $"{mim.MovieConfig.ShowNameWithYear}";
            }
            olvSize.AspectToStringConverter = delegate (object x)
            {
                long sizeBytes = (long)x;

                return $"{(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}";
            };
            olvChooseDownload.SetObjects(options);
            SetButtonVisiblity();
            olvChooseDownload.Sort(olvSeeders, SortOrder.Descending);
        }

        public ActionTDownload? UserChosenAction
        {
            get;
            private set;
        }

        private void OlvChooseDownload_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SetButtonVisiblity();
        }

        private void SetButtonVisiblity()
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
