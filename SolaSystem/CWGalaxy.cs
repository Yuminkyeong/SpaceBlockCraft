using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using CWUnityLib;
using CWStruct;
using CWEnum;
public class CWGalaxy : CWSingleton<CWGalaxy>
{

    
    


    // 개념 4개의 태양이 존재 
    public CWStar[] m_kStarList;

    public CWStar m_kSelectStar;

    CWStar.SPACEMAP m_SelectType;


    public int GetSolaID()
    {
        return m_kSelectStar.m_nSolaID;
    }
    // 태양을 선택한다
    public void SelectSola(int nSolaID)
    {
        if (nSolaID <= 0) return;
        if (nSolaID > m_kStarList.Length) return;

        int  nSoloNumber = (nSolaID - 1); //

        m_kSelectStar = m_kStarList[nSoloNumber];
        m_kSelectStar.Create();


    }
    private void Start()
    {
        for(int i=0;i< m_kStarList.Length;i++)
        {
            m_kStarList[i].m_nSolaID = i + 1;

        }
        m_kSelectStar = m_kStarList[0];
    }

    public void Show(bool bflag)
    {
        foreach (var v in m_kStarList)
        {
            v.gameObject.SetActive(bflag);
        }
    }

    // 전투중에 배치되는 별 
    public void OnPlayMap()
    {
        
        Show(false);
    }
    public void OnTitleMap()
    {
        Game_App.Instance.g_bDirecting = true;

        Show(true);
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();

        Game_App.Instance.g_bDirecting = true;
        

        mm.m_bHeroStop = true;
        mm.m_gEndObject = m_kSelectStar.m_gTitlePos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;


        m_SelectType = CWStar.SPACEMAP.TITLE;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.TITLE);
        

        Camera cam = Camera.main;
        cam.fieldOfView = 60;
    }
    public void OnMultiPlanetMap()
    {
        Show(true);
        m_SelectType = CWStar.SPACEMAP.MULTIPLANET;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.MULTIPLANET);
        // 지정된 행성 정지 
        // 카메라 워킹 
        

        Camera cam = Camera.main;
        cam.fieldOfView = 60;

    }

    public void OnPVPPlanetMap()
    {
        Show(true);
        m_SelectType = CWStar.SPACEMAP.PVPPLANET;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.PVPPLANET);
        // 지정된 행성 정지 
        // 카메라 워킹 


        Camera cam = Camera.main;
        cam.fieldOfView = 60;

    }
    public void OnMYPlanetMap()
    {
        Show(true);
        m_SelectType = CWStar.SPACEMAP.MYPLANET;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.MYPLANET);
        // 지정된 행성 정지 
        // 카메라 워킹 


        Camera cam = Camera.main;
        cam.fieldOfView = 60;

    }

    public void OnPlanetMap(CallBackFunction Closefunc)
    {
        Show(true);
        // 현재 지정된 행성을 가르킨다 
        // 테스트는 첫번째 행성
        //m_kSelectStar.m_gPlanet[0];

        // 행성 정렬 
        m_SelectType = CWStar.SPACEMAP.PLANET;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.PLANET, Closefunc);

        // 지정된 행성 정지 
        // 카메라 워킹 

        
        Camera cam = Camera.main;
        cam.fieldOfView = 60;


    }
    public void OnSolaMap(CallBackFunction func=null)
    {

        

        Show(true);
        m_SelectType = CWStar.SPACEMAP.SOLA;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.SOLA, func);

        
        Camera cam = Camera.main;
        cam.fieldOfView = 60;

    }
    public void OnGalaxyMap()
    {
        Show(true);
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();


        Game_App.Instance.g_bDirecting = true;
        

        mm.m_bHeroStop = true;
        mm.m_gEndObject = m_kSelectStar.m_gGalxyPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        m_SelectType = CWStar.SPACEMAP.GALAXY;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.GALAXY);
        

        Camera cam = Camera.main;
        cam.fieldOfView = 60;

    }
    // 행성 이동 
    public void OnPlanetLeft()
    {
        m_kSelectStar.m_nSelectPlanet--;
        if (m_kSelectStar.m_nSelectPlanet < 0) m_kSelectStar.m_nSelectPlanet = 0;
        OnSolaMap();
    }
    public void OnPlanetRight()
    {
        m_kSelectStar.m_nSelectPlanet++;
        if (m_kSelectStar.m_nSelectPlanet >= 5) m_kSelectStar.m_nSelectPlanet = 5;
        OnSolaMap();
    }
    public bool CheckPlanet()
    {

        Vector3 v2 = Camera.main.transform.position;

        int nPlanet = -1;
        float fcheck = 0f;
        CWPlanet [] aa = gameObject.GetComponentsInChildren<CWPlanet>();
        for(int i=0;i<aa.Length;i++)
        {
            Vector3 v1= aa[i].transform.position;
            float fdist = Vector3.Distance(v1, v2);
            if(fcheck==0||fcheck>fdist)
            {
                nPlanet = i;
                fcheck = fdist;
            }
        
        }
        if (m_kSelectStar.m_nSelectPlanet == nPlanet) return false;
        m_kSelectStar.m_nSelectPlanet = nPlanet;
        OnSolaMap();

        return true;
    }
    // 지역 이동
    #region 지역 회전
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
            
            return;
        }
        if (nFace >= 6)
        {
           
            return;
        }
        //PVPPLANET,MYPLANET 
        if (m_SelectType == CWStar.SPACEMAP.MULTIPLANET)
        {
            GameObject gg = m_kSelectStar.GetMultiPlanet();
            if (gg == null) return;
            BasePlanet pp = gg.GetComponent<BasePlanet>();
            pp.Rotate(VRotate[nFace]);

        }
        //else if(m_SelectType == CWStar.SPACEMAP.PVPPLANET)
        //{
        //    GameObject gg = m_kSelectStar.GetPVPPlanet();
        //    if (gg == null) return;
        //    BasePlanet pp = gg.GetComponent<BasePlanet>();
        //    pp.Rotate(VRotate[nFace]);

        //}
        //else if (m_SelectType == CWStar.SPACEMAP.MYPLANET)
        //{
        //    GameObject gg = m_kSelectStar.GetMyPlanet();
        //    if (gg == null) return;
        //    BasePlanet pp = gg.GetComponent<BasePlanet>();
        //    pp.Rotate(VRotate[nFace]);

        //}
        else
        {
            GameObject gg = m_kSelectStar.GetSelectPlanet();
            if (gg == null) return;
            CWPlanet pp = gg.GetComponent<CWPlanet>();
            pp.Rotate(VRotate[nFace]);

        }



    }


    #endregion
    // 은하계 이동 

    #region  카메라 이동 새롭게 적용

    
    // 해당 행성을 바라 본다
    public void LookPlanet(int nPlanet,Action cbClose)
    {

        int nStar = nPlanet / 6;
        CWPlanet pp = m_kStarList[nStar].m_gPlanet[nPlanet % 6];
        if (pp == null) return;
        m_kStarList[nStar].m_nSelectPlanet = nPlanet % 6;
        // 멈추고, 카메라를 위치 시킨다
        pp.LookPlanet(cbClose);

    }


    #endregion

    public void UpdateSelectPlanet()
    {

        m_kSelectStar.UpdateSelectPlanet();
    }


}
