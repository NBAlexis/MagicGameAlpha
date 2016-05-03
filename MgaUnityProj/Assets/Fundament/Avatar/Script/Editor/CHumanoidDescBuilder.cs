using UnityEngine;
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
        if (null != m_pEditing && GUILayout.Button("生成Atlas"))
        {
            CharactorAtlasUtil.MakeAtlasReplace(m_pEditing, "Fundament/Avatar/Artwork/Generated/Humanoid");
        }
        if (null != m_pEditing && GUILayout.Button("生成Desc"))
        {
            BuildDesc(m_pEditing);
        }
        if (null != m_pEditing && GUILayout.Button("根据Desc填写脚本"))
        {
        }
	}

    protected void BuildDesc(GameObject pEditing)
    {
        CHumanoidDesc newDesc = new CHumanoidDesc();
        foreach (MeshRenderer mesh in pEditing.GetComponentsInChildren<MeshRenderer>())
        {
            CHumanoidDescElement element = newDesc.CreateElement();
            if (!newDesc.ChangeName(element.m_sElementName, mesh.gameObject.name))
            {
                CRuntimeLogger.LogError("名称重复了:" + mesh.gameObject.name);
                return;
            }
            element.m_sObjectPath = CommonFunctions.FindFullName(pEditing.gameObject, mesh.gameObject);
            element.m_sObjectPath = element.m_sObjectPath.Replace(pEditing.gameObject.name + "/", "");
            if (mesh.transform.parent != pEditing.transform)
            {
                element.m_sDependency = new[] { mesh.transform.parent.gameObject.name };
            }
        }
        newDesc.Save();
    }
}
