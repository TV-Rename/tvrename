// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;

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

        public static bool OnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        public static string PersonalTag()
        {
            // NOTE: If you're doing your own branch/patch version number, please put your name or nickname here,
            // to indicate that it is not an official release.  Also, talk to me (tvrename@tvrename.com) so any 
            // I can consider merging your changes into the main version!
            return "-ss"; // e.g. "-ss", "-foo".
        }

        public static string DisplayVersionString()
        {
            // all versions while developing are marked (dev)
            // only remove for final release build for upload
            // to site.

            // Release history:
            // Version 2.2.3 released 20 August 2012, r214
            // Version 2.2.2 released 19 August 2012, r209
            // Version 2.2.2a1 released 18 August 2012, r207
            // Version 2.2.1 released 12 August 2012, r204
            // Version 2.2.0b10 released 7 October 2011, r173
            // Version 2.2.0b9 released 18 June 2011, r161
            // Version 2.2.0b8 released 18 June 2011, r159
            // Version 2.2.0b7 released 20 January 2011, r143
            // Version 2.2.0b6 unofficial release 2010
            // Version 2.2.0b5 released 2 May 2010, r133
            // Version 2.2.0b4 released 26 April 2010, r128
            // Version 2.2.0b3 released 16 April 2010, r110
            // Version 2.2.0b2 released 14 April 2010, r108
            // Version 2.2.0b1 released 9 April 2010, r94

            string v = "2.2.3" + PersonalTag();
#if DEBUG
            return v + " ** Debug Build **";
#else
            return v;
#endif
        }
    }
}
