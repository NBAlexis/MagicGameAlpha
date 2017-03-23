using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CMGNetworkMessage
{
    public enum ENetworkMessageDataElementType : byte
    {
        ENMDET_Int,
        ENMDET_Long,
        ENMDET_Float,
        ENMDET_String,
        ENMDET_ByteArray,
    }

    public const int MaxBuffSize = 4096;
    public static byte[] buff = new byte[MaxBuffSize];

    public int m_iMessageId;
    public Dictionary<byte, object> m_pData;

    public byte[] ToByte()
    {
        return null;
    }

    public void Decode(Stream stream, int iStart)
    {
        //parse id
        stream.Read(buff, 0, 4);

        //parse diction count
        stream.Read(buff, 0, 4);


    }
}

public abstract class ISocket 
{
    public delegate void ReceiveMsgDelegate(CMGNetworkMessage msg);
    public delegate void OnDisconnect();

    public abstract void Initial(string sIp, int iPort, ReceiveMsgDelegate callback, OnDisconnect disconnectCallback);
    public abstract bool IsConnected();
    public abstract bool IsSending();
    public abstract bool SendMsg(CMGNetworkMessage msg);
}
