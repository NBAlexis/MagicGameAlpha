using UnityEngine;
using UnityEditor;

public class CHumanoidDescBuilder : EditorWindow
{
    [MenuItem("MGA/Editor/Charactor/Humanoid Builder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CHumanoidDescBuilder));
    }

    protected GameObject m_pEditing;
	public void OnGUI()
	{
        m_pEditing = (GameObject)EditorGUILayout.ObjectField("把角色拖入", m_pEditing, typeof(GameObject), true);
        if (null != m_pEditing && GUILayout.Button("生成Atlas"))
        {
            CharactorAtlasUtil.MakeAtlasReplaceWithCloth(m_pEditing, "Fundament/Avatar/Artwork/Generated/Humanoid");
        }
        if (null != m_pEditing && GUILayout.Button("根据Desc填写脚本"))
        {
            m_pEditing.GetComponentInChildren<AHumanoidModel>().EditorFix();
        }
	}
}
