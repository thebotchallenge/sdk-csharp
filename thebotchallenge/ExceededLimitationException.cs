using System;
using System.Diagnostics;

#if !DEBUG
    [DebuggerStepThrough]
#endif
namespace thebotchallenge
{
    public sealed class ExceededLimitationException : Exception
    {
        public ExceededLimitationException(string message) : base(message)
        {
        }
    }
}
