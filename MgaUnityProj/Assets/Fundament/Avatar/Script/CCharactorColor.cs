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

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CCharactorColorElement[] ret = new CCharactorColorElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CCharactorColorElement();
        }
        return ret;
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

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);

        sRet += Write("Color", sParent, m_cColor);
        sRet += Write("IsMain", sParent, m_bMain);
        sRet += Write("MainColor", sParent, m_eMainColor);
        sRet += Write("SubColor", sParent, m_eSubColor);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_cColor = (Color)GetElementValue(sTextToParse, sParent, "Color", m_cColor);
        m_bMain = (bool)GetElementValue(sTextToParse, sParent, "IsMain", m_bMain);
        m_eMainColor = (ECharactorMainColor)GetElementValue(sTextToParse, sParent, "MainColor", m_eMainColor);
        m_eSubColor = (ECharactorSubColor)GetElementValue(sTextToParse, sParent, "SubColor", m_eSubColor);

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
