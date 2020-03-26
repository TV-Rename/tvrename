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
    public class ShowNotFoundException : Exception
    {
        public readonly int ShowId;
        public readonly ShowItem.ProviderType ShowIdProvider;
        public readonly ShowItem.ProviderType ErrorProvider;

        public ShowNotFoundException(int id, string message, ShowItem.ProviderType showIdProvider, ShowItem.ProviderType errorProvider) :base(message)
        {
            ShowId = id;
            ShowIdProvider = showIdProvider;
            ErrorProvider = errorProvider;
        }
    }
}
