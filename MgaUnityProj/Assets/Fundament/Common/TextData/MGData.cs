using System;
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
}

public abstract class CMGDataElement
{
	public int m_iID = -1;
    public string m_sElementName = "";
    public string[] m_sTags = new string[0];
    public int m_iIndex = -1;
    public CMGDataBaseClass m_pOwner = null;

    #region Need to be override

    virtual public string GetString()
    {
        string sRet = "\n";
        sRet += Write("ID", m_iID);
        sRet += Write("ElementName", m_sElementName);
        sRet += Write("Tags", m_sTags);
        return sRet;
    }

    virtual public void LoadData(string sTextToParse)
    {
        m_iID = (int)GetElementValue(sTextToParse, "ID", m_iID);
        m_sElementName = (string)GetElementValue(sTextToParse, "ElementName", m_sElementName);
        m_sTags = (string[])GetElementValue(sTextToParse, "Tags", m_sTags);
    }

    public abstract CMGDataElement GetDefault();

    public abstract CMGDataElement Copy();

    #endregion

    #region Parse

    protected object GetElementValue(string sContent, string sName, object defaultValue)
    {
        string sFormat = string.Format("\\\"{0}\\\"[\\s]*\\:[\\s]*\\\"[^\\\"]+\\\"[\\s]*", sName);
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
                sFound = matches[0].Value.Substring(iIndex3 + 1, iIndex4 - iIndex3 - 1);
            }
        }

        if (string.IsNullOrEmpty(sFound))
        {
            return defaultValue;
        }

        #region PrimeTypes

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
        if (defaultValue is Enum)
        {
            return Enum.Parse(defaultValue.GetType(), sFound);
        }

        #endregion

        #region Lists

        if (defaultValue is string[])
        {
            return sFound.Split(';');
        }
        if (defaultValue is int[])
        {
            List<int> intlist = new List<int>();
            foreach (string sInner in sFound.Split(';'))
            {
                int ielement;
                if (!string.IsNullOrEmpty(sInner) && int.TryParse(sInner, out ielement))
                {
                    intlist.Add(ielement);
                }
            }
            return intlist.ToArray();
        }
        if (defaultValue is float[])
        {
            List<float> intlist = new List<float>();
            foreach (string sInner in sFound.Split(';'))
            {
                float ielement;
                if (!string.IsNullOrEmpty(sInner) && float.TryParse(sInner, out ielement))
                {
                    intlist.Add(ielement);
                }
            }
            return intlist.ToArray();
        }

        #endregion

        #region Child Data

        CMGDataElement defC = defaultValue as CMGDataElement;
        if (null != defC)
        {
            defC.LoadData(sFound);
            return defC;
        }

        CMGDataElement[] defLst = defaultValue as CMGDataElement[];
        if (null != defLst && defLst.Length > 0 && null != defLst[0])
        {
            string[] args = sFound.Split(';');
            defLst = new CMGDataElement[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                CMGDataElement newElement = defLst[0].GetDefault();
                newElement.LoadData(args[i]);
                defLst[i] = newElement;
            }
            return defLst;
            
        }

        #endregion

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
            return string.Format("\"{0}\" : \"{1}\",\n", sName, sWrite);
        }

        int[] intlist = content as int[];
        if (null != intlist)
        {
            string sWrite = "";
            for (int i = 0; i < intlist.Length; ++i)
            {
                sWrite += (0 == i) ? intlist[i].ToString() : (";" + intlist[i]);
            }
            return string.Format("\"{0}\" : \"{1}\",\n", sName, sWrite);
        }

        float[] fltlist = content as float[];
        if (null != fltlist)
        {
            string sWrite = "";
            for (int i = 0; i < fltlist.Length; ++i)
            {
                sWrite += (0 == i) ? GetFloatTo4(fltlist[i]) : (";" + GetFloatTo4(fltlist[i]));
            }
            return string.Format("\"{0}\" : \"{1}\",\n", sName, sWrite);
        }

        CMGDataElement[] cmlist = content as CMGDataElement[];
        if (null != cmlist)
        {
            string sWrite = "";
            foreach (CMGDataElement t in cmlist)
            {
                if (null != t)
                {
                    sWrite += (string.IsNullOrEmpty(sWrite)) ? t.GetString() : (";" + t.GetString());
                }
            }
            return string.Format("\"{0}\" : \"{1}\",\n", sName, sWrite);
        }

        CMGDataElement element = content as CMGDataElement;
        if (null != element)
        {
            return string.Format("\"{0}\" : \"{1}\",\n", sName, element.GetString());
        }

        if (content is Color)
        {
            return string.Format("\"{0}\" : \"{1},{2},{3},{4}\",\n",
                sName,
                Mathf.Clamp(Mathf.RoundToInt(((Color)content).r * 255.0f), 0, 255),
                Mathf.Clamp(Mathf.RoundToInt(((Color)content).g * 255.0f), 0, 255),
                Mathf.Clamp(Mathf.RoundToInt(((Color)content).b * 255.0f), 0, 255),
                Mathf.Clamp(Mathf.RoundToInt(((Color)content).a * 255.0f), 0, 255)
                );
        }
        if (content is Vector2)
        {
            return string.Format("\"{0}\" : \"{1},{2}\",\n",
                sName,
                GetFloatTo4(((Vector2)content).x),
                GetFloatTo4(((Vector2)content).y)
                );
        }
        if (content is Vector3)
        {
            return string.Format("\"{0}\" : \"{1},{2},{3}\",\n",
                sName,
                GetFloatTo4(((Vector3)content).x),
                GetFloatTo4(((Vector3)content).y),
                GetFloatTo4(((Vector3)content).z)
                );
        }
        if (content is Vector4)
        {
            return string.Format("\"{0}\" : \"{1},{2},{3},{4}\",\n",
                sName,
                GetFloatTo4(((Vector4)content).x),
                GetFloatTo4(((Vector4)content).y),
                GetFloatTo4(((Vector4)content).z),
                GetFloatTo4(((Vector4)content).w)
                );
        }
        if (content is float)
        {
            return string.Format("\"{0}\" : \"{1}\",\n", sName, GetFloatTo4((float)content));
        }

        return string.Format("\"{0}\" : \"{1}\",\n", sName, content);
    }

    protected string GetFloatTo4(float fInner)
    {
        int iRet = Mathf.RoundToInt(fInner * 10000.0f);
        if (iRet < 10000)
        {
            string sRet = (iRet + 10000).ToString();
            return "0." + sRet.Substring(1, 4);
        }

        return (iRet / 10000) + "." + (iRet % 10000);
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

    private const string m_sNoTag = "No Tag";
    protected int m_iMaxId = 0;
    protected Dictionary <int, CMGDataElement> m_pIdTable = null;
    protected Dictionary <string, CMGDataElement> m_pDataTable = null;
    protected Dictionary <string, List<T>> m_pTagTabls = null;

    public List<T> m_pElement = null;

    public void Save()
    {
        Save(GetDefaultSavePath());    
    }

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

    public bool Load()
    {
        return Load(GetDefaultLoadPath());
    }

    public bool Load(string sFileNameWithoutSurfix)
    {
        m_pElement = new List<T>();
        TextAsset txtAsset = ResourcesManager.Load<TextAsset>(sFileNameWithoutSurfix);
        if (null == txtAsset)
        {
            CRuntimeLogger.Log("Data not found at " + sFileNameWithoutSurfix + ", Create a new one");
            return false;
        }

        return LoadFromString(txtAsset.text);
    }

    public bool LoadFromString(string sString)
    {
        MatchCollection matches = Regex.Matches(sString, @"[\s]*\{([^\}])*\}");
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
                if (m_pElement[i].m_iID > m_pElement[j].m_iID)
                {
                    T pElement = m_pElement[j];
                    m_pElement[j] = m_pElement[i];
                    m_pElement[i] = pElement;
                }
            }
        }

        m_pDataTable = new Dictionary<string, CMGDataElement>();
        m_pIdTable = new Dictionary<int, CMGDataElement>(m_intEc);
        m_pTagTabls = new Dictionary<string, List<T>>();

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

            int iAddTags = 0;
            if (null != m_pElement[i].m_sTags)
            {
                foreach (string sTag in m_pElement[i].m_sTags)
                {
                    if (!string.IsNullOrEmpty(sTag) && !sTag.Equals(m_sNoTag))
                    {
                        ++iAddTags;
                        if (!m_pTagTabls.ContainsKey(sTag))
                        {
                            m_pTagTabls.Add(sTag, new List<T>());
                        }   
                        m_pTagTabls[sTag].Add(m_pElement[i]);
                    }
                }
            }
            if (0 == iAddTags)
            {
                if (!m_pTagTabls.ContainsKey(m_sNoTag))
                {
                    m_pTagTabls.Add(m_sNoTag, new List<T>());
                }
                m_pTagTabls[m_sNoTag].Add(m_pElement[i]);
            }
        }
        return true;
    }

    protected bool RemoveElementPredicate(T obj)
    {
        return obj.m_iIndex < 0;
    }

    public void DeleteElement(params int[] iIndex)
    {
        if (null == iIndex || iIndex.Length < 1)
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

    public T CreateElement(string sName = "new element")
    {
        if (null == m_pElement)
        {
            m_pElement = new List<T>();
        }

        T newelement = new T { m_pOwner = this, m_sElementName = sName, m_iID = m_iMaxId };

        if (null != FindElement(newelement.m_sElementName))
        {
            bool bChangeName = true;
            int iChangeStart = 1;
            while (bChangeName)
            {
                newelement.m_sElementName = sName + iChangeStart;
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

    public bool ChangeId(int iOldId, int iNewId)
    {
        T pEditing = FindElement(iOldId);
        if (null == pEditing)
        {
            return false;
        }

        if (null != FindElement(iNewId))
        {
            return false;
        }

        m_pElement[pEditing.m_iIndex].m_iID = iNewId;
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

    public T[] this[string[] sTags]
    {
        get { return FindElementWithTags(sTags); }
    }

    public List<T> FindElementWithTag(string sTag)
    {
        if (string.IsNullOrEmpty(sTag))
        {
            sTag = m_sNoTag;
        }
        if (null == m_pTagTabls || !m_pTagTabls.ContainsKey(sTag))
        {
            return new List<T>();
        }
        return m_pTagTabls[sTag];
    }

    public T[] FindElementWithTags(string[] sTags)
    {
        List<List<T>> allTags = new List<List<T>>();
        foreach (string sT in sTags)
        {
            allTags.Add(FindElementWithTag(sT));
        }
        Dictionary<int, T> ret = new Dictionary<int, T>(new IntEqualityComparer());

        foreach (T theone in allTags[0])
        {
            if (ret.ContainsKey(theone.m_iID))
            {
                continue;
            }
            bool bInList = true;
            for (int i = 1; i < allTags.Count; ++i)
            {
                bool bInThisList = false;
                for (int j = 0; j < allTags[i].Count; ++j)
                {
                    if (allTags[i][j].m_iID != theone.m_iID)
                    {
                        continue;
                    }
                    bInThisList = true;
                    break;
                }
                bInList = bInThisList;
                if (!bInList)
                {
                    break;
                }
            }
            if (bInList)
            {
                ret.Add(theone.m_iID, theone);
            }
        }
        return ret.Values.ToArray();
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
