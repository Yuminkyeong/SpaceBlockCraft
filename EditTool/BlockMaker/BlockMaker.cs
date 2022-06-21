using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using CWEnum;
using System;

public class BlockMaker : MonoBehaviour {


    public CWAirObject m_kBuildObject;

    public GameObject m_gTarget;
    public GameObject m_gBox;
    GameObject m_gDir;

    
    


    const int MAXHEIGHT = 64;
    public const int m_nWidth = 256;
    #region 블록데이타
    byte[] BLOCKDATA = new byte[m_nWidth * m_nWidth * m_nWidth];
    void SetData(int x,int y,int z,int nBlock)
    {
        if (y >= MAXHEIGHT) return;
        int num = (x * m_nWidth + z) * m_nWidth + y;
        BLOCKDATA[num] =(byte)nBlock;
    }
    int GetData(int x,int y,int z)
    {
        int num = (x * m_nWidth + z) * m_nWidth + y;
        return (int)BLOCKDATA[num];
    }

    public Texture2D m_kTexture;

    public Texture2D[] m_kTexArry ;
    Vector3[] m_vColorArray;
    public Color[] m_kColorArray;






    #endregion

    List<Vector3> m_temp = new List<Vector3>();

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
        public Color m_Color;
    };

    List<BLockData> m_kData = new List<BLockData>();



    // 평면에 대해서 
    Vector3 GetVect(int nFace,int i,int j)
    {
        if(nFace==0)//-z 평면
        {
            return new Vector3(i, j, 0);
        }
        if (nFace == 1)//+z 평면
        {
            return new Vector3(i, j, m_nWidth);
        }
        if (nFace == 2)//+x 평면
        {
            return new Vector3(m_nWidth, i, j);
        }
        if (nFace == 3)//+x 평면
        {
            return new Vector3(0, i, j);
        }
        if (nFace == 4)//+y 평면
        {
            return new Vector3(i, m_nWidth, j);
        }
        if (nFace == 5)//+y 평면
        {
            return new Vector3(i, 0, j);
        }


        return Vector3.zero;
    }
    int GetValue(int nFace,Vector3 v)
    {
        if(nFace==0)
        {
            return (int)v.z;
        }
        if (nFace == 1)
        {
            return (int)v.z;
        }
        if (nFace == 2)
        {
            return (int)v.x;
        }
        if (nFace == 3)
        {
            return (int)v.x;
        }
        if (nFace == 4)
        {
            return (int)v.y;
        }
        if (nFace == 5)
        {
            return (int)v.y;
        }

        return 0;
    }

    Color GetTexture(Vector2 UV)
    {
        int x, y;
        int dx =m_kTexture.width;
        int dy = m_kTexture.height;
        x = (int)((float)dx * UV.x);
        y = (int)((float)dy * UV.y);
        return m_kTexture.GetPixel(x, y);

    }



    Vector3 Raycast(int nFace,int i,int j, ref Color kColr)
    {
        Vector3 vStart = GetVect(nFace, i, j);
        m_temp.Add(vStart);
        int nMask = (1 << 14);//10만 디텍트 
        RaycastHit hit;
        Ray ray = new Ray(vStart, m_vFaceArray[nFace]);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
        {
            kColr= GetTexture(hit.textureCoord);
            return hit.point;
        }
        return Vector3.zero;
    }

    Vector3 Raycast(int nFace,Vector3 vStart,ref Color kColr)
    {
        m_temp.Add(vStart);
        RaycastHit hit;
        int nMask = (1 << 14);//10만 디텍트 
        Ray ray = new Ray(vStart, m_vFaceArray[nFace]);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, nMask))
        {
            kColr = GetTexture(hit.textureCoord);
            return hit.point;
        }
        return Vector3.zero;
    }
    Vector3 NextVector(int nFace,Vector3 v)
    {
        v = v + m_vFaceArray[nFace];

        return v;
    }
    void MakeFaceBlock(int nFace)
    {
        // 한면에 대해서 충돌체크를 한다
        for(int i=0;i< m_nWidth; i++)
        {
            for (int j = 0; j < m_nWidth; j++)
            {
                Color kColor = new Color();
                Vector3 v = Raycast(nFace, i, j,ref kColor);
                if(v!=Vector3.zero)
                {
                    

                    BLockData bData = new BLockData
                    {
                        m_vPos = v,
                        m_Color = kColor
                    };

                    m_kData.Add(bData);

                    bool flag = true;
                    while(flag)
                    {

                      v = v + m_vFaceArray[nFace];
                      Vector3 vv = Raycast(nFace,v, ref kColor);
                      if (vv == Vector3.zero) break;
                      

                        BLockData bData2 = new BLockData
                        {
                            m_vPos = vv,
                            m_Color = kColor
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
        SkinnedMeshRenderer [] sr = m_gTarget.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var v in sr)
        {
            MeshCollider collider = v.gameObject.AddComponent<MeshCollider>();
            Mesh colliderMesh = new Mesh();
            v.BakeMesh(colliderMesh);
            collider.sharedMesh = colliderMesh;
            v.gameObject.layer = 14;
        }



        for (int i=0;i<6;i++)
        {
            
            MakeFaceBlock(i);
        }
    }

	// Use this for initialization
	void Start () {

        m_vColorArray = new Vector3[m_kColorArray.Length];
        for(int i=0;i<m_kColorArray.Length;i++)
        {
            m_vColorArray[i] = new Vector3(m_kColorArray[i].r, m_kColorArray[i].g, m_kColorArray[i].b);
        }


}


Color GetColor(int nBlock)
    {
        //return Color.black;
        float r = nBlock / (255 * 255);
        float g = (nBlock / 255) %255;
        float b = nBlock %255;
        return new Color(r/255f, g / 255f, b / 255f);

    }
    
    // 변환 배열을 블록데이타로 바꾼다

    // c#에서 enum 값을 스트링으로 바꾸는 개념이된다

    int ConvertToBlock(int n)
    {
        if (n >= m_kTexArry.Length) return 0;
        string sz = m_kTexArry[n].name.ToUpper();
        for (int i=0;i<(int)OLDBLOC.MAX;i++)
        {
            string szname = ((OLDBLOC)i).ToString();

            if (sz== szname.ToUpper())
            {
                return i;
            }
        }
        return 0;


    }
    //  블록데이타를 배열로 바꾼다
    int ConvertToArray(int nBlock)
    {
        OLDBLOC kValues = (OLDBLOC)nBlock;
        string szname = kValues.ToString();
        for (int i=0;i<m_kTexArry.Length;i++)
        {
            if(m_kTexArry[i].name==szname)
            {
                return i;
            }

        }
        return 0;
    }
    int GetBlock(Color kColor)
    {
        float r = kColor.r ;
        float g = kColor.g ;
        float b = kColor.b ;
        Vector3 vColor = new Vector3(r, g, b);

        float fMin = 1000f;
        int n = 0;
        for(int i=0;i<m_vColorArray.Length;i++)
        {
            if (m_vColorArray[i] == Vector3.zero) continue; //zero는 사용안함

            float fdist = Vector3.Distance(m_vColorArray[i], vColor);
            if(fdist< fMin)
            {
                fMin = fdist;
                n = i;
            }
        }
        return ConvertToBlock(n);
     
    }

    Texture2D GetBlockTexture(int nBlock)
    {
        int num = ConvertToArray(nBlock);
        return m_kTexArry[num];
    }

    void MakeBox()
    {
        foreach(var v in m_kData)
        {
            
            int x = (int)v.m_vPos.x;
            int y = (int)v.m_vPos.y;
            int z = (int)v.m_vPos.z;
            int nBlock = GetBlock(v.m_Color);
            SetData(x, y, z,nBlock);
        }


        

        AddBuillData();



        for (int z = 0; z < m_nWidth; z++)
        {
            for (int y = 0; y < m_nWidth; y++)
            {
                for (int x = 0; x < m_nWidth; x++)
                {
                    int nBlock = GetData(x, y, z);
                    if (nBlock > 0)
                    {
                        GameObject gg = Instantiate(m_gBox);
                        gg.SetActive(true);
                        gg.transform.parent = m_gDir.transform;
                        gg.transform.name = nBlock.ToString();
                        gg.transform.localPosition = new Vector3(x, y, z);
                        Renderer rr = gg.GetComponent<Renderer>();
                        rr.material.mainTexture = GetBlockTexture(nBlock);
                        //rr.material.color = GetColor(nBlock);

                    }
                }

            }

        }


    }
    void AddBuillData()
    {
        m_kBuildObject.Create(0);
        int minx, maxx;
        int miny, maxy;
        int minz, maxz;
        minx = 10000;
        maxx = 0;
        miny = 10000;
        maxy = 0;
        minz = 10000;
        maxz = 0;
        for (int y=0;y<m_nWidth;y++)
        {
            for (int z = 0; z < m_nWidth; z++)
            {
                for (int x = 0; x < m_nWidth; x++)
                {
                    int nBlock = GetData(x, y, z);
                    if(nBlock>0)
                    {
                        if (minx > x) minx = x;
                        if (maxx <= x) maxx = x;
                        if (miny > y) miny = y;
                        if (maxy <= y) maxy = y;
                        if (minz > z) minz = z;
                        if (maxz <= z) maxz = z;
                    }
                }

            }
        }

        int cx, cy, cz;
        cx = minx + (maxx - minx) / 2;
        cy = miny + (maxy - miny) / 2;
        cz = minz + (maxz - minz) / 2;
        List<Vector3> kSaveData = new List<Vector3>();

        for (int y = 0; y < m_nWidth; y++)
        {
            for (int z = 0; z < m_nWidth; z++)
            {
                for (int x = 0; x < m_nWidth; x++)
                {
                    int nBlock = GetData(x, y, z);
                    if (nBlock > 0)
                    {
                        //                        Vector3 v = new Vector3(x - cx, y - cy, z - cz);
                        //                      kSaveData.Add(v);
                        //cx = build cx
                        int tx = x  - cx;
                        int ty = y  - miny;
                        int tz = z -  cz;
                        m_kBuildObject.AddBlock(tx+32, ty, tz + 32,nBlock);
                    }
                }

            }
        }

        string szpath = string.Format("{0}/Resources/Gamedata/{1}.bytes", Application.dataPath, m_gTarget.name);
        m_kBuildObject.Save(szpath);

    }


    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),"Maker"))
        {

            m_vColorArray = new Vector3[m_kColorArray.Length];
            for (int i = 0; i < m_kColorArray.Length; i++)
            {
                m_vColorArray[i] = new Vector3(m_kColorArray[i].r, m_kColorArray[i].g, m_kColorArray[i].b);
            }


            for (int z = 0; z < m_nWidth; z++)
            {
                for (int y = 0; y < m_nWidth; y++)
                {
                    for (int x = 0; x < m_nWidth; x++)
                    {
                        SetData(x, y, z, 0);
                    }
                }
            }

            if (m_gDir!=null)
            {
                Destroy(m_gDir);
            }
            m_gDir = new GameObject();
            m_gDir.transform.parent = transform;
            m_gDir.transform.localPosition = new Vector3();
            m_gDir.name = "dir";
            MakeBlock();
            MakeBox();
          //  Save();
        }

    }

    // Update is called once per frame
    void Update () {

	
        //foreach(var v in m_temp)
        //{
        //    Debug.DrawRay(v, m_vFaceArray[0]);
        //}
        
	}










}
