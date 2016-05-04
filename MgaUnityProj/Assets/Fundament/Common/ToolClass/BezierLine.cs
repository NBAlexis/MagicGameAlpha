using UnityEngine;
//using UnityEditor;
//using System.Collections;

[ExecuteInEditMode]
public class BezierLine : MonoBehaviour
{

    public bool BackToEditMode = false;
	public Color m_cDrawColor = Color.white;
	public Transform[] m_pInterplatePoints;
	public Transform[] m_pInterplateControlPoints;
	public int m_iSampleNum = 64;
	public int m_iInterplatePointNum = 0;
	protected bool m_bEditorMode = true;
	protected bool m_bPointFix = false;
	
	public Vector3[] m_vInterplationPoint = null;
	public Vector3[] m_vInterplationControlPoint = null;
	public Vector3[] m_vSamplePoints = null;
	
	public void Start() {
		if (null != m_pInterplatePoints && m_pInterplatePoints.Length > 2 && m_iInterplatePointNum == m_pInterplatePoints.Length) {
			m_bPointFix = true;
		}
	}
	
	public void FixPoints(int iPointNumber) {
		m_pInterplatePoints = new Transform[iPointNumber];
		m_pInterplateControlPoints = new Transform[iPointNumber];
		m_vInterplationPoint = new Vector3[iPointNumber];
		m_vInterplationControlPoint = new Vector3[iPointNumber];
		for (int i = 0; i < iPointNumber; ++i) {
			m_pInterplatePoints[i] = GameObject.Find(gameObject.name + "/interplate" + i.ToString()).transform;
			m_vInterplationPoint[i] = m_pInterplatePoints[i].position;
			m_pInterplateControlPoints[i] = GameObject.Find(gameObject.name + "/interplate" + i.ToString() + "/control").transform;
			m_vInterplationControlPoint[i] = m_pInterplateControlPoints[i].position;
		}
		m_iInterplatePointNum = iPointNumber;
		m_bPointFix = true;
		SampleLocations();
	}
	
	public void UseAsGame() {
		m_bEditorMode = false;
		for (int i = 0; i < m_iInterplatePointNum; ++i) {
			Object.Destroy(m_pInterplatePoints[i].gameObject.GetComponent<Renderer>());
            Object.Destroy(m_pInterplatePoints[i].gameObject.GetComponent<Collider>());
            Object.Destroy(m_pInterplateControlPoints[i].gameObject.GetComponent<Renderer>());
            Object.Destroy(m_pInterplateControlPoints[i].gameObject.GetComponent<Collider>());			
		}
	}
	
	public void Update() {
	    if (BackToEditMode) {
	        BackToEditMode = false;
	        m_bEditorMode = true;
	        m_bPointFix = true;
	    }
		if (m_bEditorMode && m_bPointFix) {
			bool bChanged = false;
			for (int i = 0; i < m_iInterplatePointNum; ++i) {
				if (m_vInterplationPoint[i] != m_pInterplatePoints[i].position) {
					bChanged = true;
					break;
				}
				if (m_vInterplationControlPoint[i] != m_pInterplateControlPoints[i].position) {
					bChanged = true;
					break;
				}				
			}
			
			if (bChanged) {
				SampleLocations();
			}

			for (int i = 0; i < (m_iInterplatePointNum - 1) * m_iSampleNum - 1; ++i) {
				Debug.DrawLine(m_vSamplePoints[i], 
				               m_vSamplePoints[i + 1], m_cDrawColor);
			}			
			
			for (int i = 0; i < m_iInterplatePointNum; ++i) {
				Debug.DrawLine(m_pInterplateControlPoints[i].position, 
				               2.0f * m_pInterplatePoints[i].position - m_pInterplateControlPoints[i].position, m_cDrawColor);
			}
		}
	}
	
	public void SampleLocations() {
		for (int i = 0; i < m_iInterplatePointNum; ++i) {
			m_vInterplationPoint[i] = m_pInterplatePoints[i].position;
			m_vInterplationControlPoint[i] = m_pInterplateControlPoints[i].position;
		}
		
		m_vSamplePoints = new Vector3[m_iSampleNum * (m_iInterplatePointNum - 1) + 1];
		for (int i = 0; i < m_iInterplatePointNum - 1; ++i) {
			Vector3 P0 = m_vInterplationPoint[i];
			Vector3 P1 = m_vInterplationControlPoint[i];
			Vector3 P2 = 2.0f * m_vInterplationPoint[i + 1] - m_vInterplationControlPoint[i + 1];
			Vector3 P3 = m_vInterplationPoint[i + 1];
			
			float Length = 0.0f;
			float q = 1.0f/((float)m_iSampleNum);
			
			Vector3 a = P0;
			Vector3 b = 3.0f * (P1 - P0);
			Vector3 c = 3.0f * (P2 - 2.0f * P1 + P0);
			Vector3 d = P3 - 3.0f * P2 + 3.0f * P1 - P0;
			
			Vector3 S  = a;						// the poly value
			Vector3 U  = b*q + c*q*q + d*q*q*q;	// 1st order diff (quadratic)
			Vector3 V  = 2.0f*c*q*q + 6.0f*d*q*q*q;	// 2nd order diff (linear)
			Vector3 W  = 6.0f*d*q*q*q;				// 3rd order diff (constant)
			
			Vector3 OldPos = P0;
			m_vSamplePoints[m_iSampleNum * i] = P0;
			
			for (int j = m_iSampleNum * i + 1; j < m_iSampleNum * (i + 1) + 1; ++j) {
				S += U;			// update poly value
				U += V;			// update 1st order diff value
				V += W;			// update 2st order diff value
				// 3rd order diff is constant => no update needed.
	
				// Update Length.
				Length += (S - OldPos).magnitude;
				OldPos  = S;
				m_vSamplePoints[j] = S;
			}
		}
	}
	
	public Vector3 GetLocation(float fRate) {
		fRate = Mathf.Clamp(fRate, 0.0f, 0.9999f);
		
		fRate = fRate * (m_iSampleNum * (m_iInterplatePointNum - 1) - 1);//0.0f to 190.999f
		int iPreLoc = Mathf.FloorToInt(fRate);
		fRate = fRate - iPreLoc;
		return m_vSamplePoints[iPreLoc] * (1.0f - fRate) + m_vSamplePoints[iPreLoc + 1] * fRate;
	}
}
