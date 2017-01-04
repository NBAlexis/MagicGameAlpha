using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PathBuilder : EditorWindow
{
    public Texture2D m_pGroundHeightmap;
    public Texture2D m_pGroundNormalmap;
    public APahtData m_pPathData;

    [MenuItem("MGA/Create/Scene/Create Path")]
    public static void ShowWindow()
    {
        PathBuilder pEditor = (PathBuilder)GetWindow(typeof(PathBuilder));
        pEditor.Initial();
    }

    protected void Initial()
    {
    }

    protected void OnGUI()
    {
        //heightmap, r is height, r and b are walkables
        m_pGroundHeightmap = (Texture2D)EditorGUILayout.ObjectField("地面图(walkmap)", m_pGroundHeightmap, typeof(Texture2D), false);
        m_pGroundNormalmap = (Texture2D)EditorGUILayout.ObjectField("地面法线图(normalmap)", m_pGroundNormalmap, typeof(Texture2D), false);

        if (null != m_pGroundHeightmap && null != m_pGroundNormalmap)
        {
            if (GUILayout.Button("创建基础信息"))
            {
                BuildBaseData();
            }
        }

        m_pPathData = (APahtData)EditorGUILayout.ObjectField("已创建的数据", m_pPathData, typeof(APahtData), true);
        if (null != m_pPathData)
        {
            if (GUILayout.Button("创建Swamp信息"))
            {
                m_pPathData.FindAllSwamps();
            }
            if (GUILayout.Button("创建JPS邻居表"))
            {
                m_pPathData.BakePruningRule();
            }
            /*
            if (GUILayout.Button("创建 Block AStar的LDDB"))
            {
                m_pPathData.GenerateLDDB();
            }
            */
        }
    }

    //TODO fix the filename of the textures as constant
    private void BuildBaseData()
    {
        #region Step1: Create Script

        GameObject pathDataObj = new GameObject();
        pathDataObj.name = "PathData";
        pathDataObj.transform.position = Vector3.zero;

        APahtData pathData = pathDataObj.AddComponent<APahtData>();

        #endregion

        #region Step2: Load HeightData and Obstacle 1

        EditorUtility.DisplayProgressBar("创建中", "正在创建行走数据以及高度法线数据, 1/34", 1 / 34.0f);

        Texture2D hTx = (Texture2D)EditorCommonFunctions.GetReadWritable(m_pGroundHeightmap);
        Texture2D nTx = (Texture2D)EditorCommonFunctions.GetReadWritable(m_pGroundNormalmap);
        pathData.m_fGridHeightData = new float[SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize];
        pathData.m_byGridStateData = new byte[SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize];
        pathData.m_v3NormalData = new Vector3[SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize];

        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                pathData.m_fGridHeightData[i * SceneConstant.m_iSceneSize + j] = 4.0f * SceneConstant.m_fHighgroundHeight * (hTx.GetPixel(i, j).r - 0.5f);
                pathData.m_byGridStateData[i * SceneConstant.m_iSceneSize + j] = hTx.GetPixel(i, j).g > hTx.GetPixel(i, j).b
                    ? GridMath.m_byWalkable
                    : GridMath.m_byNoWalkable;
                Color cNormal = nTx.GetPixel(i, j);
                pathData.m_v3NormalData[i * SceneConstant.m_iSceneSize + j] = new Vector3(cNormal.r, cNormal.g, cNormal.b);
                pathData.m_v3NormalData[i * SceneConstant.m_iSceneSize + j] = (pathData.m_v3NormalData[i * SceneConstant.m_iSceneSize + j] - new Vector3(0.5f, 0.5f, 0.5f)) * 2.0f;
                pathData.m_v3NormalData[i * SceneConstant.m_iSceneSize + j].Normalize();
            }
        }

        #endregion

        #region Step3: Do Generate

        pathData.GenerateOptimaztion();

        #endregion

        EditorUtility.ClearProgressBar();
    }

    #region JPS

    private void MakeJPSNeighbourTable()
    {
        
    }

    #endregion
}
