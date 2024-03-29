//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;

namespace TVRename;

public class SourceConnectivityException : Exception
{
    public SourceConnectivityException(string message, Exception e) : base(message, e)
    {
    }

    public SourceConnectivityException(Exception ex) : this (ex.ErrorText(), ex)
    {
    }

    public SourceConnectivityException(string? message) : base(message)
    {
    }
}
