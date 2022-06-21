using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;

public class CWAirBlockCopy 
{

    const int SELLSIZE=64;
    // 충돌박스를 구한다 
    public delegate void dlSetblock(int x, int y, int z, int nBlock,int nShape,int nColor);

    public CWAirBlockCopy()
    {
        

    }

    byte[] m_bData;
    byte[] m_bDataShape;//
    byte[] m_bDataColor;


    public void Load(string szfile)
    {

        string szPath = CWLib.pathForDocumentsPath();
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/BuilBlock/{1}.bytes", szPath, szfile);
        if (cf.Load(szpath))
        {
            m_bData = cf.GetBuffer();
            m_bDataShape = cf.GetBuffer();
            m_bDataColor = cf.GetBuffer();
        }

    }
    public void Save(string szfile)
    {
        string szPath = CWLib.pathForDocumentsPath();
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/BuilBlock/{1}.bytes", szPath, szfile);

        cf.PutBuffer(m_bData);
        cf.PutBuffer(m_bDataShape);
        cf.PutBuffer(m_bDataColor);
        cf.Save(szpath);

    }
    public void ApplyMap(int sx, int sy, int sz, dlSetblock _Setblock)
    {

        if (m_bData == null) return;
        for (int z = 0; z < SELLSIZE; z++)
        {
            for (int y = 0; y < SELLSIZE; y++)
            {
                for (int x = 0; x < SELLSIZE; x++)
                {
                    int num = (x * SELLSIZE + z) * SELLSIZE + y;

                    if (m_bData[num] > 0)
                    {
                        _Setblock(x + sx , y + sy, z + sz, m_bData[num], m_bDataShape[num], m_bDataColor[num]);
                    }

                }

            }

        }

    }
    public void TakeMap(int sx, int sy, int sz, int ex, int ey, int ez, CWAirObject kObject)
    {


        m_bData = new byte[SELLSIZE * SELLSIZE * SELLSIZE];
        m_bDataShape = new byte[SELLSIZE * SELLSIZE * SELLSIZE];
        m_bDataColor = new byte[SELLSIZE * SELLSIZE * SELLSIZE];

        for (int z = sz; z < ez; z++)
        {
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    if (y < 0) continue;
                    int nBlock = kObject.GetBlock(x, y, z);
                    int nBlock1 = kObject.GetShape(x, y, z);
                    int nBlock2 = kObject.GetColor(x, y, z);
                    AddData(x, y, z , nBlock, nBlock1, nBlock2);
                }
            }
        }

    }
    void AddData(int x, int y, int z, int nBlock,int nShape,int nColor)
    {
        if (x < 0) return;
        if (y < 0) return;
        if (z < 0) return;
        if (z >= SELLSIZE) return;
        if (z >= SELLSIZE) return;
        int num = (x * SELLSIZE + z) * SELLSIZE + y;
        if (nBlock > 0)
        {
            m_bData[num] = (byte)nBlock;
            m_bDataShape[num] = (byte)nShape;
            m_bDataColor[num] = (byte)nColor;
        }


    }


}
