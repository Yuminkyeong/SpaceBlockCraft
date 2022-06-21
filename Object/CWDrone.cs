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

public class CWDrone : CWAIObject
{
    public int m_nParent;// 모선

    public int m_nMob;
    

    CWPVPAI m_vPVPAI;


    public delegate void CWDIEFUC();

    public CWDIEFUC m_DieEvent=null;
    public bool m_bBoss = false;

  
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
    public override void SetPos(Vector3 vPos)
    {
       

       
        base.SetPos(vPos);
        if (m_vPVPAI == null) return;
        m_vPVPAI.SetPos(vPos);


    }
    private void OnDestroy()
    {
        if (m_vPVPAI != null)
            Destroy(m_vPVPAI.gameObject);
        m_vPVPAI = null;
    }
    int GetLevel()
    {
        int lv= NLevel / 3;
        if (lv > 10) return 10;
        return lv;
    }
    protected override void AiAttach(GameObject gg)
    {
        GameObject gNew = new GameObject();
        m_vPVPAI = gNew.AddComponent<CWPVPAI>(); 
        m_vPVPAI.transform.SetParent(Game_App.Instance.m_gGameEnvDir.transform);


        m_vPVPAI.Begin(transform.position, CWHero.Instance, this, GetLevel(),true);

        SetTag("AI");
    }
    public override void SettingPower()
    {
        base.SettingPower();

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
        if (m_bBoss)
        {
            nDamage = mData.nDamage;// 그대로 적용
        }
        if (nDamage == 0) nDamage = 1;
        foreach (var v in m_kWeapon)
        {
            v.m_bAIflag = true;
            v.m_fSpeed = mData.fSpeed;

            if (m_bBoss)
            {
                v.m_nDamage = (int)(nDamage * fRatePower);
                v.m_fSpeed = mData.fSpeed * fRateSpeed;
            }
            else
            {
                v.m_nDamage = nDamage;
            }
            v.m_nBlockCount = 1;
          

        }
        KPower.m_nHp = mData.nHp;
        if (m_bBoss)
        {
            KPower.m_nHp = (int)(KPower.m_nHp * fRatePower);
        }



    }
    public override void SetDie()
    {

        NoticeMessage.Instance.Show("적기를 파괴하였습니다!!");

        CWMobManager.Instance.AddDie(m_nID);

        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai != null)
        {
            ai.Stop();
        }

        if(m_DieEvent!=null)
        {
            m_DieEvent();
        }
        base.SetDie();

    }
    protected override bool Run()
    {
     


        return true;

    }
    public override string GetName()
    {
        if (m_bBoss)
        {
            return string.Format("Lv. {0} BOSS",  NLevel);
        }
        
          return string.Format("Lv. {0}",  NLevel);
    }
    protected override string GetUI()
    {
             return "DroneUI";
    }

    protected override void BeginHPBar()
    {
        
    }

    
}
