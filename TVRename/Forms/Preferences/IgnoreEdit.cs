// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
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
        private readonly System.Collections.Generic.List<IgnoreItem> ignore;
        private readonly TVDoc mDoc;

        public IgnoreEdit(TVDoc doc)
        {
            mDoc = doc;
            ignore = new System.Collections.Generic.List<IgnoreItem>();

            foreach (IgnoreItem ii in TVSettings.Instance.Ignore)
                ignore.Add(ii);

            InitializeComponent();

            FillList();
        }

        private void bnOK_Click(object sender, System.EventArgs e)
        {
            TVSettings.Instance.Ignore = ignore;
            mDoc.SetDirty();
            Close();
        }

        private void bnRemove_Click(object sender, System.EventArgs e)
        {
            foreach (int i in lbItems.SelectedIndices)
                foreach (IgnoreItem iitest in ignore)
                    if (lbItems.Items[i].ToString().Equals(iitest.FileAndPath))
                    {
                        ignore.Remove(iitest);
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

            foreach (IgnoreItem ii in ignore)
            {
                string s = ii.FileAndPath;
                if (all || s.ToLower().Contains(f))
                {
                    lbItems.Items.Add(s);
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
