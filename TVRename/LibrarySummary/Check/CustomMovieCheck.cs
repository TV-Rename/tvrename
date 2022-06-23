namespace TVRename;

internal abstract class CustomMovieCheck : MovieCheck
{
    protected CustomMovieCheck(MovieConfiguration movie, TVDoc doc) : base(movie, doc)
    {
    }

    protected override string MovieCheckName => FieldName;
    protected abstract string FieldName { get; }
    protected abstract bool Field { get; }
    protected abstract string CustomFieldValue { get; }
    protected abstract string DefaultFieldValue { get; }

    public override bool Check() => Field;

    public override string Explain() => $"{FieldName} is enabled for this Movie [{CustomFieldValue}], by default is is not [{DefaultFieldValue}].";
}
