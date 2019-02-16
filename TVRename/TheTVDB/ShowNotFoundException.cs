// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;

namespace TVRename
{
    internal class ShowNotFoundException : Exception
    {
        public readonly int ShowId;

        public ShowNotFoundException(int id)
        {
            ShowId = id;
        }
    }
}
