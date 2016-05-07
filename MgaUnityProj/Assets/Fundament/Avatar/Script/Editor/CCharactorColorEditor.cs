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

    protected override void EditorOneElement(CCharactorColorElement element, bool bFocus)
    {
        base.EditorOneElement(element, bFocus);

        m_pMainEditor.BeginLine();

        element.m_cColor = (Color)EditorField("Color", element.m_cColor);
        element.m_bMain = (bool) EditorField("IsMain", element.m_bMain);

        m_pMainEditor.EndLine();

        m_pMainEditor.BeginLine();

        element.m_eMainColor = (ECharactorMainColor)EditorField("Main Color", element.m_eMainColor);
        element.m_eSubColor = (ECharactorSubColor)EditorField("Sub Color", element.m_eSubColor);

        m_pMainEditor.EndLine();
    }

    #endregion
}
