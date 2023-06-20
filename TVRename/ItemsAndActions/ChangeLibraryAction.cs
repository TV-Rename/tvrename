using System.Threading;
using System;

namespace TVRename;

internal abstract class ActionChangeLibrary : Action
{
    protected readonly TVDoc Doc;

    protected ActionChangeLibrary(TVDoc doc)
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
    public override QueueName Queue() => QueueName.writeMetadata;
}

internal class ActionChangeLibraryRemoveMovie : ActionChangeLibrary, IEquatable<ActionChangeLibraryRemoveMovie>
{
    public ActionChangeLibraryRemoveMovie(MovieConfiguration si, TVDoc doc) : base(doc)
    {
        Movie = si;
    }

    public override string Produces => Movie?.Name ?? string.Empty;
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

    #region EqualMethods
    public override int CompareTo(Item? o)
    {
        if (o is not ActionChangeLibraryRemoveMovie r)
        {
            return -1;
        }

        return string.Compare(Movie?.Name, r.Movie?.Name, StringComparison.Ordinal);
    }

    public override bool SameAs(Item o) =>
        o is ActionChangeLibraryRemoveMovie cmr && Movie == cmr.Movie;
   
    public override bool Equals(object? obj) => obj is ActionChangeLibraryRemoveMovie rs && Equals(rs);
    public bool Equals(ActionChangeLibraryRemoveMovie? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || SameAs(other);
    }

    public override int GetHashCode() => HashCode.Combine(Movie);
    #endregion
}

internal class ActionChangeLibraryRemoveShow : ActionChangeLibrary, IEquatable<ActionChangeLibraryRemoveShow>
{
    private readonly ShowConfiguration si;

    public ActionChangeLibraryRemoveShow(ShowConfiguration si, TVDoc doc) : base(doc)
    {
        this.si = si;
    }

    public override string Name => "Remove TV Show Configuration";
    public override string Produces => si.Name ?? string.Empty;
    public override ShowConfiguration Series => si;
    public override string SeriesName => si.ShowName;

    public override ActionOutcome Go(TVRenameStats stats, CancellationToken cancellationToken)
    {
        Doc.TvLibrary.Remove(si);
        Doc.TvAddedOrEdited(false, true, true, null, si);
        return ActionOutcome.Success();
    }

    #region EqualMethods
    public override int CompareTo(Item? o)
    {
        if (o is not ActionChangeLibraryRemoveShow r)
        {
            return -1;
        }

        return string.Compare(si.Name, r.si.Name, StringComparison.Ordinal);
    }

    public override bool SameAs(Item o) =>
        o is ActionChangeLibraryRemoveShow cmr && si == cmr.si;

    public override bool Equals(object? obj) => obj is ActionChangeLibraryRemoveShow rs && Equals(rs);

    public bool Equals(ActionChangeLibraryRemoveShow? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && si.Equals(other.si);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), si);
    #endregion
}

