using UnityEngine;
using System.Collections;
using UnityEditor;

public class CSceneTemplatesEditor : TMGTextDataEditor<CSceneTemplateElement>
{
    [MenuItem("MGA/Editor/Scene/Scene Templates Editor")]
    public static void ShowWindow()
    {
        CSceneTemplatesEditor pEditor = (CSceneTemplatesEditor)GetWindow(typeof(CSceneTemplatesEditor));
        pEditor.InitEditor();
    }

    #region Override Functions

    public override void InitEditor()
    {
        base.InitEditor();

        //load data
        m_pEditingData = new CSceneTemplate();
        m_pEditingData.Load();
    }

    protected override void EditorOneElement(CSceneTemplateElement element, bool bFocus)
    {
        base.EditorOneElement(element, bFocus);

        m_pMainEditor.BeginLine();

        element.m_sDecoratePath = (string)EditorField("Decorate", element.m_sDecoratePath);
        element.m_sHeightPath = (string)EditorField("Height", element.m_sHeightPath);
        element.m_sGroundPath = (string)EditorField("Ground", element.m_sGroundPath);

        m_pMainEditor.EndLine();
    }

    #endregion
}

