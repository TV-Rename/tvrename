using System.Runtime.InteropServices;

namespace TVRename
{
    internal static partial class NativeMethods
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(uint dwProcessId);
        private const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

        // Attach to console window – this may modify the standard handles
        public static bool AttachParentConsole() =>AttachConsole(ATTACH_PARENT_PROCESS);
    }
}
