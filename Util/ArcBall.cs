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


public class ArcBall : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform m_tTarget;
    Quaternion m_qDown = new Quaternion();
    Quaternion m_qNow = new Quaternion();
    Vector3 m_vDownPt;           // starting point of rotation arc
    Vector3 m_vCurrentPt;        // current point of rotation arc
    float m_fRadius=1f;
    void Start()
    {
        m_qDown = m_tTarget.rotation;
    }
    void Update()
    {
        
    }

    Vector3 ScreentoVector(float px,float py)
    {
        float x = -(px - Screen.width / 2) / (m_fRadius*Screen.width/2);
        float y = -(py - Screen.height / 2) / (m_fRadius*Screen.height / 2);

        float z = 0f;
        float mag = x * x + y * y;

        if(mag>1.0f)
        {
            float scale = 1 / Mathf.Sqrt(mag);
            x*= scale;
            y*= scale;
        }
        else
        {
            z = Mathf.Sqrt(1 - mag);
        }
        return new Vector3(x, y, z);

    }
    Quaternion QuatFromBallPoints(Vector3 vFrom, Vector3 vTo)
    {
        Vector3 vPart;
        float fDot = Vector3.Dot(vFrom, vTo);//  D3DXVec3Dot(&vFrom, &vTo);
        //D3DXVec3Cross( &vPart, &vFrom, &vTo );
        vPart = Vector3.Cross(vFrom, vTo);
        return new Quaternion(vPart.x, vPart.y, vPart.z, fDot);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        //Vector3 globalMousePos = Vector3.zero;
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out globalMousePos))
        //{
        //    //JoyBegin(globalMousePos);
        //    m_vDownPt = globalMousePos;
        //    m_qDown = m_tTarget.rotation;
        //}

        m_qDown = m_tTarget.rotation;
        m_vDownPt = ScreentoVector(Input.mousePosition.x, Input.mousePosition.y);


    }
    public void OnDrag(PointerEventData eventData)
    {

        //Vector3 globalMousePos = Vector3.zero;
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out globalMousePos))
        //{
        //    m_tTarget.rotation = m_qDown * QuatFromBallPoints(m_vDownPt, globalMousePos);
        //}
        m_vCurrentPt = ScreentoVector(Input.mousePosition.x, Input.mousePosition.y);
        m_tTarget.rotation = m_qDown * QuatFromBallPoints(m_vDownPt, m_vCurrentPt);
        

        //m_vCurrentPt.x = eventData.position.x;
        //m_vCurrentPt.z = eventData.position.y;
        //m_tTarget.rotation = m_qDown * QuatFromBallPoints(m_vDownPt, m_vCurrentPt);
    }
    public void  OnEndDrag(PointerEventData eventData)
    {

    }
}
