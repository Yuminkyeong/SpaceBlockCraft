using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class IngameInvenDlg : WindowUI<IngameInvenDlg>
{

    int _nType = 0;
    public int m_nType
    {
        get
        {
            return _nType;
        }
        set
        {
            _nType = value;

            UpDateInven();
        }
    }
    // 현재 선택된 탭에 맞게 인벤에 맞춘다
    SLOTITEM GetConvertSlot(int num)
    {
        int nSlot = m_gScrollList.GetInt(num, "Slot");
        return CWInvenManager.Instance.GetItembySlot(nSlot);
    }

    int GetListCount()
    {
        return CWInvenManager.Instance.m_nInvenDB.Count;
    }
    int GetListColumnCount()
    {
        return 8;
    }
    string GetListColumn(int Col)
    {
        if (Col == 0) return "Slot";
        if (Col == 1) return "ItemID";
        if (Col == 2) return "Count";
        if (Col == 3) return "New";
        if (Col == 4) return "Check";
        if (Col == 5) return "InvenType";
        if (Col == 6) return "Level";
        if (Col == 7) return "blockName";
        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        if (Col == 0)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].m_nSlot.ToString();
        }
        if (CWInvenManager.Instance.m_nInvenDB[Raw].NItem == 0) return "";
        if (CWInvenManager.Instance.m_nInvenDB[Raw].NCount == 0) return "";

        if (Col == 1)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].NItem.ToString();
        }
        if (Col == 2)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].NCount.ToString();
        }
        if (Col == 3)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].m_nNewItem.ToString();
        }
        if (Col == 4)
        {
            if (CWInvenManager.Instance.m_nInvenDB[Raw].m_bCheck) return "true";
            return "false";
        }
        if (Col == 5)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(CWInvenManager.Instance.m_nInvenDB[Raw].NItem);
            if (gData.type == "shipblock")
            {
                return "0";
            }
            if (gData.type == "color")
            {
                return "1";
            }

            return "2";
            

        }

        if (Col == 6)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(CWInvenManager.Instance.m_nInvenDB[Raw].NItem);
            return "Lv." + gData.level.ToString();
        }
        if (Col == 7)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(CWInvenManager.Instance.m_nInvenDB[Raw].NItem);
            return gData.m_szTitle;
        }

        return "";
    }

    void UpDateInven()
    {
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_bStartClick = false;
        m_gScrollList.m_szCondition = "InvenType = " + m_nType.ToString();


        
        CWInvenManager.Instance.ReArrange();
        UpdateData();
        SetSelect(0);
    }
    protected override void _Open()
    {
        UpDateInven();
        base._Open();
        SetSelect(0);

    }
    public override void SetSelect(int num)
    {
        
        base.SetSelect(num);
    }
    public void OnReArrange()
    {
        CWInvenManager.Instance.ReArrange();

        UpdateData();

    }
    public override void OnSelect(int num)
    {


        base.OnSelect(num);
    }
    void DelCheckFile()
    {
        foreach (var v in CWInvenManager.Instance.m_nInvenDB)
        {
            if (v.m_bCheck)
            {
                v.NCount = 0;
                v.NItem = 0;
            }
        }
        UpdateData();
    }
    

    public override void OnButtonClick(int num)
    {

        SLOTITEM ss = GetConvertSlot(num);
        int Item = ss.NItem;
        int Count = ss.NCount;

        CWInvenManager.Instance.DelItem(Item,Count);
        EquipInvenList.Instance.AddItem(Item, Count,false);
        UpdateData(false);
        base.OnButtonClick(num);
    }



    public void OnType1()
    {
        m_nType = 0;
    }
    public void OnType2()
    {
        m_nType = 1;
    }
    public void OnType3()
    {
        m_nType = 2;
    }
    public void OnType4()
    {
        m_nType = 3;
    }



}
