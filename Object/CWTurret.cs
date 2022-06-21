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
using CWUnityLib;
using CWStruct;
using CWEnum;
using DG.Tweening;
/*
 * 터렛의 개념
 * 
 * 무기가 존재하지 않는다 
 * 
 * 
 * */

public class CWTurret : CWAIObject
{


    #region 초기화 

    GameObject m_gShootDummy;// 슈팅 더미 
    GameObject m_gRotationBody;// 회전 몸체

    

    public float m_fSpeed;

    public override void Create(int nID)
    {
        base.Create(nID);
        CWLib.SetGameObjectTag(gameObject, "AI");
        UserType = USERTYPE.TURRET;
        m_gShootDummy = CWLib.FindChild(gameObject, "dummy");
        m_gRotationBody = CWLib.FindChild(gameObject, "Body");
        if (m_gShootDummy == null) m_gShootDummy = gameObject;
        if (m_gRotationBody == null) m_gRotationBody = gameObject;



    }
    protected override void SetObjectType()
    {
        m_ObjectType = CWOBJECTTYPE.TURRET;
    }


    #endregion

    #region AI 세팅
    // 

   

    #endregion
    #region 회전


    public override void RotateWeaponIdle()
    {
        StopCoroutine("RotateIdleRun");
        StartCoroutine("RotateIdleRun");

    }
    IEnumerator RotateIdleRun()
    {
        float fdelta = 0;
        int r = CWLib.Random(0, 4);
        float rr = CWLib.Random(50, 150)/100f;
        float fdir = 0;

        if (r == 0) fdir = rr;
        if (r == 1) fdir = -rr;

        while (true)
        {
            Vector3 v = m_gRotationBody.transform.eulerAngles;
            v.y += (Time.deltaTime * 50f) * fdir;
            fdelta += (Time.deltaTime * 50f);
            m_gRotationBody.transform.eulerAngles = v;
            if (fdelta > 180) break;
            yield return null;
        }

        yield return null;
    }

    public override void DoLookAt(Vector3 vRot, float fTime)
    {
        m_gRotationBody.transform.DOLookAt(vRot, fTime);
    }

    #endregion

    #region  공격

    

    #endregion




    protected override void MakeProgressBar()
    {

    }


}
