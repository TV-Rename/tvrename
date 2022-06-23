namespace TVRename;

internal abstract class DefaultMovieCheck : MovieCheck
{
    protected DefaultMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

    protected override string MovieCheckName => FieldName;
    protected abstract string FieldName { get; }
    protected abstract bool Field { get; }
    protected abstract bool Default { get; }

    public override bool Check() => Field != Default;

    public override string Explain() => $"Default value for '{FieldName}' is {Default}. For this Movie it is {Field}.";
}
