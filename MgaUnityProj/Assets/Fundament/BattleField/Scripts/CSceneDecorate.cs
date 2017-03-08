using UnityEngine;

public class CSceneDecorateElement : CMGDataElement
{
    public int m_iDecrateRepeat = 1;
    public int m_iDecrateSize = 1;
    public bool m_bBlockPathfinding = false;
    public bool m_bOnlyRotateY = false;

    public override CMGDataElement GetDefault()
    {
        return new CSceneDecorateElement();
    }

    public override CMGDataElement Copy()
    {
        return new CSceneDecorateElement
        {
            m_iDecrateRepeat = m_iDecrateRepeat,
            m_iDecrateSize = m_iDecrateSize,
            m_bBlockPathfinding = m_bBlockPathfinding,
            m_bOnlyRotateY = m_bOnlyRotateY
        };
    }

    override public string GetString()
    {
        string sRet = base.GetString();
        sRet += Write("Repeat", m_iDecrateRepeat);
        sRet += Write("Size", m_iDecrateSize);
        sRet += Write("Block", m_bBlockPathfinding);
        sRet += Write("RotateY", m_bOnlyRotateY);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_iDecrateRepeat = (int)GetElementValue(sTextToParse, "Repeat", m_iDecrateRepeat);
        m_iDecrateSize = (int)GetElementValue(sTextToParse, "Size", m_iDecrateSize);
        m_bBlockPathfinding = (bool)GetElementValue(sTextToParse, "Block", m_bBlockPathfinding);
        m_bOnlyRotateY = (bool)GetElementValue(sTextToParse, "RotateY", m_bOnlyRotateY);
    }
}

public class CSceneDecorate : TMGData<CSceneDecorateElement>
{
    public string m_sSceneType = "Default";
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/BattleField/Resources/" + m_sSceneType + "SceneDecorate";
    }

    public override string GetDefaultLoadPath()
    {
        return m_sSceneType + "SceneDecorate";
    }
}
