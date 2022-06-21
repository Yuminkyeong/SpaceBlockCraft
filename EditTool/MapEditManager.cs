

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CWStruct;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using System.Diagnostics;
using CWEnum;

public class MapEditManager : CWFaceMap
{

    // 평지 만들기
    public void MakePlane()
    {
        for(int z=0;z< WORLDSIZE;z++)
        {
            for (int x = 0; x < WORLDSIZE; x++)
            {
                UpdateBlock(x, 0, z, 1);
                UpdateBlock(x, 1, z, 1);
            }

        }

    }

    public int m_nResblockCnt;
    public int m_nGemblockCnt;
    public int m_nBlockCount;
    public int[] m_nBlockCnt = new int[256];
   


    public void MakeMeshUP()
    {
        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;
        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
               
                m_kSellGroup[num].MakeMeshVertex();
            }
        }
    }


    public void MakeAssetTest()
    {
#if UNITY_EDITOR     

        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;

        Mesh kMesh = new Mesh();
        CombineInstance[] combine = new CombineInstance[dx * dx];

        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
                combine[num].mesh = m_kSellGroup[num].CombineMesh();
                combine[num].transform = Matrix4x4.Translate(new Vector3(x * CWSellGroup.SELLSIZE, 0, z * CWSellGroup.SELLSIZE));
            }
        }

        kMesh.CombineMeshes(combine);
        kMesh.RecalculateNormals();
        string szPath = string.Format("Assets/Resources/MeshAsset/T_{0}.asset", m_nMapID);
        AssetDatabase.CreateAsset(kMesh, szPath);
#endif

    }
    public void MakeAsset()
    {
#if UNITY_EDITOR     

        int dx = WORLDSIZE / CWSellGroup.SELLSIZE;

        Mesh kMesh = new Mesh();
        CombineInstance[] combine = new CombineInstance[dx * dx];

        for (int z = 0; z < dx; z++)
        {
            for (int x = 0; x < dx; x++)
            {
                int num = z * dx + x;
                combine[num].mesh = m_kSellGroup[num].CombineMesh();
                combine[num].transform = Matrix4x4.Translate(new Vector3(x * CWSellGroup.SELLSIZE, 0, z * CWSellGroup.SELLSIZE));
            }
        }

        kMesh.CombineMeshes(combine);
        kMesh.RecalculateNormals();
        string szPath = string.Format("Assets/Resources/MeshAsset/{0}.asset", m_nMapID);
        AssetDatabase.CreateAsset(kMesh, szPath);
#endif

    }

    public override void MakeMesh()
    {
        base.MakeMesh();
        MakeHeight();
    }
    protected override int ConvertBlock(int nBlock,int h)
    {
        if(nBlock>0) m_nBlockCount++;
        m_nBlockCnt[nBlock]++;

        if (nBlock==(int)OLDBLOC.ResBlock)        m_nResblockCnt++;
        if (nBlock == (int)OLDBLOC.GemBlock) m_nGemblockCnt++;
        if (nBlock == (int)OLDBLOC.GoldBlock) m_nGemblockCnt++;


        

        return base.ConvertBlock(nBlock,h);
    }
}
