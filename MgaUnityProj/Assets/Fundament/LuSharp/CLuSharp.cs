using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELuaVariableUsage : int
{
    ELVU_Member = 0x00000001,
    ELVU_Local = 0x00000002,
    ELVU_FuncParam = 0x00000004,
    ELVU_Public = 0x00000008,
    ELVU_Const = 0x00000010,
    ELVU_Static = 0x00000020,
    ELVU_Nouse = 0x00000040,
}

public class CLuaVariable
{
    public string m_sName = "";
    public string m_sId = "";
    public ELuaVariableUsage m_eUseage;
    public string m_sType = "";
    public string m_sWeakType = "";

    public bool m_bNeverUsed = false;
    public bool m_bOnlyOneAssign = false;

    public string m_sAssignCode;

    public string GetDeclare()
    {
        return m_sType + " " + m_sName;
    }

    public string GetAssginCode()
    {
        string sAssign = m_sAssignCode + ";";
        if (m_sType == "rlist")
        {
            sAssign = "new object[] __L__" + m_sAssignCode + "__R__;";
        }
        else if (m_sType == "object[]" || (string.IsNullOrEmpty(m_sType) && m_sWeakType == "object[]"))
        {
            string sStrip = m_sAssignCode.Replace("{", "__L__");
            sStrip = sStrip.Replace("}", "__R__");
            sAssign = "new object[] " + sStrip + ";";
        }
        else if (m_sType == "Dictionary<string, CPseduLuaValue>")
        {
            string sStrip = m_sAssignCode.Replace("{", "__L__");
            sStrip = sStrip.Replace("}", "__R__");
            sAssign = "new Dictionary<string, CPseduLuaValue>()\n" + sStrip + ";";
        }
        else if (m_sType == "Dictionary<int, CPseduLuaValue>")
        {
            string sStrip = m_sAssignCode.Replace("{", "__L__");
            sStrip = sStrip.Replace("}", "__R__");
            sAssign = "new Dictionary<int, CPseduLuaValue>(new IntEqualityComparer())\n" + sStrip + ";";
        }
        return sAssign;
    }

    public string GetAssgin()
    {
        if (0 != ((int)ELuaVariableUsage.ELVU_Const & (int)m_eUseage))
        {
            return GetHeader() + m_sType + " " + m_sName + " = " + GetAssginCode();    
        }
        return GetHeader() + "CPseduLuaValue " + m_sName + " = " + GetAssginCode();
    }

    public string GetHeader()
    {
        string sRet = "";
        if (0 != ((int) m_eUseage & (int) ELuaVariableUsage.ELVU_Public))
        {
            sRet = "public ";
        }
        else if (0 != ((int)m_eUseage & (int)ELuaVariableUsage.ELVU_Member))
        {
            sRet = "protected ";
        }

        if (0 != ((int)m_eUseage & (int)ELuaVariableUsage.ELVU_Const))
        {
            if (m_sType == "string" || m_sType == "int" || m_sType == "float" || m_sType == "bool")
            {
                sRet = sRet + "const ";
            }
            else
            {
                sRet = sRet + "static readonly ";    
            }
        }
        else if (0 != ((int)m_eUseage & (int)ELuaVariableUsage.ELVU_Static))
        {
            sRet = sRet + "static ";
        }

        return sRet;
    }
}

public enum EFunctionProperty : int
{
    //Search From Base Class
    ELF_Public = 0x00000001,
    ELF_Private = 0x00000002,
    ELF_Protected = 0x00000004,
    ELF_Override = 0x00000008,

    ELF_Static = 0x00000010,
}

public enum EFunctionReturnType
{
    EFR_Single,
    EFR_List,
    EFR_None,
}

public class CLuaFunction
{
    public string m_sId = "";
    public EFunctionProperty m_eProperty;
    public string m_sReturn = "";

    public string m_sFunctionName = "";
    public string m_sParams = "";
    public string m_sContent = "";
    public int m_iCallNumber = 0;
    public List<CLuaVariable> m_sFunctionParameters = new List<CLuaVariable>();
    public Dictionary<string, CLuaVariable> m_sUsingMemberVariables = new Dictionary<string, CLuaVariable>();
    public Dictionary<string, CLuaVariable> m_sReturns = new Dictionary<string, CLuaVariable>();

    public string GetId()
    {
        string sParams = "";
        for (int i = 0; i < m_sFunctionParameters.Count; ++i)
        {
            sParams += (string.IsNullOrEmpty(sParams) ? "" : ",") + m_sFunctionParameters[i].GetDeclare();
        }
        return string.Format("{3} {0} {1}({2}){4}\n", m_sReturn, m_sFunctionName, sParams, GetPerporty(), "\n{\n" + m_sContent + "\n}\n");
    }

    public string Porperties()
    {
        string sRet = "";

        if (m_sUsingMemberVariables.Count > 0)
        {
            sRet += "local vars=================================\n";
            foreach (KeyValuePair<string, CLuaVariable> kvp in m_sUsingMemberVariables)
            {

                sRet += "vars:" + kvp.Key + " |content:" + kvp.Value.m_sAssignCode + "\n";
            }
        }

        if (m_sReturns.Count > 0)
        {
            sRet += "Returns =================================\n";
            foreach (KeyValuePair<string, CLuaVariable> kvp in m_sReturns)
            {

                sRet += "vars:" + kvp.Key + " |type:" + kvp.Value.m_sType + " |content:" + kvp.Value.m_sAssignCode + "\n";
            }
        }


        return sRet;
    }

    public string GetPerporty()
    {
        string sRet = "";
        if (0 != ((int)m_eProperty & (int)EFunctionProperty.ELF_Public))
        {
            sRet = "public";
        }
        else if (0 != ((int)m_eProperty & (int)EFunctionProperty.ELF_Protected))
        {
            sRet = "protected";
        }
        else if (0 != ((int)m_eProperty & (int)EFunctionProperty.ELF_Private))
        {
            sRet = "private";
        }

        if (0 != ((int)m_eProperty & (int)EFunctionProperty.ELF_Override))
        {
            sRet = sRet + " override";
        }
        else if (0 != ((int)m_eProperty & (int)EFunctionProperty.ELF_Static))
        {
            sRet = "static " + sRet;
        }

        return sRet;
    }
}

public class CLuaComments
{
    public string m_sId;
    public string m_sContent;
    public bool m_bMultiLine = false;

    public string GetContent()
    {
        string sRet = m_sContent.Replace("{", "__L__");
        sRet = sRet.Replace("}", "__R__");
        if (m_bMultiLine)
        {
            return "/*\n" + sRet + "\n*/";
        }
        return "//" + sRet;
    }
}

public class CLuaScript
{
    public string m_sClassName = "";

    #region Inherent

    public string m_sInherent = "";
    public Dictionary<string, CLuaFunction> m_pInherentFunctions = new Dictionary<string, CLuaFunction>();
    public List<string> m_sGatherFunctions = new List<string>();

    #endregion

    #region Comments

    public Dictionary<string, CLuaComments> m_pComments = new Dictionary<string, CLuaComments>();
    public Dictionary<string, string> m_pStringTable = new Dictionary<string, string>();
    public Dictionary<string, string> m_pAllFunctionCalls = new Dictionary<string, string>();

    #endregion

    public Dictionary<string, CLuaVariable> m_pAllMemberVariables = new Dictionary<string, CLuaVariable>();
    public Dictionary<string, CLuaFunction> m_pAllFunctions = new Dictionary<string, CLuaFunction>();

    public string ToTxt()
    {
        string sRet = "";
        sRet += "=============================== inhere =====================================\n";
        sRet += string.Format("Inhere from:{0}\n", m_sInherent);
        foreach (KeyValuePair<string, CLuaFunction> kvp in m_pInherentFunctions)
        {
            sRet += string.Format("Inhere function:{0}\n", kvp.Value.GetId());
        }

        sRet += "=============================== string table =====================================\n";
        foreach (KeyValuePair<string, string> kvp in m_pStringTable)
        {
            sRet += string.Format("{0} = {1}\n", kvp.Key, kvp.Value);
        }

        sRet += "=============================== funccall table =====================================\n";
        foreach (KeyValuePair<string, string> kvp in m_pAllFunctionCalls)
        {
            sRet += string.Format("{0} = {1}\n", kvp.Key, kvp.Value);
        }

        sRet += "=============================== comment =====================================\n";
        foreach (KeyValuePair<string, CLuaComments> kvp in m_pComments)
        {
            sRet += string.Format("Comments id:{0}, content:{1}\n", kvp.Key, kvp.Value.m_sContent);
        }

        sRet += "=============================== function =====================================\n";
        foreach (KeyValuePair<string, CLuaFunction> kvp in m_pAllFunctions)
        {
            sRet += string.Format("function:{0}\n {1}\n", kvp.Value.m_sFunctionName, kvp.Value.Porperties());
            sRet += kvp.Value.GetId();
        }

        sRet += "=============================== members =====================================\n";
        foreach (KeyValuePair<string, CLuaVariable> kvp in m_pAllMemberVariables)
        {
            sRet += string.Format("member:{0}\n", kvp.Value.GetAssgin());
        }

        return sRet;
    }

    public string ToCS(string sFile, bool bDebug)
    {
        return "";
    }
}
