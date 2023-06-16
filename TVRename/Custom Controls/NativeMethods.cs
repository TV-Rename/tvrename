using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TVRename;

internal static partial class NativeMethods
{
    // MAH: Added in support of the Filter TextBox Button
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    public static void StopTextDisappearing(TextBox tb, Control filterButton)
    {
        // Send EM_SETMARGINS to prevent text from disappearing underneath the button
        SendMessage(tb.Handle, 0xd3, (IntPtr)2, (IntPtr)(filterButton.Width << 16));
    }
}
