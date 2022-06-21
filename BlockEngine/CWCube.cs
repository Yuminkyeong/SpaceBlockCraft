using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWCube : MonoBehaviour
{

    #region 배열들
    const float FRate = 1 / 64f;
    Vector3[,] g_Varray =
    {

    {  // -Z 평면
		new Vector3(1, 1, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 0),


    },
    {  // +Z 평면
		new Vector3(1, 1, 1),
        new Vector3(1, 0, 1),
        new Vector3(0, 1, 1),
        new Vector3(0, 0, 1),

    },
    {	//+X 평면
		new Vector3(1, 1, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 0),
        new Vector3(1, 0, 0),

    },
    {	//-X 평면 
		new Vector3(0, 1, 1),
        new Vector3(0, 0, 1),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 0),

    },
    {  //+ Y 평면
		new Vector3(1, 1, 1),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 1),
        new Vector3(0, 1, 0),

    },
    {  //- Y 평면
		new Vector3(1, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, 0),
    },


    };
    int[,] g_nIndexOrder =
    {

        { 0,1,2,3,2,1},
        { 0,2,1,3,1,2}

    };
    Vector2[] g_VUV =
    {
        new Vector2(FRate, FRate),
        new Vector2(FRate, 0) ,
        new Vector2(0, FRate),
        new Vector2(0, 0),
    };
    #endregion

    public Vector3[] m_kVerts;
    public Vector2[] m_kUV;
    
    public int[] m_Indexs;

    public int m_dwVertexCount;
    public int m_dwIndexCount;

    void MakeBox()
    {
        for (int i = 0; i < 6; i++)
        {
            MakeFace(i);
        }
    }

    public void MakeFace(int nFace)
    {


        for (int i = 0; i < 4; i++)
        {
            m_kVerts[m_dwVertexCount] = g_Varray[nFace, i];
            m_kUV[m_dwVertexCount] = g_VUV[i];
            m_dwVertexCount++;
        }

        // 
        int nd = nFace % 2;
        for (int i = 0; i < 6; i++)
        {
         
            m_Indexs[m_dwIndexCount] =  g_nIndexOrder[nd, i];
            m_dwIndexCount++;

        }
     


    }

    public Mesh GetMesh()
    {
        m_dwVertexCount = 0;
        m_dwIndexCount = 0;
        Mesh kMesh;
        kMesh = new Mesh();

        m_kVerts = new Vector3[6 * 4];
        m_kUV = new Vector2[6 * 4];
        m_Indexs = new int[6 * 6];

        MakeBox();

        kMesh.vertices = m_kVerts;
        kMesh.uv = m_kUV;
        kMesh.triangles = m_Indexs;
        kMesh.RecalculateNormals();


        return kMesh;


    }


}
