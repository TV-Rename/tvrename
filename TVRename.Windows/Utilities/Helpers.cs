using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem;
using TVRename.Windows.Configuration;
using static System.Windows.Forms.Control;

namespace TVRename.Windows.Utilities
{
    public static class Helpers
    {
        /// <summary>
        /// Gets the application display version from the current assemblies <see cref="AssemblyInformationalVersionAttribute"/>.
        /// </summary>
        /// <value>
        /// The application display version.
        /// </value>
        public static string DisplayVersion
        {
            get
            {
                string v = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().First().InformationalVersion;

#if DEBUG
                v += " Debug";
#endif

                return v;
            }
        }

        public static void DoubleBuffer(Control control)
        {
            if (SystemInformation.TerminalServerSession) return;

            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(control, true, null);
        }

        public static void DoubleBuffer(ControlCollection controls)
        {
            if (SystemInformation.TerminalServerSession) return;

            foreach (Control control in controls)
            {
                DoubleBuffer(control);
            }
        }

        public static string EscapeTemplatePath(string path)
        {
            foreach (KeyValuePair<char, string> replacement in Settings.Instance.FilenameReplacements)
            {
                path = path.Replace(replacement.Key.ToString(), replacement.Value);
            }

            return path;
        }

        public static bool IsVideo(this FileInfo file)
        {
            return Settings.Instance.VideoFileExtensions.Contains(file.Extension.Trim('.'));
        }
    }
}
