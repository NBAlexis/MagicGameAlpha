using UnityEngine;

public enum ESceneEdgeType
{
    ESET_Water,
    ESET_Lava,
    ESET_Cave,
    ESET_Clif,
    ESET_Ground,
}

public class CSceneTypeElement : CMGDataElement
{
    public int m_iCliffType = 0;
    public bool m_bHasGroundOffset = false;
    public bool m_bCanRot = false;
    public ESceneEdgeType m_eEdgeType = ESceneEdgeType.ESET_Water;

    public override CMGDataElement GetDefault()
    {
        return new CSceneTypeElement();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CSceneTypeElement[] ret = new CSceneTypeElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CSceneTypeElement();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        return new CSceneTypeElement
        {
            m_iCliffType = m_iCliffType,
            m_bHasGroundOffset = m_bHasGroundOffset,
            m_bCanRot = m_bCanRot,
            m_eEdgeType = m_eEdgeType,
        };
    }

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);
        sRet += Write("Cliff", sParent, m_iCliffType);
        sRet += Write("HasOffset", sParent, m_bHasGroundOffset);
        sRet += Write("Edge", sParent, m_eEdgeType);
        sRet += Write("Rot", sParent, m_bCanRot);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_iCliffType = (int)GetElementValue(sTextToParse, sParent, "Cliff", m_iCliffType);
        m_bHasGroundOffset = (bool)GetElementValue(sTextToParse, sParent, "HasOffset", m_bHasGroundOffset);
        m_eEdgeType = (ESceneEdgeType)GetElementValue(sTextToParse, sParent, "Edge", m_eEdgeType);
        m_bCanRot = (bool)GetElementValue(sTextToParse, sParent, "Rot", m_bCanRot);
    }
}

public class CSceneType : TMGData<CSceneTypeElement>
{
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/BattleField/Resources/" + "SceneType";
    }

    public override string GetDefaultLoadPath()
    {
        return "SceneType";
    }
}

