using UnityEngine;

using System;
using System.Runtime.InteropServices;
using CWUnityLib;
using CWEnum;
/*
 * 플랜메쉬 
 * */


public class CWPlanMesh
{

    const float FRate = 1 / 64f;
    const float GRIDE = 64f;

    int[,] g_nIndexOrder =
    {

    { 0,1,2,3,2,1},
    { 0,2,1,3,1,2}

};
    Vector2[] g_VUV =
    {
    new Vector2(0, 0),
    new Vector2(0, FRate),
    new Vector2(FRate, 0) ,
    new Vector2(FRate, FRate),
};

    Vector3[] g_Varray =
    {
		new Vector3(1, 1, 1),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 1),
        new Vector3(0, 1, 0),

};
    int m_nWidth = 32;
    public delegate int dgGetBlock(int x, int z);
    public delegate int dgGetHeight(int x, int z);


    // 면을 만든다 
    public Vector3[] m_kVerts;
    public Vector2[] m_kUV;
    public int[] m_Indexs;

    public int m_dwVertexCount;
    public int m_dwIndexCount;
    

    void GetUV(ref Vector2 vUV, byte nBlock)
    {

        int x = CWArrayManager.Instance.m_kBlock[nBlock].x;
        int y = CWArrayManager.Instance.m_kBlock[nBlock].y;

        vUV.x = x / GRIDE;
        vUV.y = y / GRIDE;

    }

    Vector3 MakePos(Vector3 vPos, dgGetBlock _getblock, dgGetHeight _getheight)
    {
        int x = (int)vPos.x;
        int z = (int)vPos.z;
        int h = _getheight(x, z);
        return new Vector3(x, h, z);


    }
    Vector3 vPos1 = Vector3.zero;
    Vector3 vPos2 = Vector3.zero;

    Vector2 MakeUV(int num,Vector3 vPos, dgGetBlock _getblock, dgGetHeight _getheight)
    {
        int nBlock = _getblock((int)vPos.x, (int)vPos.z);
        Vector2 vUV = new Vector2();// uv start
        GetUV(ref vUV, (byte)nBlock);
        return vUV+g_VUV[num];

    }

    void MakeFace( int x, int z, int nLOD, dgGetBlock _getblock, dgGetHeight _getheight)
    {
        vPos2.x = x;
        vPos2.z = z;
        vPos2.y = _getheight(x, z);
        int dwVerCnt = m_dwVertexCount;

        for (int i = 0; i < 4; i++)
        {
            vPos1 = vPos2 + g_Varray[ i]* nLOD;
            m_kVerts[m_dwVertexCount] = MakePos(vPos1,_getblock,_getheight);
            m_kUV[m_dwVertexCount] = MakeUV(i,vPos1, _getblock, _getheight);
            m_dwVertexCount++;
        }

        // 
        int nd = 0;
        for (int i = 0; i < 6; i++)
        {
            if (m_dwIndexCount >= m_Indexs.Length) continue;
            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;
        }


    }

    public void Make(int width,int nLOD, dgGetBlock _getblock, dgGetHeight _getheight)
    {
        m_nWidth = width;
        m_dwVertexCount = 0;
        m_dwIndexCount = 0;

        int dx = (m_nWidth/ nLOD) + 1;
        int nVerCnt = (dx) * (dx) * 4;
        int nIndexCnt = (dx) * (dx) * 6;

        m_kVerts = new Vector3[nVerCnt];
        m_kUV = new Vector2[nVerCnt];
        m_Indexs = new int[nIndexCnt];
        for (int z = 0; z <= m_nWidth; z += nLOD)
        {
            for (int x = 0; x <= m_nWidth; x += nLOD)
            {
                MakeFace(x, z, nLOD, _getblock, _getheight);
            }
        }
    }
    public Mesh GetMesh()
    {

        if (m_dwVertexCount == 0) return null;

        Mesh kMesh;
        kMesh = new Mesh();
        
        kMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[m_dwVertexCount];
        Vector2[] uv = new Vector2[m_dwVertexCount];
        int[] triangles = new Int32[m_dwIndexCount];
 
        Array.Copy(m_kVerts, vertices, m_dwVertexCount);
        Array.Copy(m_kUV, uv, m_dwVertexCount);
        Array.Copy(m_Indexs, triangles, m_dwIndexCount);
 
        kMesh.vertices = vertices;
        kMesh.uv = uv;
        kMesh.triangles = triangles;
        kMesh.RecalculateNormals();

        return kMesh;


    }
}