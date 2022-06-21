using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using CWEnum;
using System;

public class BlockConverter : MonoBehaviour {

    #region 초기화
    public delegate void __SetBlock(int x, int y, int z, int nBlock);

    __SetBlock CBSetBlock;

    public GameObject m_gTarget;// 변환 시킬 모델
    public Texture2D m_kTexture;// 모델 텍스쳐

    const int MAXHEIGHT = 64;
    public int m_nWidth = 256;

    
    Vector3[] m_vColorArray;
    public Color[] m_kColorArray;

    private void Start()
    {
        m_vColorArray = new Vector3[m_kColorArray.Length];
        for (int i = 0; i < m_kColorArray.Length; i++)
        {
            m_vColorArray[i] = new Vector3(m_kColorArray[i].r, m_kColorArray[i].g, m_kColorArray[i].b);
        }

    }


    #endregion

    #region 변환 알고리즘

    public float m_fMultiValues = 2f;

    Vector3[] m_vFaceArray =
    { 
        new Vector3(0, 0, 1),// 
        new Vector3(0, 0, -1),
        new Vector3(-1, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, -1, 0),
        new Vector3(0, 1, 0),

    };
    class BLockData
    {
        public Vector3 m_vPos;
        public Color m_kColor;
    };

    List<BLockData> m_kData = new List<BLockData>();


    // 평면에 대해서 
    Vector3 GetVect(int nFace, float i, float j)
    {
        if (nFace == 0)//-z 평면
        {
            return new Vector3(i, j, -m_nWidth/2);
        }
        if (nFace == 1)//+z 평면
        {
            return new Vector3(i, j, m_nWidth/2);
        }
        if (nFace == 2)//+x 평면
        {
            return new Vector3(m_nWidth / 2, i, j);
        }
        if (nFace == 3)//+x 평면
        {
            return new Vector3(-m_nWidth / 2, i, j);
        }
        if (nFace == 4)//+y 평면
        {
            return new Vector3(i, m_nWidth / 2, j);
        }
        if (nFace == 5)//+y 평면
        {
            return new Vector3(i, -m_nWidth / 2, j);
        }


        return Vector3.zero;
    }



    Color GetTexture(Vector2 UV)
    {
        if (m_kTexture == null) return Color.green;
        int x, y;
        int dx = m_kTexture.width;
        int dy = m_kTexture.height;
        x = (int)((float)dx * UV.x);
        y = (int)((float)dy * UV.y);
        return m_kTexture.GetPixel(x, y);

    }



    Vector3 Raycast(int nFace, float i, float j,ref Color kColor)
    {
        Vector3 vStart = GetVect(nFace, i, j);
        
        int nMask = (1 << 14);//10만 디텍트 
        RaycastHit hit;
        Ray ray = new Ray(vStart, m_vFaceArray[nFace]);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
        {

            if (m_kTexture == null)
            {
                Renderer rr = hit.collider.GetComponent<Renderer>();
                if (rr != null)
                {
                    kColor = rr.material.color;
                }

            }
            else
            {
                kColor = GetTexture(hit.textureCoord);
            }

            return hit.point;
        }
        return Vector3.zero;
    }

    Vector3 Raycast(int nFace, Vector3 vStart, ref Color kColor)
    {
     
        RaycastHit hit;
        int nMask = (1 << 14);//10만 디텍트 
        Ray ray = new Ray(vStart, m_vFaceArray[nFace]);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
        {
            if (m_kTexture == null)
            {
                Renderer rr = hit.collider.GetComponent<Renderer>();
                if (rr != null)
                {
                    kColor = rr.material.color;
                }

            }
            else
            {
                kColor = GetTexture(hit.textureCoord);
            }

            return hit.point;
        }
        return Vector3.zero;
    }
    Vector3 NextVector(int nFace, Vector3 v)
    {
        v = v + m_vFaceArray[nFace];

        return v;
    }
    void MakeFaceBlock(int nFace)
    {
        // 한면에 대해서 충돌체크를 한다
        
        for (float i = 0; i < m_nWidth; i+=(1f/ m_fMultiValues))
        {
            for (float j =0; j < m_nWidth; j += (1f / m_fMultiValues))
            {

                Color kColor = new Color();
                Vector3 v = Raycast(nFace, i, j,ref kColor);
                if (v != Vector3.zero)
                {
                    BLockData bData = new BLockData
                    {
                        m_vPos = v,
                        m_kColor = kColor
                    };
                   

                    m_kData.Add(bData);

                    bool flag = true;
                    while (flag)
                    {

                        v = v + m_vFaceArray[nFace];
                        Vector3 vv = Raycast(nFace, v, ref kColor);
                        if (vv == Vector3.zero) break;


                        BLockData bData2 = new BLockData
                        {
                            m_vPos = vv,
                            m_kColor = kColor,
                        };
                        m_kData.Add(bData2);

                    }

                }


            }
        }
    }


    void MakeBlock()
    {
        m_kData.Clear();
        Renderer[] array = m_gTarget.GetComponentsInChildren<Renderer>();
        foreach (var v in array)
        {

            v.gameObject.AddComponent<MeshCollider>();
            v.gameObject.layer = 14;
        }
        SkinnedMeshRenderer[] sr = m_gTarget.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var v in sr)
        {
            MeshCollider collider = v.gameObject.AddComponent<MeshCollider>();
            Mesh colliderMesh = new Mesh();
            v.BakeMesh(colliderMesh);
            collider.sharedMesh = colliderMesh;
            v.gameObject.layer = 14;
        }



        for (int i = 0; i < 6; i++)
        {

            MakeFaceBlock(i);
        }
    }


    #endregion
    #region 외부 접근함수
    

    public void Converter(MapEditManager kMap, Texture2D kTexture, __SetBlock _setBock)
    {

        m_nWidth = kMap.WORLDSIZE;

        m_kTexture = kTexture;

        m_vColorArray = new Vector3[m_kColorArray.Length];
        for (int i = 0; i < m_kColorArray.Length; i++)
        {
            m_vColorArray[i] = new Vector3(m_kColorArray[i].r, m_kColorArray[i].g, m_kColorArray[i].b);
        }


        MakeBlock();
        foreach (var v in m_kData)
        {

            int x = (int)v.m_vPos.x;
            int y = (int)v.m_vPos.y;
            int z = (int)v.m_vPos.z;
            int nBlock = (int)OLDBLOC.stone;

            nBlock = CWGlobal.ConvertColorBlock(v.m_kColor);
            if(nBlock == 0)
            {
                nBlock = (int)OLDBLOC.stone;
            }
            _setBock(x, y, z, nBlock);
        }

    }
    //public void ConverterAirObject(AirPlaneEdit kEdit)
    //{


    //    m_gTarget.transform.position = new Vector3(16, 0, 16);

    //    m_nWidth = 32;

    //    m_vColorArray = new Vector3[m_kColorArray.Length];
    //    for (int i = 0; i < m_kColorArray.Length; i++)
    //    {
    //        m_vColorArray[i] = new Vector3(m_kColorArray[i].r, m_kColorArray[i].g, m_kColorArray[i].b);
    //    }


    //    MakeBlock();
    //    foreach (var v in m_kData)
    //    {

    //        int x = (int)v.m_vPos.x;
    //        int y = (int)v.m_vPos.y;
    //        int z = (int)v.m_vPos.z;
    //        kEdit.Convert_SetBlock(x-16, y, z - 16);
    //    }

    //    m_gTarget.transform.position = Vector3.zero;
    //}


    #endregion
}
