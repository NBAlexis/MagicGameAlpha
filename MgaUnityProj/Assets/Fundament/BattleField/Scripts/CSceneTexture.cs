using UnityEngine;

public class CSceneTextureElement : CMGDataElement
{
    public int m_iTemplate = 0;
    public int m_iTextureCount = 0;
    public bool m_bCanRot = false;
    public Vector4 m_rcUVRect = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
    public int m_iRotNumber = 0;

    override public string GetString()
    {
        string sRet = base.GetString();

        sRet += Write("Template", m_iTemplate);
        sRet += Write("TextureCount", m_iTextureCount);
        sRet += Write("CanRot", m_bCanRot);
        sRet += Write("UVRect", m_rcUVRect);
        sRet += Write("RotNumber", m_iRotNumber);

        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_iTemplate = (int)GetElementValue(sTextToParse, "Template", m_iTemplate);
        m_iTextureCount = (int)GetElementValue(sTextToParse, "TextureCount", m_iTextureCount);
        m_bCanRot = (bool)GetElementValue(sTextToParse, "CanRot", m_bCanRot);
        m_rcUVRect = (Vector4)GetElementValue(sTextToParse, "UVRect", m_rcUVRect);
        m_iRotNumber = (int)GetElementValue(sTextToParse, "RotNumber", m_iRotNumber);

    }
}

public class CSceneTexture : TMGData<CSceneTextureElement>
{
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/BattleField/Resources/SceneTexture";
    }

    public override string GetDefaultLoadPath()
    {
        return "SceneTexture";
    }
}
