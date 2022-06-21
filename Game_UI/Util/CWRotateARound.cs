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


public class CWRotateARound : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    #region 이벤트 함수
    float m_fStartTime;
    public UnityEvent ClickFunction;
    public void OnPointerDown(PointerEventData data)
    {
        m_fStartTime = Time.time;
        m_vPos = ConvertPos(data);
    }
    public void OnPointerUp(PointerEventData data)
    {
        if (Time.time - m_fStartTime < 0.2f)
        {
            ClickFunction.Invoke();
        }
            
    }
    public void OnBeginDrag(PointerEventData data)
    {

    }
    public void OnDrag(PointerEventData data)
    {
        Vector2 vPos = ConvertPos(data);
        
        Vector2 vDir = m_vPos - vPos;
        float fdist = Vector3.Distance(m_vPos, vPos);

        vDir.Normalize();
        DragFunction(vDir, fdist);
        m_vPos = vPos;
    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }
    private void Update()
    {
        UpdateData();
    }
    void Start()
    {

        if(m_gActor==null)
        {
            m_gActor = CWGlobal.FindObject(m_gActorString).transform;
        }

        if (m_gTarget==null)
        {
            m_gTarget = CWGlobal.FindObject(m_gTargetString).transform;
        }

        Create();
    }

    #endregion
    #region 구현함수

    public float m_fDistMax = 100;

    Vector2 ConvertPos(PointerEventData data)
    {
        Vector2 vPos = Vector2.zero;

        RectTransform Panel = transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Panel, data.position, data.pressEventCamera, out vPos))
        {

            vPos += Panel.anchoredPosition;
            return vPos;
        }
        return Vector2.zero;
    }

    void DragFunction(Vector2 vDelta,float fdist)
    {

        if (Input.touchSupported)
        {
            if (Input.touchCount == 2) return;
        }


                //float fRate = 1 + (fdist / m_fDistMax);
       float fRate = (fdist / m_fDistMax)*2f;


        m_fYaw -= vDelta.x* m_fSpeed* fRate * Time.deltaTime;
        m_fPitch -= vDelta.y * m_fSpeed * fRate * Time.deltaTime;
        if (m_fPitch >= 89) m_fPitch = 89;
        if (m_fPitch <= -89) m_fPitch = -89;


    }
    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        m_fDist += deltaMagnitudeDiff * speed;
        if (m_fDist > m_fMaxDist) m_fDist = m_fMaxDist;
        if (m_fDist < m_fMinDist) m_fDist = m_fMinDist;
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
    public void Create()
    {
        
        m_fDist = Vector3.Distance(m_gActor.transform.position, m_gTarget.position);

    }
    void UpdateData()
    {
        TouchZoom();
        Quaternion qq = Quaternion.Euler(new Vector3(m_fPitch, m_fYaw, 0));
        m_vDir = qq * Vector3.forward;
        m_gActor.transform.position = m_gTarget.position + (m_vDir) * m_fDist;
        m_gActor.transform.LookAt(m_gTarget.transform);
    }
    #endregion
    #region 변수들
    public Transform m_gTarget;
    public string m_gTargetString;

    public Transform m_gActor;
    public string m_gActorString;



    public float m_fSpeed = 10f;

    public float m_fYaw = 0;
    public float m_fPitch = 0;
    public float m_fDist = 0;
    
    public float TouchZoomSpeed = 0.07f;
    public float MouseZoomSpeed = 5.0f;

    public float m_fMaxDist = 23;
    public float m_fMinDist = 1;

    Vector2 m_vPos;
    Vector3 m_vDir;



    #endregion

}
