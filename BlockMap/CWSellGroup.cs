//#define DONTTRHEAD 
#define TEST
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using CWUnityLib;
using UnityEngine.Rendering;
using CWEnum;

public class CWSellGroup : MonoBehaviour {


    public string Debugstr;

    #region 초기화

    public CWFaceMap m_kParentMap;


    public const int SELLSIZE = 128;
    int SELLWIDTH;
    
    public int m_nSellX=-1;
    public int m_nSellZ = -1;
    public int m_nMapID=0;
    private bool bDeleteMark = false;
    

    public bool m_bVisible = false;
    public CWMapSell[] m_kMapSell =null;

    
    

    public int m_nVerCount = 0;
    
    


    public bool BDeleteMark
    {
        get
        {
            return bDeleteMark;
        }

        set
        {
            bDeleteMark = value;
        }
    }

    //List<CWSellBlock> m_kThreadWork = new List<CWSellBlock>();

    private void Start()
    {
        
    }
    
    
    public void Create(CWFaceMap kMap, int nID,int sx,int sz)
    {
        m_kParentMap = kMap;

        SELLWIDTH = SELLSIZE / CWGlobal.SELLCOUNT;//32/128 = 4 
        m_kMapSell = new CWMapSell[SELLWIDTH * SELLWIDTH];

        m_nMapID = nID;
        m_nSellX = sx;
        m_nSellZ = sz;

        for (int z = 0; z < SELLWIDTH; z++)
        {

            for (int x = 0; x < SELLWIDTH; x++)
            {
                int num = z * SELLWIDTH + x;

                m_kMapSell[num] = new CWMapSell(m_kParentMap);
                int mx = (m_nSellX * SELLSIZE) / CWGlobal.SELLCOUNT + x;
                int mz = (m_nSellZ * SELLSIZE) / CWGlobal.SELLCOUNT + z;
                
                m_kMapSell[num].Create(this, nID, mx,  mz);
            }

        }

        BDeleteMark = false;
        transform.localPosition = new Vector3(m_nSellX * SELLSIZE,0, m_nSellZ * SELLSIZE);


    }
    public void Close()
    {
        if (m_kMapSell == null) return;
        foreach(var v in m_kMapSell)
        {
            v.Close();
        }

    }
    #endregion
    #region 외부접근
    public Vector3 GetCenter()
    {
        Vector3 vPos1= new Vector3( SELLSIZE / 2, 64/2,  SELLSIZE / 2);
        Vector3 vPos2 = transform.position;
        return vPos1 + vPos2;

    }



    public bool IsMakeSell()
    {
        return (m_DistType == DISTTYPE.MAINSELL) ;
    }





    enum DISTTYPE {MAINSELL,LOD_2, LOD_3, LOD_4 };
    DISTTYPE m_DistType = DISTTYPE.LOD_4;

    
    public void ShowSell()
    {
    }

    
    public CWMapSell GetSell(int mx,int mz)//32단위
    {
        if (mx < 0) return null;
        if (mz < 0) return null;
        if(SELLWIDTH==0)
        {
            return null;
        }
        int nx = mx % SELLWIDTH;
        int nz = mz % SELLWIDTH;
        int num = nz * SELLWIDTH + nx;
        return m_kMapSell[num];

        
    }


    #endregion

    
    
    

 
    public Mesh CombineMesh()
    {
        Mesh kMesh = new Mesh();
        
        kMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        CombineInstance[] combine = new CombineInstance[SELLWIDTH * SELLWIDTH];
        int cnt = 0;
        for (int z = 0; z < SELLWIDTH; z++)
        {
            
            {
                for (int x = 0; x < SELLWIDTH; x++)
                {
                    int num = z * SELLWIDTH + x;

                    combine[num].mesh = m_kMapSell[num].GetMesh();
                   
                    combine[num].transform = Matrix4x4.Translate(new Vector3(x * CWGlobal.SELLCOUNT, 0, z * CWGlobal.SELLCOUNT));

                    if (combine[num].mesh != null)
                    {
                        cnt += combine[num].mesh.vertexCount;
                    }
                }

            }
        }
        kMesh.CombineMeshes(combine);
        return kMesh;
    }
    public float GetDir()
    {
        GameObject gg = CWLib.FindChild(gameObject, "Center");
        Vector3 vPos = gg.transform.position;
        Vector3 _Way = Vector3.zero - vPos;
        float _Angle = Mathf.Atan2(_Way.x, _Way.z) * Mathf.Rad2Deg;
        return _Angle;
        

    }
#if UNITY_EDITOR


    private void OnEnable()
    {
        if(CWGlobal.g_bEditmode)
            StartCoroutine("RunCheckVertex");

       /// LODGroup ls = GetComponent<LODGroup>();
        //ls.SetLODs

    }
    public int GetVertexCount()
    {
        return m_nVerCount;
    }
    IEnumerator RunCheckVertex()
    {
        while(true)
        {
            int vcnt = 0;


            MeshFilter [] ff= GetComponentsInChildren<MeshFilter>();
            foreach(var v in ff)
            {
                if (v.sharedMesh != null)
                    vcnt += v.sharedMesh.vertexCount;

            }

            if (vcnt>0)
            {
                m_nVerCount = vcnt;
                if (vcnt > 65000)
                {
                    name = string.Format("경고! : {0} {1}_{2}", vcnt, m_nSellX,  m_nSellZ);
                }
                else
                {
                    name = string.Format("{0} {1}_{2}", vcnt, m_nSellX,  m_nSellZ);

                }

            }
            yield return new WaitForSeconds(20);
        }
    }

  

    // 디버깅
    
    //private void Update()
    //{
    //    //float fangle= CWMath.GetLookYaw(Vector3.zero, transform.position);
        

    //    GameObject gg = CWLib.FindChild(gameObject, "Center");
    //    Vector3 vPos = gg.transform.position;
    //    Vector3 _Way = Vector3.zero - vPos;
    //    float _Angle = Mathf.Atan2(_Way.x, _Way.z) * Mathf.Rad2Deg;


    //    Debugstr = string.Format("{0},{1} ={2}",m_nSellX,m_nSellZ, _Angle);
        
        
    //}


#endif

    #region  새로운 맵로드 
    public GameObject m_kPrefabs;

    public void MakeMeshVertexLOD(int nSize)
    {

        int dx = CWSellGroup.SELLSIZE / CWGlobal.SELLCOUNT;
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                CWMapSell kMapSell = GetSell(x, z);
                kMapSell.MakeMeshObjectLOD(nSize);

            }

        }

    }


    public void MakeMeshVertex()
    {
        
        int dx = CWSellGroup.SELLSIZE / CWGlobal.SELLCOUNT;
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                CWMapSell kMapSell= GetSell(x,z);
                kMapSell.MakeMeshObject();

            }

        }

        

    }




    #endregion


}
