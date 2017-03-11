using System;
using System.Diagnostics;

#if !DEBUG
    [DebuggerStepThrough]
#endif
namespace vikebot
{
    public sealed class ExceededLimitationException : Exception
    {
        public ExceededLimitationException(string message) : base(message)
        {
        }
    }
}
