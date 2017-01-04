using UnityEngine;

public class CSceneTemplateElement : CMGDataElement
{
    public string m_sDecoratePath = "";
    public string m_sHeightPath = "";
    public string m_sGroundPath = "";

    override public string GetString()
    {
        string sRet = base.GetString();
        sRet += Write("Decorate", m_sDecoratePath);
        sRet += Write("Height", m_sHeightPath);
        sRet += Write("Ground", m_sGroundPath);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);

        m_sDecoratePath = (string)GetElementValue(sTextToParse, "Decorate", m_sDecoratePath);
        m_sHeightPath = (string)GetElementValue(sTextToParse, "Height", m_sHeightPath);
        m_sGroundPath = (string)GetElementValue(sTextToParse, "Ground", m_sGroundPath);
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
