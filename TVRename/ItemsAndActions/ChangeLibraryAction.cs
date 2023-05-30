using System.Threading;
using System;

namespace TVRename;

internal abstract class ChangeLibraryAction : Action
{
    protected readonly TVDoc Doc;

    protected ChangeLibraryAction(TVDoc doc)
    {
        Doc = doc;
    }
    public override string ScanListViewGroup => "lvgActionOther";
    public override long SizeOfWork => 1;
    public override IgnoreItem? Ignore => null;
    public override string? TargetFolder => null;
    public override int IconNumber => 9;
    public override string DestinationFolder => Produces;
    public override string DestinationFile => Produces;
    public override string ProgressText => Produces;
}

internal class RemoveMovie : ChangeLibraryAction
{
    public RemoveMovie(MovieConfiguration si, TVDoc doc) : base(doc)
    {
        Movie = si;
    }

    public override int CompareTo(Item? o)
    {
        if (o is not RemoveMovie r)
        {
            return -1;
        }

        return string.Compare(this.Movie?.Name, r.Movie?.Name, StringComparison.Ordinal);
    }

    public override bool SameAs(Item o) =>
        o is RemoveMovie cmr && Movie == cmr.Movie;

    public override string Name => "Remove Movie Configuration";

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        if (Movie != null)
        {
            Doc.FilmLibrary.Remove(Movie);
            Doc.MoviesAddedOrEdited(false, true, true, null, Movie);
        }
        return ActionOutcome.Success();
    }

    public override string Produces => Movie?.Name ?? string.Empty;
}

internal class RemoveShow : ChangeLibraryAction
{
    private readonly ShowConfiguration si;

    public RemoveShow(ShowConfiguration si, TVDoc doc) : base(doc)
    {
        this.si = si;
    }
    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        Doc.TvLibrary.Remove(si);
        Doc.TvAddedOrEdited(false, true, true, null, si);
        return ActionOutcome.Success();
    }
    public override string Produces => si.Name ?? string.Empty;

    public override int CompareTo(Item? o)
    {
        if (o is not RemoveShow r)
        {
            return -1;
        }

        return string.Compare(this.si.Name, r.si.Name, StringComparison.Ordinal);
    }

    public override bool SameAs(Item o) =>
        o is RemoveShow cmr && si == cmr.si;

    public override string Name => "Remove TV Show Configuration";

    public override ShowConfiguration Series => si;
    public override string SeriesName => si.ShowName;
}

