using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CWEnum;
using CWUnityLib;
using DG.Tweening;



public class CWPVPAI : MonoBehaviour
{
    GameObject m_gChild;// 패턴으로 움직인다
    public CWAirObject m_gTarget;
    public CWAirObject m_gActor;

    bool m_bDrone=false;

    public float m_fSpeed=10f;
    // 타겟과 거리 두기


    

    bool DistMove()
    {
        if (m_gTarget == null) return false;

        ///////////////

        if (m_bAvoidFlag) return false;
        //////////////

        Vector3 my = transform.position;
        Vector3 target=m_gTarget.transform.position;
        Vector3 vdir = target-my;
        vdir.Normalize();
        float fDist = Vector3.Distance(my, target);
        if (fDist < m_fRange)
        {
            // 너무 가깝다면?
            // 나보다 강한 적이 가깝게 왔다면?
            if(fDist < 20)
            {
                if(m_nDiff>=3)
                {
                    my -= vdir * m_fSpeed * Time.deltaTime;
                    transform.position = my;
                    return true;
                }

            }

        }
        //my += vdir * m_fSpeed * Time.deltaTime;

        ////if(my.x<=30&&my.x>220 && my.z <= 30 && my.z > 220)
        ////{
        ////    return;
        ////}
        //transform.position = my;

        return false;


    }
   
    void SetLinkPos()
    {
        if (m_gChild == null) return;
        if (m_gActor==null) return;
        if (m_bStop) return;
        
        Vector3 vPos = m_gChild.transform.position;
        Vector3 vActor = m_gActor.transform.localPosition;

        vActor.y = CWHero.Instance.GetPosY()-5;

      //  vPos = Vector3.Lerp(vPos, vActor, Time.deltaTime * 10f);
        m_gActor.transform.localPosition = vPos;// vPos;



        if (m_bRotate) return;
        if (m_gTarget)
        {
            float fYaw = CWMath.GetLookYaw(m_gActor.transform.position, m_gTarget.transform.position);
            m_gActor.SetYaw(fYaw);

        }
        

    }

    

    float m_fSpeedRate = 0.1f;
    float m_fStCooltime = 0.5f;//간격

    float m_fCooltime = 0.5f;//간격

    float m_fRange = 32;// 사정거리 

    int m_nDiff = 0;
    int m_nLevel = 0;

    // 격차 계산
    int GetDiff()
    {
        if (m_gActor == null) return 0; 
        //0~5 
        int h1= m_gActor.GetDamage()*3+(int)m_gActor.GetHP();
        int h2= m_gTarget.GetDamage()*3+(int)m_gTarget.GetHP();
        float ff = (float)h2 / (float)h1;

        if(ff<= 0.5f) // 내가 완전이김
        {
            return 0;
        }
        if (ff <= 0.9 && ff>0.5f) //
        {
            return 1;
        }
        if (ff <= 1.1 && ff > 0.9f) //비슷함
        {
            return 2;
        }
        if (ff <= 1.5 && ff > 1.1f) // 조금 강함
        {
            return 3;
        }
        return 4;// 상대가 강함
    }
    // 랜덤 래벨 

    public void SetPos(Vector3 vPos)
    {
        transform.position = vPos;
    }

    string GetPath()
    {
        
        int rr = Random.Range(0, 9);
        string szfile= "PVP_" + rr.ToString();
        return szfile;
    }
    // 상대가 결정이 된다
    public void Begin(Vector3 vPos, CWAirObject gTarget, CWAirObject gMyActor,int Level,bool bDrone=false)
    {
        
        m_gTarget = gTarget;
        m_gActor = gMyActor;
        m_nDiff = GetDiff();// 격차 계산

        m_bDrone = bDrone;
        if (bDrone)
        {
            m_nDiff = Random.Range(0, 1);
        }
        m_nLevel = Level;
        transform.position = vPos;

       // Debug.Log(string.Format("diff {0} Lv {1}  ", m_nDiff, m_nLevel));

        CWArrayManager.PVPData pData = CWArrayManager.Instance.GetPVPData(m_nDiff, m_nLevel);
        m_fStCooltime = pData.m_fCooltime+0.5f;
        m_fCooltime =  m_fStCooltime;
        if(bDrone)
        {
            m_fCooltime = 1.2f;
            m_fStCooltime = m_fCooltime;


        }
        


        m_fSpeedRate = pData.m_fSpeedRate;
        m_fRange = 90;//pData.m_fRange;

      //  Debug.Log(string.Format("diff {0} Lv {1}  cooltime {2} range {3} SpeedRate{4} ",pData.m_nDiff, pData.m_nLevel, pData.m_fCooltime,pData.m_fRange, pData.m_fSpeedRate));

        
        
        DOTweenPath gPath = CWResourceManager.Instance.GetPVPPath(GetPath());


        gPath.transform.SetParent(transform);
        gPath.transform.localPosition = Vector3.zero;
        gPath.DOPlay();
        
        m_gChild = gPath.gameObject;


        m_fSpeed =  m_fSpeedRate;


        StartCoroutine("IRun");

    }
 
    void Shoot()
    {

        if (m_gActor == null) return;
        // 사정거리가 가까워야 한다 


        Vector3 my = m_gActor.transform.position;
        Vector3 target = m_gTarget.transform.position;
        float fDist = Vector3.Distance(my, target);
        if (fDist > m_fRange * 2) return;// 너무 멀다면 공격 불가

        int RR = Random.Range(0, 3);
        if(!m_bDrone)
        {
            RR = 2;
        }
        if (RR == 1)
        {

            Vector3 vdir = target - my;
            vdir.Normalize();
            int aa = Random.Range(-30, 30);
            Vector3 vv = CWMath.CalYaw(aa, vdir);
            Vector3 vPos = my + vv * 100;
            m_gActor.AIShootPos(true, vPos);
        }
        else if (RR == 2)
        {
            m_gActor.AIShoot(true, m_gTarget);
        }
        else
        {
            Vector3 vmdir = m_gTarget.GetMoveDir();// 이동하는 방향
            Vector3 vPos = target + vmdir * 200;
            m_gActor.AIShootPos(true, vPos);
        }




    }


    IEnumerator IRun()
    {

        
        float fStart= Time.time;
        m_gChild.transform.DOLocalMoveY(60, 0);

        
        
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (m_gTarget == null) break;
            if(m_gTarget.GetHP()<=0)
            {
                break;
            }
            if (m_gActor == null) break;
            if(m_gActor.GetHP()<=0)
            {
                break;
            }
            float ftime = Time.time - fStart;
            if (ftime > m_fCooltime)
            {
                float RR = (Random.Range(0, 1)-0.5f);
                m_fCooltime = m_fStCooltime + RR;
                if(m_fCooltime<0.2f)
                {
                    m_fCooltime = 0.2f;
                }

                Shoot();
                fStart = Time.time;
            }
            //DistMove();

        }
        Destroy(gameObject);

    }

    private void Update()
    {
        if (DistMove()) return;
        SetLinkPos();
        AvoidPlay();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

    
    bool m_bRotate = false;
    float m_fPosX = 0;
    bool m_bStop = false;

    void _DontStop()
    {
        m_bStop = false;
    }
    void StopMove()
    {
        m_bStop = true;
        Invoke("_DontStop", 1f);
    }

    #region 충돌회피

    Vector3 m_vMissile=Vector3.zero;
    bool m_bAvoidFlag=false;// 회피 시작
    // 미사일과 가까운가?
    bool IsMissileNear()
    {
        CWMissile[] cs = CWPoolManager.Instance.gameObject.GetComponentsInChildren<CWMissile>();
        if (cs.Length == 0) return false;

        foreach(var v in cs)
        {
            if (v.gameObject.tag != "Hero") continue;
            float fdist = Vector3.Distance(v.gameObject.transform.position, transform.position);
            if(fdist<32)
            {
                m_vMissile = v.gameObject.transform.position;
                return true;
            }
        }



        return false;
    }
    // 회피 
    void AvoidMove()
    {
        if (m_gActor == null) return;
        
        m_bAvoidFlag = true;
        m_fPosX = transform.localPosition.x;

        
        float fdeltaValue = Random.Range(10, 30+ m_nLevel*2);

        if(CWHeroManager.Instance.m_bTuto)
        {
            fdeltaValue = 4;
        }



        int ndir = 1;
        float fDelta = fdeltaValue;
     //   Debug.Log(string.Format(" {0}  {1}", m_fPosX, m_vMissile.x));
        if (m_fPosX < m_vMissile.x)
        {
            fDelta = -fdeltaValue;
            ndir = -1;

        }
        //if (my.x <= 30 && my.x > 220 && my.z <= 30 && my.z > 220)
        //{
        //    return;
        //}

        float xx = m_fPosX + fDelta;
        if (xx <= 20) xx = 20;
        if (xx >= 240) xx = 240;
        transform.DOLocalMoveX(xx, 0.7f).OnComplete(() => {
            m_bAvoidFlag = false;
          //  Debug.Log("Detect Move End");
        });

        //        m_bRotate = true;
        float fval = 10 * ndir;
        Transform tForm = m_gActor.m_gCenterObject.transform;
        tForm.DOLocalRotate(new Vector3(0, 0, fval), 0.7f).OnComplete(() => {

            m_bRotate = false;
            tForm.DOLocalRotate(new Vector3(0, 0, 0), 0.3f);
        });



    }
    void AvoidPlay()
    {
        if (m_bAvoidFlag) return;

        if (!IsMissileNear()) return; // 미사일과 가깝다
        AvoidMove();



    }


    #endregion


}
