//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRename;

/// <summary>
/// Summary for CustomNameTagsFloatingWindow
///
/// WARNING: If you change the name of this class, you will need to change the
///          'Resource File Name' property for the managed resource compiler tool
///          associated with all .resx files this class depends on.  Otherwise,
///          the designers will not be able to interact properly with localized
///          resources associated with this form.
/// </summary>
public partial class CustomNameTagsFloatingWindow : Form
{
    public CustomNameTagsFloatingWindow(ProcessedEpisode? pe)
    {
        InitializeComponent(CustomEpisodeName.TAGS, pe != null, s => CustomEpisodeName.NameForNoExt(pe!, s));
    }

    public CustomNameTagsFloatingWindow(ProcessedSeason? pe)
    {
        InitializeComponent(CustomSeasonName.TAGS, pe != null, s => CustomSeasonName.NameFor(pe!, s));
    }

    public CustomNameTagsFloatingWindow(MovieConfiguration? movie)
    {
        InitializeComponent(CustomMovieName.TAGS, movie != null, s => CustomMovieName.NameFor(movie, s));
    }

    public CustomNameTagsFloatingWindow(ShowConfiguration? tvShow)
    {
        InitializeComponent(CustomTvShowName.TAGS, tvShow != null, s => CustomTvShowName.NameFor(tvShow, s));
    }

    private void InitializeComponent(List<string> tags, bool showExamples, Func<string, string> createExampleFromTagFunc)
    {
        InitializeComponent();

        foreach (string s in tags)
        {
            if (showExamples)
            {
                label1.Text += $"{s} - {createExampleFromTagFunc(s)}\r\n";
            }
            else
            {
                label1.Text += $"{s}\r\n";
            }
        }
    }
}
