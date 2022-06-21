using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;
using CWEnum;
/*
 * 복셀 데이타를 찾는다. 
 * */

public class MakeVoxel
{

    public  bool m_bAirmode;// 비행기 모드 인가?
   
    public delegate  void __SetBlock(int x, int y, int z, int nBlock);

    __SetBlock CBSetBlock;


    
    const int DELVALUE = 100;
    int WSIZE = 64;

    int m_nLayer;

    MAPBLOCK[] m_kArray ;
    Texture2D m_kTexture;// 모델 텍스쳐

    int GetBlockNumber(int x,int y,int z)
    {
        int num = (x * WSIZE + z) * WSIZE + y;
        return num;
    }
    int GetBlock(int x,int y,int z)
    {
        int num=GetBlockNumber(x, y, z);
        return m_kArray[num].nblock;
    }
    MAPBLOCK GetVoxel(int x,int y,int z)
    {
        int num = GetBlockNumber(x, y, z);
        return m_kArray[num];
    }
    void SetData(int x,int y,int z,Color kColor)
    {
      
        int num = GetBlockNumber(x, y, z);
        if(num<0)
        {
            return;
        }
        if(num>= m_kArray.Length)
        {
            return;
        }
        if(m_bAirmode)
        {
            if (Color.clear == kColor)
            {
                m_kArray[num].nblock = DELVALUE;
            }
            else
            {
                m_kArray[num].nblock = (int)CWGlobal.ConvertColorItem(kColor);
            }

            
        }
        else
        {
            if(Color.clear==kColor)
            {
                m_kArray[num].nblock = DELVALUE;
            }
            else
            {
                int nColor = CWGlobal.ConvertColorBlock(kColor);
                if (nColor == 0)
                {
                    nColor = (int)OLDBLOC.stone;
                }
                m_kArray[num].nblock = nColor;

            }


        }
    }
    Vector3[] m_vFaceArray =
{
        new Vector3(0, 0, 1),// 
        new Vector3(0, 0, -1),
        new Vector3(-1, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, -1, 0),
        new Vector3(0, 1, 0),

    };

    Vector3 GetVect(int nFace, float i, float j)
    {
        if (nFace == 0)//-z 평면
        {
            return new Vector3(i, j, 0);
        }
        if (nFace == 1)//+z 평면
        {
            return new Vector3(i, j,WSIZE);
        }
        if (nFace == 2)//+x 평면
        {
            return new Vector3(WSIZE, i, j);
        }
        if (nFace == 3)//+x 평면
        {
            return new Vector3(0, i, j);
        }
        if (nFace == 4)//+y 평면
        {
            return new Vector3(i, WSIZE , j);
        }
        if (nFace == 5)//+y 평면
        {
            return new Vector3(i, 0, j);
        }


        return Vector3.zero;
    }
    Vector3 Raycast(int nFace, float i, float j, ref Color kColr)
    {
        Vector3 vStart = GetVect(nFace, i, j);
        if(m_bAirmode)
        {
            vStart.x -= 16;
            vStart.z -= 16;
        }
        vStart.x += 0.5f;
        vStart.y += 0.5f;
        vStart.z += 0.5f;

        int nMask = (1 << m_nLayer);//10만 디텍트 
        RaycastHit hit;
        Ray ray = new Ray(vStart, m_vFaceArray[nFace]);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
        {
            if(m_kTexture==null)
            {
                Renderer rr = hit.collider.GetComponent<Renderer>();
                if (rr != null)
                {
                    kColr = rr.material.color;
                }

            }
            else
            {
                kColr = GetTexture(hit.textureCoord);
            }

            Debug.Log(string.Format("p= {0}",hit.point));
            if (m_bAirmode)
            {
                return hit.point + new Vector3(16,0,16);
            }
            return hit.point;
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

    public static float GetDistColor(Color a1, Color a2)
    {
        Vector3 v1 = new Vector3(a1.r, a1.g, a1.b);
        Vector3 v2 = new Vector3(a2.r, a2.g, a2.b);
        v1.Normalize();
        v2.Normalize();

        return Vector3.Distance(v1, v2);
    }
    





    
    void ConvertDelData(int nFace, float i, float j,Vector3 v,Color kColor)
    {

        int vx = (int)(v.x+0.5f);
        int vy = (int)(v.y+0.5f);
        int vz = (int)(v.z+0.5f);
        

        if(nFace==0)//-z평면
        {
            int x = 0, y = 0;
            x = (int)(i+0.5f);
            y = (int)(j + 0.5f);
            for (int z=0;z<vz;z++)
            {
                DelData(x, y, z);
            }
            

            if (kColor == Color.clear) return;

            
            SetData(x, y, vz, kColor);

        }
        if (nFace == 1)//+z평면
        {
            int x = 0, y = 0;
            x = (int)(i + 0.5f);
            y = (int)(j + 0.5f);
            for (int z = WSIZE-1; z >= vz; z--)
            {
                DelData(x, y, z);
            }
            if (kColor == Color.clear) return;

            
            SetData(x, y, vz-1, kColor);

        }

        if (nFace == 2)//+x평면
        {
            int y = 0, z = 0;
            y = (int)(i + 0.5f);
            z = (int)(j + 0.5f);
            for (int x = WSIZE - 1; x >= vx; x--)
            {
                DelData(x, y, z);
            }
            
            if (kColor == Color.clear) return;

            
            SetData(vx-1, y, z , kColor);


        }
        if (nFace == 3)//-x평면
        {
            int y = 0, z = 0;
            y = (int)(i + 0.5f);
            z = (int)(j + 0.5f);
            for (int x = 0; x < vx; x++)
            {
                DelData(x, y, z);
            }
            if (kColor == Color.clear) return;

            
            SetData(vx , y, z, kColor);


        }
        if (nFace == 4)//+y평면
        {
            
            int x = 0, z = 0;
            x = (int)(i + 0.5f);
            z = (int)(j + 0.5f);
          

            for (int y = WSIZE - 1; y >= vy; y--)
            {
                DelData(x, y, z);
            }
            if (kColor == Color.clear) return;

            
            SetData(x, vy-1, z, kColor);

        }
        if (nFace == 5)//+y평면
        {
            int x = 0, z = 0;
            x = (int)(i + 0.5f);
            z = (int)(j + 0.5f);
            for (int y = 0; y < vy; y++)
            {
                DelData(x, y, z);
            }
            if (kColor == Color.clear) return;


            SetData(x, vy , z, kColor);

        }


    }
    void DelData(int x,int y,int z)
    {
        
        SetData(x, y, z, Color.clear);
    }
    // 개념
    // i,j 평면 시작에서 끝으로 향하는 직선 충돌
    // 충돌된 좌표전까지 모두 삭제
    // 나머지는 삭제 하지 않는다
    // 칼러가 나오면 칼러 저장 

    void MakeFaceBlock(int nFace)
    {
        // 한면에 대해서 충돌체크를 한다

        for (float j = 0 ; j < WSIZE ; j +=0.5f)
        {
            for (float i = 0; i < WSIZE; i += 0.5f)
            {
                Color kColor = Color.clear;
                Vector3 v = Raycast(nFace, i, j, ref kColor);

                if (v == Vector3.zero)
                {
                 
                    if (nFace==0)
                    {
                        v.z = WSIZE;

                    }
                    if (nFace == 1)
                    {
                        v.z = 0;
                    }
                    if (nFace == 2)
                    {
                        v.x = 0;
                    }
                    if (nFace == 3)
                    {
                        v.x = WSIZE;
                    }
                    if (nFace == 4)
                    {
                        v.y = 0;
                    }
                    if (nFace == 5)
                    {
                        
                        v.y = WSIZE;
                    }



                }

                ConvertDelData(nFace, i, j, v,kColor);


            }
        }
    }



    void AddData(int x,int y,int z, MAPBLOCK  v)
    {
        
        CBSetBlock(x, y, z, v.nblock);

    }
    
    public void Make(bool bAirmode, int nSize, GameObject gTarget, Texture2D kTexture, __SetBlock _SetBlock)
    {
        m_bAirmode = bAirmode;
        WSIZE = nSize;
        m_kArray = new MAPBLOCK[WSIZE * WSIZE * WSIZE];

        CBSetBlock = _SetBlock;

        m_kTexture = kTexture;

        m_nLayer = LayerMask.NameToLayer("Edit");
    
        Renderer[] array = gTarget.GetComponentsInChildren<Renderer>();
        foreach (var v in array)
        {

            v.gameObject.AddComponent<MeshCollider>();
            v.gameObject.layer = m_nLayer;
        }
        SkinnedMeshRenderer[] sr = gTarget.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var v in sr)
        {
            MeshCollider collider = v.gameObject.AddComponent<MeshCollider>();
            Mesh colliderMesh = new Mesh();
            v.BakeMesh(colliderMesh);
            collider.sharedMesh = colliderMesh;
            v.gameObject.layer = m_nLayer;
        }


        
        for(int z=0;z<WSIZE;z++)
        {
            for (int y = 0; y < WSIZE; y++)
            {
                for (int x = 0; x < WSIZE; x++)
                {
                    int num= GetBlockNumber(x, y, z);
                    m_kArray[num] = new MAPBLOCK
                    {
                        x = x,
                        y = y,
                        z = z,
                        nblock =(int) OLDBLOC.WHITE
                    };
                    if(m_bAirmode)
                    {
                        m_kArray[num].nblock = (int)COLORNUMBER.NONE; 
                    }
                }

            }

        }

        for (int i=0;i<5;i++)
        {
            MakeFaceBlock(i);
        }

        for (int z=1;z<WSIZE-1;z++)
        {
            for (int y = 0; y < WSIZE-1; y++)
            {
                for (int x = 1; x < WSIZE-1; x++)
                {

                    if(GetBlock(x,y,z)==DELVALUE)
                    {
                        continue;
                    }
                    AddData(x, y, z, GetVoxel(x,y,z));
                }

            }

        }
    }

}
