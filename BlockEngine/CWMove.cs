using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CWStruct;

public class CWMove
{


    Transform m_tStart;
    Transform m_tTarget;
    float m_fStarttime;
    public float m_fSpeed;
    float m_fAcelSpeed;
    Vector3 m_vCurve;

    float m_fPrevdist = 0;
    Vector3 m_vDir;
    Vector3 m_vStart;

    float m_fCurLifetime = 0.5f;
    float m_fCurSpeed = 1f;

    float m_fTargetDist = 0;

    bool m_bStop = false;
    public CWMove(Transform tStart, Transform tTarget, Vector3 vCurve, float fSpeed = 20f, float fAcelSpeed = 20f, float fCurtime = 1f, float fcurspeed = 20f)
    {
        m_vStart = tStart.position;
        m_tStart = tStart;
        m_tTarget = tTarget;
        m_fStarttime = Time.time;
        m_fSpeed = fSpeed;
        m_fAcelSpeed = fAcelSpeed;
        m_vCurve = vCurve;
        m_fPrevdist = 0;
        m_fCurLifetime = fCurtime;
        m_fCurSpeed = fcurspeed;
        m_fTargetDist = 0;
        m_bStop = false;

    }
    Vector3 GetCurveVector(float fPastTime)
    {
        if (m_vCurve == null) return Vector3.zero;
        if (fPastTime >= m_fCurLifetime) return Vector3.zero;
        float fSpeed = m_fCurSpeed * (1f - fPastTime / m_fCurLifetime);
        return m_vCurve * fSpeed * Time.deltaTime;
    }
    // 시간이 지나면서 변하는 값
    public Vector3 GetMove()
    {
        if (m_bStop) return Vector3.zero;
        float fPastTime = Time.time - m_fStarttime;
        float fSec = fPastTime;

        float fdist = m_fSpeed * fSec + 0.5f * m_fAcelSpeed * fSec * fSec;

        float fDelta = fdist - m_fPrevdist;
        m_fPrevdist = fdist;

        float ff = Vector3.Distance(m_tTarget.position, m_tStart.position);


        if (ff < fDelta + 0.1f)
        {
            m_bStop = true;
            return Vector3.zero;
        }
        //Debug.Log(fPastTime.ToString() +"--"+  ff.ToString()+ "  "+ m_fTargetDist.ToString() + "  " + fff.ToString());
        m_fTargetDist = ff;

        m_vDir = m_tTarget.position - m_tStart.position;

        m_vDir.Normalize();

        Vector3 vdir = fDelta * m_vDir + GetCurveVector(fPastTime);


        return vdir;
    }

}
