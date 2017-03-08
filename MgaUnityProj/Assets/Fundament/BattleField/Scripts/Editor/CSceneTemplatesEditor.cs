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

    [MGADataEditor(typeof(CSceneTemplateElement))]
    protected static void EditOneElement(CSceneTemplateElement element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);

        element.m_sDecoratePath = (string)EditorField("Decorate", element.m_sDecoratePath, iLineStart, ref bBeginLine);
        element.m_sHeightPath = (string)EditorField("Height", element.m_sHeightPath, iLineStart, ref bBeginLine);
        element.m_sGroundPath = (string)EditorField("Ground", element.m_sGroundPath, iLineStart, ref bBeginLine);

        EndLine(ref bBeginLine);
    }

    #endregion
}

