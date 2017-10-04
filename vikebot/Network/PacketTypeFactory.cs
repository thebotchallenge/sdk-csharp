using System;

namespace vikebot.Network
{
    internal static class PacketTypeFactory
    {
        internal static byte[] ToBuffer(PacketType packetType)
        {
            return BitConverter.GetBytes((int)packetType);
        }

        internal static PacketType FromBuffer(byte[] buffer)
        {
            int packetTypeId = BitConverter.ToInt32(buffer, 0);

            if (!Enum.IsDefined(typeof(PacketType), packetTypeId))
                throw new Exception();

            return (PacketType)packetTypeId;
        }
    }
}
