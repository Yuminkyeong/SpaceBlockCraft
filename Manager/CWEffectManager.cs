using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWEffectManager : CWManager<CWEffectManager>
{

    const int _MaxCount = 3;

    public class PoolData
    {
        public string m_szname;
        LinkedList<GameObject> m_List = new LinkedList<GameObject>();
        Transform m_gParent;
        public PoolData(Transform gParent)
        {
            m_gParent = gParent;
        }

        GameObject AddData()
        {
            GameObject gg;
            gg = CWResourceManager.Instance.GetEffect(m_szname);
            gg.transform.parent = m_gParent;
            gg.transform.localPosition = Vector3.zero;
            m_List.AddLast(gg);
            return gg;
        }
        public GameObject GetData(Vector3 vPos, Transform tParent, float fLifetime=0)
        {
            if (m_List.Count > _MaxCount)
            {
                // 가장 먼저 사용한데이타 출력 
                //
                GameObject gg = m_List.First.Value;
                if(gg==null)
                {

                    m_List.RemoveFirst();
                    gg = m_List.First.Value;
                    if (gg == null) return null;

                }
                m_List.AddLast(gg);
                m_List.RemoveFirst();

                if (gg == null) return null;
                CWEffect cs = gg.GetComponent<CWEffect>();
                cs.Begin(vPos, tParent,fLifetime);

                return gg;
            }
            {
                GameObject gg = AddData();
                CWEffect cs = gg.GetComponent<CWEffect>();
                cs.Begin(vPos, tParent, fLifetime);
                return gg;
            }
        }

    };

    private Dictionary<string, PoolData> m_Pool = new Dictionary<string, PoolData>();

    public GameObject GetEffect(Vector3 vPos, string szname, float fLifetime = 0)
    {
#if DONTPARTICLE
        return null;
#endif
        //if (DebugTest.g_bTest3) return null;
        if (CWHero.Instance.IsFarDist(vPos)) return null;

        if (szname == null) return null;
        if(szname.Length<1) return null;

        return GetObject(vPos, null, szname, fLifetime);
    }
    public GameObject GetEffect(Transform tParent, string szname, float fLifetime = 0)
    {
#if DONTPARTICLE
        return null;
#endif

        if (szname == null) return null;
        if (szname.Length < 1) return null;

        return GetObject(Vector3.zero, tParent, szname, fLifetime);
    }

    GameObject GetObject(Vector3 vPos, Transform tParent, string szname, float fLifetime = 0)
    {
        // 같은 값을 풀로 저장 
#if DONTPARTICLE
        return null;
#endif

        if (!m_Pool.ContainsKey(szname))
        {
            PoolData ndata = new PoolData(transform);
            ndata.m_szname = szname;
            m_Pool.Add(szname, ndata);
        }
        return m_Pool[szname].GetData(vPos, tParent, fLifetime);
    }
    public void BlockEfect(Vector3 vPos,int tcnt)
    {
#if DONTPARTICLE
        return ;
#endif

        GameObject gg = GetObject(vPos, null, "BlockEffect");
        var pp = gg.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule mainModule=pp.main;
        mainModule.maxParticles = tcnt;

    }


}
