using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWStruct;
using CWEnum;

public class CWCollectUser : CWUser
{
    public override void Create(int nID)
    {
        
        base.Create(nID);
        UserType = USERTYPE.COLLECTUSER;
    }
    protected override void ReceiveEnd()
    {
        
        CWAIEntity cs = gameObject.GetComponent<CWAIEntity>();
        if(cs==null)
        {
            cs=gameObject.AddComponent<CWAIEntity>();
        }
        if (cs != null)
        {


            cs.m_fCooltime = 1;
            cs.m_fSightRange = 128;
            cs.m_Range = 64;
            cs.m_Speed = 12;
            
            cs.m_szEnmeyTag = "Hero";
            cs.m_bFirstAttack = false;// 선공 
       //     cs.Create(this, CWHeroManager.Instance.m_kCollectAIData.GetObject());
        }

      

    }
    protected override int ChangeItem(int nID)
    {
        GITEMDATA nData = CWArrayManager.Instance.GetItemData(nID);
        if (nData.type == "weapon")
        {
            int rr = UnityEngine.Random.Range(1, 2);
            if (rr == 1)
            {
                return CWArrayManager.Instance.GetWeaponItem("weapon_speed", nData.level);
            }

        }


        return base.ChangeItem(nID);
    }
  
    // 총알이 여러발 나간다
    bool m_beginWar = false;
    void BeginPVP()
    {
        CWAIEntity cs = gameObject.GetComponent<CWAIEntity>();
        if (cs != null)
        {
            //HelpMessageDlg.Instance.Show(null, "전투가 시작 되었습니다!");
           NoticeMessage.Instance.Show("전투가 시작 되었습니다!");

            cs.SetEnemy(CWHero.Instance);
        }

    }
    public override bool Hit(CWObject kKiller, int nDamage)
    {
        if(kKiller==CWHero.Instance)
        {
            CWAIEntity cs = gameObject.GetComponent<CWAIEntity>();
            if (cs != null)
            {
                if (m_beginWar==false)
                {
                    m_beginWar = true;
                    Invoke("BeginPVP", 0.5f);
                    return false;
                }

                if (cs.GetEnemy() == null) return false;
            }


        }

        return base.Hit(kKiller, nDamage);
    }
}
