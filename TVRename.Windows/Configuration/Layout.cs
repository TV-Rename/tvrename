using System.Drawing;
using TVRename.Core.Utility;

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
        public WindowPosition Window { get; set; } = new WindowPosition();

        public class WindowPosition
        {
            public Size Size { get; set; } = new Size(950, 650);

            public Point Location { get; set; } = new Point(100, 100);

            public bool Maximized { get; set; } = false;

            public int Splitter { get; set; } = 250;
        }
    }
}
