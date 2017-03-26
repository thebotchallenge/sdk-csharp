using System;
using System.Diagnostics;
using vikebot.Network;

namespace vikebot
{
    /// <summary>
    /// Represents the current player
    /// </summary>
#if !DEBUG
    [DebuggerStepThrough]
#endif
    public sealed class Player
    {
        private NetworkClient network;

        /// <summary>
        /// The direction this player is currently looking
        /// </summary>
        public Direction WatchDirection { get; private set; }
        
        internal Player(NetworkClient network)
        {
            this.network = network;
            this.WatchDirection = Direction.Forward;
        }

        private void ThrowCommonExceptions(PacketType type)
        {
            switch (type)
            {
                case PacketType.ExceededGameCommandLimitation:
                    throw new ExceededLimitationException("Exceeded the limitation for this action. Read the documentation to learn more about specific limits");
                default:
                    throw new Exception();
            }
        }


        public void Rotate(Angle angle)
        {
            if (!Enum.IsDefined(typeof(Angle), (short)angle))
            {
                throw new ArgumentException();
            }

            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Rotate, (short)angle));

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                this.ThrowCommonExceptions(response);
            }

            if (angle == Angle.Right)
            {
                this.WatchDirection++;
                if ((int)this.WatchDirection == 4)
                    this.WatchDirection = Direction.Forward;
            }
            else if (angle == Angle.Left)
            {
                this.WatchDirection--;
                if ((int)this.WatchDirection == -1)
                    this.WatchDirection = Direction.Left;
            }
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
                if (response == PacketType.InvalidMove)
                    throw new InvalidGameActionException("You cannot move onto this field. Check if this block is another player or EOF (End-of-map)");

                this.ThrowCommonExceptions(response);
            }
        }

        /// <summary>
        /// Attacks the player positioned in the direction we are currently watching
        /// </summary>
        public void Attack()
        {
            this.network.SendPacketType(PacketType.Attack);
            
            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.CannotAttackEmptyFields)
                {
                    throw new InvalidGameActionException("You cannot attack empty fields. The block in your watch direction must be an opponent (BlockType.Opponent)");
                }
                    
                this.ThrowCommonExceptions(response);
            }
        }

        /// <summary>
        /// Scans the surrounding area of the player and returns a 5x5 matrix of <see cref="BlockType"/>
        /// with the found information. If the area is not accessible by the player <see cref="BlockType.EndOfMap"/>
        /// will be passed. If it's an enemy <see cref="BlockType.Opponent"/> will be passed.
        /// </summary>
        /// <returns>A 5x5 matrix containing the information for the player's surrounding area</returns>
        public BlockType[,] GetSurrounding()
        {
            this.network.SendPacketType(PacketType.Surrounding);

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.CannotAttackEmptyFields)
                    throw new InvalidGameActionException("You cannot attack empty fields. Check if the block in your attack direction is a opponent (BlockType.Opponent)");

                this.ThrowCommonExceptions(response);
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
            
        }

        public void Undefend()
        {
            
        }
    }
}
