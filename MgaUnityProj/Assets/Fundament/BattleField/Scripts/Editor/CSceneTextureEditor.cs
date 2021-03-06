using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CSceneTextureEditor : TMGTextDataEditor<CSceneTextureElement>
{
    private string m_sType = "Default";

    [MenuItem("MGA/Editor/Scene/Scene Texture Editor")]
    public static void ShowWindow()
    {
        CSceneTextureEditor pEditor = (CSceneTextureEditor)GetWindow(typeof(CSceneTextureEditor));
        pEditor.InitEditor();
    }

    #region Override Functions

    protected override void OnGUI()
    {
        m_sType = EditorGUILayout.TextField("Type", m_sType);
        base.OnGUI();
    }

    public override void InitEditor()
    {
        base.InitEditor();

        //load data
        m_pEditingData = new CSceneTexture();
        ((CSceneTexture) m_pEditingData).m_sSceneType = m_sType;
        m_pEditingData.Load();
    }

    [MGADataEditor(typeof(CSceneTextureElement))]
    protected static void EditOneElement(CSceneTextureElement element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);

        element.m_iTemplate = (int)EditorField("Template", element.m_iTemplate, iLineStart, ref bBeginLine);
        element.m_iRotNumber = (int)EditorField("Rots", element.m_iRotNumber, iLineStart, ref bBeginLine);
        element.m_bReflect = (bool)EditorField("Relf", element.m_bReflect, iLineStart, ref bBeginLine);
        element.m_iTextureCount = (int)EditorField("Count", element.m_iTextureCount, iLineStart, ref bBeginLine);

        EndLine(ref bBeginLine);
    }

    #endregion

}
