using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename.Forms.Supporting
{
    public partial class ChooseDownload : Form
    {
        public ChooseDownload(ItemMissing im, IEnumerable<ActionTDownload> options)
        {
            InitializeComponent();
            lblEpisodeName.Text = $"{im.Show.ShowName} - {im}";
            olvSize.AspectToStringConverter = delegate (object x) {
                long sizeBytes = (long)x;

                return $"{(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}";
            };
            olvChooseDownload.SetObjects(options);
            SetButtonVisiblity();
            olvChooseDownload.Sort(olvSeeders,SortOrder.Descending);
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
            UserChosenAction = (ActionTDownload)olvChooseDownload.SelectedObject;
            DialogResult = olvChooseDownload.SelectedObject != null ? DialogResult.OK : DialogResult.Cancel;
        }

        private void bnCancelAll_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
