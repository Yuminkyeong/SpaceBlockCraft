using UnityEngine;
using System.Collections;

public class CWExplosion : MonoBehaviour {



    public float m_fLifeTime=2;


    public delegate void DgCloseEvent(GameObject gg);

    DgCloseEvent  CloseEventFuc;

    bool m_bflag = false;
    public Vector3 m_vStart;
    public Vector3 m_vStartEuler;
    
    Vector3 m_vEuler = new Vector3();
    

    float m_fStartTime;
    float m_fGravityPrevdist = 0;
    float m_fPreDist = 0;
    float m_fSpeed;
    public float m_fRotateSpeed=100f;
    public float m_fGravitySpeed = 150;
    public float m_fAcceleration=100;
    public float m_fPowerMin=10;
    public float m_fPowerMax=50;
    public float m_fPitchMin=-85;
    public float m_fPitchMax=-40;
    public float m_fPower = 1f;
    public bool m_AutoStart = false;

    public Transform m_FollowTarget;//타겟으로 이동
    public float m_fCheckFollow=1f;// 쫓아갈 시간
    public float m_fFollowSpeed = 20f;
    public float m_fFollowASpeed = 20f;


    CWMove m_kMove;
    public void Close()
    {

      //  Debug.Log("Explosion Close!!!!!!");
        m_bflag = false;

        //transform.localPosition = m_vStart;
        //transform.eulerAngles = m_vStartEuler;

        if (CloseEventFuc != null)
        {
            CloseEventFuc(gameObject);
        }
    }
    public void Begin(float fPower, DgCloseEvent dgfuc = null)
    {
        CloseEventFuc = dgfuc;
        m_bflag = true;
        
        m_fStartTime = Time.time;
        m_fPreDist = 0;
        m_fGravityPrevdist = 0;

        

        m_fSpeed = Random.Range(m_fPowerMin * fPower, m_fPowerMax*fPower);
        float x, y;
        x = Random.Range(m_fPitchMin, m_fPitchMax);
        y = Random.Range(0,360);

        m_vEuler.x = x;
        m_vEuler.y = y;


        transform.eulerAngles = m_vEuler;
        
        m_kMove = null;
        //m_vDir
    }
    Vector3 GetGravity(float fPastTime)
    {
        float fSec = fPastTime;
        float fdist =  fSec + 0.5f * m_fGravitySpeed * fSec * fSec;
        float fDelta = fdist - m_fGravityPrevdist;
        m_fGravityPrevdist = fdist;
        return fDelta * Vector3.down;

    }
    Vector3 GetMoveVector(float fPastTime)
    {
        float fSec = fPastTime;
        float fdist = m_fSpeed * fSec + 0.5f * m_fAcceleration * fSec * fSec;
        float fdistdelta = fdist - m_fPreDist;
        m_fPreDist = fdist;
        return fdistdelta * transform.forward;


    }
    void FollowMove()
    {

        if (m_kMove == null)
        {
            m_kMove = new CWMove(transform, m_FollowTarget, Vector3.up, m_fFollowSpeed, m_fFollowASpeed, 0.1f, 0.1f);
        }
        Vector3 vDir = m_kMove.GetMove();
        if (vDir == Vector3.zero)
        {
            transform.localPosition = new Vector3(0, -100, 0);
            Close();
            
            return;
        }

        m_kMove.m_fSpeed -= Time.deltaTime * 5f;
        if (m_kMove.m_fSpeed < 5f)
        {
            m_kMove.m_fSpeed = 5f;
        }

        transform.localPosition += vDir;

        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.05f,0.05f,0.05f), Time.deltaTime * 3f);

    }
    void Move()
    {

        float fPastTime = Time.time - m_fStartTime;
        if(m_FollowTarget!=null)//쫓아가야 되는 타겟이 있다면
        {
            if (fPastTime > m_fCheckFollow)
            {
                FollowMove();

                return;
            }

        }
        if (fPastTime>m_fLifeTime)
        {
            Close();
            return;
        }
        transform.localPosition += GetMoveVector(fPastTime) + GetGravity(fPastTime);

        m_vEuler.z += Time.deltaTime * m_fRotateSpeed;
        
        transform.eulerAngles = m_vEuler;
        



    }
	void Start () 
    {

        m_vStartEuler = transform.eulerAngles;
        m_vStart = transform.localPosition;

        if (m_AutoStart)
        {
            Begin(m_fPower);
        }
	}
	void Update () 
    {
        if (!m_bflag) return;
        Move();
	}
}
