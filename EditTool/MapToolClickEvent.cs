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

public class MapToolClickEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public UnityEvent PressFunction;
    public UnityEvent RelaseFunction;
    public void OnPointerDown(PointerEventData data)
    {
        if(PressFunction!=null)
            PressFunction.Invoke();
    }
    public void OnPointerUp(PointerEventData data)
    {
        if (RelaseFunction != null)
            RelaseFunction.Invoke();

    }

}
