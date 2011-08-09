using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVRename.Utility
{
    class FileNameUtility
    {
        public static bool OKExtensionsString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;

            string[] t = s.Split(';');
            foreach (string s2 in t)
            {
                if ((string.IsNullOrEmpty(s2)) || (!s2.StartsWith(".")))
                    return false;
            }
            return true;
        }
    }
}
