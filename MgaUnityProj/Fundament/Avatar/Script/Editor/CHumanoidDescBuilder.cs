using UnityEngine;
using System.Collections;
using UnityEditor;

public class CHumanoidDescBuilder : EditorWindow
{
    [MenuItem("MGA/Editor/Charactor/Humanoid Desc Builder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CHumanoidDescBuilder));
    }

    protected GameObject m_pEditing;
	public void OnGUI()
	{
        m_pEditing = (GameObject)EditorGUILayout.ObjectField("把角色拖入", m_pEditing, typeof(GameObject), true);
        if (null != m_pEditing && GUILayout.Button("生成"))
	    {
	        
	    }
	}
}
