using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;

public class MoveObject : MonoBehaviour
{

    public float m_fLifetime;
    public CallBackFunction CBCloseFuc;
    public GameObject m_gStartObject;
    public GameObject m_gEndObject;
    public GameObject m_gTargetObject;

    public string m_szStartObject;
    public string m_szEndObject;
    public string m_szTargetObject;

    public bool m_bLookTarget=true;
    

    public float m_fSpeed;

    public bool m_bRealtime;


    public float m_fRestDist = 1f; // 남아 있는 거리 

         
    Vector3 m_vStart;
    Vector3 m_vEnd;
    Quaternion m_qStart;
    Quaternion m_qEnd;

    float m_fStartTime;
    bool m_bOnce = false;
    float m_ftime = 0;

    public bool m_bHeroStop=false;

    bool m_bTempHero = false;
    void Start()
    {
    }
    private void OnDestroy()
    {
    }

    void SetMove(string szTarget,GameObject gLook,float fSpeed)
    {
        m_szEndObject = szTarget;

    }

    private void OnEnable()
    {
        m_bOnce = false;// 클리어

    }
    void FindObject()
    {
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


        if (m_gStartObject)
        {
            m_vStart = m_gStartObject.transform.position;
            m_qStart = m_gStartObject.transform.rotation;
        }
        else
        {
            m_vStart = m_gTargetObject.transform.position;
            m_qStart = m_gTargetObject.transform.rotation;

        }
        if (m_gEndObject)
        {
            m_vEnd = m_gEndObject.transform.position;
            m_qEnd = m_gEndObject.transform.rotation;
        }


    }
    bool Once()
    {
        if (m_bOnce) return true;
        FindObject();

        if (m_gTargetObject && m_gEndObject)
        {
            CWObject cg = m_gTargetObject.GetComponent<CWObject>();
            if (cg)
            {
                cg.SetDirection(m_gEndObject.transform.position);
            }
            else
            {
                m_gTargetObject.transform.LookAt(m_gEndObject.transform);
            }

        }

        m_fStartTime = Time.time;
        m_bOnce = true;
        return m_bOnce;
    }
    void Run()
    {
        m_bOnce = Once();
        if (!m_bOnce) return;

        float fpast = Time.time - m_fStartTime;
        if (fpast > m_fLifetime)
        {
        //    Destroy(this);
         //   return;
        }

        float fRatetime = fpast / m_fLifetime;//0~ 1
        if (m_bRealtime)
        {
            if (m_gStartObject)
            {
                m_vStart = m_gStartObject.transform.position;
            }
            if (m_gEndObject)
            {
                m_vEnd = m_gEndObject.transform.position;
            }
            if (m_fSpeed == 0)
            {

                m_ftime += (Time.deltaTime / m_fLifetime);
                m_gTargetObject.transform.position = Vector3.Lerp(m_vStart, m_vEnd, m_ftime);
            }
            else
            {


                Vector3 vDir;
                Vector3 v1 = m_vStart;
                Vector3 v2 = m_vEnd;
                vDir = v2 - v1;
                vDir.Normalize();

                Vector3 vPos = m_gTargetObject.transform.position;
                vPos+= (vDir * m_fSpeed* Time.deltaTime);
                m_gTargetObject.transform.position = vPos;

               

            }

        }
        else
        {
            if (m_fSpeed == 0)
            {

                m_ftime += (Time.deltaTime / m_fLifetime);
                m_gTargetObject.transform.position = Vector3.Lerp(m_vStart, m_vEnd, fRatetime);
            }
            else
            {


                Vector3 vDir;
                Vector3 v1 = m_vStart;
                Vector3 v2 = m_vEnd;
                vDir = v2 - v1;
                vDir.Normalize();
                m_gTargetObject.transform.position = v1 + vDir * fpast * m_fSpeed;

            }

        }

        if (m_bLookTarget)// 타겟을 바라본다
        {
            if(m_gTargetObject && m_gEndObject)
            {
                m_gTargetObject.transform.LookAt(m_gEndObject.transform);
            }
        }
        else // 좌표를 똑같이 맞춘다 
        {
            
            m_gTargetObject.transform.rotation = Quaternion.Slerp(m_qStart, m_qEnd, fRatetime);

        }

        float fdist = Vector3.Distance(m_gTargetObject.transform.position, m_vEnd);
        if (fdist <= m_fRestDist)
        {
            m_gTargetObject.transform.position = m_vEnd;
            m_gTargetObject.transform.rotation = m_qEnd;

            CBCloseFuc?.Invoke();
            Destroy(this);
            return;
        }

    }
    private void FixedUpdate()
    {
        Run();
    }
    void Update()
    {

        
    }
}
