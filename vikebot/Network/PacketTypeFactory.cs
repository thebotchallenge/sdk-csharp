using System;

namespace vikebot.Network
{
    internal static class PacketTypeFactory
    {
        internal static byte[] ToBuffer(PacketType packetType)
        {
            return BitConverter.GetBytes((short)packetType);
        }
        internal static void ToBuffer(byte[] buffer, int offset, PacketType packetType)
        {
            byte[] packet = BitConverter.GetBytes((short)packetType);
            Array.Copy(packet, 0, buffer, offset, packet.Length);
        }

        internal static PacketType FromBuffer(byte[] buffer)
        {
            short packetTypeId = BitConverter.ToInt16(buffer, 0);

            if (!Enum.IsDefined(typeof(PacketType), packetTypeId))
                throw new Exception();

            return (PacketType)packetTypeId;
        }
    }
}
