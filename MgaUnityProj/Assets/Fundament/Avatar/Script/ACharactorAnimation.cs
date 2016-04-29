using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public enum EAnimationState
{
    EAS_Born,
    EAS_Move,
    EAS_Fight,
    EAS_Spell,
    EAS_Die,
    EAS_GameOver,

    EAS_Building,
    EAS_Avatar,

    EAS_Max,
}

public enum EAnimationType
{
    EAT_None,

    EAT_Idle,
    EAT_Run,
    EAT_Attack,
    EAT_Skill,
    EAT_SkillHold,
    EAT_KnockDown,
    EAT_Dash,
    EAT_Born,
    EAT_Die,

    EAT_Max,
}

[AddComponentMenu("MGA/Avatar/ACharactorAnimation")]
public class ACharactorAnimation : MonoBehaviour 
{
    public string[] m_sAnimList = null;
    public float[] m_fAnimSpeed = null;
    public Animation m_pAnim = null;

    public string[] m_sTurrentAnimList = null;
    public float[] m_fTurrentAnimSpeed = null;
    public Animation m_pTurrentAnim = null;

    public EAnimationState m_eCurrentAnimState = EAnimationState.EAS_Avatar;
    public EAnimationType m_eAnimType = EAnimationType.EAT_Idle;

    public ACharactor m_pOwner = null;
    protected float m_fOnceAnimTime = 1.0f;

    public float GetOnceAnim()
    {
        return m_fOnceAnimTime;
    }

    public void SetOnceAnim(float fOnce)
    {
        m_fOnceAnimTime = fOnce;
    }

    public void OnDisable()
    {
        m_eAnimType = EAnimationType.EAT_Idle;
    }

    public void AReset()
    {
        m_eCurrentAnimState = EAnimationState.EAS_Move;
        if (m_sAnimList != null)
        {
            if (m_pAnim != null && (EAnimationType.EAT_Born != m_eAnimType))
            {
                if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Idle]))
                {
                    m_pAnim.Play(m_sAnimList[(int)EAnimationType.EAT_Idle], PlayMode.StopAll);
                    m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].time = 0.001f;
                    m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].wrapMode = WrapMode.Loop;
                    m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].speed = 1.0f * m_fAnimSpeed[(int)EAnimationType.EAT_Idle];
                    m_pAnim.Sample();
                }
                else
                {
                    CRuntimeLogger.Log(m_pOwner.gameObject.name + " m_sAnimList does not have EAnimationType.EAT_Idle");
                }
            }
        }
    }

    public void Initial()
    {
        //If the length of animation list is not EAnimationType.EAT_Max, fill it with blank at first
        List<string> sCopyAnims = null == m_sAnimList ? new List<string>() : m_sAnimList.ToList();
        for (int i = sCopyAnims.Count; i < (int)EAnimationType.EAT_Max; ++i)
        {
            sCopyAnims.Add("");
        }
        m_sAnimList = sCopyAnims.ToArray();

        List<float> fCopyAnims = null == m_fAnimSpeed ? new List<float>() : m_fAnimSpeed.ToList();
        for (int i = fCopyAnims.Count; i < (int)EAnimationType.EAT_Max; ++i)
        {
            fCopyAnims.Add(1.0f);
        }
        m_fAnimSpeed = fCopyAnims.ToArray();
    }

    protected float m_fLastAnimSpeed = 1.0f;
    public void Update()
    {
        if (null == m_pAnim 
          || null == m_sAnimList
          || (int)EAnimationType.EAT_Max != m_sAnimList.Length
          || null == m_pOwner)
        {
            return;
        }

        float fDeltaTime = Time.deltaTime;
        //for some buff that can forzen the animation
        if (Mathf.Abs(m_fLastAnimSpeed - m_pOwner.GetAnimRate()) > 0.1f)
        {
            m_fLastAnimSpeed = m_pOwner.GetAnimRate();
            if (!string.IsNullOrEmpty(m_sAnimList[(int)m_eAnimType]) && null != m_pAnim.GetClip(m_sAnimList[(int)m_eAnimType]))
            {
                m_pAnim[m_sAnimList[(int)m_eAnimType]].speed = m_fAnimSpeed[(int)m_eAnimType] * m_fLastAnimSpeed;
            }
        }

        //Our animation is based on renderer, so need to count the time
        if (m_fOnceAnimTime > 0.0f)
        {
            m_fOnceAnimTime -= fDeltaTime;
        }

        //if not special case, check idle or fight or running
        //if idle, do something
        float fStopSpeed = 0.2f * m_pOwner.m_fMoveSpeed;
        float fSlowSpeed = 0.5f * m_pOwner.m_fMoveSpeed;

        switch (m_eCurrentAnimState)
        {
            case EAnimationState.EAS_Avatar:
                {
                    //This is for not self-controlled charactors
                    if (m_fOnceAnimTime < 0.0F)
                    {
                        if (m_pOwner.m_vLastDelta.sqrMagnitude > fStopSpeed * fStopSpeed * fDeltaTime * fDeltaTime)
                        {
                            PlayAnimation(EAnimationType.EAT_Run);
                            if (m_pOwner.m_vLastDelta.sqrMagnitude < fSlowSpeed * fSlowSpeed * fDeltaTime * fDeltaTime)
                            {
                                float fAnimSpeed = Random.Range(0.85f, 1.15f)
                                        * (m_pOwner.m_vLastDelta.magnitude - fSlowSpeed * fDeltaTime * 1.5f) / fSlowSpeed;
                                fAnimSpeed = Mathf.Max(fAnimSpeed, 0.3f);
                                m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Run]].speed = fAnimSpeed * m_fAnimSpeed[(int)EAnimationType.EAT_Run];
                            }
                            else
                            {
                                m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Run]].speed = Random.Range(0.85f, 1.15f) * m_fAnimSpeed[(int)EAnimationType.EAT_Run];
                            }
                        }
                        else
                        {
                            PlayAnimation(EAnimationType.EAT_Idle);
                        }
                    }
                }
            break;
            case EAnimationState.EAS_Move:
                {
                    /*
                    if (EPawnState.EPS_Attack == m_pOwner.GetState())
                    {
                        SetAnimState(EAnimationState.EAS_Fight);
                        return;
                    }

                    if (EAnimationType.EAT_Spell == m_eAnimType
                     || EAnimationType.EAT_Spell2 == m_eAnimType)
                    {
                        if (m_fOnceAnimTime > 0.0f)
                        {
                            return;
                        }
                    }

                    //Tick Move and stop
                    float fDeltaMove = m_pOwner.m_vLastDelta.magnitude;
                    if (fDeltaMove > fStopSpeed * fDeltaTime)
                    {
                        PlayAnimation(EAnimationType.EAT_Run);
                        if (fDeltaMove < fSlowSpeed * fDeltaTime)
                        {
                            float fAnimSpeed = Random.Range(0.95f, 1.05f) * m_fAnimSpeed[(int)EAnimationType.EAT_Run] * (fDeltaMove / fDeltaTime)
                                / (m_pOwner.Size * (null == m_pOwner.m_pCard.m_pCardInfo ? 4.0f : m_pOwner.m_pCard.m_pCardInfo.m_fCreatureSpeed));
                            fAnimSpeed = Mathf.Max(fAnimSpeed, 0.1f);
                            m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Run]].speed = m_fLastAnimSpeed * fAnimSpeed * m_fAnimSpeed[(int)EAnimationType.EAT_Run];
                        }
                        else
                        {
                            m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Run]].speed = m_fLastAnimSpeed * Random.Range(0.95f, 1.05f) * m_fAnimSpeed[(int)EAnimationType.EAT_Run] * (fDeltaMove / fDeltaTime)
                                / (m_pOwner.Size * (null == m_pOwner.m_pCard.m_pCardInfo ? 4.0f : m_pOwner.m_pCard.m_pCardInfo.m_fCreatureSpeed));
                        }
                    }
                    else
                    {
                        PlayAnimation(EAnimationType.EAT_Idle);
                        m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].speed = m_fLastAnimSpeed;
                    }
                     */
                }
                break;
            case EAnimationState.EAS_Fight:
                {
                    /*
                    if (EPawnState.EPS_Attack != m_pOwner.GetState())
                    {
                        SetAnimState(EAnimationState.EAS_Move);
                        return;
                    }

                    if (!m_pOwner.m_bMoveOrAttack)
                    {
                        if (m_pOwner.m_vLastDelta.sqrMagnitude > fStopSpeed * fStopSpeed * fDeltaTime * fDeltaTime)
                        {
                            PlayAnimation(EAnimationType.EAT_Run);
                            if (m_pOwner.m_vLastDelta.sqrMagnitude < fSlowSpeed * fSlowSpeed * fDeltaTime * fDeltaTime)
                            {
                                float fAnimSpeed = Random.Range(0.85f, 1.15f)
                                        * (m_pOwner.m_vLastDelta.magnitude - fSlowSpeed * fDeltaTime * 1.5f) / fSlowSpeed;
                                fAnimSpeed = Mathf.Max(fAnimSpeed, 0.5f);
                                m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Run]].speed = m_fLastAnimSpeed * fAnimSpeed * m_fAnimSpeed[(int)EAnimationType.EAT_Run];
                            }
                            else
                            {
                                m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Run]].speed = m_fLastAnimSpeed * Random.Range(0.85f, 1.15f) * m_fAnimSpeed[(int)EAnimationType.EAT_Run] * m_fAnimSpeed[(int)EAnimationType.EAT_Run];
                            }
                        }
                        else
                        {
                            PlayAnimation(EAnimationType.EAT_Idle);
                            m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].speed = m_fLastAnimSpeed;
                        }
                    }
                    else
                    {
                        if (EAnimationType.EAT_Attack == m_eAnimType)
                        {
                            if (!m_pAnim.isPlaying)
                            {
                                PlayAnimation(EAnimationType.EAT_Idle);
                            }
                        }
                    }

                    if (!m_pAnim.isPlaying)
                    {
                        PlayAnimation(EAnimationType.EAT_Idle);
                    }
                     */
                }
                break;
            case EAnimationState.EAS_GameOver:
                {
                    PlayAnimation(EAnimationType.EAT_Idle);
                }
                break;
            default:
                {
                    /*
                    if (m_pOwner.m_iQuickKill > 0 && m_fOnceAnimTime > 0.0F && EPawnState.EPS_Die == m_pOwner.GetState())
                    {
                        m_fOnceAnimTime = -0.1F;
                    }
                    if (m_fOnceAnimTime <= 0.0f)//Our animation is based on renderer
                    {
                        if (EPawnState.EPS_Die == m_pOwner.GetState())
                        {
                            if (!m_pOwner.IsCanHide())
                            {
                                m_pOwner.OnCanHide();
                            }
                        }
                        else
                        {
                            if (EAnimationType.EAT_KnockDown == m_eAnimType)
                            {
                                PlayAnimation(EAnimationType.EAT_KnockUp);
                            }
                            else
                            {
                                SetAnimState(EAnimationState.EAS_Move);
                            }
                        }
                    }
                     */
                }
                break;
        }
    }

    public EAnimationState GetState()
    {
        return m_eCurrentAnimState;
    }

    public void SetAnimState(EAnimationState eState)
    {
        if (EAnimationState.EAS_Fight == eState &&
           (EAnimationState.EAS_Born != eState))
        {
            m_eCurrentAnimState = eState;
        }

        if (eState != m_eCurrentAnimState)
        {
            switch (eState)
            {
                case EAnimationState.EAS_Born:
                    PlayAnimation(EAnimationType.EAT_Born);
                    break;
                case EAnimationState.EAS_Move:
                    PlayAnimation(EAnimationType.EAT_Idle);
                    break;
                case EAnimationState.EAS_Die:
                    PlayAnimation(EAnimationType.EAT_Die);
                    break;
                case EAnimationState.EAS_GameOver:
                    PlayAnimation(EAnimationType.EAT_Idle);
                    break;
            }

            m_eCurrentAnimState = eState;
        }
    }

    public float PlayAttackAnimation(float fMaxLength, bool bSkill)
    {
        /*
        if (m_fOnceAnimTime > 0.0F &&
            (EAnimationType.EAT_Attack == m_eAnimType
          || EAnimationType.EAT_Skill == m_eAnimType
          || EAnimationType.EAT_SkillHold == m_eAnimType
          ))
        {
            CRuntimeLogger.Log("Should Never Be Here if is not replay or Runtime Online Pvp!");
            return -1.0f;
        }

        List<EAnimationType> eTypes = new List<EAnimationType>();
        if (bSkill)
        {
            if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Spell]))
            {
                eTypes.Add(EAnimationType.EAT_Spell);
            }
            if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Spell2]))
            {
                eTypes.Add(EAnimationType.EAT_Spell2);
            }
        }

        if (eTypes.Count < 1)
        {
            if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Attack]))
            {
                eTypes.Add(EAnimationType.EAT_Attack);
            }
            if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Attack2]))
            {
                eTypes.Add(EAnimationType.EAT_Attack2);
            }
            if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Attack3]))
            {
                eTypes.Add(EAnimationType.EAT_Attack3);
            }
        }

        EAnimationType eType = eTypes.Count > 0 ? eTypes[Random.Range(0, eTypes.Count)] : EAnimationType.EAT_Idle;

        string sAnim = m_sAnimList[(int)eType];
        float fSpeed = m_fAnimSpeed[(int)eType];
        m_pAnim.CrossFade(sAnim, 0.15f);
        m_pAnim[sAnim].time = 0.01F;
        m_pAnim[sAnim].wrapMode = WrapMode.Once;
        m_eAnimType = eType;
        if (m_pAnim[sAnim].length > fMaxLength * fSpeed)
        {
            fMaxLength = fMaxLength * Random.Range(0.8f, 0.95f);
            m_pAnim[sAnim].speed = (m_pAnim[sAnim].length / fMaxLength);
            m_fOnceAnimTime = fMaxLength;
            return fMaxLength;
        }

        float fRate = Random.Range(1.0f, 1.3f) * fSpeed;
        m_pAnim[sAnim].speed = fRate;
        m_fOnceAnimTime = m_pAnim[sAnim].length / fRate;
         */
        return m_fOnceAnimTime;
    }

    public void PlayTimeTickSpellAnimation(float fLength)
    {
        /*
        if (m_pOwner.IsDied())
        {
            return;
        }

        if (EAnimationType.EAT_Spell == m_eAnimType
         || EAnimationType.EAT_Spell2 == m_eAnimType)
        {
            string sAnim1 = m_sAnimList[(int)m_eAnimType];
            if (m_fOnceAnimTime > 0.01F && fLength > 0.01f)
            {
                m_pAnim[sAnim1].wrapMode = WrapMode.Loop;
                m_fOnceAnimTime = fLength;
                return;
            }
        }

        if (EAnimationType.EAT_Die == m_eAnimType
         || EAnimationType.EAT_Die2 == m_eAnimType
         || EAnimationType.EAT_Born == m_eAnimType
         || EAnimationType.EAT_Born2 == m_eAnimType
         || EAnimationType.EAT_Win == m_eAnimType
         || EAnimationType.EAT_Lose == m_eAnimType
         || EAnimationType.EAT_Spell == m_eAnimType
         || EAnimationType.EAT_Spell2 == m_eAnimType
            )
        {
            return;
        }

        if (EAnimationState.EAS_Born == m_eCurrentAnimState
         || EAnimationState.EAS_Die == m_eCurrentAnimState
         || EAnimationState.EAS_GameOver == m_eCurrentAnimState)
        {
            return;
        }

        List<EAnimationType> eTypes = new List<EAnimationType>();
        if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Spell]))
        {
            eTypes.Add(EAnimationType.EAT_Spell);
        }
        if (!string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Spell2]))
        {
            eTypes.Add(EAnimationType.EAT_Spell2);
        }
        if (string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Spell])
         && string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Spell2])
        && !string.IsNullOrEmpty(m_sAnimList[(int)EAnimationType.EAT_Attack]))
        {
            eTypes.Add(EAnimationType.EAT_Attack);
        }

        if (eTypes.Count < 1)
        {
            return;
        }

        EAnimationType eType = eTypes[Random.Range(0, eTypes.Count)];
        string sAnim = m_sAnimList[(int)eType];
        float fSpeed = m_fAnimSpeed[(int)eType];
        float fAniLength = m_pAnim[sAnim].length;


        if (fLength < 0.6f)
        {
            m_pAnim.CrossFade(sAnim, 0.01f);
            m_pAnim[sAnim].wrapMode = WrapMode.Once;
        }
        else
        {
            m_pAnim.CrossFade(sAnim, 0.15f);
            m_pAnim[sAnim].wrapMode = fLength > 0.01f ? WrapMode.Loop : WrapMode.Once;
        }

        m_pAnim[sAnim].time = 0.01F;
        m_eAnimType = eType;
        m_pAnim[sAnim].speed = fSpeed * Random.Range(1.0f, 1.3f);
        m_fOnceAnimTime = fLength > 0.01f ? fLength : m_pAnim[sAnim].length * m_pAnim[sAnim].speed;
         */
    }

    public void PlayAnimation(EAnimationType eType, bool bRefresh = false)
    {
        if (null != m_pAnim && (EAnimationType.EAT_Born == eType || EAnimationType.EAT_Die == eType))
        {
            m_pAnim.cullingType = AnimationCullingType.AlwaysAnimate;
        }
        else if (null != m_pAnim)
        {
            m_pAnim.cullingType = AnimationCullingType.BasedOnRenderers;
        }

        if (null == m_sAnimList || (int)EAnimationType.EAT_Max != m_sAnimList.Length)
        {
            return;
        }

        if ((eType != m_eAnimType || bRefresh) && null != m_pAnim)
        {
            // ReSharper disable ReplaceWithSingleAssignment.False
            bool bIgnore = false;
            // ReSharper disable once ConvertIfToOrExpression
            if ((EAnimationType.EAT_Idle == eType
               || EAnimationType.EAT_Run == eType
               || EAnimationType.EAT_Attack == eType)
            && (
                EAnimationType.EAT_KnockDown == m_eAnimType
             || EAnimationType.EAT_Skill == m_eAnimType
             || EAnimationType.EAT_SkillHold == m_eAnimType
             || EAnimationType.EAT_Dash == m_eAnimType
               )
            && m_fOnceAnimTime > 0.0f
                )
            {
                bIgnore = true;
            }
            // ReSharper restore ReplaceWithSingleAssignment.False

            if (EAnimationType.EAT_Born == eType
             || EAnimationType.EAT_Die == eType
                )
            {
                bIgnore = false;
            }

            EAnimationType eTypePlayed = eType;
            if (!bIgnore)
            {
                //CRuntimeLogger.Log(eType);
                switch (eType)
                {
                    case EAnimationType.EAT_Idle:
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            CRuntimeLogger.LogWarning("Animation list has animation not in Model:" + m_pOwner.gameObject.name + "@" + eType);
                            return;
                        }
                        m_pAnim.CrossFade(m_sAnimList[(int)eType], 0.5f, PlayMode.StopAll);
                        m_pAnim[m_sAnimList[(int)eType]].wrapMode = WrapMode.Loop;
                        m_pAnim[m_sAnimList[(int)eType]].speed = Random.Range(0.85f, 1.15f) * m_fAnimSpeed[(int)eType];
                        break;
                    case EAnimationType.EAT_Run:
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            CRuntimeLogger.LogWarning("Animation list has animation not in Model:" + m_pOwner.gameObject.name + "@" + eType);
                            return;
                        }
                        m_pAnim.CrossFade(m_sAnimList[(int)eType], 0.5f, PlayMode.StopAll);
                        m_pAnim[m_sAnimList[(int)eType]].wrapMode = WrapMode.Loop;
                        m_pAnim[m_sAnimList[(int)eType]].speed = Random.Range(0.85f, 1.15f) * m_fAnimSpeed[(int)eType];
                        break;
                    case EAnimationType.EAT_Born:
                        string sAnim = "";
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            sAnim = m_sAnimList[(int)EAnimationType.EAT_Idle];
                        }
                        m_pAnim.CrossFade(sAnim, 0.01f, PlayMode.StopAll);
                        m_pAnim[sAnim].normalizedTime = 0.0f;
                        m_pAnim[sAnim].wrapMode = WrapMode.Clamp;
                        m_pAnim[sAnim].speed = Random.Range(0.8f, 1.0f) * m_fAnimSpeed[(int)EAnimationType.EAT_Idle];
                        m_fOnceAnimTime = m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].length * 0.99f / m_pAnim[m_sAnimList[(int)EAnimationType.EAT_Idle]].speed;
                        m_pAnim.Sample();
                        break;
                    case EAnimationType.EAT_Attack:
                    case EAnimationType.EAT_Skill:
                        //Only Avatar will enter this, for normal unit, will use PlayAttackAnimation function to play
                        eTypePlayed = eType;
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            eTypePlayed = EAnimationType.EAT_Attack;
                        }
                        if (null == m_pAnim[m_sAnimList[(int)eTypePlayed]])
                        {
                            CRuntimeLogger.LogWarning("Animation list has animation not in Model:" + m_pOwner.gameObject.name + "@" + eTypePlayed);
                            return;
                        }
                        m_pAnim.CrossFade(m_sAnimList[(int)eTypePlayed], 0.1f, PlayMode.StopAll);
                        m_pAnim[m_sAnimList[(int)eTypePlayed]].normalizedTime = Random.Range(0.0f, 0.1f);
                        m_pAnim[m_sAnimList[(int)eTypePlayed]].wrapMode = WrapMode.Once;
                        m_pAnim[m_sAnimList[(int)eTypePlayed]].speed = Random.Range(0.9f, 1.1f) * m_fAnimSpeed[(int)eTypePlayed];
                        m_fOnceAnimTime = m_pAnim[m_sAnimList[(int)eTypePlayed]].length / m_pAnim[m_sAnimList[(int)eTypePlayed]].speed;
                        break;
                    case EAnimationType.EAT_Dash:
                    case EAnimationType.EAT_SkillHold:
                        //Only Avatar will enter this, for normal unit, will use PlayAttackAnimation function to play
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            return;
                        }
                        m_pAnim.CrossFade(m_sAnimList[(int)eType], 0.1f, PlayMode.StopAll);
                        m_pAnim[m_sAnimList[(int)eType]].normalizedTime = Random.Range(0.0f, 0.1f);
                        m_pAnim[m_sAnimList[(int)eType]].wrapMode = WrapMode.Loop;
                        m_pAnim[m_sAnimList[(int)eType]].speed = Random.Range(0.9f, 1.1f) * m_fAnimSpeed[(int)eType];
                        //TODO how to give a length for these?
                        m_fOnceAnimTime = 1.0f;
                        break;
                    case EAnimationType.EAT_Die:
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            CRuntimeLogger.LogWarning("Animation list has animation not in Model:" + m_pOwner.gameObject.name + "@" + eType);
                            return;
                        }
                        m_pAnim.CrossFade(m_sAnimList[(int)eType], 0.1f, PlayMode.StopAll);
                        m_pAnim[m_sAnimList[(int)eType]].normalizedTime = Random.Range(0.0f, 0.05f);
                        m_pAnim[m_sAnimList[(int)eType]].wrapMode = WrapMode.Clamp;
                        m_pAnim[m_sAnimList[(int)eType]].speed = Random.Range(0.85f, 1.15f) * m_fAnimSpeed[(int)eType];
                        m_fOnceAnimTime = m_pAnim[m_sAnimList[(int)eType]].length / m_pAnim[m_sAnimList[(int)eType]].speed;

                        break;
                    case EAnimationType.EAT_KnockDown:
                        if (null == m_pAnim[m_sAnimList[(int)eType]])
                        {
                            return;
                        }
                        m_pAnim.CrossFade(m_sAnimList[(int)eType], 0.1f, PlayMode.StopAll);
                        m_pAnim[m_sAnimList[(int)eType]].normalizedTime = Random.Range(0.0f, 0.05f);
                        m_pAnim[m_sAnimList[(int)eType]].wrapMode = WrapMode.Clamp;
                        m_pAnim[m_sAnimList[(int)eType]].speed = Random.Range(0.85f, 1.15f) * m_fAnimSpeed[(int)eTypePlayed];
                        m_fOnceAnimTime = m_pAnim[m_sAnimList[(int)eType]].length / m_pAnim[m_sAnimList[(int)eType]].speed;
                        break;
                }

                m_eAnimType = eType;
            }
        }
    }

    #region Animation Events

    public void AnimationFunction_Msg(string sMsg)
    {
        
    }

    #endregion
}
