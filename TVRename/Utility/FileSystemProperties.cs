//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

public class FileSystemProperties(long? totalBytes, long? freeBytes, long? availableBytes)
{

    /// <summary>
    /// Gets the total number of bytes on the drive.
    /// </summary>
    public long? TotalBytes { get; } = totalBytes;

    /// <summary>
    /// Gets the number of bytes free on the drive.
    /// </summary>
    public long? FreeBytes { get; } = freeBytes;

    /// <summary>
    /// Gets the number of bytes available on the drive (counts disk quotas).
    /// </summary>
    public long? AvailableBytes { get; } = availableBytes;
}
