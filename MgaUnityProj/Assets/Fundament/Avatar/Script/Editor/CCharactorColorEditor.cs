using UnityEngine;
using UnityEditor;

public class CCharactorColorEditor : TMGTextDataEditor<CCharactorColorElement>
{

    [MenuItem("MGA/Editor/Charactor/Charactor Color Editor")]
    public static void ShowWindow()
    {
        CCharactorColorEditor pEditor = (CCharactorColorEditor)GetWindow(typeof(CCharactorColorEditor));
        pEditor.InitEditor();
    }

    #region Override Functions

    public override void InitEditor()
    {
        base.InitEditor();

        //load data
        m_pEditingData = new CCharactorColor();
        m_pEditingData.Load();
    }

    [MGADataEditor(typeof(CCharactorColorElement))]
    protected static void EditOneElement(CCharactorColorElement element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);

        element.m_cColor = (Color)EditorField("Color", element.m_cColor, iLineStart, ref bBeginLine);
        element.m_bMain = (bool)EditorField("IsMain", element.m_bMain, iLineStart, ref bBeginLine);

        EndLine(ref bBeginLine);

        BeginLine(iLineStart, ref bBeginLine);

        element.m_eMainColor = (ECharactorMainColor)EditorField("Main Color", element.m_eMainColor, iLineStart, ref bBeginLine);
        element.m_eSubColor = (ECharactorSubColor)EditorField("Sub Color", element.m_eSubColor, iLineStart, ref bBeginLine);

        EndLine(ref bBeginLine);
    }

    #endregion
}
