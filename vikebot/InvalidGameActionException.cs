using System;
using System.Diagnostics;

#if !DEBUG
    [DebuggerStepThrough]
#endif
namespace vikebot
{
    public sealed class InvalidGameActionException : Exception
    {
        public InvalidGameActionException(string message) : base(message)
        {
        }
    }
}
