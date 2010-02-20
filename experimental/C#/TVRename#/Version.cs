//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//

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
            string v = "2.2.0a8";
#if DEBUG
        return v + " (C# Debug)";
#else
            return v;
#endif
        }

        public static bool ForceExperimentalOn()
        {
            return true; // ************************
        }


    }
} // namespace