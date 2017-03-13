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

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CSceneDecorateElement[] ret = new CSceneDecorateElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CSceneDecorateElement();
        }
        return ret;
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

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);
        sRet += Write("Repeat", sParent, m_iDecrateRepeat);
        sRet += Write("Size", sParent, m_iDecrateSize);
        sRet += Write("Block", sParent, m_bBlockPathfinding);
        sRet += Write("RotateY", sParent, m_bOnlyRotateY);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_iDecrateRepeat = (int)GetElementValue(sTextToParse, sParent, "Repeat", m_iDecrateRepeat);
        m_iDecrateSize = (int)GetElementValue(sTextToParse, sParent, "Size", m_iDecrateSize);
        m_bBlockPathfinding = (bool)GetElementValue(sTextToParse, sParent, "Block", m_bBlockPathfinding);
        m_bOnlyRotateY = (bool)GetElementValue(sTextToParse, sParent, "RotateY", m_bOnlyRotateY);
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
