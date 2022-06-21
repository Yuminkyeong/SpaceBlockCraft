using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;

public class RotateObject : MonoBehaviour
{
    public float m_fLifetime;
    public GameObject m_gStartObject;
    public GameObject m_gEndObject;
    public GameObject m_gTargetObject;

    public string m_szStartObject;
    public string m_szEndObject;
    public string m_szTargetObject;



    public float m_fSpeed;

    float m_fStartTime;
    bool m_bOnce = false;
    void Start()
    {

    }
    private void OnEnable()
    {
        m_bOnce = false;// 클리어





    }
    bool Once()
    {
        if (m_bOnce) return true;

        if (CWLib.IsString(m_szStartObject))
        {
            m_gStartObject = CWGlobal.FindObject(m_szStartObject);
        }
        if (CWLib.IsString(m_szEndObject))
        {
            m_gEndObject = CWGlobal.FindObject(m_szEndObject);
        }
        if (CWLib.IsString(m_szTargetObject))
        {
            m_gTargetObject = CWGlobal.FindObject(m_szTargetObject);
        }
        if (m_gTargetObject == null)
        {
            m_gTargetObject = gameObject;
        }
        m_bOnce = true;
        m_fStartTime = Time.time;

        return false;
    }
    void Run()
    {
        m_bOnce = Once();
        if (!m_bOnce) return;

        float fpast = Time.time - m_fStartTime;
        if (fpast >= m_fLifetime)
        {
            Destroy(this);
        }

        if (m_fSpeed == 0)
        {
            float ftime = 0;
            ftime = (Time.deltaTime / m_fLifetime);
            m_gTargetObject.transform.rotation = Quaternion.Slerp(m_gStartObject.transform.rotation, m_gEndObject.transform.rotation, ftime);
        }
        else
        {

            float ftime = 0;
            ftime = Time.deltaTime * m_fSpeed;
            m_gTargetObject.transform.rotation = Quaternion.Slerp(m_gTargetObject.transform.rotation, m_gEndObject.transform.rotation, ftime);

        }


    }
    void Update()
    {

        Run();
    }
}
