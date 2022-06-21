using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using CWStruct;
public class MakeBlockTile : MonoBehaviour {

    // 블록타일을 만든다 
    // Use this for initialization
    const int IMAGESIZE = 1024;
    const int TILEWIDTH = 32;
    public string m_szName="terrain";
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void SaveImage(Texture2D tImage, string szname)
    {

        string szPath;
        szPath = string.Format("{0}/{1}", Application.persistentDataPath, szname);

        Texture2D kNew = new Texture2D(tImage.width, tImage.height);
        kNew.SetPixels32(tImage.GetPixels32());
        kNew.Apply(false);
        File.WriteAllBytes(szPath, kNew.EncodeToPNG());

    }

    public void MakeLODTile()
    {
        int dx = 16;
        string szName = "terrain_lod";
        string szPath = string.Format("{0}/Resources/Texture/{1}.png", Application.dataPath, szName);
        Texture2D kTerrain = new Texture2D(IMAGESIZE, IMAGESIZE);
        int sx = 0, sy = 0;
        int nWidth = IMAGESIZE / (16 * 32);//=4
        for (int i = 1; i < 8; i++)
        {
            BLOCKINFO v = CWArrayManager.Instance.m_kBlock[i];
            if (v.nID == 0) continue;

            sx = (i-1)% nWidth;
            sy = (i - 1) / nWidth;

            Texture2D kTile = CWResourceManager.Instance.GetTile(v.name);
            if (kTile == null)
            {
                print("file not found " + v.name);
                continue;
            }
            for (int y = 0; y < dx; y++)
            {
                for (int x = 0; x < dx; x++)
                {
                    kTerrain.SetPixels32(sx + x * TILEWIDTH, sy + y * TILEWIDTH, TILEWIDTH, TILEWIDTH, kTile.GetPixels32());
                }
            }


        }
        kTerrain.Apply(false);
        File.WriteAllBytes(szPath, kTerrain.EncodeToPNG());

    }

    void CopyPixel(Texture2D kTarget, int sx,int sy,int dx,int dy,Texture2D pSource)
    {

        
        float rx = (float)pSource.width/ TILEWIDTH;
        float ry = (float)pSource.height/ TILEWIDTH;

        for(int y=0;y<dy;y++)
        {
            for (int x = 0; x < dx; x++)
            {
                int tx, ty;
                tx = (int)((float)x* rx);
                ty = (int)((float)y * ry);
                Color kColor = pSource.GetPixel(tx, ty);
                kTarget.SetPixel(sx+x,sy+ y, kColor);
            }

        }

    }

    public void MakeTile()
    {

        
        string szPath = string.Format("{0}/Resources/Texture/{1}.png", Application.dataPath, m_szName);
        Texture2D kTerrain = new Texture2D(IMAGESIZE, IMAGESIZE);
        int sx=0, sy=0;

        //foreach (var v in CWArrayManager.Instance.m_kBlock)
        //for(int i=1;i<256;i++)
        for (int i = 1; i < 256; i++)
        {
            BLOCKINFO v=CWArrayManager.Instance.m_kBlock[i];
            if (v.nID == 0) continue;

            sx = v.x * TILEWIDTH; 
            sy = v.y * TILEWIDTH;

            Texture2D kTile= CWResourceManager.Instance.GetTile(v.name);
            if(kTile==null)
            {
                print("file not found " + v.name);
                continue;
            }
            CopyPixel(kTerrain, sx , sy , TILEWIDTH, TILEWIDTH, kTile);
            // 화이트 

            Texture2D kTileW = CWResourceManager.Instance.GetTile(v.name+"_w");
            if (kTileW == null)
            {
                kTileW = kTile;
                continue;
            }

            sx = v.white_x * TILEWIDTH;
            sy = v.white_y * TILEWIDTH;

            CopyPixel(kTerrain, sx, sy, TILEWIDTH, TILEWIDTH, kTileW);


        }

        kTerrain.Apply(false);
        File.WriteAllBytes(szPath, kTerrain.EncodeToPNG());

        print("작업 완료!!!!" );
    }

    // 타일로 옮긴다
    void CopyFile(string szDest,Texture2D kSource)
    {
        
        Texture2D kNewTile = new Texture2D(TILEWIDTH, TILEWIDTH);
        CopyPixel(kNewTile, 0,0, TILEWIDTH, TILEWIDTH, kSource);
        kNewTile.Apply(false);
        File.WriteAllBytes(szDest, kNewTile.EncodeToPNG());


    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),"make"))
        {
            MakeTile();
        }
    }




}
