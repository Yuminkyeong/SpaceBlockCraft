using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWUnityLib;
public class CWMissile : MonoBehaviour {


    public enum MTYPE {NORMAL,FOLLOW,LASER,YUDO };

    public MTYPE m_mType = MTYPE.NORMAL;
    public GameObject Visible;

    public string m_szlaser;
    public CWObject m_gShooter; // 슛을 때린 객체 

    public float m_fSpeed =1f;
    public int m_nCount = 1;
    public int m_nDamage = 0;

    public string m_szHitEffect;
    public string m_szStartEffect;

    public string m_szStartSound;
    public string m_szBombSound;
   


    public Vector3 m_vTarget;

    GameObject m_gTargetObject;
    
    

    // Use this for initialization

    bool m_bDetected = false;
    Vector3 m_vDir;
    Vector3 m_vStart;
    bool m_bStop = false;

    float m_fSpeedDelta = 0;

    void Start () {
        if(CWLib.IsString(m_szlaser))
        {
            m_mType = MTYPE.LASER;
        }
    }

	
	// Update is called once per frame


	void Update ()
    {
        if (CWGlobal.g_GameStop) return;
        if (m_bStop) return;

        if(m_mType==MTYPE.NORMAL)
        {

            float fspeed = m_fSpeed;// Mathf.Lerp(m_fSpeed/2, m_fSpeed*2, m_fSpeedDelta);
            transform.position += m_vDir * fspeed * Time.deltaTime;
            m_fSpeedDelta += Time.deltaTime / 2f;

        }
        // 유도탄

        if (m_mType == MTYPE.YUDO)
        {
            if(m_gTargetObject)
            {
                Vector3 vDir = m_gTargetObject.transform.position - transform.position;
                vDir.Normalize();
                m_vDir = vDir;
                transform.forward = vDir;
            }
            float fspeed = Mathf.Lerp(m_fSpeed / 2, m_fSpeed * 2, m_fSpeedDelta);
            transform.position += m_vDir * fspeed * Time.deltaTime;

            m_fSpeedDelta += Time.deltaTime / 2f;

        }

        if (m_mType == MTYPE.FOLLOW)
        {
            transform.position = Vector3.Lerp(transform.position, m_vTarget, 1.0f - Mathf.Exp(-m_fSpeed/7 * Time.deltaTime));

        }



        if (CWHero.Instance.IsFarDist(transform.position))
        {
            if (Visible == null) return;
            Visible.SetActive(false);
            return;
        }


    }

    public void Begin(bool bDetected, CWObject gShooter, Transform tDummy, Vector3 vTarget,string sztag,float fspeed,int nblockcount,int ndamage)
    {
        if (CWGlobal.g_GameStop) return;

        if (Visible == null)
        {
            Visible = gameObject;
        }
        m_vTarget = vTarget;
        m_bStop = false;
        Visible.SetActive(true);


        m_bDetected = false;
        m_fSpeed = fspeed;

        m_gShooter = gShooter;
        transform.position = tDummy.position;
        Vector3 vDir = vTarget - transform.position;
        vDir.Normalize();
        gameObject.tag = sztag;

        CWLib.SetGameObjectTag(gameObject, sztag);
        
        transform.LookAt(vTarget);

        m_vStart = tDummy.position;
        m_vDir = vDir;
        
        m_nCount = nblockcount;
        m_nDamage = ndamage;

        Quaternion qq = transform.rotation * Quaternion.Euler(180, 0, 0);

        CWPoolManager.Instance.GetParticle(tDummy.position,qq , m_szStartEffect);


        // 드론일 경우 소리를 줄인다
        if (gShooter != CWHero.Instance)
        {
            if (gShooter.UserType != USERTYPE.TURRET)
            {
                CWResourceManager.Instance.PlaySound(m_szStartSound, gameObject, 0.5f);// 볼륨을 줄임
                return;
            }
        }
        CWResourceManager.Instance.PlaySound(m_szStartSound, gameObject);
        m_fSpeedDelta = 0;

       // StartCoroutine("IMissEffect");
    }

    
    IEnumerator IMissEffect()
    {
        yield return new WaitForSeconds(CWGlobal.g_fShootTime);

        CWPoolManager.Instance.GetParticle(gameObject.transform.position, "Missffect", 2f);
        CWResourceManager.Instance.PlaySound("missmissle");
        //if(CWGlobal.g_bGameBegin)
        //{
        //    if(GamePlay.Instance.m_nWType==GamePlay.WTYPE.SINGLE)
        //    {
        //        if (m_misscount < 3)
        //        {
        //            if (GamePlay.Instance.IsGamePlay() && CWHero.Instance.GetHpRate() > 0)
        //            {
        //                NoticeMessage.Instance.Show(193, "가까이 가서 공격하세요!");
        //            }

        //        }
        //        m_misscount++;
        //    }

        //}
        
        
    }
    void _CancelYudo()
    {
        m_mType = MTYPE.NORMAL;
        m_gTargetObject = null;
    }
    public void SetYudotan(GameObject gTarget)
    {
        if (gTarget == null) return;


        


        m_gTargetObject = gTarget;
        m_mType = MTYPE.YUDO;
        Invoke("_CancelYudo", 3f);


    }
    


    private void OnTriggerEnter(Collider other)
    {
        if (m_bDetected) return;
        if (CWGlobal.g_GameStop) return;
        if (!GamePlay.Instance.IsGamePlay()) return;
        if (!DebugTest.g_bTest4) return;
        if (other.tag == gameObject.tag) return;
        // 충돌
        

        if (gameObject.tag=="User")
        {
            gameObject.SetActive(false);
            m_bDetected = true;

            if (other.tag == "map")// 블록 충돌 
            {

                //CWMapManager.SelectMap.Hit(false, m_vTarget, m_nCount);

                CWPoolManager.Instance.GetParticle(m_vTarget, m_szHitEffect, 2f);

                CWPoolManager.Instance.GetParticle(m_vTarget, "pf_interstellar", 1f);//pf_interstellar  pf_smoke

                CWResourceManager.Instance.PlaySound("hammer_01");
                gameObject.SetActive(false);

                m_bDetected = true;



            }
            return; 
        }
        
        
        

        if(gameObject.tag=="Hero")// 때리는 자가 hero일때
        {
            if (other.tag == "User" || other.tag == "AI" || other.tag == "Build")
            {
                if(other.tag=="User"||other.tag=="Build")
                {
                    // 주인공은 유저와 건물을 공격 못한다
                    if (CWMapManager.BDontFight) return; // 전투 금지 지역
                }
                CWAirObject bb = other.gameObject.GetComponentInParent<CWAirObject>();//GetComponent<CWBuildObject>();
                if (bb)
                {

                   
                    if(bb.IsHeroTeam())
                    {
                        return;// 내편
                    }

                    RaycastHit hit;
                    Ray ray = new Ray(m_vStart, m_vDir);
                    int nMask = (1 << 10);//10만 디텍트 

                    Vector3 vDetect;
                    Vector3 vDir;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
                    {

                        vDetect = hit.point;
                        vDir = hit.normal;

                    }
                    else
                    {
                        vDetect = transform.position;
                        vDir = -transform.forward;

                    }

                    bb.Hit(m_gShooter, m_nDamage);

                    CWPoolManager.Instance.GetParticle(vDetect, bb.GetHitEffect() , 1f);
                    CWPoolManager.Instance.GetParticle(vDetect, m_szHitEffect, 2f);
                  
                    gameObject.SetActive(false);

                    m_bDetected = true;

                }
                else
                {
                   // CWDebugManager.Instance.Log("미사일맞음");
                    return;
                }

                
            }
            if (other.tag == "map")// 블록 충돌 
            {

                // 비행기는 블록을 깨지 못한다
                //CWMapManager.SelectMap.Hit(true, m_vTarget, m_nCount);

                CWPoolManager.Instance.GetParticle(m_vTarget, m_szHitEffect, 2f);
                CWPoolManager.Instance.GetParticle(m_vTarget, "pf_interstellar", 1f);//pf_interstellar  pf_smoke
                CWResourceManager.Instance.PlaySound("hammer_01");
                gameObject.SetActive(false);
                m_bDetected = true;



            }
           if (other.tag == "AI")
            {
                CWResourceManager.Instance.PlaySound("Turretboom1");
            }
            

        }
        else if(gameObject.tag=="AI"|| gameObject.tag == "Build")//
        {
            
            if ( other.tag == "Hero"|| other.tag == "User"||other.tag== "Build")// 주인공이 맞을 때
            {

                CWAirObject bb = other.gameObject.GetComponentInParent<CWAirObject>();//GetComponent<CWBuildObject>();
                if (bb)
                {
                    
                    if (!m_gShooter.IsEnemy(bb))
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                    RaycastHit hit;
                    Ray ray = new Ray(m_vStart, m_vDir);
                    int nMask = (1 << 12);//10만 디텍트 

                    Vector3 vDetect;
                    Vector3 vDir;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
                    {
                        vDetect = hit.point;
                        vDir = hit.normal;

                    }
                    else
                    {
                        vDetect = transform.position;
                        vDir = -transform.forward;
                    }

                    
                    bb.Hit(m_gShooter, m_nDamage);
                    //CWEffectManager.Instance.GetEffect(hit.point, m_szHitEffect);

                    CWPoolManager.Instance.GetParticle(hit.point, m_szHitEffect, 2f);

                    CWResourceManager.Instance.PlaySound(m_szBombSound, gameObject);

                    gameObject.SetActive(false);

                    m_bDetected = true;


                }
            }
            if (other.tag == "map")// 블록 충돌 
            {

                RaycastHit hit;
                Ray ray = new Ray(m_vStart, m_vDir);
                int nMask = (1 << 11);//10만 디텍트 
                Vector3 vDetect;
                Vector3 vDir;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
                {
                    vDetect = hit.point;
                    vDir = hit.normal;
                }
                else
                {
                    vDetect = transform.position;
                    vDir = -transform.forward;
                }


                //CWMapManager.SelectMap.Hit(false,  vDetect, m_nCount);
                //CWEffectManager.Instance.GetEffect(m_vTarget, m_szHitEffect);
                CWPoolManager.Instance.GetParticle(m_vTarget, m_szHitEffect, 2f);
                CWResourceManager.Instance.PlaySound("hammer_01");
                gameObject.SetActive(false);

                m_bDetected = true;




            }


        }
      

        




    }

/*
    #region 레이저


    void ShootBeamInDir( LineRenderer line,Vector3 start, Vector3 dir)
    {

#if UNITY_5_5_OR_NEWER
        line.positionCount = 2;
#else
		line.SetVertexCount(2); 
#endif
        line.SetPosition(0, start);
        beamStart.transform.position = start;

        Vector3 end = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(start, dir, out hit))
            end = hit.point - (dir.normalized * beamEndOffset);
        else
            end = transform.position + (dir * 100);

        beamEnd.transform.position = end;
        line.SetPosition(1, end);

        beamStart.transform.LookAt(beamEnd.transform.position);
        beamEnd.transform.LookAt(beamStart.transform.position);

        float distance = Vector3.Distance(start, end);
        line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
        line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
    }

    #endregion
*/
}
