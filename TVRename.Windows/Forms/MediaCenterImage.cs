using System;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Humanizer;
using TVRename.Core.Metadata;
using TVRename.Core.Metadata.Identifiers;

namespace TVRename.Windows.Forms
{
    public partial class MediaCenterImage : Form
    {
        public ImageIdentifier Identifier { get; private set; }

        public MediaCenterImage(ImageIdentifier identifier) : this()
        {
            this.comboBoxType.SelectedItem = identifier.ImageType.Humanize();
            this.comboBoxFormat.SelectedItem = identifier.ImageFormat.ToString().ToUpperInvariant();
            this.textBoxName.Text = identifier.FileName;
            this.textBoxLocation.Text = identifier.Location;
        }

        public MediaCenterImage()
        {
            InitializeComponent();
        }

        private void MediaCenterAdd_Load(object sender, EventArgs e)
        {
            if (this.comboBoxType.SelectedIndex == -1) this.comboBoxType.SelectedIndex = 0;
            if (this.comboBoxFormat.SelectedIndex == -1) this.comboBoxFormat.SelectedIndex = 0;
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            ValidatePaths();
        }

        private void textBoxLocation_TextChanged(object sender, EventArgs e)
        {
            ValidatePaths();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Identifier = new TVDBImageIdentifier
            {
                ImageType = this.comboBoxType.SelectedItem.ToString().DehumanizeTo<ImageType>(),
                ImageFormat = this.comboBoxFormat.SelectedIndex == 0 ? ImageFormat.Jpeg : ImageFormat.Png,
                FileName = this.textBoxName.Text,
                Location = this.textBoxLocation.Text
            };

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ValidatePaths()
        {
            this.buttonOk.Enabled = false;

            if (string.IsNullOrWhiteSpace(this.textBoxName.Text) || string.IsNullOrWhiteSpace(this.textBoxLocation.Text)) return;

            this.buttonOk.Enabled = true;
        }
    }
}
