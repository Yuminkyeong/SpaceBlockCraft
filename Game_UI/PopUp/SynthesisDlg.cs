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

public class SynthesisDlg : WindowUI<SynthesisDlg>
{

    
    public int m_nItem;
    public int m_nItem2;

    public List<SLOTITEM> m_nInvenDB = new List<SLOTITEM>();

    int GetListCount()
    {
        return m_nInvenDB.Count;
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
            return m_nInvenDB[Raw].m_nSlot.ToString();
        }

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
            return m_nInvenDB[Raw].m_nNewItem.ToString();
        }
        if (Col == 4)
        {
            if (m_nInvenDB[Raw].m_bCheck) return "true";
            return "false";
        }
        if (Col == 5)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            return gData.InvenType.ToString();
        }
        if (Col == 6)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            return "Lv." + gData.level.ToString();
        }
        if (Col == 7)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            return gData.m_szTitle;
        }


        return "";
    }

    void UpDateInven()
    {
        m_nInvenDB.Clear();
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_bStartClick = false;
        // 블록 인벤만 가져 온다
        List<SLOTITEM> ss= CWInvenManager.Instance.GetList("shipblock");
        foreach(var v in ss)
        {
            if (v.NCount <= 1) continue;
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
            if (gData.level >= CWGlobal.MAXBLOCKLEVEL) continue;

            m_nInvenDB.Add(v);
        }
        UpdateData();
        
    }
    //m_nSelectItem
    public void Show(int nSelectItem=0)
    {


        SynthesisCompleteDlg ss = GetComponentInChildren<SynthesisCompleteDlg>(true);
        if(ss)
        {
            ss.gameObject.SetActive(false);
        }

        m_nItem = nSelectItem;
        int snum = 0;
        if(nSelectItem==0)
        {
            if(m_nInvenDB.Count>0)
                m_nItem = m_nInvenDB[0].NItem;
        }
        else
        {
            for (int i = 0; i < m_nInvenDB.Count; i++)
            {
                if (m_nInvenDB[i].NItem == nSelectItem)
                {
                    snum = i;
                    break;
                }
            }
        }
        SetSelect(snum);
        m_gEffect.SetActive(false);
        Open();

        if (m_nInvenDB.Count == 0)
        {
            NoticeMessage.Instance.Show("합성할 블록이 없습니다!");
            Close();
            return;
        }

    }

    protected override void _Open()
    {
        UpDateInven();
        base._Open();
        UpdateInfo();

    }
    public override void OnSelect(int num)
    {
        base.OnSelect(num);
        m_nItem = m_gScrollList.GetInt(num, "ItemID");
        UpdateInfo();
        
    }


    #region 정보창


    public GameObject m_gEffect;
    

    public Text level1;
    public Text level2;
    public Text level3;
    public Text blocknum1;
    public Text blocknum2;
    public RawImage blockImage1;
    public RawImage blockImage2;
    public RawImage blockImage3;

    public Text blockname1;
    public Text blockname2;
    public Text blockname3;


    public Text blocknum3_selectable;

    public Text orginHP;
    public Text changeHP;

    int m_nCursor=0;
    int m_Count
    {
        get
        {
            int nCount = m_gScrollList.GetSelectValueInt("Count");
            return (nCount + m_nCursor)/2;
        }
    }
    void CountInfo()
    {
        int nCount = m_gScrollList.GetSelectValueInt("Count"); 
        if (m_nCursor >= 0) m_nCursor = 0;
        if (nCount + m_nCursor <= 0) m_nCursor = -nCount;
        UpdateInfo();
    }

    public void OnLeft()
    {
        m_nCursor--;
        CountInfo();

    }
    public void OnRight()
    {
        m_nCursor++;
        CountInfo();

    }



    void UpdateInfo()
    {
        

        


        GITEMDATA kSelectItem = CWArrayManager.Instance.GetItemData(m_nItem);

        int nNextItem = CWArrayManager.Instance.GetNextLevelBlock(kSelectItem.level);
        m_nItem2 = nNextItem;

        GITEMDATA kNextItem = CWArrayManager.Instance.GetItemData(nNextItem);

        level1.text = string.Format("Lv.{0}", kSelectItem.level);
        level2.text = string.Format("Lv.{0}", kSelectItem.level);
        level3.text = string.Format("Lv.{0}", kSelectItem.level + 1);

        blockname1.text = kSelectItem.m_szTitle;
        blockname2.text = kSelectItem.m_szTitle;

        blocknum1.text = m_Count.ToString();
        blocknum2.text = m_Count.ToString();

        blockImage1.texture = CWResourceManager.Instance.GetItemIcon(kSelectItem.nID);
        blockImage2.texture = CWResourceManager.Instance.GetItemIcon(kSelectItem.nID);


        blockImage3.texture = CWResourceManager.Instance.GetItemIcon(nNextItem);
        blockname3.text = kNextItem.m_szTitle;

        blocknum3_selectable.text = m_Count.ToString();

        orginHP.text = string.Format("HP {0}", kSelectItem.hp);
        changeHP.text = string.Format("HP {0}", kNextItem.hp);
    


    }


    #endregion

    // 결과
    public void OnResult()
    {


        CWInvenManager.Instance.DelItem(m_nItem, m_Count * 2);
        CWInvenManager.Instance.AddItem(m_nItem2, m_Count);

        CWQuestManager.Instance.CheckUpdateData(23, 1);//블록합성하기

        CheckUpdateData(m_nItem2);



        UpDateInven();
        m_gEffect.SetActive(false);
        ActiveUI aa =GetComponentInChildren<ActiveUI>();
        if (aa != null) aa.gameObject.SetActive(false);
        SetSelect(0);

        if(m_gScrollList.GetCount()==0)
        {
            // 더이상 없다!
            NoticeMessage.Instance.Show("합성할 블록이 없습니다!");
            Close();

        }


    }

   
    public void OnEnchntResult()
    {
        CWResourceManager.Instance.PlaySound("Synthesis");
        m_gEffect.SetActive(true);
    }
    public void CheckUpdateData(int nItem)
    {
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if (gData.level == 10)
        {
            CWQuestManager.Instance.CheckUpdateData(24, 1);//블록레벨 10 달성
        }
        if (gData.level == 12)
        {
            CWQuestManager.Instance.CheckUpdateData(28, 1);//블록레벨 12 달성
        }

        if (gData.level == 15)
        {
            CWQuestManager.Instance.CheckUpdateData(25, 1);//블록레벨 15 달성
        }
        if (gData.level == 16)
        {
            CWQuestManager.Instance.CheckUpdateData(42, 1);//블록레벨 16 달성
        }
        if (gData.level == 17)
        {
            CWQuestManager.Instance.CheckUpdateData(43, 1);//블록레벨 17 달성
        }
        if (gData.level == 18)
        {
            CWQuestManager.Instance.CheckUpdateData(26, 1);//블록레벨 18 달성
        }

        if (gData.level == 19)
        {
            CWQuestManager.Instance.CheckUpdateData(44, 1);//블록레벨 19 달성
        }
        if (gData.level == 20)
        {
            CWQuestManager.Instance.CheckUpdateData(45, 1);//블록레벨 20 달성
        }

        if (gData.level == 21)
        {
            CWQuestManager.Instance.CheckUpdateData(27, 1);//블록레벨 21 달성
        }

    }
    public void OnAllCombineBlock()
    {
        int nItem=CWInvenManager.Instance.AllCombineBlock();
        UpDateInven();
        UpdateInfo();
        CWQuestManager.Instance.CheckUpdateData(23, 1);//블록합성하기
        BlockInfoDlg.Instance.Show(nItem);

        Close();

    }
}
