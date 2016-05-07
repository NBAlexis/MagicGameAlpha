using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    public static byte GetAlphaBeta(string cnChar)
    {
        byte[] arrCN = Encoding.GetEncoding(936).GetBytes(cnChar.Substring(0, 1));
        if (arrCN.Length > 1)
        {
            int area = arrCN[0];
            int pos = arrCN[1];
            int code = (area << 8) + pos;
            int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
            for (int i = 0; i < 26; i++)
            {
                int max = 55290;
                if (i != 25) max = areacode[i + 1];
                if (areacode[i] <= code && code < max)
                {
                    return (byte)(65 + i);
                }
            }
            return (byte)'?';
        }

        if (arrCN[0] >= 'a' && arrCN[0] <= 'z')
        {
            return (byte)(65 + arrCN[0] - 'a');
        }

        if (arrCN[0] >= '0' && arrCN[0] <= '1')
        {
            return arrCN[0];
        }

        return (byte)(65 + arrCN[0] - 'A');
    }

    public static float Levenshtein_Distance(string str1, string str2)
    {
        str1 = str1.ToLower();
        str2 = str2.ToLower();
        int n = str1.Length;
        int m = str2.Length;

        int i, j;
        if (n == 0)
        {
            return m;
        }
        if (m == 0)
        {
            return n;
        }
        int[,] theMatrix = new int[n + 1, m + 1];

        for (i = 0; i <= n; i++)
        {
            theMatrix[i, 0] = i;
        }

        for (j = 0; j <= m; j++)
        {
            theMatrix[0, j] = j;
        }

        for (i = 1; i <= n; i++)
        {
            char ch1 = str1[i - 1];
            for (j = 1; j <= m; j++)
            {
                char ch2 = str2[j - 1];
                int temp = ch1.Equals(ch2) ? 0 : 1;
                theMatrix[i, j] = Mathf.Min(theMatrix[i - 1, j] + 1, theMatrix[i, j - 1] + 1, theMatrix[i - 1, j - 1] + temp);
            }
        }
        return 1.0f - Mathf.Clamp01((float)theMatrix[n, m] / Mathf.Max(n, m));
    }

    public static List<string> GuessWord(List<string> list, string sMatch, float fDistMin, int iNumber)
    {
        string[] retReady = new string[iNumber];
        float[] retDist = new float[iNumber];
        for (int i = 0; i < iNumber; ++i)
        {
            retDist[i] = -1.0f;
        }
        foreach (string word in list)
        {
            if (!string.IsNullOrEmpty(word))
            {
                float fDist = Levenshtein_Distance(sMatch, word);
                if (fDist > fDistMin)
                {
                    for (int i = 0; i < iNumber; ++i)
                    {
                        if (fDist > retDist[i])
                        {
                            retDist[i] = fDist;
                            retReady[i] = word;
                            break;
                        }
                    }
                }
            }
        }

        List<string> ret = new List<string>();
        for (int i = 0; i < iNumber; ++i)
        {
            if (retDist[i] > 0.0f && !string.IsNullOrEmpty(retReady[i]))
            {
                ret.Add(retReady[i]);
            }
        }
        return ret;
    }

    public static GameObject FindChildrenByName(GameObject theGo, string sName, bool bIncludeDeactive = false)
    {
        foreach (Transform trans in theGo.GetComponentsInChildren<Transform>(bIncludeDeactive))
        {
            if (FindFullName(theGo, trans.gameObject).Contains(sName))
            {
                return trans.gameObject;    
            }
        }
        return null;
    }

    public static string BuildStringOrder(string[] strings)
    {
        strings.ToList().Sort();
        return strings.Aggregate("", (current, s) => current + s);
    }

    public static int IntPow(int iBase, int iPow)
    {
        if (iPow < 1)
        {
            return 1;
        }

        int ret = iBase;
        for (int i = 1; i < iPow; ++i)
        {
            ret *= iBase;
        }
        return ret;
    }
}
