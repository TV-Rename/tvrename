using System.Drawing;
using TVRename.Core.Utility;
using TVRename.Windows.Models;

namespace TVRename.Windows.Configuration
{
    /// <summary>
    /// Stores and represents application layout.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Layout}" />
    /// <inheritdoc />
    public class Layout : JsonSettings<Layout>
    {
        public WindowPosition Window { get; set; } = new WindowPosition
        {
            Size = new Size(950, 650),
            Location = new Point(100, 100),
            Maximized = false,
            Splitter = 250
        };
    }
}
