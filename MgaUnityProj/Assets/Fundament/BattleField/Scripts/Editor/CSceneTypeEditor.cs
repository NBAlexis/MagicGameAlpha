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

    [MGADataEditor(typeof(CSceneTypeElement))]
    protected static void EditOneElement(CSceneTypeElement element, int iLineStart)
    {
        bool bBeginLine = false;
        BeginLine(iLineStart, ref bBeginLine);

        element.m_iCliffType = (int)EditorField("Cliff", element.m_iCliffType, iLineStart, ref bBeginLine);
        element.m_bHasGroundOffset = (bool)EditorField("HasOffset", element.m_bHasGroundOffset, iLineStart, ref bBeginLine);
        element.m_eEdgeType = (ESceneEdgeType)EditorField("Edge", element.m_eEdgeType, iLineStart, ref bBeginLine);
        element.m_bCanRot = (bool)EditorField("Rot", element.m_bCanRot, iLineStart, ref bBeginLine);

        EndLine(ref bBeginLine);
    }

    #endregion
}
