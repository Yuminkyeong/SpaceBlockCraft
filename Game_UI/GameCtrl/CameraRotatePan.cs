using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
using UnityEngine.Events;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

/* 개요
 * 
 * 좌우로 움직이는 카메라 조이스틱
 * 
 * 높이값 움직임이 감지가 되면 높이값 조이스틱이 출력이 된다 
 * */

public class CameraRotatePan : CWJoystickCtrl
{
    public CWJoystickCtrl m_kHeight;

    bool m_bHeightStart = false;
    Vector2 m_vPrev= Vector2.zero;
    public override void JoyBegin(Vector2 vPos)
    {
        m_vPrev = vPos;
        base.JoyBegin(vPos);
    }
    public override void JoyRun(Vector2 vTouchPos)
    {
        if (m_bHeightStart)
        {
            m_kHeight.JoyRun(vTouchPos);
            return;
        }
            
        // 높이값이 감지가 되었는가?
        // 높이값 감지 
        // 가로보다 세로가 길다 
        // 세로 길이가 일정길이보다 길다

        float fx = Mathf.Abs(m_vPrev.x - vTouchPos.x);
        float fy = Mathf.Abs(m_vPrev.y - vTouchPos.y);

        if(fx<fy)
        {
            if(fy>2f)
            {
                m_kHeight.gameObject.SetActive(true);
                m_kHeight.JoyBegin(vTouchPos);
                m_bHeightStart = true;
            }
        }
        base.JoyRun(vTouchPos);
    }
    public override void JoyStop()
    {
        m_kHeight.gameObject.SetActive(false);
        m_kHeight.JoyStop();
        m_bHeightStart = false;
        base.JoyStop();
    }
}
