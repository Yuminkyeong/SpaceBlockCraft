using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CWUnityLib;
using DG.Tweening;

using CWEnum;
using CWStruct;
/// <summary>
/// 개념 정리 
/// 은하계 시스템에 의해서 만들어지고
/// 각각의 
/// 
/// </summary>


public class CWStar : MonoBehaviour
{

    CallBackFunction CBBackFunction;
    const float LEVELDIST_1 = 80000f;
    const float LEVELDIST_2 = 500000f;
    public CWPlanet[] m_gPlanet; //6개의 행성 


    [HideInInspector] public GameObject m_gTitlePos;
    [HideInInspector] public GameObject m_gGalxyPos;



    public Material[] m_kPtMaterial;


    public float m_fTrailTime=30;
    public float m_fWidth;

    public int m_nLevel;

    public int m_nSolaID = 1;
    public int m_nSelectPlanet = 0;

    public bool m_bStarted = false;


    private void Awake()
    {
        m_gPlanet = gameObject.GetComponentsInChildren<CWPlanet>();
        for(int i=0;i< m_gPlanet.Length; i++)
        {
            m_gPlanet[i].m_nNumber = i;
            TrailRenderer ts = m_gPlanet[i].GetComponent<TrailRenderer>();
            if (ts == null) continue;
            ts.material = m_kPtMaterial[i];
        }


        m_gTitlePos = CWLib.FindChild(gameObject, "TitlePos");
        m_gGalxyPos = CWLib.FindChild(gameObject, "GalaxyPos");

    }
    private void Start()
    {
        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach(var v in tt)
        {
            v.enabled = false;
            //v.time = m_fTrailTime;
        }
        StartCoroutine("IRun");

    }
    float GetMainCamDist()
    {
        float fDist;
        fDist = Vector3.Distance(transform.position, Camera.main.transform.position);
        return fDist;
    }
    void ResetRun()
    {
        RotateAround[] tt = gameObject.GetComponentsInChildren<RotateAround>();
        foreach (var v in tt)
        {
            v.m_fSpeed = Random.Range(5f, 15f);
            v.ResetPlay();
        }

    }
    void FastRun()
    {
        RotateAround[] tt = gameObject.GetComponentsInChildren<RotateAround>();
        foreach (var v in tt)
        {
            v.m_fSpeed = Random.Range(50f, 150f);
            v.ResetPlay();
        }
        Invoke("ResetRun",0.8f);

    }
    void StartTrail()
    {
        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.enabled = true;
            v.time = m_fTrailTime;
        }

    }
    private void OnEnable()
    {
     
    }
    IEnumerator IRun()
    {
        
        FastRun();
        yield return null;
        StartTrail();
        yield return new WaitForSeconds(1f);
        ResetRun();
        

    }
    
    // 행성들을 형성한다
    public void Create()
    {
        int nPlanetID = (m_nSolaID-1)*6+1;//
        m_nSelectPlanet = 0; 
        //1 부터 온다 
        for (int i=0;i<6;i++)
        {
            m_gPlanet[i].Setting(nPlanetID + i);
        }

    }

    #region 우주 지도 

    void CloseFuc()
    {
        Game_App.Instance.g_bDirecting = false;

        if (gameObject.activeSelf == false) return;

        StartCoroutine("IRotatePlanet");
    }

    public float planetSpeed = 0.5f;

    IEnumerator IRotatePlanet()
    {
        while(true)
        {

            Vector3 vEuler= m_gPlanet[m_nSelectPlanet].transform.eulerAngles;
            vEuler.y += Time.deltaTime* planetSpeed;
            m_gPlanet[m_nSelectPlanet].transform.eulerAngles = vEuler;
            yield return null;
        }
    }

    public enum SPACEMAP {TITLE,PLANET,SOLA,GALAXY,MULTIPLANET,PLAYMAP,PVPPLANET,MYPLANET };

    SPACEMAP m_SpaceMapType;
    // 타이틀, 행성지도,태양계지도,은하계지도
    void ChangePlanet()
    {

         
            MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
            mm.m_bHeroStop = true;
            mm.m_gEndObject = m_gPlanet[m_nSelectPlanet].m_gMapCameraPos;
            mm.m_fLifetime = 2f;
            mm.m_bLookTarget = false;

            mm.CBCloseFuc = CloseFuc;
            Game_App.Instance.g_bDirecting = true;
        
        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }

        Game_App.Instance.m_gSelect = m_gPlanet[m_nSelectPlanet].gameObject;

        if (CBBackFunction != null)
        {
            CBBackFunction();
            CBBackFunction = null;
        }


    }
    void ChangeMyPlanet()
    {
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        mm.m_bHeroStop = true;
        MyPlanet gMultiPlanet = GetComponentInChildren<MyPlanet>();
        mm.m_gEndObject = gMultiPlanet.m_gMapCameraPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        mm.CBCloseFuc = CloseFuc;
        Game_App.Instance.g_bDirecting = true;

        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }

        Game_App.Instance.m_gSelect = gMultiPlanet.gameObject;

        if (CBBackFunction != null)
        {
            CBBackFunction();
            CBBackFunction = null;
        }

    }
    void ChangePVPPlanet()
    {
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        //mm.m_gTargetObject =; 지정안하면, 자신 오브젝트
        //mm.m_gStartObject = ;// 지정안하면, 자신 오브젝트 
        mm.m_bHeroStop = true;
        PvpPlanet gMultiPlanet = GetComponentInChildren<PvpPlanet>();
        mm.m_gEndObject = gMultiPlanet.m_gMapCameraPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        mm.CBCloseFuc = CloseFuc;
        Game_App.Instance.g_bDirecting = true;



        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }

        Game_App.Instance.m_gSelect = gMultiPlanet.gameObject;
        if (CBBackFunction != null)
        {
            CBBackFunction();
            CBBackFunction = null;
        }


    }
    void ChangeMultiPlanet()
    {
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        //mm.m_gTargetObject =; 지정안하면, 자신 오브젝트
        //mm.m_gStartObject = ;// 지정안하면, 자신 오브젝트 
        mm.m_bHeroStop = true;
        MultiPlanet gMultiPlanet = GetComponentInChildren<MultiPlanet>();
        mm.m_gEndObject = gMultiPlanet.m_gMapCameraPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        mm.CBCloseFuc = CloseFuc;
        Game_App.Instance.g_bDirecting = true;
        


        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }

        Game_App.Instance.m_gSelect = gMultiPlanet.gameObject;
        if (CBBackFunction != null)
        {
            CBBackFunction();
            CBBackFunction = null;
        }


    }
    void ChangeSola()
    {
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        //mm.m_gTargetObject =; 지정안하면, 자신 오브젝트
        //mm.m_gStartObject = ;// 지정안하면, 자신 오브젝트 
        mm.m_bHeroStop = true;
        mm.m_gEndObject = m_gPlanet[m_nSelectPlanet].m_gMapCameraPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        mm.CBCloseFuc = CloseFuc;
        Game_App.Instance.g_bDirecting = true;
        


        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }

        Game_App.Instance.m_gSelect = m_gPlanet[m_nSelectPlanet].gameObject;
    }

    public void SetMap(SPACEMAP kType,CallBackFunction func= null)
    {

        m_SpaceMapType = kType;
        StopCoroutine("IRotatePlanet");
        CBBackFunction = func;
        GameObject gSun = CWLib.FindChild(gameObject, "Sun");
        if (kType == SPACEMAP.PLAYMAP)
        {
            gSun.SetActive(false);

            RotateAround[] rr = gameObject.GetComponentsInChildren<RotateAround>();
            foreach (var v in rr)
            {
                v.m_fSpeed = Random.Range(0.1f, 1f);
            }
            TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
            foreach (var v in tt)
            {
                v.Clear();
            }

        }
        else
        {
            gSun.SetActive(true);
            if (kType == SPACEMAP.TITLE)
            {
                AxisRotate aa = m_gPlanet[m_nSelectPlanet].GetAxixRotate();
                aa.enabled = true; // 행성 정지 
                ResetRun();
                foreach (var v in m_gPlanet)
                {
                    v.SetMap();
                }

            }
            // 
            if (kType == SPACEMAP.PLANET)
            {
                // 행성 정렬
                m_gPlanet[m_nSelectPlanet].ResetRotate();
                AxisRotate aa = m_gPlanet[m_nSelectPlanet].GetAxixRotate();
                aa.enabled = false; // 행성 정지 

                for(int i=0;i<m_gPlanet.Length;i++)
                {
                    RotateAround rr= m_gPlanet[i].GetComponentInChildren<RotateAround>();
                    if(i== m_nSelectPlanet)
                    {
                        //rr.m_fSpeed = 500f;
                        //rr.SetAngle(120, ChangePlanet);
                        rr.ResetAngle(120, ChangePlanet);

                    }
                    else
                    {
                        rr.m_fSpeed = 10f;
                        rr.SetAngle(120, null);
                    }
                }
                EventPlanet[] gMultiPlanet = GetComponentsInChildren<EventPlanet>();

                foreach(var v in gMultiPlanet)
                {
                    RotateAround r2 = v.GetComponent<RotateAround>();
                    r2.m_fSpeed = 5f;
                }
                foreach (var v in m_gPlanet)
                {
                    v.SetMap();
                }


            }
            if (kType == SPACEMAP.MULTIPLANET)
            {
                ResetRun();
                // 행성 정렬
                MultiPlanet gMultiPlanet = GetComponentInChildren<MultiPlanet>();
                gMultiPlanet.transform.rotation = new Quaternion();

                gMultiPlanet.SetMap();
                RotateAround rr = gMultiPlanet.GetComponent<RotateAround>();
                rr.ResetAngle(120, ChangeMultiPlanet);

            }
            if (kType == SPACEMAP.PVPPLANET)
            {
                ResetRun();
                // 행성 정렬
                PvpPlanet gMultiPlanet = GetComponentInChildren<PvpPlanet>();
                gMultiPlanet.transform.rotation = new Quaternion();

                gMultiPlanet.SetMap();
                RotateAround rr = gMultiPlanet.GetComponent<RotateAround>();
                rr.ResetAngle(120, ChangePVPPlanet);

             

            }
            if (kType == SPACEMAP.MYPLANET)
            {
                ResetRun();
                // 행성 정렬
                MyPlanet gMultiPlanet = GetComponentInChildren<MyPlanet>();
                gMultiPlanet.transform.rotation = new Quaternion();

                gMultiPlanet.SetMap();
                RotateAround rr = gMultiPlanet.GetComponent<RotateAround>();
                rr.ResetAngle(120, ChangeMyPlanet);


            }

            // 행성들이 일렬로 정렬해야 한다
            // 선택 가능하게 
            if (kType == SPACEMAP.SOLA)
            {
                // 행성 정렬
                if(m_nSelectPlanet<0 || m_nSelectPlanet>=6)
                {
                    Debug.Log("머냐?");
                    return;
                }
                m_gPlanet[m_nSelectPlanet].transform.rotation = new Quaternion();
                AxisRotate aa = m_gPlanet[m_nSelectPlanet].GetAxixRotate();
                aa.enabled = false; // 행성 정지 

                RotateAround[] rr = gameObject.GetComponentsInChildren<RotateAround>();
                foreach (var v in rr)
                {
                    v.m_fSpeed = 100f;
                    v.SetAngle(120, ChangeSola);
                }

                foreach (var v in m_gPlanet)
                {
                    v.SetMap();
                }


            }
            // 
            if (kType == SPACEMAP.GALAXY)
            {
                AxisRotate aa = m_gPlanet[m_nSelectPlanet].GetAxixRotate();
                aa.enabled = true; // 행성 정지 
                ResetRun();


                foreach(var v in m_gPlanet)
                {
                    v.SetMap();
                }
                //m_gPlanet

            }

        }
    }

    #endregion


    public GameObject GetSelectPlanet()
    {
        return m_gPlanet[m_nSelectPlanet].gameObject;
    }
    public GameObject GetMultiPlanet()
    {
        MultiPlanet gMultiPlanet = GetComponentInChildren<MultiPlanet>();
        return gMultiPlanet.gameObject;

    }
    public GameObject GetPVPPlanet()
    {
        PvpPlanet gMultiPlanet = GetComponentInChildren<PvpPlanet>();
        return gMultiPlanet.gameObject;

    }
    public GameObject GetMyPlanet()
    {
        MyPlanet gMultiPlanet = GetComponentInChildren<MyPlanet>();
        return gMultiPlanet.gameObject;
    }
    public void UpdateSelectPlanet()
    {

        if(m_SpaceMapType==SPACEMAP.PLANET)
        {
            m_gPlanet[m_nSelectPlanet].UpdateSelectPlanet();
        }
        else
        {
            MyPlanet gMultiPlanet = GetComponentInChildren<MyPlanet>();
            gMultiPlanet.UpdateSelectPlanet();
        }
        
    }

}
