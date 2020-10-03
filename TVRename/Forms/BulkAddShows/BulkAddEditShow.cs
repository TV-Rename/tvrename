// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using JetBrains.Annotations;

namespace TVRename
{
    using System;
    using System.Windows.Forms;

    public partial class FolderMonitorEdit : Form
    {
        public int Code;

        private readonly TheTvdbCodeFinder codeFinderControl;

        public FolderMonitorEdit([NotNull] PossibleNewTvShow hint)
        {
            InitializeComponent();

            codeFinderControl = new TheTvdbCodeFinder("") {Dock = DockStyle.Fill};
            codeFinderControl.SelectionChanged += CodeChanged;
            codeFinderControl.lvMatches.DoubleClick += MatchDoubleClick;

            pnlCF.SuspendLayout();
            pnlCF.Controls.Add(codeFinderControl);
            pnlCF.ResumeLayout();

            if (hint.CodeKnown)
            {
                codeFinderControl.SetHint(hint.TVDBCode.ToString());
            }
            else
            {
                string s = hint.Folder.FullName;
                int p = s.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                codeFinderControl.SetHint(string.IsNullOrWhiteSpace(hint.RefinedHint)
                    ? s.Substring(p + 1)
                    : hint.RefinedHint);
            }
            Code = -1;
        }

        private void MatchDoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Code = codeFinderControl.SelectedCode();
            Close();
        }

        private static void CodeChanged(object sender, EventArgs e)
        {
            //Nothing to do
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Code = codeFinderControl.SelectedCode();
            Close();
        }
    }
}
