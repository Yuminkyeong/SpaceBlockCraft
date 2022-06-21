using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using CWStruct;
using DG.Tweening;
/*
 * 현재 스테이지에 맞는 능력치로 세팅
 * */

public class CWAIObject : CWAirObject
{

    bool m_bIdleStop = false;

    public override void SetDie()
    {
        CWVibration.Vibrate(300);

        base.SetDie();

    }

    public override void Create(int nID)
    {
        base.Create(nID);
        NTeam = 1;//NPC 
    }
    protected override void OnLoadEnd()
    {
        base.OnLoadEnd();

        SphereCollider ss = gameObject.GetComponent<SphereCollider>();
        if (ss == null)
        {
            ss = gameObject.AddComponent<SphereCollider>();
        }
        ss.radius = m_vSize.x;
        ss.center = m_vCenter;
        ss.isTrigger = true;

    }
    public override void CalPower()
    {
        base.CalPower();
        SettingPower();
        AiAttach(gameObject);

    }
    public override void WeaponSetting()
    {
        
    }
    // 능력치를 세팅한다 
    public virtual void SettingPower()
    {


        int nCount = m_kWeapon.Count;
        if (nCount == 0) nCount = 1;
        int nDamage = CWArrayManager.Instance.GetMOBDamage(NLevel)/nCount;
        if (nDamage == 0) nDamage = 1;

        foreach (var v in m_kWeapon)
        {
            WEAPON nData = CWArrayManager.Instance.GetWeapon(v.m_ID);
            v.m_bAIflag = true;
            v.m_nDamage = nDamage;
            v.m_nBlockCount = 1;
         
            //v.m_fSpeed = CWArrayManager.Instance.GetMOBSpeed(NLevel);
        }

        KPower.m_nHp = CWArrayManager.Instance.GetMOBHP(NLevel);

        //int layer1 = 1 << LayerMask.NameToLayer("Detect");int layer1 = 1 << LayerMask.NameToLayer("Detect");
        CWLib.SetGameObjectLayer(gameObject, LayerMask.NameToLayer("Detect"));
    }

    public override Vector3 GetHitPos()
    {
        if (m_gItemBody == null)
        {
            return base.GetHitPos();
        }
        if (m_gItemBody.transform.childCount == 0)
        {
            GameObject gg= CWLib.FindChild(gameObject, "HitDummy");
            if(gg)
            {
                return gg.transform.position;
            }
           return transform.position;
        }


        int rr = UnityEngine.Random.Range(0, m_gItemBody.transform.childCount);
        if (rr >= m_gItemBody.transform.childCount) rr = 0;
        Transform tChild = m_gItemBody.transform.GetChild(rr);
        return tChild.position;
    }

    protected virtual void AiAttach(GameObject gg)
    {
        
        CWAIEntity ai = gg.GetComponent<CWAIEntity>();
        if (ai == null)
        {
            ai = gg.AddComponent<CWAIEntity>();
        }

        ai.Create(this, CWHeroManager.Instance.m_kAITurretUnit.GetObject());
        ai.m_fCooltime = 1f; //m_fCooltime;
        ai.m_fThinktime = 2f;// m_fAItime;
        ai.m_Range = 80;
        ai.m_szEnmeyTag = "Hero";
        ai.m_bFirstAttack = true;// 선공 
        //ai.m_fSightRange = ai.m_Range - 2;// 사격범위 보다 작아야 한다
        ai.m_AITYPE = AITYPE.PASSIVE;

        if(UserType== USERTYPE.FIGHTUSER)
        {
            StartCoroutine("ICheckHeight");
        }

        
    }

    CWObject m_gTarget = null;
    Vector3 m_vTarget;
    bool m_bDetected = false;
    void _Shoot()
    {
        base.AIShoot(m_bDetected, m_gTarget);
    }
    void _ShootPos()
    {


        base.AIShootPos(m_bDetected, m_vTarget);
    }
    public override void RotateWeaponIdle()
    {
        if (m_bIdleStop) return;
        // 무기 아이들
        //m_kWeapon.RotateWeaponIdle();
        foreach (var v in m_kWeapon)
        {
            v.RotateWeaponIdle();
        }


    }

    public override void AIShoot(bool bDetected, CWObject gTarget)
    {
        if (gTarget == null) return;
        if (gTarget.m_nRest == 1) return;
        if (IsDie())
        {
            return;
        }
        m_bDetected = bDetected;
        m_gTarget = gTarget;
        m_bIdleStop = true;


        Invoke("StartIdle", 5f);
        TurntoEnmey(gTarget.transform.position, _Shoot);

    }
    void StartIdle()
    {
        m_bIdleStop = false;
    }

    public override void AIShootPos(bool bDetected, Vector3 vPos)
    {
        if (IsDie())
        {
            return;
        }

        m_bDetected = bDetected;
        m_bIdleStop = true;
        Invoke("StartIdle", 5f);

        m_vTarget = vPos;
        TurntoEnmey(vPos, _ShootPos);
    }
    public int GetAreaHeight(Vector3 vSize, Vector3 vPos)
    {
        if (CWMapManager.SelectMap == null) return 0;
        Vector3Int vInt = CWMapManager.ConvertPos(vPos);

        int sx = vInt.x - (int)(vSize.x / 2);
        int sz = vInt.z - (int)(vSize.z / 2);
        int ex = vInt.x + (int)(vSize.x / 2);
        int ez = vInt.z - (int)(vSize.z / 2);

        int tcnt = 0;
        int total = 0;
        // 평균값보다 높은 곳에 있어야 한다
        for (int z = sz; z <= ez; z++)
        {
            for (int x = sx; x <= ex; x++)
            {
                int h = CWMapManager.SelectMap.GetHeight(x, z);
                if (h > 0)
                {
                    total += h;
                    tcnt++;
                }
            }
        }
        if (tcnt == 0) return 0;

        int aa = total / tcnt;// 평균값
        total = 0;
        tcnt = 0;
        for (int z = sz; z <= ez; z++)
        {
            for (int x = sx; x <= ex; x++)
            {
                int h = CWMapManager.SelectMap.GetHeight(x, z);
                if (h >= aa)// 평균값 보다 높은 값만
                {
                    total += h;
                    tcnt++;
                }
            }
        }
        return total / tcnt;
    }
    // 높이 값에 자동으로 맞추어진다
    IEnumerator ICheckHeight()
    {
        while (true)
        {

            Vector3 vPos = GetPosition();
            vPos.y = GetAreaHeight(m_vSize, vPos);
            SetPos(vPos);
            yield return new WaitForSeconds(1f);
        }
    }


    
    protected override void HitReAction()
    {
        if (m_bHitReAction) return;
        m_bHitReAction = true;
        if (m_gBody == null) return;


        m_gBody.transform.DOShakeRotation(0.3f, 10).OnComplete(() => {
            m_bHitReAction = false;
        });


    }

    // 여기서 미사일을 넣어줌
    protected override string FindMissile(int wtype, int nLevel)
    {
        if (nLevel == 0) nLevel = 1;
        return "Mob_" + nLevel.ToString();
        //return base.FindMissile(wtype, nLevel);
    }

}
