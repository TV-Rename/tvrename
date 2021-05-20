//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Collections.Generic;

namespace TVRename
{
    internal abstract class ShowsExporter : Exporter
    {
        protected readonly List<ShowConfiguration> Shows;

        protected ShowsExporter(List<ShowConfiguration> shows)
        {
            Shows = shows;
        }
    }
}