using System;
using System.Diagnostics;
using System.Text;
using vikebot.Encoding;
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
        private bool inDefend;
        private string name;

        /// <summary>
        /// The direction this player is currently watching
        /// </summary>
        public Direction WatchDirection { get; private set; }

        /// <summary>
        /// The name that will be rendered for this player instance. Only available in team challenges where
        /// the users account name isn't a unique identifier.
        /// </summary>
        public string Name
        {
            get => this.name;
            set
            {
                byte[] nameBuffer = DefaultEncoding.String.GetBytes(value);
                this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Name, nameBuffer));

                PacketType response = this.network.ReceivePacketType();
                if (response != PacketType.ACK)
                {
                    this.ThrowCommonExceptions(response);
                }

                this.name = value;
            }
        }

        /// <summary>
        /// Indicates if the user is in defend mode. Can only be changed once every 4 seconds. If activated the
        /// player cannot move.
        /// </summary>
        public bool InDefend
        {
            get => this.inDefend;
            set
            {
                this.network.SendPacketType(PacketType.InDefend);
                PacketType response = this.network.ReceivePacketType();

                if (response != PacketType.ACK)
                {
                    this.ThrowCommonExceptions(response);
                }

                this.inDefend = value;
            }
        }

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

        /// <summary>
        /// Rotates the player in the specified direction definded by a <see cref="Angle"/> type
        /// </summary>
        /// <param name="angle">The angle in which the player will rotate. Can ether be <see cref="Angle.Right"/> or <see cref="Angle.Left"/>.</param>
        public void Rotate(Angle angle)
        {
            this.network.SendBuffer(PacketFactory.ToBuffer(PacketType.Rotate, (int)angle));

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                this.ThrowCommonExceptions(response);
            }

            // Server accepted request -> update local properties 
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
        /// Scans the surrounding area of the player and returns the position of enemies (<see cref="BlockType.Opponent"/>)
        /// </summary>
        /// <returns></returns>
        public BlockType[,] Watch()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scans the surrounding area of the player and returns a 5x5 matrix of <see cref="BlockType"/>
        /// with the found information. If the area is not accessible by the player <see cref="BlockType.EndOfMap"/>
        /// will be passed. This method won't tell you if there is an enemy (<see cref="BlockType.Opponent"/>).
        /// </summary>
        /// <returns>A 11x11 matrix containing the information for the player's surrounding area</returns>
        public BlockType[,] WatchEnvironment()
        {
            this.network.SendPacketType(PacketType.WatchEnvironment);

            PacketType response = this.network.ReceivePacketType();
            if (response != PacketType.ACK)
            {
                if (response == PacketType.CannotAttackEmptyFields)
                    throw new InvalidGameActionException("You cannot attack empty fields. Check if the block in your attack direction is a opponent (BlockType.Opponent)");

                this.ThrowCommonExceptions(response);
            }

            byte[] buffer = this.network.ReceiveBuffer(484);

            BlockType[,] result = new BlockType[11, 11];

            Type blockType = typeof(BlockType);
            int sizeOfInt32 = sizeof(int);
            for (int i = 0; i < 484; i += sizeOfInt32)
            {
                byte[] currentBytePair = new byte[sizeOfInt32];
                Array.Copy(buffer, i * sizeOfInt32, currentBytePair, 0, sizeOfInt32);

                int value = Convert.ToInt32(currentBytePair);

                if (!Enum.IsDefined(blockType, value))
                {
                    throw new Exception();
                }

                result[i / result.GetLength(0), i % result.GetLength(0)] = (BlockType)value;
            }

            return result;
        }

        /// <summary>
        /// Counts the amount of enemies (<see cref="BlockType.Opponent"/>) in the player's action area. Value is returned
        /// as an int value.
        /// </summary>
        /// <returns>Amount of enemies inside the users action area.</returns>
        public int Radar()
        {
            this.network.SendPacketType(PacketType.Radar);

            PacketType response = this.network.ReceivePacketType();
            if (response == PacketType.ACK)
            {
                this.ThrowCommonExceptions(response);
            }

            byte[] buffer = this.network.ReceiveBuffer(4);
            int value = Convert.ToInt32(buffer);

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Scout()
        {
            throw new NotImplementedException();
        }
    }
}
