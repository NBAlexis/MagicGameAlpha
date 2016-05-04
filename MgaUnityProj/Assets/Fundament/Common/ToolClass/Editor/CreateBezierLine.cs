using UnityEngine;
using UnityEditor;

public class CreateBezierLine : ScriptableWizard {
	
	static public int m_iCreatedNum;
	public string LineName = "";
	public int InterplateNumber = 4;
	
	[MenuItem ("GameObject/MGA/Create Bezier Line")]
	static void CreateWizard () {
		++m_iCreatedNum;
		
		CreateBezierLine pBezierEditor = DisplayWizard<CreateBezierLine>("Create Bezier", "Create");
		pBezierEditor.LineName = "Bezier" + m_iCreatedNum;
	}
	
	void OnWizardCreate () {
	    GameObject pParentObj = new GameObject {name = LineName};
	    pParentObj.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		
		for (int i = 0; i < InterplateNumber; ++i) {
			GameObject pInterplateObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pInterplateObj.name = "interplate" + i;
			pInterplateObj.transform.position = new Vector3(10.0f * i, 0.0f, 0.0f);
			pInterplateObj.transform.parent = pParentObj.transform;
			
			GameObject pControlObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			pControlObj.name = "control";
			pControlObj.transform.position = new Vector3(10.0f * i, 0.0f, 10.0f);
			pControlObj.transform.parent = pInterplateObj.transform;

		}
		
		BezierLine pLine = pParentObj.AddComponent<BezierLine>();
		pLine.FixPoints(InterplateNumber);
	}
}
