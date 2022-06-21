using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWUnityLib;
public class CWMobManager : CWManager<CWMobManager>
{
    const int MAXDIST = 128;
    const int ALLMAXDIST = 512;
    
    
    int m_nMapID;

    public void AddDie(int nID)
    {
        
        if (m_kMobList.ContainsKey(nID))
        {
            m_kMobList[nID].KMob = null;
        }

    }
    public CWObject GetObject(int nID)
    {
        if (m_kMobList.ContainsKey(nID))
        {
            return m_kMobList[nID].KMob;
        }
        return null;
    }
   

    public class MOBINFO
    {
        public int m_nNumber;
        public string m_szname;
        public int m_nParent = 0;
        public AIOBJECTTYPE m_AIType;
        public Vector3 m_vPos;
        public float m_fYaw;
        

        

        private int nLevel;
        
        private CWAIObject _kMob = null;

        
  

        public CWAIObject KMob
        {
            get
            {
                return _kMob;
            }

            set
            {
                _kMob = value;
            }
        }

        public int CLassLevel
        {
            get
            {
                return nLevel;
            }
            set
            {
                
                nLevel = value;
            }
        }


   

     
    };

    
    Dictionary<int, MOBINFO> m_kMobList = new Dictionary<int, MOBINFO>();
    int m_nLastNumber = CWGlobal.NPC_STARTNUBER;
    public override void Create()
    {
        base.Create();
        

    }
    public int GetGroup(int nLevel)
    {
        if (nLevel < 5) return 1;
        if (nLevel < 10) return 2;
        if (nLevel < 30) return 3;
        if (nLevel < 60) return 4;
        return 5;
    }

    #region 로드/세이브
    CWObject MakeObject(AIOBJECTTYPE kType, int nID, Vector3 vPos, float fYaw, int nStage, string szName)
    {

        GameObject gg = new GameObject();
        if(kType== AIOBJECTTYPE.TURRET)
        {
            gg.AddComponent<CWTurret>();
        }
        else if (kType == AIOBJECTTYPE.BOSS)
        {
            gg.AddComponent<CWBossMob>();

            CWBossMob kBoss = gg.GetComponent<CWBossMob>();

            int nKey = ((nStage - 1) / 6) + 1;
            kBoss.SetBossNumber(nKey);

        }
        else
        {
            gg.AddComponent<CWDrone>();
        }
        CWAIObject kUser = gg.GetComponent<CWAIObject>();
        kUser.m_AIType = kType;
        gg.name = szName;
        gg.transform.parent = transform;
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localRotation = new Quaternion();
        kUser.NLevel = nStage;

        CWPower cp = gg.AddComponent<CWPower>();
        cp.FhpRate = 1;

        kUser.SetPos(vPos);
        kUser.SetYaw(fYaw);
        kUser.Create(nID);

        return kUser;

    }
 

    public void AddData(Vector3 vPos,float fYaw,int nClassLevel,string szname, AIOBJECTTYPE kType,int nStage=0)
    {
       

        MOBINFO kInfo = new MOBINFO
        {
            m_nNumber = m_nLastNumber,
            m_szname = szname,
            CLassLevel = nClassLevel,
            m_AIType = kType,
            m_vPos = vPos,
            m_fYaw=fYaw,

        };
        m_nLastNumber++;

        kInfo.KMob=(CWAIObject) MakeObject(kInfo.m_AIType, kInfo.m_nNumber,vPos, fYaw,  nStage, kInfo.m_szname);

        m_kMobList.Add(kInfo.m_nNumber, kInfo);
        

    }



    
    public int GetDroneCountForChallenge()
    {
        CWObject[] aa = GetComponentsInChildren<CWObject>();
        return aa.Length;
    }
    public void Close()
    {
        CWObject[] array = gameObject.GetComponentsInChildren<CWObject>();
        foreach (var v in array)
        {
            Destroy(v.gameObject);
        }
        m_kMobList.Clear();


    }

    
    public void Clear()
    {
        if (CWMapManager.SelectMap == null) return;
        CWObject[] array = gameObject.GetComponentsInChildren<CWObject>();
        foreach (var v in array)
        {
            Destroy(v.gameObject);
        }
        m_kMobList.Clear();

    }


    void CheckObject()
    {
        List<int> ktemp = new List<int>();
        foreach (var v in m_kMobList)
        {
            if(v.Value.KMob==null)
            {
                ktemp.Add(v.Key);
            }
        }
        foreach(var v in ktemp)
        {
            m_kMobList.Remove(v);
        }
    }
    public void Save(CWJSon jSon)
    {
        CheckObject();
        CWFile cf = new CWFile();
        cf.PutInt(m_kMobList.Count);
        foreach(var v in m_kMobList)
        {
            cf.PutString(v.Value.m_szname);
            cf.PutInt(v.Value.m_nNumber);
            cf.PutInt(v.Value.CLassLevel);
            cf.PutInt((int)v.Value.m_AIType);
            Vector3 vPos = v.Value.KMob.transform.position;
            cf.PutFloat(vPos.x);
            cf.PutFloat(vPos.y);
            cf.PutFloat(vPos.z);
            cf.PutFloat(v.Value.KMob.GetYaw());
        }
        cf.Save(jSon,"Turret");

    }
    public void Load(CWJSon jSon)
    {
        Close();
        int nStage = 0;
        if (Space_Map.Instance)
            nStage = Space_Map.Instance.GetStageID();

/*
        CWFile cf = new CWFile();
        if (cf.Load(jSon, "Turret"))
        {
            int tcnt = cf.GetInt();
            for (int i = 0; i < tcnt; i++)
            {
                string szname = cf.GetString();
                int lnumber = cf.GetInt();
                int level = cf.GetInt();
                int nType = cf.GetInt();
                float x = cf.GetFloat();
                float y = cf.GetFloat();
                float z = cf.GetFloat();
                float fYaw =  cf.GetFloat();
                AIOBJECTTYPE ktype = (AIOBJECTTYPE)nType;
                AddData(new Vector3(x, y, z), fYaw, level, szname, ktype, nStage);
            }
        }
*/

    }







    #endregion
    #region 플레이

    CWProductionRoot m_gBossProduction=null;
    public void StagePlay()
    {
        StartCoroutine("IStagePlay");
    }
    public void ClearProcuction()
    {
        if (m_gBossProduction == null) return;
        Destroy(m_gBossProduction);
        m_gBossProduction = null;

    }
    public void BossWar(Action cbfuc)
    {
        int nStage = 0;
        nStage = Space_Map.Instance.GetStageID();
        int nKey = ((nStage - 1) / 6) + 1;
        string szProduction = CWTableManager.Instance.GetTable("스테이지 - 보스", "production", nKey);

        string szfilename = CWTableManager.Instance.GetTable("스테이지 - 보스", "File", nKey);
        CWProductionRoot pt = CWResourceManager.Instance.GetProduction(szProduction);
        if (pt)
        {
            pt.Begin(cbfuc);
        }
        m_gBossProduction = pt;

        Vector3 vPos = new Vector3(128, 64, 128);
        AddData(vPos, 180, nStage, szfilename, AIOBJECTTYPE.BOSS, nStage);

    }
    void _StagePlay()
    {
        int nStage = 0;
        if (Space_Map.Instance)
            nStage = Space_Map.Instance.GetStageID();
        CWArrayManager.StageData kStagedata = CWArrayManager.Instance.GetStageData(nStage);
        string szfilename = CWArrayManager.Instance.GetStageMobName(kStagedata.m_nStage);
        int tcnt = kStagedata.m_nMobCount;
        Vector3 vStart = new Vector3(CWGlobal.START_X, CWGlobal.START_HEIGHT, 256 - CWGlobal.START_Z);
        for (int i = 0; i < tcnt; i++)
        {
            Vector3 vPos = vStart;
            vPos.x += UnityEngine.Random.Range(-30, 30);
            AddData(vPos, 0, kStagedata.m_nStage, szfilename, AIOBJECTTYPE.DRONE, nStage);
            //yield return new WaitForSeconds(3f);// 주기적으로 나오게 함 
        }

    }

    IEnumerator IStagePlay()
    {
        yield return null;

        _StagePlay();

        
    }
  

    
    

#endregion
#region 외부접근



    public int GetCount()
    {
        return m_kMobList.Count ;
    }
    public int GetKillCount()
    {
        int tcnt = 0;
        foreach(var v in m_kMobList)
        {
            if (v.Value.KMob == null) { tcnt++; continue; }
            if (v.Value.KMob.gameObject.activeSelf == false) { tcnt++; continue; }
            if (v.Value.KMob.IsDie()) { tcnt++; continue; }
        }
        return tcnt;
    }
    public void AllKill()
    {
        foreach (var v in m_kMobList)
        {
            Destroy(v.Value.KMob);
            v.Value.KMob = null;
        }

    }
    public bool IsAllKill()
    {
        if (GetCount() == 0) return false;
        if(GetCount()==GetKillCount())
        {
            return true;
        }
        return false;
    }
    public bool FindType(AIOBJECTTYPE Aitype)
    {
        foreach(var v in m_kMobList)
        {
            if(v.Value.KMob.IsDie())
            {
                continue;
            }
            if(v.Value.m_AIType==Aitype)
            {
                return true;
            }
        }

        return false;
    }
  
    public CWObject Search(AIOBJECTTYPE Aitype, Vector3 vPos, int nSightRange)
    {

        CWObject kResult = null;
        float fMindist = 100000f;
        // hero 포함
        foreach (var v in m_kMobList)
        {
            CWObject o = v.Value.KMob;
            if (o == null) continue;
            if (o.m_AIType!=Aitype) continue;

            float fDist = CWMath.VectorDistSQ(o.transform.position, vPos);
            //float fDist = Vector3.Distance(o.transform.position, vPos);
            if (fDist <= (nSightRange - 1)* (nSightRange - 1))
            {
                //if (!IsFrontEnemy(v.transform.position)) continue;// 내 시야에서 안보이면 적으로 간주 안함 
                //if (IsBlockWall(v.transform.position)) continue;// 블록 장애물이 존재한다 
                if (fDist < fMindist)
                {
                    fMindist = fDist;
                    kResult = o;
                }
            }
        }

        return kResult;
    }
    #endregion
}
