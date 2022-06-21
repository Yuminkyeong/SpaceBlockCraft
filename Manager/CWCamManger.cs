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


public class CWCamManger : CWSingleton<CWCamManger>
{
    /*
     * 카메라 연출 
     * 1. 히로를 쫓아간다.
     * 2. 카메라 연출 모드 패스를 쫓아간다 
     * */
     // 연출 모드 
    public enum TYPE {FollowHero,Effectmode };
    TYPE m_Type = TYPE.FollowHero;
    public Camera m_gMain;

    public float m_followSpeed=10f;
    public float m_followRotate = 1f;
    public float m_fHeight=4f; // 주인공보다 높이 값
    public float m_fmindist=3f;// 최소거리 

    public float m_fDist=10f;// 주인공과 거리 
	// Use this for initialization
    void SetType(TYPE ntype)
    {
        m_Type = ntype;
    }

	void Start () {

        DontDestroyOnLoad(m_gMain);
    }
    void Follow()
    {

        CWObject  kHero = CWHeroManager.Instance.GetHero();
        GameObject gHero = kHero.gameObject;

        Vector3 vMin =kHero.GetPosition() - gHero.transform.forward * m_fmindist;
        vMin.y = m_fHeight;
        Vector3 vDir = kHero.GetPosition() - vMin;
        vDir.Normalize();

        Vector3 vTarget =vMin + vDir* m_fDist; //

        m_gMain.transform.position = Vector3.Lerp(m_gMain.transform.position, vTarget, Time.deltaTime * m_followSpeed);

        Vector3 vLookdir = gHero.transform.position - m_gMain.transform.position;
        Quaternion qRot = Quaternion.LookRotation(vLookdir);
        m_gMain.transform.rotation = Quaternion.Lerp(m_gMain.transform.rotation, qRot, Time.deltaTime * m_followRotate);

    }
    // 
    private void Update()
    {
        if(m_Type==TYPE.FollowHero)// 주인공을 따라가는 모드 
        {

            Follow();
        }
        else
        {

        }
    }

    public Camera GetObject()
    {
        return m_gMain;
    }
	

}
