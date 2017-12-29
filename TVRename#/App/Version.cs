// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Linq;
using System.Reflection;

namespace TVRename
{
    // What version are we?
    // Are we running under Mono, rather than MS.NET ?
    public static class Version
    {
        private static bool? _onMonoCached;

        public static bool OnMono()
        {
            if (!_onMonoCached.HasValue)
                _onMonoCached = Type.GetType("Mono.Runtime") != null;
            return _onMonoCached.Value;
        }

        public static bool OnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        public static string DisplayVersionString()
        {

            string v = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).Cast<AssemblyInformationalVersionAttribute>().First().InformationalVersion;
#if DEBUG
            v += " ** Debug Build **";
#endif
            return v;

        }
    }
}
