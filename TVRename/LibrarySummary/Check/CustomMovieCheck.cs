namespace TVRename;

internal abstract class CustomMovieCheck(MovieConfiguration movie, TVDoc doc) : MovieCheck(movie, doc)
{
    protected override string MovieCheckName => FieldName;
    protected abstract string FieldName { get; }
    protected abstract bool Field { get; }
    protected abstract string? CustomFieldValue { get; }
    protected abstract string? DefaultFieldValue { get; }

    public override bool Check() => Field;

    public override string Explain() => $"{FieldName} is enabled for this Movie [{CustomFieldValue}], by default it is not [{DefaultFieldValue}].";
}
