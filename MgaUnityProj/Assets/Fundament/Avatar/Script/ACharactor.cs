using UnityEngine;
using System.Collections;
using Debug = System.Diagnostics.Debug;

[AddComponentMenu("MGA/Avatar/ACharactor")]
public class ACharactor : MonoBehaviour
{
    public bool m_bInitialed = false;
	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (!m_bInitialed)
	    {
	        Initial();
	    }
        m_pMachine.Update(Time.deltaTime);
    }

    public void Initial()
    {
        if (m_bInitialed)
        {
            return;
        }
        m_bInitialed = true;
        m_pAnim.Initial();

        InitialState();
        m_pStates[(int)ECharactorState.ECS_Avatar] = new CharactorState_Avatar();
        SetState(ECharactorState.ECS_Avatar);
    }

    #region State

    protected TStateMachine<ACharactor> m_pMachine = null;
    protected static CharactorState[] m_pStates = null;

    public void InitialState()
    {
        m_pMachine = new TStateMachine<ACharactor>(this);
        if (null == m_pStates || 0 == m_pStates.Length)
        {
            m_pStates = new CharactorState[(int)ECharactorState.EPS_Max];
        }
    }

    public void SetState(ECharactorState eState)
    {
        m_pMachine.ChangeState(m_pStates[(int)eState]);
    }

    public ECharactorState GetCurrentState()
    {
        CharactorState charactorState = m_pMachine.CurrentState() as CharactorState;
        if (charactorState != null)
        {
            return charactorState.m_eState;
        }
        return ECharactorState.EPS_Max;
    }

    public ECharactorState GetOldState()
    {
        CharactorState charactorState = m_pMachine.PreviousState() as CharactorState;
        if (charactorState != null)
        {
            return charactorState.m_eState;
        }
        return ECharactorState.EPS_Max;
    }

    #endregion

    //has to be a creature first! buildings cannot move!
    #region Move 

    public sfloat m_fMoveSpeed = 0.0f; //Current Max Speed
    public Vector2 m_vLastDelta = Vector2.zero;

    #endregion

    #region Model

    public ACharactorModel m_pModel;
    public sfloat m_fModelSize = 1.0f;

    #endregion

    #region Animation

    public ACharactorAnimation m_pAnim;
    //for some buff that can forzen the animation
    public float GetAnimRate()
    {
        return 1.0f;
    }

    #endregion
}
