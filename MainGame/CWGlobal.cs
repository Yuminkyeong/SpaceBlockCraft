using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;
using System;
using System.IO;
public class CWGlobal
{

    

   


    public enum SYSTEMSTATE {BEST,GOOD,BAD };
    static public SYSTEMSTATE g_SystemState;// 시스템 상태



    static public UnityEngine.Rendering.IndexFormat IndexFormat
    {
        get
        {
            return UnityEngine.Rendering.IndexFormat.UInt32;
            
        }

    }

    #region 아이템 타입 

    static public int GetItemType(GITEMDATA gData)
    {
        if (gData.type == "원석")
        {
            return 1;
        }
        if (gData.type == "stoneblock")
        {
            return 2;
        }
        if (gData.type == "energy")
        {
            return 3;
        }
        if (gData.type == "강화석")
        {
            return 4;
        }
        if (gData.type == "보석")
        {
            return 5;
        }
        if (gData.type == "resblock")
        {
            return 6;
        }
        if (gData.type == "color")
        {
            return 7;
        }
        if (gData.type == "부스터원소")
        {
            return 8;
        }
        if (gData.type == "Buster")
        {
            return 9;
        }
        if (gData.type == "아이템")
        {
            return 10;
        }
        if (gData.type == "shipblock")
        {
            return 11;
        }
        if (gData.type == "weapon")
        {
            return 12;
        }
        if (gData.type == "무기원소")
        {
            return 13;
        }
        return 0;

    }




    #endregion

    #region 게임 제어 관련 변수

    // 게임에 필요한 체크 변수
    public static bool g_bCheckValue1;
    public static bool g_bCheckValue2;
    public static bool g_bCheckValue3;

    public static bool g_bGameBegin
    {
        get
        {
            if(CWHeroManager.Instance.GetMyStageCount() <= 6)
            {
                return true;
            }
            return false;
        }
    }

    public static bool g_bCamMode=true;// 1인칭만 게임에 사용 3인칭은 테스트 용도로만

    public static bool g_bMapClear;// 맵을 클리어 했다


    public static bool g_bVibration = false;
    public static float g_fShootTime = 4f;
    
    public const int START_HEIGHT = 85;//전투시  시작 높이 
    public const int START_X = 128;//전투시 X 위치
    public const int START_Z = 10;//전투시  Z 위치  // 고정


    static public bool g_bUserBuildDontTouch = false;// 유저 건물 클릭 못한다

    static public bool g_bSingleGame = true;// 싱글게임은 스토리 게임이다 개념 정립
    
    static public bool g_bStopHeroLookAtCamera = false;// 주인공 보는 방향 정지

    static public bool g_bStopAI=false;//AI 정지

    static public bool g_bStopAIAttack = false;//AI 공격 금지 

    static public bool g_bFirstCamera=false;//1 인칭 모드인가?

    static public bool g_bStopCameraFollow;// 카메라 따라다니기 모드 금지  프로덕션모드에서 


    static public int g_nAILevel=1; // AI 최하 레벨 

    static public float g_fHeroRange=128; // 주인공 사거리 
    static public bool g_bDontSave = false; // 저장하면 안되는 상태


    static public bool g_bWarmode=false;// 전투 모드 혹은 파밍모드 

    public const int MAXSTAGE = 10000; // 최대 스테이지 
    public const int MAXGRADE = 8; // 최대 등급

    static public string g_StartAir = "chobo_1";// 시작 비행기

    static public bool g_bIsMail=false;// 메일이 존재하는가?

    static public bool g_bToday = false;// 오늘 처음 게임한다

    // 배속 관련 

    static public int g_nBombMul=1;// 폭탄 배속
    static public int g_nSpeedMul=1; // 게임 배속

    static public int HIDENITEMCOUNT2 = 4;// 숨어 있는 희든 광물 
    static public int HIDENITEMCOUNT3 = 4;// 숨어 있는 희든 광물 



    // 공격력 2배 
    //
    
    static public bool g_bDamageDouble;

    static public bool g_bADDouble = false;//보상 2배 


    static public DateTime g_kHelpPackageDate;


    #endregion

    #region  멀티 행성 
    // 항성별로 다른 레벨로 조정이 필요하다면, 여기 변수는 새로 설정해야 한다

    static public int m_nMultiPlanet = 500; // 임시 번호로 정한다. 추후에 유저가 늘어나면 이것의 개념을 바꾼다 
    static public int m_nMultiRoomNumber = 1; // 멀티맵 룸번호 시작번호 0이면 싱글로 정하고, 1부터 룸번호 시작 


    static public string g_szMultiRanking_1_Name=""; // 멀티 랭킹 이름 
    static public int  g_nMultiRanking_1_Count=1; // 멀티 랭킹 킬 카운트 

    static public string g_szMultiRanking_2_Name = ""; // 멀티 랭킹 이름 
    static public int g_nMultiRanking_2_Count = 1; // 멀티 랭킹 킬 카운트 

    static public string g_szMultiRanking_3_Name = ""; // 멀티 랭킹 이름 
    static public int g_nMultiRanking_3_Count = 1; // 멀티 랭킹 킬 카운트 


    #endregion

    #region 게임 파라메터 관련

    static public string g_szParam1 = "";
    static public string g_szParam2 = "";
    static public string g_szParam3 = "";
    static public string g_szParam4 = "";
    static public bool g_GameStop = false;

    static public float g_TimeSec;// 초단위
    static public int g_BlockCount = 0;
    static public int g_CheckBlock = 0;
    static public int g_MaxBlockCount = 0;


    static public int g_TimeExtend = 40;//40초씩 늘어난다
    static public int g_TimeExtendbyMulti = 30;//90초씩 늘어난다


    #endregion



    static public void SetCheckBlock(int nBlock,int MaxCount)
    {
       g_CheckBlock = nBlock;
       g_MaxBlockCount = MaxCount;
    }
    static public void CheckBlock(int nBlock)
    {
        if (g_CheckBlock == 0) return;
        if(g_CheckBlock==nBlock)
        {
            g_BlockCount++;
        }
    }

    
    private static bool g_bGameStart = false;
    static public bool g_bSoundOn = false;
    static public bool g_bBgmOn
    {
        get
        {
            if (CWBgmManager.Instance == null) return false;
            return CWBgmManager.Instance.m_bBgmOn;
        }
        set
        {
            if (CWBgmManager.Instance == null) return;
            CWBgmManager.Instance.m_bBgmOn = value;
        }
    }

    static public bool g_bAutoHeight = false;


    public static int m_nCreateSellDist = 256;
    public static int m_nCreateMeshDist = 128;


    
    public const int GALAXYWIDTH = 1000;// 은하계에 존재하는 최대별
    

    // NPC 번호
    // 유저 건물 번호 
    // 가상 유저 번호 
    public const int NPC_STARTNUBER = 10000000;// NPC CWTurretManager 관리 드론,해적선 포함
    public const int USERBUILD_STARTNUBER = 20000000;// 유저 건물 CWUserBuildManager 관리 
    public const int FAKEUSER_STARTNUBER = 30000000;// 가상 유저 서버에서 관리 



    public const int BATTLE_AI_STARTNUBER = 50000000;// 배틀 AI 시작번호 

    public const int MAP_USER_REST_COUNT = 10;// 맵에서 휴식을 취할 수 있는 유저 개수 
    public const int MAP_USERBUILD_REST_COUNT = 10;// 맵 유저 건물 개수
    public const int MAP_SELL_GRID_SIZE = 16;// 맵에서 서로 떨어져 있어야 하는 간격 최소 16 이상 

    static public bool g_LODWORK;// LOD작업

    static public byte[] BITArray = { 1, 2, 4, 8, 16, 32, 64, 128 };

    static public int g_BestIron=(int)GITEM.douglasrium;//가장 비싼 금속 


    static public int RENAMEPRICE = 2;// 이름 변경 가격
    
    
    static public int MAXBUSTERCOUNT = 2;//부스터슬롯 개수 

    static public int MAXBLOCKLEVEL = 21;// 최대 블록 레벨


    static public int REPARIPRICE = 1;// HP 당 수리 비용

    static public int WEAPONPRICE = 500000;

    #region 데이타베이스 상수
    static public int[] RANKING_POINT = new int[10];
    static public int TICKET_ADCOUNT = 5;// 하루 구입 할 수 있는 최대 광고 티켓수 

    static public int MULTI_HP = 100; // 멀티 HP
    static public int MULTI_ATTACK = 10;// 멀티 attack

    //즉시수리가격
    static public float DRINK_BY_GEM = 1;// 드링크 보석 가격

    static public int REPAIR_PRICE = 10;// 즉시 수리 가격
    static public int REPAIRTIME = 100;// 
    static public int UPGRADE_GEM = 5;// 강화에 필요한 업그레이드 보석
    static public int CHAR_PRICE = 90;// 캐릭터 가격
    static public DateTime RANKRESETTIME;
    

    static public int DAYMISSION = 10;// 일일 미션 
    static public int DAYMISSIONTICKET = 30;// 일일티켓 
    static public int TODAY_RESET = 10;

    static public int MISSIONTICKET = 5;
    static public int GET_TICKETTIME = 10;// 티켓을 얻는 초
    static public int TICKETTIME_MAX = 10;// 최대 티켓 얻는 개수 


    static public int PVP_SCORE = 5;
    static public int PVP_TICKET = 5;
    static public int PVP_GOLD = 30000;



    static public int MULTI_CONTINUE = 10; // 멀티 계속하기
    static public int MULTI_TICKETRANDOM = 1; // 램덤 확률
    static public int MULTI_RESETTIMER = 60*60*1000; // 
    

    

    static public int REWARDTICKET = 2; //광고 입장권으로 변경

    static public int MULTIMAPID_1 = 83; // 첫번째 멀티맵 번호 
    static public int MYPLANETMAPID = 100; // 내행성맵




    static public int MULTIENTERPRICE = 20;// 멀티 입장료
    static public int MULTICOUNT = 5; //멀티 최대 입장료 회수 


    static public int UPLOADCASH = 10000;// 업로드하는 파일 가격
    static public int OnlyBuyDesign = 10000;// 오직 비행기 모형만 가져가는 금액 
    static public int WAR_TIME = 120;// 전투시간
    static public int PVP_TIME = 120;

    static public int MULTI_TIME = 180;// 멀티 시간

    static public float COOLTIME = 0.2f;
    static public int MAXWEAPONCOUNT = 8;//무기 슬롯 개수 
    

    

    static public string EVENTSEASON1; //시즌시작
    static public string EVENTSEASON2; // 시즌끝
    static public string EVENTSEASONTITLE;

    


    

    #endregion

    
         


    // 인벤 최대 들어가는 개수 
    public const int MAXINVENCOUNT = 10000;// 무제한 

    //#나중에
    static public int g_HajukReward = 5;// 해적에게 받는 보상 나중에 구체적으로 

    // 최대 연료통


    

    
    static public int AUTOREPAIRGIFTCOUNT=10; // 친구선물 HP 최대 개수
    static public int AUTOREPAIRADCOUNT=3; // 광고 3개


    static public int MAXGIFTCOUNT = 10;// 하루에 보낼 수 있는 선물 개수
    static public int FACEIMAGESIZE = 32;

    static public int ICONIMAGESIZE = 256;
    public const int MAX_TIMER = 60; // 쿨타임은 최대 60초를 넘지 않게 한다 


    public const int WD_HEIGHT = 64;

    //public const int LOD = 16;
    static public int LOD = 16;

    public const int WORLDMAPVIEW_SIZE = 128;// 32맵셀 4 * 4 개
    public const int SELLCOUNT = 32;
    public const int WD_WORLD_HEIGHT = 64;

    public const int GRIDSIZE = 256;

    
    public const int MAXHEIGHT = 100; /// 최대 올라갈 수 있는 높이 
    public const int MINHEIGHT = 1; /// 최대 올라갈 수 있는 높이 

    public const int WORLDWIDTH  = 100 ; /// 월드맵 가로 
    public const int WORLDHEIGHT = 100; /// 월드맵 세로  
    public const int WORLDLEVELGRID = 5; /// 레벨당 간격 

    public const int WORLDLEVELRESGRID = 3; /// 자원월드 간격

    public const int WORLDMAXLMAPLEVEL = (CWGlobal.WORLDWIDTH / CWGlobal.WORLDLEVELGRID)* CWGlobal.WORLDWIDTH / CWGlobal.WORLDLEVELGRID; // 월드맵 최대 레벨 

    //public const int MAXWEAPONCOUNT = 10; //최대 장착 무기수 
    //public const int MAXBUSTERCOUNT = 10; //최대 장착 부스터수  
    //public const int MAXENGINECOUNT = 10; //최대 장착 엔진수
    //public const int MAXBLOCCOUNT = 1696; //최대 장착 엔진수

    static public bool g_bEditmode = false;


    



    // 데이타를 모두 받은 후!
    public static bool G_bGameStart
    {

        get
        {
        
           return g_bGameStart;
        }
        set
        {
            g_bGameStart = value;
        }

    }

    #region 컬러 관련



    
    

    static public COLORNUMBER GetColorItem(GITEM nItem)
    {
        if (nItem == GITEM.Black) return COLORNUMBER.Black;
        if (nItem == GITEM.Blue) return COLORNUMBER.BLUE;
        if (nItem == GITEM.Red) return COLORNUMBER.RED;
        if (nItem == GITEM.Orange) return COLORNUMBER.ORANGE;
        if (nItem == GITEM.Yellow) return COLORNUMBER.YELLOW;
        if (nItem == GITEM.Green) return COLORNUMBER.GREEN;
        if (nItem == GITEM.indigo) return COLORNUMBER.INDIGO;
        if (nItem == GITEM.Purple) return COLORNUMBER.PURPLE;
        if (nItem == GITEM.white) return COLORNUMBER.WHITE;
        if (nItem == GITEM.Skyblue) return COLORNUMBER.SkyBlue;
        if (nItem == GITEM.LightGreen) return COLORNUMBER.LightGreen;
        if (nItem == GITEM.Gray) return COLORNUMBER.Gray;
        if (nItem == GITEM.DarkGray) return COLORNUMBER.DarkGray;
        if (nItem == GITEM.LightGray) return COLORNUMBER.LightGray;
        if (nItem == GITEM.Pink) return COLORNUMBER.Pink;


        if (nItem == GITEM.DarkBlue) return COLORNUMBER.DarkBlue;
        if (nItem == GITEM.Darkpurple) return COLORNUMBER.Darkpurple;
        if (nItem == GITEM.Lightbluegreen) return COLORNUMBER.Lightbluegreen;
        if (nItem == GITEM.Darkyellowgreen) return COLORNUMBER.Darkyellowgreen;
        if (nItem == GITEM.Bluepurple) return COLORNUMBER.Bluepurple;
        if (nItem == GITEM.magenta) return COLORNUMBER.magenta;

        if (nItem == GITEM.Cyanblue) return COLORNUMBER.Cyanblue;
        if (nItem == GITEM.Lightgreencyan) return COLORNUMBER.Lightgreencyan;

        if (nItem == GITEM.scarlet) return COLORNUMBER.scarlet;
        if (nItem == GITEM.Blackgreen) return COLORNUMBER.Blackgreen;
        if (nItem == GITEM.DarkBrown) return COLORNUMBER.DarkBrown;
        if (nItem == GITEM.Brown) return COLORNUMBER.Brown;
        if (nItem == GITEM.amber) return COLORNUMBER.amber;
        if (nItem == GITEM.Deeppink) return COLORNUMBER.Deeppink;
        if (nItem == GITEM.Darkgreen) return COLORNUMBER.Darkgreen;


        if (nItem == GITEM.DarkRed) return COLORNUMBER.DarkRed;
        if (nItem == GITEM.LightRed) return COLORNUMBER.LightRed;
        if (nItem == GITEM.DarkYellow) return COLORNUMBER.DarkYellow;
        if (nItem == GITEM.LightSkyblue) return COLORNUMBER.LightSkyblue;
        if (nItem == GITEM.DarkSkyblue) return COLORNUMBER.DarkSkyblue;
        if (nItem == GITEM.glassGreen) return COLORNUMBER.glassGreen;
        if (nItem == GITEM.Lightblue) return COLORNUMBER.Lightblue;
        if (nItem == GITEM.LightYellowGreen) return COLORNUMBER.LightYellowGreen;
        if (nItem == GITEM.LightYellow) return COLORNUMBER.LightYellow;



        return COLORNUMBER.NONE;

    }
    static public Color GetColor(COLORNUMBER kColor)
    {
        int nItemID= CWArrayManager.Instance.GetColorItemID(kColor);
        return CWArrayManager.Instance.GetColor(nItemID);

    }

    // 칼러를 아이템번호
    static public COLORNUMBER ConvertColorItem(Color kColor)
    {

        int nID= CWArrayManager.Instance.GetColorItem(kColor);
        return CWArrayManager.Instance.GetColorItemIDByColoritem(nID);
        

    }
    // 칼러번호를 블록번호로 
    static public int ConvertItemColorBlock(COLORNUMBER nColor)
    {
        if (nColor == COLORNUMBER.Black) return (int) OLDBLOC.Black;
        if (nColor == COLORNUMBER.BLUE) return (int)OLDBLOC.Blue;
        if (nColor == COLORNUMBER.DarkGray) return (int)OLDBLOC.DarkGray;
        if (nColor == COLORNUMBER.Gray) return (int)OLDBLOC.Gray;
        if (nColor == COLORNUMBER.GREEN) return (int)OLDBLOC.Green;
        if (nColor == COLORNUMBER.INDIGO) return (int)OLDBLOC.indigo;
        if (nColor == COLORNUMBER.LightGray) return (int)OLDBLOC.LightGray;
        if (nColor == COLORNUMBER.LightGreen) return (int)OLDBLOC.LightGreen;
        if (nColor == COLORNUMBER.ORANGE) return (int)OLDBLOC.Orange;
        if (nColor == COLORNUMBER.Pink) return (int)OLDBLOC.Pink;
        if (nColor == COLORNUMBER.PURPLE) return (int)OLDBLOC.Purple;
        if (nColor == COLORNUMBER.RED) return (int)OLDBLOC.Red;
        if (nColor == COLORNUMBER.SkyBlue) return (int)OLDBLOC.SkyBlue;
        if (nColor == COLORNUMBER.WHITE) return (int)OLDBLOC.WHITE;
        if (nColor == COLORNUMBER.YELLOW) return (int)OLDBLOC.Yellow;



        if (nColor == COLORNUMBER.DarkBlue) return (int)OLDBLOC.DarkBlue;
        if (nColor == COLORNUMBER.Darkpurple) return (int)OLDBLOC.Darkpurple;
        if (nColor == COLORNUMBER.Lightbluegreen) return (int)OLDBLOC.Lightbluegreen;
        if (nColor == COLORNUMBER.Darkyellowgreen) return (int)OLDBLOC.Darkyellowgreen;
        if (nColor == COLORNUMBER.Bluepurple) return (int)OLDBLOC.Bluepurple;
        if (nColor == COLORNUMBER.magenta) return (int)OLDBLOC.magenta;

        if (nColor == COLORNUMBER.Cyanblue) return (int)OLDBLOC.Cyanblue;
        if (nColor == COLORNUMBER.Lightgreencyan) return (int)OLDBLOC.Lightgreencyan;

        if (nColor == COLORNUMBER.scarlet) return (int)OLDBLOC.scarlet;
        if (nColor == COLORNUMBER.Blackgreen) return (int)OLDBLOC.Blackgreen;
        if (nColor == COLORNUMBER.DarkBrown) return (int)OLDBLOC.DarkBrown;
        if (nColor == COLORNUMBER.Brown) return (int)OLDBLOC.Brown;
        if (nColor == COLORNUMBER.amber) return (int)OLDBLOC.amber;
        if (nColor == COLORNUMBER.Deeppink) return (int)OLDBLOC.Deeppink;
        if (nColor == COLORNUMBER.Darkgreen) return (int)OLDBLOC.Darkgreen;



        if (nColor == COLORNUMBER.DarkRed) return (int)OLDBLOC.DarkRed;
        if (nColor == COLORNUMBER.LightRed) return (int)OLDBLOC.LightRed;
        if (nColor == COLORNUMBER.DarkYellow) return (int)OLDBLOC.DarkYellow;
        if (nColor == COLORNUMBER.LightSkyblue) return (int)OLDBLOC.LightSkyblue;
        if (nColor == COLORNUMBER.DarkSkyblue) return (int)OLDBLOC.DarkSkyblue;
        if (nColor == COLORNUMBER.glassGreen) return (int)OLDBLOC.glassGreen;
        if (nColor == COLORNUMBER.Lightblue) return (int)OLDBLOC.Lightblue;
        if (nColor == COLORNUMBER.LightYellowGreen) return (int)OLDBLOC.LightYellowGreen;
        if (nColor == COLORNUMBER.LightYellow) return (int)OLDBLOC.LightYellow;
        



        return (int)OLDBLOC.stone;

    }
    //칼러를 블록으로 
    static public int ConvertColorBlock(Color kColor)
    {
        COLORNUMBER kItem= ConvertColorItem(kColor);
        return ConvertItemColorBlock(kItem);
    }

    #endregion


    static public bool IsGoldBlock(int nBlock)// 게임 머니로 바꿀 수 있는 
    {
        int nItemID = CWArrayManager.Instance.GetItemFromBlock(nBlock);
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItemID);
        if (gData.pricesell > 10) return true;
        return false;
    }

    static public int GetResBlock(string szval)
    {

        if (szval == "Gold") return (int)OLDBLOC.GoldBlock;
        if (szval == "Ruby") return (int)OLDBLOC.Ruby;
        if (szval == "Emerald") return (int)OLDBLOC.Emerald;
        if (szval == "Diamond") return (int)OLDBLOC.Diamond;
        return (int)OLDBLOC.stone;
    }
    
  

    
    static GameObject  GetGloblaObject(string szval)
    {

        szval = szval.Trim();
        if (szval.Equals("Hero", StringComparison.CurrentCulture))
        {
            return CWHero.Instance.gameObject;
        }
        if (szval.Equals("ChHero", StringComparison.CurrentCulture))
        {
            return CWChHero.Instance.gameObject;//CWHero.Instance.gameObject;
        }

        if (szval.Equals("Target", StringComparison.CurrentCulture))
        {
            if (CWObject.g_kSelectObject == null) return null;
            return CWObject.g_kSelectObject.gameObject;
        }
      
        if (szval.Equals("MainCamera", StringComparison.CurrentCulture))
        {
            if (Camera.main == null)
            {
                //Debug.LogError("메인카메라 실종!!");
                return null;
            }
                
            return Camera.main.gameObject;
        }
/*
        // 현재 행성의 카메라 위치 
        if (szval.Equals("NowPlanetCampos", StringComparison.CurrentCulture))
        {
            //GameObject gg = CWMySola.Instance.m_gSelectPlanet;
            //return CWLib.FindChild(gg, "Campos");
        }
        if (szval.Equals("NowPlanet", StringComparison.CurrentCulture))
        {
            //GameObject gg = CWMySola.Instance.m_gSelectPlanet;
            //return gg;
        }
        if (szval.Equals("SelectPlanetCampos", StringComparison.CurrentCulture))
        {
            //GameObject gg = CWMySola.Instance.m_gSelectPlanet;
            //return CWLib.FindChild(gg, "Cam_Planet");
        }

        if (szval.Equals("SelectPlanet", StringComparison.CurrentCulture))
        {
            //GameObject gg = CWMySola.Instance.m_gSelectPlanet;
            //return gg;
        }
*/
        // 나를 죽인 유저 

        if (szval.Equals("MyKiller", StringComparison.CurrentCulture))
        {
            if (CWHero.Instance.m_gKiller == null) return null;
            return CWHero.Instance.m_gKiller.gameObject;


        }
        //MyKiller

        return null;
    }

    static public GameObject FindObject(string szName)
    {
        if (!CWLib.IsString(szName)) return null;
        if (CWLib.IsDigit(szName)) return null;

        GameObject gRet = CWGlobal.GetGloblaObject(szName);
        if (gRet == null)
        {

            gRet = CWLib.FindChild(Game_App.Instance.m_gUIDir, szName);
            if (gRet == null)
            {
                   gRet = CWLib.FindChild(Game_App.Instance.gameObject, szName);
            }

        }
        if(gRet==null)
        {
            Debug.Log(string.Format("{0}을 못찾았다 ", szName));
        }

        return gRet;
    }


    
    static public string GetSolaGrade(int nSolaID)
    {
        if (nSolaID == 1) return "Bronze";
        if (nSolaID == 2) return "Silver";
        if (nSolaID == 3) return "Gold";
        if (nSolaID == 4) return "Platinum";
        if (nSolaID == 5) return "Diamond";
        if (nSolaID == 6) return "Master";
        if (nSolaID == 7) return "Grandmaster";
        return "challenge";

    }
    
    // 행성 등급

    static public string GetGradestring(int nGrade)
    {

        int val = nGrade;
        if (val == 0) return "브론즈";
        if (val == 1) return "실버";
        if (val == 2) return "골드";
        if (val == 3) return "플래티넘";
        if (val == 4) return "다이아몬드";
        if (val == 5) return "마스터";
        if (val == 6) return "그랜드마스터";
        return "챌린저"; 



    }
    // 1


    static public string GetGradeFileName(int nGrade)
    {
        int val = nGrade;
        if (val == 0) return "bronze";
        if (val == 1) return "silver";
        if (val == 2) return "gold";
        if (val == 3) return "platinum";
        if (val == 4) return "diamond";
        if (val == 5) return "master"; //
        if (val == 6) return "Grandmaster"; //
        return "challenge";


    }
    static public string GetGradeCircleFileName(int nGrade)//
    {
        int val = nGrade;
     
        if (val == 0) return "bronze_ring";
        if (val == 1) return "silver_ring";
        if (val == 2) return "gold_ring";
        if (val == 3) return "platinum_ring";
        if (val == 4) return "diamond_ring";
        if (val == 5) return "master_ring";
        if (val == 6) return "grandmaster_ring";
        if (val == 7) return "challege_ring";
        return "bronze_ring";


    }
    static public string GetGradeStr(int nlevel)
    {
        if (nlevel == 1) return "F";
        if (nlevel == 2) return "E";
        if (nlevel == 3) return "E+";
        if (nlevel == 4) return "D";
        if (nlevel == 5) return "D+";
        if (nlevel == 6) return "D++";
        if (nlevel == 7) return "C-";
        if (nlevel == 8) return "C";
        if (nlevel == 9) return "C+";
        if (nlevel == 10) return "C++";

        if (nlevel == 11) return "B-";
        if (nlevel == 12) return "B";
        if (nlevel == 13) return "B+";
        if (nlevel == 14) return "B++";

        if (nlevel == 15) return "A-";
        if (nlevel == 16) return "A";
        if (nlevel == 17) return "A+";
        if (nlevel == 18) return "A++";

        if (nlevel == 19) return "S-";
        if (nlevel == 20) return "S";
        if (nlevel == 21) return "S+";
        if (nlevel == 22) return "S++";


        if (nlevel == 23) return "SS-";
        if (nlevel == 24) return "SS";
        if (nlevel == 25) return "SS+";
        if (nlevel == 26) return "SS++";
        return "";

    }
    // 아이템 등급 
    static public string GetGradeItemName(int nlevel)
    {
        string str = CWLocalization.Instance.GetLanguage("{0}등급");
        return string.Format(str, GetGradeStr(nlevel));


        
    }
    static public string GetSolaNameByRoom(int roomnumber)
    {
        int s = CWArrayManager.Instance.GetSunID(roomnumber);
        string str = CWLocalization.Instance.GetLanguage("Sun {0}");
        return string.Format(str, s);

    }

    static public string GetPlanetName(int nStage)
    {
        //
        int nPlanet = ((nStage - 1) / 6) % 6+1;

        return GetGradestring(nPlanet);
    }

    static public string GetRoomName(int roomnumber)
    {
        int p= CWArrayManager.Instance.GetPlanetID(roomnumber);
        int s= CWArrayManager.Instance.GetSunID(roomnumber);

        string str = CWLocalization.Instance.GetLanguage("{0}번 태양 {1}행성 {2}구역");
        return string.Format(str,s,p,((roomnumber-1)%6)+1);

    }
    // 은하계 거리 
    static public int GetGalaxyDist(Vector3 vPos)
    {
        Vector3 v = CWHero.Instance.GetPosition();
        float fdist = Vector3.Distance(v, vPos);
        return(int)(fdist / 1000);
    }

    static public string StartAir()// 시작 비행기
    {
        return g_StartAir;//하나로 통일 
    }
    static public string GetCurrentArea(int num)
    {
        if (num == 0) return "첫번째 면";
        if (num == 1) return "두번째 면";
        if (num == 2) return "세번째 면";
        if (num == 3) return "네번째 면";
        if (num == 4) return "다섯째 면";
        if (num == 5) return "BOSS";

        return "";

    }
    static public string GetCurrentPlanet(int num)
    {
        if (num == 0) return "첫번째 행성";
        if (num == 1) return "두번째 행성";
        if (num == 2) return "세번째 행성";
        if (num == 3) return "네번째 행성";
        if (num == 4) return "다섯째 행성";
        if (num == 5) return "여섯째 행성";

        return "";

    }

    // 태양계 이름 
    static public string GetSolaName(int num)
    {
        if (num == 1) return "SUN # 01";
        if (num == 2) return "SUN # 02";
        if (num == 3) return "SUN # 03";
        if (num == 4) return "SUN # 04";
        if (num == 5) return "SUN # 05";
        if (num == 6) return "SUN # 06";

        
        return "";
    }


    // 코인 유형 
    // 1: 골드
    // 2: 보석
    // 3: 광고
    // 4: 현금

    static public string GetPrice(int nPrice,int cType, string szPID)
    {
        if (cType != 4) return nPrice.ToString();
        return CWArrayManager.Instance.GetPrice(szPID);

    }
    // Sprite
    static public string GetCoinSpriteStr(int cType)
    {
        if(cType==1)
        {
            return "gold";
        }
        if (cType == 2)
        {
            return "gem";
        }
        if (cType == 3)
        {
            return "Icon_Vedio";
        }
        if (cType == 4)
        {
            int nval = CWPrefsManager.Instance.GetLanguage();// 현재 랭귀지 
            if (nval == 0)// 한국어 
            {
                return "Won";
            }
            else if (nval == 2)// 일본
            {
                return "Yen";
            }
            else
            {
                return "Dollar";
            }

        }
        if (cType == 5)// 티켓
        {
            return "ticket";
        }

        return "";
    }

    static public void Buy(int cType, int Price, string szPID, Action func)
    {

        if (cType == 1)
        {
            if(Price==0)
            {
                if (func != null) func();
                return;
            }
            CWSocketManager.Instance.UseCoin(COIN.GOLD, -Price, (jData) => {

                if (jData["Result"].ToString() == "ok")
                {
                    CWCoinManager.Instance.SetData(jData["Coins"]);
                    if (func != null)
                    {
                        func();
                    }
                }

            }, "ReceiveData");

        }
        if (cType == 2)
        {
            if (Price == 0)
            {
                if (func != null) func();
                return;
            }

            CWSocketManager.Instance.UseCoin(COIN.GEM, -Price, (jData) => {

                if (jData["Result"].ToString() == "ok")
                {
                    CWCoinManager.Instance.SetData(jData["Coins"]);
                    if (func != null)
                    {
                        func();
                    }
                }

            }, "ReceiveData");


        }
        if (cType == 5)
        {
            if (Price == 0)
            {
                if (func != null) func();
                return;
            }

            CWSocketManager.Instance.UseCoin(COIN.TICKET, -Price, (jData) => {

                if (jData["Result"].ToString() == "ok")
                {
                    CWCoinManager.Instance.SetData(jData["Coins"]);
                    if (func != null)
                    {
                        func();
                    }
                }

            }, "ReceiveData");


        }
        if (cType == 3)
        {
            CWADManager.Instance.RewardShow(() => {

                AnalyticsLog.Print("ADLog", "Stor AD");
                func();
            });

        }
        if (cType == 4)
        {
            CWCashManager.Instance.BuyProductID(szPID, (bRet) =>
            {
                AnalyticsLog.Print("Cash", "Stor", szPID);
                if (bRet)
                {
                    func();
                    CWSocketManager.Instance.UpdateUser("SafeUser", "1");// 현금결제
                }
                else
                {
                    NoticeMessage.Instance.Show("결제를 실패하였습니다!");
                }
            });

        }

    }

    public static int GetRanomBomb(bool bHigh)
    {
        /*
        int RR = CWLib.Random(0, 100);
        int nItem = (int)GITEM.Bomb1;
        if(bHigh)
        {
            if (RR >= 5 && RR < 30)
            {
                nItem = (int)GITEM.Bomb2;
            }
            if (RR >= 30 && RR < 60)
            {
                nItem = (int)GITEM.Bomb3;
            }
            if (RR >= 60 && RR < 80)
            {
                nItem = (int)GITEM.Bomb3;
            }
            if (RR >= 80 && RR < 90)
            {
                nItem = (int)GITEM.Bomb4;
            }
            if (RR >= 90 && RR < 100)
            {
                nItem = (int)GITEM.Bomb5;
            }

        }
        else
        {
            if (RR >= 20 && RR < 60)
            {
                nItem = (int)GITEM.Bomb2;
            }
            if (RR >= 60 && RR < 80)
            {
                nItem = (int)GITEM.Bomb3;
            }
            if (RR >= 80 && RR < 90)
            {
                nItem = (int)GITEM.Bomb3;
            }
            if (RR >= 90 && RR < 95)
            {
                nItem = (int)GITEM.Bomb4;
            }
            if (RR >= 95 && RR < 100)
            {
                nItem = (int)GITEM.Bomb5;
            }

        }
        return nItem;
*/
        return 0;
    }
    // 골드는 유저의 레벨에 따라 수치가 달라진다
    //시세에 따른 골드가격 보석당 골드 가격 
    // 보석 5개당 1레벨을 올린다는 가정
    public static int GetGoldbyPrice(int nPrice)
    {
        int Gold = (CWHeroManager.Instance.NextUpgradeGold)* nPrice;// 다음 업그레이드 가격
        if (Gold <= 0) return 1;
        return Gold;


    }

    public static string GetPlanetLocalName(int nstage)
    {
        return string.Format("map_{0}_{1}",CWHero.Instance.m_nID,nstage);
    }
    public static string GetMyLocalName()
    {
        return string.Format("mymap_{0}", CWHero.Instance.m_nID);
    }
    public static string GetLocalFile()
    {
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MYROOM)
        {
            return GetMyLocalName();

        }
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.SINGLE)
        {
            return GetPlanetLocalName(Space_Map.Instance.GetStageID());
        }

        return "";
    }

    public static bool CheckLoacalfile(string szlocalfile)
    {
        if (!CWLib.IsString(szlocalfile)) return false;
        string szpath = string.Format("{0}/{1}", Application.persistentDataPath, szlocalfile);

        if (File.Exists(szpath))
        {
            return true;
        }
        return false;
    }

    public static int GetGrade(int RankPoint)
    {

        for (int i = 0; i < 10; i++)
        {
            if (CWGlobal.RANKING_POINT[i] == 0) continue;
            if (RankPoint < CWGlobal.RANKING_POINT[i])
            {
                return i;
            }

        }

        return 7;

    }


}
