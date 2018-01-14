using System.Collections.Generic;
using System.Drawing;
using TVRename.Core.Utility;

namespace TVRename.Core.Models.Settings
{
    /// <summary>
    /// Stores and represents application layout.
    /// See <see cref="JsonSettings{T}"/>.
    /// </summary>
    /// <seealso cref="JsonSettings{Layout}" />
    /// <inheritdoc />
    public class Layout : JsonSettings<Layout> // TODO: Use
    {
        public Size Size { get; set; } = new Size(950, 650);

        public Point Location { get; set; } = new Point(100);

        public bool Maximized { get; set; } = false;

        public int Splitter { get; set; } = 280;

        public List<int> WhenToWatch { get; set; } = new List<int>();

        public List<int> AllInOne { get; set; } = new List<int>();
    }
}
