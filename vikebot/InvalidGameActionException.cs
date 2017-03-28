using System;
using System.Diagnostics;

namespace vikebot
{
    /// <summary>
    /// Thrown when you try to do any invalid game actions
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class InvalidGameActionException : Exception
    {
        internal InvalidGameActionException(string message) : base(message)
        {
        }
    }
}
