using System;
using System.Diagnostics;
using vikebot.Network;

namespace vikebot
{
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class Player
    {
        private const string LIMITATION = "Limitation for the '{0}' action exeeded. Allowed: {1} call per {2}";

        private NetworkClient network;

        public PlayerState State { get; private set; }

        internal Player(NetworkClient network)
        {
            this.network = network;
            this.State = PlayerState.Default;
        }

        /// <summary>
        /// Moves the player one block into the specified direction.
        /// </summary>
        /// <param name="direction">The direction in which the player will move.</param>
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
        /// Attacks the player positioned in the specified direction.
        /// </summary>
        /// <param name="direction">The direction in which the player will perform it's attack</param>
        public void Attack(Direction direction)
        {
            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Attack, (short)direction));
            
            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.ExceededGameCommandLimitation)
                    throw new ExceededLimitationException(string.Format(LIMITATION, "Attack", 1, "500ms"));

                if (response == PacketType.CannotAttackEmptyFields)
                    throw new InvalidGameActionException("You cannot attack empty fields. Check if the block in your attack direction is a opponent (BlockType.Opponent)");
            }
        }

        /// <summary>
        /// Scans the surrounding area of the player and returns a 5x5 matrix of <see cref="thebotchallenge.BlockType"/>
        /// with the found information. If the area is not accessible by the player <see cref="thebotchallenge.BlockType.EndOfMap"/>
        /// will be passed. If it's an enemy <see cref="thebotchallenge.BlockType.Opponent"/> will be passed.
        /// </summary>
        /// <returns>A 5x5 matrix containing the information for the player's surrounding area</returns>
        public BlockType[,] GetSurrounding()
        {
            this.network.SendPacketType(PacketType.Surrounding);

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.ExceededGameCommandLimitation)
                    throw new ExceededLimitationException(string.Format(LIMITATION, "GetSurrounding", 1, "500ms"));

                else if (response == PacketType.CannotAttackEmptyFields)
                    throw new InvalidGameActionException("You cannot attack empty fields. Check if the block in your attack direction is a opponent (BlockType.Opponent)");
            }

            byte[] buffer = new byte[50];
            this.network.ReceiveBuffer(buffer);

            BlockType[,] result = new BlockType[5, 5];

            Type blockType = typeof(BlockType);
            for (int i = 0; i < 25; i++)
            {
                byte[] currentBytePair = new byte[2];
                Array.Copy(buffer, i * 2, currentBytePair, 0, currentBytePair.Length);
                short value = Convert.ToInt16(currentBytePair);

                if (!Enum.IsDefined(blockType, value))
                    throw new Exception();

                result[i / 5, i % 5] = (BlockType)value;
            }

            return result;
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
            this.State = PlayerState.InDefendMode;
        }

        public void Undefend()
        {
            this.State = PlayerState.Default;
        }
    }
}
