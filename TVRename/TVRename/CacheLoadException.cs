using System;

namespace TVRename;

public class CacheLoadException : Exception
{
    // Thrown if an error occurs in the XML when reading TheTVDB.xml
    public CacheLoadException(string message, Exception e)
        : base(message,e)
    {
    }
}
