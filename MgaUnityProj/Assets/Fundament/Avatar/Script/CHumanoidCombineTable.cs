using UnityEngine;

public class CHumanoidCombineTableElement : CMGDataElement
{
    public string[] m_sCombine = new string[0];

    public override CMGDataElement GetDefault()
    {
        return new CHumanoidCombineTableElement();
    }

    public override CMGDataElement Copy()
    {
        CHumanoidCombineTableElement ret = new CHumanoidCombineTableElement();
        m_sCombine.CopyTo(ret.m_sCombine, 0);
        return ret;
    }

    override public string GetString()
    {
        string sRet = base.GetString();
        sRet += Write("Combine", m_sCombine);
        return sRet;
    }

    override public void LoadData(string sTextToParse)
    {
        base.LoadData(sTextToParse);
        m_sCombine = (string[])GetElementValue(sTextToParse, "Combine", m_sCombine);
    }
}

public class CHumanoidCombineTable : TMGData<CHumanoidCombineTableElement>
{
    public override string GetDefaultSavePath()
    {
        return Application.dataPath + "/Fundament/Avatar/Resources/HumanoidCombineTable";
    }

    public override string GetDefaultLoadPath()
    {
        return "HumanoidCombineTable";
    }
}
