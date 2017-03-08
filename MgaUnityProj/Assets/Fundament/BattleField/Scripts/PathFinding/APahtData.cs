//remove those all
//#define DebugAStar

//using System;
using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
//using UnityEngine.UI;
#endif

public enum EPathRes : byte
{
    EPR_Done,
    EPR_Incomplete,
    EPR_NoPath,
    EPR_IncompleteBad, //There is an incomplete path with lowest F farther from target.
}

public class COneGrid
{
    public S2DPoint m_sPos;
    public Vector2 m_v2Pos;

    public const float m_fSwampDist = 100.0f;

    public int m_iOpenList;
    public int m_iCloseList;
    public int m_iHCalculated;
    public COneGrid m_pParentNode;
    public int m_iParentNode;

    public float m_fFValue;
    public float m_fGValue;
    public float m_fHValue;

    public bool m_bOccupied = false; //TODO initial occupy

#if DebugAStar
    public COneGrid[] m_pAllConnected;
    public float[] m_fConnectDist;
    public int[] m_iInverseIndex;
#endif

    public byte m_bySwampTag;
    public ushort m_usCleanRect;

    #region LinkList

    public COneGrid m_pPrev = null;
    public COneGrid m_pNext = null;

    #endregion

#if DebugAStar
    #region Astar use

    public void CalculateWeight(S2DPoint vFrom, S2DPoint vTo, int iPathFindIndex)
    {
        m_fGValue = (null == m_pParentNode) ? 0.0F : (m_pParentNode.m_fGValue + m_fConnectDist[m_iParentNode]);

        //Bigger H, worse choise
        if (m_iHCalculated != iPathFindIndex)
        {
            m_fHValue = Mathf.Abs(vTo.m_iX - m_sPos.m_iX)+ Mathf.Abs(vTo.m_iY - m_sPos.m_iY);
            m_iHCalculated = iPathFindIndex;
        }

        m_fFValue = m_fGValue + m_fHValue;
    }

    public void CalculateWeightSwamp(S2DPoint vFrom, S2DPoint vTo, int iPathFindIndex, byte bySwampStart, byte bySwampEnd)
    {
        m_fGValue = (null == m_pParentNode) ? 0.0F : (m_pParentNode.m_fGValue + m_fConnectDist[m_iParentNode]);

        //Bigger H, worse choise
        if (m_iHCalculated != iPathFindIndex)
        {
            m_fHValue = Mathf.Abs(vTo.m_iX - m_sPos.m_iX) + Mathf.Abs(vTo.m_iY - m_sPos.m_iY);
            if (0 != m_bySwampTag && bySwampStart != m_bySwampTag && bySwampEnd != m_bySwampTag)
            {
                m_fHValue += m_fSwampDist;
            }
            m_iHCalculated = iPathFindIndex;
        }

        m_fFValue = m_fGValue + m_fHValue;
    }

    public bool GViaMe(COneGrid pParent, int iParentIndex)
    {
        float fNewG = pParent.m_fGValue + m_fConnectDist[iParentIndex];
        if (fNewG < m_fGValue)
        {
            m_fGValue = fNewG;
            m_fFValue = m_fGValue + m_fHValue;
            m_pParentNode = pParent;
            m_iParentNode = iParentIndex;
            return true;
        }
        return false;
    }

    #endregion
#endif

    #region JPS use

    /// <summary>
    /// parent to me
    /// </summary>
    public int m_iParentDir = -1;

    //public S2DPoint m_sDoubleParent = S2DPoint.invalid;

    public void CalculateWeightJPS(S2DPoint vFrom, S2DPoint vTo, int iPathFindIndex, byte bySwampStart, byte bySwampEnd)
    {
        m_fGValue = 0.0f;
        //m_sDoubleParent = S2DPoint.invalid;
        if (null != m_pParentNode)
        {
            if (m_sPos.m_iX == m_pParentNode.m_sPos.m_iX)
            {
                m_fGValue = m_pParentNode.m_fGValue + Mathf.Abs(m_sPos.m_iY - m_pParentNode.m_sPos.m_iY) * GridMath.m_fStraightDist;
            }
            else if (m_sPos.m_iY == m_pParentNode.m_sPos.m_iY)
            {
                m_fGValue = m_pParentNode.m_fGValue + Mathf.Abs(m_sPos.m_iX - m_pParentNode.m_sPos.m_iX) * GridMath.m_fStraightDist;
            }
            else
            {
                m_fGValue = m_pParentNode.m_fGValue + Mathf.Abs(m_sPos.m_iY - m_pParentNode.m_sPos.m_iY) * GridMath.m_fDiagDist;
            }            
        }

        //Bigger H, worse choise
        if (m_iHCalculated != iPathFindIndex)
        {
            m_fHValue = Mathf.Abs(vTo.m_iX - m_sPos.m_iX) + Mathf.Abs(vTo.m_iY - m_sPos.m_iY);
            if (0 != m_bySwampTag && bySwampStart != m_bySwampTag && bySwampEnd != m_bySwampTag)
            {
                m_fHValue += m_fSwampDist;
            }
            m_iHCalculated = iPathFindIndex;
        }

        m_fFValue = m_fGValue + m_fHValue;
    }

    public bool GViaMeJPS(COneGrid pParent, int iDir)
    {
        float fNewG;
        if (m_sPos.m_iX == pParent.m_sPos.m_iX)
        {
            fNewG = pParent.m_fGValue + Mathf.Abs(m_sPos.m_iY - pParent.m_sPos.m_iY) * GridMath.m_fStraightDist;
        }
        else if (m_sPos.m_iY == pParent.m_sPos.m_iY)
        {
            fNewG = pParent.m_fGValue + Mathf.Abs(m_sPos.m_iX - pParent.m_sPos.m_iX) * GridMath.m_fStraightDist;
        }
        else
        {
            fNewG = pParent.m_fGValue + Mathf.Abs(m_sPos.m_iY - pParent.m_sPos.m_iY) * GridMath.m_fDiagDist;
        }

        if (fNewG < m_fGValue - 0.01f) //make have little error
        {
            //m_sDoubleParent = S2DPoint.invalid;
            m_fGValue = fNewG;
            m_fFValue = m_fGValue + m_fHValue;
            m_pParentNode = pParent;
            m_iParentDir = iDir;
            return true;
        }
        return false;
    }

    /// <summary>
    /// for add double jump points
    /// </summary>
    /// <param name="vFrom"></param>
    /// <param name="vTo"></param>
    /// <param name="iPathFindIndex"></param>
    /// <param name="bySwampStart"></param>
    /// <param name="bySwampEnd"></param>
    public void CalculateWeightJPSDouble(S2DPoint vFrom, S2DPoint vTo, /*S2DPoint midparent,*/ int iPathFindIndex, byte bySwampStart, byte bySwampEnd)
    {
        m_fGValue = 0.0f;
        //m_sDoubleParent = midparent;
        if (null != m_pParentNode)
        {
            int dX = Mathf.Abs(m_sPos.m_iX - m_pParentNode.m_sPos.m_iX);
            int dY = Mathf.Abs(m_sPos.m_iY - m_pParentNode.m_sPos.m_iY);
            if (dX > dY)
            {
                m_fGValue = (dX - dY) * GridMath.m_fStraightDist + dY * GridMath.m_fDiagDist + m_pParentNode.m_fGValue;
            }
            else
            {
                m_fGValue = (dY - dX) * GridMath.m_fStraightDist + dX * GridMath.m_fDiagDist + m_pParentNode.m_fGValue;
            }
        }

        //Bigger H, worse choise
        if (m_iHCalculated != iPathFindIndex)
        {
            m_fHValue = Mathf.Abs(vTo.m_iX - m_sPos.m_iX) + Mathf.Abs(vTo.m_iY - m_sPos.m_iY);
            if (0 != m_bySwampTag && bySwampStart != m_bySwampTag && bySwampEnd != m_bySwampTag)
            {
                m_fHValue += m_fSwampDist;
            }
            m_iHCalculated = iPathFindIndex;
        }

        m_fFValue = m_fGValue + m_fHValue;
    }

    public bool GViaMeJPSDouble(COneGrid pParent, /*S2DPoint midparent,*/ int iDir)
    {
        float fNewG;
        int dX = Mathf.Abs(m_sPos.m_iX - pParent.m_sPos.m_iX);
        int dY = Mathf.Abs(m_sPos.m_iY - pParent.m_sPos.m_iY);
        if (dX > dY)
        {
            fNewG = (dX - dY) * GridMath.m_fStraightDist + dY * GridMath.m_fDiagDist + pParent.m_fGValue;
        }
        else
        {
            fNewG = (dY - dX) * GridMath.m_fStraightDist + dX * GridMath.m_fDiagDist + pParent.m_fGValue;
        }

        if (fNewG < m_fGValue - 0.01f) //make have little error
        {
            //m_sDoubleParent = S2DPoint.invalid;
            m_fGValue = fNewG;
            m_fFValue = m_fGValue + m_fHValue;
            m_pParentNode = pParent;
            m_iParentDir = iDir;
            return true;
        }
        return false;
    }

    //TODO check occupy
    /// <summary>
    /// Should be modified after occupation
    /// </summary>
    public byte m_byCachedForceTable = 0;
    public byte[] m_bCachedNeigTable = null;

    #endregion
}

[AddComponentMenu("MGA/GridPath/PathData")]
public class APahtData : MonoBehaviour
{
    //Before any bake, those data should been set.
    #region BaseData

    /// <summary>
    /// The data stores whether the grid is walkable
    /// </summary>
    public byte[] m_byGridStateData;

    /// <summary>
    /// The data stores the height of the ground
    /// </summary>
    public float[] m_fGridHeightData;

    /// <summary>
    /// The data stores the normal of the grid
    /// </summary>
    public Vector3[] m_v3NormalData;

    /// <summary>
    /// Those are for block based optimization, need to be maintained occupation runtime!
    /// </summary>
    public ulong[] m_ulWalkable0;
    public ulong[] m_ulWalkable90; //down to up
    public ulong[] m_ulWalkable180;
    public ulong[] m_ulWalkable270;

    public void InitialBlockbasedGrid()
    {
#if DebugAStar
        if (SceneConstant.m_iSceneSize > 64)
        {
            CRuntimeLogger.LogError("Impossible to generate for size larger then 64!");
            return;
        }
#endif

        m_ulWalkable0 = new ulong[SceneConstant.m_iSceneSize + 2];
        m_ulWalkable90 = new ulong[SceneConstant.m_iSceneSize + 2];
        m_ulWalkable180 = new ulong[SceneConstant.m_iSceneSize + 2];
        m_ulWalkable270 = new ulong[SceneConstant.m_iSceneSize + 2];

        //add edge
        m_ulWalkable0[0] = 0xffffffffffffffffLu;
        m_ulWalkable0[SceneConstant.m_iSceneSize + 1] = 0xffffffffffffffffLu;
        m_ulWalkable90[0] = 0xffffffffffffffffLu;
        m_ulWalkable90[SceneConstant.m_iSceneSize + 1] = 0xffffffffffffffffLu;
        m_ulWalkable180[0] = 0xffffffffffffffffLu;
        m_ulWalkable180[SceneConstant.m_iSceneSize + 1] = 0xffffffffffffffffLu;
        m_ulWalkable270[0] = 0xffffffffffffffffLu;
        m_ulWalkable270[SceneConstant.m_iSceneSize + 1] = 0xffffffffffffffffLu;

        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            ulong ulCurrent0 = 0Lu;
            ulong ulCurrent90 = 0Lu;
            ulong ulCurrent180 = 0Lu;
            ulong ulCurrent270 = 0Lu;

            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                S2DPoint pt0 = new S2DPoint(j, SceneConstant.m_iSceneSize - i - 1);
                S2DPoint pt180 = new S2DPoint(SceneConstant.m_iSceneSize - j - 1, SceneConstant.m_iSceneSize - i - 1);

                S2DPoint pt90 = new S2DPoint(i, SceneConstant.m_iSceneSize - j - 1);
                S2DPoint pt270 = new S2DPoint(i, j);

                if (GridMath.m_byWalkable != m_byGridStateData[pt0])
                {
                    ulCurrent0 |= (1Lu << j);
                }
                if (GridMath.m_byWalkable != m_byGridStateData[pt90])
                {
                    ulCurrent90 |= (1Lu << j);
                }
                if (GridMath.m_byWalkable != m_byGridStateData[pt180])
                {
                    ulCurrent180 |= (1Lu << j);
                }
                if (GridMath.m_byWalkable != m_byGridStateData[pt270])
                {
                    ulCurrent270 |= (1Lu << j);
                }
            }

            m_ulWalkable0[i + 1] = ulCurrent0;
            m_ulWalkable90[i + 1] = ulCurrent90;
            m_ulWalkable180[i + 1] = ulCurrent180;
            m_ulWalkable270[i + 1] = ulCurrent270;
        }

        /*
        for (int i = 1; i < 65; ++i)
        {
            CRuntimeLogger.Log(Convert.ToString((long)m_ulWalkable0[i], 2));
        }
        CRuntimeLogger.Log("===========================");
        for (int i = 1; i < 65; ++i)
        {
            CRuntimeLogger.Log(Convert.ToString((long)m_ulWalkable180[i], 2));
        }
        CRuntimeLogger.Log("===========================");

        for (int i = 1; i < 65; ++i)
        {
            CRuntimeLogger.Log(Convert.ToString((long)m_ulWalkable90[i], 2));
        }
        CRuntimeLogger.Log("===========================");
        for (int i = 1; i < 65; ++i)
        {
            CRuntimeLogger.Log(Convert.ToString((long)m_ulWalkable270[i], 2));
        }
        CRuntimeLogger.Log("===========================");
        */
    }

    #endregion

    #region Constants

    public const float m_fPushAwayRate = 300.0f;
    public const float m_fPushAwayRateBuilding = 3.0f;
    public const float m_fPushAwayRatePawn = 1.0f;

    #endregion

    public void Start () 
    {
	
	}
	
    public void Update()
    {
        ProcessAgentAvoidence();
    }

    #region Dynamic Change

    public void OnOccupy(params S2DPoint[] occupy)
    {
        
    }

    #endregion

    #region Common

    public void GenerateAll(bool bBase, bool bBaseOptimaztion, bool bAStar)
    {
        if (bBase)
        {
            GenerateBaseData();
        }

        if (bBaseOptimaztion)
        {
            GenerateOptimaztion();
        }
    }

    public int m_iFindingIndex = 0;

    public void GenerateBaseData()
    {
        
    }

    public S2DPoint GetNearestWalkable(Vector2 vP)
    {
        return new S2DPoint();
    }

    public Vector2 GetShootingPoint(Vector2 vTarget, float fMin, float fMax)
    {
        return new Vector2(0.0f, 0.0f);
    }

    #endregion

    #region Initial

    /// <summary>
    /// Call this before first path-finding
    /// </summary>
    public void InitialAll()
    {
        BakePruningRule();
        InitialBlockbasedGrid();
        ResetGrids();
    }

    protected COneGrid[,] m_pGrids = null;
    private readonly Stack<Vector2> _tmpRes = new Stack<Vector2>();

    private const int m_iAstarProtect = 4096;

    /// <summary>
    /// Generate the data structure for all path finding use
    /// </summary>
    public void ResetGrids()
    {
        m_pGrids = new COneGrid[SceneConstant.m_iSceneSize, SceneConstant.m_iSceneSize];
        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                m_pGrids[i, j] = new COneGrid
                {
                    m_sPos = new S2DPoint(i, j),
                    m_v2Pos = (new S2DPoint(i, j)).ToV2(),
                    m_iOpenList = 0,
                    m_iCloseList = 0,
                    m_iHCalculated = 0,
                    m_pParentNode = null,

                    m_bySwampTag =
                    (null != m_bySwampData && (i * SceneConstant.m_iSceneSize + j) < m_bySwampData.Length)
                    ? m_bySwampData[i * SceneConstant.m_iSceneSize + j]
                    : (byte)0,

                    m_usCleanRect =
                    (null != m_ushGridRectangleData && (i * SceneConstant.m_iSceneSize + j) < m_ushGridRectangleData.Length)
                    ? m_ushGridRectangleData[i * SceneConstant.m_iSceneSize + j]
                    : (ushort)0,
                };
            }
        }

#if DebugAStar

        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                m_pGrids[i, j].m_iInverseIndex = Enumerable.Repeat(-1, 8).ToArray();
            }
        }

        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                List<COneGrid> adjucent = new List<COneGrid>();
                List<float> dist = new List<float>();
                COneGrid theOne = m_pGrids[i, j];

                for (int ii = i - 1; ii <= i + 1; ++ii)
                {
                    for (int jj = j - 1; jj <= j + 1; ++jj)
                    {
                        if (ii >= 0
                         && ii < SceneConstant.m_iSceneSize
                         && jj >= 0
                         && jj < SceneConstant.m_iSceneSize
                         && (!(ii == i && jj == j)))
                        {
                            if (GridMath.m_byWalkable == m_byGridStateData[ii * SceneConstant.m_iSceneSize + jj])
                            {
                                adjucent.Add(m_pGrids[ii, jj]);
                                dist.Add((m_pGrids[ii, jj].m_v2Pos - theOne.m_v2Pos).magnitude);
                            }
                        }
                    }
                }

                theOne.m_pAllConnected = adjucent.ToArray();
                theOne.m_fConnectDist = dist.ToArray();

            }
        }

        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                COneGrid theOne = m_pGrids[i, j];
                List<int> child = new List<int>();

                for (int k = 0; k < theOne.m_pAllConnected.Length; ++k)
                {
                    for (int l = 0; l < theOne.m_pAllConnected[k].m_pAllConnected.Length; ++l)
                    {
                        if (theOne.m_pAllConnected[k].m_pAllConnected[l] == theOne)
                        {
                            child.Add(l);
                        }
                    }
                }

                theOne.m_iInverseIndex = child.ToArray();

            }
        }
#endif
        m_iFindingIndex = 0;

        CacheForceTableAndNeighborTable();
    }


    public void GenerateAll()
    {
        GenerateOptimaztion();
    }

    #endregion

    #region Common Optimaztion

    //Find rectangles without blocks, to return a path quickly
    #region Clean Rectangle

    /// <summary>
    /// The rectangle without any blocks. store as ushort (can convert to Rectangle).
    /// </summary>
    public ushort[] m_ushGridRectangleData;

    /// <summary>
    /// If 2 point are in a rectangle without any blocks, one can fast reture a path just as a line
    /// This is the function to generate large rectangles without blocks
    /// </summary>
    private void GenerateCleanRectangle()
    {
        m_ushGridRectangleData = new ushort[SceneConstant.m_iSceneSizeSq];
        List<SRectangleData> foundRect = new List<SRectangleData>();
        for (int x = 0; x < 6; ++x)
        {
            for (int y = 0; y < 6; ++y)
            {
                SRectangleData found = FindLargestCleanRectInRect(
                    new SRectangleData
                    {
                        m_sPLT = new S2DPoint(
                            x * SceneConstant.m_iSceneSize / 6 - SceneConstant.m_iSceneSize / 12,
                            y * SceneConstant.m_iSceneSize / 6 - SceneConstant.m_iSceneSize / 12
                        ),
                        m_sPRB = new S2DPoint(
                            (x + 1) * SceneConstant.m_iSceneSize / 6 + SceneConstant.m_iSceneSize / 12,
                            (y + 1) * SceneConstant.m_iSceneSize / 6 + SceneConstant.m_iSceneSize / 12
                        )
                    },
                    m_byGridStateData
                    );

                if (found.IsValid())
                {
                    foundRect.Add(found);
                }
            }
        }

        //sort
        for (int i = 0; i < foundRect.Count; ++i)
        {
            for (int j = i + 1; j < foundRect.Count; ++j)
            {
                if (foundRect[i].Area() < foundRect[j].Area())
                {
                    SRectangleData tmp = foundRect[i];
                    foundRect[i] = foundRect[j];
                    foundRect[j] = tmp;
                }
            }
        }

        //there are at most 16 rectangles
        for (int i = 0; i < foundRect.Count && i < 16; ++i)
        {
            for (int xx = foundRect[i].m_sPLT.m_iX; xx <= foundRect[i].m_sPRB.m_iX; ++xx)
            {
                for (int yy = foundRect[i].m_sPLT.m_iY; yy <= foundRect[i].m_sPRB.m_iY; ++yy)
                {
                    m_ushGridRectangleData[xx * SceneConstant.m_iSceneSize + yy] |= (ushort)(1 << i);
                }
            }
        }
    }

    private static SRectangleData FindLargestCleanRectInRect(SRectangleData rect, byte[] grid)
    {
        int iMaxArea = 0;
        SRectangleData ret = SRectangleData.invalid;

        for (int i = rect.m_sPLT.m_iX; i <= rect.m_sPRB.m_iX; ++i)
        {
            for (int j = rect.m_sPLT.m_iY; j <= rect.m_sPRB.m_iY; ++j)
            {
                //Find the largest as quickly as possible so that the rest will quick fail if area is smaller
                for (int ii = rect.m_sPRB.m_iX; ii > i; --ii)
                {
                    for (int jj = rect.m_sPRB.m_iY; jj > j; --jj)
                    {
                        SRectangleData rct = new SRectangleData
                        {
                            m_sPLT = new S2DPoint(i, j),
                            m_sPRB = new S2DPoint(ii, jj)
                        };

                        int iAreaNow = rct.Area();
                        if (iAreaNow > iMaxArea && rct.IsValid() && IsRectClean(rct, grid))
                        {
                            iMaxArea = iAreaNow;
                            ret = rct;
                        }
                    }
                }
            }
        }
        return ret;
    }

    private static bool IsRectClean(SRectangleData rect, byte[] grid)
    {
        for (int i = rect.m_sPLT.m_iX; i <= rect.m_sPRB.m_iX; ++i)
        {
            for (int j = rect.m_sPLT.m_iY; j <= rect.m_sPRB.m_iY; ++j)
            {
                if (GridMath.m_byWalkable != grid[i * SceneConstant.m_iSceneSize + j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    #endregion

    //Divide grids into seperate island if the map is not a connect map
    #region Islands

    /// <summary>
    /// We shall have islands fewer then 255
    /// </summary>
    public byte m_byIslandNumber;

    /// <summary>
    /// Stores the index + 1 of the island the grid belongs
    /// 0 is the blocks
    /// </summary>
    public byte[] m_byIslandData;

    private void GenerateIsland()
    {
        byte[,] tmpWalkable = new byte[SceneConstant.m_iSceneSize, SceneConstant.m_iSceneSize];
        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                tmpWalkable[i, j] = m_byGridStateData[new S2DPoint(i, j)];
            }            
        }

        m_byIslandData = new byte[SceneConstant.m_iSceneSizeSq];
        m_byIslandNumber = 0;

        S2DPoint firstPoint = FindAWalkableGroundPoint(tmpWalkable);

        while (firstPoint.m_bValid)
        {
            tmpWalkable[firstPoint.m_iX, firstPoint.m_iY] = 2;
            GridMath.ExpandPaint(ref tmpWalkable, SceneConstant.m_iSceneSize, SceneConstant.m_iSceneSize);

            byte[] tmpWalkabledata = Enumerable.Repeat(GridMath.m_byNoWalkable, SceneConstant.m_iSceneSizeSq).ToArray();
            int iIslandWalkable = 0;
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    if (2 == tmpWalkable[i, j])
                    {
                        S2DPoint pt = new S2DPoint(i, j);
                        tmpWalkable[i, j] = 1; //mark to 1 for next paint
                        m_byIslandData[pt] = (byte)(m_byIslandNumber + 1);
                        tmpWalkabledata[pt] = GridMath.m_byWalkable;
                        ++iIslandWalkable;
                    }
                }
            }

            firstPoint = FindAWalkableGroundPoint(tmpWalkable);
            if (iIslandWalkable > 0)
            {
                ++m_byIslandNumber;    
            }
            CRuntimeLogger.Log(string.Format("Find island {1} with walkable:{0}", iIslandWalkable, m_byIslandNumber));
        }

        if (m_byIslandNumber > 250)
        {
            CRuntimeLogger.LogError("Island number toooo many! :" + m_byIslandNumber);
        }
    }

    /// <summary>
    /// both 1 and 2 are not walkable in walkabledata
    /// </summary>
    /// <param name="walkable">walkable data</param>
    /// <returns></returns>
    static private S2DPoint FindAWalkableGroundPoint(byte[,] walkable)
    {
        S2DPoint ret = S2DPoint.invalid;
        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                if (0 == walkable[i, j])
                {
                    return new S2DPoint(i, j);
                }
            }
        }
        return ret;
    }

    #endregion

    //Near grid data should based on island data, make sure to generate it first.
    #region Near Grid

    public short[] m_shNearGridData;

    private void GenerateNearGridData()
    {
        m_shNearGridData = new short[(m_byIslandNumber + 1) * SceneConstant.m_iSceneSizeSq];
        for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
        {
            for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
            {
                S2DPoint currentPt = new S2DPoint(i, j);
                if (GridMath.m_byWalkable == m_byGridStateData[currentPt])
                {
                    m_shNearGridData[currentPt] = currentPt;
                }
                else
                {
                    bool bFound = false;
                    for (short ii = 1; ii < SceneConstant.m_iSceneSize; ++ii)
                    {
                        int maxk = S2DPoint.GetRadiusNumber(ii);
                        for (int kk = 0; kk < maxk; ++kk)
                        {
                            S2DPoint pt = S2DPoint.GetRadiusPoint(currentPt, ii, kk);
                            if (pt.m_bValid && GridMath.m_byWalkable == m_byGridStateData[pt])
                            {
                                m_shNearGridData[currentPt] = pt;
                                bFound = true;
                                break;
                            }
                        }

                        if (bFound)
                        {
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        CRuntimeLogger.LogError("There is a strange point!");
                        return;
                    }
                }

                for (int k = 0; k < m_byIslandNumber; ++k)
                {
                    if (k + 1 == m_byIslandData[currentPt])
                    {
                        m_shNearGridData[(k + 1) * SceneConstant.m_iSceneSizeSq + currentPt] = currentPt;
                    }
                    else
                    {
                        bool bFound = false;
                        for (short ii = 1; ii < SceneConstant.m_iSceneSize; ++ii)
                        {
                            int maxk = S2DPoint.GetRadiusNumber(ii);
                            for (int kk = 0; kk < maxk; ++kk)
                            {
                                S2DPoint pt = S2DPoint.GetRadiusPoint(currentPt, ii, kk);
                                if (pt.m_bValid && k + 1 == m_byIslandData[pt])
                                {
                                    m_shNearGridData[(k + 1) * SceneConstant.m_iSceneSizeSq + currentPt] = pt;
                                    bFound = true;
                                    break;
                                }
                            }

                            if (bFound)
                            {
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            CRuntimeLogger.LogError("There is a strange point in island:" + (k + 1) + "currnetpt:" + currentPt.m_iX + "," + currentPt.m_iY);
                            return;
                        }
                    }                    
                }
            }
        }
    }

    /// <summary>
    /// 1 to 250 as island index, 0 for all
    /// </summary>
    /// <param name="islandChannel">1 to 250 as island index, 0 for all</param>
    /// <param name="point"></param>
    /// <returns></returns>
    private S2DPoint GetNearGrid(byte islandChannel, S2DPoint point)
    {
        if (0 == islandChannel)
        {
            return m_shNearGridData[point];
        }

        return m_shNearGridData[islandChannel * SceneConstant.m_iSceneSizeSq + point];
    }

    #endregion

    public void GenerateOptimaztion()
    {
        GenerateCleanRectangle();
        GenerateIsland();
        GenerateNearGridData();
    }

    #endregion

    #region AStar



#if DebugAStar
    /// <summary>
    /// This function is only for performance test
    /// </summary>
    /// <param name="vStart"></param>
    /// <param name="vTarget"></param>
    /// <param name="iStep"></param>
    /// <param name="eRes"></param>
    /// <returns></returns>
    public Stack<Vector2> FindPathAStarOld(Vector2 vStart, Vector2 vTarget, int iStep, out EPathRes eRes)
    {
        ++m_iFindingIndex;

        S2DPoint start = vStart;

        S2DPoint realStart = GetNearGrid(0, start.ToValid());
        byte island = m_byIslandData[realStart];
        S2DPoint end = vTarget;
        S2DPoint realEnd = GetNearGrid(island, end);

        if (realStart == realEnd)
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        //check clean rectangle
        COneGrid pStartNode = m_pGrids[realStart.m_iX, realStart.m_iY];
        COneGrid pEndNode = m_pGrids[realEnd.m_iX, realEnd.m_iY];
        if (0 != (pStartNode.m_usCleanRect & pEndNode.m_usCleanRect))
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        pStartNode.m_iOpenList = m_iFindingIndex;
        pStartNode.m_pParentNode = null;

        pStartNode.CalculateWeight(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex);

        pEndNode.m_pParentNode = null;

        pStartNode.m_pPrev = null;
        pStartNode.m_pNext = null;

        int iProtect = m_iAstarProtect;
        float fLowestH = -1.0f;
        COneGrid pLowestH = null;

        COneGrid pLowestF = pStartNode;
        while (null != pLowestF && iProtect > 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                CRuntimeLogger.LogError("No Way!");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }
            //find lowest f in open list
            //lowest f is always the first one
            //Add to close list
#if DebugAStar
            //Not possible
            if (m_iFindingIndex == pLowestF.m_iCloseList)
            {
                pLowestF = pLowestF.m_pNext;
                if (null == pLowestF)
                {
                    return;
                }
                continue;
            }

            if (island != m_byIslandData[pLowestF.m_sPos])
            {
                eRes = EPathRes.EPR_NoPath;
                CRuntimeLogger.LogError(string.Format("something wrong start island:{0}, lowestF island:{1}", island, m_byIslandData[pLowestF.m_sPos]));
                return null;
            }

            //not possbile
            if (m_iFindingIndex != pLowestF.m_iHCalculated)
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                pLowestF.CalculateWeight(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex);
            }
#endif

            pLowestF.m_iCloseList = m_iFindingIndex;
            if (pLowestF == pEndNode)
            {
                //found
                break;
            }

            if (0 != (pLowestF.m_usCleanRect & pEndNode.m_usCleanRect))
            {
                pEndNode.m_pParentNode = pLowestF;
                break;
            }

            if (pLowestF.m_fHValue < fLowestH || fLowestH < 0.0f)
            {
                fLowestH = pLowestF.m_fHValue;
                pLowestH = pLowestF;
            }

            #region Connect Points

            //check connected points
            for (int i = 0; i < pLowestF.m_pAllConnected.Length; ++i)
            {
                if (island == m_byIslandData[pLowestF.m_pAllConnected[i].m_sPos])
                {
                    COneGrid pConnectedNode = pLowestF.m_pAllConnected[i];
                    if (m_iFindingIndex != pConnectedNode.m_iCloseList)
                    {//Ignore cloes list
                        if (m_iFindingIndex != pConnectedNode.m_iOpenList)
                        {//Add to open list
                            pConnectedNode.m_pParentNode = pLowestF;
                            pConnectedNode.m_iParentNode = pLowestF.m_iInverseIndex[i];
                            pConnectedNode.m_iOpenList = m_iFindingIndex;

                            pConnectedNode.CalculateWeight(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex);

                            COneGrid pLast = pLowestF;
                            for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                            {
                                pLast = node;
                                if (node.m_fFValue > pConnectedNode.m_fFValue)
                                {
                                    node.m_pPrev.m_pNext = pConnectedNode;
                                    pConnectedNode.m_pPrev = node.m_pPrev;
                                    pConnectedNode.m_pNext = node;
                                    node.m_pPrev = pConnectedNode;
                                    pLast = null;
                                    break;
                                }
                            }
                            if (null != pLast)
                            {
                                pLast.m_pNext = pConnectedNode;
                                pConnectedNode.m_pPrev = pLast;
                                pConnectedNode.m_pNext = null;
                            }
                        }
                        else
                        {
                            //Check G Value
                            if (pConnectedNode.GViaMe(pLowestF, pLowestF.m_iInverseIndex[i]))
                            {
                                if (pConnectedNode.m_fFValue < pConnectedNode.m_pPrev.m_fFValue)
                                {
                                    if (null != pConnectedNode.m_pNext)
                                    {
                                        pConnectedNode.m_pNext.m_pPrev = pConnectedNode.m_pPrev;
                                    }
                                    pConnectedNode.m_pPrev.m_pNext = pConnectedNode.m_pNext;
                                    //must not be the last one
                                    for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                                    {
                                        if (node.m_fFValue > pConnectedNode.m_fFValue)
                                        {
                                            node.m_pPrev.m_pNext = pConnectedNode;
                                            pConnectedNode.m_pPrev = node.m_pPrev;
                                            pConnectedNode.m_pNext = node;
                                            node.m_pPrev = pConnectedNode;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            pLowestF = pLowestF.m_pNext;
        }

        if (null != pEndNode.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            COneGrid pLastParent = pEndNode;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = EPathRes.EPR_Done;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);

            return _tmpRes;
        }

        if (null != pLowestH && null != pLowestH.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(pLowestH.m_v2Pos);
            COneGrid pLastParent = pLowestH;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = (
                Mathf.Abs(pLowestH.m_sPos.m_iX - realEnd.m_iX) +
                Mathf.Abs(pLowestH.m_sPos.m_iY - realEnd.m_iY)
                ) >
                (
                Mathf.Abs(realStart.m_iX - realEnd.m_iX) +
                Mathf.Abs(realStart.m_iY - realEnd.m_iY)
                )
             ? EPathRes.EPR_Incomplete : EPathRes.EPR_IncompleteBad;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        if (null != pLowestH && pLowestH == pStartNode)
        {
            eRes = EPathRes.EPR_NoPath;
            return null;
        }

        eRes = EPathRes.EPR_NoPath;
        return null;
    }


    /// <summary>
    /// This is AStar with swamp
    /// </summary>
    /// <param name="vStart"></param>
    /// <param name="vTarget"></param>
    /// <param name="iStep"></param>
    /// <param name="eRes"></param>
    /// <returns></returns>
    public Stack<Vector2> FindPathAStar(Vector2 vStart, Vector2 vTarget, int iStep, out EPathRes eRes)
    {
        ++m_iFindingIndex;

        S2DPoint start = vStart;

        //TODO Occupy check
        S2DPoint realStart = GetNearGrid(0, start.ToValid());
        byte island = m_byIslandData[realStart];
        S2DPoint end = vTarget;
        S2DPoint realEnd = GetNearGrid(island, end);

        if (realStart == realEnd)
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        //check clean rectangle
        COneGrid pStartNode = m_pGrids[realStart.m_iX, realStart.m_iY];
        COneGrid pEndNode = m_pGrids[realEnd.m_iX, realEnd.m_iY];
        if (0 != (pStartNode.m_usCleanRect & pEndNode.m_usCleanRect))
        {
            //TODO occupycheck
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        byte uiThisSwampStart = (pStartNode.m_bySwampTag);
        byte uiThisSwampEnd = (pEndNode.m_bySwampTag); //include the swamp of start and end

        pStartNode.m_iOpenList = m_iFindingIndex;
        pStartNode.m_pParentNode = null;
        pStartNode.CalculateWeightSwamp(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);    

        pEndNode.m_pParentNode = null;
        pStartNode.m_pPrev = null;
        pStartNode.m_pNext = null;
        COneGrid pLowestF = pStartNode;
        int iProtect = m_iAstarProtect;
        float fLowestH = -1.0f;
        COneGrid pLowestH = null;

        //TODO: use step
        while (null != pLowestF && iProtect > 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                CRuntimeLogger.LogError("No Way!");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }
            //find lowest f in open list
            //lowest f is always the first one
            //Add to close list
#if DebugAStar
            //Not possible
            if (m_iFindingIndex == pLowestF.m_iCloseList)
            {
                pLowestF = pLowestF.m_pNext;
                if (null == pLowestF)
                {
                    return;
                }
                continue;
            }

            if (island != m_byIslandData[pLowestF.m_sPos])
            {
                eRes = EPathRes.EPR_NoPath;
                CRuntimeLogger.LogError(string.Format("something wrong start island:{0}, lowestF island:{1}", island, m_byIslandData[pLowestF.m_sPos]));
                return null;
            }

            //not possbile
            if (m_iFindingIndex != pLowestF.m_iHCalculated)
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                pLowestF.CalculateWeight(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex);
            }
#endif

            pLowestF.m_iCloseList = m_iFindingIndex;
            if (pLowestF == pEndNode)
            {
                //found
                break;
            }

            if (0 != (pLowestF.m_usCleanRect & pEndNode.m_usCleanRect))
            {
                //TODO occupycheck
                pEndNode.m_pParentNode = pLowestF;
                break;
            }

            if (pLowestF.m_fHValue < fLowestH || fLowestH < 0.0f)
            {
                fLowestH = pLowestF.m_fHValue;
                pLowestH = pLowestF;
            }

            #region Connect Points

            //check connected points
            for (int i = 0; i < pLowestF.m_pAllConnected.Length; ++i)
            {
                //TODO Occupy check
                if (island == m_byIslandData[pLowestF.m_pAllConnected[i].m_sPos])
                {
                    COneGrid pConnectedNode = pLowestF.m_pAllConnected[i];
                    if (m_iFindingIndex != pConnectedNode.m_iCloseList)
                    {//Ignore cloes list
                        if (m_iFindingIndex != pConnectedNode.m_iOpenList)
                        {//Add to open list
                            pConnectedNode.m_pParentNode = pLowestF;
                            pConnectedNode.m_iParentNode = pLowestF.m_iInverseIndex[i];
                            pConnectedNode.m_iOpenList = m_iFindingIndex;
                            pConnectedNode.CalculateWeightSwamp(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);

                            COneGrid pLast = pLowestF;
                            for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                            {
                                pLast = node;
                                if (node.m_fFValue > pConnectedNode.m_fFValue)
                                {
                                    node.m_pPrev.m_pNext = pConnectedNode;
                                    pConnectedNode.m_pPrev = node.m_pPrev;
                                    pConnectedNode.m_pNext = node;
                                    node.m_pPrev = pConnectedNode;
                                    pLast = null;
                                    break;
                                }
                            }
                            if (null != pLast)
                            {
                                pLast.m_pNext = pConnectedNode;
                                pConnectedNode.m_pPrev = pLast;
                                pConnectedNode.m_pNext = null;
                            }
                        }
                        else
                        {
                            //Check G Value
                            if (pConnectedNode.GViaMe(pLowestF, pLowestF.m_iInverseIndex[i]))
                            {
                                if (pConnectedNode.m_fFValue < pConnectedNode.m_pPrev.m_fFValue)
                                {
                                    if (null != pConnectedNode.m_pNext)
                                    {
                                        pConnectedNode.m_pNext.m_pPrev = pConnectedNode.m_pPrev;
                                    }
                                    pConnectedNode.m_pPrev.m_pNext = pConnectedNode.m_pNext;
                                    //must not be the last one
                                    for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                                    {
                                        if (node.m_fFValue > pConnectedNode.m_fFValue)
                                        {
                                            node.m_pPrev.m_pNext = pConnectedNode;
                                            pConnectedNode.m_pPrev = node.m_pPrev;
                                            pConnectedNode.m_pNext = node;
                                            node.m_pPrev = pConnectedNode;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region New Lowest F, Resort Order

            pLowestF = pLowestF.m_pNext;

            #endregion
        }

        if (null != pEndNode.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            COneGrid pLastParent = pEndNode;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);    
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = EPathRes.EPR_Done;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        if (null != pLowestH && null != pLowestH.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(pLowestH.m_v2Pos);
            COneGrid pLastParent = pLowestH;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = (
                Mathf.Abs(pLowestH.m_sPos.m_iX - realEnd.m_iX) +
                Mathf.Abs(pLowestH.m_sPos.m_iY - realEnd.m_iY)
                ) > 
                (
                Mathf.Abs(realStart.m_iX - realEnd.m_iX) + 
                Mathf.Abs(realStart.m_iY - realEnd.m_iY)
                )
             ? EPathRes.EPR_Incomplete : EPathRes.EPR_IncompleteBad;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        if (null != pLowestH && pLowestH == pStartNode)
        {
            eRes = EPathRes.EPR_NoPath;
            /*
            CRuntimeLogger.LogWarning("nop:" + (null == pStartNode ? "null" : string.Format("({0},{1})", pStartNode.m_sPos.m_iX, pStartNode.m_sPos.m_iY)) + " TO "
                + (null == pEndNode ? "null" : string.Format("({0},{1})", pEndNode.m_sPos.m_iX, pEndNode.m_sPos.m_iY))
                + " prot" + iProtect.ToString(CultureInfo.InvariantCulture) + "openlist:" + iOpenListStart.ToString(CultureInfo.InvariantCulture) + "loweset h:"
                + (null == pLowestH ? "null" : string.Format("({0},{1})", pLowestH.m_sPos.m_iX, pLowestH.m_sPos.m_iY)) + "lowest h  parent:"
                + (null == pLowestH.m_pParentNode ? "null" : string.Format("({0},{1})", pLowestH.m_pParentNode.m_sPos.m_iX, pLowestH.m_pParentNode.m_sPos.m_iY)));
             */
            return null;
        }

        eRes = EPathRes.EPR_NoPath;
        /*
        CRuntimeLogger.LogWarning("nop:" + (null == pStartNode ? "null" : string.Format("({0},{1})", pStartNode.m_sPos.m_iX, pStartNode.m_sPos.m_iY)) + " TO "
            + (null == pEndNode ? "null" : string.Format("({0},{1})", pEndNode.m_sPos.m_iX, pEndNode.m_sPos.m_iY))
            + " prot" + iProtect.ToString(CultureInfo.InvariantCulture) + "openlist:" + iOpenListStart.ToString(CultureInfo.InvariantCulture) + "loweset h:"
            + (null == pLowestH ? "null" : string.Format("({0},{1})", pLowestH.m_sPos.m_iX, pLowestH.m_sPos.m_iY)) + "lowest h  parent:"
            + (null == pLowestH.m_pParentNode ? "null" : string.Format("({0},{1})", pLowestH.m_pParentNode.m_sPos.m_iX, pLowestH.m_pParentNode.m_sPos.m_iY)));
        */
        return null;
    }
#endif

    #endregion

    #region JPS

    #region Bake Prun Rule

    public byte[] m_bNeighbourTable = null;
    public bool[] m_bForceTable = null;

    /// <summary>
    /// There is constant tables, too big to write as constant, so use this function to generate them and store them. Only call in Editor
    /// This function is in fact. fast enough to also call from a start up initial, but call in Editor is recommanded
    /// </summary>
    public void BakePruningRule()
    {
        m_bNeighbourTable = new byte[8 * 256];
        m_bForceTable = new bool[8 * 256];
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 256; ++j)
            {
                bool bHasForce;
                m_bNeighbourTable[i * 256 + j] = GetPruningRule(i, j, true, out bHasForce);
                m_bForceTable[i * 256 + j] = bHasForce;
                if (
                    (0 != (m_bNeighbourTable[i * 256 + j] & j)) 
                 || (0 != (m_bNeighbourTable[i * 256 + j] & (1 << ((i + 4)%8))))
                    )
                {
                    CRuntimeLogger.LogError("something wrong with table!");
                }
                if (0 == (j & (1 << ((i + 4) % 8))) && 0 != m_bNeighbourTable[i * 256 + j])
                {
                    CRuntimeLogger.LogError("something wrong with table!");
                }
                
                /*
                if (4 == i)
                {
                    bool bPisOccupied = (0 != (j & (1 << ((i + 4)%8))));
                    bool bForce = bHasForce;
                    string sN = bForce ? "F" : "N";
                    if (bPisOccupied)
                    {
                        CRuntimeLogger.Log(string.Format(
                            "{0} {1} {2}\n{7} O {3}\n{6} {5} {4}",
                            (0 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 0)) ? sN : (0 == (j & (1 << 0)) ? "O" : "X")),
                            (1 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 1)) ? sN : (0 == (j & (1 << 1)) ? "O" : "X")),
                            (2 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 2)) ? sN : (0 == (j & (1 << 2)) ? "O" : "X")),
                            (3 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 3)) ? sN : (0 == (j & (1 << 3)) ? "O" : "X")),
                            (4 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 4)) ? sN : (0 == (j & (1 << 4)) ? "O" : "X")),
                            (5 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 5)) ? sN : (0 == (j & (1 << 5)) ? "O" : "X")),
                            (6 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 6)) ? sN : (0 == (j & (1 << 6)) ? "O" : "X")),
                            (7 == (i + 4) % 8) ? "P" : (0 != (m_bNeighbourTable[i * 256 + j] & (1 << 7)) ? sN : (0 == (j & (1 << 7)) ? "O" : "X"))
                            ));                        
                    }
                }
                */
            }            
        }
    }

    /// <summary>
    /// Only use for BakePruningRule. Only call in Editor
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="occupy"></param>
    /// <param name="bAllowCornerMove"></param>
    /// <param name="bHasForce"></param>
    /// <returns></returns>
    private byte GetPruningRule(int dir, int occupy, bool bAllowCornerMove, out bool bHasForce)
    {
        byte ret = 0;
        bHasForce = false;

        int iInvertDir = (dir + 4) % 8;
        if (0 == ((1 << iInvertDir) & occupy)) //We require the parent node as occupied
        {

            return ret;
        }

        //clock order
        bool[,] occupytable = 
        {
            {0 != ((1 << 0) & occupy), 0 != ((1 << 1) & occupy), 0 != ((1 << 2) & occupy)},
            {0 != ((1 << 7) & occupy), false,                    0 != ((1 << 3) & occupy)},
            {0 != ((1 << 6) & occupy), 0 != ((1 << 5) & occupy), 0 != ((1 << 4) & occupy)},
        };

        int iParentX = S2DPoint.dirs[iInvertDir].m_iX + 1;
        int iParentY = S2DPoint.dirs[iInvertDir].m_iY + 1;

        //now we have occupytable, we have parent node position
        for (int i = 0; i < 8; ++i)
        {
            if (0 == ((1 << i) & occupy))
            {
                bool[] lens = GetLen(iParentX, iParentY, S2DPoint.dirs[i].m_iX + 1, S2DPoint.dirs[i].m_iY + 1, dir, occupytable);

                if (0 == (dir & 1)) //diagonal
                {
                    if (lens[0])
                    {
                        ret |= (byte) (1 << i);
                    }
                    if (lens[1] && bAllowCornerMove)
                    {
                        bHasForce = true;
                    }
                }
                else
                {
                    if (lens[0])
                    {
                        ret |= (byte)(1 << i);
                    }
                    if (lens[1])
                    {
                        bHasForce = true;
                    }
                }
            }
        }
        return ret;
    }

    /// <summary>
    /// this is len(p->n). Only use for BakePruningRule. Only call in Editor
    /// </summary>
    /// <param name="iParentX"></param>
    /// <param name="iParentY"></param>
    /// <param name="iTargetX"></param>
    /// <param name="iTargetY"></param>
    /// <param name="idir"></param>
    /// <param name="occupyTable"></param>
    /// <returns>return(is neighbour, is forced neighbour)</returns>
    private bool[] GetLen(int iParentX, int iParentY, int iTargetX, int iTargetY, int idir, bool[,] occupyTable)
    {
        if (Mathf.Abs(iTargetX - iParentX) <= 1 && Mathf.Abs(iTargetY - iParentY) <= 1)
        {
            //the point is next to p
            return new [] { false, false };
        }

        //first step, we rotate the table, so dir is always 0 or 1
        bool[,] table =
        {
            {occupyTable[0, 0], occupyTable[0, 1], occupyTable[0, 2]},
            {occupyTable[1, 0], occupyTable[1, 1], occupyTable[1, 2]},
            {occupyTable[2, 0], occupyTable[2, 1], occupyTable[2, 2]}
        };
        int iOldX;
        switch (idir)
        {
            case 2:
                /*
                 * 0 0 0        0 0 0
                 * 0 0 0    ->  0 0 0
                 * 1 0 0        0 0 1
                 */
                iParentX = 2 - iParentX;
                iTargetX = 2 - iTargetX;
                table = _vfm(table);
                break;
            case 3:
                /*
                 * 0 0 0        0 0 0
                 * 1 0 0    ->  0 0 0
                 * 0 0 0        0 1 0
                 */
            case 4:
                /*
                 * 1 0 0        0 0 0
                 * 0 0 0    ->  0 0 0
                 * 0 0 0        0 0 1
                 */
                iOldX = iParentX;
                iParentX = 2 - iParentY;
                iParentY = 2 - iOldX;
                iOldX = iTargetX;
                iTargetX = 2 - iTargetY;
                iTargetY = 2 - iOldX;

                table = _stm(table);
                break;
            case 5:
                /*
                 * 0 1 0        0 0 0
                 * 0 0 0    ->  0 0 0
                 * 0 0 0        0 1 0
                 */
            case 6:
                /*
                 * 0 0 1        0 0 0
                 * 0 0 0    ->  0 0 0
                 * 0 0 0        0 0 1
                 */
                iParentY = 2 - iParentY;
                iTargetY = 2 - iTargetY;
                table = _hfm(table);
                break;
            case 7:
                /*
                 * 0 0 0        0 0 0
                 * 0 0 1    ->  0 0 0
                 * 0 0 0        0 1 0
                 */
                iOldX = iParentX;
                iParentX = iParentY;
                iParentY = iOldX;
                iOldX = iTargetX;
                iTargetX = iTargetY;
                iTargetY = iOldX;
                table = _tm(table);
                break;
        }

        if (0 == (idir & 1)) //diagonal
        {
            if (0 == iTargetX && 0 == iTargetY)
            {
                /*
                 * n 0 0
                 * 0 X 0
                 * 0 0 P
                 */
                return new[] { true, false }; 
            }
            if (1 == iTargetX && 0 == iTargetY)
            {
                /*
                 * 0 n 0
                 * 0 X 0
                 * 0 0 P
                 */
                return new[] { true, false }; 
            }
            if (2 == iTargetX && 0 == iTargetY)
            {
                /*
                 * 0 0 n
                 * 0 X 0
                 * 0 0 P
                 */
                if (table[1, 2])
                {
                    return new[] { true, true }; 
                }
                return new[] { false, false }; 
            }
            if (0 == iTargetX && 1 == iTargetY)
            {
                /*
                 * 0 0 0
                 * n X 0
                 * 0 0 P
                 */
                return new[] { true, false }; 
            }
            if (0 == iTargetX && 2 == iTargetY)
            {
                /*
                 * 0 0 0
                 * 0 X 0
                 * n 0 P
                 */
                if (table[2, 1])
                {
                    return new[] { true, true };
                }
                return new[] { false, false }; 
            }
        }
        else
        {
            //all is
            /*
             * n n n
             * 0 X 0
             * 0 P 0
             */
            if (0 == iTargetX && 0 == iTargetY)
            {
                /*
                 * n 0 0
                 * 0 X 0
                 * 0 P 0
                 */
                if (table[1, 0])
                {
                    return new[] { true, true };
                }
                return new[] { false, false }; 
            }
            if (1 == iTargetX && 0 == iTargetY)
            {
                /*
                 * 0 n 0
                 * 0 X 0
                 * 0 P 0
                 */
                return new[] { true, false }; 
            }
            if (2 == iTargetX && 0 == iTargetY)
            {
                /*
                 * 0 0 n
                 * 0 X 0
                 * 0 P 0
                 */
                if (table[1, 2])
                {
                    return new[] { true, true };
                }
                return new[] { false, false }; 
            }
        }

        CRuntimeLogger.LogError(string.Format("There is condition not considered!:tx:{0},ty:{1},px:{2},py:{3},dir:{4}",
            iTargetX,iTargetY,iParentX,iParentY,idir));
        return new[] { false, false };
    }

    /// <summary>
    /// transverse matrix, Only use for BakePruningRule. Only call in Editor
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private bool[,] _tm(bool[,] m)
    {
        return new[,]        
        {
            {m[0, 0], m[1, 0], m[2, 0]},
            {m[0, 1], m[1, 1], m[2, 1]},
            {m[0, 2], m[1, 2], m[2, 2]}
        };
    }

    /// <summary>
    /// flip matrix by left-button/top right axis, Only use for BakePruningRule. Only call in Editor
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private bool[,] _stm(bool[,] m)
    {
        return new[,]        
        {
            {m[2, 2], m[1, 2], m[0, 2]},
            {m[2, 1], m[1, 1], m[0, 1]},
            {m[2, 0], m[1, 0], m[0, 0]}
        };
    }

    /// <summary>
    /// horizen flip matrix, Only use for BakePruningRule. Only call in Editor
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private bool[,] _hfm(bool[,] m)
    {
        return new[,]        
        {
            {m[2, 0], m[2, 1], m[2, 2]},
            {m[1, 0], m[1, 1], m[1, 2]},
            {m[0, 0], m[0, 1], m[0, 2]}
        };
    }

    /// <summary>
    /// vertical flip matrix, Only use for BakePruningRule. Only call in Editor
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    private bool[,] _vfm(bool[,] m)
    {
        return new[,]        
        {
            {m[0, 2], m[0, 1], m[0, 0]},
            {m[1, 2], m[1, 1], m[1, 0]},
            {m[2, 2], m[2, 1], m[2, 0]}
        };
    }

    #endregion

    #region Blockbased Prun Rule

    private const ulong _largest = 1Lu << 63;
    /// <summary>
    /// Return the first bit 1 of number start from start
    /// E for edge, simpliy consider as count the number of "0" + 1
    /// for ... 0 0 0 0 s 0 0 1 0 0 0 E, will return 3
    /// for ... 0 0 0 0 s 1 0 0 0 0 0 E, will return 1
    /// for ... 0 0 0 0 s 0 0 0 0 0 0 E, will return 7
    /// </summary>
    /// <param name="number"></param>
    /// <param name="iStart"></param>
    /// <returns></returns>
    private int _ffs(ulong number, int iStart)
    {
        ulong check = number << iStart;
        for (int i = 0; i < 63 - iStart; ++i) //not include start
        {
            check = check << 1;
            if (check >= _largest)
            {
                return i + 1;
            }
        }

        return 64 - iStart;
    }

    private S2DPoint BlockedJump(S2DPoint x, int dir, S2DPoint target)
    {
        //CRuntimeLogger.Log("in BlockedJump: start:" + x + ",dir:" + dir + ":" + S2DPoint.dirs[dir]);

        if (0 != (dir & 1))
        {
            return BlockedJumpVertical(true, true, x, dir, target);
        }

        //Tell me! why is stack call faster then while?
        S2DPoint n = new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX, x.m_iY + S2DPoint.dirs[dir].m_iY);
        //while (true)
        //{
            if (!n.m_bValid || GridMath.m_byWalkable != m_byGridStateData[n])
            {
                return S2DPoint.invalid;
            }
            if (n == target)
            {
                return n;
            }

            /*
        int iInvertDir = (dir + 4) % 8;
        int X = n.m_iX, Y = n.m_iY;
        byte tag = (byte)(
                (
                iInvertDir != 0 && (
                    ((X - 1 >= 0) && (Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 0)
                )
                | (
                iInvertDir != 1 && (
                    ((Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[X * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 1)
                )
                | (
                iInvertDir != 2 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) && (Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 2)
                )
                | (
                iInvertDir != 3 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y])
                    ) ? 0 : (1 << 3)
                )
                | (
                iInvertDir != 4 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 4)
                )
                | (
                iInvertDir != 5 && (
                    ((Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 5)
                )
                | (
                iInvertDir != 6 && (
                    ((X - 1 >= 0) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 6)
                )
                | (
                iInvertDir != 7 && (
                    ((X - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y])
                    ) ? 0 : (1 << 7)
                )
            );

        if (m_bForceTable[dir * 256 + tag])
        {
            //CRuntimeLogger.Log("in diagonal force table");
            return n;
        }
        */

            if (0 != (m_pGrids[n.m_iX, n.m_iY].m_byCachedForceTable & (byte) (1 << dir)))
            {
                return n;
            }

            //middle diagonal point
            S2DPoint ret2 = BlockedJumpVertical(false, true, n, dir + 1, target);
            if (ret2.m_bValid)
            {
                return n;
            }

            S2DPoint ret3 = BlockedJumpVertical(false, true, n, 0 == dir ? 7 : dir - 1, target);
            if (ret3.m_bValid)
            {
                return n;
            }
            //n = new S2DPoint(n.m_iX + S2DPoint.dirs[dir].m_iX, n.m_iY + S2DPoint.dirs[dir].m_iY);
        //}
        return BlockedJump(n, dir, target);
    }

    /// <summary>
    /// return invalid indicates a dead end
    /// </summary>
    /// <param name="bDiagonalMove"></param>
    /// <param name="x"></param>
    /// <param name="dir"></param>
    /// <param name="target"></param>
    /// <param name="bAsStart"></param>
    /// <returns></returns>
    private S2DPoint BlockedJumpVertical(bool bAsStart, bool bDiagonalMove, S2DPoint x, int dir, S2DPoint target)
    {
        int iStartPoint = 0;
        ulong walkable = 0Lu;
        ulong walkableUp = 0Lu;
        ulong walkableDown = 0Lu;
        int iTargetOffset = -1;

        switch (dir)
        {
            case 7: //right to left
                if (target.m_iY == x.m_iY)
                {
                    iTargetOffset = x.m_iX - target.m_iX;
                }
                iStartPoint = SceneConstant.m_iSceneSize - 1 - x.m_iX;
                walkable = m_ulWalkable180[x.m_iY + 1];
                walkableUp = m_ulWalkable180[x.m_iY + 2];
                walkableDown = m_ulWalkable180[x.m_iY];
                break;
            case 3: //left to right
                if (target.m_iY == x.m_iY)
                {
                    iTargetOffset = target.m_iX - x.m_iX;
                }
                iStartPoint = x.m_iX;
                walkable = m_ulWalkable0[x.m_iY + 1];
                walkableUp = m_ulWalkable0[x.m_iY + 2];
                walkableDown = m_ulWalkable0[x.m_iY];
                break;
            case 5: //up to down
                if (target.m_iX == x.m_iX)
                {
                    iTargetOffset = target.m_iY - x.m_iY;
                }
                iStartPoint = x.m_iY;
                walkable = m_ulWalkable90[x.m_iX + 1];
                walkableUp = m_ulWalkable90[x.m_iX + 2];
                walkableDown = m_ulWalkable90[x.m_iX];
                break;
            case 1: //down to up
                if (target.m_iY <= x.m_iY && target.m_iX == x.m_iX)
                {
                    iTargetOffset = x.m_iY - target.m_iY;
                }
                iStartPoint = SceneConstant.m_iSceneSize - 1 - x.m_iY;
                walkable = m_ulWalkable270[x.m_iX + 1];
                walkableUp = m_ulWalkable270[x.m_iX + 2];
                walkableDown = m_ulWalkable270[x.m_iX];
                break;
            default:
                CRuntimeLogger.LogError("Not possible here!");
                break;
        }

        ulong Bs = ((walkableUp >> 1) & (~walkableUp))
                 | ((walkableDown >> 1) & (~walkableDown));
                 //| walkable;
        int bn = _ffs(walkable, iStartPoint);

        int bs;
        if (bAsStart) //if from start, myself is a force jump point, will not move!
        {
            bs = _ffs(Bs, iStartPoint + 1) + 1;
        }
        else
        {
            bs = _ffs(Bs, iStartPoint);
        }
        
        /*
        CRuntimeLogger.Log("start:" + x + " dir:" + dir + ":" + S2DPoint.dirs[dir]);
        CRuntimeLogger.Log(string.Format("walkable:{0}", Convert.ToString((long)walkable, 2)));
        CRuntimeLogger.Log(string.Format("walkable:{0}", Convert.ToString((long)walkableUp, 2)));
        CRuntimeLogger.Log(string.Format("walkable:{0}", Convert.ToString((long)walkableDown, 2)));
        CRuntimeLogger.Log(string.Format("Bs:{0}", Convert.ToString((long)Bs, 2)));
        CRuntimeLogger.Log("bs, bn:" + bs + "," + bn);
        */

        if (iTargetOffset >= 0 && iTargetOffset < bn)
        {
            return target;
        }

        //if "bAsStart", we in fact have jumped one step, so bs could be 0
        //bs <= bn because
        //  O O O X X
        //->S O P X X
        //  X X X N O
        //  P is also a force jump point because it has a forced neigbour "N"
        if (bs > 1 && bDiagonalMove && bs <= bn)
        {
            //forced
            return new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX * (bs - 1), x.m_iY + S2DPoint.dirs[dir].m_iY * (bs - 1));
        }
        if (bs > 1 && !bDiagonalMove && bs < bn)
        {
            //forced
            return new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX * (bs - 1), x.m_iY + S2DPoint.dirs[dir].m_iY * (bs - 1));
        }  

#if DebugJPSOpenlist
        _debugJpsDeadList[
            new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX*(bs - 1), x.m_iY + S2DPoint.dirs[dir].m_iY*(bs - 1))] = x;
#endif

        //dead end
        return S2DPoint.invalid;

        //S2DPoint ret = new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX, x.m_iY * bn + S2DPoint.dirs[dir].m_iY * bn);

        //CRuntimeLogger.Log(ret);
        //CRuntimeLogger.Log(m_byGridStateData[ret]);
        //return ret;
    }

    #endregion

    #region Cache force table and neighbor table

    private void CacheForceTableAndNeighborTable()
    {
        for (int i = 0; i < SceneConstant.m_iSceneSizeSq; ++i)
        {
            RecalculateNode(i);
        }
    }

    private void RecalculateNode(S2DPoint pt)
    {
        byte byForce = 0;
        byte[] neib = new byte[8];
        for (int dir = 0; dir < 8; ++dir)
        {
            int iInvertDir = (dir + 4) % 8;
            int X = pt.m_iX, Y = pt.m_iY;
            byte b = (byte)(
                    (
                    iInvertDir != 0 && (
                        ((X - 1 >= 0) && (Y - 1 >= 0) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y - 1])
                        ) ? 0 : (1 << 0)
                    )
                    | (
                    iInvertDir != 1 && (
                        ((Y - 1 >= 0) &&
                         GridMath.m_byWalkable == m_byGridStateData[X * SceneConstant.m_iSceneSize + Y - 1])
                        ) ? 0 : (1 << 1)
                    )
                    | (
                    iInvertDir != 2 && (
                        ((X + 1 < SceneConstant.m_iSceneSize) && (Y - 1 >= 0) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y - 1])
                        ) ? 0 : (1 << 2)
                    )
                    | (
                    iInvertDir != 3 && (
                        ((X + 1 < SceneConstant.m_iSceneSize) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y])
                        ) ? 0 : (1 << 3)
                    )
                    | (
                    iInvertDir != 4 && (
                        ((X + 1 < SceneConstant.m_iSceneSize) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y + 1])
                        ) ? 0 : (1 << 4)
                    )
                    | (
                    iInvertDir != 5 && (
                        ((Y + 1 < SceneConstant.m_iSceneSize) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X) * SceneConstant.m_iSceneSize + Y + 1])
                        ) ? 0 : (1 << 5)
                    )
                    | (
                    iInvertDir != 6 && (
                        ((X - 1 >= 0) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y + 1])
                        ) ? 0 : (1 << 6)
                    )
                    | (
                    iInvertDir != 7 && (
                        ((X - 1 >= 0) &&
                         GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y])
                        ) ? 0 : (1 << 7)
                    )
                );

            byForce |= (m_bForceTable[dir*256 + b]) ? (byte)(1 << dir) : (byte)0;
            neib[dir] = m_bNeighbourTable[dir * 256 + b];
        }

        m_pGrids[pt.m_iX, pt.m_iY].m_byCachedForceTable = byForce;
        m_pGrids[pt.m_iX, pt.m_iY].m_bCachedNeigTable = neib;
    }

    #endregion

#if DebugJPS
    private S2DPoint Jump(S2DPoint x, int dir, S2DPoint target)
    {
        S2DPoint n = new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX, x.m_iY + S2DPoint.dirs[dir].m_iY);
        //CRuntimeLogger.Log(string.Format("step dir: {0},  ({1},{2})", dir, n.m_iX, n.m_iY));
        if (!n.m_bValid || GridMath.m_byWalkable != m_byGridStateData[n])
        {
            return S2DPoint.invalid;
        }
        if (n == target)
        {
            return n;
        }

        /*
        int iInvertDir = (dir + 4) % 8;
        int X = n.m_iX, Y = n.m_iY;
        byte tag = (byte)(
                (
                iInvertDir != 0 && (
                    ((X - 1 >= 0) && (Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 0)
                )
                | (
                iInvertDir != 1 && (
                    ((Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[X * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 1)
                )
                | (
                iInvertDir != 2 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) && (Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 2)
                )
                | (
                iInvertDir != 3 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y])
                    ) ? 0 : (1 << 3)
                )
                | (
                iInvertDir != 4 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 4)
                )
                | (
                iInvertDir != 5 && (
                    ((Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 5)
                )
                | (
                iInvertDir != 6 && (
                    ((X - 1 >= 0) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 6)
                )
                | (
                iInvertDir != 7 && (
                    ((X - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y])
                    ) ? 0 : (1 << 7)
                )
            );

        if (m_bForceTable[dir * 256 + tag])
        {
            //CRuntimeLogger.Log(dir);
            //CRuntimeLogger.Log(tag);
            //CRuntimeLogger.Log(n.m_iX.ToString() + n.m_iY);
            return n;
        }
        */
        if (0 != (m_pGrids[n.m_iX, n.m_iY].m_byCachedForceTable & (byte)(1 << dir)))
        {
            //CRuntimeLogger.Log(dir);
            //CRuntimeLogger.Log(tag);
            //CRuntimeLogger.Log(n.m_iX.ToString() + n.m_iY);
            return n;
        }

        if (0 == (dir & 1)) //diagonal
        {
            if (Jump(n, dir + 1, target).m_bValid)
            {
                return n;
            }
            if (Jump(n, 0 == dir ? 7 : dir - 1, target).m_bValid)
            {
                return n;
            }
        }
        return Jump(n, dir, target);
    }
#endif

    private readonly COneGrid[] _tmpSuccessor = new COneGrid[8];

#if DebugJPS
    /// <summary>
    /// Onlt for performance test
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="dir"></param>
    private void SuccessorsOld(S2DPoint current, S2DPoint target, int dir)
    {
        if (-1 == dir)
        {
            for (int i = 0; i < 8; ++i)
            {
                int iX = current.m_iX + S2DPoint.dirs[i].m_iX;
                int iY = current.m_iY + S2DPoint.dirs[i].m_iY;
                if (iX >= 0 && iX < SceneConstant.m_iSceneSize
                 && iY >= 0 && iY < SceneConstant.m_iSceneSize
                 && GridMath.m_byWalkable == m_byGridStateData[iX * SceneConstant.m_iSceneSize + iY])
                {
                    S2DPoint jmpres = Jump(current, i, target);
                    if (jmpres.m_bValid)
                    {
                        _tmpSuccessor[i] = m_pGrids[jmpres.m_iX, jmpres.m_iY];
                    }
                }
            }
            return;
        }

        byte neighbour = m_pGrids[current.m_iX, current.m_iY].m_bCachedNeigTable[dir];
        for (int i = 0; i < 8; ++i)
        {
            if (0 != ((1 << i) & neighbour))
            {
                S2DPoint jmpres = Jump(current, i, target);
                if (jmpres.m_bValid)
                {
                    _tmpSuccessor[i] = m_pGrids[jmpres.m_iX, jmpres.m_iY];
                }
            }
        }
    }
#endif

    private void Successors(S2DPoint current, S2DPoint target, int dir)
    {
        //CRuntimeLogger.Log("<color=#00FF00>in Successors</color>, current:" + current + " ,parent dir:" + ((-1 == dir) ? "none" : S2DPoint.dirs[dir].ToString()));
        if (-1 == dir)
        {
            for (int i = 0; i < 8; ++i)
            {
                int iX = current.m_iX + S2DPoint.dirs[i].m_iX;
                int iY = current.m_iY + S2DPoint.dirs[i].m_iY;
                //no parent dir, all walkable is neighbour
                if (iX >= 0 && iX < SceneConstant.m_iSceneSize
                 && iY >= 0 && iY < SceneConstant.m_iSceneSize
                 && GridMath.m_byWalkable == m_byGridStateData[iX * SceneConstant.m_iSceneSize + iY])
                {
                    S2DPoint jmpres = BlockedJump(current, i, target);
                    if (jmpres.m_bValid)
                    {
                        //CRuntimeLogger.Log("<color=#00FF00>add to _tmpSuccessor</color>" + jmpres);
                        _tmpSuccessor[i] = m_pGrids[jmpres.m_iX, jmpres.m_iY];
                    }
                }
            }
            return;
        }

        /*
        int iInvertDir = (dir + 4) % 8;
        int X = current.m_iX, Y = current.m_iY;
        byte tag = (byte)(
                (
                iInvertDir != 0 && (
                    ((X - 1 >= 0) && (Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 0)
                )
                | (
                iInvertDir != 1 && (
                    ((Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[X * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 1)
                )
                | (
                iInvertDir != 2 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) && (Y - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y - 1])
                    ) ? 0 : (1 << 2)
                )
                | (
                iInvertDir != 3 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y])
                    ) ? 0 : (1 << 3)
                )
                | (
                iInvertDir != 4 && (
                    ((X + 1 < SceneConstant.m_iSceneSize) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X + 1) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 4)
                )
                | (
                iInvertDir != 5 && (
                    ((Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 5)
                )
                | (
                iInvertDir != 6 && (
                    ((X - 1 >= 0) && (Y + 1 < SceneConstant.m_iSceneSize) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y + 1])
                    ) ? 0 : (1 << 6)
                )
                | (
                iInvertDir != 7 && (
                    ((X - 1 >= 0) &&
                     GridMath.m_byWalkable == m_byGridStateData[(X - 1) * SceneConstant.m_iSceneSize + Y])
                    ) ? 0 : (1 << 7)
                )
            );

        byte neighbour = m_bNeighbourTable[dir*256 + tag];
         */
        byte neighbour = m_pGrids[current.m_iX, current.m_iY].m_bCachedNeigTable[dir];
        for (int i = 0; i < 8; ++i)
        {
            if (0 != ((1 << i) & neighbour))
            {
                //Jump this neighbour
                S2DPoint jmpres = BlockedJump(current, i, target);
                if (jmpres.m_bValid)
                {
                    //CRuntimeLogger.Log("<color=#00FF00>add to _tmpSuccessor</color>" + jmpres);
                    _tmpSuccessor[i] = m_pGrids[jmpres.m_iX, jmpres.m_iY];
                }
            }
        }
    }

#if DebugJPS
    /// <summary>
    /// Only for performance test
    /// </summary>
    /// <param name="vStart"></param>
    /// <param name="vTarget"></param>
    /// <param name="iStep"></param>
    /// <param name="eRes"></param>
    /// <returns></returns>
    public Stack<Vector2> FindPathJPSOld(Vector2 vStart, Vector2 vTarget, int iStep, out EPathRes eRes)
    {
        ++m_iFindingIndex;

        S2DPoint start = vStart;
        S2DPoint realStart = GetNearGrid(0, start.ToValid());
        byte island = m_byIslandData[realStart];
        S2DPoint end = vTarget;
        S2DPoint realEnd = GetNearGrid(island, end.ToValid());

        if (realStart == realEnd)
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        //check clean rectangle
        COneGrid pStartNode = m_pGrids[realStart.m_iX, realStart.m_iY];
        COneGrid pEndNode = m_pGrids[realEnd.m_iX, realEnd.m_iY];
        if (0 != (pStartNode.m_usCleanRect & pEndNode.m_usCleanRect))
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        byte uiThisSwampStart = (pStartNode.m_bySwampTag);
        byte uiThisSwampEnd = (pEndNode.m_bySwampTag); //include the swamp of start and end

        pStartNode.m_iOpenList = m_iFindingIndex;
        pStartNode.m_pParentNode = null;
        pStartNode.m_iParentDir = -1;
        pStartNode.CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);
        pEndNode.m_pParentNode = null;
        pStartNode.m_pNext = null;
        pStartNode.m_pPrev = null;
        int iProtect = m_iAstarProtect;
        float fLowestH = -1.0f;
        COneGrid pLowestF = pStartNode;
        COneGrid pLowestH = null;

        while (null != pLowestF && iProtect > 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                CRuntimeLogger.LogError("Run out without a result");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }

#if DebugAStar //Not Possible to happen
            if (m_iFindingIndex == pLowestF.m_iCloseList)
            {
                pLowestF = pLowestF.m_pNext;
                if (null == pLowestF)
                {
                    break;
                }
                continue;
            }

            if (GridMath.m_byWalkable != m_byGridStateData[pLowestF.m_sPos])
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }

            if (m_iFindingIndex != pLowestF.m_iHCalculated)
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                pLowestF.CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);
            }
#endif

            //Add to close list
            pLowestF.m_iCloseList = m_iFindingIndex;
            if (pLowestF == pEndNode)
            {
                //target
                //CRuntimeLogger.Log("lowestF = EndNode");
                break;
            }

            if (0 != (pLowestF.m_usCleanRect & pEndNode.m_usCleanRect))
            {
                pEndNode.m_pParentNode = pLowestF;
                break;
            }

            if (pLowestF.m_fHValue < fLowestH || fLowestH < 0.0f)
            {
                fLowestH = pLowestF.m_fHValue;
                pLowestH = pLowestF;
            }

            //Open List is sorted

            #region Connect Points

            //check connected points
            SuccessorsOld(pLowestF.m_sPos, realEnd, pLowestF.m_iParentDir);
            for (int i = 0; i < 8; ++i)
            {
                if (null != _tmpSuccessor[i])
                {
                    if (m_iFindingIndex != _tmpSuccessor[i].m_iCloseList)
                    {
                        if (m_iFindingIndex != _tmpSuccessor[i].m_iOpenList)
                        {
                            //Add to open list
                            _tmpSuccessor[i].m_pParentNode = pLowestF;
                            _tmpSuccessor[i].m_iParentDir = i;
                            _tmpSuccessor[i].m_iOpenList = m_iFindingIndex;
                            _tmpSuccessor[i].CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);

                            COneGrid pLast = pLowestF;
                            for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                            {
                                pLast = node;
                                if (node.m_fFValue > _tmpSuccessor[i].m_fFValue)
                                {
                                    node.m_pPrev.m_pNext = _tmpSuccessor[i];
                                    _tmpSuccessor[i].m_pPrev = node.m_pPrev;
                                    _tmpSuccessor[i].m_pNext = node;
                                    node.m_pPrev = _tmpSuccessor[i];
                                    pLast = null;
                                    break;
                                }
                            }
                            if (null != pLast)
                            {
                                pLast.m_pNext = _tmpSuccessor[i];
                                _tmpSuccessor[i].m_pPrev = pLast;
                                _tmpSuccessor[i].m_pNext = null;
                            }
                        }
                        else
                        {
                            //Check G Value
                            if (_tmpSuccessor[i].GViaMeJPS(pLowestF, i))
                            {
                                if (_tmpSuccessor[i].m_fFValue < _tmpSuccessor[i].m_pPrev.m_fFValue)
                                {
                                    if (null != _tmpSuccessor[i].m_pNext)
                                    {
                                        _tmpSuccessor[i].m_pNext.m_pPrev = _tmpSuccessor[i].m_pPrev;
                                    }
                                    _tmpSuccessor[i].m_pPrev.m_pNext = _tmpSuccessor[i].m_pNext;
                                    //must not be the last one
                                    for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                                    {
                                        if (node.m_fFValue > _tmpSuccessor[i].m_fFValue)
                                        {
                                            node.m_pPrev.m_pNext = _tmpSuccessor[i];
                                            _tmpSuccessor[i].m_pPrev = node.m_pPrev;
                                            _tmpSuccessor[i].m_pNext = node;
                                            node.m_pPrev = _tmpSuccessor[i];
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _tmpSuccessor[i] = null;//important to clear the old values
                }
            }

            #endregion

            pLowestF = pLowestF.m_pNext;
        }

        if (null != pEndNode.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            COneGrid pLastParent = pEndNode;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = EPathRes.EPR_Done;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            //CRuntimeLogger.Log("JPS:" + m_pOpenList.Count);
            return _tmpRes;
        }

        if (null != pLowestH && null != pLowestH.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(pLowestH.m_v2Pos);
            COneGrid pLastParent = pLowestH;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = (
                Mathf.Abs(pLowestH.m_sPos.m_iX - realEnd.m_iX) +
                Mathf.Abs(pLowestH.m_sPos.m_iY - realEnd.m_iY)
                ) >
                (
                Mathf.Abs(realStart.m_iX - realEnd.m_iX) +
                Mathf.Abs(realStart.m_iY - realEnd.m_iY)
                )
             ? EPathRes.EPR_Incomplete : EPathRes.EPR_IncompleteBad;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        if (null != pLowestH && pLowestH == pStartNode)
        {
            CRuntimeLogger.LogWarning("Why? What happend?");
            eRes = EPathRes.EPR_NoPath;
            return null;
        }

        eRes = EPathRes.EPR_NoPath;
        /*CRuntimeLogger.LogWarning("nop:" + (null == pStartNode ? "null" : string.Format("({0},{1})", pStartNode.m_sPos.m_iX, pStartNode.m_sPos.m_iY)) + " TO "
            + (null == pEndNode ? "null" : string.Format("({0},{1})", pEndNode.m_sPos.m_iX, pEndNode.m_sPos.m_iY))
            + " prot" + iProtect.ToString(CultureInfo.InvariantCulture) + "openlist:" + iOpenListStart.ToString(CultureInfo.InvariantCulture) + "loweset h:"
            + (null == pLowestH ? "null" : string.Format("({0},{1})", pLowestH.m_sPos.m_iX, pLowestH.m_sPos.m_iY)) + "lowest h  parent:"
            + (null == pLowestH.m_pParentNode ? "null" : string.Format("({0},{1})", pLowestH.m_pParentNode.m_sPos.m_iX, pLowestH.m_pParentNode.m_sPos.m_iY)));*/
        return null;
    }
#endif

    public Stack<Vector2> FindPathJPS(Vector2 vStart, Vector2 vTarget, int iStep, out EPathRes eRes)
    {
        ++m_iFindingIndex;

        S2DPoint start = vStart;
        S2DPoint realStart = GetNearGrid(0, start.ToValid());
        byte island = m_byIslandData[realStart];
        S2DPoint end = vTarget;
        S2DPoint realEnd = GetNearGrid(island, end.ToValid());

        if (realStart == realEnd)
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        //check clean rectangle
        COneGrid pStartNode = m_pGrids[realStart.m_iX, realStart.m_iY];
        COneGrid pEndNode = m_pGrids[realEnd.m_iX, realEnd.m_iY];
        if (0 != (pStartNode.m_usCleanRect & pEndNode.m_usCleanRect))
        {
            //TODO occupycheck
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        byte uiThisSwampStart = (pStartNode.m_bySwampTag);
        byte uiThisSwampEnd = (pEndNode.m_bySwampTag); //include the swamp of start and end

        pStartNode.m_iOpenList = m_iFindingIndex;
        pStartNode.m_pParentNode = null;
        pStartNode.m_iParentDir = -1;
        pStartNode.CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);
        pEndNode.m_pParentNode = null;
        pStartNode.m_pNext = null;
        pStartNode.m_pPrev = null;
        int iProtect = m_iAstarProtect;
        float fLowestH = -1.0f;
        COneGrid pLowestF = pStartNode;
        COneGrid pLowestH = null;
        if (iStep > 5)
        {
            iProtect = iStep;
        }

        while (null != pLowestF && iProtect > 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                //CRuntimeLogger.LogError("Run out without a result");
                //we have lowest H any how
                break;
            }

#if DebugAStar //Not Possible to happen
            //make sure it is not in the close list
            if (m_iFindingIndex == pLowestF.m_iCloseList)
            {
                pLowestF = pLowestF.m_pNext;
                if (null == pLowestF)
                {
                    break;
                }
                continue;
            }

            if (GridMath.m_byWalkable != m_byGridStateData[pLowestF.m_sPos])
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }

            if (m_iFindingIndex != pLowestF.m_iHCalculated)
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                pLowestF.CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);
            }
#endif

            //Add to close list
            pLowestF.m_iCloseList = m_iFindingIndex;
            if (pLowestF == pEndNode)
            {
                //target
                //CRuntimeLogger.Log("lowestF = EndNode");
                break;
            }

            if (0 != (pLowestF.m_usCleanRect & pEndNode.m_usCleanRect))
            {
                //TODO occupycheck
                pEndNode.m_pParentNode = pLowestF;
                break;
            }

            if (pLowestF.m_fHValue < fLowestH || fLowestH < 0.0f)
            {
                fLowestH = pLowestF.m_fHValue;
                pLowestH = pLowestF;
            }

            #region Connect Points

            //check connected points
            //_tmpSuccessor only add in Successors, clear it after
            Successors(pLowestF.m_sPos, realEnd, pLowestF.m_iParentDir);
            for (int i = 0; i < 8; ++i)
            {
                if (null != _tmpSuccessor[i])
                {
                    if (m_iFindingIndex != _tmpSuccessor[i].m_iCloseList)
                    {
                        if (m_iFindingIndex != _tmpSuccessor[i].m_iOpenList)
                        {
                            //Add to open list
                            _tmpSuccessor[i].m_pParentNode = pLowestF;
                            _tmpSuccessor[i].m_iParentDir = i;
                            _tmpSuccessor[i].m_iOpenList = m_iFindingIndex;
                            _tmpSuccessor[i].CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);

                            COneGrid pLast = pLowestF;
                            for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                            {
                                pLast = node;
                                if (node.m_fFValue > _tmpSuccessor[i].m_fFValue)
                                {
                                    node.m_pPrev.m_pNext = _tmpSuccessor[i];
                                    _tmpSuccessor[i].m_pPrev = node.m_pPrev;
                                    _tmpSuccessor[i].m_pNext = node;
                                    node.m_pPrev = _tmpSuccessor[i];
                                    pLast = null;
                                    break;
                                }
                            }
                            if (null != pLast)
                            {
                                pLast.m_pNext = _tmpSuccessor[i];
                                _tmpSuccessor[i].m_pPrev = pLast;
                                _tmpSuccessor[i].m_pNext = null;
                            }
                        }
                        else
                        {
                            //Check G Value
                            if (_tmpSuccessor[i].m_pParentNode != pLowestF && _tmpSuccessor[i].GViaMeJPS(pLowestF, i))
                            {
                                if (_tmpSuccessor[i].m_fFValue < _tmpSuccessor[i].m_pPrev.m_fFValue)
                                {
                                    if (null != _tmpSuccessor[i].m_pNext)
                                    {
                                        _tmpSuccessor[i].m_pNext.m_pPrev = _tmpSuccessor[i].m_pPrev;
                                    }
                                    _tmpSuccessor[i].m_pPrev.m_pNext = _tmpSuccessor[i].m_pNext;
                                    //must not be the last one
                                    for (COneGrid node = pLowestF.m_pNext; null != node; node = node.m_pNext)
                                    {
                                        if (node.m_fFValue > _tmpSuccessor[i].m_fFValue)
                                        {
                                            node.m_pPrev.m_pNext = _tmpSuccessor[i];
                                            _tmpSuccessor[i].m_pPrev = node.m_pPrev;
                                            _tmpSuccessor[i].m_pNext = node;
                                            node.m_pPrev = _tmpSuccessor[i];
                                            break;
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                    _tmpSuccessor[i] = null;//important to clear the old values
                }
            }

            #endregion

            #region New Lowest F, Resort Order

            pLowestF = pLowestF.m_pNext;

            #endregion
        }

        if (null != pEndNode.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            COneGrid pLastParent = pEndNode;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = EPathRes.EPR_Done;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            //CRuntimeLogger.Log("JPS:" + m_pOpenList.Count);
            return _tmpRes;
        }

        if (null != pLowestH && null != pLowestH.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(pLowestH.m_v2Pos);
            COneGrid pLastParent = pLowestH;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = (
                Mathf.Abs(pLowestH.m_sPos.m_iX - realEnd.m_iX) +
                Mathf.Abs(pLowestH.m_sPos.m_iY - realEnd.m_iY)
                ) >
                (
                Mathf.Abs(realStart.m_iX - realEnd.m_iX) +
                Mathf.Abs(realStart.m_iY - realEnd.m_iY)
                )
             ? EPathRes.EPR_Incomplete : EPathRes.EPR_IncompleteBad;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        if (null != pLowestH && pLowestH == pStartNode)
        {
            //CRuntimeLogger.LogWarning("Why? What happend?");
            eRes = EPathRes.EPR_NoPath;
            return null;
        }

        eRes = EPathRes.EPR_NoPath;
        /*CRuntimeLogger.LogWarning("nop:" + (null == pStartNode ? "null" : string.Format("({0},{1})", pStartNode.m_sPos.m_iX, pStartNode.m_sPos.m_iY)) + " TO "
            + (null == pEndNode ? "null" : string.Format("({0},{1})", pEndNode.m_sPos.m_iX, pEndNode.m_sPos.m_iY))
            + " prot" + iProtect.ToString(CultureInfo.InvariantCulture) + "openlist:" + iOpenListStart.ToString(CultureInfo.InvariantCulture) + "loweset h:"
            + (null == pLowestH ? "null" : string.Format("({0},{1})", pLowestH.m_sPos.m_iX, pLowestH.m_sPos.m_iY)) + "lowest h  parent:"
            + (null == pLowestH.m_pParentNode ? "null" : string.Format("({0},{1})", pLowestH.m_pParentNode.m_sPos.m_iX, pLowestH.m_pParentNode.m_sPos.m_iY)));*/
        return null;
    }

    #region Multiple Jump

    /// <summary>
    /// return true if find target (not to ruin the lowestF list!)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="parent"></param>
    /// <param name="dir"></param>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="startSwamp"></param>
    /// <param name="targetSwamp"></param>
    /// <returns></returns>
    private bool BlockedJumpMJ(S2DPoint x, COneGrid parent, int dir, S2DPoint start, S2DPoint target, byte startSwamp, byte targetSwamp)
    {
        //CRuntimeLogger.Log("in BlockedJumpMJ: start:" + x + ", dir:" + dir + ":" + S2DPoint.dirs[dir]);
        if (0 != (dir & 1))
        {
            S2DPoint ret = BlockedJumpVertical(true, true, x, dir, target);
            if (ret.m_bValid)
            {
                COneGrid nextoplist = m_pGrids[ret.m_iX, ret.m_iY];
                if (m_iFindingIndex == nextoplist.m_iCloseList)//over
                {
                    return false;
                }

                if (ret == target)
                {
                    nextoplist.m_pParentNode = parent;
                    nextoplist.m_iParentDir = dir;
                    nextoplist.m_iOpenList = m_iFindingIndex;

                    nextoplist.m_pNext = null;
                    nextoplist.m_pPrev = m_pLowestF;
                    m_pLowestF.m_pNext = nextoplist;
                    //CRuntimeLogger.Log("end search due to reach target");
                    return true;
                }

                if (m_iFindingIndex != nextoplist.m_iOpenList)
                {
                    nextoplist.m_pParentNode = parent;
                    nextoplist.m_iParentDir = dir;
                    nextoplist.m_iOpenList = m_iFindingIndex;
                    nextoplist.CalculateWeightJPSDouble(start, target, m_iFindingIndex, startSwamp, targetSwamp);

                    //CRuntimeLogger.Log("add open list:" + ret);
                    COneGrid pLast = m_pLowestF;
                    for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                    {
                        pLast = node;
                        if (node.m_fFValue > nextoplist.m_fFValue)
                        {
                            node.m_pPrev.m_pNext = nextoplist;
                            nextoplist.m_pPrev = node.m_pPrev;
                            nextoplist.m_pNext = node;
                            node.m_pPrev = nextoplist;
                            pLast = null;
                            break;
                        }
                    }
                    if (null != pLast)
                    {
                        pLast.m_pNext = nextoplist;
                        nextoplist.m_pPrev = pLast;
                        nextoplist.m_pNext = null;
                    }
                }
                else if (nextoplist.m_pParentNode != parent && nextoplist.GViaMeJPSDouble(parent, dir) && nextoplist.m_fFValue < nextoplist.m_pPrev.m_fFValue)
                {
                    if (null != nextoplist.m_pNext)
                    {
                        nextoplist.m_pNext.m_pPrev = nextoplist.m_pPrev;
                    }
                    nextoplist.m_pPrev.m_pNext = nextoplist.m_pNext;
                    //must not be the last one
                    for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                    {
                        if (node.m_fFValue > nextoplist.m_fFValue)
                        {
                            node.m_pPrev.m_pNext = nextoplist;
                            nextoplist.m_pPrev = node.m_pPrev;
                            nextoplist.m_pNext = node;
                            node.m_pPrev = nextoplist;
                            break;
                        }
                    }
                }
            }
            return false;
        }

        S2DPoint n = new S2DPoint(x.m_iX + S2DPoint.dirs[dir].m_iX, x.m_iY + S2DPoint.dirs[dir].m_iY);

        #region while

        //test shows stack call faster then while!
        //while (true)
        //{
            if (!n.m_bValid || GridMath.m_byWalkable != m_byGridStateData[n])
            {
                //CRuntimeLogger.Log("end search due to reach edge");
                return false;
            }

            COneGrid now = m_pGrids[n.m_iX, n.m_iY];
            if (n == target)
            {
                //add target as child of me and return
                now.m_pParentNode = parent;
                now.m_iParentDir = dir;
                now.m_iOpenList = m_iFindingIndex;

                now.m_pNext = null;
                now.m_pPrev = m_pLowestF;
                m_pLowestF.m_pNext = now;
                //CRuntimeLogger.Log("end search due to reach target");
                return true;
            }

            if (0 != (m_pGrids[n.m_iX, n.m_iY].m_byCachedForceTable & (byte)(1 << dir)))
            {
                if (m_iFindingIndex == now.m_iCloseList)//over
                {
                    //CRuntimeLogger.Log("end search due to reach close point");
                    return false;
                }

                if (m_iFindingIndex != now.m_iOpenList) //over
                {
                    now.m_pParentNode = parent;
                    now.m_iParentDir = dir;
                    now.m_iOpenList = m_iFindingIndex;
                    now.CalculateWeightJPSDouble(start, target, m_iFindingIndex, startSwamp, targetSwamp);

                    //CRuntimeLogger.Log("add open list:" + n);
                    COneGrid pLast = m_pLowestF;
                    for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                    {
                        pLast = node;
                        if (node.m_fFValue > now.m_fFValue)
                        {
                            node.m_pPrev.m_pNext = now;
                            now.m_pPrev = node.m_pPrev;
                            now.m_pNext = node;
                            node.m_pPrev = now;
                            pLast = null;
                            break;
                        }
                    }
                    if (null != pLast)
                    {
                        pLast.m_pNext = now;
                        now.m_pPrev = pLast;
                        now.m_pNext = null;
                    }
                }
                else if (now.m_pParentNode != parent && now.GViaMeJPSDouble(parent, dir) && now.m_fFValue < now.m_pPrev.m_fFValue)
                {
                    if (null != now.m_pNext)
                    {
                        now.m_pNext.m_pPrev = now.m_pPrev;
                    }
                    now.m_pPrev.m_pNext = now.m_pNext;
                    //must not be the last one
                    for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                    {
                        if (node.m_fFValue > now.m_fFValue)
                        {
                            node.m_pPrev.m_pNext = now;
                            now.m_pPrev = node.m_pPrev;
                            now.m_pNext = node;
                            node.m_pPrev = now;
                            break;
                        }
                    }
                }
                //CRuntimeLogger.Log("end search due to reach diagonal force jump point");
                return false;
            }

            //middle diagonal point
            S2DPoint ret2 = BlockedJumpVertical(false, true, n, dir + 1, target);
            S2DPoint ret3 = BlockedJumpVertical(false, true, n, 0 == dir ? 7 : dir - 1, target);
            if (ret2.m_bValid || ret3.m_bValid)
            {
                //add double parent child
                COneGrid childNode;
                if (ret2.m_bValid)
                {
                    childNode = m_pGrids[ret2.m_iX, ret2.m_iY];
                    if (m_iFindingIndex != childNode.m_iCloseList)
                    {
                        if (m_iFindingIndex != childNode.m_iOpenList) //over
                        {
                            childNode.m_pParentNode = parent;
                            childNode.m_iParentDir = dir + 1;
                            childNode.m_iOpenList = m_iFindingIndex;
                            childNode.CalculateWeightJPSDouble(start, target, m_iFindingIndex, startSwamp, targetSwamp);
                            //CRuntimeLogger.Log("add open list double:" + ret2);

                            COneGrid pLast = m_pLowestF;
                            for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                            {
                                pLast = node;
                                if (node.m_fFValue > childNode.m_fFValue)
                                {
                                    node.m_pPrev.m_pNext = childNode;
                                    childNode.m_pPrev = node.m_pPrev;
                                    childNode.m_pNext = node;
                                    node.m_pPrev = childNode;
                                    pLast = null;
                                    break;
                                }
                            }
                            if (null != pLast)
                            {
                                pLast.m_pNext = childNode;
                                childNode.m_pPrev = pLast;
                                childNode.m_pNext = null;
                            }
                        }
                        else if (childNode.m_pParentNode != parent && childNode.GViaMeJPSDouble(parent, dir + 1))
                        {
                            if (childNode.m_fFValue < childNode.m_pPrev.m_fFValue)
                            {
                                if (null != childNode.m_pNext)
                                {
                                    childNode.m_pNext.m_pPrev = childNode.m_pPrev;
                                }
                                childNode.m_pPrev.m_pNext = childNode.m_pNext;
                                //must not be the last one
                                for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                                {
                                    if (node.m_fFValue > now.m_fFValue)
                                    {
                                        node.m_pPrev.m_pNext = childNode;
                                        childNode.m_pPrev = node.m_pPrev;
                                        childNode.m_pNext = node;
                                        node.m_pPrev = childNode;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (ret3.m_bValid)
                {
                    childNode = m_pGrids[ret3.m_iX, ret3.m_iY];
                    if (m_iFindingIndex != childNode.m_iCloseList)
                    {
                        if (m_iFindingIndex != childNode.m_iOpenList) //over
                        {
                            childNode.m_pParentNode = parent;
                            childNode.m_iParentDir = (0 == dir ? 7 : dir - 1);
                            childNode.m_iOpenList = m_iFindingIndex;
                            childNode.CalculateWeightJPSDouble(start, target, m_iFindingIndex, startSwamp, targetSwamp);
                            //CRuntimeLogger.Log("add open list double:" + ret3);

                            COneGrid pLast = m_pLowestF;
                            for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                            {
                                pLast = node;
                                if (node.m_fFValue > childNode.m_fFValue)
                                {
                                    node.m_pPrev.m_pNext = childNode;
                                    childNode.m_pPrev = node.m_pPrev;
                                    childNode.m_pNext = node;
                                    node.m_pPrev = childNode;
                                    pLast = null;
                                    break;
                                }
                            }
                            if (null != pLast)
                            {
                                pLast.m_pNext = childNode;
                                childNode.m_pPrev = pLast;
                                childNode.m_pNext = null;
                            }
                        }
                        else if (childNode.m_pParentNode != parent && childNode.GViaMeJPSDouble(parent, (0 == dir ? 7 : dir - 1)))
                        {
                            if (childNode.m_fFValue < childNode.m_pPrev.m_fFValue)
                            {
                                if (null != childNode.m_pNext)
                                {
                                    childNode.m_pNext.m_pPrev = childNode.m_pPrev;
                                }
                                childNode.m_pPrev.m_pNext = childNode.m_pNext;
                                //must not be the last one
                                for (COneGrid node = m_pLowestF.m_pNext; null != node; node = node.m_pNext)
                                {
                                    if (node.m_fFValue > now.m_fFValue)
                                    {
                                        node.m_pPrev.m_pNext = childNode;
                                        childNode.m_pPrev = node.m_pPrev;
                                        childNode.m_pNext = node;
                                        node.m_pPrev = childNode;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }//ret2 & ret3
            //n = new S2DPoint(n.m_iX + S2DPoint.dirs[dir].m_iX, n.m_iY + S2DPoint.dirs[dir].m_iY);
        //}

        #endregion

        return BlockedJumpMJ(n, parent, dir, start, target, startSwamp, targetSwamp);
    }

    /// <summary>
    /// Use for add node to open list in Multi-Jump Point
    /// </summary>
    private COneGrid m_pLowestF = null;

    /// <summary>
    /// Direct add all jump points to open list
    /// </summary>
    /// <param name="current"></param>
    /// <param name="currentn"></param>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="dir"></param>
    /// <param name="startSwamp"></param>
    /// <param name="targetSwamp"></param>
    private void SuccessorsMJ(S2DPoint current, COneGrid currentn, S2DPoint start, S2DPoint target, int dir, byte startSwamp, byte targetSwamp)
    {
        //CRuntimeLogger.Log("<color=#00FF00>in Successors</color>, current:" + current + " ,parent dir:" + ((-1 == dir) ? "none" : S2DPoint.dirs[dir].ToString()));
        if (-1 == dir)
        {
            for (int i = 0; i < 8; ++i)
            {
                int iX = current.m_iX + S2DPoint.dirs[i].m_iX;
                int iY = current.m_iY + S2DPoint.dirs[i].m_iY;
                //no parent dir, all walkable is neighbour
                if (iX >= 0 && iX < SceneConstant.m_iSceneSize
                 && iY >= 0 && iY < SceneConstant.m_iSceneSize
                 && GridMath.m_byWalkable == m_byGridStateData[iX * SceneConstant.m_iSceneSize + iY])
                {
                    if (BlockedJumpMJ(current, currentn, i, start, target, startSwamp, targetSwamp))
                    {
                        return;
                    }
                }
            }
            return;
        }

        byte neighbour = m_pGrids[current.m_iX, current.m_iY].m_bCachedNeigTable[dir];
        for (int i = 0; i < 8; ++i)
        {
            if (0 != ((1 << i) & neighbour))
            {
                if (BlockedJumpMJ(current, currentn, i, start, target, startSwamp, targetSwamp))
                {
                    return;
                }
            }
        }
    }

#if DebugJPSOpenlist
    public short[] _debugJpsOpenList = null;
    public short[] _debugJpsDeadList = null;
#endif

    public Stack<Vector2> FindPathJPSMJ(Vector2 vStart, Vector2 vTarget, int iStep, out EPathRes eRes)
    {
        ++m_iFindingIndex;

        S2DPoint start = vStart;
        S2DPoint realStart = GetNearGrid(0, start.ToValid());
        byte island = m_byIslandData[realStart];
        S2DPoint end = vTarget;
        S2DPoint realEnd = GetNearGrid(island, end.ToValid());

#if DebugJPSOpenlist
        _debugJpsOpenList = Enumerable.Repeat((short)-1, SceneConstant.m_iSceneSizeSq).ToArray();
        _debugJpsDeadList = Enumerable.Repeat((short)-1, SceneConstant.m_iSceneSizeSq).ToArray();
        _debugJpsOpenList[realStart] = realStart;
#endif

        if (realStart == realEnd)
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        //check clean rectangle
        COneGrid pStartNode = m_pGrids[realStart.m_iX, realStart.m_iY];
        COneGrid pEndNode = m_pGrids[realEnd.m_iX, realEnd.m_iY];
        if (0 != (pStartNode.m_usCleanRect & pEndNode.m_usCleanRect))
        {
            //TODO occupycheck
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        byte uiThisSwampStart = (pStartNode.m_bySwampTag);
        byte uiThisSwampEnd = (pEndNode.m_bySwampTag); //include the swamp of start and end

        pStartNode.m_iOpenList = m_iFindingIndex;
        pStartNode.m_pParentNode = null;
        pStartNode.m_iParentDir = -1;
        pStartNode.CalculateWeightJPSDouble(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);
        pEndNode.m_pParentNode = null;
        m_pLowestF = pStartNode;
        pStartNode.m_pNext = null;
        pStartNode.m_pPrev = null;
        int iProtect = m_iAstarProtect;
        float fLowestH = -1.0f;
        COneGrid pLowestH = null;

        if (iStep > 5)
        {
            iProtect = iStep;
        }

        while (null != m_pLowestF && iProtect > 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                break;
            }

#if DebugAStar //Not Possible to happen
            //make sure it is not in the close list
            if (m_iFindingIndex == pLowestF.m_iCloseList)
            {
                m_pLowestF = m_pLowestF.m_pNext;
                if (null == m_pLowestF)
                {
                    break;
                }
                continue;
            }

            if (GridMath.m_byWalkable != m_byGridStateData[pLowestF.m_sPos])
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }

            if (m_iFindingIndex != pLowestF.m_iHCalculated)
            {
                CRuntimeLogger.LogWarning("Why? What happend?");
                pLowestF.CalculateWeightJPS(pStartNode.m_sPos, pEndNode.m_sPos, m_iFindingIndex, uiThisSwampStart, uiThisSwampEnd);
            }
#endif

            //Add to close list
            m_pLowestF.m_iCloseList = m_iFindingIndex;
            if (m_pLowestF == pEndNode)
            {
                //target
                //CRuntimeLogger.Log("lowestF = EndNode");
                break;
            }

            if (0 != (m_pLowestF.m_usCleanRect & pEndNode.m_usCleanRect))
            {
                //TODO occupycheck
                //pEndNode.m_sDoubleParent = S2DPoint.invalid;
                pEndNode.m_pParentNode = m_pLowestF;
                break;
            }

            if (m_pLowestF.m_fHValue < fLowestH || fLowestH < 0.0f)
            {
                fLowestH = m_pLowestF.m_fHValue;
                pLowestH = m_pLowestF;
            }

            //Open List is sorted
            SuccessorsMJ(m_pLowestF.m_sPos, m_pLowestF, realStart, realEnd, m_pLowestF.m_iParentDir, uiThisSwampStart, uiThisSwampEnd);
            m_pLowestF = m_pLowestF.m_pNext;
        }

        if (null != pEndNode.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            COneGrid pLastParent = pEndNode;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                /*
                if (pLastParent.m_sDoubleParent.m_bValid)
                {
                    _tmpRes.Push(pLastParent.m_sDoubleParent.ToV2());
                }
                */
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = EPathRes.EPR_Done;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            //CRuntimeLogger.Log("JPS:" + m_pOpenList.Count);
            return _tmpRes;
        }

        if (null != pLowestH && null != pLowestH.m_pParentNode)
        {
            _tmpRes.Clear();
            _tmpRes.Push(pLowestH.m_v2Pos);
            COneGrid pLastParent = pLowestH;
            int iProtected2 = m_iAstarProtect;
            while (pLastParent.m_pParentNode != null && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    eRes = EPathRes.EPR_NoPath;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }
                /*
                if (pLastParent.m_sDoubleParent.m_bValid)
                {
                    _tmpRes.Push(pLastParent.m_sDoubleParent.ToV2());
                }
                */
                if (pLastParent.m_pParentNode.m_sPos != realStart)
                {
                    _tmpRes.Push(pLastParent.m_pParentNode.m_v2Pos);
                }
                pLastParent = pLastParent.m_pParentNode;
            }
            eRes = (
                Mathf.Abs(pLowestH.m_sPos.m_iX - realEnd.m_iX) +
                Mathf.Abs(pLowestH.m_sPos.m_iY - realEnd.m_iY)
                ) >
                (
                Mathf.Abs(realStart.m_iX - realEnd.m_iX) +
                Mathf.Abs(realStart.m_iY - realEnd.m_iY)
                )
             ? EPathRes.EPR_Incomplete : EPathRes.EPR_IncompleteBad;
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        if (null != pLowestH && pLowestH == pStartNode)
        {
            //CRuntimeLogger.LogWarning("Why? What happend?");
            eRes = EPathRes.EPR_NoPath;
            return null;
        }

        eRes = EPathRes.EPR_NoPath;
        /*CRuntimeLogger.LogWarning("nop:" + (null == pStartNode ? "null" : string.Format("({0},{1})", pStartNode.m_sPos.m_iX, pStartNode.m_sPos.m_iY)) + " TO "
            + (null == pEndNode ? "null" : string.Format("({0},{1})", pEndNode.m_sPos.m_iX, pEndNode.m_sPos.m_iY))
            + " prot" + iProtect.ToString(CultureInfo.InvariantCulture) + "openlist:" + iOpenListStart.ToString(CultureInfo.InvariantCulture) + "loweset h:"
            + (null == pLowestH ? "null" : string.Format("({0},{1})", pLowestH.m_sPos.m_iX, pLowestH.m_sPos.m_iY)) + "lowest h  parent:"
            + (null == pLowestH.m_pParentNode ? "null" : string.Format("({0},{1})", pLowestH.m_pParentNode.m_sPos.m_iX, pLowestH.m_pParentNode.m_sPos.m_iY)));*/
        return null;
    }

    #endregion

    #endregion

    #region Swamp

    [HideInInspector]
    public S2DPoint[] m_sSwampSeeds;

    public byte[] m_bySwampData;
    private const int m_iSwampLimit = SceneConstant.m_iSceneSize / 4;

    /// <summary>
    /// this is just a region
    /// </summary>
    private class CSwampData
    {
        public List<S2DPoint> m_pPoints;
        public S2DPoint m_v2Seed;

        public bool Contains(S2DPoint p)
        {
            for (int i = 0; i < m_pPoints.Count; ++i)
            {
                S2DPoint sp = m_pPoints[i];
                if (sp == p)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// make sure this region contains only one region which is connected and contains seed.
        /// </summary>
        public void TrimToConnectedRegionOfSeed()
        {
            if (!Contains(m_v2Seed))
            {
                m_pPoints = new List<S2DPoint>();
                return;
            }

            //1 is not reachable
            byte[,] grid = new byte[SceneConstant.m_iSceneSize, SceneConstant.m_iSceneSize];
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    grid[i, j] = 1;
                }                
            }
            for (int i = 0; i < m_pPoints.Count; ++i)
            {
                grid[m_pPoints[i].m_iX, m_pPoints[i].m_iY] = 0;
            }
            grid[m_v2Seed.m_iX, m_v2Seed.m_iY] = 2;

            GridMath.ExpandPaint(ref grid, SceneConstant.m_iSceneSize, SceneConstant.m_iSceneSize);
            m_pPoints = new List<S2DPoint>();
            for (int i = 0; i < SceneConstant.m_iSceneSize; ++i)
            {
                for (int j = 0; j < SceneConstant.m_iSceneSize; ++j)
                {
                    if (2 == grid[i, j])
                    {
                        m_pPoints.Add(new S2DPoint(i, j));
                    }
                }
            }
        }
    }

    private List<CSwampData> _tmpFoundSwanps;
    private Dictionary<int, float> _foundDic;
    private Dictionary<int, S2DPoint[]> _foundPDic;

    /// <summary>
    /// Use to record the grid state, and mark the state of previous swamp as occupy
    /// </summary>
    private byte[] _tmpGrid;

    /// <summary>
    /// The seeds are simply the point at a corner of a wall "L"
    /// </summary>
    /// <returns>a list of all seeds</returns>
    static private List<S2DPoint> FindAllSwampSeeds(byte[] griddata)
    {
        List<S2DPoint> ret = new List<S2DPoint>();
        for (int iX = 0; iX < SceneConstant.m_iSceneSize; ++iX)
        {
            for (int iY = 0; iY < SceneConstant.m_iSceneSize; ++iY)
            {
                if (GridMath.m_byWalkable == griddata[iX * SceneConstant.m_iSceneSize + iY])
                {
                    bool[,] occupied =
                    {
                        {
                            iX - 1 < 0 || iY - 1 < 0 || GridMath.m_byWalkable != griddata[(iX - 1)*SceneConstant.m_iSceneSize + iY - 1],
                            iY - 1 < 0 || GridMath.m_byWalkable != griddata[(iX)*SceneConstant.m_iSceneSize + iY - 1],
                            iX + 1 >= SceneConstant.m_iSceneSize || iY - 1 < 0 || GridMath.m_byWalkable != griddata[(iX + 1)*SceneConstant.m_iSceneSize + iY - 1]
                        },
                        {
                            iX - 1 < 0 || GridMath.m_byWalkable != griddata[(iX - 1)*SceneConstant.m_iSceneSize + iY],
                            false,
                            iX + 1 >= SceneConstant.m_iSceneSize || GridMath.m_byWalkable != griddata[(iX + 1)*SceneConstant.m_iSceneSize + iY]
                        },
                        {
                            iX - 1 < 0 || iY + 1 >= SceneConstant.m_iSceneSize || GridMath.m_byWalkable != griddata[(iX - 1)*SceneConstant.m_iSceneSize + iY + 1],
                            iY + 1 >= SceneConstant.m_iSceneSize || GridMath.m_byWalkable != griddata[(iX)*SceneConstant.m_iSceneSize + iY + 1],
                            iX + 1 >= SceneConstant.m_iSceneSize || iY + 1 >= SceneConstant.m_iSceneSize || GridMath.m_byWalkable != griddata[(iX + 1)*SceneConstant.m_iSceneSize + iY + 1]
                        },
                    };

                    /*
                     * X X X    X X X     O O O    O O O
                     * X S O or O S X or  X S O or O S X
                     * O O O    O O O     X X X    X X X
                     */
                    if (occupied[0, 0] && occupied[0, 1] && occupied[0, 2] && occupied[1, 0])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }
                    else if (occupied[0, 0] && occupied[0, 1] && occupied[0, 2] && occupied[1, 2])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }
                    else if (occupied[2, 0] && occupied[2, 1] && occupied[2, 2] && occupied[1, 0])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }
                    else if (occupied[2, 0] && occupied[2, 1] && occupied[2, 2] && occupied[1, 2])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }

                    /*
                     * X X O    X O O     O O X    O X X
                     * X S O or X S O or  O S X or O S X
                     * x O O    X X O     O X X    O O X
                     */
                    else if (occupied[0, 0] && occupied[1, 0] && occupied[2, 0] && occupied[0, 1])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }
                    else if (occupied[0, 0] && occupied[1, 0] && occupied[2, 0] && occupied[2, 1])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }
                    else if (occupied[0, 2] && occupied[1, 2] && occupied[2, 2] && occupied[0, 1])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }
                    else if (occupied[0, 2] && occupied[1, 2] && occupied[2, 2] && occupied[2, 1])
                    {
                        ret.Add(new S2DPoint(iX, iY));
                    }                    
                }
            }            
        }
        return ret;
    }

    /// <summary>
    /// This is a function using in editor to pre-determine all swamps
    /// </summary>
    public void FindAllSwamps()
    {
        _tmpFoundSwanps = new List<CSwampData>();
        _foundDic = new Dictionary<int, float>(new IntEqualityComparer());
        _foundPDic = new Dictionary<int, S2DPoint[]>(new IntEqualityComparer());
        _tmpGrid = new byte[SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize];
        for (int xx = 0; xx < SceneConstant.m_iSceneSize; ++xx)
        {
            for (int yy = 0; yy < SceneConstant.m_iSceneSize; ++yy)
            {
                _tmpGrid[xx*SceneConstant.m_iSceneSize + yy] = m_byGridStateData[xx*SceneConstant.m_iSceneSize + yy];
            }
        }

        List<S2DPoint> allSeeds = FindAllSwampSeeds(m_byGridStateData);
        m_sSwampSeeds = allSeeds.ToArray();
        int iAll = allSeeds.Count;
        int iProg = 0;
#if UNITY_EDITOR
        EditorUtility.DisplayProgressBar("查找中", string.Format("查找进度 - {0}/{1}", iProg, iAll), 0.0f);
#endif
        for (int i = 0; i < allSeeds.Count; ++i)
        {
            S2DPoint seeds = allSeeds[i];
            CSwampData swap = ExtendSeed(seeds, m_iSwampLimit, iProg, iAll);
            if (null != swap)
            {
                _tmpFoundSwanps.Add(swap);
                for (int j = 0; j < _tmpFoundSwanps[_tmpFoundSwanps.Count - 1].m_pPoints.Count; ++j)
                {
                    _tmpGrid[_tmpFoundSwanps[_tmpFoundSwanps.Count - 1].m_pPoints[j]] = GridMath.m_byNoWalkable;
                }
            }
            ++iProg;
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("查找中", string.Format("查找进度 - {0}/{1}", iProg, iAll), iProg/(float) iAll);
#endif
        }

        CRuntimeLogger.Log(_tmpFoundSwanps.Count);
        int iCount = 0;
        for (int i = 0; i < _tmpFoundSwanps.Count; ++i)
        {
            CSwampData smp = _tmpFoundSwanps[i];
            iCount += smp.m_pPoints.Count;
        }
        CRuntimeLogger.Log(iCount);

        //keep the largest 254 swaps (mark as 1-255)
        for (int i = 0; i < _tmpFoundSwanps.Count; ++i)
        {
            for (int j = i + 1; j < _tmpFoundSwanps.Count; ++j)
            {
                if (_tmpFoundSwanps[i].m_pPoints.Count < _tmpFoundSwanps[j].m_pPoints.Count)
                {
                    CSwampData smp = _tmpFoundSwanps[i];
                    _tmpFoundSwanps[i] = _tmpFoundSwanps[j];
                    _tmpFoundSwanps[j] = smp;
                }
            }            
        }

        m_bySwampData = new byte[SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize];
        for (int i = 0; i < _tmpFoundSwanps.Count && i < 254; ++i)
        {
            for (int j = 0; j < _tmpFoundSwanps[i].m_pPoints.Count; ++j)
            {
                m_bySwampData[
                    _tmpFoundSwanps[i].m_pPoints[j].m_iX*SceneConstant.m_iSceneSize +
                    _tmpFoundSwanps[i].m_pPoints[j].m_iY] = (byte)(i + 1);
            }
        }
#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
    }

    /// <summary>
    /// Increase the seed to a swamp region
    /// </summary>
    /// <param name="seed">seed to extend</param>
    /// <param name="iSizeLimit">if not limit the size, strictly speaking, the all the grids form a swamp</param>
    /// <param name="iProg">to display progressbar</param>
    /// <param name="iAll">to display progressbar</param>
    /// <returns>swamp region</returns>
    private CSwampData ExtendSeed(S2DPoint seed, int iSizeLimit, int iProg, int iAll)
    {
        CSwampData initialSwamp = GetSwampRegionWithRadius(seed, iSizeLimit);
        if (0 == initialSwamp.m_pPoints.Count)
        {
            //CRuntimeLogger.LogWarning("no point in connect region of seed!");
            return null;
        }

        return TrimToSwamp(initialSwamp, iProg, iAll);
    }

    /// <summary>
    /// get boundary of a region, which are just walkable neighbour grids
    /// </summary>
    /// <param name="swamp">the region</param>
    /// <returns>the boundary grids</returns>
    private List<S2DPoint> GetBoundary(CSwampData swamp)
    {
        List<S2DPoint> ret = new List<S2DPoint>();
        List<short> retdic = new List<short>();
        for (int i = 0; i < swamp.m_pPoints.Count; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                S2DPoint bpoint = new S2DPoint(swamp.m_pPoints[i].m_iX + S2DPoint.dirs[j].m_iX, swamp.m_pPoints[i].m_iY + S2DPoint.dirs[j].m_iY);
                if (bpoint.m_bValid 
                 && GridMath.m_byWalkable == m_byGridStateData[bpoint]
                 && !swamp.Contains(bpoint) 
                 && !retdic.Contains(bpoint))
                {
                    ret.Add(bpoint);
                    retdic.Add(bpoint);
                }
            }
        }
        return ret;
    }

    private CSwampData TrimToSwamp(CSwampData swamp, int iProg, int iAll)
    {
        //=================================================
        //Step1: find boundary
        //Step2: find path with or without group.
        //Step3: if path is different, add all points in both path and group, to boundary and remove them from group.
        //  Step3.5: if path contains seed, return empty
        //Step4: repeat until valid

        //=================================================
        //Step1: find boundary
        List<S2DPoint> b = GetBoundary(swamp);
        bool bDirty = true;
        int iProtected = 10000;
        while (swamp.m_pPoints.Count > 0 && b.Count > 1 && bDirty)
        {
            --iProtected;
            if (iProtected < 0)
            {
                CRuntimeLogger.LogWarning("why enter protected?");
                return null;
            }
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("查找中", string.Format("节点个数{2},边界数{3} - {0}/{1}",
                      iProg, iAll, swamp.m_pPoints.Count, b.Count), iProg / (float)iAll);
#endif
            //Check dirty
            bDirty = false;
            byte[] grid2 = new byte[SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize];
            for (int xx = 0; xx < SceneConstant.m_iSceneSize; ++xx)
            {
                for (int yy = 0; yy < SceneConstant.m_iSceneSize; ++yy)
                {
                    grid2[xx*SceneConstant.m_iSceneSize + yy] = 
                        GridMath.m_byWalkable == m_byGridStateData[xx*SceneConstant.m_iSceneSize + yy]
                        ? (!swamp.Contains(new S2DPoint(xx, yy))
                            ?  GridMath.m_byWalkable
                            :  GridMath.m_byNoWalkable)
                        :  GridMath.m_byNoWalkable;
                }
            }

            //=================================================
            //Step2: find path with or without group.
            for (int i = 0; i < b.Count; ++i)
            {
                for (int j = i + 1; j < b.Count; ++j)
                {
                    int iTag = (short) b[i] > (short) b[j]
                        ? (b[i]*(SceneConstant.m_iSceneSize*SceneConstant.m_iSceneSize + 1) + b[j])
                        : (b[j]*(SceneConstant.m_iSceneSize*SceneConstant.m_iSceneSize + 1) + b[i]);

                    //find path i, j
                    float fP1, fP2;
                    S2DPoint[] path1;

                    if (_foundDic.ContainsKey(iTag))
                    {
                        fP1 = _foundDic[iTag];
                        path1 = _foundPDic[iTag];
                    }
                    else
                    {
                        path1 = GridMath.StrictAStar(b[i], b[j], m_byGridStateData, out fP1);
                        _foundDic.Add(iTag, fP1);
                        _foundPDic.Add(iTag, path1);
                    }

                    GridMath.StrictAStar(b[i], b[j], grid2, out fP2);

                    if (null != path1 && path1.Length > 1 && (fP1 + 0.2f < fP2 || fP2 < 0.1f))
                    {
                        List<S2DPoint> newGroup = new List<S2DPoint>();
                        for (int ii = 0; ii < swamp.m_pPoints.Count; ++ii)
                        {
                            bool bValid = true;
                            for (int jj = 0; jj < path1.Length; ++jj)
                            {
                                if (path1[jj] == swamp.m_pPoints[ii])
                                {
                                    bValid = false;
                                    break;
                                }
                            }
                            if (bValid)
                            {
                                newGroup.Add(swamp.m_pPoints[ii]);
                            }
                        }
                        if (swamp.m_pPoints.Count == newGroup.Count)
                        {
                            CRuntimeLogger.LogWarning("why not exclude anything?");
                        }
                        else
                        {
                            bDirty = true;
                            swamp = new CSwampData
                            {
                                m_pPoints = newGroup,
                                m_v2Seed = swamp.m_v2Seed,
                            };
                            swamp.TrimToConnectedRegionOfSeed();
                            if (swamp.m_pPoints.Count < 1)
                            {
                                return null;
                            }
                            b = GetBoundary(swamp);

                            break;
                        }
                    }

                }
                if (bDirty)
                {
                    break;
                }
            }
        }

        if (swamp.m_pPoints.Count > 0)
        {
            return swamp;
        }
        return null;
    }

    /// <summary>
    /// it is need to be not in _tmpFoundSwanps
    /// </summary>
    private CSwampData GetSwampRegionWithRadius(S2DPoint seed, int iRadius)
    {
        List<S2DPoint> points = new List<S2DPoint>();
        for (int i = seed.m_iX - iRadius; i < seed.m_iX + iRadius; ++i)
        {
            for (int j = seed.m_iY - iRadius; j < seed.m_iY + iRadius; ++j)
            {
                S2DPoint pt = new S2DPoint(i, j);
                if (pt.m_bValid && GridMath.m_byWalkable == _tmpGrid[pt])
                {
                    points.Add(pt);
                }
            }            
        }

        CSwampData swmp = new CSwampData
        {
            m_pPoints = points,
            m_v2Seed = seed,
        };
        swmp.TrimToConnectedRegionOfSeed();
        return swmp;
    }

    #endregion

#if DebugBlockAStar
    #region Block AStar

    public class COneSBlockGrid
    {
        public bool m_bOccupied;
        public S2DPoint m_sWorldPos;
        public S2DPoint m_sLocalPos;
        public COneBBlockGrid m_pParent;
        public float m_GValue;
        public byte m_iEdgeIndex;
        public int m_iGCalculated;
        public int m_iCloseList;
        public int m_iIsIngress;

        /// <summary>
        /// if we are ingress, we have ingress dir to move back to the other Big block
        /// </summary>
        public byte m_byInGressDir;

        /// <summary>
        /// if we are not ingress, we have walk data to move inside this Big block to a ingress
        /// </summary>
        public ushort m_shWalk;

        public byte m_byWalk;

        /// <summary>
        /// like a index of the parent small node of this node
        /// </summary>
        public byte m_byEdge;
    }

    public class COneBBlockGrid
    {
        public int m_iOpenList;
        public int m_iCloseList;
        public int m_iHeapCalced;
        public float m_fHeap;

        #region Neighbour

        public COneBBlockGrid[] m_pNeighbour;

        #endregion

        #region Cached LDDB

        public byte[] m_byLen;
        public ushort[] m_ushWalk;

        #endregion

        #region Sub grids

        public COneSBlockGrid[] m_pChildGird;

        #endregion

        #region LinkList

        public COneBBlockGrid m_pPrev = null;
        public COneBBlockGrid m_pNext = null;

        #endregion

    }

    #region Generate LDDB

    /// <summary>
    /// 7 bit "nnnxxyy"
    /// xx, yy is number of straight and diagonal moves
    /// </summary>
    public byte[] m_byLDDBA = null;

    /// <summary>
    /// The LDDB, which is binary as 16 bit "aaabbbcccdddxxyy"
    /// aaa,bbb,ccc,ddd is the direction
    /// </summary>
    public ushort[] m_ushLDDBB = null;

    public void GenerateLDDB()
    {
        m_byLDDBA = new byte[512 * 28];
        m_ushLDDBB = new ushort[512 * 28];
        for (int k = 0; k < 512; ++k)//2^9
        {
            // index of boundary
            // 0 7 6
            // 1 x 5
            // 2 3 4
            for (int i = 0; i < 8; ++i)
            {
                for (int j = i + 1; j < 8; ++j)//index i to index j, i < j, i, j in 0-7
                {
                    //for x > y, n=7*y+x-(y(y+1)/2)-1 can map (x,y) to 0-27
                    int n = 7 * i + j - (i * (i + 1) / 2) - 1;
#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar("查找中", string.Format("查找进度 - {0}/{1}", k * 28 + n, 512 * 28), (k * 28 + n) / (float)(512 * 28));
#endif
                    S2DPoint start = new S2DPoint(1 + S2DPoint.dirs[i].m_iX, 1 + S2DPoint.dirs[i].m_iY);
                    S2DPoint end = new S2DPoint(1 + S2DPoint.dirs[j].m_iX, 1 + S2DPoint.dirs[j].m_iY);

                    byte[,] grid = new byte[3,3];
                    for (int xx = 0; xx < 3; ++xx)
                    {
                        for (int yy = 0; yy < 3; ++yy)
                        {
                            int gg = xx*3 + yy;
                            grid[xx, yy] = (0 == (k & (1 << gg))) ? (byte) 0 : (byte) 1;
                        }                        
                    }

                    byte len = 0;
                    ushort ret = PathIn3x3(grid, start, end, out len);

                    //CRuntimeLogger.Log("generate for :k:" + k + ", n:" + n + ", i:" + i + ", j:" + j);
                    m_ushLDDBB[k * 28 + n] = ret;
                    m_byLDDBA[k * 28 + n] = len;
                }
            }

        }
#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
    }

    /// <summary>
    /// grid is given as: 0 - walkable, 1 - non-walkable
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="byLength"></param>
    /// <returns></returns>
    private ushort PathIn3x3(byte[,] grid, S2DPoint start, S2DPoint end, out byte byLength)
    {
        if (1 == grid[start.m_iX, start.m_iY]
         || 1 == grid[end.m_iX, end.m_iY])
        {
            byLength = 0;
            return 0;
        }

        byte[,] initialstate = new byte[3,3];
        ushort[,] initialstep = new ushort[3, 3];
        byte[,] initiallength = new byte[3,3];
        float[,] flength = new float[3, 3];

        string[,] debugString = new string[3,3];

        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                initialstep[i, j] = 0;
                initiallength[i, j] = 0;
                flength[i, j] = -1.0f;
                if (i == start.m_iX && j == start.m_iY)
                {
                    initialstate[i, j] = 2;
                    flength[i, j] = 0.0f;
                }
                else if (1 == grid[i, j])
                {
                    initialstate[i, j] = 1;
                }
                else if (0 == grid[i, j])
                {
                    initialstate[i, j] = 0;
                }

                debugString[i, j] = "-1";
            }            
        }

        //paint the grid
        for (int n = 0; n < 4; ++n) //max steps of a 3x3 grid is 4
        {
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (0 == initialstate[i, j])
                    {
                        int ilowestdir = -1;
                        float flowestG = -1.0f;
                        int lowestX = -1;
                        int lowestY = -1;
                        bool bAdded = false;
                        for (int k = 0; k < 8; ++k)
                        {
                            int x = i + S2DPoint.dirs[k].m_iX;
                            int y = j + S2DPoint.dirs[k].m_iY;
                            if (x >= 0 && x < 3 && y >= 0 && y < 3 && 2 == initialstate[x, y])
                            {
                                bool bdiagonal = (0 == (k & 1));
                                if (flowestG < 0.0f
                                 || flowestG > flength[x, y] + (bdiagonal ? GridMath.m_fDiagDist : GridMath.m_fStraightDist) + 0.1f)
                                {
                                    flowestG = flength[x, y] + (bdiagonal ? GridMath.m_fDiagDist : GridMath.m_fStraightDist);
                                    ilowestdir = k;
                                    lowestX = x;
                                    lowestY = y;
                                    bAdded = true;
                                }
                            }
                        }

                        if (bAdded)
                        {
                            initialstate[i, j] = 2;
                            flength[i, j] = flowestG;
                            initialstep[i, j] = (ushort)((initialstep[lowestX, lowestY] << 3) + ilowestdir);
                            int stpNum = (initiallength[lowestX, lowestY] >> 4);
                            int stpNumStraight = (initiallength[lowestX, lowestY] >> 2) % 4;
                            int stpNumDiag = initiallength[lowestX, lowestY] % 4;

                            ++stpNum;
                            if (0 == (ilowestdir & 1))
                            {
                                ++stpNumDiag;
                            }
                            else
                            {
                                ++stpNumStraight;
                            }
                            if (stpNum != (stpNumDiag + stpNumStraight))
                            {
                                CRuntimeLogger.Log("stp:" + stpNum + "," + (stpNum << 4) + ",straight:" + stpNumStraight + "," + (stpNumStraight << 2) + ",diag:" + stpNumDiag + ",value:" + initiallength[i, j] + "," + (stpNum << 4 + stpNumStraight << 2 + stpNumDiag));
                            }
                            initiallength[i, j] = (byte)((stpNum << 4) + (stpNumStraight << 2) + stpNumDiag);
                            //CRuntimeLogger.Log("stp:" + stpNum + "," + (stpNum << 4) + ",straight:" + stpNumStraight + "," + (stpNumStraight << 2) + ",diag:" + stpNumDiag + ",value:" + initiallength[i, j] + "," + (stpNum << 4 + stpNumStraight << 2 + stpNumDiag));
                        }                        
                    }
                }
            }
        }

        byLength = initiallength[end.m_iX, end.m_iY];

        /*
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (2 == initialstate[i, j])
                {
                    if (i == start.m_iX && j == start.m_iY)
                    {
                        debugString[i, j] = "<color=#FFFF00>" + Mathf.RoundToInt(flength[i, j] * 10.0f) + "</color>" + "," + initiallength[i, j] + "," + initialstep[i, j];
                    }
                    else if (i == end.m_iX && j == end.m_iY)
                    {
                        debugString[i, j] = "<color=#00FFFF>" + Mathf.RoundToInt(flength[i, j] * 10.0f) + "</color>" + "," + initiallength[i, j] + "," + initialstep[i, j];
                    }
                    else
                    {
                        debugString[i, j] = Mathf.RoundToInt(flength[i, j] * 10.0f) + "," + initiallength[i, j] + "," + initialstep[i, j];
                    }
                }
            }            
        }

        CRuntimeLogger.Log(string.Format("{0}, {1}, {2}\n {3}, {4}, {5}\n {6}, {7}, {8}",
            debugString[0, 0], debugString[0, 1], debugString[0, 2],
            debugString[1, 0], debugString[1, 1], debugString[1, 2],
            debugString[2, 0], debugString[2, 1], debugString[2, 2]));
        
        */
        return initialstep[end.m_iX, end.m_iY];
    }

    #endregion

    #region prepare grids

    public COneSBlockGrid[] m_pSmallGrids;
    public COneBBlockGrid[] m_pBigGrids;

    public void ResetBlockAStarBlocks()
    {
        m_pBigGrids = new COneBBlockGrid[22 * 22];
        for (int i = 0; i < 22; ++i)
        {
            for (int j = 0; j < 22; ++j)
            {
                int entry = 0;
                for (int xx = i * 3 - 1; xx <= i * 3 + 1; ++xx)
                {
                    for (int yy = j * 3 - 1; yy <= j * 3 + 1; ++yy)
                    {
                        S2DPoint pt = new S2DPoint(xx, yy);
                        int gg = (xx - (i * 3 - 1)) * 3 + yy - (j * 3 - 1);
                        byte occupy = pt.m_bValid ? (GridMath.m_byWalkable == m_byGridStateData[pt] ? (byte)0 : (byte)1) : (byte)1;
                        entry |= (occupy << gg);
                    }
                }

                m_pBigGrids[i * 22 + j] = new COneBBlockGrid
                {
                    m_byLen = new byte[28],
                    m_ushWalk = new ushort[28],
                    m_pChildGird = new COneSBlockGrid[9],
                    m_pNeighbour = new COneBBlockGrid[8],
                };
                for (int dt = 0; dt < 28; ++dt)
                {
                    m_pBigGrids[i * 22 + j].m_byLen[dt] = m_byLDDBA[entry * 28 + dt];
                    m_pBigGrids[i * 22 + j].m_ushWalk[dt] = m_ushLDDBB[entry * 28 + dt];
                }
            }
        }

        m_pSmallGrids = new COneSBlockGrid[66 * 66];
        for (int i = 0; i < 66; ++i)
        {
            for (int j = 0; j < 66; ++j)
            {
                S2DPoint realPt = new S2DPoint(i - 1, j - 1);
                m_pSmallGrids[i * 66 + j] = new COneSBlockGrid
                {
                    m_sWorldPos = realPt,
                    m_sLocalPos = new S2DPoint(i % 3, j % 3),
                    m_bOccupied = realPt.m_bValid && GridMath.m_byWalkable != m_byGridStateData[realPt],
                    m_pParent = m_pBigGrids[(i / 3) * 22 + (j / 3)],
                };

                m_pSmallGrids[i * 66 + j].m_pParent.m_pChildGird[
                    m_pSmallGrids[i * 66 + j].m_sLocalPos.m_iX * 3 
                  + m_pSmallGrids[i * 66 + j].m_sLocalPos.m_iY] = m_pSmallGrids[i * 66 + j];

                for (byte k = 0; k < 8; ++k)
                {
                    if (S2DPoint.dirs[k].m_iX == m_pSmallGrids[i * 66 + j].m_sLocalPos.m_iX - 1
                     && S2DPoint.dirs[k].m_iY == m_pSmallGrids[i * 66 + j].m_sLocalPos.m_iY - 1)
                    {
                        m_pSmallGrids[i * 66 + j].m_iEdgeIndex = k;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < 22; ++i)
        {
            for (int j = 0; j < 22; ++j)
            {
                for (int k = 0; k < 8; ++k)
                {
                    int x = S2DPoint.dirs[k].m_iX + i;
                    int y = S2DPoint.dirs[k].m_iY + j;
                    if (x >= 0 && x < 22 && y >= 0 && y < 22)
                    {
                        m_pBigGrids[i * 22 + j].m_pNeighbour[k] = m_pBigGrids[x * 22 + y];
                    }
                    else
                    {
                        m_pBigGrids[i * 22 + j].m_pNeighbour[k] = null;
                    }
                }
            }
        }
    }

    #endregion

    private void ExpandBlock(COneBBlockGrid currentBlock, bool bFirst, COneSBlockGrid start)
    {
        //CRuntimeLogger.Log("<color=#00FFFF>expand block</color>:" + currentBlock.m_pChildGird[4].m_sWorldPos);

        #region First Block

        if (bFirst)
        {
            //1-current node is in middle
            for (int i = 0; i < 8; ++i)
            {
                S2DPoint edge = new S2DPoint(1 + S2DPoint.dirs[i].m_iX, 1 + S2DPoint.dirs[i].m_iY);
                if (edge != start.m_sLocalPos)
                {
                    //update fvalue, and add neigbour to open list
                    if (1 == start.m_sLocalPos.m_iX && 1 == start.m_sLocalPos.m_iY)
                    {
                        currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_GValue = (0 == (i & 1))
                            ? GridMath.m_fDiagDist
                            : GridMath.m_fStraightDist;
                        currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_iGCalculated = m_iFindingIndex;
                    }
                    else
                    {
                        int n = i > start.m_iEdgeIndex
                            ? (7*start.m_iEdgeIndex + i - (start.m_iEdgeIndex*(start.m_iEdgeIndex + 1)/2) - 1)
                            : (7*i + start.m_iEdgeIndex - (i*(i + 1)/2) - 1);
                        byte len = currentBlock.m_byLen[n];
                        if (len > 0) //should be connected
                        {
                            currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_GValue
                                    = ((len >> 2) % 4) * GridMath.m_fStraightDist
                                      + (len % 4) * GridMath.m_fDiagDist;
                            currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_iGCalculated = m_iFindingIndex;

                            currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_byWalk = len;
                            currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_shWalk = currentBlock.m_ushWalk[n];
                            currentBlock.m_pChildGird[edge.m_iX * 3 + edge.m_iY].m_byEdge = start.m_iEdgeIndex;
                        }
                    }
                }
            }
        }
        #endregion
        #region not first block
        else
        {
            //we find our ingress that ingress has ever been calculated
            for (int i = 0; i < 8; ++i)
            {
                COneSBlockGrid edge = currentBlock.m_pChildGird[(S2DPoint.dirs[i].m_iX + 1) * 3 + S2DPoint.dirs[i].m_iY + 1];
                if (m_iFindingIndex != edge.m_iIsIngress && !edge.m_bOccupied)
                {
                    //fill GValue
                    float fLowestG = -1.0f;
                    ushort walkdata = 0;
                    byte bywalkdata = 0;
                    byte ingressedge = 0;

                    for (int j = 0; j < 8; ++j)
                    {
                        COneSBlockGrid edge2 = currentBlock.m_pChildGird[(S2DPoint.dirs[j].m_iX + 1) * 3 + S2DPoint.dirs[j].m_iY + 1];
                        if (m_iFindingIndex == edge2.m_iIsIngress)
                        {
                            int n = i > j
                            ? (7*j + i - (j*(j + 1)/2) - 1)
                            : (7*i + j - (i*(i + 1)/2) - 1);
                            byte thisWKData = currentBlock.m_byLen[n];
                            if (thisWKData > 0) //should be connected at first!
                            {
                                float fNewG = edge2.m_GValue
                                  + ((thisWKData >> 2) % 4) * GridMath.m_fStraightDist
                                  + (thisWKData % 4) * GridMath.m_fDiagDist;

                                if (fNewG > 0.0f && (fLowestG < 0.0f || fNewG < fLowestG))
                                {
                                    fLowestG = fNewG;
                                    bywalkdata = thisWKData;
                                    ingressedge = (byte)j;

                                    walkdata = currentBlock.m_ushWalk[n];
                                }
                            }
                        }
                    }

                    if (fLowestG > 0.0f)
                    {
                        edge.m_iGCalculated = m_iFindingIndex;
                        edge.m_byEdge = ingressedge;
                        edge.m_GValue = fLowestG;
                        edge.m_byWalk = bywalkdata;
                        edge.m_shWalk = walkdata;
                    }
                    else
                    {
                        /*
                        CRuntimeLogger.LogWarning(string.Format("we are expanding a block without ingress nodes!\n {0},{1},{2}\n{3},heap={4},{5},{6},{7},{8}",
                            currentBlock.m_pChildGird[(S2DPoint.dirs[0].m_iX + 1) * 3 + S2DPoint.dirs[0].m_iY + 1].m_iIsIngress,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[1].m_iX + 1) * 3 + S2DPoint.dirs[1].m_iY + 1].m_iIsIngress,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[2].m_iX + 1) * 3 + S2DPoint.dirs[2].m_iY + 1].m_iIsIngress,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[7].m_iX + 1) * 3 + S2DPoint.dirs[7].m_iY + 1].m_iIsIngress,
                            currentBlock.m_fHeap,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[3].m_iX + 1) * 3 + S2DPoint.dirs[3].m_iY + 1].m_iIsIngress,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[6].m_iX + 1) * 3 + S2DPoint.dirs[6].m_iY + 1].m_iIsIngress,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[5].m_iX + 1) * 3 + S2DPoint.dirs[5].m_iY + 1].m_iIsIngress,
                            currentBlock.m_pChildGird[(S2DPoint.dirs[4].m_iX + 1) * 3 + S2DPoint.dirs[4].m_iY + 1].m_iIsIngress
                            ));
                        */
                    }
                }
            }
        }

        #endregion

        #region Mark G Calculated as close list

        for (int i = 0; i < 8; ++i)
        {
            COneSBlockGrid edge = currentBlock.m_pChildGird[(S2DPoint.dirs[i].m_iX + 1) * 3 + S2DPoint.dirs[i].m_iY + 1];
            if (m_iFindingIndex == edge.m_iGCalculated)
            {
                edge.m_iCloseList = m_iFindingIndex;
            }
        }

        #endregion

        #region neighbor blocks add to open-list

        //add current to close list
        currentBlock.m_iCloseList = m_iFindingIndex;

        //add neigbour to me
        for (int dir = 0; dir < 8; ++ dir)
        {
            //no fucking close list!
            if (null != currentBlock.m_pNeighbour[dir] /*&& m_iFindingIndex != currentBlock.m_pNeighbour[dir].m_iCloseList*/)
            {
                //CRuntimeLogger.Log("try neighbour: " + currentBlock.m_pNeighbour[dir].m_pChildGird[4].m_sWorldPos);
                UpdateHeap(currentBlock, currentBlock.m_pNeighbour[dir], (byte)dir);
            }
        }

        #endregion

    }

    private void BuildEndBlock(COneBBlockGrid endBlock, COneSBlockGrid end)
    {
        #region Calculate Endblock

        if (m_iFindingIndex == end.m_iIsIngress) //no need to build
        {
            return;
        }
        //middle
        if (1 == end.m_sLocalPos.m_iX && 1 == end.m_sLocalPos.m_iY)
        {
            float fLowestG = -1.0f;
            byte ingress = 0;
            for (byte i = 0; i < 8; ++i)
            {
                COneSBlockGrid edge = endBlock.m_pChildGird[(1 + S2DPoint.dirs[i].m_iX) * 3 + 1 + S2DPoint.dirs[i].m_iY];
                if (m_iFindingIndex == edge.m_iIsIngress)
                {
                    float fNewG = edge.m_GValue + ((0 == (i & 1)) ? GridMath.m_fDiagDist : GridMath.m_fStraightDist);
                    if (fLowestG < 0.0f || fNewG < fLowestG)
                    {
                        fLowestG = fNewG;
                        ingress = i;
                    }
                }
            }
            if (fLowestG > 0.0f)
            {
                end.m_iGCalculated = m_iFindingIndex;
                end.m_byEdge = ingress;
                end.m_GValue = fLowestG;
            }
            else
            {
                CRuntimeLogger.LogWarning("end block without ingress!");
            }
        }
        else
        {
            float fLowestG = -1.0f;
            byte ingress = 0;
            byte byWalk = 0;
            ushort ushWalk = 0;
            for (byte i = 0; i < 8; ++i)
            {
                COneSBlockGrid edge = endBlock.m_pChildGird[(1 + S2DPoint.dirs[i].m_iX) * 3 + 1 + S2DPoint.dirs[i].m_iY];
                if (m_iFindingIndex == edge.m_iIsIngress)
                {
                    int n = i > end.m_iEdgeIndex
                        ? (7 * end.m_iEdgeIndex + i - (end.m_iEdgeIndex * (end.m_iEdgeIndex + 1) / 2) - 1)
                        : (7 * i + end.m_iEdgeIndex - (i * (i + 1) / 2) - 1);

                    byte len = endBlock.m_byLen[n];
                    if (len > 0)
                    {
                        float fNewG = edge.m_GValue
                          + ((len >> 2) % 4) * GridMath.m_fStraightDist
                          + (len % 4) * GridMath.m_fDiagDist;
                        if (fLowestG < 0.0f || fNewG < fLowestG)
                        {
                            fLowestG = fNewG;
                            ingress = i;
                            byWalk = len;
                            ushWalk = endBlock.m_ushWalk[n];
                        }
                    }
                }
            }
            if (fLowestG > 0.0f)
            {
                end.m_iGCalculated = m_iFindingIndex;
                end.m_byEdge = ingress;
                end.m_GValue = fLowestG;
                end.m_byWalk = byWalk;
                end.m_shWalk = ushWalk;
            }
            else
            {
                //CRuntimeLogger.LogWarning("end block without ingress!");
            }
        }

        if (m_iFindingIndex == end.m_iGCalculated)
        {
            return;
        }

        #endregion

        #region Mark G Calculated as close list

        for (int i = 0; i < 8; ++i)
        {
            COneSBlockGrid edge = endBlock.m_pChildGird[(S2DPoint.dirs[i].m_iX + 1) * 3 + S2DPoint.dirs[i].m_iY + 1];
            if (!edge.m_bOccupied && m_iFindingIndex == edge.m_iGCalculated)
            {
                edge.m_iCloseList = m_iFindingIndex;
            }
        }

        #endregion

        #region Expand

        //add current to close list
        endBlock.m_iCloseList = m_iFindingIndex;

        //add neigbour to me
        for (int dir = 0; dir < 8; ++dir)
        {
            //no fucking close list!
            if (null != endBlock.m_pNeighbour[dir] /*&& m_iFindingIndex != currentBlock.m_pNeighbour[dir].m_iCloseList*/)
            {
                //CRuntimeLogger.Log("try neighbour: " + currentBlock.m_pNeighbour[dir].m_pChildGird[4].m_sWorldPos);
                UpdateHeap(endBlock, endBlock.m_pNeighbour[dir], (byte)dir);
            }
        }

        #endregion
    }

    /// <summary>
    /// Check the neighbours of current blocks, assign in-gress points. All open list operation is in here
    /// </summary>
    /// <param name="currentBlock"></param>
    /// <param name="nextBlock"></param>
    /// <param name="idir"></param>
    private void UpdateHeap(COneBBlockGrid currentBlock, COneBBlockGrid nextBlock, byte idir)
    {
        #region update egress

        bool bHeapChanged = false;
        bool bDiagonal = 0 == (1 & idir);
        int iInverseDir = (idir + 4) % 8;

        //for dir = diagonal, edge is inverse dir
        if (bDiagonal)
        {
            COneSBlockGrid ingress =
                currentBlock.m_pChildGird[(1 + S2DPoint.dirs[idir].m_iX) * 3 + 1 + S2DPoint.dirs[idir].m_iY];
            COneSBlockGrid egress =
                nextBlock.m_pChildGird[(1 + S2DPoint.dirs[iInverseDir].m_iX) * 3 + 1 + S2DPoint.dirs[iInverseDir].m_iY];
            if (m_iFindingIndex == ingress.m_iGCalculated)
            {
                float fNewG = ingress.m_GValue + GridMath.m_fDiagDist;
                if (!egress.m_bOccupied
                && fNewG > 0.0f
                && m_iFindingIndex != egress.m_iCloseList 
                && (m_iFindingIndex != egress.m_iGCalculated || fNewG < egress.m_GValue - 0.1f))
                {
                    egress.m_GValue = fNewG;
                    egress.m_iGCalculated = m_iFindingIndex;
                    egress.m_iIsIngress = m_iFindingIndex;
                    egress.m_byInGressDir = idir; //nextBlock = currentBlock.neibuour[idir]
                    egress.m_byEdge = idir;
                    bHeapChanged = true;
                }
            }
            else
            {
                if (!ingress.m_bOccupied) //maybe ingress is not reachable in "currentBlock"
                {
                    //CRuntimeLogger.LogWarning("diagonal expand, without ingress!");    
                }
            }
        }
        else
        {
            //for dir = straight, edge is inverse dir, inverse dir + 1, inverse dir - 1
            COneSBlockGrid egressmid =
                nextBlock.m_pChildGird[(1 + S2DPoint.dirs[iInverseDir].m_iX) * 3 + 1 + S2DPoint.dirs[iInverseDir].m_iY];
            COneSBlockGrid egressup =
                nextBlock.m_pChildGird[(1 + S2DPoint.dirs[iInverseDir - 1].m_iX) * 3 + 1 + S2DPoint.dirs[iInverseDir - 1].m_iY];
            COneSBlockGrid egressdown =
                nextBlock.m_pChildGird[
                   (1 + S2DPoint.dirs[7 == iInverseDir ? 0 : iInverseDir + 1].m_iX) * 3
                  + 1 + S2DPoint.dirs[7 == iInverseDir ? 0 : iInverseDir + 1].m_iY];

            COneSBlockGrid ingressmid =
                currentBlock.m_pChildGird[(1 + S2DPoint.dirs[idir].m_iX) * 3 + 1 + S2DPoint.dirs[idir].m_iY];
            COneSBlockGrid ingressdown =
                currentBlock.m_pChildGird[(1 + S2DPoint.dirs[idir - 1].m_iX) * 3 + 1 + S2DPoint.dirs[idir - 1].m_iY];
            COneSBlockGrid ingressup =
                currentBlock.m_pChildGird[
                    (1 + S2DPoint.dirs[7 == idir ? 0 : idir + 1].m_iX) * 3 + 1 + S2DPoint.dirs[7 == idir ? 0 : idir + 1].m_iY];

            #region up

            if (!egressup.m_bOccupied && m_iFindingIndex != egressup.m_iCloseList)
            {
                float flowestG = -1.0f;
                byte byedge = 0;
                //byte ingressdir = 0;
                if (m_iFindingIndex == ingressup.m_iGCalculated)
                {
                    flowestG = ingressup.m_GValue + GridMath.m_fStraightDist;
                    byedge = (byte)(7 == idir ? 0 : idir + 1); //edge index of ingressup
                    //ingressdir = idir; //real move dir
                }
                if (m_iFindingIndex == ingressmid.m_iGCalculated)
                {
                    float fNlowestG = ingressmid.m_GValue + GridMath.m_fDiagDist;
                    if (fNlowestG < flowestG || flowestG < 0.0f)
                    {
                        flowestG = fNlowestG;
                        byedge = idir; //edge index of ingressup
                        //ingressdir = (byte)(idir - 1); //real move dir
                    }
                }

                if (flowestG > 0.0f && (m_iFindingIndex != egressup.m_iGCalculated || flowestG < egressup.m_GValue - 0.1f))
                {
                    egressup.m_GValue = flowestG;
                    egressup.m_iGCalculated = m_iFindingIndex;
                    egressup.m_iIsIngress = m_iFindingIndex;
                    egressup.m_byEdge = byedge;
                    //egressup.m_byInGressDir = ingressdir;
                    egressup.m_byInGressDir = idir;
                    bHeapChanged = true;
                }
            }

            #endregion

            #region down

            if (!egressdown.m_bOccupied && m_iFindingIndex != egressdown.m_iCloseList)
            {
                float flowestG = -1.0f;
                //byte ingressdir = 0;
                byte byedge = 0;
                if (m_iFindingIndex == ingressdown.m_iGCalculated)
                {
                    flowestG = ingressdown.m_GValue + GridMath.m_fStraightDist;
                    byedge = (byte)(idir - 1);
                    //ingressdir = idir;
                }
                if (m_iFindingIndex == ingressmid.m_iGCalculated)
                {
                    float fNlowestG = ingressmid.m_GValue + GridMath.m_fDiagDist;
                    if (fNlowestG < flowestG || flowestG < 0.0f)
                    {
                        flowestG = fNlowestG;
                        byedge = idir;
                        //ingressdir = (byte)(7 == idir ? 0 : idir + 1);
                    }
                }

                if (flowestG > 0.0f && (m_iFindingIndex != egressdown.m_iGCalculated || flowestG < egressdown.m_GValue - 0.1f))
                {
                    egressdown.m_GValue = flowestG;
                    egressdown.m_iGCalculated = m_iFindingIndex;
                    egressdown.m_iIsIngress = m_iFindingIndex;
                    egressdown.m_byEdge = byedge;
                    //egressdown.m_byInGressDir = ingressdir;
                    egressdown.m_byInGressDir = idir;
                    bHeapChanged = true;
                }
            }

            #endregion

            #region middle

            if (!egressmid.m_bOccupied && m_iFindingIndex != egressmid.m_iCloseList)
            {
                float flowestG = -1.0f;
                //byte ingressdir = 0;
                byte byedge = 0;
                if (m_iFindingIndex == ingressmid.m_iGCalculated)
                {
                    flowestG = ingressmid.m_GValue + GridMath.m_fStraightDist;
                    //ingressdir = idir;
                    byedge = idir;
                }

                if (m_iFindingIndex == ingressup.m_iGCalculated)
                {
                    float fNlowestG = ingressup.m_GValue + GridMath.m_fDiagDist;
                    if (fNlowestG < flowestG || flowestG < 0.0f)
                    {
                        flowestG = fNlowestG;
                        byedge = (byte) (7 == idir ? 0 : idir + 1);
                        //ingressdir = (byte) (idir - 1);
                    }
                }
                if (m_iFindingIndex == ingressdown.m_iGCalculated)
                {
                    float fNlowestG = ingressdown.m_GValue + GridMath.m_fDiagDist;
                    if (fNlowestG < flowestG || flowestG < 0.0f)
                    {
                        flowestG = fNlowestG;
                        byedge = (byte)(idir - 1);
                        //ingressdir = (byte) (7 == idir ? 0 : idir + 1);
                    }
                }

                if (flowestG > 0.0f && (m_iFindingIndex != egressmid.m_iGCalculated || flowestG < egressmid.m_GValue - 0.1f))
                {
                    egressmid.m_GValue = flowestG;
                    egressmid.m_iGCalculated = m_iFindingIndex;
                    egressmid.m_iIsIngress = m_iFindingIndex;
                    //egressmid.m_byInGressDir = ingressdir;
                    egressmid.m_byInGressDir = idir;
                    egressmid.m_byEdge = byedge;
                    bHeapChanged = true;
                }
            }

            #endregion

            //CRuntimeLogger.Log("has change?" + bHeapChanged);
        }

        #endregion

        #region open list operation

        if (bHeapChanged)
        {
            float fMinHeap = -1.0f;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (m_iFindingIndex == nextBlock.m_pChildGird[i * 3 + j].m_iIsIngress
                     && m_iFindingIndex != nextBlock.m_pChildGird[i * 3 + j].m_iCloseList) //only check those not in a close list
                    {
                        if (fMinHeap < 0.0f || fMinHeap > nextBlock.m_pChildGird[i * 3 + j].m_GValue)
                        {
                            fMinHeap = nextBlock.m_pChildGird[i * 3 + j].m_GValue;
                        }
                    }
                }                
            }

            if (fMinHeap > 0.0f)
            {
                AddBigBlockToOpenlist(currentBlock, nextBlock, fMinHeap);
            }
        }

        #endregion
    }

    /// <summary>
    /// for heap value updated grid, add to open-list
    /// </summary>
    /// <param name="lowestF"></param>
    /// <param name="toAdd"></param>
    /// <param name="fNewHeap"></param>
    private void AddBigBlockToOpenlist(COneBBlockGrid lowestF, COneBBlockGrid toAdd, float fNewHeap)
    {
        bool bChanged = false;
        //update new heap
        if (m_iFindingIndex != toAdd.m_iHeapCalced || m_iFindingIndex == toAdd.m_iCloseList)
        {
            toAdd.m_fHeap = fNewHeap;
            toAdd.m_iHeapCalced = m_iFindingIndex;
            if (m_iFindingIndex == toAdd.m_iCloseList)
            {
                if (null != toAdd.m_pNext)
                {
                    toAdd.m_pNext.m_pPrev = toAdd.m_pPrev;
                }
                if (null != toAdd.m_pPrev)
                {
                    toAdd.m_pPrev.m_pNext = toAdd.m_pNext;
                }
                toAdd.m_pNext = null;
                toAdd.m_pPrev = null;
                toAdd.m_iCloseList = m_iFindingIndex - 1;
            }
            bChanged = true;
        }
        else
        {
            if (fNewHeap + 0.1f < toAdd.m_fHeap)
            {
                toAdd.m_fHeap = fNewHeap;
                toAdd.m_iHeapCalced = m_iFindingIndex;
                if (toAdd.m_fHeap + 0.1f < toAdd.m_pPrev.m_fHeap && toAdd.m_pPrev != lowestF)
                {
                    if (null != toAdd.m_pNext)
                    {
                        toAdd.m_pNext.m_pPrev = toAdd.m_pPrev;
                    }
                    if (null != toAdd.m_pPrev)
                    {
                        toAdd.m_pPrev.m_pNext = toAdd.m_pNext;
                    }
                    toAdd.m_pNext = null;
                    toAdd.m_pPrev = null;
                    bChanged = true;
                }
            }
        }

        if (bChanged)
        {
            //CRuntimeLogger.Log("<color=#FFFF00>open list add</color>" + toAdd.m_pChildGird[4].m_sWorldPos + " new heap:" + toAdd.m_fHeap);
            COneBBlockGrid pLast = lowestF;
            int prot = m_iAstarProtect;
            for (COneBBlockGrid node = lowestF.m_pNext; null != node; node = node.m_pNext)
            {
                --prot;
                if (prot < 0)
                {
                    CRuntimeLogger.LogError("endless loop!");
                    COneBBlockGrid logNode = lowestF.m_pNext;
                    for (int i = 0; i < 99; ++i)
                    {
                        CRuntimeLogger.Log(logNode.m_pChildGird[4].m_sWorldPos);
                        logNode = logNode.m_pNext;
                    }
                    return;
                }
                pLast = node;
                if (node.m_fHeap > toAdd.m_fHeap)
                {
                    node.m_pPrev.m_pNext = toAdd;
                    toAdd.m_pPrev = node.m_pPrev;
                    toAdd.m_pNext = node;
                    node.m_pPrev = toAdd;
                    pLast = null;
                    break;
                }
            }
            if (null != pLast)
            {
                pLast.m_pNext = toAdd;
                toAdd.m_pPrev = pLast;
                toAdd.m_pNext = null;
            }
        }
    }

    public Stack<Vector2> FindPathBlockAStar(Vector2 vStart, Vector2 vTarget, int iStep, out EPathRes eRes)
    {
        //find first block
        //expand it
        //while have lowest Heap block, expand it

        ++m_iFindingIndex;

        S2DPoint start = vStart;

        S2DPoint realStart = GetNearGrid(0, start.ToValid());
        byte island = m_byIslandData[realStart];
        S2DPoint end = vTarget;
        S2DPoint realEnd = GetNearGrid(island, end.ToValid());

        if (realStart == realEnd)
        {
            eRes = EPathRes.EPR_Done;
            _tmpRes.Clear();
            _tmpRes.Push(vTarget);
            if (end != realEnd)
            {
                _tmpRes.Push(realEnd.ToV2());
            }
            if (start != realStart)
            {
                _tmpRes.Push(realStart.ToV2());
            }
            _tmpRes.Push(vStart);
            return _tmpRes;
        }

        //check clean rectangle
        COneSBlockGrid pStartSNode = m_pSmallGrids[(realStart.m_iX + 1) * 66 + realStart.m_iY + 1];
        COneSBlockGrid pEndSNode = m_pSmallGrids[(realEnd.m_iX + 1) * 66 + realEnd.m_iY + 1];
        //CRuntimeLogger.Log(pEndSNode.m_bOccupied + ",pos:" + pEndSNode.m_sWorldPos + ",island:" + island);
        COneBBlockGrid pStartBNode = pStartSNode.m_pParent;
        COneBBlockGrid pEndBNode = pEndSNode.m_pParent;

        if (pStartBNode == pEndBNode)
        {
            eRes = EPathRes.EPR_Done;
            return null;
        }

        pStartBNode.m_pPrev = null;
        pStartBNode.m_pNext = null;
        ExpandBlock(pStartBNode, true, pStartSNode);

        int iProtect = m_iAstarProtect;

        COneBBlockGrid next = pStartBNode.m_pNext;
        if (null == next)
        {
            //CRuntimeLogger.LogError("No Way!");
            eRes = EPathRes.EPR_NoPath;
            return null;
        }
        if (pEndBNode == next)
        {
            BuildEndBlock(pEndBNode, pEndSNode);
            next = null;
        }

        while (null != next && iProtect >= 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                CRuntimeLogger.LogError("No Way!");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }

            if (pEndBNode == next)
            {
                BuildEndBlock(pEndBNode, pEndSNode);
            }
            else
            {
                ExpandBlock(next, false, pStartSNode);
            }
            if (m_iFindingIndex == pEndSNode.m_iGCalculated)
            {
                //CRuntimeLogger.Log("in here?" + pEndSNode.m_iGCalculated);
                break;
            }
            next = next.m_pNext;
            if (null == next)
            {
                //CRuntimeLogger.LogError("No Way!");
                eRes = EPathRes.EPR_NoPath;
                return null;
            }

        }

        #region Build Path

        _tmpRes.Clear();
        _tmpRes.Push(vTarget);

        iProtect = m_iAstarProtect;
        COneSBlockGrid nextt = pEndSNode;
        //CRuntimeLogger.Log(string.Format("Add path:{0}\n is Ingress:{1} byEdge:{2}, GCalc:{3}, GV:{4}", nextt.m_sWorldPos, nextt.m_iIsIngress, nextt.m_byEdge, nextt.m_iGCalculated, nextt.m_GValue));

        while (null != nextt && iProtect > 0)
        {
            --iProtect;
            if (iProtect <= 0)
            {
                CRuntimeLogger.LogWarning("build the path with wrong data");
                eRes = EPathRes.EPR_NoPath;
                return _tmpRes;
            }

            if (nextt.m_pParent == pStartBNode 
            && nextt != pStartSNode
            && (1 == pStartSNode.m_sLocalPos.m_iX && 1 == pStartSNode.m_sLocalPos.m_iY)
                )
            {
                _tmpRes.Push(nextt.m_sWorldPos.ToV2()); //add self
                nextt = pStartSNode;
            }

            if (nextt == pStartSNode)
            {
                if (realStart != start)
                {
                    _tmpRes.Push(realStart.ToV2());
                }
                _tmpRes.Push(vStart);
                eRes = EPathRes.EPR_Done;
                return _tmpRes;
            }

            if (nextt == pEndSNode && 1 == pEndSNode.m_sLocalPos.m_iX && 1 == pEndSNode.m_sLocalPos.m_iY)
            {
                if (realEnd != end)
                {
                    _tmpRes.Push(realEnd.ToV2());
                }

                nextt =
                    nextt.m_pParent.m_pChildGird[
                        (1 + S2DPoint.dirs[nextt.m_byEdge].m_iX)*3 
                       + 1 + S2DPoint.dirs[nextt.m_byEdge].m_iY];

                continue;
            }

            #region normal point

            _tmpRes.Push(nextt.m_sWorldPos.ToV2()); //add self
            //CRuntimeLogger.Log(string.Format("Add path:{0}\n is Ingress:{1} byEdge:{2}, GCalc:{3}, GV:{4}, egindex:{5}", nextt.m_sWorldPos, nextt.m_iIsIngress, nextt.m_byEdge, nextt.m_iGCalculated, nextt.m_GValue, nextt.m_iEdgeIndex));
            if (m_iFindingIndex != nextt.m_iIsIngress) //not ingress, go to ingress of this Big block
            {
                ushort shmove = nextt.m_shWalk;
                int walkstep = nextt.m_byWalk >> 4;
                S2DPoint movestart = nextt.m_sWorldPos;
                //if (walkstep > 4)
                //{
                //    CRuntimeLogger.LogWarning("not possible!" + nextt.m_byWalk);
                //}
                if (nextt.m_iEdgeIndex > nextt.m_byEdge)
                {
                    for (int i = 0; i < walkstep - 1; ++i)
                    //the first move is not need, because the ingress will add itself
                    {
                        int nextdir = shmove % 8;
                        shmove = (ushort)(shmove >> 3);
                        movestart = new S2DPoint(
                            movestart.m_iX + S2DPoint.dirs[nextdir].m_iX,
                            movestart.m_iY + S2DPoint.dirs[nextdir].m_iY);
                        //CRuntimeLogger.Log(string.Format("Add path:{0}\n step:{1} len:{2}, walk:{3}, times:{4}, dir:{5}", movestart, i, nextt.m_byWalk, nextt.m_shWalk, 1, nextdir));
                        _tmpRes.Push(movestart.ToV2());
                    }
                }
                else
                {
                    for (int i = 0; i < walkstep - 1; ++i)
                    //the first move is not need, because the ingress will add itself
                    {
                        int istartbit = 1 << (3 * (walkstep - i));
                        int iendbit = 3*(walkstep - i - 1);
                        int nextdir = (shmove % istartbit) >> iendbit;
                        movestart = new S2DPoint(
                            movestart.m_iX - S2DPoint.dirs[nextdir].m_iX,
                            movestart.m_iY - S2DPoint.dirs[nextdir].m_iY);
                        //CRuntimeLogger.Log(string.Format("Add path:{0}\n step:{1} len:{2}, walk:{3}, times:{4}, dir:{5}", movestart, i, nextt.m_byWalk, nextt.m_shWalk, -1, nextdir));
                        _tmpRes.Push(movestart.ToV2());
                    }
                }

                nextt =
                    nextt.m_pParent.m_pChildGird[
                        (1 + S2DPoint.dirs[nextt.m_byEdge].m_iX) * 3
                       + 1 + S2DPoint.dirs[nextt.m_byEdge].m_iY];
            }
            else //ingress, move to next big block
            {
                COneBBlockGrid nextBBlock = nextt.m_pParent.m_pNeighbour[(nextt.m_byInGressDir + 4) % 8];

                if (null == nextBBlock)
                {
                    CRuntimeLogger.LogWarning("wrong ingress!");
                    eRes = EPathRes.EPR_NoPath;
                    return null;
                }

                nextt =
                    nextBBlock.m_pChildGird[
                        (1 + S2DPoint.dirs[nextt.m_byEdge].m_iX)*3
                       + 1 + S2DPoint.dirs[nextt.m_byEdge].m_iY];
            }

            #endregion
        }

        #endregion

        eRes = EPathRes.EPR_NoPath;
        return null;

    }

    #endregion
#endif

    #region Agent

    [HideInInspector]
    public List<ANavAgent> m_pAgents = null;
    [HideInInspector]
    public List<ANavAgent> m_pActiveAgents = null;

    public void ClearAgents()
    {
        m_pAgents = new List<ANavAgent>();
        m_pActiveAgents = new List<ANavAgent>();
        m_iAgentID = 0;
    }

    protected int m_iAgentID = 0;

    public void RegisterAgent(ANavAgent pAgent)
    {
        if (null == m_pAgents)
        {
            m_pAgents = new List<ANavAgent>();
        }
        if (null == m_pActiveAgents)
        {
            m_pActiveAgents = new List<ANavAgent>();
        }
        if (null == pAgent || null == m_pAgents || null == m_pActiveAgents)
        {
            CRuntimeLogger.LogWarning(string.Format("Add Nav Agent {0} to {1} and {2} not work", pAgent, m_pAgents, m_pActiveAgents));
            return;
        }
        if (m_pAgents.Contains(pAgent))
        {
            if (!m_pActiveAgents.Contains(pAgent))
            {
                m_pActiveAgents.Add(pAgent);
            }
            return;
        }
        //CRuntimeLogger.LogWarning("!!!!!!!!!!!!!!!!!! Regist" + pAgent.name);
        pAgent.m_pOwner = this;
        pAgent.m_iID = m_iAgentID;
        ++m_iAgentID;
        m_pAgents.Add(pAgent);
        m_pActiveAgents.Add(pAgent);        
    }

    public void UnRegisterAgent(ANavAgent pAgent)
    {
        if (null == m_pAgents || null == m_pActiveAgents)
        {
            return;
        }

        int iIndex = m_pAgents.IndexOf(pAgent);
        if (iIndex >= 0)
        {
            m_pAgents.RemoveAt(iIndex);
        }
        m_pActiveAgents.Remove(pAgent);        
    }

    public void ActivateAgent(ANavAgent agent)
    {
        if (!m_pActiveAgents.Contains(agent))
        {
            m_pActiveAgents.Add(agent);
        }
    }

    public void DeActivateAgent(ANavAgent agent)
    {
        m_pActiveAgents.Remove(agent);
    }

    /// <summary>
    /// push away active agents
    /// </summary>
    private void ProcessAgentAvoidence()
    {
        int iCheckingCount = m_pActiveAgents.Count;
        for (int i = 0; i < iCheckingCount - 1; ++i)
        {
            ANavAgent pAgent1 = m_pActiveAgents[i];
            int iPiror1 = pAgent1.m_iRealAvoidenceLevel * iCheckingCount + pAgent1.m_iID;
            int iCamp1 = pAgent1.m_iCamp;
            byte iPiror3 = pAgent1.m_byAvoidenceLevel;
            for (int j = i + 1; j < iCheckingCount; ++j)
            {
                ANavAgent pAgent2 = m_pActiveAgents[j];
                if (pAgent1.m_uiChannel == pAgent2.m_uiChannel)
                {
                    Vector2 v12 = pAgent1.m_vGroundPos - pAgent2.m_vGroundPos;

                    //AABB Test
                    if (
                        Mathf.Abs(v12.x) < pAgent1.m_fRadius + pAgent2.m_fRadius 
                     && Mathf.Abs(v12.y) < pAgent1.m_fRadius + pAgent2.m_fRadius)
                    {
                        float fRate = v12.magnitude;
                        if (fRate <= 0.0001f)
                        {
                            v12 = pAgent1.m_vGroundPos - pAgent2.m_vGroundPos + new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                            fRate = v12.magnitude;
                        }
                        fRate = (pAgent1.m_fRadius + pAgent2.m_fRadius - fRate) 
                              * (pAgent1.m_fRadius + pAgent2.m_fRadius) * m_fPushAwayRate;
                        int iPiror2 = pAgent2.m_iRealAvoidenceLevel * iCheckingCount + pAgent2.m_iID;
                        byte iPiror4 = pAgent2.m_byAvoidenceLevel;
                        bool bIsBuilding1 = pAgent1.m_bBuilding;
                        bool bIsBuilding2 = pAgent1.m_bBuilding;
                        if (fRate > 0.0f)
                        {
                            if (iCamp1 == pAgent2.m_iCamp)
                            {
                                if (iPiror1 > iPiror2)
                                {
                                    pAgent2.m_vForce -= fRate * v12.normalized * (bIsBuilding1 ? m_fPushAwayRateBuilding : m_fPushAwayRatePawn);
                                }
                                else
                                {
                                    pAgent1.m_vForce += fRate * v12.normalized * (bIsBuilding2 ? m_fPushAwayRateBuilding : m_fPushAwayRatePawn);
                                }
                            }
                            else
                            {
                                if (iPiror3 > iPiror4)
                                {
                                    pAgent2.m_vForce -= fRate * v12.normalized * (bIsBuilding1 ? m_fPushAwayRateBuilding : m_fPushAwayRatePawn);
                                }
                                else if (iPiror3 < iPiror4)
                                {
                                    pAgent1.m_vForce += fRate * v12.normalized * (bIsBuilding2 ? m_fPushAwayRateBuilding : m_fPushAwayRatePawn);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //TODO start from nearest
    //TODO distinguish air and ground
    //TODO make this function S2DPoint
    /// <summary>
    /// return a shoot pos
    /// </summary>
    /// <param name="vTarget"></param>
    /// <param name="fMaxDist"></param>
    /// <param name="fMinDist"></param>
    /// <param name="channel"></param>
    /// <param name="bFound"></param>
    /// <returns></returns>
    public Vector2 GetShootPos(Vector2 vTarget, float fMaxDist, float fMinDist, uint channel, out bool bFound)
    {
        bFound = false;
        short minRadius = (short)Mathf.CeilToInt(fMinDist);
        short maxRadius = (short)Mathf.CeilToInt(fMaxDist);
        S2DPoint start = ((S2DPoint)vTarget).ToValid();
        for (short ii = minRadius; ii < maxRadius; ++ii)
        {
            int maxk = S2DPoint.GetRadiusNumber(ii);
            for (int kk = 0; kk < maxk; ++kk)
            {
                S2DPoint pt = S2DPoint.GetRadiusPoint(start, ii, kk);
                if (pt.m_bValid && GridMath.m_byWalkable == m_byGridStateData[pt])
                {
                    bFound = true;
                    return pt.ToV2();
                }
            }
        }
        return vTarget;
    }

    public Vector3 GetDefaultPos()
    {
        return transform.position;
    }

    public bool IsWalkable(Vector2 pos, uint uiChannel)
    {
        S2DPoint pt = pos;
        if (!pt.m_bValid)
        {
            return false;
        }

        pt.ToValid();
        return m_pGrids[pt.m_iX, pt.m_iY].m_bOccupied;
    }

    public Vector2 GetNearPos(Vector2 pos)
    {
        S2DPoint gridpos = pos;
        return GetNearGrid(0, gridpos).ToV2();
    }

    /// <summary>
    /// return the height of a position on grids
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public float GetHeight(Vector2 pos)
    {
        //go to valid pos at first
        S2DPoint gd1 = pos;
        gd1 = GetNearGrid(0, gd1);
        Vector2 gridCenter = gd1.ToV2();

        
        int iDeltaX = pos.x > gridCenter.x ? 1 : -1;
        int iDeltaY = pos.y > gridCenter.y ? 1 : -1;

        S2DPoint gd2 = new S2DPoint(gd1.m_iX + iDeltaX, gd1.m_iY).ToValid();
        S2DPoint gd3 = new S2DPoint(gd1.m_iX, gd1.m_iY + iDeltaY).ToValid();
        S2DPoint gd4 = new S2DPoint(gd1.m_iX + iDeltaX, gd1.m_iY + iDeltaY).ToValid();

        float ht1 = m_fGridHeightData[gd1];
        float ht2 = m_fGridHeightData[gd2];
        float ht3 = m_fGridHeightData[gd3];
        float ht4 = m_fGridHeightData[gd4];

        float fDist1 = (pos - gd1.ToV2()).sqrMagnitude + 0.001f;
        float fDist2 = (pos - gd2.ToV2()).sqrMagnitude + 0.001f;
        float fDist3 = (pos - gd3.ToV2()).sqrMagnitude + 0.001f;
        float fDist4 = (pos - gd4.ToV2()).sqrMagnitude + 0.001f;

        return (ht1 * fDist1 / (fDist1 + fDist2) + ht2 * fDist2 / (fDist1 + fDist2)) * fDist1 / (fDist1 + fDist3)
             + (ht3 * fDist3 / (fDist3 + fDist4) + ht4 * fDist4 / (fDist3 + fDist4)) * fDist3 / (fDist1 + fDist3);
    }

    /// <summary>
    /// return the normal of position on grids
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 GetNormal(Vector2 pos)
    {
        S2DPoint gd1 = pos;
        gd1 = GetNearGrid(0, gd1);
        Vector2 gridCenter = gd1.ToV2();


        int iDeltaX = pos.x > gridCenter.x ? 1 : -1;
        int iDeltaY = pos.y > gridCenter.y ? 1 : -1;

        S2DPoint gd2 = new S2DPoint(gd1.m_iX + iDeltaX, gd1.m_iY).ToValid();
        S2DPoint gd3 = new S2DPoint(gd1.m_iX, gd1.m_iY + iDeltaY).ToValid();
        S2DPoint gd4 = new S2DPoint(gd1.m_iX + iDeltaX, gd1.m_iY + iDeltaY).ToValid();

        Vector3 ht1 = m_v3NormalData[gd1];
        Vector3 ht2 = m_v3NormalData[gd2];
        Vector3 ht3 = m_v3NormalData[gd3];
        Vector3 ht4 = m_v3NormalData[gd4];

        float fDist1 = (pos - gd1.ToV2()).sqrMagnitude + 0.001f;
        float fDist2 = (pos - gd2.ToV2()).sqrMagnitude + 0.001f;
        float fDist3 = (pos - gd3.ToV2()).sqrMagnitude + 0.001f;
        float fDist4 = (pos - gd4.ToV2()).sqrMagnitude + 0.001f;

        return (ht1 * fDist1 / (fDist1 + fDist2) + ht2 * fDist2 / (fDist1 + fDist2)) * fDist1 / (fDist1 + fDist3)
             + (ht3 * fDist3 / (fDist3 + fDist4) + ht4 * fDist4 / (fDist3 + fDist4)) * fDist3 / (fDist1 + fDist3);
    }

    #endregion

}
