using System;
using System.Diagnostics;

namespace vikebot
{
    /// <summary>
    /// Thrown when the game has ended
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class GameEndedException : Exception
    {
        internal GameEndedException(string message) : base(message)
        {
        }
    }
}
