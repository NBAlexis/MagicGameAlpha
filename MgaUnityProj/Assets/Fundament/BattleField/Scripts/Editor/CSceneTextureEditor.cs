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

    protected override void EditorOneElement(CSceneTextureElement element, bool bFocus)
    {
        base.EditorOneElement(element, bFocus);

        m_pMainEditor.BeginLine();

        element.m_iTemplate = (int)EditorField("Template", element.m_iTemplate);
        element.m_iRotNumber = (int)EditorField("Rots", element.m_iRotNumber);
        element.m_bReflect = (bool)EditorField("Relf", element.m_bReflect);
        element.m_iTextureCount = (int)EditorField("Count", element.m_iTextureCount);

        m_pMainEditor.EndLine();
    }

    #endregion

}
