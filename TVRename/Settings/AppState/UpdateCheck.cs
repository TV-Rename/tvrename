using System;
using System.Xml.Serialization;

namespace TVRename.Settings.AppState;

public class UpdateCheck
{
    public DateTime? LastUpdateCheckUtc { get; set; }

    // This state can be extended when we want to provide a "skip this version" feature etc.

    [XmlIgnore]
    public TimeSpan LastUpdate => TimeHelpers.UtcNow() - LastUpdateCheckUtc.GetValueOrDefault(DateTime.MinValue);
}
