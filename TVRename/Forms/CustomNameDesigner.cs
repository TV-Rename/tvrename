// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System.Collections.Generic;
using System.Windows.Forms;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;



namespace TVRename
{
    /// <summary>
    /// Summary for CustomNameDesigner
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class CustomNameDesigner : Form
    {
        private readonly CustomEpisodeName CN;
        private readonly List<ProcessedEpisode> Eps;
        private TVDoc mDoc;

        public CustomNameDesigner(List<ProcessedEpisode> pel, CustomEpisodeName cn, TVDoc doc)
        {
            Eps = pel;
            CN = cn;
            mDoc = doc;

            InitializeComponent();

            if (Eps == null)
                lvTest.Enabled = false;
            txtTemplate.Text = CN.StyleString;

            FillExamples();
            FillCombos();
        }

        private void FillCombos()
        {
            cbTags.Items.Clear();
            cbPresets.Items.Clear();
            ProcessedEpisode pe = null;
            if (lvTest.SelectedItems.Count == 0)
                pe = ((Eps != null) && (Eps.Count > 0)) ? Eps[0] : null;
            else
                pe = (ProcessedEpisode) (lvTest.SelectedItems[0].Tag);

            foreach (string s in CustomEpisodeName.Tags)
            {
                string txt = s;
                if (pe != null)
                    txt += " - " + CustomEpisodeName.NameForNoExt(pe, s);
                cbTags.Items.Add(txt);
            }

            foreach (string s in CustomEpisodeName.Presets)
            {
                cbPresets.Items.Add(pe != null ? CustomEpisodeName.NameForNoExt(pe, s) : s);
            }
        }

        private void FillExamples()
        {
            if (Eps == null)
                return;

            lvTest.Items.Clear();
            foreach (ProcessedEpisode pe in Eps)
            {
                ListViewItem lvi = new ListViewItem();
                string fn = TVSettings.Instance.FilenameFriendly(CN.NameFor(pe));
                lvi.Text = fn;

                bool ok = TVDoc.FindSeasEp(new FileInfo(fn + ".avi"), out int seas, out int ep, out int maxEp, pe.SI);
                bool ok1 = ok && (seas == pe.AppropriateSeasonNumber);
                bool ok2 = ok && (ep == pe.AppropriateEpNum);
                string pre1 = ok1 ? "" : "* ";
                string pre2 = ok2 ? "" : "* ";

                lvi.SubItems.Add(pre1 + ((seas != -1) ? seas.ToString() : ""));
                lvi.SubItems.Add(pre2 + ((ep != -1) ? ep.ToString() : "") + (maxEp != -1 ? "-" + maxEp : ""));

                
                lvi.Tag = pe;

                if (!ok || !ok1 || !ok2)
                    lvi.BackColor = Helpers.WarningColor();
                lvTest.Items.Add(lvi);
            }
        }

        private void cbPresets_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int n = cbPresets.SelectedIndex;
            if (n == -1)
                return;

            txtTemplate.Text = CustomEpisodeName.Presets[n];
            cbPresets.SelectedIndex = -1;
        }

        private void txtTemplate_TextChanged(object sender, System.EventArgs e)
        {
            CN.StyleString = txtTemplate.Text;
            FillExamples();
        }

        private void cbTags_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int n = cbTags.SelectedIndex;
            if (n == -1)
                return;

            int p = txtTemplate.SelectionStart;
            string s = txtTemplate.Text;
            txtTemplate.Text = s.Substring(0, p) + CustomEpisodeName.Tags[cbTags.SelectedIndex] + s.Substring(p);

            cbTags.SelectedIndex = -1;
        }

        private void lvTest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FillCombos();
        }
    }
}
