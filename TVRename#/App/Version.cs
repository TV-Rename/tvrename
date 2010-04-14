// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 
namespace TVRename
{
    // What version are we?
    // Are we running under Mono, rather than MS.NET ?
    public static class Version
    {
        private static bool? OnMonoCached;

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

            // Release history:
            // Version 2.2.0b1 released 9 April 2010, r94

            string v = "2.2.0b2";
#if DEBUG
            return v + " ** Debug Build **";
#else
            return v;
#endif
        }
    }
}