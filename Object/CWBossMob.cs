using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;
using TMPro;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;


public class CWBossMob : CWAIObject
{

    public int m_nParent;// 모선

    public int m_nMob;


    public int m_nKey;
    


    public delegate void CWDIEFUC();

    public CWDIEFUC m_DieEvent = null;


    protected override void MakeProgressBar()
    {
        m_nHeightInfo = 0;
        base.MakeProgressBar();
    }
    protected override void SetObjectType()
    {
        m_ObjectType = CWOBJECTTYPE.DRONE;
    }

    public override void Create(int nID)
    {

        UserType = USERTYPE.DRONE;

        if (NLevel == 0) NLevel = 1;

        base.Create(nID);
    }
    public override bool IsDrone()
    {
        return true;
    }
    protected override void AiAttach(GameObject gg)
    {
        CWAIEntity ai = gg.GetComponent<CWAIEntity>();
        if (ai == null)
        {
            ai = gg.AddComponent<CWAIEntity>();
        }
        ai.Create(this, CWHeroManager.Instance.m_kAIBossUnit.GetObject());
        ai.m_fCooltime = 1f; //m_fCooltime;
        ai.m_fThinktime = 2f;// m_fAItime;
        ai.m_Range = 80;
        ai.m_szEnmeyTag = "Hero";
        ai.m_bFirstAttack = true;// 선공 
        //ai.m_fSightRange = ai.m_Range - 2;// 사격범위 보다 작아야 한다
        ai.m_AITYPE = AITYPE.PASSIVE;

        SetTag("AI");
    }
    public override void CalPower()
    {
        base.CalPower();

        m_nMob = CWArrayManager.Instance.GetMobNumber(name);
        // 최대 6
        float fMax = 4f;
        float fRatePower = 1 + fMax * (NLevel / 10f);// 레벨에 따른 파워 변화 
        float fRateSpeed = 1 + (NLevel / 15);// 레벨에 따른 파워 변화 

        if (fRatePower > 5f) fRatePower = 5f;
        if (fRateSpeed > 2f) fRateSpeed = 2f;


        CWArrayManager.MOBDATA mData = CWArrayManager.Instance.GetMOBData(NLevel);
        int nCount = m_kWeapon.Count;
        if (nCount == 0)
        {
            nCount = 1;
        }

        int nDamage = mData.nDamage / nCount; // 무기의 개수를 나눠서 밸런스를 맞춘다 

        foreach (var v in m_kWeapon)
        {
            v.m_bAIflag = true;
            v.m_fSpeed = mData.fSpeed;
            v.m_nDamage = (int)(nDamage * fRatePower);
            v.m_fSpeed = mData.fSpeed * fRateSpeed;

            v.m_nBlockCount = 1;


        }
        
        KPower.m_nHp = (int)(KPower.m_nHp * fRatePower*2);



    }
    public override void SetDie()
    {


        CWMobManager.Instance.AddDie(m_nID);
        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai != null)
        {
            ai.Stop();
        }
        if (m_DieEvent != null)
        {
            m_DieEvent();
        }
        base.SetDie();

    }
    public override string GetName()
    {

        string szname = CWTableManager.Instance.GetTable("스테이지 - 보스", "Name", m_nKey);
        return szname;
    }
    protected override string GetUI()
    {
        return "DroneUI";
    }

    protected override void BeginHPBar()
    {

    }
    string GetBossName()
    {
        string szname = CWTableManager.Instance.GetTable("스테이지 - 보스", "캐릭터", m_nKey);
        return szname;
    }
    protected override GameObject CharblockAttach()
    {

        GameObject gg = new GameObject();
        CharBody cb = CWResourceManager.Instance.GetCharBody(GetBossName());
        cb.transform.parent = gg.transform;
        
        cb.transform.localPosition = new Vector3(0, -1.3f, 0);
        cb.transform.localRotation = new Quaternion();
        BoxCollider bb = gg.AddComponent<BoxCollider>();
        bb.size = Vector3.one;
        bb.isTrigger = true;
        return gg;
    }

    public void SetBossNumber(int nKey)
    {

        m_nKey = nKey;
    }

}
