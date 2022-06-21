using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGalaxy : MonoBehaviour
{

    public CWStar m_gStar;
    public GameObject[] m_gSunParticle;
    public GameObject m_gGalaxy;

    public float MAXDIST = 100000f;
    public int COUNT = 50;
    public float SOLADIST = 100000;// 8000 * 6;// 태양계 크기 

    GameObject m_gDir;
    public GameObject m_gTitlePos;
    public GameObject m_gGalxyPos;

    public CWStar m_kSelectStar;
    float GetMainCamDist()
    {
        float fDist;
        fDist = Vector3.Distance(transform.position, Camera.main.transform.position);
        return fDist;
    }

    //private void OnGUI()
    //{
    //    if(GUI.Button(new Rect(0,0,100,100),"Make"))
    //    {
    //        MakeGalaxy();
    //    }
    //   /// GUI.Label(new Rect(100, 100, 500, 150), string.Format("레벨 : {0}  거리 = {1}", m_gStar.m_nLevel,  GetMainCamDist()));
    //}

    public void OnTest()
    {
        RotateAround[] rr = m_kSelectStar.gameObject.GetComponentsInChildren<RotateAround>();
        foreach (var v in rr)
        {
            v.m_fSpeed = 100f;
            v.SetAngle(120, null);
        }

    }

    public void OnTitleMap()
    {
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        mm.m_gEndObject = m_gTitlePos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.TITLE);
    }
    public void OnPlanetMap()
    {
        // 현재 지정된 행성을 가르킨다 
        // 테스트는 첫번째 행성
        //m_kSelectStar.m_gPlanet[0];

        // 행성 정렬 
        m_kSelectStar.SetMap(CWStar.SPACEMAP.PLANET);

        // 지정된 행성 정지 
        // 카메라 워킹 


    }
    public void OnSolaMap()
    {
        m_kSelectStar.SetMap(CWStar.SPACEMAP.SOLA);
    }
    public void OnGalaxyMap()
    {
        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        mm.m_gEndObject = m_gGalxyPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;
        m_kSelectStar.SetMap(CWStar.SPACEMAP.GALAXY);

    }
    
    public void OnLeft()
    {
        m_kSelectStar.m_nSelectPlanet--;
        if (m_kSelectStar.m_nSelectPlanet < 0) m_kSelectStar.m_nSelectPlanet = 0;
        OnSolaMap();
    }
    public void OnRight()
    {
        m_kSelectStar.m_nSelectPlanet++;
        if (m_kSelectStar.m_nSelectPlanet >= 5) m_kSelectStar.m_nSelectPlanet = 5;
        OnSolaMap();

    }
}
