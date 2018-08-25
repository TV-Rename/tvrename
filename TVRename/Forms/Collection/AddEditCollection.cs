using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVRename
{
    public partial class AddEditCollection : Form
    {
        private static AddEditCollection aecf;
        private readonly TVDoc  mDoc;
        private int             iColl = -1;
        private bool            bActualMode;
        private bool            bActualButtons;
        private bool            bAddMode;

        public AddEditCollection(TVDoc doc)
        {
            InitializeComponent();
            mDoc           = doc;
            aecf           = this;
            bActualMode    = SetTextBoxes(false, true);
            bActualButtons = SetButtons(false, true);

            FillCollTreeView();
        }

        private static bool SetTextBoxes(bool bMode, bool bFull = false)
        {
            if (bFull)
            {
                aecf.TxtCollPath.Enabled = bMode;
            }
            aecf.TxtCollName.Enabled = bMode;
            aecf.TxtCollDesc.Enabled = bMode;

            return bMode;
        }

        private static bool SetButtons(bool bMode, bool bFull = false)
        {
            aecf.BtEdit.Enabled = bMode;
            aecf.BtDel.Enabled = bMode;
            aecf.BtUp.Enabled = bMode;
            aecf.BtDown.Enabled = bMode;
            if (bFull)
            {
                aecf.BtSave.Enabled = bMode;
                aecf.BtCancel.Enabled = bMode;
            }
            return bMode;
        }

        private static void ClearTextBoxes ()
        {
            aecf.TxtCollName.Text = "";
            aecf.TxtCollPath.Text = "";
            aecf.TxtCollDesc.Text = "";
        }

        private static void FillTextBoxes (ShowCollection Collection)
        {
            aecf.TxtCollName.Text = Collection.Name;
            aecf.TxtCollPath.Text = Collection.Path;
            aecf.TxtCollDesc.Text = Collection.Description;
        }

        private static ShowCollection FillCollectionFromTextBoxes ()
        {
            ShowCollection Collection = new ShowCollection(aecf.TxtCollPath.Text);
            Collection.Name = aecf.TxtCollName.Text;
            Collection.Description = aecf.TxtCollDesc.Text;

            return Collection;
        }

        private static void FillCollTreeView()
        {
            aecf.TvColl.Nodes.Clear();
            if (aecf.mDoc.ShowCollections.Count > 0)
            {
                TreeNode RootNode;
                RootNode = aecf.TvColl.Nodes.Add("Root");
                int iCurr = 0;
                foreach (ShowCollection ShowColl in aecf.mDoc.ShowCollections)
                {
                    TreeNode CurNode;
                    CurNode = RootNode.Nodes.Add(ShowColl.Name);
                    CurNode.ToolTipText = ShowColl.Description;
                    CurNode.Tag = iCurr;
                    iCurr++;
                }
                aecf.TvColl.ExpandAll();
            }
        }

        #region Control functions
        private void TvColl_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode Node = TvColl.SelectedNode;
            string[] Nodelevels = Node.FullPath.Split('\\');

            if (Nodelevels.Length == 2)
            {
                iColl = (int)Node.Tag;
                FillTextBoxes(mDoc.ShowCollections[iColl]);
                bActualButtons = SetButtons(true);
            }
        }

        private void BtDel_Click(object sender, EventArgs e)
        {
            if (iColl != -1)
            {
                mDoc.ShowCollections.RemoveAt(iColl);
            }

            mDoc.WriteXMLFile(FileToHandle.Collections);
            ClearTextBoxes();
            bActualButtons = SetButtons(false, true);
            iColl = -1;
            FillCollTreeView();
        }

        private void BtEdit_Click(object sender, EventArgs e)
        {
            if (iColl > 0)
            {
                bActualButtons = SetButtons(true, true);
                bActualButtons = SetButtons(false);
                bActualMode = SetTextBoxes(true);
                BtAdd.Enabled = false;
                bAddMode = false;
                TvColl.Enabled = false;
            }
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            ShowCollection Sc = FillCollectionFromTextBoxes();
            if (bAddMode)
            {
                mDoc.ShowCollections.Add(Sc);
            }
            else
            {
                mDoc.ShowCollections[iColl] = Sc;
            }

            mDoc.WriteXMLFile(FileToHandle.Collections);

            ClearTextBoxes();
            FillCollTreeView();

            TvColl.Enabled = true;
            bActualMode    = SetTextBoxes(false, true);
            bActualButtons = SetButtons(false, true);
            iColl = -1;
            BtAdd.Enabled  = true;
        }

        private void BtAdd_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            bActualButtons = SetButtons(true, true);
            bActualButtons = SetButtons(false);
            bActualMode    = SetTextBoxes(true, true);
            BtAdd.Enabled  = false;
            iColl          = -1;
            bAddMode       = true;
            TvColl.Enabled = false;
        }

        private void BtOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            bActualMode    = SetTextBoxes(false, true);
            bActualButtons = SetButtons(false, true);
            BtAdd.Enabled  = true;
            bAddMode       = false;
            iColl          = -1;
            FillCollTreeView();
            TvColl.Enabled = true;
        }

        private void BtUp_Click(object sender, EventArgs e)
        {
            ShowCollection Sc;
            // No swap over the Default collection
            if (iColl > 1)
            {
                Sc = mDoc.ShowCollections[iColl];
                mDoc.ShowCollections[iColl] = mDoc.ShowCollections[iColl - 1];
                mDoc.ShowCollections[iColl - 1] = Sc;
            }
            iColl = -1;
            FillCollTreeView();
        }

        private void BtDown_Click(object sender, EventArgs e)
        {
            ShowCollection Sc;
            // No Swap after last collection
            if (iColl < mDoc.ShowCollections.Count - 1)
            {
                Sc = mDoc.ShowCollections[iColl];
                mDoc.ShowCollections[iColl] = mDoc.ShowCollections[iColl + 1];
                mDoc.ShowCollections[iColl + 1] = Sc;
            }
            iColl = -1;
            FillCollTreeView();
        }
        #endregion
    }
}
