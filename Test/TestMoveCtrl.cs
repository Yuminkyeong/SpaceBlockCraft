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

public class TestMoveCtrl : MonoBehaviour, IDragHandler
{

    public Text m_kText;

    public float rotSpeed = 20;
    public float MinDelta = 70;
    public float MinDeltaDown = 30;

    //public PositionByDist m_gTarget;
    
    public Transform m_tCapsule;
    public Transform m_tCamera;

    public float m_fMaxSpeed = 20;
    float m_fSpeed = 20;

    
    #region 카메라 움직임
    public float m_fDist;
    public float m_fYaw;
    public float m_fFitch;

    // 캐릭터의 회전을 맞춘다
    bool m_bRotate = false;
    void FixRotate()
    {
        if (m_bRotate) return;
        m_bRotate = true;
        //m_tCapsule.DORotateQuaternion(m_tCamera.rotation,0.5f);
        //m_tCapsule.forward = m_tCamera.forward;

        m_tCapsule.DORotate(m_tCamera.eulerAngles, 0.3f).OnComplete(()=> {
            m_bRotate = false;
        });

    }

    void CameraMove()
    {

        Vector3 vdir= CWMath.CalAngle(m_fYaw, m_fFitch, Vector3.forward);
        Vector3 vStart = m_tCapsule.position;
        Vector3 vPos = vStart + vdir * m_fDist;
        m_tCamera.position = vPos;
        m_tCamera.forward = -vdir;


    }

    #endregion

    public void OnDrag(PointerEventData data)
    {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;


        m_fYaw +=rotX ;
        m_fFitch +=rotY ;

        if (m_fFitch > 50) m_fFitch = 50;
        if (m_fFitch < -70) m_fFitch = -70;

        m_kText.text = string.Format("{0} {1}",m_fYaw,m_fFitch);
    }
    void Start()
    {
        m_fSpeed = 0;
        
     
    }
    void Move()
    {
        if (m_tCapsule == null) return;
        
        Vector3 vPos = m_tCapsule.transform.position;
        vPos += m_tCamera.transform.forward * m_fSpeed * Time.deltaTime;
        m_tCapsule.transform.position = vPos;

    }
    
    void ForwardMove()
    {
        m_fSpeed = m_fMaxSpeed;

    }
    void BackMove()
    {
        m_fSpeed = -m_fMaxSpeed;
    }
    void Stop()
    {
        m_fSpeed = 0;
    }

    private void LateUpdate()
    {
        if (!m_tCamera) return;
        if (!m_tCapsule) return;

        Move();
        CameraMove();
        FixRotate();


    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            ForwardMove();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            Stop();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            BackMove();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Stop();
        }

    }
}
