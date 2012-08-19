// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

// Helpful functions and classes

namespace TVRename
{
    public delegate void SetProgressDelegate(int percent);

    public static class Helpers
    {
        public static string ReadStringFixQuotesAndSpaces(XmlReader r)
        {
            string res = r.ReadElementContentAsString();
            res = res.Replace("\\'", "'");
            res = res.Replace("\\\"", "\"");
            res = res.Trim();
            return res;
        }

        public static Color WarningColor()
        {
            return Color.FromArgb(255, 210, 210);
        }

        public static string TranslateColorToHtml(Color c)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
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
            return String.Compare(a.FullName, b.FullName, true) == 0; // true->ignore case
        }

        public static bool Same(DirectoryInfo a, DirectoryInfo b)
        {
            string n1 = a.FullName;
            string n2 = b.FullName;
            if (!n1.EndsWith(Path.DirectorySeparatorChar.ToString()))
                n1 = n1 + Path.DirectorySeparatorChar;
            if (!n2.EndsWith(Path.DirectorySeparatorChar.ToString()))
                n2 = n2 + Path.DirectorySeparatorChar;

            return String.Compare(n1, n2, true) == 0; // true->ignore case
        }

        public static FileInfo FileInFolder(string dir, string fn)
        {
            return new FileInfo(String.Concat(dir, dir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? "" : Path.DirectorySeparatorChar.ToString(), fn));
        }

        public static FileInfo FileInFolder(DirectoryInfo di, string fn)
        {
            return FileInFolder(di.FullName, fn);
        }

        public static string RemoveDiacritics(string stIn)
        {
            // From http://blogs.msdn.com/b/michkap/archive/2007/05/14/2629747.aspx
            string stFormD = stIn.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}