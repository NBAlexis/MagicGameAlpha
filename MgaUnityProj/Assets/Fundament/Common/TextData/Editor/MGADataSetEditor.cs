using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MGADataFieldEditor : TMGTextDataEditor<CMGADataFields>
{
    [MGADataEditor(typeof(CMGADataFields))]
    protected static void EditOneElement(CMGADataFields element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);
        element.m_sFieldType = (string)EditorField("变量类型", element.m_sFieldType, iLineStart, ref bBeginLine);
        element.m_sFieldName = (string)EditorField("变量名", element.m_sFieldName, iLineStart, ref bBeginLine);
        EndLine(ref bBeginLine);
        BeginLine(iLineStart, ref bBeginLine);
        element.m_sFieldDefault = (string)EditorField("默认值", element.m_sFieldDefault, iLineStart, ref bBeginLine);
        element.m_sFieldEdtiorName = (string)EditorField("变量编辑其中名字", element.m_sFieldEdtiorName, iLineStart, ref bBeginLine);
        EndLine(ref bBeginLine);
    }
}

public class MGADataEnumEditor : TMGTextDataEditor<CMGADataEnumes>
{
    [MGADataEditor(typeof(CMGADataEnumes))]
    protected static void EditOneElement(CMGADataEnumes element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);
        element.m_sEnumeName = (string)EditorField("枚举名", element.m_sEnumeName, iLineStart, ref bBeginLine);
        EndLine(ref bBeginLine);
        BeginLine(iLineStart, ref bBeginLine);
        element.m_sEnumeMemebers = (string[])EditorField("枚举值", element.m_sEnumeMemebers, iLineStart, ref bBeginLine);
        EndLine(ref bBeginLine);
    }
}

public class MGADataSubSetEditor : TMGTextDataEditor<CMGADataSubSet>
{
    private string m_sGameName = "";

    [MenuItem("MGA/表格/表格结构")]
    public static void ShowWindow()
    {
        MGADataSubSetEditor pEditor = (MGADataSubSetEditor)GetWindow(typeof(MGADataSubSetEditor));
        pEditor.InitEditor();
    }

    [MGADataEditor(typeof(CMGADataSubSet))]
    protected static void EditOneElement(CMGADataSubSet element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);
        element.m_sName = (string)EditorField("表格名", element.m_sName, iLineStart, ref bBeginLine);
        EndLine(ref bBeginLine);
        
        element.m_sFields = (CMGADataFields[])EditorField("成员", element.m_sFields, iLineStart, ref bBeginLine);
        element.m_sEnums = (CMGADataEnumes[])EditorField("枚举值", element.m_sEnums, iLineStart, ref bBeginLine);
    }

    public override void InitEditor()
    {
        base.InitEditor();
        m_pEditingData = new CMGADataSet();
        m_pEditingData.SortElement();
    }

    protected override void OnGUI()
    {
        GUILayout.BeginHorizontal();
        m_sGameName = EditorGUILayout.TextField("游戏名", m_sGameName);
        if (GUILayout.Button("加载"))
        {
            m_pEditingData = new CMGADataSet();
            m_pEditingData.Load(m_sGameName + "/Table");
        }
        GUILayout.EndHorizontal();

        base.OnGUI();
    }
}
