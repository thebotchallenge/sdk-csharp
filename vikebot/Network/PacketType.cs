using System;

namespace vikebot.Network
{
    internal enum PacketType
    {
        // SERVER COMMANDS
        ACK = 1,
        NAK = 2,
        InvalidPacketType = 3,
        ServerHello = 4,

        // CLIENT COMMANDS
        Login = 50,
        Crypt = 51,
        SdkId = 52,

        // GAME COMMANDS
        Move = 100,
        Attack = 101,
        Surrounding = 102,
        Radar = 103,
        Scout = 104,
        Defend = 105,
        Undefend = 106,

        // SERVER EXCEPTIONS
        ExceededGameCommandLimitation = 200,
        InvalidMove = 201,
        CannotAttackEmptyFields = 202,
        AlreadyInDefendMode = 203,
        NotInDefendMode = 204
    }
}
