using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWHeroEditCamera : MonoBehaviour {

    public float m_fSpeed = 75f;
    public float m_fWheeSpeed = 1.5f;
    public float m_fKeySpeed = 10f;
    public float m_fDagSpeedup = 10f;
    public float m_fDagSpeedright = 20f;

    public float m_fPowerDelta = 1f;

    Vector3 m_vPrev;
    bool m_bDrag;
    bool m_bDrag2;

    public Camera m_kCamera;
    void Start()
    {
        m_vPrev = Input.mousePosition;
        m_bDrag = false;
        if (m_kCamera == null) m_kCamera = Camera.main;
    }
    private void OnEnable()
    {
        m_vPrev = Input.mousePosition;
        m_bDrag = false;
        if (m_kCamera == null) m_kCamera = Camera.main;

    }
    void OnDrag(Vector3 vDir, float fPower)
    {
        Vector3 v = m_kCamera.transform.eulerAngles;

        v += vDir * m_fSpeed * Time.deltaTime * fPower;
        m_kCamera.transform.eulerAngles = v;

        //m_kCamera.transform.Rotate()
    }

    void OnDrag2(Vector3 vDir, float fPower)
    {


        //Vector3 vPos = CWMath.CalAngle(vDir.x,vDir.y, m_kCamera.transform.forward);// - m_kCamera.transform.forward;
        //        Quaternion qq = Quaternion.Euler(new Vector3(vDir.x, vDir.y, 0));
        //      Vector3 vPos = qq * m_kCamera.transform.forward;
        Vector3 vPos = m_kCamera.transform.right;
        vPos.y = 0;
        m_kCamera.transform.position += vPos * Time.deltaTime * m_fDagSpeedup * (-vDir.y) * fPower;


        m_kCamera.transform.position += Vector3.up * Time.deltaTime * m_fDagSpeedright * (vDir.x);




    }

    void OnWheel(float fDelta)
    {
        m_kCamera.transform.position += m_kCamera.transform.forward * m_fWheeSpeed * fDelta;
    }
    private void OnApplicationFocus(bool focus)
    {
        m_vPrev = Input.mousePosition;
    }

    void Update()
    {
        if (UIInput.selection) return;

        Vector3 vDir = m_vPrev - Input.mousePosition;
        float fPower = Vector3.Distance(m_vPrev, Input.mousePosition) * m_fPowerDelta;
        vDir.Normalize();
        Vector3 vDir2 = new Vector3(vDir.y, -vDir.x, 0);
        m_vPrev = Input.mousePosition;
        if (Input.GetMouseButtonDown(1))
        {
            m_bDrag = true;
            return;
        }
        if (Input.GetMouseButtonUp(1))
        {
            m_bDrag = false;
        }
        if (m_bDrag)
        {
            OnDrag(vDir2, fPower / 2);
        }
        if (m_bDrag2)
        {
            OnDrag2(vDir2, fPower);
        }
        if (Input.GetMouseButtonDown(2))
        {
            m_bDrag2 = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            m_bDrag2 = false;
        }


        if (Input.mouseScrollDelta.y != 0)
        {
            OnWheel(Input.mouseScrollDelta.y);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Vector3 vPos = m_kCamera.transform.forward;
            vPos.y = 0;
            m_kCamera.transform.position += vPos * Time.deltaTime * m_fKeySpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Vector3 vPos = -m_kCamera.transform.forward;
            vPos.y = 0;
            m_kCamera.transform.position += vPos * Time.deltaTime * m_fKeySpeed;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Vector3 vPos = -m_kCamera.transform.right;
            vPos.y = 0;
            m_kCamera.transform.position += vPos * Time.deltaTime * m_fKeySpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Vector3 vPos = m_kCamera.transform.right;
            vPos.y = 0;
            m_kCamera.transform.position += vPos * Time.deltaTime * m_fKeySpeed;
        }


    }
}
