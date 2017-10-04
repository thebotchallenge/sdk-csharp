using System;

namespace vikebot.Network
{
    internal enum PacketType
    {
        // Server OKs
        ACK = 1,
        ServerHello = 2,

        // Server NOs
        NAK = 100,
        InvalidPacketType = 101,
        NotAuthenticatedYet = 102,
        AlreadyAuthenticated = 103,
        AlreadyEncrypted = 104,

        // Client commands
        Login = 200,
        Crypt = 201,
        SdkId = 202,
        OS = 203,

        // Client game commands
        Name = 300,
        Rotate = 301,
        Move = 302,
        Attack = 303,
        Watch = 304,
        WatchEnvironment = 305,
        Radar = 306,
        Scout = 307,
        InDefend = 308,

        // Server game exceptions
        NamingNotAllowed = 400,
        InvalidDirectionRepresentation = 401,
        ExceededGameCommandLimitation = 402,
        InvalidMove = 403,
        CannotAttackEmptyFields = 404,
        AlreadyInDefend = 405,
        AlreadyInNotDefend = 406,
        InvalidAngleRepresentation = 407
    }
}
