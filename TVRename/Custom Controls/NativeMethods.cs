using System;
using System.Runtime.InteropServices;

namespace TVRename
{
    internal static partial class NativeMethods
    {
        // MAH: Added in support of the Filter TextBox Button
        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }
}