using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Diagnostics;
using vikebot.Network;
using vikebot.Security;
using vikebot.Encoding;

namespace vikebot
{
    /// <summary>
    /// <see cref="Game"/> manages all connections and authorizations for the
    /// client. Holds all states of the active players.
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class Game : IDisposable
    {
        private const string SDK_ID = "";

        private TcpClient tcp;
        private NetworkClient network;

        /// <summary>
        /// The current player. If authorization failed or didn't happen till now this will be null.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Game"/>. Also authorizes you at the server and
        /// forces a AES256-CBC encrypted connection.
        /// </summary>
        /// <param name="authToken">The authtoken you got during registering for this game.</param>
        public Game(string authToken)
        {
            OverheadCalculator.PreCalculateOverheads(100);
            this.Authorize(authToken);
        }

        /// <summary>
        /// Authorizes the client at the server side and changes plain text connection to encrypted
        /// AES256 connection.
        /// </summary>
        private void Authorize(string authtoken)
        {
            // Set the current authtokenresolver
#if DEBUG
            string apiURL = "localhost:2405/v1/roundticket/";
#else
            string apiURL = "https://api.vikebot.com/v1/roundticket/";
#endif

            // Request roundInformation from API
            RoundInformation ri = null;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(apiURL + authtoken).Result)
            using (HttpContent content = response.Content)
            {
                string jsonString = content.ReadAsStringAsync().Result;
                ri = JsonConvert.DeserializeObject<RoundInformation>(jsonString);
            }

            // Establish a new tcp connection to our server and create network instance
            this.tcp = new TcpClient(AddressFamily.InterNetwork);
            this.tcp.ConnectAsync(ri.Host, ri.Port);
            this.network = new NetworkClient(this.tcp.GetStream(), new AesHelper(ri.AesKey, ri.AesIv, CipherMode.CBC, 256, 128));

            // Send login credentials and check if we successfully authenticated
            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Login, Convert.FromBase64String(ri.Ticket)));
            if (this.network.ReceivePacketType() != PacketType.ACK)
                throw new Exception();

            // Send Crypt-packet to force AES encryption
            this.network.SendPacketType(PacketType.Crypt);
            // Tell our NetworkClient that he must decrypt the next packet! - If the next packet is not encrypted
            // the other party doesn't know our secret aes key and initialization vector hence -> unsafe connection
            this.network.IsEncrypted = true;
            // Receive already encrypted server hello
            if (this.network.ReceivePacketType() != PacketType.ServerHello)
                throw new Exception();

            // Send SdkId
            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.SdkId, DefaultEncoding.String.GetBytes(Game.SDK_ID)));

            // Create our player instance, so the user can access it
            this.Player = new Player(this.network);
        }

        /// <summary>
        /// Dispose all resources
        /// </summary>
        public void Dispose()
        {
            this.network.Dispose();
            this.tcp.Dispose();
        }
    }
}
