using UnityEngine;

public enum EHumanoidType : byte
{
    EHT_Male,
    EHT_Female,
    EHT_Both,
}

public enum EHumanoidSide : byte
{
    EHT_Left,
    EHT_Right,
}

public enum EHumanoidWeapon : byte
{
    EHW_BareHand,
    EHT_SingleHand_Wizard,
    EHT_SingleHand_Fight,
    EHT_DoubleHand_Wizard,
    EHT_DoubleHand_Fight,
    EHT_Shoot,

    EHT_Max,
}

public enum EHumanoidComponentPos
{
    ECP_Body,
    ECP_Back,
    ECP_Head,
    ECP_Hair,
    ECP_Feet,
    ECP_Hand,
    ECP_Weapon,
    ECP_Wing,
    ECP_BloodLineFront,
    ECP_Shadow,
}

public enum EHumanoidSize
{
    EHS_Normal,
    EHS_Small,
    EHS_Big,
    EHS_ThinTall,
    EHS_Fat,
}

public class CHumanoidDescElement : CMGDataElement
{
    public string m_sObjectPath = "";
    public int m_iWeight = 1;

    public EHumanoidComponentPos m_ePos = EHumanoidComponentPos.ECP_Body;
    public EHumanoidType m_eHumanType = EHumanoidType.EHT_Both;
    public EHumanoidSide m_eHumanSide = EHumanoidSide.EHT_Left;
    public EHumanoidWeapon m_eHumanWeapon = EHumanoidWeapon.EHW_BareHand;

    public override CMGDataElement GetDefault()
    {
        return new CHumanoidDescElement();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CHumanoidDescElement[] ret = new CHumanoidDescElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CHumanoidDescElement();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        return new CHumanoidDescElement
        {
            m_sObjectPath = m_sObjectPath,
            m_iWeight = m_iWeight,
            m_ePos = m_ePos,
            m_eHumanType = m_eHumanType,
            m_eHumanSide = m_eHumanSide,
            m_eHumanWeapon = m_eHumanWeapon,
        };
    }

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);

        sRet += Write("ObjectPath", sParent, m_sObjectPath);
        sRet += Write("Pos", sParent, m_ePos);
        sRet += Write("Weight", sParent, m_iWeight);
        sRet += Write("HumanType", sParent, m_eHumanType);
        sRet += Write("HumanSide", sParent, m_eHumanSide);
        sRet += Write("HumanWeapon", sParent, m_eHumanWeapon);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_sObjectPath = (string)GetElementValue(sTextToParse, sParent, "ObjectPath", m_sObjectPath);
        m_ePos = (EHumanoidComponentPos)GetElementValue(sTextToParse, sParent, "Pos", m_ePos);
        m_iWeight = (int)GetElementValue(sTextToParse, sParent, "Weight", m_iWeight);
        m_eHumanType = (EHumanoidType)GetElementValue(sTextToParse, sParent, "HumanType", m_eHumanType);
        m_eHumanSide = (EHumanoidSide)GetElementValue(sTextToParse, sParent, "HumanSide", m_eHumanSide);
        m_eHumanWeapon = (EHumanoidWeapon)GetElementValue(sTextToParse, sParent, "HumanWeapon", m_eHumanWeapon);
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
