using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CHumanoidDescEditor : TMGTextDataEditor<CHumanoidDescElement>
{

    [MenuItem("MGA/Editor/Charactor/Humanoid Desc Editor")]
    public static void ShowWindow()
    {
        CHumanoidDescEditor pEditor = (CHumanoidDescEditor)GetWindow(typeof(CHumanoidDescEditor));
        pEditor.InitEditor();
    }

    #region Override Functions

    public override void InitEditor()
    {
        base.InitEditor();

        //load data
        m_pEditingData = new CHumanoidDesc();
        m_pEditingData.Load();
    }

    protected override void EditorOneElement(CHumanoidDescElement element, bool bFocus)
    {
        base.EditorOneElement(element, bFocus);

        element.m_sMeshPath = (string)EditorField("MeshPath", element.m_sMeshPath);
        element.m_sTexturePath = (string)EditorField("TexturePath", element.m_sTexturePath);
        element.m_sPrefabPath = (string)EditorField("PrefabPath", element.m_sPrefabPath);
        element.m_sDependency = (string[])EditorField("Dependency", element.m_sDependency);

        GUILayout.BeginHorizontal();
        element.m_vLocalPos = (Vector3)EditorField("LocalPos", element.m_vLocalPos);
        element.m_vLocalEular = (Vector3)EditorField("LocalEular", element.m_vLocalEular);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        element.m_vMinSize = (Vector3)EditorField("MinSize", element.m_vMinSize);
        element.m_vMaxSize = (Vector3)EditorField("MaxSize", element.m_vMaxSize);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        element.m_eMatType = (ECharactorMatType)EditorField("MatType", element.m_eMatType);
        element.m_ePos = (EHumanoidComponentPos)EditorField("Pos", element.m_ePos);
        element.m_eWeaponType = (EHumanoidWeaponType)EditorField("WeaponType", element.m_eWeaponType);
        GUILayout.EndHorizontal();
    }

    #endregion
}
