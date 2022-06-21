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


public class CharicCamera : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    #region 변수들
    public Transform m_gCharic;
    public Transform m_gTarget;
    public Transform m_gCamera;

    public CWJoystickCtrl m_kHeight;
    bool m_bHeightStart = false;

    public float m_fSpeed = 4f;

    public float m_fYaw = 0;
    float m_fPitch = 30;
    float m_fPitchMin = -85;
    float m_fPitchMax = 85;

    public float m_fDist = 0;

    

    Vector2 m_vPos;
    Vector3 m_vDir;

    #endregion
    #region 이벤트 함수
    float m_fStartTime;

    public void OnPointerDown(PointerEventData data)
    {
        m_fStartTime = Time.time;
        m_vPos = ConvertPos(data);
        GamePlay.Instance.OnPointerDown(data.position);

        //Game_App.Instance.m_bDontFollowCamera = false;
    }
    public void OnPointerUp(PointerEventData data)
    {
        GamePlay.Instance.OnPointerUp(data.position);

    }
    public void OnBeginDrag(PointerEventData data)
    {
        

    }
    public void OnDrag(PointerEventData data)
    {

        if (Input.touchSupported)
        {
            if (Input.touchCount > 1)
            {
                if(!CWChHero.Instance.IsMove())
                {
                    Debug.Log("투 터치 체크 ");
                    return;

                }
            }
                
        }


        Vector2 vPos = ConvertPos(data);
        if (m_bHeightStart)
        {
            m_kHeight.JoyRun(vPos);
            return;
        }
        Vector2 vDir = m_vPos - vPos;
        /*
                // 내 행성에서만 적용 

                if( Mathf.Abs(vDir.x) < Mathf.Abs(vDir.y))
                {
                    if(Mathf.Abs(vDir.y)>2f)
                    {
                        m_kHeight.gameObject.SetActive(true);
                        m_kHeight.JoyBegin(vPos);
                        m_bHeightStart = true;
                    }
                }

            */
        float fdist = Vector3.Distance(m_vPos, vPos);
        vDir.Normalize();
        DragFunction(vDir, fdist);
        m_vPos = vPos;
    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }
    void FixRotate()
    {
        if (m_gCamera == null) return;
        if (m_gCharic == null) return;
        m_gCharic.DORotate(m_gCamera.eulerAngles, 0.05f);
    }
    private void Update()
    {
        if (!GamePlay.Instance) return;
        if (!GamePlay.Instance.IsGamePlay()) return;
        if (Game_App.Instance.m_bDontFollowCamera) return;// 카메라 따라가기 금지 
        FixRotate();
        UpdateData();
    }
    void Start()
    {
        m_gCharic = CWLib.FindChild(CWChHero.Instance.gameObject, "CamBody").transform;
        m_gTarget = CWLib.FindChild(CWChHero.Instance.gameObject, "CameraTarget").transform;
        m_gCamera = Camera.main.transform;
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

    void DragFunction(Vector2 vDelta, float fdist)
    {

       


        //float fRate = 1 + (fdist / m_fDistMax);
        float fRate = (fdist / m_fDistMax) * 2f;


        m_fYaw -= vDelta.x * m_fSpeed * fRate * Time.deltaTime;
        m_fPitch -= vDelta.y * m_fSpeed * fRate * Time.deltaTime;
        if (m_fPitch >= m_fPitchMax) m_fPitch = m_fPitchMax;
        if (m_fPitch <= m_fPitchMin) m_fPitch = m_fPitchMin;


    }
    
    public void Create()
    {

//        m_fDist = Vector3.Distance(m_gCamera.transform.position, m_gTarget.position);

    }
    void UpdateData()
    {
        
        Quaternion qq = Quaternion.Euler(new Vector3(m_fPitch, m_fYaw, 0));
        m_vDir = qq * Vector3.forward;
        m_gCamera.transform.position = m_gTarget.position + (m_vDir) * m_fDist;
        m_gCamera.transform.LookAt(m_gTarget.transform);
    }
    public void FitchPlay(float fdelay)
    {
        StartCoroutine(IFitch(fdelay));
    }
    IEnumerator IFitch(float fdelay)
    {
        m_fPitch = -10f;
        float tt = Time.time;
        while(true)
        {
            if(Time.time - tt> fdelay)
            {
                break;
            }
            m_fPitch -= Time.deltaTime * 30f;
            if (m_fPitch < -60) break;
            yield return null;
        }
    }
    

    #endregion


}
