using System;

namespace TVRename;

public class ActionFailedException : Exception
{
    // Thrown if an error occurs in the XML when reading TheTVDB.xml
    public ActionFailedException(string message)
        : base(message)
    {
    }
}
