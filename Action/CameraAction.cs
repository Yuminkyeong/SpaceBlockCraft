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

public class CameraAction : MonoBehaviour
{

    
    public float m_fDist;
    public float m_fYaw;
    public float m_fFitch;


    float m_fAxisX = 0;
    float m_fAxisY = 0;

    public float rotSpeed = 20;


    public Vector3 m_vLocalCam;
    public Vector3 m_vLocalCamByChar;// 캐릭터 전용 보기 

    public Vector3 m_vLocalCamByChar_3;// 3인칭 모드 시점

    public string m_szCamera;
    Transform m_tCamera;
    CWJoystickCtrl m_kJoy;
    // Start is called before the first frame update
    void Start()
    {

        m_tCamera = CWGlobal.FindObject(m_szCamera).transform;
        m_kJoy = GetComponentInChildren<CWJoystickCtrl>();
    }
   

    // 대상이 바뀜
    Vector3 GetHeroPos()
    {
        if(GamePlay.Instance.CharMode)
        {
            return CWChHero.Instance.GetPosition(); 
        }
        return CWHero.Instance.GetPosition();
    }
    Transform GetHeroTransform()
    {
        if (GamePlay.Instance.CharMode)
        {
            return CWChHero.Instance.transform;
        }
        return CWHero.Instance.transform;

    }
    Vector3 GetLocalCam()
    {
        if(GamePlay.Instance.CharMode)
        {
            if(CWGlobal.g_bCamMode)
            {
                return m_vLocalCamByChar;
            }
            else
            {
                return m_vLocalCamByChar_3;
            }
            
        }
        return m_vLocalCam;
    }
    void CameraMove()
    {

        Vector3 vdir = CWMath.CalAngle(m_fYaw, m_fFitch, Vector3.forward);
        Vector3 vStart = GetHeroPos();//CWHero.Instance.transform.position;
        Vector3 vPos = vStart + vdir * m_fDist;
        m_tCamera.position = vPos;
        m_tCamera.forward = -vdir;

        Camera.main.transform.localPosition = GetLocalCam();
        Camera.main.transform.localRotation = new Quaternion();

    }
    void RotateMove()
    {

        m_fAxisX = m_kJoy.RateX * Time.deltaTime * rotSpeed;
        m_fAxisY = m_kJoy.RateY * Time.deltaTime * rotSpeed;

        m_fYaw += m_fAxisX;
        m_fFitch += m_fAxisY;

        if (m_fFitch > 50) m_fFitch = 50;
        if (m_fFitch < -70) m_fFitch = -70;


    }
    void FixRotate()
    {
        //GetHeroTransform().DORotate(m_tCamera.eulerAngles, 0.5f);

        Vector3 ee = GetHeroTransform().eulerAngles;
        ee.y = m_tCamera.eulerAngles.y;
        GetHeroTransform().DORotate(ee, 0.5f);

    }
    private void LateUpdate()
    {
        if (!GamePlay.Instance) return;
        if (!GamePlay.Instance.IsGamePlay()) return;
        if (Game_App.Instance.m_bDontFollowCamera) return;// 카메라 따라가기 금지 
        

        RotateMove();
        CameraMove();
        FixRotate();
       
    }

}
