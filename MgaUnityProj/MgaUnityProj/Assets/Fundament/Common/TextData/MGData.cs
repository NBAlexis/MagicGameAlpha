using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class IntEqualityComparer : IEqualityComparer<int>
{
    public bool Equals(int a, int b)
    {
        return a == b;
    }

    public int GetHashCode(int data)
    {
        return data.GetHashCode();
    }
}

public class CMGDataBaseClass
{
    static protected IntEqualityComparer m_intEc = new IntEqualityComparer();
    public static byte GetAlphaBeta(string cnChar)
    {
        byte[] arrCN = Encoding.Default.GetBytes(cnChar);
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

        return (byte)(65 + arrCN[0] - 'A');
    }
}

public class CMGDataSubElement
{
    #region Need to be override

    virtual public string GetStringSub() { return ""; }

    virtual public void FromString(string sString) {}

    virtual public CMGDataSubElement CopyMe() { return null; }

    public class CMGDataSubElementList
    {
        public CMGDataSubElement[] m_pListContent;

        virtual public CMGDataElement GetDefault()
        {
            return null;
        }
    }

    #endregion

    #region Parse

    protected object GetElementValue(string sContent, string sName, object defaultValue)
    {
        string sFormat = string.Format("\\\"{0}\\\"[\\s]*\\:[\\s]*\\\"[\\w\\d_]+\\\"[\\s]*", sName);
        string sFound = null;
        MatchCollection matches = Regex.Matches(sContent, sFormat);
        if (matches.Count > 0)
        {
            int iIndex1 = matches[0].Value.IndexOf('\"');
            int iIndex2 = matches[0].Value.IndexOf('\"', iIndex1 + 1);
            int iIndex3 = matches[0].Value.IndexOf('\"', iIndex2 + 1);
            int iIndex4 = matches[0].Value.IndexOf('\"', iIndex3 + 1);
            if (iIndex3 > 0 && iIndex4 > iIndex3)
            {
                sFound = matches[0].Value.Substring(iIndex3, iIndex4 - iIndex3);
            }
        }

        if (string.IsNullOrEmpty(sFound))
        {
            return defaultValue;
        }

        if (defaultValue is string)
        {
            return sFound;
        }
        if (defaultValue is int)
        {
            int ret;
            if (int.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is sint)
        {
            int ret;
            if (int.TryParse(sFound, out ret))
            {
                return (sint)ret;
            }
        }
        if (defaultValue is uint)
        {
            uint ret;
            if (uint.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is long)
        {
            long ret;
            if (long.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is ulong)
        {
            ulong ret;
            if (ulong.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is short)
        {
            short ret;
            if (short.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is ushort)
        {
            ushort ret;
            if (ushort.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is byte)
        {
            byte ret;
            if (byte.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is float)
        {
            float ret;
            if (float.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is sfloat)
        {
            float ret;
            if (float.TryParse(sFound, out ret))
            {
                return (sfloat)ret;
            }
        }
        if (defaultValue is bool)
        {
            bool ret;
            if (bool.TryParse(sFound, out ret))
            {
                return ret;
            }
        }
        if (defaultValue is sbool)
        {
            bool ret;
            if (bool.TryParse(sFound, out ret))
            {
                return (sbool)ret;
            }
        }
        if (defaultValue is Vector2)
        {
            string[] args = sFound.Split(',');
            float fx, fy;
            if (2 == args.Length && float.TryParse(args[0], out fx) && float.TryParse(args[1], out fy))
            {
                return new Vector2(fx, fy);
            }
        }
        if (defaultValue is Vector3)
        {
            string[] args = sFound.Split(',');
            float fx, fy, fz;
            if (3 == args.Length && float.TryParse(args[0], out fx) && float.TryParse(args[1], out fy) && float.TryParse(args[2], out fz))
            {
                return new Vector3(fx, fy, fz);
            }
        }
        if (defaultValue is Vector4)
        {
            string[] args = sFound.Split(',');
            float fx, fy, fz, fw;
            if (4 == args.Length && float.TryParse(args[0], out fx) && float.TryParse(args[1], out fy) && float.TryParse(args[2], out fz) && float.TryParse(args[3], out fw))
            {
                return new Vector4(fx, fy, fz, fw);
            }
        }
        if (defaultValue is Color)
        {
            string[] args = sFound.Split(',');
            byte fr, fg, fb, fa;
            if (4 == args.Length && byte.TryParse(args[0], out fr) && byte.TryParse(args[1], out fg) && byte.TryParse(args[2], out fb) && byte.TryParse(args[3], out fa))
            {
                return new Color(fr / 255.0f, fg / 255.0f, fb / 255.0f, fa / 255.0f);
            }
        }
        CMGDataSubElement defC = defaultValue as CMGDataSubElement;
        if (null != defC)
        {
            defC.FromString(sFound);
            return defC;
        }
        if (defaultValue is string[])
        {
            return sFound.Split(';');
        }
        CMGDataSubElementList defLst = defaultValue as CMGDataSubElementList;
        if (null != defLst  && null != defLst.GetDefault())
        {
            string[] args = sFound.Split(';');
            defLst.m_pListContent = new CMGDataSubElement[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                CMGDataSubElement newElement = defLst.GetDefault();
                newElement.FromString(args[i]);
                defLst.m_pListContent[i] = newElement;
            }
            return defLst;
        }

        return defaultValue;
    }

    #endregion

    #region Write

    protected string Write(string sName, object content)
    {
        string[] stlist = content as string[];
        if (null != stlist)
        {
            string sWrite = "";
            for (int i = 0; i < stlist.Length; ++i)
            {
                sWrite += (0 == i) ? stlist[i] : (";" + stlist[i]);
            }
            return string.Format("\"{0}\" = \"{1}\",\n", sName, sWrite);
        }

        CMGDataSubElementList cmlist = content as CMGDataSubElementList;
        if (null != cmlist)
        {
            string sWrite = "";
            foreach (CMGDataSubElement t in cmlist.m_pListContent)
            {
                if (null != t)
                {
                    sWrite += (string.IsNullOrEmpty(sWrite)) ? t.GetStringSub() : (";" + t.GetStringSub());    
                }
            }
            return string.Format("\"{0}\" = \"{1}\",\n", sName, sWrite);
        }

        CMGDataSubElement element = content as CMGDataSubElement;
        if (null != element)
        {
            return string.Format("\"{0}\" = \"{1}\",\n", sName, element.GetStringSub());
        }

        if (content is Color)
        {
            return string.Format("{0},{1},{2},{3}",
                Mathf.Clamp(((Color) content).r*256.0f, 0, 255),
                Mathf.Clamp(((Color) content).g*256.0f, 0, 255),
                Mathf.Clamp(((Color) content).b*256.0f, 0, 255),
                Mathf.Clamp(((Color) content).a*256.0f, 0, 255)
                );
        }
        if (content is Vector2)
        {
            return string.Format("{0},{1}",
                GetFloatTo4(((Vector2)content).x),
                GetFloatTo4(((Vector2)content).y)
                );
        }
        if (content is Vector3)
        {
            return string.Format("{0},{1},{2}",
                GetFloatTo4(((Vector3)content).x),
                GetFloatTo4(((Vector3)content).y),
                GetFloatTo4(((Vector3)content).z)
                );
        }
        if (content is Vector4)
        {
            return string.Format("{0},{1},{2},{3}",
                GetFloatTo4(((Vector4)content).x),
                GetFloatTo4(((Vector4)content).y),
                GetFloatTo4(((Vector4)content).z),
                GetFloatTo4(((Vector4)content).w)
                );
        }
        if (content is float)
        {
            return string.Format("\"{0}\" = \"{1}\",\n", sName, GetFloatTo4((float)content));
        }

        return string.Format("\"{0}\" = \"{1}\",\n", sName, content);
    }

    protected string GetFloatTo4(float fInner)
    {
        int iRet = Mathf.RoundToInt(fInner*10000.0f);
        if (iRet < 10000)
        {
            string sRet = (iRet + 10000).ToString();
            return "0." + sRet.Substring(1, 4);
        }

        return (iRet / 10000) + "." + (iRet % 10000);
    }

    #endregion

    #region Edtior

    public struct SEdtiorDesc
    {
        public string m_sKey;
        public string m_sDesc;
        public string m_sCurrentValue;
    }

    public virtual Dictionary<string, SEdtiorDesc> GetEdtiorDescription()
    {
        return null;
    }

    public virtual void SetEdtiorValue(string sKey, string sValue)
    {
        
    }

    #endregion
}

public class CMGDataElement : CMGDataSubElement
{
	public int m_iID = -1;
    public string m_sElementName = "";
    public int m_iIndex = -1;
    public CMGDataBaseClass m_pOwner = null;

    #region Need to be override

    virtual public void ResetData()
    {

    }

    virtual public string GetString()
    {
        return "";
    }

    virtual public void LoadData(string sTextToParse)
    {

    }

    #endregion
}

public class TMGData<T> : CMGDataBaseClass where T : CMGDataElement, new()
{
    #region Need to be override

    public virtual string GetDefaultSavePath()
    {
        return "";
    }

    public virtual string GetDefaultLoadPath()
    {
        return "";
    }

    #endregion

    protected int m_iMaxId = 0;
    protected Dictionary<int, CMGDataElement> m_pIdTable = null;
    protected Dictionary <string, CMGDataElement> m_pDataTable = null;

    public List<T> m_pElement = null;

    public void Save(string sFullPathWithoutSurfix)
    {
        string filepath = sFullPathWithoutSurfix + ".bytes";
        using (TextWriter sw = new StreamWriter(filepath, false, Encoding.UTF8))
        {
            sw.Write(GetSaveString());
            sw.Flush();
            sw.Close();
        }
    }

    protected string GetSaveString()
    {
        string sRet = "\n";
        foreach (T element in m_pElement)
        {
            sRet += "{\n" + element.GetString() + "\n}\n";
        }
        return sRet;
    }

    public bool Load(string sFileNameWithoutSurfix)
    {
        m_pElement = new List<T>();
        TextAsset txtAsset = ResourcesManager.Load<TextAsset>(sFileNameWithoutSurfix);
        if (null == txtAsset)
        {
            return false;
        }

        MatchCollection matches = Regex.Matches(txtAsset.text, @"[\s]*\{([^\}])*\}");
        foreach (Match match in matches)
        {
            T newElement = new T();
            newElement.LoadData(match.Value);
            newElement.m_pOwner = this;
            m_pElement.Add(newElement);
        }

        return SortElement();
    }

    public bool SortElement()
    {
        if (null == m_pElement)
        {
            m_pElement = new List<T>();
        }

        for (int i = 0; i < m_pElement.Count; ++i)
        {
            for (int j = i + 1; j < m_pElement.Count; ++j)
            {
                if (m_pElement[i].m_iID < m_pElement[j].m_iID)
                {
                    T pElement = m_pElement[j];
                    m_pElement[j] = m_pElement[i];
                    m_pElement[i] = pElement;
                }
            }
        }

        m_pDataTable = new Dictionary<string, CMGDataElement>();
        m_pIdTable = new Dictionary<int, CMGDataElement>(m_intEc);
        for (int i = 0; i < m_pElement.Count; ++i)
        {
            m_pElement[i].m_iIndex = i;
            if (m_pDataTable.ContainsKey(m_pElement[i].m_sElementName))
            {
                Debug.LogWarning(m_pElement[i].m_sElementName + " has existed! in MGData");
            }
            else
            {
                m_pDataTable.Add(m_pElement[i].m_sElementName, m_pElement[i]);
            }
            if (m_pIdTable.ContainsKey(m_pElement[i].m_iID))
            {
                Debug.LogWarning(m_pElement[i].m_iID + " has existed! in MGData");
            }
            else
            {
                m_pIdTable.Add(m_pElement[i].m_iID, m_pElement[i]);
            }

            if (m_iMaxId <= m_pElement[i].m_iID)
            {
                m_iMaxId = m_pElement[i].m_iID;
            }
        }
        return true;
    }

    protected bool RemoveElementPredicate(T obj)
    {
        return obj.m_iIndex < 0;
    }

    public void DeleteElement(List<int> iIndex)
    {
        if (null == iIndex || iIndex.Count < 1)
        {
            return;
        }

        foreach (T t1 in iIndex.SelectMany(t => m_pElement.Where(t1 => null != t1 && t1.m_iIndex == t)))
        {
            t1.m_iIndex = -1;
        }
        m_pElement.RemoveAll(RemoveElementPredicate);
        SortElement();
    }

    public T CreateElement()
    {
        if (null == m_pElement)
        {
            m_pElement = new List<T>();
        }

        T newelement = new T();
        newelement.ResetData();
        newelement.m_pOwner = this;
        newelement.m_sElementName = "new element";
        newelement.m_iID = m_iMaxId;

        if (null != FindElement(newelement.m_sElementName))
        {
            bool bChangeName = true;
            int iChangeStart = 1;
            while (bChangeName)
            {
                newelement.m_sElementName = "new element " + iChangeStart;
                ++iChangeStart;
                bChangeName = (null != FindElement(newelement.m_sElementName));
            }
        }
        if (null != FindElement(newelement.m_iID))
        {
            bool bChangeName = true;
            while (bChangeName)
            {
                ++newelement.m_iID;
                bChangeName = (null != FindElement(newelement.m_iID));
            }
        }

        m_pElement.Add(newelement);

        SortElement();

        return newelement;
    }

    public bool AddElement(T element)
    {
        if (null == element)
        {
            return false;
        }

        if (null != FindElement(element.m_sElementName))
        {
            return false;
        }

        element.m_pOwner = this;
        m_pElement.Add(element);
        SortElement();
        return true;
    }

    public bool ChangeName(string sOldName, string sNewName)
    {
        T pEditing = FindElement(sOldName);
        if (null == pEditing)
        {
            return false;
        }

        if (null != FindElement(sNewName))
        {
            return false;
        }

        m_pElement[pEditing.m_iIndex].m_sElementName = sNewName;
        SortElement();
        return true;
    }

    public T this[int id]
    {
        get
        {
            return FindElement(id);
        }
    }

    public T this[string sElementName]
    {
        get
        {
            return FindElement(sElementName);
        }
        set
        {
            int iIndex = FindElement(sElementName).m_iIndex;
            m_pElement[iIndex] = value;
        }
    }

    protected T FindElement(string sName)
    {
        if (null == m_pDataTable)
        {
            SortElement();
        }

        if (null == m_pDataTable || string.IsNullOrEmpty(sName) || !m_pDataTable.ContainsKey(sName))
        {
            return null;
        }
        return m_pDataTable[sName] as T;
    }

    protected T FindElement(int iId)
    {
        if (null == m_pIdTable)
        {
            SortElement();
        }

        if (null == m_pIdTable || !m_pIdTable.ContainsKey(iId))
        {
            return null;
        }
        return m_pIdTable[iId] as T;
    }
}
