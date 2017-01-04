using System.CodeDom;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPathGizmoShow
{
    EPGS_None,
    EPGS_HeightWalkableNormal,
    EPGS_CommonOptimize,
    EPGS_OpenList,
    EPGS_StrictAStar,
    EPGS_Swamp,
    EPGS_JPS,
    EPGS_BlockAStar,
}

[ExecuteInEditMode]
public class APathTester : MonoBehaviour
{
    public APahtData m_pPathData;
    public EPathGizmoShow m_eGizmo;

    public GameObject m_pStart;
    public GameObject m_pEnd;

    public bool TestPath = false;

    public bool m_bSwampPath = false;
    public Stack<Vector2> m_v2PathSwamp = null;

    public bool m_bJPSPath = false;
    public Stack<Vector2> m_v2PathJPS = null;

    public bool m_bBlockAStarPath = false;
    public Stack<Vector2> m_v2PathBlockAStar = null;

    public Vector2[] m_v2PathStrictAStar = null;
    public bool m_bHasStrictAStarPath = false;

    public int GridX = 0;
    public int GridY = 0;

    private bool StartPerformanceTest = false;
    private bool Initialed = false;

    void Start () 
    {
	
	}
	
	void Update ()
    {
        #region Editor test

        if (TestPath)
	    {
	        TestPath = false;

	        if (null != m_pStart && null != m_pEnd)
	        {
                switch (m_eGizmo)
                {
                    case EPathGizmoShow.EPGS_StrictAStar:
                        {
                            m_bHasStrictAStarPath = true;
                            float flength = 0.0f;
                            S2DPoint[] res = GridMath.StrictAStar(
                                new Vector2(m_pStart.transform.position.x, m_pStart.transform.position.z),
                                new Vector2(m_pEnd.transform.position.x, m_pEnd.transform.position.z),
                                m_pPathData.m_byGridStateData,
                                out flength
                                );     
                   
                            m_v2PathStrictAStar = new Vector2[res.Length];
                            for (int i = 0; i < res.Length; ++i)
                            {
                                m_v2PathStrictAStar[i] = res[i].ToV2();
                            }
                            CRuntimeLogger.Log(flength);
                        }
                        break;
/*
                    case EPathGizmoShow.EPGS_Swamp:
                        {
                        m_pPathData.ResetGrids();
                        EPathRes eres = EPathRes.EPR_NoPath;
                        m_v2PathSwamp = m_pPathData.FindPathAStar(
                            new Vector2(m_pStart.transform.position.x, m_pStart.transform.position.z),
                            new Vector2(m_pEnd.transform.position.x, m_pEnd.transform.position.z),
                            -1,
                            out eres
                            );
                        m_bSwampPath = true;
                        }
                        break;
 */
                    case EPathGizmoShow.EPGS_JPS:
                        {
                        m_pPathData.BakePruningRule();
                        m_pPathData.InitialBlockbasedGrid();
                        m_pPathData.ResetGrids();
                        
                        EPathRes eres = EPathRes.EPR_NoPath;
                        m_v2PathJPS = m_pPathData.FindPathJPS(
                                new Vector2(m_pStart.transform.position.x, m_pStart.transform.position.z),
                                new Vector2(m_pEnd.transform.position.x, m_pEnd.transform.position.z),
                                30,
                                out eres
                                );
                            m_bJPSPath = true;                        
                        }
                        break;
/*
                    case EPathGizmoShow.EPGS_BlockAStar:
                        {
                            m_pPathData.ResetBlockAStarBlocks();

                            EPathRes eres = EPathRes.EPR_NoPath;
                            m_v2PathBlockAStar = m_pPathData.FindPathBlockAStar(
                                    new Vector2(m_pStart.transform.position.x, m_pStart.transform.position.z),
                                    new Vector2(m_pEnd.transform.position.x, m_pEnd.transform.position.z),
                                    -1,
                                    out eres
                                    );
                            m_bBlockAStarPath = true;
                        }
                        break;
*/
                }
	        }
        }

        #endregion

        #region Running Test

        if (StartPerformanceTest)
	    {
	        for (int i = 0; i < 200; ++i)
	        {
                S2DPoint p1 = S2DPoint.random;
                S2DPoint p2 = S2DPoint.random;
                EPathRes eres = EPathRes.EPR_NoPath;

                //m_pPathData.FindPathAStarOld(p1.ToV2(), p2.ToV2(), -1, out eres);
                //m_pPathData.FindPathAStar(p1.ToV2(), p2.ToV2(), -1, out eres);
                //m_pPathData.FindPathBlockAStar(p1.ToV2(), p2.ToV2(), -1, out eres);
                //m_pPathData.FindPathJPSOld(p1.ToV2(), p2.ToV2(), -1, out eres);
                //m_pPathData.FindPathJPS(p1.ToV2(), p2.ToV2(), -1, out eres);
                //m_pPathData.FindPathJPSMJ(p1.ToV2(), p2.ToV2(), -1, out eres);
                //blocked jps without multi-jump is the best, it expand more open-list at same time!(so it should be slower..but, not...)

                //for a partial path, or non-reachable, expand more open-list is even better!
                m_pPathData.FindPathJPS(p1.ToV2(), p2.ToV2(), 30, out eres);
                m_pPathData.FindPathJPSMJ(p1.ToV2(), p2.ToV2(), 7, out eres);
	        }
        }

        #endregion
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100.0f, 100.0f, 200.0f, 30.0f),  StartPerformanceTest ? "Stop Test" : "Start Test"))
        {
            StartPerformanceTest = !StartPerformanceTest;
            if (StartPerformanceTest && !Initialed)
            {
                m_pPathData.InitialAll();
                //m_pPathData.ResetBlockAStarBlocks();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (null != m_pPathData)
        {
            switch (m_eGizmo)
            {
                case EPathGizmoShow.EPGS_StrictAStar:
                    DrawStrictAStarPath();
                    break;
                case EPathGizmoShow.EPGS_HeightWalkableNormal:
                    DrawGizmoHeightWalkNormal();
                    break;
                case EPathGizmoShow.EPGS_CommonOptimize:
                    DrawCommonOptimize();
                    break;
                case EPathGizmoShow.EPGS_OpenList:
                    DrawOpenList();
                    break;
                case EPathGizmoShow.EPGS_Swamp:
                    DrawSwamp();
                    break;
                case EPathGizmoShow.EPGS_JPS:
                    DrawJPS();
                    break;
                case EPathGizmoShow.EPGS_BlockAStar:
                    DrawBlockAStar();
                    break;
            }
        }

        if (null != m_pPathData
            && GridX >= 0 && GridX < SceneConstant.m_iSceneSize
            && GridY >= 0 && GridY < SceneConstant.m_iSceneSize)
        {
            for (int i = GridX - 2; i <= GridX + 2; ++i)
            {
                for (int j = GridY - 2; j <= GridY + 2; ++j)
                {
                    S2DPoint pt = new S2DPoint(i, j);
                    if (pt.m_bValid)
                    {
                        Gizmos.color = (GridMath.m_byWalkable == m_pPathData.m_byGridStateData[pt])
                            ? Color.blue
                            : Color.red;

                        Gizmos.DrawCube(new Vector3(pt.ToV2().x, 2.0f, pt.ToV2().y), Vector3.one
                            * (1.0f / (1.0f + Mathf.Max( Mathf.Abs(i - GridX), Mathf.Abs(j - GridY) ))) );                        
                    }
                }                
            }
        }
    }

    private void DrawStrictAStarPath()
    {
        if (m_bHasStrictAStarPath)
        {
            if (null != m_v2PathStrictAStar && m_v2PathStrictAStar.Length >= 2)
            {
                for (int i = 0; i < m_v2PathStrictAStar.Length - 1; ++i)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(
                        new Vector3(m_v2PathStrictAStar[i].x, 3.0f, m_v2PathStrictAStar[i].y),
                        new Vector3(m_v2PathStrictAStar[i + 1].x, 3.0f, m_v2PathStrictAStar[i + 1].y));
                }
            }
        }
    }

    private void DrawOpenList()
    {
#if DebugJPSOpenlist
        if (null != m_pPathData 
         && null != m_pPathData._debugJpsOpenList && SceneConstant.m_iSceneSizeSq == m_pPathData._debugJpsOpenList.Length
         && null != m_pPathData._debugJpsDeadList && SceneConstant.m_iSceneSizeSq == m_pPathData._debugJpsDeadList.Length)
        {
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    S2DPoint pt = new S2DPoint(i, j);
                    if (m_pPathData._debugJpsOpenList[pt] >= 0 && m_pPathData._debugJpsOpenList[pt] != pt)
                    {
                        S2DPoint parent = m_pPathData._debugJpsOpenList[pt];
                        if (parent.m_bValid)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(
                                new Vector3(pt.ToV2().x, 5.0f, pt.ToV2().y),
                                new Vector3(parent.ToV2().x, 5.0f, parent.ToV2().y));                               
                        }
                    }

                    if (m_pPathData._debugJpsDeadList[pt] >= 0 && m_pPathData._debugJpsDeadList[pt] != pt)
                    {
                        S2DPoint parent = m_pPathData._debugJpsDeadList[pt];
                        if (parent.m_bValid)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawLine(
                                new Vector3(pt.ToV2().x, 4.5f, pt.ToV2().y),
                                new Vector3(parent.ToV2().x, 4.5f, parent.ToV2().y));
                        }
                    }
                }                
            }
        }   
#endif
    }

    private void DrawGizmoHeightWalkNormal()
    {
        if (SceneConstant.m_iSceneSize*SceneConstant.m_iSceneSize == m_pPathData.m_fGridHeightData.Length
         && SceneConstant.m_iSceneSize*SceneConstant.m_iSceneSize == m_pPathData.m_byGridStateData.Length
         && SceneConstant.m_iSceneSize*SceneConstant.m_iSceneSize == m_pPathData.m_v3NormalData.Length)
        {
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    Vector3 vPos1 = new Vector3(
                        i - SceneConstant.m_iSceneSize / 2 + 0.5f, 
                        m_pPathData.m_fGridHeightData[i * SceneConstant.m_iSceneSize + j], 
                        j - SceneConstant.m_iSceneSize / 2 + 0.5f);
                    Vector3 vPos2 = vPos1 + m_pPathData.m_v3NormalData[i * SceneConstant.m_iSceneSize + j] * 3.0f;
                    Gizmos.color = GridMath.m_byWalkable ==
                                   m_pPathData.m_byGridStateData[i*SceneConstant.m_iSceneSize + j]
                        ? Color.blue
                        : Color.red;
                    Gizmos.DrawLine(vPos1, vPos2);
                }                
            }
        }
    }

    public int m_iRectIndex = 0;
    public int m_iIslandIndex = 0;
    public int m_iNearIndex = 0;

    private void DrawCommonOptimize()
    {
        //====================================
        //Line Rect
        if (null != m_pPathData.m_ushGridRectangleData 
        && SceneConstant.m_iSceneSizeSq == m_pPathData.m_ushGridRectangleData.Length)
        {
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    if (0 != (m_pPathData.m_ushGridRectangleData[i * SceneConstant.m_iSceneSize + j] & (1 << m_iRectIndex)))
                    {
                        Gizmos.color = new Color((16 - m_iRectIndex) / 15.0f, m_iRectIndex / 15.0f, m_iRectIndex / 15.0f);
                        Vector3 vPos1 = new Vector3(
                            i - SceneConstant.m_iSceneSize / 2 + 0.5f,
                            3.0f,
                            j - SceneConstant.m_iSceneSize / 2 + 0.5f);
                        Gizmos.DrawWireCube(vPos1, Vector3.one * 0.4f);
                    }
                }
            }            
        }

        //====================================
        //Island
        if (null != m_pPathData.m_byIslandData
        && SceneConstant.m_iSceneSizeSq == m_pPathData.m_byIslandData.Length)
        {
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    if (m_pPathData.m_byIslandData[i * SceneConstant.m_iSceneSize + j] > 0
                     && m_iIslandIndex == m_pPathData.m_byIslandData[i * SceneConstant.m_iSceneSize + j])
                    {
                        Gizmos.color = new Color((16 - m_iIslandIndex) / 15.0f, m_iIslandIndex / 15.0f, m_iIslandIndex / 15.0f);
                        Vector3 vPos1 = new Vector3(
                            i - SceneConstant.m_iSceneSize / 2 + 0.5f,
                            5.0f,
                            j - SceneConstant.m_iSceneSize / 2 + 0.5f);
                        Gizmos.DrawWireCube(vPos1, Vector3.one * 0.2f);
                    }
                }
            }
        }

        //=====================================================
        //near grid
        if (null != m_pPathData.m_shNearGridData
        && SceneConstant.m_iSceneSizeSq * (m_pPathData.m_byIslandNumber + 1) == m_pPathData.m_shNearGridData.Length
        && m_iNearIndex <= m_pPathData.m_byIslandNumber)
        {
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    S2DPoint pt = m_pPathData.m_shNearGridData[m_iNearIndex * SceneConstant.m_iSceneSizeSq + i * SceneConstant.m_iSceneSize + j];
                    if (pt != new S2DPoint(i, j))
                    {
                        Gizmos.color = Color.blue;
                        Vector3 vPos1 = new Vector3(
                            i - SceneConstant.m_iSceneSize / 2 + 0.5f,
                            2.0f,
                            j - SceneConstant.m_iSceneSize / 2 + 0.5f);

                        Vector3 vPos2 = new Vector3(
                            pt.m_iX - SceneConstant.m_iSceneSize / 2 + 0.5f,
                            2.0f,
                            pt.m_iY - SceneConstant.m_iSceneSize / 2 + 0.5f);
                        Gizmos.DrawLine(vPos2, vPos1);
                    }
                }
            }
        }
    }

    public int DrawSwampMaxDimession = 7;
    private void DrawSwamp()
    {
        if (m_bSwampPath && null != m_v2PathSwamp && m_v2PathSwamp.Count >= 2)
        {
            /* MDZZ
            Vector2 p1 = m_v2PathSwamp.Pop();
            int iCount = m_v2PathSwamp.Count;
            for (int i = 0; i < iCount; ++i)
            {
                Vector2 p2 = m_v2PathSwamp.Pop();
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3(p1.x, 3.0f, p1.y),
                    new Vector3(p2.x, 3.0f, p2.y));
                p1 = p2;
            }
            */
            Vector2[] pts = m_v2PathSwamp.ToArray();
            for (int i = 0; i < pts.Length - 1; ++i)
            {
                Vector2 p1 = pts[i];
                Vector2 p2 = pts[i + 1];
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3(p1.x, 3.0f, p1.y),
                    new Vector3(p2.x, 3.0f, p2.y));
            }
        }
        else
        {
            if (SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize == m_pPathData.m_fGridHeightData.Length
             && SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize == m_pPathData.m_byGridStateData.Length
             && SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize == m_pPathData.m_v3NormalData.Length
             && null != m_pPathData.m_bySwampData
             && SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize == m_pPathData.m_bySwampData.Length)
            {
                for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
                {
                    for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                    {
                        byte iSwampIndex = m_pPathData.m_bySwampData[i * SceneConstant.m_iSceneSize + j];
                        Gizmos.color = new Color(
                            ((int)(iSwampIndex / (DrawSwampMaxDimession * DrawSwampMaxDimession))) / ((float)DrawSwampMaxDimession),
                            ((int)((iSwampIndex % (DrawSwampMaxDimession * DrawSwampMaxDimession)) / DrawSwampMaxDimession)) / ((float)DrawSwampMaxDimession),
                            (iSwampIndex % DrawSwampMaxDimession) / ((float)DrawSwampMaxDimession));
                        Vector3 vPos1 = new Vector3(
                            i - SceneConstant.m_iSceneSize / 2 + 0.5f,
                            m_pPathData.m_fGridHeightData[i * SceneConstant.m_iSceneSize + j] + 0.5f,
                            j - SceneConstant.m_iSceneSize / 2 + 0.5f);
                        if (GridMath.m_byWalkable == m_pPathData.m_byGridStateData[i * SceneConstant.m_iSceneSize + j])
                        {
                            Gizmos.DrawCube(vPos1, Vector3.one * 0.8f);
                        }
                    }
                }
            }            
        }
    }

    private void DrawJPS()
    {
        if (m_bJPSPath && null != m_v2PathJPS && m_v2PathJPS.Count >= 2)
        {
            Vector2[] pts = m_v2PathJPS.ToArray();
            for (int i = 0; i < pts.Length - 1; ++i)
            {
                Vector2 p1 = pts[i];
                Vector2 p2 = pts[i + 1];
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3(p1.x, 3.0f, p1.y),
                    new Vector3(p2.x, 3.0f, p2.y));
            }
        }        
    }

    private void DrawBlockAStar()
    {
        /*
        if (null != m_pPathData.m_pBigGrids && (22 * 22) == m_pPathData.m_pBigGrids.Length
         && null != m_pPathData.m_pSmallGrids && (66 * 66) == m_pPathData.m_pSmallGrids.Length)
        {
            
            for (int i = 0; i < 22; ++i)
            {
                for (int j = 0; j < 22; ++j)
                {
                    if (null != m_pPathData.m_pBigGrids[i*22 + j].m_pNext)
                    {
                        Vector2 p1 = m_pPathData.m_pBigGrids[i * 22 + j].m_pChildGird[4].m_sWorldPos.ToV2();
                        Vector2 p2 = m_pPathData.m_pBigGrids[i * 22 + j].m_pNext.m_pChildGird[4].m_sWorldPos.ToV2();
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(
                            new Vector3(p1.x, 3.0f, p1.y),
                            new Vector3(p2.x, 3.0f, p2.y));                        
                    }
                }                
            }
            
            for (int i = 0; i < 66; ++i)
            {
                for (int j = 0; j < 66; ++j)
                {
                    if (m_pPathData.m_pSmallGrids[i*66 + j].m_sWorldPos.m_bValid
                     && m_pPathData.m_iFindingIndex == m_pPathData.m_pSmallGrids[i * 66 + j].m_iGCalculated)
                    {
                        Vector2 p1 = m_pPathData.m_pSmallGrids[i * 66 + j].m_sWorldPos.ToV2();
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(
                            new Vector3(p1.x, 4.0f, p1.y),
                            Vector3.one * 0.3f); 
                    }
                }
            }
        }
        */

        if (m_bBlockAStarPath && null != m_v2PathBlockAStar && m_v2PathBlockAStar.Count >= 2)
        {
            Vector2[] pts = m_v2PathBlockAStar.ToArray();
            for (int i = 0; i < pts.Length - 1; ++i)
            {
                Vector2 p1 = pts[i];
                Vector2 p2 = pts[i + 1];
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3(p1.x, 3.0f, p1.y),
                    new Vector3(p2.x, 3.0f, p2.y));
            }
        }  
    }
}
