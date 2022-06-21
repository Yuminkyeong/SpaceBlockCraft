using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;

public class MapMake : CWSingleton<MapMake>
{
    public MakeMapTool m_kMapTool;
    public int PatternID;

    public string m_szMask;
    public float m_fAmp;
    public int m_TreeSet;
    public int m_BuildSet;// 건물 셋
    public float m_fTreeRate;// 나무 분포율
    public bool m_bWater;
    public int m_nGrassHeight;// 잔디 최대 높이 
    public int m_nDirtHeight;// 진흙최대 높이 
    public int m_nSandHeight;// 모래 최대 높이 
    public float m_fPyungTanRate;// 평탄화률
    public string m_szPattern;// 분류
    public int m_ResHeight;// 자원이 생기는 시작위치 
    public int m_nHeight;// 최대 높이


    public void GetPatternData()
    {
        MakeMapTool.PLANETDATA kData = m_kMapTool.GetPatternData(PatternID);
        
        m_szMask = kData.m_szMask;
        m_fAmp = kData.m_fAmp;
        m_TreeSet = kData.m_TreeSet;
        m_BuildSet = kData.m_BuildSet;
        m_fTreeRate = kData.m_fTreeRate;
        m_bWater = kData.m_bWater;
        m_nGrassHeight = kData.m_nGrassHeight;
        m_nDirtHeight = kData.m_nDirtHeight;
        m_nSandHeight = kData.m_nSandHeight;
        m_fPyungTanRate = kData.m_fPyungTanRate;
        m_szPattern = kData.m_szPattern;
        m_ResHeight = kData.m_ResHeight;
        m_nHeight = kData.m_nHeight;  


    }
    public void CreateMap()
    {

        MakeMapTool.PLANETDATA kData = new MakeMapTool.PLANETDATA();
        kData.m_nID = PatternID;
        kData.m_szMask = m_szMask;
        kData.m_fAmp = m_fAmp;
        kData.m_TreeSet = m_TreeSet;
        kData.m_BuildSet = m_BuildSet;
        kData.m_fTreeRate = m_fTreeRate;
        kData.m_bWater = m_bWater;
        kData.m_nGrassHeight = m_nGrassHeight;
        kData.m_nDirtHeight = m_nDirtHeight;
        kData.m_nSandHeight = m_nSandHeight;
        kData.m_fPyungTanRate = m_fPyungTanRate;
        kData.m_szPattern = m_szPattern;
        kData.m_ResHeight = m_ResHeight;
        kData.m_nHeight = m_nHeight;

        m_kMapTool.MakeMapWork(kData);
    }


}
