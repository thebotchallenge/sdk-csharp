using System;
using System.Diagnostics;

namespace vikebot
{
    /// <summary>
    /// Thrown when you exceed a limitation for any action.
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class ExceededLimitationException : Exception
    {
        internal ExceededLimitationException(string message) : base(message)
        {
        }
    }
}
