using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWUnityLib;
using System.Threading;
using System;
using CWStruct;
// 맵 오브젝트

public class CWFaceMap : MonoBehaviour
{
    int[,] g_vNormal =
    {
            {0,0,-1},
            {0,0, 1},
            {1,0, 0},
            {-1,0, 0},
            {0,1, 0},
            {0,-1, 0},
     };
    Vector3Int [] g_vDir8 =
    {
        new Vector3Int(1,0,0),
        new Vector3Int(0,1,0),
        new Vector3Int(0,0,1),

        new Vector3Int(1,1,0),
        new Vector3Int(0,1,1),
        new Vector3Int(1,0,1),

        new Vector3Int(0,0,0),
        new Vector3Int(1,1,1),

    };


    public string Teststr;

    bool m_bUpdated = false;

    Dictionary<int, int> m_kBlockHP = new Dictionary<int, int>();


    VERTEXBLOCK Zero = new VERTEXBLOCK();

    #region 맵의 높이값
    int m_nGrid = 16;
    //AI 전용 높이값
    public float CalHeightByAI(Vector3 vPos)
    {
        float fx = vPos.x;
        float fz = vPos.z;
        int sx = (int)((fx / m_nGrid) * m_nGrid);
        int sz = (int)((fz / m_nGrid) * m_nGrid);

        int ex = (int)(((fx + m_nGrid) / m_nGrid) * m_nGrid);
        int ez = (int)(((fz + m_nGrid) / m_nGrid) * m_nGrid);

        //4개의 점
        float v1 = GetHeight(sx, sz); //(0,0)
        float v2 = GetHeight(ex, sz); //(1,0)
        float v3 = GetHeight(sx, ez); //(0,1)
        float v4 = GetHeight(ex, ez); //(1,1)

        v1 = Mathf.Max(v1, 20);
        v2 = Mathf.Max(v2, 20);
        v3 = Mathf.Max(v3, 20);
        v4 = Mathf.Max(v4, 20);
        // fx 10.4
        // 4 
        // x 축에 대한 값
        float fdist = m_nGrid;

        float fstartx = fx - sx;//x축의 시작 
        float fRatex = fstartx / fdist; //전체에 대한 비율 
        float fvalx = v1 + (v2 - v1) * fRatex;


        float fstartz = fz - sz;//x축의 시작 
        float fRatez = fstartz / fdist; //전체에 대한 비율 
        float fvalz = v3 + (v4 - v3) * fRatez;

        float fResult = (fvalx + fvalz) / 2;

        return fResult+2;

    }
    #endregion

    #region 맵에 사용되는 공용변수


    public int m_nTotalCount = 0;
    public int m_nAddCount = 0;


    int _Worldsize = 256;
    public int WORLDSIZE
    {
        get
        {
            return _Worldsize;
        }
        set
        {
            _Worldsize = value;
        }
    }
    public int m_nMapID;
    protected bool m_bLoad = false; // 로드를 하였는가?
    public int m_nRidius = 5;
    public int m_nSellWidth = 0;
    public int m_nSellHeight = 0;

    int HEIGHT = 64;

    
    public int m_nOwnerLevel = 0;
    public string m_szOwnerTitle="내 땅이다";
    public string m_szOwnerName;
    public string m_szOwnerFace;

    public float m_fAmp = 0.01f;
    protected bool m_bDugeon = false;

    public List<int> m_kClanMember = new List<int>();

    struct HDATA
    {
        public byte Height;
        public byte Block;
    }
    HDATA[] m_kHeight;
    //byte[] m_bHeights;


    #endregion

    #region 맵에 사용되는 오브젝트
    protected GameObject m_MapSellDir;
    public GameObject m_ObjectDir;
    protected GameObject m_StartObject;

    public GameObject m_gMeshObject;





    #endregion

    

    #region 터렛 로드 
    
    public void MobLoad(CWJSon jSon)
    {

        CWMobManager.Instance.Load(jSon);

    }

    #endregion


    #region 메쉬 오브젝트
    public CWSellGroup m_gMeshPreb;
    protected CWSellGroup[] m_kSellGroup;


    #endregion

    #region 로드/세이브

    public bool m_bLoadEndTask;// 로드가 완료되었다

    public bool IsLoadEndTask()
    {
        return m_bLoadEndTask;// 로드가 완료되었다
    }


    public void CreateMap(int nID,int nWidth)
    {
        m_bUpdated = false;
        WORLDSIZE = nWidth;
        HEIGHT = CWGlobal.WD_WORLD_HEIGHT;

        m_nSellHeight = HEIGHT / CWGlobal.WD_WORLD_HEIGHT;

        m_bLoadEndTask = false;
        //월드싸이즈 
        Close();
        m_bLoad = true;

        m_ObjectDir = new GameObject();
        m_ObjectDir.transform.parent = transform;
        m_ObjectDir.transform.localPosition = Vector3.zero;
        m_ObjectDir.transform.localScale = Vector3.one;
        m_ObjectDir.transform.localRotation = new Quaternion();
        m_ObjectDir.name = "ObjectDir";

        m_StartObject = new GameObject();
        m_StartObject.transform.parent = transform;
        m_StartObject.transform.localPosition = Vector3.zero;
        m_StartObject.transform.localScale = Vector3.one;
        m_StartObject.transform.localRotation = new Quaternion();


        m_StartObject.name = "StartObject";



        m_MapSellDir = new GameObject();
        m_MapSellDir.transform.parent = transform;

        m_MapSellDir.transform.localPosition = Vector3.zero;
        m_MapSellDir.transform.localScale = Vector3.one;
        m_MapSellDir.transform.localRotation = new Quaternion();

        m_MapSellDir.name = "MapSellDir";
        m_nMapID = nID;
        m_kUpdateSell.Clear();

        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;
        m_kSellGroup = new CWSellGroup[dx * dx];
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
                m_kSellGroup[num] = Instantiate(m_gMeshPreb);
                m_kSellGroup[num].transform.parent = m_MapSellDir.transform;

                m_kSellGroup[num].transform.localRotation = new Quaternion();

                m_kSellGroup[num].name = string.Format("sell_{0}_{1}", x, z);//"sell_" + x.ToString() + ;
                m_kSellGroup[num].Create(this, m_nMapID, x, z);
            }
        }

        int Cnt = WORLDSIZE * WORLDSIZE * HEIGHT;
        m_bBlockBuffer = new byte[Cnt];
        for(int z=0;z< WORLDSIZE; z++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    int num = (x * WORLDSIZE + z) * HEIGHT + y;
                    if(y==2)
                    {
                        m_bBlockBuffer[num] = 1;
                    }
                    else
                    {
                        m_bBlockBuffer[num] = 2;
                    }
                    
                }

            }

        }
        m_bUpdated = true;

        if (!m_bIUpdateSell)
            StartCoroutine("IUpdateSell");

    }
    public void CloseSelect()
    {
    }

    
    public bool LoadLocalMap(string szFile)
    {
        // 로컬파일이 있는지 검사

        
        CWJSon kJSonData = null;
        kJSonData = LoadData(szFile, true);
        if (kJSonData == null) return false;
        MobLoad(kJSonData);
        CWMapManager.Instance.CreateMask();
        InitFindBlock();// 찾을 블록 준비

        m_bUpdated = false;
        return true;
    }

    public bool Load(int nID)
    {
        m_nMapID = nID;
        string szpath = string.Format("MapData/Map_{0}", m_nMapID);
        CWJSon kJSonData = LoadData(szpath);
        if (kJSonData == null) return false;

        MobLoad(kJSonData);
        CWMapManager.Instance.CreateMask();
        InitFindBlock();// 찾을 블록 준비


        return true;
    }

    CWJSon SaveJSon()
    {
        CWJSon JSon = new CWJSon();
        JSon.Add("Amp", m_fAmp);
        JSon.Add("Worldsize", WORLDSIZE);
        JSon.Add("WorldHeight", HEIGHT);

        //m_StartObject.transform.localPosition = m_vStartPos;

        JSon.Add("posx", m_StartObject.transform.localPosition.x);
        JSon.Add("posy", m_StartObject.transform.localPosition.y);
        JSon.Add("posz", m_StartObject.transform.localPosition.z);

        JSon.Add("Yaw", m_StartObject.transform.localEulerAngles.y);


        {//objectdir 저장
            JArray array = new JArray();
            for (int i = 0; i < m_ObjectDir.transform.childCount; i++)
            {

                Transform tChild = m_ObjectDir.transform.GetChild(i);
                if (tChild)
                {
                    JObject jt = new JObject();
                    jt.Add("name", tChild.name);


                    jt.Add("x", tChild.localPosition.x);
                    jt.Add("y", tChild.localPosition.y);
                    jt.Add("z", tChild.localPosition.z);

                    jt.Add("rx", tChild.localEulerAngles.x);
                    jt.Add("ry", tChild.localEulerAngles.y);
                    jt.Add("rz", tChild.localEulerAngles.z);

                    jt.Add("sx", tChild.localScale.x);
                    jt.Add("sy", tChild.localScale.y);
                    jt.Add("sz", tChild.localScale.z);

                    array.Add(jt);
                }

            }

            JSon.Add("ObjectDir", array);

        }

        //CWMobManager.Instance.Save(JSon);
        SaveBlockData(JSon);
        return JSon;

    }

    public CWJSon LoadData(string szFile,bool blocalfile=false)
    {

        m_nTotalCount = 0;
        m_nAddCount = 0;

        m_bUpdated = false;
        Close();
        m_bLoadEndTask = false;

        m_bLoad = true;

        CWJSon kJSonData = new CWJSon();
        m_ObjectDir = new GameObject();
        m_ObjectDir.transform.parent = transform;
        m_ObjectDir.transform.localPosition = Vector3.zero;
        m_ObjectDir.transform.localScale = Vector3.one;
        m_ObjectDir.transform.localRotation = new Quaternion();
        m_ObjectDir.name = "ObjectDir";

        m_StartObject = new GameObject();
        m_StartObject.transform.parent = transform;
        m_StartObject.transform.localPosition = Vector3.zero;
        m_StartObject.transform.localScale = Vector3.one;
        m_StartObject.transform.localRotation = new Quaternion();

        m_StartObject.name = "StartObject";


        m_MapSellDir = new GameObject();
        m_MapSellDir.transform.parent = transform;

        m_MapSellDir.transform.localPosition = Vector3.zero;
        m_MapSellDir.transform.localScale = Vector3.one;
        m_MapSellDir.transform.localRotation = new Quaternion();
        m_MapSellDir.name = "MapSellDir";


        kJSonData = new CWJSon();
        if(blocalfile)
        {
            if(kJSonData.LoadLocal(szFile)==false)
            {
                return null;
            }
        }
        else
        {
            if (kJSonData.Load(szFile) == null)
            {
                return null;
            }

        }
        WORLDSIZE = kJSonData.GetInt("Worldsize");
        if (WORLDSIZE == 0)
        {
            return null;
        }
        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;
        m_kSellGroup = new CWSellGroup[dx * dx];
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
                m_kSellGroup[num] = Instantiate(m_gMeshPreb);
                m_kSellGroup[num].transform.parent = m_MapSellDir.transform;

                m_kSellGroup[num].transform.localRotation = new Quaternion();

                m_kSellGroup[num].name = string.Format("sell_{0}_{1}", x, z);//"sell_" + x.ToString() + ;
                m_kSellGroup[num].Create(this, m_nMapID, x, z);
            }
        }


        HEIGHT = CWGlobal.WD_WORLD_HEIGHT;
        if (HEIGHT == 0)
        {
            HEIGHT = CWGlobal.WD_HEIGHT;
        }



        m_nSellWidth = WORLDSIZE / CWGlobal.SELLCOUNT;
        m_nSellHeight = HEIGHT / CWGlobal.WD_WORLD_HEIGHT;

        m_fAmp = kJSonData.GetFloat("Amp");


        Vector3 vStartPos;
        vStartPos.x = kJSonData.GetFloat("posx");
        vStartPos.y = kJSonData.GetFloat("posy");
        vStartPos.z = kJSonData.GetFloat("posz");
        float fYaw = kJSonData.GetFloat("Yaw");
        m_StartObject.transform.localPosition = vStartPos;
        m_StartObject.transform.localEulerAngles = new Vector3(0, fYaw, 0);





        {// 오브젝트 
            JArray array = (JArray)kJSonData.GetJson("ObjectDir");
            if (array != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    JToken tt = array[i];

                    string str = CWJSon.GetString(tt, "name");
                    float x = CWJSon.GetFloat(tt, "x");
                    float y = CWJSon.GetFloat(tt, "y");
                    float z = CWJSon.GetFloat(tt, "z");

                    float rx = CWJSon.GetFloat(tt, "rx");
                    float ry = CWJSon.GetFloat(tt, "ry");
                    float rz = CWJSon.GetFloat(tt, "rz");

                    float sx = CWJSon.GetFloat(tt, "sx");
                    float sy = CWJSon.GetFloat(tt, "sy");
                    float sz = CWJSon.GetFloat(tt, "sz");

                    GameObject gg = CWResourceManager.Instance.GetPrefab(str);
                    if (gg != null)
                    {
                        gg.name = str;
                        gg.transform.parent = m_ObjectDir.transform;
                        gg.transform.localPosition = new Vector3(x, y, z);
                        gg.transform.localEulerAngles = new Vector3(rx, ry, rz);
                        gg.transform.localScale = new Vector3(sx, sy, sz);

                    }

                }
            }

        }

        if (!LoadBlockData(kJSonData, blocalfile))
        {
            return null;
        }

        return kJSonData;
    }
    public void SaveData(string szFile)
    {
        if (!m_bUpdated) return;
        m_bUpdated = false;
        CWJSon JSon = SaveJSon();
        JSon.SaveLocal(szFile);

    }
    virtual public void Save()
    {
#if UNITY_EDITOR
        if (m_nMapID == 0) return;
        if (!m_bLoad) return;

        CWJSon JSon = SaveJSon();
        CWMobManager.Instance.Save(JSon);
        string szpath = string.Format("{0}/Resources/MapData/Map_{1}.bytes", Application.dataPath, m_nMapID);
        JSon.Save(szpath);


#endif
    }
    public void SaveBlockCount()
    {
        if (Space_Map.Instance == null) return;
        // 전체 블록 개수 
        int nStage = Space_Map.Instance.GetStageID();
        //string sztotal = string.Format("Total_{0}_{1}", CWHero.Instance.m_nID, nStage);
        //PlayerPrefs.SetInt(sztotal, m_nTotalCount);
        CWHeroManager.Instance.SetTotal(nStage, m_nTotalCount);
        //string szDel = string.Format("Del_{0}_{1}", CWHero.Instance.m_nID, nStage);
        //PlayerPrefs.SetInt(szDel, m_nAddCount);
        CWHeroManager.Instance.SetAdd(nStage, m_nAddCount);
        float fRate = (float)m_nAddCount / m_nTotalCount;
        if(fRate >0.7f)
        {
            CWGlobal.g_bMapClear=true;//
        }
        CWHeroManager.Instance.SaveMapData(nStage);



    }
    public void Close()
    {

        m_bIUpdateSell = false;
        m_bILoadRun = false;
        m_bLoad = false;
        StopAllCoroutines();

        if (m_ObjectDir != null)
        {
            Destroy(m_ObjectDir);
        }
        if (m_StartObject != null)
        {
            Destroy(m_StartObject);
        }
        if (m_MapSellDir != null)
        {
            Destroy(m_MapSellDir);
        }

        
        if (m_kSellGroup != null)
        {
            foreach (var v in m_kSellGroup)
            {
                if (v == null) continue;
                v.Close();
            }
        }
        m_kSellGroup = null;
        

        m_kSendDeleteSell.Clear();
        m_kUpdateSell.Clear();

        

    }
    
    
    public void Select()
    {
     
        if (!CWGlobal.g_bSingleGame)
        {
            StartCoroutine("BlockSendRun");
        }
        MakeBlockMesh();
        
    }
   

    #endregion

    #region 전송데이타

    #endregion



    #region 지워진 맵셀


    List<MAPBLOCK> m_kSendDeleteSell = new List<MAPBLOCK>();

    
    


    public void ReceveDelBlock(JObject jData)
    {
        //int tcnt = jData;
        JArray jarry = (JArray)jData["blocks"];
        int tcnt = jarry.Count;
        for (int i = 0; i < tcnt; i++)
        {
            JObject jj = (JObject)jarry[i];
            int x = jj["x"].ToObject<int>();
            int y = jj["y"].ToObject<int>();
            int z = jj["z"].ToObject<int>();
            int nblock = jj["block"].ToObject<int>();
            if (nblock == 0) continue;
            int nBlock = GetBlock(x, y, z);
            if (nblock == 1)
            {
                nblock = 0;
                if (nblock == 0) continue;// 이미 0이다 
                //CWBombManager.Instance.RegBomb(CWHero.Instance.m_nID, false, x, y, z, nBlock, new Vector3(x, y, z), 1500, 300, 1);
                UpdateBlock(x, y, z, 0);
            }
            else
            {
                if (nblock == nBlock) continue;// 결과가 같음
                UpdateBlock(x, y, z, nblock);
            }


        }


    }
    public void SendBlock()
    {
        if (m_kSendDeleteSell.Count == 0) return;
        CWSocketManager.Instance.SendUserBlock(m_kSendDeleteSell);
        m_kSendDeleteSell.Clear();
    }
    public void AddSendBlock(int x, int y, int z, int nBlock)
    {
        int nSerBlock = 0;
        UpdateBlock(x, y, z, nBlock, false);

        if (nBlock==0)
        {
            int ns = GetServerBlock(x, y, z);
            if(ns>=128) nSerBlock = ns;
            else
            {
                nSerBlock = ns + 128;
            }
        }
        else
        {
            //아이템 번호로 바꿈
            if (nBlock == 128)
            {
                nSerBlock = 128;
            }
            else
            {
                nSerBlock = CWArrayManager.Instance.GetItemFromBlock(nBlock);
            }
            
        }

        if (nSerBlock == 0) return;
        MAPBLOCK bdata = new MAPBLOCK(x, y, z, nSerBlock);
        m_kSendDeleteSell.Add(bdata);



    }

    public void AddDelBlock(int x, int y, int z)
    {
        if (CWGlobal.g_bSingleGame) return; // 싱글게임에서는 리턴

        int n = GetBlock(x, y, z);
        if (n == 0)
        {
            return;
        }
        AddSendBlock(x, y, z, 0);
    }
    // 서버에서 온 블록 값

    int GetServerBlock(int x,int y,int z)
    {
        if (m_bServerBlock == null) return 0;

        int num = (x * WORLDSIZE + z) * 64 + y;
        if (num < 0) return 0;
        if (num >= m_bServerBlock.Length) return 0;
        return m_bServerBlock[num];
    }
    public void ServerBlock(byte[] bBuffer)
    {
        m_bServerBlock = null;
        if (bBuffer == null) return;

        m_bServerBlock = CWLib.UncompressByte(bBuffer, 0, bBuffer.Length);

        CreateDelBuffer();

        for (int i=0;i<m_bServerBlock.Length;i++)
        {
            if(m_bServerBlock[i]>0)
            {
                m_bDelBuffer[i] = 1;
            }
        }

        //        for (int x = 0; x < WORLDSIZE; x++)
        //        {
        //            for (int y = 0; y < 64; y++)
        //            {
        //                for (int z = 0; z < WORLDSIZE; z++)
        //                {

        //                    int num = (x * WORLDSIZE + z) * HEIGHT + y;
        //                    if (num >= m_bServerBlock.Length)
        //                    {
        //                        continue;
        //                    }
        //                    byte b = m_bServerBlock[num];
        //                    if (b == 0) continue; //0은 의미 없음
        //                    int n = 0;
        //                    if (b >= 128) n = 0; //128이상은 모두 지워진 파일
        //                    else
        //                    {
        //                        // 1~127 이하는 클랜에서 만든 파일 
        //                        n = CWArrayManager.Instance.GetBlockFromItem(b);
        //                    }
        //                    m_bDelBuffer[num] = 1;
        ////                   SetBlock(x, y, z, n);
        //                }
        //            }
        //        }

    }

    // 블록 복원
    public void RestoreBlock(byte[] bBuffer)
    {
        for (int x = 0; x < WORLDSIZE; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                for (int z = 0; z < WORLDSIZE; z++)
                {
                    var num = (x * WORLDSIZE + z) * 64 + y;
                    if (num >= bBuffer.Length)
                    {
                        continue;
                    }
                    byte b = bBuffer[num];
                    if (b == 0) continue; //0은 의미 없음
                    int n = 0;
                    if (b >= 128) n = 0; //128이상은 모두 지워진 파일
                    else
                    {
                        // 1~127 이하는 클랜에서 만든 파일 
                        n = CWArrayManager.Instance.GetBlockFromItem(b);

                    }

                    UpdateBlock(x, y, z, n);
                }
            }
        }


    }

    #endregion
    

    #region 코루틴 작동관련

    private void OnEnable()
    {
        

    }


    // 32 단위 
  

   

    protected CWSellGroup FindGroup(int x, int z)
    {
        if (m_kSellGroup == null)
        {

            return null;
        }
        foreach (var v in m_kSellGroup)
        {
            if (v == null)
            {
                continue;
            }
            if (v.m_nMapID > 0 && v.m_nSellX == x && v.m_nSellZ == z)
            {
                return v;
            }
        }
        return null;
    }

    
    IEnumerator BlockSendRun()
    {
        while (true)
        {

            SendBlock();
            yield return new WaitForSeconds(0.1f);
        }
    }



    #endregion

    #region 블록 검색 관련

    // 자원 블록 분포 
    List<Vector3> m_kResPos = new List<Vector3>();

    List<int> m_kFindBlock = new List<int>();
    void InitFindBlock()
    {
        

        m_kFindBlock.Clear();
        m_kFindBlock.Add(m_nResblock1);
        m_kFindBlock.Add(m_nResblock2);

        for (int i = (int)OLDBLOC.Red; i <= (int)OLDBLOC.DarkGray; i++)
        {
            m_kFindBlock.Add(i);
        }

    }
    public Vector3 FindCenterResBlock()
    {


        
        Vector3 vRet= FindNearBlock(new Vector3(128,0,128), m_kFindBlock);

        
        return vRet;
    }
    public Vector3 FindNearResBlock(Vector3 vPos)
    {
        return FindNearBlock(vPos, m_kFindBlock);
    }


    Vector3 GetResHeightBlock(int tx,int tz, List<int> kBlock)
    {
        
        Vector3 vRet = Vector3.zero;
        for (int z = -3; z <= 3; z++)
        {
            for (int x = -3; x <= 3; x++)
            {
                vRet.x = tx + x;
                vRet.y = GetHeight(tx, tz);
                vRet.z = tz + z;
                int nBlock = GetHeightBlock(tx+x,tz+z);// GetBlock(vRet);
                if (kBlock.Exists(t => (t == nBlock)))
                {
                    return vRet;
                }

            }

        }
        return Vector3.zero;

    }

    // 가장 가까운 블록을 찾는다 
    Vector3 GetResBlock(Vector3 vPos, List<int> kBlock)
    {
        Vector3 vRet = Vector3.zero;
        for (int z=-3;z<=3;z++)
        {
            for (int y = -3; y <= 3; y++)
            {
                for (int x = -3; x <= 3; x++)
                {
                    vRet.x = vPos.x + x;
                    vRet.y = vPos.y + y;
                    vRet.z = vPos.z + z;
                    int nBlock = GetBlock(vRet);
                    if (kBlock.Exists(t => (t == nBlock)))
                    {
                        return vRet;
                    }

                }

            }

        }
        return Vector3.zero;

    }
    public Vector3 GetNearBlock( List<int> kBlock)
    {


        Vector3 vPos = CWHero.Instance.GetParmingCheck();
        Vector3 vRet = vPos;
        for (int i = 0;i<1000 ; i++)
        {
            int tx = 0, tz = 0;
            if(!CWCircleData.GetData(i, ref tx, ref tz))
            {
                break;
            }
            vRet.x = vPos.x + tx;
            vRet.z = vPos.z + tz;
            int h = GetHeight((int)vRet.x, (int)vRet.z);
            if (h < 20) continue;


            Vector3 vv = GetResBlock(m_kResPos[i], kBlock);
            if (vv != Vector3.zero)
            {
                return vv;
            }

        }
        float fx = UnityEngine.Random.Range(64, 180);
        float fz = UnityEngine.Random.Range(64, 180);

        return new Vector3(fx,40,fz);
       
    }
    static  int CompareDist(Vector3 a, Vector3 b)
    {
        //Vector3 v = new Vector3(128,30,128); //; CWHero.Instance.GetPosition();
        Vector3 v = CWHero.Instance.GetParmingCheck();
        float f1 = Vector3.Distance(v, a);
        float f2 = Vector3.Distance(v, b);

        return (int)(f1 - f2);

    }

    bool m_bSearchflag = false;

    public Vector3 FindNearBlock(Vector3 vPos, List<int> kBlock)
    {
        if(m_bSearchflag==false)
        {
            m_kResPos.Sort(CompareDist);
            m_bSearchflag = true;
        }

        
        for (int i=0;i< m_kResPos.Count; i++)
        {
            Vector3 vv = GetResBlock(m_kResPos[i], kBlock);
            if (vv != Vector3.zero)
            {
                float fdist = Vector3.Distance(vPos, vv);
                if(fdist>32)// 거리가 너무 멀면
                {
                    m_bSearchflag = false;
                }
                return vv;
            }

        }
        return GetNearBlock( kBlock);
    }


    #endregion


    #region 블록제어

    // 근처의 블록을 찾는다
    // 중앙에서만 검색
    public Vector3 FindBlock(Vector3 vPos, int nBlock, Vector3 vPrePos)
    {



        for (int i = 0; ; i++)
        {
            int tx = 0, tz = 0;
            if (!CWCircleData.GetData(i, ref tx, ref tz))
            {
                break;
            }
            int xx = (int)vPos.x + tx;
            int zz = (int)vPos.z + tz;

            int h = GetHeight(xx, zz);
            for (int ih = h; ih > 0; ih--)
            {
                int n = GetBlock(xx, ih, zz);
                if (n == nBlock)
                {
                    return new Vector3(xx, h, zz);
                }

            }


        }

        return Vector3.zero;
    }
    public float GetDir()
    {
        return m_kSellGroup[0].GetDir();

    }
    public Vector3 GetCameraPos()
    {
        Vector3 vPos = Camera.main.transform.position ;
        return SelectPos(vPos, Vector3.up);
    }
    public Vector3 SelectPos(Vector3 vPos, Vector3 vNormal)
    {
        Vector3 v = vPos - vNormal;

        float fx = v.x + (float)WORLDSIZE / 2; // 양수로 만듦
        if (vNormal.x < 0)
        {
            fx--;
        }
       

        int nx = (int)fx - WORLDSIZE / 2; 
        float fz = v.z + WORLDSIZE / 2;
        if (vNormal.z < 0)
        {
            fz--;
        }
        


        int nz = (int)fz - WORLDSIZE / 2;
        int ny = (int)v.y;

        if(vNormal.y<0)
        {
            ny--;
        }

        return new Vector3(nx, ny, nz);

    }

    public Vector3 GetEditBlock(Vector3 vPos, Vector3 vNormal)
    {
        float fx = vPos.x + (float)WORLDSIZE / 2;
        if (vNormal.x < 0)
        {
            fx--;
        }
        int nx = (int)fx - WORLDSIZE / 2;
        float fz = vPos.z + (float)WORLDSIZE / 2;
        if (vNormal.z < 0)
        {
            fz--;
        }
        int nz = (int)fz - WORLDSIZE / 2;
        int ny = (int)vPos.y;
        if (vNormal.y < 0)
        {
            ny--;
        }


        return new Vector3(nx, ny, nz);


    }
    
    public int GetHeight(Vector3 vPos)
    {
        Vector3Int vInt = CWMapManager.ConvertPos(vPos);
        return GetHeight(vInt.x, vInt.z);
    }
    void CheckHeight(int x,int z)
    {
        int num = x * WORLDSIZE + z;
        int h= m_kHeight[num].Height;
        if (h < 2) return;
        if(GetBlock(x,h,z)==0)
        {
            
            // 다시 세팅해야 한다
            for(int i=h;i>=0;i--)
            {
                int n = GetBlock(x, i, z);
                if(n>0)
                {
                    m_kHeight[num].Height =(byte) i;
                    m_kHeight[num].Block = (byte)n;
                }
            }

        }

    }
    //AI들의 높이값을 조절한다
    // 기준값을 잡아라
    //401 405 402.6
    float GetRate(int x1,int x2,float fx)
    {
        float vx1 = x1;
        float vx2 = x2;
        float fdist = vx2 - vx1;
        float fdelta =  (vx2 - fx)/ fdist;
        return fdelta;
    }
    
    public float GetHeightAI(Vector3 vPos)
    {
        int fx1   = (int)(vPos.x-2);
        int fx2  = (int)(vPos.x+2);

        int fz1 = (int)(vPos.z - 2);
        int fz2 = (int)(vPos.z + 2);

        float h1 = GetHeight(fx1, fz1);
        float h2 = GetHeight(fx1, fz2);

        float fratex=  vPos.x; //f

        float h3 = GetHeight(fx1, fz1);
        float h4 = GetHeight(fx2, fz1);

        float fratez = vPos.z; //fx2,fx1두개의 위치 비율

        float fvaluex = h1 + (h2 - h1) * fratex;
        float fvaluez = h3 + (h4 - h3) * fratez;


        return (fvaluex + fvaluez) / 2;




    }
    public int GetHeight(int x, int z)
    {
        if (m_kHeight == null) return 0;
        if (x < 0) return 0;
        if (z < 0) return 0;
        if (x >= WORLDSIZE) return 0;
        if (z >= WORLDSIZE) return 0;
        CheckHeight(x, z);

        int num = x * WORLDSIZE + z;
        
        return m_kHeight[num].Height;

    }
    public int GetHeightBlock(int x,int z)
    {

        if (m_kHeight == null) return 0;
        if (x < 0) return 0;
        if (z < 0) return 0;
        if (x >= WORLDSIZE) return 0;
        if (z >= WORLDSIZE) return 0;
        CheckHeight(x, z);
        int num = x * WORLDSIZE + z;
        return m_kHeight[num].Block;

    

    }
    public int GetHeight(int x, int y, int z)
    {
        int tx = x;
        int tz = z;
        int nx = ((tx % CWGlobal.SELLCOUNT) + CWGlobal.SELLCOUNT) % CWGlobal.SELLCOUNT;
        int nz = ((tz % CWGlobal.SELLCOUNT) + CWGlobal.SELLCOUNT) % CWGlobal.SELLCOUNT;
        CWMapSell pMapSell = GetMapSell(x, z);
        if (pMapSell == null)
        {
            return 0;
        }
        return pMapSell.GetHeight(nx, y, nz);
    }

    public int GetBlock(Vector3 vPos)
    {
        Vector3Int vInt = CWMapManager.ConvertPos(vPos);
        return GetBlock((int)vInt.x, (int)vInt.y, (int)vInt.z);
    }
    //public int GetBlock(int x, int y, int z)
    //{
        
    //    int tx = x;
    //    int tz = z;
    //    int nx = ((tx % CWGlobal.SELLCOUNT) + CWGlobal.SELLCOUNT) % CWGlobal.SELLCOUNT;
    //    int nz = ((tz % CWGlobal.SELLCOUNT) + CWGlobal.SELLCOUNT) % CWGlobal.SELLCOUNT;
    //    CWMapSell pMapSell = GetMapSell(x, y, z);
    //    if (pMapSell == null)
    //    {
    //        return 0;
    //    }

    //    return pMapSell.GetBlock(nx, y % CWGlobal.WD_WORLD_HEIGHT, nz);

    //}

    public CWMapSell GetMapSell(int x, int z)
    {

        CWSellGroup kGroup = FindGroup(x / CWSellGroup.SELLSIZE, z / CWSellGroup.SELLSIZE);
        if (kGroup)
        {
            return kGroup.GetSell(x / CWGlobal.SELLCOUNT,  z / CWGlobal.SELLCOUNT);
        }
        return null;

    }
    public CWSellGroup GetSellGroup(int x, int z)
    {

        return FindGroup(x / CWSellGroup.SELLSIZE, z / CWSellGroup.SELLSIZE);
    }

    #endregion

    

    #region 맵 충돌

    public void DelGameData(int x, int y, int z)
    {
        UpdateBlock(x, y, z, 0,true);
        AddDelData(x, y, z);
        
    }

    
    // 0 좌표에 맞추어 좌표 변환
    Vector3 ConvertPos(Vector3 vPos)
    {
        Vector3 v = Vector3.zero;// transform.position;
        v.x -= 0.5f;
        v.y -= 0.5f;
        v.z -= 0.5f;
        vPos -= v;
        return vPos;
    }
    // 석탄,다이아몬드 

    bool IsDestroyBlock(int x,int y,int z,int nBlock)
    {

        int nHP =CWArrayManager.Instance.GetBlockHP(nBlock);
        if (nHP == 0) return true;

        int num = (x * WORLDSIZE + z) * HEIGHT + y;

        if(!m_kBlockHP.ContainsKey(num))
        {
            m_kBlockHP.Add(num, nHP);
        }

        m_kBlockHP[num]--;
        if(m_kBlockHP[num]<=0)
        {
            m_kBlockHP.Remove(num);
            return true;
        }

        return false;
        
    }

    IEnumerator IHit( Vector3 vPos, int fBlockCount)
    {
        yield return null;
        int nx = 0, ny = 0, nz = 0;
        Vector3Int vInt = CWMapManager.ConvertPos(vPos);
        nx = (int)vInt.x;
        ny = (int)vInt.y;
        nz = (int)vInt.z;
       

        int tcnt = 0;
        
        CWBombManager.Instance.Begin_Map();

        bool bItemflag = false;
        for (int i = 0; i < 100 * fBlockCount; i++)
        {
            int tx = 0, ty = 0, tz = 0;
            if (!CWSphereData.GetData(i, ref tx, ref ty, ref tz)) break;
            int ix = nx + tx;
            int iy = ny + ty;
            int iz = nz + tz;
            if (iy < 1) continue;
            int nBlock = GetBlock(ix, iy, iz);
            if (nBlock > 0)
            {
                if(CWHero.Instance)
                {
                    CWHero.Instance.CheckBlockCount();
                }
                tcnt++;
                if(tcnt%5000==0)
                {
                    yield return null;
                }
                {
                    bool bbflag = true;

                    CWBombManager.Instance.Reg_Map(ix, iy, iz, nBlock, bbflag);
                  
                    {
                        if(EquipInvenList.Instance)
                            EquipInvenList.Instance.AddItemByBlock(nBlock);
                        if (nBlock == m_nResblock2)
                        {
                            m_nResBlockAddCount2++;
                            if (m_nResBlockAddCount2 >= m_nMaxResblock2)
                            {
                                m_nResblock2 = 0;//4개 이상 더이상 안받는다!!!!
                            }
                            int nStage = Space_Map.Instance.GetStageID();  
                            CWHeroManager.Instance.SetRes2(nStage, m_nResBlockAddCount2);
                            bItemflag = true;

                        }
                        if (nBlock == m_nResblock3)
                        {
                            m_nResBlockAddCount3++;
                            if (m_nResBlockAddCount3 >= m_nMaxResblock3)
                            {
                                m_nResblock3 = 0;//4개 이상 더이상 안받는다!!!!
                            }
                            int nStage = Space_Map.Instance.GetStageID();// 
                            CWHeroManager.Instance.SetRes3(nStage, m_nResBlockAddCount3);
                            bItemflag = true;

                        }



                        AddDelBlock(ix, iy, iz);
                    }
                    
                }
                DelGameData(ix, iy, iz);
                if (tcnt >= fBlockCount)
                {
                    break;
                }
            }
        }
        CWBombManager.Instance.End_Map();

        yield return null;
        
        

    }

    // 블록을 가져간다
    public void TakeBlock(int ix,int iy,int iz, int nBlock)
    {

        //if(nBlock==(int)GITEM.Diamond)
        //{
        //    CWSocketManager.Instance.UseCoinEx(COIN.GEM, 1);
        //    NoticeMessage.Instance.Show("다이아몬드를 획득하였습니다!");
        //}
        //else
        {
            if (nBlock == m_nResblock2)
            {
                m_nResBlockAddCount2++;

                if (CWHeroManager.Instance)
                {
                    int nStage = Space_Map.Instance.GetStageID();
                    CWHeroManager.Instance.SetRes2(nStage, m_nResBlockAddCount2);

                    FindBlockDlg.Instance.Show(m_nResblock2);
                }
            }
            if (nBlock == m_nResblock3|| nBlock == (int)OLDBLOC.Diamond)
            {
                m_nResBlockAddCount3++;

                if (CWHeroManager.Instance)
                {
                    int nStage = Space_Map.Instance.GetStageID();
                    CWHeroManager.Instance.SetRes3(nStage, m_nResBlockAddCount3);

                    FindBlockDlg.Instance.Show(nBlock);

                    RadarUI.Instance.UpdateBlock(ix, iy, iz);
                }
            }
            if (EquipInvenList.Instance)
                EquipInvenList.Instance.AddItemByBlock(nBlock);

        }


        AddDelBlock(ix, iy, iz);

        DelGameData(ix, iy, iz);

    }

    public bool Hit(bool myhit, Vector3 vPos, int fBlockCount,bool bDontDetect=true)//블록 폭파 개수  fBlockCount
    {
        if(myhit&& fBlockCount>100)
        {
            StartCoroutine(IHit(vPos,fBlockCount));
            return true;
        }
        int nx = 0, ny = 0, nz = 0;
        Vector3Int vInt = CWMapManager.ConvertPos(vPos);
        nx = (int)vInt.x;
        ny = (int)vInt.y;
        nz = (int)vInt.z;
            
        int tcnt = 0;
        if (myhit)
            CWBombManager.Instance.Begin_Map();

        for (int i = 0; i < 100 * fBlockCount; i++)
        {
            int tx = 0, ty = 0, tz = 0;
            if (!CWSphereData.GetData(i, ref tx, ref ty, ref tz)) break;

            int ix = nx + tx;
            int iy = ny + ty;
            int iz = nz + tz;
            if (iy <= 1) continue;
            int nBlock = GetBlock(ix, iy, iz);
            if(nBlock>0)
            {
                tcnt++;


                bool bbflag = true;
                if (!bDontDetect)
                {
                    int rr = CWLib.Random(0, 16);// 
                    if (rr != 1)
                    {
                        bbflag = false;
                    }
                }
                CWBombManager.Instance.Reg_Map(ix, iy, iz, nBlock, bbflag);
                if (myhit)
                {
                    TakeBlock(ix, iy, iz, nBlock);
                }
                if (tcnt >= fBlockCount)
                {
                    break;
                }
            }


            /*
                        if (nBlock > 0 )
                        {
                            tcnt++;
                            if (myhit)
                            {
                                bool bbflag=true;
                                if (!bDontDetect)
                                {
                                    int rr = CWLib.Random(0, 16);// 
                                    if(rr!=1)
                                    {
                                        bbflag = false;
                                    }
                                }


                                CWBombManager.Instance.Reg_Map(ix, iy, iz, nBlock, bbflag);
                                if(bbflag&& mytake)
                                {
                                    if(EquipInvenList.Instance)
                                        EquipInvenList.Instance.AddItemByBlock(nBlock);
                                    if(nBlock== m_nResblock2)
                                    {
                                        m_nResBlockAddCount2++;

                                        if(CWHeroManager.Instance)
                                        {
                                            int nStage = Space_Map.Instance.GetStageID();
                                            CWHeroManager.Instance.SetRes2(nStage, m_nResBlockAddCount2);
                                            CWHero.Instance.TakeHiddenBlock();
                                        }
                                    }
                                    if (nBlock == m_nResblock3)
                                    {
                                        m_nResBlockAddCount3++;

                                        if (CWHeroManager.Instance)
                                        {
                                            int nStage = Space_Map.Instance.GetStageID();
                                            CWHeroManager.Instance.SetRes3(nStage, m_nResBlockAddCount3);
                                            CWHero.Instance.TakeHiddenBlock();
                                        }
                                    }

                                }


                                AddDelBlock(ix, iy, iz);
                            }
                            DelGameData(ix, iy, iz);
                            if (tcnt >= fBlockCount)
                            {
                                break;
                            }
                        }
            */
        }
        
        if (myhit)
        {
            CWBombManager.Instance.End_Map();
            
        }
        return true;
    }

    


    #endregion
    #region 외부접근함수

    public Vector3 GetStartPos()
    {
        if (m_StartObject == null)
        {
            return Vector3.zero;
        }
        return m_StartObject.transform.position;
    }
    public float GetLookPos()
    {
        if (m_StartObject == null)
        {
            return 0;
        }
        return m_StartObject.transform.eulerAngles.y;
    }

    public bool IsWorldMap()
    {
        if (m_bDugeon) return false;
        return true;
    }


    public int ConvertLODBLOK(int x,int z,int nBlock)
    {
        if(nBlock>0)
            return GetHeightBlock(x, z);
        return 0;
        
    }
    // 반지름안이 평평한가?
    public bool IsFlat(Vector3 vPos,int nRadius)
    {

        float fh1 = GetHeight(vPos);
        Vector3Int vInt = CWMapManager.ConvertPos(vPos);
        for (int i = 0; i < 100; i++)
        {
            int tx = 0, tz = 0;
            CWCircleData.GetData(i, ref tx, ref tz);
            if (tx >= nRadius) continue;
            if (tz >= nRadius) continue;

            int x = tx + vInt.x;
            int z = tz + vInt.z;

            if (x <= 8) return false;// 평평하지 않음!
            if (z <= 8) return false;// 평평하지 않음!
            if (x >= (CWMapManager.SelectMap.WORLDSIZE - 8)) return false;// 평평하지 않음!
            if (z >= (CWMapManager.SelectMap.WORLDSIZE - 8)) return false;// 평평하지 않음!
            float fh = GetHeight(x, z);
            if(Mathf.Abs(fh-fh1)>=2)
            {
                return false;// 평평하지 않음!
            }
        }
        return true;

    }

    #endregion


    #region // 메쉬 저장 관련
    CWJSon m_kMeshJson = new CWJSon();



    public CWFile LoadMeshSell(string szfile)
    {
        byte[] bdata = m_kMeshJson.GetBytes(szfile);
        if (bdata == null) return null;
        return new CWFile(new MemoryStream(bdata));
    }
    public void SaveMeshSell(CWFile cf, string szfile)
    {
        m_kMeshJson.Add(szfile, cf.ToArray());
    }
    #endregion


    #region 새로운 로드 방식 !

    // LOD를 로드 한다 
    public void LoadLOD()
    {
        Close();
        m_gMeshObject.SetActive(true);
        MeshFilter mf = m_gMeshObject.GetComponent<MeshFilter>();
        mf.sharedMesh  = CWMeshManager.Instance.GetMesh(m_nMapID); //CWResourceManager.Instance.GetMeshAsset(m_nMapID.ToString());
        //if (mf != null)
        //{
        //    string szname = string.Format("MeshAsset/{0}", m_nMapID);
        //    mf.sharedMesh = CWMeshManager.Instance.GetMesh(szname);
        //}

    }
    // 큐브맵에서 처음으로 로드 
    public void ShowLOD(int nMapID)
    {
        m_nMapID = nMapID;
        LoadLOD();
    }

    

    // 블록을 메쉬로 전환 
    // 실제로 메쉬로 전환하는 부분 
    // 4면부터 시작해서 0 1, 2,3, 순차적 로딩 

    public void MakeBlockMesh()
    {
        if(gameObject.activeInHierarchy==false)
        {
            Debug.Log("여긴 어디??");
            return;
        }
        m_gMeshObject.SetActive(false);
        if (m_kSellGroup == null) return;
        if(!m_bILoadRun)
            StartCoroutine("ILoadRun");
    }
    bool m_bILoadRun = false;
    IEnumerator ILoadRun()
    {
        m_bILoadRun = true;
        int tcnt = m_kSellGroup.Length;

        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;
        
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
                 
                if(m_kSellGroup.Length<=num)
                {
                    continue;
                }
                m_kSellGroup[num].MakeMeshVertex();
            }
        }
        m_bLoadEndTask = true;
        yield return null;

       
        if (!m_bIUpdateSell)
            StartCoroutine("IUpdateSell");

    }


    

    #endregion

    #region  블록 업데이트 

    //맵셀리스트 
    Dictionary<int, CWMapSell> m_kUpdateSell = new Dictionary<int, CWMapSell>();
    public void AddUpdateSell(CWMapSell kSell)
    {
        int num = kSell.m_nSellZ * 1000 + kSell.m_nSellX;
        if (!m_kUpdateSell.ContainsKey(num))
        {
            m_kUpdateSell.Add(num, kSell);
        }
    }
    // 가장 가까운 셀 리턴
    int GetUpdateBestSell()
    {
        int nRet = -1;
        float fMin = 10000f;
        Vector3 vPos= CWHeroManager.Instance.GetPosition();
        foreach(var v in m_kUpdateSell)
        {
           
            if(v.Value.m_bFast)
            {
                return v.Key;
            }
            Vector3 ms = v.Value.GetPostion();
            float fDist = Vector3.Distance(vPos, ms);
            if(fMin>fDist)
            {
                nRet = v.Key;
            }
        }
        return nRet;
    }

    public void UpdateBlock()
    {
        int nKey = GetUpdateBestSell();
        if (nKey >= 0)
        {
            CWMapSell kSell = m_kUpdateSell[nKey];
           
            kSell.MakeMeshObject();
            m_kUpdateSell.Remove(nKey);
        }
    }
    bool m_bIUpdateSell = false;

    IEnumerator IUpdateSell()
    {
        m_bIUpdateSell = true;
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            int nKey = GetUpdateBestSell();
            if(nKey>=0)
            {
                CWMapSell kSell = m_kUpdateSell[nKey];
               
                kSell.MakeMeshObject();
                m_kUpdateSell.Remove(nKey);
            }
            yield return null;
        }
    }
    public  void MakeMeshLOD(int nSize)
    {
        m_kUpdateSell.Clear();

        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;

                m_kSellGroup[num].MakeMeshVertexLOD(nSize);
            }
        }

    }
    public virtual void MakeMesh()
    {
        m_kUpdateSell.Clear();

        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
              
                m_kSellGroup[num].MakeMeshVertex();
            }
        }
    }

    #endregion



    #region 새로운 블록데이타 

    
    public int m_nResblock1=0;
    public int m_nResblock2 = 0;
    public int m_nResblock3 = 0;


    public int m_nMaxResblock2 = 0;
    public int m_nMaxResblock3 = 0;


    public int m_nResBlockCount2; // 희귀자원 배치 개수 
    public int m_nResBlockAddCount2;// 희귀자원 얻은 개수

    public int m_nResBlockCount3; // 희귀자원 배치 개수 
    public int m_nResBlockAddCount3;// 희귀자원 얻은 개수


    public List<Vector3> m_vRes3block = new List<Vector3>();

    public List<Vector3> m_vPosData = new List<Vector3>();


    protected virtual int ConvertBlock(int nBlock,int h)
    {
        
        if (CWGlobal.g_bEditmode)
        {
            return nBlock;
        }
        if (GamePlay.Instance.m_nWType != GamePlay.WTYPE.SINGLE)
        {
            return nBlock;
        }


        if (nBlock==(int)OLDBLOC.Diamond)
        {
            return (int)OLDBLOC.stone;
        }

        if (nBlock==(int)OLDBLOC.ResBlock)
        {
            if (h < 2) return (int)OLDBLOC.stone;
            return m_nResblock1;
        }

        if(nBlock==(int)OLDBLOC.GemBlock)
        {
            if (h < 2) return (int)OLDBLOC.stone;
            
            int GRate = CWArrayManager.Instance.GetGoldRate(Space_Map.Instance.GetStageID());
            int RR = CWLib.Random(0, 1000);
            if (RR >= 0 && RR < 300+ GRate)
            {
                if(RR<2)
                {
                    return (int)OLDBLOC.Diamond;
                }
                return (int)OLDBLOC.GoldBlock;
            }



            return (int)OLDBLOC.stone;
        }
        return nBlock;
    }

    byte[] m_bServerBlock;//서버 블록 데이타
    // 맵에 사용되는 블록 데이타 
    byte[] m_bBlockBuffer;

    byte[] m_bColorBuffer;

    //


    // 압축 데이타를 푼다 
    bool LoadBlockData(CWJSon jSon,bool blocalfile)
    {
        // 램덤 초기화, 
        CWLib.RandomInit(200000);


        if(Space_Map.Instance!=null)
        {
            CWArrayManager.StageData kData = CWArrayManager.Instance.GetStageData(Space_Map.Instance.GetStageID());
            m_nResblock1 = CWArrayManager.Instance.GetBlockFromItem(CWArrayManager.Instance.GetBlockLevel(1, Space_Map.Instance.GetStageID()));
            m_nResblock2 = CWArrayManager.Instance.GetBlockFromItem(CWArrayManager.Instance.GetBlockLevel(2, Space_Map.Instance.GetStageID()));
            m_nResblock3 = CWArrayManager.Instance.GetBlockFromItem(CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID()));
            m_nMaxResblock2 = kData.m_nCount2;
            m_nMaxResblock3 = kData.m_nCount3;
        }
        LoadDelData();

        int Cnt = WORLDSIZE * WORLDSIZE * HEIGHT;
        m_bBlockBuffer = new byte[Cnt];

        byte[] bBuffer  = jSon.GetBytes("Blockdata");
        if (bBuffer == null) return false;

        m_bColorBuffer = jSon.GetBytes("Colordata");
        



        if (Space_Map.Instance != null)
        {
            int nStage = Space_Map.Instance.GetStageID();// 
            
            m_nResBlockAddCount2 = CWHeroManager.Instance.GetRes2(nStage); //PlayerPrefs.GetInt(szAddCount);
            m_nResBlockAddCount3 = CWHeroManager.Instance.GetRes3(nStage); //PlayerPrefs.GetInt(szAddCount);

        }

        

        

        m_nResBlockCount2 = m_nResBlockAddCount2;
        m_nResBlockCount3 = m_nResBlockAddCount3;
        m_vRes3block = new List<Vector3>();
        m_vPosData = new List<Vector3>();

        m_kResPos.Clear();
        m_kHeight = new HDATA[WORLDSIZE* WORLDSIZE];


        for (int z = 0; z < WORLDSIZE; z++)
        {
            for (int x = 0; x < WORLDSIZE; x++)
            {
                bool bflag = false;
                for (int y = HEIGHT-1; y >=0; y--)
                {
                    int num = (x * WORLDSIZE + z) * HEIGHT + y;

                    m_bBlockBuffer[num] = (byte)bBuffer[num];

                    if (m_bBlockBuffer[num] == 0) continue;
                       m_nTotalCount++;

                    if (IsDelBlock(num))
                    {
                        m_nAddCount++;
                        m_bBlockBuffer[num] = 0;
                    }
                    else
                    {

                        
                        if(!blocalfile)
                        {
                            if (y < 2)
                            {
                                if (z > 5 && z < 250)
                                {
                                    if (x > 5 && x < 250)
                                    {
                                        if (CWLib.Random(0, 50) == 1)
                                        {
                                            m_vPosData.Add(new Vector3(x, y, z));
                                        }

                                    }

                                }
                            }
                            m_bBlockBuffer[num] = (byte)ConvertBlock((int)(bBuffer[num]), y);
                        }
                        else
                        {
                            if(m_bBlockBuffer[num]== m_nResblock3)
                            {
                                m_vRes3block.Add(new Vector3(x, y, z));
                            }
                                
                        }

                        
                    }

                   
                    if (!bflag)
                    {
                        bflag = true;
                        int hnum = x * WORLDSIZE + z;
                        m_kHeight[hnum] = new HDATA();
                        m_kHeight[hnum].Block = m_bBlockBuffer[num];
                        m_kHeight[hnum].Height = (byte)y;
                    }
                }

            }

        }

        if (!blocalfile)
            CreateResBlock();


        return true;
    }
    void CreateResBlock()
    {
        if (GamePlay.Instance==null) return;
        if(GamePlay.Instance.m_nWType != GamePlay.WTYPE.SINGLE)
        {
            return;
        }

        int tcnt = 0;
        

        int max3 = m_nMaxResblock3 - m_nResBlockAddCount3;

        int RR1 = CWLib.Random(1, m_nMaxResblock3);
        int RR2 = CWLib.Random(1, m_nMaxResblock3);
        int RR3 = CWLib.Random(1, m_nMaxResblock3);
        int RR4 = CWLib.Random(1, m_nMaxResblock3);

        SRandom.Create(m_vPosData.Count);
        for (int i = 0; i < max3; i++)
        {

            int num = SRandom.GetNextValue();
            for(int j=0;j<4;j++)
            {
                tcnt++;
                if (CWHeroManager.Instance.m_bTuto)
                {
                    if(i==0)
                    {
                        m_vPosData[num] = new Vector3(128, 1, 10);
                    }
                    if (i == 2)
                    {
                        m_vPosData[num] = new Vector3(120, 1, 12);
                    }
                    if (i == 3)
                    {
                        m_vPosData[num] = new Vector3(112, 1, 20);
                    }


                }

                int nFace = CWLib.Random(0, 8);
                int x = (int)m_vPosData[num].x + g_vDir8[nFace].x;
                int y = (int)m_vPosData[num].y + g_vDir8[nFace].y;
                int z = (int)m_vPosData[num].z + g_vDir8[nFace].z;
                if (y < 0) continue;
                int num2 = (x * WORLDSIZE + z) * HEIGHT + y;
                if(num2 >= m_bBlockBuffer.Length) continue;

                if (m_bBlockBuffer[num2] == (byte)m_nResblock3) continue;
                m_bBlockBuffer[num2] = (byte)m_nResblock3;
                if (tcnt==RR1)
                {
                    m_bBlockBuffer[num2] = (byte)OLDBLOC.Diamond;
                }
                if (tcnt == RR2)
                {
                    m_bBlockBuffer[num2] = (byte)OLDBLOC.Diamond;
                }
                //if (tcnt == RR3)
                //{
                //    m_bBlockBuffer[num2] = (byte)OLDBLOC.Diamond;
                //}
                //if (tcnt == RR4)
                //{
                //    m_bBlockBuffer[num2] = (byte)OLDBLOC.Diamond;
                //}

                //OLDBLOC.Diamond

                m_vRes3block.Add(new Vector3(x, y, z));
                if (m_vRes3block.Count >= max3) break;
            }
            if (m_vRes3block.Count >= max3) break;

        }

        int max2 = m_nMaxResblock2 - m_nResBlockAddCount2;
        for (int i = 0; i < max2; i++)
        {
            int num = SRandom.GetNextValue();
            int x = (int)m_vPosData[num].x;
            int y = (int)m_vPosData[num].y;
            int z = (int)m_vPosData[num].z;
            int num2 = (x * WORLDSIZE + z) * HEIGHT + y;
            m_bBlockBuffer[num2] = (byte)m_nResblock2;
        }


    }

    public void MakeHeight()
    {
        m_kHeight = new HDATA[WORLDSIZE * WORLDSIZE];
        for (int z = 0; z < WORLDSIZE; z++)
        {
            for (int x = 0; x < WORLDSIZE; x++)
            {
             
                for (int y = HEIGHT - 1; y >= 0; y--)
                {
                    int num = (x * WORLDSIZE + z) * HEIGHT + y;
                    if(m_bBlockBuffer[num]>0)
                    {
                        int hnum = x * WORLDSIZE + z;
                        m_kHeight[hnum] = new HDATA();
                        m_kHeight[hnum].Block = m_bBlockBuffer[num];
                        m_kHeight[hnum].Height = (byte)y;
                        break;
                    }
                }

            }

        }

    }
    void SaveBlockData(CWJSon jSon)
    {
        // 라이트 정리 

        jSon.Add("Blockdata", m_bBlockBuffer);
        if(m_bColorBuffer!=null)
            jSon.Add("Colordata", m_bColorBuffer);
        


    }
    public void ClearBlock()
    {
        int Cnt = WORLDSIZE * WORLDSIZE * HEIGHT;
        m_bBlockBuffer = new byte[Cnt];

    }
    // 칠할 수 있는 블록인가?
    public bool IsAllowColor(int x,int y,int z)
    {
        int nBlock= GetBlock(x, y, z);
        int nItem = CWArrayManager.Instance.GetItemFromBlock(nBlock);
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if(gData.type=="shipblock")// 비행기 블록만 됨
        {
            return true;
        }
        return false;

    }
    public void SetBlock(int x, int y, int z, int nBlock)
    {
        int num = (x * WORLDSIZE + z) * HEIGHT + y;
        if (num < 0) return;
        if (num >= m_bBlockBuffer.Length) return;
        m_bBlockBuffer[num] = (byte)nBlock;

    }
    public void UpdateColor(int x, int y, int z, int nBlockColor)
    {
        if (x < 0) return;
        if (y < 0) return;
        if (z < 0) return;
        if (x >= WORLDSIZE) return;
        if (y >= HEIGHT) return;
        if (z >= WORLDSIZE) return;
        if(m_bColorBuffer==null)
        {
            int Cnt = WORLDSIZE * WORLDSIZE * HEIGHT;
            m_bColorBuffer = new byte[Cnt];
        }
        int nItem = CWArrayManager.Instance.GetItemFromBlock(nBlockColor);
        int nColor = (int)CWGlobal.GetColorItem((GITEM)nItem);
        int num = (x * WORLDSIZE + z) * HEIGHT + y;

        m_bColorBuffer[num] = (byte)nColor;

        m_bUpdated = true;
        CWMapSell kSell = GetMapSell(x, z);
        if (kSell != null)
        {
            AddUpdateSell(kSell);
        }

    }

    public void UpdateBlock(int x,int y,int z,int nBlock,bool bFast=false)
    {
        int num = (x * WORLDSIZE + z) * HEIGHT + y;

        if (x < 0) return;
        if (y < 0) return;
        if (z < 0) return;
        if (x >= WORLDSIZE) return;
        if (y >= HEIGHT) return;
        if (z >= WORLDSIZE) return;


//        print(string.Format("block: {0} {1} {2}",x,y,z));

        m_bBlockBuffer[num] =(byte) nBlock;

        if(nBlock==(int)OLDBLOC.grass)
        {
            int nn=GetBlock(x, y-1, z);
            if(nn==(int)OLDBLOC.grass)
            {
                SetBlock(x, y - 1, z, (int)OLDBLOC.dirt);
            }
        }

        m_bUpdated = true;
        CWMapSell kSell = GetMapSell(x, z);
        if(kSell!=null)
        {
            kSell.m_bFast = bFast;
            AddUpdateSell(kSell);
        }
        

        // 4개면을 검사 
        for(int i=0;i<4;i++)
        {
            int tx = x + g_vNormal[i, 0];
            int tz = z + g_vNormal[i, 2];
            CWMapSell kSell2 = GetMapSell(tx, tz);
            if (kSell2 == null) continue;
            if(kSell!=kSell2)
            {
                kSell2.m_bFast = bFast;
                AddUpdateSell(kSell2);
            }
        }


    }

    public Color GetColor(int x,int y,int z)
    {
        if (x < 0) return Color.white; 
        if (y < 0) return Color.white;
        if (z < 0) return Color.white;
        if (x >= WORLDSIZE) return Color.white;
        if (y >= HEIGHT) return Color.white;
        if (z >= WORLDSIZE) return Color.white;
        if (m_bColorBuffer == null) return Color.white;
        int num = (x * WORLDSIZE + z) * HEIGHT + y;
        return CWGlobal.GetColor((COLORNUMBER)m_bColorBuffer[num]);

    }

   

#if UNITY_EDITOR

    public int GetBlock(int x, int y, int z)
    {
        if (x < 0) return 0;
        if (y < 0) return 0;
        if (z < 0) return 0;
        if (x >=WORLDSIZE) return 0;
        if (y >= HEIGHT) return 0;
        if (z >= WORLDSIZE) return 0;
        if(m_bBlockBuffer==null)
        {
            return 0;
        }
        int num = (x * WORLDSIZE + z) * HEIGHT + y;


        return m_bBlockBuffer[num];
    }


#else
    public int GetBlock(int x, int y, int z)
    {
        if (x < 0) return 0;
        if (y < 0) return 0;
        if (z < 0) return 0;
        if (x >=WORLDSIZE) return 0;
        if (y >= HEIGHT) return 0;
        if (z >= WORLDSIZE) return 0;

        int num = (x * WORLDSIZE + z) * HEIGHT + y;

        return m_bBlockBuffer[num];
    }
   


#endif

   
    #endregion


    // 클라이언트에 저장
    // 게임 끝나고 저장
    // 시작할 때 우선 호출 
    #region  지워진 블록 관리 

    byte[] m_bDelBuffer=null;

    void CreateDelBuffer()
    {
        if (CWGlobal.g_bEditmode) return;

         m_bDelBuffer = new byte[WORLDSIZE* WORLDSIZE* HEIGHT];

    }
    void AddDelData(int x,int y,int z)
    {
        if (m_bDelBuffer == null) return;
        int num = (x * WORLDSIZE + z) * HEIGHT + y;
        if (x < 0) return;
        if (y < 0) return;
        if (z < 0) return;
        if (x >= WORLDSIZE) return;
        if (y >= HEIGHT) return;
        if (z >= WORLDSIZE) return;
        m_bDelBuffer[num] = 1;

        if(m_bColorBuffer!=null)
                m_bColorBuffer[num] = 0;

        m_nAddCount++;

    }
    bool IsDelBlock(int x,int y,int z)
    {
        if (m_bDelBuffer == null) return false;
        int num = (x * WORLDSIZE + z) * HEIGHT + y;
        if (x < 0) return false;
        if (y < 0) return false;
        if (z < 0) return false;
        if (x >= WORLDSIZE ) return false;
        if (y >= HEIGHT)  return false;
        if (z >= WORLDSIZE ) return false;
        if (m_bDelBuffer[num] == 1) return true;

        return false;



    }
    bool IsDelBlock(int num)
    {
        if (m_bDelBuffer == null) return false;
        if (m_bDelBuffer[num] == 1) return true;
        return false;
    }

    public void SaveLocalData()
    {
        string szPath="";
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MYROOM)
        {
            szPath = CWGlobal.GetMyLocalName();
            SaveData(szPath);
        }
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.SINGLE)
        {
            SaveBlockCount();
            szPath = CWGlobal.GetPlanetLocalName(Space_Map.Instance.GetStageID());
            SaveData(szPath);
        }

    }
    public void SaveDelData()
    {

        if (m_bDelBuffer == null) return;
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.MYROOM)
        {
            return;
        }

        if (!CWGlobal.g_bSingleGame) return;
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.SINGLE) return;
        if (GamePlay.Instance.m_nWType == GamePlay.WTYPE.PVP) return;
        

        string szPath;
        int nStage = Space_Map.Instance.GetStageID();// 
        szPath = string.Format("{0}_{1}", CWHero.Instance.m_nID, nStage);
        SaveData(szPath);
        //CWFile kFile = new CWFile();
        //kFile.PutBuffer(m_bDelBuffer);
        //kFile.Save(szPath);

        

    }
    void LoadDelData()
    {
        if (CWGlobal.g_bEditmode) return;
        if (!CWGlobal.g_bSingleGame)
        {
          
            return;
        }

        string szPath;
        szPath = string.Format("{0}/{1}_{2}", Application.persistentDataPath, CWHero.Instance.m_nID, Space_Map.Instance.GetStageID());

        CWFile kFile = new CWFile();
        if(kFile.Load(szPath))
        {
            m_bDelBuffer = kFile.GetBuffer();
        }
        else
        {
            CreateDelBuffer();
        }

    }


    #endregion


    public void SetMeshCollider(bool bflag)
    {

        MeshCollider[] aa = gameObject.GetComponentsInChildren<MeshCollider>();
        int nCount = aa.Length;

        Game_App.Instance.IRun((number)=> {
            
            aa[number].convex = bflag;
            
            if(number+1>=nCount)
            {
                return true;
            }
            return false;

        });
    }

    

    //private void Update()
    //{

    //    Teststr = string.Format("{0} {1} {2} {3} ", transform.forward,transform.up,transform.right, transform.rotation.ToString()); 
    //}


}
