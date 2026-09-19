using System;

namespace TVRename;

public class ValidationFailedException(string message) : Exception(message)
{
}
