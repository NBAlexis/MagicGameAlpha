using UnityEditor;
using UnityEngine;
using System.Collections;

public class BreakPrefab {

    [MenuItem("MGA/CMDs/Common/Disconnet Prefab")]
    static void DisconnetPrefab()
    {
        GameObject disconnectingObj = Selection.activeObject as GameObject;
        if (null != disconnectingObj)
        {
            PrefabUtility.DisconnectPrefabInstance(disconnectingObj);
            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/dummy_dummy.prefab");
            PrefabUtility.ReplacePrefab(disconnectingObj, prefab, ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.DisconnectPrefabInstance(disconnectingObj);
            AssetDatabase.DeleteAsset("Assets/dummy_dummy.prefab");
        }
    }
}
