using System.IO;
using UnityEngine;

public class CommonFunctions
{
    public static void CreateFolder(string sFullFileName)
    {
        sFullFileName = sFullFileName.Replace("\\", "/");
        string[] dirs = sFullFileName.Split('/');
        string startPath = dirs[0];
        for (int i = 1; i < dirs.Length - 1; ++i)
        {
            startPath += ("/" + dirs[i]);
            if (!Directory.Exists(startPath))
            {
                Directory.CreateDirectory(startPath);
            }
        }
    }

    public static string FindFullName(GameObject pTopParent, GameObject obj)
    {
        string sName = obj.name;
        Transform trans = obj.transform;
        while (null != trans.parent
               && trans.parent != obj.transform
               && trans != pTopParent.transform)
        {
            trans = trans.parent;
            sName = trans.gameObject.name + "/" + sName;
        }
        return sName;
    }

    public static string GetLastName(string sPath, bool bWithoutDot = true)
    {
        sPath = sPath.Replace("\\", "/");
        int iPos = sPath.LastIndexOf('/');
        string sFileName = sPath;
        if (-1 != iPos)
        {
            sFileName = sPath.Substring(iPos + 1, sPath.Length - iPos - 1);
        }

        iPos = sFileName.LastIndexOf('.');
        if (-1 == iPos)
        {
            return sFileName;
        }
        return sFileName.Substring(0, iPos);
    }
}
