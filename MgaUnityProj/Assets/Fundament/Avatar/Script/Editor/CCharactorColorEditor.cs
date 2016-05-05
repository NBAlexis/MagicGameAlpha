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

        GUILayout.BeginHorizontal();

        element.m_cColor = (Color)EditorField("Color", element.m_cColor);
        element.m_bMain = (bool) EditorField("IsMain", element.m_bMain);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        element.m_eMainColor = (ECharactorMainColor)EditorField("Main Color", element.m_eMainColor);
        element.m_eSubColor = (ECharactorSubColor)EditorField("Sub Color", element.m_eSubColor);

        GUILayout.EndHorizontal();
    }

    #endregion
}
