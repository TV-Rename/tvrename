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

        public static string DisplayVersionString()
        {
            // all versions while developing are marked (dev)
            // only remove for final release build for upload
            // to site.

            // Release history:
            // Version 2.3b1 released 18 August 2017
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

	        System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
	        string v = $"{version.Major}.{version.Minor}.{version.Build}";
	        if (version.Revision > 0) v += "." + version.Revision;
#if DEBUG
            v += " ** Debug Build **";
#endif
            return v;

        }
    }
}
