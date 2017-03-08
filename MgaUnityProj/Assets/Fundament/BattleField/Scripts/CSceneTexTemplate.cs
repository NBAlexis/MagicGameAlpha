using UnityEngine;
using System.Collections;

public class CSceneGroudTemplateElement : CMGDataElement
{
    public Vector4 m_vUV = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);

    public override CMGDataElement GetDefault()
    {
        return new CSceneGroudTemplateElement();
    }

    public override CMGDataElement Copy()
    {
        return new CSceneGroudTemplateElement
        {
            m_vUV = m_vUV,
        };
    }

    override public string GetString()
    {
        string sRet = base.GetString();
        sRet += Write("UV", m_vUV);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_vUV = (Vector4)GetElementValue(sTextToParse, "UV", m_vUV);
    }
}

public class CSceneGroudTemplate : TMGData<CSceneGroudTemplateElement>
{
    public string m_sSceneType = "Default";
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/BattleField/Resources/" + m_sSceneType + "SceneGround";
    }

    public override string GetDefaultLoadPath()
    {
        return m_sSceneType + "SceneGround";
    }
}

