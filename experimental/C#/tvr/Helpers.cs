using System.IO;
using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TVRename
{
    public delegate void SetProgressDelegate(int percent);

    public class StringList : System.Collections.Generic.List<string>
    {
    }

    public class Helpers
    {
        public static System.Drawing.Color WarningColor()
        {
            return System.Drawing.Color.FromArgb((System.Int32)((System.Byte)(255)), (System.Int32)((System.Byte)(210)), (System.Int32)((System.Byte)(210)));
        }


        public static string SimplifyName(string n)
        {
            n = n.ToLower();
            n = n.Replace("the", "");
            n = n.Replace("'", "");
            n = n.Replace("&", "");
            n = n.Replace("and", "");
            n = n.Replace("!", "");
            n = Regex.Replace(n, "[_\\W]+", " ");
            return n;
        }

        public static bool Same(FileInfo a, FileInfo b)
        {
            return string.Compare(a.FullName, b.FullName, true) == 0; // true->ignore case
        }

        public static bool Same(DirectoryInfo a, DirectoryInfo b)
        {
            string n1 = a.FullName;
            string n2 = b.FullName;
            if (!n1.EndsWith("\\"))
                n1 = n1 + "\\";
            if (!n2.EndsWith("\\"))
                n2 = n2 + "\\";

            return string.Compare(n1, n2, true) == 0; // true->ignore case
        }
        public static FileInfo FileInFolder(string dir, string fn)
        {
            return new FileInfo(string.Concat(dir, dir.EndsWith("\\") ? "" : "\\", fn));
        }
        public static FileInfo FileInFolder(DirectoryInfo di, string fn)
        {
            return Helpers.FileInFolder(di.FullName, fn);
        }
    }
}


