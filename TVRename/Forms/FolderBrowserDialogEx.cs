// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace DaveChambers.FolderBrowserDialogEx
{
    public class FolderBrowserDialogEx
    {
        #region Fields that mimic the same-named fields in FolderBrowserDialog
        public Environment.SpecialFolder RootFolder { get; set; }
        public string SelectedPath { get; set; }
        public bool ShowNewFolderButton { get; set; }
        public FormStartPosition StartPosition { get; set; }
        #endregion

        // Fields specific to CustomFolderBrowserDialog
        public string Title { get; set; }
        public bool ShowEditbox { get; set; }

        // These are the control IDs used in the dialog
        private struct CtlIds
        {
            public const int PATH_EDIT = 0x3744;
            //public const int PATH_EDIT_LABEL = 0x3748;    // Only when BIF_NEWDIALOGSTYLE
            public const int TITLE = 0x3742;
            public const int TREEVIEW = 0x3741;
            public const int NEW_FOLDER_BUTTON = 0x3746;
            public const int IDOK = 1;
            public const int IDCANCEL = 2;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct InitData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]   // Titles shouldn't too long, should they?
            public string Title;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
            public string InitialPath;

            public bool ShowEditbox;
            public bool ShowNewFolderButton;
            public FormStartPosition StartPosition;
            public IntPtr hParent;

            public InitData(FolderBrowserDialogEx dlg, IntPtr hParent)
            {
                // We need to make copies of these values from the dialog.
                // I tried passing the dlg obj itself in this struct, but Windows will barf after repeated invocations.
                Title = dlg.Title;
                InitialPath = dlg.SelectedPath;
                ShowNewFolderButton = dlg.ShowNewFolderButton;
                ShowEditbox = dlg.ShowEditbox;
                StartPosition = dlg.StartPosition;
                this.hParent = hParent;
            }
        }

        public FolderBrowserDialogEx()
        {
            Title = "Browse For Folder";  // Default to same caption as std dialog
            RootFolder = Environment.SpecialFolder.Desktop;
            SelectedPath = @"c:\";
            ShowEditbox = false;
            ShowNewFolderButton = false;
            StartPosition = FormStartPosition.WindowsDefaultLocation;
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            InitData initdata = new InitData(this, owner.Handle);

            Win32.BROWSEINFO bi = new Win32.BROWSEINFO();
            bi.iImage = 0;
            bi.hwndOwner = owner.Handle;
            if (0 != Win32.SHGetSpecialFolderLocation(owner.Handle, (int)RootFolder, ref bi.pidlRoot))
                bi.pidlRoot = IntPtr.Zero;
            bi.lpszTitle = "";
            bi.ulFlags = Win32.BIF_RETURNONLYFSDIRS;    // do NOT use BIF_NEWDIALOGSTYLE or BIF_STATUSTEXT
            if (ShowEditbox)
                bi.ulFlags |= Win32.BIF_EDITBOX;
            if (!ShowNewFolderButton)
                bi.ulFlags |= Win32.BIF_NONEWFOLDERBUTTON;
            bi.lpfn = _browseCallbackHandler;
            // Initialization data, used in _browseCallbackHandler
            IntPtr hInit = Marshal.AllocHGlobal(Marshal.SizeOf(initdata));
            Marshal.StructureToPtr(initdata, hInit, true);
            bi.lParam = hInit;

            IntPtr pidlSelectedPath = IntPtr.Zero;
            try
            {
                pidlSelectedPath = Win32.SHBrowseForFolder(ref bi);
                StringBuilder sb = new StringBuilder(256);
                if (Win32.SHGetPathFromIDList(pidlSelectedPath, sb))
                {
                    SelectedPath = sb.ToString();
                    return DialogResult.OK;
                }
            }
            finally
            {
                // Caller is responsible for freeing this memory.
                Marshal.FreeCoTaskMem(pidlSelectedPath);
            }

            return DialogResult.Cancel;
        }

        private int _browseCallbackHandler(IntPtr hDlg, int msg, IntPtr lParam, IntPtr lpData)
        {
            if (msg == Win32.BFFM_INITIALIZED)
            {
                // remove context help button from dialog caption
                int lStyle = Win32.GetWindowLong(hDlg, Win32.GWL_STYLE);
                lStyle &= ~Win32.DS_CONTEXTHELP;
                Win32.SetWindowLong(hDlg, Win32.GWL_STYLE, lStyle);
                lStyle = Win32.GetWindowLong(hDlg, Win32.GWL_EXSTYLE);
                lStyle &= ~Win32.WS_EX_CONTEXTHELP;
                Win32.SetWindowLong(hDlg, Win32.GWL_EXSTYLE, lStyle);

                _adjustUi(hDlg, lpData);
            }
            else if (msg == Win32.BFFM_SELCHANGED)
            {
                bool ok = false;
                StringBuilder sb = new StringBuilder(Win32.MAX_PATH);
                if (Win32.SHGetPathFromIDList(lParam, sb))
                {
                    ok = true;
                    string dir = sb.ToString();
                    IntPtr hEdit = Win32.GetDlgItem(hDlg, CtlIds.PATH_EDIT);
                    Win32.SetWindowText(hEdit, dir);
#if UsingStatusText
// We're not using status text, but if we were, this is how you'd set it
                            Win32.SendMessage(hDlg, Win32.BFFM_SETSTATUSTEXTW, 0, dir);
#endif

#if SHBrowseForFolder_lists_links
// This check doesn't seem to be necessary - the SHBrowseForFolder dirtree doesn't seem to list links
                            Win32.SHFILEINFO sfi = new Win32.SHFILEINFO();
                            Win32.SHGetFileInfo(lParam, 0, ref sfi, Marshal.SizeOf(sfi), Win32.SHGFI_PIDL | Win32.SHGFI_ATTRIBUTES);

                            // fail if pidl is a link
                            if ((sfi.dwAttributes & Win32.SFGAO_LINK) == Win32.SFGAO_LINK)
                                ok = false;
#endif
                }

                // if invalid selection, disable the OK button
                if (!ok)
                    Win32.EnableWindow(Win32.GetDlgItem(hDlg, CtlIds.IDOK), false);
            }

            return 0;
        }

        private void _adjustUi(IntPtr hDlg, IntPtr lpData)
        {
            // Only do the adjustments if InitData was supplied
            if (lpData == IntPtr.Zero)
                return;
            object obj = Marshal.PtrToStructure(lpData, typeof(InitData));
            if (obj == null)
                return;
            InitData initdata = (InitData)obj;

            // Only do the adjustments if we can find the dirtree control
            IntPtr hTree = Win32.GetDlgItem(hDlg, CtlIds.TREEVIEW);
            if (hTree == IntPtr.Zero)
            {
                hTree = Win32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "SysTreeView32", IntPtr.Zero);
                if (hTree == IntPtr.Zero)
                {
                    // This usually means that BIF_NEWDIALOGSTYLE is enabled.
                    hTree = Win32.FindWindowEx(hDlg, IntPtr.Zero, "SHBrowseForFolder ShellNameSpace Control", IntPtr.Zero);
                }
            }
            if (hTree == IntPtr.Zero)
                return;

            // Prep the basic UI
            Win32.SendMessage(hDlg, Win32.BFFM_SETSELECTIONW, 1, initdata.InitialPath);
            Win32.SetWindowText(hDlg, initdata.Title);

            if (initdata.StartPosition == FormStartPosition.CenterParent)
            {
                _centerTo(hDlg, initdata.hParent);
            }
            else if (initdata.StartPosition == FormStartPosition.CenterScreen)
            {
                _centerTo(hDlg, Win32.GetDesktopWindow());
            }
            // else we do nothing

            // Prep the edit box
            Win32.RECT rcEdit = new Win32.RECT();
            IntPtr hEdit = Win32.GetDlgItem(hDlg, CtlIds.PATH_EDIT);
            if (hEdit != IntPtr.Zero)
            {
                if (initdata.ShowEditbox)
                {
                    Win32.GetWindowRect(hEdit, out rcEdit);
                    Win32.ScreenToClient(hEdit, ref rcEdit);
                }
                else
                {
                    Win32.ShowWindow(hEdit, Win32.SW_HIDE);
                }
            }

            // make the dialog larger
            Win32.RECT rcDlg;
            Win32.GetWindowRect(hDlg, out rcDlg);
            rcDlg.Right += 40;
            rcDlg.Bottom += 30;
            if (hEdit != IntPtr.Zero)
                rcDlg.Bottom += (rcEdit.Height + 5);
            Win32.MoveWindow(hDlg, rcDlg, true);
            Win32.GetClientRect(hDlg, out rcDlg);

            int vMargin = 10;
            // Accomodate the resizing handle's width
            int hMargin = 10; // SystemInformation.VerticalScrollBarWidth;

            // Move the Cancel button
            Win32.RECT rcCancel = new Win32.RECT();
            IntPtr hCancel = Win32.GetDlgItem(hDlg, CtlIds.IDCANCEL);
            if (hCancel != IntPtr.Zero)
            {
                Win32.GetWindowRect(hCancel, out rcCancel);
                Win32.ScreenToClient(hDlg, ref rcCancel);

                rcCancel = new Win32.RECT(rcDlg.Right - (rcCancel.Width + hMargin),
                                            rcDlg.Bottom - (rcCancel.Height + vMargin),
                                            rcCancel.Width,
                                            rcCancel.Height);

                Win32.MoveWindow(hCancel, rcCancel, false);
            }

            // Move the OK button
            Win32.RECT rcOK = new Win32.RECT();
            IntPtr hOK = Win32.GetDlgItem(hDlg, CtlIds.IDOK);
            if (hOK != IntPtr.Zero)
            {
                Win32.GetWindowRect(hOK, out rcOK);
                Win32.ScreenToClient(hDlg, ref rcOK);

                rcOK = new Win32.RECT(rcCancel.Left - (rcCancel.Width + hMargin),
                                        rcCancel.Top,
                                        rcOK.Width,
                                        rcOK.Height);

                Win32.MoveWindow(hOK, rcOK, false);
            }

            // Manage the "Make New Folder" button
            IntPtr hBtn = Win32.GetDlgItem(hDlg, CtlIds.NEW_FOLDER_BUTTON);
            if (!initdata.ShowNewFolderButton)
            {
                // Make sure this button is not visible
                Win32.ShowWindow(hBtn, Win32.SW_HIDE);
            }
            else if (hBtn == IntPtr.Zero)
            {
                // Create a button - button is only auto-created under BIF_NEWDIALOGSTYLE
                // This is failing, and I don't know why!
                Win32.CreateWindowEx(0x50010000,
                                            "button",
                                            "&Make New Folder",
                                            0x00000004,
                                            hMargin,
                                            rcOK.Top,
                                            105,
                                            rcOK.Height,
                                            hDlg,
                                            new IntPtr(CtlIds.NEW_FOLDER_BUTTON),
                                            Process.GetCurrentProcess().Handle,
                                            IntPtr.Zero);
            }

            // Position the path editbox and it's label
            // We'll repurpose the Title (static) control as the editbox label
            int treeTop = vMargin;
            if (hEdit != IntPtr.Zero)
            {
                int xEdit = hMargin;
                int cxEdit = rcDlg.Width - (2 * hMargin);
                IntPtr hLabel = Win32.GetDlgItem(hDlg, CtlIds.TITLE);
                if (hLabel != IntPtr.Zero)
                {
                    string labelText = "Folder: ";
                    Win32.SetWindowText(hLabel, labelText);

                    // This code obtains the required size of the static control that serves as the label for the editbox.
                    // All this GDI code is a bit excessive, but I figured "what the hell".
                    IntPtr hdc = Win32.GetDC(hLabel);
                    IntPtr hFont = Win32.SendMessage(hLabel, Win32.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);
                    IntPtr oldfnt = Win32.SelectObject(hdc, hFont);
                    Size szLabel;
                    Win32.GetTextExtentPoint32(hdc, labelText, labelText.Length, out szLabel);
                    Win32.SelectObject(hdc, oldfnt);
                    Win32.ReleaseDC(hLabel, hdc);

                    Win32.RECT rcLabel = new Win32.RECT(hMargin,
                                                        vMargin + ((rcEdit.Height - szLabel.Height) / 2),
                                                        szLabel.Width,
                                                        szLabel.Height);
                    Win32.MoveWindow(hLabel, rcLabel, false);

                    xEdit += rcLabel.Width;
                    cxEdit -= rcLabel.Width;
                }

                // Expand the folder tree to fill the dialog
                rcEdit = new Win32.RECT(xEdit,
                                        vMargin,
                                        cxEdit,
                                        rcEdit.Height);

                Win32.MoveWindow(hEdit, rcEdit, false);
                treeTop = rcEdit.Bottom + 5;
            }

            Win32.RECT rcTree = new Win32.RECT(hMargin,
                treeTop,
                rcDlg.Width - (2 * hMargin),
                rcDlg.Bottom - (treeTop + (2 * vMargin) + rcOK.Height));

            Win32.MoveWindow(hTree, rcTree, false);
        }

        private void _centerTo(IntPtr hDlg, IntPtr hRef)
        {
            Win32.RECT rcDlg;
            Win32.GetWindowRect(hDlg, out rcDlg);

            Win32.RECT rcRef;
            Win32.GetWindowRect(hRef, out rcRef);

            int cx = (rcRef.Width - rcDlg.Width) / 2;
            int cy = (rcRef.Height - rcDlg.Height) / 2;
            Win32.RECT rcNew = new Win32.RECT(rcRef.Left + cx,
                                                rcRef.Top + cy,
                                                rcDlg.Width,
                                                rcDlg.Height);
            Win32.MoveWindow(hDlg, rcNew, true);
        }
    }

        internal class Win32
        {
            // Constants for sending and receiving messages in BrowseCallBackProc
            public const int WM_USER = 0x400;
            public const int WM_GETFONT = 0x0031;

            public const int MAX_PATH = 260;

            public const int BFFM_INITIALIZED = 1;
            public const int BFFM_SELCHANGED = 2;
            public const int BFFM_VALIDATEFAILEDA = 3;
            public const int BFFM_VALIDATEFAILEDW = 4;
            public const int BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*
            public const int BFFM_SETSTATUSTEXTA = WM_USER + 100;
            public const int BFFM_ENABLEOK = WM_USER + 101;
            public const int BFFM_SETSELECTIONA = WM_USER + 102;
            public const int BFFM_SETSELECTIONW = WM_USER + 103;
            public const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
            public const int BFFM_SETOKTEXT = WM_USER + 105; // Unicode only
            public const int BFFM_SETEXPANDED = WM_USER + 106; // Unicode only

            // Browsing for directory.
            public const uint BIF_RETURNONLYFSDIRS = 0x0001;  // For finding a folder to start document searching
            public const uint BIF_DONTGOBELOWDOMAIN = 0x0002;  // For starting the Find Computer
            public const uint BIF_STATUSTEXT = 0x0004;  // Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
                                                        // this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
                                                        // rest of the text.  This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
                                                        // all three lines of text.
            public const uint BIF_RETURNFSANCESTORS = 0x0008;
            public const uint BIF_EDITBOX = 0x0010;   // Add an editbox to the dialog
            public const uint BIF_VALIDATE = 0x0020;   // insist on valid result (or CANCEL)

            public const uint BIF_NEWDIALOGSTYLE = 0x0040;   // Use the new dialog layout with the ability to resize
                                                             // Caller needs to call OleInitialize() before using this API
            public const uint BIF_USENEWUI = 0x0040 + 0x0010; //(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);

            public const uint BIF_BROWSEINCLUDEURLS = 0x0080;   // Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
            public const uint BIF_UAHINT = 0x0100;   // Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
            public const uint BIF_NONEWFOLDERBUTTON = 0x0200;   // Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
            public const uint BIF_NOTRANSLATETARGETS = 0x0400;  // don't traverse target as shortcut

            public const uint BIF_BROWSEFORCOMPUTER = 0x1000;  // Browsing for Computers.
            public const uint BIF_BROWSEFORPRINTER = 0x2000; // Browsing for Printers
            public const uint BIF_BROWSEINCLUDEFILES = 0x4000; // Browsing for Everything
            public const uint BIF_SHAREABLE = 0x8000;  // sharable resources displayed (remote shares, requires BIF_USENEWUI)

            public delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);

            [StructLayout(LayoutKind.Sequential)]
            public struct BROWSEINFO
            {
                public IntPtr hwndOwner;
                public IntPtr pidlRoot;
                //public IntPtr pszDisplayName;
                public string pszDisplayName;
                //[MarshalAs(UnmanagedType.LPTStr)]
                public string lpszTitle;
                public uint ulFlags;
                public BrowseCallbackProc lpfn;
                public IntPtr lParam;
                public int iImage;
            }

            [DllImport("shell32.dll")]
            public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

            // Note that the BROWSEINFO object's pszDisplayName only gives you the name of the folder.
            // To get the actual path, you need to parse the returned PIDL
            [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
            public static extern bool SHGetPathFromIDList(IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

            [DllImport("shell32.dll", SetLastError = true)]
            public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, ref IntPtr ppidl);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetWindowText(IntPtr hwnd, string lpString);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

            public const int SW_HIDE = 0;
            public const int SW_SHOW = 5;
            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); //ShowWindowCommands nCmdShow);

            public const int SWP_ASYNCWINDOWPOS = 0x4000;
            public const int SWP_DEFERERASE = 0x2000;
            public const int SWP_DRAWFRAME = 0x0020;
            public const int SWP_FRAMECHANGED = 0x0020;
            public const int SWP_HIDEWINDOW = 0x0080;
            public const int SWP_NOACTIVATE = 0x0010;
            public const int SWP_NOCOPYBITS = 0x0100;
            public const int SWP_NOMOVE = 0x0002;
            public const int SWP_NOOWNERZORDER = 0x0200;
            public const int SWP_NOREDRAW = 0x0008;
            public const int SWP_NOREPOSITION = 0x0200;
            public const int SWP_NOSENDCHANGING = 0x0400;
            public const int SWP_NOSIZE = 0x0001;
            public const int SWP_NOZORDER = 0x0004;
            public const int SWP_SHOWWINDOW = 0x0040;

            public const int HWND_TOP = 0;
            public const int HWND_BOTTOM = 1;
            public const int HWND_TOPMOST = -1;
            public const int HWND_NOTOPMOST = -2;

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
            public static void MoveWindow(IntPtr hWnd, RECT rect, bool bRepaint)
            {
                MoveWindow(hWnd, rect.Left, rect.Top, rect.Width, rect.Height, bRepaint);
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;

                public RECT(int left, int top, int width, int height)
                {
                    Left = left;
                    Top = top;
                    Right = left + width;
                    Bottom = top + height;
                }

                public int Height => Bottom - Top;
                public int Width => Right - Left;
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    X = x;
                    Y = y;
                }

                public static implicit operator Point(POINT p)
                {
                    return new Point(p.X, p.Y);
                }

                public static implicit operator POINT(Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }

            [DllImport("user32.dll")]
            public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

            public static bool ScreenToClient(IntPtr hWnd, ref RECT rc)
            {
                POINT pt1 = new POINT(rc.Left, rc.Top);
                if (!ScreenToClient(hWnd, ref pt1))
                    return false;
                POINT pt2 = new POINT(rc.Right, rc.Bottom);
                if (!ScreenToClient(hWnd, ref pt2))
                    return false;

                rc.Left = pt1.X;
                rc.Top = pt1.Y;
                rc.Right = pt2.X;
                rc.Bottom = pt2.Y;

                return true;
            }

            public static readonly int GWL_WNDPROC = (-4);
            public static readonly int GWL_HINSTANCE = (-6);
            public static readonly int GWL_HWNDPARENT = (-8);
            public static readonly int GWL_STYLE = (-16);
            public static readonly int GWL_EXSTYLE = (-20);
            public static readonly int GWL_USERDATA = (-21);
            public static readonly int GWL_ID = (-12);
            public static readonly int DS_CONTEXTHELP = 0x2000;
            public static readonly int WS_EX_CONTEXTHELP = 0x00000400;

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr windowTitle);

            public static readonly uint SHGFI_PIDL = 0x000000008;           // pszPath is a pidl
            public static readonly uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes

            public static readonly uint SFGAO_LINK = 0x00010000;     // Shortcut (link)

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            }

            //[DllImport("shell32.dll", CharSet = CharSet.Auto)]
            //public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SHGetFileInfo(IntPtr pidlPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

            [DllImport("user32.dll")]
            public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, out Size lpSize);

            [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
            public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            public static extern IntPtr GetParent(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr CreateWindowEx(
               uint dwExStyle,
               string lpClassName,
               string lpWindowName,
               uint dwStyle,
               int x,
               int y,
               int nWidth,
               int nHeight,
               IntPtr hWndParent,
               IntPtr hMenu,
               IntPtr hInstance,
               IntPtr lpParam);
        }
    }
