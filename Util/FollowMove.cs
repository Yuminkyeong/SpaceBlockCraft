using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using DG.Tweening;
public class FollowMove : MonoBehaviour
{
    public GameObject m_gFollowObject;
    public GameObject m_gTargetObject;

    public string m_szFollowObject;
    public string m_szTargetObject;

    public float m_fSpeed = 20f;
    public float m_fSec = 2f;
    public float m_fMindist = 5f;// 최소 거리 유지 

    bool m_bOnce = false;

    private void OnEnable()
    {
        m_bOnce = false;// 클리어

    }
    void FindObject()
    {
        if (CWLib.IsString(m_szFollowObject))
        {
            m_gFollowObject = CWGlobal.FindObject(m_szFollowObject);
        }
        if (CWLib.IsString(m_szTargetObject))
        {
            m_gTargetObject = CWGlobal.FindObject(m_szTargetObject);
        }
        if (m_gTargetObject == null)
        {
            m_gTargetObject = gameObject;
        }

   


    }
    bool Once()
    {
        if (m_bOnce) return true;
        FindObject();
        if (m_gFollowObject == null) return false;
        m_bOnce = true;
        return m_bOnce;
    }
    bool m_bflag = false;
    void Run()
    {
        m_bOnce = Once();
        if (!m_bOnce) return;
        
        
        if (m_bflag) return;
        Vector3 vEnd = m_gFollowObject.transform.position;
        float fdist = Vector3.Distance(m_gTargetObject.transform.position, vEnd);
        if(fdist<=m_fMindist)
        {
            return;
        }

        //m_gTargetObject.transform.position = Vector3.Lerp(m_gTargetObject.transform.position, vEnd, Time.deltaTime* m_fSpeed);
        m_bflag = true;
        m_gTargetObject.transform.DOMove(vEnd, m_fSec).OnComplete(() => { m_bflag = false; });

        

    }
    private void FixedUpdate()
    {
        Run();
    }


}
