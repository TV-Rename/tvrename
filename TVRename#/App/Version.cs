using System;
using System.Linq;
using System.Reflection;

namespace TVRename
{
    public static class Version
    {
        public static bool OnMono() => Type.GetType("Mono.Runtime") != null;
        
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
