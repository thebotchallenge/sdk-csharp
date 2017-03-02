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
    /// <summary>
    /// <see cref="thebotchallenge.Game"/> manages all connections and authorizations for the
    /// user. Also holds a current state of the active player.
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class Game : IDisposable
    {
        private string roundTicket;
        private TcpClient tcp;
        private NetworkClient network;

        /// <summary>
        /// The current player. If authorization failed or didn't happen till now this will be null.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="thebotchallenge.Game"/>. In order to receive
        /// the <see cref="thebotchallenge.Player"/> instance you need to call <see cref="thebotchallenge.Game.Authorize(string)"/>
        /// first.
        /// </summary>
        public Game()
        {
            OverheadCalculator.PreCalculateOverheads(100);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="thebotchallenge.Game"/>. Also authorizes you
        /// at the server side and changes plain text connection to encrypted AES256 connection.
        /// </summary>
        /// <param name="authorizationToken">The authtoken you got during registering for this game.</param>
        public Game(string authorizationToken) : this()
        {
            this.Authorize(authorizationToken);
        }

        /// <summary>
        /// Authorizes you at the server side and changes plain text connection to encrypted
        /// AES256 connection. This method must be called if autorizationToken was not passed
        /// into constructor.
        /// </summary>
        /// <param name="authorizationToken"></param>
        public void Authorize(string authorizationToken)
        {
            // Set the current authtokenresolver
            string authtokenResolver;
#if DEBUG
            authtokenResolver = "localhost:2405/";
#else
            authtokenResolver = "https://authtoken-resolver.thebotchallenge.com/";
#endif

            string aes_key;
            string aes_iv;
            string gameServerIp;
            int gameServerPort;
            // Request rounticket, aes key and aes initialization vector
            WebRequest req = HttpWebRequest.Create(authtokenResolver + authorizationToken);
            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    this.roundTicket = sr.ReadLine();
                    aes_key = sr.ReadLine();
                    aes_iv = sr.ReadLine();
                    gameServerIp = sr.ReadLine();
                    if (!Int32.TryParse(sr.ReadLine(), out gameServerPort))
                        throw new InvalidCastException("Unable to cast the game server's port information to a int. AuthtokenResolver protocol changed?");
                }
            }

            // Establish a new tcp connection to our server
            this.tcp = new TcpClient(gameServerIp, gameServerPort);

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

            // Create our player instance, so the user can access it
            this.Player = new Player(this.network);
        }

        public void Dispose()
        {
            this.network.Dispose();
            this.tcp.Dispose();
        }
    }
}
