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

    #endregion

    #region Need to be override

    virtual public void Assemble()
    {
        
    }

    virtual public void AssembleAndFix()
    {

    }

    virtual public void Randomize()
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
