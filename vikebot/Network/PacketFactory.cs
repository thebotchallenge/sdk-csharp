using System;

namespace vikebot.Network
{
    internal static class PacketFactory
    {
        internal static byte[] ToBuffer(PacketType type, byte[] value)
        {
            byte[] packet = PacketTypeFactory.ToBuffer(type);

            byte[] buffer = new byte[packet.Length + value.Length];

            Array.Copy(packet, 0, buffer, 0, packet.Length);
            Array.Copy(value, 0, buffer, packet.Length, value.Length);

            return buffer;
        }

        internal static byte[] ToBuffer(PacketType type, short value)
        {
            return ToBuffer(type, BitConverter.GetBytes(value));
        }

        internal static byte[] ToBuffer(PacketType type, int value)
        {
            return ToBuffer(type, BitConverter.GetBytes(value));
        }
    }
}
