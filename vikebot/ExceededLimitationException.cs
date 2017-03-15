using System;
using System.Diagnostics;

namespace vikebot
{
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class ExceededLimitationException : Exception
    {
        public ExceededLimitationException(string message) : base(message)
        {
        }
    }
}
