using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CSeneTypeEditor : TMGTextDataEditor<CSceneTypeElement>
{
    [MenuItem("MGA/Editor/Scene/Scene Type Editor")]
    public static void ShowWindow()
    {
        CSeneTypeEditor pEditor = (CSeneTypeEditor)GetWindow(typeof(CSeneTypeEditor));
        pEditor.InitEditor();
    }

    #region Override Functions

    public override void InitEditor()
    {
        base.InitEditor();

        //load data
        m_pEditingData = new CSceneType();
        m_pEditingData.Load();
    }

    protected override void EditorOneElement(CSceneTypeElement element, bool bFocus)
    {
        base.EditorOneElement(element, bFocus);

        m_pMainEditor.BeginLine();

        element.m_iCliffType = (int)EditorField("Cliff", element.m_iCliffType);
        element.m_bHasGroundOffset = (bool)EditorField("HasOffset", element.m_bHasGroundOffset);
        element.m_eEdgeType = (ESceneEdgeType)EditorField("Edge", element.m_eEdgeType);
        element.m_bCanRot = (bool)EditorField("Rot", element.m_bCanRot);

        m_pMainEditor.EndLine();
    }

    #endregion
}
