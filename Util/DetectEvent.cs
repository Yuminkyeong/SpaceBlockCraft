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


public class DetectEvent : MonoBehaviour
{

    public UnityEvent TriggerEnter;
    public UnityEvent TriggerStay;
    public UnityEvent TriggerExit;

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.layer==LayerMask.NameToLayer("Shooting"))
        {
            // 슈팅 
            return;
        }
        TriggerEnter.Invoke();

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shooting"))
        {
            // 슈팅 
            return;
        }

        TriggerStay.Invoke();

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shooting"))
        {
            // 슈팅 
            return;
        }

        TriggerExit.Invoke();

    }
}
