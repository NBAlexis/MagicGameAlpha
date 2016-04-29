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

[AddComponentMenu("MGA/Avatar/ACharactorModel")]
public class ACharactorModel : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void Assemble()
    {
        
    }

    public void AssembleAndFix()
    {

    }

    virtual public void Randomize()
    {

    }
}
