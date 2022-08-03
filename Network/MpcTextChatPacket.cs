﻿using LiteNetLib.Utils;
using MultiplayerCore.Networking.Abstractions;

namespace BeatSaberMultiplayerChat.Network;

public class MpcTextChatPacket : MpcBasePacket
{
    /// <summary>
    /// Message contents.
    /// </summary>
    public string? Text;
    
    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
        
        writer.Put(Text);
    }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        
        Text = reader.GetString();
    }
}