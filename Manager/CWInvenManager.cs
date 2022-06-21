using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json.Linq;

public class CWInvenManager  : CWManager<CWInvenManager>
{

    

    public List<SLOTITEM> m_nInvenDB = new List<SLOTITEM>();


    public delegate void UpdateEvent();

    public UpdateEvent CBUpdateEvent;

    int m_nSelectBomb = 0;
    public void SetNowBomb(int num)
    {
        m_nSelectBomb = num;
    }
    public GITEMDATA GetNowBomb()
    {
        if(m_nSelectBomb>0)
        {
            int nItem=m_nInvenDB[m_nSelectBomb].NItem;
            if(m_nInvenDB[m_nSelectBomb].NCount==0)
            {
                m_nSelectBomb = 0;
            }
            else
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
                return gData;
            }
        }
        foreach (var v in m_nInvenDB)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
            if (gData.type== "폭탄")
            {
                return gData;
            }
        }

        return new GITEMDATA();

    }

    //nItem 보다 높은아이템 리턴
    public int FindUplevelItem(int nItem)
    {

        int nMax = 0;
        int nRet = 0;
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        foreach (var v in m_nInvenDB)
        {
            GITEMDATA gData2 = CWArrayManager.Instance.GetItemData(v.NItem);
            if(gData.type==gData2.type)
            {
                if(gData.hp<=gData2.hp)
                {
                    nMax = gData2.hp;
                    nRet = v.NItem;
                }
            }
        }

        return nRet;
    }

    public override void Create()
    {
        StartCoroutine("Run");
        base.Create();
    }
    public void Clear()
    {
        m_nInvenDB.Clear();
    }
    IEnumerator Run()
    {
        while(true)
        {
            if(GameEdit.Instance==null)
            {
                yield return null;
                continue;
            }
            // 에디터에서는 안함
            if(!GameEdit.Instance.IsShow())
            {
                if (CWSocketManager.Instance)
                    CWSocketManager.Instance.UpdateInven(m_nInvenDB);

            }
            yield return new WaitForSeconds(1f);
        }
        
    }
    public SLOTITEM GetItembySlot(int nSlot)
    {
        foreach (var v in m_nInvenDB)
        {
            if (v.m_nSlot== nSlot)
            {

                return v;
            }
        }
        return null;

    }
    public List<SLOTITEM> GetList(string szType)
    {
        List<SLOTITEM> kTemp = new List<SLOTITEM>();
        foreach (var v in m_nInvenDB)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
            if(gData.type==szType)
            {
                kTemp.Add(v);
            }
        }

        return kTemp;
    }
    public SLOTITEM GetItem(int nItemID)
    {
        foreach (var v in m_nInvenDB)
        {
            if (v.NItem == nItemID)
            {

                return v;
            }
        }
        return null;

    }
    public int GetItemTotalCount(int nItemID)
    {
        int nCount = 0;
        foreach (var v in m_nInvenDB)
        {
            if (v.NItem==nItemID)
            {
                nCount +=v.NCount;
            }
        }
        return nCount;
    }
    // 해당 아이템이 있는가?

    public bool IsHaveItem(string szType)
    {
        foreach(var v in m_nInvenDB)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
            if(gData.type==szType)
            {
                return true;
            }

            //v.NItem
        }
        return false;

    }
    public int FindHaveItem(string szType)
    {
        foreach (var v in m_nInvenDB)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
            if (gData.type == szType)
            {
                return v.m_nSlot;
            }
        }
        return 0;

    }

    public SLOTITEM FindAddSlot(int nID,int nCount)
    {

        SLOTITEM slot = m_nInvenDB.Find(x => x.NItem == nID );
        if (slot == null)
        {
            slot = m_nInvenDB.Find(x => x.NCount == 0);
            if (slot == null)
            {
                // 추가 한다
                int tt = m_nInvenDB.Count;
                slot = new SLOTITEM();
                slot.m_nSlot = tt;
                slot.SetItem(tt, 0, 0);
                m_nInvenDB.Add(slot);

            }
            return slot;
        }
        return slot;
    }
    public void UpdateInven()
    {
     
        if(CBUpdateEvent!=null)
        {
            CBUpdateEvent();
        }
     
    }
    public int GetItemSlot(int nNumber)
    {

        return m_nInvenDB[nNumber].NItem;
    }
    public int GetItemSlotCount(int nNumber)
    {

        return m_nInvenDB[nNumber].NCount;
    }

    public void DelItemSlot(int nNumber)
    {
        m_nInvenDB[nNumber].NCount--;
    }
    public void DelSlot(int nNumber)
    {
        m_nInvenDB[nNumber].NCount = 0;
        m_nInvenDB[nNumber].NItem = 0;
        // 서버 전송
        
    }
    public bool UseItem(int nID, int Count = 1)
    {
        int tcnt=GetItemTotalCount(nID);
        if(Count > tcnt)
        {
            NoticeMessage.Instance.Show("블록이 부족합니다!");
            return false;
        }

        DelItem(nID, Count);

        return true;
    }
    public void DelItem(int nID,int Count=1)
    {

        for(int i=0;i<Count;i++)
        {
            SLOTITEM slot = m_nInvenDB.Find(x => x.NItem == nID);
            if (slot != null)
            {
                slot.NCount--;
                if (slot.NCount <= 0)
                {
                    slot.NCount = 0;
                    slot.NItem = 0;
                }
            }

        }

        UpdateInven();
    }
    public void ChangeSlot(int nSlot ,int nItem,int nCount=1)
    {
        m_nInvenDB[nSlot].NItem = nItem;
        m_nInvenDB[nSlot].NCount = nCount;
        UpdateInven();
    }
    public bool AddItemOnce(int nID)
    {

        SLOTITEM slot = FindAddSlot(nID, 1);
        if (slot != null)
        {
            slot.m_nNewItem = 1;
                slot.NItem = nID;
            slot.NCount++;
            return true;
        }

        return false;


    }
    public bool AddItem_ByShop(int nID, int nCount, int nNewItem = 0)
    {
        if (nID == 0) return false;
        bool bflag = false;
        for (int i = 0; i < nCount; i++)
        {
            SLOTITEM slot = FindAddSlot(nID, 1);
            if (slot != null)
            {
                slot.m_nNewItem = nNewItem;// 새로 얻은아이템 
                bflag = true;
                slot.NItem = nID;
                slot.NShop = 1;
                slot.NCount++;

            }
            else
            {
                break;
            }
        }

        UpdateInven();

        return bflag;
    }
    bool _AddItem(int nID, int nCount, int nNewItem = 0)
    {
        if (nID == 0) return false;
        bool bflag = false;
        for (int i = 0; i < nCount; i++)
        {
            SLOTITEM slot = FindAddSlot(nID, 1);
            if (slot != null)
            {
                slot.m_nNewItem = nNewItem;// 새로 얻은아이템 
                bflag = true;
                slot.NItem = nID;
                slot.NShop = 0;
                slot.NCount++;

            }
            else
            {
                break;
            }
        }

        


        return bflag;
    }

    public bool AddItem(int nID,int nCount,int nNewItem = 0)
    {
        bool bflag = _AddItem(nID,nCount,nNewItem);

        CWHeroManager.Instance.UpdateShipBlock(nID);
        UpdateInven();
        return bflag;
    }
    public void AddJObject(JArray jData)
    {
        // 무기와, 부스터는 인벤에 있을 수 없다
        m_nInvenDB.Clear();
        int tcnt = jData.Count;
        for (int i = 0; i < tcnt; i++)
        {
            SLOTITEM slot = new SLOTITEM();
            slot.m_nSlot = i;
            int n = CWJSon.GetInt(jData[i], "Item");
            int c = CWJSon.GetInt(jData[i], "Count");
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(n);
           // if (gData.type == "weapon") continue;
            if (gData.type == "Buster") continue;
            

            


            slot.SetItem(i,n,c);
            m_nInvenDB.Add(slot);

        }

        int tt = m_nInvenDB.Count;//1
        for (int i = tt; i <tt; i++)
        {
            SLOTITEM slot = new SLOTITEM();
            slot.m_nSlot = i;
            slot.SetItem(i, 0, 0);
            m_nInvenDB.Add(slot);
        }

        ReArrange();
        UpdateInven();
    }
    public void UpGrade(int nSlot)
    {
        
        

    }

    public void MergeItem(int num)
    {
        if (m_nInvenDB[num].NItem == 0) return;
        
        for (int i = num+1; i < m_nInvenDB.Count; i++)
        {
        
            if(m_nInvenDB[i].NItem== m_nInvenDB[num].NItem)
            {
                int tcnt = m_nInvenDB[i].NCount + m_nInvenDB[num].NCount;
                m_nInvenDB[i].NCount += m_nInvenDB[num].NCount;
                m_nInvenDB[num].NCount = 0;
                m_nInvenDB[num].NItem = 0;

                if (m_nInvenDB[num].NCount==0)
                {
                    break;
                }

            }
        }
    }


    static public int CompareItem(SLOTITEM a, SLOTITEM b)
    {
        //return b.NCount - a.NCount;
        GITEMDATA gData1 = CWArrayManager.Instance.GetItemData(a.NItem);
        GITEMDATA gData2 = CWArrayManager.Instance.GetItemData(b.NItem);
        return gData2.price - gData1.price;

    }
    /* 개념
     * 
     * */
    #region 일괄작업

    int m_nItemMax = 0;// 최대 아이템
    int m_nMaxLevel = 0;
    int GetNextItem(int nItem)
    {
        GITEMDATA gdata = CWArrayManager.Instance.GetItemData(nItem);
        return CWArrayManager.Instance.GetNextLevelBlock(gdata.level);
    }
    

    
    bool CombineBlock()
    {
        bool bRet = false;
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (m_nInvenDB[i].NItem == 0) continue;
            if (m_nInvenDB[i].NCount <= 1) continue;

            GITEMDATA gNdata = CWArrayManager.Instance.GetItemData(m_nInvenDB[i].NItem);
            if (gNdata.type != "shipblock") continue;

            int nOldItem = m_nInvenDB[i].NItem;

            int nItem = GetNextItem(nOldItem);
            if (nItem == 0) continue;// 더이상 없음

            GITEMDATA gdata = CWArrayManager.Instance.GetItemData(nItem);
            if (m_nMaxLevel < gdata.level)
            {
                m_nMaxLevel = gdata.level;
                m_nItemMax = nItem;
            }

            m_nInvenDB[i].NItem = nItem;
            if(m_nInvenDB[i].NCount%2==1)
            {
                AddItemOnce(nOldItem);
            }
            m_nInvenDB[i].NCount = m_nInvenDB[i].NCount / 2;// 

            bRet = true;
        }

        return bRet;

    }
    
    public int AllCombineBlock()
    {
        m_nMaxLevel = 0;
        m_nItemMax = 0;
        while (true)
        {
            if(!CombineBlock())
            {
                break;
            }
            SynthesisDlg.Instance.CheckUpdateData(m_nItemMax);

            ReArrange();
        }
        UpdateInven();
        return m_nItemMax;


    }
    #endregion
    public void ReArrange()
    {
        // 레벨끼리 합치고
        for(int i=0;i< m_nInvenDB.Count;i++)
        {
            if(m_nInvenDB[i].NItem==0)
            {
                m_nInvenDB[i].NCount = 0;
            }
            MergeItem(i);
        }
        // 첫슬롯부터 정렬
        m_nInvenDB.Sort(CompareItem);
    }
 
    // 더 높은 HP가 존재하는가?
    bool IsHighHPBlock(int hp)
    {
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {

            
            if (m_nInvenDB[i].NItem >0)
            {
                if (m_nInvenDB[i].NItem == (int)GITEM.tempglass) continue;
                if (m_nInvenDB[i].NItem == (int)GITEM.glass) continue;

                GITEMDATA g1 = CWArrayManager.Instance.GetItemData(m_nInvenDB[i].NItem);
                if (g1.type != "shipblock") continue;
                if(g1.hp>hp)
                {
                    return true;
                }
            }
            
        }
        return false;

    }
    // 합성할 수 있는 블록이 있는가?
    public bool IsHaveUpgrade()
    {
        foreach (var v in m_nInvenDB)
        {
            if (v.NCount <= 1) continue;
            GITEMDATA g1 = CWArrayManager.Instance.GetItemData(v.NItem);
            if (g1.type == "shipblock")
            {
                return true;
            }
        }

        return false;
    }
    // 바꿀 수 있는 블록이 존재하는가?
    public bool IsChangeBlock()
    {
        // 비교
        Dictionary<int, BlockData>  kData= CWHero.Instance.GetData();
        foreach(var v in kData)
        {
            GITEMDATA g1= CWArrayManager.Instance.GetItemData(v.Value.nBlock);
            if (g1.type != "shipblock") continue;

            if (g1.nID == (int)GITEM.tempglass) continue;
            if (g1.nID == (int)GITEM.glass) continue;

            if (IsHighHPBlock(g1.hp))
            {
                return true;// 높은 블록이 존재한다
            }


        }

        return false;
    }


}
