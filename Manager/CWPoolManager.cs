using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
// 미사일 및 슈팅개념만

public class CWPoolManager : CWManager<CWPoolManager>
{

    static public  int _MaxCount = 100;
    public enum SType {NORMAL,MISSILE,LASER,PARTICLE };

    public override void Create()
    {
        
        base.Create();
    }
    public class PoolData
    {

        SType m_sType;
        public string  m_szname;
        LinkedList<GameObject> m_List = new LinkedList<GameObject>();
        Transform m_gParent;
        public void Clear()
        {
            foreach(var v in m_List)
            {
                v.SetActive(false);
            }
        }
        public PoolData(Transform gParent,SType stype)
        {
            m_gParent = gParent;
            m_sType = stype;
        }
        
        void CBClose(GameObject gg)
        {
            if(m_gParent==null)
            {
                gg.transform.SetParent(CWPoolManager.Instance.transform);
                gg.transform.localPosition = Vector3.zero;
            }
        }
        GameObject AddData()
        {
            GameObject gg;
            if(m_sType==SType.LASER)
            {
                gg = CWResourceManager.Instance.GetLaser(m_szname);
            }
            else if (m_sType == SType.MISSILE)
            {
                gg = CWResourceManager.Instance.GetMissile(m_szname);
            }
            else if(m_sType == SType.PARTICLE)
            {
                gg = CWResourceManager.Instance.GetParticle(m_szname);
            }
            else
            {
                gg = CWResourceManager.Instance.GetPrefab(m_szname);
            }
          
           
            gg.AddComponent<Pool_Lifetime>();
            if(m_gParent!=null)
            {
                gg.transform.parent = m_gParent;
                gg.transform.localPosition = Vector3.zero;
            }
            m_List.AddLast(gg);
            return gg;
            
        }
        public GameObject GetData(float fLifetime)
        {
            if(m_List.Count > _MaxCount)
            {
                GameObject gg = m_List.First.Value;
                m_List.AddLast(gg);
                m_List.RemoveFirst();
                if (gg.activeSelf == true)
                {
                    Debug.Log("풀데이타 넘어갔음");
                }
                Pool_Lifetime cs = gg.GetComponent<Pool_Lifetime>();
                cs.Begin(fLifetime, CBClose);
                return gg;
            }

            {
                GameObject gg = AddData();
                Pool_Lifetime cs = gg.GetComponent<Pool_Lifetime>();
                cs.Begin(fLifetime, CBClose);

                return gg;
            }
        }

    };

    private Dictionary<string, PoolData> m_Pool = new Dictionary<string, PoolData>();


    public GameObject GetObject( string szname, float m_fLifetime=2, Transform tParent=null)
    {
        // 같은 값을 풀로 저장 
        
        if (!m_Pool.ContainsKey(szname))
        {
            PoolData ndata = new PoolData(tParent, SType.NORMAL);
            ndata.m_szname = szname;
            m_Pool.Add(szname, ndata);
        }
        return m_Pool[szname].GetData(m_fLifetime);
    }

    public void Clear()
    {
        foreach(var v in m_Pool)
        {
            v.Value.Clear();
        }

    }
    public GameObject GetMissile(string  szname,float m_fLifetime)
    {
        if (!CWLib.IsString(szname))
        {
            Debug.Log("");
            return null;
        }

        // 같은 값을 풀로 저장 
        if (!m_Pool.ContainsKey(szname))
        {
            PoolData ndata = new PoolData(transform,SType.MISSILE);
            ndata.m_szname = szname;
            m_Pool.Add(szname, ndata);
        }
        return m_Pool[szname].GetData(m_fLifetime);
    }
    public GameObject GetLaser(string szname, float m_fLifetime)
    {
        // 같은 값을 풀로 저장 
        if (!m_Pool.ContainsKey(szname))
        {
            PoolData ndata = new PoolData(transform,SType.LASER);
            ndata.m_szname = szname;
            m_Pool.Add(szname, ndata);
        }
        return m_Pool[szname].GetData(m_fLifetime);
    }

    GameObject GetParticle(string szname, float m_fLifetime)
    {
#if DONTPARTICLE
        return null;
#endif

        if (!CWLib.IsString(szname))
        {
            Debug.Log("");
            return null;
        }
        // 같은 값을 풀로 저장 
        if (!m_Pool.ContainsKey(szname))
        {
            PoolData ndata = new PoolData(transform, SType.PARTICLE);
            ndata.m_szname = szname;
            m_Pool.Add(szname, ndata);
        }
        return m_Pool[szname].GetData(m_fLifetime);
    }

    public void GetFollowParticle(GameObject gFollowTarget, Vector3 vPos, string szname, float m_fLifetime,bool bBomb=false)
    {
#if DONTPARTICLE
        return ;
#endif

        GameObject gg = GetParticle(szname, m_fLifetime);
        if (gg)
        {
            gg.transform.position = vPos;
            CWFollowEffect cf = gg.GetComponent<CWFollowEffect>();
            if(cf==null)
            {
                cf =gg.AddComponent<CWFollowEffect>();
            }
            if(cf)
            {
                cf.Begin(gFollowTarget, bBomb);
            }


        }

    }
    public GameObject GetParticle(Vector3 vPos, string szname, float m_fLifetime=2f)
    {
#if DONTPARTICLE
        return null;
#endif

        GameObject gg = GetParticle(szname, m_fLifetime);
        if(gg)
        {
            gg.transform.position = vPos;
        }
        return gg;
    }
    public GameObject GetParticle(Vector3 vPos,Quaternion rot,  string szname, float m_fLifetime = 2f)
    {
#if DONTPARTICLE
        return null;
#endif

        if (!CWLib.IsString(szname)) return null;

        GameObject gg = GetParticle(szname, m_fLifetime);
        if (gg)
        {
            gg.transform.position = vPos;

            gg.transform.rotation = rot;
        }
        return gg;
    }

}
