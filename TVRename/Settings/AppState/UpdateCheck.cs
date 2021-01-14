using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TVRename.Settings.AppState
{
    public class UpdateCheck
    {
        public DateTime? LastUpdateCheckUtc { get; set; }

        // This state can be extended when we want to provide a "skip this version" feature etc.

        [XmlIgnore]
        public TimeSpan LastUpdate => DateTime.UtcNow - LastUpdateCheckUtc.GetValueOrDefault(DateTime.MinValue);
    }
}
