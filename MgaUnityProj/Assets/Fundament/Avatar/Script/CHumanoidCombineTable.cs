using UnityEngine;

public class CHumanoidCombineTableElement : CMGDataElement
{
    public string[] m_sCombine = new string[0];

    public override CMGDataElement GetDefault()
    {
        return new CHumanoidCombineTableElement();
    }

    public override CMGDataElement[] GetDefaultList(int iLength)
    {
        CHumanoidCombineTableElement[] ret = new CHumanoidCombineTableElement[iLength];
        for (int i = 0; i < iLength; ++i)
        {
            ret[i] = new CHumanoidCombineTableElement();
        }
        return ret;
    }

    public override CMGDataElement Copy()
    {
        CHumanoidCombineTableElement ret = new CHumanoidCombineTableElement();
        m_sCombine.CopyTo(ret.m_sCombine, 0);
        return ret;
    }

    override public string GetString(string sParent)
    {
        string sRet = base.GetString(sParent);
        sRet += Write("Combine", sParent, m_sCombine);
        return sRet;
    }

    override public void LoadData(string sTextToParse, string sParent)
    {
        base.LoadData(sTextToParse, sParent);
        m_sCombine = (string[])GetElementValue(sTextToParse, sParent, "Combine", m_sCombine);
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
