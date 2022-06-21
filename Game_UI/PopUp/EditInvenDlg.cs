using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;


public class EditInvenDlg : MessageWindow<EditInvenDlg>
{

    List<List<SLOTITEM>> m_kUndo = new List<List<SLOTITEM>>();
    
    List<SLOTITEM> m_nInvenDB = new List<SLOTITEM>();
    List<SLOTITEM> m_nBackupInvenDB = new List<SLOTITEM>();

    

    public void AddUndo()
    {

        List<SLOTITEM> kTemp = new List<SLOTITEM>();
        foreach(var v in m_nInvenDB)
        {
            SLOTITEM kk = new SLOTITEM();
            kk.m_nSlot = v.m_nSlot;
            kk.NCount = v.NCount;
            kk.NItem = v.NItem;
            kTemp.Add(kk);
        }
        m_kUndo.Add(kTemp);
    }
    public void Undo()
    {

        int tcnt = m_kUndo.Count - 1;
        if (tcnt< 0) return;
        List<SLOTITEM> nData = m_kUndo[tcnt];
        m_nInvenDB.Clear();

        foreach (var v in nData)
        {
            SLOTITEM kk = new SLOTITEM();
            kk.m_nSlot = v.m_nSlot;
            kk.NCount = v.NCount;
            kk.NItem = v.NItem;
            m_nInvenDB.Add(kk);
        }


        m_kUndo.Remove(nData);
        UpdateData();
    }

    int GetListCount()
    {
        return m_nInvenDB.Count;
    }
    int GetListColumnCount()
    {
        return 5;
    }
    // 
    string GetListColumn(int Col)
    {
        if (Col == 0) return "ItemType";
        if (Col == 1) return "ItemID";//"ItmeID";
        if (Col == 2) return "Count";
        if (Col == 3) return "title";
        if (Col == 4) return "level";
        
        return "";
    }
    string GetListValue(int Raw, int Col)
    {

        if(m_nInvenDB.Count<=Raw)
        {
            return "---";
        }

        if (Col == 0)
        {
            GITEMDATA gItem = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            if (gItem.type == "shipblock") return "1";
            if (gItem.type == "color") return "2";
            if (gItem.type == "Buster") return "3";
            if (gItem.type == "weapon") return "3";
            if (gItem.type == "charblock") return "1";

            return "0";
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

            GITEMDATA gItem = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);

            return gItem.m_szTitle;
        }

        if (Col == 4)
        {

            int level = CWArrayManager.Instance.GetItemLevel(m_nInvenDB[Raw].NItem);

            return "Lv." + level.ToString();
        }



        return "";
    }
    public void OnColor()
    {
        CWLib.FindChild(this.gameObject, "Eraser").SetActive(true);
        m_gScrollList.m_szCondition = "ItemType=2";
        UpdateData();    

    }
    public void OnBlock()
    {
        CWLib.FindChild(this.gameObject, "Eraser").SetActive(false);
        m_gScrollList.m_szCondition = "ItemType=1";
        UpdateData();

    }
    public void OnItem()
    {
        CWLib.FindChild(this.gameObject, "Eraser").SetActive(false);
        m_gScrollList.m_szCondition = "ItemType=3";
        UpdateData();

    }

    protected override void _Open()
    {

        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_szCondition = "ItemType=1";
        TakeInven();
        m_kUndo.Clear();
       // GameEdit.Instance.NSelectID = m_gScrollList.GetInt(0, "ItemID");
        base._Open();
        CopyBackup();
        CWLib.FindChild(this.gameObject, "Eraser").SetActive(false);
    }
    // 다음 블록
    public int NextBlock(int nPreItem)
    {

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nPreItem);
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            GITEMDATA gData2 = CWArrayManager.Instance.GetItemData(m_nInvenDB[i].NItem);
            if(gData.type== gData2.type)
            {
                if (m_nInvenDB[i].NItem > 0)
                {
                    if (m_nInvenDB[i].NCount > 0)
                    {
                        return m_nInvenDB[i].NItem;
                    }
                }
            }    
        }
        if (gData.type == "color") return 0;

        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            GITEMDATA gData2 = CWArrayManager.Instance.GetItemData(m_nInvenDB[i].NItem);              if (gData2.type=="shipblock")
            {
                if (m_nInvenDB[i].NItem > 0)
                {
                    if (m_nInvenDB[i].NCount > 0)
                    {
                        return m_nInvenDB[i].NItem;
                    }
                }
            }
        }


        return 0;

    }
    
   
    public override void OnSelect(int num)
    {
        GameObject gg = CWLib.FindChild(this.gameObject, "Maskback");
        gg.SetActive(true);

        GameEdit.Instance.m_bErase = false;

        int nSelect = GameEdit.Instance.NSelectID;
        GameEdit.Instance.NSelectID = m_gScrollList.GetInt(num, "ItemID");
        


        if (nSelect!= GameEdit.Instance.NSelectID)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(GameEdit.Instance.NSelectID);
            if (gData.type == "shipblock")
            {
                //NoticeMessage.Instance.Show("원하는 곳에 탭하면 블록이 추가 됩니다.");
                TipMessageDlg.Instance.Show("원하는 곳에 탭하면 블록이 추가 됩니다.");
            }
            if (gData.type == "color")
            {
                TipMessageDlg.Instance.Show("원하는 블록을 탭하면 색을 칠합니다");
            }

        }

        base.OnSelect(num);
    }

    public void ReInven()
    {
        m_nInvenDB.Clear();
        foreach (var v in m_nBackupInvenDB)
        {
            SLOTITEM ss = new SLOTITEM();
            ss.NItem = v.NItem;
            ss.NCount = v.NCount;
            ss.m_nSlot = v.m_nSlot;
            m_nInvenDB.Add(ss);
        }


        MoveToInven();

    }
    void CopyBackup()
    {
        m_nBackupInvenDB.Clear();
        foreach (var v in m_nInvenDB)
        {
            SLOTITEM ss =new SLOTITEM();
            ss.NItem = v.NItem;
            ss.NCount = v.NCount;
            ss.m_nSlot = v.m_nSlot;
            m_nBackupInvenDB.Add(ss);
        }
        
    }
    // 인벤으로 이동 
    public void MoveToInven(Action fuc=null)
    {
        if (m_nInvenDB.Count == 0) return;

        CWInvenManager.Instance.Clear();

        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (m_nInvenDB[i].NCount == 0) continue;

            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[i].NItem);
            //if (gData.type == "weapon") continue;
            if (gData.type == "Buster") continue;


            SLOTITEM kTemp = new SLOTITEM();
            kTemp.m_nSlot = m_nInvenDB[i].m_nSlot;
            kTemp.NCount = m_nInvenDB[i].NCount;
            kTemp.NItem = m_nInvenDB[i].NItem;



            CWInvenManager.Instance.AddItem(kTemp.NItem ,kTemp.NCount);
        }

        // 전부 보낸다!!
        CWSocketManager.Instance.UpdateInvenAll((jData)=> {
            if(fuc!=null) fuc();
        });

    }
    // 인벤에서 가져온다 
    public void TakeInven()
    {
        m_nInvenDB.Clear();
       
    

        for (int i = 0; i < CWInvenManager.Instance.m_nInvenDB.Count; i++)
        {

            // 업데이트 없음을 알린다!!!!!!!!!!!!!!!!!
            // 이게 없으면 절대 안된다!!!!!!!! 
            CWInvenManager.Instance.m_nInvenDB[i].m_bUpdated = false;

            int Count = CWInvenManager.Instance.m_nInvenDB[i].NCount;
            int  Item = CWInvenManager.Instance.m_nInvenDB[i].NItem;
    
            _AddItem(Item, Count);
        }
        ReArrange();


    }
    public SLOTITEM FindAddSlot(int nID, int nCount)
    {

        SLOTITEM slot = m_nInvenDB.Find(x => x.NItem == nID );
        if (slot == null)
        {
            int tt = m_nInvenDB.Count;
            slot = new SLOTITEM();
            slot.m_nSlot = tt;
            slot.SetItem(tt, 0, 0);
            m_nInvenDB.Add(slot);
            return slot;
        }
        return slot;
    }
    public void _AddItem(int nID, int nCount = 1)
    {

        for (int i = 0; i < nCount; i++)
        {
            SLOTITEM slot = FindAddSlot(nID, 1);
            if (slot != null)
            {
                slot.NItem = nID;
                slot.NCount++;
            }
            else
            {
                break;
            }
        }


    }
    public void AddItem(int nID,int nCount=1)
    {
        _AddItem(nID, nCount);
        UpdateData();

    }

    public bool DelItem(int nID)
    {
        bool bRet = false;
        SLOTITEM slot = m_nInvenDB.Find(x => x.NItem == nID);
        if (slot != null)
        {
            slot.NCount--;
            if (slot.NCount <= 0)
            {
                slot.NCount = 0;
                slot.NItem = 0;
                slot.m_bUpdated = true;
                bRet = false;
            }
            else
            {
                bRet = true;
            }
        }
        else
        {
            bRet = false;
        }
        UpdateData();

        return bRet;
    }


    public void ReArrange()
    {
        // 레벨끼리 합치고
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            MergeItem(i);
        }
        // 첫슬롯부터 정렬
        m_nInvenDB.Sort(CompareItem);
    }
    static public int CompareItem(SLOTITEM a, SLOTITEM b)
    {

        GITEMDATA gData1 = CWArrayManager.Instance.GetItemData(a.NItem);
        GITEMDATA gData2 = CWArrayManager.Instance.GetItemData(b.NItem);
        return gData2.price - gData1.price;

    }

    public void MergeItem(int num)
    {

        
        for (int i = 0; i < m_nInvenDB.Count; i++)
        {
            if (i == num) continue;
            if (m_nInvenDB[i].NItem == m_nInvenDB[num].NItem)
            {
                int tcnt = m_nInvenDB[i].NCount + m_nInvenDB[num].NCount;
                m_nInvenDB[i].NCount += m_nInvenDB[num].NCount;
                m_nInvenDB[num].NCount = 0;
                m_nInvenDB[num].NItem = 0;
            }
        }
    }
    // 색 지우기
    public void OnEraser()
    {
        GameObject gg= CWLib.FindChild(this.gameObject, "Maskback");
        gg.SetActive(false);

        GameEdit.Instance.m_bErase = true;
        m_gScrollList.OnDeSelect();

        // 키우기
        // 다른 셀렉트 없애기


    }
}
