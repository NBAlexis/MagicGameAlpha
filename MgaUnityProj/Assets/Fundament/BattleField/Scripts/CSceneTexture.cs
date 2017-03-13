using UnityEngine;

public class CSceneTextureElement : CMGDataElement
{
    public int m_iTemplate = 0;
    public bool m_bReflect = false;
    public int m_iRotNumber = 0;
    public int m_iTextureCount = 0;

    public override CMGDataElement GetDefault()
    {
        return new CSceneTextureElement();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CSceneTextureElement[] ret = new CSceneTextureElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CSceneTextureElement();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        return new CSceneTextureElement
        {
            m_iTemplate = m_iTemplate,
            m_bReflect = m_bReflect,
            m_iRotNumber = m_iRotNumber,
            m_iTextureCount = m_iTextureCount,
        };
    }

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);

        sRet += Write("Template", sParent, m_iTemplate);
        sRet += Write("Refl", sParent, m_bReflect);
        sRet += Write("RotNumber", sParent, m_iRotNumber);
        sRet += Write("Count", sParent, m_iTextureCount);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_iTemplate = (int)GetElementValue(sTextToParse, sParent, "Template", m_iTemplate);
        m_bReflect = (bool)GetElementValue(sTextToParse, sParent, "Refl", m_bReflect);
        m_iRotNumber = (int)GetElementValue(sTextToParse, sParent, "RotNumber", m_iRotNumber);
        m_iTextureCount = (int)GetElementValue(sTextToParse, sParent, "Count", m_iTextureCount);
    }
}

public class CSceneTexture : TMGData<CSceneTextureElement>
{
    public string m_sSceneType = "Default";
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/BattleField/Resources/" + m_sSceneType + "SceneTexture";
    }

    public override string GetDefaultLoadPath()
    {
        return m_sSceneType + "SceneTexture";
    }
}


