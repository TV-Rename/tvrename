using System;

namespace TVRename;

public class ValidationFailedException : Exception
{
    public ValidationFailedException(string message) : base(message)
    {
    }
}
