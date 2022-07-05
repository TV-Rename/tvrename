//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System;
using System.Windows.Forms;

// Control for searching for a source provider code, checking against local cache and
// searching on various providers

namespace TVRename;

public abstract partial class CodeFinder : UserControl
{
    private MediaConfiguration.MediaType Type { get; }
    internal TVDoc.ProviderType Source { get; private set; }
    private bool hasChanged;
    private bool mInternal;
    private readonly ListViewColumnSorter lvwCodeFinderColumnSorter;

    private const string DEFAULT_MESSAGE = "Enter the show's name, and click \"Search\"";
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    protected CodeFinder(string? initialHint, MediaConfiguration.MediaType type, TVDoc.ProviderType source)
    {
        Type = type;
        Source = source;
        mInternal = false;
        hasChanged = false;

        InitializeComponent();

        txtFindThis.Text = initialHint;

        if (!initialHint.HasValue())
        {
            ListViewItem lvi = new(string.Empty);
            lvi.SubItems.Add(DEFAULT_MESSAGE);
            lvMatches.Items.Add(lvi);
        }

        lvwCodeFinderColumnSorter = new ListViewColumnSorter(new DoubleAsTextSorter(5))
        {
            Order = SortOrder.Descending
        };

        lvMatches.ListViewItemSorter = lvwCodeFinderColumnSorter;

        label3.Text = GetPromptLabel(Source);
    }

    public void SetSource(TVDoc.ProviderType source) => SetSource(source, null);

    public void SetSource(TVDoc.ProviderType source, MediaConfiguration? mi)
    {
        UpdateSource(source);
        if (txtFindThis.Text.IsNumeric() && mi != null && mi.IdFor(source) > 0)
        {
            mInternal = true;
            txtFindThis.Text = GenerateNewHintForProvider(mi);
            mInternal = false;
            DoFind(false);
        }
        else if (txtFindThis.Text.IsNumeric() && mi != null && mi.Name.HasValue())
        {
            mInternal = true;
            txtFindThis.Text = GenerateNewHintForProvider(mi);
            mInternal = false;
            DoFind(true);
        }
        else
        {
            DoFind(false);
        }
    }

    private void UpdateSource(TVDoc.ProviderType source)
    {
        if (source == TVDoc.ProviderType.libraryDefault)
        {
            Source = Type == MediaConfiguration.MediaType.movie
                ? TVSettings.Instance.DefaultMovieProvider
                : TVSettings.Instance.DefaultProvider;
        }
        else
        {
            Source = source;
        }

        label3.Text = GetPromptLabel(Source);
    }

    private string GenerateNewHintForProvider(MediaConfiguration mi)
    {
        return mi.IdFor(Source) > 0 ? mi.IdFor(Source).ToString() : mi.ShowName;
    }

    public event EventHandler<EventArgs>? SelectionChanged;

    public bool SetHint(string s, TVDoc.ProviderType provider)
    {
        mInternal = true;
        txtFindThis.Text = s;
        SetSource(provider);
        mInternal = false;
        Search(true);
        return DoFind(true);
    }

    public CachedMovieInfo? SelectedMovie() => SelectedObject() as CachedMovieInfo;

    public CachedSeriesInfo? SelectedShow() => SelectedObject() as CachedSeriesInfo;

    private object? SelectedObject()
    {
        try
        {
            return lvMatches.SelectedItems.Count == 0 ? null : lvMatches.SelectedItems[0].Tag;
        }
        catch
        {
            return null;
        }
    }

    public int SelectedCode()
    {
        try
        {
            return lvMatches.SelectedItems.Count == 0
                ? int.Parse(txtFindThis.Text)
                : int.Parse(lvMatches.SelectedItems[0].SubItems[0].Text);
        }
        catch
        {
            return -1;
        }
    }

    private void txtFindThis_TextChanged(object sender, EventArgs e)
    {
        hasChanged = true;
        if (!mInternal && txtFindThis.Text.Length > 2)
        {
            DoFind(false);
        }
    }

    private bool DoFind(bool chooseOnlyMatch)
    {
        if (mInternal)
        {
            return false;
        }

        try
        {
            mInternal = true;
            lvMatches.BeginUpdate();

            if (!txtFindThis.Text.HasValue() && lvMatches.Items.Count == 1 && lvMatches.Items[0].SubItems[1].Text == DEFAULT_MESSAGE)
            {
                //we have no further information
                return false;
            }
            string what = txtFindThis.Text.CompareName().RemoveYearFromEnd();

            int matchedMedia = 0;

            lvMatches.Items.Clear();
            if (!string.IsNullOrEmpty(what))
            {
                bool numeric = int.TryParse(what, out int matchnum);
                MediaCache cache = TVDoc.GetMediaCache(Source);

                matchedMedia = FindMedia(cache, numeric, matchnum, what);

                if (lvMatches.Items.Count == 1 && numeric)
                {
                    lvMatches.Items[0].Selected = true;
                }
            }
            else
            {
                txtSearchStatus.Text = string.Empty;
            }

            if (matchedMedia == 1 && chooseOnlyMatch)
            {
                lvMatches.Items[0].Selected = true;
                return true;
            }

            return false;
        }
        finally
        {
            lvMatches.EndUpdate();
            mInternal = false;
        }
    }

    protected abstract int FindMedia(MediaCache cache, bool numeric, int matchnum, string what);

    protected static bool Matches(int num, CachedMediaInfo kvp, bool numeric, string what, int matchnum)
    {
        string show = kvp.Name.CompareName();

        string s = num + " " + show;
        string simpleWhat = what.CompareName();
        bool textMatch = !numeric && s.Contains(simpleWhat);

        bool numberMatch = numeric && num == matchnum;
        bool numberTextMatch = numeric && show.Contains(what);

        return numberMatch || textMatch || numberTextMatch;
    }

    private void bnGoSearch_Click(object sender, EventArgs e)
    {
        Search(true);
    }

    private void Search(bool showErrorMsgBox)
    {
        // search on site
        txtSearchStatus.Text = GetLabel(Source);
        txtSearchStatus.Update();

        if (string.IsNullOrEmpty(txtFindThis.Text))
        {
            return;
        }

        try
        {
            //TODO - make search multi language and use custom language specified

            GetSourceInstance(Source).Search(txtFindThis.Text, showErrorMsgBox, Type, new Locale());
        }
        catch (SourceConnectivityException scx)
        {
            Logger.Warn(scx);
        }
        DoFind(true);
    }

    private string GetLabel(TVDoc.ProviderType source)
    {
        return source switch
        {
            TVDoc.ProviderType.TMDB => "Searching on TMDB.com",
            TVDoc.ProviderType.TheTVDB => "Searching on TheTVDB.com",
            TVDoc.ProviderType.TVmaze => "Searching on TVmaze.com",
            TVDoc.ProviderType.libraryDefault => GetLabel(DefaultType),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private TVDoc.ProviderType DefaultType => MediaConfiguration.MediaType.movie == Type
        ? TVSettings.Instance.DefaultMovieProvider
        : TVSettings.Instance.DefaultProvider;

    private string GetPromptLabel(TVDoc.ProviderType source)
    {
        return source switch
        {
            TVDoc.ProviderType.TMDB => "TMDB &code:",
            TVDoc.ProviderType.TheTVDB => "TheTVDB &code:",
            TVDoc.ProviderType.TVmaze => "TVmaze &code:",
            TVDoc.ProviderType.libraryDefault => GetPromptLabel(DefaultType),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private MediaCache GetSourceInstance(TVDoc.ProviderType source)
    {
        return source switch
        {
            TVDoc.ProviderType.TMDB => TMDB.LocalCache.Instance,
            TVDoc.ProviderType.TheTVDB => TheTVDB.LocalCache.Instance,
            TVDoc.ProviderType.TVmaze => TVmaze.LocalCache.Instance,
            TVDoc.ProviderType.libraryDefault => GetSourceInstance(DefaultType),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private void lvMatches_SelectedIndexChanged(object sender, EventArgs e)
    {
        hasChanged = true;
        SelectionChanged?.Invoke(sender, e);
    }

    private void txtFindThis_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Enter or Keys.Return)
        {
            Search(true);

            e.Handled = true;
        }
    }

    private void lvMatches_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        lvwCodeFinderColumnSorter.ClickedOn(e.Column);
        lvwCodeFinderColumnSorter.ListViewItemSorter = GetSorter(e.Column);
        lvMatches.Sort();
    }

    private static ListViewItemSorter GetSorter(int eColumn) =>
        eColumn switch
        {
            0 => new NumberAsTextSorter(eColumn), // code
            2 => new NumberAsTextSorter(eColumn), // year
            5 => new DoubleAsTextSorter(eColumn), //  popularity
            _ => new TextSorter(eColumn)
        };
}
