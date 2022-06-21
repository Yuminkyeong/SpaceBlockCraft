using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;

public class CWAutoAirFile  {

    public struct SHIPDATA
    {
        public int x;
        public int y;
        public int z;
        public int nItemNumber;
        public int nShape;
    };

    
    const int SELLWIDTH = 32;
    public List<SHIPDATA> m_kOrder = new List<SHIPDATA>();
    bool ExistsFile(int sx, int sy, int sz)
    {
        if (!m_kOrder.Exists(x => (x.x == sx && x.y == sy && x.z == sz)))
        {
            return false;
        }
        return true;

    }

    public void Create()
    {
        m_kOrder.Clear();
    }
    public void AddData(int x,int y,int z,int nItem,int nShape)
    {
        if (ExistsFile(x, y, z)) return;
        SHIPDATA kData = new SHIPDATA();
        kData.x = x;
        kData.y = y;
        kData.z = z;
        kData.nItemNumber = nItem;
        kData.nShape = nShape;
        m_kOrder.Add(kData);

    }
    public void Save()
    {
        CWFile cf = new CWFile();

        cf.PutInt(m_kOrder.Count);
        for(int i=0;i<m_kOrder.Count;i++)
        {

            cf.PutInt(m_kOrder[i].x);
            cf.PutInt(m_kOrder[i].y);
            cf.PutInt(m_kOrder[i].z);
            cf.PutInt(m_kOrder[i].nItemNumber);
            cf.PutInt(m_kOrder[i].nShape);
        }

        cf.SaveGamedata("Autofile");

    }
    public void LoadOld()
    {
        m_kOrder.Clear();
        CWFile cf = new CWFile();
        cf.LoadResources("Autofile");

        int tcnt = cf.GetInt();
        for(int i=0;i<tcnt;i++)
        {
             int x= cf.GetInt();
             int y= cf.GetInt();
             int z= cf.GetInt();
             int nItem= cf.GetInt();
             int nShape= cf.GetInt();

            AddData(x,y,z,nItem,nShape);
        }

    }
    public void Load()
    {
        m_kOrder.Clear();
        CWFile cf = new CWFile();
        cf.LoadGamedata("Autofile");

        int tcnt = cf.GetInt();
        for (int i = 0; i < tcnt; i++)
        {
            int x = cf.GetInt();
            int y = cf.GetInt();
            int z = cf.GetInt();
            int nItem = cf.GetInt();
            int nShape = cf.GetInt();

            AddData(x, y, z, nItem, nShape);
        }

    }


    // 뒤에 블록이 있는가?

    // 해당 위치에 문제가 있는가?
    bool CheckPos(int x,int y,int z)
    {

        if (x <= 0) return false;
        if (y <= 0) return false;
        if (z <= 0) return false;
        if (x >= SELLWIDTH) return false;
        if (y >= SELLWIDTH) return false;
        if (z >= SELLWIDTH) return false;

        if (CWHero.Instance.GetBlockLocal(x, y, z) > 0) return false;
        

        // 뒤에 무기가 있는가
        {
            int nItem = CWHero.Instance.GetBlockLocal(x, y, z - 1);
            if(nItem>0)
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
                if (gData.type == "weapon") return false;
            }

        }

        // 앞에 부스터가 있는가?
        {
            int nItem = CWHero.Instance.GetBlockLocal(x, y, z + 1);
            if (nItem > 0)
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
                if (gData.type == "Buster") return false;
            }


        }
        // 좌우에 블록이 존재하는가
        // 위아래에 블록이 존재하는가?
        {
            int nItem1 = CWHero.Instance.GetBlockLocal(x, y, z + 1);
            if (nItem1 > 0) return true;
            int nItem2 = CWHero.Instance.GetBlockLocal(x, y, z - 1);
            if (nItem2 > 0) return true;
            int nItem3 = CWHero.Instance.GetBlockLocal(x+1, y, z );
            if (nItem3 > 0) return true;
            int nItem4 = CWHero.Instance.GetBlockLocal(x-1, y, z);
            if (nItem4 > 0) return true;
            int nItem5 = CWHero.Instance.GetBlockLocal(x, y+1, z);
            if (nItem5 > 0) return true;
            int nItem6 = CWHero.Instance.GetBlockLocal(x, y-1, z);
            if (nItem6 > 0) return true;

        }
        return false;
    }
    Vector3Int FindBlockEmpty(int x,int y,int z)
    {
        for (int i = 1; ; i++)
        {
            int tx = 0;
            int ty = 0;
            int tz = 0;
            if (!CWSphereData.GetData(i, ref tx, ref ty, ref tz))
            {
                break;
            }
            int item = CWHero.Instance.GetBlockLocal(x + tx, y + ty, z + tz);
            if (item == 0)
            {
                if(CheckPos(x + tx, y + ty, z + tz))
                    return new Vector3Int(x + tx, y + ty, z + tz);
            }
        }

        return Vector3Int.zero;


    }

    // 해당 위치에 맞는 좌표를 리턴
    Vector3Int GetCollectBlockPos(int x,int y,int z)
    {

        if (CWHero.Instance.GetBlockLocal(x, y, z) == 0)
        {
            if(CheckPos(x, y, z))
                return new Vector3Int(x, y, z);
        }
        
        
            // 가장 가까운 곳에 블록위치를 찾는다
        return FindBlockEmpty(x, y, z);
        

    }
    // 
    bool IsBlockZ(int x,int y)
    {
        for(int z=0;z<SELLWIDTH;z++)
        {
            if (CWHero.Instance.GetBlockLocal(x, y, z) > 0)
            {
                return true;
            }
        }
        return false;
    }
    bool IsWeaponZ(int x,int y)
    {
        for(int z=0;z<SELLWIDTH;z++)
        {
            int nID =CWHero.Instance.GetBlockLocal(x, y, z);
            if(nID>0)
            {
                GITEMDATA nData = CWArrayManager.Instance.GetItemData(nID);
                if(nData.type == "weapon")
                {
                    return true;
                }
            }
            
        }
        return false;
    }
    int GetMaxZ(int x,int y)
    {
        int nMax = 0;
        for (int z = 0; z < SELLWIDTH; z++)
        {
            if (CWHero.Instance.GetBlockLocal(x, y, z) > 0)
            {
                if(nMax < z)
                {
                    nMax = z;
                }
            }
        }
        return nMax+1;
    }
    bool IsBusterZ(int x,int y)
    {
        for(int z=0;z<SELLWIDTH;z++)
        {
            int nID =CWHero.Instance.GetBlockLocal(x, y, z);
            if(nID>0)
            {
                GITEMDATA nData = CWArrayManager.Instance.GetItemData(nID);
                if(nData.type == "Buster")
                {
                    return true;
                }
            }
            
        }
        return false;
    }
    int GetMinZ(int x,int y)
    {
        
        for (int z = 0; z < SELLWIDTH/2; z++)
        {
            if (CWHero.Instance.GetBlockLocal(x, y, z) > 0)
            {
                return z - 1;
            }
        }
        return 0;
    }
    int GetMinY(int x,int z)
    {
        for (int y = 0; y < SELLWIDTH; y++)
        {
            if (CWHero.Instance.GetBlockLocal(x, y, z) > 0)
            {
                return z - 1;
            }
        }
        return 0;
    }
    /*
        // 혼자 떨어진 블록을 찾는다
        bool IsFallenBlock(Vector3Int vPos)
        {
            for(int z=-1;z<=1;z++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue;
                        if (CWHero.Instance.GetBlockLocal(vPos.x+x, vPos.y+y, vPos.z+z)>0)
                        {
                            return false;
                        }
                    }

                }

            }
            return true;
        }
        Vector3Int FindFallenblock()
        {
            Dictionary<int, CWBuildObject.BlockData> kData= CWHero.Instance.GetData();
            foreach(var v in kData)
            {
                GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
                if ( nData.type == "Buster")
                {
                    Debug.Log("");

                }

                int x = CWHero.Instance.GetSellX(v.Key);
                int y = CWHero.Instance.GetSellY(v.Key);
                int z = CWHero.Instance.GetSellZ(v.Key);
                Vector3Int vv = new Vector3Int(x,y,z);
                if(IsFallenBlock(vv))
                {

                    // vv는 떨어진 블록이다. 
                    // 개념, vv를 기준으로 중심에 가까운 벡터로 기준을 정한다


                }

            }
            return Vector3Int.zero;

        }
    */
    // 무기의 위치를 찾는다
    Vector3Int GetCollectWeaponPos()
    {
        

        //기존 무기와 같게 
        foreach (var v in m_kOrder)
        {
            GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.nItemNumber);
            if (nData.type == "weapon")
            {
                // 기존에 블록이 없다
                if (CWHero.Instance.GetBlockLocal(v.x, v.y, v.z) == 0)
                {
                    if (!IsWeaponZ(v.x, v.y)) // 무기가 존재하지 않는다면
                    {
                        if (CheckPos(v.x, v.y, v.z))
                        {
                            return new Vector3Int(v.x, v.y, v.z);
                        }
                    }

                }
                else
                {
                    if(IsBlockZ(v.x,v.y))// 블록이 축에 존재한다
                    {
                        if(!IsWeaponZ(v.x, v.y)) // 무기가 존재하지 않는다면
                        {
                            int z= GetMaxZ(v.x, v.y);
                            if(CheckPos(v.x, v.y, z))
                            {
                                return new Vector3Int(v.x, v.y, z);
                            }
                            
                        }
                    }
                }
                


            }
        }


        for(int y=-2;y<4;y++)
        {
            foreach (var v in m_kOrder)
            {
                GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.nItemNumber);
                if (nData.type == "weapon")
                {
                    int ty = v.y + y;
                    if (IsBlockZ(v.x, ty))// 블록이 축에 존재한다
                    {
                        if (!IsWeaponZ(v.x, ty)) // 무기가 존재하지 않는다면
                        {
                            int z = GetMaxZ(v.x, ty);
                            if (CheckPos(v.x, ty, z))
                            {
                                return new Vector3Int(v.x, ty, z);
                            }

                        }
                    }

                }
            }
        }

        



        return Vector3Int.zero;

    }
    Vector3Int GetCollectEnginePos()
    {
        int rr = Random.Range(1, 20);
        int y = GetMinY(m_kOrder[rr].x, m_kOrder[rr].z);

        if(CheckPos(m_kOrder[rr].x,y, m_kOrder[rr].z))
        {
            return new Vector3Int(m_kOrder[rr].x, y, m_kOrder[rr].z);
        }
        return Vector3Int.zero;
    }
    Vector3Int GetNearBlock(int x,int y,int z)
    {
        int tx = 0;
        int ty = 0;
        int tz = 0;
        for (int i = 0; ; i++)
        {
            if (!CWSphereData.GetData(i, ref tx, ref ty, ref tz))
            {
                return Vector3Int.zero;
            }
            int ix = x + tx;
            int iy = y + ty;
            int iz = z + tz;
            if (CWHero.Instance.GetBlockLocal(ix, iy, iz) > 0)
            {
                return new Vector3Int(ix,iy,iz);
            }
        }
        
        
    }
    // 무작위로 부스터를 끝에 붙인다
    Vector3Int GetEmptyBusterPos()
    {
        for(int x=0;x<SELLWIDTH;x++)
        {
            for (int y = 0; y < SELLWIDTH; y++)
            {
                int z = GetMinZ(x, y);
                if(IsBlockZ(x,y))// 블록이 존재하면서
                {
                    if (!IsBusterZ(x, y))// 부스터가 없으면서
                    {
                        if (CheckPos(x, y, z))
                        {
                            return new Vector3Int(x, y, z);
                        }

                    }

                }

            }

        }

        return Vector3Int.zero;
    }
    Vector3Int GetCollectBusterPos()
    {
        foreach (var v in m_kOrder)
        {
            GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.nItemNumber);
            if (nData.type == "Buster")
            {
                // 기존에 블록이 없다
                if (CWHero.Instance.GetBlockLocal(v.x, v.y, v.z) == 0)
                {

                    if (!IsBusterZ(v.x, v.y))// 이줄에는 부스터가 없다
                    {
                        // 맨뒤에 붙인다
                        int z = GetMinZ(v.x, v.y);
                        if (CheckPos(v.x, v.y, z))
                        {
                            return new Vector3Int(v.x, v.y, z);
                        }
                    }

                }
                else // 기존에 다른 블록이 존재한다
                {
                    if (!IsBusterZ(v.x, v.y)) // 이줄에 부스터가 없다면
                    {
                        int z = GetMinZ(v.x, v.y);
                        if (CheckPos(v.x, v.y, z))
                        {
                            return new Vector3Int(v.x, v.y, v.z);
                        }
                    }
                }
            }
        }

        return GetEmptyBusterPos();//
    }

    // 무작위로 위치를 찾는다
    public Vector3Int GetEmptyPos()
    {
        int tx = 0;
        int ty = 0;
        int tz = 0;


        for (int i=0; ;i++)
        {
            if(!CWSphereData.GetData(i, ref tx, ref ty, ref tz))
            {
                return Vector3Int.zero;
            }
            if(CheckPos(tx+15,ty+6,tz+15))
            {
                return new Vector3Int(tx + 15, ty + 6, tz + 15);
            }
        }
        
    }
    public Vector3Int GetBlock(int num,int nItem)
    {
        // 기본 블록을 제외한 번호
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if(gData.type == "engine")
        {
            return GetCollectEnginePos();
        }
        if(gData.type == "Buster")
        {
            return GetCollectBusterPos();
        }
        if(gData.type == "weapon")
        {

            return GetCollectWeaponPos();

        }
        if (gData.type == "shipblock")
        {
            if (num < 0) { return Vector3Int.zero; }
            if (num > m_kOrder.Count)
            {
                return Vector3Int.zero;
            }
            return GetCollectBlockPos(m_kOrder[num].x, m_kOrder[num].y, m_kOrder[num].z);

        }

        return GetEmptyPos();
    }





}
