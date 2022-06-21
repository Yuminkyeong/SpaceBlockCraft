using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using CWStruct;
using DG.Tweening;

public class CWMobPath : CWAirObject
{

    public delegate void CWDIEFUC();

    public int MissileType = 0;// 한곳으로, 1 평균 분산, 2 랜덤
    public CWDIEFUC m_DieEvent = null;

    float m_fMaxDelaySec = 5f;
    float m_fMinDelaySec = 1f;

    public DOTweenPath m_gPath;
    
    int m_nMob;
    bool m_bBoss;


    Vector3 m_vStartPos = Vector3.zero;

    public void Begin(int nLevel,int Mob,bool bBoss,string szPath,float fSpeed)
    {
        m_bBoss = bBoss;
        NLevel = nLevel;
        m_nMob = Mob;
      

        string szFile = CWArrayManager.Instance.GetMobFile(m_bBoss,m_nMob);
        Load(szFile);


        m_vStartPos.x = Random.Range(-10, 10);

        m_gPath = CWResourceManager.Instance.GetPath(szPath, fSpeed);


        m_gPath.DOPlay();

        gameObject.layer = 10;
        NTeam = 1;//NPC 
        SetTag("AI");
        m_ObjectType = CWOBJECTTYPE.DRONE;
        if(m_bBoss)
        {
            MissileType = 2;
        }
        else
        {
            MissileType = 1;
        }

        

    }
    public override void SetDie()
    {
        NoticeMessage.Instance.Show(193,"적기를 파괴하였습니다!!");
        if (m_DieEvent != null)
        {
            m_DieEvent();
        }
        base.SetDie();

    }
    public override void WeaponSetting()
    {
        base.WeaponSetting();
    }
    public override void CalPower()
    {
        base.CalPower();

        // 최대 6
        float fMax = 4f;
        float fRatePower =1+ fMax*(NLevel/10f);// 레벨에 따른 파워 변화 
        float fRateSpeed =1 + (NLevel/15);// 레벨에 따른 파워 변화 

        if (fRatePower > 5f) fRatePower = 5f;
        if (fRateSpeed > 2f) fRateSpeed = 2f;


        CWArrayManager.MOBDATA mData = CWArrayManager.Instance.GetMOBData(NLevel);
        int nCount = m_kWeapon.Count;
        if (nCount == 0)
        {
            nCount = 1;
        }
            
        int nDamage = mData.nDamage / nCount; // 무기의 개수를 나눠서 밸런스를 맞춘다 
        if(m_bBoss)
        {
            nDamage = mData.nDamage;// 그대로 적용
        }

        foreach (var v in m_kWeapon)
        {
            v.m_bAIflag = true;
            v.m_fSpeed = mData.fSpeed;

            if (m_bBoss)
            {
                v.m_nDamage = (int)(nDamage * fRatePower);
                v.m_fSpeed = mData.fSpeed* fRateSpeed;
            }
            else
            {
                v.m_nDamage = nDamage;
            }
            v.m_nBlockCount = 1;
           
        }
        m_fMaxDelaySec = mData.fMaxDelay;
        m_fMinDelaySec = mData.fMinDelay;
        KPower.m_nHp = mData.nHp;
        if(m_bBoss)
        {
            KPower.m_nHp = (int)(KPower.m_nHp * fRatePower);
        }



    }
    protected override void DeleteFile()
    {
        if(m_gPath)
        {
            Destroy(m_gPath);
            m_gPath = null;
        }
        base.DeleteFile();
    }


    

    bool m_bStopFlag = false;
    private void FixedUpdate()
    {
        if (CWGlobal.g_GameStop)
        {
            m_bStopFlag = true;
            m_gPath.DOPause();
            return;
        }

        if(m_bStopFlag)
        {
            m_bStopFlag = false;
            m_gPath.DOPlay();
        }


        if (m_gPath == null) return;

        Vector3 vPos = m_gPath.transform.position + m_vStartPos;
        vPos.y = CWGlobal.START_HEIGHT;
        SetPos(vPos);
        float fYaw = CWMath.GetLookYaw(GetPosition(), CWHero.Instance.transform.position);
        SetYaw(fYaw);

    }

    // 무기와 높이를 맞춤
    public override void SetPos(Vector3 vPos)
    {
        base.SetPos(vPos);
        Vector3 v= transform.position;
        float fy = m_kWeapon[0].transform.parent.localPosition.y;
        v.y-= (m_kWeapon[0].transform.localPosition.y+fy);


        transform.position =  v;
    }

    public override void Shoot(bool bDeteted, Vector3 vTarget, GameObject gTarget = null)
    {
        

        if (CWGlobal.g_GameStop) return;
        if (CWGlobal.g_bStopAIAttack) return;
        // 평행 
        // 개념 목표, 나와의 각도, 각도를 바꾸는 값
        Vector3 vdir = vTarget- transform.position;
        vdir.Normalize();
        float fdist = Vector3.Distance(vTarget ,transform.position);

        int rr= Random.Range(1, 5);
        for (int i=0;i<m_kWeapon.Count;i++)
        {
            Vector3 t = vTarget;
            if (MissileType==1)
            {
                Vector3 yaw = CWMath.CalYaw((i-1) * rr, vdir);
                t = transform.position + yaw * fdist;
            }
            if (MissileType == 2)
            {
                float val = Random.Range(-10, 10);
                Vector3 yaw = CWMath.CalYaw(val, vdir);
                t = transform.position + yaw * fdist;
            }

            m_kWeapon[i].Shoot(this, bDeteted, t, gTarget);
        }

    }

}
