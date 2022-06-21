
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CWEnum;
using CWUnityLib;
using CWStruct;
using System.IO;



// 패턴 불러오기 
// 맵 만들기 
// 맵 저장
// 건물 붙이기
// 몹 붙이기 

public class MakeMapTool : MonoBehaviour
{

    #region 패턴 정의 
    public int RANDOMTEST = 20000;
    // 개념 설정
    // 진흙의 두께 
    // 진흙이 생기면, 그 두께만 쌓이고 위는 잔디로 채워지고, 아래는 돌로 채워진다 
    const int DIRT_WIDTH = 3; // 진흙의 두께 위는 잔디, 아래는 돌 
    const int WATER_HEIGHT = 3; // 물 높이 

    Color Grass = Color.green;
    Color Sand = Color.yellow;
    Color Stone = new Color(0.5f, 0.5f, 0.5f);
    Color Dirt =new Color(0.5f,0.5f,0);
    Color TreeGrass = new Color(0,0.5f,0);// 나무가 있는 잔디 

    Color Bora= new Color(0.5f, 0, 1f);
    Color Red=Color.red; // NPC 기지 
    Color Blue=Color.blue; // 구조물, 건축물, 조각 




    bool IsCompareColor(Color kColor,Color kColor2)
    {
        Color[] kColors = new Color[8];
        kColors[0] = Grass;
        kColors[1] = Sand;
        kColors[2] = Stone;
        kColors[3] = Dirt;
        kColors[4] = TreeGrass;
        kColors[5] = Bora;
        kColors[6] = Red;
        kColors[7] = Blue;

        float f1 = CWMath.GetDistColor(kColor, kColor2);
        if (f1 == 0) return true;

        for (int i=0;i<kColors.Length;i++)
        {
            float f2 = CWMath.GetDistColor(kColors[i], kColor);
            if(f1> f2)
            {
                return false;
            }
        }

        return true;
    }

    int MAXMAPCOUNT = 150;// 현재 까지 만들어진 맵 개수 
    enum BLOCKTYPE {GRASS,DIRT,SAND,STONE };

    int m_nGrass;
    int m_nDirt;
    int m_nSand;
    int m_nStone;

    public class PLANETDATA
    {
        public int m_nID;
        public string m_szMask;
        public float m_fAmp;
        public int m_TreeSet;
        public int m_BuildSet;// 건물 셋
        public float m_fTreeRate;// 나무 분포율
        public bool m_bWater;
        public int m_nGrassHeight;// 잔디 최대 높이 
        public int m_nDirtHeight;// 진흙최대 높이 
        public int m_nSandHeight;// 모래 최대 높이 
        public float m_fPyungTanRate;// 평탄화률


        public string m_szPattern;// 분류
        public int m_ResHeight;// 자원이 생기는 시작위치 
        public int m_nHeight;// 최대 높이
    }
    //List<PLANETDATA> m_kPlanetList = new List<PLANETDATA>();
    Dictionary<int, PLANETDATA> m_kPlanetList = new Dictionary<int, PLANETDATA>();
    void LoadPattern()
    {
        

        int tcnt = CWTableManager.Instance.GetTableCount("행성 - 맵생성");
        
        for (int i=0;i<tcnt;i++)
        {
            PLANETDATA kData = new PLANETDATA();
            int nKey = i + 1;
            kData.m_nID = nKey;
            kData.m_szMask = CWTableManager.Instance.GetTable("행성 - 맵생성", "mask", nKey);
            kData.m_fAmp = CWTableManager.Instance.GetTableFloat("행성 - 맵생성", "높이증폭", nKey);
            kData.m_TreeSet = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "나무세트", nKey);
            kData.m_fTreeRate = CWTableManager.Instance.GetTableFloat("행성 - 맵생성", "나무분포률", nKey);
            kData.m_fPyungTanRate = CWTableManager.Instance.GetTableFloat("행성 - 맵생성", "평탄률", nKey);
            kData.m_BuildSet = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "건물세트", nKey);

            string szval = CWTableManager.Instance.GetTable("행성 - 맵생성", "물", nKey);
            if(szval=="없음")
            {
                kData.m_bWater = false;
            }
            else
            {
                kData.m_bWater = true;
            }
            kData.m_nGrassHeight = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "잔디높이", nKey);
            kData.m_nDirtHeight = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "진흙높이", nKey);
            kData.m_nSandHeight = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "모래높이", nKey);
            

            kData.m_szPattern = CWTableManager.Instance.GetTable("행성 - 맵생성", "분류", nKey);
            kData.m_ResHeight = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "자원높이값", nKey);
            kData.m_nHeight = CWTableManager.Instance.GetTableInt("행성 - 맵생성", "높이값", nKey);

            




            m_kPlanetList.Add(nKey, kData);
        }

    }
    static public Texture2D LoadImage(string szname, int dx, int dy)
    {

        string szPath;
        szPath = string.Format("{0}/NewGame/Mask/{1}.bmp", Application.dataPath, szname);

        if (File.Exists(szPath))
        {

            try
            {
                byte[] fileData = File.ReadAllBytes(szPath);
                Texture2D Tx = new Texture2D(dx, dy);
                Tx.LoadImage(fileData);
                return Tx;
            }
            catch (System.Exception)
            {
                DebugX.Log("exception " + szPath);
            }


        }
        return null;
    }

    int GetPantternBlock(BLOCKTYPE blockType, string szPattern)
    {
        string szType = "";
        if(blockType== BLOCKTYPE.GRASS)
        {
            szType = "grass";
        }
        if(blockType== BLOCKTYPE.DIRT)
        {
            szType = "dirt";
        }
        if(blockType== BLOCKTYPE.SAND)
        {
            szType = "sand";
        }
        if(blockType== BLOCKTYPE.STONE)
        {
            szType = "stone";
        }


        return CWArrayManager.Instance.GetPattenBlock(szType, szPattern);

    }
    int GetPantternBlock(PLANETDATA kData,Color kColor,int h)
    {
        
        if (IsCompareColor(kColor, Grass) || IsCompareColor(kColor ,TreeGrass))// 잔디 패턴
        {
            // 잔디 -> 돌 
            if (kData.m_nGrassHeight > h)
            {
                return m_nGrass;//GetPantternBlock(BLOCKTYPE.GRASS,kData.m_szPattern);
            }
            else return m_nStone; // GetPantternBlock(BLOCKTYPE.STONE, kData.m_szPattern);
        }

        if (IsCompareColor(kColor ,Dirt))// 
        {
            // 잔디 -> 돌 
            if (kData.m_nDirtHeight > h)
            {
                return m_nDirt;// GetPantternBlock(BLOCKTYPE.DIRT, kData.m_szPattern);
            }
            else
            {
                if (kData.m_nGrassHeight < h)
                {
                    return m_nGrass;//GetPantternBlock(BLOCKTYPE.GRASS, kData.m_szPattern);
                }
                else return m_nStone;// GetPantternBlock(BLOCKTYPE.STONE, kData.m_szPattern);
            }
        }
        if (IsCompareColor(kColor, Sand))//모래
        {
            // 잔디 -> 돌 
            if (kData.m_nSandHeight > h)
            {
                return m_nSand;// GetPantternBlock(BLOCKTYPE.SAND, kData.m_szPattern);
            }
            else return m_nStone;// GetPantternBlock(BLOCKTYPE.STONE, kData.m_szPattern);
        }


        return m_nStone;
    }
        
    int GetUnderGroundBlock(int h,bool bMine=false)
    {
        // 원석  , 골드, 강화석, 다이아몬드
        // 나머지는 모두 돌
        int RR = RANDOMTEST;
        if (bMine)
        {
            RR = RANDOMTEST / 10;
        }


        int rr = CWLib.Random(1, RR);
        if(rr==2)// 다이아몬드 
        {
            return (int)OLDBLOC.Diamond;
        }
        if (rr > 10 && rr <= 500)// 
        {
            return (int)OLDBLOC.ResBlock;
        }
        if (rr > 150 && rr <= 200)// 
        {
            return (int)OLDBLOC.GemBlock;
        }


        return m_nStone;
    }
    bool IsGrass(int nBlock)
    {
        if(CWArrayManager.Instance.m_kBlock[nBlock].szType=="grass")
        {
            return true;
        }
        return false;
    }
    bool IsDirt(int nBlock)
    {
        if (CWArrayManager.Instance.m_kBlock[nBlock].szType == "dirt")
        {
            return true;
        }
        return false;

    }
    int GetBlockType(int nBlock, string sztype)
    {
        for(int i=0;i<256;i++)
        {
            if(CWArrayManager.Instance.m_kBlock[nBlock].szPatten== CWArrayManager.Instance.m_kBlock[i].szPatten)
            {
                if(CWArrayManager.Instance.m_kBlock[i].szType== sztype)
                {
                    return i;
                }
            }
        }
        return 0;
    }

    // 이미지 높이값을 터레인으로
    // 터레인을 높이값으로
    Texture2D GetMask(string szName)
    {
        foreach(var v in m_kMasks)
        {
            if(v.name==szName)
            {
                return v;
            }
        }
        return null;
    }


    bool IsNearColor(int sx,int sz,Color kColor)
    {
        for(int z=-4;z<=4;z++)
        {
            for (int x = -4; x <=4; x++)
            {
                int tx = sx + x;
                int tz = sz + z;
                if (tx < 0) continue;
                if (tz < 0) continue;
                if (tx >= m_kWorldImage.width) continue;
                if (tz >= m_kWorldImage.height) continue;
                Color cc = m_kWorldImage.GetPixel(tx,tz);
                if(kColor.Equals(cc))
                {
                    return true;
                }

            }

        }
        


        return false;
    }
    public PLANETDATA GetPatternData(int nID)
    {
        if (!m_kPlanetList.ContainsKey(nID)) return new PLANETDATA();
        return m_kPlanetList[nID];
    }

    void MakeMap(bool byTerrainHeight,int nID)
    {
        if (!m_kPlanetList.ContainsKey(nID)) return;
        PLANETDATA kData = m_kPlanetList[nID];
        __MakeMap(byTerrainHeight, kData);
    }
    void __MakeMap(bool byTerrainHeight, PLANETDATA kData)
    {
        
        m_gBlockMap.ClearBlock();
        //
        //m_gBlockMap.WORLDSIZE = m_kWorldImage.width;
        CWGlobal.g_LODWORK = false;

        m_nGrass= GetPantternBlock(BLOCKTYPE.GRASS, kData.m_szPattern);
        m_nDirt = GetPantternBlock(BLOCKTYPE.DIRT, kData.m_szPattern);
        m_nSand = GetPantternBlock(BLOCKTYPE.SAND, kData.m_szPattern);
        m_nStone = GetPantternBlock(BLOCKTYPE.STONE, kData.m_szPattern);

     


        for (int z = 0; z < m_gBlockMap.WORLDSIZE; z++)
        {
            for (int x = 0; x < m_gBlockMap.WORLDSIZE; x++)
            {
                float h = 0;
                float fx, fz;

                fx = (float)x * ((float)m_kWorldImage.width/ (float)m_gBlockMap.WORLDSIZE);
                fz = (float)z * ( (float)m_kWorldImage.width/ (float)m_gBlockMap.WORLDSIZE);

                Color kcolor = m_kWorldImage.GetPixel((int)fx, (int)fz);
                if(byTerrainHeight)
                {
                    h = m_kTerrain.terrainData.GetHeight(x, z);
                }
                else
                {
                    
                    float fhRate = kcolor.a *(1f-kData.m_fPyungTanRate);
                    if (kData.m_fAmp > 0)
                    {
                        float fh = Mathf.PerlinNoise(x * kData.m_fAmp, z * kData.m_fAmp);
                        h = (fh * fhRate ) * kData.m_nHeight;
                    }
                    else
                    {
                        h = (fhRate ) * kData.m_nHeight;
                    }
                }

                //float h = m_kTerrain.terrainData.GetHeight(x, z);

                // 진흙 계열이라면, 진흙두께 만큼만 
                int nBlock = GetPantternBlock(kData,kcolor, (int)h);
                int y = (int)h;
                m_gBlockMap.UpdateBlock(x, y, z, nBlock);

                if (IsCompareColor(kcolor, TreeGrass))
                {
                    if(kData.m_fTreeRate>0)
                    {
                        int rr = UnityEngine.Random.Range(0, 100 + (int)(200 * (1f - kData.m_fTreeRate)));
                        if (rr == 0)
                        {
                            CopyTreeBuild(x, y, z, kData.m_TreeSet);
                        }

                    }

                }



                if (IsGrass(nBlock))
                {
                    // 다음은 진흙
                    y--;
                    nBlock = GetBlockType(nBlock,"dirt");
                    m_gBlockMap.UpdateBlock(x, y, z, nBlock);
                }

                if(IsDirt(nBlock))
                {
                    y--;
                    m_gBlockMap.UpdateBlock(x, y , z, nBlock);
                    y--;
                    m_gBlockMap.UpdateBlock(x, y , z, nBlock);
                }
                bool bMine = false;
                if (IsCompareColor(kcolor, Bora))
                {
                    bMine = true;
                }
                for (int i=y-2;i>=0;i-- )
                {


                    nBlock = GetUnderGroundBlock(i, bMine);
                    m_gBlockMap.UpdateBlock(x, i, z, nBlock);
                }


            }
        }

        


    }

   


    #endregion




    //public float Delta = 0.5f;
    
    const int MAXPATTERN = 5; // 5 단계로 

    #region 셀렉트박스 크기 

    void GetSelectBox(ref int sx,ref int sy,ref int sz,ref int ex,ref int ey,ref int ez)
    {
        if (m_gSelectBox.activeSelf == false)
        {
            sx = 0;
            sy = 0;
            sz = 0;
            ex = m_nWidth;
            ey = 64;
            ez = m_nWidth;
            return;
        }

        Vector3Int vPos = CWMapManager.ConvertPos(m_gSelectBox.transform.localPosition);
        Vector3 vScale = m_gSelectBox.transform.localScale;

        sx = (int)(vPos.x - vScale.x / 2);
        sy = (int)(vPos.y - vScale.y / 2);
        sz = (int)(vPos.z - vScale.z / 2);

        ex = (int)(vPos.x + vScale.x / 2);
        ey = (int)(vPos.y + vScale.y / 2);
        ez = (int)(vPos.z + vScale.z / 2);

        if (sx < 0) sx = 0;
        if (sy < 0) sy = 0;
        if (sz < 0) sz = 0;



    }

    #endregion

    #region 패턴맵리스트
    class MAPPATTENDATA
    {
        public int nID;

        public int LvStone1;
        public int LvStone2;
        public int LvStone3;
        public int LvStone4;
        public int LvStone5;
        public int LvStone6;
        public int Gold;
        public int Ruby;
        public int Emerald;
        public int Coal;
        public int Uranium;



    }
    Dictionary<int, MAPPATTENDATA> m_kPattenList = new Dictionary<int, MAPPATTENDATA>();
    void MakeMaplist()
    {
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("주사위맵 - 자원분포");
        if (cs != null)
        {
            int nValue=0;
            int nValue2=0;

            foreach (var v in cs.m_mkData)
            {
                nValue = 0;
                MAPPATTENDATA mm = new MAPPATTENDATA();
                mm.nID = v.Key;

                nValue2 = cs.GetInt(v.Key, "초급원석");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.LvStone1 = nValue;
                }
                else mm.LvStone1 = nValue2;

                nValue2 = cs.GetInt(v.Key, "저급원석");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.LvStone2 = nValue;
                }
                else mm.LvStone2 = nValue2;

                nValue2 = cs.GetInt(v.Key, "중급원석");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.LvStone3 = nValue;
                }
                else mm.LvStone3 = nValue2;


                nValue2 = cs.GetInt(v.Key, "고급원석");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.LvStone4 = nValue;
                }
                else mm.LvStone4 = nValue2;


                nValue2 = cs.GetInt(v.Key, "최고급원석");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.LvStone5 = nValue;
                }
                else mm.LvStone5 = nValue2;


                nValue2 = cs.GetInt(v.Key, "극강원석");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.LvStone6 = nValue;
                }
                else mm.LvStone6 = nValue2;


                nValue2 = cs.GetInt(v.Key, "골드");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.Gold = nValue;
                }
                else mm.Gold = nValue2;

                nValue2 = cs.GetInt(v.Key, "루비");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.Ruby = nValue;
                }
                else mm.Ruby = nValue2;

                nValue2 = cs.GetInt(v.Key, "에메랄드");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.Emerald = nValue;
                }
                else mm.Emerald = nValue2;

                nValue2 = cs.GetInt(v.Key, "석탄");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.Coal = nValue;
                }
                else mm.Coal = nValue2;


                nValue2 = cs.GetInt(v.Key, "우라늄");
                if (nValue2 > 0)
                {
                    nValue += nValue2;
                    mm.Uranium = nValue;
                }
                else mm.Uranium = nValue2;



                m_kPattenList.Add(v.Key, mm);
            }




        }

    }

    #endregion
    #region 블록패턴 

    class BLOCKPATTERNDATA
    {
        public string [] m_szHeight=new string[MAXPATTERN];
        public Color[] m_kColor = new Color[MAXPATTERN];

    }
    List<BLOCKPATTERNDATA> m_kBlockPatten = new List<BLOCKPATTERNDATA>();

    class PTBLOCKDATA
    {
        public Vector3 vColor;
        public int [] nBlock=new int[MAXPATTERN];
    }
    List<PTBLOCKDATA> m_kPTBlockData = new List<PTBLOCKDATA>();

    void MakeBlockPattern()
    {




        
        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("BAC_블록 - 패턴");
        foreach (var v in cs.m_mkData)
        {

            BLOCKPATTERNDATA kData = new BLOCKPATTERNDATA();
            for(int i=0;i< MAXPATTERN; i++)
            {
                string szname = cs.GetString(v.Key, "높이"+(i+1).ToString());
                if(CWLib.IsString(szname))
                {
                   kData.m_szHeight[i] = szname;
                }
                string szColumn = "컬러" + (i + 1).ToString();
                szname = cs.GetString(v.Key, szColumn);


                bool flag = false;
                if (CWLib.IsString(szname))
                {
                    string[] szarray = szname.Split(',');
                    if(szarray.Length==3)
                    {
                        Color kColor = new Color();
                        kColor.r = CWLib.ConvertFloat(szarray[0]) / 255f;
                        kColor.g = CWLib.ConvertFloat(szarray[1]) / 255f;
                        kColor.b = CWLib.ConvertFloat(szarray[2]) / 255f;
                        kData.m_kColor[i] = kColor;
                        flag = true;
                    }
                }
                
                if(!flag)
                {
                    kData.m_kColor[i] = Color.white;

                }





            }
            m_kBlockPatten.Add(kData);
        }

        Vector3 XY0 = new Vector3(); //왼쪽 시작
        Vector3 X0 = new Vector3(); // 첫줄 맨
        Vector3 Y0 = new Vector3();


        for (int i=0;i< m_kBlockPatten.Count; i++)
        {
            
            for (int k=0;k< MAXPATTERN; k++)
            {
                if(i==0&&k==0)
                {
                    XY0 = new Vector3(m_kBlockPatten[i].m_kColor[k].r, m_kBlockPatten[i].m_kColor[k].g, m_kBlockPatten[i].m_kColor[k].b);
                }
                if (i == m_kBlockPatten.Count-1 && k == 0)
                {
                    X0 = new Vector3(m_kBlockPatten[i].m_kColor[k].r, m_kBlockPatten[i].m_kColor[k].g, m_kBlockPatten[i].m_kColor[k].b);
                }
                if (i == 0 && k == MAXPATTERN-1)
                {
                    Y0 = new Vector3(m_kBlockPatten[i].m_kColor[k].r, m_kBlockPatten[i].m_kColor[k].g, m_kBlockPatten[i].m_kColor[k].b);
                }
            }
        }

        float R1 = Vector3.Distance(XY0,X0);
        float R2 = Vector3.Distance(XY0,Y0);

        Vector3 vDir1 = X0 - XY0;
        vDir1.Normalize();

        Vector3 vDir2 = Y0 - XY0;
        vDir2.Normalize();

        for (int i = 0; i < m_kBlockPatten.Count; i++)
        {
            
            for (int k = 0; k < MAXPATTERN; k++)
            {
                if(CWLib.IsString(m_kBlockPatten[i].m_szHeight[k]))
                {

                    float frate1= ((float)i / (m_kBlockPatten.Count-1));
                    float fd1 = R1 * frate1;
                    Vector3 vRet= XY0 + fd1 * vDir1 ;

                    PTBLOCKDATA ptData = new PTBLOCKDATA();

                    if(m_kBlockPatten[i].m_kColor[k]==Color.white)
                    {
                        ptData.vColor = new Vector3(vRet.x, vRet.y, vRet.z);
                    }
                    else
                    {
                        ptData.vColor = new Vector3(m_kBlockPatten[i].m_kColor[k].r, m_kBlockPatten[i].m_kColor[k].g, m_kBlockPatten[i].m_kColor[k].b);
                    }
                    ptData.nBlock[k] = CWArrayManager.Instance.GetBlock(m_kBlockPatten[i].m_szHeight[k]);

                    m_kPTBlockData.Add(ptData);

                }
            }
        }



    }


    public int m_nColorPattern=0;




    #endregion

    #region 시작


    protected  bool OnceRun()
    {
        

        //PlayerPrefs.SetFloat("ColorTable", kTemp.Count);
        if(PlayerPrefs.HasKey("ColorTable"))
        {
            
            int kCount = PlayerPrefs.GetInt("ColorTable");

            m_kColorTable = new Color[kCount];
            for (int i=0;i<kCount;i++)
            {
                string sz1 = string.Format("{0}_1",i);
                float r = PlayerPrefs.GetFloat(sz1);

                string sz2 = string.Format("{0}_2",i);
                float g = PlayerPrefs.GetFloat(sz2);

                string sz3 = string.Format("{0}_3",i);
                float b = PlayerPrefs.GetFloat(sz3);

                m_kColorTable[i] = new Color(r, g, b);

            }
        }





        m_kDelType = DELTYPE.NONE;

        MakeMaplist();
        MakeBlockPattern();

        
        

        LoadPattern();

        CreateMap();

        return true;
    }
    public void CreateObject()
    {
        foreach (var v in m_Trees)
        {
            v.Create(0);
        }
        foreach (var v in m_Trees2)
        {
            v.Create(0);
        }

        CWAirObject[] bb = m_BuildingDir.GetComponentsInChildren<CWAirObject>();
        foreach (var v in bb)
        {
            v.Create(0);
        }

        foreach (var vv in m_BuildingArrayDir)
        {
            CWAirObject[] bb1 = vv.GetComponentsInChildren<CWAirObject>();
            foreach (var v in bb1)
            {
                v.Create(0);
            }

        }
        m_kBuildObject.Create(0);
        m_kAirObject.Create(0);
    }
    private void Start()
    {
        CWMapManager.g_bLight = false;
        StartCoroutine("StartRun");
    }
    IEnumerator StartRun()
    {
        while(!CWGlobal.G_bGameStart)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        CWMapManager.Instance.m_gVisable.SetActive(true);
        OnceRun();
    }

    #endregion

    #region 변수
    struct BUILDING
    {
        public int num;
        public int x;
        public int y;
        public int z;

    };
    public string m_szBlockGroup = "Build_1";

    public int m_nLOD = 1;
    public int m_nTurretLevel;
    public AIOBJECTTYPE m_kAItype = AIOBJECTTYPE.DRONE;

    public GameObject m_gMenu;

    public GameObject m_gWorldEditDir;

    public Camera m_gCapture;

    public Texture2D m_kVoxelImage;
    public GameObject m_gVoxelTarget;

    public Terrain m_kTerrain;
    public BlockConverter m_BlockConverter;

    

    public InputField m_kInput;




    public enum MAPPATTEN { P1, P2, P3 };
    public MAPPATTEN m_kPtype;


    public Texture2D m_kWorldImage;
    public Texture2D[]  m_kMasks;

    GameObject m_gBuildList;// 맵에 적용하는 것


    public bool m_bTurret;


    public float m_fAmp = 0.1f;
    public int m_nHeight = 64;
    public int m_nWidth = 256;
    public float m_fPyungTanRate=0.5f;

    public float m_fHeightRate = 1;

    public int m_ID
    {
        get
        {
            if (MapCtrl.Instance == null) return 0;
            return MapCtrl.Instance.m_nFile;
            
        }
    }


    public MapEditManager m_gBlockMap;

    public int m_PattenID=1;

    OLDBLOC m_kblock
    {
        get
        {
            return MapCtrl.Instance.GetBlock();
        }
    }


    public OLDBLOC m_ksideblock = OLDBLOC.stone;// 엣지 블록 

    public Text m_kInfo;

    public Text m_kMapInfo;

    public GameObject m_gSelect;
    

    public CWBuildObject[] m_Trees;
    public CWBuildObject[] m_Trees2;

    public string [] m_Trees3;


    public GameObject m_BuildingDir;
    public GameObject [] m_BuildingArrayDir;

    

    public string m_szTurret;
    public CWBuildObject m_kBuildObject;


    public CWAirObject m_kAirObject;
    
    
    public int m_TreeRate = 100;

    public bool m_bNotexture;
    public bool m_Buildflag = false;
    private bool bBlockflag = false;

    bool m_bChange
    {
        get
        {
            return MapCtrl.Instance.CHANGE;
        }
    }


    public bool m_bMineFlag = false;

    public bool m_bBuildStage = false;

    // 고정
    public bool m_bFixX;
    public bool m_bFixY;
    public bool m_bFixZ;


    public enum DELTYPE {NONE, PYUNGTAN,PYUNGTANDIR,DELDOWN,FULLDOWN };

    public DELTYPE m_kDelType;

    public int m_nRadius
    {
        get
        {
            return MapCtrl.Instance.m_nRange;
        }
    }

    public GameObject m_gSelectBox;

    public int m_nDelDownHeight=-1;// 이거 이하는 지우지 않는다
    public int m_nDelUpHeight=-1;// 이거 이상은 지우지 않는다


    public CWSellGroup m_kSelectSell;

    public GameObject m_gBuildPrefab;

    #endregion

    #region 버튼


    // 저장을 해야 하는 블록셀을 찾는다. 
/*
    public void CheckSaveSell()
    {


        List<int> kSellData=new List<int>();
        List<int> kBlock = new List<int>
        {
            //(int)OLDBLOC.Black,
//            (int)OLDBLOC.Blue,
//            (int)OLDBLOC.DarkBlue,
            //(int)OLDBLOC.DarkGray,
            (int)OLDBLOC.Diamond,
            (int)OLDBLOC.Emerald,
            (int)OLDBLOC.gold,
            //(int)OLDBLOC.Gray,
            //(int)OLDBLOC.LightGray,
            //(int)OLDBLOC.LightGreen,
            //(int)OLDBLOC.Orange,
            //(int)OLDBLOC.Purple,
            //(int)OLDBLOC.Red,
            (int)OLDBLOC.Ruby,
            //(int)OLDBLOC.stone3,
            //(int)OLDBLOC.stone4,
            //(int)OLDBLOC.stone5,
            //(int)OLDBLOC.stone6,
            //(int)OLDBLOC.stone7,
            //(int)OLDBLOC.ShipIron1,
            //(int)OLDBLOC.ShipIron2,
            //(int)OLDBLOC.ShipIron3,
            //(int)OLDBLOC.ShipIron4,
            //(int)OLDBLOC.ShipIron5,
            //(int)OLDBLOC.ShipIron6,
            //(int)OLDBLOC.ShipIron7,
            //(int)OLDBLOC.ShipIron8,
            //(int)OLDBLOC.SkyBlue,
            //(int)OLDBLOC.WHITE,
            //(int)OLDBLOC.Yellow
        };
        m_gBlockMap.m_kSaveSell.Clear();
        int tdy = 1;
        int tdx = m_gBlockMap.WORLDSIZE / CWGlobal.SELLCOUNT;
        for (int z = 0; z < m_gBlockMap.WORLDSIZE / CWGlobal.SELLCOUNT; z++)
        {
            for (int y = 0; y < 1; y++)
            {
                for (int x = 0; x < m_gBlockMap.WORLDSIZE / CWGlobal.SELLCOUNT; x++)
                {
                    CWMapSell pSellBlock = m_gBlockMap.GetMapSell(x * CWGlobal.SELLCOUNT,y*CWGlobal.WD_WORLD_HEIGHT, z * CWGlobal.SELLCOUNT);
                    if (pSellBlock != null)
                    {
                        if (pSellBlock.FindSaveSellBlock(kBlock))
                        {
                            int num = (z * tdx + x)* tdy + y;
                            m_gBlockMap.m_kSaveSell.Add(num);
                            CWUnityLib.DebugX.Log(string.Format("Save Sell = {0} -{1} -{2}", x,y, z));
                        }
                    }
                }

            }
        }

        CWUnityLib.DebugX.Log(string.Format("Save Count = {0}", m_gBlockMap.m_kSaveSell.Count));

        SaveData();


    }
    */
    public bool ExportBlock()
    {
        if (m_gSelectBox.activeSelf == false) return false;
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;

        int sx =(int) (vPos.x - vScale.x / 2);
        int sy = (int)(vPos.y - vScale.y / 2);
        int sz = (int)(vPos.z - vScale.z / 2);

        int ex = (int)(vPos.x + vScale.x / 2);
        int ey = (int)(vPos.y + vScale.y / 2);
        int ez = (int)(vPos.z + vScale.z / 2);

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        CWBlockGroup kBlock = new CWBlockGroup(m_gBlockMap);
        kBlock.TakeMap(sx,sy,sz, ex, ey, ez);
        kBlock.Save(m_szBlockGroup);

        return true;
    }

    public void ImportBlock(int x,int y,int z)
    {
        CWBlockGroup kBlock = new CWBlockGroup(m_gBlockMap);

        kBlock.Load(m_szBlockGroup);
        kBlock.ApplyMap(x, y, z, SetResMakeBlock);
        

        m_gBlockMap.MakeMesh();

    }
    // 부분 메이크
   
    bool CheckSell(int x,int y,int z)
    {
        if (m_gSelectBox.activeSelf == false) return false;
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;

        float fminx = vPos.x - vScale.x /2;
        float fminy = vPos.y - vScale.y / 2;
        float fminz = vPos.z - vScale.z / 2;

        float fmaxx = vPos.x + vScale.x / 2;
        float fmaxy = vPos.y + vScale.y / 2;
        float fmaxz = vPos.z + vScale.z / 2;

        if (x <= fminx) return false;
        if (y <= fminy) return false;
        if (z <= fminz) return false;

        if (x >= fmaxx) return false;
        if (y >= fmaxy) return false;
        if (z >= fmaxz) return false;

        return true;
    }
   
 

    public void CreateMap()
    {
        
        m_gBlockMap.CreateMap(m_ID, m_nWidth);
        m_gBlockMap.MakeMesh();
        m_gBuildList = CWLib.FindChild(m_gWorldEditDir, "Building");
        //FromTerrainHeight();

    }




    public void LoadData()
    {
        CWGlobal.g_LODWORK = false;

        m_gBlockMap.m_nGemblockCnt = 0;
        m_gBlockMap.m_nResblockCnt = 0;
        m_gBlockMap.m_nBlockCount = 0;

        m_gBlockMap.Load(m_ID);
        m_gBlockMap.Select();

        m_PattenID = m_ID;
        

        m_fAmp = m_gBlockMap.m_fAmp;

        // 사용된 메쉬오브젝트를 읽는다
        
        m_gBuildList = CWLib.FindChild(m_gWorldEditDir, "Building");

        m_kMapInfo.text = string.Format("Gemblock {0} , ResBlock {1} , Colorblock {2}", m_gBlockMap.m_nGemblockCnt, m_gBlockMap.m_nResblockCnt, m_gBlockMap.m_nBlockCount);

    }
    public void SaveData()
    {
        m_gBlockMap.m_nMapID = m_ID;
        m_gBlockMap.Save();
       
        CWFileManager.Instance.Save();
        
        

    }

    // 자원 재배치 
    


    public void TakeMapHeight()
    {
        CWGlobal.g_LODWORK = false;

        m_kTerrain.terrainData.size = new Vector3(m_gBlockMap.WORLDSIZE, CWGlobal.WD_WORLD_HEIGHT, m_gBlockMap.WORLDSIZE);
        m_kTerrain.terrainData.heightmapResolution = m_kWorldImage.width;

        float[,] heights = new float[m_gBlockMap.WORLDSIZE, m_gBlockMap.WORLDSIZE];

        for (int y = 0; y < m_gBlockMap.WORLDSIZE; y++)
        {
            for (int x = 0; x < m_gBlockMap.WORLDSIZE; x++)
            {
                int selx = x / CWGlobal.SELLCOUNT;
                int selz = y / CWGlobal.SELLCOUNT;
                int h=m_gBlockMap.GetHeight(x, y);
                heights[y,x] = (float)h/CWGlobal.WD_WORLD_HEIGHT;
            }
        }
        m_kTerrain.terrainData.SetHeights(0, 0, heights);


    }
    public void ImageApplay()
    {
        CWGlobal.g_LODWORK = false;
        // 터레인에 이미지를 적용한다.
        m_gBlockMap.WORLDSIZE = m_kWorldImage.width;

        m_kTerrain.terrainData.size = new Vector3(m_gBlockMap.WORLDSIZE, CWGlobal.WD_WORLD_HEIGHT, m_gBlockMap.WORLDSIZE);
        m_kTerrain.terrainData.heightmapResolution = m_kWorldImage.width;

        float[,] heights = new float[m_kWorldImage.width, m_kWorldImage.width];

        int pdx = m_kWorldImage.width / CWGlobal.GRIDSIZE;


        for (int y = 0; y < m_kWorldImage.height; y++)
        {
            for (int x = 0; x < m_kWorldImage.width; x++)
            {
                Color kcolor = m_kWorldImage.GetPixel(x, y);
                float fh = Mathf.PerlinNoise(x * m_fAmp, y * m_fAmp);
                heights[x, y] = fh * kcolor.a;
                

            }

        }
        m_kTerrain.terrainData.SetHeights(0, 0, heights);




    }


    // 이미지에서 터레인 적용
    IEnumerator FromImageToTerrainRun()
    {
        CWGlobal.g_LODWORK = false;
        m_gBlockMap.WORLDSIZE = m_kWorldImage.width;
        m_kTerrain.terrainData.size = new Vector3(m_gBlockMap.WORLDSIZE, CWGlobal.WD_WORLD_HEIGHT, m_gBlockMap.WORLDSIZE);
        m_kTerrain.terrainData.heightmapResolution = m_kWorldImage.width;
        yield return null;
        float[,] heights = new float[m_kWorldImage.width, m_kWorldImage.width];

        int pdx = m_kWorldImage.width / CWGlobal.GRIDSIZE;

        


        for (int y = 0; y < m_kWorldImage.height; y++)
        {
            for (int x = 0; x < m_kWorldImage.width; x++)
            {
                Color kcolor = m_kWorldImage.GetPixel(x, y);
                float fhRate = kcolor.a * m_fHeightRate;
                if (m_fAmp > 0)
                {
                    float fh = Mathf.PerlinNoise(x * m_fAmp, y * m_fAmp);
                    //heights[y, x] = fh * fhRate + 0.1f;
                    heights[y, x] = fh * fhRate;
                }
                else
                {
                    heights[y, x] = fhRate ;
                }
            }
        }
        m_kTerrain.terrainData.SetHeights(0, 0, heights);
        yield return null;

    }
    public void FromImageToTerrain()
    {
        CWGlobal.g_LODWORK = false;
        StartCoroutine("FromImageToTerrainRun");


    }
    // 터레인에서 높이값 가져오기
    public void FromTerrainHeight()
    {
        CWGlobal.g_LODWORK = false;

        MakeMap(true, m_PattenID);
        m_gBlockMap.MakeMesh();
       


    }
    // 이미지에서 블록가져오기 
    public void FromImageBlock()
    {
        //m_kPlanetList[m_PattenID].m_nHeight= m_nHeight;

        PLANETDATA kData = m_kPlanetList[m_PattenID];

        kData.m_nHeight = m_nHeight;
        kData.m_fPyungTanRate = m_fPyungTanRate;


        MakeMap(false, m_PattenID);
        m_gBlockMap.MakeMesh();
      
    }



    public void TakeEditCamera()
    {
#if UNITY_EDITOR
        ArrayList sceneViews;
        sceneViews = UnityEditor.SceneView.sceneViews;
        if (sceneViews.Count == 0) return;
        UnityEditor.SceneView sceneView = (UnityEditor.SceneView)sceneViews[0];
        Camera.main.transform.position = sceneView.camera.transform.position;
        Camera.main.transform.rotation = sceneView.camera.transform.rotation;
#endif

    }
    void _Load(int num)
    {
        
        m_Buildflag = true;
    }
    public void BuildingApplay()
    {
        CWAirObject[] array = m_gBuildList.GetComponentsInChildren<CWAirObject>();
        foreach(var v in array)
        {
            v.name = v.m_szName;
            v.Create(0);
            v.CopyBlockMap((int)v.transform.position.x, (int)v.transform.position.y, (int)v.transform.position.z, SetBlock);
        }

    }
  
    public void MakeMapWork(PLANETDATA kData)
    {
        CreateMap();
        m_kWorldImage = GetMask(kData.m_szMask);
        __MakeMap(false, kData);
        m_gBlockMap.MakeMesh();
    }


    //
   
    int m_nResCount1 = 0;
    int m_nResCount2 = 0;
    int m_nResCount3 = 0;
    int GetResourceBlock()
    {
        int RR = RANDOMTEST;

        int rr = CWLib.Random(1, RR);
        if (rr == 2)// 다이아몬드 
        {
            m_nResCount3++;
            return (int)OLDBLOC.Diamond;
        }
        if (rr > 100 && rr <= 400)// 
        {
            m_nResCount1++;
            return (int)OLDBLOC.ResBlock;
        }
        if (rr > 400 && rr <= 800)// 
        {
            m_nResCount2++;
            return (int)OLDBLOC.GemBlock;
        }


        return 0;
    }

    CWBlockGroup m_kBlockGroup ;
    
    public void BlockMaker()
    {
        m_kBlockGroup = new CWBlockGroup(m_gBlockMap.WORLDSIZE);
        m_BlockConverter.Converter(m_gBlockMap, m_kVoxelImage, SetResMakeBlock_ByGroup);
        m_gBlockMap.MakeMesh();

        string szfile = string.Format("file_{0}",CWLib.GetTodayString());
        m_kBlockGroup.Save(szfile);
        print(string.Format("{0} 으로 저장", szfile));

    }


    public void VoxelMaker()
    {


        m_nResCount1 = 0;
        m_nResCount2 = 0;
        m_nResCount3 = 0;

        m_kBlockGroup = new CWBlockGroup(m_nWidth);
        MakeVoxel kMake = new MakeVoxel();
        kMake.Make(false, m_nWidth, m_gVoxelTarget, m_kVoxelImage, SetResMakeBlock_ByGroup);

        m_gBlockMap.MakeMesh();
        m_kBlockGroup.Save(m_szBlockGroup);
        print(string.Format("{0} 저장 Res1 : {1} Res2 : {2} Res3 : {3} ", m_szBlockGroup, m_nResCount1, m_nResCount2, m_nResCount3));

    }


   
    
    public void OnMakeAsset()
    {
        m_gBlockMap.MakeAsset();
    }

    public void MakeProgramHeight()
    {
        m_gBlockMap.WORLDSIZE = m_kWorldImage.width;


        HeightAlgorithm m_kHeighter;
        m_kHeighter = new HeightAlgorithm();
        m_kHeighter.m_fAmp = m_fAmp;
        Color[] kColor = m_kWorldImage.GetPixels();
        m_kHeighter.SetArray(kColor, m_gBlockMap.WORLDSIZE, CWGlobal.WD_WORLD_HEIGHT);

        for (int z = 0; z < m_gBlockMap.WORLDSIZE; z++)
        {
            for (int x = 0; x < m_gBlockMap.WORLDSIZE; x++)
            {
                float h = m_kHeighter.GetHeight(x, z)* CWGlobal.WD_WORLD_HEIGHT;
                for(int y=0;y<h;y++)
                {
                    int nBlock = 1;// m_kHeighter.GetBlock(x,y,z);
                    m_gBlockMap.UpdateBlock(x, y, z,nBlock);
                }
            }
        }



    }



    #endregion

    #region 블록맵만들기

    delegate bool DgCheckSell(int x,int y, int z);
    


    



    Color GetColorMap(int mx,int my,int x,int y)
    {
        return m_kWorldImage.GetPixel(mx * CWGlobal.GRIDSIZE + x, my * CWGlobal.GRIDSIZE + y);

    }
    float GetHeightMap(int mx,int my,int x,int y)
    {
        return m_kTerrain.terrainData.GetHeight(mx * CWGlobal.GRIDSIZE + x, my * CWGlobal.GRIDSIZE + y);
    }
   

    
    // 칼러 스펙트럼 a -> b 
    // 
    // 환경 세팅
    int FindColor(Color c,int nHeight)
    {
        float fr = (float)nHeight/m_nHeight;

        int hh = 0;
        if(fr>=0f && fr<0.2f)
        {
            hh = 0;
        }
        else if (fr >= 0.2f && fr < 0.4f)
        {
            hh = 1;
        }
        else if (fr >= 0.4f && fr < 0.6f)
        {
            hh = 2;
        }
        else if (fr >= 0.6 && fr < 0.8f)
        {
            hh = 3;
        }
        else if (fr >= 0.8f )
        {
            hh = 4;
        }

        
        Vector3 kRet = new Vector3(c.r,c.g,c.b);
        float fMin = 100000f;
        int nBlock = 0;
        foreach(var v in m_kPTBlockData)
        {
            if (v.nBlock[hh] == 0) continue;
            float fdist = Vector3.Distance(v.vColor,kRet);
            if(fdist < fMin)
            {
                fMin = fdist;
                nBlock = v.nBlock[hh];
            }

        }
        return nBlock;

    }


    public int GetBlockbyPattern(Color c,int nheight)
    {
        int num=FindColor(c, nheight);
        return CWArrayManager.Instance.m_kBlock[num].nID;

    }
   


    // 주변과 높이값이 다르면 다른 라이트 텍스쳐를 사용한다 
    bool CheckHeight(int x,int z)
    {
        if (x + 1 >= CWGlobal.GRIDSIZE) return false;
        if (x - 1 <= 0) return false;
        if (z + 1 >= CWGlobal.GRIDSIZE) return false;
        if (z - 1 <= 0) return false;

        int num = z * CWGlobal.GRIDSIZE + x;
        int y = (int)m_kTerrain.terrainData.GetHeight(x, z);

        num = (z+1) * CWGlobal.GRIDSIZE + x;
        int y1 = (int)m_kTerrain.terrainData.GetHeight(x, z+1);
        if (y != y1) return true;

        num = (z - 1) * CWGlobal.GRIDSIZE + x;
        y1 = (int)m_kTerrain.terrainData.GetHeight(x, z - 1);
        if (y != y1) return true;

        num = (z) * CWGlobal.GRIDSIZE + x+1;
        y1 = (int)m_kTerrain.terrainData.GetHeight(x+1, z );
        if (y != y1) return true;

        num = (z) * CWGlobal.GRIDSIZE + x - 1;
        y1 = (int)m_kTerrain.terrainData.GetHeight(x-1, z);
        if (y != y1) return true;

        return false;

    }
 
    // 4개의 맵당 1개씩 
    

    

    #endregion

    #region 건물관련

    
    // 무조건 빨간색만 
    
    // rate 백분률
    string GetTurret(int ntype)
    {
        if (ntype==1)//육지터렛
        {
         
            return CWArrayManager.Instance.FindTurret(1, ntype);

        }
        if (ntype == 2)//바다터렛 
        {
         
            return CWArrayManager.Instance.FindTurret(0, ntype);
        }
        if (ntype == 3)//약한 보스급
        {
            return CWArrayManager.Instance.FindTurret(1, ntype);
        }

        return "";
    }
    string GetBuildName( int nType, ref bool bTurret)
    {
        if (nType == 1)//
        {
            bTurret = true;

            return GetTurret(1);

        }
        if (nType == 2) //빨간색 전함
        {
            bTurret = true;
            return GetTurret( 2);

        }
        if (nType == 3) // 빨간색 레벨3 
        {
            bTurret = true;
            return GetTurret( 3);
        }
        if (nType == 4) //흰색 레벨4 포탑 
        {
            bTurret = true;
            return GetTurret( 4);

        }
        if (nType == 5) //검은색 작은 건물
        {
            bTurret = false;
            return CWTableManager.Instance.RandomTable("Buildinglist", "type", "smallbuilding");
        }
        if (nType == 6) // 검은색 물위에 
        {
            bTurret = false;
            return CWTableManager.Instance.RandomTable("Buildinglist", "type", "waterbuilding");
        }
        if (nType == 7) // 보라색 보물 
        {
            bTurret = false;
            return CWTableManager.Instance.RandomTable("Buildinglist", "type", "resbuilding");
        }

        return "";

    }

    #endregion


    // Use this for initialization


    #region 이벤트처리

    void Line(Vector3 v1,Vector3 v2,int nBlock)
    {
        Vector3 vDir = v2 - v1;
        vDir.Normalize();
        float dist = Vector3.Distance(v1, v2);
        for(float i=0;i<dist;i+=0.5f)
        {
            Vector3 vPos = v1 + i * vDir; 
            SetBlock((int)vPos.x, (int)vPos.y, (int)vPos.z, nBlock);
        }
    }

    void LineEX(Vector3 v1, Vector3 v2,int nRadiusX, int nRadiusY, int nRadiusZ, int nBlock)
    {
        Vector3 vDir = v2 - v1;
        vDir.Normalize();
        float dist = Vector3.Distance(v1, v2);
        for (float i = 0; i < dist; i += 0.5f)
        {
            Vector3 vPos = v1 + i * vDir;
            for(int z=-nRadiusZ;z<= nRadiusZ;z++)
            {
                for (int y = -nRadiusY; y <= nRadiusY; y++)
                {
                    for (int x = -nRadiusX; x <= nRadiusX; x++)
                    {
                        SetBlock((int)vPos.x+x, (int)vPos.y+y, (int)vPos.z+z, nBlock);
                    }

                }

            }
            
        }
    }
    void MakeHeightMap(int fmax)
    {
        Texture2D kNew = new Texture2D(m_gBlockMap.WORLDSIZE, m_gBlockMap.WORLDSIZE);

        for (int sz = 0; sz < m_gBlockMap.WORLDSIZE; sz++)
        {
            for (int sx = 0; sx < m_gBlockMap.WORLDSIZE; sx++)
            {
                int x = sx;
                int z = sz;

                float h = m_kTerrain.terrainData.GetHeight(x, z);
                float fRate = h / fmax;
                kNew.SetPixel(x, z, new Color(fRate, fRate, fRate, fRate));

            }
        }
        kNew.Apply(false);
        string szPath = CWLib.pathForDocumentsPath();
        string szname = string.Format("{0}/heightmap/Heightmap.tga", szPath);
        File.WriteAllBytes(szname, kNew.EncodeToTGA());

    }

    public void Submit()
    {

        string str = m_kInput.text;
        string[] sarry = str.Split(' ');

        if (sarry[0] == "Upchange" || sarry[0] == "uc")
        {
            UpChangeMapBlock(CWLib.ConvertInt(sarry[1]));
        }

        if (sarry[0] == "change" || sarry[0] == "cg")
        {
            ChangeMapBlock(CWLib.ConvertInt(sarry[1]), CWLib.ConvertInt(sarry[2]));
        }
        if (sarry[0] == "all" || sarry[0] == "al")
        {
            ChangeAllMapBlock(CWLib.ConvertInt(sarry[1]));
        }

        if (sarry[0] == "Fill" )
        {

            FillBlock(CWLib.ConvertInt(sarry[1]));
        }
        if (sarry[0] == "Full"|| sarry[0] == "Fl")
        {

            
            Vector3Int vPos = CWMapManager.ConvertPos(m_gSelect.transform.localPosition);
            FullSpaceBlock((int)vPos.x, (int)vPos.y+1, (int)vPos.z);
            
        }


        if (sarry[0] =="Chess")
        {
            int gridx = CWLib.ConvertInt(sarry[1]);
            int gridz = CWLib.ConvertInt(sarry[2]);
            int nblock1 = CWLib.ConvertInt(sarry[3]);
            int nblock2 = CWLib.ConvertInt(sarry[4]);
            Full1(gridx,gridz, nblock1, nblock2);
        }
        if(sarry[0]=="Line")
        {
            int sx = CWLib.ConvertInt(sarry[1]);
            int sy = CWLib.ConvertInt(sarry[2]);
            int sz = CWLib.ConvertInt(sarry[3]);

            int ex = CWLib.ConvertInt(sarry[4]);
            int ey = CWLib.ConvertInt(sarry[5]);
            int ez = CWLib.ConvertInt(sarry[6]);
          //  m_kblock = (OLDBLOC)CWLib.ConvertInt(sarry[7]);
            Line(new Vector3(sx, sy, sz), new Vector3(ex, ey, ez), (int)m_kblock);
        }
        if (sarry[0] == "LineEX")
        {
            int sx = CWLib.ConvertInt(sarry[1]);
            int sy = CWLib.ConvertInt(sarry[2]);
            int sz = CWLib.ConvertInt(sarry[3]);

            int ex = CWLib.ConvertInt(sarry[4]);
            int ey = CWLib.ConvertInt(sarry[5]);
            int ez = CWLib.ConvertInt(sarry[6]);

            int nRadiusX = CWLib.ConvertInt(sarry[7]);
            int nRadiusY = CWLib.ConvertInt(sarry[8]);
            int nRadiusZ = CWLib.ConvertInt(sarry[9]);

          //  m_kblock = (OLDBLOC)CWLib.ConvertInt(sarry[10]);
            LineEX(new Vector3(sx, sy, sz), new Vector3(ex, ey, ez),nRadiusX, nRadiusY, nRadiusZ, (int)m_kblock);
        }

        if (sarry[0] == "Export")
        {
            m_szBlockGroup = sarry[1];
            ExportBlock();

            Debug.Log("Export " + sarry[1]);
        }

        if (sarry[0] == "ImportOld")
        {
            m_szBlockGroup = sarry[1];

            Vector3Int vPos = new Vector3Int();
            if (sarry.Length > 4)
            {
                vPos.x = CWLib.ConvertInt(sarry[2]);
                vPos.y = CWLib.ConvertInt(sarry[3]);
                vPos.z = CWLib.ConvertInt(sarry[4]);
            }
            else
            {
                vPos = CWMapManager.ConvertPos(m_gSelect.transform.localPosition);
            }
            ImportBlockOld((int)vPos.x, (int)vPos.y, (int)vPos.z);


            Debug.Log("ImportOld " + sarry[1]);
        }

        if (sarry[0] == "Import")
        {
            m_szBlockGroup = sarry[1];

            Vector3Int vPos=new Vector3Int();
            if (sarry.Length>4)
            {
                vPos.x= CWLib.ConvertInt(sarry[2]);
                vPos.y = CWLib.ConvertInt(sarry[3]);
                vPos.z = CWLib.ConvertInt(sarry[4]);
            }
            else
            {
                vPos = CWMapManager.ConvertPos(m_gSelect.transform.localPosition);
            }
            ImportBlock((int)vPos.x, (int)vPos.y, (int)vPos.z);


            Debug.Log("Import " + sarry[1]);
        }
       
        if(sarry[0]=="LOD")
        {
            CWGlobal.LOD = 2;
            if (sarry.Length > 1)
            {
                CWGlobal.LOD = CWLib.ConvertInt(sarry[1]);
            }

            
            m_gBlockMap.MakeMeshLOD(CWGlobal.LOD);
            m_gBlockMap.MakeAsset();
        }
        if (sarry[0] == "LODUP")
        {
            if (sarry.Length > 1)
            {
                CWGlobal.LOD = CWLib.ConvertInt(sarry[1]);
            }

            MakeLODBlockUP(CWGlobal.LOD);
        }
        if (sarry[0] == "AutoLOD")
        {
            StartCoroutine("AutoLOD");
        }
        if(sarry[0]== "CalBlockCount")
        {
            StartCoroutine("CalBlockCount");
        }
        if (sarry[0] == "AutoTestLOD")
        {
            StartCoroutine("AutoTestLOD");
        }

        if (sarry[0] == "Capture")
        {

            CaptureImage(m_gBlockMap.m_nMapID);
        }

        if (sarry[0] == "Plan")
        {
            if (sarry.Length > 1)
            {
                CWGlobal.LOD = CWLib.ConvertInt(sarry[1]);
            }

            MakePlan();
        }

       
        if (sarry[0] == "UnderRes")// 자원 설정
        {
            if (sarry.Length > 1)
            {
                m_PattenID = CWLib.ConvertInt(sarry[1]);
            }

            UpdateUnderResBlock();
        }

        if (sarry[0] == "UnderFull")//
        {
            int nBlock = 0;
            if (sarry.Length > 1)
            {
                nBlock = CWLib.ConvertInt(sarry[1]);
            }
            UpdateUnderFullBlock(nBlock);
        }


        if (sarry[0] == "Tree")// 나무 새로 심기 
        {
            int nBlock = CWLib.ConvertInt(sarry[1]);
            int nType = CWLib.ConvertInt(sarry[2]);
            TreeBatch(nBlock,nType);
        }
        // 선택한 블록을 비행기로 카피 
        if(sarry[0]=="aircopy")
        {
            CopyAirBlock(32);

        }
        if (sarry[0] == "HeightMap")
        {
            int nmax = CWLib.ConvertInt(sarry[1]);
            MakeHeightMap(nmax);
        }

        if (sarry[0] == "test")
        {
            MakeColorTable();
        }
        if (sarry[0] == "MakePlanet")
        {
            foreach (var v in m_kPlanetList)
            {
                int nID = v.Key;
                PLANETDATA kData = m_kPlanetList[nID];
                m_kWorldImage = GetMask(kData.m_szMask);
                m_PattenID = nID;
                MakeMap(false, nID);
                m_gBlockMap.m_nMapID = nID;
                m_gBlockMap.Save();
               
            }
        }
        if (sarry[0] == "MakeMap")
        {
            int nID = m_PattenID;
            PLANETDATA kData = m_kPlanetList[nID];
            m_kWorldImage = GetMask(kData.m_szMask);
            MakeMap(false, nID);
            m_gBlockMap.MakeMesh();
        }


    }


    bool m_bDrag = false;
    Vector3Int m_nPos = Vector3Int.zero;
    Vector3Int m_vPrevPos = Vector3Int.zero;
    public void OnPress()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_bDrag = true;
            int nMask = (1 << 11);// 맵만 
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
            {
                Vector3 vvPos = m_gBlockMap.SelectPos(hit.point, hit.normal);
                m_vPrevPos = CWMapManager.ConvertPos(vvPos);

                if (m_bBuildStage)
                {

                    GameObject gg = Instantiate(m_gBuildPrefab);
                    gg.transform.parent = m_gBlockMap.m_ObjectDir.transform;

                    Vector3 vv = hit.point;
                    vv.y++;
                    gg.transform.localPosition =Vector3.zero;
                    gg.transform.position = vv;

                    //gg.transform.localScale = m_gBuildPrefab;
                    gg.transform.localRotation = new Quaternion();
                    gg.name = m_gBuildPrefab.name;
                    return;
                }

            }
            if (Input.GetKey(KeyCode.Z))
            {
                m_bDrag = false;
                Vector3 vPos = m_gBlockMap.SelectPos(hit.point, hit.normal);
                m_kFillData.Clear();
                if (hit.normal == Vector3.up || hit.normal == -Vector3.up)
                {
                    FillBlockXZ((int)vPos.x, (int)vPos.y, (int)vPos.z, (int)m_kblock);
                }
                if (hit.normal == Vector3.forward|| hit.normal == -Vector3.forward)
                {
                    FillBlockXY((int)vPos.x, (int)vPos.y, (int)vPos.z, (int)m_kblock);
                }
                if (hit.normal == Vector3.right || hit.normal == -Vector3.right)
                {
                    FillBlockZY((int)vPos.x, (int)vPos.y, (int)vPos.z, (int)m_kblock);
                    
                }

                
            }


        //    m_gBlockMap.SetMaskArea(m_nPos.x, m_nPos.z, Color.red, m_nRadius);

        }



    }
    public void OnRelease()
    {
        m_bDrag = false;
    }
    

    void OnEnable()
    {
#if UNITY_EDITOR

        EditorApplication.playModeStateChanged += OnUnityPlayModeChanged;
#endif
    }
    private void OnApplicationQuit()
    {
    }
#if UNITY_EDITOR
    void OnUnityPlayModeChanged(PlayModeStateChange state)
    {
        //if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
        //{
        //    //

        //}

        Debug.Log("state = " + state.ToString());
    }
#endif

    #region 카피 블록

    List<UNDODATA> m_kCopyList = new List<UNDODATA>();

    void PasteBlock(Vector3 vPos)
    {
        foreach (var v in m_kCopyList)
        {
            
            SetBlock(v.x+(int)vPos.x, v.y + (int)vPos.y, v.z + (int)vPos.z, v.nBlock);
        }

    }

    void FillBlock(int nblock)
    {
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;

        BoxCollider bb = m_gSelectBox.GetComponentInChildren<BoxCollider>();
        //      bb.bounds.


        int sx = (int)(vPos.x - vScale.x / 2);
        int sy = (int)(vPos.y - vScale.y / 2);
        int sz = (int)(vPos.z - vScale.z / 2);

        int ex = (int)(vPos.x + vScale.x / 2);
        int ey = (int)(vPos.y + vScale.y / 2);
        int ez = (int)(vPos.z + vScale.z / 2);

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);



        for (int z = sz; z <= ez; z++)
        {
            for (int y = sy; y <= ey; y++)
            {
                for (int x = sx; x <= ex; x++)
                {
                    //if(CWLib.IsInsideBounds(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),bb))
                    {
                        SetBlock(x, y, z, nblock);
                    }
                }
            }
        }

        m_gBlockMap.MakeMesh();

    }
    void ChangeAllMapBlock(int n2)
    {
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;
        //      bb.bounds.
        int sx = (int)(vPos.x - vScale.x / 2);
        int sy = (int)(vPos.y - vScale.y / 2);
        int sz = (int)(vPos.z - vScale.z / 2);

        int ex = (int)(vPos.x + vScale.x / 2);
        int ey = (int)(vPos.y + vScale.y / 2);
        int ez = (int)(vPos.z + vScale.z / 2);
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        for (int z = sz; z <= ez; z++)
        {
            for (int y = sy; y <= ey; y++)
            {
                for (int x = sx; x <= ex; x++)
                {
                    {
                        int nBlock = m_gBlockMap.GetBlock(x, y, z);
                        if (nBlock >0)
                        {
                            SetBlock(x, y, z, n2);
                        }

                    }

                }

            }

        }

        m_gBlockMap.MakeMesh();

    }
    void UpChangeMapBlock(int n1)
    {
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;
        //      bb.bounds.
        int sx = (int)(vPos.x - vScale.x / 2);
        int sy = (int)(vPos.y - vScale.y / 2);
        int sz = (int)(vPos.z - vScale.z / 2);

        int ex = (int)(vPos.x + vScale.x / 2);
        int ey = (int)(vPos.y + vScale.y / 2);
        int ez = (int)(vPos.z + vScale.z / 2);
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        for (int z = sz; z <= ez; z++)
        {
            for (int x = sx; x <= ex; x++)
            {
                for (int y = ey; y > sy; y--)
                {
                    int nBlock = m_gBlockMap.GetBlock(x, y, z);
                    if (nBlock>0)
                    {
                        SetBlock(x, y, z, n1);
                        break;
                    }

                }

            }

        }

        m_gBlockMap.MakeMesh();
    }
    void ChangeMapBlock(int n1,int n2)
    {
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;
        //      bb.bounds.
        int sx = (int)(vPos.x - vScale.x/2);
        int sy = (int)(vPos.y - vScale.y/2);
        int sz = (int)(vPos.z - vScale.z/2);

        int ex = (int)(vPos.x + vScale.x/2);
        int ey = (int)(vPos.y + vScale.y/2);
        int ez = (int)(vPos.z + vScale.z/2);
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        for (int z = sz; z <= ez; z++)
        {
            for (int y = sy; y <= ey; y++)
            {
                for (int x = sx; x <= ex; x++)
                {
                    {
                        int nBlock = m_gBlockMap.GetBlock(x, y, z);
                        if (nBlock == n1)
                        {
                            SetBlock(x, y, z, n2);
                        }

                    }

                }

            }

        }

        m_gBlockMap.MakeMesh();
    }

    //            foreach (var v in m_kPlanetList)
    //        {
    //            int nID = v.Key;
    //PLANETDATA kData = m_kPlanetList[nID];
    //m_kWorldImage = GetMask(kData.m_szMask);
    //MakeMap(false, nID);
    //m_gBlockMap.m_nMapID = nID;
    //            m_gBlockMap.Save();


    //        }

    
    void CaptureImage(int nMapID)
    {
        if (m_gCapture == null) return;
        string szPath = CWLib.pathForDocumentsPath();
        string szpath = string.Format("{0}/MapCapture/{1}.png", szPath, nMapID);

        CWLib.CaptureImage(2048, m_gCapture, szpath);

    }

    //var MapArray = [82,83,82,114,83,115,82,121,82,31,100,82,123,83,122];
    bool IsLod2(int nID)
    {
        int[] MapIds = { 82, 83, 82, 114, 83, 115, 82, 121, 82, 31, 100, 82, 123, 83, 122 };
        for(int i=0;i<MapIds.Length;i++)
        {
            if (nID ==MapIds[i])
            {
                return true;
            }
        }
        return false;
        

    }
    void MakeTestLODBlock()
    {
        int sx = 0;
        int sy = 0;
        int sz = 0;

        int ex = m_nWidth;
        int ey = 64;
        int ez = m_nWidth;

        CWGlobal.g_LODWORK = true;

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);
        for (int z = sz; z < ez; z++)
        {
            for (int x = sx; x < ex; x++)
            {
                for (int y = 0; y < ey; y++)
                {
                    int tx = (x / CWGlobal.LOD) * CWGlobal.LOD;
                    int ty = (y / (CWGlobal.LOD)) * (CWGlobal.LOD);
                    int tz = (z / CWGlobal.LOD) * CWGlobal.LOD;
                    int nBlock = m_gBlockMap.GetBlock(tx, ty, tz);
                    if(y>=1)
                    {
                        nBlock = 0;
                    }
                    m_gBlockMap.SetBlock(x, y, z, nBlock);
                }

            }
        }
        m_gBlockMap.MakeMesh();


    }
    IEnumerator AutoTestLOD()
    {



        for (int i = 0; i < MAXMAPCOUNT; i++)
        {
            int nMapid = CWArrayManager.Instance.GetMapID(i + 1);
            if (nMapid == 0)
            {
                continue;
            }
            m_gBlockMap.m_nGemblockCnt = 0;
            m_gBlockMap.m_nResblockCnt = 0;
            m_gBlockMap.m_nBlockCount = 0;
            

            CWGlobal.LOD = 8;
            CWGlobal.g_LODWORK = true;
            m_gBlockMap.Load(nMapid);

            yield return null;
            MakeTestLODBlock();
            yield return null;
            m_gBlockMap.MakeAssetTest();
            yield return null;
        }

       


        yield return null;

    }
    IEnumerator AutoLOD()
    {
        

        string strInfo = "";

        for(int i=0;i<MAXMAPCOUNT;i++)
        {
            int nMapid = CWArrayManager.Instance.GetMapID(i+1);
            if(nMapid==0)
            {
                continue;
            }
            m_gBlockMap.m_nGemblockCnt = 0;
            m_gBlockMap.m_nResblockCnt = 0;
            m_gBlockMap.m_nBlockCount = 0;

            //CWGlobal.LOD = 1;
            //CWGlobal.g_LODWORK = false;
            //m_gBlockMap.Load(nMapid);
            //m_gBlockMap.MakeMesh();
            //Debug.Log(string.Format("make {0}", nMapid));
            //yield return null;
            //CaptureImage(nMapid);

            m_gBlockMap.Load(nMapid);
            yield return null;
            if(i<50)
            {
                CWGlobal.LOD = 8;
            }
            else
            {
                CWGlobal.LOD = 16;
            }
            if(IsLod2(nMapid)) CWGlobal.LOD = 8;
            else CWGlobal.LOD = 16;
            


            CWGlobal.g_LODWORK = true;
            
            m_gBlockMap.MakeMeshLOD(CWGlobal.LOD);
            yield return null;
            m_gBlockMap.MakeAsset();
            strInfo += string.Format("{0}] {1}       {2}       {3}", nMapid, m_gBlockMap.m_nGemblockCnt, m_gBlockMap.m_nResblockCnt, m_gBlockMap.m_nBlockCount);
            strInfo += "\n";
            yield return null;
        }

        print(strInfo);

        for(int i=0;i<256;i++)
        {
            print(string.Format("{0} : {1} ",(OLDBLOC)i, m_gBlockMap.m_nBlockCnt[i]));
        }
        


         yield return null;

    }
    IEnumerator CalBlockCount()
    {


        string strInfo = "";

        for (int i = 0; i < MAXMAPCOUNT; i++)
        {
            int nMapid = i+1;
            m_gBlockMap.m_nGemblockCnt = 0;
            m_gBlockMap.m_nResblockCnt = 0;
            m_gBlockMap.m_nBlockCount = 0;
            m_gBlockMap.Load(nMapid);
            strInfo += string.Format("{0} {1}", nMapid, m_gBlockMap.m_nBlockCount);
            strInfo += "\n";
        }

        print(strInfo);

      



        yield return null;

    }

    #endregion

    Vector3 vPreHit = Vector3.zero;

    // 블록을 깍는다
    void ShaveBlock(Vector3 vPos, Vector3 vNormal,int nRadius)
    {

        int nx=0, ny=0, nz=0;
        nx = (int)vNormal.x;
        ny = (int)vNormal.y;
        nz = (int)vNormal.z;

        int nblock = m_gBlockMap.GetBlock((int)vPos.x, (int)vPos.y, (int)vPos.z);
        int dx = m_nRadius - 1;

        for (int z = -dx; z <= dx; z++)
        {
            for (int x = -dx; x <= dx; x++)
            {
                int tx=0, ty=0, tz=0;
                if(nx!=0)
                {
                    tx = (int)vPos.x ;
                    ty = (int)vPos.y+x;
                    tz = (int)vPos.z + z;
                }
                else if (ny != 0)
                {
                    tx = (int)vPos.x + x;
                    ty = (int)vPos.y;
                    tz = (int)vPos.z + z;

                }
                else if (nz != 0)
                {
                    tx = (int)vPos.x + x;
                    ty = (int)vPos.y + z;
                    tz = (int)vPos.z ;

                }

                int n = m_gBlockMap.GetBlock(tx, ty, tz);
                SetBlock(tx, ty, tz, 0);
                if (n > 0)
                {
                    //SetBlock(tx-nx, ty -ny, tz-nz, nblock);
                }
            }

        }
    }

    // 블록을 채워 넣는다
    void FullBlock(Vector3 vPos, int nRadius)
    {
        int nblock = m_gBlockMap.GetBlock((int)vPos.x, (int)vPos.y, (int)vPos.z);
        int dx = m_nRadius - 1;
        int dy = dx / 2;

        for (int z = -dx; z <= dx; z++)
        {
            for (int x = -dx; x <= dx; x++)
            {

                for (int y = 0; y <= dy; y++)
                {
                    int tx = 0, ty = 0, tz = 0;
                    tx = (int)vPos.x + x;
                    ty = (int)vPos.y - y;
                    tz = (int)vPos.z + z;
                    int n = m_gBlockMap.GetBlock(tx, ty, tz);
                    if(n==0)
                    {
                        SetBlock(tx, ty, tz, nblock);
                    }
                    

                }
            }

        }
    }

#if UNITY_EDITOR

    int CalFaceNormal(Vector3 vNormal)
    {
        if (vNormal.z == -1) return 1;
        if (vNormal.z ==  1) return 2;
        if (vNormal.x ==  1) return 3;
        if (vNormal.x == -1) return 4;
        if (vNormal.y == 1) return 5;

        return 6;

    }
    #region 컨트롤키  카피 복사 

    bool m_bCtrlFlag = false;
    public GameObject[] m_gCtrlPos;

    int m_CtrlCnt = 0;
    void SetCtrlPos(Vector3 vPos)
    {
        if (m_CtrlCnt > 0) return;

        m_gSelectBox.SetActive(true);
        if (m_CtrlCnt==0)
        {
            m_gCtrlPos[0].SetActive(true);
            m_gCtrlPos[0].transform.position = vPos;
            
        }
        m_CtrlCnt++;

    }
    void DrawCtrl()
    {
        foreach(var v in m_gCtrlPos)
        {
            if (v.activeSelf == false) return;
        }

        int sx1 = (int)m_gCtrlPos[0].transform.position.x;
        int sy1 = (int)m_gCtrlPos[0].transform.position.y;
        int sz1 = (int)m_gCtrlPos[0].transform.position.z;

        int sx2 = (int)m_gCtrlPos[1].transform.position.x;
        int sy2 = (int)m_gCtrlPos[1].transform.position.y;
        int sz2 = (int)m_gCtrlPos[1].transform.position.z;


        int sx=sx1, sy=sy1, sz=sz1;
        int ex=sx2, ey=sy2, ez=sz2;

        if (sx1 > sx2) { sx = sx2;ex = sx1; }
        if (sy1 > sy2) { sy = sy2; ey = sy1; }
        if (sz1 > sz2) { sz = sz2; ez = sz1; }




        for (int z = sz; z <= ez; z++)
        {
            for (int y = sy; y <= ey; y++)
            {
                for (int x = sx; x <= ex; x++)
                {
                    {
                        SetBlock(x, y, z, (int)m_kblock);
                    }
                }
            }
        }

        m_gBlockMap.MakeMesh();



        m_gCtrlPos[0].SetActive(false);
        m_gCtrlPos[1].SetActive(false);
    }
    void UpdateCtrlBox()
    {
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;
               

        int sx1 = (int)m_gCtrlPos[0].transform.position.x;
        int sy1 = (int)m_gCtrlPos[0].transform.position.y;
        int sz1 = (int)m_gCtrlPos[0].transform.position.z;

        m_gCtrlPos[1].transform.position=m_gSelect.transform.position;

        int sx2 = (int)m_gCtrlPos[1].transform.position.x;
        int sy2 = (int)m_gCtrlPos[1].transform.position.y;
        int sz2 = (int)m_gCtrlPos[1].transform.position.z;


        int sx = sx1, sy = sy1, sz = sz1;
        int ex = sx2, ey = sy2, ez = sz2;

        if (sx1 > sx2) { sx = sx2; ex = sx1; }
        if (sy1 > sy2) { sy = sy2; ey = sy1; }
        if (sz1 > sz2) { sz = sz2; ez = sz1; }

        int dx, dy, dz;
        int cx, cy, cz;
        dx = Mathf.Abs(sx1 - sx2)+1;
        dy = Mathf.Abs(sy1 - sy2) + 1;
        dz = Mathf.Abs(sz1 - sz2) + 1;
        cx = sx + dx / 2;
        cy = sy + dy / 2;
        cz = sz + dz / 2;
        vPos.x = cx+0.5f;
        vPos.y = cy + 0.5f;
        vPos.z = cz + 0.5f;
        vScale.x = dx;
        vScale.y = dy;
        vScale.z = dz;

        m_gSelectBox.transform.position = vPos;
        m_gSelectBox.transform.localScale = vScale;
    }

    #endregion

    void Update()
    {
        if (m_gBlockMap == null) return;
        if(m_bCtrlFlag)
        {
            UpdateCtrlBox();
        }
        Vector3Int nPos = Vector3Int.zero;
        Vector3 vPos = Vector3.zero;
        int nBlock = 0;
        int nMask = (1 << 11);// 맵만 
        
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
            {
                vPos = m_gBlockMap.SelectPos(hit.point, hit.normal);
                m_gSelect.transform.localPosition = vPos;
                if(m_bCtrlFlag)
                {
                    SetCtrlPos(vPos);
                }
                nBlock = m_gBlockMap.GetBlock(vPos);

                //Vector3Int vInt = CWMapManager.ConvertPos(vPos);
                //m_kInfo.text = string.Format("{0} {1} {2} {3}",hit.point,hit.normal,vPos,vInt);

                nPos = CWMapManager.ConvertPos(vPos);
                m_nPos = nPos;
                if (m_bFixX)
                {
                    nPos.x = m_vPrevPos.x;
                }
                if (m_bFixY)
                {
                    nPos.y = m_vPrevPos.y;
                }
                if (m_bFixZ)
                {
                    nPos.z = m_vPrevPos.z;
                }

                OLDBLOC kk = (OLDBLOC)nBlock;
                m_kSelectSell = m_gBlockMap.GetSellGroup((int)vPos.x, (int)vPos.z);
                if (m_kSelectSell != null)
                {
                    

                    m_kInfo.text = string.Format("{0}={1}({2}) Cnt={3} Face={4} blockpos={5}"
                        , vPos, nBlock, kk, m_kSelectSell.GetVertexCount(), CalFaceNormal(hit.normal), nPos);


                    //m_gBlockMap.SetMaskArea(nPos.x, nPos.z, Color.red, m_nRadius);


                }




            }
            if (m_bDrag && !m_bCtrlFlag)
            {
                
                if (!m_kInput.isFocused)
                {
                    if (vPreHit != Input.mousePosition)
                    {




                        if (m_kDelType != DELTYPE.NONE)
                        {
                            if (m_kDelType == DELTYPE.PYUNGTAN)
                            {
                                ShaveBlock(vPos, Vector3.up, m_nRadius);

                            }
                            else if (m_kDelType == DELTYPE.PYUNGTANDIR)
                            {
                                ShaveBlock(vPos, hit.normal, m_nRadius);

                            }
                            else if (m_kDelType == DELTYPE.FULLDOWN)
                            {
                                //ShaveBlock(vPos, hit.normal, m_nRadius);
                                FullBlock(vPos, m_nRadius);

                            }
                            else
                            {
                                int nblock = m_gBlockMap.GetBlock((int)nPos.x, (int)nPos.y, (int)nPos.z);
                                int dx = m_nRadius - 1;
                                for (int z = -dx; z <= dx; z++)
                                {
                                    for (int x = -dx; x <= dx; x++)
                                    {
                                        int tx, ty, tz;
                                        tx = (int)nPos.x + x;
                                        ty = (int)nPos.y;
                                        tz = (int)nPos.z + z;

                                        int n = m_gBlockMap.GetBlock(tx, ty, tz);
                                        SetBlock(tx, ty, tz, 0);
                                        if (n > 0)
                                        {
                                            SetBlock(tx, ty - 1, tz, n);
                                        }

                                    }

                                }
                            }

                        }
                        else
                        {
                            if (m_bBuildStage)
                            {

                                return;
                            }
                            if (BBlockflag)
                            {
                                if (nPos == Vector3.zero) return;
                                
                                float fd = Vector3.Distance(m_vPrevPos, nPos);
                                if (fd > 3f) return;
                                if(MapCtrl.Instance.UpBound)
                                {
                                    if (m_nRadius >= 1)
                                    {
                                        int dx = m_nRadius -1;
                                        for (int z = -dx; z <= dx; z++)
                                        {
                                            for (int x = -dx; x <= dx; x++)
                                            {
                                                for (int y = CWGlobal.WD_HEIGHT; y >=0; y--)
                                                {

                                                    if (MapCtrl.Instance.SPHERE)
                                                    {
                                                        float fdist = Mathf.Sqrt((x * x) + (y * y) + (z * z));
                                                        if (fdist > dx) continue;

                                                    }
                                                    int tx, ty, tz;
                                                    tx = (int)nPos.x + x;
                                                    ty = (int)nPos.y + y;
                                                    tz = (int)nPos.z + z;

                                                    if (m_gBlockMap.GetBlock(tx, ty, tz) > 0)
                                                    {
                                                        SetBlock(tx, ty, tz, (int)m_kblock);
                                                        break;
                                                    }

                                                }

                                            }

                                        }

                                    }
                                }
                                else
                                {
                                    if (m_bChange)
                                    {
                                        SetBlock((int)nPos.x, (int)nPos.y, (int)nPos.z, (int)m_kblock);

                                        if (m_nRadius > 1)
                                        {
                                            int dx = m_nRadius - 1;
                                            for (int z = -dx; z <= dx; z++)
                                            {
                                                for (int y = -dx; y <= dx; y++)
                                                {
                                                    for (int x = -dx; x <= dx; x++)
                                                    {

                                                        if (MapCtrl.Instance.SPHERE)
                                                        {
                                                            float fdist = Mathf.Sqrt((x * x) + (y * y) + (z * z));
                                                            if (fdist > dx) continue;

                                                        }

                                                        int tx, ty, tz;
                                                        tx = (int)nPos.x + x;
                                                        ty = (int)nPos.y + y;
                                                        tz = (int)nPos.z + z;

                                                        if (m_gBlockMap.GetBlock(tx, ty, tz) > 0)
                                                        {
                                                            SetBlock(tx, ty, tz, (int)m_kblock);
                                                        }

                                                    }

                                                }

                                            }

                                        }


                                    }
                                    else
                                    {
                                        Vector3 vv = m_gBlockMap.GetEditBlock(hit.point, hit.normal);
                                        Vector3Int vi = CWMapManager.ConvertPos(vv);
                                        SetBlock(vi.x, vi.y, vi.z, (int)m_kblock);

                                        if (m_nRadius > 1)
                                        {
                                            int dx = m_nRadius - 1;
                                            for (int z = -dx; z <= dx; z++)
                                            {
                                                for (int y = -dx; y <= dx; y++)
                                                {
                                                    for (int x = -dx; x <= dx; x++)
                                                    {

                                                        if (MapCtrl.Instance.SPHERE)
                                                        {
                                                            float fdist = Mathf.Sqrt((x * x) + (y * y) + (z * z));
                                                            if (fdist > dx) continue;

                                                        }
                                                        int tx, ty, tz;
                                                        tx = (int)vi.x + x;
                                                        ty = (int)vi.y + y;
                                                        tz = (int)vi.z + z;
                                                        SetBlock(tx, ty, tz, (int)m_kblock);

                                                    }

                                                }

                                            }

                                        }



                                    }

                                }
                                m_vPrevPos = nPos;

                            }
                        }

                    }
                    vPreHit = Input.mousePosition;

                }
            }
         


        }

        if (!m_kInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_gMenu.SetActive(false);
            }

            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    // 평탄삭제 아래방향
            //    m_kDelType = DELTYPE.PYUNGTAN;

            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{

            //    m_kDelType = DELTYPE.PYUNGTANDIR;

            //}
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    m_kDelType = DELTYPE.DELDOWN;

            //}
            //if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    m_kDelType = DELTYPE.FULLDOWN;

            //}
            //if (Input.GetKeyDown(KeyCode.Alpha0))
            //{
            //    m_kDelType = DELTYPE.NONE;

            //}



            if (Input.GetKeyDown(KeyCode.Delete))
            {

                SetBlock((int)nPos.x, (int)nPos.y, (int)nPos.z, 0);
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {

                //                SetBlock((int)nPos.x, (int)nPos.y, (int)nPos.z, 0);
                //CWMapManager.SelectMap.Hit(true, vPos, 1);
                m_gBlockMap.Hit(false, vPos, 1);
            }


            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                UndoPlay();
            }
            // 카피 
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // 터렛
                if (m_szTurret != null && m_szTurret.Length > 1)
                {
                    if(m_bTurret)
                    {
                        CWMobManager.Instance.AddData(vPos, 0, m_nTurretLevel, m_szTurret, AIOBJECTTYPE.TURRET);
                    }
                    else
                    {
                        vPos.y = m_gBlockMap.GetHeight(vPos)+20f;
                        CWMobManager.Instance.AddData(vPos, 0, m_nTurretLevel, m_szTurret, AIOBJECTTYPE.DRONE);
                    }
                    
                }

            }
            if (Input.GetKeyDown(KeyCode.F2))
            {



                //  m_kBuildObject.Create(0);
                m_kBuildObject.CopyBlockMap((int)nPos.x, (int)nPos.y, (int)nPos.z, SetBuildBlock);

                //PasteBlock(m_gSelectBox.transform.position);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                

            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                ImportBlock((int)nPos.x, (int)nPos.y, (int)nPos.z);
                //PasteBlock(m_gSelectBox.transform.position);
            }

          

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                m_bCtrlFlag = true;

                m_CtrlCnt = 0;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                m_bCtrlFlag = false;
                //DrawCtrl();
            }


        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Submit();
        }

        EndUndo();
    }
#endif
#endregion

    #region 블록칠하기



    class UNDODATA
    {
        public int x, y, z;
        public int nBlock;
    };
    List<List<UNDODATA>> m_kUndo = new List<List<UNDODATA>>();

    List<UNDODATA> m_kUndoList = new List<UNDODATA>();

    void BeginUndo()
    {
        if(m_kUndoList==null)
            m_kUndoList = new List<UNDODATA>();
    }
    void EndUndo()
    {
        if(m_kUndoList!=null)
        {
            m_kUndo.Add(m_kUndoList);
            m_kUndoList = null;
        }
    }
    void AddUndo( int x,int y,int z)
    {
        int n = m_gBlockMap.GetBlock(x, y, z);
        UNDODATA udata = new UNDODATA();
        udata.x = x;
        udata.y = y;
        udata.z = z;
        udata.nBlock = n;
        m_kUndoList.Add(udata);
    }
    public void SetBuildBlock(int x, int y, int z, int nBlock)
    {
        if (m_nDelDownHeight >= 0)
        {
            if (y <= m_nDelDownHeight) return;
        }
        if (m_nDelUpHeight >= 0)
        {
            if (y >= m_nDelUpHeight) return;
        }

        BeginUndo();
        AddUndo(x, y, z);
        if(nBlock==(int)OLDBLOC.GoldBlock)
        {
            nBlock =(int) OLDBLOC.GemBlock;
        }
        if (nBlock == (int)OLDBLOC.Ruby)
        {
            nBlock = (int)OLDBLOC.GemBlock;
        }
        if (nBlock == (int)OLDBLOC.Emerald)
        {
            nBlock = (int)OLDBLOC.GemBlock;
        }

        if (nBlock == (int)OLDBLOC.Diamond)
        {
            nBlock = (int)OLDBLOC.GemBlock;
        }


        m_gBlockMap.UpdateBlock(x, y, z, nBlock);
    }
    void SetResMakeBlock_ByGroup(int x, int y, int z, int nBlock)
    {
        //랜덤하게 자원 블록을 배치한다
        int nRes = GetResourceBlock();
        if (nRes > 0)
        {
            nBlock = nRes;

        }
        SetBlock(x, y, z, nBlock);
        m_kBlockGroup.AddData(x, y, z, nBlock);

    }

    int ConvertOldblock(int nBlock)
    {
        if (nBlock == 212) return 212;
        if (nBlock == 213) return 216;
        if (nBlock == 214) return 217;
        if (nBlock == 215) return 218;
        if (nBlock == 216) return 219;
        if (nBlock == 217) return 220;
        if (nBlock == 218) return 221;
        if (nBlock == 219) return 222;
        if (nBlock == 220) return 223;
        if (nBlock == 221) return 224;
        if (nBlock == 222) return 225;
        if (nBlock == 223) return 214;
        if (nBlock == 224) return 213;
        if (nBlock == 225) return 215;
        if (nBlock == 226) return 226;

        if (nBlock == 13) return 4;
        if (nBlock == 14) return 4;
        if (nBlock == 15) return 4;
        if (nBlock == 16) return 4;
        if (nBlock == 17) return 4;

        if (nBlock == 38) return 205;
        if (nBlock == 39) return 205;
        if (nBlock == 40) return 205;
        if (nBlock == 41) return 205;
        if (nBlock == 42) return 205;
        if (nBlock == 43) return 205;


        if (nBlock == 70) return 89;
        if (nBlock == 71) return 90;
        if (nBlock == 72) return 91;

        if (nBlock == 74) return 92;
        if (nBlock == 75) return 89;
        if (nBlock == 76) return 89;
        if (nBlock == 77) return 89;
        if (nBlock == 78) return 89;
        if (nBlock == 79) return 89;
        if (nBlock == 80) return 89;

        if (nBlock == 73) return 227;

        if (nBlock == 56) return 89;
        if (nBlock == 57) return 89;
        if (nBlock == 58) return 89;
        if (nBlock == 64) return 89;

        if (nBlock == 49) return 75;
        if (nBlock == 50) return 75;
        if (nBlock == 51) return 75;

        if (nBlock == 61) return (int)OLDBLOC.GemBlock;
        if (nBlock == 96) return (int)OLDBLOC.GemBlock;
        if (nBlock == 99) return (int)OLDBLOC.GemBlock;
        if (nBlock == 101) return (int)OLDBLOC.GemBlock;

        if (nBlock == 65) return 204;
        if (nBlock == 66) return 204;
        if (nBlock == 67) return 204;
        if (nBlock == 68) return 204;
        if (nBlock == 69) return 204;


        return nBlock;
    }
    public void ImportBlockOld(int x, int y, int z)
    {
        CWBlockGroup kBlock = new CWBlockGroup(m_gBlockMap);

        kBlock.Load(m_szBlockGroup);
        kBlock.ApplyMap(x, y, z, SetResMakeBlockOld);


        m_gBlockMap.MakeMesh();

    }
    void SetResMakeBlockOld(int x, int y, int z, int nn)
    {
        //랜덤하게 자원 블록을 배치한다
        int nBlock = ConvertOldblock(nn);

        int nRes = GetResourceBlock();
        if (nRes > 0)
        {
            nBlock = nRes;

        }
        SetBlock(x, y, z, nBlock);
    }

    void SetResMakeBlock(int x, int y, int z, int nBlock)
    {

        int nRes = GetResourceBlock();
        if (nRes > 0)
        {
            nBlock = nRes;

        }
        SetBlock(x, y, z, nBlock);
    }

    public void SetBlock(int x, int y, int z, int nBlock)
    {
        if(m_nDelDownHeight>=0)
        {
            if (y <= m_nDelDownHeight) return;
        }
        if (m_nDelUpHeight >= 0)
        {
            if (y >= m_nDelUpHeight) return;
        }

        BeginUndo();
        AddUndo( x, y, z);
        m_gBlockMap.UpdateBlock(x, y, z, nBlock);
    }

    void UndoPlay()
    {

        int tcnt =m_kUndo.Count;
        if (tcnt == 0) return;
        List<UNDODATA> klist = m_kUndo[tcnt-1];
        foreach(var v in klist)
        {
            m_gBlockMap.UpdateBlock(v.x, v.y, v.z, v.nBlock);
        }
        m_kUndo.RemoveAt(tcnt - 1);

    }
    void FullBlock(int sx,int sy,int sz,int ex,int ey,int ez,int nblock)
    {
        for(int z=sz;z<ez;z++)
        {
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    int nBlock = m_gBlockMap.GetBlock(x, y, z);
                    if (nBlock == 0) continue;
                    SetBlock(x, y, z, nblock);
                }
            }
        }

    }
    // 같은 높이
    
    Dictionary<Vector3Int, bool> m_kFillData = new Dictionary<Vector3Int, bool>();

    void FillBlockXZ(int sx,int sy,int sz,int nBlock)
    {
        if (m_kFillData.Count > 3000) return;
        Vector3Int vv = new Vector3Int(sx, sy, sz);
        if (m_kFillData.ContainsKey(vv))
        {
            return;
        }
        m_kFillData.Add(vv,true);
        


        if (m_gBlockMap.GetBlock(sx, sy, sz) > 0)
        {
            SetBlock(sx, sy, sz, nBlock);
            for (int z = -1; z <= 1; z++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && z == 0) continue;

                    // 같은 높이인가?


                    int n1 = m_gBlockMap.GetBlock(sx + x, sy, sz+z);
                    int n2 = m_gBlockMap.GetBlock(sx + x, sy+1, sz + z);
                    if (n1>0&& n2==0)
                    {


                        FillBlockXZ(sx + x, sy, sz + z, nBlock);
                    }
                }
            }

        }
        else return;
        
    }
    void FillBlockXY(int sx, int sy, int sz, int nBlock)
    {
        if (m_kFillData.Count > 3000) return;
        Vector3Int vv = new Vector3Int(sx, sy, sz);
        if (m_kFillData.ContainsKey(vv))
        {
            return;
        }
            
        m_kFillData.Add(vv, true);

        if (m_gBlockMap.GetBlock(sx, sy, sz) > 0)
        {
            SetBlock(sx, sy, sz, nBlock);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0) continue;
                    int n1 = m_gBlockMap.GetBlock(sx + x, sy + y, sz + 1);
                    int n2 = m_gBlockMap.GetBlock(sx + x, sy + y, sz - 1);
                    if (n1 == 0 && n2 == 0)
                    {
                        FillBlockXY(sx + x, sy+y, sz , nBlock);
                    }
                }
            }

        }
        else return;
    }
    void FillBlockZY(int sx, int sy, int sz, int nBlock)
    {
        if (m_kFillData.Count > 3000) return;
        Vector3Int vv = new Vector3Int(sx, sy, sz);
        if (m_kFillData.ContainsKey(vv)) return;
        m_kFillData.Add(vv, true);

        if (m_gBlockMap.GetBlock(sx, sy, sz) > 0)
        {
            SetBlock(sx, sy, sz, nBlock);
            for (int z = -1; z <= 1; z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (y == 0 && z == 0) continue;
                    int n1 = m_gBlockMap.GetBlock(sx + 1, sy + y, sz + z);
                    int n2 = m_gBlockMap.GetBlock(sx - 1, sy + y, sz + z);
                    if (n1 == 0 && n2 == 0)
                    {
                        FillBlockZY(sx , sy + y, sz + z, nBlock);
                    }
                }
            }

        }
        else return;
    }


    void Full1(int gridx,int gridz,int nblock1,int nblock2)
    {


        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;

        int sx = (int)(vPos.x - vScale.x / 2);
        int sy = (int)(vPos.y - vScale.y / 2);
        int sz = (int)(vPos.z - vScale.z / 2);

        int ex = (int)(vPos.x + vScale.x / 2);
        int ey = (int)(vPos.y + vScale.y / 2);
        int ez = (int)(vPos.z + vScale.z / 2);



        int tcnt = 0;
        for (int z=sz;z<ez;z+= gridz)
        {
            for (int x = sx; x <ex; x+= gridx)
            {
                int nn = 0;
                if (tcnt % 2 == 0)
                {
                    nn = nblock1;
                }
                else nn = nblock2;

                FullBlock(x, sy, z, x + gridx, ey, z + gridz, nn);
                tcnt++;
            }
            tcnt++;
        }

    }
    

    

    


    #region 공간채우기 (최대 200000)
    Vector3[] g_vDir =
    {
        new Vector3(1,0,0),
        new Vector3(-1,0,0),

        new Vector3(0,1,0),
        new Vector3(0,-1,0),

        new Vector3(0,0,1),
        new Vector3(0,0,-1),


    };


    int MAXX = 200000;
    bool m_bThrFlag = false;
    Vector3Int m_vFullStart = new Vector3Int();

    Vector3Int m_vFullMin = new Vector3Int();
    Vector3Int m_vFullMax = new Vector3Int();

    Dictionary<int, int> m_kUseBlock = new Dictionary<int, int>();

    int FullSpace(int nCount, int sx, int sy, int sz)
    {

        TVECTER tt = new TVECTER();
        tt.x = (byte)sx;
        tt.y = (byte)sy;
        tt.z = (byte)sz;
        int num = tt.num;
        if (m_kUseBlock.ContainsKey(num))
        {
            return nCount;
        }
        nCount++;
        if (nCount > MAXX)
        {
            return -1;
        }
        int nblock = GetUnderGroundBlock( sy);
        m_kUseBlock.Add(num, nblock);
        int ssx = m_vFullMin.x;
        int ssy = m_vFullMin.y;
        int ssz = m_vFullMin.z;
        int sex = m_vFullMax.x;
        int sey = m_vFullMax.y;
        int sez = m_vFullMax.z;
        

        foreach (var v in g_vDir)
        {
            int tx = sx + (int)v.x;
            int ty = sy + (int)v.y;
            int tz = sz + (int)v.z;
            if (tx <= ssx) continue;
            if (ty <= ssy) continue;
            if (tz <= ssz) continue;
            if (tx >= sex) continue;
            if (ty >= sey) continue;
            if (tz >= sez) continue;


            int n2 = m_gBlockMap.GetBlock(tx, ty, tz);
            if (n2 == 0)
            {
                int ttt = m_kUseBlock.Count;
                nCount = FullSpace(nCount, tx, ty, tz);
                if (nCount == -1)
                {
                    return -1;
                }

            }

        }
        return nCount;
    }

    void ThreadFullSpace()
    {
        int nCount = 0;
        FullSpace(nCount, m_vFullStart.x, m_vFullStart.y, m_vFullStart.z);
        m_bThrFlag = false;

    }
    IEnumerator IFullSpace()
    {
        while(m_bThrFlag)
        {
            yield return null;
        }
        foreach (var v in m_kUseBlock)
        {
            TVECTER t = new TVECTER();
            t.num = v.Key;
            SetBlock(t.x, t.y, t.z, v.Value);
        }

    }
    void FullSpaceBlock(int sx, int sy, int sz)
    {
        int ssx = m_vFullMin.x;
        int ssy = m_vFullMin.y;
        int ssz = m_vFullMin.z;
        int sex = m_vFullMax.x;
        int sey = m_vFullMax.y;
        int sez = m_vFullMax.z;
        GetSelectBox(ref ssx, ref ssy, ref ssz, ref sex, ref sey, ref sez);

        m_vFullMin.x= ssx;
        m_vFullMin.y= ssy;
        m_vFullMin.z= ssz;
        m_vFullMax.x= sex;
        m_vFullMax.y= sey;
        m_vFullMax.z= sez;


        m_vFullStart = new Vector3Int(sx, sy, sz);
        m_kUseBlock.Clear();

        m_bThrFlag = true;
        ThreadStart th = new ThreadStart(ThreadFullSpace);
        Thread Thread = new Thread(th,1024*1024*1024);
        Thread.Start();

        StartCoroutine("IFullSpace");


    }

    #endregion


    void UpdateUnderResBlock()
    {
        int sx = 0;
        int sy = 0;
        int sz = 0;

        int ex = m_nWidth;
        int ey = 64;
        int ez = m_nWidth;

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);
        /*
                for (int z = sz; z <= ez; z++)
                {
                    for (int x = sx; x <= ex; x++)
                    {
                        int hh = 0;
                        for (int y = ey; y > sy; y--)
                        {
                            {
                                int nn = m_gBlockMap.GetBlock(x, y, z);
                                if (nn > 0)
                                {
                                    hh++;
                                    if (hh > 2)
                                    {

                                        int nblock = GetUnderGroundBlock(y);

                                        m_gBlockMap.UpdateBlock(x, y, z, nblock);
                                    }

                                }
                            }

                        }


                    }


                }
                */
        for (int z = sz; z <= ez; z++)
        {
            for (int x = sx; x <= ex; x++)
            {


                int h = 0;
                for (int y = ey; y > sy; y--)
                {
                    int n = m_gBlockMap.GetBlock(x, y, z);
                    if (n > 0)
                    {
                        h = y;
                        break;
                    }
                }


                for (int y = h-2; y > sy; y--)
                {
                    int nn = m_gBlockMap.GetBlock(x, y, z); 
                    if (nn>0)
                    {
                        nn = GetUnderGroundBlock(y, true);
                        if(nn>0)
                        {
                            m_gBlockMap.UpdateBlock(x, y, z, nn);
                        }
                        
                    }

                }

            }


        }
        m_gBlockMap.MakeMesh();

    }
    void UpdateUnderFullBlock(int nBlock=0)
    {
        int sx = 0;
        int sy = 0;
        int sz = 0;

        int ex = m_nWidth;
        int ey = 64;
        int ez = m_nWidth;

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        for (int z = sz; z <= ez; z++)
        {
            for (int x = sx; x <= ex; x++)
            {

                
                int h = 0;
                for(int y=ey;y>sy;y--)
                {
                    int n = m_gBlockMap.GetBlock(x, y, z);
                    if(n>0)
                    {
                        h = y;
                        break;
                    }
                }


                for (int y = h; y > sy; y--)
                {
                    int nn = nBlock;
                    if(nBlock==0)
                    {
                        nn = GetUnderGroundBlock(y, true);
                    }
                    if(nn==0)
                    {
                        nn =(int) OLDBLOC.WHITE;
                    }
                    m_gBlockMap.UpdateBlock(x, y, z, nn);
                }

            }


        }

        m_gBlockMap.MakeMesh();



    }

    void CopyBuild(int x, int y, int z, int nType)
    {


        CWAirObject[] bb = m_BuildingArrayDir[nType].GetComponentsInChildren<CWAirObject>();
        if (bb.Length == 0) return;
        int n = UnityEngine.Random.Range(0, bb.Length);
        bb[n].CopyBlockMap(x, y, z, SetBlock);



    }
    //바위  얼음 화산 사막  잔디  푸른잔디
    string ConvertLeap(string szType)
    {
        if(szType== "연두잔디")
        {
            return "잔디";
        }
        if (szType == "눈")
        {
            return "얼음";
        }
        if (szType == "메카닉")
        {
            return "사막";
        }
        if (szType == "고등잔디")
        {
            return "바위";
        }

        return szType;
    }

    void SetConvertTreeBlock(int x, int y, int z, int nBlock)
    {
        int newblock = nBlock;
        if(nBlock>=89&& nBlock<=92)
        {
            PLANETDATA kData = m_kPlanetList[m_PattenID];
            int RR = Random.Range(0, 4);
            
            List<int> ktemp = CWArrayManager.Instance.GetPatternBlocks("나뭇잎", ConvertLeap(kData.m_szPattern));
            if (RR >= ktemp.Count) RR = 0;

            newblock = ktemp[RR];
        }
        SetBlock(x, y,  z, newblock);
    }
    void CopyTreeBuild(int x, int y, int z, int nType)
    {

        int n = UnityEngine.Random.Range(0, m_Trees.Length);
        if (n >= m_Trees.Length) return;

        m_Trees[n].CopyBlockMap(x, y, z, SetConvertTreeBlock);


        //if (nType == 0)
        //{

        //}
        //else
        //{
        //    int n = UnityEngine.Random.Range(0, m_Trees2.Length);
        //    m_Trees2[n].CopyBlockMap(x, y, z, SetBlock);

        //}
    }
    void MakeTree(int sx, int sz, int ex, int ez, int nOnBlock, int nType)
    {
        for (int z = sz; z < ez; z++)
        {
            for (int x = sx; x < ex; x++)
            {
                int num = z * CWGlobal.GRIDSIZE + x;

                int h = m_gBlockMap.GetHeight(x, z);
                int nBlock = m_gBlockMap.GetBlock(x, h, z);
                if (nBlock == 0) continue;

                if (nBlock == nOnBlock)
                {
                    int rr = UnityEngine.Random.Range(0, m_TreeRate);
                    if (rr == 0)
                    {
                        CopyTreeBuild(x, h, z, nType);
                        continue;
                    }
                }

            }
        }

    }

    void TreeBatch(int nOnBlock, int nType)
    {
        int sx = 0, sy = 0, sz = 0, ex = 0, ey = 0, ez=0;
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        MakeTree(sx, sz, ex, ez, nOnBlock, nType);
        m_gBlockMap.MakeMesh();
        
    }
    void CopyAirBlock( int nWidth)
    {
        int sx = 0, sy = 0, sz = 0, ex = 0, ey = 0, ez = 0;
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        int cx = 0, cz = 0;
        int rx = sx +(ex - sx)/2;// 중심
        int rz = sz +(ez - sz)/2;// 중심

        cx = nWidth/2 - rx;
        cz = nWidth/2 - rz;


        CWAirObject kObject = null;
          kObject = m_kAirObject;
        kObject.SELLWIDTH = nWidth;
        kObject.Clear();
        for (int z = sz; z < ez; z++)
        {
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    int nBlock = m_gBlockMap.GetBlock(x, y, z);
                    if (nBlock == 0) continue;
                    {
                        int tx = x - sx;
                        int ty = y - sy;
                        int tz = z - sz;


                        int nItem = CWArrayManager.Instance.GetItemFromBlock(nBlock);
                        if (nItem == 0)
                        {
                            nItem = (int)GITEM.stone;
                        }

                        int nColor = (int)CWGlobal.GetColorItem((GITEM)nItem);
                        if (nColor > 0)
                        {
                            nItem = (int)GITEM.stone;
                        }
                        kObject.SetBlock(tx, ty, tz, nItem, 0, nColor);


                    }

                }
            }
        }
        kObject.UpdateBlock();
        string szpath = string.Format("{0}/Resources/AirCraft/{1}.bytes", Application.dataPath, kObject.name);
        kObject.Save(szpath);

    }
    #endregion
    #region LOD
    void SetLodBlock(int sx, int sy, int sz,  int nlod,int nBlock)
    {
        //int nCount = 0;
        //Dictionary<int, int> kBlocks = new Dictionary<int, int>();
        //for (int z = 0; z < nlod; z++)
        //{
        //    for (int x = 0; x < nlod; x++)
        //    {
        //        for(int y=0;y<nlod;y++)
        //        {
        //            int n=m_gBlockMap.GetBlock(sx + x, sy + y, sz + z);
        //            if(n>0)
        //            {
        //                nCount++;
        //            }
        //            if(kBlocks.ContainsKey(n))
        //            {
        //                kBlocks[n]++;
        //            }
        //            else 
        //                kBlocks.Add(n,1);
        //        }
        //    }
        //}
        //int nBlock = 0;
        //int nmax = 0;
        //foreach(var v in kBlocks)
        //{
        //    if(nmax <=v.Value)
        //    {
        //        nBlock = v.Key;
        //        nmax = v.Value;
        //    }
        //}



        for (int z = 0; z < nlod; z++)
        {
            for (int x = 0; x < nlod; x++)
            {
                for(int y=0;y<nlod;y++)
                {
                    int n =  m_gBlockMap.ConvertLODBLOK(sx + x,sz + z,nBlock);
                    m_gBlockMap.SetBlock(sx+x, sy+y, sz+z, n);
                }
            }
        }
    }

 
    void MakeLODBlockUP(int nLOD)
    {
        int sx = 0;
        int sy = 0;
        int sz = 0;

        int ex = m_nWidth;
        int ey = 64;
        int ez = m_nWidth;

        CWGlobal.g_LODWORK = true;

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        for (int z = sz; z < ez; z += nLOD)
        {
            for (int y = sy; y < ey; y += nLOD)
            {
                for (int x = sx; x < ex; x += nLOD)
                {
                    int nBlock = m_gBlockMap.GetBlock(x, y, z);
                    SetLodBlock(x, y, z, nLOD, nBlock);
                }
            }
        }
        m_gBlockMap.MakeMeshUP();


    }
    int _GetBlock(int x,int z)
    {
        
        int y = m_gBlockMap.GetHeight(x, z );
        return m_gBlockMap.GetBlock(x , y , z );
    }
    int _GetHeight(int x,int z)
    {
        
        return m_gBlockMap.GetHeight(x , z);
    }
    void MakePlan()
    {

        int nLOD = CWGlobal.LOD;
        CWPlanMesh kMesh = new CWPlanMesh();
        kMesh.Make(m_nWidth, nLOD, _GetBlock , _GetHeight);

        m_gBlockMap.Close();
        m_gBlockMap.m_gMeshObject.SetActive(true);
        MeshFilter mf = m_gBlockMap.m_gMeshObject.GetComponent<MeshFilter>();
        if (mf != null)
        {

            mf.sharedMesh = kMesh.GetMesh(); //CWMeshManager.Instance.GetMesh(szname, null);
        }


    }

    #region

    
    public Color[] m_kColorTable;
    

    // 이미지 간격 

    public float m_Colorfdist = 1f;

    public bool BBlockflag = true;
    float GetMinDist(Vector3 rr, List<Vector3> vTemp)
    {
        float fdist = 10000f;
        foreach(var v in vTemp)
        {
            float f = Vector3.Distance(rr, v);
            if(f<fdist)
            {
                fdist = f;
            }
        }
        return fdist;
    }

    void MakeColorTable()
    {

        List<Vector3> kTemp = new List<Vector3>();
        for (int y = 0; y < m_kWorldImage.height; y++)
        {
            for (int x = 0; x < m_kWorldImage.width; x++)
            {
                Color kcolor = m_kWorldImage.GetPixel(x, y);
                Vector3 vv = new Vector3(kcolor.r, kcolor.g, kcolor.b);

                float f = GetMinDist(vv, kTemp);
                if(f>= m_Colorfdist)
                {
                    kTemp.Add(vv);
                }
            }

        }
        m_kColorTable = new Color[kTemp.Count];

        PlayerPrefs.SetInt("ColorTable", kTemp.Count);

        for (int i=0;i<kTemp.Count;i++)
        {
            m_kColorTable[i] = new Color(kTemp[i].x, kTemp[i].y, kTemp[i].z);
            SaveTile(m_kColorTable[i],i+1);

            string sz1 = string.Format("{0}_1", i);
            PlayerPrefs.SetFloat(sz1, m_kColorTable[i].r);

            string sz2 = string.Format("{0}_2", i);
            PlayerPrefs.SetFloat(sz2, m_kColorTable[i].g);

            string sz3 = string.Format("{0}_3", i);
            PlayerPrefs.SetFloat(sz3, m_kColorTable[i].b);
        }
        



    }

    void SaveTile(Color kColor, int num)
    {

        Texture2D tImage = new Texture2D(32 , 32);
        for(int x=0;x<32;x++)
        {
            for (int y = 0; y < 32; y++)
            {
                tImage.SetPixel(x, y, kColor);
            }

        }

        string szPath = string.Format("{0}/Resources/Texture/tile{1}.png", Application.dataPath, num);

        Texture2D kNew = new Texture2D(tImage.width, tImage.height);
        kNew.SetPixels32(tImage.GetPixels32());
        kNew.Apply(false);
        File.WriteAllBytes(szPath, kNew.EncodeToPNG());

    }


    #endregion


    #endregion





}
