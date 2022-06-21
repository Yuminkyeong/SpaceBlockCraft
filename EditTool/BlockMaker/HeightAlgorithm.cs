using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
/*
 * 개념 
 *  
 *  주변을 검색해서 자신과의 크기를 X 이하로 맞춘다.
 * 
 *  
 * 
 * */


public class HeightAlgorithm
{




    public float m_fAmp;
    
    int m_nWidth = 100;
    int m_nHeight = 100;

    float[] m_fHeights;

    public float GetHeight(int x,int y)
    {
        int num = y * m_nWidth + x;
        return m_fHeights[num];
    }
    Color GetColor( int x, int y, Color[] kcolor)
    {
        int num = y * m_nWidth + x;
        return kcolor[num];
    }


    void SetHeight(int x,int y,float fval)
    {
        int num = y * m_nWidth + x;
        m_fHeights[num] = fval;


    }
    void Fill(int sx,int sy,int nRadius,float fvalues)
    {
        for(int y = -nRadius;y<=nRadius;y++)
        {
            for (int x = -nRadius; x <= nRadius; x++)
            {

                int tx = x + sx;
                int ty = y + sy;
                if (tx < 0) continue;
                if (ty < 0) continue;
                if (tx >= m_nWidth) continue;
                if (ty >= m_nWidth ) continue;

                int r =(int) Mathf.Sqrt(x * x + y * y);
                if (r > nRadius) continue;
                if (fvalues == 0) continue;
    
                float frate = 1-(float)r / nRadius;
                float fval = fvalues * frate;

                float ff = GetHeight(tx, ty);
                if (ff >= fval) continue;

              
                SetHeight(tx, ty,fval);


            }

        }

    }
    /*
     * 색 정의
     * 
     * 0,0,255 => 높이값 1 20 
     * 0,0,128 => 높이값 1 10 
     * 0,0,64 => 높이값 1 1 
     * 255,0,0 => 높이값 0.5 20 
     * 128,0,0 => 높이값 0.5 10 
     * 64,0,0 => 높이값 0.5 1 
     * 
     * 0,255,0 => 높이값 0.5 1 
     * 0,128,0 => 높이값 0.5 1 
     * 0,64,0 => 높이값 0.5 1 
     * */

    int GetRaidus(Color kColor)
    {
        

        if (CWLib.CompareColor(kColor, new Color(1, 0, 0)))
        {
            return 20;
        }
        if (CWLib.CompareColor(kColor, new Color(0.5f, 0, 0)))
        {
            return 10;
        }
        if (CWLib.CompareColor(kColor, new Color(0.25f, 0, 0)))
        {
            return 1;
        }

        // 노란색 계열
        if (CWLib.CompareColor(kColor, new Color(1, 1, 0, 0.5f)))
        {
            return 20;
        }
        // 푸른색 계열
        if (CWLib.CompareColor(kColor, new Color(0, 1, 0)))
        {
            return 20;
        }
        if (CWLib.CompareColor(kColor, new Color(0, 0.5f, 0)))
        {
            return 10;
        }
        if (CWLib.CompareColor(kColor, new Color(0, 0.25f, 0)))
        {
            return 1;
        }


        return 0;
    }

    float  GetHeight(int x,int y,Color kColor)
    {

        float fh = Mathf.PerlinNoise(x * m_fAmp, y * m_fAmp);

        if (CWLib.CompareColor(kColor, new Color(1,0,0)))
        {
            return kColor.a;
        }
        if (CWLib.CompareColor(kColor, new Color(0.5f, 0, 0)))
        {
            return kColor.a;
        }
        if (CWLib.CompareColor(kColor, new Color(0.25f, 0, 0)))
        {
            return kColor.a;
        }

        // 노란색 계열
        if (CWLib.CompareColor(kColor, new Color(1, 1, 0,0.5f)))
        {
            return kColor.a;
        }
        // 푸른색 계열
        if (CWLib.CompareColor(kColor, new Color(0, 1, 0)))
        {
            return kColor.a*fh;
            //return 0.5f;
        }
        if (CWLib.CompareColor(kColor, new Color(0, 0.5f, 0)))
        {
            return kColor.a * fh*0.5f;
        }
        if (CWLib.CompareColor(kColor, new Color(0,0.25f, 0)))
        {
            return kColor.a * fh * 0.25f;
        }


        return 0;
    }




    public void SetArray(Color[] bdata,int dx,int dy)
    {


        m_fHeights = new float[bdata.Length];
        m_nWidth = dx;
        m_nHeight = dy;

       

        for (int y = 0; y < m_nWidth; y++)
        {
            for (int x = 0; x < m_nWidth; x++)
            {
                Color kColor = GetColor(x, y, bdata);
                int r = GetRaidus(kColor);
                if(r>0)
                {
                    float h = GetHeight(x,y,kColor);
                    Fill(x, y, r,h);
                }
                    
            }

        }


    }

    // 지하 구조 및 표면 구조


}
