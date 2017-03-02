using System;
using System.Diagnostics;
using thebotchallenge.Network;

namespace thebotchallenge
{
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class Player
    {
        private const string LIMITATION = "Limitation for the '{0}' action exeeded. Allowed: {1} call per {2}";

        private NetworkClient network;

        internal Player(NetworkClient network)
        {
            this.network = network;
        }

        /// <summary>
        /// Will allow your player to move up
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Direction direction)
        {
            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Move, (short)direction));

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.ExceededGameCommandLimitation)
                    throw new ExceededLimitationException(string.Format(LIMITATION, "Move", 1, "500ms"));

                else if (response == PacketType.InvalidMove)
                    throw new InvalidGameActionException("You cannot move onto this field. Check if this block is another player or EOF (End-of-map)");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        public void Attack(Direction direction)
        {
            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Attack, (short)direction));

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.ExceededGameCommandLimitation)
                    throw new ExceededLimitationException(string.Format(LIMITATION, "Attack", 1, "500ms"));
            }
        }
        public BlockType[,] GetSurrounding()
        {
            throw new NotImplementedException();
        }
        public int Radar()
        {
            throw new NotImplementedException();
        }
        public int Scout(Alignment alignment)
        {
            throw new NotImplementedException();
        }
        public void Defend()
        {

        }
        public void Undefend()
        {

        }
    }
}
