using System;
using System.Collections.Generic;
using System.Text;

namespace vikebot.Encoding
{
    internal class DefaultEncoding
    {
        internal static System.Text.Encoding String { get; private set; }

        static DefaultEncoding()
        {
            String = System.Text.Encoding.UTF8;
        }
    }
}
