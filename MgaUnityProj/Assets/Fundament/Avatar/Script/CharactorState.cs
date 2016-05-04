using UnityEngine;
//using System.Collections;

public enum ECharactorState
{
    ECS_MoveWithTeam,
    ECS_Attack,
    ECS_Die,
    ECS_Born,
    ECS_Building,
    ECS_Avatar,
    ECS_RunAway,
    ECS_Actor,
    ECS_Tornado,

    EPS_Max,
}

public class CharactorState : TState<ACharactor>
{
    public ECharactorState m_eState;

    public override void Enter(ACharactor pObj) { }
    public override void Execute(ACharactor pObj) { }
    public override void Update(ACharactor pObj, float fDeltaTime) { }
    public override void Exit(ACharactor pObj) { }
}

public class CharactorState_Avatar : CharactorState
{
    public CharactorState_Avatar()
    {
        m_eState = ECharactorState.ECS_Avatar;
    }

    public override void Enter(ACharactor pObj)
    {
        pObj.m_pAnim.SetAnimState(EAnimationState.EAS_Avatar);
    }

    public override void Execute(ACharactor pObj) { }
    public override void Update(ACharactor pObj, float fDeltaTime)
    {
        //Put to ground
        if (ECharactorMoveType.ECMT_Sky == pObj.m_pModel.m_eMoveType)
        {
            
            pObj.m_pModelShell.transform.localPosition = new Vector3(pObj.m_pModelShell.transform.localPosition.x,
                Mathf.Clamp(pObj.m_pModelShell.transform.localPosition.y + fDeltaTime * 4.5f, 0.0f, 6.0f),
                pObj.m_pModelShell.transform.localPosition.z);

        }
    }
    public override void Exit(ACharactor pObj) { }
};
