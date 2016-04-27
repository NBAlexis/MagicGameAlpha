using UnityEngine;
using System.Collections;

public enum ECharactorMatType
{
    ECMT_Normal,
    ECMT_Hair,
    ECMT_Cloth,
}

public enum EHumanoidComponentPos
{
    ECP_Body,
    ECP_Back,
    ECP_BackWear,
    ECP_Head,
    ECP_HeadWear,
    ECP_Feet,
    ECP_Hand,
    ECP_Weapon,
    ECP_Wing,
}

public enum EHumanoidWeaponType
{
    EHWT_SingleHandR,
    EHWT_SingleHandL,
    EHWT_DoubleHand,
    EHWT_ShootR,
    EHWT_ShootL,
}

public class CHumanoidDescElement : CMGDataElement
{
    public string m_sMeshPath = "";
    public string m_sTexturePath = "";
    public string m_sPrefabPath = "";
    public string[] m_sDependency = new string[0];

    public Vector3 m_vLocalPos = Vector3.zero;
    public Vector3 m_vLocalEular = Vector3.zero;

    public Vector3 m_vMinSize = Vector3.one * 0.95f;
    public Vector3 m_vMaxSize = Vector3.one * 1.05f;

    public ECharactorMatType m_eMatType = ECharactorMatType.ECMT_Normal;
    public EHumanoidComponentPos m_ePos = EHumanoidComponentPos.ECP_Body;
    public EHumanoidWeaponType m_eWeaponType = EHumanoidWeaponType.EHWT_SingleHandR;

    override public void ResetData()
    {

    }

    override public string GetString()
    {
        return "";
    }

    override public void LoadData(string sTextToParse)
    {

    }
}

public class CHumanoidDesc : TMGData<CHumanoidDescElement>
{
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/Avatar/Resources/HumanoidDesc";
    }

    public override string GetDefaultLoadPath()
    {
        return "HumanoidDesc";
    }
}
