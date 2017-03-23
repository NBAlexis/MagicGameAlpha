using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ILuaRequestData
{
    public abstract Dictionary<string, object> GetRequestData(int iNetworkMessageId);
    public virtual int Test(string what, List<int> whatagain, float[] ww)
    {
        return 0;
    }
}
