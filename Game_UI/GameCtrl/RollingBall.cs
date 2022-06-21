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
public class RollingBall : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Transform m_tTarget;
    public float rotSpeed = 20;
    public float MinDelta = 70;
    public float MinDeltaDown = 30;

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData data)
    {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        m_tTarget.transform.Rotate(Vector3.up, -rotX);
        m_tTarget.transform.Rotate(Vector3.right, rotY);
        Vector3 vRot = m_tTarget.transform.eulerAngles;
        vRot.z = 0;

        float fx = vRot.x - 360;

        if (vRot.x > 0 && vRot.x < 180)
        {
            if (vRot.x >= MinDeltaDown)//0~32
            {
                vRot.x = MinDeltaDown;
            }

        }
        float ff = (vRot.x + 360) % 360;
        if (ff > 180 && ff < 360)
        {
            if (ff < 360 - MinDelta)
            {
                vRot.x = 360 - MinDelta;
            }

        }


        m_tTarget.transform.eulerAngles = vRot;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
    }

}
