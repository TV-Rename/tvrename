// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
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
        private CustomName _cn;
        private List<ProcessedEpisode> _eps;
        private TVDoc _mDoc;

        public CustomNameDesigner(List<ProcessedEpisode> pel, CustomName cn, TVDoc doc)
        {
            _eps = pel;
            _cn = cn;
            _mDoc = doc;

            InitializeComponent();

            if (_eps == null)
                lvTest.Enabled = false;
            txtTemplate.Text = _cn.StyleString;

            FillExamples();
            FillCombos();
        }

        private void FillCombos()
        {
            cbTags.Items.Clear();
            cbPresets.Items.Clear();
            ProcessedEpisode pe;
            if (lvTest.SelectedItems.Count == 0)
                pe = ((_eps != null) && (_eps.Count > 0)) ? _eps[0] : null;
            else
                pe = (ProcessedEpisode) (lvTest.SelectedItems[0].Tag);

            foreach (string s in CustomName.Tags)
            {
                string txt = s;
                if (pe != null)
                    txt += " - " + CustomName.NameForNoExt(pe, s);
                cbTags.Items.Add(txt);
            }

            foreach (string s in CustomName.Presets)
            {
                cbPresets.Items.Add(pe != null ? CustomName.NameForNoExt(pe, s) : s);
            }
        }

        private void FillExamples()
        {
            if (_eps == null)
                return;

            lvTest.Items.Clear();
            foreach (ProcessedEpisode pe in _eps)
            {
                ListViewItem lvi = new ListViewItem();
                string fn = TVSettings.Instance.FilenameFriendly(_cn.NameForExt(pe, null, 0));
                lvi.Text = fn;

                bool ok = false;
                bool ok1 = false;
                bool ok2 = false;
                if (fn.Length < 255)
                {
                    int seas;
                    int ep;
                    ok = TVDoc.FindSeasEp(new FileInfo(fn + ".avi"), out seas, out ep, pe.Si);
                    ok1 = ok && (seas == pe.SeasonNumber);
                    ok2 = ok && (ep == pe.EpNum);
                    string pre1 = ok1 ? "" : "* ";
                    string pre2 = ok2 ? "" : "* ";

                    lvi.SubItems.Add(pre1 + ((seas != -1) ? seas.ToString() : ""));
                    lvi.SubItems.Add(pre2 + ((ep != -1) ? ep.ToString() : ""));
                    lvi.Tag = pe;
                }
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

            txtTemplate.Text = CustomName.Presets[n];
            cbPresets.SelectedIndex = -1;
        }

        private void txtTemplate_TextChanged(object sender, System.EventArgs e)
        {
            _cn.StyleString = txtTemplate.Text;
            FillExamples();
        }

        private void cbTags_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            int n = cbTags.SelectedIndex;
            if (n == -1)
                return;

            int p = txtTemplate.SelectionStart;
            string s = txtTemplate.Text;
            txtTemplate.Text = s.Substring(0, p) + CustomName.Tags[cbTags.SelectedIndex] + s.Substring(p);

            cbTags.SelectedIndex = -1;
        }

        private void lvTest_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FillCombos();
        }
    }
}
