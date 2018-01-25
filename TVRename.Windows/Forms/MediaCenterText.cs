using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TVRename.Core.Metadata;
using TVRename.Core.Metadata.Identifiers;

namespace TVRename.Windows.Forms
{
    public partial class MediaCenterText : Form
    {
        private readonly List<TextIdentifier> identifiers = new List<TextIdentifier> {
            new KodiIdentifier(),
            new PyTivoIdentifier(),
            new Mede8erIdentifier(),
            new Mede8erViewIdentifier()
        };

        public TextIdentifier Identifier { get; private set; }

        public MediaCenterText(TextIdentifier identifier) : this()
        {
            this.comboBoxTarget.SelectedIndex = (int)identifier.Target;

            for (int i = 0; i < this.comboBoxType.Items.Count; i++)
            {
                if (this.comboBoxType.Items[i].ToString() != identifier.ToString()) continue;

                this.comboBoxType.SelectedIndex = i;
                break;
            }

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
        }

        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboBoxType.Items.Clear();

            TargetTypes target = 0;

            switch (this.comboBoxTarget.SelectedIndex)
            {
                case 0:
                    target = TargetTypes.Show;
                    break;
                case 1:
                    target = TargetTypes.Season;
                    break;
                case 2:
                    target = TargetTypes.Episode;
                    break;
            }

            this.comboBoxType.Items.AddRange(this.identifiers.Where(i => i.SupportedTypes.HasFlag(target)).Cast<object>().ToArray());

            this.comboBoxType.SelectedIndex = 0;
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
