using System;
using System.Diagnostics;

namespace thebotchallenge
{
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class GameEndedException : Exception
    {
        public GameEndedException(string message) : base(message)
        {
        }
    }
}
