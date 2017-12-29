// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
using System.Windows.Forms;

namespace TVRename
{
    /// <summary>
    /// Summary for IgnoreEdit
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public partial class IgnoreEdit : Form
    {
        private System.Collections.Generic.List<IgnoreItem> DisplayedSet;
        private System.Collections.Generic.List<IgnoreItem> Ignore;
        private TVDoc mDoc;

        public IgnoreEdit(TVDoc doc)
        {
            mDoc = doc;
            Ignore = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in mDoc.Ignore)
                Ignore.Add(ii);

            InitializeComponent();

            FillList();
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            mDoc.Ignore = Ignore;
            mDoc.SetDirty();
            Close();
        }

        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            foreach (int i in lbItems.SelectedIndices)
                foreach (IgnoreItem iitest in Ignore)
                    if (lbItems.Items[i].ToString().Equals(iitest.FileAndPath))
                    {
                        Ignore.Remove(iitest);
                        break;
                    }
            FillList();
        }

        private void FillList()
        {
            lbItems.BeginUpdate();
            lbItems.Items.Clear();

            string f = txtFilter.Text.ToLower();
            bool all = string.IsNullOrEmpty(f);

            DisplayedSet = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in Ignore)
            {
                string s = ii.FileAndPath;
                if (all || s.ToLower().Contains(f))
                {
                    lbItems.Items.Add(s);
                    DisplayedSet.Add(ii);
                }
            }

            lbItems.EndUpdate();
        }

        private void txtFilter_TextChanged(object sender, System.EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            timer1.Stop();
            FillList();
        }
    }
}
