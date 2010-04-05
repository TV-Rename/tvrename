//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

// What version are we?
// Are we running under Mono, rather than MS.NET ?

namespace TVRename
{
    public static class Version
    {
        private static bool? OnMonoCached = null;
        public static bool OnMono()
        {
            if (!OnMonoCached.HasValue)
                OnMonoCached = System.Type.GetType("Mono.Runtime") != null;
            return OnMonoCached.Value;
        }

        public static string DisplayVersionString()
        {
            // all versions while developing are marked (dev)
            // only remove for final release build for upload
            // to site.
            string v = "2.2.0a10 (dev)";
#if DEBUG
        return v + " ** Debug Build **";
#else
            return v;
#endif
        }
    }
} // namespace