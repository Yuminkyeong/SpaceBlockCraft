using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;



public class Space_Map : PageUI<Space_Map>
{
    public bool m_bHelpFlag = false;
    public UIOverlayPos m_gHeroTarget;
    protected override int GetUINumber()
    {
        return 1;
    }

    
    public PlanetCtrl m_gPlanetCtrl;
    

    bool m_bEnterflag
    {
        get
        {
            return CWHeroManager.Instance.IsEnterPlanet(GetPlanetID(), GetStageID());
        }
    }
    public void OnADReward()
    {
        CWADManager.Instance.RewardShow(() => {
            CWSocketManager.Instance.UseCoin(COIN.TICKET, CWGlobal.REWARDTICKET, (jData)=> {

                if (jData["Result"].ToString() == "ok")
                {
                    CWCoinManager.Instance.SetData(jData["Coins"]);
                    string sz2 = CWLocalization.Instance.GetLanguage("드링크 {0}개를 획득하였습니다!");
                    string sz= string.Format(sz2, CWGlobal.REWARDTICKET);
                    NoticeMessage.Instance.Show(sz);

                    AnalyticsLog.Print("ADLog", "ADReward-Gem");

                }
            }, "AD Reward");

        });

    }
    void HelpAD()
    {
        HelpPackageDlg.Instance.Show();
    }
    public override void Open()
    {
        _bClose = false;
        if(m_bHelpFlag)
        {
            m_bHelpFlag = false;
            Invoke("HelpAD", 1f);
        }
        

        base.Open();
    }

    bool m_bOnce = false;
    protected override void _Open()
    {
        CWGlobal.g_GameStop = false;
        CWPoolManager.Instance.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        base._Open();

        foreach (var v in m_kPlanetSlots) v.UpdateData();
        //

        CWHero.Instance.ResetHP();
   //     CWHero.Instance.Show(false);
  

        m_nSolaID = CWHeroManager.Instance.m_nSolaID;// 1부터

        FocusPlanetUpdate();


        if (!m_bOnce)
        {
            
            if (CWHeroManager.Instance.m_bTuto)
            {
                ChangePlanet(3, 3);

            }
            else
            {
                ChangePlanet(0, 0);

            }
            m_bOnce = true;
        }
        else
        {
            SetPlanet();
        }


        CWMapManager.Instance.Close();
        CWBgmManager.Instance.PlayStage();
        


        m_bEnter = false;

    }
    bool _bClose = false;
    public override void BaseClose()
    {
        if (_bClose) return;
        _bClose = true;
        CWGalaxy.Instance.OnPlayMap();
        CWSocketManager.Instance.SendLog();
        base.BaseClose();
    }
    private void Start()
    {
        
    }

    public void OnMove()
    {
        
    }
   
    #region 변수 

    //0~5 변수
    int _Face;
    public int m_nNowFace
    {
        get
        {
            return _Face%6;
        }
    }
    public int m_nFaceNumber
    {
        get
        {
            return _Face;
        }
        set
        {
            _Face = value;
        }
    }
    int _SolaID = -1;
    public int m_nSolaID
    {
        get
        {
            return CWGalaxy.Instance.GetSolaID();
        }
        set
        {
            if (_SolaID == m_nSolaID) return;
            CWGalaxy.Instance.SelectSola(value);
            
            if (m_nSolaID == CWHeroManager.Instance.m_nSolaID)
            {
                m_nPlanetNumber = (CWHeroManager.Instance.m_nPlanetID - 1) % 6;//0~5            
            }
            else
            {
                m_nPlanetNumber = 0;
            }
            _SolaID = m_nSolaID;


        }
    }

    public int m_nPlanetNumber
    {
        get
        {
            return CWGalaxy.Instance.m_kSelectStar.m_nSelectPlanet;
        }
        set
        {
            CWGalaxy.Instance.m_kSelectStar.m_nSelectPlanet = value;
            // 디폴트는 아직 점령하지 않은 맵 
            //m_nFaceNumber = 0; //
            //for(int i=0;i<6;i++)
            //{
            //    int nPlanetID = GetPlanetID();
            //    int nStage = (nPlanetID - 1) * 6 + i + 1;
            //    bool bflag = CWHeroManager.Instance.IsEndTask(nStage);
            //    if(bflag==false) // 점령하지 않은 지역이 우선순위다
            //    {
            //        m_nFaceNumber = i; //
            //        break;
            //    }

            //}

        }
    }

    #endregion

    #region 정보

    public Sprite[] m_kPlanetsprites;
    public string[] m_kPlanettext;
    public PlanetSlot[] m_kPlanetSlots;
    public GameObject[] m_gPlanetInfo;
    public int m_nPlanetType=0;
    public int m_nNowSlot=0;
    //0 스토리 행성, 1 : 멀티 행성 2: 1:1 대전  3 : 내행성
    void SetPlanet()
    {
        if (m_nPlanetType == 0)
        {

            OnNowPlanet();
        }
        if (m_nPlanetType == 1)
        {
            OnMultiPlanet();
        }
        if (m_nPlanetType == 2)
        {
            OnPVP();
        }
        if (m_nPlanetType == 3)
        {

            OnMyPlanet();
        }

    }
    

    public void ChangePlanet(int slot,int number)
    {

        //if (number == 3)
        //{
        //    NoticeMessage.Instance.Show("다음 업데이트를 기대해주세요!");
        //    return;
        //}
        //if (number == 2)
        //{
        //    NoticeMessage.Instance.Show("아직 개발 중 입니다!");
        //    return;
        //}


        m_nNowSlot = slot;
        m_nPlanetType = number;
        foreach (var v in m_gPlanetInfo) v.SetActive(false);
        int nn = m_kPlanetSlots[0].m_nNumber;
        m_kPlanetSlots[0].m_nNumber = number;
        m_kPlanetSlots[slot].m_nNumber = nn;
        m_kPlanetSlots[slot].UpdateData();
        m_kPlanetSlots[0].UpdateData();

        m_gPlanetInfo[number].SetActive(true);

        SetPlanet();


    }


    int m_nMiriType;// 미리 결정되는 
    int _nType = -1;
    public int m_nType //0 현재 행성, 1 행성탐험,2 태양보기,3 은하계보기 ,4멀티행성 5 PVP,6 내 행성
    {
        get
        {
            return _nType;
        }
        set
        {
            _nType = value;
            if(_nType==1)// 현재 행성보기라면, 
            {
                // 같은 태양이라면
                if(m_nSolaID==CWHeroManager.Instance.m_nSolaID)
                {
                    m_nPlanetNumber = (CWHeroManager.Instance.m_nPlanetID - 1) % 6;//0~5            
                }
                
            }
            ShowUI();

            //MoveUI();
            

            m_nMiriType = m_nType;
        }
    }
    
    public  Text m_kInfoText;
    

    string [] m_sztip =
    {
        "레벨이 낮아 진입이 불가합니다!",
        "진입이 가능합니다.",
        "미 정복",
        "블록캐기 가능",
    };
    public bool IsBossWar()
    {
        if (m_nNowFace == 5) return true;
        return false;
    }
    public int GetStageID()
    {
        int nPlanetID = GetPlanetID();
        return (nPlanetID - 1) * 6 + m_nNowFace + 1;

    }
    // 현재 선택한 행성 번호 
    public int GetPlanetID()
    {
        // 현재 태양 
        // 
        int nPlanetID = (m_nSolaID-1)*6+ m_nPlanetNumber+1;
        return nPlanetID;
    }

    public string CurrentAreaName()
    {

        int nStage = GetStageID();
        CWArrayManager.StageData kk = CWArrayManager.Instance.GetStageData(nStage);
        return kk.m_szName;

    }
    public string GetCurrentPlanet()
    {
        return string.Format("{0}", CWGlobal.GetCurrentPlanet(m_nPlanetNumber));
    }
    public string GetCurrentArea()
    {
        return string.Format("{0}", CWGlobal.GetCurrentArea( m_nNowFace));
    }

    public bool IsMulti()
    {
        if (m_nPlanetType == 1) return true;
        return false;
    }
    public bool IsMyPlanet()
    {
        if (m_nPlanetType == 3) return true;
        return false;
    }


    #endregion

    #region 버튼 



    //int m_nType = 0;//0 현재 행성, 1 행성탐험,2 태양보기,3 은하계보기 ,4지구 보기
    // 지구 보기 
    public void OnPVP()
    {
       
        CWGalaxy.Instance.OnPVPPlanetMap();
        UpdateData();

    }
    public void OnMultiPlanet()
    {
      
        CWGalaxy.Instance.OnMultiPlanetMap();
        UpdateData();
    }
    public void OnMyPlanet()
    {
       
        CWGalaxy.Instance.OnMYPlanetMap();
        UpdateData();

    }

    




    IEnumerator ILoadMap()
    {
        
        LoadingDlg.Instance.Open();
        
        yield return new WaitForSeconds(1.4f);
        if (m_nType == 0)
        {
            CWGalaxy.Instance.OnPlanetMap(()=> {


                MoveStage(CWHeroManager.Instance.m_nStageNumber - 1);
            });
            
        }

        if (m_nType==1)
        {
            CWGalaxy.Instance.OnSolaMap();
        }
        if (m_nType == 2)
        {
            CWGalaxy.Instance.OnTitleMap();
        }

        if (m_nType == 3)
        {
            CWGalaxy.Instance.OnGalaxyMap();
        }

        
        LoadingDlg.Instance.Close();
        yield return new WaitForSeconds(0.1f);
        UpdateData();
    }


    // 현재 태양 보기 
    public void OnSola()
    {
        
        //  if (m_nType == 2) return;
        m_nType = 2;
        if (gameObject.activeInHierarchy == false) return;
        StartCoroutine("ILoadMap");
    }

    // 현재 행성 보기 
    public void OnNowPlanet()
    {

      
        
        m_nSolaID = CWHeroManager.Instance.m_nSolaID;
        CWGalaxy.Instance.Rotate(m_nNowFace);
        m_nType = 0;
        if (gameObject.activeInHierarchy == false) return;
        StartCoroutine("ILoadMap");
        CWShowUI.g_bValueList[12] = true;

        
    }

    // 현재 행성 탐험
    public void OnPlanet()
    {
        
        m_nType = 1;
        if (gameObject.activeInHierarchy == false) return;
        StartCoroutine("ILoadMap");
        
    }
    // 은하 탐험
    public void OnGalaxy()
    {
        
        //  if (m_nType == 3) return;
        m_nType = 3;
        if (gameObject.activeInHierarchy == false) return;
        StartCoroutine("ILoadMap");
    }

    public void OnInfo()
    {


    }
    bool m_bEnter = false;
    void ReSetEnter()
    {
        m_bEnter = false;
    }
    public void OnEnter()
    {

        if (m_bEnter) return;
        m_bEnter = true;
        Invoke("ReSetEnter", 3f);

        if(CWHeroManager.Instance.IsDontUseShip() )
        {
            NoticeMessage.Instance.Show("비행기 수리가 필요합니다.");
            return;
        }

        if (m_nPlanetType == 0)// 스토리
        {
           /* //정복이 되었다면 티켓 필요x
            if (CWHeroManager.Instance.IsEndTask(Space_Map.Instance.GetStageID()))
            {
                EnterMyroomFunction();
                return;
            }*/
            // 정복이 되지 않았는가?
            if (CWHeroManager.Instance.m_nStageNumber  < GetStageID())
            {

                MessageBoxDlg.Instance.Show(() => {
                    // 행성 이동 

                    MoveStage(CWHeroManager.Instance.m_nStageNumber-1);

                }, null, "", "이전 스테이지를 클리어해야 입장 가능합니다. 정복 가능한 구역으로 이동 하시겠습니까?  ");
                return;
            }
            // 태양계 밖은 안됨
            if (GetStageID() > 6*6)
            {

                MessageBoxDlg.Instance.Show(() => {
                    // 행성 이동 

                    MoveStage(CWHeroManager.Instance.m_nStageNumber - 1);

                }, null, "", " 정복할 구역으로 이동 하시겠습니까?  ");
                return;
            }



            EnterFunction();
            return;
        }
        if (m_nPlanetType == 1)// 멀티
        {
            EnterFunction();
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.MultiCount);
            return;
        }
        if (m_nPlanetType == 2)// 1:1
        {
            EnterFunction();
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.PVPCount);
            return;
        }
        if (m_nPlanetType == 3)// 내행성
        {
            
            EnterMyroomFunction();
            CWSocketManager.Instance.UpdateDayLog(DAYLOG.MyroomCount);
            return;
        }


        /*
        // 현재 행성과 멀티만 해당
        if (m_nType==0|| m_nType==4 || m_nType == 6)
        {
            if (m_bEnterflag == false)
            {
                //NoticeMessage.Instance.Show("입장이 불가합니다!");
                NoticeMessage.Instance.Show("행성을 모두 정복해야 블록캐기가 가능합니다");
                return;
            }
            // 전투 혹은 파밍
            EnterFunction();

        }
        else
        {
            if (m_nType == 5)
            {
                PVPDlg.Instance.Open();
                Close();
                return;
            }
            else if (m_nType==3)
            {
                OnPlanet();
                return;
            }
            else if (m_nType == 1)
            {
                if (m_bEnterflag == false)
                {
                    NoticeMessage.Instance.Show("등급이 낮아 입장이 불가합니다!");
                    return;
                }
                CWHeroManager.Instance.m_nStageNumber = GetStageID();
                OnNowPlanet();
                //CWSocketManager.Instance.SendFirstData(7);// 행성 보기 
                return;
            }

        }
*/
        
    }

    // 현재의 행성에 맞추어 카메라를 움직인다
    void _CloseMovePlanet()
    {
        CWGalaxy.Instance.Rotate(m_nNowFace);
    }
   
    void FocusPlanetUpdate()
    {
        // 회전만 하는가?
        // 행성이 다른가?
        //
        int planet = _Face / 6;
        if(CWGalaxy.Instance.m_kSelectStar.m_nSelectPlanet==planet)
        {
            CWGalaxy.Instance.Rotate(m_nNowFace);
        }
        else
        {
            // 행성이동 
            CWGalaxy.Instance.LookPlanet(planet, _CloseMovePlanet);

        }


    }
    public void MoveStage(int nStage)
    {
        _Face = nStage;
        FocusPlanetUpdate();
    }

    public bool IsLeftPlanet()
    {

        int nPlanetID= ((GetStageID()-1)/6)%6;
        if (nPlanetID == 0) return false;
        return true;
    }
    public bool IsRightPlanet()
    {
        int nPlanetID = ((GetStageID() - 1) / 6) % 6;
        if (CWHeroManager.Instance.m_nGrade >= nPlanetID) return true;
        return false;
        //// 현재 행성을 정복 했다면 갈 수 있다
        //if (CWGalaxy.Instance.m_kSelectStar.m_nSelectPlanet < 5)
        //{
        //    if (CWHeroManager.Instance.IsVictoryPlanet(GetPlanetID()))
        //        return true;

        //}
        //return false;

    }

    public void OnLeft()
    {

        if(m_nFaceNumber==0)
        {
            return;
        }
        m_nFaceNumber--;
        FocusPlanetUpdate();
    }
    public void OnRight()
    {
        if (m_nFaceNumber == 6*6-1)//6면 6 행성 4개의 태양
        {
            NoticeMessage.Instance.Show("다음 업데이트를 기대해주세요!");
            return;
        }


        m_nFaceNumber++;
        FocusPlanetUpdate();
        //if (m_nType == 0)
        //{
        //    m_nFaceNumber++;
        //}

        //if (m_nType == 1)/// 행성 탐험  다른 행성을 보여준다 
        //{
        //    CWGalaxy.Instance.OnPlanetRight();
        //}
        //if (m_nType == 3 || m_nType == 2)
        //{
        //    m_nSolaID++;
        //    CWGalaxy.Instance.OnGalaxyMap();
        //}

        //UpdateData();
    }
         
    public void OnHideFaceInfo()
    {
        
    }

    #endregion

    #region 컨트롤

    public void SetFace(int nFace)
    {
        m_nFaceNumber = nFace-1;
        UpdateData();
    }
    // 가장 가까운 지점
    public void CheckPlanet()
    {
        if(CWGalaxy.Instance.CheckPlanet())
        {
            UpdateData();
        }

        
    }


    #endregion

    #region  화면 

    public GameObject[] m_gMainUI;// 메인 UI
    public GameObject[] m_gMapUI; // 지도일때 나오는 UI

    void ShowUI()
    {
        OnHideFaceInfo();
        if (m_nType==0|| m_nType == 4 || m_nType == 5)
        {
            foreach (var v in m_gMainUI)
            {
                if(v)v.SetActive(true);
            }
                
            foreach (var v in m_gMapUI)
            {
                if (v) v.SetActive(false);
            }
                
         
        }
        else
        {
            foreach (var v in m_gMainUI)
            {
                if(v)v.SetActive(false);
            }

            foreach (var v in m_gMapUI) { if(v)v.SetActive(true); }
            
        }
        
    }

    #endregion

    #region 입장

    void _RunMap()
    {


        Close();

        

        CWHero.Instance.ResetHP();


        if (IsMulti())
        {

            _MultiRun();
        }
        else
        {
            if(IsMyPlanet())
            {

                GamePlay.Instance.ShowMyPlanet();
            }
            else
            {
                CWGlobal.g_bSingleGame = true;
                
                GamePlay.Instance.ShowWar();
            }

        }

     //   _nType = -1;


    }

    void EnterFunction()
    {

        CWArrayManager.PlanetData gPlanetData = CWArrayManager.Instance.GetPlanetData(GetPlanetID());

        // 입장권이 있는지 검사 
        int tcnt = CWCoinManager.Instance.GetCoin(COIN.TICKET);
            
        if (tcnt < gPlanetData.m_nTicket)
        {
              
            //if(gPlanetData.m_nTicket == 1)
            //{
            //    MessageBoxDlg.Instance.Show(ShowAD, null, "입장권이 없습니다!", " 광고보고 입장하시겠습니까?");
            //}
            //else
            //{
            //    ItemHelpDlg.Instance.Show((int)GITEM.Ticket, "입장권이 없습니다!", "입장권은 멀티맵에서 구할 수 있습니다!");//

            //}

            ItemHelpDlg.Instance.Show((int)GITEM.Ticket, "드링크가 없습니다!", "지니의 드링크 자판기에서 구매할 수 있습니다. ");//
            return;
        }
        CWResourceManager.Instance.PlaySound("canopen");
        string strsz = CWLocalization.Instance.GetLanguage("드링크 {0}개를 사용합니다.");
        NoticeMessage.Instance.Show(string.Format(strsz, gPlanetData.m_nTicket));
        CWSocketManager.Instance.UseCoinEx(COIN.TICKET, -gPlanetData.m_nTicket);

        if (m_nPlanetType == 2)
        {
            PVPDlg.Instance.ShowSingle();
            Close();
        }
        else
        {
            _RunMap();
        }


        
        


    }

    void EnterMyroomFunction()
    {

        CWArrayManager.PlanetData gPlanetData = CWArrayManager.Instance.GetPlanetData(GetPlanetID());
     
            _RunMap();
     }



    void _WarRun()
    {

        //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelStart);
        GamePlay.Instance.ShowWar();
    }

    void _ParmingRun()
    {
        GamePlay.Instance.ShowWar();
    }
    void _MultiRun()
    {
        GamePlay.Instance.ShowMulti();
    }

    #endregion
    #region 나오기/없어지기
    //public override void OnExit()
    //{
    //    CWMainGame.Instance.Quit();
    //}
    public void ShowOnce()
    {
        m_bShow = false;
        m_bOnce = false;
        CWHeroManager.Instance.m_bTuto = false;
        Show(0);
    }
    public void Show(int nType=0)
    {
        ///m_bShow = false;
        if (nType==100)// 전투에서 복귀
        {
            m_nMiriType = m_nType;
            _nType = -1;
        }
        else
        {
            MoveStage(CWHeroManager.Instance.m_nStageNumber - 1);
            m_nMiriType = nType;
        }
        
        Open();


//        m_nPlanetType = number;
        foreach (var v in m_gPlanetInfo) v.SetActive(false);
 
        m_gPlanetInfo[m_nPlanetType].SetActive(true);

        


    }


    public override void OnEscKey()
    {
        if (m_gWindow == this)
        {
            MessageBoxDlg.Instance.Show(CWMainGame.Instance.Quit, null, "종료", "종료하시겠습니까?");
        }
        else
        {

            MessageBoxDlg.Instance.Show(m_gWindow.OnExit, null, "종료", "나가시겠습니까?");
        }

    }

    #endregion


    #region 행성 정보 표기 

    public Vector2[] m_gUIPos;

    #endregion

    public void ClearMap()
    {
        NoticeMessage.Instance.Show("블록을 전부 채집하였습니다.");
    }

    #region UI 버튼들

    // 에디터로 전환한다
    public void OnEditor()
    {
        // 전환 애니메이션
        //GameEdit.Instance.Open();
        ShipSlotDlg.Instance.Open();

    }
    
    public void OnUserMyInfoDlg()
    {
        UserMyInfoDlg.Instance.Open();
    }
    public void OnQuestDlgOpen()
    {
        QuestDlg.Instance.Open();
    }
    public void OnNoticeEventOpen()
    {
        NoticeEventDlg.Instance.Open();
    }
    public void OnGameInfoOpen()
    {
        GameInfoDlg.Instance.Open();
    }
    public void OnModeSelectOpen()
    {
        ModeSelectDlg.Instance.Open();
    }
    public void OnInventoryOpen()
    {
        InventoryDlg.Instance.Open();
    }
    public void OnTearInfoOpen()
    {
        TearInfoDlg.Instance.Open();
    }
    public void OnStoreOpen()
    {
        StoreDlg.Instance.Show();
    }
    public void OnOptionOpen()
    {
        OptionDlg.Instance.Open();
    }
    public void OnMailOpen()
    {
        MailDlg.Instance.Open();
    }
    public void OnAcheivementsOpen()
    {
        AcheivementsDlg.Instance.Open();
    }
    public void OnDailyOpen()
    {

        DailyDlg.Instance.Open();
    }
    public void OnADDia()
    {
        ADDialDlg.Instance.Open();
    }
    public void OnDailyMission()
    {
        Dailymission.Instance.Open();
    }
    public void OnCharProfill()
    {
        CharProfileDlg.Instance.Open();
    }
    public void OnRanking()
    {

        RankingDlg.Instance.Open();
    }
    public void OnShipSlotOpen()
    {

        Close();
        ShipSlotDlg.Instance.Open();
    }
    // 도감
    public void OnDictionaryOpen()
    {
        DictionaryDlg.Instance.Open();
    }
    // 블록 분해
    public void OnBlockDevide()
    {
        divideDlg.Instance.Show();
    }
    public void OnChattingDlg()
    {

        ChattingDlg.Instance.Open();
    }
    public void OnCharProfileDlg()
    {

        CharProfileDlg.Instance.Open();
    }
    public void OnReNameDlg()
    {
        UserMyInfoDlg.Instance.Open();
        //ChangeNameDlg.Instance.Open();
        //CharProfileDlg.Instance.Open();
    }

    public void OnWorldMap()
    {
        WorldMapDlg.Instance.Open();
    }
    public void OnWeaponUpgrade()
    {


        byte[] buffer = CWHero.Instance.GetBuffer();
        if (buffer == null)
        {
            TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
            buffer = aa.bytes;
        }
        WeaponUpgradeDlg.Instance.Show(CWHeroManager.Instance.m_nAirSlotID, buffer);
        //WeaponUpgradeDlg.Instance.CloseFuction = ShipSlotDlg.Instance.Open;

    }
    public void OnBlockUpgrade()
    {
        SynthesisDlg.Instance.Show();
    }

    public void OnHeroClick()
    {
        ShipInfoDlg.Instance.Open();
        
    }

    #endregion




}
