using System;
using System.Windows.Forms;
using TVRename.Core.Metadata;
using TVRename.Core.Metadata.Identifiers;

namespace TVRename.Windows.Forms
{
    public partial class MediaCenterText : Form
    {
        public TextIdentifier Identifier { get; private set; }

        public MediaCenterText(TextIdentifier identifier) : this()
        {
            switch (identifier)
            {
                case KodiIdentifier _:
                    this.comboBoxType.SelectedIndex = 0;
                    break;
                case PyTivoIdentifier _:
                    this.comboBoxType.SelectedIndex = 1;
                    break;
                case Mede8erIdentifier _: // TODO: View
                    this.comboBoxType.SelectedIndex = 2;
                    break;
                case Mede8erViewIdentifier _:
                    this.comboBoxType.SelectedIndex = 3;
                    break;
            }

            this.comboBoxTarget.SelectedIndex = (int)identifier.Target;
            this.textBoxName.Text = identifier.FileName;
            this.textBoxLocation.Text = identifier.Location;
        }
        
        public MediaCenterText()
        {
            InitializeComponent();
        }

        private void MediaCenterText_Load(object sender, EventArgs e)
        {
            if (this.comboBoxTarget.SelectedIndex == -1) this.comboBoxTarget.SelectedIndex = 0;
            if (this.comboBoxType.SelectedIndex == -1) this.comboBoxType.SelectedIndex = 0;
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
            switch (this.comboBoxType.SelectedIndex)
            {
                case 0:
                    this.Identifier = new KodiIdentifier();
                    break;
                case 1:
                    this.Identifier = new PyTivoIdentifier();
                    break;
                case 2:
                    this.Identifier = new Mede8erIdentifier();
                    break;
                case 3:
                    this.Identifier = new Mede8erViewIdentifier();
                    break;
            }

            this.Identifier.Target = (Target)this.comboBoxTarget.SelectedIndex;
            this.Identifier.FileName = this.textBoxName.Text;
            this.Identifier.Location = this.textBoxLocation.Text;

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
