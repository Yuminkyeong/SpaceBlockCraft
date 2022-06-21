using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWUnityLib;
using System.Threading;
using System;
using CWStruct;
using DG.Tweening;

public class CWMapManager : CWSingleton<CWMapManager>
{

    static public bool g_bLight = false;//true;// 맵라이트


    public GameObject m_gHeroStartPos;
    public GameObject m_gNPCStartPos;

    public GameObject m_gWall;
    public GameObject m_gCloud;

    public MeshFilter[] m_kMeshs;

    

    public float m_fSpeed = 80;

    bool m_bChanging = false;
    static public bool m_bLoadingFlag = false;
    static public bool m_bMapDontChange = false; // 맵 체인지 금지!!


    static public int HEIGHT = 64;
    public int WIDTH = 64;

    public CWFaceMap m_kSelectMap;

    static int m_nFace = 1;

    GameObject m_gBounds;
    


    


    public Material m_kMatBound;

    const int m_nHeight = 0;

    public GameObject m_gVisable;


    public GameObject [] m_gSingleBound;
    public void SetBoundSize(float fsize)
    {
        foreach(var v in m_gSingleBound)
        {
            v.transform.localScale = new Vector3(fsize, fsize, fsize);
        }
    }


    public static CWFaceMap SelectMap
    {
        get
        {
            if (instance == null) return null;
            return instance.m_kSelectMap;
        }
    }
    static bool _bdontFight;
    public static bool BDontFight
    {

        get
        {
            return false;
        }
        set
        {
            _bdontFight = value;
        }

    }

    public void SetBound()
    {
        

        NoticeMessage.Instance.Show("더이상 가지 못합니다!");
        CWVibration.Vibrate(100);

        m_gBounds.SetActive(true);
        m_kMatBound.DOFade(0, 3f).OnComplete(() => {
            m_gBounds.SetActive(false);
            m_kMatBound.DOFade(1, 0f);
        });
    }

    void OnLoadEnd()
    {
        m_bLoadingFlag = true;// 바꾸기 금지  유저가 움직인 다음 부터 바꾸기 시작 한다
        CWUserManager.Instance.Clear();
       
        CWAIOwnerManager.Instance.Clear();


        //BuildStage[] bArray = m_kSelectMap.m_ObjectDir.GetComponentsInChildren<BuildStage>();
        //CWUserBuildManager.Instance.Begin(bArray);

        
        m_kSelectMap.Select();


    }
    




   
    

    const int MAXVERTEX = 355560; //10배 
    const int MAXINDEX = 355560;

    




    // 카메라 모드로 나중 작업
    // 선택이 되면 ,회전을 하지 않고 바로 
    private void Start()
    {
        m_gVisable.SetActive(false);
        


    }
    
    


    

    bool m_bReceiveflag = false;
    bool m_bResultflag = false;
    void ReceiveResult(JObject jData)
    {
        m_bReceiveflag = true;
        if (jData["Result"].ToString() == "ok")
        {
            m_bResultflag = true;
        }
        else
        {
            m_bResultflag = false;
        }

    }

    public void Close()
    {
      

        if(m_kSelectMap!=null)
        {
            m_kSelectMap.Close();
        }
        CWMobManager.Instance.Close();
        m_gVisable.SetActive(false);
    }
    
    public static Vector3Int ConvertPos(Vector3 vPos)
    {
  //      if(CWGlobal.g_bEditmode)
        {
            return new Vector3Int((int)vPos.x, (int)vPos.y, (int)vPos.z);
        }
/*
        if (Instance == null) return Vector3Int.zero;
        float fyaw = Instance.transform.eulerAngles.y;
        Quaternion qq = Quaternion.Euler(0, -fyaw, 0);
        vPos.z += 0.5f;
        vPos.x += 0.5f;
        Vector3 v = qq * vPos;
        vPos = v;

        Vector3Int vRet = Vector3Int.zero;
        vRet.x = (int)(vPos.x + 128);
        vRet.y = (int)(vPos.y);
        vRet.z = (int)(vPos.z + 128);
        return vRet;
*/
    }
    public static Vector3 ConvertPosFloat(Vector3Int vPos)
    {
    //    if (CWGlobal.g_bEditmode)
        {
            return new Vector3((int)vPos.x, (int)vPos.y, (int)vPos.z);
        }
/*

        Vector3 vRet = Vector3.zero;
        vRet.x = (vPos.x - 128);
        vRet.y = (vPos.y - 0);
        vRet.z = (vPos.z - 128);


        float fyaw = Instance.transform.eulerAngles.y;
        Quaternion qq = Quaternion.Euler(0, fyaw, 0);
        Vector3 v = qq * vRet;

        return v;
*/
    }


    #region 맵 로딩 

    public GameObject m_gSingle;
    public GameObject m_gMyRoom;
    public GameObject m_gMulti;
    public GameObject m_gPVP;

    //0 스토리 행성, 1 : 멀티 행성 2: 1:1 대전  3 : 내행성
    void SetGround()
    {
        if (Space_Map.Instance.m_nPlanetType == 1)
        {
            m_gMyRoom.SetActive(false);
            m_gSingle.SetActive(false);
            m_gPVP.SetActive(false);
            m_gMulti.SetActive(true);

            m_gBounds = CWLib.FindChild(m_gMulti, "BoundBox");
            
        }
        else if (Space_Map.Instance.m_nPlanetType == 2)
        {
            m_gMyRoom.SetActive(false);
            m_gSingle.SetActive(false);
            m_gPVP.SetActive(true);
            m_gMulti.SetActive(false);
            m_gBounds = CWLib.FindChild(m_gPVP, "BoundBox");
        }
        else if (Space_Map.Instance.m_nPlanetType == 3)
        {
            m_gMyRoom.SetActive(true);
            m_gSingle.SetActive(false);
            m_gPVP.SetActive(false);
            m_gMulti.SetActive(false);
            m_gBounds = CWLib.FindChild(m_gMyRoom, "BoundBox");

        }
        else
        {
            m_gSingle.SetActive(true);
            m_gMulti.SetActive(false);
            m_gPVP.SetActive(false);
            m_gMyRoom.SetActive(false);
            m_gBounds = CWLib.FindChild(m_gSingle, "BoundBox");
        }

        m_gBounds.SetActive(false);
    }

    public void LoadMap(int nID)
    {

        SetGround();
        m_gVisable.SetActive(true);
        m_gVisable.transform.rotation = new Quaternion();
        //m_gBound.SetActive(false);

        m_kSelectMap.Load(nID);
        m_kSelectMap.Select();
        // 나머지 5면을 LOD로 채운다 

        int nPlanet = CWHeroManager.Instance.m_nPlanetID;
        if (nPlanet == 0) nPlanet = 1;

        for (int i = 0; i < 5; i++)
        {
            int nFace = ((nPlanet - 1) * 6 + i) + 1;// 1부터 시작 
            int nMapID = CWArrayManager.Instance.GetMapID(nFace);
            string szfile = nMapID.ToString();
            m_kMeshs[i].sharedMesh = CWMeshManager.Instance.GetMesh(nMapID);// CWResourceManager.Instance.GetMeshAsset(szfile);
        }
        
        
        



    }
    // 
    public void LoadLocalMap(string szFile)
    {
        SetGround();
        m_gVisable.SetActive(true);
        m_gVisable.transform.rotation = new Quaternion();
        //m_gBound.SetActive(false);
        // 클라이언트에 파일 존재하는가?
        m_kSelectMap.LoadLocalMap(szFile);
        m_kSelectMap.Select();

    }




    #endregion
    #region 커서
    static public Vector3Int m_vCursor = Vector3Int.zero;
    private void Update()
    {
        //if (SelectMap == null) return;
        //Vector3 vPos = Vector3.zero;
        //int nMask = (1 << 11);// 맵만 
        //RaycastHit hit;
        //Vector3 vv = Input.mousePosition;
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(vv), out hit, Mathf.Infinity, nMask))
        //{
        //    vPos = SelectMap.SelectPos(hit.point, hit.normal);
        //    m_vCursor = ConvertPos(vPos);
        //}

    }
    #endregion

    #region 마스크 
    public float m_fAlpha = 0.7f;
    public float m_fMaskAlpha = 0.0f;
    public float m_fHeroSight = 32;// 주인공 시야

    public Material m_kMaterial;
    public Material m_kLodMaterial;
    Texture2D m_kMask;

    Color[] m_kColorBuffer;

    // 마스크를 없앤다 
    public void ClearMask()
    {
        m_kMaterial.SetTexture("_DecalTex", null);
    }
    // 마스트를 다시 씌운다 
    public void ReStartMask()
    {
        m_kMaterial.SetTexture("_DecalTex", m_kMask);
    }
    public void CreateMask()
    {
        if (!g_bLight) return;
        int nWidth = m_kSelectMap.WORLDSIZE;


        m_kColorBuffer = new Color[nWidth * nWidth];

        Color kColor = new Color(0, 0, 0, m_fAlpha);
        for (int i = 0; i < nWidth * nWidth; i++)
        {
            m_kColorBuffer[i] = kColor;
        }
        m_kMask = new Texture2D(nWidth, nWidth);
        m_kMaterial.SetTexture("_DecalTex", m_kMask);

        m_kMask.SetPixels(m_kColorBuffer);
        m_kMask.Apply(false);

        m_kMaterial.SetFloat("radius", m_fHeroSight);
        m_bHeroLight = true;
    }
    public bool GetMask(int x, int z)
    {
        if (x < 0) return false;
        if (z < 0) return false;
        if (x >= m_kSelectMap.WORLDSIZE) return false;
        if (z >= m_kSelectMap.WORLDSIZE) return false;
        int num = z * m_kSelectMap.WORLDSIZE + x;
        if (m_kColorBuffer[num].a < 0.7f) return true;
        return false;

    }
    void _UpdateMask(int cx, int cz, int nRadius)
    {
        if (nRadius == 0) return;
        int RR = (nRadius / 2) * (nRadius / 2);
        for (int z = -nRadius; z <= nRadius; z++)
        {
            for (int x = -nRadius; x <= nRadius; x++)
            {
                int tx = x + cx;
                int tz = z + cz;
                if (tx < 0) continue;
                if (tz < 0) continue;
                if (tx >= m_kSelectMap.WORLDSIZE) continue;
                if (tz >= m_kSelectMap.WORLDSIZE) continue;
                int rr = x * x + z * z;
                if (rr > RR) continue;
                int num = tz * m_kSelectMap.WORLDSIZE + tx;
                //m_kColorBuffer[num].a = Mathf.Min(m_kColorBuffer[num].a, ((float)rr / RR)* m_fAlpha);//최저 0.7, 최고 1 

                m_kColorBuffer[num].a = m_kColorBuffer[num].a * ((float)rr / RR);


            }


        }

    }
    public bool m_bUpdateBuildMask = false;
    void MakeBuildMask()
    {
       

    }

    #endregion

    #region 광원

    public bool m_bHeroLight = true;
    Vector2 m_vLight = new Vector2();

    public void SetLight(float x, float z)
    {
        m_vLight.x = x;
        m_vLight.y = z;
    }
    private void FixedUpdate()
    {


        //        if (CWGlobal.g_bEditmode) return;

        if (!g_bLight) return;

        if (m_kColorBuffer == null) return;
        if (CWHero.Instance == null) return;

        if (m_bUpdateBuildMask)
        {
            MakeBuildMask();
            m_bUpdateBuildMask = false;
        }
        if (m_bHeroLight)
        {
            m_kMaterial.SetFloat("herox", CWHero.Instance.GetPosX());
            m_kMaterial.SetFloat("heroz", CWHero.Instance.GetPosZ());

        }
        else
        {
            m_kMaterial.SetFloat("herox", m_vLight.x);
            m_kMaterial.SetFloat("heroz", m_vLight.y);

        }


    }
    #endregion

    #region 회전
    /*
     * 회전 개념 1,2,3,4,5,6
     * 1에서 2로 회전 2에서 3으로 회전
     * 회전 방향 동일
     * 회전맵을 맞춘다
     * */

    Vector3[] VRotate =
    {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 90),
            new Vector3(90, 0, 0),
            new Vector3(0, 0, 270),
            new Vector3(270, 0, 0),
            new Vector3(180, 0, 0),
    };
    public void Rotate(int nFace)
    {

        if (nFace < 0)
        {
            Debug.Log("");
            return;
        }
        if (nFace >= 6)
        {
            Debug.Log("");
            return;
        }
        GameObject gg = m_gVisable;
        if (gg == null) return;
        Quaternion qq = Quaternion.Euler(VRotate[nFace]);
        m_gVisable.transform.DORotateQuaternion(qq, 2);



    }

    public void Rotate()
    {
        int nStage=CWHeroManager.Instance.GetNextStage();
        CWArrayManager.StageData kStage = CWArrayManager.Instance.GetStageData(Space_Map.Instance.GetStageID());

        if(kStage.m_nMapID>0)
        {
            string szfile = kStage.m_nMapID.ToString();
            m_kMeshs[0].sharedMesh = CWMeshManager.Instance.GetMesh(kStage.m_nMapID);// CWResourceManager.Instance.GetMeshAsset(szfile);

        }


        Renderer rr=m_kMeshs[0].GetComponent<Renderer>();
        rr.material.color = Color.white;
        Rotate(1);

    }

    public void SetCloud(bool bflag)
    {
        if (CWGlobal.g_SystemState == CWGlobal.SYSTEMSTATE.BAD) m_gCloud.SetActive(false);
        else       m_gCloud.SetActive(bflag);
    }

    #endregion

}
