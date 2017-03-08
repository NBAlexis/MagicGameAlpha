using UnityEngine;

public enum ECharactorMainColor : byte
{
    ECMC_1,
    ECMC_2,
    ECMC_3,
    ECMC_4,

    ECMC_Max,
}

public enum ECharactorSubColor : byte
{
    ECSC_1,
    ECSC_2,
    ECSC_3,
    ECSC_4,

    ECSC_Max,
}

public class CCharactorColorElement : CMGDataElement
{
    public Color m_cColor = Color.white;
    public bool m_bMain = false;
    public ECharactorMainColor m_eMainColor = ECharactorMainColor.ECMC_1;
    public ECharactorSubColor m_eSubColor = ECharactorSubColor.ECSC_1;

    public override CMGDataElement GetDefault()
    {
        return new CCharactorColorElement();
    }

    public override CMGDataElement Copy()
    {
        return new CCharactorColorElement
        {
            m_cColor = m_cColor,
            m_bMain = m_bMain,
            m_eMainColor = m_eMainColor,
            m_eSubColor = m_eSubColor
        };
    }

    override public string GetString()
    {
        string sRet = base.GetString();

        sRet += Write("Color", m_cColor);
        sRet += Write("IsMain", m_bMain);
        sRet += Write("MainColor", m_eMainColor);
        sRet += Write("SubColor", m_eSubColor);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_cColor = (Color)GetElementValue(sTextToParse, "Color", m_cColor);
        m_bMain = (bool)GetElementValue(sTextToParse, "IsMain", m_bMain);
        m_eMainColor = (ECharactorMainColor)GetElementValue(sTextToParse, "MainColor", m_eMainColor);
        m_eSubColor = (ECharactorSubColor)GetElementValue(sTextToParse, "SubColor", m_eSubColor);

        if (m_bMain)
        {
            m_sElementName = string.Format("M{0}", (int) m_eMainColor + 1);
        }
        else
        {
            m_sElementName = string.Format("S{0}",  (int)m_eSubColor + 1);
        }
    }
}

public class CCharactorColor : TMGData<CCharactorColorElement>
{
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/Avatar/Resources/CharactorColor";
    }

    public override string GetDefaultLoadPath()
    {
        return "CharactorColor";
    }
}
