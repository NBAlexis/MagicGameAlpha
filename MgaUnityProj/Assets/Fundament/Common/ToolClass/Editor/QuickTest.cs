using UnityEngine;
using UnityEditor;
using System.Collections;

public class QuickTest 
{
    [MenuItem("MGA/CMDs/Common/Quick Test")]
    static void CountLineFunc()
    {
        CSceneDecorate dec = new CSceneDecorate {m_sSceneType = "Nature"};
        dec.Load();
    }
}
