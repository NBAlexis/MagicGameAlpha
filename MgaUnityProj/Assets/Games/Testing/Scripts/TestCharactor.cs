using UnityEngine;
using System.Collections;

public class TestCharactor : MonoBehaviour
{
    public const int m_iMaxCount = 10;
    public const int m_iPrefabCount = 1;
    public ACharactor m_pModel;
    public Transform m_pLookingAt;
    public BezierLine m_pBezier;

    protected float m_fCameraRate = 0.0f;

	// Use this for initialization
	void Start () 
    {
	    m_pBezier.UseAsGame();
	    Application.targetFrameRate = 60;

        GameObject[] cs = new GameObject[m_iMaxCount];
        for (int i = 0; i < m_iMaxCount; ++i)
        {
            float fX = 1.0f * (i / 64) - 31.5f;
            float fY = 1.0f * (i % 64) - 31.5f;

            if (0 == i)
            {
                cs[0] = m_pModel.gameObject;
                cs[i].GetComponent<ACharactor>().m_pModel.Randomize(true);
                cs[i].GetComponent<ACharactor>()
                    .m_pModel.SetCamp((ECharactorCamp)Random.Range(0, (int)ECharactorCamp.ECC_Max));
                cs[i].GetComponent<ACharactor>()
                    .m_pModel.SetVisible((ECharactorVisible)Random.Range(0, (int)ECharactorVisible.ECV_None));
            }
            else if (i < m_iPrefabCount)
            {
                cs[i] = Instantiate(m_pModel.gameObject);
                cs[i].GetComponent<ACharactor>().m_pModel.Randomize(true);
                cs[i].GetComponent<ACharactor>()
                    .m_pModel.SetCamp((ECharactorCamp) Random.Range(0, (int) ECharactorCamp.ECC_Max));
                cs[i].GetComponent<ACharactor>()
                    .m_pModel.SetVisible((ECharactorVisible) Random.Range(0, (int) ECharactorVisible.ECV_None));
            }
            else
            {
                cs[i] = Instantiate(cs[Random.Range(0, m_iPrefabCount)]);
            }
            cs[i].transform.position = new Vector3(fX, 0.0f, fY);
            cs[i].name = "Unit" + (i + 1);
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
	    m_fCameraRate += Time.deltaTime*0.03f;
	    float fRate = Mathf.PingPong(m_fCameraRate, 1.0f);
	    Vector3 pos = m_pBezier.GetLocation(fRate);

	    Camera.main.transform.position = pos;
        Camera.main.transform.LookAt(m_pLookingAt);
	}
}
