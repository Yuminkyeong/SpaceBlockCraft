using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;
using DG.Tweening;


public class CWFightUser : CWUser
{

    public CWPVPAI m_gPrefabPVPAI;

    CWPVPAI m_vPVPAI;

    
    public override void Create(int nID)
    {



        base.Create(nID);
        UserType = USERTYPE.FIGHTUSER;

        m_nHeightInfo = 10;

    }
    protected override void ReceiveEnd()
    {
        Debug.Log(string.Format("유저데이타 완료!!"));
       

        m_nRankPoint = CWHero.Instance.m_nRankPoint + 10 - Random.Range(0, 20);
        if (m_nRankPoint < 0) m_nRankPoint = 0;
        base.ReceiveEnd();
    }
    /*
     *  
     *  점점 HP가 높아지는 방향으로 
     * 
     * */
    
    void GetHPByHero(ref int rethp, ref int refDamage)
    {
        int nKey = CWHero.Instance.m_nGrade+1;
        int minhp= CWTableManager.Instance.GetTableInt("티어범위 - 범위", "적 최저 HP", nKey);
        int maxhp= CWTableManager.Instance.GetTableInt("티어범위 - 범위", "적 최고 HP", nKey);
        int mindamage= CWTableManager.Instance.GetTableInt("티어범위 - 범위", "적 최저 공격력", nKey);
        int maxdamage=  CWTableManager.Instance.GetTableInt("티어범위 - 범위", "적 최고 공격력", nKey);

        rethp = Random.Range(minhp, maxhp);
        refDamage = Random.Range(mindamage, maxdamage);

    }
    // 
    // 주인공의 능력치에 따라 값이 나온다
    /* 개념 
     * 공격력을 기준으로
     * 2배에서 ~ 5배 
     * */
    /*
        void GetHPByHero(ref int rethp,ref int refDamage)
        {

            float fGradeRate = 1;
            if(m_nGrade==1)
            {
                fGradeRate = 1f + (m_nGrade)/7f;
            }

            float fvalue = CWHero.Instance.GetDamage();

            int RR = Random.Range(0, 4);
            if(RR==1)
            {
                fvalue = CWHero.Instance.GetHP();
            }



            int hp = (int)CWHero.Instance.GetHP();
            //3번씩 강한 유저 등장

            int rr = PVPDlg.Instance.m_nPVPWinCount % 5;//0,1,2,3
            float frate = Random.Range(1, 2f);
            if (rr == 2)
            {
                rethp = (int)(fvalue + fvalue * 3 + fvalue * frate);
            }
            else
            {
                rethp = (int)(fvalue + fvalue * frate);
            }


            float farate = Random.Range(0.2f, 0.8f);


            refDamage = (int)((float)rethp * farate * fGradeRate);
            if(RR<=2)
            {
                if(refDamage>hp)
                {
                    float rate = Random.Range(0.1f,0.3f);
                    refDamage =(int)( hp - hp*rate);
                }

            }

            rethp =(int)(rethp* fGradeRate);

        }
    */
    protected override void PowerSetting()
    {
        m_nGrade = CWHero.Instance.m_nGrade;
        GetHPByHero(ref m_nHP,ref m_nDamage);

        KPower.m_nDamage = m_nDamage;
        KPower.m_nHp = m_nHP;
        ChangeBlock();

        int slevel = CWHeroManager.Instance.GetWeaponSpeedLevel(1);
        int nlevel = CWHeroManager.Instance.GetWeaponDamageLevel(1);

        float frate= Random.Range(0.5f, 3.5f);

        for (int i = 0; i < m_kWeapon.Count; i++)
        {
            m_kWeapon[i].m_nSlot = i + 1;
            m_kWeapon[i].m_nDamage = KPower.m_nDamage / m_kWeapon.Count;
            if (m_kWeapon[i].m_nDamage == 0) m_kWeapon[i].m_nDamage = 1;


            m_kWeapon[i].m_fSpeed = 80 + 30 * frate;
           
        }

    }
    
    Vector3 m_vStartPos;
    public override void SetPos(Vector3 vPos)
    {
        //if(GamePlay.Instance.m_bGamePlay)
        //{
        //    vPos.y = CWMapManager.SelectMap.CalHeightByAI(vPos) + 32;
        //}

        vPos.y = 60;
        m_vStartPos = vPos;
        base.SetPos(vPos);
        if (m_vPVPAI == null) return;
        m_vPVPAI.SetPos(vPos);
        

    }
    int GetLevel()
    {
        return Random.Range(1, 10);
    }

    public void BeginAI()
    {

        m_vPVPAI = Instantiate(m_gPrefabPVPAI);
        m_vPVPAI.transform.SetParent(Game_App.Instance.m_gGameEnvDir.transform);
        m_vPVPAI.Begin( m_vStartPos,CWHero.Instance, this, GetLevel());

        CWLib.SetGameObjectTag(gameObject, "AI");

    }
    
    public override void CalPower()
    {
        base.CalPower();
        KPower.FhpRate = 1;
    }

    private void OnDestroy()
    {
        if(m_vPVPAI!=null)
            Destroy(m_vPVPAI);
        m_vPVPAI = null;
    }

    protected override void MakeProgressBar()
    {
        //base.MakeProgressBar();
    }
}
