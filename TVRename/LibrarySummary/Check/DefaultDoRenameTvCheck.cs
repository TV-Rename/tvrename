namespace TVRename;

internal class DefaultDoRenameTvCheck(ShowConfiguration show, TVDoc doc) : DefaultTvShowCheck(show, doc)
{
    protected override string FieldName => "Rename Check";

    protected override bool Field => Show.DoRename;

    protected override bool Default => TVSettings.Instance.DefShowDoRenaming;

    protected override void FixInternal()
    {
        Show.DoRename = Default;
    }
}
