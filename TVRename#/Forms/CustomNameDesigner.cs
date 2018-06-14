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
        private CustomName CN;
        private List<ProcessedEpisode> Eps;
        private TVDoc mDoc;

        public CustomNameDesigner(List<ProcessedEpisode> pel, CustomName cn, TVDoc doc)
        {
            this.Eps = pel;
            this.CN = cn;
            this.mDoc = doc;

            this.InitializeComponent();

            if (this.Eps == null)
                this.lvTest.Enabled = false;
            this.txtTemplate.Text = this.CN.StyleString;

            this.FillExamples();
            this.FillCombos();
        }

        private void FillCombos()
        {
            this.cbTags.Items.Clear();
            this.cbPresets.Items.Clear();
            ProcessedEpisode pe = null;
            if (this.lvTest.SelectedItems.Count == 0)
                pe = ((this.Eps != null) && (this.Eps.Count > 0)) ? this.Eps[0] : null;
            else
                pe = (ProcessedEpisode) (this.lvTest.SelectedItems[0].Tag);

            foreach (string s in CustomName.Tags)
            {
                string txt = s;
                if (pe != null)
                    txt += " - " + CustomName.NameForNoExt(pe, s);
                this.cbTags.Items.Add(txt);
            }

            foreach (string s in CustomName.Presets)
            {
                this.cbPresets.Items.Add(pe != null ? CustomName.NameForNoExt(pe, s) : s);
            }
        }

        private void FillExamples()
        {
            if (this.Eps == null)
                return;

            this.lvTest.Items.Clear();
            foreach (ProcessedEpisode pe in this.Eps)
            {
                ListViewItem lvi = new ListViewItem();
                string fn = TVSettings.Instance.FilenameFriendly(this.CN.NameFor(pe));
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
                this.lvTest.Items.Add(lvi);
            }
        }

        private void cbPresets_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int n = this.cbPresets.SelectedIndex;
            if (n == -1)
                return;

            this.txtTemplate.Text = CustomName.Presets[n];
            this.cbPresets.SelectedIndex = -1;
        }

        private void txtTemplate_TextChanged(object sender, System.EventArgs e)
        {
            this.CN.StyleString = this.txtTemplate.Text;
            this.FillExamples();
        }

        private void cbTags_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int n = this.cbTags.SelectedIndex;
            if (n == -1)
                return;

            int p = this.txtTemplate.SelectionStart;
            string s = this.txtTemplate.Text;
            this.txtTemplate.Text = s.Substring(0, p) + CustomName.Tags[this.cbTags.SelectedIndex] + s.Substring(p);

            this.cbTags.SelectedIndex = -1;
        }

        private void lvTest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.FillCombos();
        }
    }
}
