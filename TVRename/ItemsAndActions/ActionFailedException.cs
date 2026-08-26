using System;

namespace TVRename;

public class ActionFailedException(string message) : Exception(message)
{
}
