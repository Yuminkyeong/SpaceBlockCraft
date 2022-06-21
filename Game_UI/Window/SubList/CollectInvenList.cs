using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;

public class CollectInvenList : SubWindow<CollectInvenList>
{

    const int MAXCOUNT = 20;
    // Start is called before the first frame update
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
        if(m_nInvenDB.Count<=Raw)
        {
            return "";
        }
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
        
        m_nInvenDB = new List<SLOTITEM>();
        for (int i = 0; i < MAXCOUNT; i++)
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
        
    }
    public override void Close()
    {
        //// 모두 저장

        //foreach (var v in m_nInvenDB)
        //{
        //    CWInvenManager.Instance.AddItem(v.NItem, v.NCount);
        //}
        //if (m_nInvenDB.Count > 0)
        //    NoticeMessage.Instance.Show("채집한 블록은 인벤으로 이동됩니다.");


        base.Close();
    }

    
    int FindSlot(int nItem)
    {
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (m_nInvenDB[i].NItem == nItem)
            {
                return i;
            }
        }
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (m_nInvenDB[i].NCount == 0)
            {
                return i;
            }
        }
        return -1;
    }
    void _AddItem(int nItem, int count = 1)
    {


        int nSlot = FindSlot(nItem);
        if (nSlot == -1)
        {

            return;// 들어갈때가 없다
        }

        m_nInvenDB[nSlot].NItem = nItem;
        m_nInvenDB[nSlot].NCount += count;

    }
    public void AddItem(int nItem, int count = 1, bool bselect = true)
    {
        _AddItem(nItem, count);
        UpdateData(bselect);
    }
    public void AddItemByBlock(int nBlock)
    {
        int nItem = CWArrayManager.Instance.GetItemFromBlock(nBlock);
        if (nItem == 0)
        {
            return;
        }

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);

        if (gData.type == "color")// 컬러 라면 
        {
            _AddItem((int)GITEM.stone, 1);
        }
        _AddItem(nItem, 1);

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

    public void ClearInven()
    {
        NoticeMessage.Instance.Show("채집한 블록을 모두 잃어버렸습니다");
        m_nInvenDB.Clear();
    }
    // 인벤으로 보냄
    public void PutInven()
    {
        foreach (var v in m_nInvenDB)
        {
            CWInvenManager.Instance.AddItem(v.NItem, v.NCount);
        }
        if (m_nInvenDB.Count > 0)
            NoticeMessage.Instance.Show("채집한 블록은 인벤으로 이동됩니다.");
        m_nInvenDB.Clear();
    }

}
