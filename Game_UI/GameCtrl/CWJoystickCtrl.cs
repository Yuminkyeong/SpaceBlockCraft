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


public class CWJoystickCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //키를 받아 움직이는 개념만 

    public float m_fJoyRadius=100f;
    public GameObject m_gBall;
    public GameObject m_gJoyImage;
    public RectTransform m_gPan;
    public bool m_bFixX;
    public bool m_bFixY;
    public bool m_bMoving;// 클릭한 곳에서 움직인다
    public bool m_bJoyButton = false;


    public delegate void CBEventFunc();

    public string InputNameX;
    public string InputNameY;

    public CBEventFunc CBBeginFunc;
    public CBEventFunc CBEndFunc;


    RectTransform m_tParent;
    RectTransform m_gJoy;
    public float RateX
    {
        get
        {
            return (m_gBall.transform.localPosition.x / m_fJoyRadius);
        }
    }
    public float RateY
    {
        get
        {
            return ( m_gBall.transform.localPosition.y / m_fJoyRadius);
        }
    }
    void Awake()
    {
        m_tParent = transform.parent.GetComponent<RectTransform>();
        m_gJoy = gameObject.GetComponent<RectTransform>();
    }
    void ShowJoyImage()
    {
        m_gJoyImage.SetActive(true);
        

    }

    void Start()
    {
        Game_App.Instance.m_bGamePad = false;
        string[] names = Input.GetJoystickNames();
        foreach(var v in names)
        {
            if(v.Length>0)
            {
                //   print(string.Format("pad=> {0} ",v));
                
                if(v.ToUpper().Contains("CONTROLLER"))
                {
                    Game_App.Instance.m_bGamePad = true;
                }
                
            }
        }

        if(m_bMoving)
        {
            m_gJoyImage.SetActive(false);
        }

    }
    private void OnEnable()
    {

        m_gBall.transform.localPosition = Vector3.zero;
        if (m_bMoving)
        {
            m_gJoyImage.SetActive(false);
        }

    }


    public void Show(Vector2 vPos,Vector2 vdir)
    {
        if(gameObject.activeSelf==false)
        {
            gameObject.SetActive(true);
            m_gJoy.SetParent(MainUI.Instance.m_gTempUI.transform);
            m_gJoy.anchoredPosition = CWMath.ConvertByMousePos(vPos); //CWMath.ConvertByMousePos(Input.mousePosition);
        }
        DirMove(vdir);

    }
    public void Hide()
    {
        JoyStop();
    }

    #region JOYSTICK

    Vector2 m_vJoyStart;

    Vector2 m_vMousePos=new Vector2(100,100);
    public virtual void JoyBegin(Vector2 vPos)
    {
        m_vMousePos = vPos;
        m_vJoyStart = vPos;

   //     Debug.Log(string.Format("JoyBegin mouse {0} -{1}", m_vMousePos,transform.parent.name));
            
        if(CBBeginFunc!=null)
        {
            CBBeginFunc();
        }

        if (m_bMoving)
        {
            ShowJoyImage();
            
            MainUI.Instance.ShowMousePosUI(m_gJoyImage);
            

        }


    }
    public virtual void JoyRun(Vector2 vTouchPos)
    {
        Vector2 vPos = vTouchPos;
        Vector2 vDir = vPos - m_vJoyStart;

       
        vDir.Normalize();
        if(m_bFixX)
        {
            vDir.x = 0;
        }
        if (m_bFixY)
        {
            vDir.y = 0;
        }
        float fdist = Vector2.Distance(vPos, m_vJoyStart);
        if (fdist >= m_fJoyRadius) fdist = m_fJoyRadius;
        Vector2 v = vDir * fdist;
        SetBall(v.x, v.y);
        if (m_bMoving)
        {
            ShowJoyImage();
            m_gJoyImage.GetComponent<RectTransform>().anchoredPosition = CWMath.ConvertByMousePos(m_vMousePos);
            
        }


    }

    public virtual void JoyStop()
    {
        m_gBall.transform.DOLocalMove(Vector3.zero, 0.4f).OnComplete(()=>{
            if(m_bMoving)
            {
                m_gJoyImage.SetActive(false);
                m_gJoy.SetParent(m_tParent);
            }
        });

        if (CBEndFunc != null)
        {
            CBEndFunc();
        }
        


    }
    public void DirMove(Vector2 vDir)
    {
        if (m_bFixX)
        {
            vDir.x = 0;
        }
        if (m_bFixY)
        {
            vDir.y = 0;
        }

        Vector2 v = vDir * m_fJoyRadius;
        SetBall(v.x, v.y);

    }

    void SetBall(float x,float y)
    {
        m_gBall.transform.localPosition = new Vector3(x , y);
    }

    #endregion
   

    #region 이벤트 


    public void OnBeginDrag(PointerEventData data)
    {
        if (Game_App.Instance.g_bDirecting) return;// 연출중
        Vector3 globalMousePos = Vector3.zero;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_gPan, data.position, data.pressEventCamera, out globalMousePos))
        {
            JoyBegin(globalMousePos);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (Game_App.Instance.g_bDirecting) return;// 연출중
        Vector3 globalMousePos=Vector3.zero;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_gPan, data.position, data.pressEventCamera, out globalMousePos))
        {
            JoyRun(globalMousePos);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Game_App.Instance.g_bDirecting) return;// 연출중
        JoyStop();
    }
    #endregion


    void Update()
    {
        if (Game_App.Instance.g_bDirecting) return;// 연출중
        if (m_bJoyButton)
        {

            float v1 = Input.GetAxis(InputNameX);
            float v2 = Input.GetAxis(InputNameY);
            SetBall(v1 * m_fJoyRadius, v2 * m_fJoyRadius);
        }
    }

}
