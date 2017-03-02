using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using thebotchallenge.Network;
using thebotchallenge.Security;

namespace thebotchallenge
{
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class Game : IDisposable
    {
        private string roundTicket;
        private TcpClient tcp;
        private NetworkClient network;

        public Player Player { get; private set; }

        public Game()
        {
            this.tcp = new TcpClient("gs-proxy.thebotchallenge.com", 2400);
        }

        public void Authorize(string authorizationToken)
        {
            string aes_key;
            string aes_iv;

            WebRequest req = HttpWebRequest.Create("https://authtoken-resolver.thebotchallenge.com/" + authorizationToken);
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    this.roundTicket = sr.ReadLine();
                    aes_key = sr.ReadLine();
                    aes_iv = sr.ReadLine();
                }
            }

            // Create network instance
            this.network = new NetworkClient(tcp.GetStream(), new AesCrypt(aes_key, aes_iv, CipherMode.CBC, 256, 128));

            // Send login credentials
            this.network.SendBuffer(PacketTypeFactory.ToBuffer(PacketType.Login));
            this.network.SendBuffer(Convert.FromBase64String(this.roundTicket));

            // Check if we successfully authenticated
            if (this.network.ReceivePacketType() != PacketType.ACK)
                throw new Exception();
            
            // Send Crypt packet in order to upgrade our connection to AES encryption
            this.network.SendBuffer(PacketTypeFactory.ToBuffer(PacketType.Crypt));
            this.network.IsEncrypted = true;

            // Receive already encrypted server hello
            if (this.network.ReceivePacketType() != PacketType.ServerHello)
                throw new Exception();

            this.Player = new Player(this.network);
        }

        public void Dispose()
        {
            this.network.Dispose();
            this.tcp.Dispose();
        }
    }
}
