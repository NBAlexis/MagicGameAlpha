using UnityEngine;

public class CSceneTemplateElement : CMGDataElement
{
    public string m_sDecoratePath = "";
    public string m_sHeightPath = "";
    public string m_sGroundPath = "";

    public override CMGDataElement GetDefault()
    {
        return new CSceneTemplateElement();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CSceneTemplateElement[] ret = new CSceneTemplateElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CSceneTemplateElement();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        return new CSceneTemplateElement
        {
            m_sDecoratePath = m_sDecoratePath,
            m_sHeightPath = m_sHeightPath,
            m_sGroundPath = m_sGroundPath,
        };
    }

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);
        sRet += Write("Decorate", sParent, m_sDecoratePath);
        sRet += Write("Height", sParent, m_sHeightPath);
        sRet += Write("Ground", sParent, m_sGroundPath);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);

        m_sDecoratePath = (string)GetElementValue(sTextToParse, sParent, "Decorate", m_sDecoratePath);
        m_sHeightPath = (string)GetElementValue(sTextToParse, sParent, "Height", m_sHeightPath);
        m_sGroundPath = (string)GetElementValue(sTextToParse, sParent, "Ground", m_sGroundPath);
    }
}

public class CSceneTemplate : TMGData<CSceneTemplateElement>
{
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/BattleField/Resources/SceneTemplate";
    }

    public override string GetDefaultLoadPath()
    {
        return "SceneTemplate";
    }
}
