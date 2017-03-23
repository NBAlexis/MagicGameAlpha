using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class QuickTest 
{
    [MenuItem("MGA/CMDs/Common/Quick Test")]
    static void CountLineFunc()
    {
        CPseduLuaValue a = 1;
        object b = 1;
        CRuntimeLogger.Log(a == b);
        int c = 1;
        CRuntimeLogger.Log(a == c);
        CRuntimeLogger.Log((a + b).ToString());
        CRuntimeLogger.Log(a + b);

        /*
        v = new[]{"abc", "def"};
        CRuntimeLogger.Log(v[1].ToString());
        v = new Dictionary<string, object>
        {
            {"abc", new object[]{1, 2, 3}},
        };
        CRuntimeLogger.Log(v["abc"][2].ToString());
        v["kkk"] = 1.2f;
        CRuntimeLogger.Log(v["kkk"].ToString());
        v[1] = 2.2f;
        CRuntimeLogger.Log(v["kkk"].ToString());
        CRuntimeLogger.Log(v[1].ToString());
        v = v["abc"];
        CRuntimeLogger.Log(v[1].ToString());
        CRuntimeLogger.Log(v[1][0][0].ToString());
         */
    }
}
