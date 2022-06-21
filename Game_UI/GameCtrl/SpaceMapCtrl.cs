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

public class SpaceMapCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{

    public float TouchZoomSpeed = 0.1f;
    public float MouseZoomSpeed = 5.0f;

    public float m_fSpeed = 10f;
    Vector2 m_vCamStart;

    RectTransform m_gPan;

    void Awake()
    {
        m_gPan = GetComponent<RectTransform>();
    }

    void CamBegin(Vector2 vPos)
    {

        m_vCamStart = vPos;
        
    }
    void CamRun(Vector2 vTouchPos)
    {
        Vector2 vPos = vTouchPos;
        Vector2 vDir = vPos - m_vCamStart;
        vDir.Normalize();
        Vector3 vv = -Camera.main.transform.right;
        Vector3 v2 = Camera.main.transform.forward;
        Camera.main.transform.position += vv * Time.deltaTime * m_fSpeed* vDir.x;
        Camera.main.transform.position += v2 * Time.deltaTime * m_fSpeed * -vDir.y;
        


    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos = Vector3.zero;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_gPan, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            CamBegin(globalMousePos);
        }

    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos = Vector3.zero;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_gPan, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            CamRun(globalMousePos);
        }

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Space_Map.Instance.CheckPlanet();

    }
    public void OnPointerDown(PointerEventData data)
    {
    }
    public void OnPointerUp(PointerEventData data)
    {

     
    }
    void TouchZoom()
    {

        if (Input.touchSupported)
        {
            if (Input.touchCount == 2)
            {

                // get current touch positions
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);

                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Zoom(deltaDistance, TouchZoomSpeed);

            }

        }
        else
        {

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Zoom(-scroll, MouseZoomSpeed);

        }

    }
    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        if (deltaMagnitudeDiff == 0) return;
        Camera cam = Camera.main;
        cam.fieldOfView += deltaMagnitudeDiff * speed;
        //// set min and max value of Clamp function upon your requirement
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 24, 80);

    }
    void Update()
    {

      //  TouchZoom();
    }

    

}
