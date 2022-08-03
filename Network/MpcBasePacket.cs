﻿using LiteNetLib.Utils;
using MultiplayerCore.Networking.Abstractions;

namespace BeatSaberMultiplayerChat.Network;

public class MpcBasePacket : MpPacket
{
    /// <summary>
    /// The MPC protocol version used by the client.
    /// </summary>
    /// <see cref="MpcVersionInfo.ProtocolVersion"/>
    public uint ProtocolVersion;

    public override void Serialize(NetDataWriter writer)
    {
        writer.PutVarUInt(ProtocolVersion);
    }

    public override void Deserialize(NetDataReader reader)
    {
        ProtocolVersion = reader.GetVarUInt();
    }
}