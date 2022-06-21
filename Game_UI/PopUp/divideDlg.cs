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

public class divideDlg : WindowUI<divideDlg>
{

    //m_nInvenDB.Count -> 인벤 안의 숫자만 가져온것.
    //viewport>Content>아래 활성화된 SlotUI개수로 판단할 것. 

    public int m_nItem;
    public int m_nItem2;

    public GameObject divideCompletedlg;

    public List<SLOTITEM> m_nInvenDB = new List<SLOTITEM>();

    public GameObject divideEffect;

    int GetListCount()
    {
        
        return m_nInvenDB.Count;
    }

    int GetListColumnCount()
    {
        return 6;
    }
    string GetListColumn(int Col)
    {
        if (Col == 0) return "Slot";
        if (Col == 1) return "ItemID";
        if (Col == 2) return "Count";
        if (Col == 3) return "Use";
        if (Col == 4) return "Level";
        if (Col == 5) return "blockName";

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
        if (Col == 3)//Use
        {
            GITEMDATA kSelectItem = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            if(kSelectItem.level>1)
            {
                return "1";
            }
            return "0";
        }
        if (Col == 4)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            return "Lv." + gData.level.ToString();
        }
        if (Col == 5)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nInvenDB[Raw].NItem);
            return gData.m_szTitle;
        }


        return "";
    }

    public void setCursor()
    {
        SetSelect(0);
    }
    void UpDateInven()
    {
        m_nInvenDB.Clear();
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;

        m_gScrollList.m_szCondition = "Use = 1";// 분해 가능

        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_bStartClick = false;
        // 블록 인벤만 가져 온다
        List<SLOTITEM> ss = CWInvenManager.Instance.GetList("shipblock");
        foreach (var v in ss)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
            if (gData.level == 1) continue;
            if (gData.level >= CWGlobal.MAXBLOCKLEVEL) continue;
            Debug.Log(CWArrayManager.Instance.GetItemData(v.NItem));
            Debug.Log(CWArrayManager.Instance.GetItemData(v.NCount));
            m_nInvenDB.Add(v);
        }
        UpdateData();

    }
    public override void Close()
    {
        base.Close();

        
    }
    //m_nSelectItem
    public void Show(int nSelectItem = 0)
    {

        m_nItem = nSelectItem;
        divideCompletedlg.SetActive(false);

        int snum = 0;
        if (nSelectItem == 0)
        {
            if (m_nInvenDB.Count > 0)
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
        Open();

        //GITEMDATA kSelectItem = CWArrayManager.Instance.GetItemData(m_nItem);
        //if (kSelectItem.level == 1)
        //{
        //    NoticeMessage.Instance.Show("분해할 할 수가 없습니다");
        //    return;
        //}


        if (m_nInvenDB.Count == 0)
        {
            NoticeMessage.Instance.Show("분해할 블록이 없습니다!");
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


    


    public Text level1;
    public Text level3;
    public Text blocknum1;
    public Text blocknum3;

    public RawImage blockImage1;
    public RawImage blockImage3;

    public Text blockname1;
    public Text blockname3;

    

    public Text orginHP;
    public Text changeHP;

    int m_nCursor = 0;
    int m_Count
    {
        get
        {
            int nCount = m_gScrollList.GetSelectValueInt("Count");
            return (nCount + m_nCursor);
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

        int nNextItem = CWArrayManager.Instance.GetLevelBlock(kSelectItem.level-1);
        if(kSelectItem.level<=1)
        {
            /// 불가 
        }
        m_nItem2 = nNextItem;

        GITEMDATA kNextItem = CWArrayManager.Instance.GetItemData(nNextItem);

        level1.text = string.Format("Lv.{0}", kSelectItem.level);
        level3.text = string.Format("Lv.{0}", kSelectItem.level - 1);

        blockname1.text = kSelectItem.m_szTitle;

        blocknum1.text = m_Count.ToString();
        blocknum3.text = (m_Count*2).ToString();
        blockImage1.texture = CWResourceManager.Instance.GetItemIcon(kSelectItem.nID);


        blockImage3.texture = CWResourceManager.Instance.GetItemIcon(nNextItem);
        blockname3.text = kNextItem.m_szTitle;

        

        orginHP.text = string.Format("HP {0}", kSelectItem.hp);
        changeHP.text = string.Format("HP {0}", kNextItem.hp);



    }


    #endregion

    // 결과
    public void OnResult()
    {

        CWInvenManager.Instance.DelItem(m_nItem, m_Count);
        CWInvenManager.Instance.AddItem(m_nItem2, m_Count * 2);
        UpDateInven();


        ActiveUI aa = GetComponentInChildren<ActiveUI>();
        if (aa != null) aa.gameObject.SetActive(false);

        /*
     if (m_nInvenDB.Count == 1)
        {
            CWInvenManager.Instance.DelItem(m_nItem, m_Count);
            CWInvenManager.Instance.AddItem(m_nItem2, m_Count * 2);
            UpDateInven();

            NoticeMessage.Instance.Show("분해할 블록이 없습니다!");
            Close();
        }*/

      //  Debug.Log();
      
    }



    public void OnDivide()
    {
        GITEMDATA kSelectItem = CWArrayManager.Instance.GetItemData(m_nItem);
        if (kSelectItem.level <= 1)
        {
            
            return;
        }

        CWResourceManager.Instance.PlaySound("divide");
        divideEffect.SetActive(true);
       
        OnResult();
    }



}
