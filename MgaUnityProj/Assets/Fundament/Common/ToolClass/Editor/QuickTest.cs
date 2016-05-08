using UnityEngine;
using UnityEditor;
using System.Collections;

public class QuickTest 
{
    [MenuItem("MGA/CMDs/Common/Quick Test")]
    static void CountLineFunc()
    {
        GameObject go = AssetDatabase.LoadMainAssetAtPath("Assets/Fundament/BattleField/Artwork/Nature/Decorates/Tree_2_1.fbx") as GameObject;
        if (null != go)
        {
            foreach (MeshFilter mf in go.GetComponentsInChildren<MeshFilter>(true))
            {
                //Mesh editablemesh = (Mesh)EditorCommonFunctions.GetReadWritable(mf.sharedMesh);

                if (mf.transform == go.transform)
                {
                    CRuntimeLogger.Log("add 1");
                }
                else
                {
                    CRuntimeLogger.Log("add 2");
                }
            }
        }
    }
}
