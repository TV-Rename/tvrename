using System.Runtime.InteropServices;

namespace TVRename;

internal static partial class NativeMethods
{
    /// <summary>
    /// Gets the properties for this file system.
    /// </summary>
    /// <param name="volumeIdentifier">The path whose volume properties are to be queried.</param>
    /// <returns>A <see cref="FileSystemProperties"/> containing the properties for the specified file system.</returns>
    public static FileSystemProperties GetProperties(string volumeIdentifier)
    {
        if (NativeMethods.GetDiskFreeSpaceEx(volumeIdentifier, out ulong available, out ulong total, out ulong free))
        {
            return new FileSystemProperties((long)total, (long)free, (long)available);
        }
        return new FileSystemProperties(null, null, null);
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(uint dwProcessId);

    private const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

    // Attach to console window – this may modify the standard handles
    public static bool AttachParentConsole() => AttachConsole(ATTACH_PARENT_PROCESS);
}
