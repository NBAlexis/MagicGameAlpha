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

    [MGADataEditor(typeof(CSceneGroudTemplateElement))]
    protected static void EditOneElement(CSceneGroudTemplateElement element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);
        element.m_vUV = (Vector4)EditorField("UV", element.m_vUV, iLineStart, ref bBeginLine);
        EndLine(ref bBeginLine);
    }

    #endregion
}
