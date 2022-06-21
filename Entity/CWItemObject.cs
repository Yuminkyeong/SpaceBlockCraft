using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;
public class CWItemObject : MonoBehaviour {

    public int m_nSlot;
    public int m_ID;
    public int m_nPosNumber;
    public GITEMDATA m_kItem;
    public GameObject m_gDummy;

    public float m_fSpeed;
    public int m_nBlockCount;
    public int m_nDamage;
    public int m_nLevel;// 무조건 1부터 시작한다!!!!
    public string m_szMissile;

    public bool m_bAIflag = false;// 데미지를 외부에서 조정한다 

    public int m_nWeaponType = 0;

    //Weapon 

	void Start () {

        if(m_kItem.type== "weapon")
        {
            m_gDummy = CWLib.FindChild(gameObject, "dummy");
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Rotate(Vector3 vRot)
    {
        transform.eulerAngles = vRot;
    }
   
    public void BombShoot(string sztag, Vector3 vPos,int nBlockcount,string szMissile)
    {
        if (m_gDummy == null)
        {
            m_gDummy = CWLib.FindChild(gameObject, "dummy");
            if (m_gDummy == null)       return;
        }
        GameObject gg = CWPoolManager.Instance.GetMissile(szMissile, CWGlobal.g_fShootTime);
        CWMissile kActor = gg.GetComponent<CWMissile>();
        if (kActor)
        {
            CWObject gShooter = gameObject.GetComponentInParent<CWObject>();

            kActor.Begin(true, gShooter, m_gDummy.transform, vPos, sztag, m_fSpeed, nBlockcount, 1);

        }


    }
    
    public void Shoot(CWObject kObject, bool bDeteted, Vector3 vPos,GameObject gTarget=null,int MultiDamage=0)
    {
       
        if(m_gDummy==null)
        {
            m_gDummy = CWLib.FindChild(gameObject, "dummy");
            if (m_gDummy == null) return;

            return;
        }
        int Damage = m_nDamage;
        if (MultiDamage > 0) Damage = MultiDamage;
        CWObject gShooter = gameObject.GetComponentInParent<CWObject>();

        Damage = Damage * gShooter.BonusDamage();


        WEAPON nData = CWArrayManager.Instance.GetWeapon(m_ID);
        


        if(nData.nType== 3)// 레이저
        {
            GameObject gg = CWPoolManager.Instance.GetLaser(m_szMissile, 3f);
            CWLaser kActor = gg.GetComponent<CWLaser>();
            if (kActor)
            {
                CWHero gHero = gameObject.GetComponentInParent<CWHero>();
                if (gHero != null)
                {
                    // 주인공이라면
                    gHero.AddLaser(kActor);

                }

                kActor.Begin(m_gDummy.transform, kObject, Damage);
            }


        }
        else
        {
            GameObject gg = CWPoolManager.Instance.GetMissile(m_szMissile, CWGlobal.g_fShootTime);
            if (gg == null) return;
            CWMissile kActor = gg.GetComponent<CWMissile>();
            if (kActor)
            {


                //m_fSpeed = Game_App.Instance.Test3;

                kActor.Begin(bDeteted, gShooter, m_gDummy.transform, vPos, gameObject.tag, m_fSpeed, m_nBlockCount, Damage);
                kActor.SetYudotan(gTarget);
            }

        }


    }

    public  void RotateWeaponIdle()
    {
        StopAllCoroutines();
        StartCoroutine("RotateIdleRun");
    }
    IEnumerator RotateIdleRun()
    {
        float fdelta = 0;
        int r = Random.Range(0, 4);
        float rr = Random.Range(0.5f, 1.5f);
        float fdir = 0;

        if (r == 0) fdir = rr;
        if (r == 1) fdir = -rr;

        while (true)
        {
            Vector3 v = transform.eulerAngles;
            v.y += (Time.deltaTime * 50f) * fdir;
            fdelta += (Time.deltaTime * 50f);
            transform.eulerAngles = v;
            if (fdelta > 180) break;
            yield return null;
        }

        yield return null;
    }

    public virtual void TurntoEnmey(Vector3 vTarget, EventDelegate.Callback fuc)
    {
        StartCoroutine(TurnFucntion(vTarget, fuc));
    }
    IEnumerator TurnFucntion(Vector3 vTarget, EventDelegate.Callback fuc)
    {

        Vector3 vRot = transform.eulerAngles;
        Vector3 _Way = vTarget - transform.position;
        float _Angle = Mathf.Atan2(_Way.x, _Way.z) * Mathf.Rad2Deg;

        // 회전각도 증가는 어느방향이 가까운가?
        float fdist = CWMath.GetAngleDist(vRot.y, _Angle);
        float fdir = CWMath.GetAngleDir(vRot.y, _Angle);

    //    Debug.Log(string.Format("A={0} B={1}, {2}{3} ", vRot.y, _Angle, fdist, fdir));

        float fRet = 0;
        // 0,1,2,3,4,
        while (true)
        {
            fRet += (Time.deltaTime * 100);
            vRot.y += (Time.deltaTime * 100) * fdir;
            if (Mathf.Abs(fdist) < fRet)
            {
                fuc?.Invoke();
                break;
            }
            //            transform.eulerAngles = vRot;
            Rotate(vRot);
            yield return null;
        }
    }


}
