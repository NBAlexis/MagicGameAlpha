using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CSceneTexTemplateEditor : TMGTextDataEditor<CSceneGroudTemplateElement>
{
    private string m_sType = "Default";

    [MenuItem("MGA/Editor/Scene/Scene Texture Template Editor")]
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
        m_pEditingData = new CSceneGroudTemplate();
        ((CSceneGroudTemplate)m_pEditingData).m_sSceneType = m_sType;
        m_pEditingData.Load();
    }

    protected override void EditorOneElement(CSceneGroudTemplateElement element, bool bFocus)
    {
        base.EditorOneElement(element, bFocus);

        m_pMainEditor.BeginLine();

        element.m_vUV = (Vector4)EditorField("UV", element.m_vUV);

        m_pMainEditor.EndLine();
    }

    #endregion
}
