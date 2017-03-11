using System;

namespace vikebot.Network
{
    internal static class PacketFactory
    {
        private static byte[] BuildBuffer(PacketType type, byte[] value)
        {
            byte[] packet = PacketTypeFactory.ToBuffer(type);

            byte[] buffer = new byte[packet.Length + value.Length];

            Array.Copy(packet, 0, buffer, 0, packet.Length);
            Array.Copy(value, 0, buffer, packet.Length, value.Length);

            return buffer;
        }

        internal static byte[] ToBuffer(PacketType type, short value)
        {
            return BuildBuffer(type, BitConverter.GetBytes(value));
        }
        internal static byte[] ToBuffer(PacketType type, int value)
        {
            return BuildBuffer(type, BitConverter.GetBytes(value));
        }
    }
}
