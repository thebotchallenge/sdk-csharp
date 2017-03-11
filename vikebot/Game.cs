using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using vikebot.Network;
using vikebot.Security;

namespace vikebot
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
        /// <param name="authtoken"></param>
        public void Authorize(string authtoken)
        {
            // Set the current authtokenresolver
            string apiURL;
#if DEBUG
            apiURL = "localhost:2405/v1/roundticket/";
#else
            apiURL = "https://api.vikebot.com/v1/roundticket/";
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
            this.network = new NetworkClient(tcp.GetStream(), new AesCrypt(ri.AesKey, ri.AesIv, CipherMode.CBC, 256, 128));

            // Send login credentials and check if we successfully authenticated
            this.network.SendPacketType(PacketType.Login);
            this.network.SendBuffer(Convert.FromBase64String(ri.Ticket));
            if (this.network.ReceivePacketType() != PacketType.ACK)
                throw new Exception();

            // Send Crypt-packet to force AES encryption and receive already encrypted server hello
            this.network.SendPacketType(PacketType.Crypt);
            this.network.IsEncrypted = true;
            if (this.network.ReceivePacketType() != PacketType.ServerHello)
                throw new Exception();

            // TODO: Send SDK-ID

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
