﻿using System;
using LiteNetLib.Utils;
using MultiplayerChat.Audio;
using Zenject;

namespace MultiplayerChat.Network;

/// <summary>
/// Unreliable packet containing a Opus-encoded voice fragment.
/// </summary>
public class MpcVoicePacket : MpcBasePacket, IPoolablePacket
{
    /// <summary>
    /// Opus-encoded audio fragment (48kHz, 1 channel).
    /// If null/empty, this indicates the end of a transmission.
    /// </summary>
    public byte[]? Data;
    
    private bool _isRentedBuffer;
    private int? _bufferContentSize;

    public int DataLength => _bufferContentSize ?? Data?.Length ?? 0;

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        if (Data == null || _bufferContentSize == 0)
        {
            writer.Put(0); // int length
            return;
        }
            
        writer.PutBytesWithLength(Data, 0, DataLength);
    }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        
        Data = reader.GetBytesWithLength();
    }

    #region Packet Pool
    
    protected static PacketPool<MpcVoicePacket> Pool => ThreadStaticPacketPool<MpcVoicePacket>.pool;

    public static MpcVoicePacket Obtain() => Pool.Obtain();
    
    public void Release()
    {
        ReturnPooledBuffer();
        
        Data = null;
        
        _isRentedBuffer = false;
        _bufferContentSize = null;
        
        Pool.Release(this);
    }
    
    #endregion

    #region Byte Pool

    protected static readonly ArrayPool<byte> BytePool = ArrayPool<byte>.GetPool(VoiceManager.FrameByteSize);
    // each individual frame *should* be smaller than FrameByteSize, because that refers to unencoded data...

    public void AllocatePooledBuffer(int encodedSize)
    {
        ReturnPooledBuffer();
        
        Data = BytePool.Spawn();

        if (Data.Length < encodedSize)
            throw new InvalidOperationException("this should never happen: rented buffer is too smol");
        
        _isRentedBuffer = true;
        _bufferContentSize = encodedSize;
    }

    private void ReturnPooledBuffer()
    {
        if (Data == null || !_isRentedBuffer)
            return;
        
        BytePool.Despawn(Data);
        
        Data = null;
        
        _isRentedBuffer = false;
        _bufferContentSize = null;
    }
    
    #endregion
}