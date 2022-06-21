using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

///using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System.Linq;
using SimpleJSON;

using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

using DG.Tweening;
using CWUnityLib;
using CWEnum;
using CWStruct;
public class CWAIEntity : MonoBehaviour {

    #region 초기정의

    
    bool m_bResult = false;
    public bool m_bDebug = false;
    

    CWObject m_gParent;
    public AITYPE m_AITYPE = AITYPE.PASSIVE;
    public bool m_bFirstAttack; // 선공 유닛인가?
    public float m_fSightRange = 80f;// 검색 시야 

    public float m_fBattleSightRange = 80f;// 배틀에서만 사용
    
    public float m_Range = 32; // 사정거리 
    public float m_Speed = 0; // 이동 속도 
    public float m_BlockRange = 32;

    public float m_fCooltime = 3f;
    float m_fShoottime;
    public float m_fBoomCooltime;
    public float m_fThinktime = 1f;

    
    public string m_szEnmeyTag;// 적태그 
    Vector3 m_vMove;
    Vector3 m_vCenter;
    public int m_TargetBlock;
    public Vector3 m_vTargetBlockPos = Vector3.zero;

    public Vector3 m_vTarget = Vector3.zero;


    JObject m_jData;
    
    Dictionary<string, string> m_FuctionList = new Dictionary<string, string>();

    bool m_bCreated = false;


    int GetID()
    {
        return m_gParent.m_nID;
    }
    private void OnEnable()
    {
        if (m_bCreated)
        {
            if (m_jData != null)
                StartCoroutine("RunFunction");


        }

    }
    #endregion

    #region 변수

    float m_fMinDist = 1f;


    CWObject m_gEnemy;

    public CWObject GetEnemy()
    {
        return m_gEnemy;
    }

    public void SetEnemy(CWObject gEnemy)
    {
        if(gEnemy==CWHero.Instance)
        {
            CWHero.Instance.SetEnemy(m_gParent);
        }
        
        m_gEnemy = gEnemy;
        if (gEnemy == null) return;
        m_vTarget = m_gEnemy.GetPosition();
    }
    bool m_bMoved = false;
    bool m_bBattleStart = false;
    int m_nRandomValue = 0;

    float [] m_fDelayStart = new float[10];
    int[] Delaytime = { 10, 30, 60, 120, 180,300 };

    bool CheckDelay(int num)
    {
        if (m_fDelayStart[num] == 0)
        {
            m_fDelayStart[num] = Time.time;
            return false;
        }
        float tt = Time.time - m_fDelayStart[num];
        if (tt >= (float)Delaytime[num])
        {
            m_fDelayStart[num] = 0;
            return true;
        }
        return false;
    }


    #endregion




    #region 정의함수들 

    IEnumerator Func_Enemy_YES()// 적이 있다
    {
        yield return null;
        if (m_gEnemy == null) m_bResult = false;
        else m_bResult=true ;

    }
    IEnumerator Func_Enemy_NO()
    {
        yield return null;
        if (m_gEnemy == null) m_bResult = true;
        else m_bResult = false;
    }
    IEnumerator Func_Bomb_YES() //폭탄사용가능하다
    {
        yield return null;
        if (IsBombCoolTime())
        {
            m_bResult = true;
        }
        else  m_bResult = false;
    }
    IEnumerator Func_Bomb_NO()
    {
        yield return null;
        // 상대이면서, 
        // 퀴스트가 강화석 이기기 라면, 폭탄 사용불가
        
        if (!IsBombCoolTime())
        {
            m_bResult = true;
        }
        else m_bResult = false;

    }
    IEnumerator Func_Bomb()
    {

        yield return null;


        //if(m_gParent==CWHero.Instance)
        //{
        //    HeroShoot(m_vTargetBlockPos);
        //    m_bResult =true;
        //}
        //TurntoEnmey(m_vTargetBlockPos,_TargetBlockBoomShoot);
        _TargetBlockBoomShoot();
        m_fBoomCooltime = Random.Range(1, 4);
        m_bResult =true;
    }



    void MoveBlock_()
    {
        Vector3 vDir = m_vTargetBlockPos - GetPosition();
        vDir.Normalize();
        float fdist = Vector3.Distance(m_vTargetBlockPos, GetPosition())- (m_Range-2);
        if (fdist <= m_Range) fdist = m_Range;
         m_vMove = GetPosition() + vDir * fdist;

    }
    // 딱 한번 실행 
    bool m_bOnceFlag = false;
    IEnumerator Func_OnceRun()
    {
        m_bResult = !m_bOnceFlag;
        m_bOnceFlag = true;
        yield return null;
    }
    IEnumerator Func_Move_Block()//블록 근처 이동
    {
        yield return null;

        m_fMinDist = m_Range/2;
        MoveBlock_();
        yield return StartCoroutine("MoveRun");
        m_bResult =true;
    }
    
    


    /////////////////////////////////////////    

    IEnumerator Func_RotateIdle()// 회전 아이들
    {

        yield return null;
        m_gParent.RotateWeaponIdle();
        m_bResult =true;
    }
    // 패스 아이들
    void PathIdle(Vector3 vPos,float fYaw,float fheight)
    {
        vPos.y = fheight;

        // 싱글 멀티에서만 예외 처리 해야 한다
        if(GamePlay.Instance)
        {
            if(GamePlay.Instance.m_bTutoMulti)
            {
                m_gParent.CopyTransPos(vPos, fYaw);
                return;
            }
        }

        SendMove(vPos, fYaw);

    }

    static int m_PathCount = 0;
    IEnumerator Func_PathIdle()
    {
        yield return null;
        DOTweenPath gPath;
        
        int rr = m_PathCount+1;
        string szPath = string.Format("Idle_{0}",rr);
        float rspeed = Random.Range(0.8f, 1.2f);
        gPath = CWResourceManager.Instance.GetPath(szPath, rspeed);
        gPath.DOPlay();

        m_PathCount++;
        m_PathCount %= 15;


        Vector3 vPos = gPath.transform.position;

        float rh = Random.Range(20,40);
        float fheight = Random.Range(50, 74) ;// CWMapManager.SelectMap.GetHeight(vPos);
        
        while (true)
        {
            if(m_gEnemy)
            {
                break;
            }
            if(gPath==null)
            {
                break;
            }
            vPos = gPath.transform.position;
            PathIdle(vPos, gPath.transform.eulerAngles.y, fheight);
            yield return StartCoroutine(WaitTimeFunc(1f));// new WaitForSeconds(1f);
            //yield return null;
        }
        Destroy(gPath);

    }
    IEnumerator Func_Idle()// 아이들
    {

        yield return null;

        int rr = Random.Range(0, 4);
        if (rr < 3) m_bResult =true;
        Vector3 vStart = m_gParent.VStartPos;
        float fDist = Random.Range(10, 30);
        float ff = Vector3.Distance(vStart, GetPosition());
        if (ff > 64)
        {
            m_vMove = vStart;
        }
        else
        {
            Vector3 v = Random.insideUnitSphere;
            v.y = 0;
            m_vMove = GetPosition() + fDist * v;
        }
        yield return StartCoroutine("MoveRun");
        m_bResult =true;
    }
    IEnumerator Func_FarHome_YES()
    {
        yield return null;

        Vector3 vStart = m_gParent.VStartPos;
        float ff = Vector3.Distance(vStart, GetPosition());
        if (ff > 255) m_bResult =true;
        else     m_bResult =false;
    }
    IEnumerator Func_GoHome()
    {
        yield return null;

        Vector3 vStart = m_gParent.VStartPos;
        m_vMove = vStart;
        yield return StartCoroutine("MoveRun");

        m_bResult =true;
    }
    IEnumerator Func_Overlap()
    {
        yield return null;


        m_bResult = false;

        GameObject[] gg = GameObject.FindGameObjectsWithTag(gameObject.tag);
        foreach(var v in gg)
        {
            CWObject o = v.GetComponent<CWObject>();
            if(o)
            {
                if (o == m_gParent) continue;
                float fdist = Vector3.Distance(GetPosition(), o.GetPosition());
                if(fdist<3)
                {
                    m_bResult =true;
                }
            }
        }


        
    }
    IEnumerator Func_Move_Random()
    {
        yield return null;

      
        if (m_gParent.NLevel < 6) m_bResult =true;

        yield return StartCoroutine("AvoidMoveRun");
        m_bResult =true;
    }
    IEnumerator Func_Enemy_Shoot()//적미사일공격이있다
    {
        yield return null;

        m_bResult =true;
    }
    IEnumerator Func_AIUSER_YES()//AI유저이다
    {
        yield return null;

        if (m_gEnemy == null) m_bResult =false;
        else
        {
            if (m_gEnemy.UserType == USERTYPE.COLLECTUSER
                || m_gEnemy.UserType == USERTYPE.BATTLEAI
                )
            {
                m_bResult = true;

            }
            else m_bResult = false;

        }
    }
    IEnumerator Func_Move_YES()// 움직이고 있다
    {
        yield return null;


        m_bResult =m_bMoved;
    }
    IEnumerator Func_Move_NO()
    {
        yield return null;

        m_bResult =!m_bMoved;
    }
    IEnumerator Func_Stop_Move()
    {

        yield return null;

        m_bMoved = false;
        StopCoroutine("MoveRun");
        m_bResult =true;
    }
    IEnumerator Func_FirstAttakAI_YES()//선공몹이다
    {
        yield return null;

        m_bResult =m_bFirstAttack;
    }
    IEnumerator Func_Range_YES() //사정거리에 있다
    {
        yield return null;

        // 위에서는 밑으로 공격 가능 
        if (m_gEnemy == null) m_bResult =false;
        else
        {

            m_bResult = false;
            float fdist = Vector3.Distance(GetPosition(), m_vTarget);
            if (fdist < m_Range)
            {
                m_bResult = true;
            }
            else
            {
                m_bResult = false;

            }
                


        }


    }
    IEnumerator Func_Attack()
    {
        yield return null;

        if (m_bMoved)
        {
            _Shoot();
            m_bResult =true;
        }
        _Shoot();
        m_bResult =true;
    }
    IEnumerator Func_Range_NO()//사정거리에 없다
    {

        yield return null;


        float fdist = Vector3.Distance(GetPosition(), m_vTarget);
        if (fdist < m_Range) m_bResult =false;
        else m_bResult =true;

    }
    IEnumerator Func_Attack_YES()//일반공격가능하다
    {
        yield return null;

        m_bResult =IsCoolTime();
    }
    IEnumerator Func_Attack_NO()
    {
        yield return null;

        m_bResult =!IsCoolTime();
    }
    IEnumerator Func_DONTMOVE_YES()//움직일 수 있다
    {
        yield return null;

        if (m_bMoved) m_bResult =false;
        if (m_gParent.IsDrone()) m_bResult =true;
        else       m_bResult =false;
    }
    IEnumerator Func_DONTMOVE_NO()
    {
        yield return null;

        if (m_gParent.IsDrone()) m_bResult =false;
        else        m_bResult =true;
    }

    Vector3 GetMoveVector()
    {
        // 적이 있을 때 , 없을때 
        if(m_gEnemy!=null)
        {
            m_vTarget = m_gEnemy.GetPosition();
            Vector3 vDir = m_vTarget - GetPosition();
            vDir.Normalize();
            int rr = (int)(m_Range * 0.7f);
            float fdist = Vector3.Distance(m_vTarget, GetPosition()) - (rr);
            m_vMove = GetPosition() + vDir * fdist;
        }
        return m_vMove;
    }

    IEnumerator Func_Move_Enemy()//적 근처에 간다
    {
        yield return null;

        Vector3 vDir = m_vTarget - GetPosition();
        vDir.Normalize();
        int rr =(int) (m_Range * 0.7f);
        float fdist = Vector3.Distance(m_vTarget, GetPosition()) - (rr);
        m_vMove = GetPosition() + vDir * fdist;
        yield return  StartCoroutine("MoveRun");
        m_bResult =true;

    }
    IEnumerator Func_SearchHaijuk()
    {
        yield return null;
        m_bResult = true;

      


    }
    IEnumerator Func_SearchUser()
    {
        yield return null;
        m_bResult = true;

        if (m_gEnemy == null)
        {
            if (CWMapManager.BDontFight)
            {
                m_bResult = false; // 전투 금지!
            }
            else
            {
               

                m_gEnemy = CWUserManager.Instance.Search(m_gParent.NTeam, GetPosition(), (int)m_fSightRange);
                if (m_gEnemy != null)
                    m_vTarget = m_gEnemy.GetPosition();
            }

        }
    }
    IEnumerator Func_Search()
    {
        yield return null;

        Vector3 vStart = m_gParent.VStartPos;

        CWObject[] gg = transform.root.GetComponentsInChildren<CWObject>();
        float fMindist = 100000f;
        foreach (var v in gg)
        {
            CWObject o = v;
            if (o == null) continue;
            if(o.NTeam==1)
            {
                //AI 이다
                continue;
            }
            if(!o.IsEnemy(m_gParent))
            {
                continue;
            }


            //float fDist = Vector3.Distance(v.GetPosition(), GetPosition());
            float fDist = (v.GetPosition().x - vStart.x) * (v.GetPosition().x - vStart.x)
                + (v.GetPosition().z - vStart.z) * (v.GetPosition().z - vStart.z);
            if (fDist <= (m_fSightRange - 1) * (m_fSightRange - 1))
            {
                //if (!IsFrontEnemy(v.GetPosition())) continue;// 내 시야에서 안보이면 적으로 간주 안함 
                //if (IsBlockWall(v.GetPosition())) continue;// 블록 장애물이 존재한다 

                if (fDist < fMindist)
                {
                    fMindist = fDist;
                    if (o != null)
                    {
                        m_gEnemy = o;
                        m_vTarget = m_gEnemy.GetPosition();
                    }
                }
            }
        }

        m_bResult =true;
    }
    IEnumerator Func_Block_YES()//블록이 있다
    {

        yield return null;

        if (CWMapManager.SelectMap.GetBlock(m_vTargetBlockPos) == 0)
        {
            m_vTargetBlockPos = Vector3.zero;
        }

        if (m_vTargetBlockPos == Vector3.zero) m_bResult =false;
        else m_bResult =true;
    }
    IEnumerator Func_Block_NO()
    {
        yield return null;

        if (m_vTargetBlockPos == Vector3.zero) m_bResult =true;
        else m_bResult =false;

    }

    void RangeBlock_YES()
    {
        float fdist = Vector3.Distance(GetPosition(), m_vTargetBlockPos); //CWMath.VectorDistSQ(GetPosition(), m_vTargetBlockPos);
        if (fdist < m_Range ) m_bResult = true;
        else m_bResult = false;

    }
    IEnumerator Func_RangeBlock_YES()//블록이 사정거리에 있다
    {
        yield return null;
        RangeBlock_YES();

    }
    void RangeBlock_NO()
    {
        float fdist = Vector3.Distance(GetPosition(), m_vTargetBlockPos);
        if (fdist < m_Range )
        {
            m_bResult = false;
        }
        else m_bResult = true;

    }
    IEnumerator Func_RangeBlock_NO()
    {
        yield return null;
        RangeBlock_NO();

    }
    IEnumerator Func_CenterSearchBlock()
    {

        yield return null;

        m_bResult =true;

    }
    IEnumerator Func_AllSearchBlock()
    {
        yield return null;

        m_bResult =true;
    }
    IEnumerator Func_HeroEnemy()
    {
        yield return null;

        m_gEnemy = CWHero.Instance;
        m_vTarget = m_gEnemy.GetPosition(); //
        m_bResult =true;
    }
    
    IEnumerator Func_Connected()
    {
        yield return null;

        m_bResult =CWUdpManager.Instance.m_bConnected ;
    }
    IEnumerator Func_DontConnected()
    {
        yield return null;

        m_bResult =!CWUdpManager.Instance.m_bConnected;
    }

    IEnumerator Func_EmpytOwner()// 주인이 없는 AI
    {
        yield return null;
        m_bResult = true;
        if (!CWGlobal.g_bSingleGame)
            m_bResult =CWAIOwnerManager.Instance.IsEmpty(m_gParent.m_nID);
    }
    IEnumerator Func_SendAskOwner()//주인을 요청한다
    {
        yield return null;
        if(CWGlobal.g_bSingleGame)
        {
            m_bResult = true;
        }
        else
        {
            CWSocketManager.Instance.SendAskOwner(m_gParent.m_nID);
            m_bResult = true;

        }

    }
    IEnumerator Func_Not_MyOwner()
    {
        yield return null;
        m_bResult = false;
        if (!CWGlobal.g_bSingleGame)
        {
            m_bResult = !CWAIOwnerManager.Instance.IsMyOwner(m_gParent.m_nID);
        }
            
    }
    IEnumerator Func_MyOwner()
    {
        m_bResult = true;
        yield return null;
        if(!CWGlobal.g_bSingleGame)
        {
            m_bResult = CWAIOwnerManager.Instance.IsMyOwner(m_gParent.m_nID);
        }

        
    }
    IEnumerator Func_HeroDist_YES()
    {
        yield return null;

        

        Vector3 vPos = CWLib.GetCenterPos();
        float fdist = Vector3.Distance(vPos, GetPosition());
        if (fdist < m_fSightRange) m_bResult = true;
        else m_bResult = false;



    }
    IEnumerator Func_Reset()
    {
        yield return null;

        if (!CWGlobal.g_bSingleGame)
        {
            if (CWAIOwnerManager.Instance.IsMyOwner(m_gParent.m_nID))
                CWAIOwnerManager.Instance.SendRemoveOwner(m_gParent.m_nID);

        }
        m_gEnemy = null;
        m_bMoved = false;
        m_bResult =true;
    }
    IEnumerator Func_BattleStart()
    {
        yield return null;

        m_bBattleStart = true;
        // 시작위치

        m_vTargetBlockPos=FirstSearchBlock();
        m_vTarget = m_vTargetBlockPos;

        m_bResult =true;
    }
    IEnumerator Func_BattleStart_NO()
    {

        yield return null;

        m_bResult =m_bBattleStart;
    }

    IEnumerator Func_Enemy_Height_EQ_NO()// 적과 높이가 틀리다
    {
        m_bResult = false;
        yield return null;

        if (!m_gEnemy) m_bResult =false;
        else
        {
            if(m_gEnemy.IsDrone())
            {
                
                if (m_Speed == 0) m_bResult = false;
                Vector3 vv = m_gEnemy.GetPosition();

                if (Mathf.Abs(vv.y - GetPosition().y) > 10f) m_bResult = true;
                else m_bResult = false;

            }
            else
            {
                
            }
            


        }
    }
    IEnumerator Func_Enemy_HeightRun()
    {
        yield return null;

        if (m_Speed == 0) m_bResult =false;
        else
        {
            yield return StartCoroutine("HeightRun");

            m_bResult = true;

        }
    }
    IEnumerator Func_Move_Center()
    {

        yield return null;

        Vector3 vTarget = m_vCenter;


        Vector3 vDir = vTarget - GetPosition();
        vDir.Normalize();
        m_vMove = GetPosition() + vDir * 5f;


        yield return StartCoroutine("MoveRun");
        m_bResult =true;

    }
    IEnumerator Func_AITYPE_ACTIVE()
    {
        yield return null;

       if (m_AITYPE == AITYPE.ACTIVE) m_bResult =true;
       else m_bResult =false;
    }
    IEnumerator Func_AITYPE_PASSIVE()
    {
        yield return null;

        if (m_AITYPE == AITYPE.PASSIVE) m_bResult =true;
        else m_bResult = false;

    }
    // 드론을 만든다!
    IEnumerator Func_CreateDrone()
    {
        yield return null;
       

    }
    IEnumerator Func_RotateAttack()
    {
        yield return null;

        float fstart = Time.time;
        while (true)
        {
            float dt = Time.time - fstart;
            if (dt > 4f)
            {
                break;
            }
            Vector3 vv = m_gParent.transform.eulerAngles;
            vv.y += Time.deltaTime * 30f;
            m_gParent.transform.eulerAngles = vv;
            _Shoot();
            yield return new WaitForSeconds(0.5f);

        }

        m_bResult = true;
    }
    IEnumerator Func_BeforeAction()
    {
        yield return null;
        if (m_nRandomValue >= 0 && m_nRandomValue < 20) m_bResult = true;
        else m_bResult = false;

    }
    // 제자리 회전
    IEnumerator Func_Rotate()
    {
        yield return null;

        float fstart = Time.time;
        while(true)
        {
            float dt = Time.time - fstart;
            if(dt>2f)
            {
                break;
            }
            Vector3 vv = m_gParent.transform.eulerAngles;
            vv.y += Time.deltaTime * 30f;
            m_gParent.transform.eulerAngles = vv;

            yield return null;
        }
        //m_gParent.transform.DORotate(new Vector3(0,))
        
        m_bResult = true;

    }

    IEnumerator Func_Random_20()
    {
        yield return null;
        if (m_nRandomValue >=0 && m_nRandomValue < 20) m_bResult = true;
        else m_bResult = false;

    }

    IEnumerator Func_Random_40()
    {
        yield return null;
        if (m_nRandomValue >= 20 && m_nRandomValue < 40) m_bResult = true;
        else m_bResult = false;

    }

    IEnumerator Func_Random_60()
    {
        yield return null;
        if (m_nRandomValue >= 40 && m_nRandomValue < 60) m_bResult = true;
        else m_bResult = false;

    }

    IEnumerator Func_Random_80()
    {
        yield return null;
        if (m_nRandomValue >= 60 && m_nRandomValue < 80) m_bResult = true;
        else m_bResult = false;

    }

    IEnumerator Func_Random_100()
    {
        yield return null;
        if (m_nRandomValue >= 80 && m_nRandomValue < 100) m_bResult = true;
        else m_bResult = false;

    }


    IEnumerator Func_Random_50_YES()
    {
        yield return null;


        if (m_nRandomValue < 50) m_bResult =true;
        else m_bResult = false;
    }

    IEnumerator Func_Random_50_NO()
    {
        yield return null;

        if (m_nRandomValue >= 50) m_bResult =true;
        else m_bResult = false;
    }

    IEnumerator Func_Random_80_YES()
    {
        yield return null;
        if (m_nRandomValue < 80) m_bResult =true;
        else m_bResult = false;
    }

    IEnumerator Func_Random_80_NO()
    {
        yield return null;

        if (m_nRandomValue >= 80) m_bResult =true;
        else m_bResult = false;
    }

    IEnumerator Func_Random_99_YES()
    {
        yield return null;
        if (m_nRandomValue < 99) m_bResult =true;
        else m_bResult = false;
    }

    IEnumerator Func_Random_99_NO()
    {
        yield return null;

        if (m_nRandomValue >= 99) m_bResult =true;
        else m_bResult = false;
    }
    IEnumerator Func_EnemyStrong()
    {
        yield return null;

        // 나보다 5 이상  강할때
        if (m_gParent.NLevel+5 < m_gEnemy.NLevel)
        {
            m_bResult =true;
        }
        else m_bResult =false;

    }
    // 월드에 있는 금광지역을 검색한다

    

    IEnumerator Func_GoStage()
    {
        yield return null;

        //UI_Header.Instance.OpenStageUI();
        m_bResult =true;
    }
    IEnumerator Func_FULL_INVEN()
    {
        yield return null;

        //if (TempInven.Instance.IsFull()) m_bResult =true;
        //else  m_bResult =false;
    }

    // 일딴 다음에 작업 포기
    IEnumerator Func_Giveup()
    {
        yield return null;

        Vector3 vDir = GetPosition() - m_vTarget;// 반대로 도망
        vDir.Normalize();
        float fdist = 255;
        m_vMove = GetPosition() + vDir * fdist;

        yield return StartCoroutine("MoveRun");
        m_bResult =true;
    }
    // 시간을 잰다 

    IEnumerator Func_WAIT_01()
    {
        //yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(WaitTimeFunc(0.1f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_03()
    {
        //yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(WaitTimeFunc(0.3f));
        m_bResult = true;
    }

    IEnumerator Func_WAIT_05()
    {
        //yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(WaitTimeFunc(0.5f));
        m_bResult = true;
    }

    IEnumerator Func_WAIT_1()
    {

        //yield return new WaitForSeconds(1);
        yield return StartCoroutine(WaitTimeFunc(11f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_3()
    {
        //yield return new WaitForSeconds(3);
        yield return StartCoroutine(WaitTimeFunc(3f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_5()
    {
        //yield return new WaitForSeconds(5);
        yield return StartCoroutine(WaitTimeFunc(5f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_10()
    {
        //yield return new WaitForSeconds(10);
        yield return StartCoroutine(WaitTimeFunc(10f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_20()
    {
        //yield return new WaitForSeconds(20);
        yield return StartCoroutine(WaitTimeFunc(20f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_30()
    {
        //yield return new WaitForSeconds(30);
        yield return StartCoroutine(WaitTimeFunc(30f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_60()
    {
        //yield return new WaitForSeconds(60);
        yield return StartCoroutine(WaitTimeFunc(60f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_120()
    {
        //        yield return new WaitForSeconds(120);
        yield return StartCoroutine(WaitTimeFunc(120f));
        m_bResult = true;
    }
    IEnumerator Func_WAIT_300()
    {
        //yield return new WaitForSeconds(300);
        yield return StartCoroutine(WaitTimeFunc(300f));
        m_bResult = true;
    }


    IEnumerator Func_DELAY_10()
    {
        yield return null;

        m_bResult = CheckDelay(0);
    }

    IEnumerator Func_DELAY_30()
    {
        yield return null;
        m_bResult =CheckDelay(1);
    }
    IEnumerator Func_DELAY_60()
    {
        yield return null;
        m_bResult =CheckDelay(2);
    }
    IEnumerator Func_DELAY_120()
    {
        yield return null;
        m_bResult =CheckDelay(3);
    }
    IEnumerator Func_DELAY_180()
    {
        yield return null;
        m_bResult =CheckDelay(4);
    }
    IEnumerator Func_DELAY_300()
    {
        yield return null;
        m_bResult =CheckDelay(5);
    }
    IEnumerator Func_SUM_WAIT()// 약간대기
    {
        float fsec = Random.Range(0, 2);
        //yield return new WaitForSeconds(fsec);
        yield return StartCoroutine(WaitTimeFunc(fsec));
    }
    // 타겟방향으로 방향 전환
    IEnumerator Func_TARGET_ROTATE()
    {
        yield return null;
        TurntoEnmey(m_vTarget,null);
        m_bResult = true;
        //yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(WaitTimeFunc(0.5f));
    }
    // 자원을 캐다 
    IEnumerator Func_WORK_RES()
    {
        yield return null;

        //m_bResult = true;
        //for (int i=0;i<20;i++)
        //{
        //    CWWorkDrone gMyUser = gameObject.GetComponentInParent<CWWorkDrone>();
        //    if (gMyUser != null)
        //    {
        //        if (gMyUser.IsTakeBlock())
        //        {
        //            break;
        //        }
        //        gMyUser.AIShootPos(true, m_vTargetBlockPos);
        //    }
        //    yield return new WaitForSeconds(0.5f);
        //}
        //yield return new WaitForSeconds(0.1f);


    }
    IEnumerator Func_SAVE_RES()
    {
        yield return null;

        //CWWorkDrone gMyUser = gameObject.GetComponentInParent<CWWorkDrone>();
        //if(gMyUser)
        //{
        //    gMyUser.SaveRes();
        //    // 이펙트 
        //}


        m_bResult = true;
    }

    
    
    // 

    IEnumerator Func_RandomMove()
    {
        m_bResult = true;

        Vector3 v = Random.insideUnitSphere;
        v.y = 0;

        float ff2 = Random.Range(8, 24);
        float fSpeed = Random.Range(m_Speed-4 , m_Speed+4);
        m_vMove = GetPosition() + v * ff2;

        m_vMove.y = CWMapManager.SelectMap.GetHeight(m_vMove)+20f;


        if (m_vMove.x > 100) m_vMove.x = 100;
        if (m_vMove.x < -100) m_vMove.x = -100;

        if (m_vMove.z > 100) m_vMove.z = 100;
        if (m_vMove.z < -100) m_vMove.z = -100;


        Vector3 vStart = GetPosition();
        Vector3 vTarget = m_vMove;
        Vector3 vDir = vTarget - GetPosition();

        CWObject gMyUser = gameObject.GetComponentInParent<CWObject>();

        float fdist = Vector3.Distance(vTarget, GetPosition());
        float ff = 0;
        vDir.Normalize();

        TurntoEnmey(m_vMove, null);
        m_bMoved = true;

        if (fdist > 0)
        {

            while (true)
            {
                if (vDir == Vector3.zero) break;
                if (gMyUser == null) break;
                ff += Time.deltaTime * m_Speed;
                Vector3 vv = vStart + vDir * ff;
                if (ff >= fdist)
                {
                    break;
                }
                float fYaw = CWMath.GetLookYaw(GetPosition(), m_vMove);
                SendMove(vv, fYaw);
                yield return null;
            }

        }
        m_bMoved = false;

    }

    
    
    

    //정거장 정상 
    IEnumerator Func_STAGE_NOT_DIE()
    {
        m_bResult = false;
        //CWWorkDrone kWorker = gameObject.GetComponentInParent<CWWorkDrone>();
        //if (kWorker != null)
        //{
        //    CWStageBuild kStage = kWorker.GetStage();
        //    if (kStage != null)
        //    {
        //        if (kStage.GetHP() > 0)
        //            m_bResult = true;
        //    }
        //}

        yield return null;
    }

    // 작업 멈춤 
    // 주인공 휴식 
    IEnumerator Func_HERO_DIE()
    {
        if(CWHero.Instance.IsDie())
        {
            m_bResult = true;
        }
        else m_bResult = false;

        yield return null;
        //m_bResult = !CWUserBuildManager.Instance.IsHaveStage();
    }
    IEnumerator Func_HERO_NOT_DIE()
    {

        if (!CWHero.Instance.IsDie())
        {
            m_bResult = true;
        }
        else m_bResult = false;

        yield return null;
        //m_bResult = !CWUserBuildManager.Instance.IsHaveStage();
    }
    IEnumerator Func_HERO_TARGET()
    {
        m_bResult = true;
        m_gEnemy = CWHero.Instance;
        yield return null;
        //m_bResult = !CWUserBuildManager.Instance.IsHaveStage();
    }
    IEnumerator Func_HERO_REPAIR()
    {


        yield return null;
        //m_bResult = !CWUserBuildManager.Instance.IsHaveStage();
    }
    // Story3에서 적을 찾는다 
    // 우선 순위 정거장을 찾는다
    // 주이공이 가까우면 적으로 바꿈
    


    IEnumerator Func_WORLD_FINDBLOCK()
    {
        m_bResult = true;
        m_vTargetBlockPos = CWMapManager.SelectMap.FindNearResBlock(transform.position);
        if (m_vTargetBlockPos.x == -1 && m_vTargetBlockPos.y == -1 && m_vTargetBlockPos.z == -1)
        {
            m_vTargetBlockPos = Vector3.zero;

            m_bResult = false;// 종료
        }
        yield return null;
    }
    IEnumerator Func_WORLD_CENTERFINDBLOCK()
    {
        m_bResult = true;
        m_vTargetBlockPos = CWMapManager.SelectMap.FindCenterResBlock();
        if (m_vTargetBlockPos.x == -1 && m_vTargetBlockPos.y == -1 && m_vTargetBlockPos.z == -1)
        {
            m_vTargetBlockPos = Vector3.zero;

            m_bResult = false;// 종료
        }
        if (m_vTargetBlockPos == Vector3.zero) m_bResult = false;// 종료;
        yield return null;
    }
    IEnumerator Func_LOOKAT_TARGET()
    {
        m_bResult = true;

        float fYaw = CWMath.GetLookYaw(m_gParent.GetPosition(), m_vMove);
        if (m_gEnemy != null)
        {
            fYaw = CWMath.GetLookYaw(m_gParent.GetPosition(), m_gEnemy.transform.position);
        }

        m_gParent.SetYaw(fYaw);
        

        yield return null;
    }
    IEnumerator Func_LOOK_TARGET()
    {
        m_bResult = true;
        if (m_gEnemy != null)
        {
            m_gParent.transform.DOLookAt(m_gEnemy.transform.position, 0.5f);
        }
        else
        {
           // if (m_vTargetBlockPos != Vector3.zero) 
             //   m_gParent.transform.DOLookAt(m_vTargetBlockPos, 0.5f);
        }
            

        yield return null;
    }
    IEnumerator Func_Move()
    {
        m_bResult = true;
        while (true)
        {
            if (!MoveObject())
                break;
            yield return null;
        }


    }
    IEnumerator Func_Start()
    {
        m_bResult = true;
        yield return null;
    }




    





    #endregion

    #region 함수등록
    void RegFunction()
    {
        if(m_FuctionList.Count>0)
        {
            return;
        }

        //보스 개념

        m_FuctionList.Add("회전하기", "Func_Rotate");
        m_FuctionList.Add("전조액션", "Func_BeforeAction");//회전공격 드론생성
        m_FuctionList.Add("회전공격", "Func_RotateAttack");
        m_FuctionList.Add("드론생성", "Func_CreateDrone");

        m_FuctionList.Add("확률 0~20", "Func_Random_20");
        m_FuctionList.Add("확률 20~40", "Func_Random_40");
        m_FuctionList.Add("확률 40~60", "Func_Random_60");
        m_FuctionList.Add("확률 60~80", "Func_Random_80");
        m_FuctionList.Add("확률 80~100", "Func_Random_100");




        m_FuctionList.Add("확률 50 YES", "Func_Random_50_YES");
        m_FuctionList.Add("확률 50 NO", "Func_Random_50_NO");
        m_FuctionList.Add("확률 80 YES", "Func_Random_80_YES");
        m_FuctionList.Add("확률 80 NO", "Func_Random_80_NO");

        m_FuctionList.Add("확률 99 YES", "Func_Random_99_YES");
        m_FuctionList.Add("확률 99 NO", "Func_Random_99_NO");

        m_FuctionList.Add("적극적타입이다", "Func_AITYPE_ACTIVE");
        m_FuctionList.Add("수동적타입이다", "Func_AITYPE_PASSIVE");

        m_FuctionList.Add("적과 높이가 틀리다", "Func_Enemy_Height_EQ_NO");
        m_FuctionList.Add("적과 높이맞춤", "Func_Enemy_HeightRun");

        m_FuctionList.Add("배틀시작안했다", "Func_BattleStart_NO");
        m_FuctionList.Add("배틀시작", "Func_BattleStart");

        m_FuctionList.Add("배틀 중앙 이동", "Func_Move_Center");

        m_FuctionList.Add("통신접속중", "Func_Connected");
        m_FuctionList.Add("통신접속중아니다", "Func_DontConnected");

        m_FuctionList.Add("주인이 없는AI", "Func_EmpytOwner");
        m_FuctionList.Add("주인을 요청한다", "Func_SendAskOwner");
        m_FuctionList.Add("AI를관리한다", "Func_MyOwner");
        m_FuctionList.Add("AI를관리안한다", "Func_Not_MyOwner");

        m_FuctionList.Add("AI리셋", "Func_Reset");

        m_FuctionList.Add("주인공이 시야에 들어왔다", "Func_HeroDist_YES");
        //m_FuctionList.Add("배틀맵이다", "Func_BattleMap_YES");

        m_FuctionList.Add("폭탄사용가능하다", "Func_Bomb_YES");
        m_FuctionList.Add("폭탄사용불가능하다", "Func_Bomb_NO");

        m_FuctionList.Add("폭탄공격", "Func_Bomb");


        m_FuctionList.Add("아이들", "Func_Idle");

        m_FuctionList.Add("패스아이들", "Func_PathIdle");//패스아이들 

        m_FuctionList.Add("회전아이들", "Func_RotateIdle");

        m_FuctionList.Add("홈에서 멀어졌다", "Func_FarHome_YES");
        m_FuctionList.Add("홈으로 복귀한다", "Func_GoHome");


        m_FuctionList.Add("다른 오브젝트랑 겹쳐저있다", "Func_Overlap");
        m_FuctionList.Add("램덤하게 평행이동", "Func_Move_Random");

        m_FuctionList.Add("적미사일공격이있다", "Func_Enemy_Shoot");

        m_FuctionList.Add("AI유저이다", "Func_AIUSER_YES");

        m_FuctionList.Add("움직이고있다", "Func_Move_YES");
        m_FuctionList.Add("정지하고있다", "Func_Move_NO");

        m_FuctionList.Add("정지한다", "Func_Stop_Move");

        m_FuctionList.Add("선공몹이다", "Func_FirstAttakAI_YES");
        m_FuctionList.Add("적이있다", "Func_Enemy_YES");
        m_FuctionList.Add("적이없다", "Func_Enemy_NO");
        m_FuctionList.Add("사정거리에 있다", "Func_Range_YES");
        m_FuctionList.Add("사정거리에 없다", "Func_Range_NO");
        m_FuctionList.Add("일반공격가능하다", "Func_Attack_YES");
        m_FuctionList.Add("일반공격불가능하다", "Func_Attack_NO");
        m_FuctionList.Add("공격", "Func_Attack");
        m_FuctionList.Add("움직일 수 있다", "Func_DONTMOVE_YES");
        m_FuctionList.Add("움직일 수 없다", "Func_DONTMOVE_NO");
        m_FuctionList.Add("적 근처에 간다", "Func_Move_Enemy");
        m_FuctionList.Add("적검색", "Func_Search");
        m_FuctionList.Add("유저적검색", "Func_SearchUser");
        m_FuctionList.Add("해적선검색", "Func_SearchHaijuk");
        

        

        m_FuctionList.Add("블록이 있다", "Func_Block_YES");
        m_FuctionList.Add("블록이 없다","Func_Block_NO");
        m_FuctionList.Add("블록이 사정거리에 있다","Func_RangeBlock_YES");
        m_FuctionList.Add("블록이 사정거리에 없다","Func_RangeBlock_NO");
        m_FuctionList.Add("중앙블록찾기","Func_CenterSearchBlock");
        m_FuctionList.Add("모든블록찾기","Func_AllSearchBlock");
        m_FuctionList.Add("주인공적만들기","Func_HeroEnemy");
        m_FuctionList.Add("적이나보다강하다","Func_EnemyStrong");//
        m_FuctionList.Add("포기","Func_Giveup");

        m_FuctionList.Add("월드블록근처검색","Func_WORLD_FINDBLOCK");//
        m_FuctionList.Add("월드블록중앙검색","Func_WORLD_CENTERFINDBLOCK");//

        m_FuctionList.Add("타겟바라봄", "Func_LOOK_TARGET");//
        m_FuctionList.Add("타겟즉시바라봄", "Func_LOOKAT_TARGET");
        

        m_FuctionList.Add("인벤이꽉찾다","Func_FULL_INVEN");//
        m_FuctionList.Add("정거장으로","Func_GoStage");//


        m_FuctionList.Add("0.1초 기다림", "Func_WAIT_01");//
        m_FuctionList.Add("0.3초 기다림", "Func_WAIT_03");//
        m_FuctionList.Add("0.5초 기다림", "Func_WAIT_05");//
        m_FuctionList.Add("1초 기다림", "Func_WAIT_1");//
        m_FuctionList.Add("3초 기다림", "Func_WAIT_3");//
        m_FuctionList.Add("5초 기다림", "Func_WAIT_5");//
        m_FuctionList.Add("10초 기다림", "Func_WAIT_10");//
        m_FuctionList.Add("20초 기다림", "Func_WAIT_20");//
        m_FuctionList.Add("30초 기다림", "Func_WAIT_30");//
        m_FuctionList.Add("60초 기다림", "Func_WAIT_60");//
        m_FuctionList.Add("120초 기다림", "Func_WAIT_120");//
        m_FuctionList.Add("300초 기다림", "Func_WAIT_300");//

        m_FuctionList.Add("10초 지난후","Func_DELAY_10");//
        m_FuctionList.Add("30초 지난후","Func_DELAY_30");//
        m_FuctionList.Add("60초 지난후","Func_DELAY_60");//
        m_FuctionList.Add("120초 지난후","Func_DELAY_120");//
        m_FuctionList.Add("180초 지난후","Func_DELAY_180");//
        m_FuctionList.Add("300초 지난후","Func_DELAY_300");//


        m_FuctionList.Add("약간대기", "Func_SUM_WAIT");//
        m_FuctionList.Add("타겟방향회전", "Func_TARGET_ROTATE");//


        

        
        

        m_FuctionList.Add("랜덤하게 이동", "Func_RandomMove");//
        


        
        

        


        m_FuctionList.Add("주인공파괴", "Func_HERO_DIE");//
        m_FuctionList.Add("주인공정상", "Func_HERO_NOT_DIE");//
        m_FuctionList.Add("주인공타겟", "Func_HERO_TARGET");//
        
        m_FuctionList.Add("주인공고침", "Func_HERO_REPAIR");//


        m_FuctionList.Add("움직임", "Func_Move");//

        m_FuctionList.Add("한번실행", "Func_OnceRun");//
        /// 집단 ai

        m_FuctionList.Add("시작", "Func_Start");//

        

    }



    #endregion
    #region 외부 접근함수

    public void Create(CWObject cwobject, JObject jData)
    {
        if (CWMapManager.SelectMap == null) return;
        m_bMoved = false;
        m_gParent = cwobject;
        m_jData = jData;
        
        m_vCenter = GetRandomCenterPos();
        RegFunction();
        m_fBoomCooltime = Random.Range(1, 4);

        Begin();

    }
   // public void 
   
    public void SetMove(Vector3 vPos)
    {
        m_vMove = vPos;
    }
    public void Begin()
    {
        // 클랜맵일 경우 모든 AI는 모두 클랜편이다 


        m_vPos = m_gParent.GetPosition();

        m_bCreated = true;
        //if (CWMapManager.m_bDontFight) return; // 전투 금지는 AI 금지 
        m_gEnemy = null;
        StopAllCoroutines();
        if(m_jData!=null)
            StartCoroutine("RunFunction");
        
    }
    public void Stop()
    {
        Func_Reset();
        StopAllCoroutines();
    }
    public void SetDie()
    {
        Func_Reset();
    }

    #endregion




    #region AI구현


    Vector3 GetRandomCenterPos()
    {
        /*
                string szTag = "";
                if (m_nTeam == 0)
                {
                    szTag = "A_Pos";
                }
                else
                {
                    szTag = "B_Pos";
                }
                GameObject[] gg = GameObject.FindGameObjectsWithTag(szTag);
                if (gg.Length == 0) return Vector3.zero;

                int rr = Random.Range(0, gg.Length);
                return gg[rr].GetPosition();
        */
        int dx = CWMapManager.SelectMap.WORLDSIZE / 2;
        float y = GetPosition().y;

        float fx = Random.insideUnitCircle.x*32;
        float fz = Random.insideUnitCircle.y*32;
        return new Vector3(dx+fx,y, dx+fz);



        

    }

    
    Vector3 FirstSearchBlock()
    {

        float y = GetPosition().y;
        Vector3 vCenter = new Vector3(256, y, 256);

        Vector3 vR = Random.insideUnitSphere;
        vR.y = y;
        vCenter = vCenter + vR * 200;
        vCenter.y = y;
        Vector3 vDir = vCenter - GetPosition();
        vDir.Normalize();


        float fdist = Vector3.Distance(vCenter, GetPosition());
        int rr = Random.Range(50, 80);
        Vector3 v = GetPosition() + vDir * rr;
        return v;

    }

    bool IsCoolTime()
    {
        if (Time.time - m_fShoottime > m_fCooltime)
        {
            return true;
        }
        return false;
    }

    void _Shoot()
    {
        Shoot(m_gEnemy);
    }
    void Shoot(CWObject gEnemy)
    {
        if (CWGlobal.g_bStopAIAttack) return;
        if (gEnemy == null) return;
        if (gEnemy.GetHP() <= 0) return;

        

        m_gEnemy = gEnemy;
        m_vTarget = m_gEnemy.GetPosition();// m_gEnemy.GetPosition();
        m_fShoottime = Time.time;

        CWObject gMyUser = gameObject.GetComponentInParent<CWObject>();
        if (gMyUser == null)
        {
        
            return;
        }

        gMyUser.AIShoot(true, m_gEnemy);
        CWSocketManager.Instance.SendShoot(gMyUser.m_nID, m_gEnemy.m_nID);

    }
    int BombData(int num)
    {
        //if (num == 1)return (int)GITEM.Bomb1;
        //if (num == 2)return (int)GITEM.Bomb2;
        //if (num == 3)return (int)GITEM.Bomb3;
        //if (num == 4)return (int)GITEM.Bomb4;
        //if (num == 5)return (int)GITEM.Bomb5;
        return 0;
    }
    int GetBombID(int nLevel)
    {

        if (nLevel < 5)
        {
            return BombData(Random.Range(1, 3));
        }
        if (nLevel < 10)
        {
            return BombData(Random.Range(2, 4));
        }
        if (nLevel < 20)
        {
            return BombData(Random.Range(3, 5));
        }
        if (nLevel < 40)
        {
            return BombData(Random.Range(3, 6));
        }
        if (nLevel < 60)
        {
            return BombData(Random.Range(4, 6));
        }
        return BombData(Random.Range(5, 7));
    }
    void _TargetBlockBoomShoot()
    {
        if (m_vTargetBlockPos == Vector3.zero) return;

        m_fShoottime = Time.time;
        CWObject gMyUser = gameObject.GetComponentInParent<CWObject>();
        if (gMyUser == null) return;

        
        int nBombID = GetBombID(gMyUser.NLevel);

        WEAPON nData = CWArrayManager.Instance.GetWeapon(nBombID);
        
        gMyUser.BombShoot(m_vTargetBlockPos, 1, nData.szmissile);

        //gMyUser.SetDirection(m_vTargetBlockPos);


        CWSocketManager.Instance.SendShootPOS(gMyUser.m_nID, m_vTargetBlockPos);


    }
    void _TargetBlockShoot()
    {

        
        if (m_vTargetBlockPos == Vector3.zero) return;

        m_fShoottime = Time.time;
        CWObject gMyUser = gameObject.GetComponentInParent<CWObject>();
        if (gMyUser == null) return;

        gMyUser.AIShootPos(true, m_vTargetBlockPos);


        CWSocketManager.Instance.SendShootPOS(gMyUser.m_nID, m_vTargetBlockPos);

      


    }

    void TurntoEnmey(Vector3 vTarget, EventDelegate.Callback fuc)
    {

        m_gParent.TurntoEnmey(vTarget, fuc);
        //StartCoroutine(TurnFucntion(vTarget,fuc));
    }
    bool IsBombCoolTime()
    {
        if (Time.time - m_fShoottime > m_fBoomCooltime)
        {
            return true;
        }
        return false;
    }


    bool CheckEnmey()
    {
        Vector3 vStart = m_gParent.VStartPos;
        if (m_gEnemy)
        {

            

            float fDist = (m_gEnemy.GetPosition().x - vStart.x) * (m_gEnemy.GetPosition().x - vStart.x)
                + (m_gEnemy.GetPosition().z - vStart.z) * (m_gEnemy.GetPosition().z - vStart.z);

            if (fDist > m_fSightRange * m_fSightRange)
            {
                m_gEnemy = null;// 범위가 벗어 났다면
                return false;
            }


            if (m_gEnemy.GetHP() < 0)
            {
                m_gEnemy = null;
                return false;
            }
            if (!m_gEnemy.gameObject.activeSelf)
            {
                m_gEnemy = null;
                return false;
            }
            if(m_gEnemy.m_nRest==1)
            {
                m_gEnemy = null;
                return false;
            }

        }

        return true;
    }


    //bool OrderFunction(string szFuc)
    //{
    //    if (m_FuctionList.ContainsKey(szFuc))
    //    {
    //        OrderFuc ff = m_FuctionList[szFuc];
    //        return ff();
    //    }
    //    else
    //    {
    //        Debug.LogError("["+szFuc+"]" + " -- 없다!!");
    //    }
    //    return false;
    //}
    //void RunDir(JObject jj)
    //{
    //    if (CWGlobal.g_GameStop) return;


    //    foreach (JProperty v in jj.Properties())
    //    {
    //        if (m_bDebug)
    //        {
    //            print(string.Format("-{0}-", v.Name));
    //        }

    //        if (v.Value.Type == JTokenType.Object)
    //        {
    //            if (OrderFunction(v.Name))
    //            {
    //                if(m_bDebug)
    //                {
    //                    print(string.Format("{0} 실행", v.Name));
    //                }

    //                RunDir((JObject)v.Value);
    //            }
    //            else
    //            {
    //                if (m_bDebug)
    //                {
    //                    print(string.Format("{0} 패스", v.Name));
    //                }

    //            }
    //        }

    //    }

    //}

    #endregion
    #region 코루틴

    Vector3 m_vPos = Vector3.zero;
    Vector3 m_vCheckPos=Vector3.zero;
    float m_fCheckYaw=0;
    void SendMove(Vector3 vPos,float fYaw)
    {
        m_vPos = vPos;
        if (CWGlobal.g_bSingleGame)
        {

            m_gParent.SetPos(vPos);
            m_gParent.SetYaw(fYaw);
            return;
        }
        if (CWUdpManager.Instance && CWAIOwnerManager.Instance.IsMyOwner(m_gParent.m_nID))
        {
            float fDist = Vector3.Distance(vPos, m_vCheckPos);
            float fy =Mathf.Abs(fYaw - m_fCheckYaw);

            if(fDist>=3||fy>10)
            {
                CWUdpManager.Instance.SendMoveData(m_gParent.m_nID, vPos, fYaw);
                m_vCheckPos = vPos;
                m_fCheckYaw = fYaw;

            }

        }

    }
    Vector3 GetPosition()
    {
        if (CWGlobal.g_bSingleGame)
        {
            return m_gParent.GetPosition();
        }
        return m_vPos;
    }
    IEnumerator HeightRun()
    {
        while(true)
        {
            if (!m_gEnemy.IsDrone()) break;
            if (m_gEnemy == null) break;
            if (m_gEnemy.GetHP() <= 0) break;
            if(Mathf.Abs(GetPosition().y- m_gEnemy.GetPosition().y) <5)
            {
                break;
            }
            float hh = m_gEnemy.GetPosition().y - 8f;
            if (hh < 10) hh = 10f;

            int height = CWMapManager.SelectMap.GetHeight((int)GetPosition().x, (int)GetPosition().z) + 10;
            if (hh <= height)
            {
                hh = height;
            }

            Vector3 vv = GetPosition();
            vv.y = Mathf.Lerp(vv.y, hh, Time.deltaTime * 5f);

            
            SendMove(vv, m_gParent.GetYaw());


            yield return null;
        }
    }
    //IEnumerator RunFunction()
    //{
        
    //    while (true)
    //    {
    //        if (CWHeroManager.Instance == null) break;

    //        if(m_gParent.GetHP()>0)
    //        {
    //            if (m_gEnemy)
    //            {
    //                CheckEnmey();
    //            }
    //            m_nRandomValue = Random.Range(0, 100);
    //            if (CWHeroManager.Instance.m_GameType == GAMETYPE.WAR)
    //            {
    //                RunDir((JObject)m_jData["AI"]);
    //            }

    //        }

    //        yield return new WaitForSeconds(m_fThinktime);

    //    }
    //}

    void DebugPrint(string szstr)
    {
        print(szstr);
    }

    IEnumerator RunDirCor(JObject jj)
    {
        foreach (JProperty v in jj.Properties())
        {
            if(!CWGlobal.g_GameStop)
            {
                if (m_bDebug)
                {
                    DebugPrint(string.Format("-{0}-", v.Name));

                }
                if (m_gParent.GetHP() > 0)
                {
                    if (v.Value.Type == JTokenType.Object)
                    {
                        if (m_FuctionList.ContainsKey(v.Name))
                        {
                            string strfunname = m_FuctionList[v.Name];
                            yield return StartCoroutine(strfunname);
                            if (m_bResult)
                            {
                                if (m_bDebug)
                                    DebugPrint(string.Format("{0} 성공", v.Name));

                                yield return StartCoroutine(RunDirCor((JObject)v.Value));
                            }
                            else
                            {
                                if (m_bDebug)
                                    DebugPrint(string.Format("{0} 실패", v.Name));
                            }

                        }
                    }

                }

            }
            //yield return new WaitForFixedUpdate();
            yield return null;



        }

    }

    IEnumerator RunFunction()
    {

        while (true)
        {
            if (CWHeroManager.Instance == null) break;
            if (!CWGlobal.g_bStopAI)
            {
                if (m_gParent.GetHP() > 0)
                {
                    m_nRandomValue = Random.Range(0, 100);
                    yield return StartCoroutine(RunDirCor((JObject)m_jData["AI"]));
                    
                }
            }
                     
            yield return null;


        }
    }
    IEnumerator AvoidMoveRun()
    {


        Vector3 v = transform.right;
        v = Random.insideUnitSphere;
        v.y = 0;

        float ff = Random.Range(15, 24);
        float fSpeed = Random.Range(m_Speed * 2, m_Speed * 4);
        m_vMove = GetPosition() + v * ff;

        Vector3 vStart = GetPosition();
        Vector3 vTarget = m_vMove;
        Vector3 vDir = vTarget - GetPosition();

        CWAirObject gMyUser = gameObject.GetComponentInParent<CWAirObject>();

        float fdist = Vector3.Distance(vTarget, GetPosition());
        m_bMoved = true;
        vDir.Normalize();
        ff = 0;
        if (fdist > 0)
        {

            gMyUser.RollRoate(v.x);

            while (true)
            {
                if (vDir == Vector3.zero) break;
                Vector3 vv = vStart + vDir * ff;

                float fHeight = GetPosition().y;
                vv.y = fHeight;
                if (m_gParent == null) break;
                //m_gParent.SetPos(vv);
                SendMove(vv, m_gParent.GetYaw());
                ff += Time.deltaTime * fSpeed;
                if (ff >= fdist)
                {
                    break;
                }
                yield return null;
            }
            gMyUser.RollRoateStop();
        }
        m_bMoved = false;

    }
    bool MoveObject()
    {
        

        Vector3 vStart = GetPosition();
        Vector3 vTarget = GetMoveVector();
        Vector3 vDir = vTarget - vStart;

        float fdist = Vector3.Distance(vStart, vTarget);
        float ff = Time.deltaTime * m_Speed;
        //if (fdist <= m_fMinDist)
        if (fdist <= ff)
        {
            return false;
        }
        vDir.Normalize();

        if (vDir == Vector3.zero) return false; ;
        
        Vector3 vv = vStart + vDir * ff;


        float fYaw = CWMath.GetLookYaw(GetPosition(), m_vMove);
        if (m_gEnemy != null)
        {
            fYaw = CWMath.GetLookYaw(GetPosition(), m_gEnemy.transform.position);
        }

        SendMove(vv, fYaw);

        
        return true;
    }
    IEnumerator MoveRun()
    {

        CWObject gMyUser = gameObject.GetComponentInParent<CWObject>();

        TurntoEnmey(m_vMove,null);
        m_bMoved = true;
        while (true)
        {

            if(!MoveObject())
            {
                break;
            }

          
            yield return null;
        }
        m_bMoved = false;
        
    }

    IEnumerator WaitTimeFunc(float fwait)
    {
        float ftime = 0;
        while(ftime<fwait)
        {
            ftime += Time.timeScale * Time.deltaTime;
            yield return null;
        }
    }
    
    // 타겟을 행해 회전을 한다

    IEnumerator TurnFucntion(Vector3 vTarget, EventDelegate.Callback fuc)
    {

        Vector3 vRot= transform.eulerAngles;
        Vector3 _Way = vTarget - GetPosition();
        float _Angle = Mathf.Atan2(_Way.x, _Way.z) * Mathf.Rad2Deg;

        // 회전각도 증가는 어느방향이 가까운가?
        float fdist = CWMath.GetAngleDist(vRot.y, _Angle);
        float fdir = CWMath.GetAngleDir(vRot.y, _Angle);

        

        float fRet = 0;
        // 0,1,2,3,4,
        while (true)
        {
            fRet += (Time.deltaTime * 100);
            vRot.y += (Time.deltaTime * 100)*fdir;
            if (Mathf.Abs(fdist) <fRet)
            {
                fuc?.Invoke();
                break;
            }
//            transform.eulerAngles = vRot;
            m_gParent.Rotate(vRot);
            yield return null;
        }
    }


    #endregion

    // Use this for initialization

}
