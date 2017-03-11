using Newtonsoft.Json;

namespace vikebot.Network
{
    internal class RoundInformation
    {
        internal string Ticket { get; private set; }

        internal string AesKey { get; private set; }
        internal string AesIv { get; private set; }

        internal string Host { get; private set; }
        internal int Port { get; private set; }

        [JsonConstructor]
        internal RoundInformation(string ticket, string aeskey, string aesiv, string host, int port)
        {
            this.Ticket = ticket;
            this.AesKey = aeskey;
            this.AesIv = aesiv;
            this.Host = host;
            this.Port = port;
        }
    }
}
