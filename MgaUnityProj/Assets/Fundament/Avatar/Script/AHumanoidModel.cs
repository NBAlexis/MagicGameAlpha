using UnityEngine;
using System.Collections;

[AddComponentMenu("MGA/Avatar/AHumanoidModel")]
public class AHumanoidModel : ACharactorModel
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #region Assemble

    public GameObject[] m_pMaleRFeet;
    public GameObject[] m_pMaleLFeet;
    public GameObject[] m_pFemaleRFeet;
    public GameObject[] m_pFemaleLFeet;

    public GameObject[] m_pHands;

    public GameObject[] m_pMaleBody;
    public GameObject[] m_FemaleBody;
    public GameObject[] m_pMaleHead;
    public GameObject[] m_FemaleHead;

    public GameObject[] m_FemaleHair;
    public GameObject[] m_MaleHair;
    public GameObject[] m_FemaleHat;
    public GameObject[] m_FemaleGlass;

    public GameObject[] m_pRSHandWeaponWizard;
    public GameObject[] m_pLSHandWeaponWizard;
    public GameObject[] m_pRSHandWeaponFight;
    public GameObject[] m_pLSHandWeaponFight;

    public GameObject[] m_pRShootWeapon;
    public GameObject[] m_pLShootWeapon;

    public GameObject[] m_pDHandWeaponFight;
    public GameObject[] m_pDHandWeaponWizard;

    public GameObject[] m_pBoltPos;
    public GameObject m_pShadow;
    public GameObject m_pBloodLineBack;
    public GameObject m_pBloodLineFront;

    #endregion

    private bool m_bHasHat = false;
    private bool m_bHasGlass = false;

    public void Randomize(ECharactorAttackType eCharactorAttack, ECharactorMoveType eMove, EHumanoidType eGendle)
    {

    }

    private void FixBodyAndHead()
    {
        
    }

    private void FixHand()
    {

    }

    private void FixBack()
    {

    }

    private void FixFoot()
    {

    }

    private void FixHair()
    {
        
    }

    private void FixHat()
    {

    }

    private void FixGlass()
    {

    }

    private void FixWeapon()
    {

    }

    private void FixAnimations()
    {
        
    }

    #region Need to be override

    override public void Assemble()
    {

    }

    override public void AssembleAndFix()
    {

    }

    override public void Randomize()
    {

    }

    override public void HideAll()
    {

    }

    #endregion
}
