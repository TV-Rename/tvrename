using System.Collections.Generic;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace TVRename.Forms.Supporting
{
    public partial class ChooseDownload : Form
    {
        public ChooseDownload([NotNull] ProcessedEpisode pe, IEnumerable<ActionTDownload> options)
        {
            InitializeComponent();
            lblEpisodeName.Text = $"{pe.Show.ShowName} - {pe}";
            olvSize.AspectToStringConverter = delegate (object x) {
                long sizeBytes = (long)x;

                return $"{(sizeBytes < 0 ? "N/A" : sizeBytes.GBMB())}";
            };
            olvChooseDownload.SetObjects(options);
            SetButtonVisiblity();
        }

        public ActionTDownload UserChosenAction
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
    }
}
