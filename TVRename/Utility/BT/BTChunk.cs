//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

namespace TVRename;

// ReSharper disable once InconsistentNaming
public enum BTChunk
{
    kError,
    kDictionary,
    kDictionaryItem,
    kList,
    kListOrDictionaryEnd,
    kInteger,
    kString,

    // ReSharper disable once InconsistentNaming
    kBTEOF
}
