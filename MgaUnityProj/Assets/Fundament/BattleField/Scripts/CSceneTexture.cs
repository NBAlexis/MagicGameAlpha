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

    override public string GetString()
    {
        string sRet = base.GetString();

        sRet += Write("Template", m_iTemplate);
        sRet += Write("Refl", m_bReflect);
        sRet += Write("RotNumber", m_iRotNumber);
        sRet += Write("Count", m_iTextureCount);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_iTemplate = (int)GetElementValue(sTextToParse, "Template", m_iTemplate);
        m_bReflect = (bool)GetElementValue(sTextToParse, "Refl", m_bReflect);
        m_iRotNumber = (int)GetElementValue(sTextToParse, "RotNumber", m_iRotNumber);
        m_iTextureCount = (int)GetElementValue(sTextToParse, "Count", m_iTextureCount);
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


