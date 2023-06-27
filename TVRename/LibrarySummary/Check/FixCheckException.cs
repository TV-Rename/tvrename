using System;

namespace TVRename;

internal class FixCheckException : Exception
{
    public FixCheckException(string s) : base(s)
    {
    }

    public FixCheckException(string s, Exception e) : base(s,e)
    {
    }
}
