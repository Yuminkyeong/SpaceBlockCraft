using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;

public class EquipInvenList : SubWindow<EquipInvenList>
{

    public GameObject m_gDeleteObject;
    bool bDeletedFlag;
    public bool m_bDeletedFlag
    {
        get
        {
            return bDeletedFlag;
        }
        set
        {
            bDeletedFlag = value;
            m_gDeleteObject.SetActive(value);
            if(bDeletedFlag)
            {
                CWChHero.Instance.SelectBlock(0);
                m_gScrollList.OnDeSelect();

            }

        }
    }

    public List<SLOTITEM> m_nInvenDB = new List<SLOTITEM>();
    int GetListCount()
    {
        return 8;
    }
    int GetListColumnCount()
    {
        return 4;
    }
    string GetListColumn(int Col)
    {
        if (Col == 0) return "Slot";
        if (Col == 1) return "ItemID";
        if (Col == 2) return "Count";
        if (Col == 3) return "Title";
        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        if (Col == 0)
        {
            return m_nInvenDB[Raw].m_nSlot.ToString();
        }
        if (m_nInvenDB[Raw].NItem == 0) return "";
        if (m_nInvenDB[Raw].NCount == 0) return "";

        if (Col == 1)
        {
            return m_nInvenDB[Raw].NItem.ToString();
        }
        if (Col == 2)
        {
            return m_nInvenDB[Raw].NCount.ToString();
        }
        if (Col == 3)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            return gData.m_szTitle;

        }

        return "";
    }

    void UpDateInven()
    {
        //8개 고정
        m_nInvenDB = new List<SLOTITEM>();
        for (int i = 0; i < 8; i++)
        {
            SLOTITEM ss = new SLOTITEM();
            m_nInvenDB.Add(ss);
        }
            
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_bStartClick = false;

        
        UpdateData();
        
    //    SetSelect(0);
    }
    protected override void _Open()
    {
        UpDateInven();
        base._Open();
        m_bDeletedFlag = true;
    }
    public override void Close()
    {
        // 모두 저장

        foreach(var v in m_nInvenDB)
        {
            CWInvenManager.Instance.AddItem(v.NItem,v.NCount);
        }
        if(m_nInvenDB.Count>0)
            NoticeMessage.Instance.Show("채집한 블록은 인벤으로 이동됩니다.");

        m_nInvenDB.Clear();
        base.Close();
    }

    public void OnInven()
    {
        if( IngameInvenDlg.Instance.IsShow())
        {
            IngameInvenDlg.Instance.Close();
        }
        else
        {
            IngameInvenDlg.Instance.Open();
        }
        
    }
    int FindSlot(int nItem)
    {
        for(int i=0;i<m_nInvenDB.Count;i++)
        {
            if(m_nInvenDB[i].NItem==nItem)
            {
                return i;
            }
        }
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (m_nInvenDB[i].NCount== 0)
            {
                return i;
            }
        }
        return -1;
    }
    public void DelItem(int nItem,int count=1)
    {
        int nSlot = FindSlot(nItem);
        if (nSlot == -1)
        {
            return;// 들어갈때가 없다
        }
        if (m_nInvenDB[nSlot].NItem == 0) return;
        m_nInvenDB[nSlot].NCount-= count;
        if(m_nInvenDB[nSlot].NCount==0)
        {
            CWChHero.Instance.SelectBlock(0);
        }


    }

    void _AddItem(int nItem, int count = 1)
    {

        Dailymission.Instance.CheckUpdate(DAYMTYPE.BLOCK, count);

        CWQuestManager.Instance.CheckUpdateData(34, 1);
        CWQuestManager.Instance.CheckUpdateData(35, 1);
        CWQuestManager.Instance.CheckUpdateData(36, 1);
        CWQuestManager.Instance.CheckUpdateData(37, 1);
        CWQuestManager.Instance.CheckUpdateData(38, 1);
        CWQuestManager.Instance.CheckUpdateData(39, 1);
        CWQuestManager.Instance.CheckUpdateData(40, 1);
        CWQuestManager.Instance.CheckUpdateData(41, 1);

        int nSlot = FindSlot(nItem);
        if (nSlot == -1)
        {
            //NoticeMessage.Instance.Show("인벤슬롯이 꽉 찼습니다!!");

            // 인벤토리로 들어감
            CWInvenManager.Instance.AddItem(nItem, count);
            return;// 들어갈때가 없다
        }

        m_nInvenDB[nSlot].NItem = nItem;
        m_nInvenDB[nSlot].NCount += count;

    }
    public void AddItem(int nItem,int count=1,bool bselect=true)
    {
        _AddItem(nItem, count);
        UpdateData(bselect);
    }
    
    public void AddItemByBlock(int nBlock)
    {
        m_bAddBlock = true; // 블록을 채집으로 얻다
        int nItem = CWArrayManager.Instance.GetItemFromBlock(nBlock);
        if(nItem==0)
        {
            return;
        }
        
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);

        if (gData.type== "color")// 컬러 라면 
        {
            _AddItem((int)GITEM.stone, 1);
        }
        _AddItem(nItem,1);

        UpdateData(false);

    }
    public Vector2 GetNewBlock(int nItem)
    {
        //m_gScrollList
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (m_nInvenDB[i].NItem == nItem)
            {
                SlotItemUI ss = m_gScrollList.GetSlot(i);
                if (ss == null) continue;
                RectTransform rt = ss.GetComponent<RectTransform>();
                if (rt == null) continue;
                return rt.anchoredPosition;
            }
        }

        return Vector2.zero;
    }
    #region 블록을 얻다
    bool m_bAddBlock = false;

    public bool IsAddBlock()
    {
        return m_bAddBlock;
    }
    #endregion

    public int GetItemTotalCount(int nItemID)
    {
        int nCount = 0;
        foreach (var v in m_nInvenDB)
        {
            if (v.NItem == nItemID)
            {
                nCount += v.NCount;
            }
        }
        return nCount;
    }
    public void OnDelBlock()
    {

        m_bDeletedFlag = true;
    }

    public override void OnSelect(int num)
    {
        int nItem = m_gScrollList.GetInt(num, "ItemID");
        int nCount= m_gScrollList.GetInt(num, "Count"); 
        if (IngameInvenDlg.Instance!=null&& IngameInvenDlg.Instance.IsShow())
        {
            CWInvenManager.Instance.AddItem(nItem, nCount);
            DelItem(nItem, nCount);
            UpdateData(false);
            IngameInvenDlg.Instance.UpdateData(false);
        }
        else
        {
            
            if (nItem > 0)
            {
                int nblock = CWArrayManager.Instance.GetBlockFromItem(nItem);
                CWChHero.Instance.SelectBlock(nblock);

            }
            else
            {
                CWChHero.Instance.SelectBlock(0);
            }

        }
        m_bDeletedFlag = false;
        base.OnSelect(num);
    }

}
