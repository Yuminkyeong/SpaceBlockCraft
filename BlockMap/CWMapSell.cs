using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using CWUnityLib;
using UnityEngine.Rendering;
using CWEnum;
using CWStruct;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
// 메쉬를 가지고 있는 게임오브젝트

public class CWMapSell
{



    #region 변수

    BLOCKINFO[] m_kBlock;
    CWFaceMap m_kParentMap;
    public CWMapSell(CWFaceMap kMap)
    {
        m_kParentMap = kMap;
    }

   
    public bool m_bFast = false;




    CWSellGroup m_kSellGroup;




    public int m_nSellX;
    public int m_nSellY;
    public int m_nSellZ;
    public int m_nMapID;










    #endregion
    #region MAKEBLOCK
    int[,] g_vNormal =
    {
            {0,0,-1},
            {0,0, 1},
            {1,0, 0},
            {-1,0, 0},
            {0,1, 0},
            {0,-1, 0},
     };






    //class BLOKDATA
    //{
    //    public int m_sx, m_sy, m_sz;
    //    public int m_dx, m_dy, m_dz;
    //    public int m_nFace;
    //    public int m_nblock;
    //}
    [Serializable]
    struct BLOKDATA
    {
        public byte[] m_Data;
        public byte m_sx { get => m_Data[0]; set => m_Data[0] = (byte)value; }
        public byte m_sy { get => m_Data[1]; set => m_Data[1] = (byte)value; }
        public byte m_sz { get => m_Data[2]; set => m_Data[2] = (byte)value; }

        public byte m_dx { get => m_Data[3]; set => m_Data[3] = (byte)value; }
        public byte m_dy { get => m_Data[4]; set => m_Data[4] = (byte)value; }
        public byte m_dz { get => m_Data[5]; set => m_Data[5] = (byte)value; }

        public byte m_nFace { get => m_Data[6]; set => m_Data[6] = (byte)value; }
        public byte m_nblock { get => m_Data[7]; set => m_Data[7] = (byte)value; }

    }

    // 
    /// <summary>
    ///-Z 평면 +Z 평면 +X 평면 -X 평면 + Y 평면
    /// 0 , 1,2,3,4
    /// </summary>
    /// 
    /// <param name="t">aaa </param>



    Vector3[] vDir =
    {
        new Vector3(0, 0, -1),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),

    };
    //x 방향 증가 
    Vector3[] vDirPlusIX =
    {
        new Vector3(1, 0, 0),//new Vector3(0, 0, -1),
        new Vector3(1, 0, 0),//new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),//new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),//new Vector3(-1, 0, 0),
        new Vector3(1, 0, 0),//new Vector3(0, 1, 0),
        new Vector3(1, 0, 0),//new Vector3(0, 1, 0),

    };
    //y방향 증가 
    Vector3[] vDirPlusIY =
    {
        new Vector3(0, 1, 0),//new Vector3(0, 0, -1),
        new Vector3(0, 1, 0),//new Vector3(0, 0, 1),
        new Vector3(0, 0, 1),//new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),//new Vector3(-1, 0, 0),
        new Vector3(0, 0, 1),//new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),//new Vector3(0, 1, 0),

    };




    #endregion
    #region 블록제어


    BLOCKSHAPE _GetShape(int x, int y, int z, int nBlock)
    {
        return m_kBlock[nBlock].nShape;
    }
    public Color GetColor(int x, int y, int z)
    {
        return m_kParentMap.GetColor(m_nSellX * CWGlobal.SELLCOUNT + x, y, m_nSellZ * CWGlobal.SELLCOUNT + z);
    }
    public int GetBlock(int x, int y, int z)
    {
        return m_kParentMap.GetBlock(m_nSellX * CWGlobal.SELLCOUNT + x, y, m_nSellZ * CWGlobal.SELLCOUNT + z);
    }
    
    public int GetHeight(int x, int z)
    {
        for (int y = 1; y < CWGlobal.WD_WORLD_HEIGHT; y++)
        {
            int h = CWGlobal.WD_WORLD_HEIGHT - y;

            if (GetBlock(x, h, z) > 0)
            {
                return h;
            }
        }

        return 0;
    }

    public int GetHeight(int x, int height, int z)
    {
        for (int y = height; y >= 1; y--)
        {
            if (GetBlock(x, y, z) > 0)
            {
                return y;
            }
        }

        return 0;
    }


    #endregion
    #region 외부접근함수



    public string GetName()
    {
        string szname = string.Format("Sell_{0}_{1}_{2}", m_nMapID, m_nSellX, m_nSellZ);
        return szname;
    }
    public Vector3 GetPostion()
    {
        return new Vector3(m_nSellX * CWGlobal.SELLCOUNT, 0, m_nSellZ * CWGlobal.SELLCOUNT);
    }
    public bool Create(CWSellGroup kSellGroup, int nID, int sx, int sz)
    {

        m_nMapID = nID;
        m_nSellX = sx;
        m_nSellY = 0;
        m_nSellZ = sz;


        m_kSellGroup = kSellGroup;
        m_kBlock = CWArrayManager.Instance.m_kBlock;


        return true;
    }

    public void Close()
    {
        if (m_gObject != null)
        {
            GameObject.DestroyImmediate(m_gObject.GetComponent<MeshFilter>().sharedMesh);
            CWMakeMesh.G_MeshCount--;
            GameObject.Destroy(m_gObject);
            m_gObject = null;
        }
      
       
       


    }

    #endregion
    #region 버텍스 블록 만들기 


  
    #region 블록을 메쉬 만들기 작업 함수들
    //MakeVertextBlock 를 3개의 함수로 나눔 
    /// <summary>
    /// 라이트작업을 하기 위한 버텍스배열을 만듦
    /// </summary>




    
   

    public Mesh MakeMeshVertex()
    {
        CWMakeBlock kMakeBlock = new CWMakeBlock();// 블록을 메쉬로 만들 수 있게 머지하는 작업
        return kMakeBlock.Create(GetBlock, _GetShape, GetColor, CWGlobal.SELLCOUNT, CWGlobal.WD_WORLD_HEIGHT, CWGlobal.SELLCOUNT, false); // 블록을 가공한다  

    }
    public Mesh MakeMeshVertexLOD(int nSize)
    {
        CWMakeBlock kMakeBlock = new CWMakeBlock();// 블록을 메쉬로 만들 수 있게 머지하는 작업
        return kMakeBlock.CreateLOD(GetBlock, _GetShape, GetColor, nSize); // 블록을 가공한다  

    }

    #endregion


    #region LOD 만들기
    int GetBlockLOD(int x, int y, int z)
    {
        int nBlock = GetBlock(x, y, z);
        if (nBlock == 0) return 0;
        if (nBlock > 20) return 0;
        return nBlock;

    }
#endregion
#endregion
#region 블록메쉬오브젝트

    GameObject m_gObject;
    public void MakeMeshObject()
    {


        int x = m_nSellX * CWGlobal.SELLCOUNT - m_kSellGroup.m_nSellX * CWSellGroup.SELLSIZE;
        int z = m_nSellZ * CWGlobal.SELLCOUNT - m_kSellGroup.m_nSellZ * CWSellGroup.SELLSIZE;
        if (m_gObject == null)
        {
            m_gObject = GameObject.Instantiate(m_kSellGroup.m_kPrefabs);
            m_gObject.transform.parent = m_kSellGroup.transform;
            m_gObject.transform.localPosition = new Vector3(x, 0, z);
            m_gObject.transform.localRotation = new Quaternion();
        }

        if (m_gObject.GetComponent<MeshFilter>().sharedMesh != null)
        {

            GameObject.DestroyImmediate(m_gObject.GetComponent<MeshFilter>().sharedMesh);
            CWMakeMesh.G_MeshCount--;
        }

        Mesh kMesh = MakeMeshVertex();

        m_gObject.GetComponent<MeshFilter>().sharedMesh = kMesh;
        m_gObject.GetComponent<MeshCollider>().sharedMesh = null;
        m_gObject.GetComponent<MeshCollider>().sharedMesh = kMesh;

       
    }
    public void MakeMeshObjectLOD(int nSize)
    {


        int x = m_nSellX * CWGlobal.SELLCOUNT - m_kSellGroup.m_nSellX * CWSellGroup.SELLSIZE;
        int z = m_nSellZ * CWGlobal.SELLCOUNT - m_kSellGroup.m_nSellZ * CWSellGroup.SELLSIZE;
        if (m_gObject == null)
        {
            m_gObject = GameObject.Instantiate(m_kSellGroup.m_kPrefabs);
            m_gObject.transform.parent = m_kSellGroup.transform;
            m_gObject.transform.localPosition = new Vector3(x, 0, z);
            m_gObject.transform.localRotation = new Quaternion();
        }

        if (m_gObject.GetComponent<MeshFilter>().sharedMesh != null)
        {

            GameObject.DestroyImmediate(m_gObject.GetComponent<MeshFilter>().sharedMesh);
            CWMakeMesh.G_MeshCount--;
        }

        Mesh kMesh = MakeMeshVertexLOD(nSize);

        m_gObject.GetComponent<MeshFilter>().sharedMesh = kMesh;
        m_gObject.GetComponent<MeshCollider>().sharedMesh = null;
        m_gObject.GetComponent<MeshCollider>().sharedMesh = kMesh;


    }

    public Mesh GetMesh()
    {
        return m_gObject.GetComponent<MeshFilter>().sharedMesh;
    }

#endregion


}
