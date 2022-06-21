using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;
using DG.Tweening;

// 공격을 당하면, AI 모드로 전환
public class CWFakeUser : CWUser
{
    static int g_PathCount = 0;

    
    
    

    float m_MinHeight = 10;

    protected override void SetObjectType()
    {
        m_ObjectType = CWOBJECTTYPE.FAKEUSER;
    }
    public override bool IsDrone()
    {
        return true;
    }

    public override void Create(int nID)
    {
        

        
        base.Create(nID);
        UserType = USERTYPE.FAKEUSER;
        

        int rr = g_PathCount + 1;
        
        g_PathCount++;
        g_PathCount %= 15;
        BeginAI();

        SetPos(new Vector3( 0, -1000, 0));

    }

    public override void CopyTransPos(Vector3 vPos, float fYaw)
    {

        base.CopyTransPos(vPos, fYaw);
    }

    public override void SetTeam()
    {
        NTeam = 1;// 페이크 유저는 무조건 1번 부여 
    }

    public override void SetDie()
    {
        base.SetDie();
    }

    public void BeginAI()
    {
        
        

        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai == null)
        {
            ai = gameObject.AddComponent<CWAIEntity>();
        }
        ai.m_fCooltime = 1f;
        ai.m_fThinktime = 0.5f;
        ai.m_fSightRange = 64;
        ai.m_Range = 64;
        ai.m_Speed = 32;
        ai.m_szEnmeyTag = "Hero";
        ai.m_bFirstAttack = true;// 선공 
        ai.m_AITYPE = AITYPE.ACTIVE;
        CWLib.SetGameObjectTag(gameObject, "AI");
        ai.Create(this, CWHeroManager.Instance.m_kFakeUserAI.GetObject());

        
        
    }
    public override void Shoot(bool bDeteted, Vector3 vTarget,GameObject gTarget=null)
    {
        //if(CWDebugManager.Instance.m_bDontAttack)
        //{
        //    return;//#디버깅
        //}
        base.Shoot(bDeteted, vTarget, gTarget);
    }
    public override bool Hit(CWObject kKiller, int nDamage)
    {

        return base.Hit(kKiller,  nDamage);
    }


    protected override void OnHit(int nDamage)
    {
        base.OnHit(nDamage);


    }
    
    public override void SetPos(Vector3 vPos)
    {
        float fy= CWMapManager.SelectMap.CalHeightByAI(vPos);
        if (vPos.y  < fy + m_MinHeight-2)
        {
            vPos.y = fy + m_MinHeight;
        }
        base.SetPos(vPos);
    }


    #region 가상 맟춤

    // 가상의 ,HP를 맞춤
    bool m_bFakeFlag = false;
    int m_FakeHP = 0;
    int m_FakeDamage=0;
    public override void SetFakePower(int hp,int damage)
    {
        m_FakeDamage = damage;
        m_FakeHP = hp;
        m_bFakeFlag = true;
    }
    protected override void PowerSetting()
    {
        base.PowerSetting();
        MakeFakeHP();

    }
    // HP에 맞게 블록을 맟추어 준다!
    protected override void MakeFakeHP()
    {
        m_nHP = m_FakeHP;
        KPower.m_nHp = m_FakeHP;
        
        CalPower();
        ChangeBlock();
        KPower.m_nDamage = m_FakeDamage;
        for (int i = 0; i < m_kWeapon.Count; i++)
        {
            m_kWeapon[i].m_nDamage = KPower.m_nDamage / m_kWeapon.Count;
            if (m_kWeapon[i].m_nDamage == 0) m_kWeapon[i].m_nDamage = 1;
            
        }



    }
    #endregion


    

}
