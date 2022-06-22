using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename.Forms.ShowPreferences
{
    public partial class NewSeenEpisode : Form
    {
        public ProcessedEpisode? ChosenEpisode;

        public NewSeenEpisode(IEnumerable<ProcessedEpisode> eps)
        {
            InitializeComponent();

            comboBox1.BeginUpdate();
            comboBox1.Items.Clear();
            foreach (ProcessedEpisode ep in eps)
            {
                comboBox1.Items.Add(ep);
            }

            comboBox1.EndUpdate();
        }

        private void BnOK_Click(object sender, EventArgs e)
        {
            ChosenEpisode = (ProcessedEpisode)comboBox1.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
