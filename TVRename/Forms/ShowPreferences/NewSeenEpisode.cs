//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename.Forms.ShowPreferences;

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
        ChosenEpisode = (ProcessedEpisode?)comboBox1.SelectedItem;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}
