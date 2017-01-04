using UnityEngine;
using System.Collections;

public enum EPathProgress
{
    EPP_None = 0,
    EPP_Done,
    EPP_GiveUp,
    EPP_Attacking,
    EPP_Walking,
    EPP_Hold,
    EPP_Building,
}

public enum EAgentChannel
{
    EAT_Ground      = 0x00000001,
    EAT_Fly         = 0x00000002,
    //others
}

public struct SMoveTask
{
    public EPathProgress m_eProgress;
    /*
    private EPathProgress _eProgress;
    public EPathProgress m_eProgress
    {
        get { return _eProgress; }
        set
        {
            _eProgress = value;
            if (EPathProgress.EPP_Walking != value && EPathProgress.EPP_Attacking != value && null != m_pOwner.m_pUnit && ETeamStateChange.ETSC_AI_MoveTo == m_pOwner.m_pUnit.GetAIUnit().AIU_GetAIState())
            {
                CRuntimeLogger.LogWarning("============= what? :" + value + " and target: " + m_vPathFound.Length);
                for (int i = 0; i < m_vPathFound.Length; ++i)
                {
                    CRuntimeLogger.LogWarning("====pos:" + i + " is: " + m_vPathFound[i]);    
                }
            }
        }
    }
    */
    public int m_iStep;
    public Vector3 m_vTargetPoint;

    public Vector2[] m_vPathFound;
    /*
    private Vector2[] _vPathFound;
    public Vector2[] m_vPathFound
    {
        get { return _vPathFound; }
        set
        {
            _vPathFound = value;
            if (2 == _vPathFound.Length && (_vPathFound[0] - _vPathFound[1]).sqrMagnitude < 10.0f && (_vPathFound[0] - _vPathFound[1]).sqrMagnitude > 1.0f)
            {
                CRuntimeLogger.LogWarning("============ why find such a path?");
            }
        }
    }
    */
    public bool m_bAttack;
    public float m_fStopDistance;
    public ANavAgent m_pTarget;

    public float m_fBlockTime;
    public float m_fNotMoveTime;
    public float m_fWalkTime;
    public ANavAgent m_pOwner;
}

[AddComponentMenu("MGA/GridPath/Agent")]
public class ANavAgent : MonoBehaviour
{
    public const float BlockWait = 1.0f;
    public const float StopWait = 10.0f;
    public const float WalkCheckTime = 5.0f;
    public const float OutOfRange = 1.21f;

    /// <summary>
    /// not need to change transform.position at all, so use this swith to turn on and off
    /// </summary>
    public const bool m_bDebugPosition = true;

    /// <summary>
    /// for spawn and disable
    /// </summary>
    public int m_iID = -1;
    public APahtData m_pOwner = null;

    /// <summary>
    /// real position
    /// </summary>
    public Vector2 m_vGroundPos = Vector2.zero;

    /// <summary>
    /// walking task
    /// </summary>
    public SMoveTask m_Task;

    /// <summary>
    /// collision
    /// </summary>
    public float m_fRadius = 1.0f;
    public Vector2 m_vForce = Vector2.zero;
    public Vector2 m_vForceV = Vector2.zero;
    public bool m_bBuilding = false;
    public byte m_byAvoidenceLevel = 127; //Bigger, Heavier
    public int m_iRealAvoidenceLevel = 0;
    public bool m_bPushAwayFriends = true;
    public int m_iCamp = 0; //decide avoidence level

    /// <summary>
    /// status
    /// </summary>
    public ACharactor m_pUnit = null;
    public bool m_bAlive = false;
    public uint m_uiChannel = (uint)EAgentChannel.EAT_Ground;

    /// <summary>
    /// behaviours
    /// </summary>
    public bool m_bAutoRepath = true;
    public float m_fMoveSpeed = 5.0f; //max speed
    public float m_fAccelerate = 1.0f;
    public float m_fTurnSpeed = 120.0f;
    public float m_fAttackRange = 1.0f;
    
    private Vector2 m_vLastSpeed = Vector2.zero;
    private Vector2 m_vLastPos = Vector2.zero;
    private float m_fLastSpeed = 0.0f;

    public bool m_bBlockedByAgent = false;
    public bool m_bBlockedByWorld = false;
    

#if UNITY_EDITOR
    public Vector2 m_vDebugLastTo = new Vector2(0.0f, 0.0f);
    public EPathProgress m_eDebugProgress = EPathProgress.EPP_None;
#endif

    /// <summary>
    /// for initialize
    /// </summary>
    protected bool m_bPut = false; //TODO do we really need this?

    //TODO GetHeight
    private float GetHeight()
    {
        return 10.0f; // m_pOwner.m_fPathHeight;
    }

    public int m_iAdjustFailed = 0;
    private bool m_bStraightLinePath = false;
    public void OnDrawGizmos()
    {
        if (EPathProgress.EPP_Walking == m_Task.m_eProgress || EPathProgress.EPP_Attacking == m_Task.m_eProgress)
        {
            Gizmos.color = new Color(1.0f, 1.0f, 0.0f);
        }
        else
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        }
        Gizmos.DrawCube(transform.position, new Vector3(0.5f, 1.0f, 0.5f) * 3.0f);

        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        Gizmos.DrawCube(m_Task.m_vTargetPoint, new Vector3(0.5f, 1.0f, 0.5f));
        if (m_bStraightLinePath)
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        }
        else
        {
            switch (m_iAdjustFailed)
            {
                case 0:
                    Gizmos.color = new Color(0.0f, 1.0f, 1.0f);
                    break;
                case 1:
                    Gizmos.color = new Color(1.0f, 1.0f, 0.0f);
                    break;
                case 2:
                    Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
                    break;
            }
        }

        if (null != m_Task.m_vPathFound)
        {
            for (int i = (m_Task.m_iStep - 1) < 0 ? 0 : (m_Task.m_iStep - 1); i < m_Task.m_vPathFound.Length - 1; ++i)
            {
                Gizmos.DrawLine(new Vector3(m_Task.m_vPathFound[i].x, transform.position.y, m_Task.m_vPathFound[i].y),
                                new Vector3(m_Task.m_vPathFound[i + 1].x, transform.position.y, m_Task.m_vPathFound[i + 1].y));
            }
        }

        Gizmos.color = new Color(1.0f, 0.0f, 1.0f);
        Gizmos.DrawLine(new Vector3(m_vLastPos.x, transform.position.y, m_vLastPos.y),
                        new Vector3(m_vDebugLastTo.x, transform.position.y, m_vDebugLastTo.y));
    }

    /// <summary>
    /// after spawn, use this
    /// </summary>
    /// <param name="pData"></param>
    public void Initial(APahtData pData)
    {
        if (null == m_Task.m_pOwner)
        {
            m_Task.m_pOwner = this;
        }

        if (null == pData)
        {
            CRuntimeLogger.LogWarning("pNavMesh is null");
            return;
        }
        if (null != m_pOwner)
        {
            //Initialed just reset
            m_Task.m_eProgress = EPathProgress.EPP_None;
            if (m_bBuilding)
            {
                m_Task.m_eProgress = EPathProgress.EPP_Building;
            }
            m_bAlive = true;
            return;
        }

        m_Task.m_eProgress = EPathProgress.EPP_None;
        if (m_bBuilding)
        {
            m_Task.m_eProgress = EPathProgress.EPP_Building;
        }

        m_bAlive = true;
        m_pOwner = pData;
        m_vGroundPos = m_pOwner.GetDefaultPos();

        m_pOwner.RegisterAgent(this);
    }

    public void Update()
    {
        if (null == m_pOwner)
        {
            return;
        }

        if (m_bBuilding)
        {
            m_Task.m_eProgress = EPathProgress.EPP_Building;
        }

        if (!m_bPut)
        {
            m_vGroundPos = new Vector2(transform.position.x, transform.position.z);
            m_vLastPos = m_vGroundPos;
            SetPos(new Vector3(m_vLastPos.x, GetHeight(), m_vLastPos.y));
            m_bPut = true;
        }

        m_iRealAvoidenceLevel = (int)m_Task.m_eProgress * 255 + m_byAvoidenceLevel;
        m_vLastPos = m_vGroundPos;
        Vector2 vOldLastPos = m_vLastPos;

#if UNITY_EDITOR
        m_eDebugProgress = m_Task.m_eProgress;
        m_vDebugLastTo = m_vLastPos;
#endif

        float fDeltaTime = Time.deltaTime;
        m_bBlockedByAgent = false;
        m_bBlockedByWorld = false;
        bool bBlockAgent = m_vForce.sqrMagnitude > 0.001f;
        //Apply Move
        if (EPathProgress.EPP_Attacking == m_Task.m_eProgress)
        {
            if (null == m_Task.m_pTarget)
            {
                m_Task.m_eProgress = EPathProgress.EPP_Done;
            }
            else
            {
                if ((m_Task.m_pTarget.m_vLastPos - m_vLastPos).sqrMagnitude > m_Task.m_fStopDistance * m_Task.m_fStopDistance * OutOfRange)
                {
                    Attack(m_Task.m_pTarget);
                }
            }
        }

        if (EPathProgress.EPP_Walking == m_Task.m_eProgress)
        {
            int iTargetStep = m_Task.m_iStep;
            if (iTargetStep >= m_Task.m_vPathFound.Length - 1 && m_Task.m_vPathFound.Length > 0)
            {
                if ((m_vLastPos - m_Task.m_vPathFound[m_Task.m_vPathFound.Length - 1]).sqrMagnitude < m_fRadius * m_fRadius * 0.25f)
                {
                    m_Task.m_eProgress = EPathProgress.EPP_Done;
                }
                else
                {
                    iTargetStep = m_Task.m_vPathFound.Length - 1;
                }
            }
            else
            {
                if ((m_vLastPos - m_Task.m_vPathFound[iTargetStep]).sqrMagnitude < m_fRadius * m_fRadius * 4.0f)
                {
                    ++m_Task.m_iStep;
                    ++iTargetStep;
                }
            }

            if (m_Task.m_bAttack && null != m_Task.m_pTarget &&
                (m_Task.m_pTarget.m_vLastPos - m_vLastPos).sqrMagnitude <= m_Task.m_fStopDistance * m_Task.m_fStopDistance)
            {
                m_Task.m_eProgress = EPathProgress.EPP_Attacking;
                m_fLastSpeed -= 5.0f * m_fMoveSpeed * fDeltaTime;
                m_vForce = new Vector2(0.0f, 0.0f);
            }

            //m_vLastSpeed.Normalize();
            if (EPathProgress.EPP_Walking == m_Task.m_eProgress)
            {
#if UNITY_EDITOR
                m_vDebugLastTo = m_Task.m_vPathFound[iTargetStep];
#endif
                Vector2 vWantedDir = (m_Task.m_vPathFound[iTargetStep] - m_vLastPos).normalized;
                if (Vector2.Dot(vWantedDir, m_vLastSpeed) > 0.9f)
                {
                    m_vLastSpeed = vWantedDir;
                }
                else
                {
                    Vector2 vDirDelta = vWantedDir - m_vLastSpeed;
                    float fDelta = vDirDelta.magnitude;
                    float fRealDelta = Mathf.Clamp01(m_fTurnSpeed * fDeltaTime / fDelta * Mathf.Rad2Deg);
                    m_vLastSpeed = vDirDelta * fRealDelta + m_vLastSpeed;
                }
                m_fLastSpeed += m_fAccelerate * fDeltaTime;
            }
            else if (m_fLastSpeed >= 0.0f)
            {
                m_fLastSpeed -= 5.0f * m_fMoveSpeed * fDeltaTime;
            }
        }
        else
        {
            m_vForce *= m_fAccelerate;// *m_fRadius;
            m_fLastSpeed -= 5.0f * m_fMoveSpeed * fDeltaTime;
        }

        if (m_vForce.sqrMagnitude > 0.01f)
        {
            m_vForceV += m_vForce * fDeltaTime;
        }
        else
        {
            m_vForceV *= 0.1f;
            if (m_vForceV.sqrMagnitude < 0.01f)
            {
                m_vForceV = Vector2.zero;
            }
        }
        m_vForce = Vector2.zero;
        m_vLastSpeed.Normalize();
        m_fLastSpeed = Mathf.Clamp(m_fLastSpeed, 0.0f, m_fMoveSpeed);

        if (m_fLastSpeed > 0.0f || m_vForceV.sqrMagnitude > 0.01f)
        {
            float fForceVPower = m_vForceV.magnitude;
            m_vForceV = m_vForceV.normalized * Mathf.Clamp(fForceVPower, 0.0f, 2.0f * m_fMoveSpeed);
            Vector2 vTargetPos = m_vForceV * fDeltaTime + m_vLastPos;
            if (m_fLastSpeed > 0.0f)
            {
                vTargetPos += m_vLastSpeed * m_fLastSpeed * fDeltaTime;
            }

            //TODO Check Collid world
            /*
            if (m_pOwner.FindNearestPoly(vTargetPos, true) < 0)
            {
                m_bBlockedByWorld = true;
#if !WORLD_CUT_CORRECT
                if (EPathProgress.EPP_Walking == m_Task.m_eProgress)
                {
                    int iStep = Mathf.Clamp(m_Task.m_iStep, 1, m_Task.m_vPathFound.Length - 1);
                    //If we are pushed away from our path, move back to path first
                    Vector2 vPathPos = NavMeshMath.PointToLine(m_vLastPos, m_Task.m_vPathFound[iStep - 1],
                                                               m_Task.m_vPathFound[iStep]);
                    vTargetPos = (vPathPos - m_vLastPos).normalized * m_fLastSpeed * fDeltaTime + m_vLastPos;
                    if (m_pOwner.FindNearestPoly(vTargetPos, true) >= 0)
                    {
                        m_vLastPos = vTargetPos;
                        transform.position = new Vector3(m_vLastPos.x, GetHeight(), m_vLastPos.y);
                    }
                    else
                    {
                        //just on a cut?
                        Vector2 vNextPos = m_Task.m_vPathFound[iStep];
                        //Vector2 vNearOnPlane = m_pOwner.FindNearestOnPlanePosPublic(vTargetPos);

                        if ((m_Task.m_vPathFound[iStep] - vTargetPos).sqrMagnitude < m_fRadius * m_fRadius * 0.25f && iStep < m_Task.m_vPathFound.Length - 2)
                        {
                            vNextPos = m_Task.m_vPathFound[iStep + 1];
                        }

                        vTargetPos = vTargetPos
                            //+ (vNearOnPlane - vTargetPos).normalized * m_fLastSpeed * fDeltaTime * 2.0f
                             + (vNextPos - m_vLastPos).normalized * m_fLastSpeed * fDeltaTime * 3.0f;

                        m_vLastPos = vTargetPos;
                        float fHeight = GetHeight();
                        transform.position = new Vector3(m_vLastPos.x, fHeight + m_fHeight, m_vLastPos.y);
                    }
                }
                else
                {
                    m_fLastSpeed -= 2.0f * m_fMoveSpeed * fDeltaTime;
                }
#else
                m_vLastPos = vTargetPos;
                float fHeight = m_bNeedHeight ? GetHeight() : 0.0f;
                transform.position = new Vector3(m_vLastPos.x, fHeight + m_fHeight, m_vLastPos.y);
#endif
            }
            else
            {
                m_vLastPos = vTargetPos;
                SetPos(new Vector3(m_vLastPos.x, GetHeight(), m_vLastPos.y));
            }
             */
        }

        //Deal with block
        if (EPathProgress.EPP_Walking == m_Task.m_eProgress)
        {
            if ((vOldLastPos - m_vLastPos).sqrMagnitude < (m_fMoveSpeed * fDeltaTime * 0.3f) * (m_fMoveSpeed * fDeltaTime * 0.3f))
            {
                m_Task.m_fNotMoveTime += fDeltaTime;
            }
            else
            {
                m_Task.m_fNotMoveTime = 0.0f;
            }
            if (bBlockAgent)
            {
                m_Task.m_fBlockTime += fDeltaTime;
            }
            else
            {
                m_Task.m_fBlockTime = 0.0f;
            }
            if (m_Task.m_fNotMoveTime > StopWait)
            {
                m_Task.m_eProgress = EPathProgress.EPP_GiveUp;
            }
            else if (m_Task.m_fBlockTime > BlockWait && m_bAutoRepath)
            {
                EPathRes result;
                Vector2 vTarget = m_Task.m_vTargetPoint;
                if (m_Task.m_bAttack && null != m_Task.m_pTarget)
                {
                    bool bFound;
                    float fStopDist = m_fAttackRange + m_fRadius + m_Task.m_pTarget.m_fRadius;
                    vTarget = m_pOwner.GetShootPos(
                        m_Task.m_pTarget.m_vLastPos, 
                        m_fRadius, 
                        fStopDist,
                        (fStopDist - m_Task.m_pTarget.m_fRadius) * 0.5f + m_Task.m_pTarget.m_fRadius,
                        m_uiChannel, 
                        out bFound);
                }
                Vector2[] path = GetAPath(m_Task.m_vTargetPoint, m_Task.m_pTarget, out result);
                if (EPathRes.EPR_Done == result)
                {
                    m_Task.m_vPathFound = path;
                    m_Task.m_iStep = 0;
                    m_Task.m_fBlockTime = 0.0f;
                    m_Task.m_fNotMoveTime = 0.0f;
                    m_Task.m_fWalkTime = 0.0f;
                    m_Task.m_vTargetPoint = vTarget;
                }
            }
        }
        if ((EPathProgress.EPP_Walking == m_Task.m_eProgress || EPathProgress.EPP_GiveUp == m_Task.m_eProgress) && m_bAutoRepath)
        {
            m_Task.m_fWalkTime += fDeltaTime;
            if (m_Task.m_fWalkTime > WalkCheckTime)
            {
                EPathRes result;
                Vector2 vTarget = m_Task.m_vTargetPoint;
                if (m_Task.m_bAttack && null != m_Task.m_pTarget)
                {
                    bool bFound;
                    float fStopDist = m_fAttackRange + m_fRadius + m_Task.m_pTarget.m_fRadius;
                    vTarget = m_pOwner.GetShootPos(
                        m_Task.m_pTarget.m_vLastPos, 
                        m_fRadius, 
                        fStopDist,
                        (fStopDist - m_Task.m_pTarget.m_fRadius) * 0.5f + m_Task.m_pTarget.m_fRadius,
                        m_uiChannel, 
                        out bFound);
                }
                Vector2[] path = GetAPath(m_Task.m_vTargetPoint, m_Task.m_pTarget, out result);
                if (EPathRes.EPR_Done == result)
                {
                    m_Task.m_vPathFound = path;
                    m_Task.m_iStep = 0;
                    m_Task.m_fBlockTime = 0.0f;
                    m_Task.m_fNotMoveTime = 0.0f;
                    m_Task.m_fWalkTime = 0.0f;
                    m_Task.m_vTargetPoint = vTarget;
                }
            }
        }
    }

    private void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public Vector3[] m_vFound = null;

    private Vector2[] GetAPath(Vector2 vTarget, ANavAgent pTarget, out EPathRes result)
    {
        result = EPathRes.EPR_Done;
        return null;
        /*
        if (m_pUnit is APawn)
        {
            if (!(m_pUnit as APawn).IsGroundPawn())
            {
                //这货是飞的
                result = EPathFindingConst.PathFound;
                bStraight = true;
                return new[] { m_vLastPos, vTarget };
            }
        }

        if (!m_pOwner.EdgeCheckWithAgent(m_vLastPos, vTarget, this, pTarget))
        {
            result = EPathFindingConst.PathFound;
            bStraight = true;

            return new[] { m_vLastPos, vTarget };
        }

        bStraight = false;
        if (null != StaticInfo.m_pBattle
         && null != StaticInfo.m_pBattle.m_pGrid
         && null != StaticInfo.m_pBattle.m_pGrid.m_pNav
         && m_pOwner == StaticInfo.m_pBattle.m_pGrid.m_pNav
         && StaticInfo.m_pBattle.m_bBattleStarted
            //&& false
            )
        {
            Vector2[] ret = StaticInfo.m_pBattle.m_pGrid.AStarFindPathFromToV(m_vLastPos, vTarget, out result);
            if (null == ret || ret.Length < 2)
            {
                ret = new[] { m_vLastPos, (vTarget - m_vLastPos) * 0.1f + m_vLastPos };
            }
            return ret;
        }

        return m_pOwner.PortalFindPathWithAgent(this, pTarget, m_vLastPos, vTarget, out result);
        */
    }

    public void UnRegisterAgent()
    {
        m_pOwner.UnRegisterAgent(this);
    }

    public void ResetToPos(Vector3 vPos)
    {
        transform.position = vPos;
        m_vGroundPos = vPos;
        m_vLastPos = new Vector2(vPos.x, vPos.z);
        m_Task.m_vTargetPoint = m_vLastPos;
        m_Task.m_eProgress = EPathProgress.EPP_None;
        if (m_bBuilding)
        {
            m_Task.m_eProgress = EPathProgress.EPP_Building;
        }
    }

    /// <summary>
    /// For teleport
    /// </summary>
    /// <param name="vPos"></param>
    public void ResetToPosKeepState(Vector3 vPos)
    {
        transform.position = vPos;
        m_vGroundPos = vPos;
        m_vLastPos = new Vector2(vPos.x, vPos.z);
    }

    public void MoveTo(Vector2 vPos, bool bAttack = false, float fStopDist = 0.01f, ANavAgent pTarget = null)
    {
        /*
        EPathFindingConst result = EPathFindingConst.NoPath;
        if (m_bBuilding)
        {
            return result;
        }

        if (null != m_pOwner)
        {
            Vector2[] path = GetAPath(vPos, pTarget, out result, out m_bStraightLinePath);

            if (EPathFindingConst.NoPath == result)
            {
                m_Task.m_eProgress = EPathProgress.EPP_GiveUp;
                m_Task.m_vPathFound = null;
                m_Task.m_vTargetPoint = vPos;
                m_Task.m_iStep = 0;
                m_Task.m_bAttack = bAttack; //for attack, change to hold
                m_Task.m_fBlockTime = 0.0f;
                m_Task.m_fNotMoveTime = 0.0f;
                m_Task.m_fStopDistance = fStopDist; //for range attack
                m_Task.m_pTarget = pTarget;
                m_Task.m_fWalkTime = 0.0f;
                m_Task.m_pOwner = this;
            }
            else
            {
                m_Task.m_eProgress = EPathProgress.EPP_Walking;
                m_Task.m_vPathFound = path;
                m_Task.m_vTargetPoint = vPos;
                m_Task.m_iStep = 0;
                m_Task.m_bAttack = bAttack;
                m_Task.m_fBlockTime = 0.0f;
                m_Task.m_fNotMoveTime = 0.0f;
                m_Task.m_fStopDistance = fStopDist;
                m_Task.m_pTarget = pTarget;
                m_Task.m_fWalkTime = 0.0f;
                m_Task.m_pOwner = this;
            }
        }

        return result;
        */
    }

    public void Attack(ANavAgent pTarget)
    {
        if (m_bBuilding)
        {
            return;
        }
        bool bFound;
        float fStopDist = m_fAttackRange + m_fRadius + pTarget.m_fRadius;
        //TODO Get Shoot Pos
        /*
        Vector2 vPos = m_pOwner.GetShootPos(pTarget.m_vLastPos, m_fRadius, fStopDist,
                                            (fStopDist - pTarget.m_fRadius) * 0.5f + pTarget.m_fRadius,
                                            m_iAvoidenceLayer.value, out bFound);
        */
        //MoveTo(vPos, true, fStopDist, pTarget);
    }

}
