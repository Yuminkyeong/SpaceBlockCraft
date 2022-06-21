using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : CWSingleton<CameraMoving>
{
    public Camera m_Camera;
    public float m_fSpeed = 1f;

    Vector3 m_vPos;
    Vector3 m_vEnd;

    Quaternion m_qNow;
    Quaternion m_qEnd;



    public Transform m_tCamPath;

    int m_nCount = 0;

    bool m_bStart = false;

    private void Start()
    {

        if (PlayerPrefs.HasKey("LOBBY"))
        {
            m_nCount = PlayerPrefs.GetInt("LOBBY");
        }
        else
        {
            m_nCount = 35;

        }
        if (m_tCamPath.childCount <= m_nCount)
        {
            m_nCount = 0;
        }
        Transform tChild = m_tCamPath.GetChild(m_nCount);
        m_vPos = tChild.position;

    }

    public void Begin()
    {
        m_bStart = true;
        PlayerPrefs.SetInt("LOBBY", m_nCount+1);
        if (m_nCount <= 0) m_nCount = 0;
        Transform tChild = m_tCamPath.GetChild(m_nCount);
        m_vPos = tChild.position;
        m_qNow = tChild.rotation;
        m_nCount++;
        if (m_nCount >= m_tCamPath.childCount) m_nCount = 0;
         Transform tChild2 = m_tCamPath.GetChild(m_nCount);
        m_vEnd = tChild2.position;
        m_qEnd = tChild2.rotation;

        Debug.Log(string.Format("{0} ->  {1}",tChild.name,tChild2.name));

        
    }

    void  GetNext()
    {
        if (m_nCount >= m_tCamPath.childCount)
        {
            m_nCount = 0;
        }

        Transform tChild = m_tCamPath.GetChild(m_nCount++);
        m_vEnd = tChild.position;
        m_qEnd = tChild.rotation;

        Debug.Log(string.Format("Next {0} Count : {1}", tChild.name, m_nCount));
        PlayerPrefs.SetInt("LOBBY", m_nCount+1);

    }
    private void FixedUpdate()
    {
        if (!m_bStart) return;
        if (m_Camera == null) return;
        m_qNow = Quaternion.Lerp(m_qNow, m_qEnd, Time.deltaTime * m_fSpeed );
        m_vPos =Vector3.Lerp(m_vPos, m_vEnd, Time.deltaTime * m_fSpeed);

        m_Camera.transform.position = m_vPos;
        m_Camera.transform.rotation = m_qNow;

        float ff = Vector3.Distance(m_vPos, m_vEnd);
        if (ff<50f)
        {
            GetNext();
        }
    }
    public Vector3 GetPos()
    {
        return m_vPos;
    }
}
