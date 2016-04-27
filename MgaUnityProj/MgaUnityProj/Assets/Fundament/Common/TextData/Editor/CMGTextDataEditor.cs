using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

//This editor is only editor sub element
public class CMGSubDataEditor
{
    public virtual void InitEditor()
    {

    }

    protected void ShowSepB() { GUILayout.Label("=============================================================================="); }

    protected void ShowSepS() { GUILayout.Label("---------------------------------------------------"); }

    protected Dictionary<string, string[]> m_pBitFiledDesc = null;

    protected void PutBitField(string sId, string[] field)
    {
        if (null == m_pBitFiledDesc)
        {
            m_pBitFiledDesc = new Dictionary<string, string[]>();
        }
        if (!m_pBitFiledDesc.ContainsKey(sId))
        {
            m_pBitFiledDesc.Add(sId, field);
        }
        else
        {
            m_pBitFiledDesc[sId] = field;
        }
    }

    public object EditorField(string sDesc, object current)
    {
        if (current is int)
        {
            return EditorGUILayout.IntField(sDesc, (int)current);
        }
        if (current is sint)
        {
            return (sint)EditorGUILayout.IntField(sDesc, (sint)current);
        }
        if (current is short)
        {
            return (short)EditorGUILayout.IntField(sDesc, (short)current);
        }
        if (current is ushort)
        {
            return (ushort)EditorGUILayout.IntField(sDesc, (ushort)current);
        }
        if (current is byte)
        {
            return (byte)EditorGUILayout.IntField(sDesc, (byte)current);
        }
        if (current is long)
        {
            return EditorGUILayout.LongField(sDesc, (long)current);
        }

        if (current is float)
        {
            return EditorGUILayout.FloatField(sDesc, (float)current);
        }
        if (current is sfloat)
        {
            return (sfloat)EditorGUILayout.FloatField(sDesc, (sfloat)current);
        }

        string sCurrent = current as string;
        if (null != sCurrent)
        {
            return EditorGUILayout.TextField(sDesc, sCurrent);
        }

        //=====================================================================
        //Bit Field
        if (current is uint)
        {
            return (uint)EditorGUILayout.LongField(sDesc, (uint)current);
        }
        if (current is ulong)
        {
            return (ulong)EditorGUILayout.LongField(sDesc, (long)(ulong)current);
        }

        //=====================================================================
        //Struct Field
        if (current is Vector2)
        {
            return EditorGUILayout.Vector2Field(sDesc, (Vector2)current);
        }
        if (current is Vector3)
        {
            return EditorGUILayout.Vector3Field(sDesc, (Vector3)current);
        }
        if (current is Vector4)
        {
            return EditorGUILayout.Vector4Field(sDesc, (Vector4)current);
        }
        if (current is Color)
        {
            return EditorGUILayout.ColorField(sDesc, (Color)current);
        }

        //=====================================================================
        //String List Field

        return null;
    }

    public virtual void EditorOneElement(CMGDataSubElement element)
    {

    }
}

public class CMGTextDataEditor<T> : EditorWindow where T : CMGDataElement, new()
{
    protected CMGSubDataEditor m_pMainEditor;
    protected Dictionary<string, CMGSubDataEditor> m_pSubElements;
    protected TMGData<T> m_pEditingData;

    protected Dictionary<int, List<int>> m_pOrder;
    protected Dictionary<int, bool> m_pFold;
    protected bool m_bOrderByIndex = true;

    protected virtual void OnGUI()
    {
        if (null == m_pEditingData)
        {
            return;
        }

        //Editing

        //Order By Index
        //Order By Name
        bool bOrder = m_bOrderByIndex;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ID排序"))
        {
            bOrder = true;
        }
        if (GUILayout.Button("名称排序"))
        {
            bOrder = false;
        }
        EditorGUILayout.EndHorizontal();

        if (bOrder != m_bOrderByIndex || null == m_pOrder)
        {
            m_bOrderByIndex = bOrder;
            //Order
        }

        for (int i = 0; i < m_pEditingData.m_pElement.Count; ++i)
        {
            EditorOneElement(m_pEditingData.m_pElement[i]);
        }
    }

    public virtual void InitEditor()
    {
        m_pMainEditor = new CMGSubDataEditor();
        m_pSubElements = new Dictionary<string, CMGSubDataEditor>();

        //load data
        //add bit fields
        //add sub editors
    }

    protected void CreateFolder(string sFullFileName)
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

    protected virtual void EditorOneElement(CMGDataElement element)
    {
        
    }

    protected virtual void ShowDeleteMe(CMGDataElement element)
    {
        
    }

    public object EditorField(string sDesc, object current)
    {
        if (current is CMGDataSubElement)
        {
            if (null != m_pSubElements && m_pSubElements.ContainsKey(sDesc))
            {
                m_pSubElements[sDesc].EditorOneElement(current as CMGDataSubElement);
            }
        }

        if (current is CMGDataSubElement[])
        {
            if (null != m_pSubElements && m_pSubElements.ContainsKey(sDesc))
            {

            }
        }

        return m_pMainEditor.EditorField(sDesc, current);
    }
}
