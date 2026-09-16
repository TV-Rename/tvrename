using System;

namespace TVRename;

public class CacheLoadException(string message, Exception e) : Exception(message,e)
{
}
