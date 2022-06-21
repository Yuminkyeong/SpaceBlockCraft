using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CWCircleData {

    const int RAIDUIS = 256;

    struct POSDATA
    {
        public int len;
        public int x;
        public int y;
    }
    static POSDATA[] m_Data;
    static int Compare(POSDATA a, POSDATA b)
    {
        return a.len - b.len;
    }
    static void Create()
    {

        m_Data = new POSDATA[RAIDUIS * 2 * RAIDUIS*2];

        List<POSDATA> kList = new List<POSDATA>();
//        int num = 0;
        for (int y = 0; y < RAIDUIS * 2; y++)
        {
            for (int x = 0; x < RAIDUIS * 2; x++)
            {
                POSDATA nData = new POSDATA();
                nData.x = x - RAIDUIS;
                nData.y = RAIDUIS - y;
                //m_Data[num].x = tx;
                nData.len = (nData.x * nData.x + nData.y * nData.y);

                kList.Add(nData);
            }
        }

        kList.Sort(Compare);
        int num = 0;
        foreach(var v in kList)
        {
            m_Data[num++] = v;
        }

    }
    static public bool GetData(int num,ref int x,ref int y)
    {
        if(m_Data==null)
        {
            Create();
        }
        if (num >= m_Data.Length)
        {
            num = m_Data.Length - 1;
            return false;
        }

        x = m_Data[num].x;
        y = m_Data[num].y;
        return true;
    }
    static public void GetRandomData(int num, ref int x, ref int y)
    {
        if (m_Data == null)
        {
            Create();
        }
        num += (Random.Range(0, 4) - 2);
        if (num < 0) num = 0;
        if (num >= m_Data.Length) num = m_Data.Length-1;

        x = m_Data[num].x;
        y = m_Data[num].y;
    }

}
public class CWSphereData
{

    const int RAIDUIS = 32;

    struct POSDATA
    {
        public int len;
        public int x;
        public int y;
        public int z;
    }
    static POSDATA[] m_Data;
    static int Compare(POSDATA a, POSDATA b)
    {
        return a.len - b.len;
    }
    static void Create()
    {

        
        List<POSDATA> kList = new List<POSDATA>();
        for (int y = 0; y < RAIDUIS * 2; y++)
        {
            for (int x = 0; x < RAIDUIS * 2; x++)
            {
                for (int z = 0; z < RAIDUIS * 2; z++)
                {
                    //if (z < RAIDUIS) continue;

                    POSDATA nData = new POSDATA();
                    nData.x = x - RAIDUIS;
                    nData.y = RAIDUIS - y;
                    nData.z = z - RAIDUIS;
                    nData.len = (nData.x * nData.x + nData.y * nData.y + nData.z* nData.z);
                    kList.Add(nData);
                }
            }
        }
        kList.Sort(Compare);

        m_Data = new POSDATA[kList.Count];
        int num = 0;
        foreach (var v in kList)
        {
            m_Data[num++] = v;
        }

    }
    static public bool GetData(int num, ref int x, ref int y,ref int z)
    {
        if (m_Data == null)
        {
            Create();
        }
        if (num >= m_Data.Length)
        {
            num = m_Data.Length - 1;
            return false;
        }
            
        x = m_Data[num].x;
        y = m_Data[num].y;
        z = m_Data[num].z;
        return true;
    }
    static public void GetRandomData(int num, ref int x, ref int y,ref int z)
    {
        if (m_Data == null)
        {
            Create();
        }
        num += (Random.Range(0, 4) - 2);
        if (num < 0) num = 0;
        if (num >= m_Data.Length) num = m_Data.Length - 1;

        GetData(num,ref x,ref y,ref z);

    }

}
