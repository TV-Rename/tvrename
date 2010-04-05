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
            this.mDoc = doc;
            this.Ignore = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in this.mDoc.Ignore)
                this.Ignore.Add(ii);

            this.InitializeComponent();

            this.FillList();
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            this.mDoc.Ignore = this.Ignore;
            this.mDoc.SetDirty();
            this.Close();
        }

        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            foreach (int i in this.lbItems.SelectedIndices)
                this.Ignore.Remove(this.DisplayedSet[i]);
            this.FillList();
        }

        private void FillList()
        {
            this.lbItems.BeginUpdate();
            this.lbItems.Items.Clear();

            string f = this.txtFilter.Text.ToLower();
            bool all = string.IsNullOrEmpty(f);

            this.DisplayedSet = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in this.Ignore)
            {
                string s = ii.FileAndPath;
                if (all || s.ToLower().Contains(f))
                {
                    this.lbItems.Items.Add(s);
                    this.DisplayedSet.Add(ii);
                }
            }

            this.lbItems.EndUpdate();
        }

        private void txtFilter_TextChanged(object sender, System.EventArgs e)
        {
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            this.timer1.Stop();
            this.FillList();
        }
    }
}