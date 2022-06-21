#define ASSETDATA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWUnityLib;
using System.Threading;
using System;
using CWStruct;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CWMeshManager : CWManager<CWMeshManager>
{

    
    #region 메쉬를 만든다


    int[,] g_vNormal =
    {
            {0,0,-1},
            {0,0, 1},
            {1,0, 0},
            {-1,0, 0},
            {0,1, 0},
            {0,-1, 0},
    };

    int m_nLevel;
    int WORLDSIZE = 256;
    byte[] m_bBlockData;
    byte[] m_bColorData;
    public Color GetColor(int x, int y, int z)
    {
        if (x < 0) return Color.white;
        if (y < 0) return Color.white;
        if (z < 0) return Color.white;
        if (m_bColorData == null) return Color.white;
        int num = (x * WORLDSIZE + z) * CWGlobal.WD_WORLD_HEIGHT + y;
        return CWGlobal.GetColor((COLORNUMBER)m_bColorData[num]);

    }

    int GetBlock(int x,int y,int z)
    {
        if (m_bBlockData == null) return 0;

        if (x < 0) return 0;
        if (y < 0) return 0;
        if (z < 0) return 0;
        int num = (x * WORLDSIZE + z) * CWGlobal.WD_WORLD_HEIGHT + y;
        if(num>= WORLDSIZE * WORLDSIZE * CWGlobal.WD_WORLD_HEIGHT)
        {
            return 0;
        }
        return (int)m_bBlockData[num];
    }
    
    
    bool LoadMap(int nID,string szlocalfile)
    {

        CWJSon JSonData = new CWJSon();
        if (CWGlobal.CheckLoacalfile(szlocalfile))
        {
            JSonData.LoadLocal(szlocalfile);
        }
        else
        {
            string szpath = string.Format("MapData/Map_{0}", nID);
            if (JSonData.Load(szpath) == null) return false;
        }

        WORLDSIZE = JSonData.GetInt("Worldsize");

        m_bBlockData = JSonData.GetBytes("Blockdata");
        if (m_bBlockData == null) return false;

        m_bColorData = JSonData.GetBytes("Colordata");

        return true;
    }

    bool IsBlockFace(int nFace, int x, int y, int z)
    {
        int tx = x + g_vNormal[nFace, 0]* m_nLevel;
        int ty = y + g_vNormal[nFace, 1] * m_nLevel;
        int tz = z + g_vNormal[nFace, 2] * m_nLevel;
        int tblock = GetBlock(tx, ty, tz);
        if (tblock <= 0) return false;

        return true;

    }
    void MakeBox(CWMakeMesh kMakeMesh,int x, int y, int z, int nBlock)
    {

        for (int i = 0; i < 6; i++)
        {
            if (IsBlockFace(i, x, y, z)) continue; // 옆면이 존재한다면 통과  //
            kMakeMesh.MakeFace(i, x, y, z, m_nLevel, m_nLevel, m_nLevel, nBlock, GetBlock, GetColor, false);
        }

    }


    Mesh MakeMesh(int nMapID, CWMakeMesh kMakeMesh)
    {

        

        for (int z = 0; z < WORLDSIZE ; z+= m_nLevel)
        {
            for (int y = 0; y < CWGlobal.WD_WORLD_HEIGHT; y += m_nLevel)
            {
                for (int x = 0; x < WORLDSIZE; x += m_nLevel)
                {
                    int nblock =  GetBlock(x, y, z);
                    if (nblock <= 0) continue;
                    

                    MakeBox(kMakeMesh,x, y, z, nblock);

                }
            }
        }
        Mesh kMesh = kMakeMesh.GetMesh();

        
        return kMesh;

    }
    #endregion

    Dictionary<string, Mesh> m_kMeshList = new Dictionary<string, Mesh>();

    

    Mesh _GetMesh(int nMapID, string szlocalfile,bool bUpdated)
    {
        if (nMapID == 0) return null;
        string szFile = nMapID.ToString()+"_"+m_nLevel.ToString();

        if(bUpdated)
        {
            DeleteMesh(szFile);
            LoadMap(nMapID, szlocalfile);
        }
        else
        {
            if (m_kMeshList.ContainsKey(szFile))
            {
                return m_kMeshList[szFile];
            }
            LoadMap(nMapID, szlocalfile);
        }

        CWMakeMesh kMakeMesh = new CWMakeMesh();
        Mesh kMesh = MakeMesh(nMapID, kMakeMesh);
        if (kMesh == null)
        {
            return null;
        }
        kMakeMesh = null;
        m_kMeshList.Add(szFile, kMesh);
        return kMesh;


    }
    public Mesh GetAssetMesh(int nMapID)
    {
        string szfile = nMapID.ToString();
        return CWResourceManager.Instance.GetMeshAsset(szfile);

    }
    public Mesh GetMesh_MyPlanet()
    {
        m_nLevel = 1;
        

        return _GetMesh(CWGlobal.MYPLANETMAPID, CWGlobal.GetMyLocalName(), true);
    }


    public Mesh GetMeshLOD2(int nMapID, string szlocalfile,bool bupdated=false)
    {
        if (CWGlobal.g_SystemState==CWGlobal.SYSTEMSTATE.BAD)
        {
            return GetAssetMesh(nMapID);
        }
        if (CWGlobal.g_SystemState == CWGlobal.SYSTEMSTATE.GOOD)
        {
            m_nLevel = 4;
        }
        else
        {
            m_nLevel = 2;
        }

        return _GetMesh(nMapID,szlocalfile, bupdated);
    }

    public Mesh GetMesh(int nMapID)
    {
        if (CWGlobal.g_SystemState == CWGlobal.SYSTEMSTATE.BEST)
        {
            m_nLevel = 8;// 최상위는 레벨 8로 만든다 
            return _GetMesh(nMapID,"",false);
        }

        return GetAssetMesh(nMapID);
    }
    void DeleteMesh(string szFile)
    {
        
        if (m_kMeshList.ContainsKey(szFile))
        {
            Mesh kMesh= m_kMeshList[szFile];
            if(kMesh!=null)
                GameObject.DestroyImmediate(kMesh);
            m_kMeshList.Remove(szFile);

            
        }

    }
    
        
    
    
  

}
