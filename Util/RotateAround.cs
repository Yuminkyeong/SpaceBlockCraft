using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class RotateAround : MonoBehaviour
{


    CallBackFunction m_CBFunction;
    public Transform Target;
    public float m_fSpeed = 10f;

    
    public float m_fOvalMin=0.5f;
    public float m_fOvalMax=1.2f;

    float m_fAngle = 0;
    Vector3 m_vStart;

    float m_fDist = 0;
    
    



    bool m_bEndFlag = false;
    float m_fEndYaw = 0;

    bool m_bStop = false;
    bool m_bOnce = false;
    
    void RotateRound()
    {
        float fRet = m_fAngle + (m_fSpeed * Time.deltaTime);
        if (m_bEndFlag)
        {
            if(CWMath.IsEqualAnlge(fRet, m_fEndYaw,10f))
            {
                m_fAngle = m_fEndYaw;
                m_bEndFlag = false;
                m_bStop = true;
                if(m_CBFunction!=null)
                {
                    m_CBFunction();
                }
            }
            else
            {
                m_fAngle = fRet;
            }
        }
        else
        {
            m_fAngle = fRet;
        }
        _SetAngle(m_fAngle);

    }
    void _SetAngle(float fAngle)
    {
        m_fAngle = fAngle;
        Vector3 vDir = CWMath.CalYaw(fAngle, Vector3.forward);
        transform.position = Target.position + vDir * m_fDist;

    }
    void OnceRun()
    {
        if (m_bOnce) return;
        m_bOnce = true;
        m_vStart = transform.position;
        
        m_fDist = Vector3.Distance(m_vStart, Target.position);
        

    }
    private void OnEnable()
    {
        m_bOnce = false;
        m_bStop = false;
    }
    private void Update()
    {
        if (m_bStop) return;
        if (Target == null) return;
        OnceRun();
        RotateRound();
    }

    public void ResetAngle(float fYaw, CallBackFunction cbfunc)
    {
        _SetAngle(fYaw);
        m_bEndFlag = false;
        m_bStop = true;
        enabled = false;
        cbfunc();
    }

    // 각도 만큼 움직인다 
    public void SetAngle(float fYaw, CallBackFunction cbfunc)
    {
        m_CBFunction = cbfunc;
        m_bEndFlag = true;
        m_fEndYaw = fYaw;
        m_bStop = false;
        enabled = true;
    }
    public void ResetPlay()
    {
        m_bStop = false;
        m_bEndFlag = false;
        enabled = true;
    }

   
}

