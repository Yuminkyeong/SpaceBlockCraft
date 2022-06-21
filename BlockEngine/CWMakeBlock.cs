

using System.Collections;
using System.Collections.Generic;
using CWEnum;
using System;
using System.Linq;

using UnityEngine;
/*
 * 1차 시도 c와 똑같은 알고리즘으로구현
 * 2차 시도 폴리곤을 줄이는 개념 구현 
 * */


public class CWMakeBlock
{



    public delegate BLOCKSHAPE dgGetShape(int x, int y, int z, int nBlock);
    int[,] g_vNormal =
    {
            {0,0,-1},
            {0,0, 1},
            {1,0, 0},
            {-1,0, 0},
            {0,1, 0},
            {0,-1, 0},
        };

    bool m_bAirmode;

    CWMakeMesh m_kMakeMesh;
    CWMakeMesh.dgGetBlock _GetBlock;
    CWMakeMesh.dgGetColor _GetColor;

    dgGetShape _GetShape;




    // 옆면에 블록이 존재하는가?


    #region MAKEBOX
    #region MakeFace
    void BlockMakeFace(int nFace, float x, float y, float z, float fsizex, float fsizey, float fsizez, int nBlock)
    {


        m_kMakeMesh.MakeFace(nFace, x, y, z, fsizex, fsizey, fsizez, nBlock, _GetBlock, _GetColor, m_bAirmode);

    }
    void SubMakeFace_Slant(int nFace, float fx, float fy, float fz, int nType, int ndir, float fHeight)
    {
        m_kMakeMesh.MakeFace_Slant(nFace, fx, fy, fz, nType, ndir, fHeight, _GetBlock, _GetColor);
    }
    void SubMakeFace_Slant_F(int nFace, float fx, float fy, float fz, int nType, int ndir, float fHeight)
    {
        m_kMakeMesh.MakeFace_Slant_F(nFace, fx, fy, fz, nType, ndir, fHeight, _GetBlock, _GetColor);
    }



    void BlockMakeFace_Size_F(int nFace, float fx, float fy, float fz, float fSizeY, int nBlock)
    {
        m_kMakeMesh.BlockMakeFace_Size_F(nFace, fx, fy, fz, nBlock, fSizeY, _GetBlock,_GetColor);
    }
    #endregion MakeFace
    bool IsBlockFace(int nFace, int x, int y, int z)
    {
        int tx = x + g_vNormal[nFace, 0];
        int ty = y + g_vNormal[nFace, 1];
        int tz = z + g_vNormal[nFace, 2];
        int tblock = _GetBlock(tx, ty, tz);
        if (tblock <= 0) return false;

        //BLOCKSHAPE nShape = CWArrayManager.Instance.g_Block[tblock].nShape;
        BLOCKSHAPE nShape = _GetShape(tx, ty, tz, tblock);

        if (nShape == BLOCKSHAPE.NORMAL) return true;
        return false;

    }
    bool IsBlockFaceLOD(int nFace, int x, int y, int z,int nSize)
    {
        int nCount = 0;
        bool bEmpty = false;// 빈곳
        for(int sz=z;sz<z+nSize;sz++)
        {
            for (int sy = y; sy < y + nSize; sy++)
            {
                for (int sx = x; sx < x + nSize; sx++)
                {
                    int tx = sx + g_vNormal[nFace, 0];
                    int ty = sy + g_vNormal[nFace, 1];
                    int tz = sz + g_vNormal[nFace, 2];
                    int tblock = _GetBlock(tx, ty, tz);
                    if (tblock>0)
                    {
                        nCount++;
                    }
                    else
                    {
                        bEmpty = true;
                    }
                }

            }

        }
        // 개념 : 블록이 존재하면, 빈틈이 하나도 있으면 안된다 

        if (nCount == 0) return false;
        if (bEmpty) return false;


        return true;

    }

    
    void MakeBox( int x, int y, int z, int nBlock)
    {
        byte bBlock = (byte)nBlock;
        //BLOCKSHAPE nShape = CWArrayManager.Instance.g_Block[nBlock].nShape;
        BLOCKSHAPE nShape = _GetShape(x, y, z, bBlock);

        {

            if (nShape == BLOCKSHAPE.NORMAL)
            {
                BlockAddBox( x, y, z, nBlock);
                
            }
            else if (nShape == BLOCKSHAPE.PLAN)
            {
                BlockAddPlan( x, y, z, nBlock,  0);
            }
            else if (nShape == BLOCKSHAPE.PLAN_1)
            {
                BlockAddPlan( x, y, z, nBlock,  1);
            }
            else if (nShape == BLOCKSHAPE.PLAN_2)
            {
                BlockAddPlan( x, y, z, nBlock,  2);
            }
            else if (nShape == BLOCKSHAPE.PLAN_3)
            {
                BlockAddPlan( x, y, z, nBlock,  3);
            }
            else if (nShape == BLOCKSHAPE.PLAN_4)
            {
                BlockAddPlan( x, y, z, nBlock,  4);
            }

            else if (nShape == BLOCKSHAPE.X_PLAN)
            {
                BlockAddXPlan(x,y,z,nBlock);
            }

            else if (nShape == BLOCKSHAPE.HALF_F)
            {
                BlockAddSize_F( x, y, z, nBlock,  0.5f);
            }
            else if (nShape == BLOCKSHAPE.HALF)
            {
                BlockAddSize( x, y, z, nBlock,  0.5f);
            }
            else if (nShape == BLOCKSHAPE.QURT)
            {
                BlockAddSize(x, y, z, nBlock, 0.25f);
            }
            else if (nShape == BLOCKSHAPE.QURT3)
            {
                BlockAddSize(x, y, z, nBlock, 0.75f);
            }

            else if (nShape == BLOCKSHAPE.QURT_F)
            {
                BlockAddSize_F( x, y, z, nBlock,  0.25f);
            }
            else if (nShape == BLOCKSHAPE.QURT3_F)
            {
                BlockAddSize_F( x, y, z, nBlock,  0.75f);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_1)
            {
                BlockAddStairs( x, y, z, nBlock,  0);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_2)
            {
                BlockAddStairs( x, y, z, nBlock,  1);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_3)
            {
                BlockAddStairs( x, y, z, nBlock,  2);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_4)
            {
                BlockAddStairs( x, y, z, nBlock,  3);
            }

            else if (nShape == BLOCKSHAPE.STAIRS_1_B)
            {
                BlockAddStairs_B( x, y, z, nBlock,  0);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_2_B)
            {
                BlockAddStairs_B( x, y, z, nBlock,  1);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_3_B)
            {
                BlockAddStairs_B( x, y, z, nBlock,  2);
            }
            else if (nShape == BLOCKSHAPE.STAIRS_4_B)
            {
                BlockAddStairs_B( x, y, z, nBlock,  3);
            }

            else if (nShape == BLOCKSHAPE.FANCE)
            {
                BlockAddFance( x, y, z, nBlock);
            }
            else if (nShape == BLOCKSHAPE.STICK)
            {
                BlockAddSTICK( x, y, z, nBlock,0);
            }
            else if (nShape == BLOCKSHAPE.STICK_1)
            {
                BlockAddSTICK( x, y, z, nBlock,1);
            }
            else if (nShape == BLOCKSHAPE.STICK_2)
            {
                BlockAddSTICK( x, y, z, nBlock,2);
            }
            else if (nShape == BLOCKSHAPE.STICK_3)
            {
                BlockAddSTICK( x, y, z, nBlock,0);
            }
            else if (nShape == BLOCKSHAPE.STICK_4)
            {
                BlockAddSTICK( x, y, z, nBlock,0);
            }
            else if (nShape == BLOCKSHAPE.SLANT_1)
            {
                BlockAddSlant( x, y, z, nBlock,  0, 1);
            }
            else if (nShape == BLOCKSHAPE.SLANT_2)
            {
                BlockAddSlant( x, y, z, nBlock,  1, 1);
            }
            else if (nShape == BLOCKSHAPE.SLANT_3)
            {
                BlockAddSlant( x, y, z, nBlock,  2, 1);
            }
            else if (nShape == BLOCKSHAPE.SLANT_4)
            {
                BlockAddSlant( x, y, z, nBlock,  3, 1);
            }

            else if (nShape == BLOCKSHAPE.SLANT_5)
            {
                BlockAddSlant(x, y, z, nBlock, 0+4, 1);
            }
            else if (nShape == BLOCKSHAPE.SLANT_6)
            {
                BlockAddSlant(x, y, z, nBlock, 1 + 4, 1);
            }
            else if (nShape == BLOCKSHAPE.SLANT_7)
            {
                BlockAddSlant(x, y, z, nBlock, 2 + 4, 1);
            }
            else if (nShape == BLOCKSHAPE.SLANT_8)
            {
                BlockAddSlant(x, y, z, nBlock, 3 + 4, 1);
            }

            else if (nShape == BLOCKSHAPE.HSLANT_1)
            {
                BlockAddSlant( x, y, z, nBlock,  0, 0.5f);
            }
            else if (nShape == BLOCKSHAPE.HSLANT_2)
            {
                BlockAddSlant( x, y, z, nBlock,  1, 0.5f);
            }
            else if (nShape == BLOCKSHAPE.HSLANT_3)
            {
                BlockAddSlant( x, y, z, nBlock,  2, 0.5f);
            }
            else if (nShape == BLOCKSHAPE.HSLANT_4)
            {
                BlockAddSlant( x, y, z, nBlock,  3, 0.5f);
            }


            else if (nShape == BLOCKSHAPE.HSLANT_5)
            {
                BlockAddSlant_F(x, y, z, nBlock, 0 + 4, 0.5f);
            }
            else if (nShape == BLOCKSHAPE.HSLANT_6)
            {
                BlockAddSlant_F(x, y, z, nBlock, 1 + 4, 0.5f);
            }
            else if (nShape == BLOCKSHAPE.HSLANT_7)
            {
                BlockAddSlant_F(x, y, z, nBlock, 2 + 4, 0.5f);
            }
            else if (nShape == BLOCKSHAPE.HSLANT_8)
            {
                BlockAddSlant_F(x, y, z, nBlock, 3 + 4, 0.5f);
            }



            else if (nShape >= BLOCKSHAPE._WEDGE && nShape<=BLOCKSHAPE._WEDGE2_6)
            {
                BlockAddWedge(x, y, z,(int) nShape, nBlock);
            }
            else
            {
                BlockAddBox(x, y, z, nBlock);
            }


        }

        

    }
    bool BlockAddBox( int x, int y, int z, int nBlock)
    {
        bool bFlag = false;
        for (int i = 0; i < 6; i++)
        {
            if (IsBlockFace(i, x, y, z)) continue; // 옆면이 존재한다면 통과  //
            // 현재면의 4방면에서 어떤 것이 접하는가?
            BlockMakeFace(i, x, y, z, 1, 1, 1, nBlock);
            bFlag = true;
        }
        return bFlag;
    }
    void BlockAddWedge(int x, int y, int z,int nShape, int nBlock)
    {

        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;
        for (int i = 0; i < 6; i++)
        {
           
            m_kMakeMesh.MakeFace_Wedge(i, fx, fy, fz, nBlock, nShape, _GetBlock, _GetColor);

        }


    }
    void BlockAddPlan(int x, int y, int z, int nBlock, int ndir)
    {

        int nFace = 0;
        //float fx,float fy,float fz = CWVECTOREX(x, y, (float)z + 0.5f);
        float fx, fy, fz;
        fx = x;fy = y;fz = (float)z + 0.5f;
        if (ndir == 1)//-Z
        {
            //vPos = CWVECTOREX(x, y, z + 0.9f);
            fx = x; fy = y; fz = (float)z + 0.9f;
        }
        if (ndir == 2)//+Z
        {
            //vPos = CWVECTOREX(x, y, z + 0.1f);
            fx = x; fy = y; fz = (float)z + 0.1f;
        }
        if (ndir == 3)//+X
        {
            nFace = 3;
            //vPos = CWVECTOREX(x + 0.1f, y, z);
            fx = x + 0.1f; fy = y; fz =z;
        }
        if (ndir == 4)//-X
        {
            nFace = 3;
            //vPos = CWVECTOREX(x + 0.9f, y, z);
            fx = x + 0.9f; fy = y; fz = z;
        }

        BlockMakeFace(nFace,fx, fy, fz, 1, 1, 1, nBlock);
       
    }
    void BlockAddXPlan(int x, int y, int z, int nBlock)
    {

        m_kMakeMesh.MakeXPlan( x, y, z, nBlock);

    }


    void BlockAddSTICK(int x, int y, int z, int nBlock,int ndir)
    {

        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;
        for (int i = 0; i < 6; i++)
        {
            BlockMakeStick( i, fx,fy,fz, nBlock, ndir);
        }
       

    }
    void BlockMakeStick( int nFace, float fx,float fy,float fz, int nBlock,int ndir)
    {
        float fSizeX = 0.2f;
        float fSizeY = 1f;
        float fSizeZ = 0.2f;
        if (ndir == 0)
        {
            fSizeX = 0.2f;
            fSizeY = 1f;
            fSizeZ = 0.2f;
            fx += 0.4f;
            fz += 0.4f;
        }

        if (ndir==1)
        {
            fSizeX = 1f;
            fSizeY = 0.2f;
            fSizeZ = 0.2f;
            fy += 0.4f;
            fz += 0.4f;
        }
        if (ndir == 2)
        {
            fSizeX = 0.2f;
            fSizeY = 0.2f;
            fSizeZ = 1f;
            fx += 0.4f;
            fy += 0.4f;
        }

        BlockMakeFace( nFace, fx,fy,fz,  fSizeX, fSizeY, fSizeZ, nBlock);

    }
    void BlockAddSize_F(int x, int y, int z, int nBlock,  float fSize)
    {
        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;
        float fSizeY = fSize;

        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace_Size_F(i, fx, fy, fz,  fSizeY, nBlock);

        }

    }
    void BlockAddSize( int x, int y, int z, int nBlock, float fSize)
    {
        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;
        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace(i, fx,fy,fz, 1, fSize, 1, nBlock);
        }
    
    }
    void BlockAddStairs(int x, int y, int z, int nType, int ndir)
    {

        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;

        
        {


            byte nNumber1 = (byte)_GetBlock(x , y, z  - 1);
            byte nNumber2 = (byte)_GetBlock(x , y, z  + 1);
            byte nNumber3 = (byte)_GetBlock(x  + 1, y, z );
            byte nNumber4 = (byte)_GetBlock(x  - 1, y, z );



            //BLOCKSHAPE n1 = CWArrayManager.Instance.g_Block[nNumber1].nShape;
            //BLOCKSHAPE n2 = CWArrayManager.Instance.g_Block[nNumber2].nShape;
            //BLOCKSHAPE n3 = CWArrayManager.Instance.g_Block[nNumber3].nShape;
            //BLOCKSHAPE n4 = CWArrayManager.Instance.g_Block[nNumber4].nShape;

            BLOCKSHAPE n1 = _GetShape(x, y, z - 1, nNumber1);
            BLOCKSHAPE n2 = _GetShape(x, y, z + 1, nNumber2);
            BLOCKSHAPE n3 = _GetShape(x + 1, y, z, nNumber3);
            BLOCKSHAPE n4 = _GetShape(x - 1, y, z, nNumber4);

            bool bflag = false;
            int nShape = 0;
            if (n2 == BLOCKSHAPE.STAIRS_2 && n4 == BLOCKSHAPE.STAIRS_1)
            {
                nShape = 0;
                bflag = true;
            }
            if (n2 == BLOCKSHAPE.STAIRS_4 && n3 == BLOCKSHAPE.STAIRS_1)
            {
                nShape = 1;
                bflag = true;
            }
            if (n1 == BLOCKSHAPE.STAIRS_4 && n3 == BLOCKSHAPE.STAIRS_3)
            {
                nShape = 2;
                bflag = true;
            }
            if (n1 == BLOCKSHAPE.STAIRS_2 && n4 == BLOCKSHAPE.STAIRS_3)
            {
                nShape = 3;
                bflag = true;
            }
            ///////////////////////////////////////////////////////////////////////////////
            if (n1 == BLOCKSHAPE.STAIRS_2 && n3 == BLOCKSHAPE.STAIRS_1)
            {
                nShape = 5;
                bflag = true;
            }
            if (n1 == BLOCKSHAPE.STAIRS_4 && n4 == BLOCKSHAPE.STAIRS_1)
            {
                nShape = 4;
                bflag = true;
            }
            if (n2 == BLOCKSHAPE.STAIRS_2 && n3 == BLOCKSHAPE.STAIRS_3)
            {
                nShape = 6;
                bflag = true;
            }
            if (n2 == BLOCKSHAPE.STAIRS_4 && n4 == BLOCKSHAPE.STAIRS_3)
            {
                nShape = 7;
                bflag = true;
            }


            if (bflag)
            {
                if (nShape == 0)
                {
                    BlockAddSize( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 2, 0.5f);
                }
                else if (nShape == 1)
                {
                    BlockAddSize( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 3, 0.5f);
                }
                else if (nShape == 2)
                {
                    BlockAddSize( x, y, z, nType,  0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  1, 0.5f);
                }
                else if (nShape == 3)
                {
                    BlockAddSize( x, y, z, nType,  0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  0, 0.5f);
                }
                else if (nShape == 4)
                {
                    BlockAddSize( x, y, z, nType,  0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  1, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  2, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  3, 0.5f);
                }
                else if (nShape == 5)
                {
                    BlockAddSize( x, y, z, nType,  0.5f);


                    BlockAddBoxSmall( x, y, z, nType,  2, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  3, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  0, 0.5f);
                }
                else if (nShape == 6)
                {
                    BlockAddSize( x, y, z, nType,  0.5f);

                    BlockAddBoxSmall( x, y, z, nType,  0, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  1, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  2, 0.5f);

                }
                else if (nShape == 7)
                {
                    BlockAddSize( x, y, z, nType,  0.5f);

                    BlockAddBoxSmall( x, y, z, nType,  3, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  0, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType,  1, 0.5f);


                }

                
            }

        }


        //  계단 

        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace_Long( i, fx,fy,fz, nType,  ndir);
        }
        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace_Small( i, fx, fy, fz, nType,  ndir);
        }

        

    }
    void BlockMakeFace_SmallBox( int nFace, float fx,float fy,float fz, int nBlock,  int dir)
    {
        
        float fSizeX = 0.5f;
        float fSizeY = 0.5f;
        float fSizeZ = 0.5f;

        if (dir == 0)
        {
            fx += 0;
            fz += 0;
        }
        if (dir == 1)
        {
            fx += 0.5f;
            fz += 0;
        }
        if (dir == 2)
        {
            fx += 0;
            fz += 0.5f;
        }
        if (dir == 3)
        {
            fx += 0.5f;
            fz += 0.5f;
        }

        BlockMakeFace( nFace, fx,fy,fz,  fSizeX, fSizeY, fSizeZ, nBlock);


    }
    void BlockMakeFace_Small_B( int nFace, float fx,float fy,float fz, int nType,  int dir)
    {
        float fSizeX = 1f;
        float fSizeY = 1f;
        float fSizeZ = 1f;
        fy += 0.5f;
        if (dir == 0)
        {

            fSizeZ = 0.5f;
            fSizeY = 0.5f;
        }
        if (dir == 1)
        {
            fx += 0.5f;
            fSizeX = 0.5f;
            fSizeY = 0.5f;
        }
        if (dir == 2)
        {
            fz += 0.5f;
            fSizeZ = 0.5f;
            fSizeY = 0.5f;
        }
        if (dir == 3)
        {

            fSizeX = 0.5f;
            fSizeY = 0.5f;
        }

        BlockMakeFace( nFace, fx,fy,fz,  fSizeX, fSizeY, fSizeZ, nType);


    }
    void BlockMakeFace_Small( int nFace, float fx,float fy,float fz, int nType,  int dir)
    {
        float fSizeX = 1f;
        float fSizeY = 1f;
        float fSizeZ = 1f;
        if (dir == 0)
        {

            fSizeZ = 0.5f;
            fSizeY = 0.5f;
        }
        if (dir == 1)
        {
            fx += 0.5f;
            fSizeX = 0.5f;
            fSizeY = 0.5f;
        }
        if (dir == 2)
        {
            fz += 0.5f;
            fSizeZ = 0.5f;
            fSizeY = 0.5f;
        }
        if (dir == 3)
        {

            fSizeX = 0.5f;
            fSizeY = 0.5f;
        }

        BlockMakeFace( nFace, fx,fy,fz,  fSizeX, fSizeY, fSizeZ, nType);

    }
    void BlockMakeFace_Long( int nFace, float fx,float fy,float fz, int nType,  int dir)
    {

        float fSizeX = 1f;
        float fSizeY = 1f;
        float fSizeZ = 1f;
        if (dir == 0)
        {
            fz += 0.5f;
            fSizeZ = 0.5f;
        }
        if (dir == 1)
        {
            fSizeX = 0.5f;
        }
        if (dir == 2)
        {
            fSizeZ = 0.5f;
        }
        if (dir == 3)
        {
            fx += 0.5f;
            fSizeX = 0.5f;
        }
        BlockMakeFace( nFace, fx,fy,fz,  fSizeX, fSizeY, fSizeZ, nType);

    }
    void BlockMakeFace_Long_B( int nFace, float fx,float fy,float fz, int nType,  int dir)
    {

        float fSizeX = 1f;
        float fSizeY = 1f;
        float fSizeZ = 1f;
        if (dir == 0)
        {
            fz += 0.5f;
            fSizeZ = 0.5f;
        }
        if (dir == 1)
        {
            fSizeX = 0.5f;
        }
        if (dir == 2)
        {
            fSizeZ = 0.5f;
        }
        if (dir == 3)
        {
            fx += 0.5f;
            fSizeX = 0.5f;
        }

        BlockMakeFace( nFace, fx,fy,fz,  fSizeX, fSizeY, fSizeZ, nType);

    }
    void BlockAddBoxSmall( int x, int y, int z, int nBlock, int ndir, float height)
    {
        float fx, fy, fz;
        fx = x;
        fy = y+height;
        fz = z;
                
        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace_SmallBox( i, fx,fy,fz, nBlock, ndir);
        }
    }
    void BlockAddStairs_B( int x, int y, int z, int nType,int ndir)
    {

        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;

        bool bflag = false;
        
        {



            byte nNumber1 = (byte)_GetBlock(x, y, z - 1);
            byte nNumber2 = (byte)_GetBlock(x, y, z + 1);
            byte nNumber3 = (byte)_GetBlock(x + 1, y, z);
            byte nNumber4 = (byte)_GetBlock(x - 1, y, z);


            //BLOCKSHAPE n1 = CWArrayManager.Instance.g_Block[nNumber1].nShape;
            //BLOCKSHAPE n2 = CWArrayManager.Instance.g_Block[nNumber2].nShape;
            //BLOCKSHAPE n3 = CWArrayManager.Instance.g_Block[nNumber3].nShape;
            //BLOCKSHAPE n4 = CWArrayManager.Instance.g_Block[nNumber4].nShape;

            BLOCKSHAPE n1 = _GetShape(x, y, z - 1,nNumber1);
            BLOCKSHAPE n2 = _GetShape(x, y, z + 1, nNumber2);
            BLOCKSHAPE n3 = _GetShape(x + 1, y, z, nNumber3);
            BLOCKSHAPE n4 = _GetShape(x - 1, y, z, nNumber4);

            int nShape = 0;
            if (n2 == BLOCKSHAPE.STAIRS_2_B && n4 == BLOCKSHAPE.STAIRS_1_B)
            {
                nShape = 0;
                bflag = true;
            }
            if (n2 == BLOCKSHAPE.STAIRS_4_B && n3 == BLOCKSHAPE.STAIRS_1_B)
            {
                nShape = 1;
                bflag = true;
            }
            if (n1 == BLOCKSHAPE.STAIRS_4_B && n3 == BLOCKSHAPE.STAIRS_3_B)
            {
                nShape = 2;
                bflag = true;
            }
            if (n1 == BLOCKSHAPE.STAIRS_2_B && n4 == BLOCKSHAPE.STAIRS_3_B)
            {
                nShape = 3;
                bflag = true;
            }
            ///////////////////////////////////////////////////////////////////////////////
            if (n1 == BLOCKSHAPE.STAIRS_2_B && n3 == BLOCKSHAPE.STAIRS_1_B)
            {
                nShape = 5;
                bflag = true;
            }
            if (n1 == BLOCKSHAPE.STAIRS_4_B && n4 == BLOCKSHAPE.STAIRS_1_B)
            {
                nShape = 4;
                bflag = true;
            }
            if (n2 == BLOCKSHAPE.STAIRS_2_B && n3 == BLOCKSHAPE.STAIRS_3_B)
            {
                nShape = 6;
                bflag = true;
            }
            if (n2 == BLOCKSHAPE.STAIRS_4_B && n4 == BLOCKSHAPE.STAIRS_3_B)
            {
                nShape = 7;
                bflag = true;
            }


            if (bflag)
            {
                if (nShape == 0)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 2, 0);
                }
                else if (nShape == 1)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 3, 0);
                }
                else if (nShape == 2)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 1, 0);
                }
                else if (nShape == 3)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 0, 0);
                }
                else if (nShape == 4)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);
                    BlockAddBoxSmall( x, y, z, nType, 1, 0);
                    BlockAddBoxSmall( x, y, z, nType, 2, 0);
                    BlockAddBoxSmall( x, y, z, nType, 3, 0);
                }
                else if (nShape == 5)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);


                    BlockAddBoxSmall( x, y, z, nType, 2, 0);
                    BlockAddBoxSmall( x, y, z, nType, 3, 0);
                    BlockAddBoxSmall( x, y, z, nType, 0, 0);
                }
                else if (nShape == 6)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);

                    BlockAddBoxSmall( x, y, z, nType, 0, 0);
                    BlockAddBoxSmall( x, y, z, nType, 1, 0);
                    BlockAddBoxSmall( x, y, z, nType, 2, 0);

                }
                else if (nShape == 7)
                {
                    BlockAddSize_F( x, y, z, nType, 0.5f);

                    BlockAddBoxSmall( x, y, z, nType, 3, 0);
                    BlockAddBoxSmall( x, y, z, nType, 0, 0);
                    BlockAddBoxSmall( x, y, z, nType, 1, 0);


                }

                
            }

        }

        //  계단 

        
        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace_Long_B( i, fx, fy, fz, nType, ndir);
        }
        for (int i = 0; i < 6; i++)
        {
            BlockMakeFace_Small_B( i, fx, fy, fz, nType, ndir);
        }

        

    }
    void BlockAddFance( int x, int y, int z, int nType)
    {
        
        int nBlock = 0;
        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;

        for (int i = 0; i < 6; i++)
        {
            // 주변의 영향에 따라서 모양이 바꿘다
            // -x 방향
            // +x 방향
            // -z 방향
            // +z 방향
            BlockMakeFance( i, fx,fy,fz, nType);
        }
        nBlock = _GetBlock(x  - 1, y, z );
        
        if (nBlock > 0)
        {
            //BLOCKSHAPE n1 = CWArrayManager.Instance.g_Block[nBlock].nShape;
            BLOCKSHAPE n1 = _GetShape(x - 1, y, z,nBlock);
            if (n1 == BLOCKSHAPE.FANCE )
            {
                BlockMakeFance_Dir( fx,fy,fz, nType,  -1, 0);
            }

        }
        nBlock = _GetBlock(x  + 1, y, z );
        if (nBlock > 0)
        {
            //pkBlockItem = CCWBlockItem::GetBlockItem(nBlock);
            //BLOCKSHAPE n1 = CWArrayManager.Instance.g_Block[nBlock].nShape;
            BLOCKSHAPE n1 = _GetShape(x + 1, y, z, nBlock);

            if  (n1 == BLOCKSHAPE.FANCE )
                BlockMakeFance_Dir( fx,fy,fz, nType,  1, 0);
        }
        nBlock = _GetBlock(x , y, z  - 1);
        if (nBlock > 0)
        {
            //BLOCKSHAPE n1 = CWArrayManager.Instance.g_Block[nBlock].nShape;
            BLOCKSHAPE n1 = _GetShape(x, y, z - 1,nBlock);

            if (n1 == BLOCKSHAPE.FANCE )
                BlockMakeFance_Dir( fx,fy,fz, nType,  0, -1);

        }
        nBlock = _GetBlock(x , y, z  + 1);
        if (nBlock > 0)
        {
            BLOCKSHAPE n1 = _GetShape(x, y, z + 1, nBlock);
            if (n1 == BLOCKSHAPE.FANCE )
                BlockMakeFance_Dir( fx,fy,fz, nType,  0, 1);

        }

        
    }
    void BlockAddSlant_F(int x, int y, int z, int nType, int ndir, float fHeight)
    {

        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;
        for (int i = 0; i < 6; i++)
        {
            SubMakeFace_Slant_F(i, fx, fy, fz, nType, ndir, fHeight);

        }


    }

    void BlockAddSlant( int x, int y, int z, int nType, int ndir, float fHeight)
    {

        float fx, fy, fz;
        fx = x;
        fy = y;
        fz = z;
        for (int i = 0; i < 6; i++)
        {
            SubMakeFace_Slant( i, fx,fy,fz, nType, ndir, fHeight);

        }
        

    }
    Vector3 GetShapePos(BLOCKSHAPE nShpae)
    {
        if (nShpae == BLOCKSHAPE._WEDGE)
        {
            return new Vector3(0,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2)
        {
            return new Vector3(0,0,-1);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_2)
        {
            return new Vector3(0,0,1);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_3)
        {
            return new Vector3(1,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_4)
        {
            return new Vector3(-1,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_5)
        {
            return new Vector3(0,1,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_6)
        {
            return new Vector3(0,-1,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_2)
        {
            return new Vector3(0,0,1);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_3)
        {
            return new Vector3(1,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_4)
        {
            return new Vector3(-1,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_5)
        {
            return new Vector3(0,1,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_6)
        {
            return new Vector3(0,-1,0);
        }

        return Vector3.zero;
    }
    Vector3 GetShapeVecter(BLOCKSHAPE nShpae)
    {
        if (nShpae == BLOCKSHAPE._WEDGE)
        {
            return new Vector3(0,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2)
        {
            return new Vector3(0,0,-180);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_2)
        {
            return new Vector3(0,180,-180);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_3)
        {
            return new Vector3(0,-90,-180);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_4)
        {
            return new Vector3(0,90,-180);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_5)
        {
            return new Vector3(0,-180,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE2_6)
        {
            return new Vector3(0,-80,180);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_2)
        {
            return new Vector3(0,-180,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_3)
        {
            return new Vector3(0,-90,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_4)
        {
            return new Vector3(0,90,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_5)
        {
            return new Vector3(0,0,0);
        }
        if (nShpae == BLOCKSHAPE._WEDGE_6)
        {
            return new Vector3(0,0,180);
        }

        return Vector3.zero;
    }
    


    void BlockMakeFance( int nFace, float fx,float fy,float fz, int nType)
    {
        float fSizeX = 0.2f;
        float fSizeY = 1f;
        float fSizeZ = 0.2f;
        fx += 0.4f;
        fz += 0.4f;
        BlockMakeFace( nFace, fx,fy,fz, fSizeX, fSizeY, fSizeZ, nType);

    }
    void SubBlockMakeFance_Dir( int nFace, float fx,float fy,float fz, int nType,  int xdir, int zdir)
    {

        
        float fSizeX = 0.4f;
        float fSizeY = 0.2f;
        float fSizeZ = 0.2f;

        fy += 0.6f;
        if (xdir < 0)
        {
            fSizeX = 0.4f;
            fSizeZ = 0.2f;
            fz += 0.4f;
        }
        if (xdir > 0)
        {
            fSizeX = 0.4f;
            fSizeZ = 0.2f;
            fz += 0.4f;
            fx += 0.6f;
        }
        if (zdir < 0)
        {
            fSizeZ = 0.4f;
            fSizeX = 0.2f;
            fx += 0.4f;

        }
        if (zdir > 0)
        {
            fSizeZ = 0.4f;
            fSizeX = 0.2f;
            fx += 0.4f;
            fz += 0.6f;

        }

        //SubMakeFace( nFace, fx,fy,fz, nType, fSizeX, fSizeY, fSizeZ);
        BlockMakeFace(nFace, fx, fy, fz, fSizeX, fSizeY, fSizeZ, nType);

    }
    // 방향별로 막대기를 그린다
    void BlockMakeFance_Dir( float fx,float fy,float fz, int nType, int xdir, int zdir)
    {

        fy -= 0.4f;
        for (int i = 0; i < 6; i++)
        {
            SubBlockMakeFance_Dir( i, fx,fy,fz, nType,  xdir, zdir);
            SubBlockMakeFance_Dir( i, fx, fy, fz, nType,  xdir, zdir);
        }

    }
    #endregion MAKEBOX
    void Clear()
    {

    }


    public Mesh Create(CWMakeMesh.dgGetBlock cbgetblock, dgGetShape cbgetshape, CWMakeMesh.dgGetColor cbgetcolor, int dx,int dy,int dz,bool bAirmode)
    {
        m_bAirmode = bAirmode;
        _GetBlock = cbgetblock;
        _GetShape = cbgetshape;
        _GetColor = cbgetcolor;
        Clear();


        m_kMakeMesh = new CWMakeMesh();
        
        for (int z=0;z < dz;z++)
        {
            for (int y = 0; y < dy; y++)
            {
                for (int x = 0; x < dx; x++)
                {
                    int nblock = _GetBlock(x, y, z);
                    if (nblock <= 0) continue;
                    MakeBox(x,y,z,nblock);

                }
            }
        }
        Mesh kMesh = m_kMakeMesh.GetMesh();
        m_kMakeMesh = null;
        return kMesh;


    }
    void MakeBoxLOD(int x,int y,int z,int nblock,int nSize)
    {
        
        for (int i = 0; i < 6; i++)
        {
            if (IsBlockFaceLOD(i, x, y, z,nSize)) continue; // 옆면이 존재한다면 통과  //
            // 현재면의 4방면에서 어떤 것이 접하는가?
            BlockMakeFace(i, x, y, z, nSize, nSize, nSize, nblock);
        
        }

    }

    public Mesh CreateLOD(CWMakeMesh.dgGetBlock cbgetblock, dgGetShape cbgetshape, CWMakeMesh.dgGetColor cbgetcolor, int nSize)
    {
        _GetBlock = cbgetblock;
        _GetShape = cbgetshape;
        _GetColor = cbgetcolor;
        Clear();

        m_kMakeMesh = new CWMakeMesh();

        for (int z = 0; z < CWGlobal.SELLCOUNT; z += nSize)
        {
            for (int y = 0; y < CWGlobal.WD_WORLD_HEIGHT; y += nSize)
            {
                for (int x = 0; x < CWGlobal.SELLCOUNT; x += nSize)
                {
                    int nblock = _GetBlock(x, y, z);
                    if (nblock <= 0) continue;
                    MakeBoxLOD(x, y, z, nblock,nSize);

                }
            }
        }
        Mesh kMesh = m_kMakeMesh.GetMesh();
        m_kMakeMesh = null;
        return kMesh;


    }


}
