using System;
using System.Diagnostics;

namespace vikebot
{
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class InvalidGameActionException : Exception
    {
        public InvalidGameActionException(string message) : base(message)
        {
        }
    }
}
