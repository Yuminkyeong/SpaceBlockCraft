using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;
using TMPro;

public class GamePlay : PageUI<GamePlay>
{
    public GameObject[] m_gDiggObject;// 블록캐기 관련 
    //public GameObject[] CharModeObject;



    protected override int GetUINumber()
    {
        return 2;
    }
    
    void SetLogDB()
    {
        if (m_nWType == WTYPE.SINGLE)
            BaseUI.g_kOpenList.Add(101);
        if (m_nWType == WTYPE.MULTI)
            BaseUI.g_kOpenList.Add(103);
        if (m_nWType == WTYPE.PVP)
            BaseUI.g_kOpenList.Add(104);


    }

    

    public bool m_bTutoMulti = false;// 투토리얼 멀티이다, 예외처리 변수로 활용한다
    

    public enum WTYPE {SINGLE,MULTI,PVP,MYROOM };
    public WTYPE m_nWType;

    public GameObject[] m_gWar;
    public GameObject[] m_gMulti;
    public GameObject[] m_gSingle;

    public GameObject m_gMission;
    public Text m_kMissionText;

    public RectTransform m_gShoot;


    public GameObject m_kShootImage;
    public GameObject m_gSelectShoot;


    public Image m_CoolTime;
    public Image m_CoolTimejump;

    


    public Text m_kTimerText;
    public float m_Playtime=100;

    public bool m_bStarted = false;


    float m_fStartUseTime;

    CWProductionRoot m_kProduction=null;

    //public 

    CWArrayManager.StageData m_kStage;


    public bool m_bTargetViewON = false;// 타겟뷰를 보여준다


    int _KillCount = 0;
    
    public int m_nKillCount
    {
        get
        {
            return _KillCount;
        }
        set
        {
            _KillCount = value;

        }
    }
    #region 캐릭터 전환

    bool _charmode = false;
    public bool CharMode
    {
        get
        {
            
            return _charmode;
        }
        set
        {
            Game_App.Instance.m_gSelectBlock.SetActive(value);
            _charmode = value;
        }
    }
    protected override void _Open()
    {
        base._Open();
    }

    public void OnCharmode()
    {
        CharMode = !CharMode;
        if(CharMode)
        {

            Vector3 vPos = Vector3.zero;
            // 오브젝트 검사
            Vector3 vCenter = new Vector3(128, 0, 128);
            float fdist = 10;
            Vector3 vdir = vCenter - CWHero.Instance.GetPosition();
            vdir.Normalize();
            vPos = CWHero.Instance.GetPosition() + vdir * fdist;
            CWChHero.Instance.Charic_OUT(vPos);
            CWHero.Instance.ShowChar(false);

            CWMapManager.Instance.SetBoundSize(1.0135f);


        }
        else
        {
            CWMapManager.Instance.SetBoundSize(1.5f);
            CWChHero.Instance.Charic_IN();
            CWHero.Instance.ShowChar(true);
        }
    }
    public void OnAirmode()
    {
        CharMode = false;
    }


    #endregion




    #region 전투 정보 




    #endregion
    // 전투가 끝났는가?

    bool m_bEndWar = false;
    public bool IsEndWar()
    {

        return m_bEndWar;
    }
    // 게임 시작 
    public bool IsGamePlay()
    {
        if (CWGlobal.g_GameStop) return false;
        return m_bShow;
    }
    public bool IsMultiPlay()
    {
        if (!IsShow()) return false;
        if (m_nWType != WTYPE.MULTI) return false;
        return true;
    }
    public void BeginOpen()
    {
        
        CharMode = false;// 무조건 초기화 해야 한다!!
        CWGlobal.g_GameStop = false;
        CWBombManager.Instance.ResetObject();

        bool bflag = false;

        if (m_nWType == WTYPE.SINGLE)
        {
            bflag = CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID());

            m_Playtime = CWGlobal.WAR_TIME;
        }
        if (m_nWType==WTYPE.MYROOM)
        {
            bflag = true;
        }
        if (m_nWType == WTYPE.PVP)
        {
            bflag = false;
            m_Playtime = CWGlobal.PVP_TIME;
        }

        foreach (var v in m_gDiggObject)
        {
            if (v) v.SetActive(bflag);
        }

        CWHero.Instance.m_bYudotan = false;
        
        Game_App.Instance.m_bTuboFlag = false;
        if (CWGlobal.g_bGameBegin)
        {
            // 비기너는 자동 유도탄으로 
            if (m_nWType == WTYPE.SINGLE)
                CWHero.Instance.m_bYudotan = true;
        }

        
        
        
        SetLogDB();
        ///CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID())
        if (CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID()))// 적이 없다면
        {
            CWBgmManager.Instance.PlayDigg();
        }
        else
        {
            CWBgmManager.Instance.PlayWar();
            
        }
        
        m_kStage = CWArrayManager.Instance.GetStageData(Space_Map.Instance.GetStageID());

        CWDebugManager.Instance.m_bMusuk = false;


        foreach (var v in m_gWar)
        {
            if (v != null) v.SetActive(false);
        }
            
        foreach (var v in m_gMulti)
        {
            if(v!=null)         v.SetActive(false);
        }
            
        foreach (var v in m_gSingle) v.SetActive(false);

        if (m_nWType == WTYPE.SINGLE)
        {

            CWSocketManager.Instance.SendFirstData(13);// 싱글
            foreach (var v in m_gWar)
            {
                if (v != null) v.SetActive(true);
            }
                
            
        }
        else if (m_nWType == WTYPE.MULTI)
        {
            CWSocketManager.Instance.SendFirstData(14);// 멀티

            m_fStartUseTime = Time.time;


            RenderSettings.skybox = Game_App.Instance.m_kSkyMat[0];
        }


        if (CWGlobal.g_bSingleGame)
        {
            foreach (var v in m_gSingle)
            {
                if (v != null) v.SetActive(true);
            }
                
        }
        else
        {
            foreach (var v in m_gMulti)
            {
                if (v != null) v.SetActive(true);
            }

        }






        m_nKillCount = 0;// 드론 사망 개수 


        
       
        m_CoolTime.fillAmount = 0;
        m_CoolTimejump.fillAmount = 0;
        


        if (!m_bShow)   Open();



        if(m_nWType == WTYPE.MYROOM)
        {
            CWHero.Instance.SetPos(new Vector3(64   , 32, 64));
            OnCharmode();

            return;
        }

        if (m_nWType == WTYPE.PVP || m_nWType == WTYPE.MULTI) return;


        StartCoroutine("IPlay");

        CWMapManager.Instance.SetBoundSize(1.5f);
        
    }

    void HeroReady()
    {


        // 중요 , X 축 중앙 Z 축 약간 위
        // 거리 고정!!!!
        int sx = CWMapManager.SelectMap.WORLDSIZE / 2;

        Vector3 vPos = new Vector3(sx, CWGlobal.START_HEIGHT, CWGlobal.START_Z);
        CWHero.Instance.SetPos(vPos);
        CWHero.Instance.SetEnemy(null);

        CWHero.Instance.gameObject.SetActive(true);
        CWHero.Instance.Show(true);
        CharMode = false;// 비행기 모드 

        CWHero.Instance.ResetHP();


        Game_App.Instance.g_bDirecting = false;
        CWGlobal.g_bStopAI = false;
        CWMapManager.Instance.ReStartMask();

        

    }
  
    





    public override void OnExit()
    {
        CWGlobal.g_bDamageDouble = false;
        CharMode = false;
        if(m_nWType==WTYPE.PVP)
        {
            PVPDlg.Instance.OnExit();
            return;
        }
        Space_Map.Instance.Show(100);

        if (m_nWType == WTYPE.MYROOM)
        {
            CWHeroManager.Instance.SaveMyPlanet();
        }
        else if (m_nWType == WTYPE.SINGLE)
        {
            CWMapManager.Instance.m_kSelectMap.SaveLocalData();
        }
        

        if (m_nWType == WTYPE.SINGLE || m_nWType == WTYPE.MYROOM)
        {
            CWGalaxy.Instance.UpdateSelectPlanet();
        }

        

    }
    public void GoStage()
    {
       
        
        Camera.main.fieldOfView = 60;
        m_bStarted = false;
        CWMobManager.Instance.Clear();
        CWHero.Instance.Show(false);
        Space_Map.Instance.Show(100);

    }
    public void GoStageByTuto()
    {

        
        Camera.main.fieldOfView = 60;
        m_bStarted = false;
        CWMobManager.Instance.Clear();
        CWHero.Instance.Show(false);


        Close();

        Space_Map.Instance.Show(100);
    }

    
    IEnumerator IHelpparming()
    {
        yield return new WaitForSeconds(1f);
        TutoMissionDlg.Instance.Open();

        yield return new WaitForSeconds(10f);
        TutoMissionDlg.Instance.Close();
    }


    float m_fPastTime;
    public void StartWarMessage()
    {

        m_gMission.transform.DOLocalMove(new Vector3(0, 159, 0), 0).OnComplete(() => {

            m_gMission.transform.DOScale(4, 0).OnComplete(() => {

                m_gMission.transform.DOScale(1, 1f).OnComplete(() => {

                    m_gMission.transform.DOLocalMove(new Vector3(-727, 253.2f, 0), 0.5f).OnComplete(() => {

                        m_gMission.transform.DOLocalMove(new Vector3(-727, 253.2f, 0), 4f).OnComplete(() => {

                            //m_gMission.transform.DORotate(new Vector3(0, 90, 0), 0.3f).OnComplete(() => {


                            //});

                        });

                    });
                });
            });

        });
    }
    
    void ParmainStart()
    {
        OnCharmode();
    }
    IEnumerator IPlay()
    {
        m_bEndWar = false;
        yield return null;
        // 적이 존재


        if (!CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID()))
        {
            CWMobManager.Instance.StagePlay();// 적 출현
            Vector3 vPos=Vector3.zero;
            //if (FindTarget(ref vPos))
            //{
            //    m_gAim.transform.localEulerAngles
            //}


            bool bexit = false;
            float fStarttime = Time.time;
            float PlayTime = m_Playtime;
            float fstart= fStarttime;
            float fRestTime=0;
            while (true)
            {
                while (CWGlobal.g_GameStop)
                {
                    PlayTime =  m_Playtime - fRestTime;
                    fstart = Time.time;
                    yield return null;
                }
                fRestTime = (Time.time - fstart);
                float ff = PlayTime - fRestTime;
                
                m_kTimerText.text = CWLib.GetTimeString(ff);
                if(ff<=0)
                {
                    if(!CWHeroManager.Instance.m_bTuto)
                    {
                        FailStageDlg.Instance.Show(Close);
                        break;
                    }
                    else
                    {
                        CWMobManager.Instance.AllKill();
                        CWUserManager.Instance.Clear();
                        bexit = true;
                    }


                }
                

                if(CWHero.Instance.IsDie())
                {
                    //NoticeMessage.Instance.Show("사망했습니다 ");

                    yield return new WaitForSeconds(0.8f);
                    // 내구력을 줄인다
                    CWHeroManager.Instance.m_nRepairValue -= 20;
                    if (CWHeroManager.Instance.m_nRepairValue < 0) CWHeroManager.Instance.m_nRepairValue = 0;
                    CWSocketManager.Instance.UpdateRepairAir(CWHeroManager.Instance.m_nAirSlotID, CWHeroManager.Instance.m_nRepairValue);

                    //MessageOneBoxDlg.Instance.Show(OnExit, "사망", "사망했습니다.");

                    DieMessageDlg.Instance.ShowSingle(OnExit);

                    CWMobManager.Instance.ClearProcuction();
                    CWGlobal.g_GameStop = true;

                    CWSocketManager.Instance.UpdateDayLog(DAYLOG.StoryDie);

                    break;
                    
                }
                if (bexit|| CWMobManager.Instance.IsAllKill())
                {
                    foreach (var v in m_gDiggObject) if (v) v.SetActive(true);

                    // 보스 전투인가?
                    if (Space_Map.Instance.IsBossWar())
                    {

                        int nStage = 0;
                        nStage = Space_Map.Instance.GetStageID();
                        int nKey = ((nStage - 1) / 6) + 1;
                        string szProduction = CWTableManager.Instance.GetTable("스테이지 - 보스", "dieproduction", nKey);
                        CWProductionRoot pt = CWResourceManager.Instance.GetProduction(szProduction);
                        pt.Begin(() => {
                            StageResultDlg.Instance.Show(ParmainStart);
                        });
                    }
                    else
                    {
                        StageResultDlg.Instance.Show(ParmainStart);
                    }

                    CWHeroManager.Instance.m_nStageNumber++;// 앞으로 전진

                    Dailymission.Instance.CheckUpdate(DAYMTYPE.PLANET, 1);

                    CWSocketManager.Instance.UpdateUser("Stage", CWHeroManager.Instance.m_nStageNumber.ToString());


                    CWBombManager.Instance.ResetObject();

                    CWGlobal.g_GameStop = true;
                    break;
                }

                yield return new WaitForSeconds(0.2f);
            }

        }
        else
        {

            if(CWHeroManager.Instance.m_nStageNumber<=2)
            {
                StartCoroutine("IHelpparming");
            }

            CWHero.Instance.SetPos(new Vector3( 128,64,128));
            OnCharmode();
        }
        CWUserManager.Instance.Clear();

        m_bEndWar = true;
    }

    // 사용안함
    public bool Boss_OK()
    {
        return true;
    }
    // 사용안함
    public bool MiddleBoss_OK()
    {
        return true;
    }

    // 남아 있는 시간 
    public float GetRespPlayTime()
    {
        return m_fPastTime;
    }
    protected override void _Close()
    {
        
        CWUserManager.Instance.Clear();


        CWGlobal.g_nSpeedMul = 1;
        m_nMultiCount = 0;
        
        CWResourceManager.Instance.CloseTempDir();

        if (!CWGlobal.g_bSingleGame)
        {
            CWSocketManager.Instance.SendWorldClose();
            CWGlobal.g_bSingleGame = true;
            CWUdpManager.Instance.CloseUDP();
            
        }

        CWUserManager.Instance.Clear();
        




        base._Close();
    }

    public void UpdateInven()
    {
        
    }
    public override void Close()
    {
        CWGlobal.g_bDamageDouble = false;
        CharMode = false;// 무조건 초기화 해야 한다!!
        if (m_kProduction!=null)
        {
            m_kProduction.Close();
            m_kProduction = null;
        }
        base.Close();
    }
    // 재도전
    public void ReShow()
    {
        m_nWType = WTYPE.SINGLE;
        CWGlobal.g_bSingleGame = true;
        BeginOpen();
        CWGlobal.g_bWarmode = true;
        CWInvenManager.Instance.CBUpdateEvent = UpdateInven;
        

    }

    


    float m_fStartCooltime = 0;
    float GetCooltime()
    {
        if(CWHero.Instance.m_nSelectWeaponType==1)
        {
            return 0.2f;
        }
        if (CWHero.Instance.m_nSelectWeaponType == 2)
        {
            return 0.5f;
        }

        return 0.5f;
    }
    public bool IsCooltime()
    {
        float fCooltime = CWGlobal.COOLTIME;
        float ft = Time.time - m_fStartCooltime;

        if (ft < fCooltime) return true;

        return false;
    }
    // 버튼 

    void _Shoot(Vector3 vPos)
    {
        if (IsCooltime()) return; // 쿨타임 적용 
        m_gShoot.DORotate(new Vector3(0, 0, 30), 0.2f).SetLoops(2, LoopType.Yoyo);

        
        
        CWHero.Instance.Shoot(true, vPos,null);
        m_CoolTime.fillAmount = 1;
        m_CoolTime.DOFillAmount(0, CWGlobal.COOLTIME);
        m_fStartCooltime = Time.time;
        


        if (CWHeroManager.Instance.GetWeaponDamageLevel(1) == 1)
        {
            if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.SINGLE)
            {
                TipMessageDlg.Instance.Show("무기의 공격력을 늘려보세요~");
            }


        }


    }
    private void Update()
    {
        if (!IsShow()) return;
        if (Input.GetAxisRaw("Shoot")==1)
        {
            OnShootClick();
        }

    }

    #region Aim 작업
    public GameObject m_gAim;
    public float RadiusTest = 10;
    bool FindTarget(ref Vector3 vPos)
    {

        int layer1 = 1 << LayerMask.NameToLayer("Detect");
        int nMask = layer1;//| layer2;
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        //public static bool SphereCast(Ray ray, float radius, [Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction);
        if (Physics.SphereCast(ray, RadiusTest, out hit, Mathf.Infinity, nMask))
        {
            // 해당 오브젝트 선택한 이펙트 나와야 함
            vPos = hit.point;

            return true;
        }
        return false;

    }

    // 가장 가까운 블록 리턴
    Vector3 FindNearBlock(Vector3 vPos)
    {
        int x = 0, y = 0, z = 0;
        int sx = (int)vPos.x;
        int sy = (int)vPos.y;
        int sz = (int)vPos.z;
        for (int i = 0; i < 100; i++)
        {
            CWSphereData.GetData(i, ref x, ref y, ref z);
            int nBlock = CWMapManager.SelectMap.GetBlock(sx + x, sy + y, sz + z);
            if (nBlock > 0)
            {
                vPos.x = sx + x;
                vPos.y = sy + y;
                vPos.z = sz + z;
                return vPos;
            }
        }
        return Vector3.zero;
    }
    Vector3 m_vPrePos= Vector3.zero;
    bool FindBlock(ref Vector3 vPos)
    {
        int layer1 = 1 << LayerMask.NameToLayer("BlockMap");
        int nMask = layer1;//| layer2;
        RaycastHit hit;
        //        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 vRet = CWMath.ConvertMousePos(vPos);
        if (Physics.Raycast(Camera.main.ScreenPointToRay(vRet), out hit, Mathf.Infinity, nMask))
            //if (Physics.SphereCast(ray,1f, out hit, 10f, nMask))
        {
            vPos = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);
            vPos= FindNearBlock(vPos);
            // 선택한 블록이 보여야 함
            m_vPrePos = vPos;
            return true;
        }
        else
        {
            if (m_vPrePos != Vector3.zero)
            {
                float fdist = Vector3.Distance(m_vPrePos, Camera.main.transform.position);
                if (fdist < 20)// 근처에 블록있다면 가장 가까운 블록을 선택 
                {
                    vPos= FindNearBlock(m_vPrePos);

                    if(vPos!=Vector3.zero)
                    {
                        m_vPrePos = vPos;
                        return true;
                    }

                }

            }
         
        }
        m_vPrePos = Vector3.zero;

        return false;


    }

    /*
     *  근처의 오브젝트를 타겟으로 한다
     *  오브젝트가 없다면, 블록을 타겟으로 한다
     * */

    public void OnJump()
    {
        //m_CoolTimejump.fillAmount = 1;
        //m_CoolTimejump.DOFillAmount(0, CWGlobal.COOLTIME);
        CWChHero.Instance.OnJump();
    }

    public void OnShootDown()
    {
        if (CWHero.Instance.m_bLaserFlag)
        {
            OnShootClick();
        }

    }
    public void OnShootUp()
    {
        if(CWHero.Instance.m_bLaserFlag)
        {
            CWHero.Instance.LaserStop();
        }
        else
        {
            OnShootClick();
        }
    }
    public void OnTouch(Vector2 pos)
    {
        int layer1 = 1 << LayerMask.NameToLayer("Detect");
        int layer2 = 1 << LayerMask.NameToLayer("BlockMap");
        int nMask =  layer2;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hit, Mathf.Infinity, nMask))
        {
            // 해당 오브젝트 선택한 이펙트 나와야 함
            Game_App.Instance.m_gSelectBlock.transform.position = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);

            //vPos = m_gBlockMap.SelectPos(hit.point, hit.normal);
            OnShootClick();
            return ;
        }
        return ;

    }
    public void OnShootClick()
    {
      
        if(CharMode)
        {

            // CWChHero.Instance.BlockDigging(Game_App.Instance.m_gSelectBlock.transform.position);
            //Game_App.Instance.m_gSelectBlock.transform.localPosition = vPos;

            //CWChHero.Instance.SetAttack();


        }
        else
        {
            Vector3 vPos = Vector3.zero;
            // 오브젝트 검사
            if (!FindTarget(ref vPos))
            {
                //// 블록 검사  
                //if (!FindBlock(ref vPos))
                //{
                    //vPos = Camera.main.transform.position + Camera.main.transform.forward * 100f;
                    vPos = CWHero.Instance.GetHitPos() + CWHero.Instance.transform.forward * 100f;

                //}
            }
            _Shoot(vPos);

        }




    }

    #endregion




    #region  멀티 맵 




    int _nMultiCount =0;
    
    public int m_nMultiCount
    {
        get
        {
            return _nMultiCount;
        }
        set
        {
            _nMultiCount = value;

        }
    }
    // 나중에 유저가 많아지면, 레벨별로 나눌 수 있다  항성에 맞게 

    void ReceiveWorldData(JObject jData)
    {

        {
            JArray ja1 = (JArray)jData["Ai"];
            JArray ja2 = (JArray)jData["Owner"];
            int tcnt = ja1.Count;
            for (int i = 0; i < tcnt; i++)
            {
                if (ja1[i] == null) continue;
                if (ja2[i] == null) continue;

                int nAi = CWLib.ConvertInt(ja1[i].ToString());
                int nOwner = CWLib.ConvertInt(ja2[i].ToString());

                if (nOwner == 0) continue;
                CWAIOwnerManager.Instance.AddOwnerList(nAi, nOwner);
            }

        }
      
        {
            JArray ja = (JArray)jData["UserList"];
            // 유저 
            int tcnt = ja.Count;
            for (int i = 0; i < tcnt; i++)
            {
                CWJSon jj = new CWJSon((JObject)ja[i]);
                int nId = jj.GetInt("Id");
                int NHP = jj.GetInt("NHP");
                int HP = jj.GetInt("HP");
                int X = jj.GetInt("X");
                int Y = jj.GetInt("Y");
                int Z = jj.GetInt("Z");
                float fYaw = jj.GetFloat("Yaw");
                int Facker = jj.GetInt("Facker");
                CWUserManager.Instance.RegUser(nId, Facker, new Vector3(X, Y, Z), fYaw, HP,NHP);
            }

        }
        {
            CWJSon jj = new CWJSon(jData);
            byte[] bTemp = jj.GetBytes("BlockMap");
            CWMapManager.SelectMap.ServerBlock(bTemp);
            m_nWType = WTYPE.MULTI;

            LoadMap();
        }

      

    }

    public void CloseMultiMap(string szKiller)
    {

        CWHeroManager.Instance.m_nRepairValue -= 20;
        if (CWHeroManager.Instance.m_nRepairValue < 0) CWHeroManager.Instance.m_nRepairValue = 0;
        CWSocketManager.Instance.UpdateRepairAir(CWHeroManager.Instance.m_nAirSlotID, CWHeroManager.Instance.m_nRepairValue);
        DieMessageDlg.Instance.ShowMulti(OnExit, szKiller);
        CWMobManager.Instance.ClearProcuction();
        CWGlobal.g_GameStop = true;

        if(CollectInvenList.Instance)
        {
            CollectInvenList.Instance.ClearInven();
        }


    }

    IEnumerator ILoadMap()
    {

        LoadingDlg.Instance.Show(true);
        yield return null;
        int nMapID = 0;
        if (m_nWType == WTYPE.SINGLE)
        {
            nMapID = CWArrayManager.Instance.GetMapID(Space_Map.Instance.GetStageID());
        }
        if (m_nWType == WTYPE.MYROOM)
        {
            nMapID = CWGlobal.MYPLANETMAPID;
        }
        if (m_nWType == WTYPE.MULTI)
        {
            nMapID = CWGlobal.MULTIMAPID_1;
        }
        string szlocalfile = CWGlobal.GetLocalFile();
        if (CWGlobal.CheckLoacalfile(szlocalfile))
        {
            CWMapManager.Instance.LoadLocalMap(szlocalfile);
        }
        else
        {
            CWMapManager.Instance.LoadMap(nMapID);
        }
        GamePlay.Instance.EnvironmaenON();

        yield return null;

        HeroReady();
        
        if (m_nWType == WTYPE.SINGLE)
        {

            if (Space_Map.Instance.IsBossWar() && !CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID()))
            {
                CWMobManager.Instance.BossWar(BeginOpen);
            }
            else
            {
                BeginOpen();
            }

        }
        else
        {
            BeginOpen();
        }

    }
    void LoadMap()
    {
        StartCoroutine("ILoadMap");



    }

    public override void EnvironmaenON()
    {
        base.EnvironmaenON();
        int cnt = Game_App.Instance.m_kSkyMat.Length;
        int RR = Random.Range(0, cnt);

        RenderSettings.skybox = Game_App.Instance.m_kSkyMat[RR];


    }
    int GetMultiID()
    {
        return CWGlobal.MULTIMAPID_1;// 첫번째 맵 번호

    }
    void Musuk()
    {
        CWDebugManager.Instance.m_bMusuk = false;
    }
    public void ShowWar()
    {
        m_kMissionText.text = "적 전투기를 모두 파괴하세요!";
        m_nWType = WTYPE.SINGLE;
        CWGlobal.g_bSingleGame = true;
        LoadMap();
        CWGlobal.g_bWarmode = true;
        CWInvenManager.Instance.CBUpdateEvent = UpdateInven;

    }
    public void ShowMyPlanet()
    {
        m_kMissionText.text = "적 전투기를 모두 파괴하세요!";
        m_nWType = WTYPE.MYROOM;
        CWGlobal.g_bSingleGame = true;
        LoadMap();
        CWGlobal.g_bWarmode = true;
        CWInvenManager.Instance.CBUpdateEvent = UpdateInven;

    }

    public void ShowPVP()
    {

        
        m_nWType = WTYPE.PVP;
        
        CWGlobal.g_bWarmode = true;
        CWInvenManager.Instance.CBUpdateEvent = UpdateInven;
        Open();
    }
    public void ShowMultibyTuto()
    {
        m_bTutoMulti = true;// 싱글멀티 예외 처리 변수로 사용한다
        CWGlobal.g_bSingleGame = true;
        CWGlobal.g_bWarmode = false;
        CWInvenManager.Instance.CBUpdateEvent = UpdateInven;
        

        CWSocketManager.Instance.ConnectWoarldbyTuto( (jData)=> {

            JArray ja = (JArray)jData["UserList"];
            // 유저 
            int tcnt = ja.Count;
            for (int i = 0; i < tcnt; i++)
            {
                CWJSon jj = new CWJSon((JObject)ja[i]);
                int nId = jj.GetInt("Id");
                int NHP = 20;
                int HP = jj.GetInt("HP");
                int X = jj.GetInt("X");
                int Y = jj.GetInt("Y");
                int Z = jj.GetInt("Z");
                float fYaw = jj.GetFloat("Yaw");
                int Facker = jj.GetInt("Facker");
                CWUserManager.Instance.RegUser(nId, Facker, new Vector3(X, Y, Z),fYaw, HP, NHP);

            }

            m_nWType = WTYPE.MULTI;
            LoadMap();


        });


    }
    
    public void ShowMulti()
    {
        m_bTutoMulti = false;
        CWGlobal.g_bSingleGame = false;
        CWUdpManager.Instance.StartUDP();
        CWGlobal.g_bWarmode = false;
        CWInvenManager.Instance.CBUpdateEvent = UpdateInven;



        int X = 10;
        int Z = 10;
        int rr = UnityEngine.Random.Range(0, 1);
        if (rr == 0)
        {
            X = UnityEngine.Random.Range(10, 246);
        }
        else
        {
            Z = UnityEngine.Random.Range(10, 246);
        }


        CWSocketManager.Instance.AskRoomNumberWorld(1,(jData) => {

            //
            if(jData["Result"].ToString()=="ok")
            {
                CWGlobal.m_nMultiRoomNumber = jData["roomnumber"].Value<int>();
                CWSocketManager.Instance.ConnectWorld(CWGlobal.m_nMultiRoomNumber,new Vector3(X,60,Z),  ReceiveWorldData, "ReceiveWorldData");
            }

        }, "AskRoomNumberWorld");

        // 5초 동안 무적
        CWDebugManager.Instance.m_bMusuk = true;
        Invoke("Musuk", 5f);


    }

    // 멀티 

    public void MultiKill(int Count)
    {
        if (m_nWType != WTYPE.MULTI) return;

        Dailymission.Instance.CheckUpdate(DAYMTYPE.MULTI, 1);
        // 현재 몇단계인가?
        m_nMultiCount += Count;
        MultiWinDlg.Instance.Show(()=> {

            CWQuestManager.Instance.CheckUpdateData(10, 1);//유저 파괴

        });// 입장권을 준다 

    }


    #endregion


    

    #region 이모티콘

    public void SendEmoticon(int nType)
    {
        CWSocketManager.Instance.SendEmoticon(nType);
    }
    #endregion

    #region 캐릭터 인칭 변화

    
    
    public void ChangeCamMode()
    {
        CWGlobal.g_bCamMode = !CWGlobal.g_bCamMode;
    }



    #endregion



    #region 아이템들 





    #endregion

    #region 블록캐기 

    // 블록을 선택한다
    // 커서를 위치시킨다
    float m_fDiggtime = 0;
    bool m_bDiggStartFlag = false;
    //1초에 한개씩 
    float m_fDiggingtime = 0.5f;
    Vector3 m_vSelectblock=Vector3.zero;

    void EditPlay(Vector2 vPos)
    {


        int nMask = 1 << LayerMask.NameToLayer("BlockMap");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(vPos), out hit, Mathf.Infinity, nMask))
        {

            Vector3 pp = Vector3.zero;
            if (CWChHero.Instance.IsUpdateBlock())
            {
                // 삭제하거나 색을 칠함
                pp = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);
            }
            else
            {
                pp = CWMapManager.SelectMap.GetEditBlock(hit.point, hit.normal);
            }


            if (pp== m_vSelectblock)
            {
                float fdist = Vector3.Distance(hit.point, CWChHero.Instance.GetPosition());
                if (fdist > 32)
                {
                    return;
                }


                m_vSelectblock = pp;
                Vector3Int nPos = CWMapManager.ConvertPos(m_vSelectblock);
                if (!EquipInvenList.Instance.m_bDeletedFlag)
                {
                    if (CWChHero.Instance.m_nSelectBlock == 0)
                    {

                        EquipInvenList.Instance.m_bDeletedFlag = true;
                        return;
                    }
                        

                }
                CWChHero.Instance.SetBlock(nPos.x, nPos.y, nPos.z);

            }


        }


    }
    /*
        public void OnDiggStart(Vector2 vPos)
        {

            int nMask = 1 << LayerMask.NameToLayer("BlockMap");
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(vPos), out hit, Mathf.Infinity, nMask))
            {
                // 해당 오브젝트 선택한 이펙트 나와야 함
                float fdist = Vector3.Distance(hit.point , CWChHero.Instance.GetPosition());
                if(fdist>32)
                {
                    return;
                }
                //m_gBlockMap.GetEditBlock(hit.point, hit.normal);

                //if (m_nWType == WTYPE.MYROOM)
                //{
                //    if(CWChHero.Instance.m_nSelectItem==0)
                //    {
                //        m_vSelectblock = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);
                //    }
                //    else
                //    {
                //        m_vSelectblock = CWMapManager.SelectMap.GetEditBlock(hit.point, hit.normal);
                //    }


                //    return;
                //}
                m_vSelectblock = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);
                m_bDiggStartFlag = true;
                Game_App.Instance.m_gSelectBlock.SetActive(false);
                m_fDiggtime = Time.time;


                return;
            }

        }
    */
    public void DiggPlay()
    {
        if (!m_bDiggStartFlag) return;
        int nMask = 1 << LayerMask.NameToLayer("BlockMap");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
        {
            {
                // 한 블록을 0.2초 동안 응시 했는가
                Vector3 vv = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);
                if (m_vSelectblock == vv)
                {
                    float ff = Time.time - m_fDiggtime;
                    if (ff > 0.2f)
                    {
                        // 이펙트 시작
                        if (Game_App.Instance.m_gSelectBlock.activeSelf == false)
                        {
                            Game_App.Instance.m_gSelectBlock.transform.position = vv;
                            Game_App.Instance.m_gSelectBlock.SetActive(true);
                            CWChHero.Instance.BlockBegin(Game_App.Instance.m_gSelectBlock.transform.position);
                        }
                    }
                    if (ff > m_fDiggingtime)
                    {
                        // 블록을 캔다
                        CWChHero.Instance.BlockDigging(Game_App.Instance.m_gSelectBlock.transform.position);

                        // 다시 시작
                        Game_App.Instance.m_gSelectBlock.SetActive(false);
                        m_fDiggtime = Time.time;
                        CWChHero.Instance.BlockDiggStop();

                    }

                }
                else
                {
                    m_fDiggtime = Time.time;
                    m_vSelectblock = vv;
                    CWChHero.Instance.BlockDiggStop();
                }

            }
        }

    }
    // 블록을 캐고, 부서지는 이펙트 작업
    


    public void  OnPointerDown(Vector2 pos)
    {

        int nMask = 1 << LayerMask.NameToLayer("BlockMap");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hit, Mathf.Infinity, nMask))
        {
            // 해당 오브젝트 선택한 이펙트 나와야 함
            float fdist = Vector3.Distance(hit.point, CWChHero.Instance.GetPosition());
            if (fdist > 32)
            {
                return;
            }

            Vector3 pp = Vector3.zero;
            if (CWChHero.Instance.IsUpdateBlock())
            {
                // 삭제하거나 색을 칠함
                m_vSelectblock = CWMapManager.SelectMap.SelectPos(hit.point, hit.normal);
            }
            else
            {
                m_vSelectblock = CWMapManager.SelectMap.GetEditBlock(hit.point, hit.normal);
            }



            m_fDiggtime = Time.time;
            Game_App.Instance.m_gSelectBlock.SetActive(true);

            Game_App.Instance.m_gSelectBlock.transform.position = m_vSelectblock;

            //if (m_nWType == WTYPE.MYROOM)
            //{
            //    Game_App.Instance.m_gSelectBlock.SetActive(true);
            //    Game_App.Instance.m_gSelectBlock.transform.position = m_vSelectblock;
            //}
            //else
            //{
            //    Game_App.Instance.m_gSelectBlock.SetActive(false);
            //    m_bDiggStartFlag = true;
            //    StartCoroutine("IDiggingRun");

            //}
            return;
        }

    }
    public void OnPointerUp(Vector2 pos)
    {
        //if (m_nWType == WTYPE.MYROOM)
        {
            EditPlay(pos);
            Game_App.Instance.m_gSelectBlock.SetActive(false);
            //CWResourceManager.Instance.PlaySound("digup"); //블록 캐는 소리
            return;
        }
        m_bDiggStartFlag = false;

        CWChHero.Instance.BlockDiggStop();
        StopCoroutine("IDiggingRun");

    }
    // 블록 캐기 시작
    IEnumerator IDiggingRun()
    {
        while (true)
        {
            DiggPlay();
            yield return null;
        }
    }



    #endregion

    #region  버튼

    public void OnTempInven()
    {
        
    }
    public void OnCollect()// 수거하기
    {
    }
    public void OnEnchant()// 합성하기
    {
        SynthesisDlg.Instance.Show();
    }
    public void OnPause()
    {
        PauseDlg.Instance.Open();
    }

    #endregion
    #region 조이스틱

    public CWJoystickCtrl m_AircraftJoy;
    public CWJoystickCtrl m_HeightJoy;

    public CameraAction m_CharCamAction;
    public CameraAction m_HeroCamAction;


    public CWJoystickCtrl GetCharJoy()
    {
        return m_AircraftJoy;
    }
    public CWJoystickCtrl GetHeightJoy()
    {
        return m_HeightJoy;
    }
    public CWJoystickCtrl GetAircraftJoy()
    {
        return m_AircraftJoy;
    }

    #endregion


}
