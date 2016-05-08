using UnityEngine;
using System.Collections;

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

    override public string GetString()
    {
        string sRet = base.GetString();
        sRet += Write("Cliff", m_iCliffType);
        sRet += Write("HasOffset", m_bHasGroundOffset);
        sRet += Write("Edge", m_eEdgeType);
        sRet += Write("Rot", m_bCanRot);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_iCliffType = (int)GetElementValue(sTextToParse, "Cliff", m_iCliffType);
        m_bHasGroundOffset = (bool)GetElementValue(sTextToParse, "HasOffset", m_bHasGroundOffset);
        m_eEdgeType = (ESceneEdgeType)GetElementValue(sTextToParse, "Edge", m_eEdgeType);
        m_bCanRot = (bool)GetElementValue(sTextToParse, "Rot", m_bCanRot);
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

