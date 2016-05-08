using UnityEngine;
using System.Collections;
using UnityEditor;

public class ClearResourcesManager
{

    [MenuItem("MGA/CMDs/Common/Clear Resources Mananger")]
    static void CountLineFunc()
    {
        ResourcesManager.Clear();
        AssetDatabase.Refresh();
        CRuntimeLogger.Log("Done!");
    }
}
