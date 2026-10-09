//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//



namespace TVRename;

internal class FixCheckException : Exception
{
    public FixCheckException(string s) : base(s)
    {
    }

    public FixCheckException(string s, Exception e) : base(s, e)
    {
    }
}
