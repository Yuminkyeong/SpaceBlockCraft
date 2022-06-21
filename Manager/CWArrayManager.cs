using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using System;
using System.Linq;
using CWUnityLib;
using CWStruct;
using Newtonsoft.Json.Linq;

// 속도를 요구하는 테이블관련 
// 
/* 개념 :
 * 테이블매니저에서 읽은 후에 
 * 
 * */
public class CWArrayManager : CWManager<CWArrayManager>
{
    #region 초기화

    bool m_Initflag = false;
    public void InitData()
    {
        if (m_Initflag) return;
        m_Initflag = true;
        MakeLevelRateData();// 각종 레벨 비율에 대한 정보
        InitBlock();
        InitGItem();
        InitExp();
        InitUserInfo();
        InitTurretInfo();

        InitBattleMap();
        
        InitDrone();
        InitSunData();
        
        


        InitGalaxyData();
        InitPlanetData();
        InitStageData();
        InitMOB();
        InitMobData();
        InitColorData();
        InitCoinData();
        InitPVPData();
        

        InitGradePowerData();
        InitSlotPowerData();
        InitShipBlockData();
        if (CWQuestManager.Instance)
            CWQuestManager.Instance.LoadCSV();
        // 데이타를 모두 읽은 후
        //CWGlobal.G_bGameStart = true;
        if(CWLocalization.Instance)
        {
            if(CWPrefsManager.Instance)
                CWLocalization.Instance.SetLanguage(CWPrefsManager.Instance.GetLanguage());
        }
        if(CWCashManager.Instance)
            CWCashManager.Instance.InitializePurchasing();
            

#if UNITY_EDITOR
        {
            CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_아이템 - 상점");
            if (cs != null)
            {
                foreach (var v in cs.m_mkData)
                {
                    if (CWLocalization.Instance)
                        CWLocalization.Instance.AddData(cs.GetString(v.Key, "제목"));
                    if (CWLocalization.Instance)
                        CWLocalization.Instance.AddData(cs.GetString(v.Key, "설명"));
                }
            }

        }

        {
            CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_아이템 - 모양");
            if (cs != null)
            {
                foreach (var v in cs.m_mkData)
                {
                    if (CWLocalization.Instance)
                        CWLocalization.Instance.AddData(cs.GetString(v.Key, "title"));
                }
            }

        }

#endif

    }

    public override void Create()
    {
        base.Create();
    }
    public bool IsInit()
    {
        return m_Initflag;
    }
    #endregion

    #region 유저정보

    public class USERINFO
    {
        public int nEngineCount;
        public int nBlockCount;
        public int nHP;
        public int nWeaponCount;
        public int nDamage;
        public int nTurretDamage;
        public int nTurretHP;
    };
    Dictionary<int, USERINFO> m_kUserInfo = new Dictionary<int, USERINFO>();

    public USERINFO GetUserInfo(int nLevel)
    {
        if(m_kUserInfo.ContainsKey(nLevel))
        {
            return m_kUserInfo[nLevel];
        }
        return null;
    }
    void InitUserInfo()
    {
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("UserLevel");
        if (cs != null)
        {

            foreach (var v in cs.m_mkData)
            {
                USERINFO kInfo = new USERINFO();

                kInfo.nDamage = cs.GetInt(v.Key, "최대데미지");
                kInfo.nHP = cs.GetInt(v.Key, "최대 HP");
                kInfo.nEngineCount = cs.GetInt(v.Key, "엔진개수");
                kInfo.nWeaponCount = cs.GetInt(v.Key, "무기최대개수");
                kInfo.nBlockCount = cs.GetInt(v.Key, "블록최대 개수");

                kInfo.nTurretHP = cs.GetInt(v.Key, "터렛HP");
                kInfo.nTurretDamage = cs.GetInt(v.Key, "터렛데미지");

                m_kUserInfo.Add(v.Key, kInfo);
            }
        }


    }


    #endregion


    #region 아이템 /블록


    

    GITEMDATA Zero = new GITEMDATA();
    // 최대 아이템 카운트

    public BLOCKINFO[] m_kBlock;
    GITEMDATA[] m_kGItemdata;
    public Dictionary<int, WEAPON> m_kWeapon = new Dictionary<int, WEAPON>();
    public Dictionary<int, BUSTER> m_kBuster = new Dictionary<int, BUSTER>();

    

    Dictionary<string, int> m_kItemIndex = new Dictionary<string, int>();
    Dictionary<string, int> m_kBlockName = new Dictionary<string, int>();

    
    
    List<float>[] m_kRate = new List<float>[5];

    void InitBlock()
    {
        
        m_kBlock = new BLOCKINFO[256];// 최대치를 잡는다 
        for(int i=0;i<256;i++)
        {
            m_kBlock[i] = new BLOCKINFO();
            m_kBlock[i].nID = 0;
        }
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_블록 - 시트1");
        if (cs != null)
        {

           
          
            
            foreach (var v in cs.m_mkData)
            {
                int i = v.Key;
                int num = CWLib.ConvertInt(cs.GetString(i, "num")); //  cs.GetString(i,"x");//CWTableManager.I.GetTableInt("Block", "x", i);
                if (num >0)
                {
                    m_kBlock[num].nID = num;
                    m_kBlock[num].HP = CWLib.ConvertInt(cs.GetString(i, "HP"));

                    m_kBlock[num].x = cs.GetInt(i,"x");
                    m_kBlock[num].y = cs.GetInt(i, "y");

                    m_kBlock[num].white_x = cs.GetInt(i, "white_x");
                    m_kBlock[num].white_y = cs.GetInt(i, "white_y");



                    m_kBlock[num].name = cs.GetString(i, "Name");
                    m_kBlock[num].szItem = cs.GetString(i, "gameitem");
                    m_kBlock[num].sidename = cs.GetString(i, "Side");

                    m_kBlock[num].szType = cs.GetString(i, "타입");
                    m_kBlock[num].szPatten = cs.GetString(i, "분류");


                    string szShape = cs.GetString(i, "Shape"); //CWTableManager.I.GetTable("Block", "Shape", i);
                    m_kBlock[num].nShape = GetShape(szShape);
                    if (!m_kBlockName.ContainsKey(m_kBlock[num].name))
                    {
                        m_kBlockName.Add(m_kBlock[num].name, num);
                    }

                  

                }




            }
            // 사이드 
            for (int i=0;i<m_kBlock.Length;i++)
            {
                if (m_kBlock[i].sidename == null) continue;
                int nb = GetBlock(m_kBlock[i].sidename);
                if(nb==0)
                {
                    m_kBlock[i].side_x = m_kBlock[i].x;
                    m_kBlock[i].side_y = m_kBlock[i].y;

                }
                else
                {
                    m_kBlock[i].side_x = m_kBlock[nb].x;
                    m_kBlock[i].side_y = m_kBlock[nb].y;

                }
            }

        }

    }
    // 아이템 레벨을 여기서 만든다
    // 레벨에 따른 비율을 구한다
    public float GetLevelValue(int minvalue,int maxvalues,int nLevel,int nType)
    {
        return minvalue + maxvalues * m_kRate[nType][nLevel];
    }

    public int GetPattenBlock(string sztype,string szPattern)
    {
        foreach(var v in m_kBlock)
        {
            if(v.szType==sztype && v.szPatten==szPattern)
            {
                return v.nID;
            }
        }
        return 0;
    }
    public List<int> GetPatternBlocks(string sztype, string szPattern)
    {

        List<int> ktemp = new List<int>();
        foreach (var v in m_kBlock)
        {
            if (v.szType == sztype && v.szPatten == szPattern)
            {
                ktemp.Add(v.nID);
            }
        }

        return ktemp;
    }


    void MakeLevelRateData()
    {
        m_kRate[0] = new List<float>();
        m_kRate[1] = new List<float>();
        m_kRate[2] = new List<float>();
        m_kRate[3] = new List<float>();

        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_아이템 - 레벨업");
        foreach (var v in cs.m_mkData)
        {
            float f1 = cs.GetFloat(v.Key, "비율");
            float f2 = cs.GetFloat(v.Key, "비율1");
            float f3 = cs.GetFloat(v.Key, "비율2");
            float f4 = cs.GetFloat(v.Key, "비율3");

            m_kRate[0].Add(f1);
            m_kRate[1].Add(f2);
            m_kRate[2].Add(f3);
            m_kRate[3].Add(f4);
        }
      
    }
    public string GetMissile(int nType,int level)
    {

        if(nType==1)
        {
            return CWTableManager.Instance.GetTable("무기업그레이드 - 시트1", "gun", level); ;
        }
        if (nType == 2)
        {
            return CWTableManager.Instance.GetTable("무기업그레이드 - 시트1", "gun", level); ;
        }
        if (nType == 3)
        {
            return CWTableManager.Instance.GetTable("무기업그레이드 - 시트1", "gun", level); ;
        }
        return "gun_1";

    }
    void InitWeapon()
    {
        m_kWeapon.Clear();
        string szFile = "BAC_아이템 - 무기";
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable(szFile);
        if (cs != null)
        {


            foreach (var v in cs.m_mkData)
            {
                int i = v.Key;
                WEAPON ws = new WEAPON
                {
                    nID = cs.GetInt(i, "ItemID"),
                    nType = cs.GetInt(i, "Type"),
                    Damage = cs.GetInt(i, "damage"),
                    Level = cs.GetInt(i, "Level"),

                };
                if (ws.nID == 0) continue;
//                ws.szmissile = GetMissile(ws.nType, ws.Level);

                m_kWeapon.Add(ws.nID, ws);
            }
        }

    }
    
    void InitGItem()
    {

        
        string szFile = "BAC_아이템 - 아이템";
        string szBlockFile = "BAC_블록 - 시트1";
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable(szFile);
        if (cs != null)
        {

            int tcnt =cs.GetCount()+100;
            m_kGItemdata = new GITEMDATA[512];
            foreach (var v in cs.m_mkData)
            {
                int i = v.Key;

                string szblock = cs.GetString(i, "블록");
                
                int nKey = CWTableManager.Instance.Find(szBlockFile, "Name", szblock);
                int nnblock = CWTableManager.Instance.GetTableInt(szBlockFile, "Num", nKey);

                int num= cs.GetInt(i, "ID");
                if (num == 0) continue;

                m_kGItemdata[num] = new GITEMDATA
                {
                    nID = cs.GetInt(i, "ID"),
                    szname = cs.GetString(i, "name"),
                    szfilename = cs.GetString(i, "filename"),
                    InvenType = cs.GetInt(i, "InvenType"),
                    nblock = nnblock,
                    type = cs.GetString(i, "Type"),
                    price = cs.GetInt(i, "가격"),
                    pricesell = cs.GetInt(i, "Price_Sell"),
                    cash = cs.GetInt(i, "Cash"),
                    sziconname = cs.GetString(i, "iconame"),
                    szGroup = cs.GetString(i, "group"),
                    m_szTitle = cs.GetString(i, "title"),
                    szInfo = cs.GetString(i, "info"),
                    Damage = cs.GetInt(i, "Damage"),
                    hp = cs.GetInt(i, "HP"),
                    subtype = cs.GetInt(i, "subtype"),
                    level = cs.GetInt(i, "Level"),

                };

#if UNITY_EDITOR
                if(CWLocalization.Instance)
                    CWLocalization.Instance.AddData(m_kGItemdata[num].m_szTitle);
#endif
                m_kGItemdata[num].nKey = i;


                if (m_kGItemdata[num].nID == 0) continue;
                if (!CWLib.IsString(m_kGItemdata[num].szname)) continue;

                if (m_kGItemdata[num].nID == (int)GITEM.tree)
                {
                    m_kGItemdata[num].nblock = (int)OLDBLOC.tree;
                }
                if (m_kGItemdata[num].nID == (int)GITEM.wood)
                {
                    m_kGItemdata[num].nblock = (int)OLDBLOC.wood;
                }


                string szItem = m_kGItemdata[num].szname.ToUpper();
                if(!m_kItemIndex.ContainsKey(szItem))
                {
                    m_kItemIndex.Add(szItem, num);
                }
                else
                {
                    Debug.Log(szItem);
                }
                
                

            }

            
        }

        for (int i = 0; i < m_kBlock.Length; i++)
        {
            m_kBlock[i].nItemID = GetItemNumber(m_kBlock[i].szItem);
        }
        InitWeapon();
        
    }

    
    

    public int GetWeaponValue(int nType,int nLevel)
    {
       
        return 0;
    }

    
    public BUSTER GetBuster(int nItemID)
    {
        if (m_kBuster.ContainsKey(nItemID))
        {
            return m_kBuster[nItemID];
        }
        return new BUSTER();
    }

    public WEAPON GetWeapon(int nItemID)
    {
        nItemID = nItemID % 256;
        if (nItemID <= 0) nItemID = 0;

        if (m_kWeapon.ContainsKey(nItemID))
        {
            return m_kWeapon[nItemID];
        }
        return new WEAPON();
    }
    public int GetBlock(string szName)
    {
        if (szName == null) return 0;
        if(m_kBlockName.ContainsKey(szName))
        {
            return m_kBlockName[szName];
        }
        return 0;
        
    }
    // 비싼 아이템인가?
    public bool IsRoyalItem(int nItem)
    {
        GITEMDATA nData = GetItemData(nItem);
        if(nData.cash >10)
        {
            return true;
        }
        return false;

    }
    BLOCKSHAPE GetShape(string szname)
    {
        int nCount = Enum.GetValues(typeof(BLOCKSHAPE)).Cast<int>().Max();
        for (int i = 0; i < nCount; i++)
        {
            string szstr = ((BLOCKSHAPE)i).ToString();
            if (szstr == szname)
            {
                return ((BLOCKSHAPE)i);
            }
        }
        return 0;
    }
    //아이템번호에서 블록으로 변환 
    public int GetBlockFromItem(int nGNumber)
    {
        if(m_kGItemdata==null)
        {
            return 0;
        }
        if (nGNumber < 0)
        {
            DebugX.Log(string.Format("GetBlockToItem(int nBlock) error {0}", nGNumber));
            return 0;
        }

        if (nGNumber >= m_kGItemdata.Length)
        {
            DebugX.Log(string.Format("GetBlockToItem(int nBlock) error {0}", nGNumber));
            return 0;
        }

        return m_kGItemdata[nGNumber].nblock;
    }
    public int GetBlockHP(int nBlock)
    {
        if (nBlock < 0)
        {
            DebugX.Log(string.Format("GetItemToBlock(int nBlock) error {0}", nBlock));
            return 0;
        }

        if (nBlock >= m_kBlock.Length)
        {
            DebugX.Log(string.Format("GetItemToBlock(int nBlock) error {0}", nBlock));
            return 0;
        }
        return m_kBlock[nBlock].HP;

    }
    
    // 블록에서 아이템으로
    public int GetItemFromBlock(int nBlock)
    {
        if (nBlock < 0)
        {
            DebugX.Log(string.Format("GetItemToBlock(int nBlock) error {0}", nBlock));
            return 0;
        }
            
        if(nBlock>= m_kBlock.Length)
        {
            DebugX.Log(string.Format("GetItemToBlock(int nBlock) error {0}",nBlock));
            return 0;
        }
        return m_kBlock[nBlock].nItemID;
    }

    public int GetLevelBlock(int nLevel)
    {

        for (int i = 0; i < m_kGItemdata.Length; i++)
        {
            if (m_kGItemdata[i].type == "shipblock")
            {
                if (m_kGItemdata[i].level == nLevel )
                {
                    return m_kGItemdata[i].nID;
                }
            }
        }

        return 0;

    }
    // 블록 중에서 다음 레벨 
    public int GetNextLevelBlock(int nLevel)
    {
        return GetLevelBlock(nLevel + 1);


    }
    public int GetItemLevel(int nID)
    {
        if(nID>=256)
        {
            return nID / 256 + 1;
        }
        nID = nID % 256;
        if (nID <= 0) nID = 0;
        if (nID >= m_kGItemdata.Length) nID = 0;
        return m_kGItemdata[nID].level;
    }
    public GITEMDATA GetItemData(int nID)
    {
        nID = nID % 256;
        if (nID <= 0) nID=0;
        if(nID>= m_kGItemdata.Length) nID = 0;
        return m_kGItemdata[nID];
    }

    

    public string GetItemName(int nID)
    {
        if (nID >= m_kGItemdata.Length) return "";
        return m_kGItemdata[nID].szname;
    }
    public string GetItemFileName(int nID)
    {
        return m_kGItemdata[nID].szfilename;
    }

    public int GetItemNumber(string szname)
    {
        if (szname == null) return 0;
        if (szname.Length < 1) return 0;
        if (m_kItemIndex.ContainsKey(szname.ToUpper()))
        {
            return m_kItemIndex[szname.ToUpper()];
        }
        //       Debug.Log("error " + szname);

        return 0;
    }
    //possible
    public bool IsUpgradePossible(int nItem)
    {
        return true;
     
    }

    public int GetWeaponItem(string szgroup, int nLevel)
    {

        foreach(var v in m_kGItemdata)
        {
            if(v.szGroup==szgroup)
            {
                if (v.level == nLevel )
                {
                    return v.nID;
                }

            }
        }
        return 0;
    }

    public int GetEngineItem(int nLevel)
    {

        foreach(var v in m_kGItemdata)
        {
            if(v.type=="engine")
            {
                if (v.level == nLevel )
                {
                    return v.nID;
                }

            }
        }
        return 0;
    }


    #endregion

    #region 경험치 경험치가 등급이다 

    struct EXPDATA
    {
        public int exp;
        public int gridexp;// 구간 경험치
    };

    EXPDATA[] m_kExp = new EXPDATA[256];
    int m_nMaxLevel = 0;
    void InitExp()
    {
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("경험치 - 레벨");
        if (cs != null)
        {

            foreach (var v in cs.m_mkData)
            {
                m_kExp[v.Key] = new EXPDATA
                {

                    exp = cs.GetInt(v.Key, "경험치"),
                    gridexp = cs.GetInt(v.Key, "몹경험치")
                };
                m_nMaxLevel++;
            }
        }
    }

    public int Getlevel(int exp)
    {
        if (exp <= 1) return 1;
        for (int i = 1; i < m_kExp.Length - 1; i++)
        {
            if (exp >= m_kExp[i].exp && exp < m_kExp[i + 1].exp)
            {
                return i;
            }
        }
        return 100;
    }

    public int GetExp(int Level)
    {
        return m_kExp[Level].exp;
    }

    


    #endregion

    #region 터렛정보

    public class TurretInfo
    {
        public string szname;
        public float fhp;
        public float fdamage;
        public int nRange;
        public int nAi;
        public int nSight;
        public int nCooltime;
        public string szmissile;
        public int nType;

    };
    Dictionary<int, TurretInfo> m_kTurretInfo = new Dictionary<int, TurretInfo>();


    void InitTurretInfo()
    {
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("turretinfo");
        if (cs != null)
        {

            foreach (var v in cs.m_mkData)
            {
                TurretInfo kInfo = new TurretInfo();

                kInfo.szname = cs.GetString(v.Key, "name");
                kInfo.fhp = cs.GetFloat(v.Key, "hp");
                kInfo.fdamage = cs.GetFloat(v.Key, "damage");
                kInfo.nRange = cs.GetInt(v.Key, "사격범위");
                kInfo.nAi = cs.GetInt(v.Key, "AI");
                kInfo.nSight = cs.GetInt(v.Key, "적시야");
                kInfo.nCooltime = cs.GetInt(v.Key, "쿨타임");
                kInfo.szmissile = cs.GetString(v.Key, "미사일");
                kInfo.nType = cs.GetInt(v.Key, "type");
                m_kTurretInfo.Add(v.Key, kInfo);
            }
        }

    }

    public TurretInfo GetTurretInfo(int nKey)
    {
        return m_kTurretInfo[nKey];
    }

    public string FindTurret(int nLand,int nType)
    {

        foreach(var v in m_kTurretInfo)
        {
            if (nLand == 1)// 육지
            {
                if(nType==1)
                {
                    if(v.Value.nType==1)
                    {
                        return v.Value.szname;
                    }
                }
                if (nType == 2)
                {
                    if (v.Value.nType == 2)
                    {
                        return v.Value.szname;
                    }
                }
                if (nType == 3)
                {
                    if (v.Value.nType == 3)
                    {
                        return v.Value.szname;
                    }
                }
            }
            else
            {
                if (nType == 1)
                {
                    if (v.Value.nType == 6)
                    {
                        return v.Value.szname;
                    }
                }
                if (nType == 2)
                {
                    if (v.Value.nType == 7)
                    {
                        return v.Value.szname;
                    }
                }
                if (nType == 3)
                {
                    if (v.Value.nType == 8)
                    {
                        return v.Value.szname;
                    }
                }

            }

        }


        return "";
    }

    #endregion
    
    #region 배틀맵
    public class BattleMapInfo
    {

        public int m_nMapID;
        public int m_Reward;
        public int m_nBlock;
        public float m_fTimeSec;
        public string m_szMission;
        public int m_nGrade;

    }
    List<BattleMapInfo> m_kBattleMapInfo = new List<BattleMapInfo>();
    void InitBattleMap()
    {
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("battlemap");
        if (cs != null)
        {

            foreach (var v in cs.m_mkData)
            {
                BattleMapInfo kInfo = new BattleMapInfo();
                kInfo.m_nMapID = cs.GetInt(v.Key, "MapID");
                kInfo.m_Reward = cs.GetInt(v.Key, "reward");
                kInfo.m_nBlock = cs.GetInt(v.Key, "블록");
                kInfo.m_nGrade = cs.GetInt(v.Key, "grade");
                kInfo.m_fTimeSec = cs.GetFloat(v.Key, "timer");
                kInfo.m_szMission = cs.GetString(v.Key, "Mission");
                m_kBattleMapInfo.Add( kInfo);
            }
        }

    }
    public BattleMapInfo GetBattleMapInfo(int num)
    {
        return m_kBattleMapInfo[num];
        
    }
    public int GetBattleMapID(int nGrade,int nRandom)
    {

        List<int> temp = new List<int>();
        //foreach(var v in m_kBattleMapInfo)
        for(int i=0;i<m_kBattleMapInfo.Count;i++)
        {
            if(m_kBattleMapInfo[i].m_nGrade == nGrade)
            {
                temp.Add(i);
            }
        }
        

        if (temp.Count <= 0)
        {
            return 0;
        }
        
        int num = nRandom%temp.Count;
        
        return temp[num];
    }
    #endregion
    #region 드론정보
    struct DRONEDATA
    {
        public string szName;
        public int nlevel;
    }
    List<DRONEDATA> m_kDroneList = new List<DRONEDATA>();
    void InitDrone()
    {
        m_kDroneList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("드론 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                DRONEDATA kInfo = new DRONEDATA();
                kInfo.szName = cs.GetString(v.Key, "name");
                kInfo.nlevel = cs.GetInt(v.Key, "level");
                m_kDroneList.Add(kInfo);
            }
        }

    }
    public string GetDrone(int nLevel)
    {
        foreach(var v in m_kDroneList)
        {
            if(v.nlevel >=nLevel)// level 20이라면 25가 리턴됨
            {
                return v.szName;
            }
        }
        return "";
    }


    #endregion
    #region  태양시스템

    class SUNDATA
    {
        public int nSingle;
        public int nID;
        public int nColor;
        public int[] nRoomNumber;
    }
    List<SUNDATA> m_kSunData = new List<SUNDATA>();

    public int  GetSunID(int nSingle, int nRoomNumber)
    {
        
        foreach (var v in m_kSunData)
        {
            for(int i=0;i<6;i++)
            {
                if(v.nSingle==nSingle)
                {
                    if (v.nRoomNumber[i] == nRoomNumber)
                    {
                        return v.nID;
                    }

                }
            }
        }
        return 0;
    }
    public int [] GetSunRoomnumber(int nSunId)
    {
        SUNDATA kData= m_kSunData.Find(x => x.nID == nSunId);
        if (kData == null) return null;

        return kData.nRoomNumber;

    }
    void InitSunData()
    {
        m_kSunData.Clear();

        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("태양시스템 - 싱글");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                SUNDATA kInfo = new SUNDATA();
                kInfo.nID = cs.GetInt(v.Key, "ID");
                kInfo.nColor= cs.GetInt(v.Key, "색상");
                kInfo.nSingle = cs.GetInt(v.Key, "온라인");
                string szArray = cs.GetString(v.Key, "룸번호");//
                string [] aa= szArray.Split(',');
                kInfo.nRoomNumber = new int[6];
                int ccc = 0;
                foreach(var vv in aa)
                {
                    kInfo.nRoomNumber[ccc] =  CWLib.ConvertInt(vv);
                    ccc++;
                }
                m_kSunData.Add(kInfo);
            }
        }

    }



    #endregion

    

    

    

    

    

    #region 은하 정보 

    public struct GalaxyData
    {
        public string m_szName;
        public float m_fDist;
        public int m_pGold;
        public int m_pGem;
        public int m_SGold;
        public int m_SGem;


    }
    List<GalaxyData> m_kGalaxyDataList = new List<GalaxyData>();
    void InitGalaxyData()
    {
        
        m_kGalaxyDataList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("은하계정보 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                GalaxyData kInfo = new GalaxyData();
                kInfo.m_szName = cs.GetString(v.Key, "name");
                kInfo.m_fDist = cs.GetFloat(v.Key, "거리");
                kInfo.m_pGold = cs.GetInt(v.Key, "행성골드");
                kInfo.m_pGem = cs.GetInt(v.Key, "행성보석");
                kInfo.m_SGold = cs.GetInt(v.Key, "항성골드");
                kInfo.m_SGem = cs.GetInt(v.Key, "항성보석");

             


                m_kGalaxyDataList.Add(kInfo);
            }
        }

    }

    public int GetGalxyMaxCount()
    {
        return m_kGalaxyDataList.Count;
    }
    public GalaxyData GetGalxyData(int num)// 1 부터 시작
    {
        if (num <= 0)
        {
            return new GalaxyData();
        }
        if (num > m_kGalaxyDataList.Count)
        {
            return new GalaxyData();
        }

        return m_kGalaxyDataList[num-1];
    }
    public string GetGalaxyDataName(int num)// 1 부터 시작
    {

        if (num <= 0)
        {
            return "";
        }
        if (num > m_kGalaxyDataList.Count)
        {
            return "";
        }

        return m_kGalaxyDataList[num-1].m_szName;
    }
    
    public float GetGalaxyDataDist(int num)// 1 부터 시작
    {
        if(num<=0)
        {
            return 0;
        }
        if (num > m_kGalaxyDataList.Count)
        {
            return 0;
        }

        return m_kGalaxyDataList[num-1].m_fDist;
    }



    #endregion

    #region  스테이지 정보 

    //public class MAPLIST
    //{
    //    public int m_nRoomNumber;
    //    public int m_nMapID;
    //    public int m_nSunID;
    //    public int m_nPlanetID;
    //    public int m_nLevel;
    //    public int m_nSingle;
    //}


    public struct StageData
    {
        public int m_nStage;
        public int m_nPlanetID;
        public int m_nSunID;
        public int m_nMapID;
        public int m_nMobCount;
        public int m_nGoldRate;// 금 확률
        public int m_nIronRate;// 철 확률 
        public int m_nLevel1;
        public int m_nLevel2;
        public int m_nLevel3;

        public string m_szName;// 맵 이름

        public int m_nCount2;// 매장 개수 
        public int m_nCount3;
    }

    List<StageData> m_kStageDataList = new List<StageData>();

    
    
    void InitStageData()
    {
        m_kStageDataList.Clear();

        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("스테이지 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                StageData kInfo = new StageData();
                kInfo.m_nStage = cs.GetInt(v.Key, "stage");


                kInfo.m_nPlanetID = cs.GetInt(v.Key, "행성");
                kInfo.m_nSunID = cs.GetInt(v.Key, "항성");
                kInfo.m_nMapID = cs.GetInt(v.Key, "MapID");
                kInfo.m_nMobCount= cs.GetInt(v.Key, "Mobcount");

                kInfo.m_nGoldRate = cs.GetInt(v.Key, "골드");
                kInfo.m_nIronRate = cs.GetInt(v.Key, "철");

                kInfo.m_nLevel1 = cs.GetInt(v.Key, "광물1");
                kInfo.m_nLevel2 = cs.GetInt(v.Key, "광물2");
                kInfo.m_nLevel3 = cs.GetInt(v.Key, "광물3");
                kInfo.m_nCount2 = cs.GetInt(v.Key, "개수2"); 
                kInfo.m_nCount3 = cs.GetInt(v.Key, "개수3");
                kInfo.m_szName = cs.GetString(v.Key, "면이름");
                m_kStageDataList.Add( kInfo);


            }
        }



    }
    public int GetBlockLevel(int level, int nstage)
    {
        StageData kk = GetStageData(nstage);
        if (level == 1) return GetLevelBlock(kk.m_nLevel1);
        if (level == 2) return GetLevelBlock(kk.m_nLevel2);
        if (level == 3) return GetLevelBlock(kk.m_nLevel3);
        return 0;
    }
    public int GetGoldRate(int nStage)
    {
        StageData kk=GetStageData(nStage);
        return kk.m_nGoldRate;
    }
    public int GetIRonRate(int nStage)
    {
        StageData kk = GetStageData(nStage);
        return kk.m_nIronRate;
    }

    // 레벨에 맞는 블록 출력 
    public StageData GetStageData(int nStage)//1부터 시작
    {
        if (nStage <= 0   )
        {
            return new StageData();
        }
        if(nStage> m_kStageDataList.Count) return new StageData();
        return m_kStageDataList[nStage-1];
    }


    public StageData GetMapListInfoByPlanet(int nPlanet)
    {
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nPlanetID == nPlanet)
            {
                return v;
            }
        }
        return new StageData(); 
    }
    public StageData GetMapListInfoBySun(int nSun)
    {
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nSunID == nSun)
            {
                return v;
            }
        }
        return new StageData();
    }

    public string GetStageMobName(int nStage)
    {
        MOBDATA kMob= GetMOBData(nStage);
        return GetMobName(kMob.nMob);
    }



    // 행성번호로 방문할 맵 번호를 리턴한다
    public int GetRoomNumberByPlanet(int nPlanet)
    {
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nPlanetID == nPlanet)
            {
                return v.m_nStage;
            }
        }
        return 0;
    }
    public int GetMapID(int nStageNumber)
    {
        // 멀티 행성일 경우 , 멀티맵 번호를 준다 
        if (nStageNumber <= 0) return 0;
        if (nStageNumber > m_kStageDataList.Count) return 0;

        return m_kStageDataList[nStageNumber - 1].m_nMapID;

    }
    public int GetPlanetID(int nStageNumber)
    {

        if (nStageNumber <= 0) return 0;
        if (nStageNumber > m_kStageDataList.Count) return 0;

        return m_kStageDataList[nStageNumber - 1].m_nPlanetID;

    }
    
    public int GetFirstStageNumber(int nPlanet)
    {
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nPlanetID == nPlanet)
            {
                  return v.m_nStage;
            }
        }
        return 0;
    }

    public List<int> GetPlanetList(int nSunID)
    {
        List<int> kPlanet = new List<int>();
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nSunID == nSunID)
            {
                if (kPlanet.Exists(x => x == v.m_nPlanetID)) continue;

                kPlanet.Add(v.m_nPlanetID);
            }
        }
        return kPlanet;
    }
    public int GetSunID(int nRoomnumber)
    {

        if (nRoomnumber <= 0) return 0;
        if (nRoomnumber > m_kStageDataList.Count) return 0;

        return m_kStageDataList[nRoomnumber - 1].m_nSunID;

    }

    public int GetSunIDbyPlanet(int nPlanetID)
    {

        foreach (var v in m_kStageDataList)
        {
            if (v.m_nPlanetID == nPlanetID)
            {
                return v.m_nSunID;
            }
        }

        return 0;

    }
    // 행성의 맵인가?
    public bool IsRoom(int nRoomnumber, int nPlanetID)
    {
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nPlanetID == nPlanetID)
            {
                if (nRoomnumber == v.m_nStage)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int[] GetCubeRoomList(int nPlanetID)
    {
        int[] nRoomList = new int[6];
        int cnt = 0;
        foreach (var v in m_kStageDataList)
        {
            if (v.m_nPlanetID == nPlanetID)
            {
                nRoomList[cnt] = v.m_nStage;
                cnt++;
            }
        }
        return nRoomList;
    }



    #endregion

    #region 드론정보
    public struct MOBDATA
    {
        public int nHp;
        public int nDamage;
        public float fSpeed;
        public float fPathSpeed;
        public float fMaxDelay;
        public float fMinDelay;
        public int nMob;
    }
    List<MOBDATA> m_kMOBList = new List<MOBDATA>();
    void InitMOB()
    {
        m_kMOBList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("스테이지 - 몹");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                MOBDATA kInfo = new MOBDATA();
                kInfo.nHp = cs.GetInt(v.Key, "hp");
                kInfo.nDamage = cs.GetInt(v.Key, "damage");
                kInfo.fMinDelay = cs.GetFloat(v.Key, "MinDelay");
                kInfo.fMaxDelay = cs.GetFloat(v.Key, "MaxDelay");
                kInfo.fSpeed = cs.GetFloat(v.Key, "speed");
                kInfo.fPathSpeed = cs.GetFloat(v.Key, "PathSpeed"); // 절대값이 아니라 비율 1이면 원래 속도 2 2배 속도 이런 개념 
                kInfo.nMob = cs.GetInt(v.Key, "몹번호");
                m_kMOBList.Add(kInfo);
            }
        }

    }
    public int GetMOBHP(int nLevel)
    {
        if (nLevel <= 0) return 0;
        if (nLevel > m_kMOBList.Count) return 0;

        return m_kMOBList[nLevel-1].nHp;
    }
    public int GetMOBDamage(int nLevel)
    {
        if (nLevel <= 0) return 0;
        if (nLevel > m_kMOBList.Count) return 0;

        return m_kMOBList[nLevel - 1].nDamage;
    }
    public float GetMOBSpeed(int nLevel)
    {
        if (nLevel <= 0) return 0;
        if (nLevel > m_kMOBList.Count) return 0;

        return m_kMOBList[nLevel - 1].fSpeed;
    }
    public MOBDATA GetMOBData(int nLevel)
    {
        if (nLevel <= 0) return new MOBDATA();
        if (nLevel > m_kMOBList.Count)
        {
            nLevel = m_kMOBList.Count - 1;
        }

        return m_kMOBList[nLevel - 1];

    }

    #endregion

    #region 행성정보

    public struct PlanetData
    {

        public int m_nLimit;// 제한등급
        public int m_nGold;//골드 보상
        public int m_nTicket;// 입장권 개수 
    }
    List<PlanetData> m_kPlanetDataList = new List<PlanetData>();
    void InitPlanetData()
    {
        
        m_kPlanetDataList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("은하계정보 - 행성");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                PlanetData kInfo = new PlanetData();



                kInfo.m_nTicket = cs.GetInt(v.Key, "입장권");


                kInfo.m_nLimit = cs.GetInt(v.Key, "제한등급");
                kInfo.m_nGold = cs.GetInt(v.Key, "골드");

                m_kPlanetDataList.Add(kInfo);
            }
        }

    }
    
    public PlanetData GetPlanetData(int num)// 1 부터 시작
    {
        if (num <= 0)
        {
            return new PlanetData();
        }
        if (num > m_kPlanetDataList.Count)
        {
            return new PlanetData();
        }

        return m_kPlanetDataList[num - 1];
    }
    

    #endregion

    

    #region 몹종류

    public struct MobData
    {

        public string m_szName;
        public string m_szFile;
        public string m_szMissile;
        public string m_szBossMissile;
        public string m_szBossFile;
        

    }


    Dictionary<int, MobData> m_kMobDataList = new Dictionary<int, MobData>();


    void InitMobData()
    {
        m_kMobDataList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("몹 - 정보");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                MobData kInfo = new MobData();
                kInfo.m_szName = cs.GetString(v.Key, "name");
                kInfo.m_szFile = cs.GetString(v.Key, "File");
                kInfo.m_szBossFile = cs.GetString(v.Key, "BossFile");
                kInfo.m_szMissile = cs.GetString(v.Key, "missile");
                kInfo.m_szBossMissile = cs.GetString(v.Key, "Bossmissile");
                m_kMobDataList.Add(v.Key, kInfo);
            }
        }

    }
    public int GetMobNumber(string szName)
    {
        foreach(KeyValuePair<int,MobData> items in m_kMobDataList)
        {
            if(szName.ToUpper()== items.Value.m_szName.ToUpper())
            {
                return items.Key;
            }
        }
        return 0;

    }

    public string GetMobName(int nMob)// 1부터
    {
        if(m_kMobDataList.ContainsKey(nMob))
        {
            return m_kMobDataList[nMob].m_szName;
        }
        return "";
    }

    public string GetMobFile(bool bBoss, int nMob)// 1부터
    {
        if (m_kMobDataList.ContainsKey(nMob))
        {
            if(bBoss)
            {
                return  m_kMobDataList[nMob].m_szBossFile;
            }
            return m_kMobDataList[nMob].m_szFile;
        }
        return "";
    }
    public string GetMobMissile(bool bBoss, int nMob)// 1부터
    {
        if (m_kMobDataList.ContainsKey(nMob))
        {
            if (bBoss)
            {
                return m_kMobDataList[nMob].m_szBossMissile;
            }
            return m_kMobDataList[nMob].m_szMissile;
        }
        return "";
    }



    #endregion

    #region  컬러 테이블 

    public struct ColorData
    {

        public string m_szName;
        public int R;
        public int G;
        public int B;
        public int nID;
        public Color kColor;
        public COLORNUMBER m_nColorNumber;
    }


    Dictionary<int, ColorData> m_kColorDataList = new Dictionary<int, ColorData>();


    void InitColorData()
    {
        m_kColorDataList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_아이템 - 칼러");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                ColorData kInfo = new ColorData();
                kInfo.m_szName = cs.GetString(v.Key, "name");
                kInfo.R = cs.GetInt(v.Key, "R");
                kInfo.G = cs.GetInt(v.Key, "G");
                kInfo.B = cs.GetInt(v.Key, "B");
                kInfo.nID = cs.GetInt(v.Key, "ItemID");

                Color kColor = new Color();
                kColor.r = ((float)kInfo.R / 255f);
                kColor.g = ((float)kInfo.G / 255f);
                kColor.b = ((float)kInfo.B / 255f);
                kInfo.kColor = kColor;
                kInfo.m_nColorNumber = CWGlobal.GetColorItem((GITEM)kInfo.nID);
                m_kColorDataList.Add(kInfo.nID, kInfo);
            }
        }

    }

    public int GetColorItemID(COLORNUMBER  num)
    {

        foreach(var v in m_kColorDataList)
        {
            if(v.Value.m_nColorNumber==num)
            {
                return v.Value.nID;
            }
        }
        return 0;
    }

    public COLORNUMBER GetColorItemIDByColoritem(int nItemID)
    {
        if (m_kColorDataList.ContainsKey(nItemID))
        {
            return m_kColorDataList[nItemID].m_nColorNumber;
        }
        return COLORNUMBER.NONE;

    }
    public Color GetColor(int nID)
    {
        if(m_kColorDataList.ContainsKey(nID))
        {
            return m_kColorDataList[nID].kColor;
        }
        return Color.white;
    }
    public int GetColorItem(Color kColor)
    {

        int nRet = 0;
        float fMin = 10000f;

        foreach(var v in m_kColorDataList)
        {
            float ff = CWMath.GetDistColor(v.Value.kColor, kColor);
            if (ff < fMin)
            {
                fMin = ff;
                nRet = v.Key;
            }

        }

        return nRet;

    }
    public int GetRandomColor()
    {
        int RR = CWLib.Random(0, m_kColorDataList.Count);
        int nCount = 0;
        foreach (var v in m_kColorDataList)
        {
            if(nCount==RR)
            {
                return v.Value.nID;
            }
            nCount++;
        }
        return 0;
    }


    #endregion
    #region 가격표

    public struct COINDATA
    {
        public string PID;
        public string [] Price;
    }

    public Dictionary<string, COINDATA> m_kCoinData = new Dictionary<string, COINDATA>();

    void InitCoinData()
    {
        m_kCoinData.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("가격표 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                COINDATA kData = new COINDATA();

                kData.PID = cs.GetString(v.Key, "PID");
                kData.Price = new string[3];
                kData.Price[0] = cs.GetString(v.Key, "KRW"); 
                kData.Price[1] = cs.GetString(v.Key, "USD"); 
                kData.Price[2] = cs.GetString(v.Key, "JPY");
                

                m_kCoinData.Add(kData.PID, kData);
            }
        }

    }
    
    public string GetPrice(string PID)
    {
        int nval = CWPrefsManager.Instance.GetLanguage();// 현재 랭귀지 
        if (nval > 2) nval = 1;
        if(m_kCoinData.ContainsKey(PID))
        {
            return m_kCoinData[PID].Price[nval];
        }
        return "";
    }


    #endregion


    


    #region PVP 정보

    public struct PVPData
    {

        public int m_nLevel;// 
        public int m_nDiff;// 
        public float m_fCooltime;// 
        public float m_fSpeedRate;// 
        public float m_fRange;// 

    }
    List<PVPData> m_kPVPDataList = new List<PVPData>();
    void InitPVPData()
    {
        m_kPVPDataList.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("PVP_AI - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                PVPData kInfo = new PVPData();
                kInfo.m_nDiff = cs.GetInt(v.Key, "격차");
                kInfo.m_nLevel = cs.GetInt(v.Key, "레벨");


                kInfo.m_fCooltime = cs.GetFloat(v.Key, "Cooltime");
                kInfo.m_fSpeedRate = cs.GetFloat(v.Key, "SpeedRate");
                kInfo.m_fRange = cs.GetFloat(v.Key, "사정거리");
                m_kPVPDataList.Add(kInfo);
            }
        }

    }

    public PVPData GetPVPData(int Diff,int Level)
    {
        for (int i = 0; i < m_kPVPDataList.Count; i++)
        {
            if (Diff == m_kPVPDataList[i].m_nDiff
                && Level == m_kPVPDataList[i].m_nLevel)
            {

                return m_kPVPDataList[i];
            }

        }
        return m_kPVPDataList[0];
    }



    #endregion


    #region GradePower 정보

    class GradePowerData
    {
        public int m_nLevel;
        public int m_nDamage;// 
        public int m_nHP;// 

    }

    class GRADEDATA
    {
        public GradePowerData[] kData = new GradePowerData[4];
    }

    Dictionary<int, GRADEDATA> m_kGradeData = new Dictionary<int, GRADEDATA>();

    


    void InitGradePowerData()
    {
        m_kGradeData.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("등급능력치 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                

                int Grade = cs.GetInt(v.Key, "등급");
                int Level = cs.GetInt(v.Key, "레벨");
                int Damage = cs.GetInt(v.Key, "공격력");
                int hp = cs.GetInt(v.Key, "HP");
                if (!m_kGradeData.ContainsKey(Grade))
                {
                    GRADEDATA kNewGData = new GRADEDATA();
                    m_kGradeData.Add(Grade, kNewGData);
                }

                GRADEDATA kGData = m_kGradeData[Grade];
                kGData.kData[Level - 1] = new GradePowerData();
                kGData.kData[Level - 1].m_nDamage = Damage;
                kGData.kData[Level - 1].m_nHP = hp;


            }
        }

    }

    public int GetGradeHP(int Ranking,int Grade)
    {
        if (!m_kGradeData.ContainsKey(Grade)) return 0;

        if (Ranking == 0)
        {
            return m_kGradeData[Grade].kData[0].m_nHP;
        }

        if (Ranking < 10)
        {
            return m_kGradeData[Grade].kData[3].m_nHP; 
        }
        if (Ranking < 20)
        {

            return m_kGradeData[Grade].kData[2].m_nHP;
        }
        if (Ranking < 30)
        {

            return m_kGradeData[Grade].kData[1].m_nHP;
        }

        return m_kGradeData[Grade].kData[0].m_nHP;

    }
    public int GetGradeDamage(int Ranking, int Grade)
    {

        if (!m_kGradeData.ContainsKey(Grade)) return 0;
        if(Ranking==0)
        {
            return m_kGradeData[Grade].kData[0].m_nDamage;
        }
        if (Ranking < 10)
        {
            return m_kGradeData[Grade].kData[3].m_nDamage;
        }
        if (Ranking < 20)
        {

            return m_kGradeData[Grade].kData[2].m_nDamage;
        }
        if (Ranking < 30)
        {

            return m_kGradeData[Grade].kData[1].m_nDamage;
        }

        return m_kGradeData[Grade].kData[0].m_nDamage;
     
    }



    #endregion

    #region SlotPower 정보 슬롯 강화

    public class SlotPowerData
    {
        public int m_nLevel;
        public int m_blockcount;// 
        public int m_Price;// 
        public int m_AllPrice;// 

    }

    
    List<SlotPowerData> m_kSlotData = new List<SlotPowerData>();

    

    void InitSlotPowerData()
    {
        m_kSlotData.Clear();
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("슬롯강화 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {

                SlotPowerData ss = new SlotPowerData();
                ss.m_nLevel= cs.GetInt(v.Key, "level");
                ss.m_blockcount = cs.GetInt(v.Key, "blockcount");
                ss.m_Price = cs.GetInt(v.Key, "Price");
                ss.m_AllPrice = cs.GetInt(v.Key, "AllPrice");
                m_kSlotData.Add(ss);
            }
        }

    }

    public SlotPowerData GetSlotData(int blockcount)
    {
        int min = 0;
        for(int i=0;i< m_kSlotData.Count;i++)
        {
            if(blockcount>min && blockcount<= m_kSlotData[i].m_blockcount)
            {
                return m_kSlotData[i];
            }
            min = m_kSlotData[i].m_blockcount;

        }
        return null;
    }


    #endregion

    #region 비행블록


    int[] m_kShipData = new int[100];
    void InitShipBlockData()
    {

        int num = 0;
        m_kShipData = new int[100];
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_아이템 - 비행기블록");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                m_kShipData[v.Key] = cs.GetInt(v.Key, "ItemID");
            }
        }

    }
    // 순서에 따라서 아이템 번호를 리턴한다
    public int GetShipBlockID(int num)
    {
        return  m_kShipData[num];
    }
    public int GetShipBlockbyID(int nID)
    {
        for(int i=0;i< m_kShipData.Length; i++)
        {
            if (m_kShipData[i] == nID) return i;
        }
        return 0;
    }


    #endregion
}
