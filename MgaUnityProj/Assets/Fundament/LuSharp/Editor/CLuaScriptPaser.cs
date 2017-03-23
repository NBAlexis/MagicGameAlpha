using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class CLuaScriptPaser
{
    [MenuItem("MGA/Tools/LuSharp(Debug)")]
    public static void ConvertLuatoCs()
    {
        UnityEngine.Object[] objects = Selection.objects;
        foreach (UnityEngine.Object obj in objects)
        {
            string sLuaFile = AssetDatabase.GetAssetPath(obj);
            int iEnd = sLuaFile.LastIndexOf(".lua", StringComparison.Ordinal);
            if (iEnd < 0)
            {
                CRuntimeLogger.LogError("You did not select a .cs file");
                return;
            }

            string sFilename = sLuaFile.Substring(0, iEnd);
            string sClassName = sFilename;
            int iLastSlash = sFilename.LastIndexOf("/", StringComparison.Ordinal);
            if (iLastSlash > 0)
            {
                sClassName = sClassName.Substring(iLastSlash + 1, sClassName.Length - 1 - iLastSlash);
            }
            string sSource;

            if (File.Exists(sLuaFile))
            {
                FileStream pFile = File.OpenRead(sLuaFile);
                long iFileLength = pFile.Length;
                byte[] pFileBytes = new byte[iFileLength];
                pFile.Read(pFileBytes, 0, (int)iFileLength);
                byte[] unicodeBytes = Encoding.Convert(Encoding.Unicode, Encoding.Unicode, pFileBytes);
                sSource = Encoding.Unicode.GetString(unicodeBytes);
                pFile.Close();
            }
            else
            {
                CRuntimeLogger.LogError("FILE NOT EXIST:" + sLuaFile);
                return;
            }

            string cCode = ConvertToC(sSource, sClassName, sFilename, true);
            if (cCode.Equals(""))
            {
                CRuntimeLogger.LogError("Failed converting " + sFilename);
                continue;
            }
            /*
            try
            {
                TextWriter tw = new StreamWriter(sFilename, false, Encoding.Unicode);
                tw.Write(cCode);
                tw.Close();
            }
            catch (IOException IOEx)
            {
                CRuntimeLogger.LogError("Incorrect file permissions? " + IOEx);
            }

            AssetDatabase.ImportAsset(sFilename, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
             */
            CRuntimeLogger.Log("Converted " + Selection.activeObject.name + " to " + sFilename);
        }
    }

    [MenuItem("MGA/Tools/LuSharp(Test)")]
    public static void ConvertLuatoCsTest()
    {
        UnityEngine.Object[] objects = Selection.objects;
        foreach (UnityEngine.Object obj in objects)
        {
            string sLuaFile = AssetDatabase.GetAssetPath(obj);
            int iEnd = sLuaFile.LastIndexOf(".lua", StringComparison.Ordinal);
            if (iEnd < 0)
            {
                CRuntimeLogger.LogError("You did not select a .cs file");
                return;
            }

            string sFilename = sLuaFile.Substring(0, iEnd);
            string sClassName = sFilename;
            int iLastSlash = sFilename.LastIndexOf("/", StringComparison.Ordinal);
            if (iLastSlash > 0)
            {
                sClassName = sClassName.Substring(iLastSlash + 1, sClassName.Length - 1 - iLastSlash);
            }
            string sSource;

            if (File.Exists(sLuaFile))
            {
                FileStream pFile = File.OpenRead(sLuaFile);
                long iFileLength = pFile.Length;
                byte[] pFileBytes = new byte[iFileLength];
                pFile.Read(pFileBytes, 0, (int)iFileLength);
                byte[] unicodeBytes = Encoding.Convert(Encoding.Unicode, Encoding.Unicode, pFileBytes);
                sSource = Encoding.Unicode.GetString(unicodeBytes);
                pFile.Close();
            }
            else
            {
                CRuntimeLogger.LogError("FILE NOT EXIST:" + sLuaFile);
                return;
            }
            sClassName = sClassName + "_Test";
            sFilename = sFilename + "_Test";

            string cCode = ConvertToC(sSource, sClassName, sFilename, false);
            if (cCode.Equals(""))
            {
                CRuntimeLogger.LogError("Failed converting " + sFilename);
                continue;
            }

            File.WriteAllText(sFilename + ".cs", cCode);
            CRuntimeLogger.Log("Converted " + Selection.activeObject.name + " to " + sFilename);
        }
    }

    [MenuItem("MGA/Tools/LuSharp(Real)")]
    public static void ConvertLuatoCsReal()
    {
        UnityEngine.Object[] objects = Selection.objects;
        foreach (UnityEngine.Object obj in objects)
        {
            string sLuaFile = AssetDatabase.GetAssetPath(obj);
            int iEnd = sLuaFile.LastIndexOf(".lua", StringComparison.Ordinal);
            if (iEnd < 0)
            {
                CRuntimeLogger.LogError("You did not select a .cs file");
                return;
            }

            string sFilename = sLuaFile.Substring(0, iEnd);
            string sClassName = sFilename;
            int iLastSlash = sFilename.LastIndexOf("/", StringComparison.Ordinal);
            if (iLastSlash > 0)
            {
                sClassName = sClassName.Substring(iLastSlash + 1, sClassName.Length - 1 - iLastSlash);
            }
            string sSource;

            if (File.Exists(sLuaFile))
            {
                FileStream pFile = File.OpenRead(sLuaFile);
                long iFileLength = pFile.Length;
                byte[] pFileBytes = new byte[iFileLength];
                pFile.Read(pFileBytes, 0, (int)iFileLength);
                byte[] unicodeBytes = Encoding.Convert(Encoding.Unicode, Encoding.Unicode, pFileBytes);
                sSource = Encoding.Unicode.GetString(unicodeBytes);
                pFile.Close();
            }
            else
            {
                CRuntimeLogger.LogError("FILE NOT EXIST:" + sLuaFile);
                return;
            }

            string cCode = ConvertToC(sSource, sClassName, sFilename, false);
            if (cCode.Equals(""))
            {
                CRuntimeLogger.LogError("Failed converting " + sFilename);
                continue;
            }

            File.WriteAllText(sFilename + ".cs", cCode);
            CRuntimeLogger.Log("Converted " + Selection.activeObject.name + " to " + sFilename);
        }
    }

    private static string ConvertToC(string output, string sClassName, string sOutPutFile, bool bDebug)
    {
        CLuaScript theScritp = new CLuaScript();
        theScritp.m_sClassName = sClassName;

        output = output.Replace("\uFEFF", "");
        output = output.Replace("\uFFFE", "");
        output = output.Replace("\u000D", "\n");
        output = output.Replace("\u000A", "\n");
        output = output.Replace("\u0A0D", "\n");
        output = output.Replace("\r\n", "\n");
        output = output.Replace("\n\r", "\n");
        output = output.Replace("\r\r", "\n");
        string orig = output;

        LuSharpConvert.GetInherentAndHead(ref output, ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_comment.txt", sDebug, Encoding.Unicode);            
        }

        LuSharpConvert.GetStringContent(ref output, ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_stringtable.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.GetFunctions(ref output, ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_function.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.GetMembers(ref output, ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_member.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.GetFunctionVariables(ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_functionvar.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.GetFunctionLocalVariables(ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_funclocal.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.GetReturns(ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_return.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.CheckDictionaryType(ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_dictype.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.CheckIfForWhile(ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_ifforwhile.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.MarkVariables(ref theScritp, orig);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_markv.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.MarkFunctions(ref theScritp);

        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_markf.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.AddSemicolon(ref theScritp);
        if (bDebug)
        {
            string sDebug = theScritp.ToTxt();
            sDebug += "\n\n";
            sDebug += output;
            File.WriteAllText(sOutPutFile + "_addsemic.txt", sDebug, Encoding.Unicode);
        }

        LuSharpConvert.WrightBackMemberAndFunction(ref theScritp, ref output);

        if (string.IsNullOrEmpty(theScritp.m_sInherent))
        {
            output = "using System.Collections;\nusing System.Collections.Generic;\n\npublic class " + sClassName + "\n{\n" + output + "\n}\n";
        }
        else
        {
            output = "using System.Collections;\nusing System.Collections.Generic;\n\npublic class " + sClassName + " : " + theScritp.m_sInherent + "\n{\n" + output + "\n}\n";
        }

        if (bDebug)
        {
            File.WriteAllText(sOutPutFile + "_writememberandfunction.txt", output, Encoding.Unicode);
        }

        LuSharpConvert.ChangeOperator(ref output);

        if (bDebug)
        {
            File.WriteAllText(sOutPutFile + "_operator.txt", output, Encoding.Unicode);
        }

        LuSharpConvert.WriteBackFuncCallAndComment(theScritp, ref output);
        if (bDebug)
        {
            File.WriteAllText(sOutPutFile + "_wbcomm.txt", output, Encoding.Unicode);
        }

        LuSharpConvert.FormatRets(theScritp, ref output);
        if (bDebug)
        {
            File.WriteAllText(sOutPutFile + "_form1.txt", output, Encoding.Unicode);
        }

        return output;
    }
}
