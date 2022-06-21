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


public class ActiveUI : MonoBehaviour
{

    public UnityEvent ClickFunction;

    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;
        int tcnt = ClickFunction.GetPersistentEventCount();
        
        if (tcnt==0)
        {
            StartCoroutine("IRun");
        }
        else
        {
            
            ClickFunction.Invoke();
        }
        
    }
    IEnumerator IRun()
    {
        yield return null;
        BaseUI baseui = gameObject.GetComponentInChildren<BaseUI>();
        if (baseui)
        {
            baseui.Open();
        }

    }
    private void OnDisable()
    {
        gameObject.SetActive(false);
        BaseUI baseui = gameObject.GetComponentInChildren<BaseUI>();
        if (baseui)
        {
            baseui.Close();
        }

    }
}
