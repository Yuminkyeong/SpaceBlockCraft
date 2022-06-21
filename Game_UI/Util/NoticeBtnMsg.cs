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


public class NoticeBtnMsg : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public string Msg;
    public string Msg2;

    public void OnPointerDown(PointerEventData data)
    {

    }
    public void OnPointerUp(PointerEventData data)
    {
        NoticeMessage2.Instance.Show(Msg, Msg2);
    }
}
