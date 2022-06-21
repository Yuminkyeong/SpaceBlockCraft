using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System;
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
using CWStruct;
using CWUnityLib;
using CWEnum;
using UnityEngine.Networking;
public class CWHeroManager : CWManager<CWHeroManager>
{
    #region 내구력

    public int m_nRepairValue = 0;
    public DateTime Repairtime= DateTime.MinValue;
    // 선택한 비행기의 내구력
    // 전투 참여 가능한가?
    
        // 내구력이 없어서 전투 못함
    public bool IsDontUseShip()
    {
        if (m_nRepairValue <= 0) return true;
        if (Repairtime != DateTime.MinValue) return true;
        return false;
        
    }
    // 현재 수리중
    public bool IsRepairIng()
    {
        if (Repairtime != DateTime.MinValue) return true;
        return false;

    }


    // 수리할 상황인지 체크
    public bool CheckRepair(int RepairRate,DateTime dt)
    {
        if (RepairRate == 100) return false;
        if (dt == DateTime.MinValue) return false;
        TimeSpan ss = DateTime.Now - dt;
        int rr = (100 - RepairRate) * CWGlobal.REPAIRTIME;
        if ((int)ss.TotalSeconds >= rr) return false;// 수리를 했음

        return true;
    }
    // 수리 완료
    public void ClearRepair()
    {
        
        m_nRepairValue = 100;
        Repairtime = DateTime.MinValue;
    }
    public void StartRepair()
    {
        Repairtime = DateTime.Now;
    }

    #endregion

    #region 멀티맵 

    public int[] m_MultiMapLists;

    #endregion

    #region 맵 클리어 개념

    // Stage를 클리어 했는가>
    // 
    public int GetStageValue(int nStage)
    {
        if (m_nStageNumber > nStage) return 1;
        return 0;

    }
    public float GetStageRate(int nStage)
    {
        float aa= GetTotal(nStage);
        float bb= GetAdd(nStage);
        if (aa == 0) return 0;
        return bb / aa;

    }
    // 0 모두 캔맵 1 하나도 안캔맵 //0~1


    // 맵을 클리어 했다

    #endregion
    #region 맵 저장데이타

    class MapData
    {
        public int m_nTotalCount;
        public int m_nAddCount;


        public int m_nResCount2;
        public int m_nResCount3;


    }
    List<MapData> m_kMapSaveData = new List<MapData>();


    public void SetTotal(int stage, int val)
    {

        m_kMapSaveData[stage].m_nTotalCount = val;
    }

    public void SetAdd(int stage, int val)
    {
        m_kMapSaveData[stage].m_nAddCount = val;
    }
    
    public void SetRes2(int stage, int val)
    {
        m_kMapSaveData[stage].m_nResCount2 = val;
    }
    public int GetRes2(int stage)
    {
        return m_kMapSaveData[stage].m_nResCount2;
    }


    
    public int GetTotal(int stage)
    {
        if (stage >= m_kMapSaveData.Count) return 0;
        return m_kMapSaveData[stage].m_nTotalCount;
    }

    public int GetAdd(int stage)
    {
        return m_kMapSaveData[stage].m_nAddCount;
    }

    public void SetRes3(int stage, int val)
    {
        m_kMapSaveData[stage].m_nResCount3 = val;
    }
    public int GetRes3(int stage)
    {
        return m_kMapSaveData[stage].m_nResCount3;
    }
   

    public void SaveMapData(int stage)
    {

        CWSocketManager.Instance.SaveMapData(stage, GetTotal(stage),GetAdd(stage),GetRes2(stage), GetRes3(stage));

    }
    public void LoadMapData(JArray array)
    {
        m_kMapSaveData.Clear();
        for (int i = 0; i < 255; i++)
        {
            MapData kMapdata = new MapData();
            m_kMapSaveData.Add(kMapdata);
        }

        if (!CWLib.IsJSonData(array)) return;
        for (int i = 0; i < array.Count; i++)
        {
            JToken jt = array[i];
            if (jt == null)
            {
                continue;
            }
            MapData kMapdata = m_kMapSaveData[i];
            kMapdata.m_nTotalCount = CWJSon.GetInt(jt, "total");
            kMapdata.m_nAddCount = CWJSon.GetInt(jt, "add");
            kMapdata.m_nResCount2 = CWJSon.GetInt(jt, "res2");
            kMapdata.m_nResCount3 = CWJSon.GetInt(jt, "res3");

        }

    }


    #endregion

    #region 오늘 처음 들어왔다 !!

    public bool m_bTuto;
    //오늘만 사는 상품 개념 
    // 오늘 사용한 상품인가?
    //List<int> m_kTodayGoods = new List<int>();

    CWJSon m_kTodayData = new CWJSon();
    
    public bool IsUseGoods(int nID)
    {


        int nn=m_kTodayData.GetInt(nID.ToString());
        if (nn == 0) return false;
        return true;
    }
    public void AddTodayGoods(int nID)
    {
        m_kTodayData.Add(nID.ToString(), 1);

        CWPlayerPrefs.SetString("TodayGoods",m_kTodayData.ToString());

    }

    void __Work()
    {
        if (CWGlobal.g_bToday)
        {
            m_kTodayData = new CWJSon();

            DailyDlg.Instance.Open();
            Dailymission.Instance.Create();
        }
        else
        {

           

            Dailymission.Instance.Load();
            m_kTodayData.LoadString(CWPlayerPrefs.GetString("TodayGoods"));
        }
        m_bTuto = false;
    }

    public void TodayWork()
    {
        if(m_bTuto)
        {

            CWProductionRoot pt = CWResourceManager.Instance.GetProduction("TutoProduction");
            pt.Begin(__Work);
        }
        else
        {
            __Work();
        }
        
    }


    #endregion

    #region 좋아요 리스트 
    List<int> m_kLikeUser = new List<int>();

    public int GetLikeCount()
    {
        return m_kLikeUser.Count;
    }
    public int m_nLike;// 나의 좋아요 
    public bool SendAddLike(int UserID)
    {
        if (UserID == CWHero.Instance.m_nID) return true;
        bool bFlag = false;
        if(m_kLikeUser.Exists(x => x== UserID))
        {
            bFlag = true;// 이미 존재 : -1 
            m_kLikeUser.Remove(UserID);
        }
        else
        {
            m_kLikeUser.Add(UserID);
        }
        CWSocketManager.Instance.SendAddLike(UserID, bFlag, m_kLikeUser, (jData)=> {


        });

        return bFlag;
    }
    public bool IsLikeUser(int LikeUser)
    {
        if (LikeUser == CWHero.Instance.m_nID) return true;
        if (m_kLikeUser.Exists(x => x == LikeUser))
        {
            return true;// 이미 존재 
        }

        return false;
    }




    #endregion

    #region 행성관련


    
    public const int MAXSOLA = 14; // 항성 개수 
    public const int MAXSTAGE = 36* MAXSOLA;

    public int m_nStageNumber;// 현재 정복해야 되는 행성 번호 

    // 1부터 시작되는 태양계 절대값
    public int m_nSolaID
    {
        get
        {
            return CWArrayManager.Instance.GetSunID(Space_Map.Instance.GetStageID());
        }

    }
    //1 부터 시작되는 행성 번호 절대값!!!!!!!!!!!!!
    public int m_nPlanetID
    {
        get
        {
           
            return CWArrayManager.Instance.GetPlanetID(Space_Map.Instance.GetStageID());
        }
    }


    // 정복한 행성 개수
    public int GetMyPlanetCount()
    {
        return (m_nStageNumber - 1)/6 + (m_nStageNumber - 1) % 6;
    }
    //정복한 스테이지 개수 
    public int GetMyStageCount()
    {
        return m_nStageNumber;
    }


    

    // 전투를 완료했는가?
    public bool IsEndTask(int nStage)//
    {
        if (m_nStageNumber > nStage) return true;
        return false;

    }
    public int GetNextStage()
    {

        //6개 

        int start = (m_nPlanetID - 1) * 6+1;

        for(int i=start;i<6+ start; i++)
        {
            if(!IsEndTask(i))
            {
                return i;
            }
        }
        return 0;
    }
    public bool IsVictoryPlanet(int nPlanet)
    {
        return (GetEndTaskPlanet(nPlanet) == 6);
    }
    public int GetEndTaskPlanet(int nPlanet)
    {
        if (nPlanet == CWGlobal.m_nMultiPlanet) return 0;

        int ncnt = 0;

        int start = (nPlanet - 1) * 6 + 1;

        for (int i = start; i < 6+start; i++)
        {
            if (IsEndTask(i))
            {
                ncnt++;
            }
        }

        return ncnt;
    }
   
    // 현재 있는 행성에 맞게 룸을 리턴 
    public int GetRoomNumber()
    {
        return CWGlobal.m_nMultiRoomNumber;// 유저가 늘어나면 변경
    }

    // 개념 바꿈, 정복한 면은 들어 갈 수 있다
    // 입장이 가능한가?
    public bool IsEnterPlanet(int nPlanet,int nStage=-1)
    {
        // 멀티맵은 현재로서는 무조건 입장이 가능하다
        if (Space_Map.Instance.m_nType == 4) return true;


        int nCnt = GetMyPlanetCount();
        if (Space_Map.Instance.m_nType == 0)
        {
            //if (!IsEndTask(nStage)) return true;// 전투 할 지역
            //return false;
            return true; // 무조건 들어 간다
        }
        else
        {
            // 행성은 차례대로 깨야 한다 개념 
            // 현재 내가깬 행성 개수 
            if (nPlanet <= nCnt + 1) return true;

            return false;

        }

    }





    #endregion

    #region 모양 관련

    const int SHAPECOUNT = 7;
    bool[] m_kShape = new bool[SHAPECOUNT];

    public bool GetShape(int number)
    {

        return m_kShape[number-1];
    }
    public void UpdateShape(int number)
    {
        m_kShape[number-1]=true;
        CWSocketManager.Instance.UpdateShape(number-1, ReceiveShape, "ReceiveShape");
    }
    void ReceiveShape(JObject jData)
    {
        ReceiveShapeData((JArray)jData["Shape"]);
    }
    void ReceiveShapeData(JArray ja)
    {
        if (ja == null) return;
        for (int i=0;i<ja.Count;i++)
        {
            if(ja[i]!=null)
            {
                if (ja[i].ToString() == "") continue;
                m_kShape[i] = true;
            }
                
        }
    }

    #endregion

    

    #region 친구관련

    List<int> m_kFriend = new List<int>();
    List<int> m_kMyAskFriend = new List<int>(); // 내가 친구 요청한 리스트들
    List<int> m_kGameFriend = new List<int>();
    List<int> m_kUseFriend = new List<int>();

    public int CallFriend()
    {
        if (m_kGameFriend.Count == 0) return 0;
       int num=UnityEngine.Random.Range(0, m_kGameFriend.Count);
       int nID = m_kGameFriend[num];

        m_kGameFriend.Remove(nID);

        m_kUseFriend.Add(nID);
        CWDayValueManager.Instance.SetArray<int>("UseFriend", m_kUseFriend);
        return nID;
    }
    public bool IsGameFriend()
    {
        if (m_kGameFriend.Count == 0) return false;
        return true;
    }
    void ReceiveFriendList(JObject jData)
    {
        if (jData["Result"].ToString() == "ok")
        {

            if(jData["List"].Type == JTokenType.Array)
            {
                JArray jj = (JArray)jData["List"];
                JObject jList = (JObject)jj[0];


                m_kGameFriend.Clear();
                m_kFriend.Clear();
                m_kFriend.AddRange(jList["Friend"].ToObject<int[]>());
                foreach (var v in m_kFriend)
                {
                    if(m_kUseFriend.Exists(x => (x==v)))
                    {
                        continue;// 있으면 통과
                    }
                    m_kGameFriend.Add(v);

                }

               // CWDayValueManager.Instance.SetArray<int>("Friend", m_kFriend);

            }


        }
        else
        {
            print(" fail!!");
        }

    }
    public void AskFriendList()
    {
        
        string szfind = string.Format(@"{{""_id"": {0}}}", GetIDX());
        string szSelect = string.Format("Friend");
        string szSort = string.Format("");
        string szLimit = string.Format("");
        string szStart = string.Format("");


        
        int nVal= CWDayValueManager.Instance.GetInt("LoadFriend");
        if (nVal==0)
        {
            CWDayValueManager.Instance.SetInt("LoadFriend",1);// 사용했음
            
        }
        m_kUseFriend = CWDayValueManager.Instance.GetArray<int>("UseFriend");
        if(m_kUseFriend==null)
        {
            m_kUseFriend = new List<int>();
        }

        CWSocketManager.Instance.UserList(szfind, szSelect, szSort, szLimit, szStart, ReceiveFriendList, "ReceiveFriendList");

    }
    // 친구 인가?
    public bool IsFriend(int nID)
    {
        if (m_kFriend.Exists(x => (x == nID))) return true;
        return false;
            
    }
    // 요청한 친구인가?
    public bool IsAskFriend(int nID)
    {
        if (m_kMyAskFriend.Exists(x => (x == nID))) return true;
        return false;
    }
    public void AddAskFriend(int nID)
    {
        m_kMyAskFriend.Add(nID);

        CWUserFileManager.Instance.SetArray<int>("MyfriendAsk",m_kMyAskFriend);
    }

    #endregion

    #region 시간단축관련

    class TimerItemData
    {
        public int m_Coooltime;
        public DateTime m_Begintime;
        public int m_term;
        //음수값이라면, 사용안하는 것
        public float GetRepairTime()
        {
            if (m_term == 0) return 0;
            TimeSpan ts = DateTime.Now - m_Begintime;
            return m_term -(int)ts.TotalSeconds;
        }
    }



    // 일딴 하나로 통일
    TimerItemData m_kRepairTimer= new TimerItemData();

    public void TimerJData(JToken jData)
    {
        if (jData == null) return;
        JToken jt = jData;
        if (jt["Begintime"] == null) return;
        if(jt["Begintime"].ToString().Length>1)
        {
            m_kRepairTimer.m_Begintime = jt["Begintime"].Value<DateTime>();
            m_kRepairTimer.m_Coooltime = jt["Cooltime"].Value<int>();
            m_kRepairTimer.m_term = jt["Term"].Value<int>();
        }



    }
    public int GetRepairCooltime()
    {
        return m_kRepairTimer.m_Coooltime;
    }
    public int GetPotionCooltime()
    {
        //return m_kPotionTimer.m_Coooltime;
        return m_kRepairTimer.m_Coooltime;
    }

    //남은 시간 초
    public float GetRepairTime()
    {
        return m_kRepairTimer.GetRepairTime();
    }
    public float GetPotionTime()
    {
        return m_kRepairTimer.GetRepairTime();
        //return m_kPotionTimer.GetRepairTime();
    }


    #endregion

    #region 주인공능력치 관련 


    int _nCharNumber;

    public int m_nCharNumber
    {
        get
        {
            return _nCharNumber;
        }
        set
        {
            _nCharNumber = value;
        }
    }

    // 다음 번 공격력 업그레이드 가격
    public int NextUpgradeGold
    {

        get
        {
            int Damagelevel = GetWeaponDamageLevel(1);
            int nGold = GetWeaponDamageGold(CWHero.Instance.m_nSelectWeaponType, Damagelevel +1);
            return nGold;
        }
    }

    public int m_nAirBlockCount=24;// 블록 수 
    private int nSlotLevel;

    

    public bool[] m_bFirstData = new bool[100];
    public void UpdateFirstData(JArray ja)
    {
        if (ja == null) return;
        for(int i=0;i<ja.Count;i++)
        {
            if (ja[i] == null) continue;

            int tt = CWLib.ConvertInt(ja[i].ToString());
            if (tt == 1) m_bFirstData[i] = true;
        }
    }

    //토탈 범위
    public int GetTotalRange()
    {
        int count = 0;
        for(int i=0;i< m_nWeaponCount; i++)
        {
            int lv= GetWeaponRangeLevel(i+1);
            count+=GetWeaponRange(lv);
        }
        return count;
        
    }

    #endregion
    #region PVP 결과

    public int m_nPVPTotal = 0;
    public int m_nPVPWin = 0;


    #endregion
    #region 무기슬롯 

    public int m_nWeaponCount; // 보유 무기 개수 
    public List<WEAPONSLOT> m_kWeaponSlot = new List<WEAPONSLOT>();
    public void UpdateWeaponSlot(JArray ja)
    {
        m_kWeaponSlot.Clear();
        for (int i=0;i<ja.Count;i++)
        {
            CWJSon jj = new CWJSon((JObject)ja[i]);
            
            WEAPONSLOT ws = new WEAPONSLOT();
            ws.DamageLv = jj.GetInt("Damage");
            ws.SpeedLv = jj.GetInt("Speed");
            ws.RangeLv = jj.GetInt("Range");

            m_kWeaponSlot.Add(ws);
        }
    }
    // 다음 업그레이드 가격
  
    public int GetWeaponDamageGold(int nType , int nlevel)
    {
        if(nType==1)
            return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "골드1", nlevel);
        if (nType == 2)
            return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "골드2", nlevel);
        if (nType == 3)
            return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "골드3", nlevel);

        return 0;
    }

    public int GetWeaponDamage(int nType, int nlevel)
    {
        if(nType==1)
        {
            return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "기관총", nlevel );
        }
        if (nType == 2)
        {
            return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "미사일", nlevel );
        }
        if (nType == 3)
        {
            return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "레이저", nlevel );
        }

        return 0;
    }


    public int GetWeaponRangeGold(int nlevel)
    {
        return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "골드3", nlevel);
    }
    public int GetWeaponRange(int nlevel)
    {
        return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "범위", nlevel);
    }


    public int GetWeaponSpeedGold(int nlevel)
    {
        return CWTableManager.Instance.GetTableInt("무기업그레이드 - 시트1", "골드2", nlevel);
    }
    public float GetWeaponSpeed(int nlevel)
    {
        return CWTableManager.Instance.GetTableFloat("무기업그레이드 - 시트1", "속도", nlevel);
    }
    public string GetWeaponIcon(int nlevel)
    {
        return CWTableManager.Instance.GetTable("무기업그레이드 - 시트1", "icon", nlevel);
    }
    public string GetWeaponmissile(int nlevel)
    {
        return CWTableManager.Instance.GetTable("무기업그레이드 - 시트1", "missile", nlevel);
    }


    public int GetWeaponDamageLevel(int nSlot)// 1부터 시작 
    {
        if (nSlot <= 0) return 0;
        if (nSlot > m_kWeaponSlot.Count) return 0;
        int num = nSlot-1;
        return m_kWeaponSlot[num].DamageLv;
    }
    public int GetWeaponRangeLevel(int nSlot)// 1부터 시작 
    {
        if (nSlot <= 0) return 0;
        if (nSlot > m_kWeaponSlot.Count) return 0;
        int num = nSlot - 1;
        return m_kWeaponSlot[num].RangeLv;
    }

    public int GetWeaponSpeedLevel(int nSlot)// 1부터 시작 
    {
        if (nSlot <= 0) return 0;
        if (nSlot > m_kWeaponSlot.Count) return 0;
        int num = nSlot - 1;
        return m_kWeaponSlot[num].SpeedLv;
    }



    #endregion
    #region 부스터 

    public int m_nBusterCount;
   


   public List<int> m_kBusterSlot = new List<int>();
   

    public void UpdateBusterSlot(JArray ja)
    {
        m_kBusterSlot.Clear();
        for(int i=0;i<ja.Count;i++)
        {

            m_kBusterSlot.Add(CWLib.ConvertIntbyJson(ja[i]));
        }
        
    }

    public int GetBusterLevel(int num)// 1부터 시작
    {
        if (num <= 0) return 0;
        if (num > m_kBusterSlot.Count) return 0;
       return m_kBusterSlot[num-1];
    }



    public float GetBusterSpeed(int lv)
    {
        float val= CWTableManager.Instance.GetTableFloat("무기업그레이드 - 부스터", "speed", lv);
        return val * 80;
    }
    public int GetBusterGold(int lv)
    {
        return CWTableManager.Instance.GetTableInt("무기업그레이드 - 부스터", "골드", lv);

    }

    public float GetSpeed()
    {

        //int lv= GetBusterLevel(1);
        //int lv2= GetBusterLevel(2);
        int lv = 100;
        int lv2 = 100;
        float fv1= GetBusterSpeed(lv);
        float fv2= GetBusterSpeed(lv2);

        return 10 + fv1/100 + fv2/100;
    }
    public float GetSpeedL()
    {

        int lv = 100;//GetBusterLevel(1);
        float fv1 = GetBusterSpeed(lv)*3f;
        return 10 + fv1 / 100 ;
    }

    public float GetSpeedR()
    {

        int lv2 = 100;// GetBusterLevel(2);

        float fv2 = GetBusterSpeed(lv2) * 3f;

        return 10 + fv2 / 100;
    }


    #endregion
    #region 변수

    public bool m_bBlockUser;// 차단 유저

    public GAMETYPE m_GameType;

    public CWObject m_gHero;

    private float[] fNowTong = new float[4];

    public int m_nRanking = 0;
    

    public int m_nRankNumber = 1;// 랭킹 순서 : 순서로 나열됨

    public int ADCount = 0;// 광고 본 횟수
    public int m_nRankPoint = 1;
    public int m_nWinScore = 0;// 지금 얻은 점수 

    public int m_nVIP;// vip 레벨
    public int m_nCharLevel;// 유저 충성레벨

    public bool m_bGradUP = false;
    // 행성 1

    public int m_nAirSlotID = 0;
    public int m_nGrade
    {
        get
        {
            return CWGlobal.GetGrade(m_nRankPoint);
        }
    }

    


    public int m_nPrice
    {
        get
        {
            return CWHero.Instance.GetPrice();
        }
    }

    public int m_nNowPoint = 0;// 현재 얻은 포인트


    

    public bool m_bAdDel = false;// 광고제거가 되어있나?

    public int MultiCount = 0;// 멀티 횟수
    public int MultiPrice
    {
        get
        {
            //return CWGlobal.MULTIENTERPRICE* ( (MultiCount+2)- CWGlobal.MULTICOUNT);
            return CWGlobal.MULTIENTERPRICE;
        }
    }

    #endregion


    #region 클랜
    public int m_nClanID;
    public int m_nClanLeaderID;// 클랜장 아이디 
    List<int> m_kAskClan = new List<int>(); // 내가 신청한 클랜 리스트

    public bool IsClanLeader()
    {
        if (m_nClanLeaderID == CWHero.Instance.m_nID) return true;
        return false;
    }
    public void AddAskClan(int nClanId)
    {
        m_kAskClan.Add(nClanId);
    }
    public bool IsAskClan(int nClanId)
    {
        return m_kAskClan.Exists(x => x == nClanId);
    }
    #endregion



    public int NPreRanking
    {
        get
        {
            return CWPlayerPrefs.GetInt("PreRank");
        }
        set
        {
            CWPlayerPrefs.SetInt("PreRank", value);
        }

    }

    

    public int NSlotLevel //{ get => nSlotLevel; set => nSlotLevel = value; }
    {
        get
        {
            return nSlotLevel;
        }
        set
        {
            nSlotLevel = value;
            m_nAirBlockCount = CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "blockcount", nSlotLevel); 
            
        }

    }

    //0이면 없음 1 보상 존재 2: 보상 받았음
    public List<int> m_kDailyList = new List<int>();
    public int m_nDayCount;


        

    public CWJSon m_kDroneAIData=new CWJSon();
    


    public CWJSon m_kGroupAIDrone=new CWJSon();
    public CWJSon m_kAITurretUnit = new CWJSon();
    public CWJSon m_kAIBossUnit = new CWJSon();
    public CWJSon m_kAIGroup = new CWJSon();

    public CWJSon m_kAIHeroParming = new CWJSon();// 주인공 자동 파밍하기 
    public CWJSon m_kFakeUserAI = new CWJSon();//
    public CWJSon m_kFightUserAI = new CWJSon();//

    




    static int CampareSortInven(int a, int b)
    {
        if (a == 0) a = 10000;
        if (b == 0) b = 1000;
        return a - b;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //각 타입에 맞게 월드 좌표를 리턴 




    public Vector3 GetPosition()
    {


        if (m_gHero == null) return Vector3.zero;
        return m_gHero.GetPosition();
    }

    // 인벤 포함해서 현재 최대 데미지
    // 엔진당 무기수에 대한 정의 
    // 같은 수로 올라간다!!
    public int GetMaxDamage()
    {

        return CWTableManager.Instance.GetTableInt("UserLevel", "최대데미지", CWHero.Instance.NLevel);
    }

    public int GetMaxBlockCount()
    {
        return CWTableManager.Instance.GetTableInt("UserLevel", "블록수", CWHero.Instance.NLevel);
        
    }


    public CWPower GetPower()
    {
        return m_gHero.GetComponent<CWPower>();
    }
    
    public CWObject GetHero()
    {
        return m_gHero;
    }

    public int  GetIDX()
    {
        if (m_gHero == null) return 0;
        return m_gHero.m_nID;
    }
    public override void Create()
    {
        StartCoroutine("IRun");
        base.Create();
    }
    // 유닛 업데이트 관련 
    IEnumerator IRun()
    {
        while(true)
        {
            //CWSocketManager.Instance.UpdateUnit();
            yield return new WaitForSeconds(1);
        }
    }
    public void UpdateSelectAir(JToken jData)
    {
        byte[] bBuffer = null;
        JToken jt = jData;
        if (jt == null)
        {
            TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
            bBuffer = aa.bytes;

        }
        else
        {
            if (jt.Type == JTokenType.Null)
            {
                TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                bBuffer = aa.bytes;

            }
            else
            {
                bBuffer = jt.ToObject<byte[]>();
                if (bBuffer.Length < 10)
                {
                    TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                    bBuffer = aa.bytes;
                }

            }
        }
        m_gHero.SetBuffer(bBuffer);

    }

    public void UpdateData(JToken jData)
    {

        Debug.Log("UpdateData(JToken jData)!!!");


        ADCount = CWJSon.GetInt(jData, "ADCount");


        m_nCharNumber = CWJSon.GetInt(jData, "CharNumber");
        if (m_nCharNumber == 0) m_nCharNumber = 1;

         m_nRankPoint = CWJSon.GetInt(jData, "RankPoint");

        if(CWJSon.GetInt(jData, "AdDel")!=0)
        {
            m_bAdDel = true;
        }
        MultiCount = CWJSon.GetInt(jData, "MultiCount");
//        NSlotLevel = CWJSon.GetInt(jData, "SlotLevel");
        

        m_nDayCount = CWJSon.GetInt(jData, "DayCount");

        m_nPVPTotal = CWJSon.GetInt(jData, "PvpTotal");
        m_nPVPWin = CWJSon.GetInt(jData, "PvpWin");

        

        m_nStageNumber = CWJSon.GetInt(jData, "Stage");
        

        


        UpdateFirstData((JArray)jData["FirstData"]);


        m_nAirSlotID = CWJSon.GetInt(jData, "AirSlotID");
        // 싱글맵을 바꾼다




        CWHero.Instance.m_szFace = CWJSon.GetString(jData, "Icon");
        CWHero.Instance.m_nRankPoint = m_nRankPoint;

        //_Data["Config"]


        CWQuestManager.Instance.ReceiveData((JArray)jData["Quest"]);
        

        CWInvenManager.Instance.AddJObject((JArray)jData["Inven"]);

        ReceiveShapeData((JArray)jData["Shape"]);
        m_nBusterCount = CWJSon.GetInt(jData, "BusterCount");
        m_nWeaponCount = CWJSon.GetInt(jData, "WeaponCount");

        if(CWJSon.GetInt(jData, "IsTuto") ==1)
        {
            m_bTuto = false;
            

            for (int i=0;i<m_bFirstData.Length;i++)
            {
                m_bFirstData[i] = true;// 전부 완수로 간주한다!!!
            }
        }
        else
        {
            m_bTuto = true;// 현재 튜토 진행중
            
        }
        if(CWDebugManager.Instance.NotTuto)
        {
            m_bTuto = false;
        }
        

        //
        //m_kWeaponSlot.AddRange(_Data["WeaponSlot"].ToObject<int[]>());
        {
            
            
            UpdateWeaponSlot((JArray)jData["WeaponSlot"]);

        }

        
        UpdateBusterSlot((JArray)jData["BusterSlot"]);
        if (m_kBusterSlot.Count<2)
        {
            m_kBusterSlot.Add(0);
            m_kBusterSlot.Add(0);
        }


        if (CWLib.IsJSonData(jData["LikeList"]))
        {
            m_kLikeUser.AddRange(jData["LikeList"].ToObject<int[]>());
        }

        LoadMapData((JArray)jData["MapSaveData"]);

        AddDicItem(jData["DicItemData"]);

/*
        m_kGameFriend.AddRange(jData["Friend"].ToObject<int[]>());
        m_kFriend.AddRange(m_kGameFriend);
        m_kAskClan.AddRange(jData["ClanAsk"].ToObject<int[]>());
*/


        CWCoinManager.Instance.SetData(jData["Coins"]);

        

        

        //TimerJData(jData["Timer"]);
        //AutoPlayJData(_Data["AutoBattle"]);
        //DateTime dt = _Data[""]


        // 첫음 접속인지 확인 
        //CWWorldListManager.Instance.AutolevelWorld();
        //        m_vWoldPos = new Vector3(wx, 30, wz);




        m_kDroneAIData.LoadGamedata("Gamedata/DroneAIData");
        


        m_kGroupAIDrone.LoadGamedata("Gamedata/GroupAIDrone");
        m_kAITurretUnit.LoadGamedata("Gamedata/TurretUnit");
        m_kAIBossUnit.LoadGamedata("Gamedata/BossAI");

        m_kAIGroup.LoadGamedata("Gamedata/AIGroup");

        m_kAIHeroParming.LoadGamedata("Gamedata/Parming");
        m_kFakeUserAI.LoadGamedata("Gamedata/FakeUser");
        m_kFightUserAI.LoadGamedata("Gamedata/FightUser");






        //if(MissionDlg.Instance)
        //{
        //    CWChattingManager.Instance.SystemMessage("서버에 접속하였습니다.!");
        //    MissionDlg.Instance.m_DayQuestDlg.Init();
        //    m_kMyAskFriend = CWUserFileManager.Instance.GetArray<int>("MyfriendAsk");
        //    if (m_kMyAskFriend == null)
        //    {
        //        m_kMyAskFriend = new List<int>();
        //    }
        //    CWResourceManager.Instance.GetFaceImage(CWHero.Instance.m_szFace, null);
        //}


        CWGlobal.G_bGameStart = true;

        

        CWGalaxy.Instance.SelectSola(CWHeroManager.Instance.m_nSolaID);


        m_gHero.Create(jData["_id"].Value<int>());
        //m_gHero.Show(false);
       // CWHero.Instance.Show(false);
            

        m_gHero.name = jData["Name"].Value<string>();

        if (CWLib.IsJSonData(jData["Like"]))
        {
            m_nLike = jData["Like"].Value<int>();
        }
        else m_nLike = 0;


        m_nVIP = CWLib.ConvertIntbyJson(jData["VIP"]);
        m_nCharLevel = CWLib.ConvertIntbyJson(jData["CharLevel"]);

        


        if(CWJSon.GetInt(jData, "BlockUser")==1)
        {
            m_bBlockUser = true;
        }
        

        if(m_nRanking==0)
        {
            m_nRanking = 100;
        }


        
        CWUdpManager.Instance.Connect();

        if(!CWLib.IsJSonData(jData["Country"]))
        {
            StartCoroutine("GetCountyrCode");
        }
        CWChHero.Instance.SettingChar(m_nCharNumber);


        if (jData["DailyReward"]!=null)
        {
            m_kDailyList.AddRange(jData["DailyReward"].ToObject<int[]>());
            if(m_kDailyList.Count!=7)
            {
                m_kDailyList.Clear();
                for (int i = 0; i < 7; i++)
                {
                    m_kDailyList.Add(0);
                }

            }
         

        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                m_kDailyList.Add(0);
            }

        }




    }
    public bool CheckBlock(int nBlock)
    {
        int nGItem = CWArrayManager.Instance.GetItemFromBlock(nBlock);
        if (nGItem == 0) return false;
        return true;
    }

    // 유닛관리 
    public CWObject GetObject(int nID)
    {
        if (nID == 0) return null;
        if(nID< CWGlobal.NPC_STARTNUBER)
        {
            return CWUserManager.Instance.GetUser(nID);
        }
        if(nID< CWGlobal.USERBUILD_STARTNUBER)
        {
            return CWMobManager.Instance.GetObject(nID);
            
        }
        
        return null;
    }

    
    public void UpdatePrice()
    {
        
        // 업그레이드 가격 
        CWSocketManager.Instance.UpdatePrice();


    }



    #region 국가 코드 얻어오기 

    IEnumerator GetCountyrCode()
    {

        string url = "http://ip2c.org/self";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            string[] szvalue = www.downloadHandler.text.Split(';');
            if (szvalue.Length > 1)
            {
                CWSocketManager.Instance.UpdateUser("Country", szvalue[1]);
            }

        }
        else
        {
            Debug.Log("");
        }

    }


    #endregion

    #region 내 행성 저장 개념

    public void SaveMyPlanet()
    {
        
        CWMapManager.SelectMap.SaveData(CWGlobal.GetMyLocalName());


        
    }

    #endregion
    #region 비행기 슬롯

    public void SelectAirSlot(int nID)
    {
        m_nAirSlotID = nID;
        CWSocketManager.Instance.SelectAirSlot(m_nAirSlotID,(jData)=> {

            m_nRepairValue = CWJSon.GetInt(jData, "Repair");
            Repairtime = CWJSon.GetTime(jData, "Repairdate");

            byte[] bBuffer = null;
            JToken jt = jData["BlockBuffer"];
            if (jt == null)
            {
                TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                bBuffer = aa.bytes;

            }
            else
            {
                if (jt.Type == JTokenType.Null)
                {
                    TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                    bBuffer = aa.bytes;

                }
                else
                {
                    bBuffer = jt.ToObject<byte[]>();
                    if (bBuffer.Length < 10)
                    {
                        TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                        bBuffer = aa.bytes;
                    }

                }
            }
            CWHero.Instance.CopyBuffer(bBuffer);

        });

    }
    public int GetSelectSlotID()
    {
        return m_nAirSlotID;
    }

    #endregion


    #region 상점 타이머 관련
    //int year, int month, int day, int hour, int minute, int second,
    public DateTime GetStoreTimer(string szID)
    {
      

        if (CWPlayerPrefs.HasKey(CWHero.Instance.m_nID.ToString() + "_"+ szID + "year"))
        {
            int year = CWPlayerPrefs.GetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "year");
            int month = CWPlayerPrefs.GetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "month");
            int day = CWPlayerPrefs.GetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "day");
            int hour = CWPlayerPrefs.GetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "hour");
            int minute = CWPlayerPrefs.GetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "minute");
            int second = CWPlayerPrefs.GetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "second");
            return new DateTime(year, month, day, hour, minute, second);
        }
        return DateTime.Now;
    }
    public void SetStoreTimer(string szID)
    {
        CWPlayerPrefs.SetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "year", DateTime.Now.Year);
        CWPlayerPrefs.SetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "month", DateTime.Now.Month);
        CWPlayerPrefs.SetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "day", DateTime.Now.Day);
        CWPlayerPrefs.SetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "hour", DateTime.Now.Hour);
        CWPlayerPrefs.SetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "minute", DateTime.Now.Minute);
        int ss = DateTime.Now.Second - 2;
        if (ss < 0) ss = 0;
        CWPlayerPrefs.SetInt(CWHero.Instance.m_nID.ToString() + "_" + szID + "second", ss);
    }

    public int GetStoreTimerCount(string szID)
    {
        string str = CWHero.Instance.m_nID.ToString() + "_" + szID + "Count";
        if (CWPlayerPrefs.HasKey(str))
        {
            return CWPlayerPrefs.GetInt(str);
        }

        return 0;
    }
    public void SetStoreTimerCount(string szID,int nCount)
    {
        string str = CWHero.Instance.m_nID.ToString() + "_" + szID + "Count";

        CWPlayerPrefs.SetInt(str, nCount);
    }

    #endregion

    #region 도감

    List<int> m_kDicItem = new List<int>();

    void AddDicItem(JToken jData)
    {
        m_kDicItem = new List<int>();
        if (CWLib.IsJSonData(jData))
        {
            JArray ja = (JArray)jData;
            for(int i=0;i<ja.Count;i++)
            {
                if(ja[i]==null)
                {
                    m_kDicItem.Add(0);
                }
                else
                {

                    m_kDicItem.Add(CWLib.ConvertInt(ja[i].ToString()));

                }
                
            }
            //m_kDicItem.AddRange(jData.ToObject<int[]>());

            if(m_kDicItem.Count<2)
            {
                for (int i = 0; i < 30; i++)
                {
                    m_kDicItem.Add(0);
                }

            }
        }
        else
        {
            for(int i=0;i<30;i++)
            {
                m_kDicItem.Add(0);
            }
            
        }
            
    }
    // 보상 받을 것이 아나라도 있는가?
    public bool IsRewardItems()
    {
        for(int i=1;i<m_kDicItem.Count;i++)
        {
            if (m_kDicItem[i] == 1) return true;
        }

        return false;
    }

    // 내가 한번이라도 얻은 아이템인가?
    public bool IsTakeItem(int nItem)
    {


        int num= CWArrayManager.Instance.GetShipBlockbyID(nItem);
        if (m_kDicItem[num] > 0) return true;
        return false;
    }
    // 보상 받을 아이템인가?
    public bool IsRewardItem(int nItem)
    {
        int num = CWArrayManager.Instance.GetShipBlockbyID(nItem);
        if (m_kDicItem[num] == 1) return true;
        return false;
    }

    // 도감을 업데이트 1 수령 대기, 2 수령완료
    public void UpdateDicItem(int nItem,int nval,Action func)
    {
        int num = CWArrayManager.Instance.GetShipBlockbyID(nItem);
        CWSocketManager.Instance.UpdateDicItem(num, nval, (jData)=> {

            AddDicItem(jData["DicItemData"]);
            func();
        });
    }
    public void UpdateShipBlock(int nItem)
    {
        int num = CWArrayManager.Instance.GetShipBlockbyID(nItem);
        if (m_kDicItem[num] > 0) return ;

        UpdateDicItem(nItem, 1, () => {

        });

    }

    #endregion


}
