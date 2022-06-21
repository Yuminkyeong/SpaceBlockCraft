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
/*
 * 카메라 컨트롤 개념
 * 
 * 개념
 * 
 * 좌우 스크롤을 감지하면, 
 * 첫번째 감지한 방향으로만 움직이고
 * 그것에 대한 이미지를 출력한다 
 * 
 * 조이스틱은 이미지만 연출하고, 이벤트는 삭제해야 한다
 * */


public class CameraPan : CWJoystickCtrl, IPointerDownHandler, IPointerUpHandler
{


    public void OnPointerDown(PointerEventData data)
    {

        GamePlay.Instance.OnPointerDown(data.position);

        //GamePlay.Instance.OnDiggStart();

        //StartCoroutine("IRun");
    }
    public void OnPointerUp(PointerEventData data)
    {
        //StopAllCoroutines();
        //GamePlay.Instance.OnDiggEnd();
        GamePlay.Instance.OnPointerUp(data.position);
    }



}
