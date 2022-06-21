using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

/*
 * 비행기 움직임
 * 
 * */
public class AircraftAction : ObejctAction
{
    CWJoystickCtrl m_kJoy;
    bool m_bStartFlag = false;
    public float m_fSpeed=10f;
    float GetHeroNowSpeed()
    {
        //return m_fSpeed * CWHero.Instance.NSpeedCount;
        return m_fSpeed * 2;
    }
    protected override void MovePosition(Vector3 vPos)
    {
       // if (m_bStop) return;
        if(!CheckMove(vPos))
        {
            return;
        }

        HeightAction hh = GetComponent<HeightAction>();
        if (hh == null) return;

        if (hh.GetDir() != 0)
        {
            if (GetDir() == Vector3.zero)
            {
                GetRigidbody().velocity = Vector3.zero;
                return;
            }
        }

        GetRigidbody().MovePosition(vPos);

    }
    Rigidbody m_Rigidbody = null;
    public Rigidbody GetRigidbody()
    {
        if (m_Rigidbody == null)
        {
            m_Rigidbody = gameObject.GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
            {
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

                m_Rigidbody.freezeRotation = true;
              //  m_Rigidbody.drag = 0.6f;
              //  m_Rigidbody.angularDrag = 0.8f;
              //  m_Rigidbody.mass = 10000;
            }
        }
        return m_Rigidbody;
    }
    private void Start()
    {
        m_kJoy = GamePlay.Instance.GetAircraftJoy();
        m_kJoy.CBBeginFunc = StartEvent;
        m_kJoy.CBEndFunc = StopEvent;

#if UNITY_EDITOR || UNITY_STANDALONE
        m_kJoy.m_bJoyButton = true;
#else 
          m_kJoy.m_bJoyButton = false;
#endif

    }

    public override bool CheckMove(Vector3 vPos)
    {
        if (vPos.y < 10) return false;
        return base.CheckMove(vPos);
    }
    public override Vector3 GetDir()
    {
        if (m_kJoy.RateX == 0 && m_kJoy.RateY == 0)
        {
            return Vector3.zero;
        }

        HeightAction hh = GetComponent<HeightAction>();
        if (hh == null) return Vector3.zero;

        if (hh.GetDir() == 0)
        {
            Transform m_tCamera = CWHero.Instance.transform;
            float fyaw = CWMath.CalRadian_Yaw(new Vector3(m_kJoy.RateX, 0, m_kJoy.RateY));
            return CWMath.CalYaw(fyaw,  m_tCamera.forward);
        }
        else
        {
            Transform m_tCamera = CWHero.Instance.transform;
            float fyaw = CWMath.CalRadian_Yaw(new Vector3(m_kJoy.RateX, 0, m_kJoy.RateY));
            Vector3 vdir= CWMath.CalYaw(fyaw, m_tCamera.forward);
            vdir.y = hh.GetDir();
            return vdir;

        }

    }
    public override float GetSpeed()
    {

        float fmax = 1f;
        if (m_kJoy.RateY==0)
        {
            fmax =  Mathf.Abs(m_kJoy.RateX * 2);//Mathf.Max(Mathf.Abs(m_kJoy.RateY) , Mathf.Abs(m_kJoy.RateX));
        }

        return GetHeroNowSpeed() * fmax;
    }
    protected override void Move()
    {
        if (GamePlay.Instance.CharMode) return;

        if (m_bStartFlag)
        {
            RollRoate(m_kJoy.RateX);
        }

        base.Move();
    }
    #region  회전

    bool m_bRotateflag = false;
    bool m_bRotateStopflag = false;
    bool m_bDir = false;

    void RollRoate(float ndir)
    {
        bool bdir = false;
        if (ndir > 0) bdir = true;
        if (bdir != m_bDir)
        {
            m_bDir = bdir;
            //          RollRoateStop();
            //            return;
        }
        m_bDir = bdir;
        m_bRotateStopflag = false;

        CWDebugManager.Instance.Print(string.Format("좌표! {0}",ndir));

        {
            m_bRotateflag = true;
            float fval = 20;
            if(bdir)
            {
                fval = -20;
            }

            Transform tForm = CWHero.Instance.m_gCenterObject.transform;
            tForm.DOLocalRotate(new Vector3(0, 0, fval), 1.1f).OnComplete(() =>
            {
                m_bRotateflag = false;
            });
        }
    }
    void StartEvent()
    {
        m_bStartFlag = true;
    }
    void StopEvent()
    {
        m_bStartFlag = false;
        RollRoateStop();
    }

    void RollRoateStop()
    {
        if (m_bRotateStopflag) return;
        m_bRotateStopflag = true;
        m_bRotateflag = false;
        Transform tForm = CWHero.Instance.m_gCenterObject.transform;
        tForm.DOKill();
        tForm.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).OnComplete(() =>
        {
            m_bRotateStopflag = false;
        });
    }
    private void OnEnable()
    {
        m_bStartFlag = false;
        if (CWHero.Instance == null) return;

        Transform tForm = CWHero.Instance.m_gCenterObject.transform;
        tForm.localRotation = new Quaternion();

    }

    public void ResetRotation() //회전값 초기화 해주기. 
    {
        m_bRotateStopflag = true;
        m_bRotateflag = false;
        Transform tForm = CWHero.Instance.m_gCenterObject.transform;
        tForm.DOKill();
        tForm.DOLocalRotate(new Vector3(0, 0, 0), 1.0f).OnComplete(() =>
        {
            m_bRotateStopflag = false;
        });
    }
    #endregion
}
