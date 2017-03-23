using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ILuaMessageHandler
{
    public abstract void OnReceive(int iNetworkMessageId, Dictionary<string, object> data);
}
