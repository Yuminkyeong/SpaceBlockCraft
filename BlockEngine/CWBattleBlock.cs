using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWBattleBlock {

    int WIDTH = 32;
    int HEIGHT = 32;

    public Vector3 m_vPos;

    byte[] m_bBlockBuffer;
    
    // 블록리스트

    public int GetMaxCount()
    {
        if (m_bBlockBuffer == null)
        {
            
            return 0;
        }
            
        return m_bBlockBuffer.Length;
    }

    public void CreateBlockList(Vector3 vPos,Vector3 vScale)
    {

        WIDTH = (int)vScale.x;
        HEIGHT = (int)vScale.y;

        m_vPos = vPos;
        m_bBlockBuffer = new byte[WIDTH * WIDTH * HEIGHT];

    }
    int GetStartX()
    {
        return (int)m_vPos.x - WIDTH / 2;
    }
    int GetStartZ()
    {
        return (int)m_vPos.z - WIDTH / 2;
    }
    int GetStartY()
    {
        return (int)m_vPos.y - HEIGHT / 2; ;
    }

    int _GetBlock(int x, int y, int z)
    {
        if(m_bBlockBuffer==null)
        {
            
            return 0;
        }

        int num = (x * WIDTH + z) * HEIGHT + y;
        return m_bBlockBuffer[num];
    }
    void _SetBlock(int x,int y,int z,int nBlock)
    {
        if (m_bBlockBuffer == null)
        {
            
            return ;
        }

        int num = (x * WIDTH + z) * HEIGHT + y;
        m_bBlockBuffer[num] = (byte)nBlock;

        int tx = GetStartX() + x;
        int ty = GetStartY() + y;
        int tz = GetStartZ() + z;
        CWMapManager.SelectMap.UpdateBlock(tx,ty,tz,nBlock);
    }

    public int GetBlock(int sx, int sy, int sz)
    {

        if (m_bBlockBuffer == null)
        {
            
            return 0;
        }

        int tx = sx - GetStartX();
        int ty = sy - GetStartY() ;
        int tz = sz - GetStartZ() ;
        if (tx < 0) return -1;
        if (ty < 0) return -1;
        if (tz < 0) return -1;

        if (tx>= WIDTH) return -1;
        if (tz >= WIDTH) return -1;
        if (ty >= HEIGHT) return -1;

        int num = (tx * WIDTH + tz) * HEIGHT + ty;
        return m_bBlockBuffer[num];
    }

    public bool FindBlock(Vector3Int vPos)
    {
        int num = GetBlock(vPos.x, vPos.y, vPos.z);
        if (num > 0) return true;
        return false;
    }
    public Vector3 GetNextBlockPos()
    {
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int z = 0; z < WIDTH; z++)
                {
                    int num = _GetBlock(x, y, z);
                    if (num == 0)
                    {
                        int tx = GetStartX() + x;
                        int ty = GetStartY() + y;
                        int tz = GetStartZ() + z;
                        return new Vector3(tx, ty, tz);
                    }
                }

            }

        }

        return Vector3.zero;
    }
    // 블록을 순차적으로 쌓게 만든다
    public void AddWorldBlock( int nBlock,int nCount)
    {
        int tcnt = Mathf.Abs(nCount);
        
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int z = 0; z < WIDTH; z++)
                {
                    if(nCount>0)
                    {
                        int num = _GetBlock(x, y, z);
                        if (num == 0)
                        {
                            _SetBlock(x, y, z, nBlock);
                            tcnt--;
                            if (tcnt <= 0) return;
                        }
                    }
                    else
                    {
                        int num = _GetBlock(x, y, z);
                        if(num== nBlock)
                        {
                            _SetBlock(x, y, z, 0);
                        }
                        tcnt--;
                        if (tcnt <= 0) return;

                    }
                }

            }

        }


    }


}
