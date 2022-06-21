using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;

public class CWBlockGroup
{

    
    public int m_nDelX;
    public int m_nDelZ;
    // 충돌박스를 구한다 
    CWFaceMap m_kMap;

    public CWBlockGroup(CWFaceMap kMap)
    {
        m_kMap = kMap;

    }
    public CWBlockGroup(int nSize)
    {
        m_nDelX = nSize;
        m_nDelZ = nSize;
        m_bData = new byte[m_nDelX * CWGlobal.WD_WORLD_HEIGHT * m_nDelZ];
    }

    byte[] m_bData;

    public void Load(string szfile)
    {
        
        string szPath = CWLib.pathForDocumentsPath();
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/BlockGroup/{1}.bytes", szPath, szfile);
        if (cf.Load(szpath))
        {
            m_nDelX = cf.GetInt();
            m_nDelZ = cf.GetInt();
            m_bData = cf.GetBuffer();

        }

    }
    public void Save(string szfile)
    {
        string szPath = CWLib.pathForDocumentsPath();
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/BlockGroup/{1}.bytes", szPath, szfile);

        cf.PutInt(m_nDelX);
        cf.PutInt(m_nDelZ);
        cf.PutBuffer(m_bData);
        cf.Save(szpath);

    }
    public void ApplyMap(int sx,int sy,int sz, dlSetblock _Setblock)
    {
        if (m_bData == null) return;
        for(int z=0;z<m_nDelZ;z++)
        {
            for (int y = 0; y < CWGlobal.WD_WORLD_HEIGHT; y++)
            {
                for (int x = 0; x < m_nDelX; x++)
                {
                    int num = (x * m_nDelZ + z) * CWGlobal.WD_WORLD_HEIGHT + y;

                    if(m_bData[num]>0)
                    {
                        _Setblock(x + sx - m_nDelX / 2, y+sy, z + sz - m_nDelZ / 2, m_bData[num]);
                    }
                    
                }

            }

        }

    }
    public void TakeMap(int sx,int sy,int sz,int ex,int ey,int ez)
    {
        m_nDelX = (ex - sx)+1;
        m_nDelZ = (ez - sz) + 1;

        m_bData = new byte[m_nDelX* CWGlobal.WD_WORLD_HEIGHT * m_nDelZ];

        for (int z=sz;z<ez;z++)
        {
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    if (y < 0) continue;
                    int nBlock= m_kMap.GetBlock(x, y, z);
                    AddData(x-sx, y, z-sz, nBlock);
                }
            }
        }

    }
    public void AddData(int x,int y,int z,int nBlock)
    {
        if (x < 0) return;
        if (y < 0) return;
        if (y >= CWGlobal.WD_HEIGHT) return;
        if (z < 0) return;
        if (x >= m_nDelX) return;
        if (z >= m_nDelZ) return;
        int num = (x * m_nDelZ + z) * CWGlobal.WD_WORLD_HEIGHT + y;
        if(nBlock>0)
        {
            if (num >= m_bData.Length)
            {
                return;
            }
                
            m_bData[num] = (byte)nBlock;
        }
        

    }




}
