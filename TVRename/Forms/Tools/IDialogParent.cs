//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Windows.Forms;

namespace TVRename
{
    public interface IDialogParent : IWin32Window
    {
        void ShowChildDialog(Form childForm);
    }
}
