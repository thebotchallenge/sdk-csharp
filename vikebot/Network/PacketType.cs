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
        PlayerName = 53,

        // GAME COMMANDS
        Move = 100,
        Attack = 101,
        WatchEnvironment = 102,
        Radar = 103,
        Scout = 104,
        Defend = 105,
        Undefend = 106,
        Rotate = 107,

        // SERVER EXCEPTIONS
        GameEnded = 200,
        AlreadyDied = 201,
        ExceededGameCommandLimitation = 202,
        InvalidMove = 203,
        CannotAttackEmptyFields = 204,
        AlreadyInDefendMode = 205,
        NotInDefendMode = 206
    }
}
