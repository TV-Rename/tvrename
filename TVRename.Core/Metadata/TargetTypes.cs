using System;

namespace TVRename.Core.Metadata
{
    /// <summary>
    /// Type of metadata image.
    /// </summary>
    [Flags]
    public enum TargetTypes
    {
        Show = 1,
        Season = 2,
        Episode = 4
    }
}
