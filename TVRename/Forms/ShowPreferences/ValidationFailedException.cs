using System;

namespace TVRename
{
    internal class ValidationFailedException : Exception
    {
        public ValidationFailedException(string message):base(message)
        {
        }
    }
}