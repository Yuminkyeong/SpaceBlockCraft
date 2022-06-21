//#define BLOCKEXP // 블록경험치 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;
using CWEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using UnityEngine.UI;
using DG.Tweening;

public class CWHero : CWAirObject
{

    
    

    public bool m_bLaserFlag = false;// 레이저인가?

    public int m_nShootCount = 0; // 몇번 슈팅을 했는가? 튜토에 사용 

    public GameObject m_gHitTargetDummy;// 주인공 타겟 더미 

    public GameObject m_gBuildTarget;

    public CWObject m_gEnmey;// 현재 나하고 싸우는 적이다
    
    public GameObject m_kSmokeEffect;

    
    #region 맵 블록 캐치
    public override void TakeMapBlock(int x, int y, int z)
    {
        int nBlock = CWMapManager.SelectMap.GetBlock(x, y, z);
        CWGlobal.CheckBlock(nBlock);
        CWMapManager.SelectMap.DelGameData(x, y, z);
        

    }
    #endregion

    #region  위치
    public GameObject m_gCamPos;
    public GameObject m_gParming;
    public bool IsFarDist(Vector3 vPos)
    {
        if (CWGlobal.g_bSingleGame) return false;

        float fdist = CWMath.VectorDistSQ(transform.position, vPos);
        if(fdist>128*128)
        {
            return true;
        }
        return false;
    }

    public Vector3 GetParmingCheck()
    {
        return m_gParming.transform.position;
    }

    #endregion



    #region 초기화



    protected override void SetObjectType()
    {
        m_ObjectType = CWOBJECTTYPE.HERO;
    }

    private void OnEnable()
    {
       
        StartCoroutine("RunFunction");


    }

    private void Awake()
    {
        Instance = this;
        m_nLayer = GameLayer.Hero;
        if(m_gWarp)
        {
            m_gWarp.SetActive(false);
        }
        if (m_kSmokeEffect)
            m_kSmokeEffect.SetActive(false);
    }

    public static CWHero Instance;

    public void TutoWeaponSetting()
    {
        KPower.m_nDamage = 0;

        CWHeroManager.Instance.m_kWeaponSlot.Clear();
        for (int i=0;i< 10;i++)
        {
            WEAPONSLOT ws = new WEAPONSLOT();
            ws.DamageLv = 100;
            ws.SpeedLv = 100;
            ws.RangeLv = 100;
            CWHeroManager.Instance.m_kWeaponSlot.Add(ws);
        }

      

        KPower.FSpeed = 20;// 부스터 속도 
        KPower.FSpeedR = 20;// 부스터 속도 
        KPower.FSpeedL = 20;// 부스터 속도 
        WeaponSetting();
    }
    float GetWeaponSpeed(int nType,int nlevel)
    {
        //GetWeaponSpeed(nType);
        
        if (nType==1)// 총
        {
            //return 60;
            return CWTableManager.Instance.GetTableFloat("무기업그레이드 - 시트1", "기관총속도", nlevel);
        }
        if(nType==2)
        {
            return CWTableManager.Instance.GetTableFloat("무기업그레이드 - 시트1", "미사일속도", nlevel);
        }

        return 0;
    }
    public override void WeaponSetting()
    {
        KPower.m_nDamage = m_nADDDamage;// 추가 데미지 
        m_bLaserFlag = false;
        for (int i = 0; i < m_kWeapon.Count; i++)
        {
            m_kWeapon[i].m_nSlot = i + 1;
            int nLevel = m_kWeapon[i].m_nLevel;

            m_kWeapon[i].m_nDamage = CWHeroManager.Instance.GetWeaponDamage(m_nSelectWeaponType, nLevel);

            m_kWeapon[i].m_fSpeed = GetWeaponSpeed(m_nSelectWeaponType, nLevel);

            KPower.m_nDamage += m_kWeapon[i].m_nDamage;
            m_kWeapon[i].m_nBlockCount= 1;

            WEAPON ws = CWArrayManager.Instance.GetWeapon(m_kWeapon[i].m_ID);
            if(ws.nType==3)
            {
                m_bLaserFlag = true;
            }



        }



        KPower.FSpeed = CWHeroManager.Instance.GetSpeed();// 부스터 속도 
        KPower.FSpeedR = CWHeroManager.Instance.GetSpeedR();// 부스터 속도 
        KPower.FSpeedL = CWHeroManager.Instance.GetSpeedL();// 부스터 속도 

     
    }
    public override void Create(int nID)
    {

        
        KPower.FhpRate = 1;
        KPower.FEnRate = 1;
        UserType = USERTYPE.USER; // USER로 봄
        base.Create(nID);
#if BLOCKEXP
        StartCoroutine("RunBlockExp");
#endif
        m_bMeshCollider = true;
    }


    public override bool IsDrone()
    {
        return true;
    }

    protected override void HpEvent()
    {
        base.HpEvent();
    }
    IEnumerator RunFunction()
    {
        while (true)
        {
             Run();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void AIDelete()
    {
        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        GameObject.Destroy(ai);
    }

    public bool m_bAutoFlag = false;


    float GetAutoCooltime(int nvalue)
    {
        if (nvalue == 2) return 0.6f;
        if (nvalue == 3) return 0.4f;
        if (nvalue == 4) return 0.3f;
        if (nvalue == 5) return 0.2f;
        if (nvalue == 6) return 0.1f;
        if (nvalue == 7) return 0.05f;
        return 0;
    }
    int GetAutoSpeed(int nvalue)
    {
        if (nvalue == 2) return 40;
        if (nvalue == 3) return 80;
        if (nvalue == 4) return 100;
        if (nvalue == 5) return 140;
        if (nvalue == 6) return 160;
        if (nvalue == 7) return 200;
        return 280;
    }

    public void AutoParmingSpeed(int nvalue)
    {
        

        CWAIEntity ai =  gameObject.GetComponent<CWAIEntity>();
        ai.m_fCooltime = GetAutoCooltime(nvalue);//
        ai.m_Speed = GetAutoSpeed(nvalue);
    }


    public void BeginAIParming()
    {
       

        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai != null)
        {
            GameObject.Destroy(ai);
        }
        float fCooltime = 1.0f;
        ai = gameObject.AddComponent<CWAIEntity>();
        ai.m_fCooltime = fCooltime;
        ai.m_fThinktime = 0.01f;
        ai.m_Range = 24;
        ai.m_bFirstAttack = true;// 선공 
        ai.m_fSightRange = ai.m_Range - 2;// 사격범위 보다 작아야 한다
        ai.m_AITYPE = AITYPE.PASSIVE;
        ai.m_Speed = GetSpeed();
        ai.Create(this, CWHeroManager.Instance.m_kAIHeroParming.GetObject());

        m_bAutoFlag = true;
    }
    public void StopAIParming()
    {
        m_bAutoFlag = false;
        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai != null)
        {
            GameObject.Destroy(ai);
        }
      


    }
    int m_nBlokcCount = 0;
    public void CheckBlockCount()
    {
        m_nBlokcCount++;
    }
    public void BeginCheckBlock()
    {
        m_nBlokcCount = 0;
    }
    public bool CheckBlock(int nCount)
    {
        if (m_nBlokcCount > nCount) return true;
        return false;
    }


    // 프로그래스 바는 일딴 삭제
    protected override void MakeProgressBar()
    {
        
    }



    protected override void OnLoadEnd()
    {
        CWLib.SetGameObjectLayer(gameObject,12);
        FixPos();
        HpEvent();
        m_bLoadEnd = true;
        SetTag("Hero");

        //SphereCollider ss = gameObject.GetComponent<SphereCollider>();
        //if (ss == null)
        //{
        //    ss = gameObject.AddComponent<SphereCollider>();
        //}
        //ss.radius = m_vSize.x;
        //ss.center = m_vCenter;



    }
    #endregion


    #region 블록경험치


    float m_fBlockExp = 0;
    float m_fTempExp = 0;
    public void AddBlockExp(int nCount)
    {
#if BLOCKEXP
        m_fTempExp += nCount / CWGlobal.BLOCKEXPDELTA;
        if (m_fTempExp>1)
        {
            CWCoinManager.Instance.UseCoin(COIN.EXP, (int)m_fTempExp);
            m_fTempExp = 0;
        }

        m_fBlockExp += nCount/8f;
#endif
    }
#if BLOCKEXP
    IEnumerator RunBlockExp()
    {
        while(true)
        {
            yield return new WaitForSeconds(10f);
            if(m_fBlockExp > 20)
            {
                CWSocketManager.Instance.UseCoin(COIN.EXP, (int)m_fBlockExp, CWSocketManager.UseCoin_ResultFuc, "UseCoin_ResultFuc");
                m_fBlockExp = 0;
            }
        }
    }
#endif

#endregion

#region ACTION
    


    public override void AIShoot(bool bDetected, CWObject gTarget)
    {
        base.Shoot(true, gTarget.GetHitPos());
    }
    public void AutoShoot(Vector3 vTarget)
    {
        base.Shoot(true, vTarget);
    }
    public override void AIShootPos(bool bDetected, Vector3 vPos)
    {
        base.Shoot(true, vPos);
    }

    public override void Shoot(bool bDetected, Vector3 vTarget, GameObject gTarget)
    {
        if(IsDie())
        {
            return;
        }

        if (!m_bYudotan) gTarget = null;
        

        // 10초 안에 추락하거나, 다음발사 에너지 보다 작다면 슛금지
        //if ( m_fWeaponUseEnergy + m_fUseEnergy/3 >= KPower.GetEnergy())
        //{
        //    //MessageOneBoxDlg.Instance.Show("메세지", "연료가 부족합니다");
        //    HelpMessageDlg.Instance.Show(null, "연료가 부족합니다");
        //    return;
        //}
        m_nShootCount++;

      //  KPower.UpdateEnergy((-m_fWeaponUseEnergy));

        CWSocketManager.Instance.SendShootPOS(m_nID, vTarget,m_nSelectWeaponType);


        // 멀티라면 
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MULTI)// // 멀티라면
        {
            int damage = CWGlobal.MULTI_ATTACK / m_kWeapon.Count;
            foreach (var v in m_kWeapon)
            {
                v.Shoot(this, true, vTarget, gTarget, damage);

            }

        }
        else
        {
            base.Shoot(true, vTarget, gTarget);
            return;

        }



        



    }
    public override void BombShoot( Vector3 vTarget, int nBlockCount,string szBomb)
    {
        
       

        m_kWeapon[0].BombShoot(gameObject.tag,vTarget, nBlockCount, szBomb);

       
    }

    #region 레이저

    List<CWLaser> m_kLaserList = new List<CWLaser>();

    public void LaserStop()
    {
        foreach(var v in m_kLaserList)
        {
            v.Stop();
        }
        m_kLaserList.Clear();
    }
    public void AddLaser(CWLaser kLaser)
    {
        m_kLaserList.Add(kLaser);
    }

    #endregion





    public void OnMapDetect()
    {
     //   float ff = KPower.m_nHp * 0.005f;//1%씩 깍임
       // OnHit((int)ff);
      //  DetectBlock();
   
    }

  

#endregion
    
    
#region AUTOWORK // 자동 업그레이드 


    // 더 높은 아이템이 있는가?
    // 가격으로 한다 
    int FindUpInven(int nItem)
    {
        GITEMDATA nData = CWArrayManager.Instance.GetItemData(nItem);
        List<SLOTITEM> kInven= CWInvenManager.Instance.m_nInvenDB;
        foreach(var v in kInven)
        {
            GITEMDATA nData2 = CWArrayManager.Instance.GetItemData(v.NItem);
            if(nData.type ==nData2.type)// 같은 타입
            {
                if(nData.price < nData2.price)
                {
                    // 더 비싼 물건 있다 
                    return v.NItem;
                }
            }

        }
        return 0;

    }
    void DelInven(int nItem)
    {
        CWInvenManager.Instance.DelItem(nItem);
        
    }
   
   
    public void ChangeSlot(int nSlot, int nItem)
    {
        BlockData nData = new BlockData();
        nData.nBlock = (ushort)nItem;
        nData.nShape =0;
        m_kData[nSlot] = nData;
        m_bUpdated = true;


    }
    

    public void AutoShape(string szAirFile)
    {
        // 자동 모형바꾸기
        // 비행기 파일


    }

#endregion

    
#region AIRPOWER

    void ResetMusuk()
    {
        CWDebugManager.Instance.m_bMusuk = false;//3초간 무적 
    }
    public bool m_bYudotan;

    public void ResetHP()
    {
        m_gKiller = null;
        KPower.FhpRate = 1;
        KPower.FEnRate = 1;
        m_bDieFlag = false;
        m_kDelFile.Clear();
        m_bUpdated = true;
        MakeHPBlock();
        
        if (m_kSmokeEffect)
            m_kSmokeEffect.SetActive(false);

        //Rigidbody rr = gameObject.GetComponent<Rigidbody>();
        //if(rr!=null)
        //    Destroy(rr);
        // 능력치를 다시 세팅한다 
        WeaponSetting();
        if(m_gDieScreen)
            m_gDieScreen.SetActive(false);



        transform.rotation = new Quaternion();
        this.gameObject.GetComponent<AircraftAction>().ResetRotation(); //로테이션 리셋

    }

  
    public void ResetEnergy()
    {
        KPower.FEnRate = 1;
    }
    public override void CalPower()
    {
        base.CalPower();

        KPower.FSpeed = CWHeroManager.Instance.GetSpeed();
    }


#endregion
#region 죽음관련

    // 배틀맵에서
    public override void MultiMapDie()
    {
        


    }

    public GameObject m_gDieScreen;




    public override void SetDie()
    {
        if (m_bDieFlag) return;
        m_bDieFlag = true;

        m_kHpBar.value = 0;
        
        m_kDamageText.text = "0/" + KPower.m_nHp.ToString();

        Space_Map.Instance.m_bHelpFlag = true;


        if (!CWGlobal.g_bSingleGame)
        {
            string szKiller = "";
            if(m_gKiller)
            {
                szKiller = m_gKiller.name;
            }
            GamePlay.Instance.CloseMultiMap(szKiller);
            //월드에서 아웃 
            CWSocketManager.Instance.SendWorldClose();
            CWProductionRoot kRoot = CWResourceManager.Instance.GetProduction("HeroDie");
            kRoot.Begin();

            CWSocketManager.Instance.UpdateDayLog(DAYLOG.MultiDie);

        }
        Rigidbody rr = gameObject.AddComponent<Rigidbody>();
        if(rr!=null)
        {
            rr.mass = 1000f;
            rr.angularDrag = 0.5f;
            Destroy(rr, 2f);
        }
        

        m_bDieFlag = true;
        m_kHpBar.value = 0;
        m_kDamageText.text = "0";


        if (m_kSmokeEffect)
            m_kSmokeEffect.SetActive(true);
        if (m_gDieScreen)
            m_gDieScreen.SetActive(true);
        
    }
    public void SetResenHP()
    {
        m_bDieFlag = false;
        CWPower cp = gameObject.GetComponent<CWPower>();
        cp.FhpRate = 1;
        if (m_gDieScreen)
            m_gDieScreen.SetActive(false);
    }
    public bool WorkerRefrsh()
    {
        KPower.FhpRate += 0.02f;
        KPower.FEnRate += 0.02f;
        if(KPower.FhpRate>=0.5f)
        {
            m_bDieFlag = false;
            if (m_kSmokeEffect)
                m_kSmokeEffect.SetActive(false);

            return true;
        }
        return false;

    }


#endregion
    /*
#region 캡쳐관련

    public Camera m_gCaptureCamera;

    public void BeginCapture()
    {
        m_gCaptureCamera.gameObject.SetActive(true);
        CWLib.SetGameObjectLayer(m_gItemBody,14 );
        CWLib.SetGameObjectLayer(m_gBody, 14);

        transform.localScale = Vector3.one;

    }
    public void EndCapture()
    {
        m_gCaptureCamera.gameObject.SetActive(false);
        CWLib.SetGameObjectLayer(m_gItemBody, 12);
        CWLib.SetGameObjectLayer(m_gBody, 12);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    public void ZoomCameara(float ff)//0~ 1
    {
        Camera cc = m_gCaptureCamera;
        if (cc != null)
        {
            cc.fieldOfView = 10 + 100f * ff;
        }
    }

#endregion
*/    
#region  HP블록 관련

    class HPBLOCK
    {
        public int m_nItem;
        public int m_nHP;
        public int m_nMaxHP;
        public bool IsRepair()
        {
            if (m_nHP < m_nMaxHP) return true;
            return false;
        }
        public bool IsDestroy()
        {
            if (m_nHP <= 0) return true;
            return false;
        }
        public void Repair()
        {
            m_nHP = m_nMaxHP;

        }
    };

    Dictionary<int, HPBLOCK> m_kHPBlock = new Dictionary<int, HPBLOCK>();



#region 데미지 리액션 

    protected override void HitReAction()
    {
        if (m_bHitReAction) return;
        m_bHitReAction = true;
        if (m_gKiller == null) return;

        Vector3 v1 = m_gKiller.transform.position;// -10
        Vector3 v2 = transform.position;  // 5   
        Vector3 vdir = v2 - v1; // 때린자에서 맞은자 방향 
        vdir.Normalize();
        float fMaxdist = 0.02f; // 최대 밀리는 숫자 
        Vector3 vPos = v2 + fMaxdist * vdir;
        vPos.y = v2.y;
        transform.DOShakePosition(0.3f);
        Invoke("CloseReAction", 2);

        CWHitEffect.Instance.Begin();

        CWVibration.Vibrate(300);

    }

#endregion

    public override void SetDamage(int nHitter, int nDamage, float fhp)
    {
        base.SetDamage(nHitter,nDamage, fhp);

        HitCheck(nDamage);

    }
    protected override void CheckDie()
    {
        base.CheckDie();
    }

    protected override void OnHit(int nDamage)
    {

        if (m_bDieFlag) return;
        if (CWDebugManager.Instance.m_bMusuk) return;
        base.OnHit(nDamage);
        HitCheck(nDamage);
        

        if(!CWGlobal.g_bSingleGame)
        {
            if (GetHP() <= 0)
            {
                SetDie();
                return;
            }

        }

    }

    public override bool IsHeroTeam()
    {
        return true;
    }

    void MakeHPBlock()
    {
        return;

        if (KPower.m_nHp == 0) return;
        m_kHPBlock.Clear();
        List<int> kTemp = new List<int>();
        foreach (var v in m_kData)
        {
            kTemp.Add(v.Key);
        }
        kTemp.Sort(CampareSort);

        int nHp = 0;
        int nTotal = (int)GetHP();
        int nMaxHP = KPower.m_nHp;
        bool bflag = false;

        foreach (var v in kTemp)
        {

            HPBLOCK kdata = new HPBLOCK();
            kdata.m_nItem = m_kData[v].nBlock;
            GITEMDATA nData = CWArrayManager.Instance.GetItemData(kdata.m_nItem);

            if (nData.type != "shipblock") continue;

            kdata.m_nMaxHP = nData.hp;
            if(nMaxHP==nTotal)
            {
                kdata.m_nHP = nData.hp;
            }
            else
            {
                if (bflag)
                {
                    kdata.m_nHP = 0;
                }
                else
                {
                    nHp += nData.hp;
                    if (nHp >= nTotal && nHp <= nMaxHP)
                    {
                        kdata.m_nHP = nHp - nTotal;
                        bflag = true;
                    }
                    else
                    {
                        kdata.m_nHP = nData.hp;
                    }

                }

            }
            if(kTemp.Count - m_kHPBlock.Count <=10)//2개이상은 깨지 않게
            {
                break;
            }
            m_kHPBlock.Add(v, kdata);

        }


    }

    public void AddHpBlock(int num,int nItem)
    {
        HPBLOCK kData = new HPBLOCK();
        GITEMDATA nData = CWArrayManager.Instance.GetItemData(nItem);
        kData.m_nHP = nData.hp;
        kData.m_nMaxHP = nData.hp;
        m_kHPBlock.Add(num, kData);
    }
    public override void DelHpBlock(int num)
    {
        m_kHPBlock.Remove(num);
    }

    protected override void ConvertHPBlock()
    {
       // 일딴 안함
        MakeHPBlock();
        m_bUpdated = true;

    }
    

    // 골드
    public bool IsRepairBlock(int num)
    {
        if (m_kHPBlock.ContainsKey(num))
        {
            return m_kHPBlock[num].IsRepair();
        }
        return false;
    }

    public bool IsDestroyBlock(int num)
    {
        if (m_kHPBlock.ContainsKey(num))
        {
            return m_kHPBlock[num].IsDestroy();
        }
        return false;
    }
    public void RepairBlock(int num)
    {
        if (m_kHPBlock.ContainsKey(num))
        {
            m_kHPBlock[num].Repair();
            int hp = 0;
            foreach (var v in m_kHPBlock)
            {
                hp += v.Value.m_nHP;
            }
            KPower.SetHP(hp);
            

        }
        m_bUpdated = true;
        LoadObjectFunc();
    }
    
    public override bool IsDelFile(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        return IsDestroyBlock(num);
    }
#endregion

    


    public void UseEnergy(bool bStop)
    {
        if (CWDebugManager.Instance.m_bMusuk) return;

        if (bStop)
        {
        //    KPower.UpdateEnergy((-m_fUseEnergy * Time.deltaTime * 0.07f));
        }
        else
        {
        //    KPower.UpdateEnergy((-m_fUseEnergy * Time.deltaTime));
        }
        
        if(KPower.GetEnergy()==0)
        {
            // 추락 연출
            SetDie();

        }
    }

    public bool IsWeaponType(int nType)
    {
        foreach (var v in m_kWeapon)
        {
            if (v.m_nWeaponType == nType)
            {
                return true;
            }
        }
        return false;

    }
    

#region 워프관련
    public GameObject m_gWarp;
    public void SetWarp(bool bflag)
    {
        m_gWarp.SetActive(bflag);
    }
#endregion

    

    public bool IsShow()
    {
        return m_gCenterObject.activeInHierarchy;
    }


    public void Show(bool flag)
    {
        CWChHero.Instance.Show(false);
        ShowChar(true);
        GameObject gg = CWGlobal.FindObject(szHpBar);
        m_kHpBar = gg.GetComponent<Slider>();

        gg = CWGlobal.FindObject(szDamageText);
        m_kDamageText = gg.GetComponent<Text>();

        
        if (m_gDieScreen)
            m_gDieScreen.SetActive(false);
        if (flag)
        {
        
            CWLib.SetGameObjectLayer(gameObject, LayerMask.NameToLayer("Hero"));
            transform.DOKill();

            

        }
        else
        {
        
            transform.position = Vector3.zero;
            CWLib.SetGameObjectLayer(gameObject, LayerMask.NameToLayer("Edit"));
           
        }
        

        m_gCenterObject.SetActive(flag);
        
        m_kDamageText.text = KPower.GetHP().ToString() + "/" + KPower.m_nHp.ToString();
        m_kHpBar.value = KPower.FhpRate;
        if (flag==false)
        {
            Game_App.Instance.g_bDirecting = true;
        }
        StopAIParming();
        



    }


    protected override void BeginHPBar()
    {
        
    }
    protected override void DeleteFile()
    {
        base.DeleteFile();
    }
    public GameObject GetHeroHitTarget()
    {
        return m_gHitTargetDummy;
    }

#region 데미지 표현

    public string szDamageText;
    public string szHpBar;
    Text m_kDamageText;
    Slider m_kHpBar;
    
    // 물약을 먹는다
    
    public void SetFullHP()
    {
        int nValue = KPower.m_nHp - KPower.GetHP();
        SetHPRate(1);
        m_kDamageText.text = KPower.GetHP().ToString() + "/" + KPower.m_nHp.ToString();
        m_kHpBar.value = KPower.FhpRate;

        CWDemageManager.Instance.ShowHpText( nValue.ToString(),Color.cyan);
    }
    void HitCheck(int nDamage)
    {
        m_kDamageText.text = KPower.GetHP().ToString() + "/"+ KPower.m_nHp.ToString();

        m_kDamageText.transform.DOPunchScale(new Vector3(2, 2, 1), 0.5f).OnComplete(() => {
            m_kDamageText.transform.localScale = Vector3.one;
        });
        m_kHpBar.value = KPower.FhpRate;

        CWDemageManager.Instance.ShowDamage(Game_App.Instance.m_gUIDir.transform,"-" +nDamage.ToString());

    }



#endregion

    //int m_nAddEnergyValues = 0;
    //protected override void DamageRun()
    //{
    //    base.DamageRun();
    //    if(m_nAddEnergyValues>0)
    //    {
    //        TakeEnergyPlay();
    //        m_nAddEnergyValues = 0;
    //    }
    //}
    //void TakeEnergyPlay()
    //{
    //    Color kColor = Color.yellow;
    //    GameObject gg = CWPoolManager.Instance.GetObject("EnergyAddText", 20);
    //    if (gg)
    //    {
    //        gg.name = "EnergyAddText";
    //        Vector3 vPos = m_vCenter;
    //        vPos.y += (m_vSize.y + 10);
    //        gg.transform.SetParent(m_gTempdir.transform);
    //        gg.transform.localPosition = vPos;
    //        gg.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
    //        gg.tag = gameObject.tag;

    //        DamageText dd = gg.GetComponentInChildren<DamageText>();
    //        dd.Begin(string.Format("+{0}", m_nAddEnergyValues), kColor);
    //    }


    //}

    //public void TakeEnergyText(int nValue)
    //{
    //    m_nAddEnergyValues += nValue;
    //}

    public Vector3 GetDetectPos()
    {
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        if (array.Length == 0) return Vector3.zero;
        return array[0].gameObject.transform.position;
    }
#region Enemy target Cursor

    
//    public GameObject m_gEnemyTargetBox;
  //  public GameObject m_gTargetDummy;
    //public GameObject GetEnemyCursor()
    //{
    //    return m_gTargetDummy;
    //}
    //void InitTargetCursor()
    //{
    //    m_gEnemyTargetBox.transform.parent= m_kWeapon[0].gameObject.transform;
    //    m_gEnemyTargetBox.transform.localPosition = new Vector3(0,0,64);
    //}

    //public void ShowTargetCursor(bool bflag)
    //{
    //    m_gEnemyTargetBox.SetActive(bflag);
    //}
    

#endregion


#region 설계도 카피 

    public void CopyUserAir(CWAirObject kObject)
    {
        // 모든 파일을 인벤토리로 이동 
        foreach(var v in m_kData)
        {
            CWInvenManager.Instance.AddItem(v.Value.nBlock, 1);
        }
        m_kData.Clear(); // 모두 삭제 

        foreach (var v in kObject.GetData())
        {
            //int n = CWInvenManager.Instance.FindUplevelItem(v.Value.nBlock);
            //if(n==0)
            //{
            //    // 그냥 나무로 대체 한다, HP 1로 하기 때문에 문제가 없다 
            //    n = (int)GITEM.tree; 
            //}
            //else
            //{
            //    CWInvenManager.Instance.DelItem(n);
            //}\

            int nBlock = v.Value.nBlock;

            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nBlock);
            if(gData.type=="shipblock")
            {
                nBlock = (int)GITEM.tree;
            }


            int x = GetSellX(v.Key);
            int y = GetSellY(v.Key);
            int z = GetSellZ(v.Key);
            SetBlock(x, y, z, nBlock, v.Value.nShape, v.Value.nColor);

        }
        CalPower();
        CallSize();

        m_bUpdated = true;
    }

    #endregion




    public override int m_nGrade
    {
        get
        {
            return CWHeroManager.Instance.m_nGrade;
        }
    }


    public override int GetGrade()
    {
        return CWHeroManager.Instance.m_nGrade;
    }


    public override void Load(string szName)
    {
        string name2 = name;
        base.Load(szName);

        name = name2;// 원래 이름으로 복원해야 한다!
    }
    // 카메라가 주위를 돈다
    public  void CameraAround()
    {
        CWProductionRoot pt = CWResourceManager.Instance.GetProduction("HeroRound");
        pt.Begin();
    }



    public void CheckEquip()
    {
        int tcnt1 = CWHeroManager.Instance.m_nWeaponCount;
        int tcnt2 = GetWeaponItemCount();
        int bcnt1 = CWHeroManager.Instance.m_nBusterCount;
        int bcnt2 = GetBusterItemCount();

        if(tcnt1<tcnt2)
        {
            CWHeroManager.Instance.m_nWeaponCount = tcnt2;
            CWSocketManager.Instance.UpdateUser("WeaponCount", CWHeroManager.Instance.m_nWeaponCount.ToString());
        }
        if (bcnt1 < bcnt2)
        {
            CWHeroManager.Instance.m_nBusterCount = bcnt2;
            CWSocketManager.Instance.UpdateUser("BusterCount", CWHeroManager.Instance.m_nBusterCount.ToString());
        }


    }

    #region 적 설정관련
    public CWObject m_gEnemy=null;
             

    public void SetEnemy(CWObject kObject)
    {
        m_gEnemy = kObject;
    }
    public CWObject GetEnemy()
    {
        return m_gEnemy;
    }
    #endregion

  
    // 히든 블록 얻음
    public void TakeHiddenBlock()
    {

    



    }


    // 방어물이 있는가?

    public bool  CheckMove(Vector3 vPos)
    {
        if(vPos.x < 0) return false;
        if(vPos.x > 256) return false;
        if(vPos.z < 0) return false;
        if(vPos.z > 256) return false;
        if (vPos.y > 255) return false;
        if (vPos.y < 10) return false;


        return true;
    }




    bool m_bWait = false;
    public void DetectBlock()
    {
        //if (m_bWait) return;

        //m_bWait = true;
        Vector3 vPos = transform.position - transform.forward;

        transform.DOMove(vPos, 0.01f).OnComplete(() =>
        {
        //    m_bWait = false;
        });
    }
    // 

    private void Update()
    {
        //Vector3 vPos = CWHero.Instance.GetPosition();
        //Vector3 vStart = new Vector3(vPos.x + 0.5f, vPos.y + 32, vPos.z + 0.5f);
        //Vector3 vEnd = new Vector3(vPos.x + 0.5f, vPos.y -100, vPos.z + 0.5f);
        //Vector3 vDir = new Vector3(0, -1, 0);
        //Debug.DrawRay(vStart,vDir);
        //Debug.DrawLine(vStart, vEnd);

    }


    #region 캐릭터 

    GameObject m_gCharParent;
    CharBody m_gCharBody;
    /*
    public override void CheckCharAddBlock()
    {
        if (!CheckCharBlock())
        {

            for (int i = 0; i < 30; i++)
            {
                int y = 31 - i;
                if (GetBlock(16, y, 16) > 0)
                {
                    int yy = y + 1;
                    AddBlock(16, yy, 16, (int)GITEM.charblock);
                    
                    break;
                }
            }

        }
    }
    */
    public override void SetYaw(float fYaw)
    {

        
        base.SetYaw(fYaw);
    }
    public override void SetPos(Vector3 vPos)
    {
        
        base.SetPos(vPos);
    }
    public void ShowChar(bool bflag)
    {

        if (m_gCharParent == null) return;

   //     Vector3 vPos = GetPosition();
     //   vPos.y = 85;
     ///   SetPos(vPos);

        //Rigidbody rr = gameObject.AddComponent<Rigidbody>();
        //if (bflag)// 캐릭터가 보임
        //{
        //    rr.isKinematic = false;// 충돌체크해야됨
        //}
        //else// 캐릭터가 안보임
        //{
        //    rr.isKinematic = true;// 충돌체크 할 필요없음
        //}
        m_gCharParent.SetActive(bflag);
    }

    protected override GameObject CharblockAttach()
    {

        GameObject gg = new GameObject();
        CharBody cb = CWResourceManager.Instance.GetCharBody(CWHeroManager.Instance.m_nCharNumber);
        cb.transform.parent = gg.transform;
        cb.transform.localPosition = new Vector3(0, -1.3f, 0);
        cb.transform.localRotation = new Quaternion();
        BoxCollider bb = gg.AddComponent<BoxCollider>();
        bb.size = Vector3.one;
        bb.isTrigger = true;
        m_gCharBody = cb;
        m_gCharParent = gg;
        return gg;
    }
    public void SetRandomDance()
    {
        if (m_gCharParent == null) return;
        if (m_gCharParent.activeSelf == false) return;
        m_gCharBody.SetRandomDance();
    }

    #endregion


    public override Vector3 GetMoveDir()
    {
        AircraftAction aa = GetComponent<AircraftAction>();

        return aa.GetDir();



    }

    protected override void SetHitDummy()
    {
        if (m_gHitDummy)
        {
            Destroy(m_gHitDummy);
        }
        m_gHitDummy = new GameObject();
        m_gHitDummy.transform.parent = m_gCenterObject.transform;
        m_gHitDummy.transform.localPosition = m_vCenter;
        m_gHitDummy.name = "HitDummy";
        SphereCollider ss = m_gHitDummy.GetComponent<SphereCollider>();
        if (ss == null)
        {
            ss = m_gHitDummy.AddComponent<SphereCollider>();
        }
        ss.radius = 1.5f;
        ss.isTrigger = true;

    }

    public override int BonusDamage()
    {
        if(CWGlobal.g_bDamageDouble)
        {
            return 2;
        }
        return 1;
    }

}
