#define DONTUSELODGROUP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWSetLOD : MonoBehaviour {


    public GameObject [] m_gMesh;
    public GameObject m_gTemp;



#if DONTUSELODGROUP
    public bool SetLOD()
    {

        gameObject.SetActive(true);
        CWSellGroup gParent = GetComponentInParent<CWSellGroup>();

        int[] iarray = { 0, 2, 4, 8 };
        string szname;
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                szname = string.Format("MeshAsset/{0}_{1}_{2}", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ);
            else
            {
                szname = string.Format("MeshAsset/{0}_{1}_{2}_l{3}", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ, iarray[i]);
            }
            MeshFilter mf = m_gMesh[i].GetComponent<MeshFilter>();
            if (mf.sharedMesh == null)
            {
                mf.sharedMesh = Resources.Load(szname) as Mesh;
            }
            if (mf.sharedMesh == null)
            {
                if (i == 0)            // 어셋없음
                    return false;
                else
                {
                    MeshFilter mf2 = m_gMesh[i - 1].GetComponent<MeshFilter>();
                    mf.sharedMesh = mf2.sharedMesh;
                }


            }

            m_gMesh[i].SetActive(false);
        }

        {
            szname = string.Format("MeshAsset/{0}_{1}_{2}_t", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ);
            MeshFilter mf = m_gTemp.GetComponent<MeshFilter>();
            mf.sharedMesh = Resources.Load(szname) as Mesh;
            m_gTemp.SetActive(false);
        }


        StartCoroutine("Run");
        return true;
    }
    IEnumerator Run()
    {
        while(true)
        {
            foreach (var v in m_gMesh) v.SetActive(false);
            Vector3 vPos= Camera.main.transform.position;
            Vector3 vPos2= transform.position;
            float fDist = Vector3.Distance(vPos, vPos2);
            if(fDist<64)
            {
                m_gMesh[0].SetActive(true);
            }
            else if (fDist < 128)
            {
                m_gMesh[1].SetActive(true);
            }
            else if (fDist < 256)
            {
                m_gMesh[2].SetActive(true);
            }
            else 
            {
                m_gMesh[3].SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

#else
    public bool SetLOD()
    {
#if UNITY_EDITOR
        gameObject.SetActive(true);
        LODGroup group = GetComponent<LODGroup>();
        group.enabled = false;
        CWSellGroup gParent = GetComponentInParent<CWSellGroup>();
        string szname = string.Format("MeshAsset/{0}_{1}_{2}", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ);

        foreach(var v in m_gMesh)
        {
            v.SetActive(false);
        }

        m_gMesh[0].SetActive(true);
        {
            MeshFilter mf = m_gMesh[0].GetComponent<MeshFilter>();
            if (mf.sharedMesh == null)
            {
                mf.sharedMesh = Resources.Load(szname) as Mesh;
            }
            if (mf.sharedMesh == null)
            {
                return false;

            }
            MeshCollider ms = m_gMesh[0].GetComponent<MeshCollider>();
            if (ms)
            {
                ms.sharedMesh = null;
                ms.sharedMesh = m_gMesh[0].GetComponent<MeshFilter>().sharedMesh;
            }
        }
        {
            szname = string.Format("MeshAsset/{0}_{1}_{2}_t", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ);
            MeshFilter mf = m_gTemp.GetComponent<MeshFilter>();
            if(mf.sharedMesh==null)
            {
                mf.sharedMesh = Resources.Load(szname) as Mesh;
                if (mf.sharedMesh == null)
                {
                    m_gTemp.SetActive(false);
                }
                else
                {
                    m_gTemp.SetActive(true);
                    MeshCollider ms = m_gTemp.GetComponent<MeshCollider>();
                    if (ms)
                    {
                        ms.sharedMesh = null;
                        ms.sharedMesh = m_gTemp.GetComponent<MeshFilter>().sharedMesh;
                    }

                }


            }
        }


        return true;
#else


        gameObject.SetActive(true);
        CWSellGroup gParent = GetComponentInParent<CWSellGroup>();
        LODGroup group = GetComponent<LODGroup>();

        LOD[] lods = new LOD[4];
        int[] iarray = {0,2,4,8 };



        float[] fRate = new float[4];
        if (CWGlobal.g_SystemSpeedLevel == 0) fRate[0] = 0.2f;
        if (CWGlobal.g_SystemSpeedLevel == 1) fRate[0] = 0.3f;
        if (CWGlobal.g_SystemSpeedLevel == 2) fRate[0] = 0.4f;

        for(int i=0;i<3;i++)
        {
            fRate[i + 1] = fRate[i]/2;
        }



        string szname;
        for (int i=0;i<4;i++)
        {
            if(i==0)
                szname = string.Format("MeshAsset/{0}_{1}_{2}", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ);
            else
            {
                szname = string.Format("MeshAsset/{0}_{1}_{2}_l{3}", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ, iarray[i]);
            }
            MeshFilter mf = m_gMesh[i].GetComponent<MeshFilter>();
            if(mf.sharedMesh==null)
            {
                mf.sharedMesh = Resources.Load(szname) as Mesh;
            }
            if(mf.sharedMesh==null)
            {
                if(i==0)            // 어셋없음
                    return false;
                else
                {
                    MeshFilter mf2 = m_gMesh[i-1].GetComponent<MeshFilter>();
                    mf.sharedMesh = mf2.sharedMesh;
                }
            }
            Renderer[] renderers = new Renderer[1];

            renderers[0] = m_gMesh[i].GetComponent<Renderer>();
            lods[i] = new LOD(fRate[i], renderers);
            if(i==0)
            {
                MeshCollider ms = m_gMesh[i].GetComponent<MeshCollider>();
                if (ms)
                {
                    ms.sharedMesh = null;
                    ms.sharedMesh = m_gMesh[i].GetComponent<MeshFilter>().sharedMesh;
                }


            }


        }

        {
            szname = string.Format("MeshAsset/{0}_{1}_{2}_t", gParent.m_nMapID, gParent.m_nSellX, gParent.m_nSellZ);
            MeshFilter mf = m_gTemp.GetComponent<MeshFilter>();
            if(mf.sharedMesh==null)
            {
                mf.sharedMesh = Resources.Load(szname) as Mesh;
                if (mf.sharedMesh == null)
                {
                    m_gTemp.SetActive(false);
                }
                else
                {
                    m_gTemp.SetActive(true);
                    MeshCollider ms = m_gTemp.GetComponent<MeshCollider>();
                    if (ms)
                    {
                        ms.sharedMesh = null;
                        ms.sharedMesh = m_gTemp.GetComponent<MeshFilter>().sharedMesh;
                    }

                }


            }
        }

        group.SetLODs(lods);
        group.RecalculateBounds();

        return true;
#endif

    }

#endif

}
