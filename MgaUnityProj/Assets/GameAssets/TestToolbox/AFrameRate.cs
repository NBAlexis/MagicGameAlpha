using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[AddComponentMenu("MGA/Test/Frame Rate")]
public class AFrameRate : MonoBehaviour 
{
    public Text m_pText;
    protected int m_iIndex = 0;
    protected const int m_iMaxCount = 10;
    protected float[] m_fDeltaTime = {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
	
	// Update is called once per frame
	void Update ()
	{
	    m_fDeltaTime[m_iIndex] = Time.deltaTime;
	    m_pText.text = Mathf.RoundToInt(10.0f/m_fDeltaTime.Sum()).ToString();
	    ++m_iIndex;
	    if (m_iIndex >= m_iMaxCount)
	    {
	        m_iIndex = 0;
	    }
	}
}
