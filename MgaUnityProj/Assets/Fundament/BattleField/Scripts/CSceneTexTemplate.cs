using UnityEngine;

public class CSceneGroudTemplateElement : CMGDataElement
{
    public Vector4 m_vUV = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);

    public override CMGDataElement GetDefault()
    {
        return new CSceneGroudTemplateElement();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CSceneGroudTemplateElement[] ret = new CSceneGroudTemplateElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CSceneGroudTemplateElement();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        return new CSceneGroudTemplateElement
        {
            m_vUV = m_vUV,
        };
    }

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);
        sRet += Write("UV", sParent, m_vUV);

        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_vUV = (Vector4)GetElementValue(sTextToParse, sParent, "UV", m_vUV);
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

