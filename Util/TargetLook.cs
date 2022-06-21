using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using DG.Tweening;
public class TargetLook : MonoBehaviour {

    public GameObject m_gSource;
    public GameObject m_gTarget;

    public string m_szSource;
    public string m_szTarget;

    public float m_fSpeed = 5;

    Transform m_tActor;
    Transform m_tObject;

    void Start () {
        if (m_gTarget == null)
        {
            if (CWLib.IsString(m_szTarget))
            {
                m_gTarget = CWGlobal.FindObject(m_szTarget);
            }
        }
        if (m_gSource == null)
        {
            if (CWLib.IsString(m_szSource))
            {
                m_gSource = CWGlobal.FindObject(m_szSource);
            }
            else
            {
                m_gSource = gameObject;
            }


        }
        if (m_gTarget == null) return;
        if (m_gSource == null) return;

        m_tActor = m_gTarget.transform;
        m_tObject = m_gSource.transform;

    }

    bool m_bflag = false;
    void LateUpdate()
    {
        if (m_bflag) return;
        m_bflag = true;
        m_tActor.DOLookAt(m_tObject.position, 2f).OnComplete(() => { m_bflag = false; });
    }

}
