using UnityEngine;
using System.Collections;

public enum ECharactorAttackType : byte
{
    ECAT_Mele,
    ECAT_Range,
    ECAT_Wizard,
}

public enum ECharactorMoveType : byte
{
    ECMT_Sky,
    ECMT_Ground,
}

public enum ECharactorVisible : byte
{
    ECV_Visible,
    ECV_InVisible,

    ECV_None,
}

public enum ECharactorCamp : byte
{
    ECC_1,
    ECC_2,
    ECC_3,
    ECC_4,
    ECC_5,
    ECC_6,

    ECC_Max,
}

public enum ECharactorType : byte
{
    ECT_Humanoid,
}

public class ACharactorModel : MonoBehaviour 
{
    #region Need to fix

    public ACharactorAnimation m_pAnim;
    public ACharactor m_pOwner;
    public bool m_bFixed = false;
    public Transform m_pShell;
    public float m_fModelSize = 1.0f;

    public ECharactorVisible m_eVisible = ECharactorVisible.ECV_None;
    public ECharactorCamp m_eCamp = ECharactorCamp.ECC_Max;
    public ECharactorAttackType m_eAttackType = ECharactorAttackType.ECAT_Mele;
    public ECharactorMoveType m_eMoveType = ECharactorMoveType.ECMT_Ground;
    public ECharactorType m_eCharactorType = ECharactorType.ECT_Humanoid;

    #endregion

    #region Need to be override

    virtual public void Assemble()
    {
        
    }

    virtual public void AssembleAndFix()
    {

    }

    virtual public void Randomize(bool bFix = false)
    {

    }

    virtual public void SetVisible(ECharactorVisible eVisible)
    {

    }

    virtual public void SetCamp(ECharactorCamp eCamp)
    {

    }

    virtual public void RandomizeFix()
    {
    }

    virtual public void HideAll()
    {
        
    }

    virtual public void RecoverAll()
    {

    }

    #endregion

}
