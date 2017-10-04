using Newtonsoft.Json;

namespace vikebot.Network
{
    internal class RoundInformation
    {
        internal string Ticket { get; private set; }

        internal string AesKey { get; private set; }
        internal string AesIv { get; private set; }

        internal string HostV4 { get; private set; }
        internal string HostV6 { get; private set; }
        internal int Port { get; private set; }

        [JsonConstructor]
        internal RoundInformation(string ticket, string aeskey, string aesiv, string ipv4, string ipv6, int port)
        {
            this.Ticket = ticket;
            this.AesKey = aeskey;
            this.AesIv = aesiv;
            this.HostV4 = ipv4;
            this.HostV6 = ipv6;
            this.Port = port;
        }
    }
}
