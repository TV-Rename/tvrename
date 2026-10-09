//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.Forms.Supporting
{
    public partial class TvdbUpdateChecker : Form
    {
        public long? TimeSince;
        public MediaConfiguration? SelectedMedia;
        public TvdbUpdateChecker(TVDoc doc)
        {
            InitializeComponent();

            comboBoxShow.DataSource = doc.TvLibrary.GetSortedShows().Where(s => s.Provider == TVDoc.ProviderType.TheTVDB).ToArray();
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
