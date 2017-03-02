using System;

namespace thebotchallenge.Network
{
    internal enum PacketType
    {
        ACK = 1,
        NAK = 2,
        NAKADD = 3,
        ProxyV4 = 4,
        ProxyV6 = 5,
        Login = 6,
        Crypt = 7,
        ServerHello = 8,
        Close = 9,
        Version = 10,

        //
        // GAME COMMANDS
        //
        Move = 100,
        Attack = 101,
        Surrounding = 102,
        Radar = 103,
        Scout = 104,
        Defend = 105,
        Undefend = 106,

        ExceededGameCommandLimitation = 200,
        InvalidMove = 201,
        CannotAttackEmptyFields = 202,
        AlreadyInDefendMode = 203,
        NotInDefendMode = 204
    }
}
