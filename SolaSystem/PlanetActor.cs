using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

/*
 * 캐릭터가 움직인다. 
 * */


public class PlanetActor : MonoBehaviour
{
    public float Scale = 1;
    Vector3 Pos=new Vector3(-128,180,-128);
    Vector3 MyPos = new Vector3(-312, 120, -336);

    public Vector3 m_vPos ;


    DOTweenPath m_gPath;
    string GetPath()
    {
        int rr = Random.Range(1, 6);
        //return "Hero_" + rr.ToString();
        return "Hero_1";
    }
    Vector3 m_vStartPos=Vector3.zero;
    private void OnEnable()
    {
        if (CWHero.Instance == null) return;
        if (CWResourceManager.Instance==null) return;
        if (Game_App.Instance.m_gSelect == null) return;
        string szPath = GetPath();
        m_gPath = CWResourceManager.Instance.GetPath(szPath,1);
        m_gPath.DOPlay();
        CWHero.Instance.Show(true);

        SphereCollider [] ss = CWHero.Instance.gameObject.GetComponentsInChildren<SphereCollider>();
        foreach(var v in ss)
        {
            v.enabled = false;
        }

        CWHero.Instance.SetRandomDance();
        m_vStartPos = Game_App.Instance.m_gSelect.transform.position;
        //0 스토리 행성, 1 : 멀티 행성 2: 1:1 대전  3 : 내행성
        if (Space_Map.Instance.m_nPlanetType== 3)
        {
            m_vPos = MyPos;
        }
        else
        {
            m_vPos = Pos;
        }


        
        //Scale =;




    }
    private void OnDisable()
    {
        if (CWHero.Instance == null) return;

        CWHero.Instance.transform.localScale = new Vector3(1, 1, 1);
        SphereCollider[] ss = CWHero.Instance.gameObject.GetComponentsInChildren<SphereCollider>();
        foreach (var v in ss)
        {
            v.enabled = true;
        }

    }
    private void FixedUpdate()
    {
        if (m_gPath == null) return;
        
        float s = Scale* CWHero.Instance.GetScaleRate();
        CWHero.Instance.transform.localScale = new Vector3(s,s,s);

        Vector3 vPos = m_gPath.transform.position + m_vStartPos + m_vPos;

       

        CWHero.Instance.SetPos(vPos);
        float fYaw = CWMath.GetLookYaw(CWHero.Instance.transform.position, vPos);
        CWHero.Instance.SetYaw(fYaw);

    }



}
