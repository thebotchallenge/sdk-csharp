using System;

namespace thebotchallenge.Network
{
    internal static class PacketFactory
    {
        internal static byte[] ToBuffer(PacketType type, short value)
        {
            byte[] buffer = new byte[4];

            PacketTypeFactory.ToBuffer(buffer, 0, type);

            byte[] valueBytes = BitConverter.GetBytes(value);
            Array.Copy(valueBytes, 0, buffer, 2, valueBytes.Length);

            return buffer;
        }
    }
}
