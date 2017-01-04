using UnityEngine;
using UnityEditor;
using System.Collections;

public class QuickTest 
{
    [MenuItem("MGA/CMDs/Common/Quick Test")]
    static void CountLineFunc()
    {
        int[,] test =
        {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9},
        };
        CRuntimeLogger.Log(test[0, 0].ToString() + test[1, 0].ToString() + test[2, 0]);
    }
}
