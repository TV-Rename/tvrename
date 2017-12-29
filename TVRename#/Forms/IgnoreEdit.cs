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
        private System.Collections.Generic.List<IgnoreItem> _displayedSet;
        private readonly System.Collections.Generic.List<IgnoreItem> _ignore;
        private readonly TVDoc _mDoc;

        public IgnoreEdit(TVDoc doc)
        {
            _mDoc = doc;
            _ignore = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in _mDoc.Ignore)
                _ignore.Add(ii);

            InitializeComponent();

            FillList();
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            _mDoc.Ignore = _ignore;
            _mDoc.SetDirty();
            Close();
        }

        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            foreach (int i in lbItems.SelectedIndices)
                foreach (IgnoreItem iitest in _ignore)
                    if (lbItems.Items[i].ToString().Equals(iitest.FileAndPath))
                    {
                        _ignore.Remove(iitest);
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

            _displayedSet = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in _ignore)
            {
                string s = ii.FileAndPath;
                if (all || s.ToLower().Contains(f))
                {
                    lbItems.Items.Add(s);
                    _displayedSet.Add(ii);
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
