namespace TVRename;

internal abstract class DefaultTvShowCheck : TvShowCheck
{
    protected DefaultTvShowCheck(ShowConfiguration show, TVDoc doc) : base(show, doc)
    {
    }

    public override string CheckName => "[TV] " + FieldName;
    protected abstract string FieldName { get; }
    protected abstract bool Field { get; }
    protected abstract bool Default { get; }

    public override bool Check() => Field != Default;

    public override string Explain() => $"Default value for '{FieldName}' is {Default}. For this TV Show it is {Field}.";
}
