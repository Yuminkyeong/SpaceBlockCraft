using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using CWStruct;
// 데이타 정보를 이용해서 만든다

public class CWSolaSystem : MonoBehaviour
{

    public int m_nSola;// 태양계 번호 0이면 내꺼, 1이면 멀티행성, 나머지는 유저아이디 
    public int m_nLayer;
    public Transform m_tdir;
    public GameObject m_gPlanet;
    //별은  랜덤하게 생성한다

    public Renderer RObject;
    public Renderer PObject;

    public SOLADATA m_kSoladata;

    public void Load(Material flare, Material sun,  SOLADATA kSola)
    {
        PObject.material = flare;
        RObject.material = sun;

        int RR = kSola.m_nCount;
        for (int i = 0; i < RR; i++)
        {

            GameObject gChild = Instantiate(m_gPlanet);
            gChild.transform.parent = m_tdir;
            gChild.transform.localPosition = kSola.m_kPlanet[i].m_vPos;
            gChild.transform.localScale = m_gPlanet.transform.localScale;
            gChild.transform.rotation = m_gPlanet.transform.rotation;
        }

    }
    public void Create(Material flare,Material sun,int level,ref SOLADATA kSola, List<CWArrayManager.StageData> kList)
    {
        PObject.material = flare;
        RObject.material = sun;

        int RR = kList.Count;

        {
            
            kSola.m_kPlanet = new PLANETDATA[RR];
            kSola.m_nCount = RR;
            for (int i = 0; i < RR; i++)
            {
                int pnumber = kList[i].m_nPlanetID;
               
                GameObject gChild = Instantiate(m_gPlanet);
                float x, z;
                x = Random.insideUnitCircle.x * 10f;
                z = Random.insideUnitCircle.y * 10f;
                gChild.transform.parent = m_tdir;
                gChild.transform.localPosition = new Vector3(x, 0, z);
                gChild.transform.localScale = m_gPlanet.transform.localScale;
                gChild.transform.rotation = m_gPlanet.transform.rotation;

                gChild.name = "Planet_" + pnumber.ToString();


                kSola.m_kPlanet[i] = new PLANETDATA();
                kSola.m_kPlanet[i].m_vPos= gChild.transform.localPosition;
                kSola.m_kPlanet[i].m_nID = pnumber;


            }
        }

    }



}
