namespace TVRename
{
    /// <summary>
    /// Summary for MissingFolderAction
    ///
    /// WARNING: If you change the name of this class, you will need to change the
    ///          'Resource File Name' property for the managed resource compiler tool
    ///          associated with all .resx files this class depends on.  Otherwise,
    ///          the designers will not be able to interact properly with localized
    ///          resources associated with this form.
    /// </summary>
    public enum FaResult
    {
        kfaNotSet,
        kfaRetry,
        kfaCancel,
        kfaCreate,
        kfaIgnoreOnce,
        kfaIgnoreAlways,
        kfaDifferentFolder
    }
}