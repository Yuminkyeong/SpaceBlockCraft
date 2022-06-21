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
using CWEnum;
public class CharicAction : MonoBehaviour
{

    public CapsuleCollider m_kCollier;
    public PhysicMaterial  m_Material;

    CWJoystickCtrl m_kJoy;
    public float m_fSpeed = 10f;
    public GameObject m_dummy;

    public float m_fJumpSpeed = 10f;
    public float m_fJumpdist = 1.2f;
    CameraAction m_CamAction;

    const float COOLTIME = 0.5f;
    float m_fCooltime;


    
    Vector3 m_Input = Vector2.zero;

    bool m_bStop;

    public void Stop()
    {
        m_bStop=true;
    }
    public void Play()
    {
        m_bStop = false;
    }

    public bool IsMove()
    {
        if (m_kJoy.RateX == 0 && m_kJoy.RateY == 0)
        {
            return false;
        }
        return true;
    }
    // 내행성에서 점프를 누르면 날개모드로 바뀜
    public bool m_bFlyMode = false;


    private void Start()
    {
        m_kJoy = GamePlay.Instance.GetCharJoy();

        m_CamAction = GamePlay.Instance.m_CharCamAction;
        if(m_dummy==null)
        {
            m_dummy = gameObject;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        m_kJoy.m_bJoyButton = true;
#else 
          m_kJoy.m_bJoyButton = false;
#endif

    }



    private void Update()
    {


        m_Input = new Vector3(m_kJoy.RateX, 0,m_kJoy.RateY);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

    }
    public virtual bool CheckMove(Vector3 vPos)
    {
        if (vPos.x < 0) return false;
        if (vPos.x > 256) return false;
        if (vPos.z < 0) return false;
        if (vPos.z > 256) return false;
        if (vPos.y > 255) return false;
        //        if (vPos.y < 10) return false;


        return true;
    }


    private void FixedUpdate()
    {
        // 플레이중이 아니면 리턴
        if (!GamePlay.Instance) return;
        if (!GamePlay.Instance.IsGamePlay()) return;
        if (!GamePlay.Instance.CharMode) return;


        if (m_bStop) return;
        if (m_bStay)
        {
            // 현재 걸려 있는 상태
            // 걸리지 않은 값으로 천천히 돌아간다
            //m_vSavePos
           // transform.position = Vector3.Lerp(transform.position, m_vSavePos,Time.deltaTime);
            //== 일정거리이상 벌어지면, 원래 대로 돌아간다
        }
        Vector3 vEuler = CWChHero.Instance.m_CameraBody.transform.eulerAngles;
        Vector3 vdir = CWMath.CalYaw(vEuler.y, m_Input);

        if(m_bFlyMode)
        {
            Quaternion qq = CWChHero.Instance.m_CameraBody.transform.rotation;
            vdir = qq * m_Input;
        }


        Vector3 vPos = transform.position + vdir * Time.deltaTime * m_fSpeed;
        CWChHero.Instance.GetRigidbody().MovePosition(transform.position + vdir * Time.deltaTime * m_fSpeed);
        //if (CheckMove(vPos))// 문제가 없는가?
        //{
            
        //}
        //else
        //{
        //    CWMapManager.Instance.SetBound();
        //}


        if(m_Input!=Vector3.zero)// 전진
        {
            m_CamAction.m_fFitch = Mathf.Lerp( m_CamAction.m_fFitch,0, Time.deltaTime);

        }
    }
    

    
    
    bool _bDetected = false;
    public bool m_bDetected
    {
        get
        {
            return _bDetected;

        }
        set
        {
            _bDetected = value;
            if(_bDetected==false)
            {
                
            }
        }
    }

    Vector3 m_vSavePos = Vector3.zero;
    bool _bStay;
    public bool m_bStay
    {
        get
        {
            return _bStay;
        }
        set
        {
            _bStay = value;
            if(!value)
            {
                m_vSavePos = transform.position;
            }
        }
    }

    public void OnJump()
    {
        Jump();
    }
   
    // 현재 점프 중인가?
    public bool IsJumping()
    {
        float ft = Time.time - m_fCooltime;
        if (ft < COOLTIME)
        {
            return true;
        }
        m_fCooltime = Time.time;
        return false;
    }
    void Jump()
    {

        if(IsJumping())
        {
            return;
        }
/*
        if (Space_Map.Instance.IsMyPlanet())
        {
            m_bFlyMode = !m_bFlyMode;
            if(m_bFlyMode)
            {
                
                Vector3 vpos = transform.position;
                vpos.y += 2f;
                transform.DOMove(vpos, 0.5f);
                
                CWChHero.Instance.GetRigidbody().useGravity = false;
            }
            else
            {
                
                CWChHero.Instance.GetRigidbody().useGravity = true;
            }
            return;
        }
*/
        if(CWChHero.Instance.m_GroundState == DETECTTYPE.EXIT)
        {
            return;
        }

        Debug.Log("점프 한다!");
        Invoke("ResetJump", 0.7f);

        m_kCollier.material = m_Material;
        CWChHero.Instance.GetRigidbody().AddForce(transform.up * m_fJumpSpeed, ForceMode.Impulse);
    }
  
    void ResetJump()
    {
        m_kCollier.material = null;
    }


}
