using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

#pragma warning disable 1570

public class LuSharpConvert
{
    /// <summary>
    /// Gather all --[[]] and --
    /// use the first "--" to check inherent info
    /// </summary>
    /// <param name="sContent"></param>
    /// <param name="script"></param>
    public static void GetInherentAndHead(ref string sContent, ref CLuaScript script)
    {
        //Multi Line Comment
        int iCommentId = 0;
        MatchCollection mat1 = Regex.Matches(sContent, @"--\[\[[\s\S]*?(?<=\]\])");
        foreach (Match match in mat1)
        {
            string sCommentContent = match.Value.Replace("--[[", "").Replace("]]", "");
            CLuaComments comment = new CLuaComments {m_bMultiLine = true, m_sContent = sCommentContent};
            comment.m_sId = "__comment" + iCommentId + "__";
            sContent = sContent.Replace(match.Value, comment.m_sId);
            ++iCommentId;
            script.m_pComments.Add(comment.m_sId, comment);
        }

        //One Line Comment
        MatchCollection mat = Regex.Matches(sContent, @"--[^\n]*\n");
        bool bFirst = false;
        foreach (Match match in mat)
        {
            string sCommentContent = match.Value.Replace("--", "").Replace("\n", "");
            if (!bFirst)
            {
                bFirst = true;
                foreach (Type type in Assembly.GetAssembly(typeof(CLuSharpExcute)).GetTypes())
                {
                    if (type.Name.Equals(sCommentContent))
                    {
                        script.m_sInherent = sCommentContent;

                        foreach (MethodInfo methodinfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            if (methodinfo.IsVirtual || methodinfo.IsAbstract)
                            {
                                CLuaFunction func = new CLuaFunction
                                {
                                    m_sFunctionName = methodinfo.Name,
                                    m_sReturn = DecideTypeString(methodinfo.ReturnType)
                                };
                                if (methodinfo.IsPublic)
                                {
                                    func.m_eProperty = EFunctionProperty.ELF_Public;
                                }
                                else if (methodinfo.IsPrivate)
                                {
                                    func.m_eProperty = EFunctionProperty.ELF_Private;
                                }
                                else if (methodinfo.IsFamily)
                                {
                                    func.m_eProperty = EFunctionProperty.ELF_Protected;
                                }

                                ParameterInfo[] callparams = methodinfo.GetParameters();
                                int paramNumber = callparams.Length;
                                for (int i = 0; i < paramNumber; ++i)
                                {
                                    CLuaVariable param = new CLuaVariable
                                    {
                                        m_eUseage = ELuaVariableUsage.ELVU_FuncParam,
                                        m_sType = DecideTypeString(callparams[i].ParameterType)
                                    };
                                    func.m_sFunctionParameters.Add(param);
                                }
                                script.m_pInherentFunctions.Add(methodinfo.Name, func);
                            }
                        }
                    }
                }
            }

            CLuaComments comment = new CLuaComments { m_bMultiLine = false, m_sContent = sCommentContent };
            comment.m_sId = "__comment" + iCommentId + "__";
            sContent = sContent.Replace(match.Value, comment.m_sId);
            ++iCommentId;
            script.m_pComments.Add(comment.m_sId, comment);
        }
    }

    /// <summary>
    /// for ""{"" ""["" will complicate parse
    /// change ""xxxxx"" to ""__string_1__""..
    /// </summary>
    /// <param name="sContent"></param>
    /// <param name="script"></param>
    public static void GetStringContent(ref string sContent, ref CLuaScript script)
    {
        sContent = sContent.Replace("\\\"", "__slash_quoat__");
        MatchCollection matches = Regex.Matches(sContent, "\\\"([^\\\"]*?)\\\"");
        int iIndex = 0;
        for (int i = 0; i < matches.Count; ++i)
        {
            string sId = "__stringt" + iIndex + "__";
            ++iIndex;
            script.m_pStringTable.Add(sId, matches[i].Groups[1].Value.Replace("__slash_quoat__", "\\\""));
            sContent = sContent.Replace(matches[i].Value, "\"" + sId + "\"");
        }
    }

    /// <summary>
    /// things like CMyClass.Get("abc", 1.2f)
    /// will ruin the parse
    /// change it to "__callf_func_1__"
    /// </summary>
    /// <param name="script"></param>
    public static void GetFunctionVariables(ref CLuaScript script)
    {
        int iStart = 0;
        foreach (KeyValuePair<string, CLuaFunction> kvp in script.m_pAllFunctions)
        {
            string sContent = kvp.Value.m_sContent;
            bool bChanged = true;
            while (bChanged)
            {
                string sNewContent = sContent;
                foreach (Match match in Regex.Matches(sNewContent, @"([^\w])([\w_][\w\d_]*([\s]*(\([^\(^\)]*\))|([\s]*\[[^\[\]]*\])))"))
                {
                    string sId = "__callf_" + "_" + iStart + "__";
                    ++iStart;
                    script.m_pAllFunctionCalls.Add(sId, match.Groups[2].Value);
                    sNewContent = sNewContent.Replace(match.Groups[2].Value, sId);
                }
                bChanged = sNewContent != sContent;
                sContent = sNewContent;
            }

            bChanged = true;
            while (bChanged)
            {
                string sNewContent = sContent;
                foreach (Match match in Regex.Matches(sNewContent, @"[\w_][\w\d_]*[\s]*(\.[\s]*[\w_][\w\d_]*)+"))
                {
                    string sId = "__callf_" + "_" + iStart + "__";
                    ++iStart;
                    script.m_pAllFunctionCalls.Add(sId, match.Value);
                    sNewContent = sNewContent.Replace(match.Value, sId);
                }
                bChanged = sNewContent != sContent;
                sContent = sNewContent;
            }
            kvp.Value.m_sContent = sContent;
        }
    }

    /// <summary>
    /// Gather all "function xxx end", and "local function xx end"
    /// </summary>
    /// <param name="sContent"></param>
    /// <param name="script"></param>
    public static void GetFunctions(ref string sContent, ref CLuaScript script)
    {
        sContent = Regex.Replace(sContent, @"(\s)(local[\s]+function)(\s)", "$1__lua_local_FFunction__$3");
        sContent = Regex.Replace(sContent, @"(\s)(function)(\s)", "$1__lua_FFunction__$3");
        int iFunctionStart = sContent.IndexOf("__lua_local_FFunction__", StringComparison.Ordinal);
        int iFunctionIndex = 0;
        while (iFunctionStart >= 0)
        {
            int iEnd1 = sContent.IndexOf("__lua_local_FFunction__", iFunctionStart + 1, StringComparison.Ordinal);
            int iEnd2 = sContent.IndexOf("__lua_FFunction__", iFunctionStart + 1, StringComparison.Ordinal);
            iEnd1 = iEnd1 < 0 ? sContent.Length : iEnd1;
            iEnd2 = iEnd2 < 0 ? sContent.Length : iEnd2;
            int iNextFunction = Mathf.Min(iEnd1, iEnd2);
            string sFunctionBody = sContent.Substring(iFunctionStart, iNextFunction - iFunctionStart);
            MatchCollection fncbody = Regex.Matches(sFunctionBody, @"[\s\S]*[\s]+end[\s]+");
            if (fncbody.Count < 1)
            {
                CRuntimeLogger.LogError("Function Body Not Match! in " + script.m_sClassName);
                return;
            }
            sFunctionBody = fncbody[0].Value;

            MatchCollection match = Regex.Matches(sFunctionBody, @"__lua_local_FFunction__[\s]+[\w\d_]+[\s]?[\(][\w\d_\s\,]*[\)]");
            if (match.Count < 1)
            {
                CRuntimeLogger.LogError("Function Body Not Match! in " + script.m_sClassName);
                return;
            }
            string sName = Regex.Replace(match[0].Value, @"__lua_local_FFunction__[\s]+([\w\d_]+)[\s]?[\(]([\w\d_\s\,]*)[\)]", "$1");
            string sParams = Regex.Replace(match[0].Value, @"__lua_local_FFunction__[\s]+([\w\d_]+)[\s]?[\(]([\w\d_\s\,]*)[\)]", "$2");
            sParams = Regex.Replace(sParams, @"\s", "");
            string sFuncContent = sFunctionBody.Replace(match[0].Value, "");
            int iLastEnd = sFuncContent.LastIndexOf("end", StringComparison.Ordinal);
            if (iLastEnd < 0)
            {
                CRuntimeLogger.LogError("Function Body Not Match! in " + script.m_sClassName);
                return;
            }
            sFuncContent = sFuncContent.Substring(0, iLastEnd);

            CLuaFunction func = new CLuaFunction
            {
                m_eProperty = EFunctionProperty.ELF_Protected,
                m_sFunctionName = sName,
                m_sParams = sParams,
                m_sContent = sFuncContent,
                m_sId = "__thefunction" + iFunctionIndex + "__"
            };

            #region Check Inherent

            if (script.m_pInherentFunctions.ContainsKey(sName))
            {
                func.m_eProperty = (EFunctionProperty)((int)script.m_pInherentFunctions[sName].m_eProperty | (int)EFunctionProperty.ELF_Override);
                string[] functionParams = new string[0];
                if (!string.IsNullOrEmpty(sParams))
                {
                    functionParams = sParams.Split(',');
                }
                if (functionParams.Length != script.m_pInherentFunctions[sName].m_sFunctionParameters.Count)
                {
                    CRuntimeLogger.LogError("Function Params Not Match Inherent!");
                    return;
                }

                for (int i = 0; i < script.m_pInherentFunctions[sName].m_sFunctionParameters.Count; ++i)
                {
                    func.m_sFunctionParameters.Add(new CLuaVariable
                    {
                        m_eUseage = ELuaVariableUsage.ELVU_FuncParam,
                        m_sName = functionParams[i],
                        m_sType = script.m_pInherentFunctions[sName].m_sFunctionParameters[i].m_sType
                    });
                }
                func.m_sReturn = script.m_pInherentFunctions[sName].m_sReturn;
            }
            else
            {
                string[] functionParams = new string[0];
                if (!string.IsNullOrEmpty(sParams))
                {
                    functionParams = sParams.Split(',');
                }

                for (int i = 0; i < functionParams.Length; ++i)
                {
                    func.m_sFunctionParameters.Add(new CLuaVariable
                    {
                        m_eUseage = ELuaVariableUsage.ELVU_FuncParam,
                        m_sName = functionParams[i],
                        m_sType = ""
                    });
                }
            }

            #endregion

            if (script.m_pAllFunctions.ContainsKey(sName))
            {
                CRuntimeLogger.LogError("Duplicated function:" + script.m_sClassName + "." + sName);
            }
            else
            {
                script.m_pAllFunctions.Add(sName, func);    
            }

            sContent = sContent.Replace(sFunctionBody, "__thefunction" + iFunctionIndex + "__\n");
            ++iFunctionIndex;
            iFunctionStart = sContent.IndexOf("__lua_local_FFunction__", StringComparison.Ordinal);
        }

        iFunctionStart = sContent.IndexOf("__lua_FFunction__", StringComparison.Ordinal);
        while (iFunctionStart >= 0)
        {
            int iEnd1 = sContent.IndexOf("__lua_local_FFunction__", iFunctionStart + 1, StringComparison.Ordinal);
            int iEnd2 = sContent.IndexOf("__lua_FFunction__", iFunctionStart + 1, StringComparison.Ordinal);
            iEnd1 = iEnd1 < 0 ? sContent.Length : iEnd1;
            iEnd2 = iEnd2 < 0 ? sContent.Length : iEnd2;
            int iNextFunction = Mathf.Min(iEnd1, iEnd2);
            string sFunctionBody = sContent.Substring(iFunctionStart, iNextFunction - iFunctionStart);
            MatchCollection fncbody = Regex.Matches(sFunctionBody, @"[\s\S]*[\s]+end[\s]+");
            if (fncbody.Count < 1)
            {
                CRuntimeLogger.LogError("Function Body Not Match! in " + script.m_sClassName);
                return;
            }
            sFunctionBody = fncbody[0].Value;

            MatchCollection match = Regex.Matches(sFunctionBody, @"__lua_FFunction__[\s]+[\w\d_]+[\s]?[\(][\w\d_\s\,]*[\)]");
            if (match.Count < 1)
            {
                CRuntimeLogger.LogError("Function Body Not Match! in " + script.m_sClassName);
                return;
            }
            string sName = Regex.Replace(match[0].Value, @"__lua_FFunction__[\s]+([\w\d_]+)[\s]?[\(]([\w\d_\s\,]*)[\)]", "$1");
            string sParams = Regex.Replace(match[0].Value, @"__lua_FFunction__[\s]+([\w\d_]+)[\s]?[\(]([\w\d_\s\,]*)[\)]", "$2");
            sParams = Regex.Replace(sParams, @"\s", "");

            string sFuncContent = sFunctionBody.Replace(match[0].Value, "");
            int iLastEnd = sFuncContent.LastIndexOf("end", StringComparison.Ordinal);
            if (iLastEnd < 0)
            {
                CRuntimeLogger.LogError("Function Body Not Match! in " + script.m_sClassName);
                return;
            }
            sFuncContent = sFuncContent.Substring(0, iLastEnd);

            CLuaFunction func = new CLuaFunction
            {
                m_eProperty = EFunctionProperty.ELF_Public,
                m_sFunctionName = sName,
                m_sParams = sParams,
                m_sContent = sFuncContent,
                m_sId = "__thefunction" + iFunctionIndex + "__"
            };

            #region Check Inherent

            if (script.m_pInherentFunctions.ContainsKey(sName))
            {
                func.m_eProperty = (EFunctionProperty)((int)script.m_pInherentFunctions[sName].m_eProperty | (int)EFunctionProperty.ELF_Override);
                string[] functionParams = new string[0];
                if (!string.IsNullOrEmpty(sParams))
                {
                    functionParams = sParams.Split(',');
                }
                if (functionParams.Length != script.m_pInherentFunctions[sName].m_sFunctionParameters.Count)
                {
                    CRuntimeLogger.LogError("Function Params Not Match Inherent! in " + script.m_sClassName + " function:" + sName);
                    return;
                }

                for (int i = 0; i < script.m_pInherentFunctions[sName].m_sFunctionParameters.Count; ++i)
                {
                    func.m_sFunctionParameters.Add(new CLuaVariable
                    {
                        m_eUseage = ELuaVariableUsage.ELVU_FuncParam,
                        m_sName = functionParams[i],
                        m_sType = script.m_pInherentFunctions[sName].m_sFunctionParameters[i].m_sType
                    });
                }
                func.m_sReturn = script.m_pInherentFunctions[sName].m_sReturn;
            }
            else
            {
                string[] functionParams = new string[0];
                if (!string.IsNullOrEmpty(sParams))
                {
                    functionParams = sParams.Split(',');
                }

                for (int i = 0; i < functionParams.Length; ++i)
                {
                    func.m_sFunctionParameters.Add(new CLuaVariable
                    {
                        m_eUseage = ELuaVariableUsage.ELVU_FuncParam,
                        m_sName = functionParams[i],
                        m_sType = ""
                    });
                }
            }

            #endregion

            script.m_pAllFunctions.Add(sName, func);

            sContent = sContent.Replace(sFunctionBody, "__thefunction" + iFunctionIndex + "__\n");
            ++iFunctionIndex;
            iFunctionStart = sContent.IndexOf("__lua_FFunction__", StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Gather all "xxx = yyyy" outside functions
    /// </summary>
    /// <param name="sContent"></param>
    /// <param name="script"></param>
    public static void GetMembers(ref string sContent, ref CLuaScript script)
    {
        #region List and Dictionary

        //local xx = {[] = {}}
        MatchCollection matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{([\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})*[\,]?[\s]*\})");
        int iMemberIndex = 0;
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sWeakType = "Dictionary<object,object[]>";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //local xx = {[] = x}
        matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{([\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)*\})");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sWeakType = "Dictionary<object,object>";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //local xx = { ... }
        matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{[\s\w\d_\,\.\" + "\"" + @"]*\})");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sWeakType = "object[]";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //xx = {[] = {}}
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{([\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})*[\,]?[\s]*\})");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sWeakType = "Dictionary<object,object[]>";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //x = {[a] = b}
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{([\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)*\})");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sWeakType = "Dictionary<object,object>";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        // x={}
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{[\s\w\d_\,\.\" + "\"" + @"]*\})");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sWeakType = "object[]";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        #endregion

        #region Simple

        //local a = 1
        matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\d]+)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sType = "int";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //local a = 1.0
        matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\d]+.[\d]+)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value + "f";
            vari.m_sType = "float";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //local a = true
        matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(true|false)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value + "f";
            vari.m_sType = "bool";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //local a = "a"(.."b")
        matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*((\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")([\s]*\.\.[\s]*\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")*)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sAssignCode = Regex.Replace(vari.m_sAssignCode, @"[\s]*\.\.[\s]*", " + ");
            vari.m_sType = "string";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Member;
            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //a = 1
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\d]+)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sType = "int";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //a = 1.0
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\d]+.[\d]+)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value + "f";
            vari.m_sType = "float";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //a = true
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*(true|false)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value + "f";
            vari.m_sType = "bool";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        //a = "a"(.."b")
        matches = Regex.Matches(sContent, @"\b([a-zA-Z_][\w\d]*)[\s]*=[\s]*((\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")([\s]*\.\.[\s]*\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")*)[\s]+");
        for (int i = 0; i < matches.Count; ++i)
        {
            CLuaVariable vari = new CLuaVariable();
            vari.m_sName = matches[i].Groups[1].Value;
            vari.m_sAssignCode = matches[i].Groups[2].Value;
            vari.m_sAssignCode = Regex.Replace(vari.m_sAssignCode, @"[\s]*\.\.[\s]*", " + ");
            vari.m_sType = "string";
            vari.m_sId = "__luavariable" + iMemberIndex + "__";
            vari.m_eUseage = ELuaVariableUsage.ELVU_Public;

            if (script.m_pAllMemberVariables.ContainsKey(vari.m_sName))
            {
                CRuntimeLogger.LogError("Duplicated define:" + script.m_sClassName + "." + vari.m_sName);
            }
            else
            {
                script.m_pAllMemberVariables.Add(vari.m_sName, vari);
            }

            sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + iMemberIndex + "__\n");
            ++iMemberIndex;
        }

        #endregion
    }

    /// <summary>
    /// Gather all local xx = xx, in function body
    /// </summary>
    /// <param name="script"></param>
    public static void GetFunctionLocalVariables(ref CLuaScript script)
    {
        foreach (KeyValuePair<string, CLuaFunction> kvp in script.m_pAllFunctions)
        {
            string sContent = kvp.Value.m_sContent;
            string sFunction = kvp.Value.m_sFunctionName;

            #region List and Dictionary
            
            //local xx = {[] = {}}
            MatchCollection matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{([\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})*[\,]?[\s]*\})");
            int iMemberIndex = 0;
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sWeakType = "Dictionary<object,object[]>";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            //local xx = {[] = x}
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{([\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)*\})");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sWeakType = "Dictionary<object,object>";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            //local xx = { ... }
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(\{[\s\w\d_\,\.\" + "\"" + @"]*\})");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sWeakType = "object[]";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            #endregion

            #region Simple

            //local a = 1
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\d]+)[\s]");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sType = "int";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            //local a = 1.0
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\d]+.[\d]+)[\s]");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value + "f";
                vari.m_sType = "float";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            //local a = true
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*(true|false)[\s]");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sType = "bool";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            //local a = "a"(.."b")
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*((\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")([\s]*\.\.[\s]*\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")*)[\s]");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sAssignCode = Regex.Replace(vari.m_sAssignCode, @"[\s]*\.\.[\s]*", " + ");
                vari.m_sType = "string";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            //local a = b
            matches = Regex.Matches(sContent, @"\blocal[\s]+([a-zA-Z_][\w\d]*)[\s]*=[\s]*([\w_][\w\d_]*)[\s]");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = matches[i].Groups[1].Value;
                vari.m_sAssignCode = matches[i].Groups[2].Value;
                vari.m_sType = "";
                vari.m_sId = "__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                if (kvp.Value.m_sUsingMemberVariables.ContainsKey(vari.m_sName))
                {
                    CRuntimeLogger.LogError("Duplicated define:" + vari.m_sName + " in function:" + script.m_sClassName + "." + sFunction);
                }
                else
                {
                    kvp.Value.m_sUsingMemberVariables.Add(vari.m_sName, vari);
                }

                sContent = sContent.Replace(matches[i].Value, "\n__luavariable" + "_" + sFunction + "_" + iMemberIndex + "__\n");
                ++iMemberIndex;
            }

            #endregion

            kvp.Value.m_sContent = sContent;
        }
    }

    /// <summary>
    /// things like return, return a,b ect..
    /// </summary>
    /// <param name="script"></param>
    public static void GetReturns(ref CLuaScript script)
    {
        foreach (KeyValuePair<string, CLuaFunction> kvp in script.m_pAllFunctions)
        {
            string sContent = kvp.Value.m_sContent;
            string sFunction = kvp.Value.m_sFunctionName;

            //return 
            MatchCollection matches = Regex.Matches(sContent, @"\breturn[\s]*?\n");
            int iReturnIndex = 0;
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = "";
                vari.m_sType = "void";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            #region List

            //return { [] = {..} }
            matches = Regex.Matches(sContent, @"\breturn[\s]+(\{([\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*\{[^{}]+?\})*[\,]?[\s]*\})");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sWeakType = "Dictionary<object,object[]>";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return { [] = ... }
            matches = Regex.Matches(sContent, @"\breturn[\s]+(\{([\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)([\s]*\,[\s]*\[[^\]]+\][\s]*=[\s]*[^{}]+?)*\})");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sWeakType = "Dictionary<object,object>";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return {}
            matches = Regex.Matches(sContent, @"\breturn[\s]+(\{[\s\w\d_\,\.\" + "\"" + @"]*\})");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sWeakType = "object[]";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            #endregion

            //return 1
            matches = Regex.Matches(sContent, @"\breturn[\s]+([\d]+)[\s]*\n");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sType = "int";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return 1.1
            matches = Regex.Matches(sContent, @"\breturn[\s]+([\d]+\.[\d]+)[\s]*\n");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value + "f";
                vari.m_sType = "float";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return true
            matches = Regex.Matches(sContent, @"\breturn[\s]+(true|false)[\s]*\n");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sType = "bool";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return "a".."b"
            matches = Regex.Matches(sContent, @"\breturn[\s]+((\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")([\s]*\.\.[\s]*\" + "\"" + @"[^\" + "\"" + @"]*\" + "\"" + @")*)[\s]");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value + "f";
                vari.m_sType = "string";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return a
            matches = Regex.Matches(sContent, @"\breturn[\s]+([\w_][\w\d_]*)[\s]*\n");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sType = "";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            //return a,b,c[,]
            matches = Regex.Matches(sContent, @"\breturn[\s]+([^\n]*?)[\s]*\n");
            for (int i = 0; i < matches.Count; ++i)
            {
                CLuaVariable vari = new CLuaVariable();
                vari.m_sName = "";
                vari.m_sAssignCode = matches[i].Groups[1].Value;
                vari.m_sType = "rlist";
                vari.m_sId = "__return" + "_" + sFunction + "_" + iReturnIndex + "__";
                vari.m_eUseage = ELuaVariableUsage.ELVU_Local;

                kvp.Value.m_sReturns.Add(vari.m_sId, vari);

                sContent = sContent.Replace(matches[i].Value, "\n" + vari.m_sId + "\n");
                ++iReturnIndex;
            }

            kvp.Value.m_sContent = sContent;
        }

        foreach (KeyValuePair<string, CLuaFunction> kvp in script.m_pAllFunctions)
        {
            bool bNoVoid = false;
            foreach (KeyValuePair<string, CLuaVariable> kvpr in kvp.Value.m_sReturns)
            {
                if (kvpr.Value.m_sType != "void")
                {
                    bNoVoid = true;
                    break;
                }
            }
            if (!bNoVoid)
            {
                kvp.Value.m_sReturn = "void";
            }
        }
    }

    /// <summary>
    /// change Dictionary<object,object[]> to Dictionary<int,object[]> or Dictionary<string,object[]>
    /// </summary>
    /// <param name="script"></param>
    public static void CheckDictionaryType(ref CLuaScript script)
    {
        foreach (KeyValuePair<string, CLuaVariable> kvp in script.m_pAllMemberVariables)
        {
            if (string.IsNullOrEmpty(kvp.Value.m_sType) && !string.IsNullOrEmpty(kvp.Value.m_sWeakType) && kvp.Value.m_sWeakType == "Dictionary<object,object[]>")
            {
                string sKey = DecideKeyType(ref kvp.Value.m_sAssignCode, script, null, true);
                kvp.Value.m_sType = "Dictionary<" + sKey + ", CPseduLuaValue>";
            }
            if (string.IsNullOrEmpty(kvp.Value.m_sType) && !string.IsNullOrEmpty(kvp.Value.m_sWeakType) && kvp.Value.m_sWeakType == "Dictionary<object,object>")
            {
                string sKey = DecideKeyType(ref kvp.Value.m_sAssignCode, script, null, false);
                kvp.Value.m_sType = "Dictionary<" + sKey + ", CPseduLuaValue>";
            }
        }

        foreach (KeyValuePair<string, CLuaFunction> kvpf in script.m_pAllFunctions)
        {
            foreach (KeyValuePair<string, CLuaVariable> kvp in kvpf.Value.m_sUsingMemberVariables)
            {
                if (string.IsNullOrEmpty(kvp.Value.m_sType) && !string.IsNullOrEmpty(kvp.Value.m_sWeakType) && kvp.Value.m_sWeakType == "Dictionary<object,object[]>")
                {
                    string sKey = DecideKeyType(ref kvp.Value.m_sAssignCode, script, kvpf.Value, true);
                    kvp.Value.m_sType = "Dictionary<" + sKey + ", CPseduLuaValue>";
                }
                if (string.IsNullOrEmpty(kvp.Value.m_sType) && !string.IsNullOrEmpty(kvp.Value.m_sWeakType) && kvp.Value.m_sWeakType == "Dictionary<object,object>")
                {
                    string sKey = DecideKeyType(ref kvp.Value.m_sAssignCode, script, kvpf.Value, false);
                    kvp.Value.m_sType = "Dictionary<" + sKey + ", CPseduLuaValue>";
                }
            }

            foreach (KeyValuePair<string, CLuaVariable> kvp in kvpf.Value.m_sReturns)
            {
                if (string.IsNullOrEmpty(kvp.Value.m_sType) && !string.IsNullOrEmpty(kvp.Value.m_sWeakType) && kvp.Value.m_sWeakType == "Dictionary<object,object[]>")
                {
                    string sKey = DecideKeyType(ref kvp.Value.m_sAssignCode, script, kvpf.Value, true);
                    kvp.Value.m_sType = "Dictionary<" + sKey + ", CPseduLuaValue>";
                }
                if (string.IsNullOrEmpty(kvp.Value.m_sType) && !string.IsNullOrEmpty(kvp.Value.m_sWeakType) && kvp.Value.m_sWeakType == "Dictionary<object,object>")
                {
                    string sKey = DecideKeyType(ref kvp.Value.m_sAssignCode, script, kvpf.Value, false);
                    kvp.Value.m_sType = "Dictionary<" + sKey + ", CPseduLuaValue>";
                }
            }
        }
    }

    /// <summary>
    /// use [1] = xxx to know it is a dictionary<int, xx>
    /// use ["1"] = xxx to know it is a dictionary<string, xx>
    /// </summary>
    /// <param name="sIn"></param>
    /// <param name="script"></param>
    /// <param name="func"></param>
    /// <param name="bList"></param>
    /// <returns></returns>
    private static string DecideKeyType(ref string sIn, CLuaScript script, CLuaFunction func, bool bList)
    {
        MatchCollection matches = Regex.Matches(sIn, @"\[([^}]+?)\][\s]*=");
        if (bList)
        {
            sIn = Regex.Replace(sIn, @"\[([^}]+?)\][\s]*=[\s]*(\{[^{}]*?\})", @"{$1, new object[] $2}");
        }
        else
        {
            sIn = Regex.Replace(sIn, @"\[([^}]+?)\][\s]*=[\s]*([^\,\}]*)", @"{$1, $2}");
        }

        foreach (Match match in matches)
        {
            string sKey = match.Groups[1].Value;

            if (string.IsNullOrEmpty(Regex.Replace(sKey, @"[\s]*[\d]+[\s]*", "")))
            {
                return "int";
            }

            if (string.IsNullOrEmpty(Regex.Replace(sKey, @"[\s]*" + "\\\"[\\w\\d_]*\\\"" + @"[\s]*", "")))
            {
                return "string";
            }

            if (string.IsNullOrEmpty(Regex.Replace(sKey, @"[\s]*([\w_][\w\d_]*)[\s]*", ""))) //is variable [abc] = 
            {
                string sVarialbe = Regex.Matches(sKey, @"[\s]*([\w_][\w\d_]*)[\s]*")[0].Groups[1].Value;
                if (script.m_pAllMemberVariables.ContainsKey(sVarialbe)
                 && !string.IsNullOrEmpty(script.m_pAllMemberVariables[sVarialbe].m_sType))
                {
                    if (script.m_pAllMemberVariables[sVarialbe].m_sType == "string")
                    {
                        return "string";
                    }
                    if (script.m_pAllMemberVariables[sVarialbe].m_sType == "int")
                    {
                        return "int";
                    }
                    CRuntimeLogger.LogError("Dictionary key can only be string or int, wrong in " + script.m_sClassName + ":" + sIn);
                }

                if (null != func 
                 && func.m_sUsingMemberVariables.ContainsKey(sVarialbe)
                 && !string.IsNullOrEmpty(func.m_sUsingMemberVariables[sVarialbe].m_sType))
                {
                    if (script.m_pAllMemberVariables[sVarialbe].m_sType == "string")
                    {
                        return "string";
                    }
                    if (script.m_pAllMemberVariables[sVarialbe].m_sType == "int")
                    {
                        return "int";
                    }
                    CRuntimeLogger.LogError("Dictionary key can only be string or int, wrong in " + script.m_sClassName + "." + func.m_sFunctionName + ":" + sIn);
                }
            }
        }

        CRuntimeLogger.LogError("Unable to decide Dictionary key in " + script.m_sClassName + ":" + sIn);
        return "";
    }

    /// <summary>
    /// translate if, for, while
    /// </summary>
    /// <param name="script"></param>
    public static void CheckIfForWhile(ref CLuaScript script)
    {
        foreach (KeyValuePair<string, CLuaFunction> kvp in script.m_pAllFunctions)
        {
            string sContent = kvp.Value.m_sContent;
            string sChanged = ReplaceEnd(sContent);
            while (sChanged != sContent)
            {
                sContent = sChanged;
                sChanged = ReplaceEnd(sContent);
            }
            sContent = ReplaceIfForWhileFinal(sContent);

            sContent = sContent.Replace("CSHARPCHECKED_For", "for");
            sContent = sContent.Replace("CSHARP_If", "if");
            sContent = sContent.Replace("CSHARP_ElseIf", "else if");
            sContent = sContent.Replace("CSHARP_Else", "else");
            sContent = sContent.Replace("CSHARPCHECKED_While", "while");

            kvp.Value.m_sContent = sContent;
        }
    }

    /// <summary>
    /// replace "end"
    /// </summary>
    /// <param name="sIn"></param>
    /// <returns></returns>
    private static string ReplaceEnd(string sIn)
    {
        sIn = Regex.Replace(sIn, @"(\s)(elseif)(\s)", "$1 0ELSEF_VERYLONG $3");
        sIn = Regex.Replace(sIn, @"(\s)(else)(\s)", "$1 0ELSE_VERYLONG $3");
        sIn = Regex.Replace(sIn, @"(\s)(if)(\s)", "$1 0IF_VERYLONG $3");
        sIn = Regex.Replace(sIn, @"(\s)(for)(\s)", "$1 0FOR_VERYLONG $3");
        sIn = Regex.Replace(sIn, @"(\s)(while)(\s)", "$1 0WHILE_VERYLONG $3");
        sIn = Regex.Replace(sIn, @"(\s)(end)(\s)", "$1 0endline_VERYLONG $3");

        int iReplaced = 0;
        int iOldReplace = -1;
        int iEndIndex = sIn.IndexOf("0endline_VERYLONG", StringComparison.Ordinal);
        int iEndLength = "0endline_VERYLONG".Length;

        int iIfLong = "0IF_VERYLONG".Length;
        int iForLong = "0FOR_VERYLONG".Length;
        int iWhileLong = "0WHILE_VERYLONG".Length;

        #region replace end to endif

        while (iEndIndex >= 0 && iOldReplace != iReplaced)
        {
            iOldReplace = iReplaced;
            //Last End
            //find end --> end
            int iLastEnd = Mathf.Max(sIn.Substring(0, iEndIndex).LastIndexOf("0endline_VERYLONG", StringComparison.Ordinal)
                                   , sIn.Substring(0, iEndIndex).LastIndexOf("0ENDIF_verylong", StringComparison.Ordinal)
                                   , sIn.Substring(0, iEndIndex).LastIndexOf("0ENDFOR_verylong", StringComparison.Ordinal)
                                   , sIn.Substring(0, iEndIndex).LastIndexOf("0ENDWHILE_verylong", StringComparison.Ordinal)
                                     );
            int iLastIf = sIn.Substring(0, iEndIndex).LastIndexOf("0IF_VERYLONG", StringComparison.Ordinal);
            int iLastFor = sIn.Substring(0, iEndIndex).LastIndexOf("0FOR_VERYLONG", StringComparison.Ordinal);
            int iLastWhile = sIn.Substring(0, iEndIndex).LastIndexOf("0WHILE_VERYLONG", StringComparison.Ordinal);

            if (iLastIf > iLastEnd
             && iLastIf > iLastFor
             && iLastIf > iLastWhile
                )
            {
                sIn = sIn.Substring(0, iEndIndex) + "0ENDIF_verylong" + sIn.Substring(iEndLength + iEndIndex, sIn.Length - iEndLength - iEndIndex);
                ++iReplaced;
            }
            else if (iLastFor > iLastEnd
                 && iLastFor > iLastIf
                 && iLastFor > iLastWhile
                    )
            {
                sIn = sIn.Substring(0, iEndIndex) + "0ENDFOR_verylong" + sIn.Substring(iEndLength + iEndIndex, sIn.Length - iEndLength - iEndIndex);
                ++iReplaced;
            }
            else if (iLastWhile > iLastEnd
                 && iLastWhile > iLastFor
                 && iLastWhile > iLastIf
                    )
            {
                sIn = sIn.Substring(0, iEndIndex) + "0ENDWHILE_verylong" + sIn.Substring(iEndLength + iEndIndex, sIn.Length - iEndLength - iEndIndex);
                ++iReplaced;
            }

            iEndIndex = sIn.IndexOf("0endline_VERYLONG", StringComparison.Ordinal);
        }

        #endregion

        #region replace if to if check

        int iFound = 1;
        while (iFound > 0)
        {
            iFound = 0;
            int iIfEnd = sIn.IndexOf("0ENDIF_verylong", StringComparison.Ordinal);
            if (iIfEnd >= 0)
            {
                int iLastIf = sIn.Substring(0, iIfEnd).LastIndexOf("0IF_VERYLONG", StringComparison.Ordinal);
                int iLastElse = sIn.Substring(0, iIfEnd).LastIndexOf("0ELSE_VERYLONG", StringComparison.Ordinal);

                if (iLastIf >= 0)
                {
                    if (iLastElse > iLastIf)
                    {
                        string sStart = sIn.Substring(0, iLastIf);
                        string sMid1 = sIn.Substring(iLastIf + iIfLong, sIn.Length - "0IF_VERYLONG".Length - iLastIf - (sIn.Length - iLastElse));
                        string sMid2 = sIn.Substring(iLastElse + "0ELSE_VERYLONG".Length, sIn.Length - "0ELSE_VERYLONG".Length - iLastElse - (sIn.Length - iIfEnd));
                        string sEnd = sIn.Substring("0ENDIF_verylong".Length + iIfEnd, sIn.Length - "0ENDIF_verylong".Length - iIfEnd);
                        sIn = sStart + "0IF_CSHARP_Check" + sMid1 + "0CSHARPCHECK_Else" + sMid2 + "0CSHARPCHECK_Endif" + sEnd;
                        ++iFound;
                    }
                    else
                    {
                        string sStart = sIn.Substring(0, iLastIf);
                        string sMid = sIn.Substring(iLastIf + iIfLong, sIn.Length - iIfLong - iLastIf - (sIn.Length - iIfEnd));
                        string sEnd = sIn.Substring("0ENDIF_verylong".Length + iIfEnd, sIn.Length - "0ENDIF_verylong".Length - iIfEnd);
                        sIn = sStart + "0IF_CSHARP_Check" + sMid + "0CSHARPCHECK_Endif" + sEnd;
                        ++iFound;
                    }
                }
            }

            int iForEnd = sIn.IndexOf("0ENDFOR_verylong", StringComparison.Ordinal);
            if (iForEnd >= 0)
            {
                int iLastFor = sIn.Substring(0, iForEnd).LastIndexOf("0FOR_VERYLONG", StringComparison.Ordinal);
                if (iLastFor >= 0)
                {
                    string sStart = sIn.Substring(0, iLastFor);
                    string sMid = sIn.Substring(iLastFor + iForLong, sIn.Length - iForLong - iLastFor - (sIn.Length - iForEnd));
                    string sEnd = sIn.Substring("0ENDFOR_verylong".Length + iForEnd, sIn.Length - "0ENDFOR_verylong".Length - iForEnd);
                    sIn = sStart + "0FOR_CSHARP_Check" + sMid + "0CSHARPCHECK_Endfor" + sEnd;
                    ++iFound;
                }
            }

            int iWhileEnd = sIn.IndexOf("0ENDWHILE_verylong", StringComparison.Ordinal);
            if (iWhileEnd >= 0)
            {
                int iLastWhile = sIn.Substring(0, iWhileEnd).LastIndexOf("0WHILE_VERYLONG", StringComparison.Ordinal);
                if (iLastWhile >= 0)
                {
                    string sStart = sIn.Substring(0, iLastWhile);
                    string sMid = sIn.Substring(iLastWhile + iWhileLong, sIn.Length - iWhileLong - iLastWhile - (sIn.Length - iWhileEnd));
                    string sEnd = sIn.Substring("0ENDWHILE_verylong".Length + iWhileEnd, sIn.Length - "0ENDWHILE_verylong".Length - iWhileEnd);
                    sIn = sStart + "0WHILE_CSHARP_Check" + sMid + "0CSHARPCHECK_Endwhile" + sEnd;
                    ++iFound;
                }
            }
        }

        #endregion

        return sIn;
    }

    private static string ReplaceIfForWhileFinal(string sIn)
    {
        #region if
        //Replace if then endif
        sIn = Regex.Replace(sIn, @"( 0IF_CSHARP_Check )([\s]+)([\s\S]*?)([\s\r\n]+)(then)([\s\S\n\r]*?)( 0CSHARPCHECK_Endif )", " CSHARP_If ($2$3$4) { $6 } 0CSHARPCHECKED_Endif ");
        //Replace else if
        sIn = Regex.Replace(sIn, @"(\s)( 0CSHARPCHECK_Else )(\s)([\s\S]*?)(\} 0CSHARPCHECKED_Endif )", "$1 } CSHARP_Else { $3$4} 0CSHARPCHECKED_Endif ");
        //Replace else if then
        sIn = Regex.Replace(sIn, @"( 0ELSEF_VERYLONG )([\s]+)([\s\S]*?)([\s]+then)", "} CSHARP_ElseIf ($2$3) {");

        sIn = sIn.Replace(" 0CSHARPCHECKED_Endif ", "");

        #endregion

        #region for

        sIn = Regex.Replace(sIn,
                    @"(\s 0FOR_CSHARP_Check [\s]+)([a-zA-Z_][\w]*)[\s]*=[\s]*([\+\-]?[\s]*[a-zA-Z_\(\)\d\s\.\+\-]+?)\s*,\s*([\+\-]?[\s]*[a-zA-Z_\(\)\d\s\.\+\-]+?)\s+do([\s\S]*?)( 0CSHARPCHECK_Endfor \s)",
            //" CSHARPCHECKED_For (int $2 = math.round($3); $2 <= math.round($4); ++$2) { $5 }");
                    " CSHARPCHECKED_For (int $2 = $3; $2 <= $4; ++$2) { $5 }");

        sIn = Regex.Replace(sIn,
                            @"(\s 0FOR_CSHARP_Check [\s]+)([a-zA-Z_][\w]*)[\s]*=[\s]*([\+\-]?[\s]*[\d]+)\s*,\s*([\+\-]?[\s]*[\d]+)\s*,\s*([\+]?[\s]*[\d]+)\s+do([\s\S]*?)( 0CSHARPCHECK_Endfor \s)",
                            " CSHARPCHECKED_For (int $2 = $3; $2 <= math.round($4); $2+=$5) { $6 }");

        sIn = Regex.Replace(sIn,
                            @"(\s 0FOR_CSHARP_Check [\s]+)([a-zA-Z_][\w]*)[\s]*=[\s]*([\+\-]?[\s]*[\d]+)\s*,\s*([\+\-]?[\s]*[\d]+)\s*,\s*(\-[\s]*[\d]+)\s+do([\s\S]*?)( 0CSHARPCHECK_Endfor \s)",
                            " CSHARPCHECKED_For (int $2 = $3; $2 >= math.round($4); $2+=$5) { $6 }");

        #endregion

        #region while

        sIn = Regex.Replace(sIn, @" 0WHILE_CSHARP_Check ([\s\S]*?)do([\s\S]*?) 0CSHARPCHECK_Endwhile ", " CSHARPCHECKED_While ($1) { $2 }");

        #endregion

        return sIn;
    }

    /// <summary>
    /// add ;
    /// </summary>
    /// <param name="script"></param>
    public static void AddSemicolon(ref CLuaScript script)
    {
        foreach (KeyValuePair<string, CLuaFunction> func in script.m_pAllFunctions)
        {
            func.Value.m_sContent = func.Value.m_sContent.Replace("\n", ";\n");

            string sContent = func.Value.m_sContent;
            string sRet = Regex.Replace(func.Value.m_sContent, @";[\s]*;", ";");
            while (sRet != sContent)
            {
                sContent = sRet;
                sRet = Regex.Replace(sContent, @";[\s]*;", ";");
            }

            sContent = sRet;
            sRet = Regex.Replace(sContent, @"{[\s]*;", "{");
            while (sRet != sContent)
            {
                sContent = sRet;
                sRet = Regex.Replace(sContent, @"{[\s]*;", "{");
            }

            sContent = sRet;
            sRet = Regex.Replace(sContent, @"}[\s]*;", "}");
            while (sRet != sContent)
            {
                sContent = sRet;
                sRet = Regex.Replace(sContent, @"}[\s]*;", "}");
            }
            func.Value.m_sContent = sContent;
        }

    }

    /// <summary>
    /// if local a is never used, mark it
    /// if local a is asign only once, mark it
    /// </summary>
    /// <param name="script"></param>
    /// <param name="sOrign"></param>
    public static void MarkVariables(ref CLuaScript script, string sOrign)
    {
        foreach (KeyValuePair<string, CLuaVariable> vari in script.m_pAllMemberVariables)
        {
            if (vari.Value.m_eUseage == ELuaVariableUsage.ELVU_Member || string.IsNullOrEmpty(script.m_sInherent))
            {
                if (Regex.Matches(sOrign, "\\b" + vari.Value.m_sName + "\\b").Count < 2)
                {
                    //only once
                    vari.Value.m_bNeverUsed = true;
                }
                else if (Regex.Matches(sOrign, "\\b" + vari.Value.m_sName + "[\\s]*=[^=]+?").Count < 2)
                {
                    vari.Value.m_bOnlyOneAssign = true;
                }

                if (vari.Value.m_bOnlyOneAssign)
                {
                    vari.Value.m_eUseage = (ELuaVariableUsage)((int)ELuaVariableUsage.ELVU_Const | (int)vari.Value.m_eUseage);
                }
            }
        }

        foreach (KeyValuePair<string, CLuaFunction> func in script.m_pAllFunctions)
        {
            foreach (KeyValuePair<string, CLuaVariable> vari in func.Value.m_sUsingMemberVariables)
            {
                int iCount1 = Regex.Matches(func.Value.m_sContent, "\\b" + vari.Value.m_sName + "\\b").Count;
                foreach (KeyValuePair<string, string> kvp in script.m_pAllFunctionCalls)
                {
                    iCount1 += Regex.Matches(kvp.Value, "\\b" + vari.Value.m_sName + "\\b").Count;
                }
                foreach (KeyValuePair<string, CLuaVariable> kvpv in func.Value.m_sUsingMemberVariables)
                {
                    iCount1 += Regex.Matches(kvpv.Value.GetAssgin(), "\\b" + vari.Value.m_sName + "\\b").Count;
                }
                foreach (KeyValuePair<string, CLuaVariable> kvpr in func.Value.m_sReturns)
                {
                    iCount1 += Regex.Matches(kvpr.Value.GetAssgin(), "\\b" + vari.Value.m_sName + "\\b").Count;
                }

                if (iCount1 < 2)
                {
                    //only once
                    vari.Value.m_bNeverUsed = true;
                }
                else if (Regex.Matches(func.Value.m_sContent, "\\b" + vari.Value.m_sName + "[\\s]*=[^=]+?").Count < 2)
                {
                    vari.Value.m_bOnlyOneAssign = true;
                }

                if (vari.Value.m_bOnlyOneAssign)
                {
                    if (vari.Value.m_sType == "string" || vari.Value.m_sType == "int" || vari.Value.m_sType == "float")
                    {
                        vari.Value.m_eUseage = (ELuaVariableUsage)((int)ELuaVariableUsage.ELVU_Const | (int)vari.Value.m_eUseage);
                    }
                }
            }
        }
    }

    //not using at now
    public static void MarkFunctions(ref CLuaScript script)
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <param name="outString"></param>
    public static void WrightBackMemberAndFunction(ref CLuaScript script, ref string outString)
    {
        if (string.IsNullOrEmpty(script.m_sInherent))
        {
            foreach (KeyValuePair<string, CLuaFunction> func in script.m_pAllFunctions)
            {
                func.Value.m_eProperty = (EFunctionProperty)((int)EFunctionProperty.ELF_Public | (int)EFunctionProperty.ELF_Static);
            }

            foreach (KeyValuePair<string, CLuaVariable> vari in script.m_pAllMemberVariables)
            {
                if (0 == ((int) vari.Value.m_eUseage & (int) ELuaVariableUsage.ELVU_Static)
                 && 0 == ((int) vari.Value.m_eUseage & (int) ELuaVariableUsage.ELVU_Const))
                {
                    vari.Value.m_eUseage = (ELuaVariableUsage) ((int) vari.Value.m_eUseage | (int) ELuaVariableUsage.ELVU_Static);
                }
            }
        }

        foreach (KeyValuePair<string, CLuaFunction> func in script.m_pAllFunctions)
        {
            foreach (KeyValuePair<string, CLuaVariable> vari in func.Value.m_sUsingMemberVariables)
            {
                if (!vari.Value.m_bNeverUsed)
                {
                    func.Value.m_sContent = func.Value.m_sContent.Replace(vari.Value.m_sId, vari.Value.GetAssgin());
                }
                else
                {
                    func.Value.m_sContent = func.Value.m_sContent.Replace(vari.Value.m_sId, "");
                }
            }

            foreach (KeyValuePair<string, CLuaVariable> vari in func.Value.m_sReturns)
            {
                func.Value.m_sContent = func.Value.m_sContent.Replace(vari.Key, "return " + vari.Value.GetAssginCode());
            }

            if (string.IsNullOrEmpty(func.Value.m_sReturn))
            {
                func.Value.m_sReturn = "CPseduLuaValue";
            }
            for (int i = 0; i < func.Value.m_sFunctionParameters.Count; ++i)
            {
                if (string.IsNullOrEmpty(func.Value.m_sFunctionParameters[i].m_sType))
                {
                    func.Value.m_sFunctionParameters[i].m_sType = "CPseduLuaValue";
                }
            }
            outString = outString.Replace(func.Value.m_sId, func.Value.GetId());
        }

        foreach (KeyValuePair<string, CLuaVariable> vari in script.m_pAllMemberVariables)
        {
            if (!vari.Value.m_bNeverUsed)
            {
                outString = outString.Replace(vari.Value.m_sId, vari.Value.GetAssgin());
            }
            else
            {
                outString = outString.Replace(vari.Value.m_sId, "");
            }
        }
    }

    /// <summary>
    /// ~= -> !=  etc..
    /// </summary>
    /// <param name="outString"></param>
    public static void ChangeOperator(ref string outString)
    {
        string sContent = outString;
        sContent = sContent.Replace("  ", " ");
        sContent = sContent.Replace("\n\n\n", "\n\n");
        sContent = sContent.Replace("\n\n\n", "\n\n");

        string sRet = Regex.Replace(sContent, @";[\s]*;", ";");
        while (sRet != sContent)
        {
            sContent = sRet;
            sRet = Regex.Replace(sContent, @";[\s]*;", ";");
        }

        sContent = sRet;
        sRet = Regex.Replace(sContent, @"{[\s]*;", "{");
        while (sRet != sContent)
        {
            sContent = sRet;
            sRet = Regex.Replace(sContent, @"{[\s]*;", "{");
        }

        sContent = sRet;
        sRet = Regex.Replace(sContent, @"}[\s]*;", "}");
        while (sRet != sContent)
        {
            sContent = sRet;
            sRet = Regex.Replace(sContent, @"}[\s]*;", "}");
        }

        sContent = Regex.Replace(sContent, @"([\s\(\=]+)not[\s]+", "$1!");
        sContent = Regex.Replace(sContent, @"([\s]+)(and)([\s]+)", "$1&&$3");
        sContent = Regex.Replace(sContent, @"([\s]+)(or)([\s]+)", "$1||$3");
        sContent = sContent.Replace("~=", "!=");
        sContent = sContent.Replace("..", " + ");
        sContent = sContent.Replace("nil", "null");

        sContent = Regex.Replace(sContent, @"(\b)([\d]+\.[\d]+)([^f])", "$1$2f$3");

        outString = sContent;
    }

    public static void WriteBackFuncCallAndComment(CLuaScript script, ref string outString)
    {
        List<string> keys = new List<string>();
        foreach (KeyValuePair<string, string> kvps in script.m_pAllFunctionCalls)
        {
            keys.Add(kvps.Key);
        }
        for (int i = 0; i < keys.Count; ++i)
        {
            string sKey = keys[keys.Count - i - 1];
            outString = outString.Replace(sKey, script.m_pAllFunctionCalls[sKey]);
        }

        foreach (KeyValuePair<string, CLuaComments> kvps in script.m_pComments)
        {
            outString = outString.Replace(kvps.Key, kvps.Value.GetContent());
        }
    }

    public static void FormatRets(CLuaScript script, ref string outString)
    {
        string sOld = outString;
        sOld = sOld.Replace("{", "\n{");
        sOld = sOld.Replace("}", "}\n");
        string sNew = sOld.Replace("  ", " ");
        sNew = sNew.Replace("\n\n", "\n");
        while (sNew != sOld)
        {
            sOld = sNew;
            sNew = sOld.Replace("  ", " ");
            sNew = sNew.Replace("\n\n", "\n");
        }
        sOld = Regex.Replace(sOld, @"\n[\s]*", "\n");

        string[] allLines = sOld.Split('\n');
        int iSpace = 0;
        for (int i = 0; i < allLines.Length; ++i)
        {
            int iOldSpace = iSpace;
            if (allLines[i].Length > 0 && allLines[i][0] == '{')
            {
                iSpace += 4;
            }
            if (allLines[i].Length > 0 && allLines[i][0] == '}')
            {
                iSpace -= 4;
                iOldSpace = iSpace;
            }
            for (int j = 0; j < iOldSpace; ++j)
            {
                allLines[i] = " " + allLines[i];
            }
        }

        string sFinal = "";
        for (int i = 0; i < allLines.Length; ++i)
        {
            sFinal = sFinal + allLines[i] + "\n";
        }

        sFinal = sFinal.Replace("*/;", "*/");
        sFinal = sFinal.Replace("}", "}\n");
        sFinal = sFinal.Replace("public class", "\npublic class");

        foreach (KeyValuePair<string, string> strTable in script.m_pStringTable)
        {
            sFinal = sFinal.Replace(strTable.Key, strTable.Value);
        }

        sFinal = sFinal.Replace("__L__", "{");
        sFinal = sFinal.Replace("__R__", "}");

        outString = sFinal;
    }

    public static string DecideTypeString(Type type)
    {
        if (type.Name.Equals("Void"))
        {
            return "void";
        }
        if (type == typeof(int) || type == typeof(Int32))
        {
            return "int";
        }
        if (type == typeof(string) || type == typeof(String))
        {
            return "string";
        }
        if (type == typeof(float) || type == typeof(Single))
        {
            return "float";
        }
        if (type == typeof(byte) || type == typeof(Byte))
        {
            return "byte";
        }
        if (type == typeof(bool) || type == typeof(Boolean))
        {
            return "bool";
        }

        if (type == typeof(int[]) || type == typeof(Int32[]))
        {
            return "int[]";
        }
        if (type == typeof(float[]) || type == typeof(Single[]))
        {
            return "float[]";
        }
        if (type == typeof(string[]) || type == typeof(String[]))
        {
            return "string[]";
        }
        if (type == typeof(byte[]) || type == typeof(Byte[]))
        {
            return "byte[]";
        }
        if (type == typeof(bool[]) || type == typeof(Boolean[]))
        {
            return "bool[]";
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type itemType = type.GetGenericArguments()[0];
            return string.Format("List<{0}>", DecideTypeString(itemType));
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Type itemType1 = type.GetGenericArguments()[0];
            Type itemType2 = type.GetGenericArguments()[1];
            return string.Format("Dictionary<{0},{1}>", DecideTypeString(itemType1), DecideTypeString(itemType2));
        }

        if (type == typeof (System.Object))
        {
            return "System.Object";
        }

        CRuntimeLogger.Log("Unkown Type:" + type.Name);
        return type.ToString();
    }
}
