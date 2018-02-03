using System.Drawing;

namespace TVRename.Windows.Models
{
    public class WindowPosition
    {
        public Size Size { get; set; }

        public Point Location { get; set; } 

        public bool Maximized { get; set; }

        public int Splitter { get; set; }
    }
}
