//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using System.Xml.Serialization;

namespace TVRename.Settings.AppState;

public class UpdateCheck
{
    public DateTime? LastUpdateCheckUtc { get; set; }

    // This state can be extended when we want to provide a "skip this version" feature etc.

    [XmlIgnore]
    public TimeSpan LastUpdate => TimeHelpers.UtcNow() - LastUpdateCheckUtc.GetValueOrDefault(DateTime.MinValue);
}
