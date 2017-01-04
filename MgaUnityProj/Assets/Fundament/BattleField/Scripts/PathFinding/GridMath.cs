using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// This is a int type point
/// </summary>
public struct S2DPoint
{
    public readonly short m_iX;
    public readonly short m_iY;
    public readonly bool m_bValid;
    public readonly short m_iIndex;

    public const short maxindex = SceneConstant.m_iSceneSize * SceneConstant.m_iSceneSize;

    public S2DPoint(short iX, short iY)
    {
        m_iX = iX;
        m_iY = iY;
        m_bValid = m_iX >= 0 && m_iX < SceneConstant.m_iSceneSize && m_iY >= 0 && m_iY < SceneConstant.m_iSceneSize;
        m_iIndex = (short)(m_iX*SceneConstant.m_iSceneSize + m_iY);
    }

    public S2DPoint(int iX, int iY)
    {
        m_iX = (short)iX;
        m_iY = (short)iY;
        m_bValid = m_iX >= 0 && m_iX < SceneConstant.m_iSceneSize && m_iY >= 0 && m_iY < SceneConstant.m_iSceneSize;
        m_iIndex = (short)(m_iX * SceneConstant.m_iSceneSize + m_iY);
    }

    public S2DPoint ToValid()
    {
        return new S2DPoint(
            (short)Mathf.Clamp(m_iX, 0, SceneConstant.m_iSceneSize - 1),
            (short)Mathf.Clamp(m_iY, 0, SceneConstant.m_iSceneSize - 1)
            );
    }

    public Vector2 ToV2()
    {
        return new Vector2(m_iX - SceneConstant.m_iSceneSize / 2 + 0.5f, m_iY - SceneConstant.m_iSceneSize / 2 + 0.5f);
    }

    public static S2DPoint invalid { get { return new S2DPoint(-1, -1); } }

    public static S2DPoint random
    {
        get
        {
            return new S2DPoint(
                Random.Range(0, SceneConstant.m_iSceneSize),
                Random.Range(0, SceneConstant.m_iSceneSize));
        }
    }

    public static readonly S2DPoint[] neighbor = {
                                new S2DPoint(-1, 0),
                                new S2DPoint(1, 0),
                                new S2DPoint(0, -1),
                                new S2DPoint(0, 1),
                                new S2DPoint(-1, -1),
                                new S2DPoint(1, -1),
                                new S2DPoint(-1, 1),
                                new S2DPoint(1, 1),
                            };

    public static int GetRadiusNumber(int radius)
    {
        return radius*8;
    }

    public static S2DPoint GetRadiusPoint(S2DPoint center, int radius, int index)
    {
        if (index < 4)
        {
            switch (index)
            {
                case 0:
                    return new S2DPoint(center.m_iX, center.m_iY - radius);
                case 1:
                    return new S2DPoint(center.m_iX, center.m_iY + radius);
                case 2:
                    return new S2DPoint(center.m_iX - radius, center.m_iY);
                case 3:
                    return new S2DPoint(center.m_iX + radius, center.m_iY);
            }
        }

        int iRealIndex = ((index - 4) / 8) + 1;
        switch ((index - 4) % 8)
        {
            case 0:
                return new S2DPoint(center.m_iX + iRealIndex, center.m_iY - radius);
            case 1:
                return new S2DPoint(center.m_iX - iRealIndex, center.m_iY + radius);
            case 2:
                return new S2DPoint(center.m_iX - iRealIndex, center.m_iY - radius);
            case 3:
                return new S2DPoint(center.m_iX + iRealIndex, center.m_iY + radius);
            case 4:
                return new S2DPoint(center.m_iX - radius, center.m_iY + iRealIndex);
            case 5:
                return new S2DPoint(center.m_iX + radius, center.m_iY - iRealIndex);
            case 6:
                return new S2DPoint(center.m_iX - radius, center.m_iY - iRealIndex);
            case 7:
                return new S2DPoint(center.m_iX + radius, center.m_iY + iRealIndex);
        }

        return invalid;
    }

    //clock order
    public static readonly S2DPoint[] dirs =
    {
        new S2DPoint(-1, -1), //diagonal
        new S2DPoint(0, -1),
        new S2DPoint(1, -1),  //diagonal
        new S2DPoint(1, 0),
        new S2DPoint(1, 1),   //diagonal
        new S2DPoint(0, 1),
        new S2DPoint(-1, 1),  //diagonal
        new S2DPoint(-1, 0),
    };

    public static implicit operator short(S2DPoint c)
    {
        return c.m_iIndex;
    }

    public static implicit operator S2DPoint(short v)
    {
        return new S2DPoint((short)(v / SceneConstant.m_iSceneSize), (short)(v % SceneConstant.m_iSceneSize));
    }

    public static implicit operator S2DPoint(int v)
    {
        return new S2DPoint((short)(v / SceneConstant.m_iSceneSize), (short)(v % SceneConstant.m_iSceneSize));
    }

    public static implicit operator S2DPoint(Vector2 vPos)
    {
        return new S2DPoint
        (
            (short)Mathf.RoundToInt(vPos.x + (SceneConstant.m_iSceneSize / 2.0f) - 0.5f),
            (short)Mathf.RoundToInt(vPos.y + (SceneConstant.m_iSceneSize / 2.0f) - 0.5f)
        );
    }

    public static implicit operator S2DPoint(Vector3 vPos)
    {
        return new S2DPoint
        (
            (short)Mathf.RoundToInt(vPos.x + (SceneConstant.m_iSceneSize / 2.0f) - 0.5f),
            (short)Mathf.RoundToInt(vPos.z + (SceneConstant.m_iSceneSize / 2.0f) - 0.5f)
        );
    }

    public static bool operator ==(S2DPoint lhs, S2DPoint rhs)
    {
        return lhs.m_iIndex == rhs.m_iIndex;
    }

    public static bool operator !=(S2DPoint lhs, S2DPoint rhs)
    {
        return lhs.m_iIndex != rhs.m_iIndex;
    }

    #region Multiply

    public static S2DPoint Step(S2DPoint pt, short step)
    {
        return new S2DPoint(pt.m_iX * step, pt.m_iY * step);
    }

    public static S2DPoint operator *(S2DPoint rhs, byte lhs)
    {
        return Step(rhs, lhs);
    }

    public static S2DPoint operator *(byte lhs, S2DPoint rhs)
    {
        return Step(rhs, lhs);
    }

    public static S2DPoint operator *(S2DPoint rhs, int lhs)
    {
        return Step(rhs, (short)lhs);
    }

    public static S2DPoint operator *(int lhs, S2DPoint rhs)
    {
        return Step(rhs, (short)lhs);
    }

    public static S2DPoint operator *(S2DPoint rhs, uint lhs)
    {
        return Step(rhs, (short)lhs);
    }

    public static S2DPoint operator *(uint lhs, S2DPoint rhs)
    {
        return Step(rhs, (short)lhs);
    }

    public static S2DPoint operator *(S2DPoint rhs, short lhs)
    {
        return Step(rhs, lhs);
    }

    public static S2DPoint operator *(short lhs, S2DPoint rhs)
    {
        return Step(rhs, lhs);
    }

    public static S2DPoint operator *(S2DPoint rhs, ushort lhs)
    {
        return Step(rhs, (short)lhs);
    }

    public static S2DPoint operator *(ushort lhs, S2DPoint rhs)
    {
        return Step(rhs, (short)lhs);
    }

    #endregion

    public bool Equals(S2DPoint other)
    {
        return m_iIndex == other.m_iIndex;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        return obj is S2DPoint && Equals((S2DPoint)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (m_iX * 397) ^ m_iY;
        }
    }

    public override string ToString()
    {
        return string.Format("({0},{1})", m_iX, m_iY);
    }
}

/// <summary>
/// This is a int type rect
/// </summary>
public struct SRectangleData
{
    public S2DPoint m_sPLT;
    public S2DPoint m_sPRB;

    public bool IsInRect(S2DPoint point)
    {
        return point.m_iX >= m_sPLT.m_iX
            && point.m_iX <= m_sPRB.m_iX
            && point.m_iY >= m_sPLT.m_iY
            && point.m_iY <= m_sPRB.m_iY;
    }

    public bool IsInRect(S2DPoint a, S2DPoint b)
    {
        return a.m_iX >= m_sPLT.m_iX
            && a.m_iX <= m_sPRB.m_iX
            && a.m_iY >= m_sPLT.m_iY
            && a.m_iY <= m_sPRB.m_iY
            && b.m_iX >= m_sPLT.m_iX
            && b.m_iX <= m_sPRB.m_iX
            && b.m_iY >= m_sPLT.m_iY
            && b.m_iY <= m_sPRB.m_iY;
    }

    public int Area()
    {
        return (m_sPRB.m_iX - m_sPLT.m_iX) * (m_sPRB.m_iY - m_sPLT.m_iY);
    }

    public bool IsValid()
    {
        return m_sPLT.m_bValid && m_sPRB.m_bValid && (m_sPRB.m_iX > m_sPLT.m_iX) && (m_sPRB.m_iY > m_sPLT.m_iY);
    }

    public static implicit operator int(SRectangleData c)
    {
        return (short)c.m_sPLT * 32768 + (short)c.m_sPRB;
    }

    public static implicit operator SRectangleData(int v)
    {
        return new SRectangleData
        {
            m_sPLT = (short)(v / 32768),
            m_sPRB = (short)(v % 32768),
        };
    }

    public static SRectangleData invalid { get { return new SRectangleData { m_sPLT = S2DPoint.invalid, m_sPRB = S2DPoint.invalid }; } }
}

/// <summary>
/// This is a group for all frequently used math on grids
/// </summary>
public static class GridMath
{
    public const byte m_byWalkable = 0;

    /// <summary>
    /// occupy is add as bit
    /// </summary>
    public const byte m_byNoWalkable = 1;

    private const int m_iAstarProtect = 4096;
    public const float m_fStraightDist = 1.0f;
    public const float m_fDiagDist = 1.414f;

    /// <summary>
    /// use only in AStar, to avoid new
    /// </summary>
    private static readonly List<S2DPoint> m_opList = new List<S2DPoint>();

    /// <summary>
    /// use anywhere, to avoid new
    /// </summary>
    private static readonly List<S2DPoint> m_tmpNewAddNode = new List<S2DPoint>();
    private static readonly List<S2DPoint> m_res = new List<S2DPoint>();


    /// <summary>
    /// Those are used in AStar path findings
    /// _tmpOpList and _tmpClsList will grow with m_iAStarIndex, simply use == m_iAStarIndex will decide whether is true or not
    /// _tmpParentList, _tmpFVList and _tmpDistToParentList, whenever a node is added into op-list, it will be set.
    /// So it is not need to clear them
    /// </summary>
    private static int m_iAStarIndex = 1;
    private static readonly int[] _tmpOpList = new int[SceneConstant.m_iSceneSizeSq];
    private static readonly int[] _tmpClsList = new int[SceneConstant.m_iSceneSizeSq];
    private static readonly int[] _tmpParentList = new int[SceneConstant.m_iSceneSizeSq];
    private static readonly float[] _tmpFVList = new float[SceneConstant.m_iSceneSizeSq];
    private static readonly float[] _tmpDistToParentList = new float[SceneConstant.m_iSceneSizeSq];
    /// <summary>
    /// Using the input gridsState to find a path, not using any H-functions. Should only use in Editor.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="gridsState"></param>
    /// <param name="length">length of the path, -1 if path not exist</param>
    /// <returns>path</returns>
    static public S2DPoint[] StrictAStar(S2DPoint start, S2DPoint end, byte[] gridsState, out float length)
    {
        if (start == end)
        {
            length = 0.0f;
            return new[] { start };
        }

        if (m_byWalkable != gridsState[start])
        {
            length = -1.0f;
            //CRuntimeLogger.LogWarning("Start is not a walkable");
            return null;
        }
        ++m_iAStarIndex;

        _tmpOpList[start] = m_iAStarIndex;
        _tmpParentList[start] = 0;
        _tmpFVList[start] = 0.0f;
        _tmpDistToParentList[start] = 0.0f;

        m_opList.Clear();
        m_opList.Add(start);

        int iOpenListStart = 0;
        int iProtect = m_iAstarProtect;

        while (m_opList.Count > iOpenListStart && iProtect > 0)
        {
            --iProtect;
            if (iProtect < 0)
            {
                CRuntimeLogger.LogError("No Way!");
                length = -1.0f;
                return null;
            }
            //find lowest f in open list
            //lowest f is always the first one
            S2DPoint pLowestF = m_opList[iOpenListStart];

            //If in close list, skip it
            if (m_iAStarIndex == _tmpClsList[pLowestF])
            {
                ++iOpenListStart;
                continue;
            }

            if (m_byWalkable != gridsState[pLowestF])
            {
                CRuntimeLogger.LogError("No Way!");
                length = -1.0f;
                return null;
            }

            //Add to close list
            _tmpClsList[pLowestF] = m_iAStarIndex;
            if (pLowestF == end)
            {
                break;
            }

            //Open List is sorted, just add one index for next loop
            ++iOpenListStart;
            
            m_tmpNewAddNode.Clear();

            #region Connect Points

            //check connected points
            for (short i = -1; i < 2; ++i)
            {
                for (short j = -1; j < 2; ++j)
                {
                    if (i != 0 || j != 0)
                    {
                        S2DPoint connect = (i + pLowestF.m_iX) * SceneConstant.m_iSceneSize + (j + pLowestF.m_iY);
                        if (connect.m_bValid && m_byWalkable == gridsState[connect] && _tmpClsList[connect] != m_iAStarIndex)
                        {

                            if (_tmpOpList[connect] != m_iAStarIndex)
                            {
                                _tmpParentList[connect] = pLowestF + 1;
                                _tmpOpList[connect] = m_iAStarIndex;
                                float fDist = (connect.m_iX == pLowestF.m_iX || connect.m_iY == pLowestF.m_iY)
                                    ? m_fStraightDist
                                    : m_fDiagDist;

                                _tmpDistToParentList[connect] = fDist;
                                _tmpFVList[connect] = _tmpFVList[pLowestF] + fDist;

                                m_tmpNewAddNode.Add(connect);
                            }
                            else
                            {
                                float fNewDist = (connect.m_iX == pLowestF.m_iX || connect.m_iY == pLowestF.m_iY)
                                    ? m_fStraightDist
                                    : m_fDiagDist;
                                float newF = _tmpFVList[pLowestF] + fNewDist;

                                
                                //Check F Value
                                if (newF < _tmpFVList[connect])
                                {
                                    _tmpFVList[connect] = newF;
                                    _tmpDistToParentList[connect] = fNewDist;
                                    _tmpParentList[connect] = pLowestF + 1;
                                    m_tmpNewAddNode.Add(connect);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region New Lowest F, Resort Order

            for (int i = 0; i < m_tmpNewAddNode.Count; ++i)
            {
                int iInsertPos = -1;
                for (int j = iOpenListStart; j < m_opList.Count; ++j)
                {
                    if (_tmpFVList[m_tmpNewAddNode[i]] < _tmpFVList[m_opList[j]])
                    {
                        iInsertPos = j;
                        break;
                    }
                }

                // the connect point may be add into an open list twice (or even more)
                // however it dose not harm, because the first place is the lowestF 
                // after lowest F, it is in close list, and will be skipped anywhere
                if (-1 == iInsertPos)
                {
                    m_opList.Add(m_tmpNewAddNode[i]);
                }
                else
                {
                    m_opList.Insert(iInsertPos, m_tmpNewAddNode[i]);
                }
            }

            #endregion
        }

        if (_tmpParentList[end] > 0)
        {
            m_res.Clear();
            m_res.Add(end);
            length = 0.0f;
            S2DPoint pLastParent = end;
            int iProtected2 = m_iAstarProtect;
            while (_tmpParentList[pLastParent] > 0 && iProtected2 > 0)
            {
                --iProtected2;
                if (iProtected2 < 0)
                {
                    length = -1.0f;
                    CRuntimeLogger.LogError("what?");
                    return null;
                }

                S2DPoint parent = _tmpParentList[pLastParent] - 1;
                m_res.Insert(0, parent);
                length += _tmpDistToParentList[pLastParent];;
                pLastParent = parent;
            }
            return m_res.ToArray();
        }

        length = -1.0f;
        //CRuntimeLogger.LogWarning(string.Format("End does not have a parent. protect: {0}, oplist: {1}, opstart: {2}", iProtect, m_opList.Count, iOpenListStart));
        return null;
    }

    /// <summary>
    /// set obscale to 1, start point to 2.
    /// </summary>
    /// <param name="gridsState"></param>
    /// <param name="width"></param>
    /// <param name="iheight"></param>
    /// <returns>painted is marked as 2</returns>
    static public bool ExpandPaint(ref byte[,] gridsState, int width, int iheight)
    {
        bool bHasNew = false;
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < iheight; ++j)
            {
                if (2 == gridsState[i, j])
                {
                    //8-channle paint
                    for (int kk = -1; kk < 2; ++kk)
                    {
                        for (int ll = -1; ll < 2; ++ll)
                        {
                            if (i + kk >= 0 && i + kk < width
                             && j + ll >= 0 && j + ll < iheight
                             && 0 == gridsState[i + kk, j + ll])
                            {
                                bHasNew = true;
                                gridsState[i + kk, j + ll] = 2;
                            }
                        }
                    }
                }
            }            
        }

        if (!bHasNew)
        {
            return false;
        }

        return ExpandPaint(ref gridsState, width, iheight);
    }
}
