using UnityEngine;

public enum ECharactorMatType
{
    ECMT_Normal,
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

    override public string GetString()
    {
        string sRet = base.GetString();

        sRet += Write("MeshPath", m_sMeshPath);
        sRet += Write("TexturePath", m_sTexturePath);
        sRet += Write("PrefabPath", m_sPrefabPath);
        sRet += Write("Dependency", m_sDependency);
        sRet += Write("LocalPos", m_vLocalPos);
        sRet += Write("LocalEular", m_vLocalEular);
        sRet += Write("MinSize", m_vMinSize);
        sRet += Write("MaxSize", m_vMaxSize);
        sRet += Write("MatType", m_eMatType);
        sRet += Write("Pos", m_ePos);
        sRet += Write("WeaponType", m_eWeaponType);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_sMeshPath = (string)GetElementValue(sTextToParse, "MeshPath", m_sMeshPath);
        m_sTexturePath = (string)GetElementValue(sTextToParse, "TexturePath", m_sTexturePath);
        m_sPrefabPath = (string)GetElementValue(sTextToParse, "PrefabPath", m_sPrefabPath);
        m_sDependency = (string[])GetElementValue(sTextToParse, "Dependency", m_sDependency);
        m_vLocalPos = (Vector3)GetElementValue(sTextToParse, "LocalPos", m_vLocalPos);
        m_vLocalEular = (Vector3)GetElementValue(sTextToParse, "LocalEular", m_vLocalEular);
        m_vMinSize = (Vector3)GetElementValue(sTextToParse, "MinSize", m_vMinSize);
        m_vMaxSize = (Vector3)GetElementValue(sTextToParse, "MaxSize", m_vMaxSize);
        m_eMatType = (ECharactorMatType)GetElementValue(sTextToParse, "MatType", m_eMatType);
        m_ePos = (EHumanoidComponentPos)GetElementValue(sTextToParse, "Pos", m_ePos);
        m_eWeaponType = (EHumanoidWeaponType)GetElementValue(sTextToParse, "WeaponType", m_eWeaponType);
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
