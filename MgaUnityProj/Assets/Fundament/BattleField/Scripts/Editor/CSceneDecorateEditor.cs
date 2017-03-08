using UnityEditor;

public class CSceneDecorateEditor : TMGTextDataEditor<CSceneDecorateElement>
{
    private string m_sType = "Default";

    [MenuItem("MGA/Editor/Scene/Scene Decorate Editor")]
    public static void ShowWindow()
    {
        CSceneDecorateEditor pEditor = (CSceneDecorateEditor)GetWindow(typeof(CSceneDecorateEditor));
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
        m_pEditingData = new CSceneDecorate();
        ((CSceneDecorate)m_pEditingData).m_sSceneType = m_sType;
        m_pEditingData.Load();
    }

    [MGADataEditor(typeof(CSceneDecorateElement))]
    protected static void EditOneElement(CSceneDecorateElement element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);

        element.m_iDecrateRepeat = (int)EditorField("Repeat", element.m_iDecrateRepeat, iLineStart, ref bBeginLine);
        element.m_iDecrateSize = (int)EditorField("Size", element.m_iDecrateSize, iLineStart, ref bBeginLine);
        element.m_bBlockPathfinding = (bool)EditorField("Block", element.m_bBlockPathfinding, iLineStart, ref bBeginLine);
        element.m_bOnlyRotateY = (bool)EditorField("RotateY", element.m_bOnlyRotateY, iLineStart, ref bBeginLine);

        EndLine(ref bBeginLine);
    }

    #endregion
}
