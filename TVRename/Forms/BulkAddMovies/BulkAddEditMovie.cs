//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Drawing;

namespace TVRename;

using System;
using System.Windows.Forms;

public partial class BulkAddEditMovie : Form
{
    public int Code;
    public TVDoc.ProviderType Provider;

    private readonly CodeFinder codeFinderControl;

    public BulkAddEditMovie(PossibleNewMovie hint)
    {
        codeFinderControl = new MovieCodeFinder("", TVSettings.Instance.DefaultMovieProvider) { Dock = DockStyle.Fill };
        InitializeComponent();

        codeFinderControl.SelectionChanged += CodeChanged;
        codeFinderControl.lvMatches.DoubleClick += MatchDoubleClick;

        label1.Text = $"Search for {TVSettings.Instance.DefaultMovieProvider} entry, by partial name or ID:";

        pnlCF.SuspendLayout();
        pnlCF.Controls.Add(codeFinderControl);
        pnlCF.ResumeLayout();

        if (hint.CodeKnown)
        {
            codeFinderControl.SetHint(hint.ProviderCode.ToString(), hint.SourceProvider);
        }
        else
        {
            codeFinderControl.SetHint(string.IsNullOrWhiteSpace(hint.RefinedHint)
                ? hint.Directory.Name
                : hint.RefinedHint, TVSettings.Instance.DefaultMovieProvider);
        }
        Code = -1;
        Provider = TVDoc.ProviderType.libraryDefault;
    }
    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        base.ScaleControl(factor, specified);
        codeFinderControl.lvMatches.ScaleListViewColumns(factor);
    }
    private void MatchDoubleClick(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Code = codeFinderControl.SelectedCode();
        Provider = codeFinderControl.Source;
        Close();
    }

    private static void CodeChanged(object? sender, EventArgs e)
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
        Provider = codeFinderControl.Source;
        Close();
    }
}
