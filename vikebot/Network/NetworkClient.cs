using System;
using System.Net.Sockets;
using vikebot.Security;

namespace vikebot.Network
{
    internal class NetworkClient : IDisposable
    {
        private Random rdm;
        private NetworkStream ns;
        private AesHelper aes;

        internal bool IsEncrypted { get; set; }

        internal NetworkClient(NetworkStream ns, AesHelper aes)
        {
            this.rdm = new Random();
            this.ns = ns;
            this.IsEncrypted = false;
            this.aes = aes;
        }

        internal void SendBuffer(byte[] buffer)
        {
            if (this.IsEncrypted)
            {
                int packetTypeLength = OverheadCalculator.BlockSize;
                byte[] packetType = new byte[packetTypeLength];
                this.rdm.NextBytes(packetType);
                Array.Copy(buffer, 0, packetType, 7, 2);
                packetType = this.aes.Encrypt(packetType);

                int plainPacketInformationLength = buffer.Length - 2;
                int overhead = OverheadCalculator.GetOverhead(plainPacketInformationLength);
                byte[] packetInformation = new byte[plainPacketInformationLength + overhead];
                this.rdm.NextBytes(packetInformation);
                int garbage = overhead / 2;
                Array.Copy(buffer, 2, packetInformation, garbage, plainPacketInformationLength);
                packetInformation = this.aes.Encrypt(packetInformation);

                buffer = new byte[packetTypeLength + packetInformation.Length];
                Array.Copy(packetType, 0, buffer, 0, packetTypeLength);
                Array.Copy(packetInformation, 0, buffer, packetTypeLength, packetInformation.Length);
            }

            this.ns.Write(buffer, 0, buffer.Length);
            this.ns.Flush();
        }
        internal void ReceiveBuffer(byte[] buffer)
        {
            if (this.IsEncrypted)
            {
                int overhead = OverheadCalculator.GetOverhead(buffer.Length);
                byte[] temp = new byte[buffer.Length + overhead];
                this.ns.Read(temp, 0, temp.Length);

                temp = this.aes.Decrypt(temp);
                int garbage = overhead / 2;
                Array.Copy(temp, garbage, buffer, 0, temp.Length - garbage * 2);
            }
            else
                this.ns.Read(buffer, 0, buffer.Length);
        }

        internal byte[] ReceiveBuffer(int length)
        {
            byte[] buffer = new byte[length];
            this.ReceiveBuffer(buffer);
            return buffer;
        }

        internal void SendPacketType(PacketType type)
        {
            this.SendBuffer(PacketTypeFactory.ToBuffer(type));
        }

        internal PacketType ReceivePacketType()
        {
            byte[] buffer = new byte[2];
            this.ReceiveBuffer(buffer);

            return PacketTypeFactory.FromBuffer(buffer);
        }

        public void Dispose()
        {
            this.aes.Dispose();
            this.ns.Dispose();
        }
    }
}
