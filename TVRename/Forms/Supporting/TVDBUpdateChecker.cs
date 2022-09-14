using System;
using System.Linq;
using System.Windows.Forms;

namespace TVRename.Forms.Supporting
{
    public partial class TvdbUpdateChecker : Form
    {
        public long? TimeSince;
        public MediaConfiguration? SelectedMedia;
        public TvdbUpdateChecker(TVDoc doc)
        {
            InitializeComponent();

            comboBoxShow.DataSource = doc.TvLibrary.GetSortedShowItems().Where(s=>s.Provider==TVDoc.ProviderType.TheTVDB).ToArray();
            comboBoxShow.DisplayMember = "Name";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            TimeSince = dateTimePicker.Value.ToUnixTime();
            SelectedMedia = comboBoxShow.SelectedItem as MediaConfiguration;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void comboBoxShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            MediaConfiguration? currentShow = comboBoxShow.SelectedItem as MediaConfiguration;
            dateTimePicker.Value = currentShow?.CachedData?.SrvLastUpdated.FromUnixTime().ToLocalTime() ?? TimeHelpers.LocalNow();
        }
    }
}
