namespace vikebot.Network
{
    internal static class OverheadCalculator
    {
        private const int AES_BLOCKSIZE = 128;

        private static float blocksizef;
        private static int[] cache;

        internal static int BlockSize { get; private set; }

        internal static void PreCalculateOverheads(int length)
        {
            BlockSize = AES_BLOCKSIZE / 8;
            cache = new int[length];
            for (int i = 0; i < cache.Length; i++)
                cache[i] = CalculateOverhead(i);
        }

        internal static int GetOverhead(int length)
        {
            if (length < cache.Length)
                return cache[length];
            return CalculateOverhead(length);
        }

        private static int CalculateOverhead(int length)
        {
            if (length % BlockSize == 0)
                return BlockSize;
            else
                return (((int)((float)length / blocksizef) + 1) * BlockSize) - length;
        }
    }
}
